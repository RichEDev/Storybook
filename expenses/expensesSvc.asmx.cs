using System;
using System.Collections.Generic;
using System.Web.Services;
using SpendManagementLibrary;
using SpendManagementLibrary.Enumerators;
using Spend_Management.expenses.code.Claims;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for expensesSvc
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class expensesSvc : System.Web.Services.WebService
    {
        private System.Web.Caching.Cache CachedUser = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

        public expensesSvc()
        {
            
        }

        /// <summary>
        /// Request authentification token based on user credentials. This token if not accessed again will expire after 5 minutes
        /// </summary>
        /// <param name="companyid">ID of the company</param>
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password of the user</param>
        /// <returns>User token</returns>
        [WebMethod(EnableSession = true)]
        public string RequestAuthentificationToken(string companyid, string username, string password)
        {
            cAccounts clsAccounts = new cAccounts();
            cAccount reqAccount = clsAccounts.GetAccountByCompanyID (companyid);

            if (reqAccount == null)
            {
                return "";
            }

            if (reqAccount.archived == true)
            {
                return "";
            }

            cEmployees clsEmployees = new cEmployees(reqAccount.accountid);
            AuthenicationOutcome authOutcome = clsEmployees.Authenticate(username, password, AccessRequestType.Mobile);
       
            CurrentUser user = new CurrentUser();

            user.AccountID = reqAccount.accountid;
            user.EmployeeID = authOutcome.employeeId;;
            Guid token = Guid.NewGuid();

            CachedUser.Insert(token.ToString(), user, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.VeryShort), System.Web.Caching.CacheItemPriority.Default, null);

            return token.ToString();
        }

        /// <summary>
        /// Check if a user is authenticated to make the method call, if not a new request for authenntification will need to be made
        /// </summary>
        /// <param name="token">Current encypted user token</param>
        /// <returns>True if user is authenticated or false if not</returns>
        [WebMethod(EnableSession = true)]
        public bool IsAuthenticated(string token)
        {
            if (CachedUser[token] != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Create an example claim and then return the url to edit it
        /// </summary>
        /// <param name="token">Current encrypted user token</param>
        /// <param name="claimTitle">Title for the expense claim</param>
        /// <returns>URL to view the created expense claim</returns>
        [WebMethod(EnableSession = true)]
        public string CreateExpenseClaim(string token, string claimTitle)
        {
            if (IsAuthenticated(token))
            {
                var user = (CurrentUser)CachedUser[token];
                var claimSubmission = new ClaimSubmission(user);
                int claimid = claimSubmission.addClaim(user.EmployeeID, claimTitle, "", new SortedList<int, object>());
                return "https://testing.sel-expenses.com/expenses/claimViewer.aspx?claimid=" + claimid.ToString();
            }

            return "User not authenticated";
        }

        /// <summary>
        /// Get the status of an expense claim
        /// </summary>
        /// <param name="encryptedToken">Current encypted user token</param>
        /// <param name="claimid">Unique ID of the claim</param>
        /// <returns>A string value of the status</returns>
        [WebMethod(EnableSession = true)]
        public string GetExpenseClaimStatus(string token, int claimid)
        {
            if (IsAuthenticated(token))
            {
                CurrentUser user = (CurrentUser)CachedUser[token];
                cClaims clsClaims = new cClaims(user.AccountID);
                cClaim claim = clsClaims.getClaimById(claimid);
                return claim.status.ToString();
            }

            return "User not authenticated";
        }
        /// <summary>
        /// Amend the expense claim description with the passed in note parameter
        /// </summary>
        /// <param name="encryptedToken">Current encypted user token</param>
        /// <param name="claimid">Unique ID of the claim</param>
        /// <param name="note">value of the description to amend</param>
        [WebMethod(EnableSession = true)]
        public void AmendExpenseClaimDesc(string token, int claimid, string note)
        {
            if (IsAuthenticated(token))
            {
                CurrentUser user = (CurrentUser)CachedUser[token];
                cClaims clsClaims = new cClaims(user.AccountID);
                cClaim claim = clsClaims.getClaimById(claimid);
                clsClaims.updateClaim(claimid, claim.name, note, new SortedList<int, object>(), user.EmployeeID);
                
            }
        }
    }
}
