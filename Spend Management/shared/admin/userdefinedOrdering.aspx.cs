using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using SpendManagementLibrary;
using System.Text;

namespace Spend_Management
{
    public partial class userdefinedOrderingSummaryPage : System.Web.UI.Page
    {
        CurrentUser currentUser;
        protected void Page_Load(object sender, EventArgs e)
        {
            currentUser = cMisc.GetCurrentUser();
            Title = "Userdefined Ordering";
            Master.title = Title;

            if (!IsPostBack)
            {
                string[] gridData = createGrid();
                litgrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "udfOrderingGridVars", cGridNew.generateJS_init("udfOrderingGridVars", new List<string>() { gridData[0] }, currentUser.CurrentActiveModule), true);
            }
        }

        private string[] createGrid()
        {
            cFields  clsFields = new cFields(currentUser.Account.accountid);
            List<cNewGridColumn> lstColumns = new List<cNewGridColumn>();
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("eacd0e78-7fe6-431d-bd77-08c3b4cb18fb")))); /// ParentTableID
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("86aa4acb-a36b-458c-9b6b-1bc6fe012dcd")))); /// Applies To
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("e4567ea5-e183-4041-8d50-ce1b2e8e903b")))); /// Group Count
            lstColumns.Add(new cFieldColumn(clsFields.GetFieldByID(new Guid("add0613c-5469-4934-85c9-151de79dc2ae")))); /// Field Count;

            cTables clsTables = new cTables(currentUser.Account.accountid);
            cTable reqTable = clsTables.GetTableByID(Guid.Parse("eacd0e78-7fe6-431d-bd77-08c3b4cb18fb"));

            cGridNew grid = new cGridNew(currentUser.Account.accountid, currentUser.Employee.EmployeeID, "udfOrdering", reqTable, lstColumns);
            grid.getColumnByID(new Guid("eacd0e78-7fe6-431d-bd77-08c3b4cb18fb")).hidden = true;
            grid.enableupdating = true;
            grid.editlink = "aeUserdefinedOrdering.aspx?appliesTo={parentTableID}";

            grid.addFilter(clsFields.GetFieldByID(new Guid("e4567ea5-e183-4041-8d50-ce1b2e8e903b")), ConditionType.GreaterThan, new object[] { 0 }, null, ConditionJoiner.And);
            grid.addFilter(clsFields.GetFieldByID(new Guid("add0613c-5469-4934-85c9-151de79dc2ae")), ConditionType.GreaterThan, new object[] { 0 }, null, ConditionJoiner.Or);

            return grid.generateGrid();
        }

        protected void btnCancel_Click(object sender, ImageClickEventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            switch (currentUser.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.contracts:
                    Response.Redirect("~/MenuMain.aspx?menusection=tailoring", true);
                    break;
                default:
                    Response.Redirect("~/tailoringmenu.aspx");
                    break;
            }
        }
    }
}
