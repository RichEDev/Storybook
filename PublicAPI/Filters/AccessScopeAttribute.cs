namespace PublicAPI.Filters
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Web.Http;
    using System.Web.Http.Controllers;

    using BusinessLogic;
    using BusinessLogic.AccessRoles.Scopes;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Employees.AccessRoles;

    /// <summary>
    /// Validates a request to a controller action is correct; If not it will return a 400 Bad Request with validation failures.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AccessScopeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessScopeAttribute"/> class.
        /// </summary>
        /// <param name="element">The element to check access for.</param>
        /// <param name="type">The type of access to check for.</param>
        public AccessScopeAttribute(ModuleElements element, ScopeType type)
        {
            this.Element = element;
            this.Type = type;
        }

        /// <summary>
        /// Gets the element to check access for.
        /// </summary>
        private ModuleElements Element { get; }

        /// <summary>
        /// Gets the type of access to check for.
        /// </summary>
        private ScopeType Type { get; }

        /// <summary>
        /// When an action is being authorized this method checks if the requester has sufficient access to execute the requested end point.
        /// </summary>
        /// <param name="actionContext">The <see cref="HttpActionContext"/> for the current request.</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            Guard.ThrowIfNull(actionContext, nameof(actionContext));
            
            if (!this.IsAuthorized(actionContext))
            {
                this.HandleUnauthorizedRequest(actionContext);
            }

            bool grantAccess = false;

            if (actionContext.RequestContext.Principal.Identity is ClaimsIdentity identity)
            {
                Claim claim = identity.FindFirst(identity.RoleClaimType);

                if (claim != null && claim.Value.Length > 0)
                {
                    // Can't use constructor DI here as .NET effectively makes attributes singleton and we don't want 
                    // every "use" to have to pass an instance in so a direct reference to the container is used.
                    IEmployeeCombinedAccessRoles combinedAccessRoleFactory =
                        WebApiApplication.container.GetInstance<IEmployeeCombinedAccessRoles>();

                    IEmployeeAccessScope employeeAccessScope = combinedAccessRoleFactory.Get(claim.Value);

                    grantAccess = employeeAccessScope.Scopes.GrantAccess(this.Element, this.Type);
                }
            }

            if (grantAccess == false)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Forbidden");
            }
        }
    }
}