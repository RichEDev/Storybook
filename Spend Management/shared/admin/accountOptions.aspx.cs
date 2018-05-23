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

    using BusinessLogic;
    using BusinessLogic.AccountProperties;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Enums;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.GeneralOptions.AddEditExpense;
    using BusinessLogic.GeneralOptions.ESR;
    using BusinessLogic.GeneralOptions.Password;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using shared.code;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;

    #endregion

    /// <summary>
    ///   The account options.
    /// </summary>
    public partial class accountOptions : Page
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

        /// <summary>
        /// An instance of <see cref="IDataFactory{IAccountProperty,AccountPropertyCacheKey}"/> to get a <see cref="IAccountProperty"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IAccountProperty, AccountPropertyCacheKey> AccountPropertiesFactory { get; set; }

        /// <summary>
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

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
                case Modules.Contracts:
                case Modules.SmartDiligence:
                    currentTab = 1;
                    break;
                case Modules.Expenses:
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
                case Modules.Contracts:
                    this.Master.helpid = 1140;
                    break;
                default:
                    this.Master.helpid = 1043;
                    break;
            }

            var usingExpenses = currentUser.CurrentActiveModule == Modules.Expenses;
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
            this.VehicleLookupEnabled.Visible =
                currentUser.Account.HasLicensedElement(SpendManagementElement.VehicleLookup);
            this.SetNumberOfRememberedApproversDropDown();

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

                var clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);

                var generalOptions = this.GeneralOptionsFactory[currentUser.CurrentSubAccountId].WithAll();

                var clsEmployees = new cEmployees(currentUser.AccountID);

                var module = this.ProductModuleFactory[currentUser.CurrentActiveModule];

                string brandName = (module != null) ? module.BrandName : "Expenses";
                this.optserver.Text = string.Format("{0} server", brandName);

                #region General Options

                this.chkpreapproval.Checked = generalOptions.Claim.PreApproval;

                if (currentUser.Account.EmployeeSearchEnabled)
                {
                    this.chksearchemployees.Checked = generalOptions.Employee.SearchEmployees;
                }
                else
                {
                    this.chksearchemployees.Enabled = false;
                    this.chksearchemployees.Checked = false;
                }

                chkemployeedetailschanged.Checked = false;
                if (generalOptions.MyDetails.AllowEmployeeToNotifyOfChangeOfDetails)
                {
                    this.chkemployeedetailschanged.Checked = true;
                }


                if (currentUser.Account.HotelReviewsEnabled)
                {
                    this.chkshowreviews.Checked = generalOptions.Hotel.ShowReviews;
                    this.chksendreviewrequest.Checked = generalOptions.Hotel.SendReviewRequests;
                }
                else
                {
                    this.chkshowreviews.Enabled = false;
                    this.chkshowreviews.Checked = false;
                    this.chksendreviewrequest.Checked = false;
                    this.chksendreviewrequest.Enabled = false;
                }

                this.chkTeamMemberApproveOwnClaims.Checked = generalOptions.Claim.AllowTeamMemberToApproveOwnClaim;
                this.chkEmployeeApproveOwnClaims.Checked = generalOptions.Claim.AllowEmployeeInOwnSignoffGroup;

                this.chkrecordodometer.Checked = generalOptions.Mileage.RecordOdometer;

                if (currentUser.Account.CorporateCardsEnabled)
                {
                    this.chkonlycashcredit.Checked = generalOptions.Claim.OnlyCashCredit;
                    this.chkpartsubmittal.Checked = generalOptions.Claim.PartSubmit;
                }
                else
                {
                    this.chkonlycashcredit.Checked = false;
                    this.chkonlycashcredit.Enabled = false;
                    this.chkpartsubmittal.Checked = false;
                    this.chkpartsubmittal.Enabled = false;
                }

                this.chkeditmydetails.Checked = generalOptions.MyDetails.EditMyDetails;
                this.chkEditPreviousClaim.Checked = generalOptions.Claim.EditPreviousClaims;

                if (generalOptions.Mileage.EnterOdometerOnSubmit)
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
                    this.txtodometerday.Text = generalOptions.Mileage.OdometerDay.ToString(CultureInfo.InvariantCulture);
                    this.compOdoDayLessThan.Enabled = true;
                    this.compOdoDayGreaterThan.Enabled = true;
                    this.reqOdoDay.Enabled = true;
                }

                this.chkInternalTickets.Checked = generalOptions.HelpAndSupport.EnableInternalSupportTickets;

                this.chkallowselfreg.Checked = generalOptions.SelfRegistration.AllowSelfReg;

                var clsAccessRoles = new cAccessRoles(currentUser.AccountID, cAccounts.getConnectionString(currentUser.AccountID));

                int defaultRole = generalOptions.SelfRegistration.DefaultRole.HasValue ? generalOptions.SelfRegistration.DefaultRole.Value : 0;

                this.cmbdefaultrole.Items.AddRange(clsAccessRoles.CreateDropDown(defaultRole, true).ToArray());

                var clsItemRoles = new ItemRoles(currentUser.AccountID);

                int defaultItemRole = generalOptions.SelfRegistration.DefaultItemRole.HasValue ? generalOptions.SelfRegistration.DefaultItemRole.Value : 0;

                this.cmbdefaultitemrole.Items.AddRange(clsItemRoles.CreateDropDown(defaultItemRole, true).ToArray());

                if (generalOptions.SelfRegistration.AllowSelfReg)
                {
                    if (currentUser.Account.AdvancesEnabled)
                    {
                        this.chkselfregadvancessignoff.Checked = generalOptions.SelfRegistration.AllowSelfRegAdvancesSignOff;
                    }
                    else
                    {
                        this.chkselfregadvancessignoff.Checked = false;
                        this.chkselfregadvancessignoff.Enabled = false;
                    }

                    this.chkselfregbankdetails.Checked = generalOptions.SelfRegistration.AllowSelfRegBankDetails;
                    this.chkselfregcardetails.Checked = generalOptions.SelfRegistration.AllowSelfRegCarDetails;
                    this.chkselfregdepcostcode.Checked = generalOptions.SelfRegistration.AllowSelfRegDepartmentCostCode;
                    this.chkselfregempconact.Checked = generalOptions.SelfRegistration.AllowSelfRegEmployeeContact;
                    this.chkselfregempinfo.Checked = generalOptions.SelfRegistration.AllowSelfRegEmployeeInfo;
                    this.chkselfreghomaddr.Checked = generalOptions.SelfRegistration.AllowSelfRegHomeAddress;
                    this.chkselfregrole.Checked = generalOptions.SelfRegistration.AllowSelfRegRole;
                    this.chkselfregsignoff.Checked = generalOptions.SelfRegistration.AllowSelfRegSignOff;
                    this.chkselfregudf.Checked = generalOptions.SelfRegistration.AllowSelfRegUDF;
                    this.chkselfregitemrole.Checked = generalOptions.SelfRegistration.AllowSelfRegItemRole;
                }

                this.txtflagmessage.Text = generalOptions.Flag.FlagMessage;

                this.txtfrequencyvalue.Text = generalOptions.Claim.FrequencyValue.ToString();

                this.cmbfrequencytype.Items.Add(new ListItem("Month", "1"));
                this.cmbfrequencytype.Items.Add(new ListItem("Week", "2"));

                if (this.cmbfrequencytype.Items.FindByValue(generalOptions.Claim.FrequencyType.ToString()) != null)
                {
                    this.cmbfrequencytype.Items.FindByValue(generalOptions.Claim.FrequencyType.ToString()).Selected = true;
                }
                this.chkEnableClaimApprovalReminder.Checked = generalOptions.Reminders.EnableClaimApprovalReminders;
                this.chkEnableCurrentClaimsReminder.Checked = generalOptions.Reminders.EnableCurrentClaimsReminders;
                this.ddlClaimApprovalReminderFrequency.Text = generalOptions.Reminders.ClaimApprovalReminderFrequency.ToString();
                this.ddlCurrentClaimReminderFrequency.Text = generalOptions.Reminders.CurrentClaimsReminderFrequency.ToString();
                this.chkEnableCurrentClaimsReminder.Attributes.Add("onclick", "toggleValidator(this)");
                this.chkEnableClaimApprovalReminder.Attributes.Add("onclick", "toggleValidator(this)");



                this.chkdelsetup.Checked = generalOptions.Delegate.DelSetup;
                this.chkdelemployeeadmin.Checked = generalOptions.Delegate.DelEmployeeAdmin;
                this.chkdelreports.Checked = generalOptions.Delegate.DelReports;
                this.chkdelclaimantreports.Checked = generalOptions.Delegate.DelReportsClaimants;
                this.chkdelcheckandpay.Checked = generalOptions.Delegate.DelCheckAndPay;
                this.chkdelqedesign.Checked = generalOptions.Delegate.DelQEDesign;
                this.chkDelsSubmitClaims.Checked = generalOptions.Delegate.DelSubmitClaim;
                this.chkDelOptionForDelAccessRole.Checked = generalOptions.Delegate.EnableDelegateOptionsForDelegateAccessRole;
                if (currentUser.Account.CorporateCardsEnabled)
                {
                    this.chkdelcorporatecard.Checked = generalOptions.Delegate.DelCorporateCards;
                }
                else
                {
                    this.chkdelcorporatecard.Checked = false;
                    this.chkdelcorporatecard.Enabled = false;
                }

                if (currentUser.Account.AdvancesEnabled)
                {
                    this.chkdelapprovals.Checked = generalOptions.Delegate.DelApprovals;
                }
                else
                {
                    this.chkdelapprovals.Checked = false;
                    this.chkdelapprovals.Enabled = false;
                }

                this.chkdelexports.Checked = generalOptions.Delegate.DelExports;
                this.chkdelauditlog.Checked = generalOptions.Delegate.DelAuditLog;
                this.chkclaimantdeclaration.Checked = generalOptions.Claim.ClaimantDeclaration;
                this.txtdeclarationmsg.Text = generalOptions.Claim.DeclarationMsg;
                this.txtapproverdeclarationmsg.Text = generalOptions.Claim.ApproverDeclarationMsg;
                this.chksendreviewrequest.Checked = generalOptions.Hotel.SendReviewRequests;
                this.createDrillDown(currentUser.AccountID, new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8"), currentUser.EmployeeID, generalOptions.Report.DrilldownReport, currentUser.CurrentSubAccountId);

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
                    generalOptions.ApprovalMatrix.NumberOfApproversToRememberForClaimantInApprovalMatrixClaim.ToString(CultureInfo.InvariantCulture)).Selected =
                    true; 

                // mobile devices tab
                this.chkEnableMobileDevices.Checked = generalOptions.Mobile.UseMobileDevices;
                //Expedite Tab - Validation Options
                this.chkValidationOptionsAllowReceipt.Checked = generalOptions.Expedite.AllowReceiptTotalToPassValidation;
                this.chkEmployeeSpecifyCarStartDate.Checked = generalOptions.Car.AllowEmpToSpecifyCarStartDateOnAdd;
                this.chkEmployeeSpecifyCarStartDateMandatory.Checked = generalOptions.Car.EmpToSpecifyCarStartDateOnAddMandatory;
                this.chkEmployeeSpecifyCarDOC.Checked = generalOptions.Car.AllowEmpToSpecifyCarDOCOnAdd;
                cmbCountdown.SelectedValue = generalOptions.SessionTimeout.CountdownTimer.ToString(CultureInfo.InvariantCulture);
                cmbIdleTimeout.SelectedValue = generalOptions.SessionTimeout.IdleTimeout.ToString(CultureInfo.InvariantCulture);
                chkBlockUnmatchedExpenseItemsBeingSubmitted.Checked = generalOptions.Claim.BlockUnmatchedExpenseItemsBeingSubmitted;
                #endregion General Options

                #region Regional Settings

                var clscountries = new cCountries(currentUser.AccountID, currentUser.CurrentSubAccountId);

                this.ddlDefaultCountry.Items.AddRange(clscountries.CreateDropDown().ToArray());
                if (this.ddlDefaultCountry.Items.FindByValue(generalOptions.Country.HomeCountry.ToString(CultureInfo.InvariantCulture)) != null)
                {
                    this.ddlDefaultCountry.Items.FindByValue(generalOptions.Country.HomeCountry.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }
                else
                {
                    if (clscountries.list.ContainsKey(generalOptions.Country.HomeCountry))
                    {
                        this.ddlDefaultCountry.Items.Add(clscountries.GetListItem(generalOptions.Country.HomeCountry));
                        this.ddlDefaultCountry.Items.FindByValue(generalOptions.Country.HomeCountry.ToString(CultureInfo.InvariantCulture)).Selected = true;
                    }
                }

                var clslanguages = new cLanguages(this.GeneralOptionsFactory);
                string[] languages = clslanguages.getLangaugeList();
                int i;
                for (i = 0; i < languages.Length; i++)
                {
                    this.ddlDefaultLanguage.Items.Add(languages[i]);
                }

                if (this.ddlDefaultLanguage.Items.FindByValue(generalOptions.Language) != null)
                {
                    this.ddlDefaultLanguage.Items.FindByValue(generalOptions.Language).Selected = true;
                }

                var clscurrencies = new cCurrencies(currentUser.AccountID, currentUser.CurrentSubAccountId);

                int baseCurrency;
                if (generalOptions.Currency.BaseCurrency.HasValue && generalOptions.Currency.BaseCurrency.Value != 0)
                {
                    baseCurrency = generalOptions.Currency.BaseCurrency.Value;
                }
                else
                {
                    baseCurrency = 0;
                }

                this.ddlBaseCurrency.Items.AddRange(clscurrencies.CreateDropDown(baseCurrency));
                var clsclaims = new cClaims(currentUser.AccountID);
                if (clsclaims.getCount() > 0)
                {
                    this.ddlBaseCurrency.Enabled = false;
                }

                var clslocales = new cLocales();
                this.ddlDefaultLocale.Items.Add(new ListItem("[None]", "0"));
                this.ddlDefaultLocale.Items.AddRange(clslocales.CreateActiveDropDown().ToArray());
                if (this.ddlDefaultLocale.Items.FindByValue(generalOptions.RegionalSettings.GlobalLocaleID.ToString(CultureInfo.InvariantCulture)) != null)
                {
                    this.ddlDefaultLocale.Items.FindByValue(generalOptions.RegionalSettings.GlobalLocaleID.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }

                #endregion Regional Settings

                #region Main Administrator

                this.ddlMainAdministrator.Items.Add(new ListItem("[None]", "0"));
                this.ddlMainAdministrator.Items.AddRange(clsEmployees.CreateDropDown(0, false));

                if (this.ddlMainAdministrator.Items.FindByValue(generalOptions.Admin.MainAdministrator.ToString(CultureInfo.InvariantCulture)) != null)
                {
                    this.ddlMainAdministrator.Items.FindByValue(generalOptions.Admin.MainAdministrator.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }

                #endregion Main Administrator

                #region Email Server Settings

                this.txtEmailServer.Text = generalOptions.Email.EmailServerAddress;
                if (generalOptions.Email.SourceAddress == 1)
                {
                    this.optserver.Checked = true;
                }
                else
                {
                    this.optclaimant.Checked = true;
                }

                this.txtAuditorEmail.Text = generalOptions.AuditLog.AuditorEmailAddress;
                this.txtEmailFromAddress.Text = generalOptions.Email.EmailServerFromAddress;
                this.txtErrorEmailSubmitAddress.Text = generalOptions.Email.ErrorEmailAddress;
                this.txtErrorSubmitFromAddress.Text = generalOptions.Email.ErrorEmailFromAddress;
                this.txtEmailAdministrator.Text = generalOptions.Email.EmailAdministrator;

                #endregion Email Server Settings

                #region Password Settings

                this.txtattempts.Text = generalOptions.Password.PwdMaxRetries.ToString(CultureInfo.InvariantCulture);
                this.txtexpires.Text = generalOptions.Password.PwdExpiryDays.ToString(CultureInfo.InvariantCulture);

                if (generalOptions.Password.PwdConstraint != 0)
                {
                    this.cmblength.Items.FindByValue(((int)generalOptions.Password.PwdConstraint).ToString(CultureInfo.InvariantCulture)).Selected = true;
                }

                switch (generalOptions.Password.PwdConstraint)
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

                this.txtlength1.Text = generalOptions.Password.PwdLength1.ToString(CultureInfo.InvariantCulture);
                this.txtlength2.Text = generalOptions.Password.PwdLength2.ToString(CultureInfo.InvariantCulture);
                this.chkupper.Checked = generalOptions.Password.PwdMustContainUpperCase;
                this.chknumbers.Checked = generalOptions.Password.PwdMustContainNumbers;
                this.chksymbol.Checked = generalOptions.Password.PwdMustContainSymbol;
                this.txtprevious.Text = generalOptions.Password.PwdHistoryNum.ToString(CultureInfo.InvariantCulture);
                this.txtaccountlocked.Text = generalOptions.AccountMessages.AccountLockedMessage;
                this.txtcurrentlylocked.Text = generalOptions.AccountMessages.AccountCurrentlyLockedMessage;


                this.cmblength.Attributes.Add("onchange", "SetupPasswordFields();");

                switch (generalOptions.Password.PwdConstraint)
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
        
                this.chkActivateCarOnUserAdd.Checked = generalOptions.Car.ActivateCarOnUserAdd;
                this.chkAllowUsersToAddCars.Checked = generalOptions.Car.AllowUsersToAddCars;
                this.chkAllowMilage.Checked = generalOptions.Car.ShowMileageCatsForUsers;

                this.hdnAllowMileage.Value = generalOptions.Car.ShowMileageCatsForUsers ? "true" : "false";

                if (this.chkActivateCarOnUserAdd.Checked)
                {
                    this.chkAllowMilage.InputAttributes.Add("disabled", "disabled");
                }

                if (!this.chkEmployeeSpecifyCarStartDate.Checked)            
                {
                    this.chkEmployeeSpecifyCarStartDateMandatory.InputAttributes.Add("disabled", "disabled");
                    this.chkEmployeeSpecifyCarStartDateMandatory.Checked = false;
                }

                this.chksingleclaim.Checked = generalOptions.Claim.SingleClaim;
                this.chkcostcodes.Checked = generalOptions.CodeAllocation.UseCostCodes;
                this.chkdepartment.Checked = generalOptions.CodeAllocation.UseDepartmentCodes;
                this.chkprojectcodes.Checked = generalOptions.CodeAllocation.UseProjectCodes;
                this.chkcostcodedesc.Checked = generalOptions.CodeAllocation.UseCostCodeDescription;
                this.chkdepartmentdesc.Checked = generalOptions.CodeAllocation.UseDepartmentDescription;
                this.chkprojectcodedesc.Checked = generalOptions.CodeAllocation.UseProjectCodeDesc;
                this.chkaddlocations.Checked = generalOptions.Mileage.AddLocations;
                this.chkattach.Checked = generalOptions.Claim.AttachReceipts;
                this.chkexchangereadonly.Checked = generalOptions.AddEditExpense.ExchangeReadOnly;
                this.chkcostcodeson.Checked = generalOptions.CodeAllocation.CostCodesOn;
                this.chkdepartmentson.Checked = generalOptions.CodeAllocation.DepartmentsOn;
                this.chkprojectcodeson.Checked = generalOptions.CodeAllocation.ProjectCodesOn;
                this.chkcostcodeongendet.Checked = generalOptions.CodeAllocation.UseCostCodeOnGenDetails;
                this.chkdepartmentongendet.Checked = generalOptions.CodeAllocation.UseDeptOnGenDetails;
                this.chkprojectcodeongendet.Checked = generalOptions.CodeAllocation.UseProjectCodeOnGenDetails;
                this.chkautoassignallocation.Checked = generalOptions.CodeAllocation.AutoAssignAllocation;
                this.chkblockdrivinglicence.Checked = generalOptions.DutyOfCare.BlockDrivingLicence;
                this.chkblocktaxexpiry.Checked = generalOptions.DutyOfCare.BlockTaxExpiry;
                this.chkblockmotexpiry.Checked = generalOptions.DutyOfCare.BlockMOTExpiry;
                this.chkDrivingLicenceLookup.Checked = generalOptions.DutyOfCare.EnableAutomaticDrivingLicenceLookup;
                if (generalOptions.DutyOfCare.EnableAutomaticDrivingLicenceLookup)
                {
                    this.ddlDrivingLicenceLookupFrequency.SelectedValue = generalOptions.DutyOfCare.DrivingLicenceLookupFrequency;
                    this.ddlAutoRevokeOfConsentLookupFrequency.SelectedValue = generalOptions.DutyOfCare.FrequencyOfConsentRemindersLookup;
                }

                this.chkVehicleDocumentLookup.Checked = generalOptions.Car.PopulateDocumentsFromVehicleLookup;

                //load the team list in the approver section
                this.teamListForApprover.Items.AddRange(new cTeams(currentUser.AccountID).CreateDropDown(0));
                this.cmbLineManagerReminderDays.SelectedValue = generalOptions.DutyOfCare.RemindApproverOnDOCDocumentExpiryDays.ToString(CultureInfo.InvariantCulture);
                this.chkLicenceReview.Checked = generalOptions.DutyOfCare.EnableDrivingLicenceReview;
                this.cmbReviewFrequencyDays.SelectedValue = generalOptions.DutyOfCare.DrivingLicenceReviewFrequency.ToString(CultureInfo.InvariantCulture); 
                this.chkLicenceReviewReminderNotification.Checked = generalOptions.DutyOfCare.DrivingLicenceReviewReminder;
                this.cmbReminderReviewsDays.SelectedValue = generalOptions.Car.DrivingLicenceReviewReminderDays.ToString(CultureInfo.InvariantCulture);

                this.cmbClaimantReminderDays.SelectedValue = generalOptions.DutyOfCare.RemindClaimantOnDOCDocumentExpiryDays.ToString(CultureInfo.InvariantCulture);
                if (generalOptions.DutyOfCare.DutyOfCareTeamAsApprover != null && !string.IsNullOrEmpty(generalOptions.DutyOfCare.DutyOfCareTeamAsApprover))
                    {
                        dutyOfCareApproverTeamSection.Style.Add("display", "block");
                        teamListForApprover.SelectedValue = generalOptions.DutyOfCare.DutyOfCareTeamAsApprover;
                        this.teamAsApprover.Checked = true;
                    }
                    else
                    {
                        this.lineManagerAsApprover.Checked = true;
                    }

                this.chkblockinsuranceexpiry.Checked = generalOptions.DutyOfCare.BlockInsuranceExpiry;
                this.chkBlockBreakdownCoverExpiry.Checked = generalOptions.DutyOfCare.BlockBreakdownCoverExpiry;
                this.chkDutyOfCareDateOfExpenseCheck.Checked = generalOptions.DutyOfCare.UseDateOfExpenseForDutyOfCareChecks;
                IOwnership dcco = clsSubAccounts.GetDefaultCostCodeOwner(currentUser.AccountID, currentUser.CurrentSubAccountId);
                this.txtdefaultcostcodeowner.Text = dcco == null ? string.Empty : dcco.ItemDefinition();
                this.txtdefaultcostcodeowner_ID.Text = dcco == null ? string.Empty : dcco.CombinedItemKey;

                // chkAddCompanies.Checked = reqProperties.AddCompanies;
                this.chkUseMapPoint.Checked = generalOptions.Mileage.UseMapPoint;
                this.chkAllowMultipleDestinations.Checked = generalOptions.Mileage.AllowMultipleDestinations;
                this.chkMandatoryPostcodeForAddresses.Checked = generalOptions.Mileage.MandatoryPostcodeForAddresses;
                if (this.cmbmileagecalculationtype.Items.FindByValue(generalOptions.Mileage.MileageCalcType.ToString(CultureInfo.InvariantCulture)) != null)
                {
                    this.cmbmileagecalculationtype.Items.FindByValue(generalOptions.Mileage.MileageCalcType.ToString(CultureInfo.InvariantCulture)).Selected = true;
                }

                this.chkClaimantsCanSaveCompanyAddresses.Checked = generalOptions.AddEditExpense.ClaimantsCanAddCompanyLocations;
                this.txtHomeAddressKeyword.Text = generalOptions.AddEditExpense.HomeAddressKeyword;
                this.txtWorkAddressKeyword.Text = generalOptions.AddEditExpense.WorkAddressKeyword;

                this.cmbRetainLabelsTime.SelectedValue = generalOptions.Addresses.RetainLabelsTime;
                this.chkShowFullHomeAddress.Checked = generalOptions.Claim.ShowFullHomeAddressOnClaims;

                this.chkForceAddressNameEntry.Checked = generalOptions.AddEditExpense.ForceAddressNameEntry;
                this.chkForceAddressNameEntry.Enabled = currentUser.Account.AddressLookupProvider == AddressLookupProvider.Teleatlas;
                this.chkEnableVatOptions.Checked = generalOptions.VatCalculation.EnableCalculationsForAllocatingFuelReceiptVatToMileage;
                this.txtAddressNameEntryMessage.Text = generalOptions.AddEditExpense.AddressNameEntryMessage;

                this.chkMultipleWorkAddress.Checked = generalOptions.AddEditExpense.MultipleWorkAddress;


                this.chkEnableAutoUpdateOfExchangeRates.Checked = generalOptions.Currency.EnableAutoUpdateOfExchangeRates;
                var future = false;
                if (generalOptions.Currency.CurrencyType == BusinessLogic.GeneralOptions.Currencies.CurrencyType.Range && !generalOptions.Currency.EnableAutoUpdateOfExchangeRates)
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

                var item = this.ddlExchangeRateProvider.Items.FindByValue(generalOptions.Currency.ExchangeRateProvider.ToString(CultureInfo.InvariantCulture));
                if (item != null)
                {
                    item.Selected = true;
                }

                this.chkDisableCarOutsideOfStartEndDate.Checked = generalOptions.AddEditExpense.DisableCarOutsideOfStartEndDate;
                this.BindOwnerAutoComplete(currentUser);

                #endregion new expenses

                #region ESR Options

                if (currentUser.Account.IsNHSCustomer)
                {
                    this.litESROptions.Text = "<a href=\"javascript:changePage('ESROptions');\" id=\"lnkESROptions\">NHS Options</a>";
                    this.ddlESRActivateType.SelectedValue = ((byte)generalOptions.ESR.AutoActivateType).ToString(CultureInfo.InvariantCulture);
                    this.ddlESRArchiveType.SelectedValue = ((byte)generalOptions.ESR.AutoArchiveType).ToString(CultureInfo.InvariantCulture);
                    this.txtESRGracePeriod.Text = generalOptions.ESR.ArchiveGracePeriod.ToString(CultureInfo.InvariantCulture);
                    this.txtESRUsernameFormat.Text = generalOptions.ESR.ImportUsernameFormat;
                    this.txtESRHomeAddressFormat.Text = generalOptions.ESR.ImportHomeAddressFormat;
                    this.chkESRAssignmentsOnEmployeeAdd.Checked = generalOptions.ESR.CheckESRAssignmentOnEmployeeAdd;
                    this.ddlEsrDetail.Items.Add(new ListItem("[None]", "0"));
                    this.ddlEsrDetail.Items.Add(new ListItem("Paypoint", "1"));
                    this.ddlEsrDetail.Items.Add(new ListItem("Job Name", "2"));
                    this.ddlEsrDetail.Items.Add(new ListItem("Position Name", "3"));
                    this.ddlEsrDetail.SelectedIndex = (int)generalOptions.AddEditExpense.IncludeAssignmentDetails;
                    this.chkESRActivateCar.Checked = generalOptions.ESR.EsrAutoActivateCar;
                    this.chkDisplayEsrAddressesInSearchResults.Checked = generalOptions.AddEditExpense.DisplayEsrAddressesInSearchResults;
                    this.chkSummaryEsrInboundFile.Checked = generalOptions.ESR.SummaryEsrInboundFile;
                    this.ddlEsrRounding.Items.Clear();
                    this.ddlEsrRounding.Items.Add(new ListItem("Always Down", ((int)EsrRoundingType.Down).ToString()));
                    this.ddlEsrRounding.Items.Add(new ListItem("Maths Rounding", ((int)EsrRoundingType.Up).ToString()));
                    this.ddlEsrRounding.Items.Add(new ListItem("Always Up", ((int)EsrRoundingType.ForceUp).ToString()));
                    this.ddlEsrRounding.SelectedValue = ((byte)generalOptions.ESR.EsrRounding).ToString(CultureInfo.InvariantCulture);
                    this.chkESRManualAssignmentSupervisor.Checked = generalOptions.ESR.EnableESRManualAssignmentSupervisor;
                    this.chkEsrPrimaryAddressOnly.Checked = generalOptions.ESR.EsrPrimaryAddressOnly;
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
                this.chkUploadAttachmentEnabled.Checked = generalOptions.Attachment.EnableAttachmentUpload;
                this.chkHyperlinkAttachmentsEnabled.Checked = generalOptions.Attachment.EnableAttachmentHyperlink;
                if (this.chkHyperlinkAttachmentsEnabled.Checked == false)
                {
                    this.optLinkAttachmentDefault.SelectedIndex = 1;
                    this.optLinkAttachmentDefault.Enabled = false;
                }
                else
                {
                    this.optLinkAttachmentDefault.SelectedValue = generalOptions.Attachment.LinkAttachmentDefault.ToString();
                }

                this.chkHyperlinkAttachmentsEnabled.InputAttributes.Add(
                    "onchange",
                    "if(this.checked == true) { document.getElementById(optLinkAttachmentDefault).disabled = false; } else { document.getElementById(optLinkAttachmentDefault).options[1].selected = true; document.getElementById(optLinkAttachmentDefault).disabled = true; }");

                this.txtMaxUploadSize.Text = generalOptions.Attachment.MaxUploadSize.ToString(CultureInfo.InvariantCulture);
                this.chkNotesIconFlash.Checked = generalOptions.Contract.EnableFlashingNotesIcon;
                this.chkShowProductInSearch.Checked = generalOptions.Search.ShowProductInSearch;
                this.optFYStarts.SelectedValue = generalOptions.FinancialYear.FYStarts;
                this.optFYEnds.SelectedValue = generalOptions.FinancialYear.FYEnds;
                this.txtDefaultPageSize.Text = generalOptions.Page.DefaultPageSize.ToString(CultureInfo.InvariantCulture);
                this.chkfwEditMyDetails.Checked = generalOptions.MyDetails.EditMyDetails;

                // Framework - Products
                this.chkAutoUpdateLicenceTotal.Checked = generalOptions.Contract.AutoUpdateLicenceTotal;

                // Framework - Tasks
                this.chkTaskDueDateMandatory.Checked = generalOptions.Task.TaskDueDateMandatory;
                this.chkTaskEndDateMandatory.Checked = generalOptions.Task.TaskEndDateMandatory;
                this.txtTaskEscalationRepeat.Text = generalOptions.Task.TaskEscalationRepeat.ToString(CultureInfo.InvariantCulture);
                this.chkTaskStartDateMandatory.Checked = generalOptions.Task.TaskStartDateMandatory;

                // Framework - Help And Support
                this.chkInternalTicketsFW.Checked = generalOptions.HelpAndSupport.EnableInternalSupportTickets;

                // Framework - Contract
                this.txtContractKey.Text = generalOptions.Contract.ContractKey;
                this.txtScheduleDefault.Text = generalOptions.Contract.ContractScheduleDefault;
                this.txtContractDescriptionTitle.Text = generalOptions.Contract.ContractDescTitle;
                this.txtContractDescriptionTitleAbbrev.Text = generalOptions.Contract.ContractDescShortTitle;
                this.txtContractCategoryTitle.Text = generalOptions.Contract.ContractCategoryTitle;
                this.chkContractCategoryMandatory.Checked = generalOptions.Contract.ContractCatMandatory;
                this.txtPenaltyClauseTitle.Text = generalOptions.Contract.PenaltyClauseTitle;
                this.chkTermTypeActive.Checked = generalOptions.Contract.TermTypeActive;
                this.chkContractDatesMandatory.Checked = generalOptions.Contract.ContractDatesMandatory;
                this.chkInflatorActive.Checked = generalOptions.Contract.InflatorActive;
                this.chkContractNumberGenerate.Checked = generalOptions.Contract.ContractNumGen;
                this.chkContractNumberUpdatable.Checked = generalOptions.Contract.EnableContractNumUpdate;

                this.txtContractNumberCurSeq.Text = generalOptions.Contract.ContractNumSeq.ToString(CultureInfo.InvariantCulture);
                this.chkAutoUpdateCV.Checked = generalOptions.Contract.AutoUpdateAnnualContractValue;
                this.chkArchivedNotesAdd.Checked = generalOptions.Notes.AllowArchivedNotesAdd;
                this.chkInvoiceFrequencyActive.Checked = generalOptions.Contract.InvoiceFreqActive;
                this.chkVariationAutoSeq.Checked = generalOptions.Schedule.EnableVariationAutoSeq;
                this.txtValueComments.Text = generalOptions.Contract.ValueComments;

                // Framework - Invoice
                this.chkKeepInvForecasts.Checked = generalOptions.Invoices.KeepInvoiceForecasts;
                this.chkPONumberGenerate.Checked = generalOptions.Invoices.PONumberGenerate;
                this.txtPONumberFormat.Text = generalOptions.Invoices.PONumberFormat;
                this.txtPOSequenceNumber.Text = generalOptions.Invoices.PONumberSequence.ToString(CultureInfo.InvariantCulture);

                // Framework - Suppliers
                this.txtSupplierPrimaryTitle.Text = generalOptions.Supplier.SupplierPrimaryTitle;
                this.txtSupplierRegionTitle.Text = generalOptions.Supplier.SupplierRegionTitle;
                this.txtSupplierCategoryTitle.Text = generalOptions.Supplier.SupplierCatTitle;
                this.chkSupplierCategoryMandatory.Checked = generalOptions.Supplier.SupplierCatMandatory;
                this.txtSupplierVariationTitle.Text = generalOptions.Supplier.SupplierVariationTitle;
                this.chkSupplierStatusMandatory.Checked = generalOptions.Supplier.SupplierStatusEnforced;
                this.chkSupplierTurnoverEnabled.Checked = generalOptions.Supplier.SupplierTurnoverEnabled;
                this.chkSupplierNumEmployeesEnabled.Checked = generalOptions.Supplier.SupplierNumEmployeesEnabled;
                this.chkLastFinCheckEnabled.Checked = generalOptions.Supplier.SupplierLastFinCheckEnabled;
                this.chkLastFinStatusEnabled.Checked = generalOptions.Supplier.SupplierLastFinStatusEnabled;
                this.chkIntContactEnabled.Checked = generalOptions.Supplier.SupplierIntContactEnabled;
                this.chkFYEEnabled.Checked = generalOptions.Supplier.SupplierFYEEnabled;

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
                case Modules.Contracts:
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
            
            var accountBase = new cAccountSubAccountsBase(currentUser.AccountID);

            var generalOptions = this.GeneralOptionsFactory[currentUser.CurrentSubAccountId].WithAll();

            if (generalOptions.Car.ActivateCarOnUserAdd != this.chkActivateCarOnUserAdd.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ActivateCarOnUserAdd.GetDescription(), this.chkActivateCarOnUserAdd.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Mileage.AddLocations != this.chkaddlocations.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AddLocations.GetDescription(), this.chkaddlocations.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Mileage.AllowMultipleDestinations != this.chkAllowMultipleDestinations.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowMultipleDestinations.GetDescription(), this.chkAllowMultipleDestinations.Checked.ToString(), currentUser.CurrentSubAccountId));
            }
            ;
            if (generalOptions.SelfRegistration.AllowSelfReg != this.chkallowselfreg.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowSelfReg.GetDescription(), this.chkallowselfreg.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.SelfRegistration.AllowSelfRegAdvancesSignOff != this.chkselfregadvancessignoff.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowSelfRegAdvancesSignOff.GetDescription(), this.chkselfregadvancessignoff.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.SelfRegistration.AllowSelfRegBankDetails != this.chkselfregbankdetails.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowSelfRegBankDetails.GetDescription(), this.chkselfregbankdetails.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.SelfRegistration.AllowSelfRegCarDetails != this.chkselfregcardetails.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowSelfRegCarDetails.GetDescription(), this.chkselfregcardetails.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.SelfRegistration.AllowSelfRegDepartmentCostCode != this.chkselfregdepcostcode.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowSelfRegDepartmentCostCode.GetDescription(), this.chkselfregdepcostcode.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.SelfRegistration.AllowSelfRegEmployeeContact != this.chkselfregempconact.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowSelfRegEmployeeContact.GetDescription(), this.chkselfregempconact.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.SelfRegistration.AllowSelfRegEmployeeInfo != this.chkselfregempinfo.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowSelfRegEmployeeInfo.GetDescription(), this.chkselfregempinfo.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.SelfRegistration.AllowSelfRegHomeAddress != this.chkselfreghomaddr.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowSelfRegHomeAddress.GetDescription(), this.chkselfreghomaddr.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.SelfRegistration.AllowSelfRegRole != this.chkselfregrole.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowSelfRegRole.GetDescription(), this.chkselfregrole.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.SelfRegistration.AllowSelfRegSignOff != this.chkselfregsignoff.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowSelfRegSignOff.GetDescription(), this.chkselfregsignoff.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.SelfRegistration.AllowSelfRegUDF != this.chkselfregudf.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowSelfRegUDF.GetDescription(), this.chkselfregudf.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Claim.AllowTeamMemberToApproveOwnClaim != this.chkTeamMemberApproveOwnClaims.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowTeamMemberToApproveOwnClaim.GetDescription(), this.chkTeamMemberApproveOwnClaims.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Claim.AllowEmployeeInOwnSignoffGroup != this.chkEmployeeApproveOwnClaims.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowEmployeeInOwnSignoffGroup.GetDescription(), this.chkEmployeeApproveOwnClaims.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Car.AllowUsersToAddCars != this.chkAllowUsersToAddCars.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowUsersToAddCars.GetDescription(), this.chkAllowUsersToAddCars.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Car.AllowEmpToSpecifyCarStartDateOnAdd != this.chkEmployeeSpecifyCarStartDate.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowEmpToSpecifyCarStartDateOnAdd.GetDescription(), this.chkEmployeeSpecifyCarStartDate.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Car.AllowEmpToSpecifyCarDOCOnAdd != this.chkEmployeeSpecifyCarDOC.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowEmpToSpecifyCarDOCOnAdd.GetDescription(), this.chkEmployeeSpecifyCarDOC.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Car.EmpToSpecifyCarStartDateOnAddMandatory != this.chkEmployeeSpecifyCarStartDateMandatory.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EmpToSpecifyCarStartDateOnAddMandatory.GetDescription(), this.chkEmployeeSpecifyCarStartDateMandatory.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.MyDetails.AllowEmployeeToNotifyOfChangeOfDetails != this.chkemployeedetailschanged.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowEmployeeToNotifyOfChangeOfDetails.GetDescription(), this.chkemployeedetailschanged.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            var numberOfApproversToRememberForClaimantInApprovalMatrixClaim = Convert.ToInt32(this.cboNumberOfApproversToShowInClaimHistoryForApprovalMatrix.SelectedValue);
            if (generalOptions.ApprovalMatrix.NumberOfApproversToRememberForClaimantInApprovalMatrixClaim != numberOfApproversToRememberForClaimantInApprovalMatrixClaim)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.NumberOfApproversToRememberForClaimantInApprovalMatrixClaim.GetDescription(), numberOfApproversToRememberForClaimantInApprovalMatrixClaim.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Claim.ApproverDeclarationMsg != this.txtapproverdeclarationmsg.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ApproverDeclarationMsg.GetDescription(), this.txtapproverdeclarationmsg.Text, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Claim.AttachReceipts != this.chkattach.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AttachReceipts.GetDescription(), this.chkattach.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.CodeAllocation.AutoAssignAllocation != this.chkautoassignallocation.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AutoAssignAllocation.GetDescription(), this.chkautoassignallocation.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            var costCodeOwnerBaseKey = this.txtdefaultcostcodeowner_ID.Text.Contains(",") ? this.txtdefaultcostcodeowner_ID.Text : string.Empty;

            if (generalOptions.CodeAllocation.CostCodeOwnerBaseKey != costCodeOwnerBaseKey)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CostCodeOwnerBaseKey.GetDescription(), costCodeOwnerBaseKey, currentUser.CurrentSubAccountId));
            }

            var baseCurrency = !string.IsNullOrEmpty(this.ddlBaseCurrency.SelectedValue) ? Convert.ToInt32(this.ddlBaseCurrency.SelectedValue) : 0;

            if (generalOptions.Currency.BaseCurrency != baseCurrency)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.BaseCurrency.GetDescription(), baseCurrency.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.DutyOfCare.BlockInsuranceExpiry != this.chkblockinsuranceexpiry.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.BlockInsuranceExpiry.GetDescription(), this.chkblockinsuranceexpiry.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.DutyOfCare.BlockDrivingLicence != this.chkblockdrivinglicence.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.BlockDrivingLicence.GetDescription(), this.chkblockdrivinglicence.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.DutyOfCare.BlockMOTExpiry != this.chkblockmotexpiry.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.BlockMOTExpiry.GetDescription(), this.chkblockmotexpiry.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.DutyOfCare.BlockTaxExpiry != this.chkblocktaxexpiry.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.BlockTaxExpiry.GetDescription(), this.chkblocktaxexpiry.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.DutyOfCare.BlockBreakdownCoverExpiry != this.chkBlockBreakdownCoverExpiry.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.BlockBreakdownCoverExpiry.GetDescription(), this.chkBlockBreakdownCoverExpiry.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.DutyOfCare.EnableAutomaticDrivingLicenceLookup != this.chkDrivingLicenceLookup.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableAutomaticDrivingLicenceLookup.GetDescription(), this.chkDrivingLicenceLookup.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.DutyOfCare.DrivingLicenceLookupFrequency != this.ddlDrivingLicenceLookupFrequency.SelectedValue)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DrivingLicenceLookupFrequency.GetDescription(), this.ddlDrivingLicenceLookupFrequency.SelectedValue, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.DutyOfCare.FrequencyOfConsentRemindersLookup != this.ddlAutoRevokeOfConsentLookupFrequency.SelectedValue)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.FrequencyOfConsentRemindersLookup.GetDescription(), this.ddlAutoRevokeOfConsentLookupFrequency.SelectedValue, currentUser.CurrentSubAccountId));
            }

            var remindClaimantOnDOCDocumentExpiryDays = Convert.ToInt32(this.cmbClaimantReminderDays.SelectedValue);
            if (generalOptions.DutyOfCare.RemindClaimantOnDOCDocumentExpiryDays != remindClaimantOnDOCDocumentExpiryDays)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.RemindClaimantOnDOCDocumentExpiryDays.GetDescription(), remindClaimantOnDOCDocumentExpiryDays.ToString(), currentUser.CurrentSubAccountId));
            }

            var remindApproverOnDOCDocumentExpiryDays = Convert.ToInt32(this.cmbLineManagerReminderDays.SelectedValue);
            if (generalOptions.DutyOfCare.RemindApproverOnDOCDocumentExpiryDays != remindApproverOnDOCDocumentExpiryDays)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.RemindApproverOnDOCDocumentExpiryDays.GetDescription(), remindApproverOnDOCDocumentExpiryDays.ToString(), currentUser.CurrentSubAccountId));
            }

            var drivingLicenceReviewFrequency = Convert.ToInt32(this.cmbReviewFrequencyDays.SelectedValue);
            if (generalOptions.DutyOfCare.DrivingLicenceReviewFrequency != drivingLicenceReviewFrequency)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DrivingLicenceReviewFrequency.GetDescription(), drivingLicenceReviewFrequency.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.DutyOfCare.EnableDrivingLicenceReview != this.chkLicenceReview.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableDrivingLicenceReview.GetDescription(), this.chkLicenceReview.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.DutyOfCare.DrivingLicenceReviewReminder != this.chkLicenceReviewReminderNotification.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DrivingLicenceReviewReminder.GetDescription(), this.chkLicenceReviewReminderNotification.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            var drivingLicenceReviewReminderDays = Convert.ToByte(this.cmbReminderReviewsDays.SelectedValue);
            if (generalOptions.Car.DrivingLicenceReviewReminderDays != drivingLicenceReviewReminderDays)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DrivingLicenceReviewReminderDays.GetDescription(), drivingLicenceReviewReminderDays.ToString(), currentUser.CurrentSubAccountId));
            }

            var dutyOfCareAproverOnForm = this.lineManagerAsApprover.Checked
                ? this.lblLineManagerAsApprover.Text
                : this.lblteamAsApprover.Text;

            if (generalOptions.DutyOfCare.DutyOfCareApprover != dutyOfCareAproverOnForm)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DutyOfCareApprover.GetDescription(), dutyOfCareAproverOnForm, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.AddEditExpense.DisableCarOutsideOfStartEndDate != this.chkDisableCarOutsideOfStartEndDate.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DisableCarOutsideOfStartEndDate.GetDescription(), this.chkDisableCarOutsideOfStartEndDate.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            var dutyOfCareTeamAsApprover = this.lineManagerAsApprover.Checked ? string.Empty : this.teamListForApprover.SelectedValue;
            if (generalOptions.DutyOfCare.DutyOfCareTeamAsApprover != dutyOfCareTeamAsApprover)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DutyOfCareTeamAsApprover.GetDescription(), dutyOfCareTeamAsApprover, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.DutyOfCare.UseDateOfExpenseForDutyOfCareChecks != this.chkDutyOfCareDateOfExpenseCheck.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.UseDateOfExpenseForDutyOfCareChecks.GetDescription(), this.chkDutyOfCareDateOfExpenseCheck.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Claim.ClaimantDeclaration != this.chkclaimantdeclaration.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ClaimantDeclaration.GetDescription(), this.chkclaimantdeclaration.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.CodeAllocation.CostCodesOn != this.chkcostcodeson.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CostCodesOn.GetDescription(), this.chkcostcodeson.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Claim.DeclarationMsg != this.txtdeclarationmsg.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DeclarationMsg.GetDescription(), this.txtdeclarationmsg.Text, currentUser.CurrentSubAccountId));
            }

            var mileageCalcType = Convert.ToByte(this.cmbmileagecalculationtype.SelectedValue);
            if (generalOptions.Mileage.MileageCalcType != mileageCalcType)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.MileageCalcType.GetDescription(), mileageCalcType.ToString(), currentUser.CurrentSubAccountId));
            }

            var defaultRole = Convert.ToInt32(this.cmbdefaultrole.SelectedValue);
            if (generalOptions.SelfRegistration.DefaultRole != defaultRole)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DefaultRole.GetDescription(), defaultRole.ToString(), currentUser.CurrentSubAccountId));
            }

            var defaultItemRole = Convert.ToInt32(this.cmbdefaultitemrole.SelectedValue);
            if (generalOptions.SelfRegistration.DefaultItemRole != defaultItemRole)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DefaultItemRole.GetDescription(), defaultItemRole.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Delegate.DelApprovals != this.chkdelapprovals.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DelApprovals.GetDescription(), this.chkdelapprovals.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Delegate.DelAuditLog != this.chkdelauditlog.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DelAuditLog.GetDescription(), this.chkdelauditlog.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Delegate.DelCheckAndPay != this.chkdelcheckandpay.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DelCheckAndPay.GetDescription(), this.chkdelcheckandpay.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Delegate.DelCorporateCards != this.chkdelcorporatecard.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DelCorporateCards.GetDescription(), this.chkdelcorporatecard.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Delegate.DelEmployeeAdmin != this.chkdelemployeeadmin.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DelEmployeeAdmin.GetDescription(), this.chkdelemployeeadmin.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Delegate.DelExports != this.chkdelexports.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DelExports.GetDescription(), this.chkdelexports.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Delegate.DelQEDesign != this.chkdelqedesign.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DelQEDesign.GetDescription(), this.chkdelqedesign.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Delegate.DelReports != this.chkdelreports.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DelReports.GetDescription(), this.chkdelreports.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Delegate.DelReportsClaimants != this.chkdelclaimantreports.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DelReportsClaimants.GetDescription(), this.chkdelclaimantreports.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Delegate.DelSetup != this.chkdelsetup.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DelSetup.GetDescription(), this.chkdelsetup.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Delegate.DelSubmitClaim != this.chkDelsSubmitClaims.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DelSubmitClaim.GetDescription(), this.chkDelsSubmitClaims.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.CodeAllocation.DepartmentsOn != this.chkdepartmentson.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DepartmentsOn.GetDescription(), this.chkdepartmentson.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            var drilldownReport = new Guid(this.cmbdrilldown.SelectedValue);
            if (generalOptions.Report.DrilldownReport != drilldownReport)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DrilldownReport.GetDescription(), drilldownReport.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.MyDetails.EditMyDetails != this.chkeditmydetails.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EditMyDetails.GetDescription(), this.chkeditmydetails.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Claim.EditPreviousClaims != this.chkEditPreviousClaim.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EditPreviousClaims.GetDescription(), this.chkEditPreviousClaim.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Delegate.EnableDelegateOptionsForDelegateAccessRole != this.chkDelOptionForDelAccessRole.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableDelegateOptionsForDelegateAccessRole.GetDescription(), this.chkDelOptionForDelAccessRole.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            var idleTimeout = Convert.ToInt32(cmbIdleTimeout.SelectedValue);
            if (generalOptions.SessionTimeout.IdleTimeout != idleTimeout)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.IdleTimeout.GetDescription(), idleTimeout.ToString(), currentUser.CurrentSubAccountId));
            }

            var countdownTimer = Convert.ToInt32(cmbCountdown.SelectedValue);
            if (generalOptions.SessionTimeout.CountdownTimer != countdownTimer)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CountdownTimer.GetDescription(), countdownTimer.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.HelpAndSupport.EnableInternalSupportTickets != this.chkInternalTickets.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableInternalSupportTickets.GetDescription(), this.chkInternalTickets.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            // Email Settings
            if (generalOptions.Email.EmailServerAddress != this.txtEmailServer.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EmailServerAddress.GetDescription(), this.txtEmailServer.Text, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Email.ErrorEmailAddress != this.txtErrorEmailSubmitAddress.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ErrorEmailAddress.GetDescription(), this.txtErrorEmailSubmitAddress.Text, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Email.ErrorEmailFromAddress != this.txtErrorSubmitFromAddress.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ErrorEmailFromAddress.GetDescription(), this.txtErrorSubmitFromAddress.Text, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.AuditLog.AuditorEmailAddress != this.txtAuditorEmail.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AuditorEmailAddress.GetDescription(), this.txtAuditorEmail.Text, currentUser.CurrentSubAccountId));
            }

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.SpendManagement:
                case Modules.SmartDiligence:
                case Modules.Contracts:
                case Modules.Greenlight:
                case Modules.GreenlightWorkforce:
                    if (generalOptions.Email.EmailServerFromAddress != this.txtEmailFromAddress.Text)
                    {
                        this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EmailServerFromAddress.GetDescription(), this.txtEmailFromAddress.Text, currentUser.CurrentSubAccountId));
                    }
                    break;
            }

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.Expenses:
                case Modules.Greenlight:
                case Modules.GreenlightWorkforce:
                    var sourceAddress = (byte)(this.optserver.Checked ? 1 : 0);
                    if (generalOptions.Email.SourceAddress != sourceAddress)
                    {
                        this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.SourceAddress.GetDescription(), sourceAddress.ToString(), currentUser.CurrentSubAccountId));
                    }
                    break;
            }

            if (generalOptions.Email.EmailAdministrator != this.txtEmailAdministrator.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EmailAdministrator.GetDescription(), this.txtEmailAdministrator.Text, currentUser.CurrentSubAccountId));
            }

            var enterOdometerOnSubmit = !this.optodologin.Checked;
            if (generalOptions.Mileage.EnterOdometerOnSubmit != enterOdometerOnSubmit)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnterOdometerOnSubmit.GetDescription(), enterOdometerOnSubmit.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.AddEditExpense.ExchangeReadOnly != this.chkexchangereadonly.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ExchangeReadOnly.GetDescription(), this.chkexchangereadonly.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            var globalLocaleID = Convert.ToInt32(this.ddlDefaultLocale.SelectedValue);
            if (generalOptions.RegionalSettings.GlobalLocaleID != globalLocaleID)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.GlobalLocaleID.GetDescription(), globalLocaleID.ToString(), currentUser.CurrentSubAccountId));
            }

            var homeCountry = !string.IsNullOrEmpty(this.ddlDefaultCountry.SelectedValue) ? Convert.ToInt32(this.ddlDefaultCountry.SelectedValue) : 0;
            if (generalOptions.Country.HomeCountry != homeCountry)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.HomeCountry.GetDescription(), homeCountry.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Language != this.ddlDefaultLanguage.SelectedValue)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.Language.GetDescription(), this.ddlDefaultLanguage.SelectedValue, currentUser.CurrentSubAccountId));
            }

            var mainAdministrator = Convert.ToInt32(this.ddlMainAdministrator.SelectedValue);
            if (generalOptions.Admin.MainAdministrator != mainAdministrator)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.MainAdministrator.GetDescription(), mainAdministrator.ToString(), currentUser.CurrentSubAccountId));
            }

            if (this.chkrecordodometer.Checked)
            {
                var odometerDay = (byte)(string.IsNullOrEmpty(this.txtodometerday.Text) ? 0 : Convert.ToByte(this.txtodometerday.Text));
                if (generalOptions.Mileage.OdometerDay != odometerDay)
                {
                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.OdometerDay.GetDescription(), odometerDay.ToString(), currentUser.CurrentSubAccountId));
                }
            }

            if (generalOptions.Claim.PartSubmit != this.chkpartsubmittal.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PartSubmit.GetDescription(), this.chkpartsubmittal.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Claim.PreApproval != this.chkpreapproval.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PreApproval.GetDescription(), this.chkpreapproval.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.CodeAllocation.ProjectCodesOn != this.chkprojectcodeson.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ProjectCodesOn.GetDescription(), this.chkprojectcodeson.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            var pwdConstraint = (PasswordLength)Convert.ToInt32(this.cmblength.SelectedValue);
            if ((int)generalOptions.Password.PwdConstraint != (int)pwdConstraint)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PwdConstraint.GetDescription(), this.cmblength.SelectedValue, currentUser.CurrentSubAccountId));
            }

            var passwordExpires = false;
            var PasswordExpiryDays = 0;

            if (Convert.ToInt32(this.txtexpires.Text) > 0)
            {
                passwordExpires = true;
                PasswordExpiryDays = Convert.ToInt32(this.txtexpires.Text);
            }
            if (generalOptions.Password.PwdExpires != passwordExpires)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PwdExpires.GetDescription(), passwordExpires.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Password.PwdExpiryDays != PasswordExpiryDays)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PwdExpiryDays.GetDescription(), PasswordExpiryDays.ToString(), currentUser.CurrentSubAccountId));
            }

            var pwdHistoryNum = Convert.ToInt32(this.txtprevious.Text);
            if (generalOptions.Password.PwdHistoryNum != pwdHistoryNum)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PwdHistoryNum.GetDescription(), pwdHistoryNum.ToString(), currentUser.CurrentSubAccountId));
            }

            var passwordLength1 = 0;
            var paddwordLength2 = 0;

            switch (pwdConstraint)
            {
                case PasswordLength.AnyLength:
                    break;
                case PasswordLength.EqualTo:
                case PasswordLength.GreaterThan:
                case PasswordLength.LessThan:
                    passwordLength1 = Convert.ToInt32(this.txtlength1.Text);
                    break;
                case PasswordLength.Between:
                    passwordLength1 = Convert.ToInt32(this.txtlength1.Text);
                    paddwordLength2 = Convert.ToInt32(this.txtlength2.Text);
                    break;
            }

            if (generalOptions.Password.PwdLength1 != passwordLength1)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PwdLength1.GetDescription(), passwordLength1.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Password.PwdLength2 != paddwordLength2)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PwdLength2.GetDescription(), paddwordLength2.ToString(), currentUser.CurrentSubAccountId));
            }

            var pwdMaxRetries = Convert.ToByte(this.txtattempts.Text);
            if (generalOptions.Password.PwdMaxRetries != pwdMaxRetries)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PwdMaxRetries.GetDescription(), pwdMaxRetries.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Password.PwdMustContainNumbers != this.chknumbers.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PwdMustContainNumbers.GetDescription(), this.chknumbers.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Password.PwdMustContainUpperCase != this.chkupper.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PwdMustContainUpperCase.GetDescription(), this.chkupper.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Password.PwdMustContainSymbol != this.chksymbol.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PwdMustContainSymbol.GetDescription(), this.chksymbol.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Mileage.RecordOdometer != this.chkrecordodometer.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.RecordOdometer.GetDescription(), this.chkrecordodometer.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Employee.SearchEmployees != this.chksearchemployees.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.SearchEmployees.GetDescription(), this.chksearchemployees.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Hotel.SendReviewRequests != this.chksendreviewrequest.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.SendReviewRequests.GetDescription(), this.chksendreviewrequest.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.AccountMessages.AccountLockedMessage != this.txtaccountlocked.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AccountLockedMessage.GetDescription(), this.txtaccountlocked.Text, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.AccountMessages.AccountCurrentlyLockedMessage != this.txtcurrentlylocked.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AccountCurrentlyLockedMessage.GetDescription(), this.txtcurrentlylocked.Text, currentUser.CurrentSubAccountId));
            }

            // Use hidden field as a disabled checkbox from javascript does not get its latest value.
            var showMileageCatsForUsers = bool.TryParse(this.hdnAllowMileage.Value, out var showMileage) ? showMileage : this.chkAllowMilage.Checked;
            if (generalOptions.Car.ShowMileageCatsForUsers != showMileageCatsForUsers)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ShowMileageCatsForUsers.GetDescription(), showMileageCatsForUsers.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Hotel.ShowReviews != this.chkshowreviews.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ShowReviews.GetDescription(), this.chkshowreviews.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Claim.SingleClaim != this.chksingleclaim.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.SingleClaim.GetDescription(), this.chksingleclaim.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.CodeAllocation.UseCostCodeDescription != this.chkcostcodedesc.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.UseCostCodeDescription.GetDescription(), this.chkcostcodedesc.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.CodeAllocation.UseCostCodeOnGenDetails != this.chkcostcodeongendet.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.UseCostCodeOnGenDetails.GetDescription(), this.chkcostcodeongendet.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.CodeAllocation.UseCostCodes != this.chkcostcodes.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.UseCostCodes.GetDescription(), this.chkcostcodes.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.CodeAllocation.UseDepartmentDescription != this.chkdepartmentdesc.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.UseDepartmentDescription.GetDescription(), this.chkdepartmentdesc.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.CodeAllocation.UseDepartmentCodes != this.chkdepartment.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.UseDepartmentCodes.GetDescription(), this.chkdepartment.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.CodeAllocation.UseDeptOnGenDetails != this.chkdepartmentongendet.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.UseDeptOnGenDetails.GetDescription(), this.chkdepartmentongendet.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Mileage.UseMapPoint != this.chkUseMapPoint.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.UseMapPoint.GetDescription(), this.chkUseMapPoint.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.CodeAllocation.UseProjectCodeDesc != this.chkprojectcodedesc.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.UseProjectCodeDesc.GetDescription(), this.chkprojectcodedesc.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.CodeAllocation.UseProjectCodeOnGenDetails != this.chkprojectcodeongendet.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.UseProjectCodeOnGenDetails.GetDescription(), this.chkprojectcodeongendet.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.CodeAllocation.UseProjectCodes != this.chkprojectcodes.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.UseProjectCodes.GetDescription(), this.chkprojectcodes.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.SelfRegistration.AllowSelfRegItemRole != this.chkselfregitemrole.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowSelfRegItemRole.GetDescription(), this.chkselfregitemrole.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Claim.OnlyCashCredit != this.chkonlycashcredit.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.OnlyCashCredit.GetDescription(), this.chkonlycashcredit.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Mileage.MandatoryPostcodeForAddresses != this.chkMandatoryPostcodeForAddresses.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.MandatoryPostcodeForAddresses.GetDescription(), this.chkMandatoryPostcodeForAddresses.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.AddEditExpense.ClaimantsCanAddCompanyLocations != this.chkClaimantsCanSaveCompanyAddresses.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ClaimantsCanAddCompanyLocations.GetDescription(), this.chkClaimantsCanSaveCompanyAddresses.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Addresses.RetainLabelsTime != this.cmbRetainLabelsTime.SelectedValue)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.RetainLabelsTime.GetDescription(), this.cmbRetainLabelsTime.SelectedValue, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Claim.ShowFullHomeAddressOnClaims != this.chkShowFullHomeAddress.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ShowFullHomeAddressOnClaims.GetDescription(), this.chkShowFullHomeAddress.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.AddEditExpense.DisableCarOutsideOfStartEndDate != this.chkDisableCarOutsideOfStartEndDate.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DisableCarOutsideOfStartEndDate.GetDescription(), this.chkDisableCarOutsideOfStartEndDate.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.VatCalculation.EnableCalculationsForAllocatingFuelReceiptVatToMileage != this.chkEnableVatOptions.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableCalculationsForAllocatingFuelReceiptVatToMileage.GetDescription(), this.chkEnableVatOptions.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            // ESR Options
            if (currentUser.Account.IsNHSCustomer)
            {
                if (generalOptions.ESR.CheckESRAssignmentOnEmployeeAdd != this.chkESRAssignmentsOnEmployeeAdd.Checked)
                {
                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CheckESRAssignmentOnEmployeeAdd.GetDescription(), this.chkESRAssignmentsOnEmployeeAdd.Checked.ToString(), currentUser.CurrentSubAccountId));
                }

                var autoActivateType = (SpendManagementLibrary.AutoActivateType)Enum.Parse(typeof(SpendManagementLibrary.AutoActivateType), this.ddlESRActivateType.SelectedValue);
                if (generalOptions.ESR.AutoActivateType != (BusinessLogic.GeneralOptions.ESR.AutoActivateType) autoActivateType)
                {
                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AutoActivateType.GetDescription(), this.ddlESRActivateType.SelectedValue, currentUser.CurrentSubAccountId));
                }

                var autoArchiveType = (SpendManagementLibrary.AutoArchiveType)Enum.Parse(typeof(SpendManagementLibrary.AutoArchiveType), this.ddlESRArchiveType.SelectedValue);
                if (generalOptions.ESR.AutoArchiveType != (BusinessLogic.GeneralOptions.ESR.AutoArchiveType) autoArchiveType)
                {
                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AutoArchiveType.GetDescription(), this.ddlESRArchiveType.SelectedValue, currentUser.CurrentSubAccountId));
                }

                var archiveGracePeriod = (short)(this.txtESRGracePeriod.Text != string.Empty ? Convert.ToInt16(this.txtESRGracePeriod.Text) : 0);
                if (generalOptions.ESR.ArchiveGracePeriod != archiveGracePeriod)
                {
                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ArchiveGracePeriod.GetDescription(), archiveGracePeriod.ToString(), currentUser.CurrentSubAccountId));
                }

                var includeAssignmentDetails = (IncludeEsrDetails)int.Parse(this.ddlEsrDetail.SelectedValue);
                if (generalOptions.AddEditExpense.IncludeAssignmentDetails != includeAssignmentDetails)
                {
                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.IncludeAssignmentDetails.GetDescription(), int.Parse(this.ddlEsrDetail.SelectedValue).ToString(), currentUser.CurrentSubAccountId));
                }

                if (generalOptions.ESR.EsrAutoActivateCar != this.chkESRActivateCar.Checked)
                {
                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EsrAutoActivateCar.GetDescription(), this.chkESRActivateCar.Checked.ToString(), currentUser.CurrentSubAccountId));
                }

                if (generalOptions.AddEditExpense.DisplayEsrAddressesInSearchResults != this.chkDisplayEsrAddressesInSearchResults.Checked)
                {
                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DisplayEsrAddressesInSearchResults.GetDescription(), this.chkDisplayEsrAddressesInSearchResults.Checked.ToString(), currentUser.CurrentSubAccountId));
                }

                if (generalOptions.ESR.SummaryEsrInboundFile != this.chkSummaryEsrInboundFile.Checked)
                {
                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.SummaryEsrInboundFile.GetDescription(), this.chkSummaryEsrInboundFile.Checked.ToString(), currentUser.CurrentSubAccountId));
                }

                var esrRounding = (EsrRoundingType)Enum.Parse(typeof(EsrRoundingType), this.ddlEsrRounding.SelectedValue);
                if (generalOptions.ESR.EsrRounding != esrRounding)
                {
                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EsrRounding.GetDescription(), this.ddlEsrRounding.SelectedValue, currentUser.CurrentSubAccountId));
                }

                if (generalOptions.ESR.EnableESRManualAssignmentSupervisor != this.chkESRManualAssignmentSupervisor.Checked)
                {
                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableESRManualAssignmentSupervisor.GetDescription(), this.chkESRManualAssignmentSupervisor.Checked.ToString(), currentUser.CurrentSubAccountId));
                }

                if (generalOptions.ESR.EsrPrimaryAddressOnly != this.chkEsrPrimaryAddressOnly.Checked)
                {
                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EsrPrimaryAddressOnly.GetDescription(), this.chkEsrPrimaryAddressOnly.Checked.ToString(), currentUser.CurrentSubAccountId));
                }
            }

            // Mobile Devices
            if (generalOptions.Mobile.UseMobileDevices != this.chkEnableMobileDevices.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.UseMobileDevices.GetDescription(), this.chkEnableMobileDevices.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            //Expedite Tab - Validation Options
            if (generalOptions.Expedite.AllowReceiptTotalToPassValidation != this.chkValidationOptionsAllowReceipt.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowReceiptTotalToPassValidation.GetDescription(), this.chkValidationOptionsAllowReceipt.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            // Framework - General
            if (generalOptions.Attachment.EnableAttachmentUpload != this.chkUploadAttachmentEnabled.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableAttachmentUpload.GetDescription(), this.chkUploadAttachmentEnabled.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Attachment.EnableAttachmentHyperlink != this.chkHyperlinkAttachmentsEnabled.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableAttachmentHyperlink.GetDescription(), this.chkHyperlinkAttachmentsEnabled.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            var linkAttachmentDefault = Convert.ToByte(this.optLinkAttachmentDefault.SelectedValue);
            if (generalOptions.Attachment.LinkAttachmentDefault != linkAttachmentDefault)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.LinkAttachmentDefault.GetDescription(), linkAttachmentDefault.ToString(), currentUser.CurrentSubAccountId));
            }

            int.TryParse(this.txtMaxUploadSize.Text, out var maxUpload);
            var maxUploadSize = maxUpload == 0 ? 1024 : maxUpload;

            if (generalOptions.Attachment.MaxUploadSize != maxUploadSize)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.MaxUploadSize.GetDescription(), maxUploadSize.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Contract.EnableFlashingNotesIcon != this.chkNotesIconFlash.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableFlashingNotesIcon.GetDescription(), this.chkNotesIconFlash.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Search.ShowProductInSearch != this.chkShowProductInSearch.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ShowProductInSearch.GetDescription(), this.chkShowProductInSearch.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.FinancialYear.FYStarts != this.optFYStarts.SelectedValue)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.FYStarts.GetDescription(), this.optFYStarts.SelectedValue, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.FinancialYear.FYEnds != this.optFYEnds.SelectedValue)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.FYEnds.GetDescription(), this.optFYEnds.SelectedValue, currentUser.CurrentSubAccountId));
            }

            int.TryParse(this.txtDefaultPageSize.Text, out var pageSize);
            var defaultPageSize = pageSize == 0 ? 50 : pageSize;
            if (generalOptions.Page.DefaultPageSize != defaultPageSize)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.DefaultPageSize.GetDescription(), defaultPageSize.ToString(), currentUser.CurrentSubAccountId));
            }

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.Contracts:
                    // Framework - Help & Support
                    if (generalOptions.HelpAndSupport.EnableInternalSupportTickets != this.chkInternalTicketsFW.Checked)
                    {
                        this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableInternalSupportTickets.GetDescription(), this.chkInternalTicketsFW.Checked.ToString(), currentUser.CurrentSubAccountId));
                    }
                    break;
                case Modules.SpendManagement:
                case Modules.SmartDiligence:
                    if (generalOptions.MyDetails.EditMyDetails != this.chkfwEditMyDetails.Checked)
                    {
                        this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EditMyDetails.GetDescription(), this.chkfwEditMyDetails.Checked.ToString(), currentUser.CurrentSubAccountId));
                    }
                    // OVERRIDES THE EXP CHECKBOX IF THIS IS ACTIVE MODULE
                    break;
            }

            if (generalOptions.Flag.FlagMessage != this.txtflagmessage.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.FlagMessage.GetDescription(), this.txtflagmessage.Text, currentUser.CurrentSubAccountId));
            }

            byte frequencytype = 0;

            frequencytype = byte.Parse(this.cmbfrequencytype.SelectedValue);

            if (generalOptions.Claim.FrequencyType != frequencytype)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.FrequencyType.GetDescription(), frequencytype.ToString(), currentUser.CurrentSubAccountId));
            }

            int frequencyvalue = 0;

            if (!string.IsNullOrEmpty(this.txtfrequencyvalue.Text))
            {
                frequencyvalue = Convert.ToInt32(this.txtfrequencyvalue.Text);
            }

            if (generalOptions.Claim.FrequencyValue != frequencyvalue)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.FrequencyValue.GetDescription(), frequencyvalue.ToString(), currentUser.CurrentSubAccountId));
            }

            var claimApprovalReminderFrequency = 0;
            var enableClaimApprovalReminders = false;

            if (this.chkEnableClaimApprovalReminder.Checked)
            {
                claimApprovalReminderFrequency = Convert.ToInt32(this.ddlClaimApprovalReminderFrequency.Text);
                enableClaimApprovalReminders = true;
            }

            if (generalOptions.Reminders.ClaimApprovalReminderFrequency != claimApprovalReminderFrequency)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ClaimApprovalReminderFrequency.GetDescription(), claimApprovalReminderFrequency.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Reminders.EnableClaimApprovalReminders != enableClaimApprovalReminders)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableClaimApprovalReminders.GetDescription(), enableClaimApprovalReminders.ToString(), currentUser.CurrentSubAccountId));
            }

            var claimReminderFrequency = 0;
            var enableCurrentClaimReminders = false;

            if (this.chkEnableCurrentClaimsReminder.Checked)
            {
                claimReminderFrequency = int.Parse(this.ddlCurrentClaimReminderFrequency.Text);
                enableCurrentClaimReminders = true;
            }

            if (generalOptions.Reminders.CurrentClaimsReminderFrequency != claimReminderFrequency)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CurrentClaimsReminderFrequency.GetDescription(), claimReminderFrequency.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Reminders.EnableCurrentClaimsReminders != enableCurrentClaimReminders)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableCurrentClaimsReminders.GetDescription(), enableCurrentClaimReminders.ToString(), currentUser.CurrentSubAccountId));
            }

            // Framework - Products
            if (generalOptions.Contract.AutoUpdateLicenceTotal != this.chkAutoUpdateLicenceTotal.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AutoUpdateLicenceTotal.GetDescription(), this.chkAutoUpdateLicenceTotal.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            // Framework - Tasks
            if (generalOptions.Task.TaskStartDateMandatory != this.chkTaskStartDateMandatory.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.TaskStartDateMandatory.GetDescription(), this.chkTaskStartDateMandatory.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Task.TaskDueDateMandatory != this.chkTaskDueDateMandatory.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.TaskDueDateMandatory.GetDescription(), this.chkTaskDueDateMandatory.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Task.TaskEndDateMandatory != this.chkTaskEndDateMandatory.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.TaskEndDateMandatory.GetDescription(), this.chkTaskEndDateMandatory.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            int.TryParse(this.txtTaskEscalationRepeat.Text, out var taskEsc);
            var taskEscalationRequest = taskEsc == 0 ? 7 : taskEsc;
            if (generalOptions.Task.TaskEscalationRepeat != taskEscalationRequest)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.TaskEscalationRepeat.GetDescription(), taskEscalationRequest.ToString(), currentUser.CurrentSubAccountId));
            }

            // Framework - Contract
            if (generalOptions.Contract.ContractKey != this.txtContractKey.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ContractKey.GetDescription(), this.txtContractKey.Text, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Contract.ContractScheduleDefault != this.txtScheduleDefault.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ContractScheduleDefault.GetDescription(), this.txtScheduleDefault.Text, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Contract.ContractDescTitle != this.txtContractDescriptionTitle.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ContractDescTitle.GetDescription(), this.txtContractDescriptionTitle.Text, currentUser.CurrentSubAccountId));
                accountBase.RelabelReportColumns("CONTRACT_DESC_TITLE", generalOptions.Contract.ContractDescTitle,
                    this.txtContractDescriptionTitle.Text);
            }

            if (generalOptions.Contract.ContractDescShortTitle != this.txtContractDescriptionTitleAbbrev.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ContractDescShortTitle.GetDescription(), this.txtContractDescriptionTitleAbbrev.Text, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Contract.ContractCategoryTitle != this.txtContractCategoryTitle.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ContractCategoryTitle.GetDescription(), this.txtContractCategoryTitle.Text, currentUser.CurrentSubAccountId));
                accountBase.RelabelReportColumns("CONTRACT_CAT_TITLE", generalOptions.Contract.ContractCategoryTitle,
                    this.txtContractCategoryTitle.Text);
            }

            if (generalOptions.Contract.ContractCatMandatory != this.chkContractCategoryMandatory.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ContractCatMandatory.GetDescription(), this.chkContractCategoryMandatory.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Contract.PenaltyClauseTitle != this.txtPenaltyClauseTitle.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PenaltyClauseTitle.GetDescription(), this.txtPenaltyClauseTitle.Text, currentUser.CurrentSubAccountId));

                accountBase.RelabelReportColumns("PENALTY_CLAUSE_TITLE", generalOptions.Contract.PenaltyClauseTitle,
                    this.txtPenaltyClauseTitle.Text);
            }

            if (generalOptions.Contract.TermTypeActive != this.chkTermTypeActive.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.TermTypeActive.GetDescription(), this.chkTermTypeActive.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Contract.ContractDatesMandatory != this.chkContractDatesMandatory.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ContractDatesMandatory.GetDescription(), this.chkContractDatesMandatory.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Contract.InflatorActive != this.chkInflatorActive.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.InflatorActive.GetDescription(), this.chkInflatorActive.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Contract.ContractNumGen != this.chkContractNumberGenerate.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ContractNumGen.GetDescription(), this.chkContractNumberGenerate.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Contract.EnableContractNumUpdate != this.chkContractNumberUpdatable.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableContractNumUpdate.GetDescription(), this.chkContractNumberUpdatable.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Contract.ValueComments != this.txtValueComments.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ValueComments.GetDescription(), this.txtValueComments.Text, currentUser.CurrentSubAccountId));
            }

            int.TryParse(this.txtContractNumberCurSeq.Text, out int seq);
            var contractNumSeq = seq == 0 ? 1 : seq;
            if (generalOptions.Contract.ContractNumSeq != contractNumSeq)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ContractNumSeq.GetDescription(), contractNumSeq.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Contract.AutoUpdateAnnualContractValue != this.chkAutoUpdateCV.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AutoUpdateAnnualContractValue.GetDescription(), this.chkAutoUpdateCV.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Notes.AllowArchivedNotesAdd != this.chkArchivedNotesAdd.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AllowArchivedNotesAdd.GetDescription(), this.chkArchivedNotesAdd.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Contract.InvoiceFreqActive != this.chkInvoiceFrequencyActive.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.InvoiceFreqActive.GetDescription(), this.chkInvoiceFrequencyActive.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Schedule.EnableVariationAutoSeq != this.chkVariationAutoSeq.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableVariationAutoSeq.GetDescription(), this.chkVariationAutoSeq.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            // Framework - Invoice
            if (generalOptions.Invoices.KeepInvoiceForecasts != this.chkKeepInvForecasts.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.KeepInvoiceForecasts.GetDescription(), this.chkKeepInvForecasts.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Invoices.PONumberGenerate != this.chkPONumberGenerate.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PONumberGenerate.GetDescription(), this.chkPONumberGenerate.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Invoices.PONumberFormat != this.txtPONumberFormat.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PONumberFormat.GetDescription(), this.txtPONumberFormat.Text, currentUser.CurrentSubAccountId));
            }

            int.TryParse(this.txtPOSequenceNumber.Text, out seq);
            var pONumberSequence = seq == 1 ? 1 : seq;
            if (generalOptions.Invoices.PONumberSequence != pONumberSequence)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PONumberSequence.GetDescription(), pONumberSequence.ToString(), currentUser.CurrentSubAccountId));
            }

            // Framework - Suppliers
            if (generalOptions.Supplier.SupplierPrimaryTitle != this.txtSupplierPrimaryTitle.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PrimaryTitle.GetDescription(), this.txtSupplierPrimaryTitle.Text, currentUser.CurrentSubAccountId));

                accountBase.RelabelReportColumns("SUPPLIER_PRIMARY_TITLE", generalOptions.Supplier.SupplierPrimaryTitle,
                    this.txtSupplierPrimaryTitle.Text);
            }

            if (generalOptions.Supplier.SupplierRegionTitle != this.txtSupplierRegionTitle.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.RegionTitle.GetDescription(), this.txtSupplierRegionTitle.Text, currentUser.CurrentSubAccountId));

                accountBase.RelabelReportColumns("SUPPLIER_REGION_TITLE", generalOptions.Supplier.SupplierRegionTitle,
                    this.txtSupplierRegionTitle.Text);
            }

            if (generalOptions.Supplier.SupplierCatTitle != this.txtSupplierCategoryTitle.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CatTitle.GetDescription(), this.txtSupplierCategoryTitle.Text, currentUser.CurrentSubAccountId));

                accountBase.RelabelReportColumns("SUPPLIER_CAT_TITLE", generalOptions.Supplier.SupplierCatTitle,
                    this.txtSupplierCategoryTitle.Text);
            }

            if (generalOptions.Supplier.SupplierCatMandatory != this.chkSupplierCategoryMandatory.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CatMandatory.GetDescription(), this.chkSupplierCategoryMandatory.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Supplier.SupplierVariationTitle != this.txtSupplierVariationTitle.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.VariationTitle.GetDescription(), this.txtSupplierVariationTitle.Text, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Supplier.SupplierStatusEnforced != this.chkSupplierStatusMandatory.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.StatusEnforced.GetDescription(), this.chkSupplierStatusMandatory.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Supplier.SupplierTurnoverEnabled != this.chkSupplierTurnoverEnabled.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.TurnoverEnabled.GetDescription(), this.chkSupplierTurnoverEnabled.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Supplier.SupplierNumEmployeesEnabled != this.chkSupplierNumEmployeesEnabled.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.NumEmployeesEnabled.GetDescription(), this.chkSupplierNumEmployeesEnabled.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Supplier.SupplierLastFinCheckEnabled != this.chkLastFinCheckEnabled.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.LastFinCheckEnabled.GetDescription(), this.chkLastFinCheckEnabled.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Supplier.SupplierLastFinStatusEnabled != this.chkLastFinStatusEnabled.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.LastFinStatusEnabled.GetDescription(), this.chkLastFinStatusEnabled.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Supplier.SupplierIntContactEnabled != this.chkIntContactEnabled.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.IntContactEnabled.GetDescription(), this.chkIntContactEnabled.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Supplier.SupplierFYEEnabled != this.chkFYEEnabled.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.FYEEnabled.GetDescription(), this.chkFYEEnabled.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            var homeAddressKeyword = this.txtHomeAddressKeyword.Text.Equals(string.Empty) ? "home" : this.txtHomeAddressKeyword.Text;
            if (generalOptions.AddEditExpense.HomeAddressKeyword != homeAddressKeyword)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.HomeAddressKeyword.GetDescription(), homeAddressKeyword, currentUser.CurrentSubAccountId));
            }

            var workAddressKeyword = this.txtWorkAddressKeyword.Text.Equals(string.Empty) ? "office" : this.txtWorkAddressKeyword.Text;
            if (generalOptions.AddEditExpense.WorkAddressKeyword != workAddressKeyword)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.WorkAddressKeyword.GetDescription(), workAddressKeyword, currentUser.CurrentSubAccountId));
            }

            if (generalOptions.AddEditExpense.ForceAddressNameEntry != this.chkForceAddressNameEntry.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ForceAddressNameEntry.GetDescription(), this.chkForceAddressNameEntry.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.AddEditExpense.MultipleWorkAddress != this.chkMultipleWorkAddress.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.MultipleWorkAddress.GetDescription(), this.chkMultipleWorkAddress.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.AddEditExpense.AddressNameEntryMessage != this.txtAddressNameEntryMessage.Text)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.AddressNameEntryMessage.GetDescription(), this.txtAddressNameEntryMessage.Text, currentUser.CurrentSubAccountId));
            }

            if (this.chkEnableAutoUpdateOfExchangeRates.Checked && !generalOptions.Currency.EnableAutoUpdateOfExchangeRates)
            {
                // Activated Auto update of exchange rates so need to clear the cache.
                var currencies = new cCurrencies(currentUser.AccountID, currentUser.CurrentSubAccountId);
                foreach (cCurrency currency in currencies.currencyList.Values)
                {
                    cCurrency.RemoveFromCache(currentUser.AccountID, currency.currencyid);
                }
            }

            if (generalOptions.Currency.EnableAutoUpdateOfExchangeRates != this.chkEnableAutoUpdateOfExchangeRates.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableAutoUpdateOfExchangeRates.GetDescription(), this.chkEnableAutoUpdateOfExchangeRates.Checked.ToString(), currentUser.CurrentSubAccountId));

                if (this.chkEnableAutoUpdateOfExchangeRates.Checked)
                {
                    var enableAutoUpdateOfExchangeRatesActivatedDate = DateTime.Now;

                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.EnableAutoUpdateOfExchangeRatesActivatedDate.GetDescription(), enableAutoUpdateOfExchangeRatesActivatedDate.ToString(), currentUser.CurrentSubAccountId));

                    accountBase.PopulateExchangeRateRanges((CurrencyType) generalOptions.Currency.CurrencyType, enableAutoUpdateOfExchangeRatesActivatedDate);

                    this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.CurrencyType.GetDescription(), Convert.ToString((int)CurrencyType.Range), currentUser.CurrentSubAccountId));
                }
            }
            
            if (generalOptions.Car.PopulateDocumentsFromVehicleLookup != this.chkVehicleDocumentLookup.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.PopulateDocumentsFromVehicleLookup.GetDescription(), this.chkVehicleDocumentLookup.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            if (generalOptions.Claim.BlockUnmatchedExpenseItemsBeingSubmitted != this.chkBlockUnmatchedExpenseItemsBeingSubmitted.Checked)
            {
                this.AccountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.BlockUnmatchedExpenseItemsBeingSubmitted.GetDescription(), this.chkBlockUnmatchedExpenseItemsBeingSubmitted.Checked.ToString(), currentUser.CurrentSubAccountId));
            }

            //Invalidate cache so couchbase cache stays in line with Redis caching
            accountBase.InvalidateCache(currentUser.CurrentSubAccountId);

            if (enableCurrentClaimReminders)
            {
                Employee.SetInitialClaimantReminderDate(currentUser.AccountID);
            }
            if (generalOptions.DutyOfCare.DutyOfCareApprover != dutyOfCareAproverOnForm)
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
            var vehivleStatusFIlterOnDOC = this.chkDisableCarOutsideOfStartEndDate.Checked;

            if (generalOptions.AddEditExpense.DisableCarOutsideOfStartEndDate != vehivleStatusFIlterOnDOC)
            {
                var dutyOfCare = new DutyOfCare();
                dutyOfCare.ChangeVehicleStatusFilter(currentUser.AccountID, vehivleStatusFIlterOnDOC);    
                // this will refresh the cache.
                var customEntities = new cCustomEntities(currentUser, true);
            }

            //Check if the Automatic lookup is enabled for the account 
            if (this.chkDrivingLicenceLookup.Checked && !generalOptions.DutyOfCare.EnableAutomaticDrivingLicenceLookup)
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

            // Check if the Automatic document lookup is enabled for the account
            if (this.chkVehicleDocumentLookup.Checked && !generalOptions.Car.PopulateDocumentsFromVehicleLookup)
            {
                var employees = new cEmployees(currentUser.AccountID);
                var employeeList = employees.GetAllEmployeeIDsAndUsernamesList();
                using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
                {
                    connection.ExecuteSQL("UPDATE cars set TaxValid = 1, MotValid = 1");
                }

                foreach (var employeeId in employeeList.Keys)
                {
                    employees.Cache.Delete(currentUser.AccountID, cEmployeeCars.CacheKey, employeeId.ToString());
                }
            }
            
            // if mobile devices and attach receipts enabled, ensure PNG attachment types are enabled
            if (this.chkattach.Checked && this.chkEnableMobileDevices.Checked)
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
                case Modules.Contracts:
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
                case Modules.Contracts:
                    tabIdsToHide.Add(this.tabGeneral.ClientID);
                    tabIdsToHide.Add(this.tabSelfRegistration.ClientID);
                    tabIdsToHide.Add(this.tabDelegates.ClientID);
                    tabIdsToHide.Add(this.tabDeclarations.ClientID);
                    tabIdsToHide.Add(this.tabMobileDevices.ClientID);
                    tabIdsToHide.Add(this.tabEmployees.ClientID);
                    tabIdsToHide.Add(this.tabExpedite.ClientID);
                    break;
                case Modules.Expenses:
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
                case Modules.Contracts:
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
                case Modules.Contracts:
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

