namespace Spend_Management.shared.code.Authentication
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using System.Web.SessionState;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.Identity;

    using Common.Cryptography;

    using global::expenses;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    ///     Logon class to authenticate and create cookie to log a user on.
    /// </summary>
    public class Logon
    {
        /// <summary>
        /// A private instance of <see cref="IEncryptor"/>
        /// </summary>
        private readonly IEncryptor _encryptor;

        #region Fields

        /// <summary>
        ///     The account id.
        /// </summary>
        private int accountId;

        /// <summary>
        ///     The current sub account.
        /// </summary>
        private int currentSubAccount;

        /// <summary>
        ///     The employee id.
        /// </summary>
        private int employeeId;

        /// <summary>
        /// The forgotten details visible.
        /// </summary>
        private bool forgottenDetailsVisible;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initialises a new instance of the <see cref="Logon" /> class.
        /// </summary>
        public Logon(IEncryptor encryptor)
        {
            this._encryptor = encryptor;
            this.accountId = 0;
            this.employeeId = 0;
            this.forgottenDetailsVisible = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether to show forgotten details reminder on the logon page.
        /// </summary>
        public bool ShowForgottenDetails
        {
            get
            {
                return this.forgottenDetailsVisible;
            }
        }

        /// <summary>
        /// Gets the EmployeeId - make sure you have at least attempted a login first.
        /// </summary>
        public int EmployeeId
        {
            get
            {
                return employeeId;
            }
        }

        /// <summary>
        /// Gets the Account Id - make sure you have at least attempted a login first.
        /// </summary>
        public int AccountId
        {
            get
            {
                return accountId;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The redirect to hostname.
        /// </summary>
        /// <param name="currentAddress">
        /// The current address.
        /// </param>
        /// <param name="currentAccount">
        /// The current account.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="currentModule">
        /// The current active module
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetRedirectToHostnameAddress(
            string currentAddress, 
            cAccount currentAccount, 
            string username, 
            string companyId, 
            string password, 
            Modules currentModule)
        {
            if (!HostManager.ValidateHostAgainstAccountHostIds(currentAccount.HostnameIds, currentAddress, companyId))
            {
                var secureData = new cSecureData();
                string encCompanyId = secureData.Encrypt(companyId);
                string encUsername = secureData.Encrypt(username);
                string encPassword = secureData.Encrypt(password);
                string hostName = HostManager.GetHostName(
                    currentAccount.HostnameIds, currentModule, currentAccount.companyid);
                return "https://" + hostName + "/shared/logon.aspx?testRedirect=1&a="
                       + HttpUtility.UrlEncode(encCompanyId) + "&b=" + HttpUtility.UrlEncode(encUsername) + "&c="
                       + HttpUtility.UrlEncode(encPassword);
            }

            return string.Empty;
        }

        /// <summary>
        /// The logon user with known employee id and account id.
        /// </summary>
        /// <param name="employeeid">
        /// The employee id.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="fromSso">
        /// The from SSO.
        /// </param>
        /// <param name="rememberDetails">
        /// remember the logon details
        /// </param>
        /// <param name="module">
        /// The current active module
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string LogonUser(
            int employeeid, 
            int accountid, 
            HttpRequest request, 
            HttpSessionState session, 
            HttpResponse response, 
            bool fromSso, 
            bool rememberDetails, 
            Modules module)
        {
            this.accountId = accountid;
            this.employeeId = employeeid;
            var employees = new cEmployees(this.accountId);
            cAccount account = new cAccounts().GetAccountByID(this.accountId);
            Employee employee = employees.GetEmployeeById(this.employeeId);
            return this.LogonUser(
                employee.Username, 
                account.companyid, 
                string.Empty, 
                request, 
                session, 
                response, 
                fromSso, 
                rememberDetails, 
                module);
        }

        /// <summary>
        /// logon user.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="fromSso">
        /// The from SSO.
        /// </param>
        /// <param name="rememberDetails">
        /// Remember the logon details
        /// </param>
        /// <param name="module">
        /// The current active module
        /// </param>
        /// <returns>
        /// A message indicating the result of the logon process.
        /// </returns>
        public string LogonUser(
            string username, 
            string companyId, 
            string password, 
            HttpRequest request, 
            HttpSessionState session, 
            HttpResponse response, 
            bool fromSso, 
            bool rememberDetails, 
            Modules module)
        {
            var currentUser = cMisc.GetCurrentUser();
            if (currentUser != null)
            {
                this.RedirectAfterLogon(LoginResult.AlreadyLoggedIn, request, session, response, currentUser.CurrentActiveModule);
            }

            var accounts = new cAccounts();
            cAccount account = accounts.GetAccountByCompanyID(companyId);

            if (account == null || account.archived)
            {
                this.forgottenDetailsVisible = true;
                return "The details you have entered are incorrect.";
            }

            this.accountId = account.accountid;

            HttpContext.Current.User = new TemporaryWebPrincipal(new UserIdentity(account.accountid, 0));

            LoginResult auth = this.Authenticate(companyId.Trim(), username.Trim(), password.Trim(), request, fromSso);
            session["SubAccountID"] = this.currentSubAccount;

            // In case a user gets logged out due to session timeout, values of previously logged in subaccount id will be stored in a cookie
            if (request.Cookies["SubAccount"] != null)
            {
                var _subAccountID = request.Cookies["SubAccount"]["_ID"];
                var _username = request.Cookies["SubAccount"]["_Username"];
                if (_subAccountID != null && _username.ToLower() == username.ToLower())
                {
                    session["SubAccountID"] = Convert.ToInt32(_subAccountID);
                }
            }

            if (auth == LoginResult.Success || auth == LoginResult.ChangePassword
                || auth == LoginResult.EnterOdometerValues)
            {
                return this.SuccessfulLogin(
                    username, companyId, password, auth, request, session, response, account, rememberDetails, module, fromSso);
            }

            InvalidateCurrentCookie(request, response);

            return this.GetFailedToLogonMessage(username, module, auth, account);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find employee by using field GUID.
        ///     This works for any field where the parent table contains a column called employeeid.
        /// </summary>
        /// <param name="account">
        /// The account to search in.
        /// </param>
        /// <param name="find">
        /// The string value to search for.
        /// </param>
        /// <param name="fieldId">
        /// The GUID of the field to search in.
        /// </param>
        /// <returns>
        /// The Employee ID,
        /// </returns>
        internal int FindEmployee(cAccount account, string find, Guid fieldId)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }

            if (find == null)
            {
                throw new ArgumentNullException("find");
            }

            if (fieldId == null)
            {
                throw new ArgumentNullException("fieldId");
            }

            using (var conn = new DatabaseConnection(cAccounts.getConnectionString(account.accountid)))
            {
                conn.AddWithValue("@fieldId", fieldId);
                conn.AddWithValue("@find", find);

                using (var reader = conn.GetReader("dbo.FindEmployee", CommandType.StoredProcedure))
                {
                    int? result = null;
                    var count = 0;
                    while (reader.Read())
                    {
                        count++;
                        result = reader.GetInt32(0);
                    }
                    
                    if (count == 1)
                    {
                        return (int)result;
                    }

                    throw new InvalidOperationException("Could not log in.");
                }
            }
        }

        /// <summary>
        /// Invalidate the current cookie.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="response">
        /// The response.
        /// </param>
        private static void InvalidateCurrentCookie(HttpRequest request, HttpResponse response)
        {
            if (request.Cookies["LoginInfo"] != null)
            {
                HttpCookie loginCookie = request.Cookies["LoginInfo"];
                request.Cookies.Remove("LoginInfo");

                // Setting the below removes the cookie from the browser
                if (loginCookie != null)
                {
                    loginCookie.Expires = DateTime.Now.AddDays(-32);
                    loginCookie.Value = null;
                    response.SetCookie(loginCookie);
                }
            }
        }

        /// <summary>
        ///     The audit logon.
        /// </summary>
        private void AuditLogon()
        {
            var clsAudit = new cAuditLog();
            clsAudit.recordLogon(this.accountId, this.employeeId);
        }

        /// <summary>
        /// Authenticates the credentials and provides a friendly status.
        /// </summary>
        /// <param name="companyName">The company name.</param>
        /// <param name="username">The Username.</param>
        /// <param name="password">The Password.</param>
        /// <param name="dnsSafeHost">The dnsSafeHost.</param>
        /// <param name="userHostAddress">the user's host address.</param>
        /// <param name="fromSso">from SSO</param>
        /// <returns>A LoginResult.</returns>
        public LoginResult Authenticate(string companyName, string username, string password, string dnsSafeHost, string userHostAddress, bool fromSso)
        {   
            var clsAccounts = new cAccounts();
            cAccount reqAccount = clsAccounts.GetAccountByCompanyID(companyName);

            if (dnsSafeHost != "localhost")
            {
                var clsIpFilters = new cIPFilters(this.accountId);
                if (clsIpFilters.CheckValidIPForAccesss(userHostAddress) == false)
                {
                    return LoginResult.InvalidIPAddress;
                }
            }

            var clsEmployees = new cEmployees(reqAccount.accountid);

            AuthenicationOutcome authOutcome = clsEmployees.Authenticate(username, password, AccessRequestType.Website, this._encryptor, fromSso);
            int authenticate = authOutcome.employeeId;

            this.employeeId = Math.Abs(authenticate);

            if (authenticate == 0)
            {
                return LoginResult.InvalidUsernamePasswordCombo;
            }

            Employee reqEmployee = clsEmployees.GetEmployeeById(Math.Abs(authenticate));

            if (reqEmployee.DefaultSubAccount == -1)
            {
                return LoginResult.NoSubAccount;
            }

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (reqEmployee != null)
            {
                // ReSharper restore ConditionIsAlwaysTrueOrFalse
                this.currentSubAccount = reqEmployee.DefaultSubAccount;
            }

            HttpContext.Current.User = new TemporaryWebPrincipal(new UserIdentity(reqEmployee.AccountID, reqEmployee.EmployeeID));

            var generalOptions = FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>()[reqEmployee.DefaultSubAccount].WithPassword().WithMileage();

            if (authenticate < 0)
            {
                // negative employee ID means logon attempt failure or employee is not yet active or employee is archived.
                if (!reqEmployee.Active)
                {
                    return LoginResult.InactiveAccount;
                }

                if (reqEmployee.Archived)
                {
                    return LoginResult.EmployeeArchived;
                }

                return reqEmployee.LogonRetryCount >= generalOptions.Password.PwdMaxRetries
                           ? LoginResult.LogonAttemptsExceeded
                           : LoginResult.InvalidUsernamePasswordCombo;
                
            }

            if (reqEmployee.Locked)
            {
                return LoginResult.EmployeeLocked;
            }

            if (reqEmployee.FirstLogon)
            {
                reqEmployee.MarkFirstLogonComplete(null);
            }

            if (reqEmployee.Active == false)
            {
                return LoginResult.InactiveAccount;
            }

            if (reqEmployee.Archived)
            {
                return LoginResult.EmployeeArchived;
            }

            if (clsEmployees.CheckExpiry(reqEmployee))
            {
                return LoginResult.ChangePassword;
            }

            var employeeCars = new cEmployeeCars(this.accountId, this.employeeId);

            if (CheckOdometerReadingsRequired(employeeCars, generalOptions.Mileage.RecordOdometer, generalOptions.Mileage.EnterOdometerOnSubmit, generalOptions.Mileage.OdometerDay))
            {
                return LoginResult.EnterOdometerValues;
            } 

            return string.IsNullOrEmpty(password) && !fromSso ? LoginResult.ChangePassword : LoginResult.Success;
        }

        /// <summary>
        /// Checks if the user needs to provide odometer readings on logon
        /// </summary>
        /// <param name="employeeCars">The employee cars object</param>
        /// <param name="recordOdometer">The record odometer option from general options</param>
        /// <param name="enterOdometerOnSubmit">The enter odometer on submit option from general options</param>
        /// <param name="odometerDay">The day that odometer readings should be entered</param>
        /// <returns>If odometer readings are required on logon or not</returns>
        internal static bool CheckOdometerReadingsRequired(cEmployeeCars employeeCars, bool recordOdometer, bool enterOdometerOnSubmit, byte odometerDay)
        {
            if (recordOdometer && enterOdometerOnSubmit == false)
            {
                var ododate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, odometerDay);
                if (ododate > DateTime.Today)
                {
                    ododate = ododate.AddMonths(-1);
                }

                var cars = employeeCars.GetActiveCars().Where(e => e.fuelcard == true).ToList();

                foreach (cOdometerReading reading in cars.Select(t => t.getLastOdometerReading()))
                {
                    if (reading == null)
                    {
                        // Never had a reading
                        return true;
                    }

                    if (reading.datestamp <= ododate)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>Authenticates credentials and returns the appropriate status code.</summary>
        /// <param name="companyName">The company name.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="request">The HTTP request</param>
        /// <param name="fromSso">The from SSO.</param>
        /// <returns>The LoginResult.</returns>
        private LoginResult Authenticate(string companyName, string username, string password, HttpRequest request, bool fromSso)
        {
            return Authenticate(companyName, username, password, request.Url.DnsSafeHost, request.UserHostAddress, fromSso);
        }

        /// <summary>
        /// The check concurrent user licence limit reached.
        /// </summary>
        /// <param name="currentModule">
        /// The current module.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private Guid CheckConcurrentUserLicenceLimitReached(Modules currentModule)
        {
            Guid manageId = Guid.Empty;
            if (currentModule == Modules.contracts)
            {
                var cusers = new cConcurrentUsers(this.accountId, this.employeeId);
                manageId = cusers.LogonUser();

                if (manageId == Guid.Empty)
                {
                    // concurrent user limit reached
                    return Guid.Empty;
                }
            }

            return manageId;
        }

        /// <summary>
        /// The authenticate user.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="manageId">
        /// The manage id.
        /// </param>
        private void CreateAuthenticationInformation(HttpRequest request, HttpResponse response, Guid manageId)
        {
            string authUsername = this.accountId + "," + this.employeeId.ToString(CultureInfo.InvariantCulture);

            FormsAuthentication.SetAuthCookie(authUsername, false, FormsAuthentication.FormsCookiePath);
            HttpCookie authCookie = request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket != null)
                {
                    var ticket = new FormsAuthenticationTicket(
                        authTicket.Version, 
                        authTicket.Name, 
                        authTicket.IssueDate, 
                        authTicket.Expiration, 
                        false, 
                        manageId.ToString(), 
                        FormsAuthentication.FormsCookiePath);
                    string encTicket = FormsAuthentication.Encrypt(ticket);

                    HttpCookie httpCookie = response.Cookies[FormsAuthentication.FormsCookieName];
                    if (httpCookie != null)
                    {
                        httpCookie.Value = encTicket;
                    }
                }
            }
        }

        /// <summary>
        /// The get failed to logon message.
        /// </summary>
        /// <param name="username">
        ///     The user name.
        /// </param>
        /// <param name="module">
        ///     The module.
        /// </param>
        /// <param name="authorisationResult">
        ///     The authorisation result.
        /// </param>
        /// <param name="account">
        ///     The current account.
        /// </param>
        /// <param name="globalProperties">
        ///     global properties object
        /// </param>
        /// <param name="prop"></param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetFailedToLogonMessage(string username, Modules module, LoginResult authorisationResult, cAccount account)
        {
            var generalOptions = FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>()[this.currentSubAccount].WithPassword().WithAccountMessages();

            string message = "The details you have entered are incorrect.";
            this.forgottenDetailsVisible = true;
            switch (authorisationResult)
            {
                case LoginResult.NoSubAccount:
                    message = "Your account is incorrectly configured, contact your administrator.";
                    this.forgottenDetailsVisible = false;
                    break;
                case LoginResult.InvalidIPAddress:
                    message = "Please note that access is restricted from this location for security purposes.";
                    this.forgottenDetailsVisible = false;
                    break;
                default:
                    {
                        var clsemployees = new cEmployees(account.accountid);

                        if (authorisationResult == LoginResult.LogonAttemptsExceeded && generalOptions.Password.PwdMaxRetries != 0)
                        {
                            clsemployees.lockEmployee(username, account.accountid, module);
                            
                            message = string.IsNullOrEmpty(generalOptions.AccountMessages.AccountLockedMessage) ? "Too many attempts, your account has been locked.  Check your email for details." : generalOptions.AccountMessages.AccountLockedMessage;
                            this.forgottenDetailsVisible = false;
                        }
                        else
                        {
                            this.forgottenDetailsVisible = false;
                            switch (authorisationResult)
                            {
                                case LoginResult.EmployeeArchived:
                                    message = "Your account is currently archived, contact your administrator.";
                                    break;
                                case LoginResult.InactiveAccount:
                                    message = "Your account has not yet been activated.";
                                    break;
                                case LoginResult.InvalidUsernamePasswordCombo:
                                    message = "The details you have entered are incorrect.";
                                    this.forgottenDetailsVisible = true;
                                    break;
                                case LoginResult.EmployeeLocked:
                                    message = string.IsNullOrEmpty(generalOptions.AccountMessages.AccountCurrentlyLockedMessage) ? "Your account is currently locked, check your email for details." : generalOptions.AccountMessages.AccountCurrentlyLockedMessage;
                                    break;
                            }
                        }

                        if (authorisationResult != LoginResult.InactiveAccount)
                        {
                            if (((generalOptions.Password.PwdMaxRetries - clsemployees.GetEmployeeRetryCount(this.employeeId)) > 0)
                                && authorisationResult == LoginResult.InvalidUsernamePasswordCombo)
                            {
                                message += string.Format(
                                    "  {0} attempts left.",
                                    generalOptions.Password.PwdMaxRetries - clsemployees.GetEmployeeRetryCount(this.employeeId));
                                this.forgottenDetailsVisible = true;
                            }
                        }
                    }

                    break;
            }

            return message;
        }

        /// <summary>
        /// The redirect after successful logon.
        /// </summary>
        /// <param name="authorisedResult">
        /// The authorised result.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="session">
        /// the session object
        /// </param>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="currentModule">
        /// The current module.
        /// </param>
        private void RedirectAfterLogon(
            LoginResult authorisedResult, 
            HttpRequest request, 
            HttpSessionState session, 
            HttpResponse response, 
            Modules currentModule)
        {
            switch (authorisedResult)
            {
                case LoginResult.Success:
                    session.Remove("myid");
                    session.Remove("delegatetype");

                    // delete subaccount cookie
                    HttpCookie cookie = new HttpCookie("SubAccount");
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    response.Cookies.Add(cookie);
                    this.RedirectToHomePage(request, response, currentModule);
                    break;
                case LoginResult.ChangePassword:
                    // if the user must change password add variable to session so that he will always redirect to changepassword.aspx
                    // the employeeid is passed in the variable to be used in the global.asax for the redirect
                    session["ChangePasswordUserId"] = this.employeeId;
                    response.Redirect("~/shared/changepassword.aspx?returnto=3&employeeid=" + this.employeeId, true);
                    break;
                case LoginResult.EnterOdometerValues:
                    session["OdometerReadingsOnLogon"] = true;
                    response.Redirect("~/odometerreading.aspx?odotype=1", true);
                    break;
                case LoginResult.AlreadyLoggedIn:
                    this.RedirectToHomePage(request, response, currentModule);
                    break;
            }
        }

        /// <summary>
        /// Redirect a valid user to the home page.
        /// </summary>
        /// <param name="request">
        /// The current <see cref="HttpRequest"/>.
        /// </param>
        /// <param name="response">
        /// The current <see cref="HttpResponse"/>.
        /// </param>
        /// <param name="currentModule">
        /// The current module.
        /// </param>
        private void RedirectToHomePage(HttpRequest request, HttpResponse response, Modules currentModule)
        {
            string defaultHomePage = cModules.GetDefaultHomepageForModule(currentModule);
            if (currentModule == Modules.CorporateDiligence)
            {
                var clsSubAccounts = new cAccountSubAccounts(this.accountId);
                cAccountProperties subAccountProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties;
                if (subAccountProperties.CorporateDStartPage != string.Empty)
                {
                    defaultHomePage = subAccountProperties.CorporateDStartPage;
                }
            }

            if (request.QueryString["ReturnUrl"] == null || request.QueryString["ReturnUrl"].ToLower() == "%2f"
                                                         || request.QueryString["ReturnUrl"].Contains("errorpage"))
            {
                response.Redirect(defaultHomePage, true);
            }
            else
            {
                // check that return URL is a valid product url and not an external address
                string returnUrl = HttpUtility.UrlDecode(request.QueryString["ReturnUrl"]);
                if (returnUrl != null && (request.ApplicationPath != null && returnUrl.StartsWith(request.ApplicationPath)
                                                                          && !returnUrl.Contains("http")))
                {
                    response.Redirect(returnUrl, true);
                }
                else
                {
                    response.Redirect(defaultHomePage, true);
                }
            }
        }

        /// <summary>
        /// The store logon cookie.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <param name="authorisedResult">
        /// The authorised result.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="rememberDetails">
        /// The remember details.
        /// </param>
        /// <param name="fromSso">
        /// From single sign-on
        /// </param>
        private void StoreLogonCookie(
            string username, 
            string companyId, 
            LoginResult authorisedResult, 
            HttpRequest request, 
            HttpResponse response, 
            bool rememberDetails,
            bool fromSso)
        {
            if (authorisedResult == LoginResult.Success || authorisedResult == LoginResult.ChangePassword
                || authorisedResult == LoginResult.EnterOdometerValues)
            {
                if (rememberDetails)
                {
                    var loginCookie = new HttpCookie("LoginInfo");
                    loginCookie.Values.Add("remember", true.ToString(CultureInfo.InvariantCulture));
                    loginCookie.Values.Add("company", companyId);
                    loginCookie.Values.Add("username", username);
                    loginCookie.Expires = DateTime.Now.AddDays(31);
                    response.Cookies.Add(loginCookie);
                }
                else
                {
                    InvalidateCurrentCookie(request, response);
                }

                response.Cookies.Add(new HttpCookie("SSO", fromSso ? "1" : "0"));

            }
        }

        /// <summary>
        /// Process a successful login.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="authorisedResult">
        /// The authorised result.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="session">
        /// the session object
        /// </param>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="currentAccount">
        /// The current account.
        /// </param>
        /// <param name="rememberDetails">
        /// remember logon details
        /// </param>
        /// <param name="currentModule">
        /// The current Module.
        /// </param>
        /// <param name="fromSso">
        /// True if this login request was initiated from single sign-on, otherwise false
        /// </param>
        /// <returns>
        /// The Success/Error message
        /// </returns>
        private string SuccessfulLogin(
            string username, 
            string companyId, 
            string password, 
            LoginResult authorisedResult, 
            HttpRequest request, 
            HttpSessionState session, 
            HttpResponse response, 
            cAccount currentAccount, 
            bool rememberDetails, 
            Modules currentModule,
            bool fromSso)
        {
            this.StoreLogonCookie(username, companyId, authorisedResult, request, response, rememberDetails, fromSso);
            string address = HostManager.GetFormattedAddress(
                request.Url.AbsoluteUri, request.Url.AbsolutePath, request.Url.Query);
            string redirect = GetRedirectToHostnameAddress(
                address, currentAccount, username, companyId, password, currentModule);
            if (!string.IsNullOrEmpty(redirect))
            {
                response.Redirect(redirect, true);
            }

            Guid manageId = this.CheckConcurrentUserLicenceLimitReached(currentModule);

            if (manageId == Guid.Empty && currentModule == Modules.contracts)
            {
                return "Concurrent user licence limit reached.";
            }

            this.CreateAuthenticationInformation(request, response, manageId);

            this.AuditLogon();

            this.RedirectAfterLogon(authorisedResult, request, session, response, currentModule);

            return string.Empty;
        }

        #endregion
    }
}