namespace expenses
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Web.Services;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary.Employees;
    using Spend_Management;
    using SpendManagementLibrary;
    using Spend_Management.shared.code.ApprovalMatrix;

    /// <summary>
    /// Summary description for aegroup.
    /// </summary>
    public partial class aegroup : Page
    {
        private const string EditAction = "2";
        private readonly string SaveErrorMessage = "<br/><strong>There are configuration errors: </strong><br/>{0}";

        string action;
        int groupId = 0;
        protected System.Web.UI.WebControls.ImageButton cmdhelp;
        cGroups groups;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Add / Edit Signoff Group";
            Master.title = Title;
            Master.showdummymenu = true;
            Master.helpid = 1033;

            if (this.IsPostBack)
            {
                return;
            }

            this.Master.enablenavigation = false;
            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SignOffGroups, true, true);
            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;
            this.ViewState["claimsInProcess"] = false;
            this.groups = new cGroups(user.AccountID);

            this.ClientScript.RegisterHiddenField("accountid", user.AccountID.ToString(CultureInfo.InvariantCulture));
            if (this.Request.QueryString["action"] != null)
            {
                this.action = this.Request.QueryString["action"];
            }

            if (this.action != EditAction)
            {
                return;
            }

            this.groupId = int.Parse(this.Request.QueryString["groupid"]);
            int claimCount = this.groups.getCountOfClaimsInProcessByGroupID(this.groupId);
            
            if (claimCount > 0)
            {
                this.lblclaimsinprocess.Visible = true;
                this.lnkAddStage.Visible = false;
                this.ViewState["claimsInProcess"] = true;
            }
            
            cGroup reqgroup = this.groups.GetGroupById(this.groupId);
            this.txtaction.Text = "2";
            this.txtgroupid.Text = this.groupId.ToString();
            this.txtgroupname.Text = reqgroup.groupname;
            this.ViewState["groupid"] = this.groupId;
            this.txtdescription.Text = reqgroup.description;
            this.chkAllowOneStepAuthorisation.Checked = reqgroup.oneclickauthorisation;

            if (this.Request["created"] != null)
            {
                this.lblmsg.Text = string.Format(this.SaveErrorMessage, GroupStageValidationResult.NoStages);
                this.lblmsg.Visible = true;
            }

            this.gridstages.DataSource = this.groups.getStagesGrid(reqgroup);
            this.gridstages.DataBind();
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridstages.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(this.gridstages_InitializeRow);
            this.gridstages.InitializeLayout += new Infragistics.WebUI.UltraWebGrid.InitializeLayoutEventHandler(this.gridstages_InitializeLayout);
            this.gridstages.ClickCellButton += new Infragistics.WebUI.UltraWebGrid.ClickCellButtonEventHandler(this.gridstages_ClickCellButton);
            this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);
            this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);

        }
        #endregion

        private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            groups = new cGroups((int)ViewState["accountid"]);

            action = txtaction.Text;
            var isEdit = action == EditAction;
            string groupName = this.txtgroupname.Text;
            string description = this.txtdescription.Text;
            bool oneClickAuth = this.chkAllowOneStepAuthorisation.Checked;
            CurrentUser user = cMisc.GetCurrentUser();
            cGroup currentGroup;

            if (isEdit)
            {
                groupId = int.Parse(txtgroupid.Text);
                currentGroup = groups.GetGroupById(groupId);
                groups.SaveGroup(groupId, groupName, description, oneClickAuth, user, 2,
                    currentGroup.NotifyClaimantWhenEnvelopeReceived, currentGroup.NotifyClaimantWhenEnvelopeNotReceived);
            }
            else
            {
                groupId = groups.SaveGroup(groupId, groupName, description, oneClickAuth, user, 0);
                currentGroup = groups.GetGroupById(groupId);

                if (groupId > -1)
                {
                    var url = Request.Url.PathAndQuery + string.Format("?action={0}&groupId={1}&created=true", EditAction, groupId);
                    Response.Redirect(url, true);
                }
                else
                {
                    this.txtaction.Text = this.txtgroupid.Text = "0";
                    this.lblmsg.Text = string.Format(this.SaveErrorMessage, GroupStageValidationResult.AlreadyExists);
                    this.lblmsg.Visible = true;
                    this.lnkAddStage.Visible = false;
                    return;
                }
            }

            // validate the group's stages.
            var stageValidity = groups.ValidateGroupStages(currentGroup);

            if (!stageValidity.Result)
            {
                this.lblmsg.Text = string.Format(this.SaveErrorMessage, stageValidity.Messages.Aggregate((a, b) => a + "<br/>" + b));
                this.lblmsg.Visible = true;
                return;
            }

            Response.Redirect("admingroups.aspx", true);
        }


        private void gridstages_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.FromKey("signoffid").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("amount").Hidden = true;

            e.Layout.Bands[0].Columns.FromKey("signoffid").Width = 0;
            e.Layout.Bands[0].Columns.FromKey("amount").Width = 0;

            if ((bool)ViewState["claimsInProcess"] == false)
            {
                e.Layout.Bands[0].Columns.Insert(1, "edit");
                e.Layout.Bands[0].Columns.FromKey("edit").HeaderText = "<img alt=\"Edit\" src=\"../icons/edit_blue.gif\" />";
                e.Layout.Bands[0].Columns.FromKey("edit").Type = Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink;

                e.Layout.Bands[0].Columns.Insert(2, "delete");
                e.Layout.Bands[0].Columns.FromKey("delete").HeaderText = "<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\" />";
                e.Layout.Bands[0].Columns.FromKey("delete").Type = Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink;
            }
            e.Layout.Bands[0].Columns.FromKey("signofftype").HeaderText = "Type";
            e.Layout.Bands[0].Columns.FromKey("relid").HeaderText = "Assignee";
            e.Layout.Bands[0].Columns.FromKey("include").HeaderText = "Include Type";
            e.Layout.Bands[0].Columns.FromKey("notify").HeaderText = "Action";

            e.Layout.Bands[0].Columns.FromKey("stage").HeaderText = "Stage";
            e.Layout.Bands[0].Columns.FromKey("relid").Width = Unit.Pixel(150);
            e.Layout.Bands[0].Columns.FromKey("cleanupstage").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("allocateforpayment").Hidden = true;

            // create a new list of each signoff type.
            var lststagetype = new Infragistics.WebUI.UltraWebGrid.ValueList();
            lststagetype.ValueListItems.Add(SignoffType.BudgetHolder.ToInfragisticsValueListItem());
            lststagetype.ValueListItems.Add(SignoffType.Employee.ToInfragisticsValueListItem());
            lststagetype.ValueListItems.Add(SignoffType.Team.ToInfragisticsValueListItem());
            lststagetype.ValueListItems.Add(SignoffType.LineManager.ToInfragisticsValueListItem());
            lststagetype.ValueListItems.Add(SignoffType.ClaimantSelectsOwnChecker.ToInfragisticsValueListItem());
            lststagetype.ValueListItems.Add(SignoffType.ApprovalMatrix.ToInfragisticsValueListItem());
            lststagetype.ValueListItems.Add(SignoffType.DeterminedByClaimantFromApprovalMatrix.ToInfragisticsValueListItem());
            lststagetype.ValueListItems.Add(SignoffType.CostCodeOwner.ToInfragisticsValueListItem());
            lststagetype.ValueListItems.Add(SignoffType.AssignmentSignOffOwner.ToInfragisticsValueListItem());
            lststagetype.ValueListItems.Add(SignoffType.SELScanAttach.ToInfragisticsValueListItem());
            lststagetype.ValueListItems.Add(SignoffType.SELValidation.ToInfragisticsValueListItem());
            e.Layout.Bands[0].Columns.FromKey("signofftype").ValueList = lststagetype;
            e.Layout.Bands[0].Columns.FromKey("signofftype").Type = Infragistics.WebUI.UltraWebGrid.ColumnType.DropDownList;

            if ((bool)ViewState["claimsInProcess"] == false)
            {
                e.Layout.Bands[0].Columns.FromKey("edit").Width = 15;
                e.Layout.Bands[0].Columns.FromKey("edit").CellStyle.Cursor = Infragistics.WebUI.Shared.Cursors.Hand;
                e.Layout.Bands[0].Columns.FromKey("edit").CellStyle.Font.Underline = true;
                e.Layout.Bands[0].Columns.FromKey("edit").CellStyle.ForeColor = Color.Blue;
                e.Layout.Bands[0].Columns.FromKey("edit").CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                e.Layout.Bands[0].Columns.FromKey("edit").CellButtonStyle.Cursor = Infragistics.WebUI.Shared.Cursors.Hand;
                e.Layout.Bands[0].Columns.FromKey("edit").CellButtonStyle.Font.Underline = true;
                e.Layout.Bands[0].Columns.FromKey("edit").CellButtonStyle.ForeColor = Color.Blue;
                e.Layout.Bands[0].Columns.FromKey("edit").CellButtonStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                e.Layout.Bands[0].Columns.FromKey("edit").CellButtonStyle.BackColor = Color.Transparent;


                e.Layout.Bands[0].Columns.FromKey("delete").CellStyle.Cursor = Infragistics.WebUI.Shared.Cursors.Hand;
                e.Layout.Bands[0].Columns.FromKey("delete").Width = 15;
                e.Layout.Bands[0].Columns.FromKey("delete").CellStyle.Font.Underline = true;
                e.Layout.Bands[0].Columns.FromKey("delete").CellStyle.ForeColor = Color.Blue;
                e.Layout.Bands[0].Columns.FromKey("delete").CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                e.Layout.Bands[0].Columns.FromKey("delete").CellButtonStyle.Cursor = Infragistics.WebUI.Shared.Cursors.Hand;
                e.Layout.Bands[0].Columns.FromKey("delete").CellButtonStyle.Font.Underline = true;
                e.Layout.Bands[0].Columns.FromKey("delete").CellButtonStyle.ForeColor = Color.Blue;
                e.Layout.Bands[0].Columns.FromKey("delete").CellButtonStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
                e.Layout.Bands[0].Columns.FromKey("delete").CellButtonStyle.BackColor = Color.Transparent;

                e.Layout.Bands[0].Columns.FromKey("edit").AllowResize = Infragistics.WebUI.UltraWebGrid.AllowSizing.Fixed;
                e.Layout.Bands[0].Columns.FromKey("delete").AllowResize = Infragistics.WebUI.UltraWebGrid.AllowSizing.Fixed;

                e.Layout.Bands[0].Columns.FromKey("edit").HeaderClickAction = Infragistics.WebUI.UltraWebGrid.HeaderClickAction.Select;
                e.Layout.Bands[0].Columns.FromKey("delete").HeaderClickAction = Infragistics.WebUI.UltraWebGrid.HeaderClickAction.Select;
            }
        }

        private void gridstages_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            int accountId = (int)ViewState["accountid"];
            var cleanupstage = (bool)e.Row.Cells.FromKey("cleanupstage").Value;
            var allocateforpayment = (bool)e.Row.Cells.FromKey("allocateforpayment").Value;
            if ((bool)ViewState["claimsInProcess"] == false)
            {
                e.Row.Cells.FromKey("edit").Value =
                    string.Format(
                        "<a href=\"aestage.aspx?action=2&groupid={0}&signoffid={1}\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a>",
                        int.Parse(this.txtgroupid.Text),
                        e.Row.Cells.FromKey("signoffid").Value);

                if (!cleanupstage)
                {
                    e.Row.Cells.FromKey("delete").Value =
                        string.Format(
                            "<a href=\"javascript:deleteStage({0},{1});\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a>",
                            int.Parse(this.txtgroupid.Text),
                            e.Row.Cells.FromKey("signoffid").Value);
                }
            }

            var employees = new cEmployees(accountId);
            Employee reqemployee;

            int relId = int.Parse(e.Row.Cells.FromKey("relid").Value.ToString());

            switch ((SignoffType)int.Parse(e.Row.Cells.FromKey("signofftype").Value.ToString()))
            {
                case SignoffType.BudgetHolder:
                    cBudgetholders holders = new cBudgetholders(accountId);
                    cBudgetHolder reqHolder = holders.getBudgetHolderById(relId);
                    int employeeid = reqHolder.employeeid;
                    reqemployee = employees.GetEmployeeById(employeeid);
                    e.Row.Cells.FromKey("relid").Value = reqHolder.budgetholder + " (" + reqemployee.Surname + ", " + reqemployee.Title + " " + reqemployee.Forename + ")";
                    break;
                case SignoffType.Employee:
                    reqemployee = employees.GetEmployeeById(relId);
                    e.Row.Cells.FromKey("relid").Value = reqemployee.Surname + ", " + reqemployee.Title + " " + reqemployee.Forename;
                    break;
                case SignoffType.Team:
                    cTeams teams = new cTeams(accountId);
                    cTeam reqteam = teams.GetTeamById(relId);
                    e.Row.Cells.FromKey("relid").Value = reqteam.teamname;
                    break;
                case SignoffType.CostCodeOwner:
                case SignoffType.LineManager:
                case SignoffType.AssignmentSignOffOwner:
                case SignoffType.SELScanAttach:
                case SignoffType.SELValidation:
                    e.Row.Cells.FromKey("relid").Value = string.Empty;
                    break;
                case SignoffType.ApprovalMatrix:
                case SignoffType.DeterminedByClaimantFromApprovalMatrix:
                    ApprovalMatrix matrix = new ApprovalMatrices(accountId).GetById(relId);
                    e.Row.Cells.FromKey("relid").Value = matrix.Name;
                    break;
            }

            switch (int.Parse(e.Row.Cells.FromKey("include").Value.ToString()))
            {
                case 1:
                    e.Row.Cells.FromKey("include").Value = "Always include stage";
                    break;
                case 2:
                    e.Row.Cells.FromKey("include").Value = "Only include stage if claim amount is > " + decimal.Parse(e.Row.Cells.FromKey("amount").Value.ToString()).ToString("###,###,##0.00");
                    break;
                case 3:
                    e.Row.Cells.FromKey("include").Value = "Only if an item exceeds allowed amount";
                    break;
                case 4:
                    e.Row.Cells.FromKey("include").Value = "Only if claim includes specified cost code";
                    break;
                case 5:
                    e.Row.Cells.FromKey("include").Value = "Only include stage if claim amount is < " + decimal.Parse(e.Row.Cells.FromKey("amount").Value.ToString()).ToString("###,###,##0.00");
                    break;
                case 6:
                    e.Row.Cells.FromKey("include").Value = "Only if claim include specified expense item";
                    break;
                case 7:
                    e.Row.Cells.FromKey("include").Value = "Only if claim includes an expense item older than " + e.Row.Cells.FromKey("amount").Value.ToString() + " days";
                    break;
                case 8:
                    e.Row.Cells.FromKey("include").Value = "Only if claim includes specified department ";
                    break;
                case 9:
                    e.Row.Cells.FromKey("include").Value = "Only if an expense item fails validation twice";
                    break;
            }

            switch (int.Parse(e.Row.Cells.FromKey("notify").Value.ToString()))
            {
                case 1:
                    e.Row.Cells.FromKey("notify").Value = string.Format("Stage is notified of claim {0}{1}", allocateforpayment ? "(Allocate for payment)" : string.Empty, cleanupstage ? "(Verify)" : string.Empty);
                    break;
                case 2:
                    e.Row.Cells.FromKey("notify").Value = string.Format("Stage is to check claim {0}{1}", allocateforpayment ? "(Allocate for payment)" : string.Empty, cleanupstage ? "(Verify)" : string.Empty);
                    break;
            }
        }

        private void gridstages_ClickCellButton(object sender, Infragistics.WebUI.UltraWebGrid.CellEventArgs e)
        {
            int signOffId = int.Parse(e.Cell.Row.Cells.FromKey("signoffid").Value.ToString());
            int groupId = int.Parse(this.txtgroupid.Text);

            if (e.Cell.Column.Key == "edit")
            {
                Response.Redirect("aestage.aspx?action=2&groupid=" + groupId + "&signoffid=" + signOffId, true);
            }
            else
            {
                groups = new cGroups((int)ViewState["accountid"]);
                cGroup reqGroup = groups.GetGroupById(groupId);
                groups.deleteStage(reqGroup, signOffId);
                gridstages.DataSource = groups.getStagesGrid(reqGroup);
                gridstages.DataBind();
            }
        }

        protected void lnkAddStage_Click(object sender, System.EventArgs e)
        {
            action = txtaction.Text;
            groups = new cGroups((int)ViewState["accountid"]);
            CurrentUser user = cMisc.GetCurrentUser();

            string groupName = this.txtgroupname.Text;
            string description = this.txtdescription.Text;
            bool onecClickAuth = this.chkAllowOneStepAuthorisation.Checked;
            int act = 0;

            var group = (action == EditAction)
                ? groups.GetGroupById(int.Parse(txtgroupid.Text))
                : new cGroup(this.groups.AccountId, this.groupId, groupName, description, onecClickAuth, DateTime.UtcNow,
                    user.EmployeeID, DateTime.UtcNow, user.EmployeeID, new SerializableDictionary<int, cStage>(), notifyClaimantWhenEnvelopeReceived: false, notifyClaimantWhenEnvelopeNotReceived: false);

            int outcome = groups.SaveGroup(group.groupid, group.groupname, group.description,
                group.oneclickauthorisation, user, group.groupid == 0 ? 0 : 2,
                group.NotifyClaimantWhenEnvelopeReceived, group.NotifyClaimantWhenEnvelopeNotReceived);

            if (outcome == -1)
            {
                this.lblmsg.Text = string.Format(this.SaveErrorMessage, GroupStageValidationResult.AlreadyExists);
                this.lblmsg.Visible = true;
                return;
            }

            Response.Redirect("aestage.aspx?groupid=" + outcome, true);
        }

        private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            action = this.txtaction.Text;
            groups = new cGroups((int)ViewState["accountid"]);

            if (action == EditAction) // edit
            {
                groupId = int.Parse(txtgroupid.Text);
                var group = groups.GetGroupById(groupId);
                var stageValidity = groups.ValidateGroupStages(group);

                if (!stageValidity.Result)
                {
                    this.lblmsg.Text = string.Format(this.SaveErrorMessage, stageValidity.Messages.Aggregate((a, b) => a + "<br/>" + b));
                    this.lblmsg.Visible = true;
                    return;
                }
            }

            Response.Redirect("admingroups.aspx", true);
        }

        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);
            gridstages.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridstages_InitializeDataSource);
        }

        void gridstages_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            if (this.ViewState["groupid"] == null)
            {
                return;
            }

            cGroups groups = new cGroups((int)this.ViewState["accountid"]);
            cGroup reqgroup = groups.GetGroupById((int)this.ViewState["groupid"]);
            this.gridstages.DataSource = groups.getStagesGrid(reqgroup);
            this.gridstages.DataBind();
        }

        [WebMethod(EnableSession = true)]
        public static byte DeleteStage(int accountId, int groupId, int signOffId)
        {
            cGroups groups = new cGroups(accountId);
            cGroup reqGroup = groups.GetGroupById(groupId);
            return groups.deleteStage(reqGroup, signOffId);
        }

    }
}
