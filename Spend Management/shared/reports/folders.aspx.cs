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
	/// Summary description for folders.
	/// </summary>
	public partial class folders : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

            Title = "Summary";
            Master.title = Title;
            Master.PageSubTitle = "Report Categories";
			if (IsPostBack == false)
			{				
				int i;
				
				cGridColumn newcol;
				cGridRow reqrow;

                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reports, true, true);

                switch (user.CurrentActiveModule)
                {
                    case Modules.Contracts:
                        Master.helpid = 1029;
                        break;
                    default:
                        Master.helpid = 1117;
                        break;
                }

                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                int employeeid = user.EmployeeID;

                cReportFolders clsfolders = new cReportFolders(user.AccountID);

				if (Request.QueryString["callback"] != null)
				{
					switch (int.Parse(Request.QueryString["callback"]))
					{
						case 1: //delete
							clsfolders.deleteFolder(new Guid(Request.Form["folderid"]));
							Response.End();
							break;
					}
				}
				
				cGrid clsgrid = new cGrid(clsfolders.getGrid(),true,false);

				clsgrid.getColumn("folderid").hidden = true;
				clsgrid.getColumn("employeeid").hidden = true;
				clsgrid.getColumn("personal").hidden = true;
				clsgrid.getColumn("folder").description = "Category";
                clsgrid.getColumn("reportArea").hidden = true;

				newcol = new cGridColumn("Delete","<img alt=\"Delete\" src=\"../images/icons/delete2_blue.gif\">","S","",false, true);
				clsgrid.gridcolumns.Insert(0,newcol);
                newcol = new cGridColumn("Edit", "<img alt=\"Edit\" src=\"../images/icons/edit_blue.gif\">", "S", "", false, true);
				clsgrid.gridcolumns.Insert(0,newcol);
				clsgrid.tblclass = "datatbl";
				clsgrid.tableid = "folders";
				clsgrid.idcolumn = clsgrid.getColumn("folderid");
				clsgrid.getData();

				for (i = 0; i < clsgrid.gridrows.Count; i++)
				{
					reqrow = (cGridRow)clsgrid.gridrows[i];
                    if ((int)reqrow.getCellByName("employeeid").thevalue == employeeid)
                    {
                        reqrow.getCellByName("Edit").thevalue = "<a href=\"aefolder.aspx?action=2&folderid=" + reqrow.getCellByName("folderid").thevalue + "\"><img alt=\"Edit\" src=\"../images/icons/edit.gif\"></a>";
                        reqrow.getCellByName("Delete").thevalue = "<a href=\"javascript:deleteFolder('" + reqrow.getCellByName("folderid").thevalue + "');\"><img alt=\"Delete\" src=\"../images/icons/delete2.gif\"></a>";
                    }
                    else
                    {
                        reqrow.getCellByName("Edit").thevalue = "";
                        reqrow.getCellByName("Delete").thevalue = "";
                    }
				}
				litgrid.Text = clsgrid.CreateGrid();
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
	}
}
