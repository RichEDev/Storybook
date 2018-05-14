namespace SpendManagementApi.Attributes
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    using BusinessLogic.Identity;

    using Common;
    using Controllers;
    using Interfaces;
    using Models.Common;
    using Models.Types;
    using Utilities;

    using Spend_Management;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;

    /// <summary>
    /// Authorises and Authenticates an ActionMethod, using the request headers,
    /// coupled with Spend Management's Access Roles.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthAuditAttribute : ActionFilterAttribute
    {
// ReSharper disable once NotAccessedField.Local
        private cMisc _cMisc;

        #region Constants

        /// <summary>The Request Header Key to look for to find the Auth Token.</summary>
        public const string HeaderKeyAuthToken = "AuthToken";
        private const string LoginActionName = "Login";
        private const string LoginControllerName = "Account";

        #endregion


        #region Constructor

        /// <summary>
        /// Marks a method for Authentication and Authorisation
        /// </summary>
        /// <param name="accessRoleElement">Specified (required) Access Role element, so Authorisation can determin if the user is allowed here.</param>
        /// <param name="accessRoleType">Specified (required) Access Role type, so Authorisation can determine if the user is allowed here.</param>
        /// <param name="callsToDecrementBy">The amount of licensed calls to decrement the account by when the user calls this action.</param>
        /// <param name="ignoreAudit">Whether to log that this method was called.</param>
        public AuthAuditAttribute(SpendManagementElement accessRoleElement, AccessRoleType accessRoleType, int callsToDecrementBy = 1, bool ignoreAudit = false)
        {
            AccessRoleElement = accessRoleElement;
            AccessRoleType = accessRoleType;
            CallsToDecrement = callsToDecrementBy;
            IgnoreAudit = ignoreAudit;
        }

        #endregion


        #region Properties
        
        /// <summary>
        /// Whether to ignore the Audit part.
        /// </summary>
        protected bool IgnoreAudit { get; set; }

        /// <summary>
        /// The amount of licensed calls to decrement the account by when the client calls this action.
        /// </summary>
        protected int CallsToDecrement { get; set; }

        /// <summary>
        /// Specified (required) Access Role type, so Authorisation can determine if the user is allowed here.
        /// </summary>
        protected AccessRoleType AccessRoleType { get; set; }

        /// <summary>
        /// Specified (required) Access Role element, so Authorisation can determin if the user is allowed here.
        /// </summary>
        protected SpendManagementElement AccessRoleElement { get; set; }

        /// <summary>
        /// The licensed call monitor and auditer.
        /// </summary>
        protected ILicenseAndAudit LicenseAndAudit { get; set; }
        
        #endregion


        #region Override Methods

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            #region Error Trap

            // attempt to get the controller as a BaseApiController
            var controller = actionContext.ControllerContext.Controller as BaseApiController;

            // throw if the controller is not a BaseApiController...
            if (controller == null)
            {
                throw new NotImplementedException("The controller containing the action this is decorating is not a BaseAPIController.");
            }

            #endregion Error Trap

            #region 'Skip' Config Flag

            // skip what's below if the config says so...
            if (ConfigurationManager.AppSettings["skipAuth"] == "1")
            {
                Helper helper = new Helper();
                controller.Initialise(helper.GetDummyUser(), null);
                base.OnActionExecuting(actionContext);
                return;
            }

            #endregion 'Skip' Config Flag

            // if we got this far, then we must perform Authentication, Authorisation and Audit

            #region Authenticate

            // get the AuthToken from the header and throw if it doesn't exist
            if (!actionContext.Request.Headers.Contains(HeaderKeyAuthToken))
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, ApiResources.ResponseForbiddenNoAuthToken));
            }

            var authParts = GetAuthenticationTokenElements(actionContext);
            
            // identify the user
            var accounts = new cAccounts();
            var reqAccount = accounts.GetAccountByID(authParts.AccountId);
            
            // ditch if account isn't there
            if (reqAccount == null)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized, ApiResources.HttpStatusCodeUnauthorised));
            }
            
            // ditch if the account is archived
            //////// ReSharper disable once PossibleNullReferenceException
            if (reqAccount.archived)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized, ApiResources.HttpStatusCodeUnauthorisedArchived));
            }

            // validate token
            var provider = new ApiAuthTokenProvider();
            var result = ApiDetails.Authenticate(authParts.AccountId, authParts.ApiDetailsId, authParts.AuthToken, provider);
            if (result.Result != ApiDetails.ApiDetailsValidity.Valid)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized, ApiResources.HttpStatusCodeUnauthorised));
            }

            HttpContext.Current.User = new WebPrincipal(new UserIdentity(reqAccount.accountid, result.User.EmployeeID));

            // set the cMisc CurrentUser
            _cMisc = new cMisc(reqAccount.accountid);

            //TODO:Add authentication to Public API and store result

            #endregion Authenticate

            #region Check IP Address

            // validate that the IP isn't banned
            if (!IPValidator.Validate(actionContext.Request, reqAccount.accountid))
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbiddenInvalidIP));
            }

            #endregion Check IP Address

            #region Authorise

            // authorisation cannot be skipped.

            var mobileRequest = Helper.IsMobileRequest(actionContext.Request.Headers.UserAgent.ToString());       
            var canAccess = result.User.CheckAccessRoleApi(AccessRoleElement, AccessRoleType, mobileRequest ? AccessRequestType.Mobile : AccessRequestType.Api);
            
            if (!canAccess)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbidden));
            }

            if (result.User.Employee.Locked)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbiddenLockedPassword));
            }

            if (result.User.Employee.Archived)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeUnauthorisedArchived));
            }

            if (!result.User.Employee.Active)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ApiResources.HttpStatusCodeForbiddenInactiveAccount));
            }

            #endregion Authorise

            #region Check Amount of calls

            // check now that the account has enough tickets to access the
            LicenseAndAudit = new ApiAuditLog();
            var callStatus = LicenseAndAudit.DetermineAccessAndDecrement(reqAccount.accountid, false, mobileRequest ? 0 : CallsToDecrement, mobileRequest: mobileRequest);
            
            if (!callStatus.Allowed)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Forbidden, callStatus.FriendlySummary));
            }
            
            #endregion Check Amount of calls

            // set the user in httpcontext
            var identity = new GenericIdentity(authParts.AccountId + "," + result.User.EmployeeID);
            HttpContext.Current.User = new GenericPrincipal(identity, new string[]{});

            // we now have a valid ICurrentUser. Finally, initialise the controller
            controller.Initialise(result.User, callStatus, mobileRequest);

            // if we got this far, allow access to the action...
            base.OnActionExecuting(actionContext);
        }

        private ApiDetails.AuthTokenElements GetAuthenticationTokenElements(HttpActionContext actionContext)
        {
            var token = actionContext.Request.Headers.GetValues(HeaderKeyAuthToken).FirstOrDefault();
            if (string.IsNullOrEmpty(token))
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Forbidden, ApiResources.ResponseForbiddenNoAuthToken));
            }

            // validate the segements in the header
            var authParts = ApiDetails.GetAuthTokenElements(token);
            if (authParts.Result != ApiDetails.ApiDetailsValidity.Valid)
            {
                throw new HttpResponseException(actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized, ApiResources.HttpStatusCodeUnauthorised));
            }

            return authParts;
        }


        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (IgnoreAudit)
            {
                base.OnActionExecuted(actionExecutedContext);
                return;
            }

            if (LicenseAndAudit == null) LicenseAndAudit = new ApiAuditLog();
            var controllerName = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;

            // get the user from the controller (which is always a BaseController)
            var controller = actionExecutedContext.ActionContext.ControllerContext.Controller as BaseApiController;
            if (controller == null)
            {
                // ignore the error if the account controller is the target
                if (controllerName == LoginControllerName)
                {
                    base.OnActionExecuted(actionExecutedContext);
                }
                else
                {
                    throw new ArgumentException("The controller containing the action this is decorating is not a BaseApiController.");
                }

                return;
            }


            // grab the current user
            var user = controller.CurrentUser;

            // if we have a user (and we obviously have the controller now too), then log.
            if (user != null && actionExecutedContext.Response != null)
            {
                ApiResponse response;
                var canGetResponse = actionExecutedContext.Response.TryGetContentValue(out response);
                if (canGetResponse)
                {
                    var result = actionExecutedContext.Response.IsSuccessStatusCode && response.ResponseInformation != null
                        ? response.ResponseInformation.Status.ToString()
                        : actionExecutedContext.Response.ReasonPhrase;
                    LicenseAndAudit.RecordApiAction(user, actionExecutedContext.Request.RequestUri.AbsoluteUri, result);

                    // note: in here, instead of the above, we can probably make calls to the same audit log
                    // that the website does, but we need to have other stuff passed through and at least a 
                    // convention for which properties to shove into the audit log.
                    // for the moment just record a logon.

                    if (controllerName == LoginControllerName && actionName == LoginActionName)
                    {
                        var audit = new cAuditLog(user.AccountID, user.EmployeeID);
                        audit.recordLogon(user.AccountID, user.EmployeeID);
                    }

                    var mobileRequest = Helper.IsMobileRequest(actionExecutedContext.Request.Headers.UserAgent.ToString());       
                    var authParts = GetAuthenticationTokenElements(actionExecutedContext.ActionContext);

                    CallsToDecrement = mobileRequest ? 0 : ((IInterceptTransactionDecrement)controller).TransactionCount;

                    var callStatus = LicenseAndAudit.DetermineAccessAndDecrement(authParts.AccountId, !IPValidator.IsSelenity(actionExecutedContext.Request), CallsToDecrement, mobileRequest: mobileRequest);
                    if (!callStatus.Allowed)
                    {
                        throw new HttpResponseException(actionExecutedContext.ActionContext.Request.CreateErrorResponse(
                            HttpStatusCode.Forbidden, callStatus.FriendlySummary));
                    }

                    if (!mobileRequest && response.ResponseInformation != null)
                    {
                        response.ResponseInformation.LicensedCallStatus = callStatus.FriendlySummary;
                    }
                }
            }

            // carry on with the request flow
            base.OnActionExecuted(actionExecutedContext);
        }

        #endregion
    }
}