using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using SpendManagementLibrary;
using Spend_Management;
using System.Collections.Generic;
using System.Text;

namespace Spend_Management
{
	/// <summary>
	/// Summary description for adminsubcats.
	/// </summary>
	public partial class adminsubcats : Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ExpenseItems, true, true);

			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

			Title = "Expense Items";
            Master.title = Title;
            Master.helpid = 1023;
			if (IsPostBack == false)
			{
                string[] gridData = createGrid(user.AccountID, user.EmployeeID);
                litgrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "SubcatsGridVars", cGridNew.generateJS_init("SubcatsGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
			}			
		}

        private string[] createGrid(int accountid, int employeeid)
        {
            cTables clstables = new cTables(accountid);
            cFields clsfields = new cFields(accountid);
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("d4ed76bd-605c-45ce-b075-4c6018a50b08"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("44c2a53a-db33-45af-af66-d0055aad48ec"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("abfe0bb2-e6ac-40d0-88ce-c5f7b043924d"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("379b67dd-654e-43cd-b55d-b9b5262eeeee"))));
            cGridNew clsgrid = new cGridNew(accountid, employeeid, "gridSubcats", clstables.GetTableByID(new Guid("401b44d7-d6d8-497b-8720-7ffcc07d635d")), columns);
            clsgrid.getColumnByName("subcatid").hidden = true;
            clsgrid.KeyField = "subcatid";
            clsgrid.enabledeleting = true;
            clsgrid.deletelink = "javascript:deleteSubcat({subcatid});";
            clsgrid.enableupdating = true;
            clsgrid.editlink = "aesubcat.aspx?subcatid={subcatid}";
            return clsgrid.generateGrid();
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


        [WebMethod(EnableSession = true)]
        public static int deleteSubcat(int accountid, int subcatid)
        {
            cSubcats clssubcats = new cSubcats(accountid);
            return clssubcats.DeleteSubcat(subcatid);
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
