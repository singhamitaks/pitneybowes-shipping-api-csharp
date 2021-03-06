﻿/*
Copyright 2019 Pitney Bowes Inc.

Licensed under the MIT License(the "License"); you may not use this file except in compliance with the License.  
You may obtain a copy of the License in the README file or at
   https://opensource.org/licenses/MIT 
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed 
on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the License 
for the specific language governing permissions and limitations under the License.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/

using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace PitneyBowes.Developer.ShippingApi.Mock
{
    /// <summary>
    /// Mock class for testing. Instead of calling the API the Mock looks for a file in the file system and returns its contents.
    /// </summary>
    public class ShippingAPIMock : IHttpRequest
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dirname">Root directory for the mock files</param>
        public ShippingAPIMock( string dirname = null )
        {
            Dirname = dirname;
        }

        /// <summary>
        /// Root directory for the mock files
        /// </summary>
        public string Dirname { get; set; }
#pragma warning disable CS1998 // No async operations in mock
        /// <summary>
        /// Implements the same method as the real service interface allowing the mock to be plugged in
        /// </summary>
        /// <typeparam name="Response"></typeparam>
        /// <typeparam name="Request"></typeparam>
        /// <param name="resource"></param>
        /// <param name="verb"></param>
        /// <param name="request"></param>
        /// <param name="deleteBody"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<ShippingApiResponse<Response>> HttpRequest<Response, Request>(string resource, HttpVerb verb, Request request, bool deleteBody, ISession session = null) where Request : IShippingApiRequest
        {
            string fullPath = request.RecordingFullPath(resource, session);

            if ( File.Exists(fullPath))
            {
                var apiResponse = new ShippingApiResponse<Response> { HttpStatus = HttpStatusCode.OK, Success = true };
                long jsonPosition = 0;
                using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                using (var mimeStream = new MimeStream(fileStream))
                {
                    mimeStream.SeekNextPart(); //request
                    mimeStream.SeekNextPart(); //response
                    mimeStream.ClearHeaders();
                    mimeStream.ReadHeaders(); // reads http headers as well
                    if (!mimeStream.FirstLine.StartsWith("HTTP", StringComparison.InvariantCulture))
                    {
                        apiResponse = new ShippingApiResponse<Response> { HttpStatus = HttpStatusCode.InternalServerError, Success = false };
                        session.LogDebug(string.Format("Mock request failed {0}", fullPath));
                        apiResponse.Errors.Add(new ErrorDetail() { ErrorCode = "Mock 500", Message = "Bad format " + fullPath });
                        return apiResponse;
                    }
                    var hrc = mimeStream.FirstLine.Split(' ');
                    apiResponse.HttpStatus = (HttpStatusCode)int.Parse(hrc[1]);
                    apiResponse.Success = apiResponse.HttpStatus == HttpStatusCode.OK;

                    foreach (var h in mimeStream.Headers)
                    {
                        apiResponse.ProcessResponseAttribute(h.Key, h.Value);
                    }

                    using (var recordingStream = new RecordingStream(mimeStream, request.RecordingFullPath(resource, session), FileMode.Create, RecordingStream.RecordType.PlainText))
                    {
                        try
                        {
                            //dont open the record file
                            recordingStream.IsRecording = false;
                            ShippingApiResponse<Response>.Deserialize(session, recordingStream, apiResponse, jsonPosition);
                        }
                        catch (Exception ex)
                        {
                            session.LogError(string.Format("Mock request {0} got deserialization exception {1}", fullPath, ex.Message));
                            throw ex;
                        }
                    }
                }
                return apiResponse;
            }

            else
            {
                var apiResponse = new ShippingApiResponse<Response> { HttpStatus = HttpStatusCode.NotFound, Success = false };
                session.LogDebug(string.Format("Mock request failed {0}",fullPath));
                apiResponse.Errors.Add(new ErrorDetail() { ErrorCode = "Mock 401", Message = "Could not find response file" + fullPath });
                return apiResponse;

            }
        }
#pragma warning restore CS1998
    }
}

