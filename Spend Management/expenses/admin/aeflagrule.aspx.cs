namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.FinancialYears;
    using SpendManagementLibrary.Flags;

    using Spend_Management.expenses.code;
    using System.ComponentModel;


    /// <summary>
    /// The aeflagrule.
    /// </summary>
    public partial class aeflagrule : System.Web.UI.Page
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
            this.Master.UseDynamicCSS = true;

            int flagid = 0;
            if (!this.IsPostBack)
            {
                this.smProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/shortcut.js"));
                this.smProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery-ui.datepicker-en-gb.js"));
                this.smProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.ui.timepicker-0.3.2.js"));
                this.smProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.ui.multiselect.js"));
                this.smProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/json2.min.js"));

                this.ddlstFlagType.Attributes.Add("onchange", "SEL.FlagsAndLimits.ddlstFlagType_OnChange();");
                this.ddlstDateComparisonType.Attributes.Add("onchange", "SEL.FlagsAndLimits.ddlstDateComparisonType_OnChange();");
                this.ddlstFrequencyType.Attributes.Add("onchange", "SEL.FlagsAndLimits.ddlstFrequencyType_OnChange();");
                this.ddlstPeriodType.Attributes.Add("onchange", "SEL.FlagsAndLimits.ddlstPeriodType_OnChange();");
                this.ddlstAction.Attributes.Add("onchange", "SEL.FlagsAndLimits.ddlstAction_OnChange();");
                this.ddlstItemRoleInclusionType.Attributes.Add("onchange", "SEL.FlagsAndLimits.ToggleDisplayItemRoleGrid();");
                this.ddlstExpenseItemInclusionType.Attributes.Add("onchange", "SEL.FlagsAndLimits.ToggleExpenseItemsGrid();");
                StringBuilder js = new StringBuilder();
                js.Append("SEL.FlagsAndLimits.ddlstFlagTypeID = '" + this.ddlstFlagType.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.ddlstActionID = '" + this.ddlstAction.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtFlagTextID = '" + this.txtflagtext.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtAmberToleranceID = '" + this.txtambertolerance.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.ddlstInvalidDateTypeID = '" + this.ddlstDateComparisonType.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtDateID = '" + this.txtDate.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtMonthsID = '" + this.txtMonths.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtFrequencyID = '" + this.txtFrequency.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtPeriodID = '" + this.txtPeriod.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.ddlstPeriodyTypeID = '" + this.ddlstPeriodType.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtLimitID = '" + this.txtLimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.lblDateComparisonValueID = '" + this.lbldatecomparisonvalue.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.lblFrequencyID = '" + this.lblFrequency.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtFrequencyID = '" + this.txtFrequency.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtLimitID = '" + this.txtLimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtGroupLimitID = '" + this.txtgrouplimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtDescription = '" + this.txtdescription.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.chkActive = '" + this.chkactive.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.chkClaimantJustificationRequired = '" + this.chkClaimantJustificationRequired.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.chkDisplayFlagImmediately = '" + this.chkDisplayFlagImmediately.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtNoFlagTolerance = '" + this.txtNoFlagTolerance.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.reqDate = '" + this.reqDate.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.reqMonths = '" + this.reqMonths.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.compDate = '" + this.compDate.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.compMonths = '" + this.compMonths.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.ddlstFrequencyTypeID = '" + this.ddlstFrequencyType.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.ddlstFinancialYearID = '" + this.ddlstFinancialYear.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.reqFrequency = '" + this.reqFrequency.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.rangeFrequency = '" + this.rangeFrequency.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.reqPeriod = '" + this.reqPeriod.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.rangePeriod = '" + this.rangePeriod.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.reqLimit = '" + this.reqLimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.compLimit = '" + this.compLimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.reqGroupLimit = '" + this.reqGroupLimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.compGroupLimit = '" + this.compGroupLimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.rangeNoFlagTolerance = '" + this.rangeNoFlagTolerance.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.rangeAmber = '" + this.rangeAmber.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.reqTipLimit = '" + this.reqTipLimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.rangeTipLimit = '" + this.rangeTipLimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtTipLimit = '" + this.txtTipLimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.ddlstFlagLevel = '" + this.ddlstFlagLevel.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.chkApproverJustificationRequired = '" + this.chkApproverJustificationRequired.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.chkIncreaseByNumOthers = '" + this.chkIncreaseByNumOthers.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.chkDisplayLimit = '" + this.chkDisplayLimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtNotesForAuthoriser = '" + this.txtNotesForAuthoriser.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.ddlstItemRoleInclusionType = '" + this.ddlstItemRoleInclusionType.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.ddlstExpenseItemInclusionType = '" + this.ddlstExpenseItemInclusionType.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtPassengerLimit = '" + this.txtPassengerLimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.reqPassengerLimit = '" + this.reqpassengerlimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.compPassengerLimit = '" + this.compPassengerLimit.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.txtRestrictDailyMileage = '" + this.txtRestrictDailyMileage.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.reqRestrictDailyMileage = '" + this.reqRestrictDailyMileage.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.compRestrictDailyMileage = '" + this.compRestrictDailyMileage.ClientID + "';\n");
                js.Append("SEL.FlagsAndLimits.configureTabs();\n");
                js.Append("SEL.FlagsAndLimits.configureItemRoleModal();\n");
                js.Append("SEL.FlagsAndLimits.configureExpenseItemModal();\n");
                js.Append("SEL.FlagsAndLimits.configureFieldModal();\n");
                js.Append("$( \"#" + this.txtDate.ClientID + "\" ).datepicker();");
            
                CurrentUser user = cMisc.GetCurrentUser(User.Identity.Name);
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FlagsAndLimits, true, true);
                this.ViewState["accountid"] = user.AccountID;
                this.ViewState["employeeid"] = user.EmployeeID;
                if (Request.QueryString["flagid"] != null)
                {
                    int.TryParse(Request.QueryString["flagid"], out flagid);
                }

                this.Title = "New Flag Rule";       

                svcFlagRules svcflags = new svcFlagRules();

                this.reqDate.Enabled = false;
                this.reqMonths.Enabled = false;
                this.compDate.Enabled = false;
                this.compMonths.Enabled = false;
                if (flagid > 0)
                {
                    user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.FlagsAndLimits, true, true);
                    FlagManagement clsflags = new FlagManagement(user.AccountID);
                    Flag flag = clsflags.GetBy(flagid);
                    if (flag != null)
                    {
                        FieldInfo fieldType = flag.FlagType.GetType().GetField(flag.FlagType.ToString());
                        DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldType.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        this.Title = "Flag Rule: " + attributes[0].Description;
                        this.txtdescription.Text = flag.Description;
                        this.chkactive.Checked = flag.Active;
                        this.chkClaimantJustificationRequired.Checked = flag.ClaimantJustificationRequired;
                        this.chkDisplayFlagImmediately.Checked = flag.DisplayFlagImmediately;

                        string[] gridData = svcflags.CreateRolesGrid(flagid);
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "rolesGridVars", cGridNew.generateJS_init("rolesGridVars", new List<string> { gridData[0] }, user.CurrentActiveModule), true);
                        this.litroles.Text = gridData[1];
                        gridData = svcflags.CreateExpenseItemGrid(flagid);
                        this.litexpenseitems.Text = gridData[1];
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "expenseitemGridVars", cGridNew.generateJS_init("expenseitemGridVars", new List<string> { gridData[0] }, user.CurrentActiveModule), true);
                        this.ddlstFlagType.Enabled = false;
                        if (this.ddlstFlagType.Items.FindByValue(((byte)flag.FlagType).ToString(CultureInfo.InvariantCulture)) != null)
                        {
                            this.ddlstFlagType.Items.FindByValue(((byte)flag.FlagType).ToString(CultureInfo.InvariantCulture)).Selected = true;
                        }

                        if (this.ddlstAction.Items.FindByValue(((byte)flag.Action).ToString(CultureInfo.InvariantCulture)) != null)
                        {
                            this.ddlstAction.Items.FindByValue(((byte)flag.Action).ToString(CultureInfo.InvariantCulture)).Selected = true;
                        }

                        if (this.ddlstItemRoleInclusionType.Items.FindByValue(((byte)flag.ItemRoleInclusionType).ToString()) != null)
                        {
                            this.ddlstItemRoleInclusionType.Items.FindByValue(((byte)flag.ItemRoleInclusionType).ToString()).Selected = true;
                        }

                        if (this.ddlstExpenseItemInclusionType.Items.FindByValue(((byte)flag.ExpenseItemInclusionType).ToString()) != null)
                        {
                            this.ddlstExpenseItemInclusionType.Items.FindByValue(((byte)flag.ExpenseItemInclusionType).ToString()).Selected = true;
                        }

                        this.txtflagtext.Text = flag.CustomFlagText;
                        this.chkApproverJustificationRequired.Checked = flag.ApproverJustificationRequired;
                        this.ddlstFlagLevel.Items.FindByValue(((byte)flag.FlagLevel).ToString(CultureInfo.InvariantCulture)).Selected = true;
                        this.txtNotesForAuthoriser.Text = flag.NotesForAuthoriser;
                        switch (flag.FlagType)
                        {
                            case FlagType.Duplicate:
                                gridData = svcflags.CreateFieldsGrid(flagid.ToString(CultureInfo.InvariantCulture));
                                Page.ClientScript.RegisterStartupScript(
                                    this.GetType(),
                                    "fieldsGridVars",
                                    cGridNew.generateJS_init(
                                        "fieldsGridVars",
                                        new List<string> { gridData[0] },
                                        user.CurrentActiveModule),
                                    true);
                                this.litFields.Text = gridData[1];
                                break;
                            case FlagType.InvalidDate:
                                InvalidDateFlag invaliddateflag = (InvalidDateFlag)flag;
                                if (
                                    this.ddlstDateComparisonType.Items.FindByValue(
                                        ((byte)invaliddateflag.InvalidDateFlagType).ToString(
                                            CultureInfo.InvariantCulture)) != null)
                                {
                                    this.ddlstDateComparisonType.Items.FindByValue(
                                        ((byte)invaliddateflag.InvalidDateFlagType).ToString(
                                            CultureInfo.InvariantCulture)).Selected = true;
                                }

                                if (invaliddateflag.InvalidDateFlagType == InvalidDateFlagType.SetDate)
                                {
                                    if (invaliddateflag.Date != null)
                                    {
                                        this.txtDate.Text = ((DateTime)invaliddateflag.Date).ToShortDateString();
                                    }
                                    this.reqDate.Enabled = true;
                                    this.compDate.Enabled = true;
                                }
                                else
                                {
                                    this.txtMonths.Text = invaliddateflag.Months.ToString();
                                    this.reqMonths.Enabled = true;
                                    this.compMonths.Enabled = true;
                                }

                                break;
                            case FlagType.GroupLimitWithoutReceipt:
                            case FlagType.GroupLimitWithReceipt:
                                GroupLimitFlag grouplimitflag = (GroupLimitFlag)flag;
                                if (grouplimitflag.AmberTolerance != null)
                                {
                                    this.txtambertolerance.Text = grouplimitflag.AmberTolerance.ToString();
                                }

                                if (grouplimitflag.NoFlagTolerance != null)
                                {
                                    this.txtNoFlagTolerance.Text = grouplimitflag.NoFlagTolerance.ToString();
                                }

                                this.txtgrouplimit.Text = grouplimitflag.Limit.ToString(CultureInfo.InvariantCulture);
                                break;
                            case FlagType.TipLimitExceeded:
                                TipFlag tipFlag = (TipFlag)flag;
                                if (tipFlag.AmberTolerance != null)
                                {
                                    this.txtambertolerance.Text = tipFlag.AmberTolerance.ToString();
                                }

                                if (tipFlag.NoFlagTolerance != null)
                                {
                                    this.txtNoFlagTolerance.Text = tipFlag.NoFlagTolerance.ToString();
                                }

                                this.txtTipLimit.Text = tipFlag.TipLimit.ToString();
                                break;
                            case FlagType.LimitWithoutReceipt:
                            case FlagType.LimitWithReceipt:
                                LimitFlag limitflag = (LimitFlag)flag;
                                this.chkIncreaseByNumOthers.Checked = limitflag.IncreaseByNumOthers;
                                this.chkDisplayLimit.Checked = limitflag.DisplayLimit;
                                if (limitflag.AmberTolerance != null)
                                {
                                    this.txtambertolerance.Text = limitflag.AmberTolerance.ToString();
                                }

                                if (limitflag.NoFlagTolerance != null)
                                {
                                    this.txtNoFlagTolerance.Text = limitflag.NoFlagTolerance.ToString();
                                }

                                break;
                            case FlagType.MileageExceeded:
                                MileageFlag mileageFlag = (MileageFlag)flag;
                                if (mileageFlag.AmberTolerance != null)
                                {
                                    this.txtambertolerance.Text = mileageFlag.AmberTolerance.ToString();
                                }

                                if (mileageFlag.NoFlagTolerance != null)
                                {
                                    this.txtNoFlagTolerance.Text = mileageFlag.NoFlagTolerance.ToString();
                                }

                                break;
                            case FlagType.FrequencyOfItemCount:
                            case FlagType.FrequencyOfItemSum:
                                FrequencyFlag frequencyflag = (FrequencyFlag)flag;

                                if (frequencyflag.FlagType == FlagType.FrequencyOfItemCount)
                                {
                                    this.txtFrequency.Text = frequencyflag.Frequency.ToString();
                                }
                                else
                                {
                                    txtLimit.Text = frequencyflag.Limit.ToString();
                                }

                                if (
                                    this.ddlstFrequencyType.Items.FindByValue(
                                        ((byte)frequencyflag.FrequencyType).ToString(CultureInfo.InvariantCulture))
                                    != null)
                                {
                                    this.ddlstFrequencyType.Items.FindByValue(
                                        ((byte)frequencyflag.FrequencyType).ToString(CultureInfo.InvariantCulture))
                                        .Selected = true;
                                }

                                this.txtPeriod.Text = frequencyflag.Period.ToString(CultureInfo.InvariantCulture);
                                if (
                                    this.ddlstPeriodType.Items.FindByValue(
                                        ((byte)frequencyflag.PeriodType).ToString(CultureInfo.InvariantCulture)) != null)
                                {
                                    this.ddlstPeriodType.Items.FindByValue(
                                        ((byte)frequencyflag.PeriodType).ToString(CultureInfo.InvariantCulture))
                                        .Selected = true;
                                }

                                if (frequencyflag.PeriodType == FlagPeriodType.FinancialYears)
                                {
                                    foreach (FinancialYear year in FinancialYears.ActiveYears(user))
                                    {
                                        this.ddlstFinancialYear.Items.Add(
                                            new ListItem(
                                                year.Description,
                                                year.FinancialYearID.ToString(CultureInfo.InvariantCulture)));
                                    }

                                    if (frequencyflag.FinancialYear != null
                                        && this.ddlstFinancialYear.Items.FindByValue(
                                            frequencyflag.FinancialYear.ToString()) != null)
                                    {
                                        this.ddlstFinancialYear.Items.FindByValue(
                                            frequencyflag.FinancialYear.ToString()).Selected = true;
                                    }
                                }

                                break;
                            case FlagType.NumberOfPassengersLimit:
                                NumberOfPassengersFlag passengersFlag = (NumberOfPassengersFlag)flag;
                                txtPassengerLimit.Text = passengersFlag.PassengerLimit.ToString();
                                break;
                            case FlagType.RestrictDailyMileage:
                                var dailyMileageFlag = (RestrictDailyMileageFlag)flag;
                                this.txtRestrictDailyMileage.Text = dailyMileageFlag.DailyMileageLimit.ToString();
                                break;
                        }
                    }
                }
                else
                {
                    user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.FlagsAndLimits, true, true);
                    string[] gridData = svcflags.CreateRolesGrid(0);
                    this.litroles.Text = gridData[1];
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "rolesGridVars", cGridNew.generateJS_init("rolesGridVars", new List<string> { gridData[0] }, user.CurrentActiveModule), true);
                    gridData = svcflags.CreateExpenseItemGrid(0);
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "expenseitemGridVars", cGridNew.generateJS_init("expenseitemGridVars", new List<string> { gridData[0] }, user.CurrentActiveModule), true);
                    this.litexpenseitems.Text = gridData[1];
                    gridData = svcflags.CreateFieldsGrid(flagid.ToString(CultureInfo.InvariantCulture));
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "fieldsGridVars", cGridNew.generateJS_init("fieldsGridVars", new List<string> { gridData[0] }, user.CurrentActiveModule), true);
                    this.litFields.Text = gridData[1];
                }

                this.Master.title = this.Title;
                this.Master.PageSubTitle = "Flag Rule Details";
                this.Master.enablenavigation = false;

                js.Append("$(document).ready(function() {\n");
                js.Append("SEL.FlagsAndLimits.flagID = " + flagid + ";\n");
                js.Append("SEL.FlagsAndLimits.ddlstFlagType_OnChange();\n");
                js.Append("SEL.FlagsAndLimits.ToggleDisplayItemRoleGrid();");
                js.Append("SEL.FlagsAndLimits.ToggleExpenseItemsGrid();");
                js.Append("SEL.FlagsAndLimits.ddlstAction_OnChange();");

                if (flagid > 0)
                {
                    FlagManagement clsflags = new FlagManagement(user.AccountID);
                    Flag flag = clsflags.GetBy(flagid);
                    if (flag != null && flag is FrequencyFlag)
                    {
                        js.Append("$g('" + this.ddlstPeriodType.ClientID + "').value = '" + ((byte)((FrequencyFlag)flag).PeriodType).ToString() + "';");
                    }
                }

                js.Append("});");

                ClientScript.RegisterStartupScript(this.GetType(), "js", js.ToString(), true);
            }
        }

    }
}
