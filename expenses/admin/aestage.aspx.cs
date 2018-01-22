namespace expenses
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;

    using Spend_Management;
    using Spend_Management.shared.code.ApprovalMatrix;

    /// <summary>
    ///     Summary description for aestage.
    /// </summary>
    public partial class aestage : Page
    {

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Add / Edit Stage";
            this.Master.title = this.Title;
            this.Master.showdummymenu = true;
            this.Master.enablenavigation = false;
            this.Master.helpid = 1033;
            
            if (this.IsPostBack)
            {
                var cmbinclude_SelectedIndexChanged = "Manual:cmbinclude_SelectedIndexChanged".Equals(Request["__EVENTTARGET"], StringComparison.InvariantCultureIgnoreCase);
                if (cmbinclude_SelectedIndexChanged)
                {
                    this.cmbinclude.SelectedValue = Request["__EVENTARGUMENT"];
                    this.cmbinclude_SelectedIndexChanged(null, EventArgs.Empty);
                }

                if (cmbinclude_SelectedIndexChanged || "ctl00$contentmain$chkPayBeforeValidate".Equals(Request["__EVENTTARGET"], StringComparison.InvariantCultureIgnoreCase))
                {
                    this.litScrollToBottom.Text = "true";
                }

                this.SetupPayBeforeValidate();
                this.ToggleOnHolidaySkip();

                return;
            }

            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SignOffGroups, true, true);
            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;
            
            if (!user.Account.ValidationServiceEnabled)
            {
                foreach (ListItem item in this.cmbinclude.Items)
                {
                    if (item.Value == @"9")
                    {
                        item.Enabled = false;
                    }
                }
            }

            var employees = new cEmployees(user.AccountID);
            var budgetHolders = new cBudgetholders(user.AccountID);
            var teams = new cTeams(user.AccountID);
            var approvalMatrices = new ApprovalMatrices(user.AccountID);

            string action = string.Empty;
            if (this.Request.QueryString["action"] != null)
            {
                action = this.Request.QueryString["action"];
            }

            int groupid = int.Parse(this.Request.QueryString["groupid"]);
            this.txtgroupid.Text = groupid.ToString(CultureInfo.InvariantCulture);
            cGroup reqgroup = new cGroups(user.AccountID).GetGroupById(groupid);

            if (user.Account.IsNHSCustomer)
            {
                this.cmbsignofftype.Items.Add(SignoffType.AssignmentSignOffOwner.ToListItem());
            }

            // check here for Expedite Receipts
            if (user.Account.ReceiptServiceEnabled)
            {
                this.cmbsignofftype.Items.Add(SignoffType.SELScanAttach.ToListItem());
                this.chkNotifyWhenEnvelopeReceived.Checked = reqgroup.NotifyClaimantWhenEnvelopeReceived ?? false;
                this.chkNotifyWhenEnvelopeNotReceived.Checked = reqgroup.NotifyClaimantWhenEnvelopeNotReceived ?? false;
            }
            
            // check here for Expedite Validation
            if (user.Account.ValidationServiceEnabled)
            {
                this.cmbsignofftype.Items.Add(SignoffType.SELValidation.ToListItem());
            }

            this.ContainerNotifyWhenEnvelopeReceived.Visible = false;
            this.ContainerNotifyWhenEnvelopeNotReceived.Visible = false;
            this.rowLineManagerAssignmentSupervisor.Visible = false;
            this.ReqLineManagerAssignmentSupervisor.Enabled = false;

            if (action == "2")
            {
                // edit
                this.txtaction.Text = "2";

                int signoffId = int.Parse(this.Request.QueryString["signoffid"]);
                this.txtsignoffid.Text = signoffId.ToString(CultureInfo.InvariantCulture);
                cStage reqstage = reqgroup.stages[signoffId];

                if (reqstage.signofftype == SignoffType.None)
                {
                    this.cmbsignofftype.Items.Insert(0, SignoffType.None.ToListItem());
                    this.cmdcancel.Visible = false;
                }

                if (reqstage.stage == 1)
                {
                    this.cmbsignofftype.Items.Add(SignoffType.ClaimantSelectsOwnChecker.ToListItem());
                    this.cmbsignofftype.Items.Add(SignoffType.ApprovalMatrix.ToListItem());
                    this.cmbsignofftype.Items.Add(SignoffType.DeterminedByClaimantFromApprovalMatrix.ToListItem());
                }

                this.cmbsignofftype.Items.FindByValue(((int)reqstage.signofftype).ToString(CultureInfo.InvariantCulture)).Selected = true;
                this.chksendmail.Checked = reqstage.sendmail;
                this.chksinglesignoff.Checked = reqstage.singlesignoff;
                if (this.chksinglesignoff.Checked)
                {
                    this.chkApproverJustificationsRequired.Enabled = false;
                }

                if (reqstage.signofftype == SignoffType.CostCodeOwner || reqstage.signofftype == SignoffType.AssignmentSignOffOwner 
                    || reqstage.signofftype == SignoffType.SELScanAttach || reqstage.signofftype == SignoffType.SELValidation)
                {
                    this.chksinglesignoff.Enabled = false;
                }

                this.cmblist.Enabled = this.cmblist.Visible = true;
                
                #region Pay Before Validate

                // enable it only if no other stage has it selected.
                var allowPayBeforeValidate =
                    user.Account.ValidationServiceEnabled && 
                    !reqgroup.stages.Values.Any(s => s.signofftype == SignoffType.SELValidation && s.stage < reqstage.stage) &&
                    !reqgroup.stages.Any(s => s.signoffid != reqstage.signoffid && s.AllocateForPayment) &&
                    !reqstage.IsPostValidationCleanupStage;

                this.hdnAllowPayBeforeValidate.Value = allowPayBeforeValidate.ToString();
                this.SectionPayBeforeValidate.Visible =
                    reqstage.AllocateForPayment ||
                    (allowPayBeforeValidate &&
                    reqstage.signofftype != SignoffType.SELScanAttach &&
                    reqstage.signofftype != SignoffType.SELValidation);

                if (this.SectionPayBeforeValidate.Visible)
                {
                    this.divTooltipPadder.Visible = true;
                    this.chkPayBeforeValidate.Checked = reqstage.AllocateForPayment;
                    this.SetupPayBeforeValidate();
                    this.trThresholdPayBeforeValidate.Visible = this.chkPayBeforeValidate.Checked;
                    this.txtThreshold.Text = (!reqstage.ValidationCorrectionThreshold.HasValue ? cStage.DefaultValidationCorrectionThreshold : reqstage.ValidationCorrectionThreshold.Value).ToString();
                }

                if (reqstage.IsPostValidationCleanupStage)
                {
                    this.divIsPostValidationStageComment.Visible = true;
                    this.cmbinclude.Enabled = false;
                    this.cmbinvolvement.Enabled = false;
                    this.cmbsignofftype.Items.Remove(SignoffType.SELValidation.ToListItem());
                }

                this.ToggleOnHolidaySkip();

                #endregion Pay Before Validate
                
                switch (reqstage.signofftype)
                {
                    case SignoffType.BudgetHolder:
                        this.cmblist.Items.AddRange(budgetHolders.CreateDropDown().ToArray());

                        if (this.cmblist.Items.FindByValue(reqstage.relid.ToString(CultureInfo.InvariantCulture)) != null)
                        {
                            this.cmblist.Items.FindByValue(reqstage.relid.ToString(CultureInfo.InvariantCulture)).Selected = true;
                        }

                        break;

                    case SignoffType.Employee:
                        this.cmblist.Items.AddRange(employees.CreateCheckPayDropDown(reqstage.relid, user.AccountID));
                        break;

                    case SignoffType.Team:
                        this.cmblist.Items.AddRange(teams.CreateDropDown(reqstage.relid));
                        break;
                    case SignoffType.CostCodeOwner:
                        if (user.Account.IsNHSCustomer)
                        {
                            this.rowLineManagerAssignmentSupervisor.Visible = true;
                            this.ReqLineManagerAssignmentSupervisor.Enabled = true;
                            string nextApproverWhenNoCostCodeOrCostCodeOwner = reqstage.NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner ? "AssignmentSupervisor" : "LineManager";
                            this.DropDownLineManagerAssignmentSupervisor.Items.FindByValue(nextApproverWhenNoCostCodeOrCostCodeOwner).Selected = true;
                        }

                        this.cmblist.Visible = false;
                        this.reqsignoff.Enabled = false;
                        break;
                    case SignoffType.None:
                    case SignoffType.LineManager:
                    case SignoffType.ClaimantSelectsOwnChecker:
                    case SignoffType.AssignmentSignOffOwner:
                        this.cmblist.Visible = false;
                        this.reqsignoff.Enabled = false;
                        break;
                    case SignoffType.ApprovalMatrix:
                    case SignoffType.DeterminedByClaimantFromApprovalMatrix:
                        this.cmblist.Items.AddRange(approvalMatrices.CreateDropDown(reqstage.relid, false).ToArray());

                        if (reqstage.signofftype == SignoffType.DeterminedByClaimantFromApprovalMatrix)
                        {
                            this.trExtraLevels.Visible = true;
                            this.rfvExtraLevels.Enabled = true;
                            this.txtExtraLevels.Text = reqstage.ExtraApprovalLevels.ToString(CultureInfo.InvariantCulture);
                            this.trFromMyLevel.Visible = true;
                            this.chkFromMyLevel.Checked = reqstage.FromMyLevel;
                        }
                        break;
                    case SignoffType.SELScanAttach:
                    case SignoffType.SELValidation:
                        this.cmbsignofftype_SelectedIndexChanged(null, null);
                        break;
                }

                this.cmbinclude.Items.FindByValue(((byte)reqstage.include).ToString(CultureInfo.InvariantCulture)).Selected = true;

                var inclusionType = (StageInclusionType)int.Parse(this.cmbinclude.SelectedValue);

                switch (inclusionType)
                {
                    case StageInclusionType.ClaimTotalExceeds:
                    case StageInclusionType.ClaimTotalBelow:
                        this.compamount.Enabled = true;
                        this.reqamount.Enabled = true;
                        this.txtamount.Visible = true;
                        this.txtamount.Text = reqstage.amount.ToString("######0.00");
                        break;

                    case StageInclusionType.OlderThanDays:
                        this.compamount.Enabled = true;
                        this.reqamount.Enabled = true;
                        this.txtamount.Visible = true;
                        this.txtamount.Text = ((int)reqstage.amount).ToString(CultureInfo.InvariantCulture);
                        break;

                    case StageInclusionType.IncludesCostCode:
                        this.cmbincludelst.Items.Clear();
                        this.cmbincludelst.Visible = true;
                        this.compamount.Enabled = false;
                        this.reqamount.Enabled = false;
                        var clscostcodes = new cCostcodes((int)this.ViewState["accountid"]);
                        this.cmbincludelst.Items.AddRange(clscostcodes.CreateDropDown(false).ToArray());
                        this.cmbincludelst.Items.FindByValue(reqstage.includeid.ToString(CultureInfo.InvariantCulture)).Selected = true;
                        break;
                    case StageInclusionType.IncludesDepartment:
                        this.cmbincludelst.Items.Clear();
                        this.cmbincludelst.Visible = true;
                        this.compamount.Enabled = false;
                        this.reqamount.Enabled = false;
                        var departments = new cDepartments((int)this.ViewState["accountid"]);
                        this.cmbincludelst.Items.AddRange(departments.CreateDropDown(true).ToArray());
                        this.cmbincludelst.Items.FindByValue(reqstage.includeid.ToString(CultureInfo.InvariantCulture)).Selected = true;
                        break;
                    case StageInclusionType.IncludesExpenseItem:
                        this.cmbincludelst.Items.Clear();
                        this.cmbincludelst.Visible = true;
                        this.compamount.Enabled = false;
                        this.reqamount.Enabled = false;
                        var clssubcats = new cSubcats((int)this.ViewState["accountid"]);
                        this.cmbincludelst.Items.AddRange(clssubcats.CreateDropDown().ToArray());

                        if (this.cmbincludelst.Items.FindByValue(reqstage.includeid.ToString(CultureInfo.InvariantCulture)) != null)
                        {
                            this.cmbincludelst.Items.FindByValue(reqstage.includeid.ToString(CultureInfo.InvariantCulture)).Selected = true;
                        }

                        break;

                    default:
                        this.cmbincludelst.Visible = false;
                        this.compamount.Enabled = false;
                        this.reqamount.Enabled = false;
                        break;
                }

                this.chkApproverJustificationsRequired.Checked = reqstage.ApproverJustificationsRequired;
                if (this.chkApproverJustificationsRequired.Checked)
                {
                    this.chksinglesignoff.Enabled = false;
                }

                this.chkclaimantmail.Checked = reqstage.claimantmail;
                this.cmbinvolvement.ClearSelection();
                this.cmbinvolvement.Items.FindByValue(reqstage.notify.ToString(CultureInfo.InvariantCulture)).Selected = true;
                this.chkdisplaydeclaration.Checked = reqstage.displaydeclaration;

                if (this.cmbonholiday.Items.FindByValue(reqstage.onholiday.ToString(CultureInfo.InvariantCulture)) != null)
                {
                    this.cmbonholiday.Items.FindByValue(reqstage.onholiday.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }

                if (this.cmbsignofftype.SelectedValue == ((int)SignoffType.DeterminedByClaimantFromApprovalMatrix).ToString(CultureInfo.InvariantCulture))
                {
                    this.cmbonholiday.Enabled = false;
                }

                if (this.cmbonholiday.SelectedValue == "3")
                {
                    this.cmbholidaytype.Visible = true;
                    this.cmbholidaylist.Visible = true;
                    this.cmbholidaytype.Items.FindByValue(((int)reqstage.holidaytype).ToString()).Selected = true;

                    switch (reqstage.holidaytype)
                    {
                        case SignoffType.BudgetHolder:
                            this.cmbholidaylist.Items.AddRange(budgetHolders.CreateDropDown().ToArray());
                            if (this.cmbholidaylist.Items.FindByValue(reqstage.holidayid.ToString(CultureInfo.InvariantCulture)) != null)
                            {
                                this.cmbholidaylist.Items.FindByValue(reqstage.holidayid.ToString(CultureInfo.InvariantCulture)).Selected = true;
                            }

                            this.reqholidaylst.Enabled = true;
                            break;

                        case SignoffType.Employee:
                            this.cmbholidaylist.Items.AddRange(employees.CreateCheckPayDropDown(reqstage.holidayid, user.AccountID));
                            this.reqholidaylst.Enabled = true;
                            break;

                        case SignoffType.Team:
                            this.cmbholidaylist.Items.AddRange(teams.CreateDropDown(reqstage.holidayid));
                            this.reqholidaylst.Enabled = true;
                            break;

                        case SignoffType.LineManager:
                        case SignoffType.CostCodeOwner:
                        case SignoffType.AssignmentSignOffOwner:
                            this.cmbholidaylist.Visible = false;
                            break;
                    }
                }
            }
            else
            {
                this.cmblist.Items.AddRange(budgetHolders.CreateDropDown().ToArray());
                this.cmbholidaylist.Items.AddRange(budgetHolders.CreateDropDown().ToArray());

                if (reqgroup.stagecount == 0)
                {
                    this.cmbsignofftype.Items.Add(SignoffType.ClaimantSelectsOwnChecker.ToListItem());
                    this.cmbsignofftype.Items.Add(SignoffType.ApprovalMatrix.ToListItem());
                    this.cmbsignofftype.Items.Add(SignoffType.DeterminedByClaimantFromApprovalMatrix.ToListItem());
                }

                #region Pay Before Validate

                // enable it only if no other stage has it selected.
                this.SectionPayBeforeValidate.Visible =
                    user.Account.ValidationServiceEnabled &&
                    !reqgroup.stages.Values.Any(s => s.signofftype == SignoffType.SELValidation) &&
                    !reqgroup.stages.Values.Any(s => s.AllocateForPayment);

                this.hdnAllowPayBeforeValidate.Value = this.SectionPayBeforeValidate.Visible.ToString();

                if (this.SectionPayBeforeValidate.Visible)
                {
                    this.divTooltipPadder.Visible = true;
                    this.txtThreshold.Text = cStage.DefaultValidationCorrectionThreshold.ToString();
                    this.SetupPayBeforeValidate();
                    this.ToggleOnHolidaySkip();
                }

                #endregion Pay Before Validate
                
            }
        }

        /// <summary>
        /// This changes the list of entities presented that a claim can go to when the original approver is on holiday,
        ///     when the holiday entity type is changed (and the holiday action is set to go to a different approver
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbholidaytype_SelectedIndexChanged(object sender, EventArgs e)
        {
            int accountId = (int)this.ViewState["accountid"];
            this.cmbholidaylist.Items.Clear();

            switch (this.cmbholidaytype.SelectedValue)
            {
                case "1":
                    this.cmbholidaylist.Items.AddRange(new cBudgetholders(accountId).CreateDropDown().ToArray());
                    this.cmbholidaylist.Visible = true;
                    this.reqholidaylst.Enabled = true;
                    break;

                case "2":
                    this.cmbholidaylist.Items.AddRange(new cEmployees(accountId).CreateCheckPayDropDown(0, accountId));
                    this.cmbholidaylist.Visible = true;
                    this.reqholidaylst.Enabled = true;
                    break;

                case "3":
                    this.cmbholidaylist.Items.AddRange(new cTeams(accountId).CreateDropDown(0));
                    this.cmbholidaylist.Visible = true;
                    this.reqholidaylst.Enabled = true;
                    break;

                case "4":
                    this.cmbholidaylist.Visible = false;
                    this.reqholidaylst.Enabled = false;
                    break;
            }
        }

        /// <summary>
        /// This changes the list of values or the textbox that can be entered when the "when to include" type changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbinclude_SelectedIndexChanged(object sender, EventArgs e)
        {
            int accountId = (int)this.ViewState["accountid"];
            var inclusionType = (StageInclusionType)int.Parse(this.cmbinclude.SelectedValue);

            switch (inclusionType)
            {
                case StageInclusionType.ClaimTotalExceeds:
                case StageInclusionType.ClaimTotalBelow:
                case StageInclusionType.OlderThanDays:
                    this.reqamount.Enabled = true;
                    this.compamount.Enabled = true;
                    this.txtamount.Visible = true;
                    this.cmbincludelst.Visible = false;
                    break;

                case StageInclusionType.IncludesCostCode:
                    this.reqamount.Enabled = false;
                    this.compamount.Enabled = false;
                    this.txtamount.Visible = false;
                    this.cmbincludelst.Visible = true;
                    this.cmbincludelst.Items.Clear();
                    this.cmbincludelst.Items.AddRange(new cCostcodes(accountId).CreateDropDown(false).ToArray());
                    break;

                case StageInclusionType.IncludesDepartment:
                    this.reqamount.Enabled = false;
                    this.compamount.Enabled = false;
                    this.txtamount.Visible = false;
                    this.cmbincludelst.Visible = true;
                    this.cmbincludelst.Items.Clear();
                    this.cmbincludelst.Items.AddRange(new cDepartments(accountId).CreateDropDown(true).ToArray());
                    break;

                case StageInclusionType.IncludesExpenseItem:
                    this.cmbincludelst.Items.Clear();
                    this.cmbincludelst.Visible = true;
                    this.compamount.Enabled = false;
                    this.txtamount.Visible = false;
                    this.reqamount.Enabled = false;
                    this.cmbincludelst.Items.AddRange(new cSubcats(accountId).CreateDropDown().ToArray());
                    break;

                default:
                    this.cmbincludelst.Visible = false;
                    this.reqamount.Enabled = false;
                    this.compamount.Enabled = false;
                    this.txtamount.Visible = false;
                    break;
            }
        }

        /// <summary>
        /// This controls the visibility of related alternate holiday approver choice controls when the "what to do when the main approver is on holiday" choice changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbonholiday_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbonholiday.SelectedValue == "3")
            {
                this.cmbholidaytype.Visible = true;
                this.cmbholidaylist.Visible = true;
                this.reqholidaylst.Enabled = true;
            }
            else
            {
                this.cmbholidaytype.Visible = false;
                this.cmbholidaylist.Visible = false;
                this.reqholidaylst.Enabled = false;
            }
        }

        /// <summary>
        /// This controls the visibility of related approver controls when the main approver type for the stage is changed
        ///     The list of approvers is updated to a list of the relevant type
        ///     Signoff choice is made mandatory if anything but line manager or determined by claimant is selected
        ///     The choice of extra approval levels for "determined by claimant from approval matrix" is shown if that is chosen (and made mandatory)
        ///         and the what to do if approver is on holiday is set to take no action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbsignofftype_SelectedIndexChanged(object sender, EventArgs e)
        {
            int accountId = (int)this.ViewState["accountid"];
            this.cmblist.Items.Clear();
            this.cmblist.Enabled = this.cmblist.Visible = true;
            this.cmbinclude.Visible = true;
            this.cmbinclude.Enabled = !this.divIsPostValidationStageComment.Visible;
            this.cmbinvolvement.Enabled = !this.divIsPostValidationStageComment.Visible;
            this.rowLineManagerAssignmentSupervisor.Visible = false;
            this.ReqLineManagerAssignmentSupervisor.Enabled = false;
            this.chksendmail.Enabled = true;
            this.chkdisplaydeclaration.Enabled = true;
            this.chkclaimantmail.Enabled = true;
            this.chkclaimantmail.Checked = false;
            this.chksinglesignoff.Enabled = true;
            this.ContainerNotifyWhenEnvelopeReceived.Visible = false;
            this.ContainerNotifyWhenEnvelopeNotReceived.Visible = false;
            this.chkNotifyWhenEnvelopeReceived.Enabled = false;
            this.chkNotifyWhenEnvelopeNotReceived.Enabled = false;
            this.chkApproverJustificationsRequired.Enabled = true;
            var currentSignoffType = (SignoffType)Enum.Parse(typeof (SignoffType), this.cmbsignofftype.SelectedValue);
            this.SectionPayBeforeValidate.Visible = bool.Parse(this.hdnAllowPayBeforeValidate.Value);
            

            switch (currentSignoffType)
            {
                case SignoffType.BudgetHolder:
                    this.cmblist.Items.AddRange(new cBudgetholders(accountId).CreateDropDown().ToArray());
                    this.cmblist.Visible = true;
                    this.reqsignoff.Enabled = true;
                    this.rfvExtraLevels.Enabled = false;
                    this.trExtraLevels.Visible = false;
                    this.trFromMyLevel.Visible = false;
                    this.cmbonholiday.Enabled = true;
                    this.chksinglesignoff.Enabled = true;
                    break;

                case SignoffType.Employee:
                    this.cmblist.Items.AddRange(new cEmployees(accountId).CreateCheckPayDropDown(0, accountId));
                    this.cmblist.Visible = true;
                    this.reqsignoff.Enabled = true;
                    this.rfvExtraLevels.Enabled = false;
                    this.trExtraLevels.Visible = false;
                    this.trFromMyLevel.Visible = false;
                    this.cmbonholiday.Enabled = true;
                    this.chksinglesignoff.Enabled = true;
                    break;

                case SignoffType.Team:
                    this.cmblist.Items.AddRange(new cTeams(accountId).CreateDropDown(0));
                    this.cmblist.Visible = true;
                    this.reqsignoff.Enabled = true;
                    this.rfvExtraLevels.Enabled = false;
                    this.trExtraLevels.Visible = false;
                    this.trFromMyLevel.Visible = false;
                    this.cmbonholiday.Enabled = true;
                    this.chksinglesignoff.Enabled = true;
                    break;

                case SignoffType.LineManager:
                case SignoffType.ClaimantSelectsOwnChecker:
                    this.cmblist.Visible = false;
                    this.reqsignoff.Enabled = false;
                    this.rfvExtraLevels.Enabled = false;
                    this.trExtraLevels.Visible = false;
                    this.trFromMyLevel.Visible = false;
                    this.cmbonholiday.Enabled = true;
                    this.chksinglesignoff.Enabled = true;
                    break;

                case SignoffType.CostCodeOwner:
                    this.rowLineManagerAssignmentSupervisor.Visible = true;
                    this.ReqLineManagerAssignmentSupervisor.Enabled = true;
                    this.cmblist.Visible = false;
                    this.reqsignoff.Enabled = false;
                    this.rfvExtraLevels.Enabled = false;
                    this.trExtraLevels.Visible = false;
                    this.trFromMyLevel.Visible = false;
                    this.cmbonholiday.Enabled = true;
                    this.chksinglesignoff.Enabled = false;
                    this.chksinglesignoff.Checked = false;
                    break;

                case SignoffType.AssignmentSignOffOwner:
                    this.cmblist.Visible = false;
                    this.reqsignoff.Enabled = false;
                    this.rfvExtraLevels.Enabled = false;
                    this.trExtraLevels.Visible = false;
                    this.trFromMyLevel.Visible = false;
                    this.cmbonholiday.Enabled = true;
                    this.chksinglesignoff.Enabled = false;
                    this.chksinglesignoff.Checked = false;
                    break;

                case SignoffType.ApprovalMatrix:
                    this.cmblist.Items.AddRange(new ApprovalMatrices(accountId).CreateDropDown(0, false).ToArray());
                    this.cmblist.Visible = true;
                    this.reqsignoff.Enabled = true;
                    this.rfvExtraLevels.Enabled = false;
                    this.trExtraLevels.Visible = false;
                    this.trFromMyLevel.Visible = false;
                    this.chkFromMyLevel.Checked = false;
                    this.cmbonholiday.Enabled = true;
                    this.chksinglesignoff.Enabled = true;
                    break;

                case SignoffType.DeterminedByClaimantFromApprovalMatrix:
                    this.cmblist.Items.AddRange(new ApprovalMatrices(accountId).CreateDropDown(0, false).ToArray());
                    this.cmblist.Visible = true;
                    this.reqsignoff.Enabled = true;
                    this.trExtraLevels.Visible = true;
                    this.trFromMyLevel.Visible = true;
                    this.rfvExtraLevels.Enabled = true;
                    this.cmbonholiday.ClearSelection();
                    this.cmbonholiday.Items.FindByValue("1").Selected = true;
                    this.cmbonholiday.Enabled = false;
                    this.cmbonholiday_SelectedIndexChanged(null, null);
                    this.chksinglesignoff.Enabled = true;
                    this.chkFromMyLevel.Checked = false;
                    break;

                case SignoffType.SELScanAttach: 
                    ConfigureViewForSelStage();
                    ContainerNotifyWhenEnvelopeReceived.Visible = true;
                    chkNotifyWhenEnvelopeReceived.Enabled = true;
                    ContainerNotifyWhenEnvelopeNotReceived.Visible = true;
                    chkNotifyWhenEnvelopeNotReceived.Enabled = true;
                    this.chkApproverJustificationsRequired.Checked = false;
                    break;

                case SignoffType.SELValidation:
                    ConfigureViewForSelStage();
                    break;
            }

            if (this.cmbsignofftype.Items.Count > 0 && this.cmbsignofftype.Items[0].Value == ((int)SignoffType.None).ToString())
            {
                this.cmbsignofftype.Items.RemoveAt(0);
            }

        }

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);
            this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);
        }

        /// <summary>
        /// Returns to the group edit page in edit mode for the groupid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdcancel_Click(object sender, ImageClickEventArgs e)
        {
            var groupid = int.Parse(this.txtgroupid.Text);
            var signoffId = 0;
            if (int.TryParse(this.txtsignoffid.Text, out signoffId))
            {
                var user = cMisc.GetCurrentUser();

                cGroup reqgroup = new cGroups(user.AccountID).GetGroupById(groupid);
                cStage reqstage = reqgroup.stages[signoffId];
                if (reqstage.IsPostValidationCleanupStage)
                {
                    this.Validate();
                    if (!this.IsValid || reqstage.signofftype == SignoffType.None)
                    {
                        return;
                    }
                }
            }

            this.Response.Redirect("aegroup.aspx?action=2&groupid=" + groupid, true);
        }

        /// <summary>
        /// Attempts to save the new/updated stage and return to the edit page for the groupid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            int extraLevels = 0;
            int relid = 0;
            var include = StageInclusionType.None;
            int notify = 0;
            int holidayid = 0;
            var holidaytype = SignoffType.None;
            int onholiday = 0;
            decimal amount = 0;
            int groupid = 0;
            int includeid = 0;
            bool claimantmail = this.chkclaimantmail.Checked;
            bool singlesignoff = this.chksinglesignoff.Checked;
            bool sendmail = this.chksendmail.Checked;
            bool displaydeclaration = this.chkdisplaydeclaration.Checked;
            bool approveHigherLevelsOnly = false;
            bool nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner = true;
            var signofftype = (SignoffType) Enum.Parse(typeof (SignoffType), this.cmbsignofftype.SelectedValue);
            bool allocateForPayment = this.chkPayBeforeValidate.Checked;
            int? validationCorrectionThreshold = null;
            int i;
            if (int.TryParse(this.txtThreshold.Text, out i))
            {
                validationCorrectionThreshold = i;
            }
            bool approverJustificationsRequired = chkApproverJustificationsRequired.Checked;

            // should we only worry about this when the signoff type = costcodeowner? if so, move this inside the switch.
            nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner = this.DropDownLineManagerAssignmentSupervisor.SelectedValue == "AssignmentSupervisor";

            switch (signofftype)
            {
                case SignoffType.LineManager:
                case SignoffType.ClaimantSelectsOwnChecker:
                case SignoffType.CostCodeOwner:
                case SignoffType.SELScanAttach:
                case SignoffType.SELValidation:
                case SignoffType.AssignmentSignOffOwner:
                    relid = 0;
                    break;
                case SignoffType.DeterminedByClaimantFromApprovalMatrix:
                    int.TryParse(this.txtExtraLevels.Text, out extraLevels);
                    approveHigherLevelsOnly = this.chkFromMyLevel.Checked;
                    relid = int.Parse(this.cmblist.SelectedValue);
                    break;
                case SignoffType.ApprovalMatrix:
                    relid = int.Parse(this.cmblist.SelectedValue);
                    break;
                default:
                    relid = int.Parse(this.cmblist.SelectedValue);
                    break;
            }

            include = (StageInclusionType)int.Parse(this.cmbinclude.SelectedValue);

            switch (include)
            {
                case StageInclusionType.ClaimTotalBelow:
                case StageInclusionType.ClaimTotalExceeds:
                case StageInclusionType.OlderThanDays:
                    amount = decimal.Parse(this.txtamount.Text);
                    break;
            }

            notify = int.Parse(this.cmbinvolvement.SelectedValue);
            onholiday = int.Parse(this.cmbonholiday.SelectedValue);
            groupid = int.Parse(this.txtgroupid.Text);
            
            var accountId = (int)ViewState["accountid"];
            var groups = new cGroups(accountId);
            var group = groups.GetGroupById(groupid);

            if (signofftype == SignoffType.SELScanAttach)
            {
                notify = 2;
                groups.SaveGroup(group.groupid, group.groupname, group.description, group.oneclickauthorisation,
                    cMisc.GetCurrentUser(), 1, chkNotifyWhenEnvelopeReceived.Checked, chkNotifyWhenEnvelopeNotReceived.Checked);
            }

            if (signofftype == SignoffType.SELValidation)
            {
                notify = 2;
            }

            if (this.cmbincludelst.SelectedValue != string.Empty)
            {
                includeid = int.Parse(this.cmbincludelst.SelectedValue);
            }

            if (onholiday == 3)
            {
                holidaytype = (SignoffType)int.Parse(this.cmbholidaytype.SelectedValue);

                if (this.cmbholidaylist.SelectedValue != null && (holidaytype != SignoffType.LineManager && holidaytype != SignoffType.CostCodeOwner && holidaytype != SignoffType.AssignmentSignOffOwner))
                {
                    holidayid = int.Parse(this.cmbholidaylist.SelectedValue);
                }
                else
                {
                    holidayid = 0;
                }
            }
            
            if (this.txtaction.Text == "2")
            {
                // update
                int signoffid = int.Parse(this.txtsignoffid.Text);
                groups.updateStage(signoffid, signofftype, relid, (int)include, amount, notify, onholiday, holidaytype, holidayid, includeid, claimantmail, singlesignoff, sendmail, displaydeclaration, (int)this.ViewState["employeeid"], extraLevels, approveHigherLevelsOnly, approverJustificationsRequired, nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner, allocateForPayment, validationCorrectionThreshold);
            }
            else
            {
                groups.addStage(groupid, signofftype, relid, (int)include, amount, notify, onholiday, holidaytype, holidayid, includeid, claimantmail, singlesignoff, sendmail, displaydeclaration, (int)this.ViewState["employeeid"], 0, false, extraLevels, approveHigherLevelsOnly, approverJustificationsRequired, nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner, allocateForPayment, false, validationCorrectionThreshold);
            }

            if (allocateForPayment && !group.stages.Values.Any(s => s.IsPostValidationCleanupStage))
            {
                groups.addStage(groupid, SignoffType.None, 0, (int)StageInclusionType.Always, 0, 2, 0, SignoffType.None, 0, 0, false, false, false, false, (int)this.ViewState["employeeid"], 0, false, 0, false, approverJustificationsRequired, nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner, false, true, null);
                var newGroup = new cGroups(accountId); // initialise the Group to fix error on saving PBV enabled signoff stage
                group = newGroup.GetGroupById(groupid); // get updated list of stages
                this.Response.Redirect(String.Format("aestage.aspx?action=2&groupid={0}&signoffid={1}", groupid, group.stages.FirstOrDefault(s => s.IsPostValidationCleanupStage).signoffid));
            }

            this.Response.Redirect("aegroup.aspx?action=2&groupid=" + groupid, true);
        }


        private void ConfigureViewForSelStage()
        {
            // set include to always and disable.
            this.cmbinclude.ClearSelection();
            this.cmbinclude.Items.FindByValue("1").Selected = true;
            this.cmbinclude.Enabled = false;
            this.cmbinclude_SelectedIndexChanged(null, null);

            // disable the side list
            this.cmblist.ClearSelection();
            this.cmblist.Enabled = this.cmblist.Visible = false;
            
            // set involvement to 'Just notify' and disable.
            this.cmbinvolvement.ClearSelection();
            this.cmbinvolvement.Items.FindByValue("2").Selected = true;
            this.cmbinvolvement.Enabled = false;

            // set holiday to 'take no action' and disable.
            this.cmbonholiday.ClearSelection();
            this.cmbonholiday.Items.FindByValue("1").Selected = true;
            this.cmbonholiday.Enabled = false;
            this.cmbonholiday_SelectedIndexChanged(null, null);

            // configure checkboxes.
            this.chksinglesignoff.Checked = this.chksinglesignoff.Enabled = false;
            this.chkclaimantmail.Checked = true;
            this.chksendmail.Checked = this.chksendmail.Enabled = false;
            this.chkdisplaydeclaration.Checked = this.chkdisplaydeclaration.Enabled = false;
            this.chkPayBeforeValidate.Checked = false;

            this.SectionPayBeforeValidate.Visible = false;
            this.chkPayBeforeValidate.Enabled = this.chkPayBeforeValidate.Checked = false;
            this.chkApproverJustificationsRequired.Enabled = false;
            this.chkApproverJustificationsRequired.Checked = false;
        }


        protected void chksinglesignoff_CheckedChanged(object sender, EventArgs e)
        {
            this.chkApproverJustificationsRequired.Enabled = !this.chksinglesignoff.Checked;
            this.chkApproverJustificationsRequired.Checked = false;
        }

        /// <summary>
        /// Setup pay before validate form fields.
        /// </summary>
        private void SetupPayBeforeValidate()
        {
            this.reqThreshold.Enabled = this.chkPayBeforeValidate.Checked;
            this.rangeThreshold.Enabled = this.chkPayBeforeValidate.Checked;
            this.trThresholdPayBeforeValidate.Visible = this.chkPayBeforeValidate.Checked;
            this.cmbinclude.Enabled = !this.chkPayBeforeValidate.Checked;
            this.cmbinvolvement.Enabled = !this.chkPayBeforeValidate.Checked;

            var currentSignoffType = (SignoffType)Enum.Parse(typeof(SignoffType), this.cmbsignofftype.SelectedValue);
            if (currentSignoffType == SignoffType.AssignmentSignOffOwner ||
                currentSignoffType == SignoffType.CostCodeOwner) return;

            this.chksinglesignoff.Enabled = !this.chkPayBeforeValidate.Checked;
        }

        private void ToggleOnHolidaySkip()
        {
            if (this.chkPayBeforeValidate.Checked || this.divIsPostValidationStageComment.Visible)
            {
                this.cmbonholiday.Items.Remove(this.cmbonholidaySkipStage);
            }
            else if (this.cmbonholiday.Items.Count == 2)
            {
                this.cmbonholiday.Items.Insert(1, this.cmbonholidaySkipStage);
            }
        }

        protected void chkApproverJustificationsRequired_CheckedChanged(object sender, EventArgs e)
        {
            var currentSignoffType = (SignoffType)Enum.Parse(typeof(SignoffType), this.cmbsignofftype.SelectedValue);
            if (currentSignoffType == SignoffType.AssignmentSignOffOwner ||
                currentSignoffType == SignoffType.CostCodeOwner) return;

            this.chksinglesignoff.Enabled = !this.chkApproverJustificationsRequired.Checked;
            this.chksinglesignoff.Checked = false;
        }
    }
}