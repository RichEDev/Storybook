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


namespace Spend_Management
{
	/// <summary>
	/// Summary description for adminallowances.
	/// </summary>
	public partial class adminallowances : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

			
			Title = "Allowances";
            Master.title = Title;
            Master.helpid = 1003;
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Allowances, true, true);
                string[] gridData = createGrid();
                litgrid.Text = gridData[1];  user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Allowances, true, true);

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "AllowancesGridVars", cGridNew.generateJS_init("AllowancesGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);                
			}

		}

        public string[] createGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cAllowances clsallowances = new cAllowances(user.AccountID);
            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridAllowances", clsallowances.createGrid());
            clsgrid.getColumnByName("allowanceid").hidden = true;
            clsgrid.KeyField = "allowanceid";
            clsgrid.enableupdating = true;
            clsgrid.editlink = "aeallowance.aspx?allowanceid={allowanceid}";
            clsgrid.enabledeleting = true;
            clsgrid.deletelink = "javascript:deleteAllowance({allowanceid});";
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

        /// <summary>
        /// Deletes an access role based on its Id
        /// </summary>
        /// <param name="allowanceid"></param>
        /// <returns>The outcome of the delete</returns>
        [WebMethod(EnableSession = true)]
        public static int deleteAllowance(int allowanceid)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            if (user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Allowances, true, true)) ;
            {
                var clsallowances = new cAllowances(user.AccountID);
                return clsallowances.deleteAllowance(allowanceid);
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
