namespace Spend_Management.shared.admin
{
    using System;
    using SpendManagementLibrary;
    using Spend_Management;
    using Spend_Management.shared.code;
    using System.Collections.Generic;
    public partial class AuthoriserLevel : System.Web.UI.Page
    {
        /// <summary>
        /// Current user object
        /// </summary>
        CurrentUser currentUser;
        /// <summary>
        /// Called when page is loaded.
        /// </summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="e">Events argument</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            var title = "Authoriser Levels";
            this.Master.Page.Title = title;
            this.Master.PageSubTitle = title;
            this.Master.UseDynamicCSS = true;

            currentUser = cMisc.GetCurrentUser();
            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuthoriserLevel, true, true);

            if (IsPostBack == false)
            {
                lblNewAuthoriserLevel.Visible = currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.AuthoriserLevel, true);
                ViewState["accountid"] = currentUser.AccountID;
                ViewState["employeeid"] = currentUser.EmployeeID;

                const string sSQL = "SELECT AuthoriserLevelDetailId,Amount,Description FROM AuthoriserLevelDetails ";

                cGridNew gridAuthoriserLevelDetail = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridAuthoriserLevelDetailGrid", sSQL);
                gridAuthoriserLevelDetail.KeyField = "AuthoriserLevelDetailId";
                gridAuthoriserLevelDetail.EmptyText = "There are currently no authoriser levels defined.";
                gridAuthoriserLevelDetail.getColumnByName("AuthoriserLevelDetailId").hidden = true;
                gridAuthoriserLevelDetail.enabledeleting = currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.AuthoriserLevel, true);
                gridAuthoriserLevelDetail.enableupdating = currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.AuthoriserLevel, true);
                gridAuthoriserLevelDetail.editlink = "AuthoriserLevelDetails.aspx?authoriserLevelDetailId={AuthoriserLevelDetailId}";
                gridAuthoriserLevelDetail.deletelink = "javascript:SEL.AuthoriserLevel.Menu.Delete({AuthoriserLevelDetailId});";

                var amount = 0;
                var clsfields = new cFields(currentUser.AccountID);

                gridAuthoriserLevelDetail.addFilter(clsfields.GetFieldByID(new Guid("78FB2B48-7462-48FE-9E1E-A0D3E4A4FB4C")),
                        ConditionType.GreaterThanEqualTo, new object[] { amount }, null, ConditionJoiner.And);
                gridAuthoriserLevelDetail.SortedColumn = gridAuthoriserLevelDetail.getColumnByName("Amount");
                gridAuthoriserLevelDetail.SortDirection = SpendManagementLibrary.SortDirection.Ascending;

                string[] gridData = gridAuthoriserLevelDetail.generateGrid();
                litAuthoriserLevelDetailGrid.Text = gridData[1];
                Page.ClientScript.RegisterStartupScript(this.GetType(), "vars", cGridNew.generateJS_init("vars", new System.Collections.Generic.List<string>() { gridData[0] }, currentUser.CurrentActiveModule), true);
            }

            var tables = new cTables(currentUser.AccountID);

            // bind the jQuery auto complete to the txtUser field
            List<object> acParam = AutoComplete.getAutoCompleteQueryParams("EmployeeView");

            string acBindEmployeeName = AutoComplete.createAutoCompleteBindString("txtDefaultApprover", 15, tables.GetTableByName("EmployeeView").TableID, (Guid)acParam[0], (List<Guid>)acParam[1], 500, keyFieldIsString: false);
            System.Web.UI.ClientScriptManager scmgr = this.ClientScript;
            scmgr.RegisterStartupScript(this.GetType(), "autocompleteBindDefaultApprover", AutoComplete.generateScriptRegisterBlock(new List<string>() { acBindEmployeeName }), true);
            
            var authoriserLevelDetail = new AuthoriserLevelDetail();
            var defaultEmployee = authoriserLevelDetail.GetDefultAuthoriserEmployee();
            
            txtDefaultApprover_ID.Text = defaultEmployee.EmployeeId.ToString();
            txtDefaultApprover.Text = defaultEmployee.FullName;
            var employee = new cEmployees(currentUser.AccountID);
            var currency = employee.GetCurrencySymbol(currentUser, false);
            lblnamemsg.Text = lblnamemsg.Text + currency;
        }

        /// <summary>
        ///  Close current screen.
        /// </summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="e">Event arguments</param>
        protected void cmdClose_Click(object sender, EventArgs e)
        {
            if (currentUser == null)
            {
                currentUser = cMisc.GetCurrentUser();
            }
            switch (currentUser.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                    break;
                case Modules.SpendManagement:
                    break;
                case Modules.contracts:
                    Response.Redirect("~/MenuMain.aspx?menusection=employee", true);
                    break;
                default:
                    Response.Redirect("~/usermanagementmenu.aspx", true);
                    break;
            }
        }
    }
}