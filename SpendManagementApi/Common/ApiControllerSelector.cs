namespace SpendManagementApi.Common
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;
    using System.Web.Http.Routing;

    /// <summary>
    /// A custom implementation of a controller selector.
    /// </summary>
    public class ApiControllerSelector : DefaultHttpControllerSelector
    {
        /// <summary>
        /// Allows selection of an API controller.
        /// </summary>
        /// <param name="config">The http config</param>
        public ApiControllerSelector(HttpConfiguration config)
            : base(config)
        {
        }

        /// <summary>
        /// Selects a <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor"/> for the given <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor"/> instance for the given <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </returns>
        /// <param name="request">The HTTP request message.</param>
        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            int defaultApiVersion;
            int.TryParse(ConfigurationManager.AppSettings["DefaultApiVersion"], out defaultApiVersion);
            defaultApiVersion = defaultApiVersion == 0 ? 1 : defaultApiVersion;

            HttpControllerDescriptor controllerDescriptor = null;
            IHttpRouteData routeData = request.GetRouteData();

            if (routeData == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            //check if this route is actually an attribute route
            IEnumerable<IHttpRouteData> attributeSubRoutes = routeData.GetSubRoutes();

            string apiVersion = GetVersionFromHttpHeader(request);
            if (string.IsNullOrEmpty(apiVersion))
            {
                apiVersion = defaultApiVersion.ToString(CultureInfo.InvariantCulture);
            }
            
            string controllerName = string.Empty;
            var attributedRoutesData = request.GetRouteData().GetSubRoutes();
            var subRouteData = attributedRoutesData.FirstOrDefault();

            if (subRouteData != null)
            {
                var actions = (HttpActionDescriptor[])subRouteData.Route.DataTokens["actions"];
                controllerName = actions[0].ControllerDescriptor.ControllerName;
                string[] splitControllerName = controllerName.Split('V');
                if (splitControllerName.Length > 0)
                {
                    int versionNumber;
                    if (int.TryParse(splitControllerName[splitControllerName.Length - 1], out versionNumber))
                    {
                        controllerName = controllerName.Replace("V" + versionNumber, string.Empty);
                    }
                }
            }

            string newControllerNameSuffix = string.IsNullOrEmpty(apiVersion) ? string.Empty : String.Concat("V", apiVersion);

            IEnumerable<IHttpRouteData> filteredSubRoutes = attributeSubRoutes.Where(attrRouteData =>
            {
                HttpControllerDescriptor currentDescriptor = GetControllerDescriptor(attrRouteData);

                bool match = currentDescriptor.ControllerName == controllerName + newControllerNameSuffix;

                if (match && (controllerDescriptor == null))
                {
                    controllerDescriptor = currentDescriptor;
                }

                return match;
            });

            routeData.Values["MS_SubRoutes"] = filteredSubRoutes.ToArray();

            return controllerDescriptor;
        }

        /// <summary>
        /// Gets the version number supplied in the http header. (header name version)
        /// </summary>
        /// <param name="request">The http request.</param>
        /// <returns>The version number as a string.</returns>
        private static string GetVersionFromHttpHeader(HttpRequestMessage request)
        {
            const string HeaderName = "version";

            if (!request.Headers.Contains(HeaderName))
            {
                return string.Empty;
            }

            var versionHeader = request.Headers.GetValues(HeaderName).FirstOrDefault();

            return versionHeader ?? string.Empty;
        }

        /// <summary>
        /// Gets the controller descriptor.
        /// </summary>
        /// <param name="routeData">The route data.</param>
        /// <returns></returns>
        private static HttpControllerDescriptor GetControllerDescriptor(IHttpRouteData routeData)
        {
            return ((HttpActionDescriptor[])routeData.Route.DataTokens["actions"]).First().ControllerDescriptor;
        }
    }
}