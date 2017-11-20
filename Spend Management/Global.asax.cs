using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Web.UI;

namespace Spend_Management
{
    using SpendManagementLibrary;

    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                cSecureData crypt = new cSecureData();
                SqlConnectionStringBuilder sConnectionString = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
                sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);
                SqlDependency.Start(sConnectionString.ToString());
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

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
    }
}