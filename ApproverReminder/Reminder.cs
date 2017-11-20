using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using APICallsHelper;
using Common.Logging;
using Common.Logging.Log4Net;

namespace ApproverReminder
{
    /// <summary>
    /// A base class for Reminders of type <see cref="IReminderResponse"/>
    /// </summary>
    /// <typeparam name="T">An instance of type <see cref="IReminderResponse"/></typeparam>
    public class Reminder<T> where T:IReminderResponse
    {
        private readonly RequestHelper _requestHelper;
        private readonly ResponseHelper _responseHelper;
        private readonly List<cAccount> _accounts;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging information.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// An instance of <see cref="IExtraContext"/> for logging extra inforamtion
        /// </summary>
        private readonly IExtraContext _loggingContent = new Log4NetContextAdapter();

        /// <summary>
        /// Create a new instance of <see cref="Reminder{T}"/>
        /// </summary>
        /// <param name="requestHelper">object of type <see cref="RequestHelper"/> which helps making API requests.</param>
        /// <param name="responseHelper">object of type <see cref="ResponseHelper"/>  which helps to carry out operations on response of api request.</param>
        /// <param name="accounts">A <see cref="List{T}"/>if <seealso cref="cAccount"/></param>
        /// <param name="log">An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.</param>
        public Reminder(RequestHelper requestHelper, ResponseHelper responseHelper, List<cAccount> accounts, ILog log)
        {
            this._requestHelper = requestHelper;
            this._responseHelper = responseHelper;
            this._accounts = accounts;
            this._log = log;
        }

        /// <summary>
        /// Generate the reminders for the current class type.
        /// </summary>
        /// <param name="apiEndPoint">The Api Endpoint to use NOTE: Any {0} will be replaced by the current AccountID (if any).</param>
        public void GenerateReminders(string apiEndPoint)
        {
            this._log.Debug("Method GenerateReminders has started.");

            foreach (cAccount account in this._accounts)
            {
                var accountid = account.AccountId;
                this._loggingContent["accountid"] = accountid;
                var endPoint = string.Format(apiEndPoint, accountid);
                var request = this._requestHelper.GetHttpWebRequest(endPoint);
                request.Timeout = 300000;

                try
                {
                    if (this._log.IsInfoEnabled)
                    {
                        this._log.Info($"Generating reminders from {endPoint}");
                    }

                    var response = (HttpWebResponse) request.GetResponse();
                    T emailSenderResponse =
                        this._responseHelper.GetResponseObject<T>(response,
                            new StreamReader(response.GetResponseStream()));

                    if (!emailSenderResponse.IsSendingSuccessful)
                    {
                        if (this._log.IsWarnEnabled)
                        {
                            this._log.Warn($"Email reminders were not sent, message: {emailSenderResponse.ErrorMessage}, status code: {response.StatusCode}");
                        }

                        continue;
                    }

                    if (this._log.IsInfoEnabled)
                    {
                        this._log.Info($"Emails reminders were sent successfully, status code: {response.StatusCode}");
                    }
                }
                catch (ProtocolViolationException protocolViolationException)
                {
                    this.LogErrorMessage(endPoint, protocolViolationException);
                }
                
                catch (NotSupportedException notSupportedException)
                {
                    this.LogErrorMessage(endPoint, notSupportedException);
                }
                catch (WebException webException)
                {
                    this.LogErrorMessage(endPoint, webException);
                }
                catch (InvalidOperationException ioe)
                {
                    this.LogErrorMessage(endPoint, ioe);
                }
            }
            
            this._log.Debug("Method GenerateReminders has completed.");
        }

        /// <summary>
        /// Logs handled errors when sending email reminders
        /// </summary>
        /// <param name="endPoint">Location of the API endpoint where the error occured</param>
        /// <param name="error">The error that occured</param>
        public void LogErrorMessage(string endPoint, Exception error)
        {
            if (this._log.IsWarnEnabled)
            {
                this._log.Warn($"Email reminders failed on {endPoint} due to {error.Message}", error);
            }
        }
    }
}
