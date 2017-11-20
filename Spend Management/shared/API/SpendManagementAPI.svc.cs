using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SpendManagementLibrary;
using System.IO;
using System.Web.Script.Serialization;
using System.ServiceModel.Web;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using System.Threading;
using System.Web;

namespace Spend_Management
{
    /// <summary>
    /// API service for third party providers
    /// </summary>
    public class SpendManagementAPI : ISpendManagementAPI
    {
       
        public string DoWork()
        {
            return "Hello";
        }

        /// <summary>
        /// Get a list of all the employee IDs and their usernames for a specific company
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetEmployeeInfo(string companyID)
        {
            cAccounts clsAccounts = new cAccounts();
            Dictionary<int, string> lstEmployees = new Dictionary<int, string>();

            cAccount account = clsAccounts.GetAccountByCompanyID(companyID);

            if (account != null)
            {
                cEmployees clsEmployees = new cEmployees(account.accountid);
                lstEmployees = clsEmployees.GetAllEmployeeIDsAndUsernamesList();
            }

            return lstEmployees;
        }

    }

    
}
