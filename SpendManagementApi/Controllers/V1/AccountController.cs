namespace SpendManagementApi.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Common;
    using Attributes;
    using Models.Common;
    using Models.Requests;
    using Models.Responses;
    using Models.Types;
    using Utilities;
    using Models;
    using System.Linq;
    using Repositories;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Employees.ElementAccess;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Holidays;
    using SpendManagementLibrary.Mobile;
    using SpendManagementLibrary.MobileAppReview;

    using Spend_Management;

    using ForgottenDetailsResponse = Models.Responses.ForgottenDetailsResponse;
    using IAccessRoleElementPermissions = Spend_Management.shared.code.AccessRoleElementPermission.Interfaces.IAccessRoleElementPermissions;

    /// <summary>
    /// Contains account specific actions.
    /// </summary>
    [RoutePrefix("Account")]
    [Version(1)]
    public class AccountV1Controller : BaseApiController
    {
        private readonly cAccounts _accounts;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AccountV1Controller()
        {
            this._accounts = new cAccounts();
        }

        /// <summary>
        /// Constructor for unit testing...
        /// </summary>
        /// <param name="accounts">A cAccounts instance, (with it's cache initialised).</param>
        //public AccountV1Controller(cAccounts accounts)
        //{
        //    this._accounts = accounts;
        //}

        /// <summary>
        /// Gets ALL of the available end points from the API.
        /// </summary>
        /// <returns>A list of available Links</returns>
        [HttpOptions]
        [Route("~/Options")]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        /// <summary>
        /// Log into the API using the same credentials you would to log into Expenses online.
        /// You will be provided with a LoginResponse. 
        /// The properties of this response should be included in the header of all subsequent requests. 
        /// </summary>
        /// <param name="request">Provide a fully populated <see cref="LoginRequest">LoginRequest</see> object.</param>
        /// <returns>A LoginResponse, containing a new AuthToken and AccountId, which should be used in the headers of all further requests.</returns>
        /// <exception cref="HttpStatusCode.Unauthorized">You are not authorised to access this API.</exception>
        /// <exception cref="HttpStatusCode.Forbidden">You might be authorised, however your user settings are set so that you are forbidden from accessing the API.</exception>
        /// <exception cref="HttpStatusCode.BadRequest">The details you supplied are incorrect.</exception>
        [HttpPost]
        [Route("Login")]
        public LoginResponse Login(LoginRequest request)
        {
            // all properties are validated, and the request is good, so look up the user
            var reqAccount = this._accounts.GetAccountByCompanyID(request.Company);
            var mobileRequest = this.IsMobileRequest();

            // ditch if account isn't there
            if (reqAccount == null)
            {
                if (mobileRequest)
                {
                    return new MobileLoginResponse { LoginResponse = (int)LoginResult.IncorrectCompanyName };
                }

                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ApiResources.HttpStatusCodeUnauthorised));
            }

            var subAccounts = new cAccountSubAccounts(reqAccount.accountid);
            int subaccountid = subAccounts.getFirstSubAccount().SubAccountID;
            cAccountProperties subAccountProperties = subAccounts.getSubAccountById(subaccountid).SubAccountProperties;

            if (mobileRequest)
            {
                var useMobileDevices = subAccounts.getSubAccountById(subaccountid).SubAccountProperties.UseMobileDevices;

                if (!useMobileDevices)
                {
                    {
                        return new MobileLoginResponse { LoginResponse = (int)LoginResult.MobileDevicesDeactivated };
                    }
                }
            }

            if (reqAccount.archived)
            {
                if (mobileRequest)
                {
                    return new MobileLoginResponse { LoginResponse = (int)LoginResult.AccountArchived };
                }

                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ApiResources.HttpStatusCodeUnauthorisedArchived));
            }

            if (!IPValidator.Validate(this.Request, reqAccount.accountid))
            {
                if (mobileRequest)
                {
                    return new MobileLoginResponse { LoginResponse = (int)LoginResult.InvalidIPAddress };
                }

                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbiddenInvalidIP));
            }

            var employees = new cEmployees(reqAccount.accountid);
            var misc = new cMisc(reqAccount.accountid);
            var reqGlobalProperties = misc.GetGlobalProperties(reqAccount.accountid);
            var authenticate = employees.Authenticate(request.Username, request.Password, mobileRequest ? AccessRequestType.Mobile : AccessRequestType.Api);

            if (authenticate.employeeId == 0)
            {
                if (mobileRequest)
                {
                    return new MobileLoginResponse { LoginResponse = (int)LoginResult.InvalidUsernamePasswordCombo };
                }

                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ApiResources.HttpStatusCodeUnauthorised));
            }

            var reqEmployee = employees.GetEmployeeById(Math.Abs(authenticate.employeeId));

            if (reqEmployee.DefaultSubAccount == -1)
            {
                if (mobileRequest)
                {
                    return new MobileLoginResponse { LoginResponse = (int)LoginResult.NoSubAccount };
                }

                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbiddenSubAccount));
            }

            if (authenticate.employeeId < 0)
            {
                // negative employee ID means logon attempt failure or employee is not yet active.
                if (!reqEmployee.Active)
                {
                    if (mobileRequest)
                    {
                        return new MobileLoginResponse { LoginResponse = (int)LoginResult.InactiveAccount };
                    }

                    throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbiddenInactiveAccount));
                }

                if (reqEmployee.Archived)
                {
                    if (mobileRequest)
                    {
                        return new MobileLoginResponse { LoginResponse = (int)SpendManagementLibrary.Enumerators.LoginResult.EmployeeArchived };
                    }

                    throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeUnauthorisedArchived));
                }

                if (reqEmployee.Locked)
                {
                    if (mobileRequest)
                    {
                        return new MobileLoginResponse { LoginResponse = (int)LoginResult.EmployeeLocked, AccountCurrentlyLockedMessage = subAccountProperties.AccountCurrentlyLockedMessage };
                    }

                    throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbiddenLockedPassword));
                }

                // if they're not already locked and they've not yet exceeded the retry count
                if ((!reqEmployee.Locked && reqEmployee.LogonRetryCount < reqGlobalProperties.attempts) || reqGlobalProperties.attempts == 0)
                {
                    if (!mobileRequest)
                    {
                        throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ApiResources.HttpStatusCodeUnauthorised));
                    }

                    MobileLoginResponse mobileLoginResponse;

                    if (authenticate.LoginResult == LoginResult.EmployeeHasInsufficientPermissions)
                    {
                        mobileLoginResponse = new MobileLoginResponse { LoginResponse = (int)LoginResult.EmployeeHasInsufficientPermissions };
                    }
                    else
                    {
                        mobileLoginResponse = new MobileLoginResponse { LoginResponse = (int)LoginResult.InvalidUsernamePasswordCombo };

                        if (reqGlobalProperties.attempts > 0)
                        {
                            mobileLoginResponse.AttemptsRemaining = reqGlobalProperties.attempts - reqEmployee.LogonRetryCount;
                        }
                    }

                    return mobileLoginResponse;
                }

                // if they've exceeded the retry count and aren't yet locked
                if (!reqEmployee.Locked)
                {
                    var moduleType = HostManager.GetModule(this.Request.RequestUri.DnsSafeHost);
                    employees.lockEmployee(request.Username, reqAccount.accountid, moduleType, fromMobile: mobileRequest);
                    reqEmployee.Locked = true;

                    if (mobileRequest)
                    {
                        return new MobileLoginResponse { LoginResponse = (int)LoginResult.LogonAttemptsExceeded, AccountLockedMessage = subAccountProperties.AccountLockedMessage };
                    }

                    throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbiddenLockedPassword));
                }
            }

            if (reqEmployee.Locked)
            {
                if (mobileRequest)
                {
                    return new MobileLoginResponse { LoginResponse = (int)LoginResult.EmployeeLocked, AccountCurrentlyLockedMessage = subAccountProperties.AccountCurrentlyLockedMessage };
                }

                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbiddenLockedPassword));
            }

            if (reqEmployee.FirstLogon)
            {
                reqEmployee.MarkFirstLogonComplete(null);
            }

            if (reqEmployee.Archived)
            {
                if (mobileRequest)
                {
                    return new MobileLoginResponse { LoginResponse = (int)LoginResult.EmployeeArchived };
                }

                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeUnauthorisedArchived));
            }

            if (!reqEmployee.Active)
            {
                if (mobileRequest)
                {
                    return new MobileLoginResponse { LoginResponse = (int)LoginResult.InactiveAccount };
                }

                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbiddenInactiveAccount));
            }

            if (employees.CheckExpiry(reqEmployee) && !mobileRequest)
            {
                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbiddenLockedPassword));
            }

            // get the current user
            var creds = reqAccount.accountid + "," + reqEmployee.EmployeeID;
            var user = cMisc.GetCurrentUser(creds);

            // check they are allowed to use the API - they can only access it if it's a mobile request and they have mobile devices on their access role, or it's a non-mobile request and they have the API element.
            if ((!user.CheckAccessRoleApi(SpendManagementElement.None, AccessRoleType.View, mobileRequest ? AccessRequestType.Mobile : AccessRequestType.Api)))
            {
                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbidden));
            }

            var details = ConstructApiDetails(user);

            // this is intentionally done separately so we can return slightly different information to mobile users        
            if (mobileRequest)
            {
                subAccountProperties = subAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
                var mobileDevices = new cMobileDevices(user.AccountID);
                var vehicleRepository = new VehicleRepository(user);
                var claims = new cClaims(user.AccountID);
                var employeeCards = new cEmployeeCorporateCards(user.AccountID);
                MobileLoginResponse response = this.GenerateMobileLoginResponse(user, reqAccount, employees, reqEmployee, details, subAccountProperties,vehicleRepository,claims, employeeCards, mobileDevices);

                return response;
            }
            else
            {
                // prepare and return a response.          
                var response = new LoginResponse
                {
                    ResponseInformation = new ApiResponseInformation
                    {
                        Status = ApiStatusCode.Success,
                        LicensedCallStatus = this.AccountCallStatus(user.AccountID).FriendlySummary
                    },
                    AuthToken = details.AuthToken,
                };

                return response;
            }
        }

     
        /// <summary>
        /// Gets the current call status of the Api for your company account. 
        /// This means the number of API calls remaining in total, and for each time threshold.
        /// </summary>
        /// <returns>A CallStatus object representing the above.</returns>
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View, 0)]
        [HttpGet, Route("CallStatus")]
        public ApiAuditLogCallResult AccountCallStatus()
        {
            return this.AccountCallStatus(this.CurrentUser.AccountID);
        }

        /// <summary>
        /// Gets a claimant's account details, when called as the claimant's approver
        /// </summary>
        /// <param name="employeeId">Employee id</param>
        /// <param name="companyId">Company id</param>
        /// <returns></returns>
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [HttpGet, Route("GetClaimantAccountDetails")]
        public GetClaimantAccountDetailResponse GetClaimantAccountDetails(int employeeId, string companyId)
        {
            var employees = new cEmployees(this.CurrentUser.AccountID);
            var reqEmployee = employees.GetEmployeeById(Math.Abs(employeeId));

            // get the current user
            var creds = this.CurrentUser.AccountID + "," + reqEmployee.EmployeeID;
            var user = cMisc.GetCurrentUser(creds);

            var employeeCards = new cEmployeeCorporateCards(user.AccountID);
            GetClaimantAccountDetailResponse response = new GetClaimantAccountDetailResponse() {HasCreditCard = employeeCards.HasCreditCard(employeeId), HasPurchaseCard = employeeCards.HasPurchaseCard(employeeId), IsNHSCustomer = user.Account.IsNHSCustomer};

            return response;
        }

        /// <summary>
        /// Given an Id of a Sub Account, returns the description.
        /// </summary>
        /// <param name="subAccountId">The Id of the sub account to get the description for.</param>
        /// <returns>A GetSubAccountNameResponse object containing the Id and Description.</returns>
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [HttpGet, Route("GetSubAccountDescription/{subAccountId:int}")]
        public GetSubAccountNameResponse GetSubAccountDescription([FromUri] int subAccountId)
        {
            var response = this.InitialiseResponse<GetSubAccountNameResponse>();
            var subAccounts = new cAccountSubAccounts(this.CurrentUser.AccountID);
            var account = subAccounts.getSubAccountById(subAccountId);
            if (account != null)
            {
                response.Description = account.Description;
                response.Id = account.SubAccountID;
                return response;
            }

            throw new InvalidDataException(ApiResources.ApiErrorWrongAccountIdMessage);
        }

        /// <summary>
        /// Gets a list of Ids of Accounts that have the Receipts Service enabled.
        /// </summary>
        /// <returns>The company id.</returns>
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, Route("GetAccountsWithReceiptServiceEnabled")]
        public GetAccountsResponse GetAccountsWithReceiptServiceEnabled()
        {
            var response = InitialiseResponse<GetAccountsResponse>();
            response.List = new cAccounts().GetAccountsWithReceiptServiceEnabled();
            return response;
        }

        /// <summary>
        /// Gets a list of Ids of Accounts that have the Validation Service enabled.
        /// </summary>
        /// <returns>The company id.</returns>
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, Route("GetAccountsWithValidationServiceEnabled")]
        public GetAccountsResponse GetAccountsWithValidationServiceEnabled()
        {
            var response = InitialiseResponse<GetAccountsResponse>();
            response.List = new cAccounts().GetAccountsWithValidationServiceEnabled();
            return response;
        }

        /// <summary>
        /// Gets the list of <see cref="GeneralOptionEnabledAccount"/> which have Approver reminders or claimant reminders enabled 
        /// </summary>
        /// <returns>A list of accounts with claim reminders enabled</returns>
        [HttpGet, Route("GetAccountsWithClaimRemindersEnabled")]
        [InternalSelenityMethod]
        [ApiExplorerSettings(IgnoreApi = true)]
        public GeneralOptionAccountsResponse GetAccountsWithClaimRemindersEnabled()
        {
            var accountList = new GeneralOptionAccountsResponse();
            var accounts = new cAccounts().GetAllAccounts().Where(a => !a.archived).ToList();

            if (accounts == null || accounts.Count == 0)
            {
                return accountList;
            }

            foreach (var account in accounts)
            {
                var subAccounts = new cAccountSubAccounts(account.accountid);
                var reqSubAccount = subAccounts.getFirstSubAccount();
                if (reqSubAccount.SubAccountProperties.EnableClaimApprovalReminders || reqSubAccount.SubAccountProperties.EnableCurrentClaimsReminders)
                {
                    accountList.AccountList.Add(new GeneralOptionEnabledAccount(account.accountid, account.companyid));
                }
            }

            return accountList;
        }

        /// <summary>
        /// Gets the company Id from the account Id.
        /// </summary>
        /// <returns>The company id.</returns>
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, Route("GetCompanyNameById/{id:int}")]
        public string GetAccountNameById([FromUri] int id)
        {
            var accounts = new cAccounts();
            var account = accounts.GetAccountByID(id);
            return account == null ? null : account.companyid;
        }

        /// <summary>
        /// Resets the daily free call limits for all accounts.
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut, Route("ResetDailyFreeCallLimits")]
        public void ResetDailyFreeCallLimits()
        {
            if (this.Request.IsLocal())
            {
                var accounts = new cAccounts();
                accounts.ResetDailyFreeCallLimits();
            }
        }

        /// <summary>
        /// Gets the password complexity that users on this account must adhere to.
        /// </summary>
        /// <returns>A <see cref="PasswordPolicy"/> detailing the required complexity of passwords in this account.</returns>
        [HttpGet]
        [Route("PasswordPolicy")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public PasswordPolicy PasswordPolicy()
        {
            var subAccounts = new cAccountSubAccounts(this.CurrentUser.AccountID);
            var subaccountid = this.CurrentUser.Employee.DefaultSubAccount >= 0
                       ? this.CurrentUser.Employee.DefaultSubAccount
                       : subAccounts.getFirstSubAccount().SubAccountID;

            PasswordPolicy response;

            if (this.MobileRequest)
            {
                response = new PasswordPolicy
                {
                    Requirements = subAccounts.getSubAccountById(subaccountid).PasswordPolicyText
                };
            }
            else
            {
                response = new PasswordPolicy
                {
                    Requirements = subAccounts.getSubAccountById(subaccountid).PasswordPolicyText,
                    ResponseInformation =
                        new ApiResponseInformation
                        {
                            Status = ApiStatusCode.Success,
                            LicensedCallStatus = this.AccountCallStatus(this.CurrentUser.AccountID).FriendlySummary
                        }
                };
            }

            return response;
        }

        /// <summary>
        /// Requests a reminder of logon credentials for the user matching the email parameter.
        /// </summary>
        /// <param name="email">
        /// The email of the user.
        /// </param>
        /// <param name="brandName">The product name against which the user is associated, e.g. Expenses</param>
        /// <returns>
        /// A <see cref="Models.Responses.ForgottenDetailsResponse"/> detailing success or failure.
        /// </returns>
        [HttpGet]
        [Route("ForgottenPassword")]
        public ForgottenDetailsResponse ForgottenDetailsReminder([FromUri]string email, [FromUri]string brandName)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ApiResources.ValidationError));
            }

            var mobileRequest = this.IsMobileRequest();        
            string requestSource = this.Request.Headers.UserAgent.ToString();
            
            var forgottenDetailsResponse = cEmployees.RequestForgottenDetails(email, HostManager.GetModule(this.Request.RequestUri.DnsSafeHost), mobileRequest, brandName, requestSource);

            var response = new ForgottenDetailsResponse { RequestResponse = (int)forgottenDetailsResponse };

            return response;
        }

        /// <summary>
        /// Allows a user to change their password.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// A <see cref="ChangePasswordResponse">ChangePasswordResponse</see> detailing success or failure.
        /// </returns>
        [HttpPost]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        [Route("ChangePassword")]
        public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
        {
            /* Internal remarks:
             This method will only allow a password change in these scenarios:
             1. The request contains a validated reset key - in which case request.OldPassword is not required because the user has forgotten it and has 
             verified they are the right person by clicking the link in an email
             2. The employee's password has expired - in which case request.OldPassword is not required because we're handling the redirection after a successful login
             3. Both the request.NewPassword & request.OldPassword properties have been supplied and the old password matches with their employee record.
            */

            if (request == null || string.IsNullOrEmpty(request.NewPassword))
            {
                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ApiResources.ValidationError));
            }

            var employees = new cEmployees(this.CurrentUser.AccountID);
            var employee = this.CurrentUser.Employee;
            var subaccs = new cAccountSubAccounts(this.CurrentUser.AccountID);
            var subaccountProperties = this.CurrentUser.Employee.DefaultSubAccount >= 0 ? subaccs.getSubAccountById(this.CurrentUser.Employee.DefaultSubAccount).SubAccountProperties : subaccs.getFirstSubAccount().SubAccountProperties;

            var oldpass = string.IsNullOrEmpty(request.OldPassword) ? string.Empty : request.OldPassword;

            var passwordHasExpired = employee.CheckPasswordExpiry(subaccountProperties);
            var requestContainsResetKey = !string.IsNullOrEmpty(request.ResetKey);
            var requestContainsBothParts = !string.IsNullOrEmpty(request.OldPassword) && !string.IsNullOrEmpty(request.NewPassword);
            var resetKeyIsValid = false;

            if (requestContainsResetKey)
            {
                var returnTuple = cMisc.MatchPasswordKey(request.ResetKey);
                resetKeyIsValid = (returnTuple != null) && (returnTuple.Item2 != null);
            }

            if (!passwordHasExpired && (!requestContainsResetKey || !resetKeyIsValid) && !requestContainsBothParts)
            {
                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ApiResources.HttpStatusCodeUnauthorised));
            }

            var checkOldPassword = !requestContainsResetKey && !passwordHasExpired;

            var checkpwd = employees.checkpassword(request.NewPassword, this.CurrentUser.AccountID, this.CurrentUser.EmployeeID); // checks to make sure the password meets the complexity on the account
            var returncode = employee.ChangePassword(oldpass, request.NewPassword, checkOldPassword, checkpwd, subaccountProperties.PwdHistoryNum, this.CurrentUser); // 0 on success, 1 if the old password doesn't match, 2 if it doesn't conform to the account password policy.

            if (returncode == 0 && requestContainsResetKey)
            {
                employees.RemovePasswordKey(request.ResetKey);
            }

            ChangePasswordResponse response;

            if (this.MobileRequest)
            {
                response = new ChangePasswordResponse { RequestResponse = returncode };
            }
            else
            {
                response = new ChangePasswordResponse
                {
                    RequestResponse = returncode,
                    ResponseInformation = new ApiResponseInformation
                    {
                        Status = ApiStatusCode.Success,
                        LicensedCallStatus = this.AccountCallStatus(this.CurrentUser.AccountID).FriendlySummary
                    }
                };
            }

            return response;
        }

        /// <summary>
        /// Validates an employee's password reset key.
        /// </summary>
        /// <param name="request">
        /// A validly formed <see cref="ResetKeyRequest"/>
        /// </param>
        /// <returns>
        /// A LoginResponse, containing a new AuthToken which should be used in the headers of all further requests.
        /// </returns>
        [HttpPost]
        [Route("ValidateResetKey")]
        public LoginResponse ValidateResetKey(ResetKeyRequest request)
        {
            if (request == null)
            {
                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ApiResources.ValidationError));
            }

            var match = cMisc.MatchPasswordKey(request.ResetKey);

            if (match == null || match.Item2 == null)
            {
                if (this.IsMobileRequest())
                {
                    return new MobileLoginResponse { LoginResponse = (int)SpendManagementLibrary.Enumerators.LoginResult.PasswordResetKeyInvalid };
                }
               
                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ApiResources.HttpStatusCodeUnauthorised));
            }

            var employee = match.Item2;
            var accountId = match.Item1;
            employee.MarkFirstLogonComplete(null);

            var creds = accountId + "," + employee.EmployeeID;
            var currentUser = cMisc.GetCurrentUser(creds);

            // check they are allowed to use the API - they can only access it if it's a mobile request and they have mobile devices on their access role, or it's a non-mobile request and they have the API element.
            if ((!currentUser.CheckAccessRoleApi(SpendManagementElement.None, AccessRoleType.View, this.IsMobileRequest() ? AccessRequestType.Mobile : AccessRequestType.Api) ))
            {
                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbidden));
            }

            var details = ConstructApiDetails(currentUser);

            if (this.IsMobileRequest())
            {
                var reqAccount = this._accounts.GetAccountByID(accountId);
                var employees = new cEmployees(reqAccount.accountid);
                var reqEmployee = employees.GetEmployeeById(Math.Abs(employee.EmployeeID));
                var subAccounts = new cAccountSubAccounts(currentUser.AccountID);
                var subAccountProperties = subAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
                var mobileDevices = new cMobileDevices(currentUser.AccountID);
                var vehicleRepository = new VehicleRepository(currentUser);
                var claims = new cClaims(currentUser.AccountID);
                var employeeCards = new cEmployeeCorporateCards(currentUser.AccountID);

                MobileLoginResponse response = this.GenerateMobileLoginResponse(currentUser, reqAccount, employees, reqEmployee, details, subAccountProperties, vehicleRepository, claims, employeeCards, mobileDevices);

                return response;
            }

            return new LoginResponse
            {
                ResponseInformation =
                    new ApiResponseInformation
                    {
                        Status = ApiStatusCode.Success,
                        LicensedCallStatus = this.AccountCallStatus(this.CurrentUser.AccountID).FriendlySummary
                    },
                AuthToken = details.AuthToken
            };
        }

        /// <summary>
        /// Gets the session timeout settings
        /// </summary>
        /// <returns>A <see cref="SessionTimeoutSettingsResponse"/> detailing session expiry and timeout warning periods.</returns>
        [HttpGet]
        [Route("SessionTimeoutSettings")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public SessionTimeoutSettingsResponse SessionTimeoutSettings()
        {
            var subAccounts = new cAccountSubAccounts(this.CurrentUser.AccountID);
            var subaccountid = this.CurrentUser.Employee.DefaultSubAccount >= 0
                       ? this.CurrentUser.Employee.DefaultSubAccount
                       : subAccounts.getFirstSubAccount().SubAccountID;
            var subAccountProperties = subAccounts.getSubAccountById(subaccountid).SubAccountProperties;


            var response = new SessionTimeoutSettingsResponse
            {
                IdleTimeoutPeriod = subAccountProperties.IdleTimeout,
                IdleTimeoutWarningPeriod = subAccountProperties.CountdownTimer
            };

            if (!this.IsMobileRequest())
            {
                response.ResponseInformation = new ApiResponseInformation
                {
                    Status = ApiStatusCode.Success,
                    LicensedCallStatus = this.AccountCallStatus(this.CurrentUser.AccountID).FriendlySummary
                };
            }

            return response;
        }

        /// <summary>
        /// Gets the Expedite HTML guidelines.
        /// </summary>
        /// <returns>
        /// The <see cref="StringResponse">A string response with the HTML guidelines</see>.
        /// </returns>
        [HttpGet]
        [Route("GetExpediteGuidelines")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public StringResponse GetExpediteGuidelines()
        {
            string infoUrl = GlobalVariables.GetAppSetting("EnvelopeAttachmentInstructionsUrl");

            //build up path to guidline text
            var directoryInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/")).Parent;

            if (directoryInfo != null)
            {
                var textpath = string.Format("{0}/expenses{1}", directoryInfo.FullName, infoUrl);
                WebClient wc = new WebClient();
                string guidelineText = wc.DownloadString(textpath);

                var response = new StringResponse { Value = guidelineText };
                return response;
            }

            return null;
        }
        
        /// <summary>
        /// Constructs a <see cref="ApiDetails" /> including its containing authentication token.
        /// </summary>
        /// <param name="user">
        /// The <see cref="ICurrentUser"/>
        /// </param>
        /// <returns>
        /// The <see cref="ApiDetails"/>.
        /// </returns>
        private static ApiDetails ConstructApiDetails(ICurrentUser user)
        {
            // get the User's ApiDetails (either way requires an auth)
            var details = ApiDetails.Get(user);
            var auth = new ApiAuthTokenProvider();

            // create a new token if the user doesn't have one, recreate if the expiry time has been exceeded, or return the existing valid token with an updated expiry time
            if (details == null)
            {
                details = ApiDetails.Create(user, auth);
            }
            else if (details.ExpiryTime < DateTime.Now)
            {
                details.RecreateAuthToken(auth);
            }
            else
            {
                details.UpdateExpiryTime();
            }

            return details;
        }

        /// <summary>
        /// Whether or not the request to this controller came from one of our mobile applications. We cannot use the property of <see cref="BaseApiController"/> in all of these methods because they don't all have the <see cref="AuthAuditAttribute"/>.
        /// </summary>
        /// <returns>
        /// True if the request originated from mobile, false otherwise.
        /// </returns>
        private bool IsMobileRequest()
        {
            return Helper.IsMobileRequest(this.Request.Headers.UserAgent.ToString());
        }

        /// <summary>
        /// Necessary for BaseApiController, even if it does nothing.
        /// </summary>
        protected override void Init() { }


        /// <summary>
        /// Gets the current call status for a given account.
        /// </summary>
        /// <param name="accountId">The Id of the account to get.</param>
        /// <returns>An ApiAuditLogCallResult</returns>
        private ApiAuditLogCallResult AccountCallStatus(int accountId)
        {
            var callAudit = new ApiAuditLog();
            return callAudit.DetermineAccessAndDecrement(accountId, true, 0, null, false);
        }


        /// <summary>
        /// Gets a list of Ids of Accounts that have the Payment Service enabled.
        /// </summary>
        /// <returns>List of company ids.</returns>
        [InternalSelenityMethod, ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, Route("GetAccountsWithPaymentServiceEnabled")]
        public GetAccountsResponse GetAccountsWithPaymentServiceEnabled()
        {
            var response = InitialiseResponse<GetAccountsResponse>();
            response.List = new cAccounts().GetAccountsWithPaymentServiceEnabled();
            return response;
        }

        /// <summary>
        /// Get accounts with auto populate of exchange rates general option enabled 
        /// </summary>
        /// <returns> Account details</returns>
        [HttpGet, Route("GetAccountsWithExchangeRatesUpdateEnabled")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [InternalSelenityMethod]
        public GeneralOptionAccountsResponse GetAccountsWithExchangeRatesUpdateEnabled()
        {
            var accountList = new GeneralOptionAccountsResponse();
            var accounts = new cAccounts().GetAllAccounts().Where(a => !a.archived).ToList();

            if (accounts == null || accounts.Count == 0)
            {
                return accountList;
            }

            foreach (var account in accounts)
            {
                var subAccounts = new cAccountSubAccounts(account.accountid);
                var reqSubAccount = subAccounts.getFirstSubAccount();
                if (reqSubAccount.SubAccountProperties.EnableAutoUpdateOfExchangeRates)
                {
                    accountList.AccountList.Add(new GeneralOptionEnabledAccount(account.accountid, account.companyid, reqSubAccount.SubAccountProperties.ExchangeRateProvider));
                }
            }

            return accountList;
        }

        /// <summary>
        /// Checks whether the account permits employees to notify the admin of changes to their details
        /// </summary>
        /// <param name="subAccountProperties">The subaccount properties</param>
        /// <returns>Whether the account allows employees to notify admin of changes to their details</returns>
        private bool AccountAllowsForNotifyingAdminOfChanges(cAccountProperties subAccountProperties)
        {
            return subAccountProperties.MainAdministrator != 0 && subAccountProperties.EmailServerAddress != string.Empty
                   && subAccountProperties.AllowEmployeeToNotifyOfChangeOfDetails;
        }

        /// <summary>
        /// Generates the mobile login response.
        /// </summary>
        /// <param name="user">
        /// An instances of <see cref="ICurrentUser"/> user.
        /// </param>
        /// <param name="reqAccount">
        /// An instances of <see cref="cAccount"/> user.
        /// </param>
        /// <param name="employees">
        /// An instances of <see cref="cEmployees"/> user.
        /// </param>
        /// <param name="reqEmployee">
        /// An instances of <see cref="Employee"/> user.
        /// </param>
        /// <param name="details">
        /// An instances of <see cref="ApiDetails"/> user.
        /// </param>
        /// <param name="subAccountProperties">
        /// An instances of <see cref="subAccountProperties"/> user.
        /// </param>
        /// <param name="vehicleRepository">
        /// An instances of <see cref="VehicleRepository"/> user.
        /// </param>
        /// <param name="claims">
        /// An instances of <see cref="cClaims"/> user.
        /// </param>
        /// <param name="employeeCards">
        /// An instances of <see cref="cEmployeeCorporateCards"/> user.
        /// </param>
        /// <param name="mobileDevices">
        /// An instances of <see cref="cMobileDevices"/> user.
        /// </param>
        /// <returns>
        /// The <see cref="MobileLoginResponse"/>.
        /// </returns>
        private MobileLoginResponse GenerateMobileLoginResponse(ICurrentUser user, cAccount reqAccount, cEmployees employees, Employee reqEmployee, ApiDetails details, cAccountProperties subAccountProperties, VehicleRepository vehicleRepository, cClaims claims, cEmployeeCorporateCards employeeCards, cMobileDevices mobileDevices)
        {
            bool canAccessCheckAndPay = (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CheckAndPay, true));        
            int numberOfClaimsAwaitingApproval = claims.getClaimsToCheckCount(user.EmployeeID, false, null);    
            bool allowMultipleStepJourneys = subAccountProperties.AllowMultipleDestinations;
            bool mandatoryPostcodeForAddresses = subAccountProperties.MandatoryPostcodeForAddresses;
            bool hasActiveJourneys = mobileDevices.EmployeeActiveJourneysCount(user.EmployeeID) > 0;
            bool hasCreditCard = employeeCards.HasCreditCard(user.EmployeeID);
            bool hasPurchaseCard = employeeCards.HasPurchaseCard(user.EmployeeID);
            bool bankAccountRequiredForExpense = user.MustHaveBankAccount;
            bool canAddManualAddres = subAccountProperties.AddLocations;
            bool isAddressNameMandatory = subAccountProperties.ForceAddressNameEntry;
            bool isReceiptServiceEnabled = user.Account.ReceiptServiceEnabled;
            bool isValidationServiceEnabled = user.Account.ValidationServiceEnabled;
            bool whetherExcludeExpiredVehicles = subAccountProperties.DisableCarOutsideOfStartEndDate;
            bool canNotifyAdminOfChanges = this.AccountAllowsForNotifyingAdminOfChanges(subAccountProperties);
            bool canAddVehicle = subAccountProperties.AllowUsersToAddCars;
            bool canAddVehicleJourneyRates = subAccountProperties.ShowMileageCatsForUsers;
            bool canSpecifyStartDate = subAccountProperties.AllowEmpToSpecifyCarStartDateOnAdd;
            bool startDateMandatoryForVehicle = subAccountProperties.EmpToSpecifyCarStartDateOnAddMandatory;
            bool activateCarOnUserAdd = subAccountProperties.ActivateCarOnUserAdd;
            bool showFullHomeAddressOnClaims = subAccountProperties.ShowFullHomeAddressOnClaims;
            string homeAddressKeyword = subAccountProperties.HomeAddressKeyword;
            int activeVehicleCount =
                vehicleRepository.EmployeeActiveVehicleCount(subAccountProperties.DisableCarOutsideOfStartEndDate, DateTime.Now);
            string pcaKey = reqAccount.PostcodeAnywhereKey;
            bool useDateOfExpenseForDutyOfCareChecks = subAccountProperties.UseDateOfExpenseForDutyOfCareChecks;

            var connection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID));
            var holidays = new Holidays(connection);
            bool showHolidays = holidays.DoesEmployeeHaveAccessToHolidays(user.EmployeeID);
            bool esrAssignmentRequriedForExpense = subAccountProperties.CheckESRAssignmentOnEmployeeAdd;
            bool showMyAdvances = user.Employee.AdvancesSignOffGroup != 0 && user.Account.AdvancesEnabled;
            bool isMapsEnabled = user.Account.MapsEnabled;
            bool showAdvances = user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Advances, true) && user.Account.AdvancesEnabled;
            string accountLockedMessage = subAccountProperties.AccountLockedMessage;
            string accountCurrentlyLockedMessage = subAccountProperties.AccountCurrentlyLockedMessage;
            bool canAskForReviews = new EmployeeAppReviewPreference().PermittedToAskEmployeeForReview(user.EmployeeID, user.AccountID);
      

            IAccessRoleElementPermissions bankAccountElementPermissions = DetermineElementAccess.SetElementPermissions(new ElementAccessPermissions(), user, SpendManagementElement.BankAccounts);

            CostcodeBreakdownSettings costcodeBreakdownSettings = new CostcodeBreakdownSettings
            {
                CostCodesOn = subAccountProperties.CostCodesOn,
                UseCostCodeDescription = subAccountProperties.UseCostCodeDescription,
                UseCostCodes = subAccountProperties.UseCostCodes,
                UseCostCodeOnGenDetails = subAccountProperties.UseCostCodeOnGenDetails,
                ProjectCodesOn = subAccountProperties.ProjectCodesOn,
                UseProjectCodeDescription = subAccountProperties.UseProjectCodeDescription,
                UseProjectCodes = subAccountProperties.UseProjectCodes,
                UseProjectCodeOnGenDetails = subAccountProperties.UseProjectCodeOnGenDetails,
                DepartmentsOn = subAccountProperties.DepartmentsOn,
                UseDepartmentCodeDescription = subAccountProperties.UseDepartmentCodeDescription,
                UseDepartmentCodes = subAccountProperties.UseDepartmentCodes,
                UseDeptOnGenDetails = subAccountProperties.UseDeptOnGenDetails,
                UseDefaultAllocation = subAccountProperties.AutoAssignAllocation
            };

            var response = new MobileLoginResponse
            {
                AuthToken = details.AuthToken,
                LoginResponse = employees.CheckExpiry(reqEmployee)
                                           ? (int)LoginResult.ChangePassword
                                           : (int)LoginResult.Success,
                CanAccessCheckAndPay = canAccessCheckAndPay,
                NumberOfClaimsAwaitingApproval = numberOfClaimsAwaitingApproval,
                AllowMultipleStepJourneys = allowMultipleStepJourneys,
                MandatoryPostcodeForAddresses = mandatoryPostcodeForAddresses,
                HasActiveJourneys = hasActiveJourneys,
                HasElectronicDeclaration = subAccountProperties.ClaimantDeclaration,
                ApproveDeclarationMessage = subAccountProperties.ApproverDeclarationMsg,
                ClaimantDeclarationMessage = subAccountProperties.DeclarationMsg,
                SingleClaimOnly = subAccountProperties.SingleClaim,
                AllowReceiptsForExpenseItems = subAccountProperties.AttachReceipts,
                HasCreditCard = hasCreditCard,
                HasPurchaseCard = hasPurchaseCard,
                BankAccountRequiredForExpense = bankAccountRequiredForExpense,
                BankAccountElementAccessPermissions = bankAccountElementPermissions,
                AllowClaimantsToAddManualAddresses = canAddManualAddres,
                IsAddressNameMandatory = isAddressNameMandatory,
                ReceiptServiceEnabled = isReceiptServiceEnabled,
                ValidationServiceEnabled = isValidationServiceEnabled,
                WhetherExcludeExpiredVehicles = whetherExcludeExpiredVehicles,
                IsHomeToOfficeEnabled = subAccountProperties.HomeToOffice,
                UserMayEditDetails = subAccountProperties.EditMyDetails,
                CostcodeBreakdownSettings = costcodeBreakdownSettings,
                CanNotifyAdminOfChanges = canNotifyAdminOfChanges,
                IsNHSCustomer = user.Account.IsNHSCustomer,
                UserMayAddVehicles = canAddVehicle,
                UserMaySelectVehicleRates = canAddVehicleJourneyRates,
                UserCanSpecifyVehicleStartDate = canSpecifyStartDate,
                StartDateMandatoryWhenAddingVehicle = startDateMandatoryForVehicle,
                ActivateCarOnUserAdd = activateCarOnUserAdd,
                ShowFullHomeAddressOnClaims = showFullHomeAddressOnClaims,
                HomeAddressKeyword = homeAddressKeyword,
                ActiveVehicleCount = activeVehicleCount,
                PostCodeAnywhereKey = pcaKey,
                UseDateOfExpenseForDutyOfCareChecks = useDateOfExpenseForDutyOfCareChecks,
                ShowHolidays = showHolidays,
                EsrAssignmentRequriedForExpense = esrAssignmentRequriedForExpense,
                ShowMyAdvances = showMyAdvances,
                IsMapsEnabled = isMapsEnabled,
                ShowAdvances = showAdvances,
                AccountLockedMessage = accountLockedMessage,
                AccountCurrentlyLockedMessage = accountCurrentlyLockedMessage,
                CanAskForReviews = canAskForReviews
            };

            return response;
        }
    }
}