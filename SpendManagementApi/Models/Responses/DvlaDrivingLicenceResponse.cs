namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using SpendManagementApi.Models.Common;
    using SpendManagementLibrary.DVLA;

    /// <summary>
    /// The Driving licnce response for auto lookup Dvla service.
    /// </summary>
    public class DvlaDrivingLicenceResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Driving licence is added successfully.
        /// </summary>
        public bool IsDrivingLicenceAdded { get; set; }

        /// <summary>
        /// Gets or sets the current record of the Driving licence.
        /// </summary>
        public List<CurrentCustomEntityRecord> CurrentRecord { get; set; }
    }
}