using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Services;
using SpendManagementLibrary;
using System.Web.Script.Services;
using System.Text;

namespace Spend_Management
{
    public partial class poolcars : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
			Title = "Pool Vehicles";
            Master.title = Title;
            Master.helpid = 1077;

            if (IsPostBack == false)
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                ViewState["accountid"] = currentUser.AccountID;
                ViewState["employeeid"] = currentUser.EmployeeID;

                currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.PoolCars, true, true);

                cPoolCars clsPoolCars = new cPoolCars(currentUser.AccountID);

                string[] gridData = clsPoolCars.CreatePoolCarsGrid();

                litGridPoolCars.Text = gridData[2];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "PoolCarsGridVars", cGridNew.generateJS_init("PoolCarsGridVars", new System.Collections.Generic.List<string>() { gridData[1] }, currentUser.CurrentActiveModule), true);
            }
        }

        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(sPreviousURL, true);
        }
    }
}
