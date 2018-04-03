namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Web.Services;
    using SpendManagementLibrary;
    using System.Text;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Employees.DutyOfCare;
    using Spend_Management.shared.code;

    /// <summary>
    /// Summary description for aeemployee.
    /// </summary>
    public partial class aeemployee : Page
    {
        private cAccountSubAccounts _subAccounts;
        
        private cAccessRoles _accessRoles;

        private cEmployees _employees;

        private cTables _tables;

        private cFields _fields;

        private cAccountProperties _accountProperties;

        private cUserdefinedFields _userDefinedFields;

        private CurrentUser _user;

        protected ImageButton cmdhelp;
        protected Label Label9;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Fix IE10 Image Button issues
            if (base.Master != null)
            {
                var theMetaTag = new HtmlMeta();

                theMetaTag.Attributes.Add("http-equiv", "x-ua-compatible");
                theMetaTag.Attributes.Add("content", "IE=9");

                base.Master.Page.Header.Controls.AddAt(0, theMetaTag);
            }

            Master.UseDynamicCSS = true;
            Response.Buffer = true;
            Response.Expires = 0;
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            Response.CacheControl = "no-cache";
            Response.AddHeader("pragma", "no-cache");
            Response.AddHeader("cache-control", "private");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));

            Master.showdummymenu = true;
            Master.enablenavigation = false;

            this._user = cMisc.GetCurrentUser();
            this._user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, true, true);

            this._subAccounts = new cAccountSubAccounts(this._user.AccountID);
            this._employees = new cEmployees(this._user.AccountID);
            this._tables = new cTables(this._user.AccountID);
            this._fields = new cFields(this._user.AccountID);
            this._userDefinedFields = new cUserdefinedFields(this._user.AccountID);

            ViewState["accountid"] = this._user.AccountID;
            ViewState["adminemployeeid"] = this._user.EmployeeID;

            Employee reqemp = null;
            this._accountProperties = this._subAccounts.getSubAccountById(this._user.CurrentSubAccountId).SubAccountProperties;

            if (this._user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Employees, false) == false && this._user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Employees, false) == false)
            {
                spanSaveButton.Visible = false;
            }

            if (this._user.Account.IsNHSCustomer == false)
            {
                pnlNHSDetails.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
            else
            {
                var clsESRTrusts = new cESRTrusts(this._user.AccountID);
                ddlNHSTrust.Items.Add(new ListItem("[None]", "0"));
                clsESRTrusts.CreateDropDownList(ref ddlNHSTrust);

                if (this._accountProperties.CheckESRAssignmentOnEmployeeAdd)
                {
                    var checkESRVal = new CustomValidator();
                    checkESRVal.ID = "custCheckESRVal";
                    checkESRVal.ClientValidationFunction = "CheckEmployeeHasAnESRAssignment";
                    checkESRVal.ValidationGroup = "vgAssign";
                    checkESRVal.Text = "*";
                    checkESRVal.ErrorMessage = "There must be an ESR Assignment number created for this employee before saving";
                    pnlESRAssignments.Controls.Add(checkESRVal);
                }

                List<object> acParams = AutoComplete.getAutoCompleteQueryParams("employeesWithUsername");
                string acBindStr = AutoComplete.createAutoCompleteBindString("txtEsrSupervisor", 15, this._tables.GetTableByName("employees").TableID, (Guid)acParams[0], (List<Guid>)acParams[1], 500);
                this.ClientScript.RegisterStartupScript(this.GetType(), "autocompleteBindAssignmentSupervisor", AutoComplete.generateScriptRegisterBlock(new List<string>() { acBindStr }), true);
            }

            if (this._user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EmployeeBankAccounts, true))
            {
                usrBankAccounts.Visible = true;
                var output = new StringBuilder();
                output.Append("<div class=\"sectiontitle\">Employee Bank Accounts</div>");

                usrBankAccounts.AccountsEmployeeId = 0;
                usrBankAccounts.RedactGridData = true;
                usrBankAccounts.AllowEdit = this._user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.EmployeeBankAccounts, true);
                usrBankAccounts.AllowDelete = this._user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.EmployeeBankAccounts, true);

                if (this._user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.EmployeeBankAccounts, true))
                {
                    output.Append("<a href=\"javascript:showBankAccount(true);\">New Bank Account</a>");
                }

                litbankdetails.Text = output.ToString();
            }
            else
            {
                usrBankAccounts.Visible = false;
            }

            if (IsPostBack == false)
            {
                int employeeid = 0;
                if (Request.QueryString["employeeid"] != null)
                {
                    employeeid = Convert.ToInt32(Request.QueryString["employeeid"]);
                    usrMobileDevices.DevicesEmployeeID = employeeid;
                    usrBankAccounts.AccountsEmployeeId = employeeid;
                }
                ViewState["employeeid"] = employeeid;

                var script = new StringBuilder();

                if (employeeid > 0)
                {
                    Title = "Employee: " + _employees.GetEmployeeById(employeeid).Username;
                }
                else
                {
                    Title = "New Employee";
                }

                switch (this._user.CurrentActiveModule)
                {
                    case Modules.contracts:
                        if (employeeid > 0)
                            Master.helpid = 1139;
                        else
                            Master.helpid = 1164;
                        break;
                    default:
                        Master.helpid = 1031;
                        break;
                }

                Master.title = Title;
                Master.PageSubTitle = "Employee Details";

                #region set-up screen
                var clscountries = new cCountries(this._user.AccountID, this._user.CurrentSubAccountId);
                var clscurrencies = new cCurrencies(this._user.AccountID, this._user.CurrentSubAccountId);
                var clsgroups = new cGroups(this._user.AccountID);
                var clsitemroles = new ItemRoles(this._user.AccountID);

                switch (this._user.CurrentActiveModule)
                {
                    case Modules.SpendManagement:
                    case Modules.SmartDiligence:
                    case Modules.CorporateDiligence:
                    case Modules.contracts:
                        var sb = new StringBuilder();
                        sb.Append("document.getElementById('lineManagerDiv').style.display = 'none';\n");
                        sb.Append("document.getElementById('startMileageDiv').style.display = 'none';\n");
                        sb.Append("document.getElementById('startMileageDiv2').style.display = 'none';\n");
                        sb.Append("document.getElementById('ccodecontrol').style.display = 'none';\n");
                        string tabId = this.tabClaims.ClientID;
                        sb.Append("hideTab('" + tabId + "');\n");
                        tabId = this.tabPersonal.ClientID;
                        sb.Append("hideTab('" + tabId + "');\n");
                        tabId = this.tabEmailNotifications.ClientID;
                        sb.Append("hideTab('" + tabId + "');\n");
                        ClientScriptManager sm = Page.ClientScript;
                        if (!sm.IsClientScriptBlockRegistered("hideTabs"))
                        {
                            sm.RegisterStartupScript(this.GetType(), "hideTabs", sb.ToString(), true);
                        }

                        // disable validators for hidden fields in the vgMain validation group
                        compstartmiles.Enabled = false;
                        reghome.Enabled = false;
                        compdateofbirth.Enabled = false;
                        compdateofbirthgt.Enabled = false;
                        break;
                    case Modules.Greenlight:
                    case Modules.GreenlightWorkforce:
                        var stringBuilder = new StringBuilder();
                        stringBuilder.Append("document.getElementById('startMileageDiv').style.display = 'none';\n");
                        stringBuilder.Append("document.getElementById('startMileageDiv2').style.display = 'none';\n");
                        stringBuilder.Append("document.getElementById('ccodecontrol').style.display = 'none';\n");
                        string claimTabId = this.tabClaims.ClientID;
                        stringBuilder.Append("hideTab('" + claimTabId + "');\n");
                        ClientScriptManager scriptMan = Page.ClientScript;
                        if (!scriptMan.IsClientScriptBlockRegistered("hideTabs"))
                        {
                            scriptMan.RegisterStartupScript(this.GetType(), "hideTabs", stringBuilder.ToString(), true);
                        }

                        // disable validators for hidden fields in the vgMain validation group
                        this.compstartmiles.Enabled = false;

                        this.pageOptions.Text = SetPageOptions(false, false);
                        break;
                    default:
                        this.pageOptions.Text = SetPageOptions(this._accountProperties.UseMobileDevices, true);
                        break;
                }

                cmbsignoffs.Items.AddRange(clsgroups.CreateDropDown(0));
                cmbsignoffcc.Items.AddRange(clsgroups.CreateDropDown(0));
                cmbsignoffpc.Items.AddRange(clsgroups.CreateDropDown(0));
                cmbadvancesgroup.Items.AddRange(clsgroups.CreateDropDown(0));

                if (this._accountProperties.OnlyCashCredit == false)
                {
                    cmbsignoffcc.Enabled = false;
                    cmbsignoffpc.Enabled = false;
                }

                var clsproviders = new CardProviders();
                cmbcardprovider.Items.AddRange(clsproviders.CreateList());

                List<sAutoCompleteResult> poolCars = AutoComplete.GetAutoCompleteMatches(this._user, 0, "A184192F-74B6-42F7-8FDB-6DCF04723CEF", "C05F029A-37B4-4FB6-AD31-CA4F2606599C", "C05F029A-37B4-4FB6-AD31-CA4F2606599C", "", true, new Dictionary<string, JSFieldFilter> { { "0", new JSFieldFilter() { ConditionType = ConditionType.DoesNotContainData, FieldID = new Guid("5ddbf0ef-fa06-4e7c-a45a-54e50e33307e") } } });
                if (poolCars.Count <= 25)
                {
                    txtPoolCar.Style.Add(HtmlTextWriterStyle.Display, "none");
                    txtPoolCarSearchIcon.Style.Add(HtmlTextWriterStyle.Display, "none");
                    txtPoolCarSelect.Items.AddRange(poolCars.Select(x => new ListItem(x.label, x.value)).ToArray());
                }
                else
                {
                    txtPoolCarSelect.Style.Add(HtmlTextWriterStyle.Display, "none");
                    txtPoolCarSearchIcon.ImageUrl = GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/find.png";
                    var parameters = new List<Guid>() { new Guid("C05F029A-37B4-4FB6-AD31-CA4F2606599C") }; // dbo.GetCarMakeModelRegistrationById
                    string acBindStr = AutoComplete.createAutoCompleteBindString("txtPoolCar", 15, new Guid("A184192F-74B6-42F7-8FDB-6DCF04723CEF"), parameters[0], parameters, fieldFilters: new SortedList<int, FieldFilter> { { 0, new FieldFilter(new cFields(this._user.AccountID).GetFieldByID(new Guid("5ddbf0ef-fa06-4e7c-a45a-54e50e33307e")), ConditionType.DoesNotContainData, "", "", 0, null) } });
                    this.ClientScript.RegisterStartupScript(this.GetType(), "poolCarAutoCompleteBind", AutoComplete.generateScriptRegisterBlock(new List<string> { acBindStr }), true);
                }

                aeCar.EmployeeID = employeeid;
                aeCar.AccountID = this._user.AccountID;
                aeCar.EmployeeAdmin = true;
                this.aeCar.Action = employeeid > 0 ? aeCarPageAction.Edit : aeCarPageAction.Add;

                // load the user's currently active cars so they can choose to replace old with new
                var employeeCars = new cEmployeeCars(this._user.AccountID, employeeid);
                this.aeCar.cmbPreviousCar.Items.AddRange(employeeCars.CreateCurrentValidCarDropDown(DateTime.UtcNow, "This employee currently has no cars to replace").ToArray());
                
                costcodebreakdown.UserControlDisplayType = UserControlType.Inline;
                costcodebreakdown.HideButtons = true;

                var clslocales = new cLocales();

                ddlstLocale.Items.Add(new ListItem("[None]", "0"));
                ddlstLocale.Items.AddRange(clslocales.CreateActiveDropDown().ToArray());

                List<sAutoCompleteResult> lineManagers = AutoComplete.GetAutoCompleteMatches(this._user, 0, "618DB425-F430-4660-9525-EBAB444ED754", "142EA1B4-7E52-4085-BAAA-9C939F02EB77", "142EA1B4-7E52-4085-BAAA-9C939F02EB77,0F951C3E-29D1-49F0-AC13-4CFCABF21FDA", "", true, new Dictionary<string, JSFieldFilter> { { "0", new JSFieldFilter() { ConditionType = ConditionType.NotLike, FieldID = new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795"), ValueOne = "admin%" } } });
                if (lineManagers.Count <= 25)
                {
                    txtLineManager.Style.Add(HtmlTextWriterStyle.Display, "none");
                    txtLineManagerSearchIcon.Style.Add(HtmlTextWriterStyle.Display, "none");
                    txtLineManagerSelect.Items.Add(new ListItem("[None]", "0"));
                    txtLineManagerSelect.Items.AddRange(lineManagers.Select(x => new ListItem(x.label, x.value)).ToArray());
                }
                else
                {
                    txtLineManagerSelect.Style.Add(HtmlTextWriterStyle.Display, "none");
                    txtLineManagerSearchIcon.ImageUrl = GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/find.png";
                    List<object> parameters = AutoComplete.getAutoCompleteQueryParams("employeesWithUsername");
                    string acBindStr = AutoComplete.createAutoCompleteBindString("txtLineManager", 15, this._tables.GetTableByID(new Guid("618DB425-F430-4660-9525-EBAB444ED754")).TableID, (Guid)parameters[0], (List<Guid>)parameters[1], fieldFilters: new SortedList<int, FieldFilter> { { 0, new FieldFilter(new cFields(this._user.AccountID).GetFieldByID(new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795")), ConditionType.NotLike, "admin%", null, 0, null) } });
                    this.ClientScript.RegisterStartupScript(this.GetType(), "lineManagerAutoCompleteBind", AutoComplete.generateScriptRegisterBlock(new List<string>() { acBindStr }), true);
                }

                script.Append("var accessRoles = new Array();\nvar itemRoles = new Array();\n");

                this.modcar.CancelControlID = this.aeCar.FindControl("cmdcancel").ClientID;
                
                #endregion

                cmbSubAccounts.Items.AddRange(_subAccounts.CreateDropDown(null));

                string[] workLocationGridData;
                string[] homeLocationGridData;
                string[] ESRAssignmentGridData;
                string[] newAccessRoleGridData;
                string[] accessRoleGridData;
                string[] itemRoleGridData;
                var jsGridObjects = new List<string>();

                if (employeeid > 0)
                {
                    this._user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Employees, true, true);

                    reqemp = _employees.GetEmployeeById(employeeid);

                    cmdactivate.Visible = !reqemp.Active;


                    if (this._user.Account.IsNHSCustomer && reqemp.NhsTrustID.HasValue)
                    {
                        if (ddlNHSTrust.Items.FindByValue(reqemp.NhsTrustID.Value.ToString()) != null)
                        {
                            ddlNHSTrust.Items.FindByValue(reqemp.NhsTrustID.Value.ToString()).Selected = true;
                        }
                    }

                    // insert values into fields
                    txtusername.Text = reqemp.Username;

                    txttitle.Text = reqemp.Title;
                    txtfirstname.Text = reqemp.Forename;
                    txtmiddlenames.Text = reqemp.MiddleNames;
                    txtsurname.Text = reqemp.Surname;
                    txtmaidenname.Text = reqemp.MaidenName;
                    txtpreferredname.Text = reqemp.PreferredName;

                    if (cmbgender.Items.FindByValue(reqemp.Gender) != null)
                    {
                        cmbgender.Items.FindByValue(reqemp.Gender).Selected = true;
                    }

                    if (reqemp.DateOfBirth.HasValue)
                    {
                        txtdateofbirth.Text = reqemp.DateOfBirth.Value.ToShortDateString();
                    }

                    if (reqemp.HiredDate.HasValue)
                    {
                        txthiredate.Text = reqemp.HiredDate.Value.ToShortDateString();
                    }

                    txttelno.Text = reqemp.TelephoneNumber;
                    txtemail.Text = reqemp.EmailAddress.Trim();
                    txtcreditaccount.Text = reqemp.Creditor;

                    txtextension.Text = reqemp.TelephoneExtensionNumber;
                    txtmobileno.Text = reqemp.MobileTelephoneNumber;
                    txtstartmiles.Text = reqemp.MileageTotal.ToString();
                    txtstartmileagedate.Text = (reqemp.MileageTotalDate.HasValue ? reqemp.MileageTotalDate.Value.ToShortDateString() : "");
                    if (!_employees.enableStartMileage(reqemp.EmployeeID))
                    {
                        txtstartmiles.Enabled = false;
                        txtstartmileagedate.Enabled = false;
                        calexstartmileagedate.Enabled = false;
                        cmpmaxstartmileagedate.Enabled = false;
                    }
                    else
                    {
                        // set the validation max date to be the current date as it  can't be in the future
                        string maxDate = DateTime.Now.Date.ToShortDateString();
                        cmpmaxstartmileagedate.ValueToCompare = maxDate;
                        cmpmaxstartmileagedate.ErrorMessage = "Start mileage date must be on or before " + maxDate;
                    }

                    var currentActiveCars = employeeCars.GetActiveCars();
                    if (currentActiveCars != null && currentActiveCars.Count > 0)
                    {
                        this.txtCurrentMileage.Text = ((long)this._employees.getMileageTotal(reqemp.EmployeeID, DateTime.Today, employeeCars.GetFinancialYearId(currentActiveCars[0]))).ToString();
                    }
                    else
                    {
                        this.txtCurrentMileage.Text = ((long)this._employees.getMileageTotal(reqemp.EmployeeID, DateTime.Today)).ToString();
                    }
                    
                    txtposition.Text = reqemp.Position;
                    txtpayroll.Text = reqemp.PayrollNumber;
                    txtemailhome.Text = reqemp.HomeEmailAddress;
                    txtextension.Text = reqemp.TelephoneExtensionNumber;
                    txtmobileno.Text = reqemp.MobileTelephoneNumber;
                    txtpagerno.Text = reqemp.PagerNumber;
                    txtfaxno.Text = reqemp.FaxNumber;
                    txtEmployeeNumber.Text = reqemp.EmployeeNumber;
                    txtuniquenhsid.Text = reqemp.NhsUniqueID;
                    this.txtExcessMileage.Text = reqemp.ExcessMileage <= 0 ? String.Empty : reqemp.ExcessMileage.ToString();

                    cmbsignoffs.Items.Clear();
                    cmbsignoffs.Items.AddRange(clsgroups.CreateDropDown(reqemp.SignOffGroupID));
                    cmbsignoffcc.Items.Clear();
                    cmbsignoffcc.Items.AddRange(clsgroups.CreateDropDown(reqemp.CreditCardSignOffGroup));
                    cmbsignoffpc.Items.Clear();
                    cmbsignoffpc.Items.AddRange(clsgroups.CreateDropDown(reqemp.PurchaseCardSignOffGroup));
                    cmbadvancesgroup.Items.Clear();
                    cmbadvancesgroup.Items.AddRange(clsgroups.CreateDropDown(reqemp.AdvancesSignOffGroup));

                    // disable if any active submitted claims exists to prevent change of signoff group
                    var clsclaims = new cClaims(this._user.AccountID);
                    
                    if (clsclaims.getSubmittedClaimsCount(reqemp.EmployeeID) > 0)
                    {
                        // employee has current submitted claims, so don't let them change their signoff group
                        cmbsignoffs.Enabled = false;
                        cmbsignoffcc.Enabled = false;
                        cmbsignoffpc.Enabled = false;
                    }

                    // disable the advances signoff list if unapproved advances for the employee exist
                    var clsfloats = new cFloats(this._user.AccountID);
                    if (clsfloats.getFloatsByEmployeeId(reqemp.EmployeeID, false).Count > 0)
                    {
                        cmbadvancesgroup.Enabled = false;
                    }

                    txtninumber.Text = reqemp.NationalInsuranceNumber;

                    if (reqemp.TerminationDate != null)
                    {
                        txtleavedate.Text = ((DateTime)reqemp.TerminationDate).ToShortDateString();
                    }

                    cmbcountry.Items.AddRange(clscountries.CreateDropDown().ToArray());
                    if (cmbcountry.Items.FindByValue(reqemp.PrimaryCountry.ToString()) != null)
                    {
                        cmbcountry.Items.FindByValue(reqemp.PrimaryCountry.ToString()).Selected = true;
                    }
                    else
                    {
                        if (clscountries.list.ContainsKey(reqemp.PrimaryCountry))
                        {
                            // may be archived so populate into ddlist just for the edit
                            cmbcountry.Items.Add(clscountries.GetListItem(reqemp.PrimaryCountry));
                            cmbcountry.Items.FindByValue(reqemp.PrimaryCountry.ToString()).Selected = true;
                        }
                    }
                    if (ddlstLocale.Items.FindByValue(reqemp.LocaleID.ToString()) !=null)
                    {
                        ddlstLocale.Items.FindByValue(reqemp.LocaleID.ToString()).Selected = true;
                    }

                    cmbcurrency.Items.AddRange(clscurrencies.CreateDropDown(reqemp.PrimaryCurrency));

                    int currentcount = clsclaims.getCount(reqemp.EmployeeID, ClaimStage.Current);

                    if (currentcount == 1 && this._accountProperties.SingleClaim)
                    {
                        // are there any items on the claim
                        cClaim claim = clsclaims.getClaimById(clsclaims.getDefaultClaim(ClaimStage.Current, employeeid));
                        if (claim.NumberOfItems == 0)
                        {
                            cmbcurrency.Enabled = false;
                        }
                    }
                    else if (currentcount > 0 && !this._accountProperties.SingleClaim) //multiple claims, don't want to change currency as it will mix them up
                    {
                        cmbcurrency.Enabled = false;
                    }

                    if (reqemp.LineManager != 0)
                        {
                        string lineManagerNumber = reqemp.LineManager.ToString(CultureInfo.InvariantCulture);
                        txtLineManager_ID.Text = lineManagerNumber;
                        if (txtLineManagerSelect.Items.Count > 0 && txtLineManagerSelect.Items.FindByValue(lineManagerNumber) != null)
                    {
                            txtLineManagerSelect.Items.FindByValue(lineManagerNumber).Selected = true;
                    }
                        else if (lineManagers.Count > 25)
                        {
                            txtLineManager.Text = lineManagers.Where(x => x.value == lineManagerNumber).DefaultIfEmpty(new sAutoCompleteResult { label = string.Empty }).First().label;
                        }
                    }
                    ViewState["record"] = this._employees.GetUserDefinedFields(reqemp.EmployeeID);
                    itemRoleGridData = CreateItemRoleGrid(reqemp.EmployeeID.ToString());
                    cmbDefaultSubAccount.Items.AddRange(this._subAccounts.CreateDropDown(reqemp.DefaultSubAccount));

                    accessRoleGridData = CreateAccessRoleGrid(reqemp.EmployeeID.ToString());

                    foreach (KeyValuePair<int, cAccountSubAccount> sacc in this._subAccounts.getSubAccountsCollection())
                    {
                        cAccountSubAccount sa = sacc.Value;
                        List<int> lstAccessRoles = reqemp.GetAccessRoles().GetBy(sa.SubAccountID);
                        if (lstAccessRoles != null)
                        {
                            script.Append("var roleList = new Array();\n");
                            foreach (int val in lstAccessRoles)
                            {
                                script.Append("roleList.push(" + val + ");\n");
                            }
                            script.Append("accessRoles[" + sa.SubAccountID + "] = roleList;\n");
                        }
                    }

                    List<EmployeeItemRole> lstItemRoles = reqemp.GetItemRoles().ItemRoles;

                    foreach (EmployeeItemRole itemRole in lstItemRoles)
                    {
                        ItemRole reqItemRole = clsitemroles.GetItemRoleById(itemRole.ItemRoleId);
                        script.Append("itemRoles.push(" + reqItemRole.ItemRoleId + ");\n");
                    }
                    workLocationGridData = createWorkLocationGrid(employeeid.ToString());
                    homeLocationGridData = createHomeLocationGrid(employeeid.ToString());
                    ESRAssignmentGridData = createESRAssignmentGrid(employeeid.ToString());

                    cDepCostItem[] lstCostCodes = reqemp.GetCostBreakdown().ToArray();
                    createCostCentreGrid(lstCostCodes);
                    newAccessRoleGridData = createNewAccessRoleGrid(employeeid.ToString());
                    
                    // Audit if employee details are viewed by any user except for himself
                    if (reqemp.EmployeeID != _user.EmployeeID)
                    {
                        cAuditLog auditLog = new cAuditLog();
                        auditLog.ViewRecord(SpendManagementElement.Employees, reqemp.Username, this._user);
                    }

                }
                else
                {
                    this._user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Employees, true, true);

                    int? defSA = null;
                    if (this._subAccounts.Count == 1)
                    {
                        defSA = this._subAccounts.getFirstSubAccount().SubAccountID;
                    }
                    cmbDefaultSubAccount.Items.AddRange(this._subAccounts.CreateDropDown(defSA));

                    int selectedCountry = 0;
                    if (this._accountProperties.BaseCurrency.HasValue)
                    {
                        selectedCountry = this._accountProperties.BaseCurrency.Value;
                    }
                    cmbcurrency.Items.AddRange(clscurrencies.CreateDropDown(selectedCountry));
                    cmbcountry.Items.AddRange(clscountries.CreateDropDown().ToArray());
                    if (this._accountProperties.HomeCountry != 0 && !cmbcountry.Items.Contains(clscountries.GetListItem(this._accountProperties.HomeCountry)))
                    {
                        cmbcountry.Items.Add(clscountries.GetListItem(this._accountProperties.HomeCountry));
                    }

                    foreach (ListItem lstItem in cmbcountry.Items)
                    {
                        if (lstItem.Value == this._accountProperties.HomeCountry.ToString())
                        {
                            lstItem.Selected = true;
                            break;
                        }
                    }

                    itemRoleGridData = CreateItemRoleGrid("");
                    accessRoleGridData = CreateAccessRoleGrid("");
                    workLocationGridData = createWorkLocationGrid("");
                    homeLocationGridData = createHomeLocationGrid("");
                    ESRAssignmentGridData = createESRAssignmentGrid("");
                    newAccessRoleGridData = createNewAccessRoleGrid("");
                }

                lititemroles.Text = itemRoleGridData[2];
                litAccessRoles.Text = accessRoleGridData[2];
                litESRAssignments.Text = ESRAssignmentGridData[2];
                litworklocations.Text = workLocationGridData[2];
                lithomelocations.Text = homeLocationGridData[2];
                litNewAccessRoles.Text = newAccessRoleGridData[2];

                if (this._subAccounts.Count <= 1)
                {
                    cmbDefaultSubAccount.Enabled = false;
                    cmbSubAccounts.Enabled = false;
                }
                else
                {
                    cmbSubAccounts.Items.Insert(0, new ListItem("[None]", "-1"));
                }

                string[] carGridData = CreateCarGrid();
                litcargrid.Text = carGridData[2];

                string[] poolCarGridData = CreatePoolCarGrid();
                litpoolcargrid.Text = poolCarGridData[2];

                string[] corpCardGridData = CreateCorporateCardGrid();
                litcards.Text = corpCardGridData[2];

                // set the sel.grid javascript variables
                jsGridObjects.Add(itemRoleGridData[1]);
                jsGridObjects.Add(accessRoleGridData[1]);
                jsGridObjects.Add(ESRAssignmentGridData[1]);
                jsGridObjects.Add(workLocationGridData[1]);
                jsGridObjects.Add(homeLocationGridData[1]);
                jsGridObjects.Add(newAccessRoleGridData[1]);
                jsGridObjects.Add(carGridData[1]);
                jsGridObjects.Add(poolCarGridData[1]);
                jsGridObjects.Add(corpCardGridData[1]);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "aeEmpGridVars", cGridNew.generateJS_init("aeEmpGridVars", jsGridObjects, this._user.CurrentActiveModule), true);

                ClientScript.RegisterClientScriptBlock(this.GetType(), "script", script.ToString(), true);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "variables1", "var nEmployeeid = " + employeeid + ";", true);
                if (employeeid > 0 && this._user.Account.IsNHSCustomer && (reqemp != null && reqemp.EsrPersonID.HasValue))
                {
                    this.litShowEsr.Text = string.Format("<a href=\"javascript:showEsrDetailsModal(1, {0});\" class=\"submenuitem\">ESR Details</a>", reqemp.EsrPersonID.Value);
                }
                
                ClientScript.RegisterClientScriptBlock(this.GetType(), "variables2", "var attachDocType = 1;", true);

                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "variables3", this._accountProperties.AllowEmployeeInOwnSignoffGroup ? "var bAllowSelfInSignoffStages = true;" : "var bAllowSelfInSignoffStages = false;", true);

                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "NHSVariables1", this._accountProperties.CheckESRAssignmentOnEmployeeAdd ? "var CheckESRAssignments = true;" : "var CheckESRAssignments = false;", true);

                if (employeeid > 0)
                {
                    if (reqemp.Active == false)
                    {
                        spanPasswordKey.Style.Add(HtmlTextWriterStyle.Display, string.Empty);
                    }
                    else
                    {
                        divEmailEmp.Style.Add(HtmlTextWriterStyle.Display, "none");
                        spanPasswordKey.Style.Add(HtmlTextWriterStyle.Display, "none");
                        spanWelcomeEmail.Style.Add(HtmlTextWriterStyle.Display, "none");
                        chkWelcomeEmail.Checked = false;
                        chkSendPasswordEmail.Checked = false;
                    }

                    ClientScript.RegisterClientScriptBlock(this.GetType(), "NewEmployee", "var NewEmployee = false;", true);
                }
                else
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "NewEmployee", "var NewEmployee = true;", true);
                }

                addressDetailsPopup.AddEvents(imgHomeLocationSearch, hdnHomeLocationID, string.Empty, false); //  imgHomeLocationAddressDetails
                addressDetailsPopup.AddEvents(imgWorkLocationSearch, hdnWorkLocationID, string.Empty, false); //  imgWorkLocationAddressDetails

                #region EmailNotifications
                if (employeeid > 0)
                {
                    reqemp = this._employees.GetEmployeeById(employeeid);
                }
                int? authoriserLevelId = null;
                
                PopulateDropDownListAuthoriserLevel();
                if (reqemp != null)
                {
                    authoriserLevelId = _employees.GetAuthoriserLevelIdByEmployee(reqemp.EmployeeID);
                    hdnAuthoriserLevel.Value = authoriserLevelId.ToString();
                }
                if (authoriserLevelId != null)
                {
                    var result = this._employees.CheckDefaultApprover((int)authoriserLevelId);
                    if (result)
                    {
                        cmbAuthoriserLevel.Enabled = false;
                        lblUnAssingDefaultApprover.Visible = true;
                        lblUnAssingDefaultApprover.InnerText = "This employee has been defined as the Default authoriser therefore their Authoriser level cannot be modified.";
                        hdnDefaultAuthoriserLevelId.Value = authoriserLevelId.ToString();
                    }
                }
                
                tabAddAuthoriserLevel.Visible = this._user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuthoriserLevel, true);
                if (cmbAuthoriserLevel.Enabled)
                {
                    if (reqemp != null && authoriserLevelId != null)
                    {
                        var cmbBoxItem = cmbAuthoriserLevel.Items.FindByValue(authoriserLevelId.ToString());
                        if (cmbBoxItem != null)
                        {
                            cmbBoxItem.Selected = true;
                        }
                    }
                }

                this.GenerateEmailNotifications(employeeid, reqemp);
            }

            cTable tbl = this._tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754")); // employees uf
            StringBuilder udfscript;
            List<int> pkExcludeList = null;
            if ((int)ViewState["employeeid"] > 0)
            {
                // exclude the employee id from being edited from any auto complete UDFs
                pkExcludeList = new List<int> { (int)ViewState["employeeid"] };
            }
            this._userDefinedFields.createFieldPanel(ref holderUserdefined, this._tables.GetTableByID(tbl.UserDefinedTableID), "vgMain", out udfscript, pkExcludeList: pkExcludeList);
            this.ClientScript.RegisterStartupScript(this.GetType(), "udfs", udfscript.ToString(), true);

            if (ViewState["record"] != null)
            {
                this._userDefinedFields.populateRecordDetails(ref holderUserdefined, this._tables.GetTableByID(tbl.UserDefinedTableID), (SortedList<int, object>)ViewState["record"]);
            }

            this.BindSignOffOwnerAutoComplete(this._user);
        }

        [WebMethod(EnableSession = true)]
        public static string[] createESRAssignmentGrid(string contextKey)
        {
            int employeeid = 0;
            if (contextKey != "")
            {
                int.TryParse(contextKey, out employeeid);
            }
            CurrentUser user = cMisc.GetCurrentUser();
            cESRAssignments clsassignments = new cESRAssignments(user.AccountID, employeeid);
            return clsassignments.getCurrentAssignmentsGrid(true);

        }

        [WebMethod(EnableSession = true)]
        public static string[] createWorkLocationGrid(string contextKey)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            int employeeid = 0;
            if (contextKey != "")
            {
                int.TryParse(contextKey, out employeeid);
            }
            
            cTables tables = new cTables(user.AccountID);
            cFields fields = new cFields(user.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("f5acae20-9dd6-4e64-9b1e-740013e7e586")))); // EmployeeWorkAddressId
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("39cf4e42-97a6-442d-bd00-28b271f04057")))); // StartDate
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("66cb2f1a-0b37-41b9-ba41-c6b6c76e72a2")))); // EndDate
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("1ca2eaf7-0675-4ea6-88e2-4e6a3ed2a159")))); // Temporary
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("C3F899E2-6E27-4E35-82E8-2A72F1A437FB")))); // City
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("F35B6561-911A-4BFE-AE2C-85BC2B031920")))); // Line1
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("3CC0D44F-DE41-4C24-9990-1E24A35799DB")))); // Postcode
            
            if (user.Account.IsNHSCustomer)
            {
                columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("3ed4ae51-6695-40d3-b81f-dcf9ef0d842b")))); // EsrLocationId
            }

            var grid = new cGridNew(user.AccountID, user.EmployeeID, "gridWorkLocations", tables.GetTableByID(new Guid("2330AED0-18C0-4F69-AFF7-77BE9F041C92")), columns);
            grid.addFilter(fields.GetFieldByID(new Guid("0cb32ebc-aaa5-4b96-bb04-1de4e3985b70")), ConditionType.Equals, new object[] { employeeid }, null, ConditionJoiner.None);

            if (user.Account.IsNHSCustomer)
            {
                const string ShowEsrDetailsCellId = "EsrDetailsWork";

                grid.addTwoStateEventColumn(
                    ShowEsrDetailsCellId,
                    (cFieldColumn)grid.getColumnByName("ESRLocationID"),
                    0,
                    null,
                    "/static/icons/16/new-icons/magnifying_glass.png",
                    "javascript:showEsrDetailsModal(4,{value});",
                    "Show ESR Details",
                    "Show ESR Details",
                    "",
                    "",
                    "",
                    "");

                if (false)  // TODO: User Story 76513 - Change ESR work addresses editable/non editable feature to be controlled via registeredusers
                {
                    grid.InitialiseRow += (row, info) =>
                    {
                        var showEsrDetailsCell = row.Cells.FirstOrDefault(c => c.Column.ID == ShowEsrDetailsCellId);
                        if (showEsrDetailsCell == null)
                        {
                            return;
                        }

                        row.enabledeleting = "<a></a>".Equals(showEsrDetailsCell.Text, StringComparison.InvariantCultureIgnoreCase);

                    };
                }

            }

            grid.KeyField = "EmployeeWorkAddressId";
            grid.enableupdating = true;
            grid.enabledeleting = true;
            grid.EmptyText = "There are no work addresses defined for this employee";
            grid.getColumnByName("EmployeeWorkAddressId").hidden = true;
            grid.editlink = "javascript:addNewWorkLocation = false; editWorkLocation({EmployeeWorkAddressId});";
            grid.deletelink = "javascript:deleteWorkLocation({EmployeeWorkAddressId});";

            List<string> retVals = new List<string>();
            retVals.Add(grid.GridID);
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public static string[] createHomeLocationGrid(string contextKey)
        {
            int employeeid = 0;
            if (contextKey != "")
            {
                int.TryParse(contextKey, out employeeid);
            }

            CurrentUser user = cMisc.GetCurrentUser();
            var tables = new cTables(user.AccountID);
            var fields = new cFields(user.AccountID);

            var columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("C1E0C8F9-4A1E-4194-BD1D-148CF222D958")))); // EmployeeHomeAddressId
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("E1DF7EB9-B119-4A58-8485-FC901F3D5440")))); // StartDate
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("F5883185-4BA8-4EC7-A78C-EE114AFEC52F")))); // EndDate
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("E76A3C68-833A-4E6F-85B5-29C8EF357C50")))); // City
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("76AED3E5-33BC-4547-BD30-A5E8758AB653")))); // Line1
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("6A6F8977-2D38-4BFA-ADA3-A6BCA273F3ED")))); // Postcode
            columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("A40F4A64-EC97-4F12-B758-5B3CF5482DA2")))); // ESR Location ID

            var grid = new cGridNew(user.AccountID, user.EmployeeID, "gridHomeLocations", tables.GetTableByID(new Guid("F999A9A6-3F66-43D7-A8F7-6B92C1649825")), columns);
            grid.addFilter(fields.GetFieldByID(new Guid("730BE26D-FDF4-40EC-BB6E-2148AC1F674C")), ConditionType.Equals, new object[] { employeeid }, null, ConditionJoiner.None);
            grid.KeyField = "EmployeeHomeAddressId";
            grid.enableupdating = true;
            grid.enabledeleting = true;
            grid.EmptyText = "There are no home addresses defined for this employee";
            grid.getColumnByName("EmployeeHomeAddressId").hidden = true;
            grid.getColumnByName("ESRAddressID").hidden = true;
            grid.editlink = "javascript:addNewHomeLocation = false;editHomeLocation({EmployeeHomeAddressId}, '{ESRAddressID}');";
            grid.deletelink = "javascript:deleteHomeLocation({EmployeeHomeAddressId}, '{ESRAddressID}');";

            var retVals = new List<string> { grid.GridID };
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

        private string[] CreateCorporateCardGrid()
        {
            var columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(this._fields.GetFieldByID(new Guid("8300c392-05d6-4d72-b392-1c91f5c2e2f5"))));
            columns.Add(new cFieldColumn(this._fields.GetFieldByID(new Guid("5c4d2fc4-2e2e-4911-96e3-ac6d25f1c159"))));
            columns.Add(new cFieldColumn(this._fields.GetFieldByID(new Guid("5956859b-9463-4d06-a14f-1fe7048fb9f5"))));
            columns.Add(new cFieldColumn(this._fields.GetFieldByID(new Guid("363ba78c-1077-41bb-a9ed-e761cd50d58d"))));
            columns.Add(new cFieldColumn(this._fields.GetFieldByID(new Guid("3a14e819-108e-41a0-97a4-297bd684d346"))));

            var cardgrid = new cGridNew((int)ViewState["accountid"], (int)ViewState["adminemployeeid"], "gridCards", this._tables.GetTableByID(new Guid("9f3aa3ed-481d-499a-89b3-f1c8aefa61e5")), columns);
            cardgrid.addFilter(this._fields.GetFieldByID(new Guid("c93754a5-0945-4147-9cef-b3f51f4cc396")), ConditionType.Equals, new object[] { (int)ViewState["employeeid"] }, null, ConditionJoiner.None);
            cardgrid.getColumnByName("corporatecardid").hidden = true;
            ((cFieldColumn)cardgrid.getColumnByName("card_type")).addValueListItem((int)CorporateCardType.CreditCard, "Credit Card");
            ((cFieldColumn)cardgrid.getColumnByName("card_type")).addValueListItem((int)CorporateCardType.PurchaseCard, "Purchase Card");
            cardgrid.KeyField = "corporatecardid";
            cardgrid.enableupdating = true;
            cardgrid.enabledeleting = true;
            cardgrid.EmptyText = "There are no corporate cards defined for this employee";
            cardgrid.editlink = "javascript:editCorporateCard({corporatecardid});";
            cardgrid.deletelink = "javascript:deleteCorporateCard({corporatecardid});";

            var retVals = new List<string> { cardgrid.GridID };
            retVals.AddRange(cardgrid.generateGrid());
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public static string[] CreateItemRoleGrid(string contextKey)
        {
            int employeeid = 0;
            if (contextKey != "")
            {
                int.TryParse(contextKey, out employeeid);
            }

            CurrentUser user = cMisc.GetCurrentUser();
            var tables = new cTables(user.AccountID);
            var fields = new cFields(user.AccountID);

            var columns = new List<cNewGridColumn> 
            { 
                new cFieldColumn(fields.GetFieldByID(new Guid("8dda449d-56f2-4dc9-9d41-f3b874d09c22"))), 
                new cFieldColumn(fields.GetFieldByID(new Guid("54825039-9125-4705-b2d4-eb340d1d30de"))), 
                new cFieldColumn(fields.GetFieldByID(new Guid("34bfc6ab-b663-47da-95aa-8b23dc86dc82"))),
                new cFieldColumn(fields.GetFieldByID(new Guid("4B8E0B15-0F55-4E0E-81C1-612E6130DD8E"))),
                new cFieldColumn(fields.GetFieldByID(new Guid("77F08892-038A-48BC-9F65-C9DB412F588E")))
            };

            var clsitemgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridItemRoles", tables.GetTableByID(new Guid("8211E41F-710E-42AB-A2DF-1574FD003B31")), columns);
            clsitemgrid.addFilter(fields.GetFieldByID(new Guid("0CD532EA-7781-4831-AACC-AF6C1B32C6BB")), ConditionType.Equals, new object[] { employeeid }, null, ConditionJoiner.And);
            clsitemgrid.EnableSorting = true;
            clsitemgrid.EmptyText = "There are no items roles selected for this employee";
            clsitemgrid.getColumnByName("order").hidden = true;
            clsitemgrid.getColumnByName("itemroleid").hidden = true;
            clsitemgrid.enabledeleting = true;
            clsitemgrid.deletelink = "javascript:deleteItemRole({itemroleid}," + employeeid + ");";
            clsitemgrid.enableupdating = true;
            clsitemgrid.editlink = "javascript:editItemRole({itemroleid}," + employeeid + ");";
            clsitemgrid.KeyField = "itemroleid";
            clsitemgrid.enablepaging = false;

            var retVals = new List<string> { clsitemgrid.GridID };
            retVals.AddRange(clsitemgrid.generateGrid());
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public static string[] createNewItemRoleGrid(string contextKey)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var tables = new cTables(user.AccountID);
            var fields = new cFields(user.AccountID);

            var columns = new List<cNewGridColumn>
                                               {
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("f3016e05-1832-49d1-9d33-79ed893b4366"))), 
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("54825039-9125-4705-b2d4-eb340d1d30de")))
                                               };

            var clsitemgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridNewItemRoles", tables.GetTableByID(new Guid("db7d42fd-e1fa-4a42-84b4-e8b95c751bda")), columns)
                                       {
                                           EnableSelect = true, 
                                           enablepaging = false, 
                                           EmptyText = "There are no items roles selected for this employee", 
                                           KeyField = "itemroleid"
                                       };

            clsitemgrid.getColumnByName("itemroleid").hidden = true;


            var retVals = new List<string> { clsitemgrid.GridID };
            retVals.AddRange(clsitemgrid.generateGrid());
            return retVals.ToArray();
        }

        private static DataSet getAccessRoleDS(Employee emp, CurrentUser user)
        {
            var subAccounts = new cAccountSubAccounts(user.AccountID);
            var ds = new DataSet();
            var tbl = new DataTable();
            tbl.Columns.Add("accessRoleID", typeof(Int32));
            tbl.Columns.Add("roleName", typeof(String));
            tbl.Columns.Add("subAccountID", typeof(Int32));
            tbl.Columns.Add("description", typeof(String));

            if (emp != null)
            {
                foreach (KeyValuePair<int, cAccountSubAccount> sa in subAccounts.getSubAccountsCollection())
                {
                    cAccountSubAccount subaccount = sa.Value;
                    List<int> lstAccessRoles = emp.GetAccessRoles().GetBy(subaccount.SubAccountID);

                    if (lstAccessRoles.Count > 0)
                    {
                        if (lstAccessRoles != null)
                        {
                            foreach (int val in lstAccessRoles)
                            {
                                cAccessRole accessRole = user.AccessRoles.GetAccessRoleByID(val);
                                tbl.Rows.Add(new object[] { accessRole.RoleID, accessRole.RoleName, subaccount.SubAccountID, subaccount.Description });
                            }
                        }
                    }
                }
            }

            ds.Tables.Add(tbl);
            return ds;
        }

        public void createCostCentreGrid(cDepCostItem[] breakdown)
        {
            if (breakdown.Length > 0)
            {
                foreach (cDepCostItem item in breakdown)
                {
                    costcodebreakdown.AddCostCentreBreakdownRow(item.departmentid, item.costcodeid, item.projectcodeid, item.percentused);
                }
            }
            else
            {
                costcodebreakdown.AddCostCentreBreakdownRow(null, null, null, 100);
            }
        }

        [WebMethod(EnableSession = true)]
        public static string[] createNewAccessRoleGrid(string contextKey)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var tables = new cTables(user.AccountID);
            var fields = new cFields(user.AccountID);

            var columns = new List<cNewGridColumn>
                                               {
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("bdce49c3-eaf8-4070-91c6-959da3827db6"))), 
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("735a4159-090b-420d-80c4-57987422380c")))
                                               };

            var clsitemgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridNewAccessRoles", tables.GetTableByID(new Guid("12ded231-b220-4acb-a51d-896c52ff8979")), columns)
                                       {
                                           EmptyText = "There are no access roles selected for this employee",
                                           EnableSelect = true,
                                           KeyField = "roleID"
                                       };

            clsitemgrid.getColumnByName("roleID").hidden = true;
            var retVals = new List<string> { clsitemgrid.GridID };
            retVals.AddRange(clsitemgrid.generateGrid());
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public static string[] CreateAccessRoleGrid(string contextKey)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var tables = new cTables(user.AccountID);
            var fields = new cFields(user.AccountID);
            var employees = new cEmployees(user.AccountID);

            Employee emp = null;
            if (contextKey != "")
            {
                int employeeid = 0;
                int.TryParse(contextKey, out employeeid);
                emp = employees.GetEmployeeById(employeeid);
            }

            DataSet ds = getAccessRoleDS(emp, user);

            var columns = new List<cNewGridColumn>
                              {
                                  new cFieldColumn(
                                      fields.GetFieldByID(
                                          new Guid("008c4487-9634-4280-9f45-772cdaa7ea4d"))),
                                  new cFieldColumn(
                                      fields.GetFieldByID(
                                          new Guid("735a4159-090b-420d-80c4-57987422380c"))),
                                  new cFieldColumn(
                                      fields.GetFieldByID(
                                          new Guid("5AF302C6-40A7-425E-BF2E-D233D1131F57"))),
                                  new cFieldColumn(
                                      fields.GetFieldByID(
                                          new Guid("2D43703F-7FF2-43E2-BC4D-2BAE727A25F8")))
                              };
            var clsitemgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridAccessRoles", tables.GetTableByID(new Guid("f26c8337-98b8-45a3-adca-9549287a3610")), columns, ds)
                                  {
                                      enablepaging
                                          =
                                          true,
                                      EnableSorting
                                          =
                                          true,
                                      EmptyText
                                          =
                                          "There are no access roles selected for this employee"
                                  };
            clsitemgrid.getColumnByName("accessRoleID").hidden = true;
            clsitemgrid.getColumnByName("subAccountID").hidden = true;

            clsitemgrid.enabledeleting = true;
            clsitemgrid.deletelink = "javascript:deleteAccessRole({accessRoleID}, {subAccountID});";
            clsitemgrid.KeyField = "accessRoleID";

            var retVals = new List<string> { clsitemgrid.GridID };
            retVals.AddRange(clsitemgrid.generateGrid());
            return retVals.ToArray();
        }

        private string[] CreateCarGrid()
        {
            var clscargrid = new cGridNew((int)ViewState["accountid"], (int)ViewState["adminemployeeid"], "gridCars", this._employees.getCarGrid());
            clscargrid.addFilter(this._fields.GetFieldByID(new Guid("5ddbf0ef-fa06-4e7c-a45a-54e50e33307e")), ConditionType.Equals, new object[] { (int)ViewState["employeeid"] }, null, ConditionJoiner.None);
            clscargrid.enabledeleting = true;
            clscargrid.enableupdating = true;
            clscargrid.EnableSorting = true;
            clscargrid.enablepaging = true;
            clscargrid.KeyField = "carid";
            clscargrid.EmptyText = "There are currently no vehicles defined for this employee";
            clscargrid.editlink = "javascript:editCar({carid})";
            clscargrid.deletelink = "javascript:deleteCar({carid});";
            svcCars.AddVehicleTypes(ref clscargrid);
            
            clscargrid.getColumnByName("carid").hidden = true;

            var retVals = new List<string> { clscargrid.GridID };
            retVals.AddRange(clscargrid.generateGrid());
            return retVals.ToArray();
        }


        [WebMethod(EnableSession = true)]
        public static string[] getCarGrid(string contextKey)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var fields = new cFields(user.AccountID);
            var employees = new cEmployees(user.AccountID);

            int employeeid = Convert.ToInt32(contextKey);
            var clscargrid = new cGridNew(user.AccountID, user.EmployeeID, "gridCars", employees.getCarGrid());
            clscargrid.addFilter(fields.GetFieldByID(new Guid("5ddbf0ef-fa06-4e7c-a45a-54e50e33307e")), ConditionType.Equals, new object[] { employeeid }, null, ConditionJoiner.None);
            clscargrid.enabledeleting = true;
            clscargrid.enableupdating = true;
            clscargrid.EnableSorting = true;
            clscargrid.enablepaging = true;
            clscargrid.KeyField = "carid";
            clscargrid.EmptyText = "There are currently no vehicles defined for this employee";
            clscargrid.editlink = "javascript:editCar({carid})";
            clscargrid.deletelink = "javascript:deleteCar({carid});";
            svcCars.AddVehicleTypes(ref clscargrid);
            clscargrid.getColumnByName("carid").hidden = true;

            var retVals = new List<string> { clscargrid.GridID };
            retVals.AddRange(clscargrid.generateGrid());
            return retVals.ToArray();
        }

        #region Corporate Cards
        [WebMethod(EnableSession = true)]
        public static cEmployeeCorporateCard getCorporateCard(int employeeid, int corporatecardid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var empCards = new cEmployeeCorporateCards(user.AccountID);
            return empCards.GetCorporateCardByID(employeeid, corporatecardid);

        }
        [WebMethod(EnableSession = true)]
        public static string[] getCorporateCardGrid(string contextKey)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var tables = new cTables(user.AccountID);
            var fields = new cFields(user.AccountID);
            int employeeid = Convert.ToInt32(contextKey);
            var columns = new List<cNewGridColumn>
                                               {
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("8300c392-05d6-4d72-b392-1c91f5c2e2f5"))), 
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("5c4d2fc4-2e2e-4911-96e3-ac6d25f1c159"))), 
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("5956859b-9463-4d06-a14f-1fe7048fb9f5"))), 
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("363ba78c-1077-41bb-a9ed-e761cd50d58d"))), 
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("3a14e819-108e-41a0-97a4-297bd684d346")))
                                               };

            var cardgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridCards", tables.GetTableByID(new Guid("9f3aa3ed-481d-499a-89b3-f1c8aefa61e5")), columns);
            cardgrid.addFilter(fields.GetFieldByID(new Guid("c93754a5-0945-4147-9cef-b3f51f4cc396")), ConditionType.Equals, new object[] { employeeid }, null, ConditionJoiner.None);
            cardgrid.getColumnByName("corporatecardid").hidden = true;
            ((cFieldColumn)cardgrid.getColumnByName("card_type")).addValueListItem((int)CorporateCardType.CreditCard, "Credit Card");
            ((cFieldColumn)cardgrid.getColumnByName("card_type")).addValueListItem((int)CorporateCardType.PurchaseCard, "Purchase Card");
            cardgrid.KeyField = "corporatecardid";
            cardgrid.enableupdating = true;
            cardgrid.enabledeleting = true;
            cardgrid.EnableSorting = true;
            cardgrid.enablepaging = true;
            cardgrid.editlink = "javascript:editCorporateCard({corporatecardid});";
            cardgrid.deletelink = "javascript:deleteCorporateCard({corporatecardid});";

            var retVals = new List<string> { cardgrid.GridID };
            retVals.AddRange(cardgrid.generateGrid());
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public static int saveCorporateCard(int employeeid, int corporatecardid, int cardproviderid, string cardnumber, bool active)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clsEmployeeCards = new cEmployeeCorporateCards(user.AccountID);
            var clsproviders = new CardProviders();
            cCardProvider provider = clsproviders.getProviderByID(cardproviderid);

            cEmployeeCorporateCard card;
            if (corporatecardid > 0)
            {
                cEmployeeCorporateCard oldcard = clsEmployeeCards.GetCorporateCardByID(employeeid, corporatecardid);
                card = new cEmployeeCorporateCard(corporatecardid, oldcard.employeeid, provider, cardnumber, active, oldcard.CreatedOn, oldcard.CreatedBy, DateTime.Now, user.EmployeeID);
            }
            else
            {
                card = new cEmployeeCorporateCard(0, employeeid, provider, cardnumber, active, DateTime.Now, user.EmployeeID, null, null);
            }
            return clsEmployeeCards.SaveCorporateCard(card);
        }

        [WebMethod(EnableSession = true)]
        public static bool deleteCorporateCard(int corporatecardid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clsCards = new cEmployeeCorporateCards(user.AccountID);
            clsCards.DeleteCorporateCard(corporatecardid);
            return true;
        }
        #endregion

        #region pool cars
        private string[] CreatePoolCarGrid()
        {
            var columns = new List<cNewGridColumn>
                                               {
                                                new cFieldColumn(this._fields.GetFieldByID(new Guid("BA81207E-1DE0-4A3E-AC30-DEF3C9041778"))),
                                                new cFieldColumn(this._fields.GetFieldByID(new Guid("df695804-d4d7-4552-a396-9a5ef81491d3"))),
                                                new cFieldColumn(this._fields.GetFieldByID(new Guid("b7961f43-e439-4835-9709-396fff9bbd0c"))),
                                                new cFieldColumn(this._fields.GetFieldByID(new Guid("99a078d9-f82c-4474-bdde-6701d4bd51ea"))),
                                                new cFieldColumn(this._fields.GetFieldByID(new Guid("156ccca7-1f5c-45be-920c-5e5c199ee81a"))),
                                                new cFieldColumn(this._fields.GetFieldByID(new Guid("d226c1bd-ecc3-4f37-a5fe-58638b1bd66c"))),
                                                new cFieldColumn(this._fields.GetFieldByID(new Guid("2ab21296-77ee-4b3d-807c-56edf936613d"))),
                                                new cFieldColumn(this._fields.GetFieldByID(new Guid("24172542-3e15-4fca-b4f5-d7ffef9eed4e"))),
                                                new cFieldColumn(this._fields.GetFieldByID(new Guid("dad8f087-497b-4a83-ab40-6b5b816911eb")))
                                               };

            var clscargrid = new cGridNew(this._user.AccountID, (int)ViewState["adminemployeeid"], "gridPoolCars", this._tables.GetTableByID(new Guid("4d36cfab-1e35-44b8-9546-a5433e0517ec")), columns);
            clscargrid.addFilter(this._fields.GetFieldByID(new Guid("88c058ed-6d1c-4e8c-aab1-95d85115178a")), ConditionType.Equals, new object[] { (int)ViewState["employeeid"] }, null, ConditionJoiner.None);
            clscargrid.enabledeleting = true;
            clscargrid.EnableSorting = true;
            clscargrid.enablepaging = true;
            clscargrid.KeyField = "carid";
            clscargrid.EmptyText = "There are currently no pool vehicles defined for this employee";
            clscargrid.deletelink = "javascript:deletePoolCar({carid});";
            svcCars.AddVehicleTypes(ref clscargrid);

            clscargrid.getColumnByName("carid").hidden = true;

            var retVals = new List<string> { clscargrid.GridID };
            retVals.AddRange(clscargrid.generateGrid());
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public static string[] getPoolCarGrid(string contextKey)
        {
            int employeeid = Convert.ToInt32(contextKey);
            CurrentUser user = cMisc.GetCurrentUser();
            var tables = new cTables(user.AccountID);
            var fields = new cFields(user.AccountID);
            var columns = new List<cNewGridColumn>
                                               {
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("BA81207E-1DE0-4A3E-AC30-DEF3C9041778"))),
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("df695804-d4d7-4552-a396-9a5ef81491d3"))), 
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("b7961f43-e439-4835-9709-396fff9bbd0c"))), 
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("99a078d9-f82c-4474-bdde-6701d4bd51ea"))), 
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("156ccca7-1f5c-45be-920c-5e5c199ee81a"))), 
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("d226c1bd-ecc3-4f37-a5fe-58638b1bd66c"))), 
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("2ab21296-77ee-4b3d-807c-56edf936613d"))), 
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("24172542-3e15-4fca-b4f5-d7ffef9eed4e"))), 
                                                   new cFieldColumn(fields.GetFieldByID(new Guid("dad8f087-497b-4a83-ab40-6b5b816911eb")))
                                               };

            var clscargrid = new cGridNew(user.AccountID, user.EmployeeID, "gridPoolCars", tables.GetTableByID(new Guid("4d36cfab-1e35-44b8-9546-a5433e0517ec")), columns);
            clscargrid.addFilter(fields.GetFieldByID(new Guid("88c058ed-6d1c-4e8c-aab1-95d85115178a")), ConditionType.Equals, new object[] { employeeid }, null, ConditionJoiner.None);
            clscargrid.enabledeleting = true;
            clscargrid.EnableSorting = true;
            clscargrid.enablepaging = true;
            clscargrid.KeyField = "carid";
            clscargrid.EmptyText = "There are currently no pool vehicles defined for this employee";
            clscargrid.deletelink = "javascript:deletePoolCar({carid});";
            svcCars.AddVehicleTypes(ref clscargrid);
            clscargrid.getColumnByName("carid").hidden = true;

            var retVals = new List<string> { clscargrid.GridID };
            retVals.AddRange(clscargrid.generateGrid());
            return retVals.ToArray();
        }
        #endregion

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



        }
        #endregion

        [WebMethod]
        public static bool saveWorkLocation(int employeelocationid, int employeeid, int locationid, DateTime? startdate, DateTime? enddate, bool active, bool temporary, bool primaryrotational)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);
            Employee employee = employees.GetEmployeeById(employeeid);
            cEmployeeWorkLocation location;
            if (employeelocationid > 0)
            {
                cEmployeeWorkLocation oldlocation = employee.GetWorkAddresses()[employeelocationid];
                location = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, oldlocation.CreatedOn, oldlocation.CreatedBy, DateTime.Now, user.EmployeeID, oldlocation.EsrAssignmentLocationId, primaryrotational);
            }
            else
            {
                location = new cEmployeeWorkLocation(employeelocationid, employeeid, locationid, startdate, enddate, active, temporary, DateTime.Now, user.EmployeeID, null, null, null, primaryrotational);
            }

            employee.GetWorkAddresses().Add(location, user);

            new NotificationHelper(employee).ExcessMileage();

            return true;
        }

        [WebMethod(EnableSession = true)]
        public static object[] getWorkLocation(int employeeid, int locationid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);
            var workArr = new object[3];
            Employee employee = employees.GetEmployeeById(employeeid);
            cEmployeeWorkLocation employeeWorkLocation = employee.GetWorkAddresses().GetBy(locationid);
            workArr[0] = employeeWorkLocation;
            Address address = Address.Get(user.AccountID, employeeWorkLocation.LocationID);
            workArr[1] = employeeWorkLocation.LocationID;
            workArr[2] = address.FriendlyName;
            return workArr;
        }

        [WebMethod(EnableSession = true)]
        public static bool deleteWorkLocation(int locationid, int employeeID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);
            Employee employee = employees.GetEmployeeById(employeeID);
            employee.GetWorkAddresses().Remove(locationid, user);

            new NotificationHelper(employee).ExcessMileage();

            return true;
        }

        [WebMethod(EnableSession = true)]
        public static bool savePoolCar(int employeeid, int carid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clsPoolCars = new cPoolCars(user.AccountID);
            clsPoolCars.AddPoolCarUser(carid, employeeid);
            return true;
        }

        [WebMethod(EnableSession = true)]
        public static bool deletePoolCar(int employeeid, int carid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clsPoolCars = new cPoolCars(user.AccountID);
            clsPoolCars.DeleteUserFromPoolCar(employeeid, carid);
            return true;
        }

        [WebMethod(EnableSession = true)]
        public static int saveEmployee(int employeeid, string username, string title, string firstname, string middlenames, string surname, string maidenname, string email, 
            string extension, string pagerno, string mobileno, string emailhome, string telno, string faxno, string creditaccount, string payroll, string position, string ninumber,
            DateTime? hiredate, DateTime? leavedate, int homecountry, int currency, int linemanager, int signoff, int signoffcc, int signoffpc, int advancesgroup, string accountholder, 
            string accountnumber, string accounttype, string sortcode, string reference, string gender, DateTime? dateofbirth, int startmiles, DateTime? startmilesdate,
            int? localeID, int? NHSTrustID, string employeenumber, string preferredname, string nhsuniqueid, List<int> emailNotifications,
            object[] codebreakdown, List<object> udfs, bool SendPasswordKey, bool sendWelcomeEmail, int defaultSubAccountId, bool SendEmailCheck, bool NewEmployee, 
            cValidatorWebMethodError[] validatorErrors, bool activating, int? authoriserLevelId, double excessMileage)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);

            if (user.Account.IsNHSCustomer == false)
            {
                NHSTrustID = null;
            }

            // check the validator web method errors to see if signoffs need checking
            var clsGroups = new cGroups(user.AccountID);
            if (employeeid > 0 && validatorErrors.GetLength(0) > 0)
            {
                foreach (cValidatorWebMethodError e in validatorErrors)
                {
                    if (e.ControlToValidate.EndsWith("cmbsignoffs"))
                    {
                        if (clsGroups.EmployeeInSignoffGroup(signoff, employeeid))
                        {
                            return -3;
                        }
                    }
                    else if (e.ControlToValidate.EndsWith("cmbsignoffcc"))
                    {
                        if (clsGroups.EmployeeInSignoffGroup(signoffcc, employeeid))
                        {
                            return -4;
                        }
                    }
                    else if (e.ControlToValidate.EndsWith("cmbsignoffpc"))
                    {
                        if (clsGroups.EmployeeInSignoffGroup(signoffpc, employeeid))
                        {
                            return -5;
                        }
                    }
                }
            }

            Employee employee;

            var costcodes = new cCcbItemArray(codebreakdown);
            var breakdown = new cDepCostItem[costcodes.itemArray.Count];
            foreach (cCcbItem item in costcodes.itemArray)
            {
                int d = (item.departmentID == null) ? 0 : item.departmentID.DepartmentId;
                int c = (item.costCodeID == null) ? 0 : item.costCodeID.CostcodeId;
                int p = (item.projectCodeID == null) ? 0 : item.projectCodeID.projectcodeid;
                breakdown[costcodes.itemArray.IndexOf(item)] = new cDepCostItem(d, c, p, item.percentageSplit);
            }

            var userdefined = new SortedList<int, object>();

            foreach (object o in udfs)
            {
                userdefined.Add(Convert.ToInt32(((object[])o)[0]), ((object[])o)[1]);
            }

            if (employeeid > 0)
            {
                Employee oldemp = employees.GetEmployeeById(employeeid);
                employee = new Employee(user.AccountID, employeeid, username, oldemp.Password, email, title, firstname, middlenames, maidenname, surname, oldemp.Active, 
                    oldemp.Verified, oldemp.Archived, oldemp.Locked, oldemp.LogonCount, oldemp.LogonRetryCount, oldemp.CreatedOn, oldemp.CreatedBy, DateTime.UtcNow,
                    user.EmployeeID, new BankAccount(accountholder, accountnumber, accounttype, sortcode, reference),
                        signoff, extension, mobileno, pagerno, faxno, emailhome, linemanager, advancesgroup, preferredname,
                        gender, dateofbirth, hiredate, leavedate, payroll, position, telno, creditaccount, oldemp.CreationMethod, oldemp.PasswordMethod, oldemp.FirstLogon, oldemp.AdminOverride,
                        defaultSubAccountId, currency, homecountry, signoffcc, signoffpc, oldemp.HasCustomisedAddItems, localeID, NHSTrustID, ninumber, employeenumber, nhsuniqueid, 
                        oldemp.EsrPersonID, oldemp.EsrEffectiveStartDate, oldemp.EsrEffectiveEndDate, oldemp.CurrentClaimNumber, oldemp.LastChange, oldemp.CurrentReferenceNumber,
                       startmiles, startmilesdate, false, null, 0, authoriserLevelId, oldemp.NotifyClaimUnsubmission, null, null, null, null, null,null,null,null,null,null,null,null,null, excessMileage);
                if (employee.EmailAddress != oldemp.EmailAddress)
                {
                    Notifications.InvalidateCache(user.AccountID);
                }
            }
            else
            {
                employee = new Employee(user.AccountID, employeeid, username, string.Empty, email, title, firstname, middlenames, maidenname, surname, true, true, false, false, 0, 0, DateTime.UtcNow, user.EmployeeID, null, null, new BankAccount(accountholder, accountnumber, accounttype, sortcode, reference), 
                     signoff, extension, mobileno, pagerno, faxno, emailhome, linemanager,
                    advancesgroup, preferredname, gender, dateofbirth, hiredate, leavedate, payroll, position, telno, creditaccount, CreationMethod.Manually, 
                    PasswordEncryptionMethod.SaltedHash, true, false, defaultSubAccountId, currency, homecountry, signoffcc, signoffpc, false, localeID, NHSTrustID,
                     ninumber, employeenumber, nhsuniqueid, null, null, null, 1, DateTime.UtcNow, 1, startmiles, startmilesdate, false, null, 0, authoriserLevelId, false, null, null, null, null, null, null, null, null, null, null, null, null, null, excessMileage);
            }

            employeeid = employees.SaveEmployee(employee, breakdown, emailNotifications, userdefined);
          
            if (employeeid > 0)
            {
                if (SendEmailCheck)
                {
                    if (SendPasswordKey && (NewEmployee || (activating && !string.IsNullOrWhiteSpace(email))))
                    {
                        employees.SendPasswordKey(employeeid, cEmployees.PasswordKeyType.NewEmployee, null, user.CurrentActiveModule);
                    }

                    if (sendWelcomeEmail && (NewEmployee || (activating && !string.IsNullOrWhiteSpace(email))))
                    {
                        var subaccs = new cAccountSubAccounts(user.AccountID);
                        cAccountProperties properties = subaccs.getSubAccountById(subaccs.getFirstSubAccount().SubAccountID).SubAccountProperties;
                        int senderId = properties.MainAdministrator > 0 ? properties.MainAdministrator : user.EmployeeID;
                                       
                        employees.SendWelcomeEmail(senderId, employeeid, user, activating);
                    }
                }

                if (activating)
                {
                    employees.Activate(employeeid);
                    new cAuditLog().editRecord(employee.EmployeeID, "Account activated for "+ username, SpendManagementElement.Employees, new Guid("B8D81E67-51C2-483A-8015-606A3DBDA0A4"), "0", "1");
                }
            }

            return employeeid;
        }

        [WebMethod(EnableSession = true)]
        public static bool saveAccessRoles(int employeeid, List<int> roles, int subaccountid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
         
            if (user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Employees, false)  
                || user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Employees, false))     
            {       
                var employees = new cEmployees(user.AccountID);
                Employee employee = employees.GetEmployeeById(employeeid);
                employee.GetAccessRoles().Add(roles, subaccountid, user);
                return true;
            }

            return false;
        }

        [WebMethod(EnableSession = true)]
        public static bool saveItemRoles(int employeeid, List<EmployeeItemRole> roles)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);
            Employee employee = employees.GetEmployeeById(employeeid);
            employee.GetItemRoles().Add(roles, user);
            return true;
        }

        [WebMethod(EnableSession = true)]
        public static int deleteAccessRole(int employeeid, int roleid, int subaccountid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);
            Employee employee = employees.GetEmployeeById(employeeid);
            return employee.GetAccessRoles().Remove(roleid, subaccountid, user);
        }

        [WebMethod(EnableSession = true)]
        public static bool deleteItemRole(int employeeid, int roleid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);
            Employee employee = employees.GetEmployeeById(employeeid);
            employee.GetItemRoles().Remove(roleid, user);
            return true;
        }

        [WebMethod(EnableSession = true)]
        public static bool saveHomeLocation(int employeelocationid, int employeeid, int locationid, DateTime? startdate, DateTime? enddate)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);
            Employee employee = employees.GetEmployeeById(employeeid);
            cEmployeeHomeLocation location;

            if (employeelocationid > 0)
            {
                cEmployeeHomeLocation oldlocation = employee.GetHomeAddresses().GetBy(employeelocationid);
                location = new cEmployeeHomeLocation(employeelocationid, employeeid, locationid, startdate, enddate, oldlocation.CreatedOn, oldlocation.CreatedBy, DateTime.Now, user.EmployeeID);
            }
            else
            {
                location = new cEmployeeHomeLocation(employeelocationid, employeeid, locationid, startdate, enddate, DateTime.Now, user.EmployeeID, null, null);
            }

            new NotificationHelper(employee).ExcessMileage();

            employee.GetHomeAddresses().Add(location, user);

            return true;
        }

        [WebMethod(EnableSession = true)]
        public static object[] getHomeLocation(int employeeid, int locationid, int esrId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);
            var homeArr = new object[3];
            Employee employee = employees.GetEmployeeById(employeeid);
            cEmployeeHomeLocation empHomeLoc = employee.GetHomeAddresses().GetBy(locationid, esrId);
            homeArr[0] = empHomeLoc;
            Address address = Address.Get(user.AccountID, empHomeLoc.LocationID);
            homeArr[1] = empHomeLoc.LocationID;
            homeArr[2] = address.FriendlyName;
            return homeArr;
        }

        [WebMethod(EnableSession = true)]
        public static bool deleteHomeLocation(int locationid, int employeeID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);
            Employee employee = employees.GetEmployeeById(employeeID);
            employee.GetHomeAddresses().Remove(locationid, user);

            new NotificationHelper(employee).ExcessMileage();

            return true;
        }

        [WebMethod(EnableSession = true)]
        public static int saveESRAssignment(int esrassignmentid, int employeeid, string assignmentnumber, bool active, bool primary, DateTime startdate, DateTime? enddate, string signOffOwner)
        {
            var user = cMisc.GetCurrentUser();
            var assignments = new cESRAssignments(user.AccountID, employeeid);
            var oldAssignment = assignments.getAssignmentById(esrassignmentid);
            cESRAssignment newAssignment;
            if (oldAssignment == null)
            {
                newAssignment = new cESRAssignment(esrassignmentid, 0, assignmentnumber, startdate, enddate, ESRAssignmentStatus.NotSpecified, String.Empty,
                    String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, false, String.Empty,
                    String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, primary, (primary ? "Yes" : "No"), 0, String.Empty, 0, 0,
                    String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty,
                    String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, null, null, null, active,
                    Ownership.Parse(user.AccountID, user.CurrentSubAccountId, signOffOwner), DateTime.Now, user.EmployeeID, null, null);
            }
            else
            {
                newAssignment = new cESRAssignment(oldAssignment.assignmentid, esrassignmentid, assignmentnumber, startdate, enddate,
                    oldAssignment.assignmentstatus, oldAssignment.payrollpaytype, oldAssignment.payrollname, oldAssignment.payrollpaytype,
                    oldAssignment.assignmentaddress1, oldAssignment.assignmentaddress2, oldAssignment.assignmentaddresstown, oldAssignment.assignmentaddresscounty,
                    oldAssignment.assignmentaddresspostcode, oldAssignment.assignmentaddresscountry, oldAssignment.supervisorflag,
                    oldAssignment.supervisorassignmentnumber, oldAssignment.supervisoremployementnumber, oldAssignment.supervisorfullname,
                    oldAssignment.accrualplan, oldAssignment.employeecategory, oldAssignment.assignmentcategory, primary, (primary ? "Yes" : "No"),
                    oldAssignment.normalhours, oldAssignment.normalhoursfrequency, oldAssignment.gradecontracthours, oldAssignment.noofsessions,
                    oldAssignment.sessionsfrequency, oldAssignment.workpatterndetails, oldAssignment.workpatternstartday, oldAssignment.flexibleworkingpattern,
                    oldAssignment.availabilityschedule, oldAssignment.organisation, oldAssignment.legalentity, oldAssignment.positionname, oldAssignment.jobname,
                    oldAssignment.occupationcode, oldAssignment.assignmentlocation, oldAssignment.grade, oldAssignment.jobname, oldAssignment.group,
                    oldAssignment.tandaflag, oldAssignment.nightworkeroptout, oldAssignment.projectedhiredate, oldAssignment.vacancyid,
                    oldAssignment.esrLocationId, active, Ownership.Parse(user.AccountID, user.CurrentSubAccountId, signOffOwner),
                    oldAssignment.CreatedOn, oldAssignment.CreatedBy, DateTime.Now, user.EmployeeID);
            }
            return assignments.saveESRAssignment(newAssignment);
        }

        [WebMethod(EnableSession = true)]
        public static bool deleteESRAssignment(int employeeID, int id)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clsassignments = new cESRAssignments(user.AccountID, employeeID);
            clsassignments.deleteESRAssignment(id);
            return true;
        }

        [WebMethod(EnableSession = true)]
        public static cESRAssignment getESRAssignment(int employeeID, int id)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clsassignments = new cESRAssignments(user.AccountID, employeeID);
            return clsassignments.getAssignmentById(id);
        }

        [WebMethod(EnableSession = true)]
        public static object getCurrentAssignmentSupervisor(string supervisorAssignmentNumber)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var employees = new cEmployees(user.AccountID);
            Employee supervisorEmployee = employees.GetEmployeeById(employees.getEmployeeidByAssignmentNumber(user.AccountID, supervisorAssignmentNumber));

            return supervisorEmployee == null ? null : new { EmployeeId = supervisorEmployee.EmployeeID, EmployeeName = string.Format("{0} ({1})", supervisorEmployee.FullName, supervisorEmployee.Username) };
        }

        /// <summary>
        /// Creates all of the required Page Options for the employee page
        /// </summary>
        /// <param name="useMobileDevices">Use Mobile Devices</param>
        /// <param name="usingExpenses">Using Expenses</param>
        /// <returns>Returns the JavaScript links required for Page Options</returns>
        private static string SetPageOptions(bool useMobileDevices, bool usingExpenses)
        {
            var sb = new StringBuilder();
            sb.Append("<a href=\"javascript:changePage('General');\" id=\"lnkGeneral\" class=\"selectedPage\">Employee Details</a>");
            sb.Append("<a href=\"javascript:changePage('Cars');\" id=\"lnkCars\">Vehicles</a>");
            if (usingExpenses)
            {
                sb.Append("<a href=\"javascript:changePage('PoolCars');\" id=\"lnkPoolCars\">Pool Vehicles</a>");
                sb.Append("<a href=\"javascript:changePage('CorporateCards');\" id=\"lnkCorporateCards\">Corporate Cards</a>");
            }
            sb.Append("<a href=\"javascript:changePage('WorkLocations');\" id=\"lnkWorkLocations\">Work Addresses</a>");
            sb.Append("<a href=\"javascript:changePage('HomeLocations');\" id=\"lnkHomeLocations\">Home Addresses</a>");
            if (useMobileDevices)
            {
                sb.Append("<a href=\"javascript:changePage('MobileDevices');\" id=\"lnkMobileDevices\">Mobile Devices</a>");
            }

            return sb.ToString();
        }

        [WebMethod(EnableSession = true)]
        public static bool ValidateSignoff(int groupID, int employeeID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var groups = new cGroups(user.AccountID);

            // check to see if the user is in the signoff group
            bool employeeIsInSignoff = groups.EmployeeInSignoffGroup(groupID, employeeID);

            // if they're in the group it has failed validation
            return !employeeIsInSignoff;
        }

        /// <summary>
        /// Binds the jQuery autocomplete to the sign-off owner text box
        /// </summary>
        /// <param name="user">Current User object</param>
        private void BindSignOffOwnerAutoComplete(CurrentUser user)
        {
            // bind the jQuery auto complete to the field
            List<object> acParams = AutoComplete.getAutoCompleteQueryParams("signoffentities");
            this.ClientScript.RegisterStartupScript(this.GetType(), "autocompleteBind", AutoComplete.generateScriptRegisterBlock(new List<string>
            {
                AutoComplete.createAutoCompleteBindString("txtSignOffOwner", 15, new cTables(user.AccountID).GetTableByName("signoffentities").TableID, (Guid)acParams[0], (List<Guid>)acParams[1], keyFieldIsString: true)
            }), true);
        }
        /// <summary>
        /// Populate Drop DownList item  from AuthoriserLevelDetails table
        /// </summary>
        private void PopulateDropDownListAuthoriserLevel()
        {
            cmbAuthoriserLevel.Items.Clear();
            List<ListItem> lstAuthoriserLevelDetails = _employees.PopulateDropDownListAuthoriserLevel();
            var itemNone = new ListItem("[None]", "0");
            lstAuthoriserLevelDetails.Insert(0, itemNone);
            cmbAuthoriserLevel.Items.AddRange(lstAuthoriserLevelDetails.ToArray());
            cmbAuthoriserLevel.Enabled = true;
        }

        /// <summary>
        /// Populates the email notification panel
        /// </summary>
        public void GenerateEmailNotifications(int employeeid, Employee reqemp)
        {
            var Notifications = new Notifications(this._user.AccountID);
            Label lblLabel;
            Label lblInputs;
            Label lblIcons;
            Label lblTooltips;
            Label lblValidators;
            CheckBox chkBox;

            #region Standard Notifications
            SortedList<int, Notification> lstStandardNotifications = Notifications.EmailNotificationsByCustomerType(CustomerType.Standard);

            bool secondColumn = false;

            var pnlTwoColumn = new Panel();

            foreach (Notification notification in lstStandardNotifications.Values)
            {
                if (!notification.Enabled || (this._user.CurrentActiveModule != Modules.expenses &&
                                              notification.EmailNotificationType ==
                                              EmailNotificationType.ExcessMileage)) continue;
                if (secondColumn)
                {
                    secondColumn = false;
                }
                else
                {
                    pnlTwoColumn = new Panel { CssClass = "twocolumn" };
                    this.pnlEmailNotifications.Controls.Add(pnlTwoColumn);
                    secondColumn = true;
                }

                lblLabel = new Label { Text = notification.Name };
                pnlTwoColumn.Controls.Add(lblLabel);

                lblInputs = new Label { CssClass = "inputs" };
                chkBox = new CheckBox { ID = "emailNotification_" + notification.EmailNotificationID };

                if (employeeid > 0 && reqemp.GetEmailNotificationList().Contains(notification.EmailNotificationID))
                {
                    chkBox.Checked = true;
                }

                lblLabel.AssociatedControlID = chkBox.ID;
                lblInputs.Controls.Add(chkBox);

                pnlTwoColumn.Controls.Add(lblInputs);

                lblIcons = new Label { Text = "&nbsp;", CssClass = "inputicon" };
                pnlTwoColumn.Controls.Add(lblIcons);
                string notificationTooltip = "&nbsp;";
                switch (notification.EmailNotificationType)
                {
                    case EmailNotificationType.AuditLogCleared:
                        notificationTooltip = "<img onmouseover=" + "" + "SEL.Tooltip.Show('5849FB1E-6602-49AF-AE01-C838507676F8','sm',this);" + "" + " src='../images/icons/16/plain/tooltip.png' class='tooltipicon'>";
                        break;
                    case EmailNotificationType.ExcessMileage:
                        notificationTooltip = "<img onmouseover=" + "" + "SEL.Tooltip.Show('4AE95AE1-D44F-4B3A-8067-89E26ECE447A','ex',this);" + "" + " src='../images/icons/16/plain/tooltip.png' class='tooltipicon'>";
                        break;
                    default:
                        break;
                }
                lblTooltips = new Label { Text = notificationTooltip, CssClass = "inputtooltipfield" };
                pnlTwoColumn.Controls.Add(lblTooltips);
                lblValidators = new Label { Text = "&nbsp;", CssClass = "inputvalidatorfield" };
                pnlTwoColumn.Controls.Add(lblValidators);
            }
            #endregion Standard Notifications

            #region NHS Notifications
            if (this._user.Account.IsNHSCustomer)
            {
                SortedList<int, Notification> lstEmailNotifications = Notifications.EmailNotificationsByCustomerType(CustomerType.NHS);
                pnlTwoColumn = new Panel();
                pnlTwoColumn.CssClass = "twocolumn";

                secondColumn = false;

                pnlESREmailNotifications.Controls.Add(pnlTwoColumn);

                foreach (Notification notification in lstEmailNotifications.Values)
                {
                    if (!notification.Enabled) continue;
                    if (secondColumn)
                    {
                        secondColumn = false;
                    }
                    else
                    {
                        secondColumn = true;
                        pnlTwoColumn = new Panel();
                        pnlTwoColumn.CssClass = "twocolumn";
                        this.pnlESREmailNotifications.Controls.Add(pnlTwoColumn);
                    }

                    lblLabel = new Label();
                    lblLabel.Text = notification.Name;
                    pnlTwoColumn.Controls.Add(lblLabel);

                    lblInputs = new Label();
                    lblInputs.CssClass = "inputs";
                    chkBox = new CheckBox();
                    chkBox.ID = "emailNotification_" + notification.EmailNotificationID.ToString();


                    if (employeeid > 0 && reqemp.GetEmailNotificationList().Contains(notification.EmailNotificationID) == true)
                    {
                        chkBox.Checked = true;
                    }

                    lblLabel.AssociatedControlID = chkBox.ID;
                    lblInputs.Controls.Add(chkBox);

                    pnlTwoColumn.Controls.Add(lblInputs);

                    lblIcons = new Label();
                    lblIcons.Text = "&nbsp;";
                    lblIcons.CssClass = "inputicon";
                    pnlTwoColumn.Controls.Add(lblIcons);
                    lblTooltips = new Label();
                    lblTooltips.Text = "&nbsp;";
                    lblTooltips.CssClass = "inputtooltipfield";
                    pnlTwoColumn.Controls.Add(lblTooltips);
                    lblValidators = new Label();
                    lblValidators.Text = "&nbsp;";
                    lblValidators.CssClass = "inputvalidatorfield";
                    pnlTwoColumn.Controls.Add(lblValidators);
                }
            }
            else
            {
                pnlESREmailNotificatonsSectionTitle.Visible = false;
                pnlESREmailNotifications.Visible = false;
            }
            #endregion NHSRegions

            #endregion EmailNotifications
        }
    }
}
