namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Web;
    using System.Web.UI;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.HelpAndSupport;

    public partial class helpAndSupportTickets : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = cMisc.GetCurrentUser();
            if (user.isDelegate)
            {
                Response.Redirect("~/restricted.aspx", true);
            }
            // if there's nothing to see on this page redirect back to help and support home
            cAccountProperties accountProperties = new cAccountSubAccounts(user.AccountID).getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            if (user.Account.ContactHelpDeskAllowed == false && accountProperties.EnableInternalSupportTickets == false && user.Employee.ContactHelpDeskAllowed == false) 
            {
                Response.Redirect("helpAndSupport.aspx");
            }

            this.circleLink.Visible = Knowledge.CanAccessCircle(user);

            var tables = new cTables(user.AccountID);
            var fields = new cFields(user.AccountID);

            if (user.Account.ContactHelpDeskAllowed || user.Employee.ContactHelpDeskAllowed)
            {
                this.divSalesForceTickets.Visible = true;

                DataSet ticketData = SupportTicketSalesForce.GetDataset(user.Account.companyid, user);

                var salesForceTicketsGridColumns = new List<cNewGridColumn>
                                {
                                    new cFieldColumn(fields.GetFieldByID(new Guid("5542DC9A-D009-44A9-8425-B0A009D58697"))), // SupportTicketId
                                    new cFieldColumn(fields.GetFieldByID(new Guid("4433DFB1-C89A-4F88-A1F9-D5358008CD80"))), // CaseNumber
                                    new cFieldColumn(fields.GetFieldByID(new Guid("2E5A3CF3-B48F-4B35-BB5E-70943030FF00"))), // Subject
                                    new cFieldColumn(fields.GetFieldByID(new Guid("4220F88E-491A-4B73-805E-BE3A3BE13EE7"))), // Status
                                    new cFieldColumn(fields.GetFieldByID(new Guid("9119EB04-6BAA-4261-BBE4-09A7DAD22F47"))), // CreatedOn
                                    new cFieldColumn(fields.GetFieldByID(new Guid("50AA7D78-7502-40E2-9047-FE3D851EB763")))  // ModifiedOn
                                };

                var salesForceTicketsGrid = new cGridNew(user.AccountID, user.EmployeeID, "gridSalesForceTickets", tables.GetTableByID(new Guid("741A0EB4-3589-4F02-9043-A9C564A317F5")), salesForceTicketsGridColumns, ticketData)
                {
                    editlink = "/shared/helpAndSupportTicket.aspx?TicketType=2&SupportTicketId={SupportTicketId}",
                    enableupdating = true,
                    enablepaging = true,
                    KeyField = "SupportTicketId",
                    pagesize = 10,
                    EmptyText = "You do not have any existing support tickets with Selenity."
                };

                salesForceTicketsGrid.getColumnByName("SupportTicketId").hidden = true;
                salesForceTicketsGrid.getColumnByName("CaseNumber").HeaderText = "Ticket ID";

                string[] salesForceGridStrings = salesForceTicketsGrid.generateGrid();
                TicketsSalesForce.Text = salesForceGridStrings[1];

                this.ClientScript.RegisterStartupScript(this.GetType(), "salesForceTicketsGridJavaScript", cGridNew.generateJS_init("salesForceTicketsGridJavaScript", new List<string> { salesForceGridStrings[0] }, user.CurrentActiveModule), true);
            }

            if (accountProperties.EnableInternalSupportTickets)
            {
                this.divInternalTickets.Visible = true;

                var internalTicketsGrid = new cGridNew(user.AccountID, user.EmployeeID, "gridInternalTickets", "SELECT [SupportTicketId], [Subject], [Status], [CreatedOn], [ModifiedOn] FROM [SupportTickets]")
                {
                    editlink = "/shared/helpAndSupportTicket.aspx?TicketType=1&SupportTicketId={SupportTicketId}",
                    enableupdating = true,
                    enablepaging = true,
                    KeyField = "SupportTicketId",
                    pagesize = 10,
                    EmptyText = "You do not have any existing support tickets with your administrator."
                };

                internalTicketsGrid.addFilter(fields.GetFieldByID(new Guid("4133E2C8-285D-4283-8D99-1A5C6234C442")), ConditionType.Equals, new object[] { user.EmployeeID }, null, ConditionJoiner.None);
                internalTicketsGrid.getColumnByName("SupportTicketId").HeaderText = "Ticket ID";
                ((cFieldColumn)internalTicketsGrid.getColumnByName("Status")).addValueListItems(typeof(SupportTicketStatus));

                string[] internalGridStrings = internalTicketsGrid.generateGrid();
                TicketsInternal.Text = internalGridStrings[1];

                this.ClientScript.RegisterStartupScript(this.GetType(), "internalTicketsGridJavaScript", cGridNew.generateJS_init("internalTicketsGridJavaScript", new List<string> { internalGridStrings[0] }, user.CurrentActiveModule), true);
            }

            this.Title = "My Tickets";
            this.Master.PageSubTitle = this.Title;
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