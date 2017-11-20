﻿namespace SpendManagementApi
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "help/{controller}/{action}/{id}",
                defaults: new { Area = "HelpPage", controller = "Help", action = "Home", id = UrlParameter.Optional }
            );
        }
    }
}
