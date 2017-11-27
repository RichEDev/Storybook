using System.Web;

[assembly: PreApplicationStartMethod(typeof(expenses.PageInitializerModule), "Initialize")]
namespace expenses
{
    using BusinessLogic.Identity;

    using Common.Logging;
    using Common.Logging.Converters;

    using System;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Optimization;
    using System.Web.UI;
    
    using Bootstrap;
    using SimpleInjector;

    using Spend_Management;
    using Spend_Management.App_Start;
    using SpendManagementLibrary;

    using System.Collections.Generic;


    /// <summary>
    /// The global.asax class
    /// </summary>
    public class Global : HttpApplication
    {
        public static ILog Logger = new LogFactory<Global>().GetLogger();

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public static Container container;

        /// <summary>
        /// Initialises a new instance of the <see cref="Global"/> class.
        /// </summary>
        public Global()
        {
            this.InitializeComponent();
        }

        public static void InitializeHandler(IHttpHandler handler)
        {
            container.GetRegistration(handler.GetType(), true).Registration.InitializeInstance(handler);
        }

        /// <summary>
        /// The application_ start.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Application_Start(object sender, EventArgs e)
        {
            container = Bootstraper.Bootstrap();
            // Loads global variables
            new GlobalVariables(GlobalVariables.ApplicationType.Web);

            cEventlog.LogEntry("Starting" + GlobalVariables.DefaultModule);

            bool enableOptimisations;
            // ReSharper disable SimplifyConditionalTernaryExpression
            BundleTable.EnableOptimizations = bool.TryParse(ConfigurationManager.AppSettings["BundlingEnableOptimisations"], out enableOptimisations)
                ? enableOptimisations
                : true;
            // ReSharper restore SimplifyConditionalTernaryExpression
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Caching of data
            var clsAccounts = new cAccounts();
            clsAccounts.CacheList();

            HostManager.SetHostInformation();

            AutomapperConfig.Configure();

        }

        /// <summary>
        /// The session_ start.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Session_Start(object sender, EventArgs e)
        {
            if (this.Response.Cookies.Count > 0)
            {
                foreach (var httpCookie in this.Response.Cookies.AllKeys.Where(s => s == System.Web.Security.FormsAuthentication.FormsCookieName || s.ToLower() == "asp.net_sessionid").Select(s => this.Response.Cookies[s]).Where(httpCookie => httpCookie != null))
                {
                    httpCookie.HttpOnly = false;
                }
            }
        }


        #region request cycle

        #region 1 Application_BeginRequest

        /// <summary>
        /// The application_ begin request.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <remarks>
        ///     1. Application_BeginRequest():
        ///     This method is called at the start of every request.
        ///     HTTPS is in the request header X-Forwarded-Proto on the live enviroment. 
        ///     All requests to and from the load balancer need to be kept as HTTP requests 
        ///     so we don't need to force a HTTPS redirect (otherwise we get an endless redirect loop)
        /// </remarks>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // set extra logging properties
            IExtraContext loggingContext = container.GetInstance<IExtraContext>();
            loggingContext["activityid"] = new TraceActivityId();

            HttpContext httpContext = HttpContext.Current;
            loggingContext["url"] = httpContext.Request.Url.AbsoluteUri;
            loggingContext["useragent"] = httpContext.Request.UserAgent;

            string header = this.Request.Headers["X-Forwarded-Proto"];
            bool hasXForward = string.Equals(header, "https", StringComparison.OrdinalIgnoreCase);

            if (!hasXForward)
            {

                if (!this.Request.Url.Host.ToLower().StartsWith("api") && !this.Request.IsSecureConnection && (Convert.ToBoolean(ConfigurationManager.AppSettings["ForceHTTPSRedirect"] ?? "false")))
                {
                    this.Response.Redirect($"https://{this.Request.Url.Host}{this.Request.Url.AbsolutePath}{this.Request.Url.Query}", true);
                }
            }
        }

        #endregion 1 Application_BeginRequest

        #region 2 Application_AuthenticateRequest
        /// <summary>
        /// The application_ authenticate request.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <remarks>
        ///     2. Application_AuthenticateRequest()
        ///     This method is called just before authentication is performed. This is a jumping-off point for creating your own authentication logic.
        /// </remarks>
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // set extra logging properties
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null)
            {
                IExtraContext loggingContext = container.GetInstance<IExtraContext>();

                UserIdentity user = container.GetInstance<IIdentityProvider>().GetUserIdentity();

                if (user != null)
                {
                    if (user.EmployeeId > 0)
                    {
                        loggingContext["employeeid"] = user.EmployeeId;
                    }

                    if (user.AccountId > 0)
                    {
                        loggingContext["accountid"] = user.AccountId;
                    }

                    if (user.SubAccountId != -1)
                    {
                        loggingContext["subaccountid"] = user.SubAccountId;
                    }

                    if (user.DelegateId.HasValue)
                    {
                        loggingContext["delegateid"] = user.DelegateId.Value;
                    }
                }
            }
        }
        #endregion 2 Application_AuthenticateRequest

        #region 3 Application_AuthorizeRequest
        /// <summary>
        /// Application_AuthorizeRequest
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        ///     3. Application_AuthorizeRequest():
        ///     After the user is authenticated (identified), it’s time to determine the user’s permissions. You can use this method to assign special privileges.
        /// </remarks>
        protected void Application_AuthorizeRequest(object sender, EventArgs e)
        {
        }
        #endregion 3 Application_AuthorizeRequest

        #region 4 Application_ResolveRequestCache
        /// <summary>
        /// Application_ResolveRequestCache
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        ///     4. Application_ResolveRequestCache():
        ///     This method is commonly used in conjunction with output caching.
        ///     With output caching (described in Chapter 11), the rendered HTML of a web form is reused, without executing any of your code.
        ///     However, this event handler still runs.
        /// </remarks>
        protected void Application_ResolveRequestCache(object sender, EventArgs e)
        {
        }
        #endregion 4 Application_ResolveRequestCache

        #region 5 handler
        // 5. At this point, the request is handed off to the appropriate handler. For example, for a web form request, this is the point when the page is compiled (if necessary) and instantiated.
        #endregion 5 handler

        #region 6 Application_AcquireRequestState
        /// <summary>
        /// Application_AcquireRequestState
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        ///     6. Application_AcquireRequestState():
        ///     This method is called just before session-specific information is retrieved for the client and used to populate the Session collection.
        /// </remarks>
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
        }
        #endregion 6 Application_AcquireRequestState

        #region 7 Application_PreRequestHandlerExecute
        /// <summary>
        /// Application_PreRequestHandlerExecute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        ///     7. Application_PreRequestHandlerExecute():
        ///     This method is called before the appropriate HTTP handler executes the request.
        /// </remarks>
        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (HttpContext.Current != null)
            {
                var p = HttpContext.Current.Handler as Page;

                if (p != null)
                {
                    p.StyleSheetTheme = HostManager.GetTheme(HttpContext.Current.Request.Url.Host);
                }
            }
        }
        #endregion 7 Application_PreRequestHandlerExecute

        #region 8 handler
        // 8. At this point, the appropriate handler executes the request. For example, if it’s a web form request, the event-handling code for the page is executed, and the page is rendered to HTML.
        #endregion 8 handler

        #region 9 Application_PostRequestHandlerExecute
        /// <summary>
        /// Application_PostRequestHandlerExecute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        ///     9. Application_PostRequestHandlerExecute():
        ///     This method is called just after the request is handled.
        /// </remarks>
        protected void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
        }
        #endregion 9 Application_PostRequestHandlerExecute

        #region 10 Application_ReleaseRequestState
        /// <summary>
        /// Application_ReleaseRequestState
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        ///     10. Application_ReleaseRequestState():
        ///     This method is called when the session-specific information is about to be serialized from the Session collection so that it’s available for the next request.
        /// </remarks>
        protected void Application_ReleaseRequestState(object sender, EventArgs e)
        {
        }
        #endregion 10 Application_ReleaseRequestState

        #region 11 Application_UpdateRequestCache
        /// <summary>
        /// Application_UpdateRequestCache
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        ///     11. Application_UpdateRequestCache():
        ///     This method is called just before information is added to the output cache.
        ///     For example, if you’ve enabled output caching for a web page, ASP.NET will insert the rendered HTML for the page into the cache at this point.
        /// </remarks>
        protected void Application_UpdateRequestCache(object sender, EventArgs e)
        {
        }
        #endregion 11 Application_UpdateRequestCache

        #region 12 Application_EndRequest
        /// <summary>
        /// Application_EndRequest
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        ///     12. Application_EndRequest():
        ///     This method is called at the end of the request, just before the objects are released and reclaimed.
        ///     It’s a suitable point for cleanup code.
        /// </remarks>
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            NewRelicAccountAndEmployeeIds();
        }
        #endregion 12 Application_EndRequest

        #endregion request cycle

        /// <summary>
        /// The application_ error.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception lastException = this.Server.GetLastError();

            /*
            Module
             
            reportid
            report name
            report export type
            report processed rows
                         */

            IExtraContext extraContext = container.GetInstance<IExtraContext>();

            // session
            if (HttpContext.Current.Session.Count > 0)
            {
                Dictionary<string, string> sessionProperties = new Dictionary<string, string>();
                foreach (string key in HttpContext.Current.Session.Cast<string>().Where(key => HttpContext.Current.Session[key] != null))
                {
                    if (sessionProperties.ContainsKey(key) == false)
                    {
                        sessionProperties.Add(key, HttpContext.Current.Session[key].ToString());
                    }
                }

                extraContext["session"] = sessionProperties;
            }

            // form
            Dictionary<string, string> formProperties = new Dictionary<string, string>();
            if (HttpContext.Current.Request.Form.Count > 0)
            {
                foreach (string key in HttpContext.Current.Request.Form.AllKeys.Where(key => key != "__VIEWSTATE" && HttpContext.Current.Request.Form[key] != null))
                {
                    if (formProperties.ContainsKey(key) == false)
                    {
                        formProperties.Add(key, key.ToLower().Contains("password") ? "Hidden" : HttpContext.Current.Request.Form[key]);
                    }
                }

                extraContext["post"] = formProperties;
            }

            // cookies
            Dictionary<string, string> cookiesProperties = new Dictionary<string, string>();
            if (HttpContext.Current.Request.Cookies.Count > 0)
            {
                foreach (string key in HttpContext.Current.Request.Cookies.AllKeys.Where(key => HttpContext.Current.Request.Cookies[key] != null))
                {
                    if (cookiesProperties.ContainsKey(key) == false)
                    {
                        // removal of any prefixed "." character as this does not work with elasticsearch
                        string cleanKey = key;
                        if (cleanKey.StartsWith("."))
                        {
                            cleanKey = cleanKey.Remove(0, 1);
                        }

                        cookiesProperties.Add(cleanKey, HttpContext.Current.Request.Cookies[key].Value);
                    }
                }

                extraContext["cookies"] = cookiesProperties;
            }

            Logger.Error("Unhandled Exception", lastException);


            // Traditional unhandled error emailing - to be removed once json logging usable
            var errorHandler = new ErrorHandlerWeb();
            errorHandler.SendError(cMisc.GetCurrentUser(), lastException);
        }

        /// <summary>
        /// The session_ end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Session_End(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// The application_ end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Application_End(object sender, EventArgs e)
        {
            cEventlog.LogEntry("Shutting down");
        }

        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
        }
        #endregion

        #region NewRelic helpers

        /// <summary>
        /// Indicates whether or not new relic logging is enabled.
        /// </summary>
        private static readonly bool NewRelicLogEnabled;

        /// <summary>
        /// Initialises static members of the <see cref="Global"/> class.
        /// </summary>
        static Global()
        {
            if (!bool.TryParse(ConfigurationManager.AppSettings["NewRelicDetailedLogging"], out NewRelicLogEnabled))
            {
                throw new ConfigurationErrorsException("The Web.config \"NewRelicDetailedLogging\" app setting key is either missing or does not contain true/false. This is needed to determine whether or not to track request timing, session key and account information to NewRelic.");
            }
        }

        /// <summary>
        /// Adds the Account ID and Employee ID as a custom parameter to the new relic agent, so that it can be seen within new relic transaction traces.
        /// </summary>
        private static void NewRelicAccountAndEmployeeIds()
        {
            if (!NewRelicLogEnabled)
            {
                return;
            }

            const string ParameterKey = "Account ID / Employee ID";
            if (HttpContext.Current == null || HttpContext.Current.User == null)
            {
                return;
            }

            var identityName = HttpContext.Current.User.Identity.Name;
            if (string.IsNullOrEmpty(identityName))
            {
                return;
            }

            NewRelic.Api.Agent.NewRelic.AddCustomParameter(ParameterKey, identityName);
        }
        #endregion NewRelic helpers
    }
}
