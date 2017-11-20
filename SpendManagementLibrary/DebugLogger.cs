using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

using SpendManagementLibrary.Helpers;

namespace SpendManagementLibrary
{
    /// <summary>
    /// Class used to log developer messages to the metabase to aid in debugging
    /// </summary>
    public class DebugLogger
    {

        private readonly bool _disabled;
        private readonly ICurrentUserBase _currentUser;
        private readonly HttpRequest _request;

        public DebugLogger(ICurrentUserBase currentUser, HttpRequest request = null)
        {
            if (!GlobalVariables.GetAppSettingAsBoolean("DebugMerge"))
            {
                this._disabled = true;
                return;
            }

            this._currentUser = currentUser;
            this._request = request;

        }

        /// <summary>
        /// Add an entry to the debug log (if DebugMerge is true)
        /// AccountId, UserEmployeeId, DeligateEmployeeId, URI, Request Body, Headers, PostData, Cookies, Server Variables and StackTrace are logged automatically if available
        /// </summary>
        /// <param name="source">A name used to group several messages together (for example it might be a method name)</param>
        /// <param name="exception">The exception to be logged</param>
        /// <param name="additionalData">Any additional data you wish to log along with the exception</param>
        public DebugLogger Log(string source, Exception exception, params object[] additionalData)
        {
            if (this._disabled)
            {
                return this;
            }

            var messages = new List<string>();
            var ex = exception;

            while (ex != null)
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }

            return this.Log(source, String.Join("\n=>\n", messages), additionalData);

        }

        /// <summary>
        /// Add an entry to the debug log (if DebugMerge is true)
        /// AccountId, UserEmployeeId, DeligateEmployeeId, URI, Request Body, Headers, PostData, Cookies, Server Variables and StackTrace are logged automatically if available
        /// </summary>
        /// <param name="source">A name used to group several messages together (for example it might be a method name)</param>
        /// <param name="message">The distinct message (for example a line number, or "Opening db connection")</param>
        /// <param name="additionalData">Any additional data you wish to log along with the message</param>
        public DebugLogger Log(string source, string message = null, params object[] additionalData)
        {
            if (this._disabled)
            {
                return this;
            }

            int? accountId = null, userEmployeeId = null, deligateEmployeeId = null;

            try
            {
                if (this._currentUser != null)
                {
                    accountId = this._currentUser.AccountID;
                    userEmployeeId = this._currentUser.EmployeeID;

                    if (this._currentUser.isDelegate)
                    {
                        deligateEmployeeId = this._currentUser.Delegate.EmployeeID;
                    }
                }
            }
            catch
            {
            }

            string uri = null, body = null, headers = null, postData = null, cookies = null, vars = null;

            try
            {
                if (this._request != null)
                {
                    uri = this._request.Url.ToString();
                    body = StreamToString(this._request.InputStream);
                    headers = JsonEncode(this._request.Headers);
                    postData = JsonEncode(this._request.Form);
                    cookies = JsonEncode(this._request.Cookies);
                    vars = JsonEncode(this._request.ServerVariables);
                }
            }
            catch
            {
            }

            try
            {
                using (var databaseConnection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@Source", source);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@Message", (!String.IsNullOrWhiteSpace(message) ? (object)message : DBNull.Value));
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@AccountId", (object)accountId ?? DBNull.Value);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@UserEmployeeId", (object)userEmployeeId ?? DBNull.Value);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@DeligateEmployeeId", (object)deligateEmployeeId ?? DBNull.Value);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@Uri", (object)uri ?? DBNull.Value);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@Body", (object)body ?? DBNull.Value);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@Headers", (object)headers ?? DBNull.Value);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@PostData", (object)postData ?? DBNull.Value);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@Cookies", (object)cookies ?? DBNull.Value);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@Servervariables", (object)vars ?? DBNull.Value);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@AdditionalData", (additionalData.Length > 0 ? (object)JsonEncode(additionalData) : DBNull.Value));
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@StackTrace", Environment.StackTrace);
                    databaseConnection.ExecuteProc("AddDebugLog");
                }
            }
            catch
            {
            }

            return this;

        }

        private static string StreamToString(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using (var bodyReader = new StreamReader(stream))
            {
                return bodyReader.ReadToEnd();
            }
        }

        private static string JsonEncode(NameValueCollection collection)
        {
            return (collection.AllKeys.Length > 0 ? "{\n\t" + String.Join(",\n\t", collection.AllKeys.Select(key => JsonEncode(key) + ": " + JsonEncode(collection[key]))) + "\n}" : null);
        }

        private static string JsonEncode(HttpCookieCollection collection)
        {
            return (collection.AllKeys.Length > 0 ? "{\n\t" + String.Join(",\n\t", collection.AllKeys.Where(key => collection[key] != null).Select(key => JsonEncode(key) + ": " + JsonEncode(collection[key] == null ? null : collection[key].Value))) + "\n}" : null);
        }

        private static string JsonEncode(object[] collection)
        {
            return (collection.Length > 0 ? "[\n\t" + String.Join(",\n\t", collection.Select(key => key == null ? String.Empty : JsonEncode(key.ToString()))) + "\n]" : null);
        }

        private static string JsonEncode(string text)
        {
            if (text == null)
            {
                return "null";
            }
            return "\"" + text.Replace("\"", "''") + "\"";
        }
    }
}
