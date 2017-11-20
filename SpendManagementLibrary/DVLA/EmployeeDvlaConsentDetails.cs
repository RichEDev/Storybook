namespace SpendManagementLibrary.DVLA
{
    /// <summary>
    /// The employee dvla consent details.
    /// </summary>
    public class EmployeeDvlaConsentDetails
    {
        /// <summary>
        /// Gets or sets the Driverid from DVLA consent for driving licence.
        /// </summary>
        public long DriverId { get; set; }

        /// <summary>
        /// Gets or sets the Security code from DVLA consent, it will be used in navigating user to DVLA portal.
        /// </summary>
        public string SecurityCode { get; set; }

        /// <summary>
        /// Gets or sets the Response message from DVLA portal. 
        /// </summary> 
        public string ResponseMessage { get; set; }

        /// <summary>
        /// Gets or sets the Response Code from DVLA portal. 
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the Url to navigate DVLA portal
        /// </summary>
        public string LicencePortalUrl { get; set; }
        
        /// <summary>
        /// Gets or sets the if the employees lookup date has updated previously
        /// </summary>
        public bool EmployeeLookUpDateHasUpdated { get; set; }
    }
}
