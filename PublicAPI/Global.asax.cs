namespace PublicAPI
{
    using System;
    using System.Web;
    using System.Web.Http;
    using SimpleInjector;
    using SimpleInjector.Integration.WebApi;
    using PublicAPI.Bootstrap;

    /// <summary>
    /// Defines the methods, properties, and events that are common to all application
    /// objects in an ASP.NET application. This class is the base class for applications
    /// that are defined by the user in the Global.asax file.
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        public static Container container;

        /// <summary>
        /// Application start method, called when the application starts.
        /// </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            container = Bootstrapper.Bootstrap();

            MapObjects.Create();

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }
    }
}
