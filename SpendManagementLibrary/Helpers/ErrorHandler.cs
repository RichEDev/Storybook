using SpendManagementLibrary.Employees;

namespace SpendManagementLibrary.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;

    using BusinessLogic.ProductModules;

    /// <summary>
    /// Handles the sending of errors caught in the application
    /// </summary>
    public abstract class ErrorHandler
    {
        /// <summary>
        /// The values to be used in error emails
        /// </summary>
        private enum ErrorInformation
        {
            /// <summary>
            /// The name of the machine which caused the error
            /// </summary>
            MachineName,

            /// <summary>
            /// The DateTime the error happened
            /// </summary>
            ErrorDateTime,

            /// <summary>
            /// The companyID being used
            /// </summary>
            CompanyID,

            /// <summary>
            /// The accountID being used
            /// </summary>
            AccountID,

            /// <summary>
            /// The company name being used
            /// </summary>
            CompanyName,

            /// <summary>
            /// The subAccountID being accessed
            /// </summary>
            SubAccountID,

            /// <summary>
            /// The subAccount name being accessed
            /// </summary>
            SubAccountName,

            /// <summary>
            /// The employees employeeid
            /// </summary>
            EmployeeID,

            /// <summary>
            /// The employees username
            /// </summary>
            Username,

            /// <summary>
            /// The employees title
            /// </summary>
            Title,

            /// <summary>
            /// The employees forename/firstname
            /// </summary>
            Forename,

            /// <summary>
            /// The employees surname
            /// </summary>
            Surname,

            /// <summary>
            /// The employees email address
            /// </summary>
            EmailAddress,

            /// <summary>
            /// The delegates employeeid
            /// </summary>
            DelegateEmployeeID,

            /// <summary>
            /// The delegates username
            /// </summary>
            DelegateUsername,

            /// <summary>
            /// The delegates title
            /// </summary>
            DelegateTitle,

            /// <summary>
            /// The delegates forename/firstname
            /// </summary>
            DelegateForename,

            /// <summary>
            /// The delegates surname
            /// </summary>
            DelegateSurname,

            /// <summary>
            /// The delegates email address
            /// </summary>
            DelegateEmailAddress,

            /// <summary>
            /// The browser being used
            /// </summary>
            Browser,

            /// <summary>
            /// The browser type being used
            /// </summary>
            BrowserType,

            /// <summary>
            /// The browser agent being used
            /// </summary>
            BrowserAgent,

            /// <summary>
            /// The current page being accessed
            /// </summary>
            Page,

            /// <summary>
            /// The current active module
            /// </summary>
            Module,

            /// <summary>
            /// The id of the report
            /// </summary>
            ReportID,

            /// <summary>
            /// The name of the report
            /// </summary>
            ReportName,

            /// <summary>
            /// The number of rows processed when generating the report
            /// </summary>
            ReportProcessedRows,

            /// <summary>
            /// The export type of the report
            /// </summary>
            ReportExportType,
        }

        /// <summary>
        /// Gets or sets EmailFrom.
        /// </summary>
        protected string EmailFrom { get; set; }

        /// <summary>
        /// Gets or sets EmailTo.
        /// </summary>
        protected string EmailTo { get; set; }

        /// <summary>
        /// Gets or sets EmailServerHostname.
        /// </summary>
        protected string EmailServerHostname { get; set; }

        /// <summary>
        /// Generates an error email from the specified parameters
        /// </summary>
        /// <param name="account">
        /// The account.
        /// </param>
        /// <param name="subAccount">
        /// The sub account.
        /// </param>
        /// <param name="module">
        /// The module.
        /// </param>
        /// <param name="employee">
        /// The employee.
        /// </param>
        /// <param name="delegateEmployee">
        /// The delegate employee.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="isAutomatedProcess">
        /// The is automated process.
        /// </param>
        /// <param name="reportRequest">
        /// The report request.
        /// </param>
        /// <returns>
        /// true if the email was sent, false if it failed
        /// </returns>
        public bool GenerateEmail(cAccount account, cAccountSubAccount subAccount, IProductModule module, Employee employee, Employee delegateEmployee, Exception exception, bool isAutomatedProcess = false, cReportRequest reportRequest = null)
        {
            Dictionary<ErrorInformation, string> replacements = new Dictionary<ErrorInformation, string>();

            bool hasAccountInformation = false;
            bool hasSubAccountInformation = false;
            bool hasEmployeeInformation = false;
            bool hasDelegateInformation = false;
            bool hasWebContext = false;
            bool hasReportInformation = false;

            #region Add Information
            replacements.Add(ErrorInformation.MachineName, Environment.MachineName);
            replacements.Add(ErrorInformation.ErrorDateTime, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            #region Account
            if (account != null)
            {
                hasAccountInformation = true;
                replacements.Add(ErrorInformation.CompanyID, account.companyid);
                replacements.Add(ErrorInformation.AccountID, account.accountid.ToString(CultureInfo.InvariantCulture));
                replacements.Add(ErrorInformation.CompanyName, account.companyname);
            }
            #endregion
            #region Sub Account
            if (subAccount != null)
            {
                hasSubAccountInformation = true;
                replacements.Add(ErrorInformation.SubAccountID, subAccount.SubAccountID.ToString(CultureInfo.InvariantCulture));
                replacements.Add(ErrorInformation.SubAccountName, subAccount.Description);
            }
            #endregion
            replacements.Add(ErrorInformation.Module, (module != null ? module.Name : "Unknown"));
            #region user details
            if (employee != null && employee.EmployeeID != 0)
            {
                hasEmployeeInformation = true;
                replacements.Add(ErrorInformation.EmployeeID, employee.EmployeeID.ToString(CultureInfo.InvariantCulture));
                replacements.Add(ErrorInformation.Title, employee.Title);
                replacements.Add(ErrorInformation.Forename, employee.Forename);
                replacements.Add(ErrorInformation.Surname, employee.Surname);
                replacements.Add(ErrorInformation.EmailAddress, employee.EmailAddress);
                replacements.Add(ErrorInformation.Username, employee.Username);
            }
            #endregion
            #region Delegate
            if (delegateEmployee != null && delegateEmployee.EmployeeID != 0)
            {
                hasDelegateInformation = true;
                replacements.Add(ErrorInformation.DelegateEmployeeID, delegateEmployee.EmployeeID.ToString(CultureInfo.InvariantCulture));
                replacements.Add(ErrorInformation.DelegateTitle, delegateEmployee.Title);
                replacements.Add(ErrorInformation.DelegateForename, delegateEmployee.Forename);
                replacements.Add(ErrorInformation.DelegateSurname, delegateEmployee.Surname);
                replacements.Add(ErrorInformation.DelegateEmailAddress, delegateEmployee.EmailAddress);
                replacements.Add(ErrorInformation.DelegateUsername, delegateEmployee.Username);
            }
            #endregion
            #region Browser
            if (System.Web.HttpContext.Current != null)
            {
                hasWebContext = true;

                replacements.Add(ErrorInformation.Browser, System.Web.HttpContext.Current.Request.Browser.Browser);
                replacements.Add(ErrorInformation.BrowserType, System.Web.HttpContext.Current.Request.Browser.Type);
                if (System.Web.HttpContext.Current.Request.Browser.Capabilities != null && System.Web.HttpContext.Current.Request.Browser.Capabilities[string.Empty] != null)
                {
                    replacements.Add(ErrorInformation.BrowserAgent, System.Web.HttpContext.Current.Request.Browser.Capabilities[string.Empty].ToString());
                }
            }
            #endregion
            #region Page
            if (System.Web.HttpContext.Current != null && (System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Request.Url != null))
            {
                replacements.Add(ErrorInformation.Page, System.Web.HttpContext.Current.Request.Url.AbsoluteUri);
            }
            #endregion
            #region Report
            if (reportRequest != null)
            {
                hasReportInformation = true;
                replacements.Add(ErrorInformation.ReportID, reportRequest.report.reportid.ToString());
                replacements.Add(ErrorInformation.ReportName, reportRequest.report.reportname);
                replacements.Add(ErrorInformation.ReportProcessedRows, reportRequest.ProcessedRows.ToString(CultureInfo.InvariantCulture));
                replacements.Add(ErrorInformation.ReportExportType, reportRequest.exporttype.ToString());
            }
            #endregion
            #endregion

            #region subject
            StringBuilder subject = new StringBuilder("[" + replacements[ErrorInformation.Module] + "][" + replacements[ErrorInformation.MachineName] + "]");
            if (hasAccountInformation)
            {
                subject.Append("[" + account.companyid + "]");
            }

            if (hasEmployeeInformation)
            {
                subject.Append("[" + replacements[ErrorInformation.Username] + "]");
                subject.Append("[" + replacements[ErrorInformation.Forename] + " " + replacements[ErrorInformation.Surname] + "]");
            }
            else
            {
                subject.Append("[Not logged in]");
            }
            #endregion

            StringBuilder body = new StringBuilder();
            #region Generate Email
            body.AppendLine("<html>");
            body.AppendLine("<head>");
            body.Append(this.AddStyles());
            body.AppendLine("</head>");
            body.AppendLine("<body>");
            body.AppendLine("<table>");

            body.Append(AddHeader("Application Details"));
            body.AppendLine(AddLine("Machine Name", replacements[ErrorInformation.MachineName]));
            body.AppendLine(AddLine("Module", replacements[ErrorInformation.Module]));
            body.AppendLine(AddLine("DateTime", replacements[ErrorInformation.ErrorDateTime]));
            if (replacements.ContainsKey(ErrorInformation.Page))
            {
                body.AppendLine(AddLine("Page", replacements[ErrorInformation.Page]));
            }

            body.Append(AddSpaceSpacer());

            #region account/subaccount
            if (hasAccountInformation || hasSubAccountInformation)
            {
                body.Append(AddHeader("Account Details"));
                #region account
                if (hasAccountInformation)
                {
                    body.AppendLine(AddLine("Account", string.Format("{0} ({1})", replacements[ErrorInformation.CompanyName], replacements[ErrorInformation.AccountID])));
                    body.AppendLine(AddLine("CompanyID", replacements[ErrorInformation.CompanyID]));
                }
                #endregion
                #region subaccount
                if (hasSubAccountInformation)
                {
                    body.AppendLine(AddLine("Sub Account", string.Format("{0} ({1})", replacements[ErrorInformation.SubAccountName], replacements[ErrorInformation.SubAccountID])));
                }
                #endregion
                body.Append(AddSpaceSpacer());
            }
            #endregion

            #region user
            body.Append(AddHeader("User Details"));
            if (hasEmployeeInformation)
            {
                body.AppendLine(AddLine("Username", string.Format("{0} ({1})", replacements[ErrorInformation.Username], replacements[ErrorInformation.EmployeeID])));
                body.AppendLine(AddLine("Name", string.Format("{0} {1} {2}", replacements[ErrorInformation.Title], replacements[ErrorInformation.Forename], replacements[ErrorInformation.Surname])));
                body.AppendLine(AddLine("Email", replacements[ErrorInformation.EmailAddress]));
            }
            else
            {
                body.AppendLine(AddLine("User", "Not logged in."));
            }

            body.Append(AddSpaceSpacer());
            #endregion

            #region delegate
            if (hasDelegateInformation)
            {
                body.Append(AddHeader("Delegate Details"));
                body.AppendLine(AddLine("Username", string.Format("{0} ({1})", replacements[ErrorInformation.DelegateUsername], replacements[ErrorInformation.DelegateEmployeeID])));
                body.AppendLine(AddLine("Name", string.Format("{0} {1} {2}", replacements[ErrorInformation.DelegateTitle], replacements[ErrorInformation.DelegateForename], replacements[ErrorInformation.DelegateSurname])));
                body.AppendLine(AddLine("Email", replacements[ErrorInformation.DelegateEmailAddress]));
                body.Append(AddSpaceSpacer());
            }
            #endregion delegate

            #region browser
            if (hasWebContext)
            {
                body.Append(AddHeader("Browser Details"));
                body.AppendLine(AddLine("Browser", string.Format("{0} ({1})", replacements[ErrorInformation.Browser], replacements[ErrorInformation.BrowserType])));
                if (replacements.ContainsKey(ErrorInformation.BrowserAgent))
                {
                    body.AppendLine(AddLine("Browser Agent", replacements[ErrorInformation.BrowserAgent]));
                }

                body.Append(AddSpaceSpacer());
            }
            #endregion

            #region report
            if (hasReportInformation)
            {
                body.Append(AddHeader("Report Details"));
                body.AppendLine(AddLine("Report ID", replacements[ErrorInformation.ReportID]));
                body.AppendLine(AddLine("Name", replacements[ErrorInformation.ReportName]));
                body.AppendLine(AddLine("Export Type", replacements[ErrorInformation.ReportExportType]));
                body.AppendLine(AddLine("Processed Rows", replacements[ErrorInformation.ReportProcessedRows]));
                body.Append(AddSpaceSpacer());
            }
            #endregion

            #region stack trace
            if (exception != null)
            {
                body.Append(AddHeader("Exception Details"));

                if (replacements.ContainsKey(ErrorInformation.Page))
                {
                    body.AppendLine(AddLine("Page", replacements[ErrorInformation.Page]));
                }

                body.AppendLine(AddLine("Message", exception.Message));
                body.AppendLine(AddLine("Stack Trace", exception.StackTrace.Replace("\n", "<br />").Replace("\r\n", "<br />")));
                
                if (exception.InnerException != null)
                {
                    body.AppendLine(AddLine("Inner Exception Message", exception.InnerException.Message));
                    body.AppendLine(AddLine("Inner Exception Stack Trace", exception.InnerException.StackTrace.Replace("\n", "<br />").Replace("\r\n", "<br />")));
                }

                body.Append(AddSpaceSpacer());
            }
            #endregion

            if (hasWebContext)
            {
                if (System.Web.HttpContext.Current.Session != null)
                {
                    if (System.Web.HttpContext.Current.Session.Count > 0)
                    {
                        body.Append(AddHeader("Session Details"));
                        foreach (string key in System.Web.HttpContext.Current.Session.Cast<string>().Where(key => System.Web.HttpContext.Current.Session[key] != null))
                        {
                            body.AppendLine(AddLine(key, System.Web.HttpContext.Current.Session[key] != null ? System.Web.HttpContext.Current.Session[key].ToString() : ""));
                        }

                        body.Append(AddSpaceSpacer());
                    }
                }

                #region form values
                if (System.Web.HttpContext.Current.Request.Form.Count > 0)
                {
                    body.Append(AddHeader("Form Details"));
                    foreach (string key in System.Web.HttpContext.Current.Request.Form.AllKeys.Where(key => key != "__VIEWSTATE").Where(key => System.Web.HttpContext.Current.Request.Form[key] != null))
                    {
                        body.Append(key.ToLower().Contains("password") == false ? AddLine(key, System.Web.HttpContext.Current.Request.Form[key]) : AddLine(key, "Password field not displayed"));
                    }

                    body.Append(AddSpaceSpacer());
                }
                #endregion
                #region cookies
                if (System.Web.HttpContext.Current.Request.Cookies.Count > 0)
                {
                    body.Append(AddHeader("Cookies"));
                    foreach (string key in System.Web.HttpContext.Current.Request.Cookies.AllKeys.Where(key => System.Web.HttpContext.Current.Request.Cookies[key] != null))
                    {
                        body.AppendLine(AddLine(key, System.Web.HttpContext.Current.Request.Cookies[key].Value));
                    }

                    body.Append(AddSpaceSpacer());
                }
                #endregion
                #region Server variables
                if (System.Web.HttpContext.Current.Request.ServerVariables.Count > 0)
                {
                    body.Append(AddHeader("Server Variables"));
                    foreach (string key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys.Where(key => System.Web.HttpContext.Current.Request.ServerVariables[key] != null))
                    {
                        body.AppendLine(AddLine(key, System.Web.HttpContext.Current.Request.ServerVariables[key]));
                    }

                    body.Append(AddSpaceSpacer());
                }
                #endregion
                #region Application
                if (System.Web.HttpContext.Current.Application.Count > 0)
                {
                    body.Append(AddHeader("Application Variables"));
                    foreach (string key in System.Web.HttpContext.Current.Application.AllKeys)
                    {
                        body.AppendLine(AddLine(key, System.Web.HttpContext.Current.Application[key].ToString()));
                    }

                    body.Append(AddSpaceSpacer());
                }
                #endregion
            }

            body.AppendLine("</table>");
            body.AppendLine("</body>");
            body.AppendLine("</html>");
            #endregion           
           
            #region to/from/server
            if (GlobalVariables.CompiledMode == ProductIntegrity.CompileMode.Debug)
            {
                this.EmailFrom = GlobalVariables.DefaultTestingErrorEmailFromAddress;
                this.EmailTo = GlobalVariables.DefaultTestingErrorEmailToAddress;
                this.EmailServerHostname = GlobalVariables.DefaultTestingErrorEmailHostname;
            } 
            else if (hasEmployeeInformation)
            {
                this.EmailFrom = subAccount.SubAccountProperties.ErrorEmailFromAddress;
                this.EmailTo = subAccount.SubAccountProperties.ErrorEmailAddress;
                this.EmailServerHostname = subAccount.SubAccountProperties.EmailServerAddress;
            }
            else
            {
                this.EmailFrom = GlobalVariables.DefaultErrorEmailFromAddress;
                this.EmailTo = GlobalVariables.DefaultErrorEmailToAddress;
                this.EmailServerHostname = GlobalVariables.DefaultEmailServerHostname;
            }
            #endregion to/from/server

            EmailSender sender = new EmailSender(this.EmailServerHostname);
            MailMessage msg = new MailMessage(this.EmailFrom, this.EmailTo, subject.ToString(), body.ToString())
                { IsBodyHtml = true };
            bool emailSent = sender.SendEmail(msg);

            return emailSent;
        }
        #region helper methods

        /// <summary>
        /// Adds a header to the error email table
        /// </summary>
        /// <param name="header">The header text to use</param>
        /// <returns>A header row</returns>
        private static string AddHeader(string header)
        {
            return string.Format("<tr><th colspan=\"2\">{0}</th></tr>", header);
        }

        /// <summary>
        /// Add a line to the error email table
        /// </summary>
        /// <param name="label">The label to use in the new row</param>
        /// <param name="value">The value to use in the new row</param>
        /// <returns>A single row populated with label and value</returns>
        private static string AddLine(string label, string value)
        {
            return string.Format("<tr><td class=\"label\">{0}</td><td>{1}</td></tr>", label, value);
        }

        /// <summary>
        /// Generates a blank spacer column in a new row
        /// </summary>
        /// <returns>
        /// A spacer cell in a new row
        /// </returns>
        private static string AddSpaceSpacer()
        {
            return "<tr><td class=\"blank\" colspan=\"2\">&nbsp;</td>";
        }

        /// <summary>
        /// Adds the styles used to the email error html
        /// </summary>
        /// <returns>A style block</returns>
        private string AddStyles()
        {
            StringBuilder styles = new StringBuilder();
            styles.AppendLine("<style>");
            styles.AppendLine("th { font-family: verdana; background-color: #0066cc; color: #ffffff; text-align: left; padding: 3px; font-size: 1em; }");
            styles.AppendLine("td {  font-family: verdana; font-size: 0.7em; border-bottom: 1px dashed #000000; padding: 4px; }");
            styles.AppendLine("td.label {  font-weight: bold; font-size: 0.8em; vertical-align: top; }");
            styles.AppendLine("td.blank { border-bottom: 0px; }");
            styles.AppendLine("</style>");
            return styles.ToString();
        }
        #endregion
    }
}
