namespace PublicAPI
{
    using System;
    using System.Net.Http.Formatting;
    using System.Web.Http;
    using System.Web.Http.Cors;

    using Filters;
    using Security.Filters;

    /// <summary>
    /// Standard WebApiConfig class used for configuring the WebApi
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers the common properties, routing and filters for this WebApi
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new JwtAuthenticationAttribute());
            config.Filters.Add(new ValidateModelAttribute());

            config.MapHttpAttributeRoutes();
            
            EnableCorsAttribute corsAttribute = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(corsAttribute);

            config.Routes.MapHttpRoute("DefaultApi", "{controller}/{id}", new { id = RouteParameter.Optional });

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(new RequestHeaderMapping("Accept", "text/html", StringComparison.InvariantCultureIgnoreCase, true, "application/json"));
        }
    }
}
