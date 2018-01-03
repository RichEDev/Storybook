namespace ApiClientHelper
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;

    using ApiClientHelper.Models;
    using Common.Logging;
    using DutyOfCareAPI.DutyOfCare;

    using Newtonsoft.Json;
    using Responses;
    using RestSharp;

    /// <summary>
    /// The client.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// The username.
        /// </summary>
        private readonly string _username;

        /// <summary>
        /// The password.
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// Api url path.
        /// </summary>
        private readonly string _apiUrlPath;

        /// <summary>
        /// The auth token.
        /// </summary>
        private string _authToken;

        /// <summary>
        /// The company id.
        /// </summary>
        private string _companyId;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<Client>().GetLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <param name="apiUrlPath">Api url path.</param>
        public Client(string companyId, string apiUrlPath)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info($"Instantiate class with companyid '{companyId}' and apiUrlPath '{apiUrlPath}'");
            }
            this._companyId = companyId;
            this._apiUrlPath = apiUrlPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class which doesn't require authentication.
        /// </summary>
        /// <param name="apiUrlPath"></param>
        public Client(string apiUrlPath)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info($"Instantiate class with apiUrlPath '{apiUrlPath}'");
            }
            this._apiUrlPath = apiUrlPath;
        }

        /// <summary>
        /// Allows the API client to switch between SEL customer accounts, dropping the auth token will force the client to perform a new logon prior to the next request.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        public void SetCompany(string companyId)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"Set Company({companyId})");
            }

            if (this._companyId == companyId)
            {
                return;
            }

            this._authToken = string.Empty;
            this._companyId = companyId;
        }

        /// <summary>
        /// A generic, error handling, connectivity checking RestSharp wrapper for performing asynchronous web requests
        /// All API requests should use this method rather than calling RestSharpClient.ExecuteWhatever method
        /// </summary>
        /// <typeparam name="T">The response object type</typeparam>
        /// <param name="request">The <see cref="RestRequest">request</see> object</param>
        /// <param name="withAuthentication">Optionally skip adding an authentication token to the request by setting this to false, default is true</param>
        /// <returns>A <see cref="IRestResponse" /></returns>
        public async Task<IRestResponse<T>> BaseRequest<T>(RestRequest request, bool withAuthentication = true)
        {
            Log.Debug("BaseRequest");
            IRestResponse<T> response;

            if (withAuthentication)
            {
                if (string.IsNullOrEmpty(this._authToken))
                {
                    await this.Logon();
                }

                request.AddHeader("AuthToken", this._authToken);
            }

            try
            {
                var client = new RestClient(this._apiUrlPath);
                if (Log.IsDebugEnabled)
                {
                    Log.Debug($"rest client url {client.BaseUrl}{request.Resource} request verb {request.Method}");
                }

                Console.WriteLine($"rest client url {client.BaseUrl}{request.Resource} request verb {request.Method}");
                response = await client.ExecuteTaskAsync<T>(request, new CancellationTokenSource(360000).Token);
            }
            catch (Exception exception)
            {
                response = new RestResponse<T>
                {
                    ResponseStatus = exception is TaskCanceledException ? ResponseStatus.TimedOut : ResponseStatus.Error,
                    ErrorException = exception
                };

                if (response.ResponseStatus == ResponseStatus.TimedOut)
                {
                    Log.Error("A timeout occurred when communicating with the API.");
                    response.ErrorMessage = "A timeout occurred when communicating with the API.";
                }
            }

          if (!this.ValidateResponse(response))
            {
                var message = string.Empty;
                var logEntry = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
                {
                    Log.Error($"Error message from API : {response.ErrorMessage}");
                    logEntry.AppendLine($"Error Message: {response.ErrorMessage}");
                }

                if (response.ErrorException != null)
                {
                    Log.Error($"Error exception from API : {response.ErrorException}");
                    logEntry.AppendLine($"Error Exception: {response.ErrorException}");
                }

                logEntry.AppendLine($"Response Content: {response.Content}");
                logEntry.AppendLine($"Response Status Code: {response.StatusCode}");
                if (Log.IsDebugEnabled)
                {
                    Log.Debug($"Response Status Code: {response.StatusCode}");
                }

                Log.Info(logEntry);

                EventLog.WriteEntry("Api client", logEntry.ToString());

                if (!string.IsNullOrEmpty(message))
                {
                    response.ErrorMessage = message;
                }
            }

            return response;
        }

        /// <summary>
        /// A generic, error handling, connectivity checking RestSharp wrapper for performing asynchronous web requests
        /// All API requests should use this method rather than calling RestSharpClient.ExecuteWhatever method
        /// </summary>
        /// <typeparam name="T">The response object type</typeparam>
        /// <param name="request">The <see cref="RestRequest">request</see> object</param>
        /// <param name="withAuthentication">Optionally skip adding an authentication token to the request by setting this to false, default is true</param>
        /// <returns> Json Deserialised response object type<see cref="T" /></returns>
        public async Task<T> BaseRequestJSon<T>(RestRequest request, bool withAuthentication = true)
        {
            Log.Debug("BaseRequestJSon");
            IRestResponse<T> keyResponse;

            if (withAuthentication)
            {
                if (string.IsNullOrEmpty(this._authToken))
                {
                    await this.Logon();
                }

                request.AddHeader("AuthToken", this._authToken);
            }

            try
            {
                var client = new RestClient(this._apiUrlPath);
                keyResponse = await client.ExecuteTaskAsync<T>(request, new CancellationTokenSource(360000).Token);
            }
            catch (Exception exception)
            {
                keyResponse = new RestResponse<T>
                {
                    ResponseStatus = exception is TaskCanceledException ? ResponseStatus.TimedOut : ResponseStatus.Error,
                    ErrorException = exception
                };

                if (keyResponse.ResponseStatus == ResponseStatus.TimedOut)
                {
                    Log.Error("A timeout occurred when communicating with the API.");
                    keyResponse.ErrorMessage = "A timeout occurred when communicating with the API.";
                }
            }

            if (!this.ValidateResponse(keyResponse))
            {
                var message = string.Empty;
                var logEntry = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(keyResponse.ErrorMessage))
                {
                    Log.Error($"Error Message: {keyResponse.ErrorMessage}");
                    logEntry.AppendLine($"Error Message: {keyResponse.ErrorMessage}");
                }

                if (keyResponse.ErrorException != null)
                {
                    Log.Error($"Error Message: {keyResponse.ErrorException}");
                    logEntry.AppendLine($"Error Exception: {keyResponse.ErrorException}");
                }

                logEntry.AppendLine($"Response Content: {keyResponse.Content}");
                logEntry.AppendLine($"Response Status Code: {keyResponse.StatusCode}");
                EventLog.WriteEntry("Api client", logEntry.ToString());
                Log.Info(logEntry);

                if (!string.IsNullOrEmpty(message))
                {
                    keyResponse.ErrorMessage = message;
                }
            }

            return JsonConvert.DeserializeObject<T>(keyResponse.Content);
        }

        /// <summary>
        /// Validate response from api
        /// </summary>
        /// <param name="response">Response to validate</param>
        /// <returns>Gives bool value to decide valid response</returns>
        public bool ValidateResponse(IRestResponse response)
        {
            return response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Calls api to get valid accounts for particular general option
        /// </summary>
        /// <param name="apiEndPointGetAccount">Api to fetch accounts</param>
        /// <returns>List of accounts</returns>
        public Task<IRestResponse<AccountsForParticularGeneralOption>> GetAccountsForParticularGeneralOptionEnabled(string apiEndPointGetAccount)
        {
            var request = new RestRequest(apiEndPointGetAccount);
            return this.BaseRequest<AccountsForParticularGeneralOption>(request, false);
        }

        /// <summary>
        /// Calls api to get valid accounts for auto update of exchange rates enabled
        /// </summary>
        /// <param name="apiEndPointGetAccountsWithAutoUpdateExchangeRateEnabled">Api to fetch accounts with auto update of exchange rates enabled</param>
        /// <returns>List of accounts</returns>
        public Task<IRestResponse<AccountsForParticularGeneralOption>> GetAccountsWithAutoUpdateExchangeRateEnabled(string apiEndPointGetAccountsWithAutoUpdateExchangeRateEnabled)
        {
            var request = new RestRequest(apiEndPointGetAccountsWithAutoUpdateExchangeRateEnabled);
            return this.BaseRequest<AccountsForParticularGeneralOption>(request, withAuthentication: false);
        }

        /// <summary>
        /// Calls api to get employees to populate driving licence
        /// </summary>
        /// <param name="apiEndPointToGetEmployeeWithDvlaConstraint">
        /// Api to fetch employees details to populate driving licence
        /// </param>
        /// <returns>
        /// The <see cref="Responses.EmployeesToPopulateDrivingLicence"/>.
        /// Employees information 
        /// </returns>
        public EmployeesToPopulateDrivingLicence GetEmployeeToPopulateDrivingLicence(string apiEndPointToGetEmployeeWithDvlaConstraint)
        {
            var request = new RestRequest(apiEndPointToGetEmployeeWithDvlaConstraint);
            var result = this.BaseRequest<EmployeesToPopulateDrivingLicence>(request, false).Result.Content;
            return new JavaScriptSerializer().Deserialize<EmployeesToPopulateDrivingLicence>(result);
        }

        /// <summary>
        /// Calls api to get the list of active currencies for an account
        /// </summary>
        /// <param name="apiEndPointToGetAListOfActiveCurrencies">
        /// Api to get the list of active currencies for an account
        /// </param>
        /// <returns>
        /// The <see cref="Responses.Currencies"/>.
        /// A list of active currencies
        /// </returns>
        public Currencies GetActiveCurrencies(string apiEndPointToGetAListOfActiveCurrencies)
        {
            var request = new RestRequest(apiEndPointToGetAListOfActiveCurrencies);
            var result = this.BaseRequest<Currencies>(request, withAuthentication: false).Result.Content;
            return new JavaScriptSerializer().Deserialize<Currencies>(result);
        }

        /// <summary>
        /// The post exchange rates.
        /// </summary>
        /// <param name="apiEndPointToPostExchangeRates">
        /// The api end point to post exchange rates.
        /// </param>
        /// <param name="exchangerates">
        /// The exchange rates that need to be updated for each account
        /// </param>
        /// <param name="accountId">Account ID the rates are for</param>
        /// <returns>
        /// The <see cref="bool"/> true if exchange rate has been updated for the account
        /// </returns>
        public bool PostExchangeRates(string apiEndPointToPostExchangeRates, CurrencyExchangeRatesList exchangerates, int accountId)
        {
            var request = new RestRequest(apiEndPointToPostExchangeRates + "?accountId=" + accountId, Method.POST)
                                {
                                    RequestFormat = DataFormat.Json
                                };
            request.AddBody(exchangerates);

            var response = this.BaseRequest<object>(request, withAuthentication: false);
            if (response.Result.Data != null)
            {
                object output;
                new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(response.Result.Content).TryGetValue("IsExchangeRateAdded", out output);
                return Convert.ToBoolean(output);
            }

            return false;
        }

        /// <summary>
        /// The email notification for enabled services.
        /// </summary>
        /// <param name="apiEndPointToTriggerSendEmailController">
        /// The api end point to trigger send email controller.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> Email reminder response.
        /// </returns>
        public Task<IRestResponse<EmailReminderResponse>> EmailNotificationForEnabledServices(string apiEndPointToTriggerSendEmailController)
        {
            var request = new RestRequest(apiEndPointToTriggerSendEmailController, Method.GET)
                              {
                                  RequestFormat = DataFormat.Json
                              };

            var response = this.BaseRequest<EmailReminderResponse>(request, false);
            return response;
        }

        /// <summary>
        /// The populate driving licences.
        /// </summary>
        /// <param name="apiEndPointToPopulateDrivingLicence">
        /// The api end point to populate driving licence.
        /// </param>
        /// <param name="employeeDetails">
        /// The employee details.
        /// </param>
        /// <returns>
        /// The <see cref="DrivingLicenceResponse"/>.
        /// Driving licence details
        /// </returns>
        public DrivingLicenceResponse PopulateDrivingLicences(string apiEndPointToPopulateDrivingLicence, EmployeesToPopulateDrivingLicence employeeDetails)
        {
            var request = new RestRequest(apiEndPointToPopulateDrivingLicence, Method.POST)
            {
                RequestFormat =
                    DataFormat.Json
                              };


            request.AddBody(employeeDetails);
            var response = this.BaseRequest<DrivingLicenceResponse>(request, false);
            return new JavaScriptSerializer().Deserialize<DrivingLicenceResponse>(response.Result.Content);
        }

        /// <summary>
        /// Calls api to get save employee driving licence information.
        /// </summary>
        /// <param name="apiEndPointToSaveDrivingLicence">Api to save driving licence information</param>
        /// <param name="licenceDetails">object of type <see cref="DrivingLicenceResponse"/>driving licence details to save</param>
        public bool SaveCustomEntity(string apiEndPointToSaveDrivingLicence, DrivingLicenceResponse licenceDetails)
        {
            var request = new RestRequest(apiEndPointToSaveDrivingLicence, Method.POST)
            {
                  RequestFormat = DataFormat.Json
            };
            request.AddJsonBody(licenceDetails);
            var response = this.BaseRequest<object>(request, false);
            if (response.Result.Data != null)
            {
                object output;
                new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(response.Result.Content).TryGetValue("IsDrivingLicenceAdded", out output);
                return Convert.ToBoolean(output);
            }

            return false;
        }

        /// <summary>
        /// Calls the API to send an email to a specific employee (contained in the  api End Point).
        /// </summary>
        /// <param name="apiEndPoint">The endpoint for the api call.</param>
        /// <returns></returns>
        public bool SendEmailNotificationForExcessMileage(string apiEndPoint)
        {

            var request = new RestRequest(apiEndPoint, Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            var response = this.BaseRequest<object>(request, false);
            if (response.Result.Data != null)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Calls the API to send an email to a specific employee (contained in the  api End Point).
        /// </summary>
        /// <param name="apiEndPoint">The endpoint for the api call.</param>
        /// <returns>True unless the api call fails</returns>
        public bool SendEmailNotificationForExcessMileage(string apiEndPoint)
        {

            var request = new RestRequest(apiEndPoint, Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            var response = this.BaseRequest<object>(request, false);
            if (response.Result.Data != null)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Process a card file via the API
        /// </summary>
        /// <param name="endPoint">The API endpoint to use</param>
        /// <param name="fileInformation">An instance of <see cref="FileDataResponse"/>containing a 64 bit encoded file content</param>
        /// <returns>An instance of <seealso cref="CorporateCardAutoMatchResponse"/>wrapped in a <seealso cref="Task"/></returns>
        public Task<IRestResponse<CorporateCardAutoMatchResponse>> ProcessCorporateCardFile(string endPoint, FileDataResponse fileInformation)
        {
            Log.Debug("ProcessCorporateCardFile");
            var request = new RestRequest(endPoint, Method.POST);
            request.AddJsonBody(fileInformation);
            var result =  this.BaseRequest<CorporateCardAutoMatchResponse>(request, false);
            if (Log.IsInfoEnabled)
            {
                Log.Info($"Returned {result.Result}");
            }
            return result;
        }

        /// <summary>
        /// Helps to switch customer accounts
        /// </summary>
        /// <returns>Login details</returns>
        private async Task<IRestResponse<LoginResponse>> Logon()
        {
            var request = new RestRequest("/Account/Login", Method.POST);
            request.AddParameter("Company", this._companyId);
            request.AddParameter("Username", this._username);
            request.AddParameter("Password", this._password);

            var response = await this.BaseRequest<LoginResponse>(request, false);

            if (response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == HttpStatusCode.OK)
            {
                this._authToken = response.Data.AuthToken;
            }

            return response;
        }
    }
}