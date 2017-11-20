namespace DutyOfCareAPI.DutyOfCare
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Driving licence Response details
    /// </summary>
    public class DrivingLicenceDetailsResponse
    {
        /// <summary>
        /// Driving licence number 
        /// </summary>
        public string DrivingLicenceNumber { get; set; }

        /// <summary>
        /// Driving licence valid from 
        /// </summary>
        public DateTime ValidFrom { get; set; }

        /// <summary>
        /// Driving licence valid till
        /// </summary>
        public DateTime ValidTo { get; set; }

        /// <summary>
        /// Indicator of which directive applies to driving licence 
        /// </summary>
        public string DirectiveIdentifier { get; set; }

        /// <summary>
        /// The licence issue number
        /// </summary>
        public string IssueNumber { get; set; }

        /// <summary>
        /// Status of driving licence
        /// </summary>
        public string DvlaStatus { get; set; }

        /// <summary>
        /// Error status message 
        /// </summary>
        public string ErrorStatus { get; set; }

        /// <summary>
        /// Driving licence owner
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Endorsements are convictions recorded against the licence 
        /// </summary>
        public List<EndorsementDetails> Endorsements { get; set; }

        /// <summary>
        /// The licence entitlement (sometimes known as the licence category) provides details of the specific driving entitlements for the specified licence
        /// </summary>
        public List<EntitlementDetails> Entitlements { get; set; }

        /// <summary>
        /// Response code for each driving licence lookup.The empty or null code implies a successfull lookup
        /// </summary>
        public string ResponseCode { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DrivingLicenceDetailsResponse"/> class.
        /// </summary>
        public DrivingLicenceDetailsResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrivingLicenceDetailsResponse"/> class. 
        /// DrivingLicenceDetailsResponse constructor when error in get driving licence details
        /// </summary>
        /// <param name="employeeId">Driving licence owner</param>
        /// <param name="errorStatus">Error status information</param>
        /// <param name="responseCode">Response code which the api returns</param>
        public DrivingLicenceDetailsResponse(int employeeId, string errorStatus, string responseCode)
        {
            this.EmployeeId = employeeId;
            this.ErrorStatus = errorStatus;
            this.ResponseCode = responseCode;
        }

        /// <summary>
        /// DrivingLicenceDetailsResponse constructor with driving licence details 
        /// </summary>
        /// <param name="drivingLicenceNumber">Driving licence number </param>
        /// <param name="validFrom">Driving licence valid from </param>
        /// <param name="validTo">Driving licence valid till</param>
        /// <param name="directiveIdentifier">Indicator of which directive applies to driving licence </param>
        /// <param name="issueNumber">Licence issue number</param>
        /// <param name="dvlaStatus">Status of driving licence</param>
        /// <param name="errorStatus">Error status message </param>
        /// <param name="endorsements">Endorsements are convictions recorded against the licence </param>
        /// <param name="entitlements">The licence entitlement (sometimes known as the licence category) provides details of the specific driving entitlements for the specified licence</param>
        /// <param name="employeeId">Driving licence owner</param>
        /// <param name="responseCode">Response code which the api returns</param>
        public DrivingLicenceDetailsResponse(string drivingLicenceNumber,DateTime validFrom,DateTime validTo,string directiveIdentifier,string issueNumber,string dvlaStatus,string errorStatus,List<EndorsementDetails> endorsements,List<EntitlementDetails> entitlements,int employeeId, string responseCode)
        {
            this.DrivingLicenceNumber = drivingLicenceNumber;
            this.ValidFrom = validFrom;
            this.ValidTo = validTo;
            this.DirectiveIdentifier = directiveIdentifier;
            this.IssueNumber = issueNumber;
            this.DvlaStatus = dvlaStatus;
            this.ErrorStatus = errorStatus;
            this.EmployeeId = employeeId;
            this.Endorsements = endorsements;
            this.Entitlements = entitlements;
            this.ResponseCode =responseCode;
        }
    }
}
