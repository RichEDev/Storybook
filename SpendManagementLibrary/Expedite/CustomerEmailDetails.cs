
namespace SpendManagementLibrary.Expedite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CustomerEmailDetails
    {

        /// <summary>
        /// account ID of the customer
        /// </summary>
        public int AccountId { get; set; }
        /// <summary>
        /// Fund limit of the customer which is set by the expedite
        /// </summary>
        public decimal FundLimit { get; set; }
        /// <summary>
        /// The amount by which available fund is falling low
        /// </summary>
        public decimal MinTopUpRequired { get; set; }
        /// <summary>
        /// Email address of the main administrator
        /// </summary>
        public string AdminEmail { get; set; }
        /// <summary>
        /// First name of the main administrator
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Surname of the main administrator
        /// </summary>
        public string SurName { get; set; }
        /// <summary>
        /// Address of the Email server that that is configured to send emails from. 
        /// </summary>
        public string EmailServerAddress { get; set; }

        public static CustomerEmailDetails LoadFromDataRow(System.Data.DataRow dr)
        {
            var customerEmailDetails = new CustomerEmailDetails();
            customerEmailDetails.AccountId = Convert.ToInt32(dr["accountId"]);
            customerEmailDetails.AdminEmail = dr["Email"].ToString();
            customerEmailDetails.EmailServerAddress = dr["EmailServerAddress"].ToString();
            customerEmailDetails.FirstName = dr["FirstName"].ToString();
            customerEmailDetails.FundLimit = Convert.ToDecimal(dr["FundLimit"]);
            customerEmailDetails.MinTopUpRequired = Convert.ToDecimal(dr["MinTopUpRequired"]);
            customerEmailDetails.SurName = dr["SurName"].ToString();
            return customerEmailDetails;
        }

    }
}