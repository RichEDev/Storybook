namespace SpendManagementApi
{
    using System.Configuration;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using System.Web.Http.Dispatcher;
    
    using SpendManagementApi.Attributes;
    using SpendManagementApi.Common;

    /// <summary>
    /// Defines routes and config items to customise the Api with.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>Configuration of the Web Api goes here.</summary>
        /// <param name="config">The passed HttpConfiguration.</param>
        public static void Register(HttpConfiguration config)
        {
            // CORS
            if (ConfigurationManager.AppSettings["EnableCors"] == "1")
            {
                config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            }

            // HTTPS
            if (ConfigurationManager.AppSettings["UseHttps"] == "1")
            {
                config.Filters.Add(new HttpsAttribute());
            }

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;

            // Request validation
            config.Filters.Add(new ValidateRequestAttribute());
            config.Filters.Add(new HandleApiError());
            
            // Web API routes
            config.MapHttpAttributeRoutes();

            // Replace the controller configuration selector for versioning capabilities
            config.Services.Replace(typeof(IHttpControllerSelector), new ApiControllerSelector((config)));
        }
    }
}
