namespace Spend_Management.shared.admin
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    using SpendManagementLibrary;

    /// <summary>
    /// Add Edit approval matrix.
    /// </summary>
    public partial class aeApprovalMatrix : Page
    {
        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.enablenavigation = false;
            this.Master.UseDynamicCSS = true;
            this.Master.helpid = 1011;

            if (IsPostBack)
            {
                return;
            }

            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ApprovalMatrix, true, true);

            var approvalMatrices = new code.ApprovalMatrix.ApprovalMatrices(currentUser.AccountID);

            this.txtDefaultApprover_ID.Text = "";

            int matrixId;
            bool addingNew = false;

            if (Request.QueryString["matrixid"] != null)
            {
                if (!int.TryParse(Request.QueryString["matrixid"], out matrixId))
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }

                ApprovalMatrix currentMatrix = approvalMatrices.GetById(matrixId);

                if (currentMatrix == null)
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }

                if (!currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ApprovalMatrix, true))
                {
                    btnSaveEntity.Visible = false;
                    btnSaveLevel.Visible = false;
                    lnkNewLevel.Visible = false;
                }

                this.Master.title = string.Format("Approval Matrix: {0}", currentMatrix.Name);

                this.txtmatrixname.Text = currentMatrix.Name;
                this.txtdescription.Text = currentMatrix.Description;
                currentMatrix.DefaultApproverFriendlyName = approvalMatrices.GetFriendlyNameOfApprover(currentMatrix.DefaultApproverKey);
                this.txtDefaultApprover_ID.Text = currentMatrix.DefaultApproverKey;
                this.txtDefaultApprover.Text = currentMatrix.DefaultApproverFriendlyName;
            }
            else
            {
                matrixId = 0;
                currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ApprovalMatrix, true, true);
                this.Master.title = "New Approval Matrix";
                addingNew = true;
            }

            this.Master.PageSubTitle = "Approval Matrix Details";

            var tables = new cTables(currentUser.AccountID);

            // bind the jQuery auto complete to the txtUser field
            List<object> acParams = AutoComplete.getAutoCompleteQueryParams("signoffentities");
            string acBindGeneralStr = AutoComplete.createAutoCompleteBindString("txtDefaultApprover", 15, tables.GetTableByName("signoffentities").TableID, (Guid)acParams[0], (List<Guid>)acParams[1], 500, keyFieldIsString: true);
            string acBindLevelsStr = AutoComplete.createAutoCompleteBindString("txtLevelApprover", 15, tables.GetTableByName("signoffentities").TableID, (Guid)acParams[0], (List<Guid>)acParams[1], 500, keyFieldIsString: true);

            ClientScriptManager scmgr = this.ClientScript;
            scmgr.RegisterStartupScript(this.GetType(), "autocompleteBindGeneral", AutoComplete.generateScriptRegisterBlock(new List<string>() { acBindGeneralStr }), true);
            scmgr.RegisterStartupScript(this.GetType(), "autocompleteBindLevels", AutoComplete.generateScriptRegisterBlock(new List<string>() { acBindLevelsStr }), true);

            ClientScript.RegisterStartupScript(GetType(), "variables", "SEL.ApprovalMatrices.IDs.MatrixId = " + matrixId + ";\n", true);

            var gridData = approvalMatrices.GetLevelGrid(matrixId, addingNew);
            this.litgrid.Text = gridData[1];
            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "gridVars", cGridNew.generateJS_init("gridVars", new List<string>() { gridData[0] }, currentUser.CurrentActiveModule), true);
        }
    }
}