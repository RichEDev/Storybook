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
using System.Text;



namespace Spend_Management.shared.admin
{
	/// <summary>
	/// Summary description for admindepartments.
	/// </summary>
	public partial class admindepartments : Page
	{
	
		

		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

			
			Title = "Departments";
            Master.title = Title;
            Master.helpid = 1021;

			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Departments, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                this.lnkNewDepartment.Visible = user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Departments,true);
                string[] gridData = createGrid(cmbfilter.SelectedValue);
                litgrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "DeptsGridVars", cGridNew.generateJS_init("DeptsGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);
			}
		}

        [WebMethod(EnableSession=true)]
        public static string[] createGrid(string FilterVal)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cDepartments clsdepartments = new cDepartments(user.AccountID);
            cGridNew newgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridDepartments", clsdepartments.GetGridSql());
            if (FilterVal != "2")
            {
                int val = int.Parse(FilterVal);
                newgrid.addFilter(((cFieldColumn)newgrid.getColumnByName("archived")).field, ConditionType.Equals, new object[] { val }, null, ConditionJoiner.None);
            }
            bool allowEdit= user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Departments, true);
            newgrid.enablearchiving = allowEdit;
            newgrid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Departments, true);
            newgrid.enableupdating = allowEdit;
            newgrid.editlink = "aedepartment.aspx?departmentid={departmentid}";
            newgrid.deletelink = "javascript:deleteDepartment({departmentid});";
            newgrid.archivelink = "javascript:changeArchiveStatus({departmentid});";
            newgrid.ArchiveField = "archived";
            newgrid.getColumnByName("departmentid").hidden = true;
            newgrid.getColumnByName("archived").hidden = true;
            newgrid.KeyField = "departmentid";
            newgrid.EmptyText = "No departments to display";
            return newgrid.generateGrid();
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


        [WebMethod(EnableSession=true)]
        public static int deleteDepartment(int accountid, int departmentid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cDepartments clsdepartments = new cDepartments(accountid);
            return clsdepartments.DeleteDepartment(departmentid, user.EmployeeID);
        }

        [WebMethod(EnableSession = true)]
        public static void changeStatus(int accountid, int departmentid)
        {
            cDepartments clsdepartments = new cDepartments(accountid);
            cDepartment reqdep = clsdepartments.GetDepartmentById(departmentid);
            clsdepartments.ChangeStatus(departmentid, !reqdep.Archived);
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
