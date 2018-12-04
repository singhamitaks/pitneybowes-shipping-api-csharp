﻿/*
Copyright 2018 Pitney Bowes Inc.

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
using System.Collections.Generic;

namespace PitneyBowes.Developer.ShippingApi
{
    /// <summary>
    /// End of day manifest object. It is used to request and receive a manifest, ASN, pickup document etc. 
    /// </summary>
    public interface IManifest
    {
        /// <summary>
        /// Unique transaction ID, generated by client, up to 25 characters.
        /// </summary>
        /// <value>The transaction identifier.</value>
        string TransactionId { get; set; }
        /// <summary>
        /// Gets or sets the carrier. Valid value(s): USPS
        /// </summary>
        /// <value>The carrier.</value>
        Carrier Carrier { get; set; }
        /// <summary>
        /// Gets or sets the submission date, the date the shipments are tendered to the carrier.Default value is the current date in GMT/UTC.
        /// </summary>
        /// <value>The submission date.</value>
        DateTimeOffset SubmissionDate { get; set; }
        /// <summary>
        /// Gets or sets from address. Required.
        /// </summary>
        /// <value>From address.</value>
        IAddress FromAddress { get; set; }
        /// <summary>
        /// Gets or sets the induction postal code. Postal code where the shipments are tendered to the carrier.
        /// </summary>
        /// <value>The induction postal code.</value>
        string InductionPostalCode { get; set; }
        /// <summary>
        /// Gets or sets the parcel tracking numbers. Required if you choose to list shipment tracking numbers in the request object. 
        /// </summary>
        /// <value>The parcel tracking numbers.</value>
        IEnumerable<string> ParcelTrackingNumbers { get; set; }
        /// <summary>
        /// Adds the parcel tracking number to the end of the ParcelTrackingNumbersList
        /// </summary>
        /// <param name="t">T.</param>
        void AddParcelTrackingNumber(string t);
        /// <summary>
        /// Gets or sets the parameters. Use the Enum ManifestParameter to see options. 
        /// </summary>
        /// <value>The parameters.</value>
        IEnumerable<IParameter> Parameters { get; set; }
        /// <summary>
        /// Adds the parameter to the end of the Parameters list.
        /// </summary>
        /// <returns>The parameter.</returns>
        /// <param name="p">P.</param>
        IParameter AddParameter(IParameter p);
        /// <summary>
        /// Response only. Gets or sets the manifest identifier.
        /// </summary>
        /// <value>The manifest identifier.</value>
        string ManifestId { get; set; }
        /// <summary>
        /// Response only. Gets or sets the manifest tracking number.
        /// </summary>
        /// <value>The manifest tracking number.</value>
        string ManifestTrackingNumber { get; set; }
        /// <summary>
        /// Response only. Gets or sets the documents.
        /// </summary>
        /// <value>The documents.</value>
        IEnumerable<IDocument> Documents { get; set; }
        /// <summary>
        /// Adds the document to the end of the document list. Unless you are doing something exotic, dont call this.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="d">D.</param>
        IDocument AddDocument(IDocument d);
    }

    public static partial class InterfaceValidators
    {
        /// <summary>
        /// If false, the object underlying the interface is not valid. If true, the object may or may not be valid.
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        public static bool IsValid(this IManifest manifest)
        {
            if (manifest.TransactionId.Length > 25) return false;
            if (manifest.Carrier != Carrier.USPS) return false;
            if (manifest.FromAddress == null) return false;
            return true;
        }
    }
}