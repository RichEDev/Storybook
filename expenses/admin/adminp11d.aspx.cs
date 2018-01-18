using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using expenses.admin;
using System.Web.Services;
using Infragistics.WebUI.UltraWebGrid;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses
{
    using SpendManagementLibrary.Employees;

	/// <summary>
	/// Summary description for adminp11d.
	/// </summary>
	public partial class adminp11d : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

			
			Title = "P11D Categories";
            Master.title = Title;
            Master.helpid = 1016;
			
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.P11D, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;


			    var gridData = this.createP11DCategoriesGrid(user);
			    litgrid.Text = gridData[1];
			    Page.ClientScript.RegisterStartupScript(this.GetType(), "p11DGridVars", cGridNew.generateJS_init("p11DGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
            }
        }

	    /// <summary>
	    /// Create cGridNew
	    /// </summary>
	    /// <param name="user">Current user</param>
	    /// <returns></returns>
	    [WebMethod(EnableSession = true)]
	    protected string[] createP11DCategoriesGrid(CurrentUser user)
	    {
	        var grid = new cGridNew(user.AccountID, user.EmployeeID, "gridP11D", "SELECT pdcatid, pdname FROM pdcats")
	        {
	            EmptyText = "No P11D Categories to display",
	            KeyField = "pdname"
	        };

	        grid.getColumnByName("pdcatid").hidden = true;
	        grid.getColumnByName("pdname").HeaderText = "P11D Category";
	        grid.enabledeleting = true;
	        grid.enableupdating = true;
            grid.editlink = "aep11d.aspx?action=2&pdcatid={pdcatid}";
            grid.deletelink = "javascript:deleteP11d({pdcatid});";

            return grid.generateGrid();
	    }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);
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
