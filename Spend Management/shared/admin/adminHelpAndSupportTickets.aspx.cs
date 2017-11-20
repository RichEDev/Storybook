using System.Web;
using SpendManagementLibrary.HelpAndSupport;

namespace Spend_Management.shared.admin
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Helpers;

    public partial class adminHelpAndSupportTickets : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupportTickets, true, true);

            var itemValues = Enum.GetValues(typeof(SupportTicketStatus));
            var itemNames = Enum.GetNames(typeof(SupportTicketStatus));

            this.cmbStatus.Items.Add(new ListItem("All Tickets", "[None]"));
            for (int i = 0; i <= itemNames.Length - 1; i++)
            {
                this.cmbStatus.Items.Add(new ListItem(itemNames.GetValue(i).ToString().SplitCamel(), Convert.ToInt32(itemValues.GetValue(i)).ToString()));
            }

            var fields = new cFields(user.AccountID);
            var tables = new cTables(user.AccountID);

            var columns = new List<cNewGridColumn>
                          {
                              new cFieldColumn(fields.GetFieldByTableAndFieldName("SupportTickets", "SupportTicketId")), 
                              new cFieldColumn(fields.GetFieldByTableAndFieldName("SupportTickets", "Subject")), 
                              new cFieldColumn(fields.GetFieldByTableAndFieldName("SupportTickets", "Status")), 
                              new cFieldColumn(fields.GetFieldByTableAndFieldName("SupportTickets", "CreatedOn")), 
                              new cFieldColumn(fields.GetFieldByTableAndFieldName("SupportTickets", "ModifiedOn")), 
                              new cFieldColumn(fields.GetFieldByTableAndFieldName("employees", "dbo.getEmployeeFullname(employees.employeeid)"), new JoinVia(0, string.Empty, Guid.NewGuid(), new SortedList<int, JoinViaPart>() { { 0, new JoinViaPart(fields.GetFieldByTableAndFieldName("SupportTickets", "Owner").FieldID, JoinViaPart.IDType.Field) } }))
                          };

            var internalTicketsGrid = new cGridNew(user.AccountID, user.EmployeeID, "gridInternalTickets", tables.GetTableByName("SupportTickets"), columns)
            {
                editlink = "/shared/admin/adminHelpAndSupportTicket.aspx?SupportTicketId={SupportTicketId}",
                enableupdating = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.SupportTickets, true, false),
                enablepaging = true,
                KeyField = "SupportTicketId",
                pagesize = 20,
                EmptyText = "There are currently no Support Tickets.",
                WhereClause = string.Empty
            };

            internalTicketsGrid.getColumnByName("SupportTicketId").HeaderText = "Ticket ID";
            ((cFieldColumn)internalTicketsGrid.getColumnByName("Status")).addValueListItems(typeof(SupportTicketStatus));

            string[] internalGridStrings = internalTicketsGrid.generateGrid();
            litInternalTickets.Text = internalGridStrings[1];

            this.ClientScript.RegisterStartupScript(this.GetType(), "internalTicketsGridJavaScript", cGridNew.generateJS_init("internalTicketsGridJavaScript", new List<string> { internalGridStrings[0] }, user.CurrentActiveModule), true);

            Title = "Support Tickets";
            Master.title = Title;
        }

        /// <summary>
        /// Close button event method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CmdClose_Click(object sender, EventArgs e)
        {
            string previousUrl = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(previousUrl, true);
        }
    }
}