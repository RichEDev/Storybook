using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace SpendManagementLibrary
{
    public class WrapperHTTPContext : IWrapperHTTPContext
    {
        public bool Request_Url_IsLoopback()
        {
            return HttpContext.Current.Request.Url.IsLoopback;
        }

        public virtual string User_Identity_Name()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        public HttpSessionState Session()
        {
            return HttpContext.Current.Session;
        }
        //System.Web.HttpContext.Current.Session["SubAccountID"] != null)
        //System.Web.HttpContext.Current.Session["SubAccountID"];
        //System.Web.HttpContext.Current.Session["myid"] != null)
        //System.Web.HttpContext.Current.Session["myid"]);
    }

    public interface IWrapperHTTPContext
    {
        bool Request_Url_IsLoopback();
        string User_Identity_Name();
        HttpSessionState Session();
    }
}
