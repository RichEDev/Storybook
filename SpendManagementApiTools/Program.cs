namespace SpendManagementApiTools
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.IO;
    using System.Security.Cryptography;
    using Newtonsoft.Json;

    internal class Program
    {
        private static void Main()
        {
           var apiCaller = new ApiCaller();
            
            apiCaller.ResetExpediteOperatorValidationProgressStatus();
            apiCaller.TriggerEmailNotifications();
            apiCaller.ResetFreeCalls();
           
        }
    }

    internal class ApiCaller
    {

        public string AuthToken { get; set; }
        private static byte[] Key
        {
            get
            {
                var key = new byte[32];
                key[0] = 201;
                key[1] = 34;
                key[2] = 61;
                key[3] = 177;
                key[4] = 73;
                key[5] = 61;
                key[6] = 42;
                key[7] = 198;
                key[8] = 179;
                key[9] = 115;
                key[10] = 39;
                key[11] = 113;
                key[12] = 42;
                key[13] = 80;
                key[14] = 255;
                key[15] = 104;
                key[16] = 185;
                key[17] = 137;
                key[18] = 89;
                key[19] = 174;
                key[20] = 45;
                key[21] = 65;
                key[22] = 172;
                key[23] = 144;
                key[24] = 206;
                key[25] = 102;
                key[26] = 201;
                key[27] = 71;
                key[28] = 178;
                key[29] = 0;
                key[30] = 11;
                key[31] = 4;

                return key;
            }
        }

        private static byte[] Iv
        {
            get
            {
                var iv = new byte[16];
                iv[0] = 148;
                iv[1] = 93;
                iv[2] = 123;
                iv[3] = 24;
                iv[4] = 109;
                iv[5] = 9;
                iv[6] = 122;
                iv[7] = 147;
                iv[8] = 64;
                iv[9] = 112;
                iv[10] = 218;
                iv[11] = 217;
                iv[12] = 11;
                iv[13] = 116;
                iv[14] = 235;
                iv[15] = 55;
                return iv;
            }
        }
        
        private string ApiPathUriPrefix { get; set; }
        public ApiCaller()
        {
            ApiPathUriPrefix = GetDomain();
            AuthToken = GetAuthToken();
        }

        private const string ApiTools = "ApiTools";
        private static HttpClient _client;

        /// <summary>
        /// Call Api to send emails
        /// </summary>
        public void TriggerEmailNotifications()
        {
            bool enableMinimumFundEmail;
            //set the boolean value
            if (!bool.TryParse(ConfigurationManager.AppSettings["EnableMinimumFundEmail"].ToString(), out enableMinimumFundEmail))
            {
                enableMinimumFundEmail = false;
            }
            //retrun if the email sending feature is not turned on
            if (enableMinimumFundEmail == false)
            {
                MakeEventLogEntry("Email Disabled", string.Empty, "Email sending feature to notify float limit is not enabled");
                return;
            }
            if (!string.IsNullOrEmpty(this.AuthToken))
            {
                this.NotifyAdministratorsOfFloatBelowLimit("Expedite/Funds/NotifyAdministratorsOfFloatBelowLimit");
            }
        }

        /// <summary>
        /// call API to reset the Expedite Operator validation 
        /// </summary>
        public void ResetExpediteOperatorValidationProgressStatus()
        {
           
            if (!string.IsNullOrEmpty(AuthToken))
            {
                var action = "Expedite/ExpenseValidationResults/ResetTheValidationProgress";
                var request = GetHttpWebRequest(action);
                request.Headers.Add("AuthToken", AuthToken);
                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    OperatorValidationStatusResetResponse operatorValidationStatusResetResponse = GetResponseObject<OperatorValidationStatusResetResponse>(response);
                    if (!operatorValidationStatusResetResponse.IsUpdated)
                    {
                        MakeEventLogEntry("Reset expedite operator validation progress status ", action, "Call successfully made : The update failed, Message: " + operatorValidationStatusResetResponse.ErrorMessage, statusCode: response.StatusCode);
                        return;
                    }
                    MakeEventLogEntry("Reset expedite operator validation progress status ", action, "Call successfully made : The update was successful", statusCode: response.StatusCode);
                }
                catch (Exception ex)
                {
                    MakeEventLogEntry("Error : Reset expedite operator validation progress status ", action, ex.Message, true);
                }
            }
        }

        public void ResetFreeCalls()
        {
            ConfigureLogs();
            ConfigureClient();

            MakePutCallAndLogResponse("Account/ResetDailyFreeCallLimits");
            MakePutCallAndLogResponse("Expedite/Envelopes/NotifyClaimantsOfUnsentEnvelopes");
        }

        /// <summary>
        /// Configures the Logs.
        /// </summary>
        private static void ConfigureLogs()
        {
            // set up event log if not exists
            var hostName = Dns.GetHostName();
            if (!EventLog.SourceExists(ApiTools))
            {
                EventLog.CreateEventSource(ApiTools, EventLog.LogNameFromSourceName(ApiTools, hostName));
            }
        }

        /// <summary>
        /// Configres the HttpClient to make API calls.
        /// </summary>
        private static void ConfigureClient()
        {
            // configure URI
            var useHttps = ConfigurationManager.AppSettings["https"] == "1";
            var domain = ConfigurationManager.AppSettings["domain"];
            var url = (useHttps ? "HTTPS" : "HTTP") + "://" + domain;
            if (string.IsNullOrWhiteSpace(domain))
            {
                throw new ConfigurationErrorsException("You must provide a domain in the config.");
            }

            // configure client
            var uri = new Uri(url);
            var handler = new HttpClientHandler();
            _client = new HttpClient(handler) { BaseAddress = uri };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Makes a PUT call and logs the response.
        /// </summary>
        /// <param name="action"></param>
        private static void MakePutCallAndLogResponse(string action)
        {
            var entry = string.Format("Reset Daily Free Calls. URL: {0}/{1}. Call successfully made. Operation completed. Status code : ({2})",
                _client.BaseAddress, action, _client.PutAsync(action, new StringContent("{}")).Result.StatusCode);
            EventLog.WriteEntry(ApiTools, entry);
            
        }

        /// <summary>
        /// calls api of notifying admin of float below limit when we have authorization token. 
        /// </summary>
        /// <param name="action"></param>
        private void NotifyAdministratorsOfFloatBelowLimit(string action)
        {
            var request = GetHttpWebRequest(action);
            request.Headers.Add("AuthToken", AuthToken);
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                EmailSenderResponse emailSenderResponse = GetResponseObject<EmailSenderResponse>(response);
                if (!emailSenderResponse.IsSendingSuccessful)
                {
                    MakeEventLogEntry("Notify admins of float below limit ", action, "Call made successfully : Emails were not sent, Message: " + emailSenderResponse.ErrorMessage, statusCode: response.StatusCode);
                    return;
                }
                MakeEventLogEntry("Notify admins of float below limit ", action, "Call made successfully : Emails were sent successfully", statusCode: response.StatusCode);
            }
            catch (Exception ex)
            {
                MakeEventLogEntry("Error : Notify admins of float below limit ", action, ex.Message, true);
            }
        }

        /// <summary>
        /// calls api of notifying Line manager and Claimant when DOC documents are about to expire. 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="authToken"></param>
        private void NotifyDutyOfCareDocumentsExpiry(string action, string authToken)
        {
            var request = GetHttpWebRequest(action);
            request.Timeout = 300000;
            request.Headers.Add("AuthToken", authToken);
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                DutyOfCareResponse emailSenderResponse = GetResponseObject<DutyOfCareResponse>(response);
                MakeEventLogEntry("Notify Line Manager and Claimant on DOC documents expiry  :", action, "Call made successfully", statusCode: response.StatusCode);
                if (!emailSenderResponse.IsSendingSuccessful)
                {
                    MakeEventLogEntry("Notify Line Manager and Claimant on DOC documents expiry :", action, "Emails were not sent, Message: " + emailSenderResponse.ErrorMessage, statusCode: response.StatusCode);
                    return;
                }
                MakeEventLogEntry("Notify Line Manager and Claimant on DOC documents expiry :", action, "Emails were sent successfully, Status code : ", statusCode: response.StatusCode);
        }
            catch (Exception ex)
            {
                MakeEventLogEntry("Error : Notify Line Manager and Claimant on DOC documents expiry :", action, ex.Message, true);
            }
        }
        #region private helper methods

        /// <summary>
        /// Calls account/login to get the token
        /// </summary>
        /// <returns></returns>
        private string GetAuthToken()
        {
            try
            {
                var companyId = ConfigurationManager.AppSettings["apiDefaultCompanyId"];
                var userName = ConfigurationManager.AppSettings["apiUsername"];
                var password = Decrypt(ConfigurationManager.AppSettings.Get("apiPassword"));

                var request = GetHttpWebRequest("Account/Login");
                var postCredentials = string.Format("Company={0}&Username={1}&Password={2}", companyId, userName, password);

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

                MakeEventLogEntry("Get AuthToken", "Account/Login", "Call succesfully made by - " + userName , statusCode: response.StatusCode);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var loginResponse = GetResponseObject<LoginResponse>(response);
                    return loginResponse.AuthToken;
                }
            }
            catch (Exception ex)
            {
                MakeEventLogEntry("Error : Get AuthToken", "/Account/Login", ex.Message, true);
            }
            return null;
        }
        /// <summary>
        /// Call it to make an event log entry when ever it is necessary
        /// </summary>
        /// <param name="actionName">Current stage of the task</param>
        /// <param name="action">Url </param>
        /// <param name="message">message regarding the task</param>
        /// <param name="iserror">should the event entry made as error</param>
        /// <param name="statusCode">status code received from http response</param>
        private void MakeEventLogEntry(string actionName, string action, string message, bool iserror = false, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            var entry = string.Format("{0}. URL: {1}/{2}. Message : {3} . Status code: ({4})",
              actionName, ApiPathUriPrefix, action, message, statusCode);
            if (iserror)
            {
                EventLog.WriteEntry(ApiTools, entry, EventLogEntryType.Error);
                return;
            }
            EventLog.WriteEntry(ApiTools, entry);
        }
        /// <summary>
        /// convert the response to desirable object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        private T GetResponseObject<T>(HttpWebResponse response)
        {
            var content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return JsonConvert.DeserializeObject<T>(content);
        }
        /// <summary>
        /// concatenates action with the base url and returns a webrequest object
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private HttpWebRequest GetHttpWebRequest(string action)
        {
            return (HttpWebRequest)WebRequest.Create(ApiPathUriPrefix + "/" + action);
        }
        /// <summary>
        /// decrypt the encrypted password given as data
        /// </summary>
        /// <param name="data"></param>
        /// <returns>decrypted password</returns>
        private static string Decrypt(string data)
        {
            string decrypted;

            var decryptor = new RijndaelManaged();
            byte[] inputByteArray = Convert.FromBase64String(data);

            using (var stream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(stream, decryptor.CreateDecryptor(Key, Iv), CryptoStreamMode.Write))
            {
                cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
                cryptoStream.FlushFinalBlock();

                decrypted = Encoding.UTF8.GetString(stream.ToArray());
            }

            return decrypted;
        }
        /// <summary>
        /// attaches http to the domain specifed in webconfig
        /// </summary>
        /// <returns></returns>
        private string GetDomain()
        {
            var useHttps = ConfigurationManager.AppSettings["https"] == "1";

            return (useHttps ? "https://" : "http://") + ConfigurationManager.AppSettings["domain"];
        }

        #endregion
    }
    /// <summary>
    /// Response object of login api call
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the authentication token.
        /// </summary>
        public string AuthToken { get; set; }
        /// <summary>
        /// Gets or sets the attempts remaining before the employee has their account locked.
        /// </summary>
        public int AttemptsRemaining { get; set; }

        /// <summary>
        /// Gets or sets the message returned in a HTTP response exception in the case of a failed login.
        /// </summary>
        public string Message { get; set; }

    }

    /// <summary>
    /// A response containing information regarding sending of email.
    /// </summary>
    public class EmailSenderResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether sending email was successful or not
        /// </summary>
        public bool IsSendingSuccessful { get; set; }

        /// <summary>
        /// Gets or sets error message if any.
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    public class OperatorValidationStatusResetResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the expense item status is updated .
        /// </summary>
        public int newStatus { get; set; }

        /// <summary>
        /// new status for the expense item
        /// </summary>
        public bool IsUpdated { get; set; }
        
        /// <summary>
        /// Gets or sets error message if any.
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// A response containing information regarding sending of DOC reminder email.
    /// </summary>
    public class DutyOfCareResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether sending email was successful or not
        /// </summary>
        public bool IsSendingSuccessful { get; set; }

        /// <summary>
        /// Gets or sets error message if any.
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// The approver email reminder response.
    /// </summary>
    public class ApproverEmailReminderResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether sending email was successful or not
        /// </summary>
        public bool IsSendingSuccessful { get; set; }

        /// <summary>
        /// Gets or sets error message if any.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}

