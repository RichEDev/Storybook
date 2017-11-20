using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary.Employees.DutyOfCare
{
    /// <summary>
    /// Details of the duty of care documents.
    /// </summary>
    public class DutyOfCareEmailDetails
    {
        /// <summary>
        /// Documents of the customer
        /// </summary>
        public string DocumentName { get; set; }
        /// <summary>
        /// Document expiry date
        /// </summary>
        public DateTime ExpiryDate { get; set; }
        /// <summary>
        /// Number of days left for the expiry of the document
        /// </summary>
        public int NumberOfDays { get; set; }
        /// <summary>
        /// Number of days left for the expiry of the document
        /// </summary>
        public int DocumentCount { get; set; }

        /// <summary>
        /// Fetch Duty of care data from datarow
        /// </summary>
        public static DutyOfCareEmailDetails LoadFromDataRow(System.Data.DataRow claimantDocumentOfCareData)
        {
            var customerEmailDetails = new DutyOfCareEmailDetails();
            customerEmailDetails.ExpiryDate = Convert.ToDateTime(claimantDocumentOfCareData["ExpiryDate"].ToString());
            customerEmailDetails.NumberOfDays = Convert.ToInt32(claimantDocumentOfCareData["NumberOfDays"].ToString());
            return customerEmailDetails;
        }

    }

    /// <summary>
    /// Details of the Claimant for the email scheduler.
    /// </summary>
    public class ClaimantDutyOfCareDetails
    {

        /// <summary>
        /// Claimant ID whos Duty of care document is expired
        /// </summary>
        public int ClaimantId { get; set; }
        /// <summary>
        /// Claimants account Id
        /// </summary>
        public int AccountId { get; set; }

        //<summary>
        //Approver details
        //</summary>
        public string Approver { get; set; }

        //<summary>
        //Team Ids from approver team
        //</summary>
        public int [] TeamIds { get; set; }

        /// <summary>
        /// Duty Of Care document details of claimant
        /// </summary>
        public static ClaimantDutyOfCareDetails InitialiseClaimantIds(int expiredDutyOfCareClaimantId, int claimantsAccountId, string approverID = null, int[] teamID = null)
        {
            var claimantDetails = new ClaimantDutyOfCareDetails();
            claimantDetails.ClaimantId = expiredDutyOfCareClaimantId;
            claimantDetails.AccountId = claimantsAccountId;
            claimantDetails.Approver = approverID;
            claimantDetails.TeamIds = teamID;
            return claimantDetails;
        }
    }

}
