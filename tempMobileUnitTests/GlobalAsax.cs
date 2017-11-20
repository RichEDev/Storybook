using System.Configuration;
using System.Data.SqlClient;
using SpendManagementLibrary;

namespace tempMobileUnitTests
{
    /// <summary>
    /// Class to simulate the application code done when the IIS web app starts/stops
    ///  - Will need to be kept in sync with the global.asax from expenses/framework
    /// </summary>
    public class GlobalAsax
    {

        ///// <summary>
        ///// Required designer variable.
        ///// </summary>
        //private System.ComponentModel.IContainer components = null;


        //public Global()
        //{

        //    InitializeComponent();
        //}	

        /// <summary>
        /// Dummy App start
        /// </summary>
        public static void Application_Start()
        {
            // Set global variables for use in SML classes
            GlobalVariables.MetabaseConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;
            GlobalVariables.DefaultModule = GlobalTestVariables.ActiveModule;
            cEventlog.LogEntry("Unit Tests Starting" + GlobalVariables.DefaultModule);
            // Starts the sql dependency on "metabase" connection string
            cSecureData crypt = new cSecureData();
            SqlConnectionStringBuilder sConnectionString = new SqlConnectionStringBuilder(GlobalVariables.MetabaseConnectionString);
            sConnectionString.Password = crypt.Decrypt(sConnectionString.Password);
            SqlDependency.Start(sConnectionString.ToString());

            // Caching of data
            cAccounts clsAccounts = new cAccounts();
            cTables clsTables = new cTables();
            cFields clsFields = new cFields();
        }

        //protected void Session_Start(Object sender, EventArgs e)
        //{
        //    if (Response.Cookies.Count > 0)
        //    {
        //        foreach (string s in Response.Cookies.AllKeys)
        //        {
        //            if (s == System.Web.Security.FormsAuthentication.FormsCookieName || s.ToLower() == "asp.net_sessionid")
        //            {
        //                Response.Cookies[s].HttpOnly = false;
        //            }
        //        }
        //    }

        //}

        //protected void Application_BeginRequest(Object sender, EventArgs e)
        //{
        //    /// Force all connections using sel-expenses in the domain name on to a secure connection
        //    if (!Request.IsSecureConnection && Request.Url.Host.ToLower().Contains("sel-expenses"))
        //    {
        //        Response.Redirect("https://" + Request.Url.Host + Request.Url.AbsolutePath + Request.Url.Query, true);
        //    }
        //}

        //protected void Application_EndRequest(Object sender, EventArgs e)
        //{

        //}

        //protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        //{

        //}

        //protected void Application_Error(Object sender, EventArgs e)
        //{
        //    cErrors clserrors = new cErrors();
        //    System.Exception exp = (System.Exception)Server.GetLastError();

        //    if (exp.GetType() == typeof(HttpException))
        //    {
        //        HttpException objHttpException = exp as HttpException;
        //        if (objHttpException.GetHttpCode() == 404)
        //        {
        //            Response.Redirect("~/shared/404.aspx", true);
        //        }
        //        else
        //        {
        //            clserrors.ReportError(exp);
        //        }
        //    }
        //    else
        //    {
        //        clserrors.ReportError(exp);
        //    }

        //}

        //protected void Session_End(Object sender, EventArgs e)
        //{
        //}

        /// <summary>
        /// Dummy App end
        /// </summary>
        public static void Application_End()
        {
            cEventlog.LogEntry("Unit Tests Shutting down");
            try
            {
                cAccounts clsAccounts = new cAccounts();
                clsAccounts.ToggleDependencies(cAccounts.ToggleDependancy.Stop);
            }
            catch { }
        }

        //protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        //{
        //    if (HttpContext.Current != null)
        //    {
        //        Page p = HttpContext.Current.Handler as Page;

        //        if (p != null)
        //        {
        //            string strTheme = "ExpensesThemeNew";

        //            if (Convert.ToString(HttpContext.Current.Session["THEME"]) != string.Empty)
        //            {
        //                strTheme = Convert.ToString(HttpContext.Current.Session["THEME"]);
        //            }

        //            p.StyleSheetTheme = strTheme;
        //        }
        //    }
        //}

        //#region Web Form Designer generated code
        ///// <summary>
        ///// Required method for Designer support - do not modify
        ///// the contents of this method with the code editor.
        ///// </summary>
        //private void InitializeComponent()
        //{    
        //    this.components = new System.ComponentModel.Container();
        //}
        //#endregion
    }
}
