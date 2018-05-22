namespace InternalApi
{
    using System.Web;
    using System.Web.Http;

    using InternalApi.Bootstrap;

    using SimpleInjector;
    using SimpleInjector.Integration.WebApi;

    using SpendManagementLibrary;

    /// <summary>
    /// Defines the methods, properties, and events that are common to all application
    /// objects in an ASP.NET application. This class is the base class for applications
    /// that are defined by the user in the Global.asax file.
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        /// <summary>
        /// The <see cref="Container"/> created by the simple injector registration
        /// </summary>
        public static Container Container;

        /// <summary>
        /// Application start method, called when the application starts.
        /// </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Loads global variables
            new GlobalVariables(GlobalVariables.ApplicationType.Service);

            Container = Bootstrapper.Bootstrap();

            FunkyInjector.Container = Container;

            MapObjects.Create();

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(Container);

        }
    }
}
