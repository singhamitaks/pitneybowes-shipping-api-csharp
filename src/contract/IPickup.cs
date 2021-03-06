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
using System.Collections.Generic;

namespace PitneyBowes.Developer.ShippingApi
{
    /// <summary>
    /// USPS package pickup from a residential or commercial location
    /// </summary>
    public interface IPickup
    {
        /// <summary>
        /// Unique transaction identifier.
        /// </summary>
        /// <value>The transaction identifier.</value>
        string TransactionId { get; set; }
        /// <summary>
        /// Gets or sets the pickup address. Required.
        /// </summary>
        /// <value>The pickup address.</value>
        IAddress PickupAddress { get; set; }
        /// <summary>
        /// Gets or sets the carrier.
        /// </summary>
        /// <value>The carrier. Only USPS supported.</value>
        Carrier Carrier { get; set; }
        /// <summary>
        /// The parcel descriptions. Each object in the array describes a group of parcels.
        /// </summary>
        /// <value>The pickup summary.</value>
        IEnumerable<IPickupCount> PickupSummary { get; set; }
        /// <summary>
        /// Add a PickupCount object to the PickupSummary enumerable.
        /// </summary>
        /// <param name="p"></param>
        void AddPickupCount(IPickupCount p);
        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>The reference.</value>
        string Reference { get; set; }
        /// <summary>
        /// Gets or sets the package location.
        /// </summary>
        /// <value>The package location.</value>
        PackageLocation PackageLocation { get; set; }
        /// <summary>
        /// Gets or sets the special instructions. Required if PackageLocation = Other
        /// </summary>
        /// <value>The special instructions.</value>
        string SpecialInstructions { get; set; }
        /// <summary>
        /// Gets or sets the pickup date. Response only.
        /// </summary>
        /// <value>The pickup date.</value>
        DateTime PickupDate { get; set; }
        /// <summary>
        /// Gets or sets the pickup confirmation number. Response only.
        /// </summary>
        /// <value>The pickup confirmation number.</value>
        string PickupConfirmationNumber { get; set; }
        /// <summary>
        /// Gets or sets the pickup identifier. Response only.
        /// </summary>
        /// <value>The pickup identifier.</value>
        string PickupId { get; set; }
    }

    public static partial class InterfaceValidators
    {
        /// <summary>
        /// Validation method for IPickup
        /// </summary>
        /// <param name="p">IPickup object</param>
        /// <returns></returns>
        public static bool IsValid(this IPickup p)
        {
            if (p.PickupAddress == null) return false;
            if (p.PackageLocation == PackageLocation.Other)
            {
                if (p.SpecialInstructions == null || p.SpecialInstructions == string.Empty) return false;
            }
            return true;
        }
    }
}

