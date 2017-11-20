using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Data.SqlClient;
using System.Text;
using System.Web.Services;
using System.Web.Script;
using System.Web.Script.Services;
using Spend_Management.AttachmentBackups;

namespace Spend_Management
{
    public partial class adminAttachmentBackups : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            CurrentUser user = cMisc.GetCurrentUser();
            string[] gridDataProcessed = null;
            string[] gridDataUnprocessed = null;

            // set the sel.grid javascript variables
            List<string> jsBlockObjects = new List<string>();

            if (!this.IsPostBack)
            {
                Master.title = "Attachment Backup Packages";

                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AttachmentBackups, true, true);
                bool bAllowAccess = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.AttachmentBackups, true, false);

                gridDataProcessed = AttachmentBackupManager.GetPackagesGrid(user.AccountID, user.EmployeeID, true);
                litGridProcessed.Text = gridDataProcessed[2];
                jsBlockObjects.Add(gridDataProcessed[1]);
                litGridProcessed.Text = gridDataProcessed[2];

                gridDataUnprocessed = AttachmentBackupManager.GetPackagesGrid(user.AccountID, user.EmployeeID, false);
                litGridUnprocessed.Text = gridDataUnprocessed[2];
                jsBlockObjects.Add(gridDataUnprocessed[1]);
                litGridUnprocessed.Text = gridDataUnprocessed[2];
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "attachmentPackageGridVars", cGridNew.generateJS_init("attachmentPackageGridVars", jsBlockObjects, user.CurrentActiveModule), true);
        }

        protected void btnClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("");
        }

        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {

        }

    }
}
