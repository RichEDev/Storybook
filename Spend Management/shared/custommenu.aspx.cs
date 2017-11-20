using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Spend_Management;
using SpendManagementLibrary;
using System.Collections.Generic;

namespace Spend_Management
{
    public partial class custommenu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int menuid = 0;
                if (Request.QueryString["menuid"] != null)
                {
                    menuid = Convert.ToInt32(Request.QueryString["menuid"]);
                }
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cCustomEntities clsentities = new cCustomEntities(user);
                List<cCustomEntityView> views = clsentities.getViewsByMenuId(menuid);
                foreach (cCustomEntityView view in views)
                {
                    Master.addMenuItem("form_yellow", 48, view.viewname, view.MenuDescription, "viewentities.aspx?entityid=" + view.entityid + "&viewid=" + view.viewid);
                }
            }
        }
    }
}
