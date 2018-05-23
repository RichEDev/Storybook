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

using SpendManagementLibrary;
using Spend_Management;
namespace Spend_Management
{
    using BusinessLogic.Modules;

    /// <summary>
	/// Summary description for aefolder.
	/// </summary>
	public partial class aefolder : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Title = "Add / Edit Category";
            Master.title = "Reports";
            Master.PageSubTitle = Title;
            Master.enablenavigation = false;
            Master.enablenavigation = false;
			if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reports, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                switch (user.CurrentActiveModule)
                {
                    case Modules.Contracts:
                        Master.helpid = 1029;
                        break;
                    default:
                        Master.helpid = 1117;
                        break;
                }

				int action = 0;
				if (Request.QueryString["action"] != null)
				{
					action = int.Parse(Request.QueryString["action"]);
				}
				ViewState["action"] = action;

				if (action == 2) //get details
				{
					Guid folderid = new Guid(Request.QueryString["folderid"]);
					ViewState["folderid"] = folderid;
                    cReportFolders clsfolders = new cReportFolders(user.AccountID);
					cReportFolder reqfolder = clsfolders.getFolderById(folderid);
					txtfolder.Text = reqfolder.folder;
					chkpersonal.Checked = reqfolder.personal;
				}
				else
				{
					chkpersonal.Checked = true;
				}
			}
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
			this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);

		}
		#endregion

		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			cReportFolders clsfolders = new cReportFolders((int)ViewState["accountid"]);
			string folder;
			bool personal;
            byte returnvalue;

			folder = txtfolder.Text;
			personal = chkpersonal.Checked;

			if ((int)ViewState["action"] == 2) //update
			{
                cReportFolder reqFolder = clsfolders.getFolderById(new Guid(ViewState["folderid"].ToString()));
				returnvalue = clsfolders.updateFolder(reqFolder.folderid,folder,personal);
			}
			else
			{
                returnvalue = clsfolders.addFolder(folder, (int)ViewState["employeeid"], personal);
			}
            if (returnvalue == 1)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('The category name you have entered already exists')", true);
            }
            else
            {
                Response.Redirect("folders.aspx", true);
            }
		}
	}
}
