using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Spend_Management
{
    using Spend_Management.expenses.code;

    public partial class flags : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FlagsAndLimits, true, true);

            if (!IsPostBack)
            {
                
            
                Title = "Flag Management";
                Master.title = Title;
                string[] gridData = createGrid(user);
                litGrid.Text = gridData[1];
                pnlLinks.Visible = user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.FlagsAndLimits, true, false);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "mileageGridVars", cGridNew.generateJS_init("mileageGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
            }
        }

        public string[] createGrid(CurrentUser user)
        {
            
            FlagManagement clsflags = new FlagManagement(user.AccountID);
            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridFlags", clsflags.CreateGrid());
            clsgrid.getColumnByName("flagID").hidden = true;
            clsgrid.KeyField = "flagID";
            if (user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.FlagsAndLimits, true))
            {
                clsgrid.enableupdating = true;
            }

            clsgrid.editlink = "aeflagrule.aspx?flagID={flagID}";
            if (user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.FlagsAndLimits, true))
            {
                clsgrid.enabledeleting = true;
            }

            clsgrid.deletelink = "javascript:SEL.FlagsAndLimits.deleteFlagRule({flagID});";
            clsgrid.EmptyText = "There are no flags rules to display.";
            ((cFieldColumn)clsgrid.getColumnByName("action")).addValueListItem(2, "Block Item");
            ((cFieldColumn)clsgrid.getColumnByName("action")).addValueListItem(1, "Flag Item");

            cFieldColumn flagtype = (cFieldColumn)clsgrid.getColumnByName("flagType");
            flagtype.addValueListItem(1, "Duplicate expense");
            flagtype.addValueListItem(2, "Limit without a receipt");
            flagtype.addValueListItem(3, "Limit with a receipt");
            flagtype.addValueListItem(4, "Item on a weekend");
            flagtype.addValueListItem(5, "Invalid date");
            flagtype.addValueListItem(6, "Frequency of item (count)");
            flagtype.addValueListItem(7, "Frequency of item (sum)");
            flagtype.addValueListItem(8, "Group limit without a receipt");
            flagtype.addValueListItem(9, "Group limit with a receipt");
            flagtype.addValueListItem(10, "Custom");
            flagtype.addValueListItem(11, "Aggregate");
            flagtype.addValueListItem(12, "Item not reimbursable");
            flagtype.addValueListItem(13, "Unused advance available");
            flagtype.addValueListItem(14, "Tip limit exceeded");
            flagtype.addValueListItem(15, "Home to location greater");
            flagtype.addValueListItem(16, "Recommended mileage exceeded");
            flagtype.addValueListItem(17, "Item reimbursable");
            flagtype.addValueListItem(20, "One item in a group");
            flagtype.addValueListItem(19, "Passenger limit");
            flagtype.addValueListItem(18, "Receipt not attached");
            flagtype.addValueListItem(21, "Only allow journeys which start and end at home or office");
            flagtype.addValueListItem(22, "Restrict number of miles per day");
            return clsgrid.generateGrid();
            
        }
    }
}
