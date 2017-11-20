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
using System.Web.Services;
using System.Text;

namespace Spend_Management
{
	/// <summary>
	/// Summary description for exporthistory.
	/// </summary>
	public partial class exporthistory : Page
	{
        static int nFinancialExportID = 0;
        static int nAppType = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            Session["ReportViewer"] = false;
			Title = "Export History";
            Master.title = Title;
            Master.PageSubTitle = "Export History Details";
            Master.helpid = 1048;
			if (IsPostBack == false)
			{
                CurrentUser currentUser = cMisc.GetCurrentUser();
                currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FinancialExports, true, true);

                int.TryParse(Request.QueryString["financialexportid"], out nFinancialExportID);
                if (nFinancialExportID == 0)
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }
                int.TryParse(Request.QueryString["apptype"], out nAppType);
                if (nAppType == 0)
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }

                svcFinancialExports svcFE = new svcFinancialExports();
                string[] gridData = svcFE.CreateExportHistoryGrid(nFinancialExportID, nAppType);
                litGrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ExpHistoryGridVars", cGridNew.generateJS_init("ExpHistoryGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, currentUser.CurrentActiveModule), true);
			}
		}
	}
}
