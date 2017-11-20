using System.Configuration;
using System.Web.Mvc;
using SpendManagementApi.Attributes;

namespace SpendManagementApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            if (ConfigurationManager.AppSettings["UseHttps"] == "1")
            {
                filters.Add(new RequireHttpsAttribute());
            }
        }
    }
}
