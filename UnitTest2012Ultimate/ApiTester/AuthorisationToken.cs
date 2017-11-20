namespace UnitTest2012Ultimate.ApiTester
{
    using System.Web.Script.Serialization;
    using SpendManagementApi.Models.Requests;
    using SpendManagementLibrary;
    using Spend_Management;
    using System.Configuration;
    using APITester;

    /// <summary>
    /// Manages the Authorisation token required on each endpoint call.
    /// </summary>
    static class AuthorisationToken
    {
        private static string AuthToken { set; get; }

        /// <summary>
        /// Returns the authorisation token. If the authorisation token is not set then it is created
        /// from the Login endpoint.
        /// </summary>
        /// <returns>The authorisation token.</returns>
        public static string GetAuthToken()
        {
            if (AuthToken == null)
            {    
                string postData = GetLoginPostData();
                string requestUrl = string.Format("{0}Account/Login", ConfigurationManager.AppSettings["ApiAddress"]);   
           
                var json = RestClient.MakeRequest(requestUrl, HttpVerb.POST, postData); 

                if (!json.jsonData.ContainsKey("error"))
                {
                    AuthToken = json.GetString("AuthToken");
                }
                else
                {
                    AuthToken = null;
                }
            }
            return AuthToken;
        }

        /// <summary>
        /// Generates a Json string of login post data.
        /// </summary>
        /// <returns></returns>
        private static string GetLoginPostData()
        {
            var accountId = int.Parse(ConfigurationManager.AppSettings.Get("AccountID"));
            var employeeId = int.Parse(ConfigurationManager.AppSettings.Get("EmployeeID"));

            var accounts = new cAccounts();
            var account = accounts.GetAccountByID(accountId);

            var employees = new cEmployees(accountId);
            var employee = employees.GetEmployeeById(employeeId);

            GlobalAsax.Application_Start(); var secureData = new cSecureData();

            var login = new LoginRequest
             {
                 Username = employee.Username,
                 Password = secureData.Decrypt(employee.Password),
                 Company = account.companyid
             };

            string postData = new JavaScriptSerializer().Serialize(login);
        
            return postData;
        }
    }
}
