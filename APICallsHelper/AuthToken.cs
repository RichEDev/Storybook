namespace APICallsHelper
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Class to generate auth token for the user details provided in app config.
    /// </summary>
    public class AuthToken
    {
        /// <summary>
        /// Instance or <see cref="EventLogger"/> To help with logging evebts in windows event log. 
        /// </summary>
        private EventLogger Logger { get; set; }

        /// <summary>
        /// Instance of <see cref="RequestHelper"/> to help with making API calls.
        /// </summary>
        private RequestHelper RequestHelper { get; }

        /// <summary>
        /// Instance of <see cref="ResponseHelper"/> to help with operations on response. 
        /// </summary>
        private ResponseHelper ResponseHelper { get; }
        
        /// <summary>
        /// UserName of the expenses user
        /// </summary>
        private string UserName { get; set; }
        
        /// <summary>
        /// Password of the expenses user
        /// </summary>
        private string Password { get; set; }
        
        /// <summary>
        /// registered company Id
        /// </summary>
        private string CompanyId { get; set; }
        
        /// <summary>
        /// Api url which will be called
        /// </summary>
        private const string ApiUrl = "Account/Login";
       
        /// <summary>
        /// Initializes new instance of <see cref="AuthToken"/> by setting necessary object attributes from the values specified in config file of the project that is currently executing.  
        /// </summary>
        public AuthToken()
        {
            this.Logger = new EventLogger();
            this.RequestHelper = new RequestHelper();
            this.ResponseHelper = new ResponseHelper();
            this.CompanyId = ConfigurationManager.AppSettings["apiDefaultCompanyId"];
            this.UserName = ConfigurationManager.AppSettings["apiUsername"];
            this.Password = new CredentialsDecryptor().Decrypt(ConfigurationManager.AppSettings.Get("apiPassword"));
        }
       
        /// <summary>
        /// Gets the authorization token by making API call to login API, which authenticates further API calls to spendMangementAPI.
        /// </summary>
        /// <returns>Authorization token</returns>
        public string GetAuthToken()
        {
            try
            {
                var request = this.RequestHelper.GetHttpWebRequest(ApiUrl); 
                var postCredentials = $"Company={this.CompanyId}&Username={this.UserName}&Password={this.Password}";

                var data = Encoding.ASCII.GetBytes(postCredentials);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                ServicePointManager.Expect100Continue = false;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                this.Logger.MakeEventLogEntry("Get AuthToken", ApiUrl, "Call succesfully made by - " + this.UserName, response.StatusCode);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var loginResponse = this.ResponseHelper.GetResponseObject<LoginResponse>(response, new StreamReader(response.GetResponseStream()));
                    return loginResponse.AuthToken;
                }
            }
            catch (Exception ex)
            {
                this.Logger.MakeEventLogEntry("Error : Get AuthToken", "/Account/Login", ex.Message, HttpStatusCode.InternalServerError);
            }
            return null;
        }
    }
}
