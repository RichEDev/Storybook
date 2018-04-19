using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using SpendManagementLibrary;
using System.ServiceModel.Activation;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using BusinessLogic.FilePath;
using Common.Cryptography;
using SpendManagementLibrary.Employees;
using SpendManagementLibrary.Enumerators;
using SpendManagementLibrary.Mobile;
using Spend_Management.expenses.code.Claims;

namespace Spend_Management
{
    using SpendManagementLibrary.Claims;
    using SpendManagementLibrary.Flags;

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MobileAPI : IMobileAPI, IMobileAPI_jsonp
    {
        public int AccountID { get; set; }
        public int SubAccountID { get; set; }

        /// <summary>
        /// MobileAPI constructor
        /// </summary>
        public MobileAPI()
        {

        }

        /// <summary>
        /// Retrieves subcats for the mobile device enquirer
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>ExpenseItemResult class record</returns>
        public ExpenseItemResult GetEmployeeSubCats(string pairingKey, string serialKey)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            ExpenseItemResult result = new ExpenseItemResult {FunctionName = "GetEmployeeSubCats", ReturnCode = retCode.ReturnCode};

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        PairingKey pairing = new PairingKey(pairingKey);
                        int employeeID = pairing.EmployeeID;
                        SortedList<int, ExpenseItemDetail> expenseItems = new SortedList<int, ExpenseItemDetail>();

                        cEmployees clsEmployees = new cEmployees(AccountID);
                        Employee reqEmployee = clsEmployees.GetEmployeeById(employeeID);

                        ItemRoles clsItemRoles = new ItemRoles(AccountID);

                        var clsSubcats = new cSubcats(AccountID);

                        // Get all the item roles for this employee
                        List<EmployeeItemRole> lstItemRoles = reqEmployee.GetItemRoles().ItemRoles;
                        // Loop through all item roles
                        foreach(EmployeeItemRole itemRole in lstItemRoles)
                        {
                            // Get this specific item role
                            ItemRole tmpItemRole = clsItemRoles.GetItemRoleById(itemRole.ItemRoleId);

                            // Loop through the subcats on this item role
                            foreach(RoleSubcat reqRoleSubcat in tmpItemRole.Items.Values)
                            {
                                if (expenseItems.ContainsKey(reqRoleSubcat.SubcatId))
                                {
                                    continue;
                                }

                                var subcat = clsSubcats.GetSubcatById(reqRoleSubcat.SubcatId);
                                expenseItems.Add
                                (
                                    subcat.subcatid,
                                    new ExpenseItemDetail
                                    {
                                        ID = subcat.subcatid,
                                        Name = subcat.subcat,
                                        Description = subcat.description,
                                        Calculation = (int)subcat.calculation,
                                        AllowanceAmount = subcat.allowanceamount,
                                        CategoryID = subcat.categoryid,
                                        VatNumberApp = subcat.vatnumberapp,
                                        VatNumberMandatory = subcat.vatnumbermand
                                    }
                                );
                            }
                        }

                        result.List = expenseItems.Values.ToList();
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.GetEmployeeSubCats():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Retrieve a receipt by its expense ID
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <param name="expenseid">Expense item ID to retrieve a receipt for</param>
        /// <returns>ReceiptResult class record</returns>
        public ReceiptResult GetReceiptByID(string pairingKey, string serialKey, int expenseid)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            ReceiptResult result = new ReceiptResult {FunctionName = "GetReceiptByID", ReturnCode = retCode.ReturnCode};

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        var pairing = new PairingKey(pairingKey);
                        var receiptData = new SpendManagementLibrary.Expedite.Receipts(pairing.AccountID, pairing.EmployeeID);
                        var claims = new cClaims();
                        var expenseItem = claims.getExpenseItemById(expenseid);
                        var user = cMisc.GetCurrentUser();
                        var claim = claims.getClaimById(expenseItem.claimid);
                        var subats = new cSubcats(user.AccountID);
                        var subCategory = subats.GetSubcatById(expenseid);


                        // multiple receipts per expense item are now possible, until the mobile apps can be enhanced we'll just return the first receipt
                        AttachedReceipt receipt = receiptData.GetByClaimLine(expenseItem, user, subCategory, claim)
                                                             .Select(r => AttachedReceipt.FromReceipt(r, pairing.AccountID))
                                                             .FirstOrDefault() ?? new AttachedReceipt();
                        
                        result.isApprover = CanCheckAndPay(pairing.AccountID, pairing.EmployeeID);
                        
                        if (receipt.filename != null)
                        {
                            Uri uri = OperationContext.Current.IncomingMessageHeaders.To;
                            string fullPath = uri.Scheme + "://" + uri.Host + (receipt.filename.StartsWith("/") ? receipt.filename : "/" + receipt.filename);
                            result.Receipt = fullPath;
                            result.Message = "success";
                            result.mimeHeader = receipt.mimeType;
                        }
                        else
                        {
                            result.Receipt = "";
                            result.Message = "file not found";
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.GetReceiptByID():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nExpenseID: " + expenseid.ToString() + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// Current DateTime of the server - UTC
        /// </summary>
        /// <returns></returns>
        public ServiceResultMessage GetServerDateTime()
        {
            var rm = new ServiceResultMessage() { FunctionName = "GetServerDateTime", ReturnCode = MobileReturnCode.Success, Message = DateTime.UtcNow.ToString("yyyyMMddHHmmsszzz") };
            return rm;
        }

        /// <summary>
        /// Retrieves sub-category list for requesting mobile device
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>SubcatResult class record</returns>
        public SubcatResult GetSubcatList(string pairingKey, string serialKey)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            SubcatResult result = new SubcatResult { FunctionName = "GetSubcatList", ReturnCode = retCode.ReturnCode };

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        PairingKey pairing = new PairingKey(pairingKey);
                        var clssubcats = new cSubcats(pairing.AccountID);
                        List<SubcatBasic> sorted = clssubcats.GetSortedList();

                        result.List = sorted;
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.GetSubcatList():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// Saves an expense item from mobile device
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <param name="items">Expense item collection to be saved</param>
        /// <returns>AddExpenseItemResult class record</returns>
        public AddExpenseItemResult SaveExpense(string pairingKey, string serialKey, List<ExpenseItem> items)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            AddExpenseItemResult result = new AddExpenseItemResult {FunctionName = "SaveExpense", ReturnCode = retCode.ReturnCode};

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        SortedList<int, int> ids = new SortedList<int, int>();
                        PairingKey pairing = new PairingKey(pairingKey);
                        cMobileDevices clsmobile = new cMobileDevices(pairing.AccountID);
                        MobileDevice curDevice = clsmobile.GetDeviceByPairingKey(pairingKey);
                        int employeeid = pairing.EmployeeID;

                        foreach(ExpenseItem item in items)
                        {
                            int id = clsmobile.saveMobileItem(employeeid, item.OtherDetails, item.ReasonID, item.Total, item.SubcatID, item.dtDate, item.CurrencyID, item.Miles, item.Quantity, item.FromLocation, item.ToLocation, item.dtAllowanceStartDate, item.dtAllowanceEndDate, item.AllowanceTypeID, item.AllowanceDeductAmount, item.ItemNotes, curDevice.DeviceType.DeviceTypeId, mobileDeviceID: curDevice.MobileDeviceID, mobileExpenseID: item.MobileID);
                            ids.Add(item.MobileID, id);
                        }
                        result.List = ids;
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.SaveExpense():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// Details of the employee - only shows specific details
        /// </summary>
        /// <returns>Firstname, Surname, PrimaryCurrency (non global currencyID)</returns>
        public EmployeeBasic GetEmployeeBasicDetails(string pairingKey, string serialKey, int employeeID)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            EmployeeBasic emp = new EmployeeBasic {FunctionName = "GetEmployeeBasicDetails", ReturnCode = retCode.ReturnCode};

            if (retCode.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    cEmployees clsEmployees = new cEmployees(AccountID);
                    Employee reqEmployee = clsEmployees.GetEmployeeById(employeeID);

                    emp.firstname = reqEmployee.Forename;
                    emp.surname = reqEmployee.Surname;
                    emp.isApprover = CanCheckAndPay(AccountID, employeeID);
                    emp.primaryCurrency = reqEmployee.PrimaryCurrency;
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("MobileAPI.GetEmployeeBasicDetails():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nEmployeeID: " + employeeID.ToString() + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                    throw ex;
                }
            }

            return emp;
        }

        /// <summary>
        /// Establishes whether the user has a check and pay access role
        /// </summary>
        /// <param name="accountID">Account ID</param>
        /// <param name="employeeID">Employee ID to check</param>
        /// <returns>TRUE if permitted</returns>
        private static bool CanCheckAndPay(int accountID, int employeeID)
        {
            return cAccessRoles.CanCheckAndPay(accountID, employeeID);
        }

        /// <summary>
        /// Pairs a mobile device with the system
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>true/false</returns>
        public ServiceResultMessage PairDevice(string pairingKey, string serialKey)
        {
            ServiceResultMessage tmpSRM = ValidatePairingKey(pairingKey, null);
            var rm = new ServiceResultMessage { FunctionName = "PairDevice", ReturnCode = tmpSRM.ReturnCode };
            if (tmpSRM.ReturnCode == MobileReturnCode.Success)
            {
                var pairing = new PairingKey(pairingKey);
                var clsEmployees = new cEmployees(pairing.AccountID);
                Employee reqEmployee = clsEmployees.GetEmployeeById(pairing.EmployeeID);
                if (reqEmployee.Archived)
                {
                    rm.ReturnCode = MobileReturnCode.EmployeeArchived;
                }
                else
                if (!reqEmployee.Active)
                {
                    rm.ReturnCode = MobileReturnCode.EmployeeNotActivated;
                }
                else
                {
                    var clsMobileDevices = new cMobileDevices(pairing.AccountID);
                rm.ReturnCode = clsMobileDevices.PairMobileDevice(pairing, serialKey);
            }
            }

            rm.Message = (rm.ReturnCode == MobileReturnCode.Success ? "success" : "fail");
            return rm;
        }

        /// <summary>
        /// Gets the screen setup for add/edit expenses
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>AddExpensesScreenDetails class record</returns>
        public AddExpensesScreenDetails GetAddEditExpensesScreenSetup(string pairingKey, string serialKey)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            AddExpensesScreenDetails details = new AddExpensesScreenDetails {FunctionName = "GetAddEditExpensesScreenSetup", ReturnCode = retCode.ReturnCode};

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        SortedList<string, DisplayField> fields = new SortedList<string, DisplayField>();
                        cMisc clsMisc = new cMisc(AccountID);
                        SortedList<string, cFieldToDisplay> screenFields = clsMisc.GetAddScreenFields();

                        foreach (KeyValuePair<string, cFieldToDisplay> kvp in screenFields)
                        {
                            switch(kvp.Key)
                            {
                                case "reason":
                                case "currency":
                                case "otherdetails":
                                    DisplayField df = new DisplayField()
                                                      {
                                                          code = kvp.Value.code,
                                                          //createdby = kvp.Value.createdby,
                                                          //createdon = kvp.Value.createdon,
                                                          description = kvp.Value.description,
                                                          display = kvp.Value.display
                                                          //displaycc = kvp.Value.displaycc,
                                                          //displaypc = kvp.Value.displaypc,
                                                          //fieldid = kvp.Value.fieldid,
                                                          //individual = kvp.Value.individual,
                                                          //mandatory = kvp.Value.mandatory,
                                                          //mandatorycc = kvp.Value.mandatorycc,
                                                          //mandatorypc = kvp.Value.mandatorypc,
                                                          //modifiedby = kvp.Value.modifiedby,
                                                          //modifiedon = kvp.Value.modifiedon
                                                      };
                                    fields.Add(kvp.Key, df);
                                    break;
                            }
                        }
                        details.Fields = fields;
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.GetAddEditExpensesScreenSetup():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }
            return details;
        }

        /// <summary>
        /// A complete list of expense item categories
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>CategoryResult class record</returns>
        public CategoryResult GetExpenseItemCategories(string pairingKey, string serialKey)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            CategoryResult result = new CategoryResult {FunctionName = "GetExpenseItemCategories", ReturnCode = retCode.ReturnCode};

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        List<Category> categories = new List<Category>();

                        cCategories clsCats = new cCategories(AccountID);

                        foreach(cCategory c in clsCats.CachedList())
                        {
                            categories.Add(new Category
                                           {
                                               category = c.category,
                                               categoryid = c.categoryid,
                                               createdby = c.createdby,
                                               createdon = c.createdon,
                                               description = c.description,
                                               modifiedby = c.modifiedby,
                                               modifiedon = c.modifiedon
                                           });
                        }
                        result.List = categories;
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.GetExpenseItemCategories():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// A complete list of reasons
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>ReasonResult class record</returns>
        public ReasonResult GetReasonsList(string pairingKey, string serialKey)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            ReasonResult result = new ReasonResult { FunctionName = "GetReasonsList", ReturnCode = retCode.ReturnCode };

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        List<Reason> reasons = new List<Reason>();

                        cReasons clsReasons = new cReasons(AccountID);
                        foreach(cReason r in clsReasons.CachedList())
                        {
                            reasons.Add(new Reason
                                        {
                                            accountcodenovat = r.accountcodenovat,
                                            accountcodevat = r.accountcodevat,
                                            accountid = r.accountid,
                                            createdby = r.createdby,
                                            createdon = r.createdon,
                                            description = r.description,
                                            modifiedby = r.modifiedby,
                                            modifiedon = r.modifiedon,
                                            reason = r.reason,
                                            reasonid = r.reasonid
                                        });
                        }
                        result.List = reasons;
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.GetReasonsList():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// A complete list of reasons
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>CurrencyResult class record</returns>
        public CurrencyResult GetCurrencyList(string pairingKey, string serialKey)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            CurrencyResult result = new CurrencyResult {FunctionName = "GetCurrencyList", ReturnCode = retCode.ReturnCode};

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        List<Currency> currencies = new List<Currency>();

                        cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
                        cCurrencies clscurrencies = new cCurrencies(AccountID, null);

                        foreach(cCurrency r in clscurrencies.currencyList.Values) // .CachedList()
                        {
                            if (!r.archived)
                            {
                                Currency currency = new Currency {currencyID = r.currencyid};
                                cGlobalCurrency globalcurrency = clsglobalcurrencies.getGlobalCurrencyById(r.globalcurrencyid);
                                if(globalcurrency != null)
                                {
                                    currency.label = globalcurrency.label;
                                    currency.symbol = globalcurrency.symbol;
                                }
                                currencies.Add(currency);
                            }
                        }
                        result.List = currencies;
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.GetCurrencyList():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// General options set on the account (only shows specific options)
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>GeneralOptions class record</returns>
        public GeneralOptions GetGeneralOptions(string pairingKey, string serialKey)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            GeneralOptions gOptions = new GeneralOptions {FunctionName = "GetGeneralOptions", ReturnCode = retCode.ReturnCode};

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(AccountID);
                        cAccountSubAccount reqSubAccount = clsSubAccounts.getFirstSubAccount();
                        cAccountProperties reqProperties;

                        if (reqSubAccount != null)
                        {
                            reqProperties = reqSubAccount.SubAccountProperties;
                        }
                        else
                        {
                            reqProperties = new cAccountProperties();
                        }

                        if (reqProperties.InitialDate != null)
                        {
                            gOptions.InitialDate = ((DateTime)reqProperties.InitialDate).ToString("yyyyMMdd");
                        }
                        gOptions.LimitMonths = reqProperties.LimitMonths;
                        gOptions.FlagDate = reqProperties.FlagDate;
                        gOptions.ApproverDeclaration = reqProperties.ApproverDeclarationMsg;
                        gOptions.ClaimantDeclarationRequired = reqProperties.ClaimantDeclaration;
                        gOptions.ClaimantDeclaration = reqProperties.DeclarationMsg;
                        gOptions.AttachReceipts = reqProperties.AttachReceipts;
                        gOptions.EnableMobileDevices = reqProperties.UseMobileDevices;
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.GetGeneralOptions():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }
            return gOptions;
        }

        /// <summary>
        /// Authenticates whether Pairing Key / Serial Key combination is valid
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>Mobile status code indicating whether authentication valid or not</returns>
        public ServiceResultMessage Authenticate(string pairingKey, string serialKey)
        {
            ServiceResultMessage validateCode = ValidatePairingKey(pairingKey, serialKey);
            ServiceResultMessage authResultMsg = new ServiceResultMessage { FunctionName = "Authenticate", ReturnCode = validateCode.ReturnCode };

            if(validateCode.ReturnCode == MobileReturnCode.Success)
            {
                PairingKey paired = new PairingKey(pairingKey);

                cMobileDevices clsdevices = new cMobileDevices(paired.AccountID);
                MobileReturnCode authenticated = clsdevices.authenticate(pairingKey, serialKey, paired.EmployeeID);
                if(authenticated == MobileReturnCode.Success)
                {
                    AccountID = paired.AccountID;
                }
                else
                {
                    authResultMsg.ReturnCode = authenticated;
                }
            }
            
            return authResultMsg;
        }

        /// <summary>
        /// Uploads mobile device receipt to the database
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <param name="mobileExpenseID">Expense Item ID to associate receipt to</param>
        /// <param name="receipt">Base64 encoded receipt file</param>
        public UploadReceiptResult UploadReceipt(string pairingKey, string serialKey, int mobileExpenseID, string receipt)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            UploadReceiptResult uploadResultMsg = new UploadReceiptResult { ReturnCode = retCode.ReturnCode, FunctionName = "UploadReceipt", MobileID = mobileExpenseID};

            if(retCode.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    byte[] bReceipt = Convert.FromBase64String(receipt);

                    PairingKey pairing = new PairingKey(pairingKey);
                    cMobileDevices clsdevices = new cMobileDevices(pairing.AccountID);
                    clsdevices.saveMobileItemReceipt(mobileExpenseID, bReceipt);
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("MobileAPI.UploadReceipt():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nExpenseID: " + mobileExpenseID.ToString() + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                    throw ex;
                }
            }

            return uploadResultMsg;
        }

        /// <summary>
        /// Retrieves claims awaiting approval for a mobile device user
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>ClaimToCheckResult class record</returns>
        public ClaimToCheckResult GetClaimsAwaitingApproval(string pairingKey, string serialKey)
        {
            //cEventlog.LogEntry("MobileAPI.getClaimsAwaitingApproval():PreAuthenticate:{ pairingKey:\"" + pairingKey + "\", serialKey:\"" + serialKey + "\" }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            //cEventlog.LogEntry("MobileAPI.getClaimsAwaitingApproval():PostAuthenticate:{ result:" + retCode.ToString() + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);
            ClaimToCheckResult result = new ClaimToCheckResult {FunctionName = "GetClaimsAwaitingApproval", ReturnCode = retCode.ReturnCode};
            try
            {
                switch(retCode.ReturnCode)
                {
                    case MobileReturnCode.Success:
                        PairingKey pairing = new PairingKey(pairingKey);
                        result.isApprover = CanCheckAndPay(pairing.AccountID, pairing.EmployeeID);

                        if (!result.isApprover)
                        {
                            result.List = new List<SpendManagementLibrary.Mobile.Claim>();
                            break;
                        }
                        
                        result.List = cClaims.GetClaimsAwaitingApproval(pairing.AccountID, pairing.EmployeeID);
                        //cEventlog.LogEntry("MobileAPI.getClaimsAwaitingApproval():Data:{ Claims.Count: " + claims.Count.ToString() + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        break;
                }
            }
            catch(Exception ex)
            {
                cEventlog.LogEntry("MobileAPI.GetClaimsAwaitingApproval():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Gets the number of claims awaiting approval by a mobile device user
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>ClaimToCheckCountResult class record</returns>
        public ClaimToCheckCountResult GetClaimsAwaitingApprovalCount(string pairingKey, string serialKey)
        {
            //cEventlog.LogEntry("MobileAPI.getClaimsAwaitingApprovalCount():PreAuthenticate:{ pairingKey:\"" + pairingKey + "\", serialKey:\"" + serialKey + "\" }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            //cEventlog.LogEntry("MobileAPI.getClaimsAwaitingApprovalCount():PostAuthenticate:{ result: " + retCode.ToString() + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);
            ClaimToCheckCountResult result = new ClaimToCheckCountResult { FunctionName = "GetClaimsAwaitingApprovalCount", ReturnCode = retCode.ReturnCode };

            try
            {
                switch(retCode.ReturnCode)
                {
                    case MobileReturnCode.Success:
                        PairingKey pairing = new PairingKey(pairingKey);
                        result.isApprover = CanCheckAndPay(pairing.AccountID, pairing.EmployeeID);

                        if (!result.isApprover)
                        {
                            result.Count = 0;
                            break;
                        }
                        
                        cClaims claims = new cClaims(pairing.AccountID);

                        result.Count = claims.getClaimsToCheckCount(pairing.EmployeeID, true, null);
                        //cEventlog.LogEntry("MobileAPI.getClaimsAwaitingApprovalCount():Data:{ Claims.Count: " + count.ToString() + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);
                        break;
                }
            }
            catch(Exception ex)
            {
                cEventlog.LogEntry("MobileAPI.GetClaimsAwaitingApprovalCount():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Gets all expense items associated to a given claim ID
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <param name="claimID">Claim ID to retrieve expense items for</param>
        /// <returns>ExpenseItemsResult class record</returns>
        public ExpenseItemsResult GetExpenseItemsByClaimID(string pairingKey, string serialKey, int claimID)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            ExpenseItemsResult result = new ExpenseItemsResult { FunctionName = "GetExpenseItemsByClaimID", ReturnCode = retCode.ReturnCode };

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        PairingKey pairing = new PairingKey(pairingKey);
                        cSubcats clssubcats = new cSubcats(pairing.AccountID);
                        cClaims clsclaims = new cClaims(pairing.AccountID);
                       svcFlagRules flags = new svcFlagRules();

                        List<cExpenseItemResult> items = new List<cExpenseItemResult>();

                        result.isApprover = CanCheckAndPay(pairing.AccountID, pairing.EmployeeID);

                        cClaim claim = clsclaims.getClaimById(claimID);

                        SortedList<int, cExpenseItem> expenseItems = clsclaims.getExpenseItemsFromDB(claimID);
                        foreach(cExpenseItem item in expenseItems.Values)
                        {
                            if (claim.checkerid == pairing.EmployeeID || (item.itemCheckerId.HasValue && item.itemCheckerId.Value == pairing.EmployeeID))
                            {
                                cExpenseItemResult details = new cExpenseItemResult { ExpenseItem = item };
                                if (item.subcatid > 0)
                                {
                                    details.Subcat = clssubcats.GetSubcatById(item.subcatid);
                                    details.Flags = flags.CreateFlagsGrid(
                                        claimID,
                                        item.expenseid.ToString(),
                                        "CheckAndPay",
                                        pairing.AccountID,
                                        pairing.EmployeeID);
                                    item.flags = new List<FlagSummary>();
                                }

                                items.Add(details);
                            }
                        }

                        result.List = items;
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.getExpenseItemsByClaimID():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nClaimID: " + claimID.ToString() + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// Approves expense items on a claim
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <param name="claimid">Claim ID containing expense items to approve</param>
        /// <param name="lstItems">List of expense item IDs to set to approved</param>
        /// <returns>ServiceResultMessage class record</returns>
        public ServiceResultMessage ApproveItems(string pairingKey, string serialKey, int claimid, List<int> approveItems)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            ServiceResultMessage result = new ServiceResultMessage { FunctionName = "ApproveItems", ReturnCode = retCode.ReturnCode };

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        PairingKey pairing = new PairingKey(pairingKey);
                        
                        if (CanCheckAndPay(pairing.AccountID, pairing.EmployeeID))
                        {
                            cClaims clsclaims = new cClaims(pairing.AccountID);

                            clsclaims.approveItems(approveItems);
                        }
                        else
                        {
                            result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.approveItems():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nClaimID: " + claimid.ToString() + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;

            }

            return result;
        }

        /// <summary>
        /// Unapproves an expense item on a claim
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <param name="claimid">Claim ID containing expense item to unapprove</param>
        /// <param name="expenseid">Expense Item ID to set to unapproved</param>
        /// <returns>ServiceResultMessage class record</returns>
        public ServiceResultMessage UnapproveItem(string pairingKey, string serialKey, int claimid, int expenseid)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            ServiceResultMessage result = new ServiceResultMessage { FunctionName = "UnapproveItem", ReturnCode = retCode.ReturnCode };

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        PairingKey pairing = new PairingKey(pairingKey);
                        if (CanCheckAndPay(pairing.AccountID, pairing.EmployeeID))
                        {
                            cClaims clsclaims = new cClaims(pairing.AccountID);
                            
                           
                                cExpenseItem expItem = clsclaims.getExpenseItemById(expenseid);
                                if (expItem != null)
                            {
                                clsclaims.UnapproveItem(expItem);
                            }
                            
                        }
                        else
                        {
                            result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                        }
                        //clsclaims.unApproveItem(claimid, expenseid);
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.unapproveItem():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nClaimID: " + claimid.ToString() + "\nExpenseID: " + expenseid.ToString() + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Sets a claim as allocated for payment
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <param name="claimid">Claim ID to set as allocate for payment</param>
        /// <returns>ServiceResultMessage class record</returns>
        public ServiceResultMessage AllocateClaimForPayment(string pairingKey, string serialKey, int claimid)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            ServiceResultMessage result = new ServiceResultMessage { FunctionName = "AllocateClaimForPayment", ReturnCode = retCode.ReturnCode };

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        PairingKey pairing = new PairingKey(pairingKey);
                        if (CanCheckAndPay(pairing.AccountID, pairing.EmployeeID))
                        {
                            cClaims clsclaims = new cClaims(pairing.AccountID);
                            cClaim claim = clsclaims.getClaimById(claimid);

                            if (clsclaims.AllowClaimProgression(claimid) == 2)
                            {
                                clsclaims.payClaim(claim, pairing.EmployeeID, null);
                            }
                            else
                            {
                                result.Message = "-1";
                                result.ReturnCode = MobileReturnCode.AllocateForPaymentFailed;
                            }
                        }
                        else
                        {
                            result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.AllocateClaimForPayment():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nClaimID: " + claimid.ToString() + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// Returns items on a claim during check and pay
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <param name="claimid">Claim ID containing expense items for returning</param>
        /// <param name="reason">Reason provided to return of expense items</param>
        /// <param name="lstItems">List of expense item IDs being returned</param>
        /// <returns>ServiceResultMessage class record</returns>
        public ServiceResultMessage ReturnItems(string pairingKey, string serialKey, int claimid, string reason, List<int> lstItems)
        {

            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            ServiceResultMessage result = new ServiceResultMessage { FunctionName = "ReturnItems", ReturnCode = retCode.ReturnCode };

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        PairingKey pairing = new PairingKey(pairingKey);
                        if (CanCheckAndPay(pairing.AccountID, pairing.EmployeeID))
                        {
                            cClaims clsclaims = new cClaims(pairing.AccountID);
                            cClaim curClaim = clsclaims.getClaimById(claimid);
                            

                            clsclaims.ReturnExpenses(curClaim, lstItems, reason, pairing.EmployeeID, null);
                        }
                        else
                        {
                            result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.returnItems():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nClaimID: " + claimid.ToString() + "\nReason: " + reason + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// Approves a claim via mobile device request
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <param name="claimid">Claim ID to approve</param>
        /// <returns>ServiceResultMessage class record</returns>
        public ServiceResultMessage ApproveClaim(string pairingKey, string serialKey, int claimid)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            ServiceResultMessage result = new ServiceResultMessage { FunctionName = "ApproveClaim", ReturnCode = retCode.ReturnCode };

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        PairingKey pairing = new PairingKey(pairingKey);
                        if (CanCheckAndPay(pairing.AccountID, pairing.EmployeeID))
                        {
                            cClaims clsclaims = new cClaims(pairing.AccountID);
                            cClaim reqclaim = clsclaims.getClaimById(claimid);

                            if (clsclaims.AllowClaimProgression(claimid) == 1)
                            {
                                if (reqclaim != null && reqclaim.NumberOfUnapprovedItems > 0)
                                {
                                    result.Message = "1";
                                }

                                // store the return code and check, rather than potentially sending the claim to the next stage multiple times
                                int nextStageRC = 0;
                                var claimSubmission = new ClaimSubmission(pairing);
                                SubmitClaimResult claimResult;
                                claimResult = claimSubmission.SendClaimToNextStage(reqclaim, false, 0, pairing.EmployeeID, null);
                                switch (claimResult.Reason)
                                {
                                    case SubmitRejectionReason.NoLineManager:
                                        result.Message = "2";
                                        break;
                                    case SubmitRejectionReason.AssignmentSupervisorNotSpecified:
                                        result.Message = "5";
                                        break;
                                    case SubmitRejectionReason.CostCodeOwnerNotSpecified:
                                        result.Message = "6";
                                        break;
                                    case SubmitRejectionReason.UserNotAllowedToApproveOwnClaim:
                                        result.Message = "3";
                                        break;
                                    case SubmitRejectionReason.UserNotAllowedToApproveOwnClaimDespiteSignoffGroup:
                                        result.Message = "4";
                                        break;
                                }
                            }
                            else
                            {
                                result.Message = "-1";
                                result.ReturnCode = MobileReturnCode.ApproveClaimFailed;
                            }
                        }
                        else
                        {
                            result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                            result.Message = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.approveClaim():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nClaimID: " + claimid.ToString() + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// Retrieve the current API version details for a particular platform
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <param name="typeKey">Indicates API type that version is being requested for</param>
        public VersionResult GetCurrentVersion(string pairingKey, string serialKey, string typeKey)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            VersionResult result = new VersionResult { FunctionName = "GetCurrentVersion", ReturnCode = retCode.ReturnCode };

            if (retCode.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    cMobileAPIVersions versions = new cMobileAPIVersions();
                    cMobileAPIVersion reqVersion = versions.GetVersionByTypeKey(typeKey);

                    if(reqVersion != null)
                    {
                        result.ApiType = reqVersion.APIType.TypeKey;
                        result.AppStoreURL = reqVersion.AppStoreURL;
                        result.DisableAppUsage = reqVersion.DisableUsage;
                        result.NotifyMessage = reqVersion.NotifyMessage;
                        result.SyncMessage = reqVersion.SyncMessage;
                        result.Title = reqVersion.Title;
                        result.VersionNumber = reqVersion.VersionNumber;
                    }
                    else
                    {
                        result.ReturnCode = MobileReturnCode.InvalidApiVersionType;
                    }
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("MobileAPI.GetCurrentVersion():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nTypeKey: " + typeKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                    throw ex;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets an expense sub-category record by its ID
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <param name="subcatid">Sub-category ID to retrieve</param>
        /// <returns>cSubcatResult class record</returns>
        public SubcatItemResult GetSubcatByID(string pairingKey, string serialKey, int subcatid)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);

            SubcatItemResult result = new SubcatItemResult { FunctionName = "GetSubcatByID", ReturnCode = retCode.ReturnCode };

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        PairingKey pairing = new PairingKey(pairingKey);
                        cSubcats clssubcats = new cSubcats(pairing.AccountID);
                        result.subcat = clssubcats.GetSubcatById(subcatid);
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.GetSubcatByID():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nSubCatID: " + subcatid.ToString() + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets an expense item by its ID
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <param name="claimid">Claim ID containing expense item</param>
        /// <param name="expenseid">Expense ID to retrieve</param>
        /// <returns>ClaimItemResult class record</returns>
        public ClaimItemResult GetExpenseItemByID(string pairingKey, string serialKey, int claimid, int expenseid)
        {
            ServiceResultMessage retCode = Authenticate(pairingKey, serialKey);
            ClaimItemResult result = new ClaimItemResult { FunctionName = "GetExpenseItemByID", ReturnCode = retCode.ReturnCode };

            switch(retCode.ReturnCode)
            {
                case MobileReturnCode.Success:
                    try
                    {
                        PairingKey pairing = new PairingKey(pairingKey);
                        cClaims clsclaims = new cClaims(pairing.AccountID);
                        


                            cExpenseItem item = clsclaims.getExpenseItemById(expenseid);
                            if (item != null)
                        {
                            result.Item = item;
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("MobileAPI.GetExpenseItemByID():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nClaimID: " + claimid.ToString() + "\nExpenseID: " + expenseid.ToString() + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                        throw ex;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// Validates if the current pairing key is correctly associated to an account
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>ServiceResultMessage class record</returns>
        public ServiceResultMessage ValidatePairingKey(string pairingKey, string serialKey)
        {
            PairingKey pKey = new PairingKey(pairingKey);
            ServiceResultMessage result = new ServiceResultMessage {FunctionName = "ValidatePairingKey", ReturnCode = MobileReturnCode.Success};
            if(!pKey.PairingKeyValid)
            {
                result.ReturnCode = MobileReturnCode.PairingKeyFormatInvalid;
            }
            else
            {
                cAccounts accs = new cAccounts();
                cAccount curAccount = accs.GetAccountByID(pKey.AccountID);
                if(curAccount == null)
                {
                    result.ReturnCode = MobileReturnCode.AccountInvalid;
                }
                else if(curAccount.archived)
                {
                    result.ReturnCode = MobileReturnCode.AccountArchived;
                }
                else
                {
                    cAccountSubAccounts subaccs = new cAccountSubAccounts(pKey.AccountID);
                    cAccountProperties clsProperties = subaccs.getFirstSubAccount().SubAccountProperties;

                    if(!clsProperties.UseMobileDevices)
                    {
                        result.ReturnCode = MobileReturnCode.MobileDevicesDisabled;
                    }
                    else
                    {
                        cMobileDevices mobileDevices = new cMobileDevices(pKey.AccountID);
                        MobileDevice device = mobileDevices.GetDeviceByPairingKey(pKey.Pairingkey);

                        if(device == null)
                        {
                            result.ReturnCode = MobileReturnCode.PairingKeyNotFound;
                        }
                        else
                        {
                            if(!string.IsNullOrEmpty(serialKey) && device.SerialKey != serialKey)
                            {
                                result.ReturnCode = MobileReturnCode.PairingKeySerialKeyMismatch;
                            }
                        }
                    }

                    if (result.ReturnCode == MobileReturnCode.Success)
                    {
                        // check employee access role
                        cEmployees employees = new cEmployees(pKey.AccountID);
                        Employee employee = employees.GetEmployeeById(pKey.EmployeeID);
                        cAccessRoles clsRoles = new cAccessRoles(pKey.AccountID, cAccounts.getConnectionString(pKey.AccountID));
                        Dictionary<int, List<int>> subaccRoles = employee.GetAccessRoles().AllAccessRoles;
                        bool roleFound = false;

                        if (subaccRoles.Count == 0)
                        {
                            result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                        }
                        else
                        {
                            foreach(KeyValuePair<int, List<int>> kvp in subaccRoles)
                            {
                                if(roleFound)
                                    break;

                                List<int> empRoles = kvp.Value;

                                if(empRoles.Count == 0)
                                {
                                    result.ReturnCode = MobileReturnCode.EmployeeHasInsufficientPermissions;
                                }
                                else
                                {
                                    MobileReturnCode tmpCode = MobileReturnCode.EmployeeHasInsufficientPermissions;

                                    foreach(int roleId in empRoles)
                                    {
                                        cAccessRole curRole = clsRoles.GetAccessRoleByID(roleId);
                                        if(curRole.AllowMobileAccess)
                                        {
                                            tmpCode = MobileReturnCode.Success;
                                            roleFound = true;
                                            break;
                                        }
                                    }

                                    result.ReturnCode = tmpCode;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves the Company Policy in either HTML or Plain Text
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>CompanyPolicyResult class object</returns>
        public CompanyPolicyResult GetCompanyPolicy(string pairingKey, string serialKey)
        {
            ServiceResultMessage returnCode = Authenticate(pairingKey, serialKey);
            CompanyPolicyResult result = new CompanyPolicyResult { FunctionName = "GetCompanyPolicy", ReturnCode = returnCode.ReturnCode };

            if (returnCode.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    PairingKey pairing = new PairingKey(pairingKey);
                    cAccountSubAccounts subaccs = new cAccountSubAccounts(pairing.AccountID);
                    cAccountProperties clsproperties = subaccs.getFirstSubAccount().SubAccountProperties;

                    switch(clsproperties.PolicyType)
                    {
                        case 1:
                            result.isHTML = false;
                            result.CompanyPolicy = clsproperties.CompanyPolicy;

                            break;
                        case 2:
                            result.isHTML = true;

                            cAccounts clsaccounts = new cAccounts();
                            cAccount reqaccount = clsaccounts.GetAccountByID(pairing.AccountID);
                            string policyPath = clsaccounts.GetFilePaths(pairing.AccountID, FilePathType.PolicyFile);
                            string fullPath = Path.Combine(policyPath, reqaccount.companyid + ".htm");

                            try
                            {
                                StringBuilder sb = new StringBuilder();
                                Uri fileUrl = new Uri(fullPath);

                                FileWebRequest myFileWebRequest = (FileWebRequest) WebRequest.CreateDefault(fileUrl);
                                FileWebResponse myFileWebResponse = (FileWebResponse) myFileWebRequest.GetResponse();

                                Stream receiveStream = myFileWebResponse.GetResponseStream();
                                Encoding encode = Encoding.GetEncoding("utf-8");

                                if(receiveStream != null)
                                {
                                    StreamReader readStream = new StreamReader(receiveStream, encode);

                                    Char[] read = new Char[256];
                                    // Read 256 characters at a time.    
                                    int count = readStream.Read(read, 0, 256);

                                    while(count > 0)
                                    {
                                        // Dump the 256 characters on a string and display the string onto the console.
                                        String str = new String(read, 0, count);
                                        sb.Append(str);
                                        count = readStream.Read(read, 0, 256);
                                    }
                                    // Release resources of stream object.
                                    readStream.Close();
                                }
                                // Release resources of response object.
                                myFileWebResponse.Close();

                                result.CompanyPolicy = sb.ToString();
                            }
                            catch(Exception ex)
                            {
                                result.CompanyPolicy = "Company Policy Not Found.";
                                result.Message = "file not found";

                                throw ex;
                            }
                            break;
                    }
                }
                catch(Exception ex)
                {
                    cEventlog.LogEntry("MobileAPI.GetCompanyPolicy():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                    throw ex;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a list of allowance types used for daily allowances
        /// </summary>
        /// <param name="pairingKey">Pairing key of the mobile device</param>
        /// <param name="serialKey">Serial key of the mobile device hardware</param>
        /// <returns>AllowanceResult class object</returns>
        public AllowanceTypesResult GetAllowanceTypes(string pairingKey, string serialKey)
        {
            ServiceResultMessage returnCode = Authenticate(pairingKey, serialKey);
            AllowanceTypesResult result = new AllowanceTypesResult { FunctionName = "GetAllowanceTypes", ReturnCode = returnCode.ReturnCode };

            if (returnCode.ReturnCode == MobileReturnCode.Success)
            {
                try
                {
                    PairingKey pairing = new PairingKey(pairingKey);
                    cAllowances allowances = new cAllowances(pairing.AccountID);

                    result.AllowanceTypes = (from x in allowances.sortList().Values
                                             select new cAPIAllowance { AllowanceID = x.allowanceid, Allowance = x.allowance, Description = x.description }).ToList();
                }
                catch (Exception ex)
                {
                    cEventlog.LogEntry("MobileAPI.GetAllowanceTypes():Error:{ Pairingkey: " + pairingKey + "\nSerialKey: " + serialKey + "\nMessage: " + ex.Message + " }", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);

                    throw ex;
                }
            }

            return result;
        }

        /// <summary>
        /// Validator to be used for the mobile api wcf service. This is an override of the UserNamePasswordValidator
        /// </summary>
        public class MobileAPIUserNamePasswordValidator : UserNamePasswordValidator
        {

            /// <summary>
            /// This override of the validate method allows us to pass in company id and username in the username field, rather than just username.
            /// </summary>
            /// <param name="userName">company id and username</param>
            /// <param name="password"></param>
            public override void Validate(string userName, string password)
            {
                string[] userNameSplit = userName.Split('-');
                if(string.IsNullOrEmpty(userNameSplit[0]) || string.IsNullOrEmpty(userNameSplit[1]) || string.IsNullOrEmpty(password))
                    throw new SecurityTokenException("Username and password required");
                EncryptorFactory.SetCurrent(new HashEncryptorFactory());
                if(!AuthenticateDetails(userNameSplit[0].Trim(), userNameSplit[1].Trim(), password.Trim(), EncryptorFactory.CreateEncryptor()))
                {
                    throw new SecurityTokenException("Username and password required");
                }
                else
                {
                    throw new SecurityTokenException("Unknown Username or Password");
                }
            }

            /// <summary>
            /// This is a cut down version of Authenticate taken from the shared logon process, just to get things rolling. return true or false
            /// rather than the LoginResult enum.
            /// </summary>
            /// <param name="CompanyName">Company ID</param>
            /// <param name="Username">Usermame</param>
            /// <param name="Password">Password</param>
            /// <param name="encryptor">An instance of <see cref="IEncryptor"/></param>
            /// <returns>TRUE if user successfully authenticated, otherwise FALSE</returns>
            private bool AuthenticateDetails(string CompanyName, string Username, string Password, IEncryptor encryptor)
            {
                cAccounts clsAccounts = new cAccounts();
                cAccount reqAccount = clsAccounts.GetAccountByCompanyID(CompanyName);

                if(reqAccount == null || reqAccount.archived)
                {
                    return false;
                }

                cEmployees clsEmployees = new cEmployees(reqAccount.accountid);
                AuthenicationOutcome authOutcome = clsEmployees.Authenticate(Username, Password, AccessRequestType.Mobile, encryptor);
             
                if (authOutcome.employeeId <= 0)
                {
                    return false;
                }

                Employee reqEmployee = clsEmployees.GetEmployeeById(Math.Abs(authOutcome.employeeId));

                if(reqEmployee.DefaultSubAccount == -1 || reqEmployee.Archived || !reqEmployee.Active || string.IsNullOrEmpty(Password))
                {
                    return false;
                }

                return true;
            }
        }
    }
}
