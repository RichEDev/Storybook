using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

using AjaxControlToolkit;

using SpendManagementLibrary;
using SpendManagementLibrary.Addresses;
using SpendManagementLibrary.Employees;
using SpendManagementLibrary.Enumerators.Expedite;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Hotels;
using SpendManagementLibrary.Mobile;

using Spend_Management;
using Spend_Management.expenses.code;

using Action = Spend_Management.Action;
using BankAccount = SpendManagementLibrary.Account.BankAccount;
using ExpenseItem = SpendManagementLibrary.Mobile.ExpenseItem;
using SpendManagementLibrary.Flags;
using Spend_Management.shared.code;
using System.Web;
using System.Web.Script.Serialization;
using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.ProjectCodes;
using Common.Logging;
using Common.Logging.Log4Net;
using expenses.admin;
using expenses.Bootstrap;

using Newtonsoft.Json;

using SpendManagementLibrary.Employees.DutyOfCare;

public partial class aeexpense : System.Web.UI.Page
{
    /// <summary>
    /// The account's postcode anywhere key (for the address widget)
    /// </summary>
    public string PostcodeAnywhereKey { get; set; }

    /// <summary>
    /// Are claimants allowed to create new manual addresses
    /// </summary>
    public bool AllowClaimantAddManualAddresses { get; set; }

    /// <summary>
    /// Are claimants allowed to save new organisations
    /// </summary>
    public bool AllowClaimantAddOrganisations { get; set; }

    /// <summary>
    /// The name by which organisations are known on this account
    /// </summary>
    public string OrganisationLabel { get; set; }

    /// <summary>
    /// The shortcut keyword for quick entry of the user's home address, usually "home"
    /// </summary>
    public string HomeAddressKeyword { get; set; }

    /// <summary>
    /// The shortcut keyword for quick entry of the user's work address, usually "office"
    /// </summary>
    public string WorkAddressKeyword { get; set; }

    /// <summary>
    /// Whether to force users to enter an address name before they can use the address, this option is only available to Address+ Standard (TeleAtlas) accounts
    /// </summary>
    public bool ForceAddressNameEntry { get; set; }

    /// <summary>
    /// The message presented to users when they are prompted to enter an address name
    /// </summary>
    public string AddressNameEntryMessage { get; set; }

    /// <summary>
    /// The current action for the page.
    /// </summary>
    public Action action { get; set; }

    [Dependency]
    public IActionContext ActionContext { get; set; }

    [Dependency]
    public IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> ProjectCodesRepository { get; set; }
    
    /// <summary>
    /// An instance of <see cref="ILog"/> for logging information.
    /// </summary>
    private static readonly ILog Log = new LogFactory<aeexpense>().GetLogger();

    /// <summary>
    /// An instance of <see cref="IExtraContext"/> for logging extra inforamtion
    /// </summary>
    private static readonly IExtraContext LoggingContent = new Log4NetContextAdapter();

    protected void Page_Load(object sender, EventArgs e)
    {
        LoggingContent["accountid"] = this.ActionContext.CurrentUser.AccountID;
        LoggingContent["employeeid"] = this.ActionContext.CurrentUser.EmployeeID;
        this.cmdok.Attributes.Add("onclick", "if (Page_ClientValidate('vgAeExpenses') == false) { return false; } else { document.getElementById(contentID + 'cmdok').disabled = true; $('select:disabled:visible').prop('disabled', false); " + this.GetPostBackEventReference(this.cmdok) + " };");

        // dcp  - Task = 44622
        if (ViewState["accountid"] != null)
        {
            var clssubcats = this.ActionContext.SubCategories;
            var subcatIdList = (from ListItem item in this.chkitems.Items where item.Text.StartsWith("<div") && item.Attributes.Count == 0 select Int32.Parse(item.Value)).ToList();

            Dictionary<int, string> selectedSubcats = clssubcats.GetSubcatNamesByIdList(subcatIdList);

            foreach (ListItem item in chkitems.Items)
            {
                if (!item.Text.StartsWith("<div") || item.Attributes.Count != 0)
                {
                    continue;
                }

                var subcatId = (Int32.Parse(item.Value));
                string subcatName;
                if (selectedSubcats.TryGetValue(subcatId, out subcatName))
                {
                    item.Attributes.Add("onmouseover", string.Format("SEL.Tooltip.customToolTip('subcatitem{0}','{1}');", item.Value, subcatName));
                    item.Attributes.Add("onmouseout", "SEL.Tooltip.hideTooltip();");
                    item.Attributes.Add("subcat", item.Value);
                    item.Attributes.Add("class", "subcatlistitem");
                }
            }
        }

        Response.Expires = 0;
        Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
        Response.AddHeader("pragma", "no-cache");
        Response.AddHeader("cache-control", "private");
        Response.CacheControl = "no-cache";

        Master.showdummymenu = true;
        Master.enablenavigation = false;
        Master.helpid = 1163;
        Master.UseDynamicCSS = true;

        txtcostcodetotal.Style.Add("display", "none");

        this.PostcodeAnywhereKey = this.ActionContext.CurrentUser.Account.PostcodeAnywhereKey;

        action = Action.Add;
        if (Request.QueryString["action"] != null)
        {
            action = (Action)byte.Parse(Request.QueryString["action"]);
        }

        if (IsPostBack == false)
        {
            ViewState["accountid"] = this.ActionContext.CurrentUser.AccountID;
            ViewState["employeeid"] = this.ActionContext.CurrentUser.EmployeeID;
            ViewState["subAccountID"] = this.ActionContext.CurrentUser.CurrentSubAccountId;


            ViewState["action"] = action;

            ClientScript.RegisterHiddenField("employeeid", ViewState["employeeid"].ToString());
            ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", "var employeeid = " + ViewState["employeeid"].ToString() + ";", true);
            ClientScript.RegisterHiddenField("accountid", this.ActionContext.CurrentUser.AccountID.ToString());
            var itemtype = ItemType.Cash;
            if (Request.QueryString["itemtype"] != null)
            {
                itemtype = (ItemType)byte.Parse(Request.QueryString["itemtype"]);
            }

            ViewState["itemtype"] = itemtype;

            if (Request.QueryString["statementid"] != null)
            {
                ViewState["statementid"] = Request.QueryString["statementid"];
            }
            int transactionid = 0;
            if (Request.QueryString["transactionid"] != null)
            {
                transactionid = int.Parse(Request.QueryString["transactionid"]);
            }
            ViewState["transactionid"] = transactionid;

            int mobileID = 0;
            if (Request.QueryString["mobileID"] != null)
            {
                int.TryParse(Request.QueryString["mobileID"], out mobileID);
            }
            ViewState["mobileID"] = mobileID;

            int mobileJourneyID = 0;
            if (Request.QueryString["mobileJourneyID"] != null)
            {
                int.TryParse(Request.QueryString["mobileJourneyID"], out mobileJourneyID);
            }
            ViewState["mobileJourneyID"] = mobileJourneyID;

            if (transactionid > 0) //update item type
            {
                cCardStatements clsstatements = this.ActionContext.CardStatements;
                cCardTransaction transaction = clsstatements.getTransactionById(transactionid);
                cCardStatement statement = clsstatements.getStatementById(transaction.statementid);
                if (statement.Corporatecard.cardprovider.cardtype == CorporateCardType.CreditCard)
                {
                    itemtype = ItemType.CreditCard;
                }
                else
                {
                    itemtype = ItemType.PurchaseCard;
                }
                ViewState["itemtype"] = itemtype;
            }

            if (Request.QueryString["stage"] != null)
            {
                ViewState["stage"] = int.Parse(Request.QueryString["stage"]);
            }
            int returnto = 0;
            if (Request.QueryString["returnto"] != null)
            {
                returnto = int.Parse(Request.QueryString["returnto"]);
            }
            ViewState["returnto"] = returnto;
            cMisc clsmisc = this.ActionContext.Misc;
            cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(this.ActionContext.AccountId);
            cClaims clsclaims = this.ActionContext.Claims;
            int claimid;
            int expenseid = 0;
            claimid = getClaimId(this.ActionContext.CurrentUser.EmployeeID);
            if (Request.QueryString["expenseid"] != null)
            {
                expenseid = int.Parse(Request.QueryString["expenseid"]);
            }
            ViewState["expenseid"] = expenseid;
            ViewState["claimid"] = claimid;

            int countryid;
            if (this.ActionContext.CurrentUser.Employee.PrimaryCountry == 0)
            {
                countryid = clsproperties.homecountry;
            }
            else
            {
                countryid = this.ActionContext.CurrentUser.Employee.PrimaryCountry;
            }

            ViewState["countryid"] = countryid;
            int currencyid = 0;
            if (this.ActionContext.CurrentUser.Employee.PrimaryCurrency != 0)
            {
                currencyid = this.ActionContext.CurrentUser.Employee.PrimaryCurrency;
            }
            else if (clsproperties.basecurrency != 0)
            {
                currencyid = clsproperties.basecurrency;
            }


            cClaim reqclaim = clsclaims.getClaimById(claimid);
            this.hdnSubCatDates.Value = this.createSubcatList(reqclaim.employeeid);
            ClientScript.RegisterClientScriptBlock(this.GetType(), "country", $"var homeCountry = {clsproperties.homecountry};\nvar homeCurrency = {clsproperties.basecurrency};\n", true);
            ViewState["currencyid"] = currencyid;

            //rebuild costcode tbl;
            if (ViewState["costcoderows"] == null)
            {
                ViewState["costcoderows"] = 0;
            }
            
            SortedList<int, cExpenseItem> items = new SortedList<int, cExpenseItem>();
            cAccountProperties reqProperties = this.ActionContext.Properties;
            cExpenseItems expitems = this.ActionContext.ExpenseItems;
            cExpenseItem item = null;
            switch (action)
            {
                case Action.Add:
                    this.ViewState["Paid"] = false;
                    

                    if (reqclaim == null)
                    {
                        Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }
                    else if (reqclaim.employeeid != this.ActionContext.CurrentUser.EmployeeID || reqclaim.ClaimStage != ClaimStage.Current)
                    {
                        Response.Redirect(ErrorHandlerWeb.InsufficientAccess, true);
                    }

                        EmployeeBankAccountCheck(this.ActionContext.CurrentUser.AccountID, this.ActionContext.CurrentUser.EmployeeID, this.ActionContext.CurrentUser.MustHaveBankAccount);
                        
                    break;

                case Action.Edit:
                    item = clsclaims.getExpenseItemById(expenseid);
                    this.ViewState["Paid"] = item.Paid;
                    if (item == null)
                    {
                        Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }

                    // determine whether user can see page
                    switch (reqclaim.ClaimStage)
                    {
                        case ClaimStage.Current:
                            if (reqclaim.employeeid != this.ActionContext.CurrentUser.EmployeeID) 
                            {
                                // can never edit item when in current unless you're the claim owner
                                Response.Redirect(ErrorHandlerWeb.InsufficientAccess, true);
                            }

                                EmployeeBankAccountCheck(this.ActionContext.CurrentUser.AccountID, this.ActionContext.CurrentUser.EmployeeID, this.ActionContext.CurrentUser.MustHaveBankAccount);
                            break;
                        case ClaimStage.Submitted:
                            if (item.returned == false && (reqclaim.checkerid != this.ActionContext.CurrentUser.EmployeeID && item.itemCheckerId != this.ActionContext.CurrentUser.EmployeeID)) 
                            {
                                // user trying to edit an item
                                Response.Redirect(ErrorHandlerWeb.InsufficientAccess, true);
                            }
                            else if (reqclaim.checkerid != this.ActionContext.CurrentUser.EmployeeID && item.itemCheckerId != this.ActionContext.CurrentUser.EmployeeID && reqclaim.employeeid != this.ActionContext.CurrentUser.EmployeeID) 
                            {
                                // sat with approver, allowed to change
                                Response.Redirect(ErrorHandlerWeb.InsufficientAccess, true);
                            }
                            break;
                        case ClaimStage.Previous:
                            if (!reqProperties.EditPreviousClaims || reqclaim.employeeid == this.ActionContext.CurrentUser.EmployeeID || !this.ActionContext.CurrentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ClaimViewer, true, false))
                            {
                                Response.Redirect(ErrorHandlerWeb.InsufficientAccess, true);
                            }
                            break;
                    }

                    if (item.Edited)
                    {
                        // can never edit item when it has been edited after Pay Before Validate
                        Response.Redirect(ErrorHandlerWeb.InsufficientAccess, true);
                    }

                    //get udf information
                    var clsuserdefined = this.ActionContext.UserDefinedFields;
                    var clstables = this.ActionContext.Tables;
                    cTable tbl = clstables.GetTableByID(new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8"));
                    cTable udftbl = clstables.GetTableByID(tbl.UserDefinedTableID);
                    var clsfields = this.ActionContext.Fields;

                    item.userdefined = clsuserdefined.GetRecord(udftbl, expenseid, clstables, clsfields);
                    item.costcodebreakdown = expitems.getCostCodeBreakdown(expenseid);
                    item.journeysteps = expitems.GetJourneySteps(expenseid);
                    items.Add(item.subcatid, item);

                    ////Get userdefined values for split items.
                    foreach (var requiredItem in items.Values)
                    {
                        requiredItem.userdefined = clsuserdefined.GetRecord(udftbl, requiredItem.expenseid, clstables, clsfields);
                        if (requiredItem.splititems != null && requiredItem.splititems.Count > 0)
                        {
                            foreach (var requiredItemSplitItem in requiredItem.splititems)
                            {
                                requiredItemSplitItem.userdefined = clsuserdefined.GetRecord(udftbl, requiredItemSplitItem.expenseid, clstables, clsfields);
                            }
                        }
                    }

                    ViewState["itemtype"] = item.itemtype;
                    ViewState["expenseitem"] = item;
                    ViewState["costcodebreakdown"] = item.costcodebreakdown;
                    ViewState["employeeid"] = reqclaim.employeeid;
                    ViewState["ValidationCount"] = item.ValidationCount;
                    ViewState["ValidationProgress"] = item.ValidationProgress;

                   break;

                case Action.Copy:
                    item = clsclaims.getExpenseItemById(expenseid).Clone();
                    item.costcodebreakdown = expitems.getCostCodeBreakdown(expenseid);
                    expitems.CloneJourneySteps(expenseid, item, reqProperties, (CurrentUser) this.ActionContext.CurrentUser, ref item);
                    items.Add(item.subcatid, item);
                    this.ViewState["Paid"] = false;
                    expenseid = 0;
                    this.ViewState["expenseid"] = expenseid;
                    this.ViewState["itemtype"] = item.itemtype;
                    this.ViewState["expenseitem"] = item;
                    this.ViewState["costcodebreakdown"] = item.costcodebreakdown;
                    this.ViewState["employeeid"] = reqclaim.employeeid;
                    this.ViewState["ValidationCount"] = item.ValidationCount;
                    this.ViewState["ValidationProgress"] = item.ValidationProgress;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (action != Action.Edit && action != Action.Copy)
            {
                litclear.Text = "<a href=\"javascript:clearGeneral();\" class=\"submenuitem\">Clear General Details</a>";
                if (itemtype == ItemType.Cash && (ViewState["mobileJourneyID"] == null || (int)ViewState["mobileJourneyID"] == 0))
                {
                    lititemcomment.Text = "<div class=\"paneltitle\">My Expense Items</div><div class=\"paneltitle-inner\">Select an item below to add it to your expense sheet:</div>";
                }

                // If the currently logged in user owns the claim and the claim is not unsubmitted do not let them add additional items to it
                // This is to prevent expense items being added to an already submitted claim and not requiring signoff.
                if (this.ActionContext.CurrentUser.Employee.EmployeeID == reqclaim.employeeid && reqclaim.status != ClaimStatus.None)
                {
                    Response.Redirect("~/claimsmenu.aspx", true);
                }

            }

            ViewState["submitted"] = reqclaim.submitted;
            hdnClaimOwnerId.Value = reqclaim.employeeid.ToString();
            ClientScript.RegisterClientScriptBlock(this.GetType(), "variables2", "var nEmployeeid = employeeid;", true);
            ViewState["items"] = items;

            this.AllowClaimantAddOrganisations = reqProperties.ClaimantsCanAddCompanyLocations;
            this.AllowClaimantAddManualAddresses = reqProperties.AddLocations;
            this.OrganisationLabel = clsmisc.GetGeneralFieldByCode("organisation").description;
            this.HomeAddressKeyword = reqProperties.HomeAddressKeyword;
            this.WorkAddressKeyword = reqProperties.WorkAddressKeyword;
            this.ForceAddressNameEntry = reqProperties.ForceAddressNameEntry;
            this.AddressNameEntryMessage = StringManipulators.HtmlSafe(reqProperties.AddressNameEntryMessage);

            if (reqProperties.ShowMileageCatsForUsers == true)
            {
                pnlAddcar.Style.Add(HtmlTextWriterStyle.Height, "500px");
                pnlAddcar.Style.Add(HtmlTextWriterStyle.Overflow, "auto");
                pnlAddcar.Style.Add(HtmlTextWriterStyle.OverflowX, "hidden");
                pnlAddcar.Style.Add(HtmlTextWriterStyle.OverflowY, "auto");
            }

            addCar.AccountID = this.ActionContext.CurrentUser.AccountID;
            addCar.EmployeeID = this.ActionContext.CurrentUser.EmployeeID;
            addCar.Action = aeCarPageAction.Add;
            addCar.SendEmailWhenNewCarAdded = true;
            addCar.EmployeeAdmin = false;
            addCar.isAeExpenses = true;
            addCar.inModalPopup = true;
            addCar.ShowStartDateOnly = reqProperties.AllowEmpToSpecifyCarStartDateOnAdd;
            addCar.ShowDutyOfCare = reqProperties.AllowEmpToSpecifyCarDOCOnAdd;

            // load the user's currently active cars so they can choose to replace old with new
            var employeeCars = this.ActionContext.EmployeeCars;
            addCar.cmbPreviousCar.Items.AddRange(employeeCars.CreateCurrentValidCarDropDown(DateTime.UtcNow, "You currently have no cars to replace",fromAeExpenses:true).ToArray());

        }

        this.Title = $"{action} Expense";
        if (this.action == Action.Copy)
        {
            this.cmdok.Text = "save copy";
        }
        this.Master.title = Title;


        generateGeneralDetails();

     
        GenerateCostcodeBreakdown();
        
           
        generateSpecificDetails();

        filterDropdownsOnPageStart();

        if (this.ActionContext.CurrentUser.Account.MapsEnabled)
        {
            var mapControl = (Map)LoadControl("~/shared/usercontrols/Map.ascx");
            mapControl.AccountId = this.ActionContext.CurrentUser.AccountID;
            mapControl.CompanyId = this.ActionContext.CurrentUser.Account.companyid;
            mapContainer.Controls.Add(mapControl);
        }

        const string mileageGridSetupScriptKey = "mileagegridsetup";
        var claimSubmitted = (bool?)this.ViewState["submitted"] ?? false;
        ScriptManager.RegisterStartupScript(upnlitems, GetType(), mileageGridSetupScriptKey, "$(document).ready(function() { SEL.MileageGrid.setup(); });", true);
        ScriptManager.RegisterStartupScript(upnlgeneral, GetType(), mileageGridSetupScriptKey, "$(document).ready(function() { SEL.MileageGrid.setup(); });", true);
        ScriptManager.RegisterStartupScript(upnlSpecific, GetType(), mileageGridSetupScriptKey, "$(document).ready(function() { SEL.MileageGrid.setup(); });", true);
        ScriptManager.RegisterStartupScript(upnlSpecific, GetType(), "mileagegridrefresh", "$(document).ready(function() {refreshMileagePanel('" + claimSubmitted + "'); });", true);
    }

    

    protected override void InitializeCulture()
    {
        base.InitializeCulture();
        cLocales.setLocale();
    }
    protected override void OnPreInit(EventArgs e)
    {

    }
    private void generateGeneralDetails()
    {
        int accountid = (int)ViewState["accountid"];
        var employeeId = (int) ViewState["employeeid"];
        cMisc clsmisc = this.ActionContext.Misc;
        cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(accountid);
        cClaims clsclaims = this.ActionContext.Claims;
        Employee reqemp = this.ActionContext.Employees.GetEmployeeById(employeeId);
        cAccountProperties reqProperties = this.ActionContext.Properties;
        Table tbl = new Table();

        TableRow row;
        Image img;
        TableCell cell;
        TextBox txtbox;
        RequiredFieldValidator reqval;
        CompareValidator compval;
        CustomValidator custval;
        DropDownList ddlst;
        AutoCompleteExtender autocomp;
        MaskedEditExtender maskededit;
        CalendarExtender cal;
        ItemType itemtype = (ItemType)ViewState["itemtype"];
        Spend_Management.Action action = (Spend_Management.Action)ViewState["action"];
        cExpenseItem expenseitem = null;
        ValidatorCalloutExtender valcall;
        HiddenField hdn;
        Literal lit;

        int countryid = 0;
        int currencyid = 0;

        //first row, date and claim

        tbl.ID = "tblGeneral";
        pnlgeneral.Controls.Add(tbl);
        cCardTransaction cardtransaction = null;
        if (ViewState["transactionid"] != null && (int)ViewState["transactionid"] > 0)
        {
            cCardStatements clsstatements = this.ActionContext.CardStatements;
            cardtransaction = clsstatements.getTransactionById((int)ViewState["transactionid"]);
        }

        ExpenseItem mobileItem = null;
        if (ViewState["mobileID"] != null && (int)ViewState["mobileID"] > 0)
        {
            int mobileID = (int)ViewState["mobileID"];
            cMobileDevices clsmobile = this.ActionContext.MobileDevices;
            mobileItem = clsmobile.getMobileItemByID(mobileID);

            if (mobileItem == null)
            {
                Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
            }
            else if (mobileItem.CreatedBy != this.ActionContext.CurrentUser.EmployeeID)
            {
                Response.Redirect(ErrorHandlerWeb.InsufficientAccess, true);
            }
        }

        if (action == Action.Edit || action == Action.Copy)
        {
            SortedList<int, cExpenseItem> items = (SortedList<int, cExpenseItem>)ViewState["items"];
            expenseitem = items.Values[0];
        }

        MobileJourney mobileJourney = null;
        if (ViewState["mobileJourneyID"] != null && (int)ViewState["mobileJourneyID"] > 0)
        {
            int mobileJourneyID = (int)ViewState["mobileJourneyID"];
            cMobileDevices clsmobile = this.ActionContext.MobileDevices;
            mobileJourney = clsmobile.GetMobileJourney(mobileJourneyID);

            if (mobileJourney == null)
            {
                Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
            }
            else if (mobileJourney.CreatedBy != this.ActionContext.CurrentUser.EmployeeID)
            {
                // don't let users get to other peoples mobile journeys
                Response.Redirect(ErrorHandlerWeb.InsufficientAccess, true);
            }
        }


        int basecurrency = reqemp.PrimaryCurrency != 0 ? reqemp.PrimaryCurrency : clsproperties.basecurrency;
        row = new TableRow();

        #region date
        cell = new TableCell
               {
                   CssClass = "labeltd",
                   Text = "Date:"
               };
        row.Cells.Add(cell);
        txtbox = new TextBox();
        txtbox.ID = "txtdate";
        string carRefreshJavascript = "";
        string refreshMileagePanel = "";
        if (reqProperties.DisableCarOutsideOfStartEndDate)
        {
            carRefreshJavascript = "RefreshCarDropDown();";
        }
        if (reqProperties.UseDateOfExpenseForDutyOfCareChecks)
        {
            var claimSubmitted = (bool?)this.ViewState["submitted"] ?? false;
            refreshMileagePanel = "refreshMileagePanel('" + claimSubmitted + "');";
        }
        txtbox.Attributes.Add("onchange", "SetHiddenDateFieldValue(this); SEL.Expenses.ExpenseItems.Filter(); if ($('td.esrAssignment select').length > 0) { RefreshAssignmentDropDown(this); } else { SEL.AddressesAndTravel.UpdateHomeAndOfficeAddresses(); } " + carRefreshJavascript + refreshMileagePanel);

        if ((action == Spend_Management.Action.Add  && itemtype == ItemType.Cash) || action == Action.Copy)
        {
            if (mobileItem != null)
            {
                txtbox.Text = mobileItem.dtDate.ToShortDateString();
                ViewState["HomeOrOfficeDate"] = mobileItem.dtDate;
            }
            else if (mobileJourney != null)
            {
                txtbox.Text = mobileJourney.JourneyDateTime.ToShortDateString();
                ViewState["HomeOrOfficeDate"] = mobileJourney.JourneyDateTime;

            }

            else if (Session["date"] != null && action != Action.Copy)
            {
                txtbox.Text = ((DateTime)Session["date"]).ToShortDateString();
                ViewState["HomeOrOfficeDate"] = ((DateTime)Session["date"]).Date;
            }
            else
            {
                txtbox.Text = DateTime.Today.AddDays(-1).ToShortDateString();
                ViewState["HomeOrOfficeDate"] = DateTime.Today.AddDays(-1).Date;
            }

            if (string.IsNullOrEmpty(this.hdnExpDate.Value))
            {
                this.hdnExpDate.Value = txtbox.Text;
            }
        }
        else if ((action == Spend_Management.Action.Add )&& (itemtype == ItemType.CreditCard || itemtype == ItemType.PurchaseCard))
        {
            if (cardtransaction.transactiondate != null)
            {
                txtbox.Text = ((DateTime)cardtransaction.transactiondate).ToShortDateString();
                ViewState["HomeOrOfficeDate"] = ((DateTime)cardtransaction.transactiondate).Date;
            }
        }
        else if (action == Spend_Management.Action.Edit)
        {
            txtbox.Text = expenseitem.date.ToShortDateString();
            ViewState["HomeOrOfficeDate"] = expenseitem.date.Date;
        }
        cell = new TableCell { CssClass = "inputtd" };
        cell.Controls.Add(txtbox);
        row.Cells.Add(cell);
        if (itemtype == ItemType.Cash)
        {
            maskededit = new MaskedEditExtender { TargetControlID = "txtdate", Mask = "99/99/9999", MaskType = MaskedEditType.Date };
            //maskededit.CultureName = "en-GB";

            cal = new CalendarExtender
                  {
                      TargetControlID = "txtdate",
                      Format = "dd/MM/yyyy",
                      PopupButtonID = "imgcal"
                  };

            cell.Controls.Add(cal);
            cell.Controls.Add(maskededit);

            reqval = new RequiredFieldValidator
                     {
                         ControlToValidate = "txtdate",
                         ErrorMessage = "Please enter the date the expense was incurred in the box provided",
                         Text = "*",
                         ValidationGroup = "vgAeExpenses"
                     };

            cell.Controls.Add(reqval);



            compval = new CompareValidator
                      {
                          ControlToValidate = "txtdate",
                          Text = "*",
                          ErrorMessage = "The latest date an expense can be claimed for is the " + DateTime.Today.ToShortDateString(),
                          Type = ValidationDataType.Date,
                          Operator = ValidationCompareOperator.LessThanEqual,
                          ID = "comptxtdatemax",
                          ValueToCompare = DateTime.Today.ToShortDateString(),
                          ValidationGroup = "vgAeExpenses"
                      };
            cell.Controls.Add(compval);

                compval = new CompareValidator
                          {
                              ControlToValidate = "txtdate",
                              Text = "*",
                              ErrorMessage = "Please enter a valid date in the box provided",
                              Type = ValidationDataType.Date,
                              Operator = ValidationCompareOperator.GreaterThanEqual,
                              ID = "comptxtdatemin",
                              ValueToCompare = "01/01/1900",
                              ValidationGroup = "vgAeExpenses"
                          };
                cell.Controls.Add(compval);

            valcall = new ValidatorCalloutExtender
                      {
                          ID = "valcalltxtdate",
                          TargetControlID = "comptxtdatemax",
                          Width = Unit.Pixel(350)
                      };
            cell.Controls.Add(valcall);

            row.Cells.Add(cell);



        }
        else
        {
            txtbox.Enabled = false;
        }

        /* EMPTY VALIDATOR CELL - due to having 3 seperate validators its simpler to just put them next to the cal icon with the short txtbox*/
        cell = new TableCell { CssClass = "inputtd", Text = "&nbsp;" };
        row.Cells.Add(cell);
        /* / */

        cell = new TableCell();
        cell.CssClass = "inputtd";

        lit = new Literal();
        if (true)//clshelp.containsTip(236))
        {
            lit.Text = "<img id=\"imgtooltip236\" onclick=\"SEL.Tooltip.Show('99b6eaac-cca6-4270-8f9a-536e0a4d4eb7', 'ex', this);\" src=\"../static/icons/16/new-icons/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);
        }

        row.Cells.Add(cell);

        #endregion

        #region claim
        bool submitted = (bool)ViewState["submitted"];

        //turn off claim if needed
        if (!submitted && clsproperties.singleclaim == false && clsclaims.getCount((int)ViewState["employeeid"], ClaimStage.Current) >= 1)
        {
            cell = new TableCell();
            cell.CssClass = "labeltd";
            cell.Text = "Claim:";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "inputtd";
            ddlst = new DropDownList();
            ddlst.ID = "cmbclaims";
            ddlst.Items.AddRange(clsclaims.CreateDropDown((int)ViewState["employeeid"]).ToArray());
            if (action == Spend_Management.Action.Edit || action== Action.Copy)
            {
                if ((int)ViewState["employeeid"] != (int)ViewState["employeeid"]) //checker
                {
                    ddlst.Items.Clear();
                    cClaim claim = clsclaims.getClaimById(expenseitem.claimid);
                    ddlst.Items.Add(new ListItem(claim.name, claim.claimid.ToString()));
                    ddlst.Enabled = false;
                }
                else
                {
                    if (ddlst.Items.FindByValue(expenseitem.claimid.ToString()) != null)
                    {
                        ddlst.Items.FindByValue(expenseitem.claimid.ToString()).Selected = true;
                    }
                }
            }
            else
            {
                if (ViewState["claimid"] != null)
                {
                    if (ddlst.Items.FindByValue(ViewState["claimid"].ToString()) != null)
                    {
                        ddlst.Items.FindByValue(ViewState["claimid"].ToString()).Selected = true;
                    }
                }
            }
            cell.Controls.Add(ddlst);

            row.Cells.Add(cell);

            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Text = "&nbsp;";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.CssClass = "inputtd";
            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip237\" onclick=\"SEL.Tooltip.Show('00763540-a655-48f0-adb9-c54920cec8e9', 'ex', this);\" src=\"../static/icons/16/new-icons/tooltip.png\" alt=\"\" class=\"tooltipicon\"/> ";
            cell.Controls.Add(lit);
            row.Cells.Add(cell);

        }

        tbl.Rows.Add(row);

        #endregion


        #region company
        cFieldToDisplay company = clsmisc.GetGeneralFieldByCode("organisation");
        lblOrganisationSearchTitle.Text = company.description;
        row = new TableRow();
        if (!company.individual && ((itemtype == ItemType.Cash && company.display) || (itemtype == ItemType.CreditCard && company.displaycc) || (itemtype == ItemType.PurchaseCard && company.displaypc)))
        {
            cell = new TableCell { CssClass = "labeltd", Text = company.description + ":" };
            row.Cells.Add(cell);

            cell = new TableCell { CssClass = "inputtd" };

            txtbox = new TextBox
            {
                ID = "txtOrganisation",
                CssClass = "organisation-autocomplete"
            };

            txtbox.Attributes.Add("data-search", "General");

            TextBox hiddenIdentifier = new TextBox { ID = "txtOrganisation_ID" };
            hiddenIdentifier.Style.Add(HtmlTextWriterStyle.Display, "none");

            Organisation organisation;

            if (action == Action.Edit || action == Action.Copy)
            {
                if (expenseitem.companyid > 0)
                {
                    organisation = Organisation.Get((int)ViewState["accountid"], expenseitem.companyid);

                    if (organisation != null)
                    {
                        txtbox.Text = organisation.Name;
                        hiddenIdentifier.Text = organisation.Identifier.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
            else if (action == Action.Add && itemtype == ItemType.Cash)
            {
                if (Session["companyid"] != null && (int)Session["companyid"] > 0)
                {
                    organisation = Organisation.Get((int)ViewState["accountid"], (int)Session["companyid"]);

                    if (organisation != null)
                    {
                        txtbox.Text = organisation.Name;
                        hiddenIdentifier.Text = organisation.Identifier.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }

            cell.Controls.Add(txtbox);
            cell.Controls.Add(hiddenIdentifier);
            cell.Controls.Add(new Literal { Text = " " });

            // search icon
            img = new Image
            {
                ImageUrl = GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/find.png",
                ID = "txtOrganisationSearchIcon",
                AlternateText = "Search " + company.description,
                CssClass = "btn"
            };

            img.Attributes.Add("onclick", "OrganisationSearch = OrganisationSearches[\"General\"];OrganisationSearches[\"General\"].Search();");

            cell.Controls.Add(img);
            row.Cells.Add(cell);

            cell = new TableCell { CssClass = "inputtd", ColumnSpan = 2 };
            if ((itemtype == ItemType.Cash && company.mandatory) || (itemtype == ItemType.CreditCard && company.mandatorycc) || (itemtype == ItemType.PurchaseCard && company.mandatorypc))
            {
                custval = new CustomValidator
                {
                    ClientValidationFunction = "SEL.Expenses.Validate.Organisation.GeneralDetailsMandatory",
                    ControlToValidate = "txtOrganisation",
                    ID = "custorganisation",
                    ValidationGroup = "vgAeExpenses",
                    ValidateEmptyText = true,
                    ErrorMessage = "Please enter a valid " + company.description + ".",
                    Text = "*"
                };

                cell.Controls.Add(custval);
            }
            else
            {
                custval = new CustomValidator
                {
                    ClientValidationFunction = "SEL.Expenses.Validate.Organisation.GeneralDetailsNotMandatory",
                    ControlToValidate = "txtOrganisation",
                    ID = "custorganisation",
                    ValidationGroup = "vgAeExpenses",
                    ValidateEmptyText = true,
                    ErrorMessage = company.description + " is not valid. Please enter a value in the box provided.",
                    Text = "*"
                };

                cell.Controls.Add(custval);
            }

            row.Cells.Add(cell);
        }

        tbl.Rows.Add(row);
        #endregion

        #region from and to
        cFieldToDisplay from = clsmisc.GetGeneralFieldByCode("from");
        cFieldToDisplay to = clsmisc.GetGeneralFieldByCode("to");
        if (!from.individual && ((itemtype == ItemType.Cash && (from.display || to.display)) || (itemtype == ItemType.CreditCard && (from.displaycc || to.displaycc)) || (itemtype == ItemType.PurchaseCard && (from.displaypc || to.displaypc))))
        {
            row = new TableRow();
            Lazy<int[]> homeAddressIds = new Lazy<int[]>(() => reqemp.GetHomeAddresses().HomeLocations.Select(l => l.LocationID).ToArray());

            if ((itemtype == ItemType.Cash && from.display) || (itemtype == ItemType.CreditCard && from.displaycc) || (itemtype == ItemType.PurchaseCard && from.displaypc))
            {
                cell = new TableCell { CssClass = "labeltd", Text = @from.description + ":" };
                row.Cells.Add(cell);

                cell = new TableCell { CssClass = "inputtd" };
                hdn = new HiddenField { ID = "txtfromid" };

                Address reqfrom;
                if (action == Action.Edit || action== Action.Copy)
                {
                    if (expenseitem.fromid > 0)
                    {
                        reqfrom = Address.Get(accountid, expenseitem.fromid);
                        if (reqfrom != null)
                        {
                            hdn.Value = reqfrom.Identifier.ToString();
                        }
                    }
                }

                cell.Controls.Add(hdn);
                txtbox = new TextBox() { ID = "txtfrom", CssClass = "ui-sel-address-picker" };
                txtbox.Attributes.Add("rel", hdn.ClientID);
                if (action == Action.Edit || action == Action.Copy)
                {
                    if (expenseitem.fromid > 0)
                    {
                        reqfrom = Address.Get(accountid, expenseitem.fromid);
                        if (reqfrom != null)
                        {
                            string fromText;
                            if (homeAddressIds.Value.Contains(reqfrom.Identifier))
                            {
                                fromText = reqProperties.HomeAddressKeyword;
                            }
                            else
                            {
                                fromText = reqfrom.FriendlyName;
                            }
                            txtbox.Text = fromText;

                        }
                    }
                }
                else if (action == Action.Add && itemtype == ItemType.Cash)
                {
                    if (Session["fromid"] != null)
                    {
                        reqfrom = Address.Get(accountid, (int)Session["fromid"]);
                        if (reqfrom != null)
                        {
                            string fromText;
                            if (homeAddressIds.Value.Contains(reqfrom.Identifier))
                            {
                                fromText = reqProperties.HomeAddressKeyword;
                            }
                            else
                            {
                                fromText = reqfrom.FriendlyName;
                            }
                            txtbox.Text = fromText;
                            hdn.Value = reqfrom.Identifier.ToString();
                        }
                    }
                }
                cell.Controls.Add(txtbox);

                img = new Image { ID = "imgFromSearchAddress", ImageUrl = "~/shared/images/icons/16/plain/find.png" };
                cell.Controls.Add(img);

                addressDetailsPopup.AddEvents(img, hdn, "ctl00_contentmain_", false);
                row.Cells.Add(cell);

                cell = new TableCell { CssClass = "inputtd", ColumnSpan = 2 };
                if ((itemtype == ItemType.Cash && from.mandatory) || (itemtype == ItemType.CreditCard && from.mandatorycc) || (itemtype == ItemType.PurchaseCard && from.mandatorypc))
                {
                    reqval = new RequiredFieldValidator { ControlToValidate = "txtfrom", ErrorMessage = @from.description + " is a mandatory field. Please enter a value in the box provided", Text = "*", ValidationGroup = "vgAeExpenses" };
                    cell.Controls.Add(reqval);
                }
                else
                {
                    cell.Text = "&nbsp;";
                }

                row.Cells.Add(cell);
            }

            if (!to.individual && ((itemtype == ItemType.Cash && to.display) || (itemtype == ItemType.CreditCard && to.displaycc) || (itemtype == ItemType.PurchaseCard && to.displaypc)))
            {
                cell = new TableCell { CssClass = "labeltd", Text = to.description + ":" };
                row.Cells.Add(cell);


                cell = new TableCell { CssClass = "inputtd" };

                hdn = new HiddenField { ID = "txttoid" };

                Address reqto;
                if (action == Action.Edit || action== Action.Copy)
                {
                    if (expenseitem.toid > 0)
                    {
                        reqto = Address.Get(accountid, expenseitem.toid);
                        if (reqto != null)
                        {
                            hdn.Value = reqto.Identifier.ToString();
                        }
                    }
                }
                cell.Controls.Add(hdn);
                txtbox = new TextBox() { ID = "txtto", CssClass = "ui-sel-address-picker" };
                txtbox.Attributes.Add("rel", hdn.ClientID);

                if (action == Action.Edit || action == Action.Copy)
                {
                    if (expenseitem.toid > 0)
                    {
                        reqto = Address.Get(accountid, expenseitem.toid);
                        if (reqto != null)
                        {
                            string toText;
                            if (homeAddressIds.Value.Contains(reqto.Identifier))
                            {
                                toText = reqProperties.HomeAddressKeyword;
                            }
                            else
                            {
                                toText = reqto.FriendlyName;
                            }
                            txtbox.Text = toText;
                        }
                    }
                }
                else if (action == Spend_Management.Action.Add && itemtype == ItemType.Cash)
                {
                    if (Session["toid"] != null)
                    {
                        reqto = Address.Get(accountid, (int)Session["toid"]);
                        if (reqto != null)
                        {
                            string toText;
                            if (homeAddressIds.Value.Contains(reqto.Identifier))
                            {
                                toText = reqProperties.HomeAddressKeyword;
                            }
                            else
                            {
                                toText = reqto.FriendlyName;
                            }
                            txtbox.Text = toText;
                            hdn.Value = reqto.Identifier.ToString();
                        }
                    }
                }
                cell.Controls.Add(txtbox);

                img = new Image { ID = "imgToSearchAddress", ImageUrl = "~/shared/images/icons/16/plain/find.png" };
                cell.Controls.Add(img);
                addressDetailsPopup.AddEvents(img, hdn, "ctl00_contentmain_", false);

                row.Cells.Add(cell);
                cell = new TableCell { CssClass = "inputtd", ColumnSpan = 2 };
                if ((itemtype == ItemType.Cash && to.mandatory) || (itemtype == ItemType.CreditCard && to.mandatorycc) || (itemtype == ItemType.PurchaseCard && to.mandatorypc))
                {
                    reqval = new RequiredFieldValidator { ControlToValidate = "txtto", ErrorMessage = to.description + " is a mandatory field. Please enter a value in the box provided", Text = "*", ValidationGroup = "vgAeExpenses" };
                    cell.Controls.Add(reqval);
                }
                else
                {
                    cell.Text = "&nbsp;";
                }

                row.Cells.Add(cell);
            }
            tbl.Rows.Add(row);

        }
        #endregion

        #region reason and country
        cFieldToDisplay reason = clsmisc.GetGeneralFieldByCode("reason");

        row = new TableRow();
        if (!reason.individual && ((itemtype == ItemType.Cash && reason.display) || (itemtype == ItemType.CreditCard && reason.displaycc) || (itemtype == ItemType.PurchaseCard && reason.displaypc)))
        {
            cell = new TableCell();
            cell.CssClass = "labeltd";
            cell.Text = reason.description + ":";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "inputtd";
            ddlst = new DropDownList();
            ddlst.Items.AddRange(this.ActionContext.ClaimReasons.CreateActiveReasonsDropDown().ToArray());
            ddlst.ID = "cmbreason";
            var reasonAttribute = this.ActionContext.FilterRules.FilterDropdown(FilterType.Reason, "", ddlst.ID);

            if (reasonAttribute != null)
            {
                ddlst.Attributes.Add(reasonAttribute[0], reasonAttribute[1]);
            }

            if (action == Action.Edit || action == Action.Copy)
            {
                if (ddlst.Items.FindByValue(expenseitem.reasonid.ToString()) != null)
                {
                    ddlst.Items.FindByValue(expenseitem.reasonid.ToString()).Selected = true;
                }
            }
            else if (action == Action.Add && itemtype == ItemType.Cash)
            {
                if (mobileItem != null && mobileItem.ReasonID != null)
                {
                    if (ddlst.Items.FindByValue(mobileItem.ReasonID.ToString()) != null)
                    {
                        ddlst.Items.FindByValue(mobileItem.ReasonID.ToString()).Selected = true;
                    }
                }
                else if (Session["reasonid"] != null)
                {
                    if (ddlst.Items.FindByValue(Session["reasonid"].ToString()) != null)
                    {
                        ddlst.Items.FindByValue(Session["reasonid"].ToString()).Selected = true;
                    }
                }
            }

            cell.Controls.Add(ddlst);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.CssClass = "inputtd";

            if ((itemtype == ItemType.Cash && reason.mandatory) || (itemtype == ItemType.CreditCard && reason.mandatorycc) || (itemtype == ItemType.PurchaseCard && reason.mandatorypc))
            {

                CompareValidator compReasonVal = GenerateCompareValidator("cmbreason", reason.description, "0");
                cell.Controls.Add(compReasonVal);
            }
            else
            {
                cell.Text = "&nbsp;";
            }

            row.Cells.Add(cell);

            cell = new TableCell();
            cell.CssClass = "inputtd";
            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip240\" onclick=\"SEL.Tooltip.Show('8dca38bb-9ef2-4de8-91e3-9402e4ace68b', 'ex', this);\" src=\"../static/icons/16/new-icons/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            row.Cells.Add(cell);
            cell.Controls.Add(lit);
        }

        cFieldToDisplay country = clsmisc.GetGeneralFieldByCode("country");

        if ((itemtype == ItemType.Cash && country.display) || (itemtype == ItemType.CreditCard && country.displaycc) || (itemtype == ItemType.PurchaseCard && country.displaypc))
        {
            cell = new TableCell();
            cell.CssClass = "labeltd";
            cell.Text = country.description + ":";
            row.Cells.Add(cell);
            cCountries clscountries = this.ActionContext.Countries;
            cell = new TableCell();
            cell.CssClass = "inputtd";
            ddlst = new DropDownList();
            ddlst.Items.AddRange(clscountries.CreateDropDown().ToArray());
            ddlst.ID = "cmbcountry";
            ddlst.Attributes.Add("onchange", "cmbcountry_onchange(); LocationSearchObject.UpdateDefaultPageCountry(this);");

            cell.Controls.Add(ddlst);
            switch (action)
            {
                case Spend_Management.Action.Add:
                    switch (itemtype)
                    {
                        case ItemType.Cash:
                            if (Session["countryid"] != null)
                            {
                                countryid = (int)Session["countryid"];
                            }
                            else
                            {
                                countryid = (int)ViewState["countryid"];
                            }
                            break;
                        case ItemType.CreditCard:
                        case ItemType.PurchaseCard:
                            if (cardtransaction.globalcountryid != null)
                            {
                                cCountry cardcountry = clscountries.getCountryByGlobalCountryId((int)cardtransaction.globalcountryid);
                                if (cardcountry != null)
                                {
                                    ddlst.Enabled = false;
                                    countryid = cardcountry.CountryId;
                                }
                            }

                            break;
                    }

                    if (ddlst.Items.FindByValue(countryid.ToString()) != null)
                    {
                        ddlst.Items.FindByValue(countryid.ToString()).Selected = true;
                    }
                    break;
                case Action.Edit:
                case Action.Copy:
                    if (ddlst.Items.FindByValue(expenseitem.countryid.ToString()) != null)
                    {
                        ddlst.Items.FindByValue(expenseitem.countryid.ToString()).Selected = true;
                    }
                    else
                    {
                        // if countryid is archived, need to retrospectively populate
                        ddlst.Items.Add(clscountries.GetListItem(expenseitem.countryid));
                        ddlst.Items.FindByValue(expenseitem.countryid.ToString()).Selected = true;
                    }
                    if (expenseitem != null && expenseitem.transactionid > 0)
                    {
                        ddlst.Enabled = false;
                    }
                    break;
            }
            ViewState["countryid"] = countryid;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.CssClass = "inputtd";
            if ((itemtype == ItemType.Cash && country.mandatory) || (itemtype == ItemType.CreditCard && country.mandatorycc) || (itemtype == ItemType.PurchaseCard && country.mandatorypc))
            {
                CompareValidator compCountryVal = GenerateCompareValidator("cmbcountry", reason.description, "0");
                cell.Controls.Add(compCountryVal);
            }
            else
            {
                cell.Text = "&nbsp;";
            }
            row.Cells.Add(cell);


            cell = new TableCell();
            cell.CssClass = "inputtd";
            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip241\" onclick=\"SEL.Tooltip.Show('4e99e549-2a07-4ada-9858-5e6bb577bd33', 'ex', this);\" src=\"../static/icons/16/new-icons/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            row.Cells.Add(cell);
            cell.Controls.Add(lit);
        }
        tbl.Rows.Add(row);

        #endregion

        #region currency and exchangerate
        cFieldToDisplay currency = clsmisc.GetGeneralFieldByCode("currency");

        if ((itemtype == ItemType.Cash && currency.display) || (itemtype == ItemType.CreditCard && currency.displaycc) || (itemtype == ItemType.PurchaseCard && currency.displaypc))
        {
            row = new TableRow();
            cell = new TableCell();
            cell.CssClass = "labeltd";
            cell.Text = currency.description + ":";
            row.Cells.Add(cell);
            cCurrencies clscurrencies = this.ActionContext.Currencies;
            cell = new TableCell();
            cell.CssClass = "inputtd";
            ddlst = new DropDownList();
            ddlst.Attributes.Add("onchange", "getExchangeRate(" + ViewState["accountid"] + ")");
            ddlst.Items.AddRange(clscurrencies.CreateDropDown().ToArray());
            ddlst.ID = "cmbcurrency";

            switch (action)
            {
                case Spend_Management.Action.Add:
                    switch (itemtype)
                    {
                        case ItemType.Cash:
                            if (mobileItem != null && mobileItem.CurrencyID != null)
                            {
                                currencyid = (int)mobileItem.CurrencyID;
                            }
                            else if (Session["currencyid"] != null)
                            {
                                currencyid = (int)Session["currencyid"];
                            }
                            else
                            {
                                currencyid = (int)ViewState["currencyid"];
                            }
                            break;
                        case ItemType.CreditCard:
                        case ItemType.PurchaseCard:
                            cCurrency cardcurrency = clscurrencies.getCurrencyByGlobalCurrencyId((int)cardtransaction.globalcurrencyid);
                            if (cardcurrency != null) //global currency not yet assigned
                            {
                                ddlst.Enabled = false;
                                currencyid = cardcurrency.currencyid;
                            }
                            break;
                    }
                    if (ddlst.Items.FindByValue(currencyid.ToString()) != null)
                    {
                        ddlst.Items.FindByValue(currencyid.ToString()).Selected = true;
                    }
                    break;
                case Spend_Management.Action.Edit:
                case Spend_Management.Action.Copy:
                    currencyid = expenseitem.currencyid;
                    if (ddlst.Items.FindByValue(expenseitem.currencyid.ToString()) != null)
                    {
                        ddlst.Items.FindByValue(expenseitem.currencyid.ToString()).Selected = true;
                    }
                    if (expenseitem != null && expenseitem.transactionid > 0)
                    {
                        ddlst.Enabled = false;
                    }
                    break;
            }
            ViewState["currencyid"] = currencyid;
            cell.Controls.Add(ddlst);
            row.Cells.Add(cell);


            cell = new TableCell();
            cell.CssClass = "inputtd";



            if ((itemtype == ItemType.Cash && currency.mandatory) || (itemtype == ItemType.CreditCard && currency.mandatorycc) || (itemtype == ItemType.PurchaseCard && currency.mandatorypc))
            {
                CompareValidator compCurrencyVal = GenerateCompareValidator("cmbcurrency", currency.description, "0");
                cell.Controls.Add(compCurrencyVal);
            }
            else
            {
                cell.Text += "&nbsp;";
            }

            row.Cells.Add(cell);


            cell = new TableCell();
            cell.CssClass = "inputtd";
            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip242\" onclick=\"SEL.Tooltip.Show('bc938807-1f8a-4e9d-86e0-7fb7a831178d', 'ex', this);\" src=\"../static/icons/16/new-icons/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);
            row.Cells.Add(cell);







            //exchange rate
            bool showExchangeRateTT = true;
            cell = new TableCell();
            cell.Text = "Exchange Rate:";
            cell.CssClass = "labeltd";
            cell.ID = "cellexchlbl";
            if (action == Spend_Management.Action.Add)
            {
                //hide

                if (currencyid == basecurrency)
                {
                    cell.Style["display"] = "none";
                    showExchangeRateTT = false;
                }




            }
            else if (action == Spend_Management.Action.Edit || action == Action.Copy)
            {
                if (expenseitem.currencyid == clsproperties.basecurrency)
                {
                    cell.Style["display"] = "none";
                    showExchangeRateTT = false;
                }
            }
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.ID = "cellexchinput";
            if (action == Spend_Management.Action.Add)
            {
                //hide
                if (currencyid == basecurrency)
                {
                    cell.Style["display"] = "none";
                    showExchangeRateTT = false;
                }
            }
            else if (action == Spend_Management.Action.Edit || action == Action.Copy)
            {
                if (expenseitem.currencyid == clsproperties.basecurrency)
                {
                    cell.Style["display"] = "none";
                    showExchangeRateTT = false;
                }
            }
            txtbox = new TextBox();
            txtbox.ID = "txtexchangerate";
            if (clsproperties.exchangereadonly || (itemtype != ItemType.Cash && (int)ViewState["transactionid"] > 0))
            {
                txtbox.Enabled = false;
            }
            if (itemtype == ItemType.Cash && action == Spend_Management.Action.Add)
            {
                if (Session["exchangerate"] != null)
                {
                    txtbox.Text = Session["exchangerate"].ToString();
                }
            }
            else if (itemtype == ItemType.CreditCard || itemtype == ItemType.PurchaseCard)
            {
                if (currencyid != basecurrency)
                {
                    if (cardtransaction != null)
                    {
                        txtbox.Text = cardtransaction.reverseexchangerate.ToString();
                    }
                    else if (expenseitem != null)
                    {
                        Session["exchangerate"] = expenseitem.exchangerate;
                        txtbox.Text = expenseitem.exchangerate.ToString("0.00000");
                        if (expenseitem.transactionid > 0)
                        {
                            txtbox.Enabled = false;
                        }
                    }
                }
            }
            else if (action == Spend_Management.Action.Edit || action == Action.Copy)
            {
                if (expenseitem.exchangerate == 0)
                {
                    txtbox.Text = "1";
                }
                else
                {
                    txtbox.Text = expenseitem.exchangerate.ToString();
                }
            }
            cell.Controls.Add(txtbox);

            row.Cells.Add(cell);
            cell = new TableCell();
            cell.ID = "exchangeratemandatory";
            cell.Text = string.Empty;           
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "inputtd";
            if (showExchangeRateTT == true)
            {
                lit = new Literal();
                lit.Text = "<img id=\"imgtooltip243\" onclick=\"SEL.Tooltip.Show('2efc4351-c272-4920-8d4b-9a596e2105ec', 'ex', this);\" src=\"../static/icons/16/new-icons/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
                cell.Controls.Add(lit);
            }
            else
            {
                lit = new Literal();
                lit.Text = "<img id=\"imgtooltip243\" onclick=\"SEL.Tooltip.Show('2efc4351-c272-4920-8d4b-9a596e2105ec', 'ex', this);\" src=\"../static/icons/16/new-icons/tooltip.png\" alt=\"\" class=\"tooltipicon\" style=\"display: none;\"/>";
                cell.Controls.Add(lit);
            }
            row.Cells.Add(cell);
            tbl.Rows.Add(row);
        }
        #endregion

        #region Costcode breakdown
        List<cDepCostItem> breakdown = new List<cDepCostItem>();

        if (clsproperties.usedepartmentongendet || clsproperties.usecostcodeongendet || clsproperties.useprojectcodeongendet)
        {
            if (ViewState["costcodebreakdown"] != null)
            {
                breakdown = (List<cDepCostItem>)ViewState["costcodebreakdown"];
            }
            else
            {
                if (action == Action.Add)
                {
                    cDepCostItem[] lstCostCodes = reqemp.GetCostBreakdown().ToArray();
                    if (lstCostCodes.Length == 0)
                    {
                        breakdown.Add(new cDepCostItem(0, 0, 0, 100));
                    }
                    else
                    {
                        for (int i = 0; i < lstCostCodes.GetLength(0); i++)
                        {
                            breakdown.Add(lstCostCodes[i]);
                        }
                    }
                }
            }
        }
        #endregion Costcode breakdown

        #region department, costcodes and projectcodes

        int cellCount = 0;

        if (clsproperties.departmentson && clsproperties.usedepartmentcodes && clsproperties.usedepartmentongendet)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.CssClass = "labeltd";
            cell.Text = clsmisc.GetGeneralFieldByCode("department").description + ":";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "inputtd";
            ddlst = new DropDownList();
            ddlst.ID = "cmbgendepartment";

            var filterAttribute = this.ActionContext.FilterRules.FilterDropdown(FilterType.Department, "", ddlst.ID);
            if (!filterAttribute.IsNullOrEmpty())
            {
                ddlst.Attributes.Add(filterAttribute[0], filterAttribute[1]);
            }

            ddlst.Items.AddRange(this.ActionContext.Departments.CreateDropDown(clsproperties.usedepartmentdesc).ToArray());

            ddlst.Enabled = this.ActionContext.CurrentUser.CanEditDepartments;

            if (breakdown.Count > 0 && ddlst.Items.FindByValue(breakdown[0].departmentid.ToString()) != null)
            {
                ddlst.Items.FindByValue(breakdown[0].departmentid.ToString()).Selected = true;
            }

            cell.Controls.Add(ddlst);

            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = "&nbsp;";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = "&nbsp;";
            row.Cells.Add(cell);

            cellCount++;
        }

        if (clsproperties.costcodeson && clsproperties.usecostcodes && clsproperties.usecostcodeongendet)
        {
            if (cellCount == 0)
            {
                row = new TableRow();
            }

            cell = new TableCell();
            cell.CssClass = "labeltd";
            var costCodeField = clsmisc.GetGeneralFieldByCode("costcode");
            cell.Text = costCodeField.description + ":";
            row.Cells.Add(cell);

            //create searchbox 
            //apply filter on select 
       

            cell = new TableCell();
            cell.CssClass = "inputtd";
                
            txtbox = new TextBox
                         {
                             ID = "txtCostCode",
                             CssClass = reqProperties.UseCostCodeDescription ? "costcodeDescription-autocomplete" : "costcode-autocomplete"
                         };
                   
            txtbox.Attributes.Add("data-search", "General");
            txtbox.Attributes.Add("placeholder", "Search");
            txtbox.Enabled = this.ActionContext.CurrentUser.CanEditCostCodes;

        

            TextBox hiddenIdentifier = new TextBox { ID = "txtCostCode_ID" };
            hiddenIdentifier.Style.Add(HtmlTextWriterStyle.Display, "none");

            string[] filterAttribute = this.ActionContext.FilterRules.FilterDropdown(FilterType.Costcode, string.Empty, hiddenIdentifier.ID);


            if (!filterAttribute.IsNullOrEmpty())
            {

                txtbox.Attributes.Add(filterAttribute[0], filterAttribute[1]);
          
            }

            cCostCode costcode = null;

            if (action == Action.Edit || action == Action.Copy)
            {
                if (expenseitem.costcodebreakdown[0].costcodeid > 0)
                {
                    costcode = ActionContext.CostCodes.GetCostcodeById(expenseitem.costcodebreakdown[0].costcodeid);

                    if (costcode != null)
                    {
                        txtbox.Text = costcode.Costcode;
                        hiddenIdentifier.Text = costcode.CostcodeId.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
            else if (action == Action.Add && itemtype == ItemType.Cash)
            {
                if (breakdown.Count > 0 && breakdown[0].costcodeid != 0)

                {
                    costcode = ActionContext.CostCodes.GetCostcodeById(breakdown[0].costcodeid);

                    //employee has default costcode breakdown so predefine
                    txtbox.Text = costcode.Costcode;
                    hiddenIdentifier.Text = costcode.CostcodeId.ToString();
                }              
            }

            //don't forget validation or setting if show the description or code name

            cell.Controls.Add(txtbox);
            cell.Controls.Add(hiddenIdentifier);
            cell.Controls.Add(new Literal { Text = " " });

            row.Cells.Add(cell);

             cell = new TableCell
            {
                ColumnSpan = 2
            };

            if ((itemtype == ItemType.Cash && costCodeField.mandatory) || (itemtype == ItemType.CreditCard && costCodeField.mandatorycc) || (itemtype == ItemType.PurchaseCard && costCodeField.mandatorypc))
            {
                custval = new CustomValidator
                {
                    ClientValidationFunction = "SEL.Expenses.Validate.CostCode.GeneralDetailsMandatory",
                    ControlToValidate = "txtCostCode",
                    ID = "custCostCodeid",
                    ValidationGroup = "vgAeExpenses",
                    ValidateEmptyText = true,
                    ErrorMessage = "Please enter a valid " + costCodeField.description + ".",
                    Text = "*"
                };

                cell.Controls.Add(custval);
            }
            else
            {
                custval = new CustomValidator
                              {
                                  ClientValidationFunction = "SEL.Expenses.Validate.CostCode.GeneralDetailsNotMandatory",
                                  ControlToValidate = "txtCostCode",
                                  ID = "custCostCode",
                                  ValidationGroup = "vgAeExpenses",
                                  ValidateEmptyText = true,
                                  ErrorMessage = costCodeField.description + " is not valid. Please enter a value in the box provided.",
                                  Text = "*"
                              };

                cell.Controls.Add(custval);
            }
           
            row.Cells.Add(cell);
           
            cell = new TableCell();
            cell.Text = "&nbsp;";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = "&nbsp;";
            row.Cells.Add(cell);

            cellCount++;
        }

        if (cellCount == 2)
        {
            tbl.Rows.Add(row);
        }

        if (clsproperties.projectcodeson && clsproperties.useprojectcodes && clsproperties.useprojectcodeongendet)
        {
            if (cellCount == 0 || cellCount == 2)
            {
                row = new TableRow();
            }

            cell = new TableCell();
            cell.CssClass = "labeltd";
            cell.Text = clsmisc.GetGeneralFieldByCode("projectcode").description + ":";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "inputtd";
            ddlst = new DropDownList();
            ddlst.ID = "cmbgenprojectcode";

            var filterAttribute = this.ActionContext.FilterRules.FilterDropdown(FilterType.Projectcode, "", ddlst.ID);
            if (!filterAttribute.IsNullOrEmpty())
            {
                ddlst.Attributes.Add(filterAttribute[0], filterAttribute[1]);
            }

            ddlst.Items.AddRange(DropDownCreator.GetProjectCodesListItems(clsproperties.useprojectcodedesc, this.ProjectCodesRepository.Get(pc => pc.Archived == false)));

            ddlst.Enabled = this.ActionContext.CurrentUser.CanEditProjectCodes;

            if (breakdown.Count > 0 && ddlst.Items.FindByValue(breakdown[0].projectcodeid.ToString()) != null)
            {
                ddlst.Items.FindByValue(breakdown[0].projectcodeid.ToString()).Selected = true;
            }

            cell.Controls.Add(ddlst);

            row.Cells.Add(cell);


            cell = new TableCell();
            cell.Text = "&nbsp;";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = "&nbsp;";
            row.Cells.Add(cell);

            cellCount++;
        }

        if (cellCount != 0)
        {
            tbl.Rows.Add(row);
        }

        #endregion

        #region other details
        cFieldToDisplay otherdetails = clsmisc.GetGeneralFieldByCode("otherdetails");

        if (!otherdetails.individual && ((itemtype == ItemType.Cash && otherdetails.display) || (itemtype == ItemType.CreditCard && otherdetails.displaycc) || (itemtype == ItemType.PurchaseCard && otherdetails.displaypc)))
        {
            row = new TableRow();
            cell = new TableCell();
            cell.CssClass = "labeltd";
            cell.Text = otherdetails.description + ":";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.ColumnSpan = 5;
            txtbox = new TextBox();
            txtbox.ID = "txtotherdetails";
            txtbox.TextMode = TextBoxMode.MultiLine;
            txtbox.Rows = 3;
            txtbox.MaxLength = 2500;
            switch (action)
            {
                case Spend_Management.Action.Add:
                    switch (itemtype)
                    {
                        case ItemType.CreditCard:
                        case ItemType.PurchaseCard:
                            txtbox.Text = cardtransaction.description;
                            break;
                        case ItemType.Cash:
                            if (mobileItem != null)
                            {
                                txtbox.Text = mobileItem.OtherDetails;
                            }
                            else if (Session["otherdetails"] != null)
                            {
                                txtbox.Text = Session["otherdetails"].ToString();
                            }
                            break;
                    }
                    break;
                case Spend_Management.Action.Edit:
                case Spend_Management.Action.Copy:
                    txtbox.Text = expenseitem.reason;
                    break;
            }

            txtbox.Width = Unit.Percentage(100);
            cell.Controls.Add(txtbox);

            row.Cells.Add(cell);

            cell = new TableCell();
            cell.CssClass = "inputtd";
            if ((itemtype == ItemType.Cash && otherdetails.mandatory) || (itemtype == ItemType.CreditCard && otherdetails.mandatorycc) || (itemtype == ItemType.PurchaseCard && otherdetails.mandatorypc))
            {
                reqval = new RequiredFieldValidator();
                reqval.ID = "reqotherdetails";
                reqval.ControlToValidate = "txtotherdetails";
                reqval.ErrorMessage = otherdetails.description + " is a mandatory field. Please enter a value in the box provided";
                reqval.Text = "*";
                reqval.ValidationGroup = "vgAeExpenses";
                cell.Controls.Add(reqval);
            }
            else
            {
                cell.Text = "&nbsp;";
            }

            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "inputtd";
            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip244\" onclick=\"SEL.Tooltip.Show('ef1754d1-4f12-4efc-8c32-33e7cc91f934', 'ex', this);\" src=\"../static/icons/16/new-icons/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);
            row.Cells.Add(cell);
            tbl.Rows.Add(row);
        }
        #endregion

        #region userdefined
        cUserdefinedFields clsuserdefined = this.ActionContext.UserDefinedFields;
        cTables clstables = this.ActionContext.Tables;
        cTable udftbl = clstables.GetTableByID(new Guid("65394331-792e-40b8-af8b-643505550783"));

        if (expenseitem != null)
        {
            clsuserdefined.addItemsToPage(ref tbl, udftbl, false, "", expenseitem.userdefined, null, "vgAeExpenses");
        }
        else
        {
            SortedList<int, object> udfs;
            if (Session["genudfs"] != null)
            {
                udfs = (SortedList<int, object>)Session["genudfs"];
            }
            else
            {
                udfs = new SortedList<int, object>();
            }
            clsuserdefined.addItemsToPage(ref tbl, udftbl, false, "", udfs, null, "vgAeExpenses");
        }
        #endregion userdefined

        #region mobile device information panel

        if (action != Action.Add || mobileItem == null) return;

        bool infoAdded = false;
        StringBuilder sbInfo = new StringBuilder();

        sbInfo.Append("<table border=\"0\" cellpadding=\"5\"><tbody><tr><td>");
        sbInfo.Append("Additional information for expense item uploaded from mobile device:<br/>");
        if (!string.IsNullOrEmpty(mobileItem.ItemNotes))
        {
            sbInfo.Append("<strong>Notes:</strong><br />" + mobileItem.ItemNotes + "<br/><br/>");
            infoAdded = true;
        }

        if (!string.IsNullOrEmpty(mobileItem.FromLocation))
        {
            sbInfo.Append("<strong>From Location:</strong>&nbsp;" + mobileItem.FromLocation + "<br/><br/>");
            infoAdded = true;
        }
        if (!string.IsNullOrEmpty(mobileItem.ToLocation))
        {
            sbInfo.Append("<strong>To Location:</strong>&nbsp;" + mobileItem.ToLocation + "<br/><br/>");
            infoAdded = true;
        }
        if (mobileItem.Miles > 0)
        {
            sbInfo.Append("<strong>Distance:</strong>&nbsp;" + mobileItem.Miles.ToString() + "<br/><br/>");
            infoAdded = true;
        }
        sbInfo.Append("</td></tr></tbody></table>\n");

        if (!infoAdded) return;

        Panel mobileInfo = new Panel { ID = "pnlMobileInfo", CssClass = "inputpaneltitle" };
        Label mobileInfoLabel = new Label { ID = "lblMobileInfo", Text = "Mobile Expense Item Details" };
        mobileInfo.Controls.Add(mobileInfoLabel);
        pnlgeneral.Controls.Add(mobileInfo);

        tbl = new Table { ID = "tblMobileInfo" };
        pnlgeneral.Controls.Add(tbl);

        row = new TableRow();

        cell = new TableCell { CssClass = "comment", Text = sbInfo.ToString() };
        row.Cells.Add(cell);
        tbl.Rows.Add(row);

        #endregion mobile device information panel
    }

    /// <summary>
    /// Generates the Costcode break down control
    /// </summary>
    private void GenerateCostcodeBreakdown()
    {
        HiddenField hdnShowCostCodeDescription = (HiddenField)pnlgeneral.FindControl("hdnShowCostCodeDescription");
   
        if (hdnShowCostCodeDescription.Value == string.Empty)
        {
            hdnShowCostCodeDescription.Value = this.ActionContext.Properties.UseCostCodeDescription.ToString().ToLower();
        }

        var misc = this.ActionContext.Misc;
        cGlobalProperties globalProperties = misc.GetGlobalProperties((int)ViewState["accountid"]);

        #region create header

        string tmpTitle = string.Empty;

        if (globalProperties.usedepartmentcodes && globalProperties.departmentson && globalProperties.usedepartmentongendet == false)
        {
            tmpTitle += misc.GetGeneralFieldByCode("department").description + " / ";
        }

        if (globalProperties.usecostcodes && globalProperties.costcodeson && globalProperties.usecostcodeongendet == false)
        {
            tmpTitle += misc.GetGeneralFieldByCode("costcode").description + " / ";
        }

        if (globalProperties.useprojectcodes && globalProperties.projectcodeson && globalProperties.useprojectcodeongendet == false)
        {
            tmpTitle += misc.GetGeneralFieldByCode("projectcode").description + " / ";
        }

        if (tmpTitle.Length > 0)
        {
            tmpTitle = tmpTitle.Remove(tmpTitle.Length - 3, 3);
            litcostcodeheader.Text = string.Format("<div class=\"inputpanel\"><div class=\"inputpaneltitle\">{0} Breakdown</div>", tmpTitle);
        }
        else
        {
            return;
        }

        #endregion create header

        #region create body

        Table tbl = tblcostcodes;
        var action = (Action)ViewState["action"];
        var breakdown = new List<cDepCostItem>();
        var row = new TableRow();
        tbl.CssClass = "datatbl";
        tbl.ID = "tblcostcodes";
        var hCell = new TableHeaderCell();
        row.Cells.Add(hCell);

        if (globalProperties.departmentson && globalProperties.usedepartmentcodes && globalProperties.usedepartmentongendet == false)
        {
            hCell = new TableHeaderCell();
            hCell.Text = misc.GetGeneralFieldByCode("department").description;
            row.Cells.Add(hCell);
        }

        if (globalProperties.costcodeson && globalProperties.usecostcodes && globalProperties.usecostcodeongendet == false)
        {
            hCell = new TableHeaderCell();
            hCell.Text = misc.GetGeneralFieldByCode("costcode").description;
            row.Cells.Add(hCell);
        }

        if (globalProperties.projectcodeson && globalProperties.useprojectcodes && globalProperties.useprojectcodeongendet == false)
        {
            hCell = new TableHeaderCell();
            hCell.Text = misc.GetGeneralFieldByCode("projectcode").description;
            row.Cells.Add(hCell);
        }

        hCell = new TableHeaderCell();
        hCell.Text = "Percentage";
        row.Cells.Add(hCell);
        tbl.Rows.Add(row);

        if (ViewState["costcodebreakdown"] != null)
        {
            breakdown = (List<cDepCostItem>)ViewState["costcodebreakdown"];
        }
        else
        {
            if (action == Action.Add)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                cDepCostItem[] lstCostCodes = this.ActionContext.CurrentUser.Employee.GetCostBreakdown().ToArray();

                if (lstCostCodes.Length == 0)
                {
                    breakdown.Add(new cDepCostItem(0, 0, 0, 100));
                }
                else
                {
                    for (int i = 0; i < lstCostCodes.GetLength(0); i++)
                    {
                        breakdown.Add(lstCostCodes[i]);
                    }
                }

                ViewState["costcodebreakdown"] = breakdown;
            }
        }

        int count = 0;
        int total = 0;

        foreach (cDepCostItem item in breakdown)
        {
            total += item.percentused;
            tbl.Rows.Add(AddCostcodeRow(count, item));
            count++;
        }

        txtcostcodetotal.Text = total.ToString();

        var txtrows = new TextBox();
        txtrows.Text = (tbl.Rows.Count - 1).ToString();
        ViewState["breakdownrows"] = txtrows.Text;
        txtrows.Style.Add("display", "none");
        txtrows.ID = "txtrows";
        row = new TableRow();
        row.Style.Add("display", "none");
        var cell = new TableCell();
        cell.Controls.Add(txtrows);
        row.Cells.Add(cell);

        tbl.Rows.Add(row);

        #endregion create body

        #region create footer

        litcostcodefooter.Text = "</div>";

        #endregion create footer
    }

    /// <summary>
    /// Adds a costcode row to the costcode breakdown control
    /// </summary>
    /// <param name="index">The index of the DepCostItem</param>
    /// <param name="breakdownItem">The DepCostItem</param>
    /// <returns>A TableRow</returns>
    private TableRow AddCostcodeRow(int index, cDepCostItem breakdownItem)
    {
        var misc = this.ActionContext.Misc;
        cGlobalProperties globalProperties = misc.GetGlobalProperties((int)ViewState["accountid"]);
        var departments = this.ActionContext.Departments;
        CurrentUser user = cMisc.GetCurrentUser();
        DropDownList ddlst;
        var row = new TableRow();
        var cell = new TableCell();

        if (index > 0)
        {
            var delete = new ImageButton
            {
                CommandArgument = index.ToString(),
                CausesValidation = false,
                ID = "cmddelete" + index,
                ImageUrl = "icons/delete2.gif"
            };
            delete.Click += delete_Click;
            cell.Controls.Add(delete);
        }

        row.Cells.Add(cell);

        var itemtype = (ItemType)ViewState["itemtype"];

        if (globalProperties.departmentson && globalProperties.usedepartmentcodes && globalProperties.usedepartmentongendet == false)
        {
            cell = new TableCell();
            ddlst = new DropDownList();
            ddlst.ID = "cmbdepartment" + index;

            string[] filterAttribute = this.ActionContext.FilterRules.FilterDropdown(FilterType.Department, index.ToString(), ddlst.ID);

            if (!filterAttribute.IsNullOrEmpty())
            {
                ddlst.Attributes.Add(filterAttribute[0], filterAttribute[1]);
            }

            ddlst.Items.AddRange(departments.CreateDropDown(globalProperties.usedepartmentdesc, true).ToArray());
            ddlst.Enabled = this.ActionContext.CurrentUser.CanEditDepartments;

            if (ddlst.Items.FindByValue(breakdownItem.departmentid.ToString()) != null)
            {
                ddlst.SelectedValue = breakdownItem.departmentid.ToString();
            }

            cell.Controls.Add(ddlst);
            row.Cells.Add(cell);

            if (user.CanEditDepartments)
            {
                cFieldToDisplay dept = misc.GetGeneralFieldByCode("department");

                if ((itemtype == ItemType.Cash && dept.mandatory) || (itemtype == ItemType.CreditCard && dept.mandatorycc) || (itemtype == ItemType.PurchaseCard && dept.mandatorypc))
                {
                    CompareValidator compDeptVal = GenerateCompareValidator(ddlst.ID, dept.description, "0");
                    cell.Controls.Add(compDeptVal);
                    row.Cells.Add(cell);
                }
            }
        }



        if (globalProperties.costcodeson && globalProperties.usecostcodes && globalProperties.usecostcodeongendet == false)
        {
            cell = new TableCell();
            var textBox = new TextBox();


      

            textBox.ID = "txtCostCode" + index;


            //if filter rules apply then apply then later with autocomplete bind, else apply the auto complete 

        
                textBox.CssClass = this.ActionContext.Properties.UseCostCodeDescription
                                       ? "costcodeDescription-autocomplete"
                                       : "costcode-autocomplete"; 
     

            textBox.Attributes.Add("data-search", "General");
            textBox.Attributes.Add("placeholder", "Search");

            textBox.Enabled = this.ActionContext.CurrentUser.CanEditCostCodes;


            TextBox hiddenIdentifier = new TextBox { ID = "txtCostCode" + index + "_ID"};          
            hiddenIdentifier.Style.Add(HtmlTextWriterStyle.Display, "none");

            
            string[] filterAttribute = this.ActionContext.FilterRules.FilterDropdown(FilterType.Costcode, index.ToString(), hiddenIdentifier.ID);


            if (!filterAttribute.IsNullOrEmpty())
            {

                textBox.Attributes.Add(filterAttribute[0], filterAttribute[1]);
          
            }

            //pre populate if edit

            var costCode = this.ActionContext.CostCodes.GetCostcodeById(breakdownItem.costcodeid);

            if (costCode != null)
            {
                textBox.Text = costCode.Costcode;
                hiddenIdentifier.Text = costCode.CostcodeId.ToString(); 
            }




            //filters

            cell.Controls.Add(textBox);
            cell.Controls.Add(hiddenIdentifier);
            row.Cells.Add(cell);

            if (user.CanEditCostCodes)
            {
                cFieldToDisplay costCodeField = misc.GetGeneralFieldByCode("costcode");

                if ((itemtype == ItemType.Cash && costCodeField.mandatory) || (itemtype == ItemType.CreditCard && costCodeField.mandatorycc) || (itemtype == ItemType.PurchaseCard && costCodeField.mandatorypc))
                {
                  
                  


                   var custval = new CustomValidator
                                  {
                                      ClientValidationFunction = "SEL.Expenses.Validate.CostCode.GeneralDetailsMandatory",
                                      ControlToValidate = textBox.ID,
                                      ID = "custCostCodeid" + index,
                                      ValidationGroup = "vgAeExpenses",
                                      ValidateEmptyText = true,
                                      ErrorMessage = "Please enter a valid " + costCodeField.description + ".",
                                      Text = "*", AccessKey = index.ToString()
                                  };

                    cell.Controls.Add(custval);
                    row.Cells.Add(cell);
                }
            }
        

            //cell = new TableCell();
            //ddlst = new DropDownList();
            //ddlst.ID = "cmbcostcode" + index;

            //string[] filterAttribute = this.ActionContext.FilterRules.FilterDropdown(FilterType.Costcode, index.ToString(), ddlst.ID);

            //if (!filterAttribute.IsNullOrEmpty())
            //{
            //    ddlst.Attributes.Add(filterAttribute[0], filterAttribute[1]);
            //}

            //ddlst.Enabled = this.ActionContext.CurrentUser.CanEditCostCodes;
            //ddlst.Items.AddRange(this.ActionContext.CostCodes.CreateDropDown(globalProperties.usecostcodedesc, includeNoneOption: true).ToArray());

            //if (ddlst.Items.FindByValue(breakdownItem.costcodeid.ToString()) != null)
            //{
            //    ddlst.SelectedValue = breakdownItem.costcodeid.ToString();
            //}

            //cell.Controls.Add(ddlst);
            //row.Cells.Add(cell);

            //if (user.CanEditCostCodes)
            //{
            //    cFieldToDisplay cc = misc.GetGeneralFieldByCode("costcode");

            //    if ((itemtype == ItemType.Cash && cc.mandatory) || (itemtype == ItemType.CreditCard && cc.mandatorycc) || (itemtype == ItemType.PurchaseCard && cc.mandatorypc))
            //    {
            //        CompareValidator compCostCodeVal = GenerateCompareValidator(hiddenIdentifier.Text, cc.description);
            //        cell.Controls.Add(compCostCodeVal);
            //        row.Cells.Add(cell);
            //    }
            // }
            }

        if (globalProperties.projectcodeson && globalProperties.useprojectcodes && globalProperties.useprojectcodeongendet == false)
        {
            cell = new TableCell();

            ddlst = new DropDownList();
            ddlst.ID = "cmbprojectcode" + index;

            string[] filterAttribute = this.ActionContext.FilterRules.FilterDropdown(FilterType.Projectcode, index.ToString(), ddlst.ID);

            if (!filterAttribute.IsNullOrEmpty())
            {
                ddlst.Attributes.Add(filterAttribute[0], filterAttribute[1]);
            }

            ddlst.Items.AddRange(DropDownCreator.GetProjectCodesListItems(globalProperties.useprojectcodedesc, this.ProjectCodesRepository.Get(pc => pc.Archived == false), true));
            ddlst.Enabled = this.ActionContext.CurrentUser.CanEditProjectCodes;

            if (ddlst.Items.FindByValue(breakdownItem.projectcodeid.ToString()) != null)
            {
                ddlst.SelectedValue = breakdownItem.projectcodeid.ToString();
            }

            cell.Controls.Add(ddlst);
            row.Cells.Add(cell);

            if (user.CanEditProjectCodes)
            {
                cFieldToDisplay project = misc.GetGeneralFieldByCode("projectcode");

                if ((itemtype == ItemType.Cash && project.mandatory) || (itemtype == ItemType.CreditCard && project.mandatorycc) || (itemtype == ItemType.PurchaseCard && project.mandatorypc))
                {
                    CompareValidator compDeptVal = GenerateCompareValidator(ddlst.ID, project.description, "0");
                    cell.Controls.Add(compDeptVal);
                    row.Cells.Add(cell);
                }
            }
        }

        cell = new TableCell();

        var txtbox = new TextBox
        {
            ID = "txtpercentage" + index,
            Text = breakdownItem.percentused.ToString(),
            AutoPostBack = true
        };

        txtbox.TextChanged += txtpercentage_TextChanged;


        //var filterRules = new cFilterRules((int)ViewState["accountid"]);
        //filterRules.filterTextbox(ref txtbox,FilterType.Costcode, index.ToString());

        cell.Controls.Add(txtbox);

        var compPercentageVal = new CompareValidator
        {
            ControlToValidate = "txtpercentage" + index,
            Type = ValidationDataType.Integer,
            Operator = ValidationCompareOperator.GreaterThanEqual,
            ValueToCompare = "0",
            Text = "*",
            ValidationGroup = "vgAeExpenses",
            ErrorMessage = "The percentage breakdown you have entered is invalid"
        };

        cell.Controls.Add(compPercentageVal);

        var filter = new FilteredTextBoxExtender
        {
            TargetControlID = "txtpercentage" + index,
            FilterType = FilterTypes.Numbers
        };

        cell.Controls.Add(filter);
        row.Cells.Add(cell);

        return row;
    }

    /// <summary>
    /// Creates a Compare Validator for a drop down list to enforce a required field. 
    /// </summary>
    /// <param name="controlId">
    /// The Id of the control to validate 
    /// </param>
    /// <param name="fieldDescription">
    /// The field description
    /// </param>
    /// <param name="valueToCompare">
    /// The value To Compare.
    /// </param>
    /// <returns>
    /// </returns>
    private static CompareValidator GenerateCompareValidator(string controlId, string fieldDescription, string valueToCompare)
    {
        var compVal = new CompareValidator
        {
            ControlToValidate = controlId,
            ErrorMessage = string.Format("{0} is a mandatory field. Please select a value", fieldDescription),
            Text = "*",
            Type = ValidationDataType.String,
            Operator = ValidationCompareOperator.NotEqual,
            ValueToCompare = valueToCompare,
            ValidationGroup = "vgAeExpenses"
        };
        return compVal;
    }


    void delete_Click(object sender, ImageClickEventArgs e)
    {
        Table tbl = tblcostcodes;
        ImageButton btn = (ImageButton)sender;
        int index = int.Parse(btn.CommandArgument);
        List<cDepCostItem> items = getCostcodeBreakdown(tbl);
        items.RemoveAt(index);
        ViewState["costcodebreakdown"] = items;
        tbl.Rows.Clear();


        GenerateCostcodeBreakdown();
        filterDropdownsOnPageStart();
    }
    void txtpercentage_TextChanged(object sender, EventArgs e)
    {
        Table tbl = tblcostcodes;

        int totalpercentage = 0;
        List<cDepCostItem> items = getCostcodeBreakdown(tbl);

        foreach (cDepCostItem item in items)
        {
            totalpercentage += item.percentused;
        }

        if (totalpercentage < 100)
        {
            int percentspare = 100 - totalpercentage;
            cDepCostItem lastitem = items[items.Count - 1];
            cDepCostItem newitem = new cDepCostItem(lastitem.departmentid, lastitem.costcodeid, lastitem.projectcodeid, percentspare);
            //tbl.Rows.Add(addCostcodeRow(tbl.Rows.Count,newitem));
            items.Add(newitem);
        }

        ViewState["costcodebreakdown"] = items;
        tbl.Rows.Clear();


       GenerateCostcodeBreakdown();
     
        filterDropdownsOnPageStart();
    }

    private List<cDepCostItem> getCostcodeBreakdown(Table tbl)
    {
        int projectcodeid = 0;
        int departmentid = 0;
        int costcodeid = 0;
        int percentused = 0;


        DropDownList ddlst;
        List<cDepCostItem> items = new List<cDepCostItem>();
        cMisc clsmisc = this.ActionContext.Misc;
        cGlobalProperties clsproperties = clsmisc.GetGlobalProperties((int)ViewState["accountid"]);
        TextBox txtbox;
        cDepCostItem item;
        int breakdownCount;

        bool displayPercentageGrid = false;
        if ((clsproperties.departmentson && !clsproperties.usedepartmentongendet) || (clsproperties.costcodeson && !clsproperties.usecostcodeongendet) || (clsproperties.projectcodeson && !clsproperties.useprojectcodeongendet))
        {
            displayPercentageGrid = true;
        }

        if (!displayPercentageGrid)
        {
            breakdownCount = 2;
        }
        else
        {
            breakdownCount = tbl.Rows.Count - 1;
        }

        for (int i = 1; i < breakdownCount; i++)
        {
            if (clsproperties.usedepartmentcodes && clsproperties.departmentson && clsproperties.usedepartmentongendet == false)
            {
                ddlst = (DropDownList)tbl.FindControl("cmbdepartment" + (i - 1));
                int.TryParse(Request[ddlst.ClientID.Replace("_", "$")], out departmentid);
                }
            else if (clsproperties.usedepartmentcodes && clsproperties.departmentson && clsproperties.usedepartmentongendet)
            {
                ddlst = (DropDownList)pnlgeneral.FindControl("cmbgendepartment");
                int.TryParse(Request[ddlst.ClientID.Replace("_", "$")], out departmentid);
                }

            if (clsproperties.usecostcodes && clsproperties.costcodeson && clsproperties.usecostcodeongendet == false)
            {
                var cctxtboxBreakdown = (TextBox)tbl.FindControl("txtCostCode" + (i -1) + "_ID");
                if (cctxtboxBreakdown.Text != string.Empty)
                {
                       costcodeid = Convert.ToInt32(cctxtboxBreakdown.Text);
                }
               
            }
            else if (clsproperties.usecostcodes && clsproperties.costcodeson && clsproperties.usecostcodeongendet)
            {
               var cctxtbox = (TextBox)pnlgeneral.FindControl("txtCostCode_ID");
      
                costcodeid = Convert.ToInt32(cctxtbox.Text);
                //ddlst = (DropDownList)pnlgeneral.FindControl("cmbgencostcode");
                //int.TryParse(Request[ddlst.ClientID.Replace("_", "$")], out costcodeid);
            }

            if (clsproperties.useprojectcodes && clsproperties.projectcodeson && clsproperties.useprojectcodeongendet == false)
            {
                ddlst = (DropDownList)tbl.FindControl("cmbprojectcode" + (i - 1));
                int.TryParse(Request[ddlst.ClientID.Replace("_", "$")], out projectcodeid);
                }
            else if (clsproperties.useprojectcodes && clsproperties.projectcodeson && clsproperties.useprojectcodeongendet)
            {
                ddlst = (DropDownList)pnlgeneral.FindControl("cmbgenprojectcode");
                int.TryParse(Request[ddlst.ClientID.Replace("_", "$")], out projectcodeid);
                }

            txtbox = (TextBox)tbl.FindControl("txtpercentage" + (i - 1));

            if (txtbox == null)
            {
                percentused = 100;
            }
            else
            {
                int.TryParse(Request[txtbox.ClientID.Replace("_", "$")], out percentused);
                }

            item = new cDepCostItem(departmentid, costcodeid, projectcodeid, percentused);
            items.Add(item);
        }

        return items;
    }
    private void generateSpecificDetails()
    {
        cAccountProperties reqProperties = this.ActionContext.Properties;

        Literal lit;
        List<int> items = new List<int>();
        cEmployees clsemployees = this.ActionContext.Employees;
        Employee reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);
        Action action = (Action)ViewState["action"];
        ItemType itemtype = (ItemType)ViewState["itemtype"];
        cExpenseItem expenseitem = null;
        int transactionid = (int)ViewState["transactionid"];
        SortedList<int, cExpenseItem> expitems = (SortedList<int, cExpenseItem>)ViewState["items"];
        bool cardAutomaticAllocation = false;

        ExpenseItem mobileItem = null;
        if (ViewState["mobileID"] != null && (int)ViewState["mobileID"] > 0)
        {
            int mobileID = (int)ViewState["mobileID"];
            cMobileDevices clsmobile = this.ActionContext.MobileDevices;
            mobileItem = clsmobile.getMobileItemByID(mobileID);
        }

        MobileJourney mobileJourney = null;
        if (ViewState["mobileJourneyID"] != null && (int)ViewState["mobileJourneyID"] > 0)
        {
            int journeyID = (int)ViewState["mobileJourneyID"];
            cMobileDevices clsmobile = this.ActionContext.MobileDevices;
            mobileJourney = clsmobile.GetMobileJourney(journeyID);
        }

        if (action == Spend_Management.Action.Add)
        {
            if (transactionid > 0)
            {
                cCardTransaction transaction = this.ActionContext.CardStatements.getTransactionById(transactionid);
                cCardStatement statement = this.ActionContext.CardStatements.getStatementById(transaction.statementid);

                if (statement.Corporatecard.allocateditem != null)
                {
                    items.Add((int)statement.Corporatecard.allocateditem);
                    cardAutomaticAllocation = true;
                }
                else
                {
                    items = reqemp.GetSubCategories().SubCategories;
                }
            }
            else if (mobileItem != null)
            {
                items.Add(mobileItem.SubcatID);
            }
            else if (mobileJourney != null)
            {
                items.Add(mobileJourney.SubcatId);
            }

            else
            {
                items = reqemp.GetSubCategories().SubCategories;
            }
        }
        else if (action == Spend_Management.Action.Edit || action == Action.Copy)
        {
            expenseitem = (cExpenseItem)ViewState["expenseitem"];
            items.Add(expenseitem.subcatid);
            if (expenseitem.WorkAddressId != 0 && string.IsNullOrEmpty(this.hdnWorkAddressId.Value))
            {
                this.hdnWorkAddressId.Value = expenseitem.WorkAddressId.ToString();
            }
        }




        int countryid = reqProperties.HomeCountry;
        int currencyid = (int)reqProperties.BaseCurrency;
        if (ViewState["currencyid"] != null)
        {
            currencyid = (int)ViewState["currencyid"];
        }
        if (ViewState["countryid"] != null)
        {
            countryid = (int)ViewState["countryid"];
        }
        TextBox txtDateOfExpense = (TextBox)pnlgeneral.FindControl("txtdate");
        DateTime expenseItemDate = Convert.ToDateTime(txtDateOfExpense.Text);
        cItemBuilder clsbuilder;
        if (transactionid > 0 || mobileItem != null)
        {
            clsbuilder = new cItemBuilder((int)ViewState["accountid"], (int)ViewState["employeeid"], expenseItemDate, (int)ViewState["subAccountID"], itemtype, transactionid, currencyid, countryid, (int?)ViewState["mobileID"], null);
        }
        else if (mobileJourney != null)
        {
            clsbuilder = new cItemBuilder((int)ViewState["accountid"], (int)ViewState["employeeid"], expenseItemDate, (int)ViewState["subAccountID"], itemtype, transactionid, currencyid, countryid, null, mobileJourney.JourneyId);
        }
        else
        {
            clsbuilder = new cItemBuilder((int)ViewState["accountid"], (int)ViewState["employeeid"], expenseItemDate, (int)ViewState["subAccountID"], currencyid, countryid, itemtype, this.Page);
        }
        cGlobalCurrencies clsglobalcurrencies = this.ActionContext.GlobalCurrencies;
        var subcats = this.ActionContext.SubCategories;
        cSubcat reqsubcat;

        cCurrencies clscurrencies = this.ActionContext.Currencies;

        TextBox txtnumitems = new TextBox();

        List<SubcatItemRoleBasic> limits = subcats.GetSubCatsByEmployeeItemRoles((int)this.ViewState["employeeid"], true);

        //get the current date of the xpense item being claimed
        HiddenField hdnDate = (HiddenField)pnlgeneral.FindControl("hdnExpdate");
        DateTime date = DateTime.Now;

        if (hdnDate.Value != "")
        {
            DateTime.TryParse(hdnDate.Value, out date);
        }

        if (items.Count == 0 || action == Action.Edit || action == Action.Copy || ((int)ViewState["transactionid"] > 0 && cardAutomaticAllocation == false) || (int)ViewState["mobileJourneyID"] > 0) //need dd boxes
        {
            pnlspecific.Controls.Clear();
            lit = new Literal();
            lit.Text = "<div class=\"inputpaneltitle\">Specific Details</div>";
            pnlspecific.Controls.Add(lit);

            if (mobileJourney != null)
            {
                reqsubcat = subcats.GetSubcatById(mobileJourney.SubcatId);
                pnlspecific.Controls.Add(generateDropDowns(null, mobileJourney));

                pnlspecific.Controls.Add(clsbuilder.generateItem("0", reqsubcat, null, null, false, date, reqemp, Request, this.ActionContext));
            }
            else if (ViewState["subcatid"] != null)
            {
                pnlspecific.Controls.Add(generateDropDowns(null, null));
                var subcatid = (int)ViewState["subcatid"];
                reqsubcat = subcats.GetSubcatById(subcatid);
                if (expitems.ContainsKey(subcatid))
                {
                    expenseitem = expitems[subcatid];
                }

                if (expenseitem == null)
                {
                    pnlspecific.Controls.Add(clsbuilder.generateItem("0", reqsubcat, null, null, false, date, reqemp, Request, this.ActionContext));
                }
                else
                {
                    pnlspecific.Controls.Add(clsbuilder.generateItem("0", reqsubcat, null, expenseitem, false, date, reqemp, Request, this.ActionContext));
                }
            }
            else
            {
                // get default values
                if (action == Action.Edit || action == Action.Copy)
                {

                    expenseitem = expitems.Values[0];
                    int subcatid = expenseitem.subcatid;
                    reqsubcat = subcats.GetSubcatById(subcatid);
                    pnlspecific.Controls.Add(generateDropDowns(expenseitem, null));
                  
                    if (expenseitem != null && expenseitem.journeysteps !=null)
                    {
                        if (expenseitem.journeysteps.Count > 0)
                        {
                            ViewState["journeysteps"] = expenseitem.journeysteps;
                        }
                        if (expenseitem.journeysteps.Count == 0 && ViewState["journeysteps"] !=null)
                        {
                          expenseitem.journeysteps = (SortedList<int,cJourneyStep>)ViewState["journeysteps"];
                        }
                    }

                    pnlspecific.Controls.Add(clsbuilder.generateItem("0", reqsubcat, null, expenseitem, false, action == Action.Edit && hdnDate.Value == "" ? expenseitem.date : date, reqemp, Request, this.ActionContext));
                }
                else if (mobileJourney != null)
                {
                    reqsubcat = subcats.GetSubcatById(mobileJourney.SubcatId);
                    pnlspecific.Controls.Add(generateDropDowns(null, mobileJourney));

                    pnlspecific.Controls.Add(clsbuilder.generateItem("0", reqsubcat, null, null, false, date, reqemp, Request, this.ActionContext));
                }
                else
                {
                    pnlspecific.Controls.Add(generateDropDowns(null, null));
                }

                txtnumitems.Text = "1";
                txtnumitems.Style.Add("display", "none");
                txtnumitems.ID = "txtnumitems";
                ViewState["numitems"] = txtnumitems.Text;
                pnlspecific.Controls.Add(txtnumitems);
            }
        }
        else
        {

            if (action == Action.Add || ((action == Action.Edit || action == Action.Copy) && ((int)ViewState["transactionid"] > 0 && cardAutomaticAllocation)))
            {
                FlagManagement flagManagement = this.ActionContext.FlagManagement;
                int itemRoleID;
                txtnumitems.Text = items.Count.ToString();
                txtnumitems.Style.Add("display", "none");
                txtnumitems.ID = "txtnumitems";
                ViewState["numitems"] = txtnumitems.Text;
                pnlspecific.Controls.Add(txtnumitems);
                for (int i = 0; i < items.Count; i++)
                {
                    reqsubcat = subcats.GetSubcatById((int)items[i]);
                    if (reqsubcat != null)
                    {
                        expitems.TryGetValue(reqsubcat.subcatid, out expenseitem);
                        lit = new Literal();
                        lit.Text = "<div class=\"inputpaneltitle\">" + reqsubcat.subcat;
                        SubcatItemRoleBasic roleSubcat = limits.OrderByDescending(x => x.Maximum).FirstOrDefault(subcat => subcat.SubcatId == reqsubcat.subcatid);
                           
                        LimitFlag limitFlag = null;
                        if (roleSubcat != null)
                        {
                            limitFlag = (LimitFlag)
                                flagManagement.GetFlagByTypeRoleAndExpenseItem(
                                    FlagType.LimitWithReceipt,
                                    roleSubcat.ItemRoleID,
                                    reqsubcat.subcatid);
                        }
                        if (limitFlag != null && limitFlag.DisplayLimit && limitFlag.Active)
                        {
                            

                            if (roleSubcat != null)
                            {
                                if (roleSubcat.Maximum != 0)
                                {
                                    cCurrency clsbasecurrency = reqemp.PrimaryCurrency != 0 ? clscurrencies.getCurrencyById(reqemp.PrimaryCurrency) : clscurrencies.getCurrencyById((int)reqProperties.BaseCurrency);

                                    lit.Text += "<span align=\"right\">&nbsp;&nbsp;Your Limit:&nbsp;";

                                    if (clsbasecurrency != null)
                                    {
                                        lit.Text += clsglobalcurrencies.getGlobalCurrencyById(clsbasecurrency.globalcurrencyid).symbol;
                                    }
                                    lit.Text += roleSubcat.Maximum.ToString("###,###,##0.00");
                                    lit.Text += "</span>";
                                }
                            }
                        }

                        lit.Text += "</div>";
                        pnlspecific.Controls.Add(lit);
                        if (expenseitem != null && expenseitem.journeysteps != null)
                        {
                            if (expenseitem.journeysteps.Count > 0)
                            {
                               ViewState[expenseitem.subcatid.ToString()] = expenseitem.journeysteps;
                            }
                            if (expenseitem.journeysteps.Count == 0 && ViewState[expenseitem.subcatid.ToString()] != null)
                            {
                                expenseitem.journeysteps = (SortedList<int, cJourneyStep>)ViewState[expenseitem.subcatid.ToString()];
                            }
                        }
                        pnlspecific.Controls.Add(clsbuilder.generateItem(i.ToString(), reqsubcat, null, expenseitem, false, date, reqemp, Request, this.ActionContext));
                    }
                }
            }
        }
    }

    private int getClaimId(int employeeid)
    {

        int claimid = 0;
        if (Request.QueryString["claimid"] != null)
        {
            claimid = int.Parse(Request.QueryString["claimid"]);
        }
        else
        {
            cClaims clsclaims = this.ActionContext.Claims;
            claimid = clsclaims.getDefaultClaim(ClaimStage.Current, employeeid);
            if (claimid == 0)
            {

                cMisc clsmisc = this.ActionContext.Misc;
                cGlobalProperties clsproperties = clsmisc.GetGlobalProperties((int)ViewState["accountid"]);
                if (clsproperties.singleclaim)
                {
                    claimid = clsclaims.insertDefaultClaim(employeeid);
                    if (claimid == -1)//The next claim number has already been used, so find the next available.
                    {
                        var loopCount = 0;
                        var employees = this.ActionContext.Employees;
                        while (claimid == -1 && loopCount < 50)
                        {
                            Employee employee = employees.GetEmployeeById(employeeid);
                            employee.IncrementClaimNumber(this.ActionContext.CurrentUser);
                            claimid = clsclaims.insertDefaultClaim(employeeid);
                            loopCount++;
                        }
                    }

                }
                else
                {
                    Response.Redirect(cMisc.Path + "/aeclaim.aspx?returnto=2", true);
                }
            }
        }
        return claimid;
    }



    private string createSubcatList(int employeeid)
    {
        var subcatDates = new Dictionary<string, List<SubCatDates>>();
        var clsemployees = this.ActionContext.Employees;
        var subcats = this.ActionContext.SubCategories;
        Employee reqemp = clsemployees.GetEmployeeById(employeeid);
        var itemtype = (ItemType) ViewState["itemtype"];
        var action = (int) ViewState["action"];
        var addControl = (itemtype == ItemType.Cash && action != 2 && action != 3 && action != 4 && (int) ViewState["mobileJourneyID"] == 0);
 
        List<SubcatItemRoleBasic> roleitems = subcats.GetSubCatsByEmployeeItemRoles(employeeid, true);
        var sortedlst = new SortedList<string, ListItem>();
        foreach (SubcatItemRoleBasic rolesub in roleitems)
        {
            if (addControl)
            {
                ListItem item;
                var subCatText = string.IsNullOrEmpty(rolesub.ShortSubcat) ? rolesub.Subcat : rolesub.ShortSubcat;
                item = new ListItem(
                    $"<div style=\"display:inline; overflow: hidden\" id=\"subcatitem{rolesub.SubcatId}\"  startdate=\"{rolesub.StartDate.ToString("yyyy-MM-dd")}\" enddate=\"{rolesub.EndDate.ToString("yyyy-MM-dd")} class=\"subcatlistitem\"  >{subCatText.Replace(" ", "&nbsp;")}</div>",
                    rolesub.SubcatId.ToString(CultureInfo.InvariantCulture));
                
                
                if (rolesub.Subcat.Length > 11)
                {
                    item.Attributes.Add("onmouseover",
                        $"SEL.Tooltip.customToolTip(\"subcatitem{rolesub.SubcatId}\",\"{rolesub.Subcat}\");");
                    item.Attributes.Add("onmouseout", "SEL.Tooltip.hideTooltip();");
                }
                

                item.Attributes.Add("subcat", rolesub.SubcatId.ToString());
                item.Attributes.Add("class", "subcatlistitem");
                if (reqemp.GetSubCategories().Contains(rolesub.SubcatId))
                {
                    item.Selected = true;
                }

                if (!sortedlst.ContainsKey(subCatText))
                {
                    sortedlst.Add(subCatText, item);
                }
            }
            

            List<SubCatDates> dates;

            if (subcatDates.ContainsKey(rolesub.SubcatId.ToString()))
            {
                dates = subcatDates[rolesub.SubcatId.ToString()];
            }
            else
            {
                dates = new List<SubCatDates>();
                subcatDates.Add(rolesub.SubcatId.ToString(), dates);
            }

            dates.Add(new SubCatDates(rolesub.StartDate, rolesub.EndDate));
        }

        if (addControl)
        {
            
            
            foreach (ListItem litem in sortedlst.Values.DistinctBy(x => x.Value))
            {
                chkitems.Items.Add(litem);
            }

            chkitems.Style.Add("display", "block");
            chkitems.Style.Add("position", "relative");
            chkitems.Style.Add("width", "113px");
            chkitems.Style.Add(HtmlTextWriterStyle.Overflow, "hidden");
            chkitems.Style.Add("z-index", "2");
        }
        

        return new JavaScriptSerializer().Serialize(subcatDates);
    }

    protected void chkitems_SelectedIndexChanged(object sender, EventArgs e)
    {
        List<int> checkeditems = new List<int>();
        this.getItems();
        cEmployees clsemployees = this.ActionContext.Employees;
        Employee reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);

        List<int> items = reqemp.GetSubCategories().SubCategories;

        for (int i = 0; i < chkitems.Items.Count; i++)
        {
            if (chkitems.Items[i].Selected)
            {
                checkeditems.Add(int.Parse(chkitems.Items[i].Value));
            }
        }

        if (items.Count < checkeditems.Count) //item added
        {
            foreach (int item in checkeditems)
            {
                if (items.Contains(item) == false)
                {
                    // add it
                    reqemp.GetSubCategories().Add(item);
                }
            }
        }
        else if (checkeditems.Count < items.Count) //item removed
        {
            foreach (int item in items)
            {
                if (checkeditems.Contains(item) == false)
                {
                    reqemp.GetSubCategories().Remove(item);
                }
            }
        }

        pnlspecific.Controls.Clear();
        generateSpecificDetails();
        upnlSpecific.Update();
        var claimSubmitted = (bool?)this.ViewState["submitted"] ?? false;
        ScriptManager.RegisterStartupScript(upnlSpecific, GetType(), "mileagegridrefresh", "$(document).ready(function() {refreshMileagePanel('" + claimSubmitted + "'); });", true);
    }

    protected void cmdok_Click(object sender, EventArgs eventArgs)
    {
        var expenseItems = this.ActionContext.ExpenseItems;
        var expenseItemPaid = (bool)this.ViewState["Paid"];
        SortedList<int, cExpenseItem> items = this.getItems(expenseItemPaid);
        int claimid = 0;
        int returncode;
        if (items.Count > 0)
        {
            claimid = items.Values[0].claimid;
        }
        else
        {
            claimid = (int)ViewState["claimid"];
        }
        cClaims claims = this.ActionContext.Claims;
        cClaim claim = claims.getClaimById(claimid);

        Employee reqEmp = this.ActionContext.Employees.GetEmployeeById((int)ViewState["employeeid"]);

        var action = (Action)ViewState["action"];

        string strLimit = string.Empty;
        
        cAccountProperties reqProperties = this.ActionContext.Properties;


        if (this.ActionContext.CurrentUser.Account.IsNHSCustomer && reqProperties.CheckESRAssignmentOnEmployeeAdd)
        {
            foreach (cExpenseItem item in items.Values)
            {
                if (item.ESRAssignmentId == 0)
                {
                    lblmsg.Text = "The item cannot be added as you do not have a valid assignment number for the date entered.";
                    lblmsg.Visible = true;
                    return;
                }
            }
        }

        var subcats = this.ActionContext.SubCategories;


        var employeeItemRoles = reqEmp.GetItemRoles();

        if (action == Action.Edit)
        {
            var reqExpenseItem = (cExpenseItem)items.Values[0];
            var roleMessage = this.ValidateEmployeeItemRoles(reqExpenseItem, subcats, employeeItemRoles);
            if (!string.IsNullOrEmpty(roleMessage))
            {
                this.lblmsg.Text = roleMessage;
                this.lblmsg.Visible = true;
                return;
            }

            if (this.ActionContext.CurrentUser.Account.ValidationServiceEnabled)
                {
                if (reqExpenseItem.Paid && claim.paid == false)
                {
                    reqExpenseItem.OriginalExpenseId = reqExpenseItem.expenseid;
                    reqExpenseItem.expenseid = expenseItems.DuplicateExpense(reqExpenseItem);
                }
            }

            // If the limit with or without a receipt is exceeded then the following message will appear
            var tempEmployee = this.ActionContext.Employees;
            Employee approverDetail = tempEmployee.GetEmployeeById(this.ActionContext.CurrentUser.EmployeeID);

            if (approverDetail.EmployeeID == claim.employeeid && this.MustHaveBankAccountCheck(this.ActionContext.CurrentUser.MustHaveBankAccount, reqExpenseItem.bankAccountId))
            {
                return;
            }

            var subcat = subcats.GetSubcatBasic(reqExpenseItem.subcatid);

            if (subcat.StartDate != null && subcat.StartDate.Value > reqExpenseItem.date)
            {
                this.lblmsg.Text =
                    $"{subcat.Subcat} cannot be added before the permitted start date of {subcat.StartDate.Value.ToString("dd/MM/yyyy")}";
                this.lblmsg.Visible = true;
                return;
            }

            if (subcat.EndDate != null && subcat.EndDate.Value < reqExpenseItem.date)
            {
                this.lblmsg.Text =
                    $"{subcat.Subcat} cannot be added after the permitted end date of {subcat.EndDate.Value.ToString("dd/MM/yyyy")}";
                this.lblmsg.Visible = true;
                return;
            }

            //Currently, if the claim is at the approval stage, DofC validation should not be performed on edits.
            bool claimAtApproval = claim.status != ClaimStatus.None && claim.status != ClaimStatus.ItemReturnedAwaitingEmployee;

            if (!claimAtApproval && subcat.Mileageapp && subcat.EnabledDoc)
            {
                var employeeCars = this.ActionContext.EmployeeCars;
                var activeCars = employeeCars.GetActiveCars(reqProperties.UseDateOfExpenseForDutyOfCareChecks ? reqExpenseItem.date : DateTime.Now, false);
                if (reqProperties.UseDateOfExpenseForDutyOfCareChecks && activeCars.Any(car => car.carid == reqExpenseItem.carid))
                {
                        activeCars = activeCars.Where(car => car.carid == reqExpenseItem.carid).ToList();
                }

                var dutyOfCareDocument = new DutyOfCareDocuments();
                var class1BusinessResults = new List<ListItem>();
                var documentExpiryResults = dutyOfCareDocument.PassesDutyOfCare(this.ActionContext.CurrentUser.AccountID, activeCars, this.ActionContext.CurrentUser.EmployeeID, reqProperties.UseDateOfExpenseForDutyOfCareChecks ? reqExpenseItem.date : DateTime.Now, this.ActionContext.CurrentUser.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect)).Keys.First();
                if (documentExpiryResults.Count == 0 && subcat.RequireClass1BusinessInsurance)
                {
                    class1BusinessResults = dutyOfCareDocument.Class1BusinessInformation(this.ActionContext.CurrentUser.AccountID, activeCars, reqProperties.UseDateOfExpenseForDutyOfCareChecks ? reqExpenseItem.date : DateTime.Now);
                }
                 
                if (documentExpiryResults.Count > 0 || class1BusinessResults.Count > 0)
                {
                    this.lblmsg.Text = "You are unable to save this expense as there are invalid documents on the selected date.";
                    this.lblmsg.Visible = true;
                    return;
                }
            }

            reqExpenseItem.total = Math.Round(reqExpenseItem.total, 2, MidpointRounding.AwayFromZero);

            if (strLimit.Length > 0)
            {
                lblmsg.Text = strLimit;
                lblmsg.Visible = true;
                return;
            }

            foreach (cExpenseItem splitItem in reqExpenseItem.splititems)
            {
                if (strLimit.Length > 0)
                {
                    lblmsg.Text = strLimit;
                    lblmsg.Visible = true;
                    return;
                }
            }


            returncode = expenseItems.updateItem(reqExpenseItem, (int)ViewState["employeeid"], (int)ViewState["claimid"], false, claim);

            if (returncode == -1)
            {
                lblmsg.Text = "The VAT amount cannot be greater than the total.";
                lblmsg.Visible = true;
            }
            if (returncode == -3)
            {
                lblmsg.Text = "The mileage item cannot be updated as the date of the expense item is not between the range of the car start date and the car end date.";
                lblmsg.Visible = true;
                return;
            }
            if (returncode == -4)
            {
                lblmsg.Text = "The mileage item cannot be added as your recommended mileage has been exceeded.";
                lblmsg.Visible = true;
                return;
            }
            if (returncode == -6)
            {
                if (!claim.submitted)
                {
                    FlaggedItemsManager flaggedItems =
                        new FlaggedItemsManager(true, false, false, true, "ClaimViewer");
                    
                    flaggedItems.Add(new ExpenseItemFlagSummary(0, reqExpenseItem.flags));

                    ScriptManager.RegisterStartupScript(upnlSpecific, GetType(), "flagScript", "$(document).ready(function() {\nSEL.Claims.ClaimViewer.configureFlagModal(); ShowFlagModal(" + flaggedItems.Serialize() + ");});", true);
                    return;
                }
            }
            if (returncode == -7)
            {
                lblmsg.Text = "An error has occurred saving your expense item, please try again.";
                lblmsg.Visible = true;
                return;
            }
        }
        else
        {
            int? mobileid = null;
            int? mobileJourneyId = null;

            if (ViewState["mobileID"] != null && (int)ViewState["mobileID"] > 0)
            {
                mobileid = (int)ViewState["mobileID"];
            }

            if (ViewState["mobileJourneyID"] != null && (int)ViewState["mobileJourneyID"] > 0)
            {
                mobileJourneyId = (int)ViewState["mobileJourneyID"];
            }

            //check item for blocked flags
            FlagManagement flagManagement = this.ActionContext.FlagManagement;

            FlaggedItemsManager blockedFlags = new FlaggedItemsManager(true, false, false, true, "ClaimViewer");
            
            foreach (cExpenseItem item in items.Values)
            {

                if (item.total > 0 || item.miles > 0)
                {

                    if (this.MustHaveBankAccountCheck(this.ActionContext.CurrentUser.MustHaveBankAccount, item.bankAccountId))
                    {
                        return;
                    }
                    var subcat = subcats.GetSubcatBasic(item.subcatid);

                    if (subcat.StartDate != null && subcat.StartDate.Value > item.date)
                    {
                        this.lblmsg.Text =
                        $"{subcat.Subcat} cannot be added before the permitted start date of {subcat.StartDate.Value.ToString("dd/MM/yyyy")}";
                        this.lblmsg.Visible = true;
                        return;
                    }

                    if (subcat.EndDate != null && subcat.EndDate.Value < item.date)
                    {
                        this.lblmsg.Text =
                            $"{subcat.Subcat} cannot be added after the permitted end date of {subcat.EndDate.Value.ToString("dd/MM/yyyy")}";
                        this.lblmsg.Visible = true;
                        return;
                    }

                    var roleMessage = this.ValidateEmployeeItemRoles(item, subcats, employeeItemRoles);
                    if (!string.IsNullOrEmpty(roleMessage))
                    {
                        this.lblmsg.Text = roleMessage;
                        this.lblmsg.Visible = true;
                        return;
                    }
                }
            }

            bool containsInvalidSubcat = false;
            foreach (cExpenseItem item in items.Values)
            {
                item.total = Math.Round(item.total, 2, MidpointRounding.AwayFromZero);

                item.mobileID = mobileid;
                var mobileDevices = this.ActionContext.MobileDevices;
                if (mobileid.HasValue)
                {
                    ExpenseItem mItem = mobileDevices.getMobileItemByID(mobileid.Value);

                    item.addedAsMobileExpense = true;
                    if (mItem.MobileDeviceTypeId.HasValue)
                    {
                        item.addedByMobileDeviceTypeId = mItem.MobileDeviceTypeId.Value;
                    }
                }
                if (mobileJourneyId.HasValue)
                {
                    item.addedAsMobileExpense = true;
                }

                if (strLimit.Length > 0)
                {
                    lblmsg.Text = strLimit;
                    lblmsg.Visible = true;
                    return;
                }

                foreach (cExpenseItem splitItem in item.splititems)
                {
                    if (strLimit.Length > 0)
                    {
                        lblmsg.Text = strLimit;
                        lblmsg.Visible = true;
                        return;
                    }
                }

                returncode = expenseItems.addItem(item, reqEmp, claim);
                switch (returncode)
                {
                    case -1:
                        lblmsg.Text =
                            "The item cannot be added to this claim as credit card items from the same statement have already been reconciled on other claims.";
                        lblmsg.Visible = true;
                        return;
                    case -2:
                        lblmsg.Text =
                            "The item cannot be added to this claim as purchase card items from the same statement have already been reconciled on other claims.";
                        lblmsg.Visible = true;
                        return;
                    case -3:
                        lblmsg.Text =
                            "The mileage item cannot be added as the date of the expense item is not between the range of the car start date and the car end date.";
                        lblmsg.Visible = true;
                        return;
                    case -4:
                        lblmsg.Text = "The mileage item cannot be added as your recommended mileage has been exceeded.";
                        lblmsg.Visible = true;
                        return;
                    case -5:
                        lblmsg.Text =
                            "The mileage item cannot be added as you have some addresses that could not be matched automatically and require correcting.";
                        lblmsg.Visible = true;
                        return;
                    case -6:
                        if (!claim.submitted)
                        {
                            var flaggedItems = new FlaggedItemsManager(true, false, false, true, "ClaimViewer");
                            flaggedItems.Add(new ExpenseItemFlagSummary(0, item.flags));
                            ScriptManager.RegisterStartupScript(upnlSpecific, GetType(), "flagScript",
                                "$(document).ready(function() {\nSEL.Claims.ClaimViewer.configureFlagModal(); ShowFlagModal(" +
                                flaggedItems.Serialize() + ");});", true);
                            return;
                        }

                        return;
                    case -7:
                        lblmsg.Text =
                            "An error has occurred saving your expense item, please try again.";
                        lblmsg.Visible = true;
                        return;

                    default:
                        if (mobileid != null)
                        {
                            mobileDevices.DeleteMobileItemByID(mobileid);
                        }
                        else if (mobileJourneyId > 0 && returncode > 0)
                        {
                            mobileDevices.DeleteMobileJourney(mobileJourneyId);
                        }

                        break;
                }

                expenseItems.ReturnToValidationStageIfAny(item, this.ActionContext.CurrentUser, claims, claim,
                    subcats.GetSubcatById(item.subcatid));
            }

            if (containsInvalidSubcat)
            {
                foreach (cExpenseItem item in items.Values)
                {
                    claims.deleteExpense(claim, item, false, this.ActionContext.CurrentUser);
                }

                return;
            }
            //check flags that need adding first and may be blocked
            blockedFlags = new FlaggedItemsManager(true, false, false, true, "ClaimViewer");
            foreach (cExpenseItem item in items.Values)
            {
                if (item.flags != null && flagManagement.ContainsBlockedItem(item.flags))
                {
                    blockedFlags.Add(new ExpenseItemFlagSummary(items.IndexOfValue(item) + 1 / -1, item.flags));
                }
            }
            
            if (blockedFlags.Count > 0)
            {
                foreach (cExpenseItem item in items.Values)
                {
                    claims.deleteExpense(claim, item, false, this.ActionContext.CurrentUser);
                }

                ScriptManager.RegisterStartupScript(upnlSpecific, GetType(), "flagScript", "$(document).ready(function() {\nSEL.Claims.ClaimViewer.configureFlagModal(); ShowFlagModal(" + blockedFlags.Serialize() + ");});", true);
                return;
            }

            //flag any items
            foreach (cExpenseItem item in items.Values)
            {
                if (item.splititems.Count > 0)
                {
                    foreach (cExpenseItem splitItem in item.splititems)
                    {
                        if (splitItem.flags != null)
                        {
                            flagManagement.FlagItem(splitItem.expenseid, splitItem.flags);
                        }
                    }
                }

                if (item.flags != null)
                {
                    flagManagement.FlagItem(item.expenseid, item.flags);
                }
            }
        }

        
        FlagManagement flagMan = this.ActionContext.FlagManagement;
        List<int> flaggedExpenses = new List<int>();
        foreach (cExpenseItem item in items.Values)
        {
            if ((int)item.expenseid != 0)
            {

                foreach (FlagSummary flaggedItem in item.flags)
                {
                    Flag flag = flagMan.GetBy((int)flaggedItem.FlagID);
                    if (flag.DisplayFlagImmediately)
                    {
                        if (!flaggedExpenses.Contains(item.expenseid))
                {
                            flaggedExpenses.Add(item.expenseid);
                            break;
                        }
                    }
                }
                }
            }

        string expenseids = string.Empty;
        foreach (int i in flaggedExpenses)
        {
            expenseids += "expenseid=" + i + "&";
        }
        
        int returnto = (int)ViewState["returnto"];

        switch (returnto)
        {
            case 2:

                Response.Redirect("expenses/claimViewer.aspx?returned=1&claimid=" + claimid, true);
                break;
            case 3:

                Response.Redirect("deletereason.aspx?action=2&claimid=" + claimid + "&expenseid=" + ViewState["expenseid"], true);
                break;
            default:
                string statementid = "";
                if (ViewState["statementid"] != null)
                {
                    statementid = "&statementid=" + ViewState["statementid"];
                }

                if (action == Action.Edit && claim.ClaimStage == ClaimStage.Previous && reqProperties.EditPreviousClaims)
                {
                    Response.Redirect(
                        "deletereason.aspx?action=2&claimid=" +
                        claimid + "&expenseid=" + ViewState["expenseid"] +
                        ((expenseids.Trim().Length > 0) ? "&" + expenseids.Replace("expenseid", "flag") : string.Empty) +
                        "&editPrevClaim=1&claimOwner=" + (int)ViewState["employeeid"],
                        true);
                }

                Response.Redirect("expenses/claimViewer.aspx?" + expenseids + "claimid=" + claimid + statementid, true);
                break;
        }
    }

    /// <summary>
    /// Validates employee item roles for the given date
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <param name="subcats">
    /// The subcats.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    private string ValidateEmployeeItemRoles(cExpenseItem item, cSubcats subcats, EmployeeItemRoles employeeItemRoles)
    {
        var itemRoles = new cItemRoles(this.ActionContext.CurrentUser.AccountID);
        FlaggedItem result = null;
        var foundValidItemRole = false;
        var subcat = subcats.GetSubcatBasic(item.subcatid);
        var message = $"{subcat.Subcat} cannot be added ";
        var messageParts = new List<string>();
        foreach (EmployeeItemRole itemRole in employeeItemRoles.ItemRoles)
        {
            var role = itemRoles.itemRoles[itemRole.ItemRoleId];
            if (role.items.ContainsKey(item.subcatid))
            {
                if ((itemRole.StartDate.HasValue && itemRole.StartDate.Value > item.date) ||
                    (itemRole.EndDate.HasValue && itemRole.EndDate.Value < item.date))
                {
                    if (itemRole.StartDate.HasValue && itemRole.EndDate.HasValue)
                    {
                        messageParts.Add(
                            $"outside the permitted date range of {itemRole.StartDate.Value.ToString("dd/MM/yyyy")} and {itemRole.EndDate.Value.ToString("dd/MM/yyyy")}");
                    }
                    if (!itemRole.StartDate.HasValue && itemRole.EndDate.HasValue)
                    {
                        messageParts.Add(
                            $"after the permitted end date of {itemRole.EndDate.Value.ToString("dd/MM/yyyy")}");
                    }

                    if (itemRole.StartDate.HasValue && !itemRole.EndDate.HasValue)
                    {
                        messageParts.Add($"before the permitted start date of {itemRole.StartDate.Value.ToString("dd/MM/yyyy")}");
                    }
                }
                else
                {
                    foundValidItemRole = true;
                }
            }
        }
        if (messageParts.Count > 1)
        {
            message =
                $"{message}{string.Join(",", messageParts.Take(messageParts.Count - 1))} and {messageParts.LastOrDefault()}"; ;
        }
        else
        {
            message = message + string.Join(",", messageParts);
        }
        
        return foundValidItemRole ? string.Empty : message;
    }

    /// <summary>
    /// Check if the user has set a bank account where they are required to
    /// </summary>
    /// <param name="mustHaveBankAccount">If the current user is required to have a bank account</param>
    /// <param name="bankAccountId">The Id of the bank account asigned to the expense</param>
    /// <returns>Whether or not they need to set a bankaccount</returns>
    private bool MustHaveBankAccountCheck(bool mustHaveBankAccount, int? bankAccountId)
    {
        if (Log.IsDebugEnabled)
        {
            Log.Debug($"mustHaveBankAccount is set to {mustHaveBankAccount} and the bankAccountId is {bankAccountId}");
        }

        if (mustHaveBankAccount && (bankAccountId == null || bankAccountId == 0))
        {
            lblmsg.Text = "The item cannot be added as you do not have a bank account selected.";
            lblmsg.Visible = true;
            return true;
        }

        return false;
    }

    public SortedList<int, cExpenseItem> getItems(bool expenseItemPaid = false)
    {
        cEmployees clsemployees = this.ActionContext.Employees;
        Employee reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);
        SortedList<int, cExpenseItem> expitems = new SortedList<int, cExpenseItem>();
        cExpenseItem item = null;
        List<int> items = new List<int>();
        cMisc clsmisc = this.ActionContext.Misc;
        cGlobalProperties clsproperties = clsmisc.GetGlobalProperties((int)ViewState["accountid"]);
        DateTime date;
        DropDownList ddlst;

        int claimid;
        int companyid = 0;
        int reasonid = 0;
        int countryid = 0;
        int currencyid = 0;
        int transactionid = 0;
        string otherdetails = "";
        double exchangerate = 0;
        int workAddressId = 0;

        ItemType itemtype = (ItemType)ViewState["itemtype"];

        if (ViewState["transactionid"] != null)
        {
            transactionid = (int)ViewState["transactionid"];
        }

        #region general details
        int basecurrency = reqemp.PrimaryCurrency != 0 ? reqemp.PrimaryCurrency : clsproperties.basecurrency;
        int homecountry = reqemp.PrimaryCountry != 0 ? reqemp.PrimaryCountry : clsproperties.homecountry;
        TextBox txtbox = (TextBox)pnlgeneral.FindControl("txtdate");
        DateTime.TryParseExact(txtbox.Text, new string[] { "dd/MM/yyyy", "MM/dd/yyyy" }, null, DateTimeStyles.None, out date);
        Session["date"] = date;
        if (clsproperties.singleclaim)
        {
            claimid = (int)ViewState["claimid"];
        }
        else
        {
            ddlst = (DropDownList)pnlgeneral.FindControl("cmbclaims");
            if (ddlst != null)
            {
                claimid = int.Parse(ddlst.SelectedItem.Value);
            }
            else
            {
                claimid = (int)ViewState["claimid"];
            }
        }

        // todo: pick up general details organisation
        // todo: pick up non-general details organisation
        cFieldToDisplay company = clsmisc.GetGeneralFieldByCode("organisation");
        if (!company.individual && ((itemtype == ItemType.Cash && company.display) || (itemtype == ItemType.CreditCard && company.displaycc) || (itemtype == ItemType.PurchaseCard && company.displaypc)))
        {
            int companyIdentifier;

            txtbox = (TextBox)pnlgeneral.FindControl("txtOrganisation_ID");
            string companyIdentifierText = txtbox.Text;

            if (int.TryParse(companyIdentifierText, out companyIdentifier))
            {
                if (companyIdentifier > 0)
                {
                    companyid = companyIdentifier;
                    Session["companyid"] = companyIdentifier;
                }
            }
        }
        cJourneyStep step = null;
        Address fromcompany = null;
        Address tocompany = null;
        cFieldToDisplay from = clsmisc.GetGeneralFieldByCode("from");
        cFieldToDisplay to = clsmisc.GetGeneralFieldByCode("to");
        if (!from.individual && ((itemtype == ItemType.Cash && from.display) || (itemtype == ItemType.CreditCard && from.displaycc) || (itemtype == ItemType.PurchaseCard && from.displaypc)))
        {
            var txtfromid = (HiddenField)pnlgeneral.FindControl("txtfromid");
            int fromAddressId;

            if (int.TryParse(txtfromid.Value, out fromAddressId))
            {
                fromcompany = Address.Get((int)ViewState["accountid"], fromAddressId);
                Session["fromid"] = fromAddressId; // todo see what else uses Session "fromid"
            }

        }
        if (!to.individual && ((itemtype == ItemType.Cash && to.display) || (itemtype == ItemType.CreditCard && to.displaycc) || (itemtype == ItemType.PurchaseCard && to.displaypc)))
        {

            var txttoid = (HiddenField)pnlgeneral.FindControl("txttoid");
            int toAddressId;

            if (int.TryParse(txttoid.Value, out toAddressId))
            {
                tocompany = Address.Get((int)ViewState["accountid"], toAddressId);
                Session["toid"] = toAddressId; // todo see what else uses Session "toid"
            }
        }

        if (fromcompany != null || tocompany != null)
        {
            step = new cJourneyStep(0, fromcompany, tocompany, 0, 0, 0, 1, 0, false);
        }
        cFieldToDisplay reason = clsmisc.GetGeneralFieldByCode("reason");
        cFieldToDisplay country = clsmisc.GetGeneralFieldByCode("country");
        if (!reason.individual && ((itemtype == ItemType.Cash && reason.display) || (itemtype == ItemType.CreditCard && reason.displaycc) || (itemtype == ItemType.PurchaseCard && reason.displaypc)))
        {
            ddlst = (DropDownList)pnlgeneral.FindControl("cmbreason");
            if (ddlst != null)
            {
                if (ddlst.SelectedValue != "0")
                {
                    reasonid = int.Parse(ddlst.SelectedValue);
                    Session["reasonid"] = reasonid;
                }
            }
        }
        if ((itemtype == ItemType.Cash && country.display) || (itemtype == ItemType.CreditCard && country.displaycc) || (itemtype == ItemType.PurchaseCard && country.displaypc))
        {
            ddlst = (DropDownList)pnlgeneral.FindControl("cmbcountry");
            if (ddlst.SelectedValue != "0")
            {
                countryid = int.Parse(ddlst.SelectedValue);
                Session["countryid"] = countryid;
            }
        }
        else
        {
            countryid = homecountry;
        }

        cFieldToDisplay currency = clsmisc.GetGeneralFieldByCode("currency");
        cCardStatements statements = this.ActionContext.CardStatements;
        cCardTransaction transaction = statements.getTransactionById(transactionid);

        if ((itemtype == ItemType.Cash && currency.display) || (itemtype == ItemType.CreditCard && currency.displaycc) || (itemtype == ItemType.PurchaseCard && currency.displaypc))
        {
            ddlst = (DropDownList)pnlgeneral.FindControl("cmbcurrency");
            if (ddlst.SelectedItem != null)
            {
                currencyid = int.Parse(ddlst.SelectedValue);
                Session["currencyid"] = currencyid;
                if (currencyid != basecurrency)
                {
                    if (clsproperties.exchangereadonly && itemtype == ItemType.Cash)
                    {
                        object[] arrcur = getExchangeRate((int)ViewState["accountid"], reqemp.EmployeeID, currencyid, date);
                        double.TryParse(arrcur[1].ToString(), out exchangerate);
                        Session["exchangerate"] = exchangerate;
                    }
                    else
                    {
                        txtbox = (TextBox)pnlgeneral.FindControl("txtexchangerate");                        
                        if (transaction == null)
                        {
                            double result;
                            if (txtbox.Text.Length > 0 && double.TryParse(txtbox.Text, out result))
                            {
                                exchangerate = result;                                    
                            }
                            else
                            {
                                object[] arrcur = getExchangeRate((int)ViewState["accountid"], reqemp.EmployeeID, currencyid, date);
                                if (arrcur != null)
                                {
                                    double.TryParse(arrcur[1].ToString(), out result);
                                    exchangerate = (double?) Session["exchangerate"] ?? result;
                                }
                                else
                                {
                                    exchangerate = (double) Session["exchangerate"];
                                }                                                                        
                            }                                                                            
                        }
                        else
                        {
                            exchangerate = (double)Math.Round(transaction.originalamount / transaction.transactionamount, 10, MidpointRounding.AwayFromZero);
                        }

                        Session["exchangerate"] = exchangerate;
                        
                    }
                }
            }
        }
        else
        {
            currencyid = basecurrency;
        }
        cFieldToDisplay fldotherdetails = clsmisc.GetGeneralFieldByCode("otherdetails");

        if (!fldotherdetails.individual && ((itemtype == ItemType.Cash && fldotherdetails.display) || (itemtype == ItemType.CreditCard && fldotherdetails.displaycc) || (itemtype == ItemType.PurchaseCard && fldotherdetails.displaypc)))
        {
            txtbox = (TextBox)pnlgeneral.FindControl("txtotherdetails");
            otherdetails = txtbox.Text;
            Session["otherdetails"] = otherdetails;
        }

        cUserdefinedFields clsuserdefined = this.ActionContext.UserDefinedFields;
        Table tbludf = (Table)pnlgeneral.FindControl("tblGeneral");
        cTable udftbl = this.ActionContext.Tables.GetTableByID(new Guid("65394331-792e-40b8-af8b-643505550783"));
        SortedList<int, object> lstgenudf = clsuserdefined.getItemsFromPage(ref tbludf, udftbl, false, "");
        Session["genudfs"] = lstgenudf;
        #endregion

        List<cDepCostItem> breakdown = getCostcodeBreakdown(tblcostcodes);
        if ((Spend_Management.Action)ViewState["action"] == Spend_Management.Action.Add)
        {
            if ((int)ViewState["transactionid"] > 0)
            {
                cCardStatement statement = statements.getStatementById(transaction.statementid);
                if (statement.Corporatecard.allocateditem != null)
                {
                    items.Add((int)statement.Corporatecard.allocateditem);
                }
                else
                {
                    ddlst = (DropDownList)pnlspecific.FindControl("cmbsubcats");
                    if (ddlst.SelectedItem != null)
                    {
                        if (ddlst.SelectedValue != "" && ddlst.SelectedValue != "0")
                        {
                            items.Add(int.Parse(ddlst.SelectedValue));
                        }
                    }
                }
            }
            else if (ViewState["mobileID"] != null && (int)ViewState["mobileID"] > 0)
            {
                cMobileDevices clsmobile = this.ActionContext.MobileDevices;
                ExpenseItem mobileitem = clsmobile.getMobileItemByID((int)ViewState["mobileID"]);
                if (mobileitem != null)
                {
                    items.Add(mobileitem.SubcatID);
                }
            }
            else if (ViewState["mobileJourneyID"] != null && (int)ViewState["mobileJourneyID"] > 0)
            {
                ddlst = (DropDownList)pnlspecific.FindControl("cmbsubcats");

                if (ddlst.SelectedValue != "" && ddlst.SelectedValue != "0")
                {
                    items.Add(int.Parse(ddlst.SelectedValue));
                }
            }
            else
            {
                items = reqemp.GetSubCategories().SubCategories;
                if (items.Count == 0 || itemtype != ItemType.Cash) // must be dropdown lists so get from there
                {
                    items.Clear();
                    ddlst = (DropDownList)pnlspecific.FindControl("cmbsubcats");

                    if (ddlst.SelectedValue != "" && ddlst.SelectedValue != "0")
                    {
                        items.Add(int.Parse(ddlst.SelectedValue));
                    }
                }
            }
        }
        else
        {
            if (ViewState["expenseitem"] != null)
            {
                item = (cExpenseItem)ViewState["expenseitem"];
                if (item.transactionid > 0)
                {
                    transactionid = item.transactionid;
                }
                items.Add(item.subcatid);
            }
        }

        #region specific details
        sExpenseItemDetails itemdetails;
        cSubcats clssubcats = this.ActionContext.SubCategories;
        cSubcat subcat;
        Panel subpnl;
        cExpenseItem newitem;
        cExpenseItem splititem;
        int.TryParse(this.hdnWorkAddressId.Value, out workAddressId);

        for (int i = 0; i < items.Count; i++)
        {

            subpnl = (Panel)pnlspecific.FindControl("pnl" + i);
            itemdetails = getSubcatDetails(items[i], i.ToString(), subpnl, reqemp);

            int bankAccountId = 0;
            ddlst = (DropDownList)pnlspecific.FindControl("cmbbankaccount" + i);

            if (ddlst != null && ddlst.SelectedValue != "")
            {
                bankAccountId = int.Parse(ddlst.SelectedValue);
            }
            else
            {
                var bankAccounts = this.ActionContext.BankAccounts;
                List<ListItem> lstItems = bankAccounts.CreateDropDown(reqemp.EmployeeID);


                // In case of single bank account, it will be automatically assigned to expense item
                if (lstItems.Count == 2)
                {
                    bankAccountId = int.Parse(lstItems[1].Value);
                }

                // This will check if item is opened for editing via approvar and then assign the existing bank account id
                else if (item != null && item.expenseid > 0)
                {
                    bankAccountId = item.bankAccountId.Value;
                }
            }

            if (step != null)
            {
                bool heavyBulkyEquipment = false;
                if (subpnl != null)
                {
                    var hbe = subpnl.FindControl("chkHeavyBulky" + i) as CheckBox;
                    if (hbe != null)
                    {
                        heavyBulkyEquipment = hbe.Checked;
                    }
                }

                step = new cJourneyStep(itemdetails.expenseid, fromcompany, tocompany, itemdetails.miles, 0, (byte)itemdetails.nopassengers, 1, itemdetails.miles, heavyBulkyEquipment);
                itemdetails.journeysteps = new SortedList<int, cJourneyStep>();
                itemdetails.journeysteps.Add(1, step);
            }

            if (itemdetails.companyid > 0)
            {
                companyid = itemdetails.companyid;
            }

            if (itemdetails.otherdetails != null)
            {
                otherdetails = itemdetails.otherdetails;
            }

            if (itemdetails.reasonid != 0)
            {
                reasonid = itemdetails.reasonid;
            }

            if (itemdetails.userdefined != null)
            {
                foreach (KeyValuePair<int, object> u in lstgenudf)
                {
                    itemdetails.userdefined.Add(u.Key, u.Value);
                }
            }
            itemtype = itemdetails.itItemtype;

            var validationProgress = ExpenseValidationProgress.ValidationServiceDisabled;
            var receiptAttached = itemdetails.receiptattached;

            if (item != null)
            {
                validationProgress = item.ValidationProgress;
                receiptAttached = item.receiptattached;
            }


            newitem = new cExpenseItem(itemdetails.expenseid, itemtype, itemdetails.bmiles, itemdetails.pmiles,
                otherdetails, itemdetails.receipt, 0, itemdetails.vat, itemdetails.total, itemdetails.subcatid, date,
                itemdetails.staff, itemdetails.others, companyid, false, itemdetails.home, "", claimid,
                itemdetails.plitres, itemdetails.blitres, currencyid, itemdetails.attendees, itemdetails.tip, countryid,
                0, 0, exchangerate, false, reasonid, itemdetails.normalreceipt, itemdetails.allowancestartdate,
                itemdetails.allowanceenddate, itemdetails.carid, itemdetails.allowancededuct, itemdetails.allowanceid,
                itemdetails.nonights, itemdetails.quantity, itemdetails.directors, itemdetails.amountpayable,
                itemdetails.hotelid, true, itemdetails.norooms, itemdetails.vatnumber, itemdetails.personalguests,
                itemdetails.remoteworkers, itemdetails.accountcode, basecurrency, 0, 0, 0, itemdetails.floatid, false,
                receiptAttached, itemdetails.transactionid, new DateTime(1900, 01, 01), 0,
                new DateTime(1900, 01, 01), 0, itemdetails.mileageid, itemdetails.unit, itemdetails.assignmentid,
                HomeToLocationType.None, validationProgress: validationProgress, bankAccountId: bankAccountId, workAddressId: workAddressId, paid: expenseItemPaid);

            if (item != null)
            {
                newitem.returned = item.returned;
                newitem.corrected = item.corrected;
                newitem.itemCheckerId = item.itemCheckerId;
                newitem.refnum = item.refnum;
                newitem.ValidationCount = item.ValidationCount;
            }

            newitem.userdefined = itemdetails.userdefined;
            newitem.transactionid = transactionid;
            newitem.costcodebreakdown = breakdown;
            newitem.journeysteps = itemdetails.journeysteps;
            expitems.Add(itemdetails.subcatid, newitem);
            subcat = clssubcats.GetSubcatById(itemdetails.subcatid);

            if (subcat != null)
            {
                newitem.homeToOfficeDeductionMethod = subcat.HomeToLocationType;
                if (subcat.subcatsplit.Count > 0)
                {
                    foreach (int subitem in subcat.subcatsplit)
                    {
                        subpnl = (Panel)pnlspecific.FindControl("pnl" + i + subitem);
                        itemdetails = getSubcatDetails(subitem, i.ToString() + subitem.ToString(), subpnl, reqemp);

                        if (itemdetails.total > 0 || itemdetails.miles > 0)
                        {
                            bankAccountId = 0;
                            ddlst = (DropDownList)pnlspecific.FindControl("cmbbankaccount" + i + subitem);

                            if (ddlst != null && ddlst.SelectedValue != "")
                            {
                                bankAccountId = int.Parse(ddlst.SelectedValue);
                            }
                            else
                            {
                                var bankAccounts = this.ActionContext.BankAccounts;
                                List<ListItem> lstItems = bankAccounts.CreateDropDown(reqemp.EmployeeID);


                                // In case of single bank account, it will be automatically assigned to expense item
                                if (lstItems.Count == 2)
                                {
                                    bankAccountId = int.Parse(lstItems[1].Value);
                                }

                                // This will check if item is opened for editing via approvar and then assign the existing bank account id
                                else if (item != null && item.expenseid > 0)
                                {
                                    bankAccountId = item.bankAccountId.Value;
                                }
                            }

                            splititem = new cExpenseItem(itemdetails.expenseid, itemtype, itemdetails.bmiles,
                                itemdetails.pmiles, otherdetails, newitem.receipt, 0, itemdetails.vat, itemdetails.total,
                                itemdetails.subcatid, date, itemdetails.staff, itemdetails.others, companyid, false,
                                itemdetails.home, "", claimid, itemdetails.plitres, itemdetails.blitres, currencyid,
                                itemdetails.attendees, itemdetails.tip, countryid, 0, 0, exchangerate, false, reasonid,
                                newitem.normalreceipt, itemdetails.allowancestartdate, itemdetails.allowanceenddate,
                                itemdetails.carid, itemdetails.allowancededuct, itemdetails.allowanceid,
                                itemdetails.nonights, itemdetails.quantity, itemdetails.directors,
                                itemdetails.amountpayable, itemdetails.hotelid, false, itemdetails.norooms,
                                itemdetails.vatnumber, itemdetails.personalguests, itemdetails.remoteworkers,
                                itemdetails.accountcode, basecurrency, 0, 0, 0, newitem.floatid, newitem.corrected,
                                newitem.receiptattached, newitem.transactionid, new DateTime(1900, 01, 01), 0,
                                new DateTime(1900, 01, 01), 0, itemdetails.mileageid, itemdetails.unit,
                                itemdetails.assignmentid, subcat.HomeToLocationType,
                                validationProgress: validationProgress, bankAccountId: bankAccountId, workAddressId: workAddressId);

                            if (item != null)
                            {
                                splititem.returned = item.returned;
                                splititem.corrected = item.corrected;
                                splititem.itemCheckerId = item.itemCheckerId;
                            }

                            splititem.userdefined = itemdetails.userdefined;
                            splititem.transactionid = transactionid;
                            splititem.setPrimaryItem(newitem);
                            splititem.costcodebreakdown = breakdown;                                

                            newitem.addSplitItem(splititem);
                            newitem.total -= splititem.total;
                        }
                    }
                }
            }
        }
        #endregion

        ViewState["items"] = expitems;
        return expitems;
    }


    public sExpenseItemDetails getSubcatDetails(int subcatid, string id, Panel pnl, Employee reqemp)
    {
        ItemType itemtype = (ItemType)ViewState["itemtype"];
        CheckBox chkbox;
        var accountIdentifier = (int)ViewState["accountid"];
        cMisc clsmisc = this.ActionContext.Misc;
        cAccountProperties reqProperties = this.ActionContext.Properties;
        sExpenseItemDetails details = new sExpenseItemDetails();
        cSubcats clssubcats = this.ActionContext.SubCategories;
        HiddenField hdn;
        if (subcatid == 0 || pnl == null)
        {
            return details;
        }
        cSubcat subcat = clssubcats.GetSubcatById(subcatid);
        details.subcatid = subcatid;
        cUserdefinedFields clsuserdefined = this.ActionContext.UserDefinedFields;
        TextBox txtbox = (TextBox)pnl.FindControl("txtexpenseid" + id);
        if (txtbox != null)
        {
            if (txtbox.Text != "")
            {
                details.expenseid = int.Parse(txtbox.Text);
            }
        }

        if (Session["date"] != null)
        {
            details.date = (DateTime)Session["date"];
        }
        cFieldToDisplay company = clsmisc.GetGeneralFieldByCode("organisation");

        if (company.individual && subcat.companyapp)
        {
            int companyIdentifier;

            txtbox = (TextBox)pnlgeneral.FindControl(string.Format("{0}_txtOrganisation_ID", id));
            string companyIdentifierText = txtbox != null ? txtbox.Text : string.Empty;

            if (int.TryParse(companyIdentifierText, out companyIdentifier))
            {
                details.companyid = companyIdentifier;
            }
        }

        cFieldToDisplay from = clsmisc.GetGeneralFieldByCode("from");
        cFieldToDisplay to = clsmisc.GetGeneralFieldByCode("to");

        #region car
        // if the car ddl was used get the car specified, otherwise get the user's default car
        cEmployeeCars clsEmployeeCars = this.ActionContext.EmployeeCars;
        bool carDdlUsed = false;
        DropDownList ddlst = (DropDownList)pnl.FindControl("cmbcars" + id);
 
        if (ddlst != null && !string.IsNullOrWhiteSpace(ddlst.SelectedValue))
        {
            carDdlUsed = true;
            int.TryParse(ddlst.SelectedValue, out details.carid);
        }

        if (details.carid == 0 || carDdlUsed == false)
        {
            details.carid = clsEmployeeCars.GetDefaultCarID(reqProperties.BlockTaxExpiry, reqProperties.BlockMOTExpiry, reqProperties.BlockInsuranceExpiry, reqProperties.BlockBreakdownCoverExpiry,  reqProperties.DisableCarOutsideOfStartEndDate, details.date);
        }

        cCar car = clsEmployeeCars.GetCarByID(details.carid);
        MileageUOM defaultuom = car == null ? MileageUOM.Mile : car.defaultuom;
        cMileagecats clsmileagecats = this.ActionContext.MileageCategories;
        #endregion car

        // add in logic to check for item specific to and from locations
        if (reqProperties.AllowMultipleDestinations && subcat.fromapp && from.individual && to.individual && subcat.toapp && subcat.mileageapp && ((itemtype == ItemType.Cash && from.display && to.display) || (itemtype == ItemType.CreditCard && from.displaycc && to.displaycc) || (itemtype == ItemType.PurchaseCard && from.displaypc && to.displaypc)))
        {
            details.journeysteps = new SortedList<int, cJourneyStep>();

            details.unit = defaultuom;

            cJourneyStep[] journeySteps = MileageGridBuilder.GetJourneyStepsFromPostback(Request, accountIdentifier, details.expenseid, subcatid);
            for (int index = 0; index < journeySteps.Length; index++)
            {
                details.journeysteps.Add(index, journeySteps[index]);
            }
        }
        else
        {
            if (carDdlUsed)
            {
                details.unit = defaultuom;
            }

            Address fromcompany = null;
            Address tocompany = null;
            details.journeysteps = new SortedList<int, cJourneyStep>();
            hdn = (HiddenField)pnl.FindControl("txtfromid" + id);
            if (hdn != null)
            {
                if (hdn.Value != "")
                {
                    fromcompany = Address.Get(accountIdentifier, Convert.ToInt32(hdn.Value));
                }
            }

            hdn = (HiddenField)pnl.FindControl("txttoid" + id);
            if (hdn != null)
            {
                if (hdn.Value != "")
                {
                    tocompany = Address.Get(accountIdentifier, Convert.ToInt32(hdn.Value));
                }
            }

            byte numpassengers = 0;
            var numPassengersTextBox = (TextBox)pnl.FindControl("txtpassengers" + id);
            if (numPassengersTextBox != null)
            {
                byte.TryParse(numPassengersTextBox.Text, out numpassengers);
            }

            var heavyBulkyCheckBox = (CheckBox)pnl.FindControl("chkHeavyBulky" + id);
            bool heavyBulkyEquipment = heavyBulkyCheckBox != null && heavyBulkyCheckBox.Checked;

            decimal nummiles = 0;
            var recMilesTextBox = (TextBox)pnl.FindControl("txtmileage" + id);
            if (recMilesTextBox != null)
            {
                decimal.TryParse(recMilesTextBox.Text, out nummiles);
            }

            if (defaultuom == MileageUOM.KM)
            {
                nummiles = ConvertKilometersToMiles.PerformConversion(nummiles);
            }

            if ((fromcompany != null && tocompany != null) || (heavyBulkyEquipment))
            {
                details.journeysteps.Add(0, new cJourneyStep(0, fromcompany, tocompany, nummiles, 0, numpassengers, 0, nummiles, heavyBulkyEquipment));
            }
        }

        if (subcat.mileageapp || subcat.calculation == CalculationType.ExcessMileage)
        {
            details.unit = defaultuom;

            if (subcat.calculation == CalculationType.ExcessMileage)
            {
                txtbox = (TextBox)pnl.FindControl("txtallowances" + id);
            }
            else
            {
                txtbox = (TextBox)pnl.FindControl("txtmileage" + id);
            }
            
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    if (subcat.IsRelocationMileage || subcat.calculation == CalculationType.ExcessMileage)
                    {
                        // Dont allow mileage to be claimed if the check box is not checked
                        chkbox = (CheckBox)pnl.FindControl("chkallowance" + id);

                        if (chkbox != null)
                        {
                            if (subcat.calculation == CalculationType.ExcessMileage)
                            {
                                details.miles = chkbox.Checked ? decimal.Parse(txtbox.Text) * (decimal)reqemp.ExcessMileage : 0;
                            }
                            else
                            {
                                details.miles = chkbox.Checked ? decimal.Parse(txtbox.Text) : 0;
                            }
                        }
                    }
                    else
                    {
                        details.miles = decimal.Parse(txtbox.Text);
                    }

                    if (defaultuom == MileageUOM.KM)
                    {
                        details.miles = ConvertKilometersToMiles.PerformConversion(details.miles);
                    }

                    if (details.journeysteps == null || (details.journeysteps != null && details.journeysteps.Count == 0))
                    {
                        details.journeysteps = new SortedList<int, cJourneyStep>();
                        details.journeysteps.Add(1, new cJourneyStep(0, null, null, details.miles, 0, 0, 1, details.miles, false));
                    }
                    else
                    {
                        details.journeysteps.Values[0].nummiles = details.miles;
                        details.journeysteps.Values[0].NumActualMiles = details.miles;
                    }
                }
            }
        }

        // this section is probably now redundant, but cannot be removed at this time as it is out of bug/testing scope
        chkbox = (CheckBox)pnl.FindControl("chkreturntostart" + id);

        if (chkbox != null)
        {
            if (chkbox.Checked)
            {
                Address startlocation = details.journeysteps.Values[0].startlocation;
                Address endlocation = details.journeysteps.Values[details.journeysteps.Values.Count - 1].endlocation;

                // Only add the step if the start and end are not the same address id
                if (endlocation != null && startlocation != null && endlocation.Identifier != startlocation.Identifier)
                {
                    details.unit = defaultuom;

                    cAccountProperties subAccountProperties = this.ActionContext.Properties;

                    // do the return calculation in case it's different from the other direction
                    decimal? retrievedMileage = AddressDistance.GetRecommendedDistance(this.ActionContext.CurrentUser, endlocation, startlocation, subAccountProperties.MileageCalcType, this.ActionContext.CurrentUser.Account.MapsEnabled);
                    decimal mileage = retrievedMileage.HasValue ? retrievedMileage.Value : 0m;

                    if (details.unit == MileageUOM.KM)
                    {
                        mileage = clsmileagecats.convertMilesToKM(mileage);
                    }

                    if (mileage >= 0)
                    {
                        details.journeysteps.Add(details.journeysteps.Count, new cJourneyStep(0, endlocation, startlocation, mileage, mileage, details.journeysteps.Values[details.journeysteps.Count - 1].numpassengers, (byte)(details.journeysteps.Count), mileage, details.journeysteps.Values[details.journeysteps.Count - 1].heavyBulkyEquipment));
                    }
                }
            }
        }

        if (subcat.calculation == CalculationType.PencePerMile || subcat.calculation == CalculationType.PencePerMileReceipt || subcat.calculation == CalculationType.FuelCardMileage || subcat.calculation == CalculationType.ExcessMileage)
        {
            txtbox = (TextBox)pnl.FindControl("txtmileagecat" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.mileageid = int.Parse(txtbox.Text);
                }
                else
                {
                    var mileageCatFormValue = Request.Form["ctl00$contentmain$cmbmileagecat" + id];
                    int mileageId;
                    if (int.TryParse(mileageCatFormValue, out mileageId))
                    //sometimes, txtmileagecat<id> won't have a value as the values in cmbmileagecat<id> 
                    //are generated after the form is first produced by a call to a web service (getMileageCategoriesByCar)
                    //using a CascadingDropDown, the client-side implementation of which doesn't call dropdown.onchange.
                    //We could arguably always use this branch and discard txtmileagecat<id>.
                    {
                        details.mileageid = mileageId;
                    }
                }
            }

            if (details.mileageid == 0)
            {
                if (car == null)
                {
                    details.mileageid = 0;
                }
                else
                {
                    if (subcat.MileageCategory != null)
                    {
                        details.mileageid = (int)subcat.MileageCategory;
                    }
                    else if (car.mileagecats.Count > 0)
                    {
                        details.mileageid = car.mileagecats[0];
                    }
                }
            }

            cMileageCat reqmileage = clsmileagecats.GetMileageCatById(details.mileageid);

            if (reqmileage != null)
            {
                details.currencyid = reqmileage.currencyid;
                details.exchangerate = getExchange(accountIdentifier, (int)ViewState["employeeid"], details.currencyid, details.date);
            }
        }

        cFieldToDisplay reason = clsmisc.GetGeneralFieldByCode("reason");

        if (reason.individual && subcat.reasonapp)
        {
            ddlst = (DropDownList)pnl.FindControl("cmbreason" + id);

            if (ddlst != null)
            {
                details.reasonid = int.Parse(ddlst.SelectedValue);
            }
        }

        cFieldToDisplay otherdetails = clsmisc.GetGeneralFieldByCode("otherdetails");

        if (otherdetails.individual && subcat.otherdetailsapp)
        {
            txtbox = (TextBox)pnl.FindControl("txtotherdetails" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.otherdetails = txtbox.Text;
                }
            }
        }

        if (subcat.hotelapp)
        {
            txtbox = (TextBox)pnl.FindControl("txthotelid" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    if (txtbox.Text != "0") //get hotelid
                    {
                        details.hotelid = int.Parse(txtbox.Text);
                    }
                    else
                    {
                        string hotelname, address1, address2, city, county, postcode, country;
                        txtbox = (TextBox)pnl.FindControl("txthotel" + id);
                        hotelname = txtbox.Text;
                        if (hotelname != "")
                        {
                            txtbox = (TextBox)pnl.FindControl("txtaddress1" + id);
                            address1 = txtbox.Text;
                            txtbox = (TextBox)pnl.FindControl("txtaddress2" + id);
                            address2 = txtbox.Text;
                            txtbox = (TextBox)pnl.FindControl("txtcity" + id);
                            city = txtbox.Text;
                            txtbox = (TextBox)pnl.FindControl("txtcounty" + id);
                            county = txtbox.Text;
                            txtbox = (TextBox)pnl.FindControl("txtpostcode" + id);
                            postcode = txtbox.Text;
                            txtbox = (TextBox)pnl.FindControl("txtcountry" + id);
                            country = txtbox.Text;
                            Hotel hotel = new Hotel();
                            details.hotelid = hotel.Add(hotelname, address1, address2, city, county, postcode, country, 0, "", "", (int)ViewState["employeeid"]);
                            if (details.hotelid == -1)
                            {
                                Hotel reqhotel = Hotel.Get(hotelname);
                                if (reqhotel == null)
                                {
                                    details.hotelid = 0;
                                }
                                else
                                {
                                    details.hotelid = reqhotel.hotelid;
                                }
                            }

                        }
                    }
                }
            }
        }

        if (subcat.nopassengersapp)
        {
            txtbox = (TextBox)pnl.FindControl("txtpassengers" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.nopassengers = int.Parse(txtbox.Text);
                    if (details.journeysteps == null)
                    {
                        details.journeysteps = new SortedList<int, cJourneyStep>();
                        details.journeysteps.Add(1, new cJourneyStep(0, null, null, 0, 0, (byte)details.nopassengers, 1, 0, false));
                    }
                    else
                    {
                        if (details.journeysteps.Count > 0)
                        {
                            details.journeysteps.Values[0].numpassengers = (byte)details.nopassengers;
                        }
                    }
                }
            }
        }

        if (subcat.bmilesapp)
        {
            txtbox = (TextBox)pnl.FindControl("txtbmiles" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.bmiles = decimal.Parse(txtbox.Text);
                }
            }
        }
        if (subcat.pmilesapp)
        {
            txtbox = (TextBox)pnl.FindControl("txtpmiles" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.pmiles = decimal.Parse(txtbox.Text);
                }
            }
        }
        if (subcat.nodirectorsapp)
        {
            txtbox = (TextBox)pnl.FindControl("txtdirectors" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.directors = byte.Parse(txtbox.Text);
                }
            }
        }
        if (subcat.staffapp)
        {
            txtbox = (TextBox)pnl.FindControl("txtstaff" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.staff = byte.Parse(txtbox.Text);
                }
            }
        }
        if (subcat.othersapp)
        {
            txtbox = (TextBox)pnl.FindControl("txtothers" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.others = byte.Parse(txtbox.Text);
                }
            }
        }
        if (subcat.nopersonalguestsapp)
        {
            txtbox = (TextBox)pnl.FindControl("txtpersonalguests" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.personalguests = byte.Parse(txtbox.Text);
                }
            }
        }
        if (subcat.noremoteworkersapp)
        {
            txtbox = (TextBox)pnl.FindControl("txtremoteworkers" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.remoteworkers = byte.Parse(txtbox.Text);
                }
            }
        }
        if (subcat.attendeesapp)
        {
            txtbox = (TextBox)pnl.FindControl("txtattendees" + id);
            if (txtbox != null)
            {
                details.attendees = txtbox.Text;
            }

        }
        if (subcat.nonightsapp)
        {
            txtbox = (TextBox)pnl.FindControl("txtnonights" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.nonights = byte.Parse(txtbox.Text);
                }
            }
        }
        if (subcat.noroomsapp)
        {
            txtbox = (TextBox)pnl.FindControl("txtnorooms" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.norooms = byte.Parse(txtbox.Text);
                }
            }
        }
        if (subcat.tipapp)
        {
            txtbox = (TextBox)pnl.FindControl("txttip" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.tip = decimal.Parse(txtbox.Text);
                }
            }
        }
        if (subcat.receiptapp)
        {
            chkbox = (CheckBox)pnl.FindControl("optnormalreceiptyes" + id);

            if (chkbox != null)
            {
                details.normalreceipt = chkbox.Checked;
            }

        }
        if (subcat.receiptapp && subcat.vatapp)
        {
            chkbox = (CheckBox)pnl.FindControl("optvatreceiptyes" + id);
            if (chkbox != null)
            {
                details.receipt = chkbox.Checked;
            }
        }
        if (subcat.eventinhomeapp)
        {
            chkbox = (CheckBox)pnl.FindControl("opteventinhomeyes" + id);
            if (chkbox != null)
            {
                details.home = chkbox.Checked;
            }
        }
        if (subcat.calculation == CalculationType.FixedAllowance || subcat.IsRelocationMileage || subcat.calculation == CalculationType.ExcessMileage)
        {
            chkbox = (CheckBox)pnl.FindControl("chkallowance" + id);
            if (chkbox != null)
            {
                if (chkbox.Checked)
                {
                    txtbox = (TextBox)pnl.FindControl("txtallowances" + id);
                    if (txtbox.Text != "")
                    {
                        details.quantity = double.Parse(txtbox.Text);
                        if (subcat.calculation != CalculationType.ExcessMileage)
                        {
                            details.total = subcat.allowanceamount * (decimal)details.quantity;
                        }
                    }
                }
            }
        }

        txtbox = (TextBox)pnl.FindControl("txtadvance" + id);
        if (txtbox != null)
        {
            if (txtbox.Text != "")
            {
                details.floatid = int.Parse(txtbox.Text);
            }
        }
        if (subcat.calculation == CalculationType.DailyAllowance)
        {
            ddlst = (DropDownList)pnl.FindControl("cmballowance" + id);
            if (ddlst != null)
            {
                DateTime tempdate = new DateTime(1900, 01, 01);
                string[] time;
                if (string.IsNullOrEmpty(ddlst.SelectedValue) == false)
                {
                    details.allowanceid = int.Parse(ddlst.SelectedValue);
                }
                else
                {
                    details.allowanceid = 0;
                }
                txtbox = (TextBox)pnl.FindControl("txtstartdate" + id);
                if (txtbox.Text != "")
                {
                    DateTime.TryParse(txtbox.Text, out tempdate);
                    //tempdate = DateTime.Parse(txtbox.Text);
                }
                txtbox = (TextBox)pnl.FindControl("txtstarttime" + id);
                if (txtbox.Text != "")
                {
                    time = txtbox.Text.Split(':');
                    tempdate = tempdate.AddHours(int.Parse(time[0]));
                    tempdate = tempdate.AddMinutes(int.Parse(time[1]));

                }
                details.allowancestartdate = tempdate;
                txtbox = (TextBox)pnl.FindControl("txtenddate" + id);
                if (txtbox.Text != "")
                {
                    DateTime.TryParse(txtbox.Text, out tempdate);

                    //tempdate = DateTime.Parse(txtbox.Text);

                }
                txtbox = (TextBox)pnl.FindControl("txtendtime" + id);
                if (txtbox.Text != "")
                {
                    time = txtbox.Text.Split(':');
                    tempdate = tempdate.AddHours(int.Parse(time[0]));
                    tempdate = tempdate.AddMinutes(int.Parse(time[1]));

                }
                details.allowanceenddate = tempdate;
                txtbox = (TextBox)pnl.FindControl("txtdeductamount" + id);
                if (txtbox.Text != "")
                {
                    details.allowancededuct = decimal.Parse(txtbox.Text);
                }
            }
        }

        txtbox = (TextBox)pnl.FindControl("txtvat" + id);
        if (txtbox != null)
        {
            if (txtbox.Text != "")
            {
                details.vat = decimal.Parse(txtbox.Text);
            }
        }
        if (subcat.calculation != CalculationType.PencePerMile && subcat.calculation != CalculationType.DailyAllowance && subcat.calculation != CalculationType.FixedAllowance && subcat.calculation != CalculationType.PencePerMileReceipt)
        {
            txtbox = (TextBox)pnl.FindControl("txttotal" + id);
            if (txtbox != null)
            {
                if (txtbox.Text != "")
                {
                    details.total = decimal.Parse(txtbox.Text);
                }
            }
        }
        ddlst = (DropDownList)pnl.FindControl("cmbpaymentmethod" + id);
        if (ddlst != null)
        {
            details.itItemtype = (ItemType)Convert.ToByte(ddlst.SelectedValue);
        }
        else
        {
            details.itItemtype = itemtype;
        }

        if (this.ActionContext.CurrentUser.Account.IsNHSCustomer)
        {
            ddlst = (DropDownList)pnl.FindControl("cmbESRAss" + id);
            if (ddlst != null && ddlst.SelectedIndex != -1)
            {
                details.assignmentid = Convert.ToInt32(ddlst.SelectedValue);
            }
            else
            {
                cESRAssignments assignments = this.ActionContext.EsrAssignments;

                DateTime theDate = (DateTime)Session["date"];

                if (assignments.ActiveAssignmentCount(theDate) == 1)
                {
                    foreach (KeyValuePair<int, cESRAssignment> e in assignments.getAssignmentsAssociated())
                    {
                        cESRAssignment assignment = (cESRAssignment)e.Value;
                        if (assignment.active)
                        {
                            details.assignmentid = assignment.sysinternalassignmentid;
                            break;
                        }
                    }
                }
            }
        }

        Table tbl = (Table)pnl.FindControl("tbl" + id);
        cTables clstables = this.ActionContext.Tables;
        cTable udftbl = clstables.GetTableByID(new Guid("65394331-792e-40b8-af8b-643505550783"));
        SortedList<int, object> userdefined = clsuserdefined.getItemsFromPage(ref tbl, udftbl, true, id);
        details.userdefined = userdefined;
        return details;
    }

    public Panel generateDropDowns(cExpenseItem expenseitem, MobileJourney journey)
    {
        bool corporatecard = false;
        var accountId = (int)ViewState["accountid"];
        var pnl = new Panel();
        var tbl = new Table();

        var subcats = this.ActionContext.SubCategories;
        cCategories categories = this.ActionContext.Categories;

        var row = new TableRow();
        var cell = new TableCell();
        var subCatId = this.ViewState["subcatid"] != null ? (int)this.ViewState["subcatid"] : 0;
        cell.Text = "Expense Category:";
        cell.CssClass = "labeltd";
        row.Cells.Add(cell);
        cell = new TableCell();
        var ddlst = new DropDownList();
        ddlst.ID = "cmbcategories";
        ddlst.SelectedIndexChanged += new EventHandler(cmbcategories_SelectedIndexChanged);
        ddlst.Items.AddRange(categories.PopulateCategoriesDropDownList(subcats.GetSubCatsByEmployeeItemRoles((int)this.ViewState["employeeid"])).ToArray());
        ddlst.Items.Insert(0, new ListItem("Please select an option", "0"));
        if (expenseitem != null)
        {
            SetSelectedCategoryOnDropDownList(expenseitem.subcatid, subcats, ref ddlst);
        }
        else if (journey != null)
        {
            SetSelectedCategoryOnDropDownList(journey.SubcatId, subcats, ref ddlst);
        }
        else
        {
            if (subCatId > 0)
            {
                SetSelectedCategoryOnDropDownList(subCatId, subcats, ref ddlst);
            }
        }

        ddlst.AutoPostBack = true;
        cell.Controls.Add(ddlst);

        row.Cells.Add(cell);
        cell = new TableCell();
        cell.Text = "Expense Item:";
        cell.CssClass = "labeltd";
        row.Cells.Add(cell);
        cell = new TableCell();
        ddlst = new DropDownList();
        ddlst.ID = "cmbsubcats";
        ddlst.AutoPostBack = true;
        if (expenseitem != null)
        {
            if (expenseitem.itemtype != ItemType.Cash)
            {
                corporatecard = true;
            }

            this.SetSelectedSubCatOnDropDownList(expenseitem.subcatid, ref ddlst, corporatecard, subcats, false);
        }
        else if (journey != null)
        {
            this.SetSelectedSubCatOnDropDownList(journey.SubcatId, ref ddlst, corporatecard, subcats, true);
        }
        else
        {
            if (subCatId > 0)
            {
                this.SetSelectedSubCatOnDropDownList(subCatId, ref ddlst, corporatecard, subcats, false);
            }
        }

        ddlst.SelectedIndexChanged += new EventHandler(cmbsubcat_SelectedIndexChanged);        
        cell.Controls.Add(ddlst);
        row.Cells.Add(cell);
        tbl.Rows.Add(row);
        
        pnl.Controls.Add(tbl);
        return pnl;
    }

    private void SetSelectedSubCatOnDropDownList(int subCatId, ref DropDownList ddlst, bool corporatecard, cSubcats subCats, bool mobilejourney)
    {
        var reqsubcat = subCats.GetSubcatBasic(subCatId);
        ddlst.Items.AddRange(this.PopulateExpensesDropDownList(subCats, reqsubcat.CategoryId, corporatecard, mobilejourney).ToArray());
        if (ddlst.Items.FindByValue(subCatId.ToString()) != null)
        {
            ddlst.Items.FindByValue(subCatId.ToString()).Selected = true;
        }
    }

    private static void SetSelectedCategoryOnDropDownList(int subCatId, cSubcats clssubcats, ref DropDownList ddlst)
    {
        var reqsubcat = clssubcats.GetSubcatBasic(subCatId);
        if (ddlst.Items.FindByValue(reqsubcat.CategoryId.ToString()) != null)
        {
            ddlst.Items.FindByValue(reqsubcat.CategoryId.ToString()).Selected = true;
        }
    }

    void cmbcategories_SelectedIndexChanged(object sender, EventArgs e)
    {
        var subcats = this.ActionContext.SubCategories;
        DropDownList lst = (DropDownList)sender;
        int categoryid = int.Parse(lst.SelectedValue);
        DropDownList cmbsubcats = (DropDownList)pnlspecific.FindControl("cmbsubcats");
        cmbsubcats.Items.Clear();
        ItemType itemtype = (ItemType)ViewState["itemtype"];
        bool corporatecard = itemtype != ItemType.Cash;
        bool mobilejourney = ViewState["mobileJourneyID"] != null && (int)ViewState["mobileJourneyID"] > 0 ? true : false;
        cmbsubcats.Items.AddRange(this.PopulateExpensesDropDownList(subcats, categoryid, corporatecard, mobilejourney).ToArray());
        cmbsubcats.Items.Insert(0, new ListItem("Please select an option", "0"));
    }

    void cmbsubcat_SelectedIndexChanged(object sender, EventArgs e)
    {
        ItemType itemtype = (ItemType)ViewState["itemtype"];


        int transactionid = (int)ViewState["transactionid"];

        cSubcats clssubcats = this.ActionContext.SubCategories;
        DropDownList lst = (DropDownList)sender;
        int subcatid = int.Parse(lst.SelectedValue);
        if (subcatid == 0)
        {
            return;
        }
        SortedList<int, cExpenseItem> tempitems = getItems();
        if (tempitems.Count > 0)
        {
            tempitems.Values[0].subcatid = subcatid;
            if (tempitems.Values[0].expenseid > 0) //editing
            {
                ViewState["expenseitem"] = tempitems.Values[0];
            }
        }

        cItemBuilder clsbuilder;
        if (transactionid > 0)
        {
            clsbuilder = new cItemBuilder((int)ViewState["accountid"], (int)ViewState["employeeid"], tempitems.Values[0].date, (int)ViewState["subAccountID"], itemtype, transactionid, (int)ViewState["currencyid"], (int)ViewState["countryid"], (int?)ViewState["mobileID"], (int?)ViewState["mobileJourneyID"]);
        }
        else if (ViewState["mobileJourneyID"] != null && (int)ViewState["mobileJourneyID"] > 0)
        {
            clsbuilder = new cItemBuilder((int)ViewState["accountid"], (int)ViewState["employeeid"], tempitems.Values[0].date, (int)ViewState["subAccountID"], itemtype, transactionid, (int)ViewState["currencyid"], (int)ViewState["countryid"], null, (int)ViewState["mobileJourneyID"]);
        }
        else
        {
            clsbuilder = new cItemBuilder((int)ViewState["accountid"], (int)ViewState["employeeid"], tempitems.Values[0].date, (int)ViewState["subAccountID"], (int)ViewState["currencyid"], (int)ViewState["countryid"], itemtype, this.Page);
        }

        //get the current date of the xpense item being claimed
        HiddenField hdnDate = (HiddenField)pnlgeneral.FindControl("hdnExpdate");
        DateTime date = (DateTime)ViewState["HomeOrOfficeDate"];

        if (hdnDate.Value != "")
        {
            DateTime.TryParse(hdnDate.Value, out date);
        }

        cSubcat reqsub = clssubcats.GetSubcatById(subcatid);

        cEmployees employees = this.ActionContext.Employees;
        Employee employee = employees.GetEmployeeById((int)ViewState["employeeid"]);

        pnlspecific.Controls.Remove(pnlspecific.FindControl("pnl0"));
        if (tempitems.Count > 0)
        {
            pnlspecific.Controls.Add(clsbuilder.generateItem("0", reqsub, null, tempitems.Values[0], false, date, employee, Request, this.ActionContext));
        }
        else
        {
            pnlspecific.Controls.Add(clsbuilder.generateItem("0", reqsub, null, null, false, date, employee, Request, this.ActionContext));
        }
        ViewState["subcatid"] = subcatid;
    }
    

    private double getExchange(int accountid, int employeeid, int currencyid, DateTime date)
    {
        cCurrencies clscurrencies = this.ActionContext.Currencies;
        double exchangerate = 0;
        cMisc clsmisc = this.ActionContext.Misc;
        cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(accountid);

        cEmployees clsemployees = this.ActionContext.Employees;
        Employee reqemp = clsemployees.GetEmployeeById(employeeid);

        int basecurrency = reqemp.PrimaryCurrency != 0 ? reqemp.PrimaryCurrency : clsproperties.basecurrency;
        exchangerate = clscurrencies.getCurrencyById(basecurrency).getExchangeRate(currencyid, date);
        if (exchangerate == 0)
        {
            exchangerate = 1;
        }
        return exchangerate;
    }

    #region Dropdown Filtering Methods

    private void filterDropdownsOnPageStart()
    {
        cMisc clsmisc = this.ActionContext.Misc;
        cGlobalProperties clsproperties = clsmisc.GetGlobalProperties((int)ViewState["accountid"]);

        DropDownList ddlst;
        TextBox textBox;

        if (clsproperties.costcodeson && clsproperties.usecostcodes && clsproperties.usecostcodeongendet == false)
        {
            for (int i = 0; i < (tblcostcodes.Rows.Count - 1); i++)
            {
                textBox = (TextBox)UpdatePanel1.FindControl("txtCostCode" + i + "_ID");
                if (textBox != null && textBox.Text != "")
                {
                    populateChildDropdowns(FilterType.Costcode, int.Parse(textBox.Text), i.ToString());
                }
            }
        }
        else if (clsproperties.costcodeson && clsproperties.usecostcodes && clsproperties.usecostcodeongendet)
        {
            textBox = (TextBox)pnlgeneral.FindControl("txtCostCode_ID");
            if (textBox != null && textBox.Text != "")
            {
                populateChildDropdowns(FilterType.Costcode, int.Parse(textBox.Text), "0");
            }
        }

        if (clsproperties.departmentson && clsproperties.usedepartmentcodes && clsproperties.usedepartmentongendet == false)
        {
            for (int i = 0; i < (tblcostcodes.Rows.Count - 1); i++)
            {
                ddlst = (DropDownList)UpdatePanel1.FindControl("cmbdepartment" + i);
                if (ddlst != null && ddlst.SelectedValue != "")
                {
                    populateChildDropdowns(FilterType.Department, int.Parse(ddlst.SelectedValue), i.ToString());
                }
            }
        }
        else if (clsproperties.departmentson && clsproperties.usedepartmentcodes && clsproperties.usedepartmentongendet)
        {
            ddlst = (DropDownList)pnlgeneral.FindControl("cmbgendepartment");
            if (ddlst != null && ddlst.SelectedValue != "")
            {
                populateChildDropdowns(FilterType.Department, int.Parse(ddlst.SelectedValue), "0");
            }
        }
        if (clsproperties.projectcodeson && clsproperties.useprojectcodes && clsproperties.useprojectcodeongendet == false)
        {
            for (int i = 0; i < (tblcostcodes.Rows.Count - 1); i++)
            {
                ddlst = (DropDownList)UpdatePanel1.FindControl("cmbprojectcode" + i);
                if (ddlst != null && ddlst.SelectedValue != "")
                {
                    populateChildDropdowns(FilterType.Projectcode, int.Parse(ddlst.SelectedValue), i.ToString());
                }
            }
        }
        else if (clsproperties.projectcodeson && clsproperties.useprojectcodes && clsproperties.useprojectcodeongendet)
        {
            ddlst = (DropDownList)pnlgeneral.FindControl("cmbgenprojectcode");
            if (ddlst != null && ddlst.SelectedValue != "")
            {
                populateChildDropdowns(FilterType.Projectcode, int.Parse(ddlst.SelectedValue), "0");
            }
        }

        int numitems;
        if (ViewState["numitems"] != null)
        {
            numitems = int.Parse(ViewState["numitems"].ToString());
        }
        else
        {
            numitems = 1;
        }

        for (int i = 0; i < numitems; i++)
        {
            Panel subpnl = (Panel)pnlspecific.FindControl("pnl" + i);
            if (subpnl != null)
            {
                ddlst = (DropDownList)subpnl.FindControl("cmbreason" + i);

                if (ddlst != null && ddlst.SelectedValue != "")
                {
                    populateChildDropdowns(FilterType.Reason, int.Parse(ddlst.SelectedValue), i.ToString());
                }

                //textBox = (TextBox)subpnl.FindControl(i + "_txtOrganisation_ID");

                //if (textBox != null && textBox.Text != "")
                //{
                //    populateChildDropdowns(FilterType.Location, int.Parse(textBox.Text), i.ToString());
                //}
            }

            ddlst = (DropDownList)pnlgeneral.FindControl("cmbreason");
            if (ddlst != null && ddlst.SelectedValue != "")
            {
                populateChildDropdowns(FilterType.Reason, int.Parse(ddlst.SelectedValue), "");
            }

            //textBox = (TextBox)pnlgeneral.FindControl("txtOrganisation_ID");

            //if (textBox != null && textBox.Text != "")
            //{
            //    populateChildDropdowns(FilterType.Location, Convert.ToInt32(textBox.Text), i.ToString());
            //}
        }
    }
    private void populateChildDropdowns(FilterType filtertype, int id, string ctlindex)
    {
        List<FilterType> types = new List<FilterType>();
        List<int> filterids = new List<int>();
        Dictionary<int, cFilterRule> filterrules = this.ActionContext.FilterRules.GetFilterRulesByType(filtertype);
        cEmployees clsemployees = this.ActionContext.Employees;
        Employee reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);
        ItemType itemtype = (ItemType)ViewState["itemtype"];
        Action action = (Action)ViewState["action"];
        cExpenseItem expenseitem = null;

        foreach (cFilterRule rule in filterrules.Values)
        {
            if (rule.rulevals.Count > 0 && rule.enabled)
            {
                types.Add(rule.child);
                filterids.Add(rule.filterid);
            }
        }

        if (action == Action.Edit || action == Action.Copy)
        {
            SortedList<int, cExpenseItem> expitems = (SortedList<int, cExpenseItem>)ViewState["items"];
            expenseitem = expitems.Values[0];
        }

        #region Get Breakdown

        List<cDepCostItem> breakdown = new List<cDepCostItem>();

        if (ViewState["costcodebreakdown"] != null)
        {
            breakdown = (List<cDepCostItem>)ViewState["costcodebreakdown"];
        }
        else
        {
            if (action == Action.Add)
            {
                cDepCostItem[] lstCostCodes = reqemp.GetCostBreakdown().ToArray();
                if (lstCostCodes.Length == 0)
                {
                    breakdown.Add(new cDepCostItem(0, 0, 0, 100));
                }
                else
                {
                    for (int i = 0; i < lstCostCodes.GetLength(0); i++)
                    {
                        breakdown.Add(lstCostCodes[i]);
                    }
                }
            }
        }

        #endregion

        List<ListItem> items = new List<ListItem>();
        for (int i = 0; i < filterids.Count; i++)
        {
            string area = "";
            string ctlid = "";

            if (types[i] == FilterType.Userdefined)
            {
                cUserdefinedFields clsudf = this.ActionContext.UserDefinedFields;
                int childuserdefineid = 0;
                cFilterRule rule = this.ActionContext.FilterRules.GetFilterRuleById(filterids[i]);

                childuserdefineid = rule.childuserdefineid;

                cUserDefinedField field = clsudf.GetUserDefinedById(childuserdefineid);

                ctlid = "cmbudf" + field.userdefineid;

                if (field.Specific == false)
                {
                    area = "general";
                }
                else
                {
                    area = "individual";
                }
            }
            else
            {
                string temp = this.ActionContext.FilterRules.getChildTargetControl(types[i]);
                if (!string.IsNullOrEmpty(temp))
                {
                    string[] ctl = temp.Split(';');

                    if (ctl.Length > 0)
                    {
                        ctlid = ctl[0];

                        if (ctl.Length > 1)
                        {
                            area = ctl[1];
                        }
                    }

                }
            }

            var ddlChildren = new List<DropDownList>();
            var txtChildren = new List<TextBox>();

            bool isForCostCode = ctlid.Contains("txtCostCode");

            var showCostCodeDescription = false;

            if (isForCostCode)
            {
                if (this.ActionContext.Properties.UseCostCodeDescription)
                {
                    showCostCodeDescription = true;
                }
            }

            items = popDropdown(filterids[i], types[i], id);

            switch (area)
            {
                case "general":

                    if (isForCostCode)
                    {
                        txtChildren.Add((TextBox)pnlgeneral.FindControl(ctlid)); 
                    }
                    else
                    {
                       ddlChildren.Add((DropDownList)pnlgeneral.FindControl(ctlid));  
                    }

                   
                        break;

                case "breakdown":
                        if (ctlindex != "")
                        {
                            if (isForCostCode)
                            {
                                txtChildren.Add((TextBox)UpdatePanel1.FindControl(ctlid + ctlindex));
                            }
                            else
                            {
                                ddlChildren.Add((DropDownList)UpdatePanel1.FindControl(ctlid + ctlindex));
                            }                   
                        }
                        else
                        {
                            int breakdownrows = int.Parse(ViewState["breakdownrows"].ToString());

                            for (int j = 0; j < breakdownrows; j++)
                            {
                                if (isForCostCode)
                                {
                                    txtChildren.Add((TextBox)UpdatePanel1.FindControl(ctlid + j));
                                }
                                else
                                {
                                    ddlChildren.Add((DropDownList)UpdatePanel1.FindControl(ctlid + j));
                                }                        
                            }
                        }
                        break;
                    
                case "individual":
                    {
                        Panel subpnl;
                        if (ctlindex != "")
                        {
                            subpnl = (Panel)pnlspecific.FindControl("pnl" + ctlindex);

                            if (isForCostCode)
                            {
                                txtChildren.Add((TextBox)subpnl.FindControl(ctlid + ctlindex));  
                            }
                            else
                            {
                              
                            ddlChildren.Add((DropDownList)subpnl.FindControl(ctlid + ctlindex));  
                            }

                        }
                        else
                        {
                            int numitems = int.Parse(ViewState["numitems"].ToString());

                            for (int j = 0; j < numitems; j++)
                            {
                                subpnl = (Panel)pnlspecific.FindControl("pnl" + j);
                                if (subpnl != null)
                                {
                                    if (isForCostCode)
                                    {
                                        txtChildren.Add((TextBox)subpnl.FindControl(ctlid + j));  
                                    }
                                    else
                                    {
                                       
                                      ddlChildren.Add((DropDownList)subpnl.FindControl(ctlid + j)); 
                                    }

                                }
                                        }
                                    }
                        break;
                                }
                            }

            ddlChildren.RemoveAll(o => o == null);
            txtChildren.RemoveAll(o => o == null);

            if (items.Count != 0)
            {
                var includeNone = (items.Count > 1);
                foreach (var ddlChild in ddlChildren)
                {
                    ListItem noneItem = (includeNone ? ddlChild.Items.FindByValue("0") : null);
                    ddlChild.Items.Clear();
                    if (noneItem != null)
                    {
                        ddlChild.Items.Add(noneItem);
                        }
                    ddlChild.Items.AddRange(items.ToArray());
                    }
            }

            foreach (var txtbox in txtChildren)
            {
                if (!string.IsNullOrEmpty(ctlindex))
                {
                    //string ids = "#" + "ctl00_contentmain_txtcostcode1";

                    var list = new List<AutoCompleteChildFieldValues>();

                    foreach (var item in items)
                    {
                        var d = new AutoCompleteChildFieldValues();
                        d.FieldToBuild = "";
                        d.Key = Convert.ToInt32(item.Value);
                        d.Value = "";
                        d.FormattedText = string.Empty;
                        list.Add(d);

                      
                    }

                    if (list.Count > 0)
                    {
                        //remove previous autocomplete binding
                        txtbox.CssClass = "";

                        var jsonSerialiser = new JavaScriptSerializer();
                        var json = JsonConvert.SerializeObject(list);

                        string displayField;

                        displayField = showCostCodeDescription ? "AF80D035-6093-4721-8AFC-061424D2AB72" : "359DFAC9-74E6-4BE5-949F-3FB224B1CBFC";
                                           
                        var sb = new StringBuilder();
                        sb.Append("SEL.AutoComplete.Bind(");
                        sb.Append("'ctl00_contentmain_" + txtbox.ID + "',");
                        sb.Append("25,");
                        sb.Append("'02009E21-AA1D-4E0D-908A-4E9D73DDFBDF',");
                        sb.Append("'" + displayField + "',");
                        sb.Append("'359DFAC9-74E6-4BE5-949F-3FB224B1CBFC, AF80D035-6093-4721-8AFC-061424D2AB72',");
                        sb.Append("null,");
                        sb.Append("'{ 0: { \"FieldID\": \"8178629C-5908-4458-89F6-D7EE7438314D\", \"ConditionType\": 1, \"ValueOne\": \"0\", \"ValueTwo\": \"\", \"Order\": 0, \"JoinViaID\": 0 } }',");
                        sb.Append("500, null, \"False\"," + json + ", \"False\", null);");

                        ScriptManager.RegisterStartupScript (this, this.GetType(), "myscript" + txtbox.ID, sb.ToString(), true);


                        //    $('input.costcode-autocomplete').focus(function () {

                        //    if (!$(this).val()) {
                        //        $(this).autocomplete("search", "%%%");
                        //    }
                        //}

                    

                    }

                  //var scripty = new StringBuilder();
                  //      scripty.Append("('#ctl00_contentmain_" + txtbox.ID + "').focus(function () {");
                  //      scripty.Append("if (!(this).val()) {");
                  //      scripty.Append("(this).autocomplete(\"search\", \"%%%\");");
                  //      scripty.Append(" }})");

                  //      ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "myscriptsearc" + txtbox.ID, scripty.ToString(), true);
                

                 
                
               

                    //register start up script with ddl details.

                    if (breakdown[int.Parse(ctlindex)].costcodeid.ToString() != "0")
                    {
                        var costcode = ActionContext.CostCodes.GetCostcodeById(breakdown[int.Parse(ctlindex)].costcodeid);

                        txtbox.Text = showCostCodeDescription ? costcode.Description : costcode.Costcode;
                    }                                   
                }

                break;
            }

            foreach (var ddlst in ddlChildren)
            {
                switch (types[i])
                {
                    //case FilterType.Costcode:
                    //    if (!string.IsNullOrEmpty(ctlindex))
                    //    {
                    //        if (ddlst.Items.FindByValue(breakdown[int.Parse(ctlindex)].costcodeid.ToString()) != null)
                    //        {
                    //            //ddlst.Items.FindByValue(breakdown[0].costcodeid.ToString()).Selected = true;
                    //            ddlst.SelectedValue = breakdown[int.Parse(ctlindex)].costcodeid.ToString();
                    //        }
                    //    }

                    //    break;
                    case FilterType.Department:
                        if (!string.IsNullOrEmpty(ctlindex))
                        {
                            if (ddlst.Items.FindByValue(breakdown[int.Parse(ctlindex)].departmentid.ToString()) != null)
                            {
                                //ddlst.Items.FindByValue(breakdown[0].departmentid.ToString()).Selected = true;
                                ddlst.SelectedValue = breakdown[int.Parse(ctlindex)].departmentid.ToString();
                            }
                        }

                        break;

                    case FilterType.Projectcode:
                        if (!string.IsNullOrEmpty(ctlindex))
                        {
                            if (ddlst.Items.FindByValue(breakdown[int.Parse(ctlindex)].projectcodeid.ToString()) != null)
                            {
                                //ddlst.Items.FindByValue(breakdown[0].projectcodeid.ToString()).Selected = true;
                                ddlst.SelectedValue = breakdown[int.Parse(ctlindex)].projectcodeid.ToString();
                            }
                        }

                        break;
                    case FilterType.Reason:

                        if (action == Spend_Management.Action.Edit || action== Action.Copy)
                        {
                            if (ddlst.Items.FindByValue(expenseitem.reasonid.ToString()) != null)
                            {
                                //ddlst.Items.FindByValue(expenseitem.reasonid.ToString()).Selected = true;
                                ddlst.SelectedValue = expenseitem.reasonid.ToString();
                            }
                        }
                        else if (action == Spend_Management.Action.Add && itemtype == ItemType.Cash)
                        {
                            if (Session["reasonid"] != null)
                            {
                                if (ddlst.Items.FindByValue(Session["reasonid"].ToString()) != null)
                                {
                                    //ddlst.Items.FindByValue(Session["reasonid"].ToString()).Selected = true;
                                    ddlst.SelectedValue = Session["reasonid"].ToString();
                                }
                            }
                        }

                        break;
                }
            }
        }
    }

    private List<ListItem> popDropdown(int filterid, FilterType type, int parentid)
    {
        cFilterRule rule = this.ActionContext.FilterRules.GetFilterRuleById(filterid);
        Dictionary<int, cFilterRuleValue> lstRuleVals = rule.rulevals;
        cMisc clsmisc = this.ActionContext.Misc;
        cGlobalProperties clsproperties = clsmisc.GetGlobalProperties((int)ViewState["accountid"]);

        List<ListItem> lstItems = new List<ListItem>();

        if (type == FilterType.Userdefined)
        {
            #region userdefined
            cUserdefinedFields clsudf = this.ActionContext.UserDefinedFields;
            int childuserdefineid = 0;



            childuserdefineid = rule.childuserdefineid;

            cUserDefinedField field = clsudf.GetUserDefinedById(childuserdefineid);


            foreach (cFilterRuleValue val in lstRuleVals.Values)
            {
                if (val.parentid == parentid)
                {
                    foreach (KeyValuePair<int, cListAttributeElement> kvp in field.items)
                    {
                        if (val.childid == kvp.Key)
                        {
                            lstItems.Add(new ListItem(kvp.Value.elementText.ToString(), val.childid.ToString()));
                        }
                    }
                }
            }

            #endregion
        }
        else
        {
            foreach (cFilterRuleValue val in lstRuleVals.Values)
            {
                if (val.parentid == parentid)
                {
                    string item = "";
                    switch (type)
                    {
                        case FilterType.Costcode:
                            item = this.ActionContext.FilterRules.GetParentOrChildItem(type, val.childid, false, clsproperties.usecostcodedesc);
                            break;
                        case FilterType.Department:
                            item = this.ActionContext.FilterRules.GetParentOrChildItem(type, val.childid, false, clsproperties.usedepartmentdesc);
                            break;
                        case FilterType.Projectcode:
                            item = this.ActionContext.FilterRules.GetParentOrChildItem(type, val.childid, false, clsproperties.useprojectcodedesc);
                            break;
                        case FilterType.Reason:
                            item = this.ActionContext.FilterRules.GetParentOrChildItem(type, val.childid, false, false);
                            break;
                        case FilterType.Userdefined:
                            item = this.ActionContext.FilterRules.GetParentOrChildItem(type, val.childid, false, false);
                            break;
                        //case FilterType.Location:
                        //    item = clsfilterrules.getParentOrChildItem(type, val.childid, false, false);
                        //    break;
                    }

                    lstItems.Add(new ListItem(item, val.childid.ToString()));
                }
            }
        }

        lstItems = lstItems.OrderBy(o => o.Text).ToList();
              
        return lstItems;
    }
    #endregion

    #region Web Methods
    [WebMethod(EnableSession = true)]
    public static object[] getExchangeRate(int accountid, int employeeid, int currencyid, DateTime date)
    {
        CurrentUser user = cMisc.GetCurrentUser();
        cCurrencies clscurrencies = new cCurrencies(accountid, user.CurrentSubAccountId);
        double exchangerate = 0;
        employeeid = cMisc.GetCurrentUser().Employee.EmployeeID;
        cMisc clsmisc = new cMisc(accountid);
        cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(accountid);

        cEmployees clsemployees = new cEmployees(accountid);
        Employee reqemp = clsemployees.GetEmployeeById(employeeid);
        int basecurrency = reqemp.PrimaryCurrency != 0 ? reqemp.PrimaryCurrency : clsproperties.basecurrency;

        cCurrency currency = clscurrencies.getCurrencyById(basecurrency);
        exchangerate = currency.getExchangeRate(currencyid, date);
        if (exchangerate == 0)
        {
            exchangerate = 1;
        }

        object[] data = new object[2];
        if (currencyid == basecurrency) // don't display
        {
            data[0] = false;
        }
        else
        {
            data[0] = true;
            data[1] = exchangerate;
        }
        return data;
    }

    //[WebMethod(EnableSession = true)]
    //public static object[] getLocationFilterChildren(int accountid, string ctlindex)
    //{
    //    cMisc clsmisc = new cMisc(accountid);
    //    cFilterRules clsfilterrules = new cFilterRules(accountid);
    //    Dictionary<int, cFilterRule> filterrules = clsfilterrules.getFilterRulesByType(FilterType.Location);

    //    string sType = "";
    //    string sFilterid = "";
    //    string ctlName = "";

    //    foreach (cFilterRule rule in filterrules.Values)
    //    {
    //        if (rule.rulevals.Count > 0 && rule.enabled)
    //        {
    //            int type = (int)rule.child;
    //            sType += type.ToString() + ";";
    //            sFilterid += rule.filterid.ToString() + ";";
    //        }
    //    }

    //    cFieldToDisplay company = clsmisc.GetGeneralFieldByCode("organisation");
    //    if (company.individual)
    //    {
    //        ctlName = ctlindex + "_txtOrganisation_ID";
    //    }
    //    else
    //    {
    //        ctlName = "txtOrganisation_ID";
    //    }
    //    object[] children = new object[] { ctlName, sType, sFilterid, ctlindex, accountid };

    //    return children;
    //}
    #endregion

    protected void cmdcancel_Click(object sender, EventArgs eventArgs)
    {
        Response.Redirect(getCancelLocation(), true);

    }

    private string getCancelLocation()
    {
        string location = "";
        int claimid = 0;
        claimid = (int)ViewState["claimid"];
        int returnto = (int)ViewState["returnto"];

        switch (returnto)
        {
            case 1:
                location = "";
                break;
            case 2:

                location = "expenses/claimViewer.aspx?returned=1&claimid=" + claimid;
                break;
            case 3:

                location = "expenses/checkexpenselist.aspx?claimid=" + claimid;
                break;
            default:
                location = "expenses/claimViewer.aspx?claimid=" + ViewState["claimid"] + ((Request.QueryString["claimSelector"] != null) ? "&claimSelector=1" : string.Empty);
                break;
        }

        return location;
    }

    private void EmployeeBankAccountCheck(int accountId, int employeeId, bool mustHaveBankAccount)
    {
        if (mustHaveBankAccount)
        {
            var bankAccounts = this.ActionContext.BankAccounts;
            var employeeBankAccounts = bankAccounts.GetActiveAccountByEmployeeId(employeeId);

            if (employeeBankAccounts.Count < 1)
            {
                Response.Redirect(ErrorHandlerWeb.NoBankAccount, true);                     
            }
        }
    }

    /// <summary>
    ///  Builds up a List of subcat data for a categoryId. Used for the expense items drop down list 
    /// </summary>
    /// <param name="subCats">An instance of <see cref="cSubcats">cSubcats</see></param>
    /// <param name="categoryId">The category Id</param>
    /// <param name="isCorpCard">Is it for a corporate card</param>
    /// <param name="isMobileJourney">Is it for a mobile journey</param>
    /// <returns>a List of listeItems</returns>
    private List<ListItem> PopulateExpensesDropDownList(cSubcats subCats, int categoryId, bool isCorpCard, bool isMobileJourney)
    {
        SortedList<int, SubcatItemRoleBasic> subCatsItems = subCats.GetExpenseSubCatsForCategory(categoryId, isCorpCard, isMobileJourney, (int)ViewState["employeeid"]);

        var result = new List<ListItem>();
        foreach (KeyValuePair<int, SubcatItemRoleBasic> itemRoleBasic in subCatsItems)
        {
            var item = new ListItem(itemRoleBasic.Value.Subcat, itemRoleBasic.Key.ToString());
            item.Attributes.Add("StartDate", itemRoleBasic.Value.StartDate.ToString("yyyy-MM-dd"));
            item.Attributes.Add("EndDate", itemRoleBasic.Value.EndDate.ToString("yyyy-MM-dd"));
            result.Add(item);
        }

        return result;
    }

    internal void addDestinationRow(int id)
    {
        SortedList<int, cExpenseItem> tempitems = getItems();
        cExpenseItem item = tempitems[id];
        item.addJourneyStep();
        ViewState["items"] = tempitems;
        pnlspecific.Controls.Clear();
        generateSpecificDetails();
        upnlSpecific.Update();
    }

    internal void removeDestinationRow(int id, int index)
    {
        SortedList<int, cExpenseItem> tempitems = getItems();
        cExpenseItem item = tempitems[id];
        item.removeJourneyStep(index);
        ViewState["items"] = tempitems;
        pnlspecific.Controls.Clear();
        generateSpecificDetails();
        upnlSpecific.Update();
    }

    /// <summary>
    /// Returns the distance between fromCompanyName and toCompanyName.
    /// </summary>
    /// <param name="fromCompanyName"></param>
    /// <param name="toAddressText"></param>
    /// <param name="date"></param>
    /// <param name="carID"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public static object[] GetDistance(int fromAddressId, string fromAddressText, int toAddressId, string toAddressText, DateTime date, int carID)
    {
        CurrentUser currentUser = cMisc.GetCurrentUser();
        object[] distance = new object[2];
        cMileagecats clsmileagecats = new cMileagecats(currentUser.AccountID);
        Address toAddress = null;
        Address fromAddress = null;
        cEmployeeCars clsEmployeeCars = new cEmployeeCars(currentUser.AccountID, currentUser.EmployeeID);

        cMisc clsmisc = new cMisc(currentUser.AccountID);
        cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(currentUser.AccountID);

        cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
        cAccountProperties reqProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties.Clone(); // clsSubAccounts.getFirstSubAccount().SubAccountProperties.Clone();

        if (fromAddressText.Trim().ToLower() == "office" || fromAddressText.Trim().ToLower() == "home")
        {
            if (fromAddressText.Trim().ToLower() == "home")
            {
                cEmployeeHomeLocation tmpHomeLocation = currentUser.Employee.GetHomeAddresses().GetBy(date);
                if (tmpHomeLocation != null)
                {
                    fromAddress = Address.Get(currentUser.AccountID, tmpHomeLocation.LocationID);
                }
            }
            else
            {
                cEmployeeWorkLocation tmpWorkLocation = currentUser.Employee.GetWorkAddresses().GetBy(date);
                if (tmpWorkLocation != null)
                {
                    fromAddress = Address.Get(currentUser.AccountID, tmpWorkLocation.LocationID);
                }
            }
        }
        else
        {
            fromAddress = Address.Get(currentUser.AccountID, fromAddressId);
        }

        if (toAddressText.Trim().ToLower() == "office" || toAddressText.Trim().ToLower() == "home")
        {
            if (toAddressText.Trim().ToLower() == "home")
            {
                cEmployeeHomeLocation tmpHomeLocation = currentUser.Employee.GetHomeAddresses().GetBy(date);
                if (tmpHomeLocation != null)
                {
                    toAddress = Address.Get(currentUser.AccountID, tmpHomeLocation.LocationID);
                }
            }
            else
            {
                cEmployeeWorkLocation tmpWorkLocation = currentUser.Employee.GetWorkAddresses().GetBy(date);
                if (tmpWorkLocation != null)
                {
                    toAddress = Address.Get(currentUser.AccountID, tmpWorkLocation.LocationID);
                }
            }
        }
        else
        {
            toAddress = Address.Get(currentUser.AccountID, toAddressId);
        }

        if (toAddress != null && fromAddress != null)
        {
            decimal? manuallyEnteredDistance = AddressDistance.GetCustom(currentUser.Account, fromAddress, toAddress);

            // A manually entered distance has been found so return this.
            if (manuallyEnteredDistance.HasValue && manuallyEnteredDistance.Value > 0)
            {
                distance[0] = 1;
                distance[1] = manuallyEnteredDistance.Value;
            }
            // Perform a postcode lookup via postcodeanywhere
            else
            {
                // the account option "Postcode Anywhere" is off
                if (reqProperties.UseMapPoint == false)
                {
                    distance[0] = 1;
                    distance[1] = 0;
                }
                else if (toAddress.Postcode != string.Empty && fromAddress.Postcode != string.Empty)
                {
                    cAccount account = new cAccounts().GetAccountByID(currentUser.AccountID);

                    distance[0] = 1;
                    distance[1] = AddressDistance.GetRecommendedDistance(currentUser, fromAddress, toAddress, reqProperties.MileageCalcType, account.MapsEnabled);

                    if (carID == 0)
                    {
                        carID = clsEmployeeCars.GetDefaultCarID(clsproperties.blocktaxexpiry, clsproperties.blockmotexpiry, clsproperties.blockinsuranceexpiry, clsproperties.BlockBreakdownCoverExpiry, reqProperties.DisableCarOutsideOfStartEndDate, date);
                    }
                    cCar car = clsEmployeeCars.GetCarByID(carID);

                    if (car != null && car.defaultuom == MileageUOM.KM)
                    {
                        distance[1] = clsmileagecats.convertMilesToKM(Convert.ToDecimal(distance[1]));
                    }
                }
                else
                {
                    distance[0] = -1;
                    distance[1] = 0;
                }
            }
        }
        else
        {
            // One of the companies could not be found.
            distance[0] = -2;
        }

        return distance;
    }

}