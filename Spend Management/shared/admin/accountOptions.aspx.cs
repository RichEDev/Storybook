namespace Spend_Management
{
    #region Using Directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.Services;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Interfaces;
    using shared.code;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;

    #endregion

    /// <summary>
    ///   The account options.
    /// </summary>
    public partial class accountOptions : Page
    {
        /// <summary>
        /// The page_ init.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        protected void Page_Init(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var currentTab = 0;
            switch (user.CurrentActiveModule)
            {
                case Modules.contracts:
                case Modules.SmartDiligence:
                    currentTab = 1;
                    break;
                case Modules.expenses:
                    currentTab = 0;
                    break;
                case Modules.Greenlight:
                case Modules.GreenlightWorkforce:
                    currentTab = 2;
                    break;
            }

            var script = string.Format("Sys.Application.add_load(function(){{setTimeout(setActiveTab,{0}); }});\n", currentTab);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "setActiveOptionsTab", script, true);
        }

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
            const string DELEGATE_EMPLOYEE_EXPENSES_TEXT = "Modify employees, sign-off groups and roles";
            const string DELEGATE_EMPLOYEE_NONEXPENSES_TEXT = "Modify employees and roles";

            CurrentUser currentUser = cMisc.GetCurrentUser();
            this.Master.title = "Tailoring";
            this.Title = "General Options";
            this.Master.PageSubTitle = this.Title;

            this.Master.showdummymenu = true;
            this.Master.enablenavigation = false;

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.contracts:
                    this.Master.helpid = 1140;
                    break;
                default:
                    this.Master.helpid = 1043;
                    break;
            }

            var usingExpenses = currentUser.CurrentActiveModule == Modules.expenses;
            this.lblemployees.Text = usingExpenses ? DELEGATE_EMPLOYEE_EXPENSES_TEXT : DELEGATE_EMPLOYEE_NONEXPENSES_TEXT;
            this.delegateExpensesDiv.Visible = usingExpenses;
            this.delegateExpensesSpan.Visible = usingExpenses;
            this.delegateExpensesSubmitClaimDiv.Visible = usingExpenses;
            this.delegateExpensesSubmitClaimDivSectionTiltle.Visible = usingExpenses;
            this.lnkNewExpenses.Visible = usingExpenses;
            this.itemRoleSpan.Visible = usingExpenses;
            this.defaultItemRoleRow.Visible = usingExpenses;
            this.signoffRow.Visible = usingExpenses;
            this.corporateCardsSpan.Visible = usingExpenses;
            this.spanGeneralOptionsExpenses.Visible = usingExpenses;
            this.dvlaLicenceElementSection.Visible = currentUser.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect);
            this.spanVehicleLookups.Visible =
                currentUser.Account.HasLicensedElement(SpendManagementElement.VehicleLookup);
            this.SetNumberOfRememberedApproversDropDown();
            var clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
            
            cAccountProperties reqProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
            var clsEmployees = new cEmployees(currentUser.AccountID);

            var clsGridNew = new cGridNew(
                currentUser.AccountID, currentUser.EmployeeID, "addScreen", "SELECT code, description, individual, display, mandatory, displaycc, mandatorycc, displaypc, mandatorypc FROM addscreen");

            clsGridNew.addEventColumn("editcode", "/shared/images/icons/edit.png", "javascript:showAddScreen('{code}');", "Edit {code}", string.Empty);
            clsGridNew.KeyField = "description";

            string[] addScreenGridData = clsGridNew.generateGrid();
            this.litAddScreenTable.Text = addScreenGridData[1];

            // set the sel.grid javascript variables
            this.Page.ClientScript.RegisterStartupScript(
                this.GetType(), "AccOptionsGridVars", cGridNew.generateJS_init("AccOptionsGridVars", new List<string> { addScreenGridData[0] }, currentUser.CurrentActiveModule), true);

            this.optodologin.Attributes.Add("onclick", "showOdoDay();");
            this.optodosubmit.Attributes.Add("onclick", "showOdoDay();");
            this.chkrecordodometer.Attributes.Add("onclick", "showOdoDay();");
           
            this.HideTabs(currentUser.CurrentActiveModule);

            if (!currentUser.Account.ValidationServiceEnabled)
            {
                this.tabExpedite.Visible = false;
            }

            if (this.IsPostBack == false)
            {
                currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.GeneralOptions, true, true);

                var clsModules = new cModules();
                cModule clsModule = clsModules.GetModuleByID((int)currentUser.CurrentActiveModule);
                string brandName = (clsModule != null) ? clsModule.BrandNamePlainText : "Expenses";
                this.optserver.Text = string.Format("{0} server", brandName);

                #region General Options

                this.chkpreapproval.Checked = reqProperties.PreApproval;

                if (currentUser.Account.EmployeeSearchEnabled)
                {
                    this.chksearchemployees.Checked = reqProperties.SearchEmployees;
                }
                else
                {
                    this.chksearchemployees.Enabled = false;
                    this.chksearchemployees.Checked = false;
                }

                chkemployeedetailschanged.Checked = false;
                if (reqProperties.AllowEmployeeToNotifyOfChangeOfDetails)
                {
                    this.chkemployeedetailschanged.Checked = true;
                }


                if (currentUser.Account.HotelReviewsEnabled)
                {
                    this.chkshowreviews.Checked = reqProperties.ShowReviews;
                    this.chksendreviewrequest.Checked = reqProperties.SendReviewRequests;
                }
                else
                {
                    this.chkshowreviews.Enabled = false;
                    this.chkshowreviews.Checked = false;
                    this.chksendreviewrequest.Checked = false;
                    this.chksendreviewrequest.Enabled = false;
                }

                this.chkTeamMemberApproveOwnClaims.Checked = reqProperties.AllowTeamMemberToApproveOwnClaim;
                this.chkEmployeeApproveOwnClaims.Checked = reqProperties.AllowEmployeeInOwnSignoffGroup;

                this.chkrecordodometer.Checked = reqProperties.RecordOdometer;

                if (currentUser.Account.CorporateCardsEnabled)
                {
                    this.chkonlycashcredit.Checked = reqProperties.OnlyCashCredit;
                    this.chkpartsubmittal.Checked = reqProperties.PartSubmit;
                }
                else
                {
                    this.chkonlycashcredit.Checked = false;
                    this.chkonlycashcredit.Enabled = false;
                    this.chkpartsubmittal.Checked = false;
                    this.chkpartsubmittal.Enabled = false;
                }

                this.chkeditmydetails.Checked = reqProperties.EditMyDetails;
                this.chkEditPreviousClaim.Checked = reqProperties.EditPreviousClaims;

                if (reqProperties.EnterOdometerOnSubmit)
                {
                    this.optodosubmit.Checked = true;
                }
                else
                {
                    this.optodologin.Checked = true;
                }

                if (this.chkrecordodometer.Checked && this.optodologin.Checked)
                {
                    // lblrecordodometer.Style.Add(HtmlTextWriterStyle.Display, "");
                    this.txtodometerday.Style.Add(HtmlTextWriterStyle.Display, string.Empty);
                    this.spanOdoDay.Style.Add(HtmlTextWriterStyle.Display, string.Empty);
                    this.txtodometerday.Text = reqProperties.OdometerDay.ToString(CultureInfo.InvariantCulture);
                    this.compOdoDayLessThan.Enabled = true;
                    this.compOdoDayGreaterThan.Enabled = true;
                    this.reqOdoDay.Enabled = true;
                }

                this.chkInternalTickets.Checked = reqProperties.EnableInternalSupportTickets;

                this.chkallowselfreg.Checked = reqProperties.AllowSelfReg;

                var clsAccessRoles = new cAccessRoles(currentUser.AccountID, cAccounts.getConnectionString(currentUser.AccountID));

                int defaultRole = reqProperties.DefaultRole.HasValue ? reqProperties.DefaultRole.Value : 0;

                this.cmbdefaultrole.Items.AddRange(clsAccessRoles.CreateDropDown(defaultRole, true).ToArray());

                var clsItemRoles = new ItemRoles(currentUser.AccountID);

                int defaultItemRole = reqProperties.DefaultItemRole.HasValue ? reqProperties.DefaultItemRole.Value : 0;

                this.cmbdefaultitemrole.Items.AddRange(clsItemRoles.CreateDropDown(defaultItemRole, true).ToArray());

                if (reqProperties.AllowSelfReg)
                {
                    if (currentUser.Account.AdvancesEnabled)
                    {
                        this.chkselfregadvancessignoff.Checked = reqProperties.AllowSelfRegAdvancesSignOff;
                    }
                    else
                    {
                        this.chkselfregadvancessignoff.Checked = false;
                        this.chkselfregadvancessignoff.Enabled = false;
                    }

                    this.chkselfregbankdetails.Checked = reqProperties.AllowSelfRegBankDetails;
                    this.chkselfregcardetails.Checked = reqProperties.AllowSelfRegCarDetails;
                    this.chkselfregdepcostcode.Checked = reqProperties.AllowSelfRegDepartmentCostCode;
                    this.chkselfregempconact.Checked = reqProperties.AllowSelfRegEmployeeContact;
                    this.chkselfregempinfo.Checked = reqProperties.AllowSelfRegEmployeeInfo;
                    this.chkselfreghomaddr.Checked = reqProperties.AllowSelfRegHomeAddress;
                    this.chkselfregrole.Checked = reqProperties.AllowSelfRegRole;
                    this.chkselfregsignoff.Checked = reqProperties.AllowSelfRegSignOff;
                    this.chkselfregudf.Checked = reqProperties.AllowSelfRegUDF;
                    this.chkselfregitemrole.Checked = reqProperties.AllowSelfRegItemRole;
                }

                this.txtflagmessage.Text = reqProperties.FlagMessage;

                this.txtfrequencyvalue.Text = reqProperties.FrequencyValue.ToString();

                this.cmbfrequencytype.Items.Add(new ListItem("Month", "1"));
                this.cmbfrequencytype.Items.Add(new ListItem("Week", "2"));

                if (this.cmbfrequencytype.Items.FindByValue(reqProperties.FrequencyType.ToString()) != null)
                {
                    this.cmbfrequencytype.Items.FindByValue(reqProperties.FrequencyType.ToString()).Selected = true;
                }
                this.chkEnableClaimApprovalReminder.Checked = reqProperties.EnableClaimApprovalReminders;
                this.chkEnableCurrentClaimsReminder.Checked = reqProperties.EnableCurrentClaimsReminders;
                this.ddlClaimApprovalReminderFrequency.Text = reqProperties.ClaimApprovalReminderFrequency.ToString();
                this.ddlCurrentClaimReminderFrequency.Text = reqProperties.CurrentClaimsReminderFrequency.ToString();
                this.chkEnableCurrentClaimsReminder.Attributes.Add("onclick", "toggleValidator(this)");
                this.chkEnableClaimApprovalReminder.Attributes.Add("onclick", "toggleValidator(this)");



                this.chkdelsetup.Checked = reqProperties.DelSetup;
                this.chkdelemployeeadmin.Checked = reqProperties.DelEmployeeAdmin;
                this.chkdelreports.Checked = reqProperties.DelReports;
                this.chkdelclaimantreports.Checked = reqProperties.DelReportsClaimants;
                this.chkdelcheckandpay.Checked = reqProperties.DelCheckAndPay;
                this.chkdelqedesign.Checked = reqProperties.DelQEDesign;
                this.chkDelsSubmitClaims.Checked = reqProperties.DelSubmitClaim;
                this.chkDelOptionForDelAccessRole.Checked = reqProperties.EnableDelegateOptionsForDelegateAccessRole;
                if (currentUser.Account.CorporateCardsEnabled)
                {
                    this.chkdelcorporatecard.Checked = reqProperties.DelCorporateCards;
                }
                else
                {
                    this.chkdelcorporatecard.Checked = false;
                    this.chkdelcorporatecard.Enabled = false;
                }

                if (currentUser.Account.AdvancesEnabled)
                {
                    this.chkdelapprovals.Checked = reqProperties.DelApprovals;
                }
                else
                {
                    this.chkdelapprovals.Checked = false;
                    this.chkdelapprovals.Enabled = false;
                }

                this.chkdelexports.Checked = reqProperties.DelExports;
                this.chkdelauditlog.Checked = reqProperties.DelAuditLog;
                this.chkclaimantdeclaration.Checked = reqProperties.ClaimantDeclaration;
                this.txtdeclarationmsg.Text = reqProperties.DeclarationMsg;
                this.txtapproverdeclarationmsg.Text = reqProperties.ApproverDeclarationMsg;
                this.chksendreviewrequest.Checked = reqProperties.SendReviewRequests;
                this.createDrillDown(currentUser.AccountID, new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8"), currentUser.EmployeeID, reqProperties.DrilldownReport, currentUser.CurrentSubAccountId);

                if (!this.chkallowselfreg.Checked)
                {
                    this.chkselfregempconact.InputAttributes["disabled"] = "true";
                    this.chkselfreghomaddr.InputAttributes["disabled"] = "true";
                    this.chkselfregempinfo.InputAttributes["disabled"] = "true";
                    this.chkselfregrole.InputAttributes["disabled"] = "true";
                    this.chkselfregitemrole.InputAttributes["disabled"] = "true";
                    this.cmbdefaultrole.Attributes["disabled"] = "true";
                    this.cmbdefaultitemrole.Attributes["disabled"] = "true";
                    this.chkselfregsignoff.InputAttributes["disabled"] = "true";
                    this.chkselfregadvancessignoff.InputAttributes["disabled"] = "true";
                    this.chkselfregdepcostcode.InputAttributes["disabled"] = "true";
                    this.chkselfregbankdetails.InputAttributes["disabled"] = "true";
                    this.chkselfregcardetails.InputAttributes["disabled"] = "true";
                    this.chkselfregudf.InputAttributes["disabled"] = "true";
                }

                this.cboNumberOfApproversToShowInClaimHistoryForApprovalMatrix.Items.FindByValue(
                    reqProperties.NumberOfApproversToRememberForClaimantInApprovalMatrixClaim.ToString(CultureInfo.InvariantCulture)).Selected =
                    true; 

                // mobile devices tab
                this.chkEnableMobileDevices.Checked = reqProperties.UseMobileDevices;
                //Expedite Tab - Validation Options
                this.chkValidationOptionsAllowReceipt.Checked = reqProperties.AllowReceiptTotalToPassValidation;
                this.chkEmployeeSpecifyCarStartDate.Checked = reqProperties.AllowEmpToSpecifyCarStartDateOnAdd;
                this.chkEmployeeSpecifyCarStartDateMandatory.Checked = reqProperties.EmpToSpecifyCarStartDateOnAddMandatory;
                this.chkEmployeeSpecifyCarDOC.Checked = reqProperties.AllowEmpToSpecifyCarDOCOnAdd;
                cmbCountdown.SelectedValue = reqProperties.CountdownTimer.ToString(CultureInfo.InvariantCulture);
                cmbIdleTimeout.SelectedValue = reqProperties.IdleTimeout.ToString(CultureInfo.InvariantCulture);
                chkBlockUnmatchedExpenseItemsBeingSubmitted.Checked = reqProperties.BlockUnmachedExpenseItemsBeingSubmitted;
                #endregion General Options

                #region Regional Settings

                var clscountries = new cCountries(currentUser.AccountID, currentUser.CurrentSubAccountId);

                this.ddlDefaultCountry.Items.AddRange(clscountries.CreateDropDown().ToArray());
                if (this.ddlDefaultCountry.Items.FindByValue(reqProperties.HomeCountry.ToString(CultureInfo.InvariantCulture)) != null)
                {
                    this.ddlDefaultCountry.Items.FindByValue(reqProperties.HomeCountry.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }
                else
                {
                    if (clscountries.list.ContainsKey(reqProperties.HomeCountry))
                    {
                        this.ddlDefaultCountry.Items.Add(clscountries.GetListItem(reqProperties.HomeCountry));
                        this.ddlDefaultCountry.Items.FindByValue(reqProperties.HomeCountry.ToString(CultureInfo.InvariantCulture)).Selected = true;
                    }
                }

                var clslanguages = new cLanguages();
                string[] languages = clslanguages.getLangaugeList();
                int i;
                for (i = 0; i < languages.Length; i++)
                {
                    this.ddlDefaultLanguage.Items.Add(languages[i]);
                }

                if (this.ddlDefaultLanguage.Items.FindByValue(reqProperties.Language) != null)
                {
                    this.ddlDefaultLanguage.Items.FindByValue(reqProperties.Language).Selected = true;
                }

                var clscurrencies = new cCurrencies(currentUser.AccountID, currentUser.CurrentSubAccountId);
                int basecurrency;
                if (reqProperties.BaseCurrency.HasValue && reqProperties.BaseCurrency.Value != 0)
                {
                    basecurrency = reqProperties.BaseCurrency.Value;
                }
                else
                {
                    basecurrency = 0;
                }

                this.ddlBaseCurrency.Items.AddRange(clscurrencies.CreateDropDown(basecurrency));
                var clsclaims = new cClaims(currentUser.AccountID);
                if (clsclaims.getCount() > 0)
                {
                    this.ddlBaseCurrency.Enabled = false;
                }

                var clslocales = new cLocales();
                this.ddlDefaultLocale.Items.Add(new ListItem("[None]", "0"));
                this.ddlDefaultLocale.Items.AddRange(clslocales.CreateActiveDropDown().ToArray());
                if (this.ddlDefaultLocale.Items.FindByValue(reqProperties.GlobalLocaleID.ToString(CultureInfo.InvariantCulture)) != null)
                {
                    this.ddlDefaultLocale.Items.FindByValue(reqProperties.GlobalLocaleID.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }

                #endregion Regional Settings

                #region Main Administrator

                this.ddlMainAdministrator.Items.Add(new ListItem("[None]", "0"));
                this.ddlMainAdministrator.Items.AddRange(clsEmployees.CreateDropDown(0, false));

                if (this.ddlMainAdministrator.Items.FindByValue(reqProperties.MainAdministrator.ToString(CultureInfo.InvariantCulture)) != null)
                {
                    this.ddlMainAdministrator.Items.FindByValue(reqProperties.MainAdministrator.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }

                #endregion Main Administrator

                #region Email Server Settings

                this.txtEmailServer.Text = reqProperties.EmailServerAddress;
                if (reqProperties.SourceAddress == 1)
                {
                    this.optserver.Checked = true;
                }
                else
                {
                    this.optclaimant.Checked = true;
                }

                this.txtAuditorEmail.Text = reqProperties.AuditorEmailAddress;
                this.txtEmailFromAddress.Text = reqProperties.EmailServerFromAddress;
                this.txtErrorEmailSubmitAddress.Text = reqProperties.ErrorEmailAddress;
                this.txtErrorSubmitFromAddress.Text = reqProperties.ErrorEmailFromAddress;
                this.txtEmailAdministrator.Text = reqProperties.EmailAdministrator;

                #endregion Email Server Settings

                #region Password Settings

                this.txtattempts.Text = reqProperties.PwdMaxRetries.ToString(CultureInfo.InvariantCulture);
                this.txtexpires.Text = reqProperties.PwdExpiryDays.ToString(CultureInfo.InvariantCulture);

                if (reqProperties.PwdConstraint != 0)
                {
                    this.cmblength.Items.FindByValue(((int)reqProperties.PwdConstraint).ToString(CultureInfo.InvariantCulture)).Selected = true;
                }

                switch (reqProperties.PwdConstraint)
                {
                    case PasswordLength.EqualTo:
                        this.lblMinimumPasswordLength.Text = "Length";
                        this.reqLength1.Enabled = true;
                        this.complength1.Enabled = true;
                        this.compLength1LessThan.Enabled = true;
                        this.compLength1Greater.Enabled = true;
                        break;
                    case PasswordLength.LessThan:
                        this.lblMinimumPasswordLength.Text = "Length";
                        this.reqLength1.Enabled = true;
                        this.complength1.Enabled = true;
                        this.compLength1LessThan.Enabled = true;
                        this.compLength1Greater.Enabled = true;
                        break;
                    case PasswordLength.GreaterThan:
                        this.reqLength1.Enabled = true;
                        this.complength1.Enabled = true;
                        this.compLength1LessThan.Enabled = true;
                        this.compLength1Greater.Enabled = true;
                        break;
                    case PasswordLength.Between:
                        this.lblMinimumPasswordLength.Text = "Minimum length";
                        this.reqLength1.Enabled = true;
                        this.complength1.Enabled = true;
                        this.compLength1LessThan.Enabled = true;
                        this.compLength1Greater.Enabled = true;
                        this.reqLength2.Enabled = true;
                        this.complength2.Enabled = true;
                        this.compLength2LessThan.Enabled = true;
                        this.compLength2Greater.Enabled = true;
                        this.compMinLess.Enabled = true;
                        this.compMaxGreater.Enabled = true;
                        break;
                }

                this.txtlength1.Text = reqProperties.PwdLength1.ToString(CultureInfo.InvariantCulture);
                this.txtlength2.Text = reqProperties.PwdLength2.ToString(CultureInfo.InvariantCulture);
                this.chkupper.Checked = reqProperties.PwdMustContainUpperCase;
                this.chknumbers.Checked = reqProperties.PwdMustContainNumbers;
                this.chksymbol.Checked = reqProperties.PwdMustContainSymbol;
                this.txtprevious.Text = reqProperties.PwdHistoryNum.ToString(CultureInfo.InvariantCulture);
                this.txtaccountlocked.Text = reqProperties.AccountLockedMessage;
                this.txtcurrentlylocked.Text = reqProperties.AccountCurrentlyLockedMessage;


                this.cmblength.Attributes.Add("onchange", "SetupPasswordFields();");

                switch (reqProperties.PwdConstraint)
                {
                    case PasswordLength.AnyLength:
                        this.plength1.Style.Add(HtmlTextWriterStyle.Display, "none");
                        this.plength2.Style.Add(HtmlTextWriterStyle.Display, "none");
                        break;
                    case PasswordLength.EqualTo:
                    case PasswordLength.GreaterThan:
                    case PasswordLength.LessThan:
                        this.plength1.Style.Add(HtmlTextWriterStyle.Display, string.Empty);
                        this.plength2.Style.Add(HtmlTextWriterStyle.Display, "none");
                        break;
                    case PasswordLength.Between:
                        this.plength1.Style.Add(HtmlTextWriterStyle.Display, string.Empty);
                        this.plength2.Style.Add(HtmlTextWriterStyle.Display, string.Empty);
                        break;
                }

                #endregion Password Settings

                #region new expenses

                this.chkActivateCarOnUserAdd.Attributes.Add("onclick", "activateCarsOnAdd();");
                this.chkAllowUsersToAddCars.Attributes.Add("onclick", "EnableUsersAddCar();");
                this.chkAllowMilage.Attributes.Add("onclick", "changeHiddenAllowMileageValue()");
                this.chkEmployeeSpecifyCarStartDate.Attributes.Add("onclick", "EnableUserCanSpecifyVehicleStartDate()");
        
                this.chkActivateCarOnUserAdd.Checked = reqProperties.ActivateCarOnUserAdd;
                this.chkAllowUsersToAddCars.Checked = reqProperties.AllowUsersToAddCars;
                this.chkAllowMilage.Checked = reqProperties.ShowMileageCatsForUsers;

                this.hdnAllowMileage.Value = reqProperties.ShowMileageCatsForUsers ? "true" : "false";

                if (this.chkActivateCarOnUserAdd.Checked)
                {
                    this.chkAllowMilage.InputAttributes.Add("disabled", "disabled");
                }

                if (!this.chkEmployeeSpecifyCarStartDate.Checked)            
                {
                    this.chkEmployeeSpecifyCarStartDateMandatory.InputAttributes.Add("disabled", "disabled");
                    this.chkEmployeeSpecifyCarStartDateMandatory.Checked = false;
                }

                this.chksingleclaim.Checked = reqProperties.SingleClaim;
                this.chkcostcodes.Checked = reqProperties.UseCostCodes;
                this.chkdepartment.Checked = reqProperties.UseDepartmentCodes;
                this.chkprojectcodes.Checked = reqProperties.UseProjectCodes;
                this.chkcostcodedesc.Checked = reqProperties.UseCostCodeDescription;
                this.chkdepartmentdesc.Checked = reqProperties.UseDepartmentCodeDescription;
                this.chkprojectcodedesc.Checked = reqProperties.UseProjectCodeDescription;
                this.chkaddlocations.Checked = reqProperties.AddLocations;
                this.chkattach.Checked = reqProperties.AttachReceipts;
                this.chkexchangereadonly.Checked = reqProperties.ExchangeReadOnly;
                this.chkcostcodeson.Checked = reqProperties.CostCodesOn;
                this.chkdepartmentson.Checked = reqProperties.DepartmentsOn;
                this.chkprojectcodeson.Checked = reqProperties.ProjectCodesOn;
                this.chkcostcodeongendet.Checked = reqProperties.UseCostCodeOnGenDetails;
                this.chkdepartmentongendet.Checked = reqProperties.UseDeptOnGenDetails;
                this.chkprojectcodeongendet.Checked = reqProperties.UseProjectCodeOnGenDetails;
                this.chkautoassignallocation.Checked = reqProperties.AutoAssignAllocation;
                this.chkblockdrivinglicence.Checked = reqProperties.BlockDrivingLicence;
                this.chkblocktaxexpiry.Checked = reqProperties.BlockTaxExpiry;
                this.chkblockmotexpiry.Checked = reqProperties.BlockMOTExpiry;
                this.chkDrivingLicenceLookup.Checked = reqProperties.EnableAutomaticDrivingLicenceLookup;
                if (reqProperties.EnableAutomaticDrivingLicenceLookup)
                {
                    this.ddlDrivingLicenceLookupFrequency.SelectedValue = reqProperties.DrivingLicenceLookupFrequency;
                    this.ddlAutoRevokeOfConsentLookupFrequency.SelectedValue = reqProperties.FrequencyOfConsentRemindersLookup;
                }

                this.chkVehicleLookup.Checked = reqProperties.VehicleLookup;

                //load the team list in the approver section
                this.teamListForApprover.Items.AddRange(new cTeams(currentUser.AccountID).CreateDropDown(0));
                this.cmbLineManagerReminderDays.SelectedValue = reqProperties.RemindApproverOnDOCDocumentExpiryDays.ToString(CultureInfo.InvariantCulture);
                this.chkLicenceReview.Checked = reqProperties.EnableDrivingLicenceReview;
                this.cmbReviewFrequencyDays.SelectedValue = reqProperties.DrivingLicenceReviewFrequency.ToString(CultureInfo.InvariantCulture); 
                this.chkLicenceReviewReminderNotification.Checked = reqProperties.DrivingLicenceReviewReminder;

                this.cmbClaimantReminderDays.SelectedValue = reqProperties.RemindClaimantOnDOCDocumentExpiryDays.ToString(CultureInfo.InvariantCulture);
                if (reqProperties.DutyOfCareTeamAsApprover != null && !string.IsNullOrEmpty(reqProperties.DutyOfCareTeamAsApprover))
                    {
                        dutyOfCareApproverTeamSection.Style.Add("display", "block");
                        teamListForApprover.SelectedValue = reqProperties.DutyOfCareTeamAsApprover;
                        this.teamAsApprover.Checked = true;
                    }
                    else
                    {
                        this.lineManagerAsApprover.Checked = true;
                    }

                this.chkblockinsuranceexpiry.Checked = reqProperties.BlockInsuranceExpiry;
                this.chkBlockBreakdownCoverExpiry.Checked = reqProperties.BlockBreakdownCoverExpiry;
                this.chkDutyOfCareDateOfExpenseCheck.Checked = reqProperties.UseDateOfExpenseForDutyOfCareChecks;
                IOwnership dcco = clsSubAccounts.GetDefaultCostCodeOwner(currentUser.AccountID, currentUser.CurrentSubAccountId);
                this.txtdefaultcostcodeowner.Text = dcco == null ? string.Empty : dcco.ItemDefinition();
                this.txtdefaultcostcodeowner_ID.Text = dcco == null ? string.Empty : dcco.CombinedItemKey;

                // chkAddCompanies.Checked = reqProperties.AddCompanies;
                this.chkUseMapPoint.Checked = reqProperties.UseMapPoint;
                this.chkAllowMultipleDestinations.Checked = reqProperties.AllowMultipleDestinations;
                this.chkMandatoryPostcodeForAddresses.Checked = reqProperties.MandatoryPostcodeForAddresses;
                if (this.cmbmileagecalculationtype.Items.FindByValue(reqProperties.MileageCalcType.ToString(CultureInfo.InvariantCulture)) != null)
                {
                    this.cmbmileagecalculationtype.Items.FindByValue(reqProperties.MileageCalcType.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }

                this.chkClaimantsCanSaveCompanyAddresses.Checked = reqProperties.ClaimantsCanAddCompanyLocations;
                this.txtHomeAddressKeyword.Text = reqProperties.HomeAddressKeyword;
                this.txtWorkAddressKeyword.Text = reqProperties.WorkAddressKeyword;

                this.cmbRetainLabelsTime.SelectedValue = reqProperties.RetainLabelsTime;
                this.chkShowFullHomeAddress.Checked = reqProperties.ShowFullHomeAddressOnClaims;

                this.chkForceAddressNameEntry.Checked = reqProperties.ForceAddressNameEntry;
                this.chkForceAddressNameEntry.Enabled = currentUser.Account.AddressLookupProvider == AddressLookupProvider.Teleatlas;
                this.chkEnableVatOptions.Checked = reqProperties.EnableCalculationsForAllocatingFuelReceiptVatToMileage;
                this.txtAddressNameEntryMessage.Text = reqProperties.AddressNameEntryMessage;

                this.chkMultipleWorkAddress.Checked = reqProperties.MultipleWorkAddress;


                this.chkEnableAutoUpdateOfExchangeRates.Checked = reqProperties.EnableAutoUpdateOfExchangeRates;
                var future = false;
                if (reqProperties.currencyType == CurrencyType.Range && !reqProperties.EnableAutoUpdateOfExchangeRates)
                {
                    var currencies = new cCurrencies(currentUser.AccountID, currentUser.CurrentSubAccountId);
                    foreach (cCurrency currency in currencies.currencyList.Values)
                    {
                        if (!currency.archived && currency is cRangeCurrency)
                        {
                            var currencyRange = (cRangeCurrency)currency;
                            future = currencyRange.exchangerates.Any(r => r.Value.enddate > DateTime.Today);
                            if (future)
                            {
                                break;
                            }
                        }
                    }
                }
                if (future)
                {
                    this.lblExchangeRateWarning.Text = @"Enabling this option will delete any future date ranges already stored.";
                }
                else
                {
                    this.divComment.Visible = false;
                }

                var item = this.ddlExchangeRateProvider.Items.FindByValue(reqProperties.ExchangeRateProvider.ToString(CultureInfo.InvariantCulture));
                if (item != null)
                {
                    item.Selected = true;
                }

                this.chkDisableCarOutsideOfStartEndDate.Checked = reqProperties.DisableCarOutsideOfStartEndDate;
                this.BindOwnerAutoComplete(currentUser);                

                #endregion new expenses

                #region ESR Options

                if (currentUser.Account.IsNHSCustomer)
                {
                    this.litESROptions.Text = "<a href=\"javascript:changePage('ESROptions');\" id=\"lnkESROptions\">NHS Options</a>";
                    this.ddlESRActivateType.SelectedValue = ((byte)reqProperties.AutoActivateType).ToString(CultureInfo.InvariantCulture);
                    this.ddlESRArchiveType.SelectedValue = ((byte)reqProperties.AutoArchiveType).ToString(CultureInfo.InvariantCulture);
                    this.txtESRGracePeriod.Text = reqProperties.ArchiveGracePeriod.ToString(CultureInfo.InvariantCulture);
                    this.txtESRUsernameFormat.Text = reqProperties.ImportUsernameFormat;
                    this.txtESRHomeAddressFormat.Text = reqProperties.ImportHomeAddressFormat;
                    this.chkESRAssignmentsOnEmployeeAdd.Checked = reqProperties.CheckESRAssignmentOnEmployeeAdd;
                    this.ddlEsrDetail.Items.Add(new ListItem("[None]", "0"));
                    this.ddlEsrDetail.Items.Add(new ListItem("Paypoint", "1"));
                    this.ddlEsrDetail.Items.Add(new ListItem("Job Name", "2"));
                    this.ddlEsrDetail.Items.Add(new ListItem("Position Name", "3"));
                    this.ddlEsrDetail.SelectedIndex = (int)reqProperties.IncludeAssignmentDetails;
                    this.chkESRActivateCar.Checked = reqProperties.EsrAutoActivateCar;
                    this.chkDisplayEsrAddressesInSearchResults.Checked = reqProperties.DisplayEsrAddressesInSearchResults;
                    this.chkSummaryEsrInboundFile.Checked = reqProperties.SummaryEsrInboundFile;
                    this.ddlEsrRounding.Items.Clear();
                    this.ddlEsrRounding.Items.Add(new ListItem("Always Down", ((int)EsrRoundingType.Down).ToString()));
                    this.ddlEsrRounding.Items.Add(new ListItem("Maths Rounding", ((int)EsrRoundingType.Up).ToString()));
                    this.ddlEsrRounding.Items.Add(new ListItem("Always Up", ((int)EsrRoundingType.ForceUp).ToString()));
                    this.ddlEsrRounding.SelectedValue = ((byte)reqProperties.EsrRounding).ToString(CultureInfo.InvariantCulture);
                    this.chkESRManualAssignmentSupervisor.Checked = reqProperties.EnableESRManualAssignmentSupervisor;
                    this.chkEsrPrimaryAddressOnly.Checked = reqProperties.EsrPrimaryAddressOnly;
                }
                else
                {
                    this.ddlESRActivateType.Visible = false;
                    this.ddlESRArchiveType.Visible = false;
                    this.txtESRGracePeriod.Visible = false;
                    this.txtESRUsernameFormat.Visible = false;
                    this.txtESRHomeAddressFormat.Visible = false;
                    this.chkESRAssignmentsOnEmployeeAdd.Visible = false;
                    this.ddlEsrDetail.Visible = false;
                    this.chkESRActivateCar.Visible = false;
                    this.chkSummaryEsrInboundFile.Visible = false;
                    this.chkESRManualAssignmentSupervisor.Visible = false;
                    this.chkEsrPrimaryAddressOnly.Visible = false;
                }

                #endregion

                #region FW General

                // populate ddlists
                var months = new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                for (int x = 1; x < 13; x++)
                {
                    this.optFYStarts.Items.Add(new ListItem(months[x - 1], x.ToString(CultureInfo.InvariantCulture)));
                    this.optFYEnds.Items.Add(new ListItem(months[x - 1], x.ToString(CultureInfo.InvariantCulture)));
                }

                this.optLinkAttachmentDefault.Items.Add(new ListItem("Hyperlink", "0"));
                this.optLinkAttachmentDefault.Items.Add(new ListItem("File", "1"));

                // Framework - General
                this.chkUploadAttachmentEnabled.Checked = reqProperties.EnableAttachmentUpload;
                this.chkHyperlinkAttachmentsEnabled.Checked = reqProperties.EnableAttachmentHyperlink;
                if (this.chkHyperlinkAttachmentsEnabled.Checked == false)
                {
                    this.optLinkAttachmentDefault.SelectedIndex = 1;
                    this.optLinkAttachmentDefault.Enabled = false;
                }
                else
                {
                    this.optLinkAttachmentDefault.SelectedValue = reqProperties.LinkAttachmentDefault.ToString();
                }

                this.chkHyperlinkAttachmentsEnabled.InputAttributes.Add(
                    "onchange",
                    "if(this.checked == true) { document.getElementById(optLinkAttachmentDefault).disabled = false; } else { document.getElementById(optLinkAttachmentDefault).options[1].selected = true; document.getElementById(optLinkAttachmentDefault).disabled = true; }");

                this.txtMaxUploadSize.Text = reqProperties.MaxUploadSize.ToString(CultureInfo.InvariantCulture);
                this.chkNotesIconFlash.Checked = reqProperties.EnableFlashingNotesIcon;
                this.chkShowProductInSearch.Checked = reqProperties.ShowProductInSearch;
                this.optFYStarts.SelectedValue = reqProperties.FYStarts;
                this.optFYEnds.SelectedValue = reqProperties.FYEnds;
                this.txtDefaultPageSize.Text = reqProperties.DefaultPageSize.ToString(CultureInfo.InvariantCulture);
                this.chkfwEditMyDetails.Checked = reqProperties.EditMyDetails;

                // Framework - Products
                this.chkAutoUpdateLicenceTotal.Checked = reqProperties.AutoUpdateLicenceTotal;

                // Framework - Tasks
                this.chkTaskDueDateMandatory.Checked = reqProperties.TaskDueDateMandatory;
                this.chkTaskEndDateMandatory.Checked = reqProperties.TaskEndDateMandatory;
                this.txtTaskEscalationRepeat.Text = reqProperties.TaskEscalationRepeat.ToString(CultureInfo.InvariantCulture);
                this.chkTaskStartDateMandatory.Checked = reqProperties.TaskStartDateMandatory;

                // Framework - Help And Support
                this.chkInternalTicketsFW.Checked = reqProperties.EnableInternalSupportTickets;

                // Framework - Contract
                this.txtContractKey.Text = reqProperties.ContractKey;
                this.txtScheduleDefault.Text = reqProperties.ContractScheduleDefault;
                this.txtContractDescriptionTitle.Text = reqProperties.ContractDescTitle;
                this.txtContractDescriptionTitleAbbrev.Text = reqProperties.ContractDescShortTitle;
                this.txtContractCategoryTitle.Text = reqProperties.ContractCategoryTitle;
                this.chkContractCategoryMandatory.Checked = reqProperties.ContractCatMandatory;
                this.txtPenaltyClauseTitle.Text = reqProperties.PenaltyClauseTitle;
                this.chkTermTypeActive.Checked = reqProperties.TermTypeActive;
                this.chkContractDatesMandatory.Checked = reqProperties.ContractDatesMandatory;
                this.chkInflatorActive.Checked = reqProperties.InflatorActive;
                this.chkContractNumberGenerate.Checked = reqProperties.ContractNumGen;
                this.chkContractNumberUpdatable.Checked = reqProperties.EnableContractNumUpdate;

                this.txtContractNumberCurSeq.Text = reqProperties.ContractNumSeq.ToString(CultureInfo.InvariantCulture);
                this.chkAutoUpdateCV.Checked = reqProperties.AutoUpdateAnnualContractValue;
                this.chkArchivedNotesAdd.Checked = reqProperties.AllowArchivedNotesAdd;
                this.chkInvoiceFrequencyActive.Checked = reqProperties.InvoiceFreqActive;
                this.chkVariationAutoSeq.Checked = reqProperties.EnableVariationAutoSeq;
                this.txtValueComments.Text = reqProperties.ValueComments;

                // Framework - Invoice
                this.chkKeepInvForecasts.Checked = reqProperties.KeepInvoiceForecasts;
                this.chkPONumberGenerate.Checked = reqProperties.PONumberGenerate;
                this.txtPONumberFormat.Text = reqProperties.PONumberFormat;
                this.txtPOSequenceNumber.Text = reqProperties.PONumberSequence.ToString(CultureInfo.InvariantCulture);

                // Framework - Suppliers
                this.txtSupplierPrimaryTitle.Text = reqProperties.SupplierPrimaryTitle;
                this.txtSupplierRegionTitle.Text = reqProperties.SupplierRegionTitle;
                this.txtSupplierCategoryTitle.Text = reqProperties.SupplierCatTitle;
                this.chkSupplierCategoryMandatory.Checked = reqProperties.SupplierCatMandatory;
                this.txtSupplierVariationTitle.Text = reqProperties.SupplierVariationTitle;
                this.chkSupplierStatusMandatory.Checked = reqProperties.SupplierStatusEnforced;
                this.chkSupplierTurnoverEnabled.Checked = reqProperties.SupplierTurnoverEnabled;
                this.chkSupplierNumEmployeesEnabled.Checked = reqProperties.SupplierNumEmployeesEnabled;
                this.chkLastFinCheckEnabled.Checked = reqProperties.SupplierLastFinCheckEnabled;
                this.chkLastFinStatusEnabled.Checked = reqProperties.SupplierLastFinStatusEnabled;
                this.chkIntContactEnabled.Checked = reqProperties.SupplierIntContactEnabled;
                this.chkFYEEnabled.Checked = reqProperties.SupplierFYEEnabled;

                #endregion
            }
        }

        /// <summary>
        /// The btn cancel_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void btnCancel_Click(object sender, ImageClickEventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            switch (currentUser.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.contracts:
                    this.Response.Redirect("~/menumain.aspx?menusection=tailoring", true);
                    break;
                default:
                    this.Response.Redirect("~/tailoringmenu.aspx");
                    break;
            }
        }

        /// <summary>
        /// Save Button Click Event Handler
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            var clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
            cAccountProperties reqProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties.Clone();
            reqProperties.ActivateCarOnUserAdd = this.chkActivateCarOnUserAdd.Checked;
            reqProperties.AddLocations = this.chkaddlocations.Checked;
            reqProperties.AllowMultipleDestinations = this.chkAllowMultipleDestinations.Checked;
            reqProperties.AllowSelfReg = this.chkallowselfreg.Checked;
            reqProperties.AllowSelfRegAdvancesSignOff = this.chkselfregadvancessignoff.Checked;
            reqProperties.AllowSelfRegBankDetails = this.chkselfregbankdetails.Checked;
            reqProperties.AllowSelfRegCarDetails = this.chkselfregcardetails.Checked;
            reqProperties.AllowSelfRegDepartmentCostCode = this.chkselfregdepcostcode.Checked;
            reqProperties.AllowSelfRegEmployeeContact = this.chkselfregempconact.Checked;
            reqProperties.AllowSelfRegEmployeeInfo = this.chkselfregempinfo.Checked;
            reqProperties.AllowSelfRegHomeAddress = this.chkselfreghomaddr.Checked;
            reqProperties.AllowSelfRegRole = this.chkselfregrole.Checked;
            reqProperties.AllowSelfRegSignOff = this.chkselfregsignoff.Checked;
            reqProperties.AllowSelfRegUDF = this.chkselfregudf.Checked;
            reqProperties.AllowTeamMemberToApproveOwnClaim = this.chkTeamMemberApproveOwnClaims.Checked;
            reqProperties.AllowEmployeeInOwnSignoffGroup = this.chkEmployeeApproveOwnClaims.Checked;
            reqProperties.AllowUsersToAddCars = this.chkAllowUsersToAddCars.Checked;
            reqProperties.AllowEmpToSpecifyCarStartDateOnAdd = this.chkEmployeeSpecifyCarStartDate.Checked;
            reqProperties.AllowEmpToSpecifyCarDOCOnAdd = this.chkEmployeeSpecifyCarDOC.Checked;
            reqProperties.EmpToSpecifyCarStartDateOnAddMandatory = this.chkEmployeeSpecifyCarStartDateMandatory.Checked;
            reqProperties.AllowEmployeeToNotifyOfChangeOfDetails = this.chkemployeedetailschanged.Checked;
            reqProperties.NumberOfApproversToRememberForClaimantInApprovalMatrixClaim = Convert.ToInt32(this.cboNumberOfApproversToShowInClaimHistoryForApprovalMatrix.SelectedValue);

            reqProperties.ApproverDeclarationMsg = this.txtapproverdeclarationmsg.Text;
            reqProperties.AttachReceipts = this.chkattach.Checked;
            reqProperties.AutoAssignAllocation = this.chkautoassignallocation.Checked;
            reqProperties.CostCodeOwnerBaseKey = this.txtdefaultcostcodeowner_ID.Text.Contains(",") ? this.txtdefaultcostcodeowner_ID.Text : string.Empty;

            reqProperties.BaseCurrency = !string.IsNullOrEmpty(this.ddlBaseCurrency.SelectedValue) ? Convert.ToInt32(this.ddlBaseCurrency.SelectedValue) : 0;
            reqProperties.BlockInsuranceExpiry = this.chkblockinsuranceexpiry.Checked;
            reqProperties.BlockDrivingLicence = this.chkblockdrivinglicence.Checked;
            reqProperties.BlockMOTExpiry = this.chkblockmotexpiry.Checked;
            reqProperties.BlockTaxExpiry = this.chkblocktaxexpiry.Checked;
            reqProperties.BlockBreakdownCoverExpiry = this.chkBlockBreakdownCoverExpiry.Checked;
            var previousEnableAutomaticLookup = reqProperties != null && reqProperties.EnableAutomaticDrivingLicenceLookup;
            reqProperties.EnableAutomaticDrivingLicenceLookup = this.chkDrivingLicenceLookup.Checked;
            reqProperties.DrivingLicenceLookupFrequency = this.ddlDrivingLicenceLookupFrequency.SelectedValue;
            reqProperties.FrequencyOfConsentRemindersLookup = this.ddlAutoRevokeOfConsentLookupFrequency.SelectedValue;
            reqProperties.RemindClaimantOnDOCDocumentExpiryDays = Convert.ToInt32(this.cmbClaimantReminderDays.SelectedValue);
            reqProperties.RemindApproverOnDOCDocumentExpiryDays = Convert.ToInt32(this.cmbLineManagerReminderDays.SelectedValue);
            reqProperties.DrivingLicenceReviewFrequency = Convert.ToInt32(this.cmbReviewFrequencyDays.SelectedValue);
            reqProperties.EnableDrivingLicenceReview = this.chkLicenceReview.Checked;
            reqProperties.DrivingLicenceReviewReminder = this.chkLicenceReviewReminderNotification.Checked;
            var dutyOfCareAproverOnForm = this.lineManagerAsApprover.Checked
                ? this.lblLineManagerAsApprover.Text
                : this.lblteamAsApprover.Text;
            var previousDutyOfCareProvider = reqProperties.DutyOfCareApprover;
            reqProperties.DutyOfCareApprover =  dutyOfCareAproverOnForm;
            var vehicleStatusFilterOnDoC = chkDisableCarOutsideOfStartEndDate.Checked;
            var previousVehicleStatusFilter = reqProperties.DisableCarOutsideOfStartEndDate;
            reqProperties.DisableCarOutsideOfStartEndDate = vehicleStatusFilterOnDoC;           
            reqProperties.DutyOfCareTeamAsApprover = this.lineManagerAsApprover.Checked ? string.Empty : this.teamListForApprover.SelectedValue;
            reqProperties.UseDateOfExpenseForDutyOfCareChecks = this.chkDutyOfCareDateOfExpenseCheck.Checked;
            reqProperties.ClaimantDeclaration = this.chkclaimantdeclaration.Checked;
            reqProperties.CostCodesOn = this.chkcostcodeson.Checked;
            reqProperties.DeclarationMsg = this.txtdeclarationmsg.Text;
            reqProperties.MileageCalcType = Convert.ToByte(this.cmbmileagecalculationtype.SelectedValue);

            reqProperties.DefaultRole = Convert.ToInt32(this.cmbdefaultrole.SelectedValue);
            reqProperties.DefaultItemRole = Convert.ToInt32(this.cmbdefaultitemrole.SelectedValue);

            reqProperties.DelApprovals = this.chkdelapprovals.Checked;
            reqProperties.DelAuditLog = this.chkdelauditlog.Checked;
            reqProperties.DelCheckAndPay = this.chkdelcheckandpay.Checked;
            reqProperties.DelCorporateCards = this.chkdelcorporatecard.Checked;
            reqProperties.DelEmployeeAdmin = this.chkdelemployeeadmin.Checked;
            reqProperties.DelExports = this.chkdelexports.Checked;
            reqProperties.DelQEDesign = this.chkdelqedesign.Checked;
            reqProperties.DelReports = this.chkdelreports.Checked;
            reqProperties.DelReportsClaimants = this.chkdelclaimantreports.Checked;
            reqProperties.DelSetup = this.chkdelsetup.Checked;
            reqProperties.DelSubmitClaim = this.chkDelsSubmitClaims.Checked;
            reqProperties.DepartmentsOn = this.chkdepartmentson.Checked;
            reqProperties.DrilldownReport = new Guid(this.cmbdrilldown.SelectedValue);
            reqProperties.EditMyDetails = this.chkeditmydetails.Checked;
            reqProperties.EditPreviousClaims = this.chkEditPreviousClaim.Checked;
            reqProperties.EnableDelegateOptionsForDelegateAccessRole = this.chkDelOptionForDelAccessRole.Checked;

            reqProperties.IdleTimeout = Convert.ToInt32(cmbIdleTimeout.SelectedValue);
            reqProperties.CountdownTimer = Convert.ToInt32(cmbCountdown.SelectedValue);

            reqProperties.EnableInternalSupportTickets = this.chkInternalTickets.Checked;

            // Email Settings
            reqProperties.EmailServerAddress = this.txtEmailServer.Text;
            reqProperties.ErrorEmailAddress = this.txtErrorEmailSubmitAddress.Text;
            reqProperties.ErrorEmailFromAddress = this.txtErrorSubmitFromAddress.Text;
            reqProperties.AuditorEmailAddress = this.txtAuditorEmail.Text;
            switch (currentUser.CurrentActiveModule)
            {
                case Modules.SpendManagement:
                case Modules.SmartDiligence:
                case Modules.contracts:
                case Modules.Greenlight:
                case Modules.GreenlightWorkforce:
                    reqProperties.EmailServerFromAddress = this.txtEmailFromAddress.Text;
                    break;
            }

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.expenses:
                case Modules.Greenlight:
                case Modules.GreenlightWorkforce:
                    reqProperties.SourceAddress = (byte)(this.optserver.Checked ? 1 : 0);
                    break;
            }

            reqProperties.EmailAdministrator = this.txtEmailAdministrator.Text;
            reqProperties.EnterOdometerOnSubmit = !this.optodologin.Checked;
            reqProperties.ExchangeReadOnly = this.chkexchangereadonly.Checked;
            reqProperties.GlobalLocaleID = Convert.ToInt32(this.ddlDefaultLocale.SelectedValue);
            reqProperties.HomeCountry = !string.IsNullOrEmpty(this.ddlDefaultCountry.SelectedValue) ? Convert.ToInt32(this.ddlDefaultCountry.SelectedValue) : 0;
            reqProperties.Language = this.ddlDefaultLanguage.SelectedValue;
            reqProperties.MainAdministrator = Convert.ToInt32(this.ddlMainAdministrator.SelectedValue);

            if (this.chkrecordodometer.Checked)
            {
                reqProperties.OdometerDay = (byte)(string.IsNullOrEmpty(this.txtodometerday.Text) ? 0 : Convert.ToByte(this.txtodometerday.Text));
            }

            reqProperties.PartSubmit = this.chkpartsubmittal.Checked;
            reqProperties.PreApproval = this.chkpreapproval.Checked;
            reqProperties.ProjectCodesOn = this.chkprojectcodeson.Checked;
            reqProperties.PwdConstraint = (PasswordLength)Convert.ToInt32(this.cmblength.SelectedValue);

            if (Convert.ToInt32(this.txtexpires.Text) > 0)
            {
                reqProperties.PwdExpires = true;
                reqProperties.PwdExpiryDays = Convert.ToInt32(this.txtexpires.Text);
            }
            else
            {
                reqProperties.PwdExpires = false;
                reqProperties.PwdExpiryDays = 0;
            }

            reqProperties.PwdHistoryNum = Convert.ToInt32(this.txtprevious.Text);

            switch (reqProperties.PwdConstraint)
            {
                case PasswordLength.AnyLength:
                    reqProperties.PwdLength1 = 0;
                    reqProperties.PwdLength2 = 0;
                    break;
                case PasswordLength.EqualTo:
                case PasswordLength.GreaterThan:
                case PasswordLength.LessThan:
                    reqProperties.PwdLength1 = Convert.ToInt32(this.txtlength1.Text);
                    reqProperties.PwdLength2 = 0;
                    break;
                case PasswordLength.Between:
                    reqProperties.PwdLength1 = Convert.ToInt32(this.txtlength1.Text);
                    reqProperties.PwdLength2 = Convert.ToInt32(this.txtlength2.Text);
                    break;
            }

            reqProperties.PwdMaxRetries = Convert.ToByte(this.txtattempts.Text);
            reqProperties.PwdMustContainNumbers = this.chknumbers.Checked;
            reqProperties.PwdMustContainUpperCase = this.chkupper.Checked;
            reqProperties.PwdMustContainSymbol = this.chksymbol.Checked;
            reqProperties.RecordOdometer = this.chkrecordodometer.Checked;
            reqProperties.SearchEmployees = this.chksearchemployees.Checked;
            reqProperties.SendReviewRequests = this.chksendreviewrequest.Checked;
            reqProperties.AccountLockedMessage = this.txtaccountlocked.Text;
            reqProperties.AccountCurrentlyLockedMessage = this.txtcurrentlylocked.Text;

            // Use hidden field as a disabled checkbox from javascript does not get its latest value.
            bool showMileage;
            reqProperties.ShowMileageCatsForUsers = bool.TryParse(this.hdnAllowMileage.Value, out showMileage) ? showMileage : this.chkAllowMilage.Checked;

            reqProperties.ShowReviews = this.chkshowreviews.Checked;
            reqProperties.SingleClaim = this.chksingleclaim.Checked;

            reqProperties.UseCostCodeDescription = this.chkcostcodedesc.Checked;
            reqProperties.UseCostCodeOnGenDetails = this.chkcostcodeongendet.Checked;
            reqProperties.UseCostCodes = this.chkcostcodes.Checked;
            reqProperties.UseDepartmentCodeDescription = this.chkdepartmentdesc.Checked;
            reqProperties.UseDepartmentCodes = this.chkdepartment.Checked;
            reqProperties.UseDeptOnGenDetails = this.chkdepartmentongendet.Checked;
            reqProperties.UseMapPoint = this.chkUseMapPoint.Checked;
            reqProperties.UseProjectCodeDescription = this.chkprojectcodedesc.Checked;
            reqProperties.UseProjectCodeOnGenDetails = this.chkprojectcodeongendet.Checked;
            reqProperties.UseProjectCodes = this.chkprojectcodes.Checked;
            reqProperties.AllowSelfRegItemRole = this.chkselfregitemrole.Checked;
            reqProperties.OnlyCashCredit = this.chkonlycashcredit.Checked;

            reqProperties.MandatoryPostcodeForAddresses = this.chkMandatoryPostcodeForAddresses.Checked;
            reqProperties.MandatoryPostcodeForAddresses = this.chkMandatoryPostcodeForAddresses.Checked;
            reqProperties.ClaimantsCanAddCompanyLocations = this.chkClaimantsCanSaveCompanyAddresses.Checked;
            reqProperties.RetainLabelsTime = this.cmbRetainLabelsTime.SelectedValue;
            reqProperties.ShowFullHomeAddressOnClaims = this.chkShowFullHomeAddress.Checked;
            reqProperties.DisableCarOutsideOfStartEndDate = this.chkDisableCarOutsideOfStartEndDate.Checked;
            reqProperties.EnableCalculationsForAllocatingFuelReceiptVatToMileage = this.chkEnableVatOptions.Checked;

            // ESR Options
            if (currentUser.Account.IsNHSCustomer)
            {
                reqProperties.CheckESRAssignmentOnEmployeeAdd = this.chkESRAssignmentsOnEmployeeAdd.Checked;
                reqProperties.AutoActivateType = (AutoActivateType)Enum.Parse(typeof(AutoActivateType), this.ddlESRActivateType.SelectedValue);
                reqProperties.AutoArchiveType = (AutoArchiveType)Enum.Parse(typeof(AutoArchiveType), this.ddlESRArchiveType.SelectedValue);

                reqProperties.ArchiveGracePeriod = (short)(this.txtESRGracePeriod.Text != string.Empty ? Convert.ToInt16(this.txtESRGracePeriod.Text) : 0);
                reqProperties.IncludeAssignmentDetails = (IncludeEsrDetails)int.Parse(this.ddlEsrDetail.SelectedValue);
                reqProperties.EsrAutoActivateCar = this.chkESRActivateCar.Checked;
                reqProperties.DisplayEsrAddressesInSearchResults = this.chkDisplayEsrAddressesInSearchResults.Checked;
                reqProperties.SummaryEsrInboundFile = this.chkSummaryEsrInboundFile.Checked;
                reqProperties.EsrRounding = (EsrRoundingType)Enum.Parse(typeof(EsrRoundingType), this.ddlEsrRounding.SelectedValue);
                reqProperties.EnableESRManualAssignmentSupervisor = this.chkESRManualAssignmentSupervisor.Checked;
                reqProperties.EsrPrimaryAddressOnly = this.chkEsrPrimaryAddressOnly.Checked;
            }

            // Mobile Devices
            reqProperties.UseMobileDevices = this.chkEnableMobileDevices.Checked;
            //Expedite Tab - Validation Options
            reqProperties.AllowReceiptTotalToPassValidation = this.chkValidationOptionsAllowReceipt.Checked;
            // Framework - General
            reqProperties.EnableAttachmentUpload = this.chkUploadAttachmentEnabled.Checked;
            reqProperties.EnableAttachmentHyperlink = this.chkHyperlinkAttachmentsEnabled.Checked;
            reqProperties.LinkAttachmentDefault = Convert.ToByte(this.optLinkAttachmentDefault.SelectedValue);
            int maxUpload;
            int.TryParse(this.txtMaxUploadSize.Text, out maxUpload);
            reqProperties.MaxUploadSize = maxUpload == 0 ? 1024 : maxUpload;
            reqProperties.EnableFlashingNotesIcon = this.chkNotesIconFlash.Checked;
            reqProperties.ShowProductInSearch = this.chkShowProductInSearch.Checked;
            reqProperties.FYStarts = this.optFYStarts.SelectedValue;
            reqProperties.FYEnds = this.optFYEnds.SelectedValue;
            int pageSize;
            int.TryParse(this.txtDefaultPageSize.Text, out pageSize);
            reqProperties.DefaultPageSize = pageSize == 0 ? 50 : pageSize;
            switch (currentUser.CurrentActiveModule)
            {
                case Modules.contracts:
                    // Framework - Help & Support
                    reqProperties.EnableInternalSupportTickets = this.chkInternalTicketsFW.Checked;
                    break;
                case Modules.SpendManagement:
                case Modules.SmartDiligence:
                    reqProperties.EditMyDetails = this.chkfwEditMyDetails.Checked;
                    // OVERRIDES THE EXP CHECKBOX IF THIS IS ACTIVE MODULE
                    break;
            }

            byte frequencytype = 0;
            int frequencyvalue = 0;

            reqProperties.FlagMessage = this.txtflagmessage.Text;
            
                frequencytype = byte.Parse(this.cmbfrequencytype.SelectedValue);
                reqProperties.FrequencyType = frequencytype;
                if (!string.IsNullOrEmpty(this.txtfrequencyvalue.Text))
                {
                    frequencyvalue = Convert.ToInt32(this.txtfrequencyvalue.Text);
                }

                reqProperties.FrequencyValue = frequencyvalue;
            if (this.chkEnableClaimApprovalReminder.Checked)
            {
                reqProperties.ClaimApprovalReminderFrequency = Convert.ToInt32(this.ddlClaimApprovalReminderFrequency.Text);
                reqProperties.EnableClaimApprovalReminders = true;
            }
            else
            {
                reqProperties.ClaimApprovalReminderFrequency = 0;
                reqProperties.EnableClaimApprovalReminders = false;
            }

            if (this.chkEnableCurrentClaimsReminder.Checked)
            {
                reqProperties.CurrentClaimsReminderFrequency = int.Parse(this.ddlCurrentClaimReminderFrequency.Text);
                reqProperties.EnableCurrentClaimsReminders = true;
            }
            else
            {
                reqProperties.CurrentClaimsReminderFrequency = 0;
                reqProperties.EnableCurrentClaimsReminders = false;
            }

            // Framework - Products
            reqProperties.AutoUpdateLicenceTotal = this.chkAutoUpdateLicenceTotal.Checked;

            // Framework - Tasks
            reqProperties.TaskStartDateMandatory = this.chkTaskStartDateMandatory.Checked;
            reqProperties.TaskDueDateMandatory = this.chkTaskDueDateMandatory.Checked;
            reqProperties.TaskEndDateMandatory = this.chkTaskEndDateMandatory.Checked;
            int taskEsc;
            int.TryParse(this.txtTaskEscalationRepeat.Text, out taskEsc);
            reqProperties.TaskEscalationRepeat = taskEsc == 0 ? 7 : taskEsc;

            // Framework - Contract
            reqProperties.ContractKey = this.txtContractKey.Text;
            reqProperties.ContractScheduleDefault = this.txtScheduleDefault.Text;
            reqProperties.ContractDescTitle = this.txtContractDescriptionTitle.Text;
            reqProperties.ContractDescShortTitle = this.txtContractDescriptionTitleAbbrev.Text;
            reqProperties.ContractCategoryTitle = this.txtContractCategoryTitle.Text;
            reqProperties.ContractCatMandatory = this.chkContractCategoryMandatory.Checked;
            reqProperties.PenaltyClauseTitle = this.txtPenaltyClauseTitle.Text;
            reqProperties.TermTypeActive = this.chkTermTypeActive.Checked;
            reqProperties.ContractDatesMandatory = this.chkContractDatesMandatory.Checked;
            reqProperties.InflatorActive = this.chkInflatorActive.Checked;
            reqProperties.ContractNumGen = this.chkContractNumberGenerate.Checked;
            reqProperties.EnableContractNumUpdate = this.chkContractNumberUpdatable.Checked;
            reqProperties.ValueComments = this.txtValueComments.Text;

            int seq;
            int.TryParse(this.txtContractNumberCurSeq.Text, out seq);
            reqProperties.ContractNumSeq = seq == 0 ? 1 : seq;
            reqProperties.AutoUpdateAnnualContractValue = this.chkAutoUpdateCV.Checked;
            reqProperties.AllowArchivedNotesAdd = this.chkArchivedNotesAdd.Checked;
            reqProperties.InvoiceFreqActive = this.chkInvoiceFrequencyActive.Checked;
            reqProperties.EnableVariationAutoSeq = this.chkVariationAutoSeq.Checked;

            // Framework - Invoice
            reqProperties.KeepInvoiceForecasts = this.chkKeepInvForecasts.Checked;
            reqProperties.PONumberGenerate = this.chkPONumberGenerate.Checked;
            reqProperties.PONumberFormat = this.txtPONumberFormat.Text;
            int.TryParse(this.txtPOSequenceNumber.Text, out seq);
            reqProperties.PONumberSequence = seq == 1 ? 1 : seq;

            // Framework - Suppliers
            reqProperties.SupplierPrimaryTitle = this.txtSupplierPrimaryTitle.Text;
            reqProperties.SupplierRegionTitle = this.txtSupplierRegionTitle.Text;
            reqProperties.SupplierCatTitle = this.txtSupplierCategoryTitle.Text;
            reqProperties.SupplierCatMandatory = this.chkSupplierCategoryMandatory.Checked;
            reqProperties.SupplierVariationTitle = this.txtSupplierVariationTitle.Text;
            reqProperties.SupplierStatusEnforced = this.chkSupplierStatusMandatory.Checked;
            reqProperties.SupplierTurnoverEnabled = this.chkSupplierTurnoverEnabled.Checked;
            reqProperties.SupplierNumEmployeesEnabled = this.chkSupplierNumEmployeesEnabled.Checked;
            reqProperties.SupplierLastFinCheckEnabled = this.chkLastFinCheckEnabled.Checked;
            reqProperties.SupplierLastFinStatusEnabled = this.chkLastFinStatusEnabled.Checked;
            reqProperties.SupplierIntContactEnabled = this.chkIntContactEnabled.Checked;
            reqProperties.SupplierFYEEnabled = this.chkFYEEnabled.Checked;
            reqProperties.HomeAddressKeyword = this.txtHomeAddressKeyword.Text.Equals(string.Empty) ? "home" : this.txtHomeAddressKeyword.Text;
            reqProperties.WorkAddressKeyword = this.txtWorkAddressKeyword.Text.Equals(string.Empty) ? "office" : this.txtWorkAddressKeyword.Text;
            reqProperties.ForceAddressNameEntry = this.chkForceAddressNameEntry.Checked;
            reqProperties.MultipleWorkAddress = this.chkMultipleWorkAddress.Checked;
            reqProperties.AddressNameEntryMessage = this.txtAddressNameEntryMessage.Text;
            
            if (this.chkEnableAutoUpdateOfExchangeRates.Checked && !reqProperties.EnableAutoUpdateOfExchangeRates)
            {
                // Activated Auto update of exchange rates so need to clear the cache.
                var currencies = new cCurrencies(currentUser.AccountID, currentUser.CurrentSubAccountId);
                foreach (cCurrency currency in currencies.currencyList.Values)
                {
                    cCurrency.RemoveFromCache(currentUser.AccountID, currency.currencyid);
                }
            }
            
            reqProperties.EnableAutoUpdateOfExchangeRates = this.chkEnableAutoUpdateOfExchangeRates.Checked;
            reqProperties.BlockUnmachedExpenseItemsBeingSubmitted = this.chkBlockUnmatchedExpenseItemsBeingSubmitted.Checked;

            reqProperties.VehicleLookup = currentUser.Account.HasLicensedElement(SpendManagementElement.VehicleLookup) && this.chkVehicleLookup.Checked;

            clsSubAccounts.SaveAccountProperties(reqProperties, currentUser.EmployeeID, currentUser.isDelegate ? currentUser.Delegate.EmployeeID : (int?)null);

            if (reqProperties.EnableCurrentClaimsReminders)
            {
                Employee.SetInitialClaimantReminderDate(currentUser.AccountID);
            }
            if (previousDutyOfCareProvider != dutyOfCareAproverOnForm)
            {
                var dutyOfCare = new DutyOfCare();
                dutyOfCare.ChangeTeamViewFilters(currentUser.AccountID, dutyOfCareAproverOnForm);
                dutyOfCare.ChangeTeamFormFilters(currentUser.AccountID, dutyOfCareAproverOnForm);

                var drivingLicence = new DrivingLicence();
                drivingLicence.ChangeTeamViewFilters(currentUser.AccountID, dutyOfCareAproverOnForm);
                drivingLicence.ChangeTeamFormFilters(currentUser.AccountID, dutyOfCareAproverOnForm);
                // this will refresh the cache.
                var customEntities = new cCustomEntities(currentUser,true);
            }
            //Reset the filters on DOC Team views if the general option is modified
            if (previousVehicleStatusFilter != vehicleStatusFilterOnDoC)
            {
                var dutyOfCare = new DutyOfCare();
                dutyOfCare.ChangeVehicleStatusFilter(currentUser.AccountID, vehicleStatusFilterOnDoC);    
                // this will refresh the cache.
                var customEntities = new cCustomEntities(currentUser, true);
            }

            //Check if the Automatic lookup is enabled for the account 
            if (this.chkDrivingLicenceLookup.Checked && !previousEnableAutomaticLookup)
            {
                var employees = new cEmployees(currentUser.AccountID);
                var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID));
                var updatedEmployeeList = employees.UpdateEmployeesDrivingLicenceLookupDate(connection);
                
                ///Reset the cache to reflect the lookup date change
                foreach (var employeeId in updatedEmployeeList)
                {
                    employees.Cache.Delete(currentUser.AccountID, Employee.CacheArea, employeeId.ToString());
                }
            }
            
            // if mobile devices and attach receipts enabled, ensure PNG attachment types are enabled
            if (reqProperties.AttachReceipts && reqProperties.UseMobileDevices)
            {
                var globalMimeTypes = new cGlobalMimeTypes(currentUser.AccountID);
                var mimeTypes = new cMimeTypes(currentUser.AccountID, currentUser.CurrentSubAccountId);

                Guid pngMimeType = globalMimeTypes.getMimeTypeByExtension("PNG").GlobalMimeID;
                if (mimeTypes.GetMimeTypeByGlobalID(pngMimeType) == null)
                {
                    mimeTypes.SaveMimeType(pngMimeType);
                }
            }

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.contracts:
                    this.Response.Redirect(cMisc.Path + "/menumain.aspx?menusection=tailoring", true);
                    break;
                default:
                    this.Response.Redirect(cMisc.Path + "/tailoringmenu.aspx", true);
                    break;
            }
        }

        /// <summary>
        /// Validates a custom home/office keyword by ensuring that it isn't in use as a label
        /// </summary>
        /// <param name="keyword">The home/office keyword to be validated</param>
        /// <returns>A success indicator</returns>
        [WebMethod(EnableSession = true)]
        public static bool ValidateAddressKeyword(string keyword)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            List<AddressLabel> labels = new AddressLabels(currentUser.AccountID).Search(keyword, AddressFilter.All);

            return keyword == "home" || keyword == "office" || labels.Any(label => label.Text.ToLower() == keyword.ToLower()) == false;
        }

        /// <summary>
        /// The hide tabs.
        /// </summary>
        /// <param name="activeModule">
        /// The active module.
        /// </param>
        private void HideTabs(Modules activeModule)
        {
            var sb = new StringBuilder();
            var tabIdsToHide = new List<string>();
            sb.AppendFormat("hidePageOptions({0});\n", (int)activeModule);

            switch (activeModule)
            {
                case Modules.SpendManagement:
                case Modules.SmartDiligence:
                case Modules.contracts:
                    tabIdsToHide.Add(this.tabGeneral.ClientID);
                    tabIdsToHide.Add(this.tabSelfRegistration.ClientID);
                    tabIdsToHide.Add(this.tabDelegates.ClientID);
                    tabIdsToHide.Add(this.tabDeclarations.ClientID);
                    tabIdsToHide.Add(this.tabMobileDevices.ClientID);
                    tabIdsToHide.Add(this.tabEmployees.ClientID);
                    tabIdsToHide.Add(this.tabExpedite.ClientID);
                    break;
                case Modules.expenses:
                    tabIdsToHide.Add(this.tabFWGeneral.ClientID);
                    tabIdsToHide.Add(this.tabContracts.ClientID);
                    tabIdsToHide.Add(this.tabInvoices.ClientID);
                    tabIdsToHide.Add(this.tabSupplier.ClientID);

                    // hide error email text boxes and disable validators
                    sb.Append("if(document.getElementById('divContractEmailFrom') != null) { document.getElementById('divContractEmailFrom').style.display = 'none'; }\n");
                    this.reqErrorEmailSubmitAddress.Enabled = false;
                    this.reqErrorSubmitFromAddress.Enabled = false;
                    this.regexErrorEmailSubmitAddress.Enabled = false;
                    this.regexErrorSubmitFromAddress.Enabled = false;
                    this.reqAuditorEmail.Enabled = false;
                    this.regexAuditorEmail.Enabled = false;
                    this.reqEmailFromAddress.Enabled = false;
                    this.regexEmailFromAddress.Enabled = false;
                    break;
                case Modules.Greenlight:
                case Modules.GreenlightWorkforce:
                    tabIdsToHide.Add(this.tabFWGeneral.ClientID);
                    tabIdsToHide.Add(this.tabDeclarations.ClientID);
                    tabIdsToHide.Add(this.tabContracts.ClientID);
                    tabIdsToHide.Add(this.tabInvoices.ClientID);
                    tabIdsToHide.Add(this.tabSupplier.ClientID);
                    tabIdsToHide.Add(this.tabMobileDevices.ClientID);
                    tabIdsToHide.Add(this.tabSelfRegistration.ClientID);
                    tabIdsToHide.Add(this.tabExpedite.ClientID);

                    // hide error email text boxes and disable validators
                    sb.Append("if(document.getElementById('divContractEmailFrom') != null) { document.getElementById('divContractEmailFrom').style.display = 'none'; }\n");
                    this.reqErrorEmailSubmitAddress.Enabled = false;
                    this.reqErrorSubmitFromAddress.Enabled = false;
                    this.regexErrorEmailSubmitAddress.Enabled = false;
                    this.regexErrorSubmitFromAddress.Enabled = false;
                    this.reqAuditorEmail.Enabled = false;
                    this.regexAuditorEmail.Enabled = false;
                    this.reqEmailFromAddress.Enabled = false;
                    this.regexEmailFromAddress.Enabled = false;
                    break;
            }

            foreach (string s in tabIdsToHide)
            {
                sb.Append("hideAjaxTab('" + s + "');\n");
            }

            string initialTab = this.tabGeneral.ClientID;
            switch (activeModule)
            {
                case Modules.contracts:
                case Modules.SmartDiligence:
                    initialTab = this.tabFWGeneral.ClientID;
                    break;
                default:
                    initialTab = this.tabGeneral.ClientID;
                    break;
            }

            for (int tabIdx = 0; tabIdx < this.tabsGeneralOptions.Tabs.Count; tabIdx++)
            {
                if (this.tabsGeneralOptions.Tabs[tabIdx].ClientID == initialTab)
                {
                    sb.AppendFormat("currentTabContainerId = '{0}'; currentActiveTabId = '{1}';\n", this.tabsGeneralOptions.ClientID, tabIdx);
                    break;
                }
            }

            ClientScriptManager smgr = this.ClientScript;
            smgr.RegisterStartupScript(this.GetType(), "hideOptions", sb.ToString(), true);
        }

        /// <summary>
        /// The create drill down.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="basetable">
        /// The basetable.
        /// </param>
        /// <param name="employeeid">
        /// The employeeid.
        /// </param>
        /// <param name="reportid">
        /// The reportid.
        /// </param>
        /// <param name="subAccountID">
        /// The sub account id.
        /// </param>
        private void createDrillDown(int accountid, Guid basetable, int employeeid, Guid? reportid, int subAccountID)
        {
            string reportsPath = string.Empty;

            CurrentUser currentUser = cMisc.GetCurrentUser();
            switch (currentUser.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.contracts:
                    reportsPath = ConfigurationManager.AppSettings["ReportsServicePath"];
                    break;
                default:
                    reportsPath = ConfigurationManager.AppSettings["ReportsServicePath"];
                    break;
            }

            cReports clsreports = new cReports(accountid, subAccountID);
           
            try
            {
                ArrayList items = clsreports.CreateDrillDownList(basetable, employeeid, subAccountID);

                this.cmbdrilldown.Items.Add(new ListItem("[None]", Guid.Empty.ToString()));
                foreach (var values in items.Cast<object[]>())
                {
                    this.cmbdrilldown.Items.Add(new ListItem((string)values[1], values[0].ToString()));
                }

                if (reportid.HasValue)
                {
                    if (this.cmbdrilldown.Items.FindByValue(reportid.ToString()) != null)
                    {
                        this.cmbdrilldown.Items.FindByValue(reportid.ToString()).Selected = true;
                    }
                }
            }
            catch
            {
                this.cmbdrilldown.Items.Add(new ListItem("[None]", Guid.Empty.ToString()));
            }
        }

        /// <summary>
        /// Binds the jQuery autocomplete to the cost code owner text box
        /// </summary>
        /// <param name="user">Current User object</param>
        private void BindOwnerAutoComplete(CurrentUser user)
        {
            // bind the jQuery auto complete to the txtUser field
            cTables clsTables = new cTables(user.AccountID);
            List<object> acParams = AutoComplete.getAutoCompleteQueryParams("signoffentities");
            string acBindStr = AutoComplete.createAutoCompleteBindString(
                "txtdefaultcostcodeowner", 15, clsTables.GetTableByName("signoffentities").TableID, (Guid)acParams[0], (List<Guid>)acParams[1], 500, keyFieldIsString: true);

            ClientScriptManager scmgr = this.ClientScript;
            scmgr.RegisterStartupScript(this.GetType(), "autocompleteBind", AutoComplete.generateScriptRegisterBlock(new List<string>() { acBindStr }), true);
        }

        private void SetNumberOfRememberedApproversDropDown()
        {
            this.cboNumberOfApproversToShowInClaimHistoryForApprovalMatrix.Items.Add(new ListItem("[None]", "0"));
            for (int i = 1; i < 11; i++)
            {
                this.cboNumberOfApproversToShowInClaimHistoryForApprovalMatrix.Items.Add(new ListItem(i.ToString(CultureInfo.InvariantCulture), i.ToString(CultureInfo.InvariantCulture)));
            }
        }
    }
}

