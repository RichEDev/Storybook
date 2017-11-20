using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace Spend_Management
{
    /// <summary>
    /// NHS Trusts summary and ae page
    /// </summary>
    public partial class trusts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "NHS Trusts";
            Master.title = Title;
            this.Master.helpid = 1127;
            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ESRTrustDetails, true, true);

            cGridNew clsGridNew = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "trusts", "SELECT trustID, trustName, trustVPD, runSequenceNumber, archived, ESRVersionNumber FROM esrTrusts");

            clsGridNew.getColumnByName("trustID").hidden = true;
            clsGridNew.KeyField = "trustID";
            clsGridNew.enablearchiving = true;
            clsGridNew.archivelink = "javascript:ArchiveTrust({trustID});";
            clsGridNew.ArchiveField = "archived";

            clsGridNew.getColumnByName("archived").hidden = true;
            clsGridNew.EmptyText = "There are currently no trusts added.";
            clsGridNew.enabledeleting = true;
            clsGridNew.enableupdating = true;
            clsGridNew.editlink = "javascript:EditTrust({trustID});";
            clsGridNew.deletelink = "javascript:DeleteTrust({trustID});";
            clsGridNew.addEventColumn("mappings", "/shared/images/icons/16/Plain/elements_selection.png", "ESRElementMappings.aspx?trustID={trustID}", "Mappings", "Mappings");

            ((cFieldColumn)clsGridNew.getColumnByName("ESRVersionNumber")).addValueListItem(1, "v1.0");
            ((cFieldColumn)clsGridNew.getColumnByName("ESRVersionNumber")).addValueListItem(2, "v2.0");

            string[] gridData = clsGridNew.generateGrid();
            litGrid.Text = gridData[1];

            Page.ClientScript.RegisterStartupScript(this.GetType(), "TrustGridVars", cGridNew.generateJS_init("TrustGridVars", new List<string>() { gridData[0] }, currentUser.CurrentActiveModule), true);
        }
    }
}
