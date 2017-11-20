namespace Spend_Management.shared.webServices
{
    using System;
    using System.ComponentModel;
    using System.Web.Script.Services;
    using System.Web.Security;
    using System.Web.Services;

    /// <summary>
    /// Summary description for SvcSession
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]

    [ScriptService]
    public class SvcSession : WebService
    {
        /// <summary>
        /// Acts as a ping helper that keeps server session alivereturning a string.
        /// </summary>
        /// <returns>
        /// The string "OK".
        /// </returns>
        [WebMethod(EnableSession = true)]
        public string PingSession()
        {
            return "OK";
        }

        /// <summary>
        /// Resets forms authentication and server session allowing for user redirect.
        /// </summary>
        /// <returns>
        /// The string "OK".
        /// </returns>
        [WebMethod(EnableSession = true)]
        public string ResetSession()
        {
            FormsAuthentication.SignOut();
            var subAccountId = Session["SubAccountID"];
            CurrentUser currentUser = cMisc.GetCurrentUser();
            System.Web.HttpContext.Current.Session.Clear();
            System.Web.HttpContext.Current.Session.Abandon();

            System.Web.HttpCookie cookie = new System.Web.HttpCookie("SubAccount");
            if (subAccountId != null)
            {
                cookie.Values.Add("_ID", subAccountId.ToString());
            }

            if (currentUser?.Employee != null)
            {
                cookie.Values.Add("_Username", currentUser.Employee.Username);
            }
            
            cookie.Expires = DateTime.Now.AddDays(1);
            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);

            var redirectUrl = "null";
            var cookies = System.Web.HttpContext.Current.Request.Cookies;
            if (cookies["SSO"] != null && cookies["SSO"].Value == "1")
            {
                var sso = SpendManagementLibrary.SingleSignOn.Get(cMisc.GetCurrentUser());
                if (sso != null && !string.IsNullOrEmpty(sso.TimeoutUrl))
                {
                    redirectUrl = sso.TimeoutUrl;
                }
            }

            return "OK," + redirectUrl;
        }
    }
}

