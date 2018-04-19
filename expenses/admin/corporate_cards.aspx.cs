namespace expenses.admin
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic;

    using Spend_Management;

    using SpendManagementLibrary;

    using WebBootstrap;

    public partial class corporate_cards : System.Web.UI.Page
    {
        [Dependency]
        public ActionContext ActionContext { get; set; }

        /// <summary>
        /// Page load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Corporate Card Providers";
            this.Master.title = this.Title;

            if (IsPostBack == false)
            {
                this.ActionContext.CurrentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CorporateCards, true, true);

                cAccount reqAccount =  this.ActionContext.Accounts.GetAccountByID(this.ActionContext.AccountId);

                if (reqAccount.CorporateCardsEnabled == false)
                {
                    Response.Redirect("~/home.aspx", true);
                }

                string[] gridData = this.CreateCorporateCardProviderGrid();
                this.litgrid.Text = gridData[1];

                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "CorpCardGridVars", cGridNew.generateJS_init("CorpCardGridVars", new List<string>() { gridData[0] }, this.ActionContext.CurrentUser.CurrentActiveModule), true);
            }
        }

        /// <summary>
        /// Create cGridNew
        /// </summary>
        /// <returns>A string array</returns>
        protected string[] CreateCorporateCardProviderGrid()
        {
            var baseTable = this.ActionContext.Tables.GetTableByID(new Guid(ReportTable.CardProviders));
            var columns = new List<cNewGridColumn>
            {
                new cFieldColumn(this.ActionContext.Fields.GetFieldByID(new Guid(ReportKeyFields.CardProvidersCardProviderId))),
                new cFieldColumn(this.ActionContext.Fields.GetFieldByID(new Guid(ReportFields.CardProvidersAutoImport))),
                new cFieldColumn(this.ActionContext.Fields.GetFieldByID(new Guid(ReportFields.CardProvidersCardProvider))),
                new cFieldColumn(this.ActionContext.Fields.GetFieldByID(new Guid(ReportFields.CardProvidersClaimantsSettleBill)))
            };

            var grid = new cGridNew(this.ActionContext.CurrentUser.AccountID, this.ActionContext.CurrentUser.EmployeeID, "gridCorporateCardProviders", baseTable, columns);

            grid.getColumnByName("cardproviderid").hidden = true;
            grid.getColumnByName("autoimport").hidden = true;
            grid.KeyField = "cardprovider";
            grid.getColumnByName("claimants_settle_bill").HeaderText = "Claimants Settle Bill";
            grid.getColumnByName("cardprovider").HeaderText = "Provider";
            grid.EmptyText = "No card providers to display";
            grid.enabledeleting = this.ActionContext.CurrentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.CorporateCards, true, false);
            grid.enableupdating = this.ActionContext.CurrentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.CorporateCards, true, false);
            grid.editlink = "aecorporatecard.aspx?cardproviderid={cardproviderid}";
            grid.deletelink = "javascript:SEL.CorporateCardProviders.DeleteCard({cardproviderid});";
            grid.addTwoStateEventColumn("autoimportLog", (cFieldColumn)grid.getColumnByName("autoimport"), false, true, "", "", "", "", "/shared/images/icons/history2.png", "../shared/admin/corporatecardlogs.aspx?cardproviderid={cardproviderid}", "View Logs", "View Logs");

            grid.addFilter(this.ActionContext.Fields.GetFieldByID(new Guid(ReportFields.CardProvidersClaimantsSettleBill)), ConditionType.ContainsData, null, null, ConditionJoiner.And);


            return grid.generateGrid();
        }

        
    }
}
