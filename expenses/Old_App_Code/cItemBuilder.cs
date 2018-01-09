using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using expenses.Bootstrap;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Mobile;

using Spend_Management;
using SpendManagementLibrary;
using SpendManagementLibrary.Addresses;
using SpendManagementLibrary.Employees;
using SpendManagementLibrary.Hotels;
using Common.Logging;

using Spend_Management.expenses.code;
using Spend_Management.shared.code.Mileage;

using ExpenseItem = SpendManagementLibrary.Mobile.ExpenseItem;
using Spend_Management.shared.code.Mobile;
using SpendManagementLibrary.Flags;
using Spend_Management.shared.code;
using SpendManagementLibrary.Employees.DutyOfCare;
using SpendManagementLibrary.Enumerators;

/// <summary>
/// Summary description for cItemBuilder
/// </summary>
public class cItemBuilder
{
    /// <summary>
    /// An instance of <see cref="ILog"/> for logging information.
    /// </summary>
    private static readonly ILog Log = new LogFactory<cItemBuilder>().GetLogger();

    private int nAccountid;

    private int nEmployeeid;

    private int nTransactionid;

    private ItemType itItem;

    private int nSelectedCurrency;

    private int nSelectedCountry;

    private cCardTransaction cardtransaction;

    private ExpenseItem mobileItem;

    /// <summary>
    /// The journey from a mobile device.
    /// </summary>
    private MobileJourney mobileJourney;

    private cMisc clsmisc;

    private cGlobalProperties clsproperties;

    private cAccountProperties accountProperties;

    private Page pgAddPage;
    private readonly MileageGridBuilder mileageGridBuilder;
    private Employee employee;
    private cAccountSubAccount subAccount;
    private CurrentUser currentUser;
    private readonly cEmployeeCars clsEmployeeCars;
    private List<Control> dependsOnMileageGrid = new List<Control>();

    /// <summary>
    /// Error message for invalid licence.
    /// </summary>
    private const string InvalidLicenceErrorMessage = "Mileage cannot be claimed because your driving licence status is set as banned/disqualified.";

    public cItemBuilder(int accountId, int employeeId, DateTime expenseItemDate, int? subAccountId = null)
    {
        nAccountid = accountId;
        nEmployeeid = employeeId;

        var employees = new cEmployees(accountId);
        employee = employees.GetEmployeeById(employeeId);
        var subAccounts = new cAccountSubAccounts(accountId);
        subAccount = subAccounts.getSubAccountById(subAccountId ?? employee.DefaultSubAccount);
        accountProperties = subAccount.SubAccountProperties.Clone();
        currentUser = cMisc.GetCurrentUser();
        clsEmployeeCars = new cEmployeeCars(accountId, employeeid, expenseItemDate);
    }

    public cItemBuilder(int accountid, int employeeid, DateTime expenseItemDate, int subaccountid, int selectedcurrency, int selectedcountry, ItemType itemtype, Page addpage)
        : this(accountid, employeeid, expenseItemDate, subaccountid)
    {
        pgAddPage = addpage;
        nSelectedCurrency = selectedcurrency;
        nSelectedCountry = selectedcountry;
        itItem = itemtype;
        clsmisc = new cMisc(accountid);
        clsproperties = clsmisc.GetGlobalProperties(accountid);
        mileageGridBuilder = new MileageGridBuilder();
    }

    public cItemBuilder(int accountid, int employeeid, DateTime expenseItemDate, int subaccountid, ItemType itemtype, int transactionid, int selectedcurrency, int selectedcountry, int? mobileID, int? mobileJourneyID)
        : this(accountid, employeeid, expenseItemDate, subaccountid)
    {
        nAccountid = accountid;
        nEmployeeid = employeeid;
        nTransactionid = transactionid;
        nSelectedCurrency = selectedcurrency;
        nSelectedCountry = selectedcountry;
        itItem = itemtype;
        if (itemtype == ItemType.CreditCard || itemtype == ItemType.PurchaseCard)
        {
            cardtransaction = getCardTransaction();
        }

        if (mobileID != null && mobileID > 0)
        {
            cMobileDevices clsmobile = new cMobileDevices(accountid);
            mobileItem = clsmobile.getMobileItemByID((int)mobileID);
            mileageGridBuilder = new MileageGridBuilder();
        }

        if (mobileJourneyID.HasValue && mobileJourneyID.Value > 0)
        {
            cMobileDevices clsmobile = new cMobileDevices(accountid);
            this.mobileJourney = clsmobile.GetMobileJourney(mobileJourneyID.Value);
            this.mileageGridBuilder = new MileageGridBuilder();
        }

        clsmisc = new cMisc(accountid);
        clsproperties = clsmisc.GetGlobalProperties(accountid);
    }

    #region properties

    public int accountid
    {
        get
        {
            return nAccountid;
        }
    }

    public int employeeid
    {
        get
        {
            return nEmployeeid;
        }
    }

    public ItemType itemtype
    {
        get
        {
            return itItem;
        }
    }

    public int transactionid
    {
        get
        {
            return nTransactionid;
        }
    }

    public int selectedcurrency
    {
        get
        {
            return nSelectedCurrency;
        }
    }

    public int selectedcountry
    {
        get
        {
            return nSelectedCountry;
        }
    }

    #endregion

    private cCardTransaction getCardTransaction()
    {
        cCardStatements clsstatements = new cCardStatements(accountid);

        cCardTransaction trans = clsstatements.getTransactionById(transactionid);
        return trans;
    }



    public Panel generateItem(string id, cSubcat subcat, cSubcat parentsubcat, cExpenseItem expenseitem, bool split, DateTime date, Employee employee, HttpRequest request, IActionContext actionContext)
    {

        FlagManagement flagManagement = actionContext.FlagManagement;
        cEmployees employees = actionContext.Employees;
        Dictionary<int, cRoleSubcat> rolesubcats = employees.getResultantRoleSet(employee);
        actionContext.EmployeeId = this.employeeid;
        dependsOnMileageGrid.Clear();
        var showingMileageGrid = false;
        var showingJourneyGridBasedOnDocChecks = false;
        Employee claimemp = null;
        cCar car = null;
        cUserdefinedFields clsuserdefined = actionContext.UserDefinedFields;
        cMisc clsmisc = actionContext.Misc;
        cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(accountid);
        Panel pnl = new Panel();
        Table tblmain = new Table();
        Table tbl = new Table();
        CollapsiblePanelExtender collapse;
        FilteredTextBoxExtender filt;
        TableRow row;
        TableCell cell;
        TextBox txtbox;
        bool showreturntostart = false;
        HyperLink hlnk;
        CustomValidator custval;
        RegularExpressionValidator regexval;
        decimal unallocatedamount = 0;
        //CustomValidator custval;
        RadioButton rd;
        CompareValidator compval;
        //CompareValidator compval;
        MaskedEditExtender maskededit;
        RequiredFieldValidator reqval;
        Literal lit;
        AutoCompleteExtender autocomp;
        Image img;
        CalendarExtender cal;
        CheckBox chkbox;
        DropDownList ddlst;
        HiddenField hdn;
        cEmployeeCorporateCards clsEmployeeCards = actionContext.EmployeeCorporateCards;
        var DutyOfCareDocuments = new DutyOfCareDocuments();
        List<DocumentExpiryResult> documentExpiryResults = null;
        List<ListItem> class1BusinessResults = null;
        bool isManualDocumentValid = false;

        if (expenseitem != null && expenseitem.transactionid > 0)
        {
            nTransactionid = expenseitem.transactionid;
        }

        bool creditcard = false;
        bool purchasecard = false;
        bool claimSubmitted = false;
        bool carIsSorn = false;
        cClaim reqclaim = null;

        if (expenseitem != null)
        {
            cClaims clsclaims = actionContext.Claims;
            reqclaim = clsclaims.getClaimById(expenseitem.claimid);
            creditcard = clsEmployeeCards.HasCreditCard(reqclaim.employeeid);
            purchasecard = clsEmployeeCards.HasPurchaseCard(reqclaim.employeeid);
            nSelectedCountry = expenseitem.countryid;
            claimSubmitted = reqclaim.submitted;

            // Check if the car is SORN on edit expense item, this check is considered only if the claim is unsubmitted and the general option for tax expiry is on
            if (!claimSubmitted && this.accountProperties.BlockTaxExpiry)
            {
                carIsSorn = this.clsEmployeeCars.SornCars.Any(item => item.Text == expenseitem.carid.ToString());
            }
        }
        else
        {
            creditcard = clsEmployeeCards.HasCreditCard(employeeid);
            purchasecard = clsEmployeeCards.HasPurchaseCard(employeeid);
        }
        pnl.ID = "pnl" + id;
        tbl.ID = "tbl" + id;

        //store the subcatid
        txtbox = new TextBox();
        txtbox.ID = "txtsubcatid" + id;

        txtbox.Text = subcat.subcatid.ToString();
        txtbox.Style.Add("display", "none");
        pnl.Controls.Add(txtbox);

        txtbox = new TextBox();
        txtbox.ID = "txtexpenseid" + id;

        if (expenseitem != null)
        {
            txtbox.Text = expenseitem.expenseid.ToString();

        }
        txtbox.Style.Add("display", "none");
        pnl.Controls.Add(txtbox);

        //get the role that allows this item
        int itemRoleID = 0;
        cRoleSubcat rolesubcat;
        if (rolesubcats.TryGetValue(subcat.subcatid, out rolesubcat))
        {
            itemRoleID = rolesubcat.roleid;
        }


        #region ESR Assignment Number

        cESRAssignments assignments = actionContext.EsrAssignments;
        if (assignments.ActiveAssignmentCount() > 0)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.CssClass = "labeltd";
            cell.Text = "ESR Assignment Numbers:";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "inputtd esrAssignment";
            ddlst = new DropDownList();
            ddlst.ID = "cmbESRAss" + id;
            ddlst.ClientIDMode = ClientIDMode.Static;
            ddlst.Items.AddRange(assignments.GetAvailableAssignmentListItems(false, date));

            cell.Controls.Add(ddlst);
            row.Cells.Add(cell);
            if (assignments.ActiveAssignmentCount() == 1)
            {
                row.Style.Add("display", "none");
            }
            tbl.Rows.Add(row);

            this.SetESRItemAsSelected(ddlst, expenseitem);
        }

        #endregion

        #region comment

        bool hasSornVehicles = this.accountProperties.BlockTaxExpiry && this.clsEmployeeCars.SornCars != null && this.clsEmployeeCars.SornCars.Count > 0;
        
        if (subcat.comment != "" || !subcat.reimbursable || (subcat.mileageapp && hasSornVehicles && !claimSubmitted) || subcat.calculation == CalculationType.ExcessMileage)
        {
            pnl.Controls.Add(CreateMileageCommentControl(subcat, expenseitem, carIsSorn));
        }

        #endregion

        #region add vehicle option

        if (subcat.mileageapp || subcat.calculation == CalculationType.ExcessMileage)
        {
            var activeCars = this.clsEmployeeCars.GetActiveCars(this.accountProperties.UseDateOfExpenseForDutyOfCareChecks ? date : DateTime.Now, false);
            var dutyOfCareExpiredDocuments = DutyOfCareDocuments.PassesDutyOfCare(accountid, activeCars, employeeid, this.accountProperties.UseDateOfExpenseForDutyOfCareChecks ? date : DateTime.Now, this.currentUser.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect)).FirstOrDefault();
            documentExpiryResults = dutyOfCareExpiredDocuments.Key;
            isManualDocumentValid = dutyOfCareExpiredDocuments.Value;

            class1BusinessResults = DutyOfCareDocuments.Class1BusinessInformation(accountid, activeCars, this.accountProperties.UseDateOfExpenseForDutyOfCareChecks ? date : DateTime.Now);

            pnl.Controls.Add(CreateCarAndJourneyRateControl(subcat, expenseitem, id, claimemp, date, carIsSorn, documentExpiryResults, class1BusinessResults, isManualDocumentValid));
            showingJourneyGridBasedOnDocChecks = DetermineEnableJourneyGridForExpenseDateDocChecks(claimSubmitted, date, subcat.EnableDoC, carIsSorn, subcat.RequireClass1BusinessInsurance, documentExpiryResults, class1BusinessResults);

            if (showingJourneyGridBasedOnDocChecks)

            {
                var helplinkouter = new Panel() { CssClass = "helplinkouter" };
                helplinkouter.Controls.Add(new Label { CssClass = "helplinkicon" });
                helplinkouter.Controls.Add(new Label { CssClass = "showhelplink", Text = "Not sure how to add a journey step?" });
                dependsOnMileageGrid.Add(helplinkouter);
                pnl.Controls.Add(helplinkouter);
            }
        }

        #endregion

        #region Organisation

        cFieldToDisplay organisationField = clsmisc.GetGeneralFieldByCode("organisation");
        if (organisationField.individual && subcat.companyapp && ((itemtype == ItemType.Cash && organisationField.display) || (itemtype == ItemType.CreditCard && organisationField.displaycc) || (itemtype == ItemType.PurchaseCard && organisationField.displaypc)))
        {
            row = new TableRow();
            cell = new TableCell { CssClass = "labeltd", Text = organisationField.description + ":" };
            row.Cells.Add(cell);

            cell = new TableCell { CssClass = "inputtd" };

            TextBox txtboxid = new TextBox { ID = id + "_txtOrganisation_ID" };
            txtboxid.Style.Add(HtmlTextWriterStyle.Display, "none");

            txtbox = new TextBox { ID = id + "_txtOrganisation", CssClass = "organisation-autocomplete" };
            txtbox.Attributes.Add("data-search", "Item" + id);

            if (expenseitem != null)
            {
                if (expenseitem.companyid > 0)
                {
                    Organisation organisation = Organisation.Get(accountid, expenseitem.companyid);

                    if (organisation != null)
                    {
                        txtboxid.Text = expenseitem.companyid.ToString(CultureInfo.InvariantCulture);
                        txtbox.Text = organisation.Name;
                    }
                }
            }

            cell.Controls.Add(txtbox);
            cell.Controls.Add(txtboxid);
            cell.Controls.Add(new Literal { Text = " " });

            // search icon
            img = new Image
            {
                ImageUrl = GlobalVariables.StaticContentLibrary + "/icons/16/plain/find.png",
                ID = id + "_txtOrganisationSearchIcon",
                AlternateText = "Search " + organisationField.description,
                CssClass = "btn"
            };

            img.Attributes.Add("onclick", "OrganisationSearch = OrganisationSearches[\"Item" + id + "\"];OrganisationSearches[\"Item" + id + "\"].Search();");

            cell.Controls.Add(img);
            row.Cells.Add(cell);

            cell = new TableCell
            {
                ColumnSpan = 2
            };

            if ((itemtype == ItemType.Cash && organisationField.mandatory) || (itemtype == ItemType.CreditCard && organisationField.mandatorycc) || (itemtype == ItemType.PurchaseCard && organisationField.mandatorypc))
            {
                custval = new CustomValidator
                {
                    ClientValidationFunction = "SEL.Expenses.Validate.Organisation.ItemLevelMandatory",
                    ControlToValidate = id + "_txtOrganisation",
                    ID = "custorganisation" + id,
                    ValidationGroup = "vgAeExpenses",
                    ValidateEmptyText = true,
                    ErrorMessage = "Please enter a valid " + organisationField.description + ".",
                    Text = "*"
                };

                cell.Controls.Add(custval);
            }
            else
            {
                custval = new CustomValidator
                {
                    ClientValidationFunction = "SEL.Expenses.Validate.Organisation.ItemLevelNotMandatory",
                    ControlToValidate = id + "_txtOrganisation",
                    ID = "custorganisation" + id,
                    ValidationGroup = "vgAeExpenses",
                    ValidateEmptyText = true,
                    ErrorMessage = organisationField.description + " is not valid. Please enter a value in the box provided.",
                    Text = "*"
                };

                cell.Controls.Add(custval);
            }

            row.Cells.Add(cell);
            tbl.Rows.Add(row);
        }

        #endregion

        #region from and to

        cFieldToDisplay from = clsmisc.GetGeneralFieldByCode("from");
        cFieldToDisplay to = clsmisc.GetGeneralFieldByCode("to");

        Lazy<int[]> homeAddressIds = new Lazy<int[]>(() => employee.GetHomeAddresses().HomeLocations.Select(l => l.LocationID).ToArray());
        if (from.individual && ((subcat.mileageapp && !clsproperties.allowmultipledestinations) || !subcat.mileageapp) && subcat.fromapp == true && ((itemtype == ItemType.Cash && from.display) || (itemtype == ItemType.CreditCard && from.displaycc) || (itemtype == ItemType.PurchaseCard && from.displaypc)))
        {

            row = new TableRow();

            cell = new TableCell();
            cell.CssClass = "labeltd";
            cell.Text = from.description + ":";
            row.Cells.Add(cell);


            cell = new TableCell();
            cell.CssClass = "inputtd";

            hdn = new HiddenField();
            hdn.ID = "txtfromid" + id;

            if (expenseitem != null)
            {
                if (expenseitem.fromid > 0)
                {
                    hdn.Value = expenseitem.fromid.ToString();
                }
            }
            cell.Controls.Add(hdn);
            txtbox = new TextBox { ID = "txtfrom" + id, CssClass = "ui-sel-address-picker" };
            txtbox.Attributes.Add("rel", hdn.ClientID);

            if (expenseitem != null)
            {
                if (expenseitem.fromid > 0)
                {
                    Address address = Address.Get(accountid, expenseitem.fromid);
                    string fromText;
                    if (homeAddressIds.Value.Contains(address.Identifier))
                    {
                        fromText = accountProperties.HomeAddressKeyword;
                    }
                    else
                    {
                        fromText = address.FriendlyName;
                    }
                    txtbox.Text = fromText;
                }
            }
            cell.Controls.Add(txtbox);

            img = new Image();
            img.ID = "imgFromAddressSearch" + id;
            img.ImageUrl = "~/shared/images/icons/16/plain/find.png";
            cell.Controls.Add(img);
            row.Cells.Add(cell);

            addressDetailsPopup.AddEvents(img, hdn, "ctl00_contentmain_", false);
            row.Cells.Add(cell);

            cell = new TableCell { ColumnSpan = 2 };
            if ((itemtype == ItemType.Cash && from.mandatory) || (itemtype == ItemType.CreditCard && from.mandatorycc) || (itemtype == ItemType.PurchaseCard && from.mandatorypc))
            {

                custval = new CustomValidator();
                custval.ControlToValidate = "txtfrom" + id;
                custval.ID = "custfrom" + id;
                custval.ValidateEmptyText = true;
                custval.ErrorMessage = from.description + " is a mandatory field. Please enter a value in the box provided";
                custval.Text = "*";
                custval.ValidationGroup = "vgAeExpenses";
                custval.ClientValidationFunction = "checkFrom";
                cell.Controls.Add(custval);
            }
            else
            {
                cell.Text = "&nbsp;";
            }

            row.Cells.Add(cell);
            tbl.Rows.Add(row);
        }

        if (to.individual && ((subcat.mileageapp && !clsproperties.allowmultipledestinations) || !subcat.mileageapp) && subcat.toapp == true && ((itemtype == ItemType.Cash && to.display) || (itemtype == ItemType.CreditCard && to.displaycc) || (itemtype == ItemType.PurchaseCard && to.displaypc)))
        {
            row = new TableRow();
            cell = new TableCell();
            cell.CssClass = "labeltd";
            cell.Text = to.description + ":";
            row.Cells.Add(cell);


            cell = new TableCell();
            cell.CssClass = "inputtd";

            hdn = new HiddenField();
            hdn.ID = "txttoid" + id;

            if (expenseitem != null)
            {
                if (expenseitem.toid > 0)
                {
                    hdn.Value = expenseitem.toid.ToString();
                }
            }
            cell.Controls.Add(hdn);
            txtbox = new TextBox { ID = "txtto" + id, CssClass = "ui-sel-address-picker" };
            txtbox.Attributes.Add("rel", hdn.ClientID);

            if (expenseitem != null)
            {
                if (expenseitem.toid > 0)
                {
                    Address address = Address.Get(accountid, expenseitem.toid);
                    string toText;
                    if (homeAddressIds.Value.Contains(address.Identifier))
                    {
                        toText = accountProperties.HomeAddressKeyword;
                    }
                    else
                    {
                        toText = address.FriendlyName;
                    }
                    txtbox.Text = toText;
                }
            }
            cell.Controls.Add(txtbox);

            img = new Image();
            img.ID = "imgToAddressSearch" + id;
            img.ImageUrl = "~/shared/images/icons/16/plain/find.png";
            cell.Controls.Add(img);
            addressDetailsPopup.AddEvents(img, hdn, "ctl00_contentmain_", false);
            row.Cells.Add(cell);

            cell = new TableCell { ColumnSpan = 2 };
            if ((itemtype == ItemType.Cash && to.mandatory) || (itemtype == ItemType.CreditCard && to.mandatorycc) || (itemtype == ItemType.PurchaseCard && to.mandatorypc))
            {

                custval = new CustomValidator();
                custval.ControlToValidate = "txtto" + id;
                custval.ID = "custto" + id;
                custval.ValidateEmptyText = true;
                custval.ValidationGroup = "vgAeExpenses";
                custval.ErrorMessage = to.description + " is a mandatory field. Please enter a value in the box provided";
                custval.Text = "*";
                custval.ClientValidationFunction = "checkTo";
                cell.Controls.Add(custval);

                if (!clsproperties.addlocations) //ensure they have picked from the list
                {
                    compval = new CompareValidator();
                    compval.ControlToValidate = "txttoid" + id;
                    compval.ErrorMessage = to.description + " is a mandatory field. The value you have entered is not valid";
                    compval.Text = "*";
                    compval.ValidationGroup = "vgAeExpenses";
                    compval.Operator = ValidationCompareOperator.GreaterThan;
                    compval.ValueToCompare = "0";
                    cell.Controls.Add(compval);
                }
            }
            else
            {
                cell.Text = "&nbsp;";
            }

            row.Cells.Add(cell);

            tbl.Rows.Add(row);
        }

        #endregion

        #region journey grid

        bool showjourneygrid = false;
        bool hideMap = false;
        if (subcat.mileageapp == true && (clsproperties.allowmultipledestinations && subcat.fromapp && from.individual && subcat.toapp && to.individual && ((itemtype == ItemType.Cash && from.display && to.display) || (itemtype == ItemType.CreditCard && from.displaycc && to.displaycc) || (itemtype == ItemType.PurchaseCard && from.displaypc && to.displaypc))))
        {
            if (showingJourneyGridBasedOnDocChecks)

            {
                tbl.CssClass = "wideExpenseItem";
                pnl.CssClass = "mileagePanel";
                showreturntostart = true;
                showjourneygrid = true;
                UpdatePanel upnljourney = new UpdatePanel();
                upnljourney.ID = "upnljourney" + subcat.subcatid;
                car = clsEmployeeCars.GetCarByID(clsEmployeeCars.GetDefaultCarID(clsproperties.blocktaxexpiry, clsproperties.blockmotexpiry, clsproperties.blockinsuranceexpiry, clsproperties.BlockBreakdownCoverExpiry, accountProperties.DisableCarOutsideOfStartEndDate, date));

                if (documentExpiryResults == null)
                {
                    var dutyOfCareExpiredDocuments = DutyOfCareDocuments.PassesDutyOfCare(accountid, this.clsEmployeeCars.GetActiveCars(includePoolCars: false), employeeid, this.accountProperties.UseDateOfExpenseForDutyOfCareChecks ? date : DateTime.Now, this.currentUser.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect)).FirstOrDefault();
                    documentExpiryResults = dutyOfCareExpiredDocuments.Key;
                    isManualDocumentValid = dutyOfCareExpiredDocuments.Value;
                }

                if (car == null && subcat.EnableDoC && claimSubmitted && documentExpiryResults.Count > 0)
                {
                    car = clsEmployeeCars.GetCarByID(clsEmployeeCars.GetDefaultCarID(false, false, false, false, accountProperties.DisableCarOutsideOfStartEndDate, date));
                }
                row = new TableRow();
                cell = new TableCell();
                cell.ColumnSpan = 4;

                cJourneyStep[] cJourneySteps = new cJourneyStep[] { };
                if (expenseitem != null && expenseitem.journeysteps != null)
                {
                    cJourneySteps = expenseitem.journeysteps.Values.ToArray();
                }

                if (this.mobileJourney != null)
                {
                    cJourneySteps = MobileJourneyParser.GetAddressSuggestionsForMobileJourney(this.mobileJourney).ToArray();
                }
                else if (this.mobileItem != null && !string.IsNullOrEmpty(this.mobileItem.JourneySteps))
                {
                    cJourneySteps = MobileJourneyParser.GetAddressSuggestionsForMobileExpenseItem(this.mobileItem).ToArray();
                }
                else
                {
                    // this is placed in an else block because the MobileJourneyParser does this step for us
                    foreach (var cJourneyStep in cJourneySteps)
                    {
                        cJourneyStep.recmiles = AddressDistance.GetRecommendedOrCustomDistance(cJourneyStep.startlocation, cJourneyStep.endlocation, this.accountid, subAccount, cMisc.GetCurrentUser()) ?? 0m;
                    }
                }

                MileageUOM uom = MileageUOM.Mile;
                if (car != null) uom = car.defaultuom;
                int[] homeAddressIdsToReplace = new int[] { };
                if (!subAccount.SubAccountProperties.ShowFullHomeAddressOnClaims || currentUser.isDelegate)
                {
                    homeAddressIdsToReplace = homeAddressIds.Value;
                }

                // Audit if home address if viewed by approver
                if (homeAddressIds.Value != null && subAccount.SubAccountProperties.ShowFullHomeAddressOnClaims && currentUser.EmployeeID != employeeid && !currentUser.isDelegate)
                {
                    cAuditLog auditLog = new cAuditLog();
                    auditLog.ViewRecord(SpendManagementElement.Claims, "Home Address of " + employee.Username + " viewed by employee.");
                }


                var officeAddress = Address.GetByReservedKeyword(currentUser, "office", date, accountProperties, (expenseitem == null ? null : (int?)expenseitem.ESRAssignmentId));
                var officeAddressId = officeAddress?.Identifier ?? 0;
                if (expenseitem != null)
                {
                    officeAddressId = expenseitem.WorkAddressId == 0
                        ? officeAddressId
                        : expenseitem.WorkAddressId;
                }

                cell.Controls.Add(mileageGridBuilder.BuildMileageGrid(cJourneySteps, subcat, request, this.accountid, expenseitem == null ? 0 : expenseitem.expenseid, employeeid, uom, homeAddressIdsToReplace, accountProperties.HomeAddressKeyword, (officeAddressId != 0 ? officeAddressId.ToString() : String.Empty)));
                //shouldn't matter if expenseid is passed as zero in the above, only used to reflect the data the user entered in case there's been an error.

                cell.Controls.Add(CreateReturnToStartControl());
                showingMileageGrid = true;

                cell.Controls.Add(upnljourney);
                row.Cells.Add(cell);
                tbl.Rows.Add(row);
            }
        }

        #endregion

        #region mileage

        if ((subcat.mileageapp && !showjourneygrid && showingJourneyGridBasedOnDocChecks) || subcat.calculation == CalculationType.ExcessMileage)
        {
            if (documentExpiryResults == null)
            {
                var dutyOfCareExpiredDocuments = DutyOfCareDocuments.PassesDutyOfCare(accountid, this.clsEmployeeCars.GetActiveCars(includePoolCars: false), employeeid, this.accountProperties.UseDateOfExpenseForDutyOfCareChecks ? date : DateTime.Now, this.currentUser.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect)).FirstOrDefault();
                documentExpiryResults = dutyOfCareExpiredDocuments.Key;
                isManualDocumentValid = dutyOfCareExpiredDocuments.Value;
            }

            if (class1BusinessResults == null)
            {
                class1BusinessResults = DutyOfCareDocuments.Class1BusinessInformation(accountid, this.clsEmployeeCars.GetActiveCars(includePoolCars: false), this.accountProperties.UseDateOfExpenseForDutyOfCareChecks ? date : DateTime.Now);
            }

            pnl.CssClass = "mileagePanel";
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Mileage:";
            cell.CssClass = "labeltd singlejourneystepmileage";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.ID = "txtmileage" + id;
            txtbox.MaxLength = 5;
            txtbox.Enabled = !subcat.IsRelocationMileage;

            if (expenseitem != null && expenseitem.carid > 0)
            {
                car = clsEmployeeCars.GetCarByID(expenseitem.carid);
            }

            if (car == null && subcat.EnableDoC && !claimSubmitted)
            {
                car = clsEmployeeCars.GetCarByID(clsEmployeeCars.GetDefaultCarID(clsproperties.blocktaxexpiry, clsproperties.blockmotexpiry, clsproperties.blockinsuranceexpiry, clsproperties.BlockBreakdownCoverExpiry, accountProperties.DisableCarOutsideOfStartEndDate, date));

            }
            if (car == null)
            {
                car = clsEmployeeCars.GetDefaultCar(false, false, false, false, accountProperties.DisableCarOutsideOfStartEndDate, documentExpiryResults, date);
            }

            if (car == null)
            {
                txtbox.ToolTip = "Mileage cannot be claimed as there are no vehicles allocated to your account. Please consult your administrator.";
                txtbox.Enabled = false;

                cell = new TableCell();
                cell.CssClass = "inputtd";

                row.Cells.Add(cell);
            }
            else
            {
                if (car.VehicleEngineTypeId == 0 || car.mileagecats.Count == 0 || (!accountProperties.DisableCarOutsideOfStartEndDate && car.active == false) || (accountProperties.DisableCarOutsideOfStartEndDate && !car.CarActiveOnDate(date)))
                {

                    txtbox.ToolTip = "Mileage cannot be claimed as all of the required details of your vehicle have not been recorded. Please contact your administrator.";
                    txtbox.Enabled = false;
                }
            }

            if (subcat.IsRelocationMileage) //work out mileage that can be claimed
            {
                txtbox.Text = ExcessMileage.GetRelocationMileage(employee, car, date, currentUser).ToString("#0.00", CultureInfo.InvariantCulture);
            }
            else
            {
                if (expenseitem != null)
                {
                    if (expenseitem.miles > 0)
                    {
                        txtbox.Text = ((car != null && car.defaultuom == MileageUOM.KM) ? new cMileagecats(accountid).convertMilesToKM(expenseitem.miles) : expenseitem.miles).ToString("#0.00", CultureInfo.InvariantCulture);
                    }
                }
            }

            if (expenseitem != null)
            {
                nSelectedCountry = expenseitem.countryid;
            }
            cell = new TableCell();
            cell.CssClass = "inputtd";

            if (this.accountProperties.BlockDrivingLicence && subcat.EnableDoC && this.accountProperties.EnableAutomaticDrivingLicenceLookup && this.currentUser.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect))
            {
                // If we don't have a consent response on record, or we do and they've said yes
                if (this.employee.AgreeToProvideConsent.HasValue == false || (this.employee.AgreeToProvideConsent.HasValue && this.employee.AgreeToProvideConsent.Value == true))
                {
                    txtbox.Enabled = true;
                    txtbox.ToolTip = this.GenerateDVLAConsentMessage(this.currentUser.isDelegate, isManualDocumentValid);
                }
            }
            else if (this.clsEmployeeCars.GetActiveCars(date, includePoolCars: false).Count == 1)
            {
                if (car != null)
                {
                    if (clsproperties.blockinsuranceexpiry || clsproperties.blockmotexpiry || clsproperties.blocktaxexpiry || clsproperties.blockdrivinglicence || clsproperties.BlockBreakdownCoverExpiry)
                    {
                        if ((CarTypes.VehicleType)car.VehicleTypeID != CarTypes.VehicleType.Bicycle)
                        {
                            if (this.accountProperties.UseDateOfExpenseForDutyOfCareChecks)
                            {
                                documentExpiryResults = documentExpiryResults.Where(i => i.carId == car.carid).ToList();
                                class1BusinessResults = class1BusinessResults.Where(i => i.Text == car.carid.ToString()).ToList();
                            }
                            if (documentExpiryResults.Count > 0 && subcat.EnableDoC && !claimSubmitted)
                            {
                                txtbox.Enabled = true;
                                txtbox.ToolTip = "Mileage cannot be claimed because - <br/> ";
                                foreach (DocumentExpiryResult result in documentExpiryResults)
                                {
                                    if (result.carId == 0 && !result.IsValidLicence)
                                    {
                                        txtbox.ToolTip = InvalidLicenceErrorMessage;
                                    }
                                    else
                                    {
                                        txtbox.ToolTip += string.Format("\u2022 {0} <br/>", result.Reason);
                                    }
                                }
                            }

                            if (documentExpiryResults.Count > 0 && class1BusinessResults.Count > 0 && subcat.EnableDoC && subcat.RequireClass1BusinessInsurance && !claimSubmitted)
                            {
                                foreach (ListItem result in class1BusinessResults)
                                {
                                    if (result != null)
                                    {
                                        txtbox.ToolTip += string.Format("\u2022 • Your vehicle with registration {0} does not have a class 1 business insurance. <br/>", result.Value);
                                    }
                                }
                            }
                            else if (documentExpiryResults.Count <= 0 && class1BusinessResults.Count > 0 && subcat.EnableDoC && subcat.RequireClass1BusinessInsurance && !claimSubmitted)
                            {
                                txtbox.Enabled = true;
                                txtbox.ToolTip = "Mileage cannot be claimed because - <br/> ";
                                foreach (ListItem result in class1BusinessResults)
                                {
                                    if (result != null)
                                    {
                                        txtbox.ToolTip += string.Format("\u2022 • Your vehicle with registration {0} does not have a class 1 business insurance. <br/>", result.Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            txtbox.Attributes["data-field"] = "recommendeddistance"; //enables jQuery to calculate mileage and put result in here
            cell.Controls.Add(txtbox);

            HtmlInputHidden input = new HtmlInputHidden { Value = (car != null ? car.defaultuom : MileageUOM.Mile).ToString().ToLower(), Name = "mileagegridtable_uom" };
            input.Attributes.Add("data-field", "uom");
            cell.Controls.Add(input);

            cell.Controls.Add(new Label { CssClass = "loading" });

            row.Cells.Add(cell);
            cell = new TableCell();
            compval = new CompareValidator(); // new CompareValidator();
            compval.ControlToValidate = "txtmileage" + id;
            compval.Type = ValidationDataType.Double;
            compval.ValidationGroup = "vgAeExpenses";
            compval.Operator = ValidationCompareOperator.DataTypeCheck;

            compval.Text = "*";
            compval.ErrorMessage = getValidationMsg("Mileage", FieldType.Number);
            cell.Controls.Add(compval);
            Flag mileageFlag = flagManagement.GetFlagByTypeRoleAndExpenseItem(FlagType.MileageExceeded, itemRoleID, subcat.subcatid);
            if (mileageFlag != null && mileageFlag.Action == FlagAction.BlockItem && subcat.calculation == CalculationType.PencePerMile)
            {
                custval = new CustomValidator(); // new CustomValidator();
                custval.ControlToValidate = "txtmileage" + id;
                custval.ClientValidationFunction = "checkUsualMileage";
                custval.ID = "custmileage" + id;
                custval.Text = "*";
                custval.ValidationGroup = "vgAeExpenses";
                custval.ErrorMessage = "The number of miles you have claimed is over the limit set by your policy";
                cell.Controls.Add(custval);
            }
            row.Cells.Add(cell);

            cell = new TableCell();
            if (true) //clshelp.containsTip(247))
            {
                lit = new Literal();
                lit.Text = "<img id=\"imgtooltip247" + id + "\" onclick=\"SEL.Tooltip.Show('e6ac9e15-9849-457c-b096-2518b0251632', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
                cell.Controls.Add(lit);
            }

            row.Cells.Add(cell);
            if (subcat.calculation != CalculationType.ExcessMileage)
            {
                tbl.Rows.Add(row);
            }
            

            showreturntostart = true;

        }

        #endregion

        pnl.Controls.Add(new HiddenField { ID = "claimempid" + id, Value = employee.EmployeeID.ToString() });

        #region Route and Map

        var user = actionContext.CurrentUser;
        if (user.Account.MapsEnabled && showjourneygrid && !hideMap && !user.isDelegate)
        {
            this.AddRouteAndMap(ref tbl, id);
        }

        #endregion Route and Map

        if (subcat.EnableHomeToLocationMileage && showjourneygrid)
        {
            this.AddHomeToOfficeComment(ref tbl, id);
        }

        #region reason

        cFieldToDisplay reason = clsmisc.GetGeneralFieldByCode("reason");

        if (reason.individual && subcat.reasonapp && ((itemtype == ItemType.Cash && reason.display) || (itemtype == ItemType.CreditCard && reason.displaycc) || (itemtype == ItemType.PurchaseCard && reason.displaypc)))
        {
            cFilterRules clsfilterrules = actionContext.FilterRules;
            row = new TableRow();

            cell = new TableCell();
            cell.CssClass = "labeltd";
            cell.Text = reason.description + ":";
            row.Cells.Add(cell);
            cReasons clsreasons = actionContext.ClaimReasons;
            cell = new TableCell();
            cell.CssClass = "inputtd";
            ddlst = new DropDownList();
            ddlst.Items.AddRange(clsreasons.CreateDropDown().ToArray());
            ddlst.ID = "cmbreason" + id;

            if (expenseitem != null)
            {
                if (ddlst.Items.FindByValue(expenseitem.reasonid.ToString()) != null)
                {
                    ddlst.Items.FindByValue(expenseitem.reasonid.ToString()).Selected = true;
                }
            }
            else if (mobileItem != null && mobileItem.ReasonID.HasValue)
            {
                if (ddlst.Items.FindByValue(mobileItem.ReasonID.Value.ToString()) != null)
                {
                    ddlst.Items.FindByValue(mobileItem.ReasonID.Value.ToString()).Selected = true;
                }
            }

            var filterAttribute = clsfilterrules.FilterDropdown(FilterType.Reason, id.ToString(), ddlst.ID);

            if (!filterAttribute.IsNullOrEmpty())
            {
                ddlst.Attributes.Add(filterAttribute[0], filterAttribute[1]);
            }

            cell.Controls.Add(ddlst);
            row.Cells.Add(cell);

            cell = new TableCell();
            if ((itemtype == ItemType.Cash && reason.mandatory) || (itemtype == ItemType.CreditCard && reason.mandatorycc) || (itemtype == ItemType.PurchaseCard && reason.mandatorypc))
            {
                custval = new CustomValidator();
                custval.ID = "compreasonval" + id;
                custval.ControlToValidate = "cmbreason" + id;
                custval.ErrorMessage = reason.description + " is a mandatory field. Please select a value";
                custval.Text = "*";
                custval.ValidationGroup = "vgAeExpenses";
                custval.ValidateEmptyText = true;
                custval.ClientValidationFunction = "checkReason";
                cell.Controls.Add(custval);

            }
            else
            {
                cell.Text = "&nbsp;";
            }

            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip240" + id + "\" onclick=\"SEL.Tooltip.Show('8dca38bb-9ef2-4de8-91e3-9402e4ace68b', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);


            row.Cells.Add(cell);
            tbl.Rows.Add(row);
        }

        #endregion


        #region hotel

        if (subcat.hotelapp)
        {
            Hotel hotel = null;
            if (expenseitem != null)
            {
                hotel = Hotel.Get(expenseitem.hotelid);
            }
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Hotel:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "inputtd";
            txtbox = new TextBox();
            txtbox.ID = "txthotel" + id;
            if (hotel != null)
            {
                txtbox.Text = hotel.hotelname;
            }
            txtbox.Attributes.Add("onblur", "getHotelDetails(" + id + ");");
            cell.Controls.Add(txtbox);
            autocomp = new AutoCompleteExtender();
            autocomp.ServicePath = "~/shared/webServices/svcAutocomplete.asmx";
            autocomp.ServiceMethod = "GetHotelList";
            autocomp.TargetControlID = "txthotel" + id;
            autocomp.CompletionSetCount = 10;
            autocomp.MinimumPrefixLength = 1;
            autocomp.EnableCaching = true;

            cell.Controls.Add(autocomp);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = "&nbsp;";
            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip256" + id + "\" onclick=\"SEL.Tooltip.Show('3d593f9c-2384-4272-aa88-01a68c13683a', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);


            row.Cells.Add(cell);

            tbl.Rows.Add(row);
            row = new TableRow();
            row.Cells.Add(new TableCell());
            cell = new TableCell();
            cell.CssClass = "inputtd";
            hlnk = new HyperLink();

            hlnk.ID = "lnkhotel" + id;
            hlnk.Text = "Show Hotel Details";
            hlnk.Style.Add("text-decoration", "underline");
            hlnk.Style.Add("color", "blue");
            hlnk.Style.Add("cursor", "hand");
            hlnk.Attributes.Add("onclick", "if (document.getElementById(contentID + 'lnkhotel" + id + "').innerHTML == 'Show Hotel Details') {document.getElementById(contentID + 'lnkhotel" + id + "').innerHTML = 'Hide Hotel Details';} else {document.getElementById(contentID + 'lnkhotel" + id + "').innerHTML = 'Show Hotel Details';}");
            cell.Controls.Add(hlnk);
            row.Cells.Add(cell);
            tbl.Rows.Add(row);


            row = new TableRow();
            row.Cells.Add(new TableCell());
            cell = new TableCell();
            cell.Controls.Add(getHotelPanel(id, hotel));

            collapse = new CollapsiblePanelExtender();
            collapse.ID = "collhotel" + id;
            collapse.ExpandDirection = CollapsiblePanelExpandDirection.Vertical;

            collapse.TargetControlID = "pnlhotel" + id;
            collapse.ExpandControlID = "lnkhotel" + id;
            collapse.CollapseControlID = "lnkhotel" + id;
            collapse.Collapsed = true;
            collapse.ExpandedText = "fred";

            cell.Controls.Add(collapse);
            row.Cells.Add(cell);



            tbl.Rows.Add(row);
        }

        #endregion

        #region Number of passengers, heavy bulky equipment

        if (showingJourneyGridBasedOnDocChecks)
        {
            if (!showjourneygrid)
            {
                var numPassengersRow = new TableRow();
                numPassengersRow.Cells.Add(CreateNumPassengersLabelCell());
                numPassengersRow.Cells.Add(CreateNumPassengersValueCell(id, expenseitem));
                numPassengersRow.Cells.Add(CreateNumPassengersFilterCell(id));
                numPassengersRow.Cells.Add(CreateNumPassengersTooltipCell(id));
                tbl.Rows.Add(numPassengersRow);
                numPassengersRow.Visible = subcat.nopassengersapp;
                //we need to add these rows even if not visible so they can be picked up by the postback
                //for editing existing items. ASP.NET remembers even if no html element rendered

                if (subcat.allowHeavyBulkyMileage)
                {
                    var heavyBulkyRow = new TableRow();
                    heavyBulkyRow.Cells.Add(CreateHeavyBulkyLabelCell());
                    heavyBulkyRow.Cells.Add(CreateHeavyBulkyValueCell(id, expenseitem));
                    tbl.Rows.Add(heavyBulkyRow);
                }
            }
        }

        #endregion

        #region Number of Business Miles

        if (subcat.bmilesapp)
        {
            if (showingJourneyGridBasedOnDocChecks)
            {
                row = new TableRow();
                cell = new TableCell();
                cell.Text = "Number of Miles (Business):";
                cell.CssClass = "labeltd";
                row.Cells.Add(cell);
                txtbox = new TextBox();
                txtbox.ID = "txtbmiles" + id;
                txtbox.MaxLength = 4;
                if (expenseitem != null)
                {
                    if (expenseitem.bmiles > 0)
                    {
                        txtbox.Text = expenseitem.bmiles.ToString();
                    }
                }
                cell = new TableCell();
                cell.Controls.Add(txtbox);
                cell.CssClass = "inputtd";
                row.Cells.Add(cell);
                cell = new TableCell();
                compval = new CompareValidator(); // new CompareValidator();
                compval.ControlToValidate = "txtbmiles" + id;
                compval.Type = ValidationDataType.Double;
                compval.Operator = ValidationCompareOperator.DataTypeCheck;
                compval.ValidationGroup = "vgAeExpenses";
                compval.Text = "*";
                compval.ErrorMessage = getValidationMsg("Number of Miles (Business)", FieldType.Number);
                cell.Controls.Add(compval);
                cell = new TableCell();
                filt = new FilteredTextBoxExtender();
                filt.ID = "fltbmiles" + id;
                filt.TargetControlID = "txtbmiles" + id;
                filt.FilterType = FilterTypes.Numbers;
                cell.Controls.Add(filt);
                row.Cells.Add(cell);

                cell = new TableCell();

                if (true) //clshelp.containsTip(250))
                {
                    lit = new Literal();
                    lit.Text = "<img id=\"imgtooltip250" + id + "\" onclick=\"SEL.Tooltip.Show('2d81f378-47ef-4553-8fcf-e2cfffbab0d7', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
                    cell.Controls.Add(lit);
                }

                row.Cells.Add(cell);
                tbl.Rows.Add(row);
            }
        }

        #endregion

        #region Number of personal Miles

        if (subcat.pmilesapp)
        {
            if (showingJourneyGridBasedOnDocChecks)
            {
                row = new TableRow();
                cell = new TableCell();
                cell.Text = "Number of Miles (Personal):";
                cell.CssClass = "labeltd";
                row.Cells.Add(cell);
                txtbox = new TextBox();
                txtbox.ID = "txtpmiles" + id;
                txtbox.MaxLength = 4;
                if (expenseitem != null)
                {
                    if (expenseitem.pmiles > 0)
                    {
                        txtbox.Text = expenseitem.pmiles.ToString();
                    }
                }
                cell = new TableCell();
                cell.CssClass = "inputtd";
                cell.Controls.Add(txtbox);
                row.Cells.Add(cell);
                cell = new TableCell();
                compval = new CompareValidator(); // new CompareValidator();
                compval.ControlToValidate = "txtpmiles" + id;
                compval.Type = ValidationDataType.Double;
                compval.Operator = ValidationCompareOperator.DataTypeCheck;
                compval.ValidationGroup = "vgAeExpenses";
                compval.Text = "*";
                compval.ErrorMessage = getValidationMsg("Number of Miles (Personal)", FieldType.Number);
                cell.Controls.Add(compval);
                cell = new TableCell();
                filt = new FilteredTextBoxExtender();
                filt.ID = "fltpmiles" + id;
                filt.TargetControlID = "txtpmiles" + id;
                filt.FilterType = FilterTypes.Numbers;
                cell.Controls.Add(filt);
                row.Cells.Add(cell);

                cell = new TableCell();
                if (true) //clshelp.containsTip(251))
                {
                    lit = new Literal();
                    lit.Text = "<img id=\"imgtooltip251" + id + "\" onclick=\"SEL.Tooltip.Show('e6a974e6-b064-48c8-990d-f77f8bfbdb09','ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
                    cell.Controls.Add(lit);

                }
                //else
                //{
                //    cell.Text = "&nbsp;";
                //}

                row.Cells.Add(cell);
                tbl.Rows.Add(row);
            }
        }

        #endregion

        #region Number of directors

        if (subcat.nodirectorsapp)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Number of Directors:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.ID = "txtdirectors" + id;
            txtbox.MaxLength = 3;
            if (expenseitem != null)
            {
                if (expenseitem.directors > 0)
                {
                    txtbox.Text = expenseitem.directors.ToString();
                }
            }
            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            row.Cells.Add(cell);
            cell = new TableCell();
            compval = new CompareValidator();
            compval.ControlToValidate = "txtdirectors" + id;
            compval.ValueToCompare = "255";
            compval.Type = ValidationDataType.Integer;
            compval.Operator = ValidationCompareOperator.LessThanEqual;
            compval.ValidationGroup = "vgAeExpenses";
            compval.Text = "*";
            compval.ErrorMessage = "The maximum number of directors that can be entered is 255.";
            cell.Controls.Add(compval);
            filt = new FilteredTextBoxExtender();
            filt.ID = "fltdirectors" + id;
            filt.TargetControlID = "txtdirectors" + id;
            filt.FilterType = FilterTypes.Numbers;
            cell.Controls.Add(filt);

            if (subcat.calculation == CalculationType.Meal)
            {
                custval = new CustomValidator(); // new CustomValidator();
                custval.ClientValidationFunction = "checkEmployees";
                custval.ControlToValidate = "txtdirectors" + id;
                custval.ID = "custdirectors" + id;
                custval.ValidationGroup = "vgAeExpenses";
                custval.ValidateEmptyText = true;
                custval.ErrorMessage = "Please enter the number of employees that attended the meal.";
                custval.Text = "*";
                cell.Controls.Add(custval);

            }

            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip252" + id + "\" onclick=\"SEL.Tooltip.Show('0a327c60-0209-4098-9bc7-add9b452b8fd', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);


            row.Cells.Add(cell);
            tbl.Rows.Add(row);

        }

        #endregion

        #region Number of Employees

        if (subcat.staffapp)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Number of Employees:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.ID = "txtstaff" + id;
            txtbox.MaxLength = 3;
            if (expenseitem != null)
            {
                if (expenseitem.staff > 0)
                {
                    txtbox.Text = expenseitem.staff.ToString();
                }
            }

            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            row.Cells.Add(cell);

            cell = new TableCell();
            compval = new CompareValidator();
            compval.ControlToValidate = "txtstaff" + id;
            compval.ValueToCompare = "255";
            compval.Operator = ValidationCompareOperator.LessThanEqual;
            compval.Type = ValidationDataType.Integer;
            compval.ValidationGroup = "vgAeExpenses";
            compval.Text = "*";
            compval.ErrorMessage = "The maximum number of employees that can be entered is 255.";
            cell.Controls.Add(compval);
            filt = new FilteredTextBoxExtender();
            filt.ID = "fltstaff" + id;
            filt.TargetControlID = "txtstaff" + id;
            filt.FilterType = FilterTypes.Numbers;
            cell.Controls.Add(filt);

            if (subcat.calculation == CalculationType.Meal)
            {
                custval = new CustomValidator(); // new CustomValidator();
                custval.ClientValidationFunction = "checkEmployees";
                custval.ControlToValidate = "txtstaff" + id;
                custval.ID = "custstaff" + id;
                custval.ValidateEmptyText = true;
                custval.ValidationGroup = "vgAeExpenses";
                custval.ErrorMessage = "Please enter the number of employees that attended the meal.";
                custval.Text = "*";
                cell.Controls.Add(custval);

            }

            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip253" + id + "\" onclick=\"SEL.Tooltip.Show('d8222bde-1137-4614-a881-2e52c4433f4b', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);


            row.Cells.Add(cell);
            tbl.Rows.Add(row);
        }

        #endregion

        #region Number of others

        if (subcat.othersapp)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Number of Others:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.ID = "txtothers" + id;
            txtbox.MaxLength = 3;
            if (expenseitem != null)
            {
                if (expenseitem.othergrandtotal > 0)
                {
                    txtbox.Text = expenseitem.othergrandtotal.ToString();
                }
            }
            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            row.Cells.Add(cell);
            cell = new TableCell();
            compval = new CompareValidator();
            compval.ValidationGroup = "vgAeExpenses";
            compval.ControlToValidate = "txtothers" + id;
            compval.ValueToCompare = "255";
            compval.Type = ValidationDataType.Integer;
            compval.Operator = ValidationCompareOperator.LessThanEqual;
            compval.Text = "*";
            compval.ErrorMessage = "The maximum number of others that can be entered is 255.";
            cell.Controls.Add(compval);
            filt = new FilteredTextBoxExtender();
            filt.ID = "fltothers" + id;
            filt.TargetControlID = "txtothers" + id;
            filt.FilterType = FilterTypes.Numbers;
            cell.Controls.Add(filt);
            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip254" + id + "\" onclick=\"SEL.Tooltip.Show('ad3f3f93-301b-4be5-abe2-1cb72c8799e1', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);


            row.Cells.Add(cell);
            tbl.Rows.Add(row);

        }

        #endregion

        #region Number of Personal Guests

        if (subcat.nopersonalguestsapp)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Number of Spouses/Partners:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.MaxLength = 3;
            txtbox.ID = "txtpersonalguests" + id;
            if (expenseitem != null)
            {
                if (expenseitem.personalguestsgrandtotal > 0)
                {
                    txtbox.Text = expenseitem.personalguestsgrandtotal.ToString();
                }
            }
            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            row.Cells.Add(cell);
            cell = new TableCell();
            compval = new CompareValidator();
            compval.ControlToValidate = "txtpersonalguests" + id;
            compval.ValueToCompare = "255";
            compval.Type = ValidationDataType.Integer;
            compval.Operator = ValidationCompareOperator.LessThanEqual;
            compval.Text = "*";
            compval.ValidationGroup = "vgAeExpenses";
            compval.ErrorMessage = "The maximum number of personal guests that can be entered is 255.";
            cell.Controls.Add(compval);
            filt = new FilteredTextBoxExtender();
            filt.ID = "fltpersonalguests" + id;
            filt.TargetControlID = "txtpersonalguests" + id;
            filt.FilterType = FilterTypes.Numbers;
            cell.Controls.Add(filt);
            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip377" + id + "\" onclick=\"SEL.Tooltip.Show('33211bad-ff14-4b07-b04b-c385891a9a8b', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);

            row.Cells.Add(cell);
            tbl.Rows.Add(row);

        }

        #endregion

        #region Number of Remote Workers

        if (subcat.noremoteworkersapp)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Number of Remote Workers:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.MaxLength = 3;
            txtbox.ID = "txtremoteworkers" + id;
            if (expenseitem != null)
            {
                if (expenseitem.remoteworkersgrandtotal > 0)
                {
                    txtbox.Text = expenseitem.remoteworkersgrandtotal.ToString();
                }
            }
            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            row.Cells.Add(cell);
            cell = new TableCell();
            compval = new CompareValidator();
            compval.ControlToValidate = "txtremoteworkers" + id;
            compval.ValueToCompare = "255";
            compval.Type = ValidationDataType.Integer;
            compval.Operator = ValidationCompareOperator.LessThanEqual;
            compval.Text = "*";
            compval.ValidationGroup = "vgAeExpenses";
            compval.ErrorMessage = "The maximum number of remote workers that can be entered is 255.";
            cell.Controls.Add(compval);
            filt = new FilteredTextBoxExtender();
            filt.ID = "fltremoteworkers" + id;
            filt.TargetControlID = "txtremoteworkers" + id;
            filt.FilterType = FilterTypes.Numbers;
            cell.Controls.Add(filt);
            row.Cells.Add(cell);
            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip278" + id + "\" onclick=\"SEL.Tooltip.Show('eae38319-92af-440f-abbb-641a4e723f08', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);


            row.Cells.Add(cell);
            tbl.Rows.Add(row);

        }

        #endregion

        #region Attendees

        if (subcat.attendeesapp)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Attendees:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.ID = "txtattendees" + id;
            txtbox.TextMode = TextBoxMode.MultiLine;
            if (expenseitem != null)
            {
                txtbox.Text = expenseitem.attendees;
            }
            txtbox.Rows = 3;

            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            row.Cells.Add(cell);

            cell = new TableCell();
            if (subcat.attendeesmand)
            {

                custval = new CustomValidator(); // new CustomValidator();
                custval.ClientValidationFunction = "checkAttendees";
                custval.ControlToValidate = "txtattendees" + id;
                custval.ID = "custattendees" + id;
                custval.ValidationGroup = "vgAeExpenses";
                custval.ValidateEmptyText = true;
                custval.ErrorMessage = "Please enter the attendees in the box provided.";
                custval.Text = "*";
                cell.Controls.Add(custval);

            }
            else
            {
                cell.Text = "&nbsp;";
            }

            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip255" + id + "\" onclick=\"SEL.Tooltip.Show('f35dc6ea-9994-4056-bfa9-204840f0cd0d', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);

            row.Cells.Add(cell);
            tbl.Rows.Add(row);

        }

        #endregion

        #region Number of nights

        if (subcat.nonightsapp)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Number of Nights:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.MaxLength = 3;
            txtbox.ID = "txtnonights" + id;
            if (expenseitem != null)
            {
                if (expenseitem.nonights > 0)
                {
                    txtbox.Text = expenseitem.nonights.ToString();
                }
            }
            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            row.Cells.Add(cell);
            cell = new TableCell();
            compval = new CompareValidator();
            compval.ControlToValidate = "txtnonights" + id;
            compval.ValueToCompare = "255";
            compval.ValidationGroup = "vgAeExpenses";
            compval.Type = ValidationDataType.Integer;
            compval.Operator = ValidationCompareOperator.LessThanEqual;
            compval.Text = "*";
            compval.ErrorMessage = "The maximum number of nights that can be entered is 255.";
            cell.Controls.Add(compval);
            filt = new FilteredTextBoxExtender();
            filt.ID = "fltnonights" + id;
            filt.TargetControlID = "txtnonights" + id;
            filt.FilterType = FilterTypes.Numbers;
            cell.Controls.Add(filt);
            row.Cells.Add(cell);

            cell = new TableCell();

            if (true) //clshelp.containsTip(257))
            {
                lit = new Literal();
                lit.Text = "<img id=\"imgtooltip257" + id + "\" onclick=\"SEL.Tooltip.Show('3392df21-aff1-4f1b-b56b-3d7c3bf75713', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
                cell.Controls.Add(lit);
            }
            //else 
            //{
            //    cell .Text = "&nbsp;";
            //}

            row.Cells.Add(cell);
            tbl.Rows.Add(row);

        }

        #endregion

        #region Number of rooms

        if (subcat.noroomsapp)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Number of Rooms:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.ID = "txtnorooms" + id;
            txtbox.MaxLength = 3;
            if (expenseitem != null)
            {
                if (expenseitem.norooms > 0)
                {
                    txtbox.Text = expenseitem.norooms.ToString();
                }
            }
            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            row.Cells.Add(cell);
            cell = new TableCell();
            compval = new CompareValidator();
            compval.ControlToValidate = "txtnorooms" + id;
            compval.ValueToCompare = "255";
            compval.Type = ValidationDataType.Integer;
            compval.Operator = ValidationCompareOperator.LessThanEqual;
            compval.Text = "*";
            compval.ValidationGroup = "vgAeExpenses";
            compval.ErrorMessage = "The maximum number of rooms that can be entered is 255.";
            cell.Controls.Add(compval);
            filt = new FilteredTextBoxExtender();
            filt.ID = "fltnorooms" + id;
            filt.TargetControlID = "txtnorooms" + id;
            filt.FilterType = FilterTypes.Numbers;
            cell.Controls.Add(filt);
            row.Cells.Add(cell);

            cell = new TableCell();
            if (true) //clshelp.containsTip(379))
            {
                lit = new Literal();
                lit.Text = "<img id=\"imgtooltip379" + id + "\" onclick=\"SEL.Tooltip.Show('adf0f265-1a7e-4bc7-8f28-465c9523757f', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
                cell.Controls.Add(lit);
            }
            //else 
            //{
            //    cell.Text = "&nbsp;";
            //}

            row.Cells.Add(cell);
            tbl.Rows.Add(row);

        }

        #endregion

        #region Tip

        //see if there is a tip flag
        TipFlag tipFlag = (TipFlag)flagManagement.GetFlagByTypeRoleAndExpenseItem(FlagType.TipLimitExceeded, itemRoleID, subcat.subcatid);
        if (subcat.tipapp)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Tip/Service Charge:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.ID = "txttip" + id;
            txtbox.MaxLength = 10;
            if (expenseitem != null)
            {
                if (expenseitem.tip > 0)
                {
                    txtbox.Text = expenseitem.tip.ToString("########0.00");
                }
            }
            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            row.Cells.Add(cell);
            cell = new TableCell();
            compval = new CompareValidator(); // new CompareValidator();
            compval.ControlToValidate = "txttip" + id;
            compval.Type = ValidationDataType.Currency;
            compval.ValidationGroup = "vgAeExpenses";
            compval.Operator = ValidationCompareOperator.GreaterThanEqual;
            compval.ValueToCompare = "0";
            compval.Text = "*";
            compval.ErrorMessage = getValidationMsg("Tip/Service Charge", FieldType.Currency);
            cell.Controls.Add(compval);

            if (tipFlag != null && tipFlag.Action == FlagAction.BlockItem)
            {
                txtbox = new TextBox();
                txtbox.ID = "txttiplimit" + id;
                txtbox.Style.Add("display", "none");
                txtbox.Text = tipFlag.TipLimit.ToString();
                cell.Controls.Add(txtbox);
                custval = new CustomValidator();
                custval.ControlToValidate = "txttip" + id;
                custval.ID = "custtip" + id;
                custval.ClientValidationFunction = "checkTip";
                custval.ErrorMessage = "The tip you have entered is invalid. The maximum tip that can be claimed can be no more than " + tipFlag.TipLimit + "% of the total";
                custval.Text = "*";
                custval.ValidationGroup = "vgAeExpenses";
                cell.Controls.Add(custval);
            }
            row.Cells.Add(cell);

            cell = new TableCell();
            if (true) //clshelp.containsTip(258))
            {
                lit = new Literal();
                lit.Text = "<img id=\"imgtooltip258" + id + "\" onclick=\"SEL.Tooltip.Show('c57f173c-bf90-4216-8664-bf8a4310777a', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
                cell.Controls.Add(lit);

            }
            //else 
            //{
            //    cell.Text = "&nbsp;";
            //}

            row.Cells.Add(cell);
            tbl.Rows.Add(row);

        }

        #endregion

        #region receipt

        if (subcat.receiptapp && !split)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Do you have a receipt:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            cell = new TableCell();

            rd = new RadioButton();
            rd.ID = "optnormalreceiptyes" + id;
            rd.ClientIDMode = ClientIDMode.Static;
            rd.Text = "Yes";
            rd.GroupName = "normalreceipt" + id;
            if (expenseitem != null)
            {
                if (expenseitem.normalreceipt)
                {
                    rd.Checked = true;
                }
            }
            else if (mobileItem != null)
            {
                rd.Checked = mobileItem.HasReceipt;
            }
            cell.Controls.Add(rd);
            rd = new RadioButton();
            rd.ID = "optnormalreceiptno" + id;
            rd.ClientIDMode = ClientIDMode.Static;
            rd.GroupName = "normalreceipt" + id;
            rd.Text = "No";
            if (expenseitem != null)
            {
                if (!expenseitem.normalreceipt)
                {
                    rd.Checked = true;
                }
            }
            cell.Controls.Add(rd);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = "&nbsp;";
            row.Cells.Add(cell);

            cell = new TableCell();


            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip259" + id + "\" onclick=\"SEL.Tooltip.Show('6785426d-b23a-4537-96ee-3f7b2056e8a7','ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);

            row.Cells.Add(cell);

            tbl.Rows.Add(row);

        }

        #endregion

        #region vat receipt

        if (subcat.vatreceipt && subcat.vatapp && !split)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Does it include a VAT number and VAT rate:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            cell = new TableCell();

            rd = new RadioButton();
            rd.ID = "optvatreceiptyes" + id;
            rd.Text = "Yes";
            rd.ClientIDMode = ClientIDMode.Static;
            rd.GroupName = "vatreceipt" + id;
            if (expenseitem != null)
            {
                if (expenseitem.receipt)
                {
                    rd.Checked = true;
                }
            }
            cell.Controls.Add(rd);
            rd = new RadioButton();
            rd.ID = "optvatreceiptno" + id;
            rd.ClientIDMode = ClientIDMode.Static;
            rd.GroupName = "vatreceipt" + id;
            rd.Text = "No";
            if (expenseitem != null)
            {
                if (!expenseitem.receipt)
                {
                    rd.Checked = true;
                }
            }
            cell.Controls.Add(rd);
            row.Cells.Add(cell);
            cell = new TableCell();
            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip260" + id + "\" onclick=\"SEL.Tooltip.Show('3fdb3dea-6290-4678-be6e-0dc0a9a5d9a8', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);

            row.Cells.Add(cell);


            tbl.Rows.Add(row);

            if (subcat.vatnumberapp)
            {
                row = new TableRow();
                cell = new TableCell();
                cell.Text = "VAT Number:";
                cell.CssClass = "labeltd";
                row.Cells.Add(cell);
                txtbox = new TextBox();
                txtbox.ID = "txtvatnum" + id;
                cell = new TableCell();
                cell.Controls.Add(txtbox);
                row.Cells.Add(cell);

                tbl.Rows.Add(row);
            }



        }

        #endregion

        #region event in home city

        if (subcat.eventinhomeapp)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Event in home city:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            cell = new TableCell();
            //cell.CssClass = "inputtd";
            rd = new RadioButton();
            rd.ID = "opteventinhomeyes" + id;
            rd.Text = "Yes";
            rd.GroupName = "eventinhome" + id;
            if (expenseitem != null)
            {
                if (expenseitem.home)
                {
                    rd.Checked = true;
                }
            }
            cell.Controls.Add(rd);
            rd = new RadioButton();
            rd.ID = "opteventinhomeno" + id;
            rd.GroupName = "eventinhome" + id;
            rd.Text = "No";
            if (expenseitem != null)
            {
                if (!expenseitem.home)
                {
                    rd.Checked = true;
                }
            }
            cell.Controls.Add(rd);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = "&nbsp;";
            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip261" + id + "\" onclick=\"SEL.Tooltip.Show('a054cdd9-0d7f-440e-a967-940ad962a0ab', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);


            row.Cells.Add(cell);
            tbl.Rows.Add(row);

        }

        #endregion

        #region allowance/excess mileage

        if (subcat.calculation == CalculationType.FixedAllowance || subcat.IsRelocationMileage || subcat.calculation == CalculationType.ExcessMileage)
        {
            #region claim allowance?

            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Claim Allowance:";
            if (subcat.calculation == CalculationType.ExcessMileage)
            {
                cell.Text = "Claim Fixed Excess Mileage:";
            }
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "inputtd";
            chkbox = new CheckBox();
            chkbox.ID = "chkallowance" + id;
            chkbox.Checked = true;
            cell.Controls.Add(chkbox);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = "&nbsp;";
            row.Cells.Add(cell);

            tbl.Rows.Add(row);

            #endregion


            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Number of Allowances:";
            if (subcat.calculation == CalculationType.ExcessMileage)
            {
                cell.Text = "Number of Journeys:";
            }
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.ID = "txtallowances" + id;

            if (expenseitem != null)
            {
                if (expenseitem.quantity > 0)
                {
                    txtbox.Text = expenseitem.quantity.ToString();
                }
            }
            else if (mobileItem != null)
            {
                if (mobileItem.Quantity > 0)
                {
                    txtbox.Text = mobileItem.Quantity.ToString();
                }
            }
            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            row.Cells.Add(cell);
            cell = new TableCell();
            filt = new FilteredTextBoxExtender();
            filt.ID = "fltallowances" + id;
            filt.TargetControlID = "txtallowances" + id;
            filt.FilterType = FilterTypes.Custom;
            filt.ValidChars = ".0123456789";
            cell.Controls.Add(filt);
            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            var tooltipid = subcat.calculation == CalculationType.ExcessMileage
                                ? "BB680AAE-3376-4777-9CAE-F550015593B8"
                                : "79094423-60a4-4672-afe7-31070b1e4084";
            lit.Text = "<img id=\"imgtooltip264" + id + "\" onclick=\"SEL.Tooltip.Show('"+ tooltipid +"','ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);


            row.Cells.Add(cell);
            tbl.Rows.Add(row);
        }

        #endregion

        #region dailyallowances

        if (subcat.calculation == CalculationType.DailyAllowance)
        {
            cAllowances clsallowances = actionContext.Allowances;

            row = new TableRow();
            cell = new TableCell();

            #region allowance top line

            cell.Text = "Allowance:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "inputtd";
            ddlst = new DropDownList();
            ddlst.ID = "cmballowance" + id;
            ddlst.Items.AddRange(clsallowances.CreateDropDown(subcat.allowances).ToArray());

            if (expenseitem != null)
            {
                if (ddlst.Items.FindByValue(expenseitem.allowanceid.ToString()) != null)
                {
                    ddlst.Items.FindByValue(expenseitem.allowanceid.ToString()).Selected = true;
                }
            }
            else if (mobileItem != null && mobileItem.AllowanceTypeID.HasValue)
            {
                if (ddlst.Items.FindByValue(mobileItem.AllowanceTypeID.Value.ToString()) != null)
                {
                    ddlst.Items.FindByValue(mobileItem.AllowanceTypeID.Value.ToString()).Selected = true;
                }
            }
            cell.Controls.Add(ddlst);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = "&nbsp;";
            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip265" + id + "\" onclick=\"SEL.Tooltip.Show('f34fdb4d-01ab-4091-958a-c450234f77e4','ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);


            row.Cells.Add(cell);

            tbl.Rows.Add(row);

            #endregion

            #region startdate

            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Start Date:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            cell = new TableCell();
            txtbox = new TextBox();
            txtbox.ID = "txtstartdate" + id;

            if (expenseitem != null)
            {
                txtbox.Text = expenseitem.allowancestartdate.ToShortDateString();
            }
            else if (mobileItem != null)
            {
                txtbox.Text = mobileItem.dtAllowanceStartDate.HasValue ? mobileItem.dtAllowanceStartDate.Value.ToShortDateString() : "";
            }
            else
            {
                txtbox.Text = DateTime.Today.AddDays(-1).ToShortDateString();
            }

            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);

            maskededit = new MaskedEditExtender();
            maskededit.TargetControlID = "txtstartdate" + id;
            maskededit.Mask = "99/99/9999";
            maskededit.MaskType = MaskedEditType.Date;
            maskededit.ID = "mskstartdate" + id;
            maskededit.CultureName = "en-GB";
            img = new Image();
            img.ImageUrl = "icons/cal.gif";
            img.ID = "imgcalstartdate" + id;
            cell.Controls.Add(img);
            cal = new CalendarExtender();
            cal.TargetControlID = "txtstartdate" + id;
            cal.Format = "dd/MM/yyyy";
            cal.PopupButtonID = "imgcalstartdate" + id;
            cal.ID = "calstartdate" + id;
            cell.Controls.Add(cal);
            cell.Controls.Add(maskededit);

            row.Cells.Add(cell);
            cell = new TableCell();

            compval = new CompareValidator();
            compval.ControlToValidate = "txtstartdate" + id;

            compval.Text = "*";
            compval.ErrorMessage = "The Start Date of the allowance you have entered is not valid";

            compval.Type = ValidationDataType.Date;
            compval.ValidationGroup = "vgAeExpenses";
            compval.Operator = ValidationCompareOperator.DataTypeCheck;
            compval.ID = "comptxtstartdate" + id;

            cell.Controls.Add(compval);

            compval = new CompareValidator();
            compval.ControlToValidate = "txtstartdate" + id;
            compval.Text = "*";
            compval.ErrorMessage = "The Start Date must be on or later than 01/01/1900";
            compval.Type = ValidationDataType.Date;
            compval.ValidationGroup = "vgAeExpenses";
            compval.Operator = ValidationCompareOperator.GreaterThanEqual;
            compval.ID = "comptxtstartdategt" + id;
            compval.ValueToCompare = "01/01/1900";
            cell.Controls.Add(compval);

            compval = new CompareValidator();
            compval.ControlToValidate = "txtstartdate" + id;
            compval.Text = "*";
            compval.ErrorMessage = "The Start Date must be on or later than 31/12/3000";
            compval.Type = ValidationDataType.Date;
            compval.ValidationGroup = "vgAeExpenses";
            compval.Operator = ValidationCompareOperator.LessThanEqual;
            compval.ID = "comptxtstartdatelt" + id;
            compval.ValueToCompare = "31/12/3000";
            cell.Controls.Add(compval);

            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip266" + id + "\" onclick=\"SEL.Tooltip.Show('1ee0a7b4-2a7a-4c7d-b665-d44b9aa69bf0', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);


            row.Cells.Add(cell);

            #endregion

            #region start time

            cell = new TableCell();
            cell.Text = "Start Time:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.ID = "txtstarttime" + id;

            if (expenseitem != null)
            {
                txtbox.Text = expenseitem.allowancestartdate.ToShortTimeString();
            }
            else if (mobileItem != null)
            {
                txtbox.Text = mobileItem.dtAllowanceStartDate.HasValue ? mobileItem.dtAllowanceStartDate.Value.ToShortTimeString() : "";
            }
            else
            {
                txtbox.Text = DateTime.Now.ToShortTimeString();
            }
            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            maskededit = new MaskedEditExtender();
            maskededit.TargetControlID = "txtstarttime" + id;
            maskededit.Mask = "99:99";
            maskededit.MaskType = MaskedEditType.Time;
            maskededit.UserTimeFormat = MaskedEditUserTimeFormat.TwentyFourHour;
            maskededit.ID = "mskstarttime" + id;
            cell.Controls.Add(maskededit);

            regexval = new RegularExpressionValidator();
            regexval.ControlToValidate = "txtstarttime" + id;
            regexval.ID = "regexstarttime" + id;
            regexval.ValidationExpression = "^([0-1]?\\d|2[0-3]):([0-5]\\d)$";
            regexval.Text = "*";
            regexval.ErrorMessage = "Invalid start time entered";
            regexval.ValidationGroup = "vgAeExpenses";
            cell.Controls.Add(regexval);
            row.Cells.Add(cell);

            cell = new TableCell();
            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip267" + id + "\" onclick=\"SEL.Tooltip.Show('eca8bc06-960b-4633-adba-cbdfce354558', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);

            row.Cells.Add(cell);
            tbl.Rows.Add(row);

            #endregion

            #region end date

            row = new TableRow();
            cell = new TableCell();
            cell.Text = "End Date:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.ID = "txtenddate" + id;
            if (expenseitem != null)
            {
                txtbox.Text = expenseitem.allowanceenddate.ToShortDateString();
            }
            else if (mobileItem != null)
            {
                txtbox.Text = mobileItem.dtAllowanceEndDate.HasValue ? mobileItem.dtAllowanceEndDate.Value.ToShortDateString() : "";
            }
            else
            {
                txtbox.Text = DateTime.Today.AddDays(-1).ToShortDateString();
            }
            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            maskededit = new MaskedEditExtender();
            maskededit.TargetControlID = "txtenddate" + id;
            maskededit.Mask = "99/99/9999";
            maskededit.MaskType = MaskedEditType.Date;
            maskededit.ID = "mskenddate" + id;
            maskededit.CultureName = "en-GB";
            img = new Image();
            img.ImageUrl = "icons/cal.gif";
            img.ID = "imgcalenddate" + id;
            cell.Controls.Add(img);
            cal = new CalendarExtender();
            cal.TargetControlID = "txtenddate" + id;
            cal.Format = "dd/MM/yyyy";
            cal.PopupButtonID = "imgcalenddate" + id;
            cal.ID = "calenddate" + id;
            cell.Controls.Add(cal);
            cell.Controls.Add(maskededit);
            row.Cells.Add(cell);
            cell = new TableCell();
            compval = new CompareValidator();
            compval.ControlToValidate = "txtenddate" + id;
            compval.Text = "*";
            compval.ValidationGroup = "vgAeExpenses";
            compval.ErrorMessage = "The End Date of the allowance you have entered is not valid";
            compval.Type = ValidationDataType.Date;
            compval.Operator = ValidationCompareOperator.DataTypeCheck;
            compval.ID = "comptxtenddate" + id;
            cell.Controls.Add(compval);

            compval = new CompareValidator();
            compval.ID = "cmpenddategt" + id;
            compval.ControlToValidate = "txtenddate" + id;
            compval.Operator = ValidationCompareOperator.GreaterThanEqual;
            compval.Type = ValidationDataType.Date;
            compval.Text = "*";
            compval.ErrorMessage = "The End Date of the allowance should be on or later than 01/01/1900";
            compval.ValidationGroup = "vgAeExpenses";
            compval.ValueToCompare = "01/01/1900";
            cell.Controls.Add(compval);

            compval = new CompareValidator();
            compval.ID = "cmpenddatelt" + id;
            compval.ControlToValidate = "txtenddate" + id;
            compval.Operator = ValidationCompareOperator.LessThanEqual;
            compval.Type = ValidationDataType.Date;
            compval.Text = "*";
            compval.ErrorMessage = "The End Date of the allowance should be on or earlier than 31/12/3000";
            compval.ValidationGroup = "vgAeExpenses";
            compval.ValueToCompare = "31/12/3000";
            cell.Controls.Add(compval);

            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip268" + id + "\" onclick=\"SEL.Tooltip.Show('3533596f-ba3d-4732-b9b7-c4dd31897a4b', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);

            row.Cells.Add(cell);

            #endregion

            #region end time

            cell = new TableCell();
            cell.Text = "End Time:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            txtbox.ID = "txtendtime" + id;
            if (expenseitem != null)
            {
                txtbox.Text = expenseitem.allowanceenddate.ToShortTimeString();
            }
            else if (mobileItem != null)
            {
                txtbox.Text = mobileItem.dtAllowanceEndDate.HasValue ? mobileItem.dtAllowanceEndDate.Value.ToShortTimeString() : "";
            }
            else
            {
                txtbox.Text = DateTime.Now.ToShortTimeString();
            }
            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            maskededit = new MaskedEditExtender();
            maskededit.TargetControlID = "txtendtime" + id;
            maskededit.Mask = "99:99";
            maskededit.MaskType = MaskedEditType.Time;
            maskededit.UserTimeFormat = MaskedEditUserTimeFormat.TwentyFourHour;
            maskededit.ID = "mskendtime" + id;
            cell.Controls.Add(maskededit);

            regexval = new RegularExpressionValidator();
            regexval.ControlToValidate = "txtendtime" + id;
            regexval.ID = "regexendtime" + id;
            regexval.ValidationGroup = "vgAeExpenses";
            regexval.ValidationExpression = "^([0-1]?\\d|2[0-3]):([0-5]\\d)$";
            regexval.Text = "*";
            regexval.ErrorMessage = "Invalid end time entered";
            cell.Controls.Add(regexval);
            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip269" + id + "\" onclick=\"SEL.Tooltip.Show('c2c76460-8c3f-415d-9482-38759c424ca7', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);

            row.Cells.Add(cell);
            tbl.Rows.Add(row);

            #endregion

            #region deduct amount

            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Deduct Amount (in GBP):";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            txtbox = new TextBox();
            if (expenseitem != null)
            {
                if (expenseitem.allowancededuct > 0)
                {
                    txtbox.Text = expenseitem.allowancededuct.ToString("########0.00");
                }
            }
            else if (mobileItem != null)
            {
                txtbox.Text = mobileItem.AllowanceDeductAmount.ToString("########0.00");
            }
            txtbox.ID = "txtdeductamount" + id;
            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            row.Cells.Add(cell);
            cell = new TableCell();
            compval = new CompareValidator();
            compval.ControlToValidate = "txtdeductamount" + id;
            compval.Type = ValidationDataType.Currency;
            compval.Operator = ValidationCompareOperator.GreaterThan;
            compval.ValidationGroup = "vgAeExpenses";
            compval.ValueToCompare = "0";
            compval.Text = "*";
            compval.ErrorMessage = getValidationMsg("Deduct Amount", FieldType.Currency);
            cell.Controls.Add(compval);
            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip270" + id + "\" onclick=\"SEL.Tooltip.Show('43cccf4d-8b88-4fcb-879f-ef334208fd3b', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);


            row.Cells.Add(cell);

            tbl.Rows.Add(row);

            #endregion

        }

        #endregion

        #region vat

        if (expenseitem != null && subcat.vatapp && subcat.calculation != CalculationType.PencePerMile)
        {
            if (expenseitem.expenseid > 0)
            {
                if (expenseitem.basecurrency == clsproperties.basecurrency)
                {
                    row = new TableRow();
                    cell = new TableCell();
                    cell.Text = "VAT:";
                    cell.CssClass = "labeltd";
                    row.Cells.Add(cell);
                    cell = new TableCell();
                    cell.CssClass = "inputtd";
                    txtbox = new TextBox();
                    txtbox.ID = "txtvat" + id;
                    txtbox.Text = expenseitem.vat.ToString("########0.00");
                    cell.Controls.Add(txtbox);
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    CompareValidator cmpVat = new CompareValidator();
                    cmpVat.ValidationGroup = "vgAeExpenses";
                    cmpVat.Operator = ValidationCompareOperator.DataTypeCheck;
                    cmpVat.ID = "compval" + id;
                    cmpVat.Type = ValidationDataType.Double;
                    cmpVat.Text = "*";
                    cmpVat.ErrorMessage = "Invalid monetary amount entered for VAT";
                    cmpVat.ControlToValidate = txtbox.ID;
                    cell.Controls.Add(cmpVat);
                    //cell.Text = "&nbsp;";
                    row.Cells.Add(cell);

                    cell = new TableCell();

                    if (true) //clshelp.containsTip(262))
                    {
                        lit = new Literal();
                        lit.Text = "<img id=\"imgtooltip262" + id + "\" onclick=\"SEL.Tooltip.Show('71f72c89-5a14-4cb5-b826-946ea53a722e', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
                        cell.Controls.Add(lit);
                    }
                    //else
                    //{
                    //    cell.Text = "&nbsp;";
                    //}



                    row.Cells.Add(cell);
                    tbl.Rows.Add(row);
                }
            }
        }

        #endregion

        #region total

        if (subcat.calculation != CalculationType.PencePerMile && subcat.calculation != CalculationType.DailyAllowance && subcat.calculation != CalculationType.FixedAllowance && subcat.calculation != CalculationType.PencePerMileReceipt && subcat.calculation != CalculationType.ExcessMileage)
        {
            row = new TableRow();
            cell = new TableCell();
            if (subcat.subcatsplit.Count > 0)
            {
                if (subcat.addasnet)
                {
                    cell.Text = "Total Bill (NET):";
                }
                else
                {
                    cell.Text = "Total Bill (Gross):";
                }
            }
            else
            {
                if (subcat.addasnet)
                {
                    cell.Text = "Total (NET):";
                }
                else
                {
                    cell.Text = "Total (Gross):";
                }
            }
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);

            txtbox = new TextBox();
            txtbox.MaxLength = 10;
            txtbox.ID = "txttotal" + id;
            if (expenseitem != null && cardtransaction == null && mobileItem == null)
            {
                if (expenseitem.convertedtotal != 0)
                {
                    txtbox.Text = expenseitem.convertedgrandtotal.ToString("######0.00");
                }
                else if (expenseitem.grandtotal != 0)
                {
                    txtbox.Text = expenseitem.grandtotal.ToString("########0.00");
                }
            }
            else if (cardtransaction != null && !split)
            {
                cCardStatements clsstatements = actionContext.CardStatements;
                unallocatedamount = clsstatements.getUnallocatedAmount(transactionid, employeeid);
                txtbox.Text = unallocatedamount.ToString("########0.00");
            }
            else if (mobileItem != null && !split)
            {
                txtbox.Text = mobileItem.Total.ToString("########0.00");
            }

            if (subcat.subcatsplit.Count > 0 || split)
            {
                //get split ids
                string strsplit = "new Array(";
                if (split)
                {
                    foreach (int x in parentsubcat.subcatsplit)
                    {
                        strsplit += "'txttotal" + id.Replace(subcat.subcatid.ToString(), "") + x + "',";
                    }
                    strsplit = strsplit.Remove(strsplit.Length - 1, 1);
                    strsplit += ")";
                    txtbox.Attributes.Add("onblur", "calculateSplit('txtsplit" + id.Replace(subcat.subcatid.ToString(), "") + "','txttotal" + id.Replace(subcat.subcatid.ToString(), "") + "'," + strsplit + ");");
                }
                else
                {
                    foreach (int x in subcat.subcatsplit)
                    {
                        strsplit += "'txttotal" + id + x + "',";
                    }
                    strsplit = strsplit.Remove(strsplit.Length - 1, 1);
                    strsplit += ")";
                    txtbox.Attributes.Add("onblur", "calculateSplit('txtsplit" + id + "','txttotal" + id + "'," + strsplit + ");");
                }

            }

            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.Controls.Add(txtbox);
            row.Cells.Add(cell);
            cell = new TableCell();
            compval = new CompareValidator(); // new CompareValidator();
            compval.ControlToValidate = "txttotal" + id;
            compval.Type = ValidationDataType.Currency;
            compval.ValidationGroup = "vgAeExpenses";
            compval.Operator = ValidationCompareOperator.DataTypeCheck;
            compval.ValidationGroup = "vgAeExpenses";
            compval.Text = "*";
            compval.ErrorMessage = getValidationMsg("Total", FieldType.Currency);
            cell.Controls.Add(compval);
            if (cardtransaction != null && !split)
            {
                compval = new CompareValidator();
                compval.ControlToValidate = "txttotal" + id;
                compval.Type = ValidationDataType.Currency;
                compval.Operator = ValidationCompareOperator.LessThanEqual;
                compval.ValidationGroup = "vgAeExpenses";
                if (expenseitem != null)
                {
                    if (expenseitem.convertedgrandtotal > 0)
                    {
                        unallocatedamount += expenseitem.convertedgrandtotal;
                    }
                    else
                    {
                        unallocatedamount += expenseitem.grandtotal;
                    }
                }
                compval.ValueToCompare = unallocatedamount.ToString("######0.00");
                compval.Text = "*";
                compval.ErrorMessage = "The maximum amount that can be claimed on this item is " + unallocatedamount.ToString("###,###,##0.00");
                cell.Controls.Add(compval);
            }
            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip271" + id + "\" onclick=\"SEL.Tooltip.Show('18641886-7ca1-42c2-b078-f7f6411271de', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);
            row.Cells.Add(cell);


            row.Cells.Add(cell);

            tbl.Rows.Add(row);

        }

        #endregion

        #region advance

        if (!split && transactionid == 0)
        {
            cFloats clsfloats = actionContext.Floats;
            List<System.Web.UI.WebControls.ListItem> lstadvances = null;
            if (expenseitem == null)
            {
                if (clsfloats.getFloatCount(employeeid, selectedcurrency) > 0 && itemtype != ItemType.CreditCard && itemtype != ItemType.PurchaseCard)
                {
                    lstadvances = clsfloats.CreateDropDown(employeeid, selectedcurrency, 0);
                }
            }
            else
            {
                //if ((clsfloats.getFloatCount(employeeid, expenseitem.currencyid) != 0 || (expenseitem.floatid != 0)) && expenseitem.itemtype != ItemType.CreditCard && itemtype != ItemType.PurchaseCard)
                if (clsfloats.getFloatCount(employeeid, expenseitem.currencyid) != 0 && expenseitem.itemtype != ItemType.CreditCard && itemtype != ItemType.PurchaseCard)
                {
                    lstadvances = clsfloats.CreateDropDown(employeeid, expenseitem.currencyid, expenseitem.floatid);

                }
            }
            if (lstadvances != null)
            {
                if (lstadvances.Count > 0)
                {

                    row = new TableRow();
                    cell = new TableCell();
                    cell.CssClass = "labeltd";
                    cell.Text = "Deduct from Advance:";
                    row.Cells.Add(cell);
                    cell = new TableCell();
                    cell.CssClass = "inputtd";
                    ddlst = new DropDownList();
                    ddlst.Attributes.Add("onchange", "document.getElementById(contentID + 'txtadvance" + id + "').value = document.getElementById(contentID + 'cmbadvance" + id + "').options[document.getElementById(contentID + 'cmbadvance" + id + "').selectedIndex].value; disabledPaymentMethod(contentID + 'cmbadvance" + id + "', contentID +  'cmbpaymentmethod" + id + "' );");
                    ddlst.ID = "cmbadvance" + id;
                    ddlst.Items.AddRange(lstadvances.ToArray());
                    if (expenseitem != null)
                    {
                        if (ddlst.Items.FindByValue(expenseitem.floatid.ToString()) != null)
                        {
                            ddlst.Items.FindByValue(expenseitem.floatid.ToString()).Selected = true;
                        }
                    }
                    cell.Controls.Add(ddlst);
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Text = "&nbsp;";
                    row.Cells.Add(cell);

                    cell = new TableCell();

                    lit = new Literal();
                    lit.Text = "<img id=\"imgtooltip270" + id + "\" onclick=\"SEL.Tooltip.Show('43cccf4d-8b88-4fcb-879f-ef334208fd3b', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
                    cell.Controls.Add(lit);


                    row.Cells.Add(cell);
                    tbl.Rows.Add(row);
                }
            }
            txtbox = new TextBox();
            txtbox.ID = "txtadvance" + id;
            if (expenseitem != null)
            {
                if (expenseitem.floatid > 0)
                {
                    txtbox.Text = expenseitem.floatid.ToString();
                }
            }
            txtbox.Style.Add("display", "none");
            pnl.Controls.Add(txtbox);
        }

        #endregion

        #region purchased with cc

        if (!split && (creditcard == true || purchasecard == true) && subcat.calculation != CalculationType.FixedAllowance && subcat.calculation != CalculationType.PencePerMile && subcat.calculation != CalculationType.PencePerMileReceipt && subcat.calculation != CalculationType.ExcessMileage)
        {
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "Payment Method:";
            cell.CssClass = "labeltd";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = "inputtd";
            ddlst = new DropDownList();
            ddlst.ID = "cmbpaymentmethod" + id;
            ddlst.Attributes.Add("onchange", "getAdvancesOnItemType();");
            ddlst.Items.Add(new ListItem("Cash/Personal Credit Card", "1"));
            if (creditcard && subcat.calculation != CalculationType.FixedAllowance && (subcat.calculation == CalculationType.NormalItem || subcat.calculation == CalculationType.Meal || subcat.calculation == CalculationType.FuelReceipt || subcat.calculation == CalculationType.FuelCardMileage))
            {
                ddlst.Items.Add(new ListItem("Corporate Credit Card", "2"));
            }
            if (purchasecard && subcat.calculation != CalculationType.FixedAllowance && (subcat.calculation == CalculationType.NormalItem || subcat.calculation == CalculationType.Meal || subcat.calculation == CalculationType.FuelReceipt || subcat.calculation == CalculationType.FuelCardMileage))
            {
                ddlst.Items.Add(new ListItem("Corporate Purchase Card", "3"));
            }
            if (expenseitem != null)
            {
                if (ddlst.Items.FindByValue(((byte)expenseitem.itemtype).ToString()) != null)
                {
                    ddlst.Items.FindByValue(((byte)expenseitem.itemtype).ToString()).Selected = true;
                }
            }
            if (transactionid > 0)
            {
                ddlst.Enabled = false;
                cCardStatements clsstatements = actionContext.CardStatements;
                cCardTransaction transaction = clsstatements.getTransactionById(transactionid);
                cCardStatement statement = clsstatements.getStatementById(transaction.statementid);
                if (statement.Corporatecard.cardprovider.cardtype == CorporateCardType.CreditCard)
                {
                    ddlst.Items.FindByValue("2").Selected = true;
                }
                else
                {
                    ddlst.Items.FindByValue("3").Selected = true;
                }
            }
            cell.Controls.Add(ddlst);

            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = "&nbsp;";
            row.Cells.Add(cell);

            cell = new TableCell();

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip380" + id + "\" onclick=\"SEL.Tooltip.Show('ee52f735-ec2f-4b7a-9aa3-ad86e0a75178', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);

            row.Cells.Add(cell);
            tbl.Rows.Add(row);
        }

        #endregion

        #region Bank Account

        //Bank account details will be hidden if viewing as approver or via claim viewer.
        if (reqclaim == null || reqclaim.ClaimStage == ClaimStage.Current || employee.EmployeeID == user.EmployeeID)
        {
            var bankAccounts = actionContext.BankAccounts;
            List<ListItem> lstItems = bankAccounts.CreateDropDown(employee.EmployeeID);

            // Show bank dropdown list if more thank one bank accounts available
            if (lstItems.Count > 2)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug($"{lstItems.Count - 1} Bank accounts found so dropdown list will be populated");
                }
                ddlst = new DropDownList { ID = "cmbbankaccount" + id };
                ddlst.Items.AddRange(lstItems.ToArray());

                row = new TableRow();
                string bankAccountRequiredIndicator = user.MustHaveBankAccount ? "*" : string.Empty;

                cell = new TableCell { Text = @"Bank Account:" + bankAccountRequiredIndicator, CssClass = "labeltd" };
                row.Cells.Add(cell);
                cell = new TableCell { CssClass = "inputtd" };

                if (expenseitem != null)
                {
                    if (ddlst.Items.FindByValue(((int)expenseitem.bankAccountId).ToString()) != null)
                    {
                        ddlst.Items.FindByValue(((int)expenseitem.bankAccountId).ToString()).Selected = true;
                    }
                }

                cell.Controls.Add(ddlst);
                row.Cells.Add(cell);

                cell = new TableCell();

                if (user.MustHaveBankAccount)
                {
                    custval = new CustomValidator();
                    string uniqueKey = Guid.NewGuid().ToString().Replace('-', '_');
                    custval.ID = "custmand" + uniqueKey + id;
                    custval.ControlToValidate = ddlst.ID;
                    custval.ClientValidationFunction = "checkMandatory";
                    custval.ErrorMessage = @"Bank Account is a mandatory field. Please select a value";
                    custval.Text = "*";
                    custval.ValidationGroup = "vgAeExpenses";
                    custval.ValidateEmptyText = true;
                    cell.Controls.Add(custval);
                }
                else
                {
                    cell.Text = "&nbsp;";
                }

                row.Cells.Add(cell);
                cell = new TableCell();
                lit = new Literal
                {
                    Text = "<img id=\"imgtooltip381" + id + "\" onclick=\"SEL.Tooltip.Show('5B8AE795-5AA9-4BEE-B92C-DE2D82C1BF2C', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>"
                };

                cell.Controls.Add(lit);
                row.Cells.Add(cell);
                tbl.Rows.Add(row);
            }
            else
            {
                if (Log.IsDebugEnabled)
                {
                    Log.Debug($"{lstItems.Count - 1} Bank accounts found so dropdown list will not be populated");
                }
            }
        }
        #endregion


        #region otherdetails

        cFieldToDisplay otherdetails = clsmisc.GetGeneralFieldByCode("otherdetails");

        if (otherdetails.individual && subcat.otherdetailsapp && ((itemtype == ItemType.Cash && otherdetails.display) || (itemtype == ItemType.CreditCard && otherdetails.displaycc) || (itemtype == ItemType.PurchaseCard && otherdetails.displaypc)))
        {
            row = new TableRow();
            cell = new TableCell();
            cell.CssClass = "labeltd";
            cell.Text = otherdetails.description + ":";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.CssClass = "inputtd";
            cell.ColumnSpan = 4;
            txtbox = new TextBox();
            txtbox.ID = "txtotherdetails" + id;
            txtbox.TextMode = TextBoxMode.MultiLine;
            txtbox.Rows = 3;

            if (expenseitem != null)
            {
                txtbox.Text = expenseitem.reason;
            }
            else if (mobileItem != null)
            {
                txtbox.Text = mobileItem.OtherDetails;
            }

            txtbox.Width = Unit.Percentage(100);
            cell.Controls.Add(txtbox);

            row.Cells.Add(cell);

            cell = new TableCell();
            if ((itemtype == ItemType.Cash && otherdetails.mandatory) || (itemtype == ItemType.CreditCard && otherdetails.mandatorycc) || (itemtype == ItemType.PurchaseCard && otherdetails.mandatorypc))
            {
                reqval = new RequiredFieldValidator();
                reqval.ID = "reqotherdetails" + id;
                reqval.ControlToValidate = "txtotherdetails" + id;
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

            lit = new Literal();
            lit.Text = "<img id=\"imgtooltip244" + id + "\" onclick=\"SEL.Tooltip.Show('ef1754d1-4f12-4efc-8c32-33e7cc91f934', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
            cell.Controls.Add(lit);


            row.Cells.Add(cell);
            tbl.Rows.Add(row);
        }

        #endregion

        cTables clstables = actionContext.Tables;
        cTable udftbl = clstables.GetTableByID(new Guid("65394331-792e-40b8-af8b-643505550783"));
        if (expenseitem != null)
        {
            clsuserdefined.addItemsToPage(ref tbl, udftbl, true, id, expenseitem.userdefined, subcat.associatedudfs, "vgAeExpenses");
        }
        else
        {
            clsuserdefined.addItemsToPage(ref tbl, udftbl, true, id, null, subcat.associatedudfs, "vgAeExpenses");
        }
        if (subcat.subcatsplit.Count > 0)
        {
            this.AddSplitItemsToPanel(id, subcat, expenseitem, date, request, tbl, pnl, employee, actionContext);
        }
        row = new TableRow();
        cell = new TableCell();
        cell.Controls.Add(tbl);
        row.Cells.Add(cell);
        tblmain.Rows.Add(row);

        pnl.Controls.Add(tblmain);
        foreach (var control in dependsOnMileageGrid)
        {
            control.Visible = showingMileageGrid;
        }

        return pnl;
    }

    /// <summary>
    /// Generate DVLA message based on different condition
    /// </summary>
    /// <param name="isDelegate">Value which indicates whether user is delegate or not</param>
    /// <param name="isManualLicenceValid">Determines whether the manual licence is valid when dvla licence fails</param>
    /// <returns>final message</returns>
    private string GenerateDVLAConsentMessage(bool isDelegate, bool isManualLicenceValid)
    {
        var errorMessage = string.Empty;
        var delegateErrorMessage = $"Mileage cannot be claimed because consent is required in order for your organisation to perform DVLA checks.<br/><br/>Delegates are not permitted to provide consent on another user's behalf, please ask {this.employee.Forename} {this.employee.Surname} to log in and update their consent.";
        var dvlaErrorMessage = "In order for you to claim mileage, we need to check that you have a valid driving licence with DVLA. To verify your licence we will need you to provide consent to say that you agree for us to perform the check.<br/><br/><a class='error-comment-link' href='/shared/information/DrivingLicenceLookupConsent.aspx' target='_blank'><u>Click here to complete the consent for DVLA lookup.</u></a>";
      
        var consentExpiryFrequency = this.accountProperties.FrequencyOfConsentRemindersLookup;
        var frequencyInDays = int.Parse(consentExpiryFrequency);

        if (isManualLicenceValid)
        {
            return errorMessage;
        }
        if (this.employee.DriverId == null && ((this.employee.AgreeToProvideConsent.HasValue == false && this.employee.DvlaConsentDate == null) || (this.employee.AgreeToProvideConsent.HasValue && this.employee.AgreeToProvideConsent.Value)))
        {
            errorMessage = !isDelegate ? dvlaErrorMessage : delegateErrorMessage;
        }
        else if (this.employee.AgreeToProvideConsent.HasValue == false && this.employee.DvlaConsentDate <= DateTime.UtcNow.Date.AddYears(-3).AddDays(frequencyInDays))
        {
            var securityCode = this.employee.SecurityCode == Guid.Empty ? string.Empty : this.employee.SecurityCode.ToString();
            errorMessage = !isDelegate ? "Mileage cannot be claimed because your consent for DVLA check has expired.<br/><br/><a class='error-comment-link' href='" + System.Configuration.ConfigurationManager.AppSettings["DVLAConsentPortal"] + securityCode + "' target='_blank'><u>Click here to renew your consent for DVLA check.</u></a>" : delegateErrorMessage;
        }

        return errorMessage;
    }

    private void AddSplitItemsToPanel(
        string id,
        cSubcat subcat,
        cExpenseItem expenseitem,
        DateTime date,
        HttpRequest request,
        Table tbl,
        Control pnl,
        Employee claimant,
        IActionContext actionContext)
    {
        var row = new TableRow();
        var cell = new TableCell
        {
            CssClass = "labeltd",
            Text =
                subcat.addasnet
                    ? "Total Bill minus further expenses (NET):"
                    : "Total Bill minus further expenses (Gross):"
        };
        row.Cells.Add(cell);
        cell = new TableCell { ID = "tdsplit" + id, CssClass = "inputtd" };
        var txtbox = new TextBox { Enabled = false, ID = "txtsplit" + id };

        if (expenseitem != null)
        {
            txtbox.Text = expenseitem.convertedtotal > 0 ? expenseitem.convertedtotal.ToString("########0.00") : expenseitem.total.ToString("######0.00");
        }
        cell.Controls.Add(txtbox);
        row.Cells.Add(cell);
        tbl.Rows.Add(row);
        cell = new TableCell { Text = "Did you incur further expenses on this receipt?", ColumnSpan = 2 };
        row = new TableRow();
        row.Cells.Add(cell);
        tbl.Rows.Add(row);

        var clssubcats = actionContext.SubCategories;
        foreach (int splititem in subcat.subcatsplit)
        {
            cExpenseItem splitexpense = null;
            cSubcat splitsubcat = clssubcats.GetSubcatById(splititem);
            if (expenseitem != null)
            {
                splitexpense = expenseitem.getSplitItemBySubcat(splitsubcat.subcatid);
            }
            row = new TableRow();
            cell = new TableCell();
            cell.CssClass = "labeltd";
            cell.Text = splitsubcat.subcat + ":";
            row.Cells.Add(cell);
            cell = new TableCell();
            var rd = new RadioButton { ID = "optyes" + id + splitsubcat.subcatid, Text = "Yes" };
            if (splitexpense != null)
            {
                rd.Checked = true;
            }
            rd.GroupName = "splititem" + id + splitsubcat.subcatid;
            cell.Controls.Add(rd);
            rd = new RadioButton { ID = "optno" + id + splitsubcat.subcatid };
            if (splitexpense == null)
            {
                rd.Checked = true;
            }
            rd.GroupName = string.Format("splititem{0}{1}", id, splitsubcat.subcatid);
            rd.Text = "No";
            cell.Controls.Add(rd);
            row.Cells.Add(cell);
            tbl.Rows.Add(row);
            pnl.Controls.Add(tbl);
            Panel pnlsplit = this.generateItem(
                id + splitsubcat.subcatid,
                splitsubcat,
                subcat,
                splitexpense,
                true,
                date,
                claimant,
                request,
                actionContext);
            pnlsplit.ID = "pnl" + id + splitsubcat.subcatid;

            var collapse = new CollapsiblePanelExtender
            {
                ID = "coll" + id + splitsubcat.subcatid,
                ExpandDirection = CollapsiblePanelExpandDirection.Vertical,
                TargetControlID = pnlsplit.ID,
                ExpandControlID = "optyes" + id + splitsubcat.subcatid,
                CollapseControlID = "optno" + id + splitsubcat.subcatid,
                Collapsed = splitexpense == null
            };
            row = new TableRow();
            row.Cells.Add(new TableCell());
            cell = new TableCell();
            cell.Controls.Add(pnlsplit);
            cell.Controls.Add(collapse);
            row.Cells.Add(cell);
            tbl.Rows.Add(row);
        }
    }

    private void AddHomeToOfficeComment(ref Table tbl, string id)
    {
        var row = new TableRow();
        var cell = new TableCell { ColumnSpan = 4, CssClass = "hometoofficewarningcell" };
        cell.Controls.Add(new Label { CssClass = "hometoofficewarningicon" });
        cell.Controls.Add(new Label
        {
            CssClass = "hometoofficewarninglabel",
            Text = "Home to office deduction rules apply"
        });
        cell.Controls.Add(new Panel
        {
            CssClass = "hometoofficecomment commenttooltip",
            ClientIDMode = ClientIDMode.Predictable,
            ID = "hometoofficecomment" + id
        });
        row.Cells.Add(cell);
        tbl.Rows.Add(row);
    }

    private static TableCell CreateNumPassengersTooltipCell(string id)
    {
        TableCell cell;
        Literal lit;
        cell = new TableCell();

        lit = new Literal();
        lit.Text = "<img id=\"imgtooltip249" + id +
                   "\" onclick=\"SEL.Tooltip.Show('593d3477-6e50-4cce-9097-f409bf44ba93', 'ex', this);\" src=\"../icons/16/plain/tooltip.png\" alt=\"\" class=\"tooltipicon\"/>";
        cell.Controls.Add(lit);
        return cell;
    }

    private static TableCell CreateNumPassengersFilterCell(string id)
    {
        TableCell cell;
        FilteredTextBoxExtender filt;
        cell = new TableCell();
        filt = new FilteredTextBoxExtender();
        filt.ID = "fltpassengers" + id;
        filt.TargetControlID = "txtpassengers" + id;
        filt.FilterType = FilterTypes.Numbers;
        cell.Controls.Add(filt);
        return cell;
    }

    private static TableCell CreateNumPassengersValueCell(string id, cExpenseItem expenseitem)
    {
        TextBox txtbox;
        TableCell cell;
        txtbox = new TextBox();
        txtbox.ID = "txtpassengers" + id;
        txtbox.MaxLength = 2;
        if (expenseitem != null)
        {
            if (expenseitem.nopassengers > 0)
            {
                txtbox.Text = expenseitem.nopassengers.ToString();
            }
        }

        cell = new TableCell();
        cell.CssClass = "inputtd";
        cell.Controls.Add(txtbox);
        return cell;
    }

    private static TableCell CreateHeavyBulkyValueCell(string id, cExpenseItem expenseitem)
    {
        CheckBox chkbox;
        TableCell cell;
        chkbox = new CheckBox();
        chkbox.ID = "chkHeavyBulky" + id;
        if (expenseitem != null && expenseitem.journeysteps != null && expenseitem.journeysteps.Values.Count == 1)
        {
            //this method should only be called if we're in single journey step mode.
            //otherwise, the heavy bulky equipment checkbox is rendered as part of the journey grid
            //(in MileageGrid.html)
            chkbox.Checked = expenseitem.journeysteps.Values.Single().heavyBulkyEquipment;
        }

        cell = new TableCell();
        cell.CssClass = "inputtd";
        cell.Controls.Add(chkbox);
        return cell;
    }

    private static TableCell CreateNumPassengersLabelCell()
    {
        TableCell cell = new TableCell();
        cell.Text = "Number of Passengers:";
        cell.CssClass = "labeltd";
        return cell;
    }

    private static TableCell CreateHeavyBulkyLabelCell()
    {
        TableCell cell = new TableCell();
        cell.Text = "Heavy / Bulky Equipment:";
        cell.CssClass = "labeltd";
        return cell;
    }

    private Control CreateReturnToStartControl()
    {
        var commentDiv = new Panel { CssClass = "mileagecomment commenttooltip" };
        commentDiv.Controls.Add(new LiteralControl("Click to automatically create a step that returns you to the first address of your journey."));

        var div = new Panel { CssClass = "returntostart" };
        div.Controls.Add(new Label { CssClass = "returntostarticon" });
        div.Controls.Add(new Label { Text = "Return to start address" });
        div.Controls.Add(commentDiv);

        return div;
    }

    private Control CreateCarAndJourneyRateControl(
        cSubcat subcat,
        cExpenseItem expenseitem,
        string id,
        Employee claimemp,
        DateTime date,
        bool carIsSorn,
        List<DocumentExpiryResult> documentExpiryResults,
        List<ListItem> class1BusinessResults, bool isManualDocumentValid)
    {
        bool claimSubmitted = false;
        if (expenseitem != null)
        {
            cClaims clsclaims = new cClaims(accountid);
            var reqclaim = clsclaims.getClaimById(expenseitem.claimid);
            claimSubmitted = reqclaim.submitted;
        }

        var carAndJourneyRateDiv = new Panel();
        carAndJourneyRateDiv.CssClass = "carandjourneyrate";
        int carid = 0;
        if (DetermineEnableCarOptionJourneyGridForDocChecks(
            claimSubmitted,
            date,
            subcat.EnableDoC,
            carIsSorn,
            subcat.RequireClass1BusinessInsurance,
            documentExpiryResults, 
            class1BusinessResults))
        {
            carAndJourneyRateDiv.Controls.Add(new Label { CssClass = "caricon" });
            DropDownList carOptionComboBox = null;
            carAndJourneyRateDiv.Controls.Add(CreateCarOptionCell(subcat, expenseitem, id, out carOptionComboBox, date));

            if (subcat.calculation == CalculationType.PencePerMile || subcat.calculation == CalculationType.ExcessMileage)
            {
                AddMileageCatControls(
                    carAndJourneyRateDiv,
                    expenseitem,
                    id,
                    claimemp,
                    carOptionComboBox,
                    date,
                    subcat,
                    subcat.MileageCategory,
                    documentExpiryResults, 
                    class1BusinessResults);
                carid = Convert.ToInt32(carOptionComboBox.SelectedValue);
            }
        }

        if (this.clsEmployeeCars.GetActiveCars(date).Count == 0)
        {
            var activeCarDiv = new Panel() { CssClass = "error-comment" };
            var dvlaConsentMessage = string.Empty;
            if (this.accountProperties.BlockDrivingLicence && subcat.EnableDoC && this.accountProperties.EnableAutomaticDrivingLicenceLookup && this.currentUser.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect)) {
                // If we don't have a consent response on record, or we do and they've said yes
                if (this.employee.AgreeToProvideConsent.HasValue == false || (this.employee.AgreeToProvideConsent.HasValue && this.employee.AgreeToProvideConsent.Value == true))
                {
                    dvlaConsentMessage = this.GenerateDVLAConsentMessage(this.currentUser.isDelegate, isManualDocumentValid);
                }
            }
            var activeCarMessage = this.GetActiveVehicleMessage(dvlaConsentMessage);
            activeCarDiv.Controls.Add(activeCarMessage);
            carAndJourneyRateDiv.Controls.Add(activeCarDiv);
        }
        else
        {
            var dutyOfCareDiv = new Panel { CssClass = "error-comment" };
            var dutyOfCareMessage = new Label();

            if (clsproperties.blockinsuranceexpiry || clsproperties.blockmotexpiry || clsproperties.blocktaxexpiry
                || clsproperties.blockdrivinglicence || clsproperties.BlockBreakdownCoverExpiry)
            {
                dutyOfCareMessage.Text = this.CreateDutyOfCareExpiryMessages(
                    subcat.nSubcatid,
                    carid,
                    claimSubmitted,
                    date,
                    this.currentUser.isDelegate);
            }
            if (!string.IsNullOrEmpty(dutyOfCareMessage.Text))
            {
                dutyOfCareDiv.Controls.Add(dutyOfCareMessage);
                carAndJourneyRateDiv.Controls.Add(dutyOfCareDiv);
            }
        }

        if ((subcat.mileageapp || subcat.calculation == CalculationType.ExcessMileage) && clsproperties.AllowUsersToAddCars)
        {
            var addNewVehicleOuter = new Label { CssClass = "addnewvehicleouter" };
            addNewVehicleOuter.Controls.Add(new Label { CssClass = "addnewvehicleicon" });
            HyperLink lnkAddCarButton = new HyperLink();
            lnkAddCarButton.NavigateUrl = "javascript:showAddCarPanel('addacarlnk" + id + "');";
            lnkAddCarButton.Text = "Add new vehicle";
            lnkAddCarButton.CssClass = "expenseitemlink addnewvehiclelink";
            lnkAddCarButton.ID = "addacarlnk" + id;
            addNewVehicleOuter.Controls.Add(lnkAddCarButton);
            carAndJourneyRateDiv.Controls.Add(addNewVehicleOuter);

        }

        if (clsproperties.ActivateCarOnUserAdd == false)
        {
            List<int> lstUnapprovedCars = clsEmployeeCars.GetUnapprovedCars();
            string unapprovedTemplate = "You currently have $UNAPPROVEDCOUNT vehicles awaiting approval";
            var waitingToBeApprovedSpanOuter = new Label() { CssClass = "waitingtobeapproved" };
            waitingToBeApprovedSpanOuter.Controls.Add(new Label { CssClass = "waitingtobeapprovedicon" });
            var carCountTxt = unapprovedTemplate.Replace("$UNAPPROVEDCOUNT", lstUnapprovedCars.Count.ToString());
            carCountTxt = Regex.Replace(carCountTxt, @" 1 (\w+)s", " 1 $1"); //remove pluralization if 1
            var waitingToBeApprovedSpan = new Label { Text = carCountTxt, CssClass = "waitingtobeapprovedtext" };
            waitingToBeApprovedSpanOuter.Attributes.Add("data-unapprovedcount", lstUnapprovedCars.Count.ToString());
            waitingToBeApprovedSpanOuter.Attributes.Add("data-unapprovedmsgtemplate", unapprovedTemplate);
            waitingToBeApprovedSpanOuter.Controls.Add(waitingToBeApprovedSpan);
            if (lstUnapprovedCars.Count == 0)
            {
                waitingToBeApprovedSpanOuter.CssClass += " hidden";
            }
            carAndJourneyRateDiv.Controls.Add(waitingToBeApprovedSpanOuter);
        }

        return carAndJourneyRateDiv;

    }

    /// <summary>
    /// Gets the error message for active car
    /// </summary>
    /// <param name="dvlaConsentMessage">dvla error message</param>
    /// <returns>lable for error message</returns>
    private Label GetActiveVehicleMessage(string dvlaConsentMessage)
    {
        var activeCarMessage = new Label();
        if (!string.IsNullOrWhiteSpace(dvlaConsentMessage))
        {
            activeCarMessage.Text = dvlaConsentMessage;
        }
        else
        {
            activeCarMessage.Text += "You currently have no active vehicles to claim mileage against, please contact your administrator";
            if (this.clsproperties.ActivateCarOnUserAdd)
            {
                activeCarMessage.Text += " or Add a vehicle using the link below.";
            }
            else
            {
                activeCarMessage.Text += " or Add a vehicle using the link below and wait for your administrator to approve it.";
            }
        }

        return activeCarMessage;
    }

    /// <summary>
    /// This Message create Doc Expiry message for the car selected based on the general option
    /// </summary>
    /// <param name="subcatid">Expense item category
    /// </param>
    /// <param name="carid">Car id
    /// </param>
    /// <param name="claimSubmitted">Claim submitted date
    /// </param>
    /// <param name="expenseItemDate">
    /// The expense Item Date.
    /// </param>
    /// <param name="isDelegate">
    /// The is Delegate.
    /// </param>
    /// <returns>
    /// </returns>
    public string CreateDutyOfCareExpiryMessages(
        int subcatid,
        int carid,
        bool claimSubmitted,
        DateTime expenseItemDate,
        bool isDelegate = false)
    {
        string dutyOfCareMessage = string.Empty;
        var subcats = new cSubcats(this.accountid);
        var subcat = subcats.GetSubcatById(subcatid);
        var DutyOfCareDocuments = new DutyOfCareDocuments();
        var activeCars = this.clsEmployeeCars.GetActiveCars(this.accountProperties.UseDateOfExpenseForDutyOfCareChecks ? expenseItemDate : DateTime.Now, false);

        if (this.accountProperties.UseDateOfExpenseForDutyOfCareChecks)
        {
            activeCars = activeCars.Where(currentCar => currentCar.carid == carid).ToList();
        }

        var dutyOfCareExpiredDocuments = DutyOfCareDocuments.PassesDutyOfCare(accountid, activeCars, employeeid, this.accountProperties.UseDateOfExpenseForDutyOfCareChecks ? expenseItemDate : DateTime.Now, this.currentUser.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect)).FirstOrDefault();
        var documentExpiryResults = dutyOfCareExpiredDocuments.Key;
        var isManualDocumentValid = dutyOfCareExpiredDocuments.Value;
        var class1BusinessResults = DutyOfCareDocuments.Class1BusinessInformation(accountid, activeCars, this.accountProperties.UseDateOfExpenseForDutyOfCareChecks ? expenseItemDate : DateTime.Now);

        if (this.accountProperties.BlockDrivingLicence && subcat.EnableDoC && this.accountProperties.EnableAutomaticDrivingLicenceLookup && this.currentUser.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect))
        {
            // If we don't have a consent response on record, or we do and they've said yes
            if (this.employee.AgreeToProvideConsent.HasValue == false || (this.employee.AgreeToProvideConsent.HasValue && this.employee.AgreeToProvideConsent.Value == true))
            {
                dutyOfCareMessage = this.GenerateDVLAConsentMessage(isDelegate, isManualDocumentValid);
                if (!string.IsNullOrEmpty(dutyOfCareMessage))
                {
                    return dutyOfCareMessage;
                }
            }
        }

        bool isBicycle = false;
        var cars = new cEmployeeCars(accountid, employeeid);
        if (carid != 0)
        {
            cCar car = cars.GetCarByID(carid);
            isBicycle = (CarTypes.VehicleType)car.VehicleTypeID == CarTypes.VehicleType.Bicycle ? true : false;
        }
        if (this.accountProperties.UseDateOfExpenseForDutyOfCareChecks)
        {
            if (!documentExpiryResults.Any(car => car.carId == 0))
            {
                documentExpiryResults = documentExpiryResults.Where(i => i.carId == carid).ToList();
            }
            else
            {
                documentExpiryResults = documentExpiryResults.Where(i => i.carId == 0 && !isBicycle).ToList();
            }
            class1BusinessResults = class1BusinessResults.Where(i => i.Text == carid.ToString() || i.Text == string.Empty).ToList();
        }
        else
        {
            if (documentExpiryResults.Any(car => car.carId == 0))
            {
                bool hasAnyActiveCars = activeCars.Count > 0 ? true : false;
                documentExpiryResults = documentExpiryResults.Where(i => i.carId == 0 && hasAnyActiveCars).ToList();
            }
        }
        if (documentExpiryResults.Count > 0 && subcat.EnableDoC && !claimSubmitted)
        {
            dutyOfCareMessage = "Mileage cannot be claimed because - <br/> ";
            foreach (DocumentExpiryResult result in documentExpiryResults)
            {
                if (result.carId == 0 && !result.IsValidLicence)
                {
                    dutyOfCareMessage = InvalidLicenceErrorMessage;
                }
                else
                {
                    dutyOfCareMessage += string.Format("\u2022 {0} <br/>", result.Reason);
                }
            }
        }

        if (documentExpiryResults.Count > 0 && class1BusinessResults.Count > 0 && subcat.EnableDoC
            && subcat.RequireClass1BusinessInsurance && !claimSubmitted)
        {
            foreach (ListItem result in class1BusinessResults)
            {
                if (result != null)
                {
                    dutyOfCareMessage +=
                        string.Format(
                            "\u2022  Your vehicle with registration {0} does not have a class 1 business insurance. <br/>",
                            result.Value);
                }
            }
        }
        else if (documentExpiryResults.Count == 0 && class1BusinessResults.Count > 0 && subcat.EnableDoC
                 && subcat.RequireClass1BusinessInsurance && !claimSubmitted)
        {
            dutyOfCareMessage = "Mileage cannot be claimed because - <br/> ";
            foreach (ListItem result in class1BusinessResults)
            {
                if (result != null)
                {
                    dutyOfCareMessage +=
                        string.Format(
                            "\u2022  Your vehicle with registration {0} does not have a class 1 business insurance. <br/>",
                            result.Value);
                }
            }
        }
        if (documentExpiryResults.Count > 0 && subcat.EnableDoC && !claimSubmitted)
        {
            string entityview = string.Empty;
            string entityGuid = string.Empty;
            string viewGuid = string.Empty;
            string addLinkText = string.Empty;
            if (!documentExpiryResults.Any(car => car.carId == 0))
            {
                entityGuid = "F0247D8E-FAD3-462D-A19D-C9F793F984E8";
                viewGuid = "F1EA11DD-A18F-466D-B638-1E2EA2201F85";
                addLinkText = "add new vehicle documents";
            }
            else if (documentExpiryResults.Any(review => review.IsReviewFailed))
            {
                entityGuid = "5137C32E-E500-4297-BFF5-69CFED26C9B6";
                viewGuid = "0A1D9BC1-D2EA-4192-BA9E-DFA7066A096B";
                addLinkText = "add new driving licence review request";
            }
            else
            {
                entityGuid = "223018FE-EDAE-408E-8851-C09ABA09DF81";
                viewGuid = "F95C0754-82FA-493A-97B4-E1ED42B3C337";
                addLinkText = documentExpiryResults.Any(review => review.UpdateDocument) ? "update driving licence" : "add new driving licence";
            }
            //Create new doc document licence link is visible only for vehicle document/driving licence document with Auto look up is disabled
            if (!this.accountProperties.EnableAutomaticDrivingLicenceLookup && documentExpiryResults.All(car => car.carId != 0 || (car.carId != 0 && !car.IsAwaitingReview) || (car.carId == 0 && !car.IsAwaitingReview && car.IsValidLicence)))
            {
                entityview = new DutyOfCareDocumentsInformation().GetDocEntityAndViewIdByGuid(entityGuid, viewGuid, this.accountid);
                dutyOfCareMessage +=
                    $"<br /> <a class=\"error-comment-link\" href=/shared/viewentities.aspx?entityid={entityview.Split(',')[0]}&viewid={entityview.Split(',')[1]} target=_blank><u>Click here to {addLinkText} now.</u></a>";
            }
        }

        return dutyOfCareMessage;
    }

    private void AddMileageCatControls(Control parent, cExpenseItem expenseitem, string id, Employee claimemp, DropDownList carOptionComboBox, DateTime date, cSubcat subcat, int? enforceMileageCat, List<DocumentExpiryResult> documentExpiryResults, List<ListItem> class1BusinessResults)
    {
        bool claimSubmitted = false;
        if (expenseitem != null)
        {

            cClaims clsclaims = new cClaims(accountid);
            var reqclaim = clsclaims.getClaimById(expenseitem.claimid);
            claimSubmitted = reqclaim.submitted;
        }

        int emp = 0;
        if (claimemp != null && claimemp.EmployeeID > 0)
        {
            emp = claimemp.EmployeeID;
        }
        else
        {
            emp = employee.EmployeeID;
        }

        cEmployeeCars empCars = new cEmployeeCars(accountid, emp);

        bool docChecksEnabled = DetermineEnableCarOptionJourneyGridForDocChecks(
            claimSubmitted,
            date,
            subcat.EnableDoC,
            false,
            subcat.RequireClass1BusinessInsurance,
            documentExpiryResults, 
            class1BusinessResults);

        bool showMileageCats = empCars.Cars.Any(
        acar => acar.active &&
                acar.VehicleEngineTypeId > 0 &&
                acar.mileagecats.Count >= 1 &&
               docChecksEnabled);


        if (showMileageCats)
        {
            var mileageCatsDropDown = new DropDownList();
            mileageCatsDropDown.Attributes.Add("data-id", id);
            mileageCatsDropDown.ID = "cmbmileagecat" + id;
            parent.Controls.Add(mileageCatsDropDown);
            parent.Controls.Add(GetMileageCatTextBox(expenseitem, id, mileageCatsDropDown));

            var mileageCommentSpan = new Label() { ID = "mileagecomment" + id, CssClass = "mileagecomment commenttooltip" };
            parent.Controls.Add(mileageCommentSpan);

            if (enforceMileageCat.HasValue)
            {
                var mileageCats = new cMileagecats(accountid);
                var mileagecat = mileageCats.GetMileageCatById(enforceMileageCat.Value);
                var mileageCatListItem = cMileagecats.GetMileageCatListItem(mileagecat);
                var mileageCatListItems = new[] { mileageCatListItem };
                mileageCatsDropDown.Items.AddRange(mileageCatListItems);
                mileageCatsDropDown.Attributes.Add("data-enforced", "true");
            }

            int carid = 0;
            if (expenseitem == null || expenseitem.carid == 0)
            {
                carid = clsEmployeeCars.GetDefaultCarID(clsproperties.blocktaxexpiry, clsproperties.blockmotexpiry,
                                                        clsproperties.blockinsuranceexpiry,
                                                        clsproperties.BlockBreakdownCoverExpiry,
                                                        accountProperties.DisableCarOutsideOfStartEndDate, date);
                if (carid == 0 && !subcat.EnableDoC)
                {
                    carid = clsEmployeeCars.GetDefaultCarID(false, false, false, false, accountProperties.DisableCarOutsideOfStartEndDate, date);
                }
            }
            else
            {
                carid = expenseitem.carid;
            }

            if (!enforceMileageCat.HasValue)
            {
                //if it is enforced, then it will have already been populated with the 
                //one that it is enforced to by the code above.
                ListItem[] mileageCatListItems = clsEmployeeCars.CreateValidMileageCatDropdown(carid).ToArray();
                mileageCatsDropDown.Items.AddRange(mileageCatListItems);
            }

            if (expenseitem != null)
            {
                var currentMileageCatItem = mileageCatsDropDown.Items.FindByValue(expenseitem.mileageid.ToString());
                if (currentMileageCatItem != null)
                {
                    currentMileageCatItem.Selected = true;
                }
            }

            if (carOptionComboBox != null)
            {
                int? carId = null;
                if (expenseitem != null)
                {
                    carId = expenseitem.carid;
                }
                else
                {
                    ListItem[] carDropDownListItems = carOptionComboBox.Items.OfType<ListItem>().ToArray();
                    ListItem selectedCarListItem = carDropDownListItems.FirstOrDefault(i => i.Selected) ??
                                                   carDropDownListItems.FirstOrDefault();
                    int carIdVal = 0;
                    if (selectedCarListItem != null && int.TryParse(selectedCarListItem.Value, out carIdVal))
                        carId = carIdVal;
                }

                if (carId.HasValue)
                {
                    var mileage = new cMileagecats(accountid);
                    var cars = new cEmployeeCars(accountid, employeeid);
                    var employees = new cEmployees(accountid);
                    cCar car = cars.GetCarByID(carId.Value);

                    int? mileageCat;
                    long esrAssignmentId = 0;
                    if (expenseitem != null)
                    {
                        mileageCat = expenseitem.mileageid;
                        esrAssignmentId = expenseitem.ESRAssignmentId;
                    }
                    else
                    {
                        mileageCat = car.mileagecats.Cast<int?>().FirstOrDefault();
                    }
                    if (mileageCat.HasValue)
                    {
                        cMileageCat mileageCatById = mileage.GetMileageCatById(mileageCat.Value);
                        if (mileageCatById != null)
                        {

                            var user = cMisc.GetCurrentUser();

                            mileageCommentSpan.Text = mileage.GetMileageCommentOnly(employees.GetEmployeeById(emp),
                                                                        mileageCatById,
                                                                        date,
                                                                        car,
                                                                        ref esrAssignmentId,
                                                                        user);
                        }
                    }
                }
            }
        }
    }

    private static TextBox GetMileageCatTextBox(cExpenseItem expenseitem, string id, DropDownList mileageCatsDropDown)
    {
        var mileageCatTextBox = new TextBox { ID = "txtmileagecat" + id };
        mileageCatTextBox.Style.Add("display", "none");
        if (expenseitem != null)
        {
            mileageCatTextBox.Text = expenseitem.mileageid.ToString();
        }
        else
        {
            if (mileageCatsDropDown.Items.Count > 0)
            {
                mileageCatTextBox.Text = mileageCatsDropDown.SelectedValue;
            }
        }
        return mileageCatTextBox;
    }

    private Control CreateMileageCommentControl(cSubcat subcat, cExpenseItem expenseitem, bool carIsSorn)
    {
        var div = new Panel();
        var label = new Label();
        var commentLines = new List<string>();
        commentLines.Add(Regex.Replace(subcat.comment, @"\r\n?|\n", "<br/>"));
        if (!string.IsNullOrEmpty(subcat.comment))
        {
            commentLines.Add("<br/>");
        }
        if (!subcat.reimbursable)
        {
            commentLines.Add("Please Note: This item will NOT be reimbursed");
        }

        if (subcat.calculation == CalculationType.ExcessMileage)
        {
            if (string.Join(string.Empty, commentLines).Trim().Length > 0)
            {
                commentLines.Add("<br/>");
            }
            commentLines.Add("You are permitted to claim " + this.employee.ExcessMileage + " excess miles on each applicable journey.");
        }

        if (subcat.mileageapp && this.accountProperties.BlockTaxExpiry)
        {
            if (this.clsEmployeeCars.SornCars.Count > 0)
            {
                if (string.Join(string.Empty, commentLines).Trim().Length > 0)
                {
                    commentLines.Add("<br/>");
                }
                if (expenseitem != null && carIsSorn)
                {
                    commentLines.Add("Vehicle Registration " + this.clsEmployeeCars.SornCars.FirstOrDefault(i => i.Text == expenseitem.carid.ToString()).Value + " has been declared as SORN.");
                }
                else
                {
                    foreach (var car in this.clsEmployeeCars.SornCars)
                    {
                        commentLines.Add("Vehicle Registration " + car.Value + " has been declared as SORN.");
                        if (this.clsEmployeeCars.SornCars.Count > 1)
                        {
                            commentLines.Add("<br/>");
                        }
                    }
                }
            }
        }

        label.Text = string.Join(string.Empty, commentLines);
        div.CssClass = "comment";
        div.Controls.Add(label);
        return div;
    }

    private TableCell CreateCarOptionCell(cSubcat subcat, cExpenseItem expenseitem, string id, out DropDownList carOptionComboBox, DateTime date)
    {
        var cell = new TableCell();
        carOptionComboBox = null;
        int claimId = 0;
        bool claimsubmitted = false;
        cClaim reqclaim = null;
        if (expenseitem != null)
        {
            claimId = expenseitem.claimid;
            cClaims clsclaims = new cClaims(accountid);
            reqclaim = clsclaims.getClaimById(expenseitem.claimid);
            claimsubmitted = reqclaim.submitted;
        }

        if (clsEmployeeCars.GetActiveCars(date).Count >= 1)
        {
            cell.CssClass = "inputtd";
            carOptionComboBox = new DropDownList { ID = "cmbcars" + id };
            carOptionComboBox.Attributes.Add("onchange", "getMileageComment('" + id + "');getDocComment('" + id + "','" + claimsubmitted + "');");
            carOptionComboBox.Items.AddRange(clsEmployeeCars.CreateCurrentValidCarDropDown(date, @"Mileage cannot be claimed as you do not have an active vehicle for the date entered.", subcat.EnableDoC, claimId, true).ToArray());
            if (expenseitem != null)
            {
                if (carOptionComboBox.Items.FindByValue(expenseitem.carid.ToString()) != null)
                {
                    carOptionComboBox.Items.FindByValue(expenseitem.carid.ToString()).Selected = true;
                }
                else
                {
                    // if pool car that is no longer assoc with employee, add to ddlst and select just for this edit
                    cPoolCars clspoolcars = new cPoolCars(accountid);
                    if (clspoolcars.GetCarByID(expenseitem.carid) != null)
                    {
                        cCar poolcar = clspoolcars.GetCarByID(expenseitem.carid);
                        string desc = poolcar.make + " " + poolcar.model;
                        if (poolcar.registration != "")
                        {
                            desc += " (" + poolcar.registration + ")";
                        }
                        carOptionComboBox.Items.Add(new ListItem(desc, poolcar.carid.ToString()));

                        carOptionComboBox.Items.FindByValue(expenseitem.carid.ToString()).Selected = true;
                    }
                }
            }
            if (carOptionComboBox.Items.Count > 0)
            {
                cell.Controls.Add(carOptionComboBox);
            }
            else
            {
                cell.Text = "Your vehicle(s) does not match the criteria to allow you to claim against, please contact your administrator.";
            }
        }

        return cell;

    }

    /// <summary>
    /// The set ESR Drop Down List item as selected based on the ESR assignment number.
    /// </summary>
    /// <param name="ddlst">
    /// The Drop Down List to update.
    /// </param>
    /// <param name="expenseitem">
    /// The expense item.
    /// </param>
    private void SetESRItemAsSelected(DropDownList ddlst, cExpenseItem expenseitem)
    {
        if (expenseitem == null)
        {
            return;
        }

        if (ddlst.Items.FindByValue(expenseitem.ESRAssignmentId.ToString(CultureInfo.InvariantCulture)) != null)
        {
            ddlst.Items.FindByValue(expenseitem.ESRAssignmentId.ToString(CultureInfo.InvariantCulture)).Selected = true;
        }
    }

    private void AddRouteAndMap(ref Table table, string expenseIndexer)
    {
        TableRow row = new TableRow();
        TableCell cell = new TableCell() { ColumnSpan = 4 };
        var link = new Label { CssClass = "link showmaplink" };
        link.Controls.Add(new Label { CssClass = "mapicon" });
        link.Controls.Add(new Label { Text = "View the recommended route and directions on a map" });
        cell.Controls.Add(link);
        row.Cells.Add(cell);
        table.Rows.Add(row);
    }

    public string getValidationMsg(string field, FieldType fieldtype)
    {
        string msg = "The value entered for " + field + " is invalid. ";
        switch (fieldtype)
        {
            case FieldType.Currency:
            case FieldType.Number:
                msg += "Valid characters are the numbers 0-9 and a full stop (.)";
                break;
            case FieldType.Integer:
                msg += "Valid characters are the numbers 0-9";
                break;
            case FieldType.DateTime:
                msg += "Valid characters are the numbers 0-9 and a forward slash (/)";
                break;
                //case FieldType.DateTime:
                //    msg += "Valid characters are the numbers 0-9 and a colon (:)";
                //    break;

        }
        return msg;
    }

    public Panel getHotelPanel(string id, Hotel hotel)
    {
        Panel pnl = new Panel();
        Table tbl = new Table();
        TableRow row;
        TableCell cell;
        TextBox txtbox;

        row = new TableRow();
        cell = new TableCell();
        cell.CssClass = "labeltd";
        cell.Text = "Address Line 1:";
        row.Cells.Add(cell);
        cell = new TableCell();
        cell.CssClass = "inputtd";
        txtbox = new TextBox();
        txtbox.Enabled = false;
        txtbox.ID = "txtaddress1" + id;
        if (hotel != null)
        {
            txtbox.Text = hotel.address1;
        }
        cell.Controls.Add(txtbox);
        row.Cells.Add(cell);
        tbl.Rows.Add(row);

        row = new TableRow();
        cell = new TableCell();
        cell.CssClass = "labeltd";
        cell.Text = "Address Line 2:";
        row.Cells.Add(cell);
        cell = new TableCell();
        cell.CssClass = "inputtd";
        txtbox = new TextBox();
        txtbox.Enabled = false;
        txtbox.ID = "txtaddress2" + id;
        if (hotel != null)
        {
            txtbox.Text = hotel.address2;
        }
        cell.Controls.Add(txtbox);
        row.Cells.Add(cell);
        tbl.Rows.Add(row);

        row = new TableRow();
        cell = new TableCell();
        cell.CssClass = "labeltd";
        cell.Text = "City:";
        row.Cells.Add(cell);
        cell = new TableCell();
        cell.CssClass = "inputtd";
        txtbox = new TextBox();
        txtbox.Enabled = false;
        txtbox.ID = "txtcity" + id;
        if (hotel != null)
        {
            txtbox.Text = hotel.city;
        }
        cell.Controls.Add(txtbox);
        row.Cells.Add(cell);
        tbl.Rows.Add(row);

        row = new TableRow();
        cell = new TableCell();
        cell.CssClass = "labeltd";
        cell.Text = "County:";
        row.Cells.Add(cell);
        cell = new TableCell();
        cell.CssClass = "inputtd";
        txtbox = new TextBox();
        txtbox.Enabled = false;
        txtbox.ID = "txtcounty" + id;
        if (hotel != null)
        {
            txtbox.Text = hotel.county;
        }
        cell.Controls.Add(txtbox);
        row.Cells.Add(cell);
        tbl.Rows.Add(row);

        row = new TableRow();
        cell = new TableCell();
        cell.CssClass = "labeltd";
        cell.Text = "Post Code:";
        row.Cells.Add(cell);
        cell = new TableCell();
        cell.CssClass = "inputtd";
        txtbox = new TextBox();
        txtbox.Enabled = false;
        txtbox.ID = "txtpostcode" + id;
        if (hotel != null)
        {
            txtbox.Text = hotel.postcode;
        }
        cell.Controls.Add(txtbox);
        row.Cells.Add(cell);
        tbl.Rows.Add(row);

        row = new TableRow();
        cell = new TableCell();
        cell.CssClass = "labeltd";
        cell.Text = "Country:";
        row.Cells.Add(cell);
        cell = new TableCell();
        cell.CssClass = "inputtd";
        txtbox = new TextBox();
        txtbox.Enabled = false;
        txtbox.ID = "txtcountry" + id;
        if (hotel != null)
        {
            txtbox.Text = hotel.country;
        }
        cell.Controls.Add(txtbox);
        row.Cells.Add(cell);
        tbl.Rows.Add(row);
        txtbox = new TextBox();
        txtbox.ID = "txthotelid" + id;
        txtbox.Style.Add("display", "none");
        if (hotel != null)
        {
            txtbox.Text = hotel.hotelid.ToString();
        }
        pnl.Controls.Add(txtbox);
        pnl.ID = "pnlhotel" + id;
        pnl.Controls.Add(tbl);
        return pnl;
    }

    /// <summary>
    /// This method returns true or false based on the DOC general options to determine whether the Journey Grid and Mileage related controls need to display 
    /// </summary>
    /// <param name="submittedClaim"></param>
    /// <param name="expenseDate"></param>
    /// <param name="isEnableDoc"></param>
    /// <param name="carIsSorn"></param>
    /// <param name="RequireClass1BusinessInsurance"></param>
    /// <returns></returns>
    public bool DetermineEnableCarOptionJourneyGridForDocChecks(
       bool submittedClaim,
       DateTime expenseDate,
       bool isEnableDoc,
       bool carIsSorn, 
       bool RequireClass1BusinessInsurance, 
       List<DocumentExpiryResult> documentExpiryResults, 
       List<ListItem> class1BusinessResults)
    {
        bool outcome = false;
        bool docChecksEnabled = false;
        var activeCars = clsEmployeeCars.GetActiveCars(expenseDate, includePoolCars: false);

        docChecksEnabled = clsproperties.blockinsuranceexpiry || clsproperties.blockmotexpiry
                           || clsproperties.blocktaxexpiry || clsproperties.blockdrivinglicence
                           || clsproperties.BlockBreakdownCoverExpiry;

        //if None of Doc checks are enabled OR Doc checks are based on Date of claim (UseDateOfExpenseForDutyOfCareChecks general option is checked)
        //Display the car drop down and mileage grid
        if ((!docChecksEnabled) || (accountProperties.UseDateOfExpenseForDutyOfCareChecks))
        {
            outcome = activeCars.Count > 0 && !carIsSorn;
        }
        else if (!accountProperties.UseDateOfExpenseForDutyOfCareChecks)
        {
            // DofC checks are based on current date , any Doc options is enabled  
            //Display the car drop down and mileage grid only if all the cars have valid doc documents
            if (docChecksEnabled)
            {
                // activeCars.Count > there are no active cars(not bicycle) , this.documentExpiryResults.Any(i => i.carId == 0) there is ivalid driving licence message
                // Do not show the message driving licence expiry message for Bicycle  
                if (documentExpiryResults.Any(car => car.carId == 0))
                {
                    bool hasAnyActiveCars = activeCars.Count > 0 ? true : false;
                    documentExpiryResults = documentExpiryResults.Where(i => i.carId == 0 && hasAnyActiveCars).ToList();
                }
                outcome = (((documentExpiryResults.Count <= 0 || !isEnableDoc || (isEnableDoc && submittedClaim))
                        && activeCars.Count > 0 && !carIsSorn)
                        && (class1BusinessResults.Count == 0 || (accountProperties.BlockInsuranceExpiry && !isEnableDoc)
                        || (accountProperties.BlockInsuranceExpiry && isEnableDoc && !RequireClass1BusinessInsurance)
                        || !accountProperties.BlockInsuranceExpiry
                        ));
            }

        }
        return outcome;
    }


    /// <summary>
    /// This method returns true or false based on the DOC general options to determine whether Journey drop down need to be displayed 
    /// </summary>
    /// <param name="submittedClaim"></param>
    /// <param name="expenseDate"></param>
    /// <param name="isEnableDoc"></param>
    /// <param name="carIsSorn"></param>
    /// <param name="RequireClass1BusinessInsurance"></param>
    /// <returns></returns>
    public bool DetermineEnableJourneyGridForExpenseDateDocChecks(
        bool submittedClaim,
        DateTime expenseDate,
        bool isEnableDoc,
        bool carIsSorn,
        bool RequireClass1BusinessInsurance, 
        List<DocumentExpiryResult> documentExpiryResults, 
        List<ListItem> class1BusinessResults)
    {
        bool outcome = false;
        bool docChecksEnabled = false;
        docChecksEnabled = clsproperties.blockinsuranceexpiry || clsproperties.blockmotexpiry
                           || clsproperties.blocktaxexpiry || clsproperties.blockdrivinglicence
                           || clsproperties.BlockBreakdownCoverExpiry;

        var activeCarCount = this.clsEmployeeCars.GetActiveCars(expenseDate, includePoolCars: false).Count;

        //if None of Doc checks are enabled OR Doc checks are based on Date of claim (UseDateOfExpenseForDutyOfCareChecks general option is checked)
        if ((!docChecksEnabled) || (accountProperties.UseDateOfExpenseForDutyOfCareChecks))
        {
            outcome = activeCarCount > 0 && !carIsSorn;
            return outcome;
        }
        //if None of Doc checks are enabled OR date of expense is current date and DOC documents are valid
        if (!docChecksEnabled || (((documentExpiryResults.Count <= 0 || !isEnableDoc || (isEnableDoc && submittedClaim))
                                   && activeCarCount > 0 &&
                                   !carIsSorn)
                                  &&
                                  (class1BusinessResults.Count == 0 ||
                                   (accountProperties.BlockInsuranceExpiry && !isEnableDoc)
                                   ||
                                   (accountProperties.BlockInsuranceExpiry && isEnableDoc &&
                                    !RequireClass1BusinessInsurance) || !accountProperties.BlockInsuranceExpiry))
        )
        {
            //Any Doc options is enabled  
            //Display the car drop down and mileage grid only if all the cars have valid doc documents
            outcome = activeCarCount > 0 && !carIsSorn;

        }
        return outcome;

    }
}

