namespace FuelReceiptToVATCalculation
{
    using Common.Logging;
    using Common.Logging.Log4Net;
    using APICallsHelper;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Net;
    using System.Linq;

    /// <summary>
    /// Console application which process Fuel receipt to mileage calculation
    /// </summary>
    public class FuelReceiptToVATCalculation
    {
        /// <summary>
        /// Api URL to  the Get Accounts With FuelReceiptToVATCalculationEnabled .
        /// </summary>
        private const string ApiEndPointGetAccount = "FuelReceiptToVATCalculation/GetAccountsWithFuelReceiptToVATCalculationEnabled";

        /// <summary>
        /// Api URL for Processing the calculation
        /// </summary>
        private const string ApiEndPointProcessCalculation = "FuelReceiptToVATCalculation/Process";

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging information.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// An instance of <see cref="IExtraContext"/> for logging extra inforamtion
        /// </summary>
        private static readonly IExtraContext LoggingContent = new Log4NetContextAdapter();

        /// <summary>
        /// Create a new instance of <see cref="FuelReceiptToVATCalculation"/>
        /// </summary>
        /// <param name="log">An instance of <see cref="ILog"/> for logging</param>
        public FuelReceiptToVATCalculation(ILog log)
        {
            this._log = log;
        }

        /// <summary>
        /// Method which calls the api GetAccountsWithFuelReceiptToVATCalculationEnabled and then Api  for Processing the calculation.
        /// </summary>
        /// <param name="requestHelper">object of type <see cref="RequestHelper"/> which helps making API requests.</param>
        /// <param name="responseHelper">object of type <see cref="ResponseHelper"/>  which helps to carry out operations on response of api request.</param>
        /// <param name="authToken">object of type <see cref="AuthToken"/> to help generate authentication to make API calls</param>
        public void AllocateFuelReceiptToMileage(RequestHelper requestHelper, ResponseHelper responseHelper, AuthToken authToken)
        {
            this._log.Debug("Method AllocateFuelReceiptToMileage has started.");

            var request = requestHelper.GetHttpWebRequest(ApiEndPointGetAccount);
            request.Timeout = 300000;
            request.Headers.Add("AuthToken", authToken.GetAuthToken());
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine("Fuel Receipt To VAT Calculation Process started");
                var fuelReceiptCalculationResponse = responseHelper.GetResponseObject<GetAccountVatCalculationEnabledResponses>(response, new StreamReader(response.GetResponseStream()));
                Console.WriteLine("Api call to Get Accounts With FuelReceiptToVATCalculationEnabled made successfully");

                if (fuelReceiptCalculationResponse.List.Count == 0)
                {
                    this._log.Info("No accounts with VAT Calculation enabled");

                    Console.WriteLine("No account with VAT Calculation Enabled");
                    return;
                }
                else
                {
                    if (this._log.IsInfoEnabled)
                    {
                        this._log.Info($"{fuelReceiptCalculationResponse.List.Count} accounts with VAT Calculation enabled found.");
                    }

                    this._log.Debug("Method ProcessCalculation has started.");
                    foreach (int accountId in fuelReceiptCalculationResponse.List)
                    {
                        Console.WriteLine("Process Vat Calculation started for account " + accountId);
                        this.ProcessCalculation(new RequestHelper(), new ResponseHelper(), new AuthToken(), accountId);
                    }

                    this._log.Debug("Method ProcessCalculation has completed.");
                }
            }
            catch (Exception ex)
            {
                if (this._log.IsWarnEnabled)
                {
                    this._log.Warn($"AllocateFuelReceiptToMileage failed on {ApiEndPointGetAccount} due to {ex.Message}", ex);
                }
            }
            finally
            {
                this._log.Debug("Method AllocateFuelReceiptToMileage has completed.");
                Console.WriteLine("Process Vat Calculation completed");
                if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count()> 1) System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// ProcessCalculation Build the api post request and call the API
        /// </summary>
        /// <param name="requestHelper"></param>
        /// <param name="responseHelper"></param>
        /// <param name="authToken"></param>
        /// <param name="accountid"></param>
        private void ProcessCalculation(RequestHelper requestHelper, ResponseHelper responseHelper, AuthToken authToken, int accountid)
        {
            LoggingContent["accountid"] = accountid;

            try
            {
                var httpWebRequest = requestHelper.GetHttpWebRequest(ApiEndPointProcessCalculation);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                Account account = new Account();
                account.AccountId = accountid;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string accountJson = JsonConvert.SerializeObject(account);
                    streamWriter.Write(accountJson);
                    streamWriter.Flush();
                    streamWriter.Close();
                    httpWebRequest.Headers.Add("AuthToken", authToken.GetAuthToken());
                    var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                    var fuelReceiptCalculationProcessResponse =
                        responseHelper.GetResponseObject<FuelReceiptToVATCalculationProcessResponse>(httpResponse,
                            new StreamReader(httpResponse.GetResponseStream()));
                    Console.WriteLine("Calculation completed successfully for the account " + accountid);

                    if (this._log.IsInfoEnabled)
                    {
                        this._log.Info($"Process Vat Calculation completed successfully for account {accountid}.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (this._log.IsWarnEnabled)
                {
                    this._log.Warn(
                        $"Process Vat Calculation failed on {ApiEndPointProcessCalculation} for account {accountid} due to {ex.Message}",
                        ex);
                }

                Console.WriteLine("Process Vat Calculation failed for account " + accountid + " " + ex.Message);
            }

            LoggingContent.Remove("accountid");
        }
    }
}
