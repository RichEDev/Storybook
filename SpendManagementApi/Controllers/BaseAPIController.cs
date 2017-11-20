namespace SpendManagementApi.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Http;
    using System.Collections.Concurrent;
    using System.Configuration;
    using System.Net;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Http.Controllers;
    using System.Web.Http.Description;

    using Spend_Management;

    using Attributes;
    using Common;
    using Common.Enums;
    using Interfaces;
    using Models.Common;
    using Models.Types;
    using Repositories;
    using Utilities;

    /// <summary>
    /// Base Api Controller, with an archivable generic type.
    /// </summary>
    public class ArchivingApiController<T> : BaseApiController<T> where T : ArchivableBaseExternalType
    {
        /// <summary>
        /// Archives or UnArchives this archivable item.
        /// </summary>
        /// <param name="id">The id of the object to archive.</param>
        /// <param name="doArchive">Whether to set the Archived property to true or false</param>
        /// <typeparam name="TResponse">The type of response (IApiResponse)</typeparam>
        /// <returns>The response of Type T</returns>
        protected TResponse Archive<TResponse>(int id, bool doArchive) where TResponse : ApiResponse<T>, new()
        {
            var response = InitialiseResponse<TResponse>();
            response.Item = ((IArchivingRepository<T>)Repository).Archive(id, doArchive);
            return response;
        }
    }




    /// <summary>
    /// Base Api Controller with a generic type. All controllers requiring a repository must inherit from this class
    /// </summary>
    /// <typeparam name="T">Type for the repository being instantiated</typeparam>
    public class BaseApiController<T> : BaseApiController where T : BaseExternalType
    {
        /// <summary>
        /// Gets or sets the repository used by the controller
        /// </summary>
        protected IRepository<T> Repository { get; set; }

        /// <summary>
        /// Intialises the repository
        /// </summary>
        protected override void Init()
        {
            var actionContext = new ActionContext(CurrentUser);
            Repository = RepositoryFactory.GetRepository<T>(new object[] { CurrentUser, actionContext });
        }

        /// <summary>
        /// Gets all objects of type TResponse.
        /// </summary>
        /// <typeparam name="TResponse">The type, which must be of type ApiResponse, and IGetResponse{T}</typeparam>
        /// <returns>The returned object of type TResponse.</returns>
        protected internal TResponse GetAll<TResponse>()
            where TResponse : GetApiResponse<T>, new()
        {
            var response = InitialiseResponse<TResponse>();
            response.List = Repository.GetAll().ToList();
            return response;
        }

        /// <summary>
        /// Gets an object of type TResponse, with the given Id.
        /// </summary>
        /// <param name="id">The Id of the object to get.</param>
        /// <typeparam name="TResponse">The type, which must be of type ApiResponse, and IResponse{T}</typeparam>
        /// <returns>The returned object of type TResponse.</returns>
        protected internal TResponse Get<TResponse>(int id)
            where TResponse : ApiResponse<T>, new()
        {
            var response = InitialiseResponse<TResponse>();
            response.Item = Repository.Get(id);
            return response;
        }

        /// <summary>
        /// Adds an item of type TResponse to the repository.
        /// </summary>
        /// <typeparam name="TResponse">The type, which must be of type ApiResponse, and IResponse{T}</typeparam>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        protected internal TResponse Put<TResponse>(T request) where TResponse : ApiResponse<T>, new()
        {
            var response = InitialiseResponse<TResponse>();
            response.Item = Repository.Update(request);

            if (response.Item == null)
            {
                SetErrorResponse(response, ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorSaveUnsuccessful);
            }
            return response;
        }

        /// <summary>
        /// Deletes an object by it's ID.
        /// </summary>
        /// <param name="id">The id of the object to delete.</param>
        /// <typeparam name="TResponse">The type, which must be of type ApiResponse, and IResponse{T}</typeparam>
        /// <returns>The object of type TResponse.</returns>
        protected internal TResponse Delete<TResponse>(int id)
            where TResponse : ApiResponse, IResponse<T>, new()
        {
            var response = InitialiseResponse<TResponse>();
            Repository.Delete(id);
            return response;
        }

        /// <summary>
        /// Updates an item of type T, and respondes with the type of TResponse.
        /// </summary>
        /// <param name="request">The request</param>
        /// <typeparam name="T">The type, which must be of type ApiRequest</typeparam>
        /// <typeparam name="TResponse">The type, which must be of type ApiResponse, and IResponse{T}</typeparam>
        /// <returns>An object of type TResponse.</returns>
        public TResponse Post<TResponse>(T request) where TResponse : ApiResponse<T>, new()
        {
            var response = InitialiseResponse<TResponse>();
            response.Item = Repository.Add(request);

            if (response.Item == null)
            {
                SetErrorResponse(response, ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorSaveUnsuccessful);
            }

            return response;
        }

        /// <summary>
        /// Returns the typed set of links for this resource.
        /// </summary>
        /// <returns>A list of Links</returns>
        protected internal virtual List<Link> Links()
        {
            return Options<T>();
        }

    }

    /// <summary>
    /// The BaseApiController, which is the subtype for all controllers in this API.
    /// It contains a <see cref="CurrentUser">CurrentUser</see> property and an
    /// <see cref="Initialise">Initialise</see> method. This will be called by the
    /// <see cref="AuthAuditAttribute">AuthAuditAttribute</see> when the current user has be discovered.
    /// </summary>
    public abstract class BaseApiController : ApiController, IInterceptTransactionDecrement
    {

        /// <summary>
        /// Gets the ApiExplorer Utility, since every request needs to look up available routes.
        /// </summary>
        private static ApiExplorer _apiExplorer;

        /// <summary>
        /// Gets the ApiExplorer Utility, since every request needs to look up available routes.
        /// </summary>
        private static ConcurrentDictionary<string, string> _apiTitleLookups;

        /// <summary>
        /// Gets the ApiExplorer Utility, since every request needs to look up available routes.
        /// </summary>
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, List<Link>>> _cachedLinks;

        /// <summary>
        /// Gets the Current User. This will be set during the <see cref="Initialise">Initialise</see> call.
        /// </summary>
        public ICurrentUser CurrentUser { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the request originated from one of our mobile applications.
        /// </summary>
        public bool MobileRequest { get; private set; }

        /// <summary>
        /// Gets the current call status of the account. This describes how many calls are left for the account.
        /// </summary>
        public ApiAuditLogCallResult CallStatus { get; private set; }

        /// <summary>
        /// Must be overridden. The current user is passed in here once an actionFilter has determined who the current user is.
        /// </summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="callStatus">The call status according to the Api Licensed Call Log...</param>
        /// <param name="mobileRequest">Specifies whether this is from a mobile request.</param>
        internal virtual void Initialise(ICurrentUser currentUser, ApiAuditLogCallResult callStatus, bool mobileRequest = false)
        {
            CallStatus = callStatus;
            CurrentUser = currentUser;
            MobileRequest = mobileRequest;
            Init();
        }

        /// <summary>
        /// Method to be implemented by derived class to initialise repository
        /// </summary>
        protected abstract void Init();

        /// <summary>
        /// Sets the error response
        /// </summary>
        /// <param name="response">Response Object</param>
        /// <param name="code">Error Code</param>
        /// <param name="description">Error Description</param>
        protected void SetErrorResponse(ApiResponse response, string code, string description)
        {
            var newError = new ApiErrorDetail
            {
                ErrorCode = code,
                Message = description
            };
            if (response.ResponseInformation == null)
            {
                response.ResponseInformation = new ApiResponseInformation
                {
                    Status = ApiStatusCode.Failure,
                    Errors = new List<ApiErrorDetail> { newError },
                    LicensedCallStatus = CallStatus.FriendlySummary
                };
                return;
            }

            response.ResponseInformation.Status = ApiStatusCode.Failure;
            response.ResponseInformation.Errors.Add(newError);


            if (CallStatus != null)
            {
                response.ResponseInformation.LicensedCallStatus = CallStatus.FriendlySummary;
            }
        }

        /// <summary>
        /// Initialises the response object
        /// </summary>
        /// <typeparam name="TResult">Response Type</typeparam>
        /// <returns>Initialises response</returns>
        protected TResult InitialiseResponse<TResult>([CallerMemberName] string callerName = null) where TResult : ApiResponse, new()
        {
            var result = new TResult
                {
                    ResponseInformation = new ApiResponseInformation
                    {
                        Status = ApiStatusCode.Success,
                        Errors = new List<ApiErrorDetail>(),
                        LicensedCallStatus = CallStatus == null ? "" : CallStatus.FriendlySummary
                    }
                };

            result.ResponseInformation.Links = (ConfigurationManager.AppSettings["IncludeResponseOptions"] == "1")
                ? Links(callerName)
                : new List<Link>();

            return result;
        }

        /// <summary>
        /// Initialises the response object
        /// </summary>
        /// <returns>Initialises response</returns>
        protected ApiResponse InitialiseResponse([CallerMemberName] string callerName = null)
        {
            var result = new ApiResponse
            {
                ResponseInformation = new ApiResponseInformation
                {
                    Status = ApiStatusCode.Success,
                    Errors = new List<ApiErrorDetail>(),
                    LicensedCallStatus = CallStatus == null ? "" : CallStatus.FriendlySummary
                }
            };

            result.ResponseInformation.Links = (ConfigurationManager.AppSettings["IncludeResponseOptions"] == "1")
                ? Links(callerName)
                : new List<Link>();

            return result;
        }

        /// <summary>
        /// Runs the given expression filters
        /// </summary>
        /// <typeparam name="T">Type of element</typeparam>
        /// <param name="masterQuery">Master query</param>
        /// <param name="findRequest">Find request</param>
        /// <param name="conditions">Filter conditions</param>
        /// <returns>Filtered list</returns>
        protected List<T> RunFindQuery<T>(IQueryable<T> masterQuery, FindRequest findRequest, List<Expression<Func<T, bool>>> conditions)
        {
            if (conditions.Count > 0)
            {
                IQueryable<T> countryQuery = masterQuery;
                Expression<Func<T, bool>> predicate;
                if (findRequest.SearchOperator == SearchOperator.Or)
                {
                    predicate = PredicateBuilder.False<T>();
                    conditions.ForEach(c => predicate = predicate.Or(c));
                }
                else
                {
                    predicate = PredicateBuilder.True<T>();
                    conditions.ForEach(c => predicate = predicate.And(c));
                }

                return countryQuery.Where(predicate).ToList();
            }
            return null;
        }

        /// <summary>
        /// Gets a list of applicable links. For the current controller and action
        /// Special case for the actionName == Links && controllerName == Home.
        /// </summary>
        /// <returns>A List of links</returns>
        protected List<Link> Options<T>([CallerMemberName] string actionName = "Options")
        {
            // get the current controller name
            var controllerDescriptor = ControllerContext.ControllerDescriptor;
            var controllerName = controllerDescriptor.ControllerName;

            // get the cached links, initialising if not already so.
            var cachedLinks = EnsureCachedDataInitialisedAndReturnCachedActions(Configuration, controllerName, actionName);
            if (cachedLinks != null) return cachedLinks;

            // no cached version... create some...
            var applicableControllers = new List<HttpControllerDescriptor>();
            var availableControllers = _apiExplorer.ApiDescriptions
                .Select(apiDesc => apiDesc.ActionDescriptor.ControllerDescriptor).ToList();

            // just add in the current controller.
            applicableControllers.Add(controllerDescriptor);

            // now check the BaseExternalType for this controller instance, 
            // in case it has any complex child BaseExternalTypes, which could be linked.
            var propertyInfos = (typeof(T)).GetProperties();
            foreach (var type in propertyInfos)
            {
                var isGenericCollection = type.PropertyType.IsGenericType;
                var genericArguments = isGenericCollection ? type.PropertyType.GenericTypeArguments : new Type[] { };
                var baseExternalType = genericArguments.FirstOrDefault(x => x.IsSubclassOf(typeof(BaseExternalType)));
                if (baseExternalType == null && type.PropertyType.IsAssignableFrom(typeof(BaseExternalType)))
                {
                    baseExternalType = type.PropertyType;
                }
                if (baseExternalType == null) continue;

                var cD = availableControllers.FirstOrDefault(x => x.ControllerName == type.Name);
                if (cD != null && !applicableControllers.Contains(cD))
                {
                    applicableControllers.Add(cD);
                }
            }

            return BuildLinksFromControllerList(applicableControllers, controllerName, actionName);
        }

        /// <summary>
        /// Gets a list of applicable links. For the current controller and action
        /// Special case for the actionName == Links && controllerName == Home.
        /// </summary>
        /// <returns>A List of links</returns>
        protected List<Link> Links(string actionName = "")
        {
            // get the current controller name
            var controllerDescriptor = ControllerContext.ControllerDescriptor;
            var controllerName = controllerDescriptor.ControllerName;

            // get the cached links, initialising if not already so.
            var cachedLinks = EnsureCachedDataInitialisedAndReturnCachedActions(Configuration, controllerName, actionName);
            if (cachedLinks != null) return cachedLinks;

            // no cached version... create one.
            // Detect the other actions in the controller and create links for them.
            var applicableControllers = new List<HttpControllerDescriptor>();

            // detect here for special case
            if (controllerName == "Account" && actionName == "Options")
            {
                // add in all controllers.
                foreach (var apiDescription in _apiExplorer.ApiDescriptions.Where(apiDescription => !applicableControllers.Contains(apiDescription.ActionDescriptor.ControllerDescriptor)))
                {
                    applicableControllers.Add(apiDescription.ActionDescriptor.ControllerDescriptor);
                }
            }
            else
            {
                // just add in the current controller.
                applicableControllers.Add(controllerDescriptor);
            }

            return BuildLinksFromControllerList(applicableControllers, controllerName, actionName);
        }

        /// <summary>
        /// Validates an enum argument to ensure that the value passed is a valid value for the type supplied.
        /// </summary>
        /// <typeparam name="TEnum">The Type of the enum. Do not pass in a non-enum type.</typeparam>
        /// <param name="enumValue">The value.</param>
        protected void EnsureEnumValueIsValid<TEnum>(object enumValue) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type");
            }

            var enumType = enumValue.GetType();
            if (!Enum.IsDefined(enumType, enumValue))
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, String.Format("{0} is not a valid value for type {1}", enumValue, enumType.Name));
            }
        }

        /// <summary>
        /// Builds the links from a list of applicable controller descriptor objects.
        /// </summary>
        /// <param name="applicableControllers">A list of ControllerDescriptors.</param>
        /// <param name="controllerName">The name of the current controller.</param>
        /// <param name="actionName">The name of the current action.</param>
        /// <returns>A list of Link objects.</returns>
        private List<Link> BuildLinksFromControllerList(IEnumerable<HttpControllerDescriptor> applicableControllers, string controllerName, string actionName)
        {
            var initialUri = string.Format("{0}://{1}/", this.Request.RequestUri.Scheme, this.Request.RequestUri.Authority);

            var links = new List<Link>();
            foreach (HttpControllerDescriptor applicableController in applicableControllers)
            {
                IEnumerable<MethodInfo> validMethods = applicableController.ControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(IsValidActionMethod);
                HttpControllerDescriptor controller = applicableController;
                IEnumerable<ReflectedHttpActionDescriptor> actionDescriptors = validMethods.Select(m => new ReflectedHttpActionDescriptor(controller, m));

                foreach (ReflectedHttpActionDescriptor descriptor in actionDescriptors)
                {
                    string rel = applicableController.ControllerName == controllerName ? descriptor.ActionName : string.Format("Child: {0}.{1}", applicableController.ControllerName, descriptor.ActionName);
                    string method = descriptor.SupportedHttpMethods.First().Method;
                    string title = _apiTitleLookups[method];
                    ReflectedHttpActionDescriptor descriptor1 = descriptor;
                    HttpControllerDescriptor controller1 = applicableController;
                    IEnumerable<ApiDescription> applicableApiDescriptions = _apiExplorer.ApiDescriptions.Where(d => d.ActionDescriptor.ActionName == descriptor1.ActionName && d.ActionDescriptor.ControllerDescriptor.ControllerName == controller1.ControllerName);

                    foreach (ApiDescription applicableApiDescription in applicableApiDescriptions)
                    {
                        string description = initialUri + (applicableApiDescription.RelativePath);
                        string linkTitle = string.Format(
                            "{0}:: {1}:: {2}",
                            method,
                            descriptor.ActionName,
                            string.Format(title, controllerName));
                        string linkRel = descriptor.ActionName == actionName ? "Self" : rel;
                        bool linkIsTemplated = description.Contains(char.Parse("{"));
                        var link = new Link
                                    {
                                        Title = linkTitle,
                                        Href = description,
                                        Rel = linkRel,
                                        IsTemplated = linkIsTemplated
                                    };

                        // the Link class now has IEquatable<Link> so that we can use contains effectively
                        if (!links.Contains(link))
                        {
                            links.Add(link);
                        }
                    }
                }
            }

            // if key doesn't exist, create it.
            if (!_cachedLinks.ContainsKey(controllerName))
            {
                _cachedLinks[controllerName] = new ConcurrentDictionary<string, List<Link>>();
            }

            // Add to cache for next time and return.
            _cachedLinks[controllerName][actionName] = links;
            return links;
        }

        /// <summary>
        /// Checks whether the static properties of this controller are instantiated, and instantiates them if not.
        /// The Concurrent Dictionaries are used all the way across the api application in every request, 
        /// to save generation time from the expensive reflection code.
        /// </summary>
        /// <param name="configuration">HttpConfiguration to pass to the ApiExplorer.</param>
        /// <param name="controllerName">The name of the controller to grab the links for if it has already been cached.</param>
        /// <param name="actionName">The name of the action to grab the links for if it has already been cached.</param>
        /// <returns>A cached list of links</returns>
        private static List<Link> EnsureCachedDataInitialisedAndReturnCachedActions(HttpConfiguration configuration, string controllerName, string actionName)
        {
            if (_apiExplorer == null)
            {
                var constants = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("OPTIONS", "The available options for {0}"),
                    new KeyValuePair<string, string>("GET", "Gets single / Gets all / Finds {0}"),
                    new KeyValuePair<string, string>("POST", "Creates new {0}"),
                    new KeyValuePair<string, string>("PUT", "Edits single {0}"),
                    new KeyValuePair<string, string>("PATCH", "Performs minor edits on {0}"),
                    new KeyValuePair<string, string>("DELETE", "Deletes single {0}")
                };

                _apiExplorer = new ApiExplorer(configuration);
                _cachedLinks = new ConcurrentDictionary<string, ConcurrentDictionary<string, List<Link>>>();
                _apiTitleLookups = new ConcurrentDictionary<string, string>(constants);
            }

            // returned the cached version if we have them!
            var cachedController = _cachedLinks.ContainsKey(controllerName) ? _cachedLinks[controllerName] : null;
            if (cachedController != null && cachedController.ContainsKey(actionName))
            {
                return cachedController[actionName];
            }


            return null;
        }

        /// <summary>
        /// Determines whether a reflected action method is a valid controller method. Used in a foreach above.
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        private static bool IsValidActionMethod(MethodInfo methodInfo)
        {
            if (methodInfo.IsSpecialName) return false;
            var declaringType = methodInfo.GetBaseDefinition().DeclaringType;
            return declaringType != null && !declaringType.IsAssignableFrom(typeof(BaseApiController));
        }

        /// <summary>
        /// Gets the default transaction count for a transaction
        /// </summary>
        public virtual int TransactionCount
        {
            get
            {
                return 1;
            }
        }
    }

}