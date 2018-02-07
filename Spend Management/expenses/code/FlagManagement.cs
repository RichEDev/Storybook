
namespace Spend_Management.expenses.code
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Text;
    using System.Web.UI;

    using Microsoft.SqlServer.Server;

    using SpendManagementHelpers.TreeControl;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Flags;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using Spend_Management.shared.code.Helpers;

    using Utilities.DistributedCaching;

    using Convert = System.Convert;

    /// <summary>
    /// The management class for flags and exceptions.
    /// </summary>
    public class FlagManagement
    {
        /// <summary>
        /// The cache area.
        /// </summary>
        public const string CacheArea = "flags";

        /// <summary>
        /// The caching object .
        /// </summary>
        private readonly Cache caching = new Cache();

        /// <summary>
        /// The collection of all flags in the system.
        /// </summary>
        private Dictionary<int, Flag> lstFlags;

        /// <summary>
        /// A private instance of <see cref="Addresses"/>
        /// </summary>
        private Addresses _addresses;

        /// <summary>
        /// Initialises a new instance of the <see cref="FlagManagement"/> class. 
        /// Initialises the flag management class
        /// </summary>
        /// <param name="accountid">
        /// The account ID of the current user
        /// </param>
        public FlagManagement(int accountid)
        {
            this.AccountID = accountid;
            this.InitialiseData();
        }
        #region properties

        /// <summary>
        /// Gets the Account ID of the current user
        /// </summary>
        public int AccountID { get; private set; }

        /// <summary>
        /// Gets or sets a list of basic subcat information referenced when checking items for flags
        /// </summary>
        private Dictionary<int, SubcatBasic> _CachedSubcats { get; set; }

        private Dictionary<int, cGlobalCurrency> _CachedGlobalCurrencies { get; set; }

        private Dictionary<int, cRoleSubcat> _Rolesubcats { get; set; }

        private Dictionary<int, List<int>> _CachedRoles { get; set; }

        private cClaim Claim { get; set; }
        #endregion

        /// <summary>
        /// Returns an instance  of a flag
        /// </summary>
        /// <param name="flagID">
        /// The ID of the flag to be retrieved
        /// </param>
        /// <returns>
        /// The flag object for the given ID
        /// </returns>
        public Flag GetBy(int flagID)
        {
            Flag flag;
            if (this.lstFlags == null)
            {
                this.InitialiseData();
            }

            this.lstFlags.TryGetValue(flagID, out flag);
            return flag;
        }

        /// <summary>
        /// Saves the item roles selected by the user
        /// </summary>
        /// <param name="flagId">
        /// The ID of the flag the roles should be associated with
        /// </param>
        /// <param name="roles">
        /// A list of role IDs to save
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SaveItemRoles(int flagId, List<int> roles, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            int returnvalue;
            Flag flag = this.GetBy(flagId);
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagID", flagId);
                databaseConnection.sqlexecute.Parameters.AddWithValue(
                    "@performItemRoleExpenseCheck",
                    Convert.ToByte(!flag.AllowMultipleFlagsOfThisType));
                databaseConnection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;
                databaseConnection.sqlexecute.Parameters.Add("@roleIDs", SqlDbType.Structured);
                List<SqlDataRecord> integers = new List<SqlDataRecord>();
                SqlMetaData[] rowMetaData = { new SqlMetaData("c1", SqlDbType.Int) };
                foreach (int i in roles)
                {
                    var row = new SqlDataRecord(rowMetaData);
                    row.SetInt32(0, i);
                    integers.Add(row);
                }

                databaseConnection.sqlexecute.Parameters["@roleIDs"].Value = integers;
                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                databaseConnection.ExecuteProc("saveFlagRuleRole");
                returnvalue = (int)databaseConnection.sqlexecute.Parameters["@returnValue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            if (returnvalue == 0)
            {
                this.InvalidateCache();
                this.InitialiseData();
            }

            return returnvalue;
        }

        /// <summary>
        /// Saves the expense items selected by the user
        /// </summary>
        /// <param name="flagId">
        /// The ID of the flag the expense items should be associated with
        /// </param>
        /// <param name="expenseitems">
        /// The expenseitems.
        /// </param>
        /// <param name="connection">Database connection</param>
        public int SaveFlagRuleExpenseItems(int flagId, List<int> expenseitems, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            int returnValue = 0;
            Flag flag = this.GetBy(flagId);

            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagID", flagId);
                databaseConnection.sqlexecute.Parameters.AddWithValue(
                    "@performItemRoleExpenseCheck",
                    Convert.ToByte(!flag.AllowMultipleFlagsOfThisType));
                databaseConnection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
                databaseConnection.sqlexecute.Parameters.Add("@subcatIDs", SqlDbType.Structured);
                List<SqlDataRecord> integers = new List<SqlDataRecord>();
                SqlMetaData[] rowMetaData = { new SqlMetaData("c1", SqlDbType.Int) };
                foreach (int i in expenseitems)
                {
                    var row = new SqlDataRecord(rowMetaData);
                    row.SetInt32(0, i);
                    integers.Add(row);
                }

                databaseConnection.sqlexecute.Parameters["@subcatIDs"].Value = integers;

                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                databaseConnection.ExecuteProc("saveFlagRuleExpenseItem");
                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnvalue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            if (returnValue == 0)
            {
                this.InvalidateCache();
                this.InitialiseData();
            }

            return returnValue;
        }

        /// <summary>
        /// Validates an expense item to check for all possible flags
        /// </summary>
        /// <param name="item">
        /// The expense item to validate
        /// </param>
        /// <param name="employeeId">
        /// The ID of the employee who saved the expense item
        /// </param>
        /// <param name="validationPoint">
        /// The validation Point.
        /// </param>
        /// <param name="validationType">
        /// The validation Type.
        /// </param>
        /// <param name="subAccount">
        /// The sub Account.
        /// </param>
        /// <param name="user">
        /// The current user
        /// </param>
        /// <param name="claim">
        /// An optional parameter for if the claim object for the expense item is already created
        /// </param>
        /// <returns>
        /// A list of flags that apply to this item
        /// </returns>
        public List<FlagSummary> CheckItemForFlags(
            cExpenseItem item,
            int employeeId,
            ValidationPoint validationPoint,
            ValidationType validationType,
            cAccountSubAccount subAccount,
            ICurrentUser user,
            cClaim claim = null)
        {
            var reqEmployee = new cEmployees(user.AccountID).GetEmployeeById(employeeId);
            return this.CheckItemForFlags(item, reqEmployee, validationPoint, validationType, subAccount, user, claim);
        }

        /// <summary>
        /// Validates an expense item to check for all possible flags
        /// </summary>
        /// <param name="item">
        /// The expense item to validate
        /// </param>
        /// <param name="reqEmployee">
        /// The employee who saved the expense item
        /// </param>
        /// <param name="validationPoint">
        /// The validation Point.
        /// </param>
        /// <param name="validationType">
        /// The validation Type.
        /// </param>
        /// <param name="subAccount">
        /// The sub Account.
        /// </param>
        /// <param name="user">
        /// The current user.
        /// </param>
        /// <param name="claim">
        /// An optional parameter for if the claim object for the expense item is already created
        /// </param>
        /// <returns>
        /// A list of flags that apply to this item
        /// </returns>
        public List<FlagSummary> CheckItemForFlags(cExpenseItem item, Employee reqEmployee, ValidationPoint validationPoint, ValidationType validationType, cAccountSubAccount subAccount, ICurrentUser user, cClaim claim = null)
        {
            // get the flags that apply to this item

            List<FlagSummary> lstResults = new List<FlagSummary>();


            if (this._Rolesubcats == null)
            {
                this._Rolesubcats = this.GetRoleSubcats(reqEmployee);
            }

            List<int> roles = this.GetRoles(item.subcatid);

            IEnumerable<Flag> flags = this.GetFlagsByExpenseItemAndRole(item.subcatid, roles);

            cAccountProperties properties = subAccount.SubAccountProperties;
            cCurrencies currencies = new cCurrencies(this.AccountID, null);
            cGlobalCurrencies globalCurrencies = new cGlobalCurrencies();

            if (claim == null)
            {
                claim = this.GetClaim(item.claimid);
            }
            else
            {
                this.Claim = claim;
            }

            foreach (Flag flag in flags)
            {
                if (flag.Active
                        && (validationType == ValidationType.Any
                        || (validationType == ValidationType.DoesNotRequireSave && !flag.RequiresSaveToValidate)
                        || (validationType == ValidationType.RequiresSave && flag.RequiresSaveToValidate))
                        && ((validationPoint == ValidationPoint.SubmitClaim && (flag.RequiresRevalidationOnClaimSubmittal ||  flag.Action == FlagAction.BlockItem || flag.ClaimantJustificationRequired))
                        || (validationPoint == ValidationPoint.AddExpense && (flag.ValidateWhenAddingAnExpense || claim.submitted))))
                {
                    List<FlaggedItem> flagResult = null;

                    SubcatBasic subcat = null;
                    switch (flag.FlagType)
                    {
                        case FlagType.LimitWithoutReceipt:
                        case FlagType.LimitWithReceipt:
                            flagResult = this.CheckLimits((LimitFlag)flag, item, this._Rolesubcats, reqEmployee);
                            break;
                        case FlagType.ItemNotReimbursable:

                            subcat = this.GetSubcat(item.subcatid);

                            ((NonReimbursableFlag)flag).Reimbursable = subcat.Reimbursable;
                            flagResult = flag.Validate(item, reqEmployee.EmployeeID, properties);
                            break;
                        case FlagType.ItemReimbursable:
                            subcat = this.GetSubcat(item.subcatid);
                            ((ReimbursableFlag)flag).Reimbursable = subcat.CalculationType == CalculationType.ItemReimburse;
                            flagResult = flag.Validate(item, reqEmployee.EmployeeID, properties);
                            break;
                        case FlagType.HomeToLocationGreater:
                            if (item.expenseid > 0 && (item.journeysteps == null || item.journeysteps.Any() == false))
                            {
                                item.journeysteps = new cExpenseItems(this.AccountID).GetJourneySteps(item.expenseid);
                            }

                            flagResult = this.CheckHomeToLocationMileage(flag, item, reqEmployee.EmployeeID, subAccount, user);
                            break;
                        case FlagType.MileageExceeded:
                            if (item.expenseid > 0 && (item.journeysteps == null || item.journeysteps.Any() == false))
                            {
                                item.journeysteps = new cExpenseItems(this.AccountID).GetJourneySteps(item.expenseid);
                            }

                            flagResult = this.CheckMileage((MileageFlag)flag, item, reqEmployee.EmployeeID);
                            break;
                        case FlagType.NumberOfPassengersLimit:
                            if (item.expenseid > 0 && (item.journeysteps == null || item.journeysteps.Any() == false))
                            {
                                item.journeysteps = new cExpenseItems(this.AccountID).GetJourneySteps(item.expenseid);
                            }

                            flagResult = flag.Validate(item, reqEmployee.EmployeeID, properties);
                            break;
                        case FlagType.JourneyDoesNotStartAndFinishAtHomeOrOffice:
                            bool isMilageItem = new cSubcats(this.AccountID).GetSubcatById(item.subcatid).mileageapp;

                            if (item.expenseid > 0 && (item.journeysteps == null || item.journeysteps.Any() == false))
                            {
                                item.journeysteps = new cExpenseItems(this.AccountID).GetJourneySteps(item.expenseid);
                            }

                            if (isMilageItem && item.journeysteps.Count > 0 && item.journeysteps.ToList().Select(keyvaluepair => keyvaluepair.Value).Select(location => location.startlocation).ToList()[0] != null)
                            {
                                bool validStartLocation = false, validEndLocation = false;
                                var employee = new cEmployees(this.AccountID).GetEmployeeById(claim.employeeid);
                                var employeeHomeAddress = employee.GetHomeAddresses().GetBy(item.date);

                                var employeeWorkLocationId = this.GetEmployeeWorkLocationId(item, user, employee);

                                var expenseStartLocationId = item.journeysteps.FirstOrDefault().Value.startlocation.Identifier;
                                var expenseEndLocationId = item.journeysteps.LastOrDefault().Value.endlocation.Identifier;

                                Address homeAddress = null; 
                                Address expenseStartAddress = null; 
                                var employeeHomeLocationId = employeeHomeAddress?.LocationID ?? 0;

                                if (employeeHomeLocationId > 0)
                                {
                                    homeAddress = Address.Get(this.AccountID, employeeHomeLocationId);
                                }

                                if (expenseStartLocationId > 0)
                                {
                                    expenseStartAddress = Address.Get(this.AccountID, expenseStartLocationId);
                                }

                                var employeeHomePostcode = string.Empty;
                                var journeyStartLocationPostCode = string.Empty;

                                if (!string.IsNullOrWhiteSpace(homeAddress?.Postcode))
                                {
                                    employeeHomePostcode = homeAddress.Postcode.RemoveWhiteSpaceAndChangeToLowerCase();
                                }

                                if (!string.IsNullOrWhiteSpace(expenseStartAddress?.Postcode))
                                {
                                    journeyStartLocationPostCode = expenseStartAddress.Postcode.RemoveWhiteSpaceAndChangeToLowerCase();
                                }

                                Address workAddress = null;
                                Address expenseEndAddress = null;

                                if (employeeWorkLocationId > 0)
                                {
                                    workAddress = Address.Get(this.AccountID, employeeWorkLocationId);
                                }

                                if (expenseEndLocationId > 0)
                                {
                                    expenseEndAddress = Address.Get(this.AccountID, expenseEndLocationId);
                                }
                              
                                var employeeWorkLocationPostcode = string.Empty;
                                var journeyEndLocationPostCode = string.Empty;

                                if (!string.IsNullOrWhiteSpace(workAddress?.Postcode))
                                {
                                    employeeWorkLocationPostcode = workAddress.Postcode.RemoveWhiteSpaceAndChangeToLowerCase();
                                }

                                if (!string.IsNullOrWhiteSpace(expenseEndAddress?.Postcode))
                                {
                                    journeyEndLocationPostCode = expenseEndAddress.Postcode.RemoveWhiteSpaceAndChangeToLowerCase();
                                }
                                
                                if (!string.IsNullOrWhiteSpace(employeeHomePostcode) && !string.IsNullOrWhiteSpace(employeeWorkLocationPostcode))
                                {
                                    validStartLocation = (journeyStartLocationPostCode == employeeHomePostcode) || (journeyStartLocationPostCode == employeeWorkLocationPostcode);
                                    if (validStartLocation)
                                    {
                                        validEndLocation = (journeyEndLocationPostCode == employeeHomePostcode) || (journeyEndLocationPostCode == employeeWorkLocationPostcode);
                                    }
                                }
                                else if (!string.IsNullOrWhiteSpace(employeeHomePostcode) && string.IsNullOrWhiteSpace(employeeWorkLocationPostcode) && (journeyStartLocationPostCode == journeyEndLocationPostCode))
                                {
                                    validStartLocation = (employeeHomePostcode == journeyStartLocationPostCode);
                                    if (validStartLocation)
                                    {
                                        validEndLocation = true;
                                    }
                                }
                                else if(string.IsNullOrWhiteSpace(employeeHomePostcode) && !string.IsNullOrWhiteSpace(employeeWorkLocationPostcode) &&(journeyStartLocationPostCode == journeyEndLocationPostCode))
                                {
                                    validStartLocation = (employeeWorkLocationPostcode == journeyStartLocationPostCode);
                                    if(validStartLocation)
                                    {
                                        validEndLocation = true;
                                    }
                                }

                                if (!validStartLocation || !validEndLocation)
                                {
                                    flagResult = flag.Validate(item, reqEmployee.EmployeeID, properties);
                                }
                            }
                            break;
                        default:
                            flagResult = flag.Validate(item, reqEmployee.EmployeeID, properties);
                            break;
                    }

                    if (flagResult != null)
                    {

                        foreach (FlaggedItem flaggedItem in flagResult)
                        {
                            if (flaggedItem != null)
                            {
                                flaggedItem.ExpenseDate = item.date;
                                flaggedItem.ExpenseTotal = item.total;

                                cCurrency currency = currencies.getCurrencyById(item.basecurrency);
                                cGlobalCurrency globalCurrency = null;
                                if (currency != null)
                                {
                                    globalCurrency = this.GetGlobalCurrency(currency.globalcurrencyid);

                                    if (globalCurrency != null)
                                    {
                                        flaggedItem.ExpenseCurrencySymbol = globalCurrency.symbol;
                                    }
                                }

                                subcat = this.GetSubcat(item.subcatid);
                                if (subcat != null)
                                {
                                    flaggedItem.ExpenseSubcat = subcat.Subcat;
                                }

                                flaggedItem.FlagDescription = globalCurrencies.ReplaceCurrencySymbol(
                                    this.AccountID,
                                    currency,
                                    globalCurrency,
                                    flaggedItem.FlagDescription);
                                lstResults.Add(new FlagSummary(flaggedItem.FlagId, flaggedItem));
                            }
                        }
                    }
                }
            }

            return lstResults;
        }

        /// <summary>
        /// Get the Work address ID based on <see cref="cExpenseItem"/> Date, WorkAddressId and Esr Assignment Id
        /// </summary>
        /// <param name="item">An instance of <see cref="cExpenseItem"/> to get an Id for</param>
        /// <param name="user">An instacne of <see cref="ICurrentUser"/>which represents the curernt logged on user</param>
        /// <param name="employee">An instance of <see cref="Employee"/>whio is the owner of the claim</param>
        /// <returns>The ID of the Employees Work Address.</returns>
        private int GetEmployeeWorkLocationId(cExpenseItem item, ICurrentUser user, Employee employee)
        {
            cEmployeeWorkLocation employeeWorkAddress = null;
            if (item.WorkAddressId > 0)
            {
                return item.WorkAddressId;
            }

            if (item.ESRAssignmentId > 0)
            {
                employeeWorkAddress = employee.GetWorkAddresses().GetBy(user, item.date, (int) item.ESRAssignmentId);
            }
            else
            {
                employeeWorkAddress = employee.GetWorkAddresses().GetBy(item.date);
            }

            var employeeWorkLocationId = employeeWorkAddress?.LocationID ?? 0;
            return employeeWorkLocationId;
        }

        /// <summary>
        /// Saves justifications provided by the claimant for the given flagged item id.
        /// </summary>
        /// <param name="flaggedItemId">
        /// The flagged item id.
        /// </param>
        /// <param name="justification">
        /// The justification.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public void SaveClaimantJustification(int flaggedItemId, string justification, ICurrentUser currentUser, IDBConnection connection = null)
        {
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {

                databaseConnection.sqlexecute.Parameters.AddWithValue("@flaggedItemid", flaggedItemId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@claimantJustification", justification);
                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue(
                        "@delegateID",
                        currentUser.Delegate.EmployeeID);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
                databaseConnection.ExecuteProc("saveClaimantFlagJustification");
                databaseConnection.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// The save authoriser justification.
        /// </summary>
        /// <param name="flaggedItemId">
        /// The flagged item id.
        /// </param>
        /// <param name="justification">
        /// The justification.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public void SaveAuthoriserJustification(int flaggedItemId, string justification, int employeeID, ICurrentUser currentUser, IDBConnection connection = null)
        {
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flaggedItemid", flaggedItemId);
                if (justification == string.Empty)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@justification", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@justification", justification);
                }
                databaseConnection.sqlexecute.Parameters.AddWithValue("@authoriser", employeeID);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue(
                        "@delegateID",
                        currentUser.Delegate.EmployeeID);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                databaseConnection.ExecuteProc("saveAuthoriserFlagJustification");
                databaseConnection.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// The get flag by type role and expense item.
        /// </summary>
        /// <param name="flagType">
        /// The flag type.
        /// </param>
        /// <param name="roleId">
        /// The role id.
        /// </param>
        /// <param name="subcatId">
        /// The subcat id.
        /// </param>
        /// <returns>
        /// The <see cref="Flag"/>.
        /// </returns>
        public Flag GetFlagByTypeRoleAndExpenseItem(FlagType flagType, int roleId, int subcatId)
        {
            return this.lstFlags.Values.FirstOrDefault(flag => flag.FlagType == flagType && flag.Active && (flag.ExpenseItemInclusionType == FlagInclusionType.All || (flag.AssociatedExpenseItems.FirstOrDefault(associatedExpense => associatedExpense.SubcatID == subcatId) != null)) && (flag.ItemRoleInclusionType == FlagInclusionType.All || flag.AssociatedItemRoles.Contains(roleId)));
        }

        /// <summary>
        /// Checks the item to see if a mileage flag is applicable
        /// </summary>
        /// <param name="flag">
        /// The flag to check
        /// </param>
        /// <param name="item">
        /// The expense item to check
        /// </param>
        /// <param name="employeeid">
        /// The id of the employee creating the flag
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// A list of flagged items for each step a flag has been created for
        /// </returns>
        public List<FlaggedItem> CheckMileage(
            MileageFlag flag,
            cExpenseItem item,
            int employeeid,
            IDBConnection connection = null)
        {
            List<FlaggedItem> items = new List<FlaggedItem>();

            CurrentUser currentUser = cMisc.GetCurrentUser();
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
            cAccount account = new cAccounts().GetAccountByID(currentUser.AccountID);
            cAccountProperties subAccountProperties =
                clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
            cEmployeeCars cars = new cEmployeeCars(this.AccountID, employeeid);
            cCar car = cars.GetCarByID(item.carid);
            cMileagecats clsmileagecats = new cMileagecats(this.AccountID);
            string unit = string.Empty;
            if (car != null)
            {
                unit = car.defaultuom == MileageUOM.KM ? "Kilometres" : "Miles";
            }

            MileageFlaggedItem flaggedItem = null;
            foreach (cJourneyStep step in item.journeysteps.Values)
            {
                if (step.startlocation != null && step.endlocation != null)
                {
                    decimal? recommendedmileage = AddressDistance.GetRecommendedDistance(
                        currentUser,
                        step.startlocation,
                        step.endlocation,
                        subAccountProperties.MileageCalcType,
                        account.MapsEnabled);
                    if (car == null)
                    {
                        break;
                    }

                    decimal numActualMiles = step.NumActualMiles;
                    if (car.defaultuom == MileageUOM.KM)
                    {
                        recommendedmileage = clsmileagecats.convertMilesToKM(Convert.ToDecimal(recommendedmileage));
                        numActualMiles = clsmileagecats.convertMilesToKM(Convert.ToDecimal(numActualMiles));
                    }


                    decimal exceeded = 0;

                    if (recommendedmileage.HasValue)
                    {
                        exceeded = numActualMiles - recommendedmileage.Value;
                    }

                    if (exceeded > 0)
                    {
                        string comment = "The recommended distance between " + step.startlocation.FriendlyName
                                         + " and " + step.endlocation.FriendlyName + " has been exceeded by "
                                         + exceeded + " " + unit + ".";

                        FlagColour flagColour = flag.CheckTolerance(step.NumActualMiles, recommendedmileage.Value);
                        if (flagColour != FlagColour.None)
                        {
                            if (flaggedItem == null)
                            {
                                flaggedItem = new MileageFlaggedItem(
                                    string.Empty,
                                    flag.CustomFlagText,
                                    flag,
                                    flagColour, flag.FlagTypeDescription, flag.NotesForAuthoriser,
                                    flag.AssociatedExpenseItems, flag.Action, flag.CustomFlagText,
                                    flag.ClaimantJustificationRequired, false);
                                items.Add(flaggedItem);
                            }

                            flaggedItem.AddFlaggedJourneyStep(step.stepnumber + 1, exceeded, comment, 0);
                            // falls in to no flag tolerance so don't flag
                        }
                    }
                }
            }


            return items;
        }

        /// <summary>
        /// Determines if any flags no longer exist when editing an expense and deletes them
        /// </summary>
        /// <param name="olditem">The expense item to check</param>
        /// <param name="flags">The new flags to compare</param>
        public void CheckExpenseForRedundantItems(cExpenseItem olditem, ref List<FlagSummary> flags)
        {
            // delete the flags that nolonger apply
            List<int> flagsToDelete = new List<int>();

            foreach (FlagSummary flaggedItem in olditem.flags)
            {
                if (flags.FirstOrDefault(flag => flag.FlagID == flaggedItem.FlagID) == null)
                {
                    if (flaggedItem.FlaggedItem.GetType() == typeof(MileageFlaggedItem))
                    {
                        MileageFlaggedItem mileageFlaggedItem = (MileageFlaggedItem)flaggedItem.FlaggedItem;
                        flagsToDelete.AddRange(mileageFlaggedItem.FlaggedJourneySteps.Select(journeystep => journeystep.FlaggedItemID));
                    }
                    else
                    {
                        flagsToDelete.Add(flaggedItem.FlaggedItem.FlaggedItemId);
                    }

                }
                else if (flaggedItem.FlaggedItem.GetType() == typeof(MileageFlaggedItem))
                {
                    MileageFlaggedItem mileageFlaggedItem = (MileageFlaggedItem)flaggedItem.FlaggedItem;
                    MileageFlaggedItem newMileageItem = (MileageFlaggedItem)flags.First(flag => flag.FlagID == flaggedItem.FlagID).FlaggedItem;
                    foreach (JourneyStepFlaggedItem journeystep in mileageFlaggedItem.FlaggedJourneySteps)
                    {
                        bool foundStep = false;
                        foreach (JourneyStepFlaggedItem newjourneystep in newMileageItem.FlaggedJourneySteps)
                        {

                            if (newjourneystep.StepNumber == journeystep.StepNumber)
                            {
                                foundStep = true;
                                break;
                            }
                        }

                        if (!foundStep)
                        {
                            flagsToDelete.Add(journeystep.FlaggedItemID);
                        }
                    }
                }
            }

            if (flagsToDelete.Count > 0)
            {
                DeleteFlaggedItems(olditem.expenseid, flagsToDelete);
            }
        }

        /// <summary>
        /// Checks a mileage item to see if the home to location mileage exceeds the office to location mileage and a flag applies
        /// </summary>
        /// <param name="flag">The flag rule to check</param>
        /// <param name="item">The expense item to validate</param>
        /// <param name="employeeid">The ID of the employee who saved the item</param>
        /// <returns>A flag result containing the flag and the custom flag text</returns>
        public List<FlaggedItem> CheckHomeToLocationMileage(Flag flag, cExpenseItem item, int employeeid, cAccountSubAccount subAccount, ICurrentUser currentUser)
        {
            FlaggedItem flagResult = null;
            string comment = string.Empty;
            cEmployeeCars clsEmployeeCars = new cEmployeeCars(this.AccountID, employeeid);
            cCar car = clsEmployeeCars.GetCarByID(item.carid);
            cMileagecats clsmileagecats = new cMileagecats(this.AccountID);
            SubcatBasic subcat = this.GetSubcat(item.subcatid);

            SortedList<int, Address> addresses = new SortedList<int, Address>();
            List<FlaggedItem> flaggedItems = new List<FlaggedItem>();
            MileageFlaggedItem flaggedItem = null;
            bool international = currentUser.Account.AddressInternationalLookupsAndCoordinates;
            AddressLookupProvider provider = currentUser.Account.AddressLookupProvider;
            EmployeeHomeAddresses homeAddresses = new EmployeeHomeAddresses(this.AccountID, employeeid);
            EmployeeWorkAddresses workAddresses = new EmployeeWorkAddresses(this.AccountID, employeeid);
            if (subcat.EnableHomeToLocationMileage
                && subcat.HomeToLocationType == HomeToLocationType.FlagHomeAndOfficeToLocationDiff
                && !car.ExemptFromHomeToOffice)
            {
                if (item.journeysteps.Count > 0)
                {
                    decimal diff = 0;

                    foreach (cJourneyStep step in item.journeysteps.Values)
                    {
                        comment = string.Empty;

                        if (step.startlocation != null && step.endlocation != null)
                        {

                            var homeAddress = homeAddresses.GetBy(item.date);
                            bool startIsHome = false;
                            bool endIsHome = false;

                            if (homeAddress != null)
                            {
                                if (step.startlocation.Identifier == homeAddress.LocationID)
                                {
                                    startIsHome = true;
                                }
                                else if (step.endlocation.Identifier == homeAddress.LocationID)
                                {
                                    endIsHome = true;
                                }
                                else
                                {
                                    if (!addresses.ContainsKey(homeAddress.LocationID))
                                    {
                                        addresses.Add(homeAddress.LocationID, Address.Get(this.AccountID, homeAddress.LocationID));
                                    }

                                    if (addresses[homeAddress.LocationID] != null)
                                    {
                                        if (step.startlocation.GetLocatorType(international, provider) == Address.LocatorType.Postcode && step.startlocation.Postcode.Replace(" ", string.Empty).Equals(addresses[homeAddress.LocationID].Postcode.Replace(" ", string.Empty), StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            startIsHome = true;
                                        }
                                        else if (step.endlocation.GetLocatorType(international, provider) == Address.LocatorType.Postcode && step.endlocation.Postcode.Replace(" ", string.Empty).Equals(addresses[homeAddress.LocationID].Postcode.Replace(" ", string.Empty), StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            endIsHome = true;
                                        }
                                    }
                                }
                            }

                            var esrAssignmentLocation = new EsrAssignmentLocations(this.AccountID).GetEsrAssignmentLocationById((int)item.ESRAssignmentId, item.date);
                            long? esrLocationId = (esrAssignmentLocation != null) ? (long?)esrAssignmentLocation.EsrLocationId : null;

                            if (startIsHome)
                            {
                                decimal hometolocationdist = step.NumActualMiles;
                                var workAddress = workAddresses.GetBy(currentUser, item.date, (int?)esrLocationId);
                                decimal officetolocationdist = AddressDistance.GetRecommendedOrCustomDistance(this._addresses.GetAddressById(workAddress.LocationID), step.endlocation, this.AccountID, subAccount, currentUser) ?? 0;

                                if (hometolocationdist > 0 && officetolocationdist > 0)
                                {
                                    if (car.defaultuom == MileageUOM.KM)
                                    {
                                        hometolocationdist = clsmileagecats.convertMilesToKM(hometolocationdist);
                                        officetolocationdist = clsmileagecats.convertMilesToKM(officetolocationdist);
                                    }

                                    if (hometolocationdist > officetolocationdist)
                                    {
                                        diff = hometolocationdist - officetolocationdist;
                                        comment = "The home to location distance is " + diff + " "
                                                  + car.defaultuom
                                                  + "s greater than the distance from the office to the location on step " + (step.stepnumber + 1) + ".";
                                    }
                                }
                            }
                            else if (endIsHome)
                            {
                                decimal hometolocationdist = step.NumActualMiles;
                                var workAddress = workAddresses.GetBy(currentUser, item.date, (int?)esrLocationId);
                                decimal officetolocationdist = AddressDistance.GetRecommendedOrCustomDistance(this._addresses.GetAddressById(workAddress.LocationID), step.startlocation, this.AccountID, subAccount, currentUser) ?? 0;

                                if (hometolocationdist > 0 && officetolocationdist > 0)
                                {
                                    if (car.defaultuom == MileageUOM.KM)
                                    {
                                        hometolocationdist = clsmileagecats.convertMilesToKM(hometolocationdist);
                                        officetolocationdist = clsmileagecats.convertMilesToKM(officetolocationdist);
                                    }

                                    if (hometolocationdist > officetolocationdist)
                                    {
                                        diff = hometolocationdist - officetolocationdist;
                                        comment = "The home to location distance is " + diff + " "
                                                  + car.defaultuom
                                                  + "s greater than the distance from the office to the location on step " + (step.stepnumber + 1) + ".";
                                    }
                                }
                            }
                        }

                        if (comment != string.Empty)
                        {
                            if (flaggedItem == null)
                            {
                                flaggedItem = new MileageFlaggedItem(
                                    string.Empty,
                                    flag.CustomFlagText,
                                    flag,
                                        flag.FlagLevel, flag.FlagTypeDescription, flag.NotesForAuthoriser, flag.AssociatedExpenseItems, flag.Action, flag.CustomFlagText, flag.ClaimantJustificationRequired, false);
                            }

                            flaggedItem.AddFlaggedJourneyStep(
                                step.stepnumber + 1,
                                diff,
                                comment,
                                0);
                        }
                    }
                }
            }

            flaggedItems.Add(flaggedItem);


            return flaggedItems;
        }

        /// <summary>
        /// Flag the expense with the flags provided
        /// </summary>
        /// <param name="expenseid">The id of the expense to flag</param>
        /// <param name="flags">The list of flags to create</param>
        public void FlagItem(int expenseid, IEnumerable<FlagSummary> flags)
        {
            foreach (FlagSummary summary in flags)
            {
                FlaggedItem item = summary.FlaggedItem;
                if (item.GetType() == typeof(MileageFlaggedItem))
                {
                    foreach (JourneyStepFlaggedItem flaggedStep in ((MileageFlaggedItem)item).FlaggedJourneySteps)
                    {
                        item.FlaggedItemId = this.FlagItem(
                        expenseid,
                        item.FlagId,
                        item.Flagtype,
                        flaggedStep.FlagDescription,
                        item.FlagText,
                        item.FlagColour,
                        null,
                        flaggedStep.StepNumber,
                        flaggedStep.ExceededAmount,
                        item.AssociatedExpenses);
                        flaggedStep.FlaggedItemID = item.FlaggedItemId;
                    }
                }
                else if (item.GetType() == typeof(DuplicateFlaggedItem))
                {
                    item.FlaggedItemId = this.FlagItem(
                        expenseid,
                        item.FlagId,
                        item.Flagtype,
                        item.FlagDescription,
                        item.FlagText,
                        item.FlagColour,
                        ((DuplicateFlaggedItem)item).DuplicateExpenseID,
                        null,
                        null,
                        item.AssociatedExpenses);
                }
                else if (item.GetType() == typeof(LimitFlaggedItem))
                {
                    item.FlaggedItemId = this.FlagItem(
                        expenseid,
                        item.FlagId,
                        item.Flagtype,
                        item.FlagDescription,
                        item.FlagText,
                        item.FlagColour,
                        null,
                        null,
                        ((LimitFlaggedItem)item).ExceededLimit,
                        item.AssociatedExpenses);
                }
                else
                {
                    item.FlaggedItemId = this.FlagItem(
                        expenseid,
                        item.FlagId,
                        item.Flagtype,
                        item.FlagDescription,
                        item.FlagText,
                        item.FlagColour,
                        null,
                        null,
                        null,
                        item.AssociatedExpenses);
                }

            }
        }

        /// <summary>
        /// The delete flagged items.
        /// </summary>
        /// <param name="expenseId">
        /// The expense id.
        /// </param>
        /// <param name="flaggedItems">
        /// The flagged items.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public void DeleteFlaggedItems(int expenseId, List<int> flaggedItems, IDBConnection connection = null)
        {
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@expenseid", expenseId);
                databaseConnection.sqlexecute.Parameters.Add("@flaggedItems", SqlDbType.Structured);
                List<SqlDataRecord> integers = new List<SqlDataRecord>();
                SqlMetaData[] rowMetaData = { new SqlMetaData("c1", SqlDbType.Int) };
                foreach (int i in flaggedItems)
                {
                    var row = new SqlDataRecord(rowMetaData);
                    row.SetInt32(0, i);
                    integers.Add(row);
                }

                databaseConnection.sqlexecute.Parameters["@flaggedItems"].Value = integers;
                databaseConnection.ExecuteProc("deleteFlaggedItems");
                databaseConnection.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// Returns the SQL used to create the flag management summary grid
        /// </summary>
        /// <returns>The SQL for the flag grid</returns>
        public string CreateGrid()
        {
            return "select flagID, flagType, description, active, [action] from dbo.flags";
        }

        /// <summary>
        /// Deletes a flag rule that exists in the system
        /// </summary>
        /// <param name="flagId">
        /// The ID of the flag rule to delete
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteFlagRule(int flagId, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            int returnvalue;
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagID", flagId);
                databaseConnection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                databaseConnection.ExecuteProc("deleteFlagRule");
                returnvalue = (int)databaseConnection.sqlexecute.Parameters["@returnvalue"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            this.InvalidateCache();
            return returnvalue;
        }

        /// <summary>
        /// Deletes an associate item role from the flag
        /// </summary>
        /// <param name="flagId">
        /// The ID of the flag rule to delete
        /// </param>
        /// <param name="itemRoleID">
        /// The item Role ID.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public void DeleteAssociatedItemRole(int flagId, int itemRoleID, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagID", flagId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@roleID", itemRoleID);
                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                databaseConnection.ExecuteProc("deleteFlagAssociatedItemRole");
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            this.InvalidateCache();
        }

        /// <summary>
        /// Deletes an associate expense item from the flag
        /// </summary>
        /// <param name="flagId">
        /// The ID of the flag rule to delete
        /// </param>
        /// <param name="subcatID">
        /// The ID of the expense item to delete
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public void DeleteAssociatedExpenseItem(int flagId, int subcatID, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagID", flagId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@subcatID", subcatID);
                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                databaseConnection.ExecuteProc("deleteFlagAssociatedExpenseItem");
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            this.InvalidateCache();
        }

        /// <summary>
        /// Determines whether a list of flagged items includes an item that should be blocked not flagged
        /// </summary>
        /// <param name="flags">The list of flagged items to check</param>
        /// <returns>True if a blocked item exists in the list</returns>
        public bool ContainsBlockedItem(List<FlagSummary> flags)
        {
            return (from flag in flags select this.GetBy(flag.FlagID)).Any(clsflag => clsflag.Action == FlagAction.BlockItem);
        }

        /// <summary>
        /// Returns a javascript array of the flags to show when displaying blocked items to claimant
        /// </summary>
        /// <param name="flags"> The flags to convert to javascript array</param>
        /// <returns>Javascript array of flags</returns>
        public string CreateClientSideFlags(List<FlaggedItem> flags)
        {
            StringBuilder output = new StringBuilder();
            output.Append("var flags = new Array();\n");
            foreach (FlaggedItem flag in from flag in flags let flagrule = this.GetBy(flag.FlagId) where flagrule.Action == FlagAction.BlockItem select flag)
            {
                output.Append("flags.push(" + (byte)flag.Flagtype + "," + flag.FlagId + ",'" + flag.FlagText + "',");
                if (flag.GetType() == typeof(DuplicateFlaggedItem))
                {
                    output.Append("null");
                }
                else
                {
                    output.Append(((DuplicateFlaggedItem)flag).DuplicateExpenseID);
                }

                output.Append(");\n");
            }

            return output.ToString();
        }

        /// <summary>
        /// Saves the fields associated with a duplicate flag.
        /// </summary>
        /// <param name="flagId">
        /// The flag id.
        /// </param>
        /// <param name="fields">
        /// The fields.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public void SaveFields(int flagId, List<Guid> fields, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagID", flagId);
                databaseConnection.sqlexecute.Parameters.Add("@fieldID", SqlDbType.UniqueIdentifier);
                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                foreach (Guid i in fields)
                {
                    databaseConnection.sqlexecute.Parameters["@fieldID"].Value = i;
                    databaseConnection.ExecuteProc("saveFlagRuleField");
                }

                databaseConnection.sqlexecute.Parameters.Clear();
            }

            this.InvalidateCache();
        }

        /// <summary>
        /// Gets a list of expense ids assoicated to an expense to reevaluate after deletion
        /// </summary>
        /// <param name="expenseid">The expense id to get the associated ids of</param>
        /// <param name="connection">The DB Connection</param>
        /// <returns></returns>
        public List<int> GetSavedExpensesRequiringRevalidationAfterDelete(int expenseid, IDBConnection connection = null)
        {
            List<int> expenseids = new List<int>();
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@expenseID", expenseid);
                using (
                    IDataReader reader =
                        databaseConnection.GetReader("GetSavedExpensesRequiringRevalidationAfterDelete", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        expenseids.Add(reader.GetInt32(0));
                    }

                    reader.Close();
                }

                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return expenseids;
        }

        /// <summary>
        /// Reevaluates the supplied list of expenses after an expense has been deleted
        /// </summary>
        /// <param name="expenseIDs">
        /// The ides of the expenses to reevaluate
        /// </param>
        /// <param name="employeeId">
        /// The id of the employee
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        public void RevalidateAfterDelete(List<int> expenseIDs, int employeeId, ICurrentUser user)
        {
            cClaims claims = new cClaims(this.AccountID);
            List<FlagSummary> flaggedItems;
            cAccountSubAccounts subAccounts = new cAccountSubAccounts(this.AccountID);
            cAccountSubAccount subAccount = subAccounts.getSubAccountById(user.CurrentSubAccountId);
            SortedList<int, cExpenseItem> items = claims.getExpenseItemsFromDB(expenseIDs);
            cExpenseItems expenseItems = new cExpenseItems(this.AccountID);
            FlaggedItemsManager allExistingFlags = expenseItems.GetFlaggedItems(expenseIDs);

            foreach (cExpenseItem item in items.Values)
            {
                flaggedItems = this.CheckItemForFlags(
                    item,
                    employeeId,
                    ValidationPoint.SubmitClaim,
                    ValidationType.Any,
                    subAccount,
                    user);
                ExpenseItemFlagSummary oldFlags;

                if (allExistingFlags.TryGetValue(item.expenseid, out oldFlags))
                {
                    item.flags = oldFlags.FlagCollection;
                }

                if (flaggedItems.Count > 0)
                {

                    this.FlagItem(item.expenseid, flaggedItems);
                }

                if (item.flags != null)
                {
                    this.CheckExpenseForRedundantItems(item, ref flaggedItems);
                }
            }
        }

        /// <summary>
        /// Validate an existing claim to see if new flags are applicable
        /// </summary>
        /// <param name="claimEmployeeId">The owner id of the claim</param>
        /// <param name="hasBlockedItems">
        /// The has Blocked Items.
        /// </param>
        /// <param name="claimId">The id of the claim</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public FlaggedItemsManager RevalidateClaim(int claimId, int claimEmployeeId, out bool hasBlockedItems, ICurrentUser user)
        {
            cAccountSubAccounts subAccounts = new cAccountSubAccounts(this.AccountID);
            cAccountSubAccount subAccount = subAccounts.getSubAccountById(user.CurrentSubAccountId);
            hasBlockedItems = false;
            cClaims claims = new cClaims(this.AccountID);

            // TODO this boolean being set to true is WRONG, but this currently mimics production, perhaps we should be comparing the the claim.employeeid to the user.employeeid to fix this.
            bool claimant = true;

            cExpenseItems expenseItems = new cExpenseItems(this.AccountID);
            SortedList<int, cExpenseItem> items = claims.getExpenseItemsFromDB(claimId);


            FlaggedItemsManager allItems = new FlaggedItemsManager();
            FlaggedItemsManager allExistingFlags = expenseItems.GetFlaggedItems(items.Keys.ToList());
            List<FlagSummary> flaggedItems;
            List<int> expenseIds = new List<int>();

            foreach (cExpenseItem item in items.Values)
            {
                expenseIds.Add(item.expenseid);
                ExpenseItemFlagSummary oldFlags;

                if (allExistingFlags.TryGetValue(item.expenseid, out oldFlags))
                {
                    item.flags = oldFlags.FlagCollection;
                }


                flaggedItems = this.CheckItemForFlags(item, claimEmployeeId, ValidationPoint.SubmitClaim, ValidationType.Any, subAccount, user);

                if (flaggedItems.Count > 0)
                {
                    this.FlagItem(item.expenseid, flaggedItems);
                    allItems.Add(new ExpenseItemFlagSummary(item.expenseid, flaggedItems));
                }

                //put existing flags in to all items so they dont' get lost
                if (oldFlags != null)
                {
                    foreach (FlagSummary flagSummary in oldFlags.FlagCollection)
                    {
                        if (flagSummary.FlaggedItem.Action == FlagAction.FlagItem && !flagSummary.FlaggedItem.RequiresRevalidationOnClaimSubmittal)
                        {
                            flaggedItems.Add(flagSummary);
                        }
                    }
                }

                if (oldFlags != null)
                {
                    this.CheckExpenseForRedundantItems(item, ref flaggedItems);
                }
            }

            FlaggedItemsManager finalResults = new FlaggedItemsManager(claimant, false, true, false, "ClaimViewer");
            allExistingFlags = expenseItems.GetFlaggedItems(expenseIds);

            // identify the ones that now cause a block and remove the ones that have already been identified
            foreach (ExpenseItemFlagSummary kv in allItems.List)
            {

                flaggedItems = kv.FlagCollection;
                ExpenseItemFlagSummary existingFlags;
                allExistingFlags.TryGetValue(kv.ExpenseID, out existingFlags);

                // go through each flagged item and remove if it's one that's already there
                Flag flag;
                ExpenseItemFlagSummary finalFlags;
                if (existingFlags != null)
                {
                    foreach (FlagSummary flagKv in flaggedItems)
                    {
                        finalResults.TryGetValue(kv.ExpenseID, out finalFlags);
                        if (finalFlags == null)
                        {
                            finalFlags = new ExpenseItemFlagSummary(kv.ExpenseID);
                            finalResults.Add(finalFlags);
                        }

                        FlagSummary summary;
                        FlaggedItem fi;
                        existingFlags.TryGetValue(flagKv.FlagID, out summary);
                        if (summary != null && summary.FlaggedItem != null && finalFlags.FlagCollection.All(f => f.FlagID != flagKv.FlagID))
                        {
                            fi = summary.FlaggedItem;
                            // check for blocks
                            flag = this.GetBy(flagKv.FlagID);
                            if (flag.Action == FlagAction.BlockItem || (flag.ClaimantJustificationRequired && string.IsNullOrEmpty(fi.ClaimantJustification)))
                            {
                                hasBlockedItems = true;
                                if (flag.Action == FlagAction.BlockItem)
                                {
                                    flagKv.FlaggedItem.FailureReason = FlagFailureReason.Blocked;
                                }
                                else if (flag.ClaimantJustificationRequired && fi.ClaimantJustification == string.Empty)
                                {
                                    flagKv.FlaggedItem.FailureReason = FlagFailureReason.ClaimantJustificationRequired;
                                }

                                finalFlags.Add(flagKv.FlagID, flagKv.FlaggedItem);
                            }
                            else if (!hasBlockedItems)
                            {
                                // see what else exists and remove if already been flagged. don't bother if we already know we need to show the claimant blocked items
                                finalFlags.Add(flagKv.FlagID, flagKv.FlaggedItem);
                            }
                        }
                    }
                }
                else
                {
                    finalFlags = new ExpenseItemFlagSummary(kv.ExpenseID);

                    // all new so need to add them
                    foreach (FlagSummary flagSummary in flaggedItems)
                    {
                        flag = this.GetBy(flagSummary.FlagID);
                        if (flag.Action == FlagAction.BlockItem || (flag.ClaimantJustificationRequired && string.IsNullOrEmpty(flagSummary.FlaggedItem.ClaimantJustification)))
                        {
                            hasBlockedItems = true;
                            if (flag.Action == FlagAction.BlockItem)
                            {
                                flagSummary.FlaggedItem.FailureReason = FlagFailureReason.Blocked;
                            }
                            else if (flag.ClaimantJustificationRequired && flagSummary.FlaggedItem.ClaimantJustification != null && flagSummary.FlaggedItem.ClaimantJustification == string.Empty)
                            {
                                flagSummary.FlaggedItem.FailureReason = FlagFailureReason.ClaimantJustificationRequired;
                            }

                            finalFlags.Add(flag.FlagID, flagSummary.FlaggedItem);
                        }
                        else if (!hasBlockedItems)
                        {
                            finalFlags.Add(flag.FlagID, flagSummary.FlaggedItem);
                        }
                    }

                    finalResults.Add(finalFlags);
                }
            }

            foreach (ExpenseItemFlagSummary kv in finalResults.List)
            {
                for (int i = kv.FlagCollection.Count - 1; i >= 0; i--)
                {
                    if (kv.FlagCollection[i].FlaggedItem.FailureReason == FlagFailureReason.None)
                    {
                        kv.FlagCollection.RemoveAt(i);
                    }
                }
            }

            finalResults.RemoveEmptyFlags();

            return finalResults;
        }

        /// <summary>
        /// See if the authoriser has provided justifications for all flags it's required on
        /// </summary>
        /// <param name="claimId">The id of the claim to check</param>
        /// <returns>The flag summary HTML to be displayed to the claimant</returns>
        public FlaggedItemsManager CheckJustificationsHaveBeenProvidedByAuthoriser(int claimId)
        {
            FlaggedItemsManager manager = new FlaggedItemsManager();
            cClaims claims = new cClaims(this.AccountID);
            cClaim claim = this.GetClaim(claimId);
            cEmployees employees = new cEmployees(this.AccountID);
            Employee employee = employees.GetEmployeeById(claim.employeeid);

            cGroups groups = new cGroups(this.AccountID);
            int groupId;

            // determine the group
            if (claim.HasPurchaseCardItems && employee.PurchaseCardSignOffGroup > 0)
            {
                groupId = employee.PurchaseCardSignOffGroup;
            }
            else if (claim.HasCreditCardItems && employee.CreditCardSignOffGroup > 0)
            {
                groupId = employee.CreditCardSignOffGroup;
            }
            else
            {
                groupId = employee.SignOffGroupID;
            }

            cGroup group = groups.GetGroupById(groupId);
            if (group == null)
            {
                return new FlaggedItemsManager();
            }

            bool needsToJustify = false;
            foreach (cStage stage in @group.stages.Values.Where(stage => stage.stage == claim.stage))
            {
                if (stage.ApproverJustificationsRequired)
                {
                    needsToJustify = true;
                }

                break;
            }

            if (!needsToJustify)
            {
                return new FlaggedItemsManager();
            }

            cExpenseItems expenseItems = new cExpenseItems(this.AccountID);

            SortedList<int, cExpenseItem> items = claims.getExpenseItemsFromDB(claimId);
            List<int> expenseIds = items.Values.Select(expenseItem => expenseItem.expenseid).ToList();

            FlaggedItemsManager allExistingFlags = expenseItems.GetFlaggedItems(expenseIds);

            FlaggedItemsManager finalFlags = new FlaggedItemsManager(false, true, false, false, "CheckAndPay");
            foreach (ExpenseItemFlagSummary kv in allExistingFlags.List)
            {
                foreach (FlagSummary flagSummary in kv.FlagCollection)
                {
                    Flag flag = this.GetBy(flagSummary.FlagID);
                    if (flag.ApproverJustificationRequired)
                    {
                        bool justificationProvided = flagSummary.FlaggedItem.AuthoriserJustifications.Any(justification => justification.Stage == claim.stage && justification.Justification != string.Empty);

                        if (!justificationProvided)
                        {
                            ExpenseItemFlagSummary itemFlags;
                            finalFlags.TryGetValue(kv.ExpenseID, out itemFlags);
                            if (itemFlags == null)
                            {
                                itemFlags = new ExpenseItemFlagSummary(kv.ExpenseID);

                                finalFlags.Add(itemFlags);
                            }

                            itemFlags.Add(flagSummary.FlagID, flagSummary.FlaggedItem);
                        }
                    }
                }
            }

            return finalFlags;
        }

        /// <summary>
        /// The create associated expenses grid.
        /// </summary>
        /// <param name="associatedExpenses">
        /// The associated expenses.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string CreateAssociatedExpensesGrid(
            IEnumerable<int> associatedExpenses,
            IDBConnection connection = null)
        {

            cTables tables = new cTables(this.AccountID);
            cFields fields = new cFields(this.AccountID);
            cTable basetable = tables.GetTableByName("savedexpenses");
            cTable claimsTable = tables.GetTableByName("claims");
            cTable subcatsTable = tables.GetTableByName("subcats");
            List<cNewGridColumn> columns = new List<cNewGridColumn>();

            columns.Add(new cFieldColumn(fields.GetFieldByTableAndFieldName(basetable.TableID, "expenseid")));
            columns.Add(new cFieldColumn(fields.GetFieldByTableAndFieldName(claimsTable.TableID, "name")));
            string datelabel = fields.GetFieldByTableAndFieldName(basetable.TableID, "date").Description;
            string refnumlabel = fields.GetFieldByTableAndFieldName(basetable.TableID, "refnum").Description;
            string subcatlabel = fields.GetFieldByTableAndFieldName(subcatsTable.TableID, "subcat").Description;
            string totallabel = fields.GetFieldByTableAndFieldName(basetable.TableID, "total").Description;
            string claimlabel = fields.GetFieldByTableAndFieldName(claimsTable.TableID, "name").Description;

            StringBuilder output = new StringBuilder();

            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {

                databaseConnection.sqlexecute.Parameters.Add("@associatedExpenses", SqlDbType.Structured);
                List<SqlDataRecord> integers = new List<SqlDataRecord>();
                SqlMetaData[] rowMetaData = { new SqlMetaData("c1", SqlDbType.Int) };
                foreach (int i in associatedExpenses)
                {
                    var row = new SqlDataRecord(rowMetaData);
                    row.SetInt32(0, i);
                    integers.Add(row);
                }

                databaseConnection.sqlexecute.Parameters["@associatedExpenses"].Value = integers;

                using (
                    IDataReader reader =
                        databaseConnection.GetReader("GetFlagAssociatedExpenses", CommandType.StoredProcedure))
                {
                    int nameOrd = reader.GetOrdinal("name");
                    int dateOrd = reader.GetOrdinal("date");
                    int totalOrd = reader.GetOrdinal("total");
                    int subcatOrd = reader.GetOrdinal("subcat");
                    int refnumOrd = reader.GetOrdinal("refnum");
                    int symbolOrd = reader.GetOrdinal("currencySymbol");
                    while (reader.Read())
                    {
                        string claimName = reader.GetString(nameOrd);
                        DateTime date = reader.GetDateTime(dateOrd);
                        string subcat = reader.GetString(subcatOrd);
                        string refnum = reader.GetString(refnumOrd);
                        string symbol = reader.GetString(symbolOrd);
                        decimal total = reader.GetDecimal(totalOrd);

                        output.Append("<div class=\"tableContainer\">");
                        output.Append("<table>");
                        output.Append(
                            "<tr><td class=\"title\">" + claimlabel
                            + "</td><td>" + claimName + "</td></tr>");
                        output.Append(
                            "<tr><td class=\"title\">" + datelabel
                            + "</td><td>" + date.ToShortDateString()
                            + "</td></tr>");
                        output.Append(
                            "<tr><td class=\"title\">" + subcatlabel
                            + "</td><td>" + subcat + "</td></tr>");
                        output.Append(
                            "<tr><td class=\"title\">" + refnumlabel
                            + "</td><td>" + refnum + "</td></tr>");
                        output.Append(
                            "<tr><td class=\"title\">" + totallabel
                            + "</td><td>"
                            + total.ToString(symbol + "###,###,##0.00") + "</td></tr>");
                        output.Append("</table>");
                        output.Append("</div>");
                    }

                    reader.Close();
                    databaseConnection.sqlexecute.Parameters.Clear();
                }
            }

            return output.ToString();
        }

        /// <summary>
        /// Saves a flag against the specified expense item
        /// </summary>
        /// <param name="expenseid">
        /// The ID of the expense item to flag
        /// </param>
        /// <param name="flaggedItem">
        /// The flagged Item.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int FlagItem(int expenseid, int flagID, FlagType FlagType, string FlagDescription, string FlagText, FlagColour FlagColour, int? duplicateExpenseID, int? stepNumber, decimal? exceededLimit, List<AssociatedExpense> AssociatedExpenses, IDBConnection connection = null)
        {
            int flaggeditemid;


            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.Add("@flaggeditemid", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@flaggeditemid"].Direction = ParameterDirection.ReturnValue;
                databaseConnection.sqlexecute.Parameters.AddWithValue("@expenseid", expenseid);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagID", flagID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagType", (byte)FlagType);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagDescription", FlagDescription);
                if (FlagText == string.Empty)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@flagText", DBNull.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@flagText", FlagText);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagColour", (byte)FlagColour);
                if (duplicateExpenseID.HasValue)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue(
                        "@duplicateExpenseID",
                        (int)duplicateExpenseID);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@duplicateExpenseID", DBNull.Value);
                }

                databaseConnection.sqlexecute.Parameters.Add("@associatedExpenses", SqlDbType.Structured);
                if (exceededLimit.HasValue)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue(
                        "@exceededLimit",
                        exceededLimit.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@exceededLimit", DBNull.Value);
                }

                if (stepNumber.HasValue)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@stepnumber", (int)stepNumber);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@stepnumber", DBNull.Value);
                }

                if (AssociatedExpenses.Count == 0)
                {
                    // databaseConnection.sqlexecute.Parameters["@associatedExpenses"].Value = DBNull.Value;
                }
                else
                {
                    List<SqlDataRecord> integers = new List<SqlDataRecord>();
                    SqlMetaData[] rowMetaData = { new SqlMetaData("c1", SqlDbType.Int) };
                    foreach (AssociatedExpense expense in AssociatedExpenses)
                    {
                        var row = new SqlDataRecord(rowMetaData);
                        row.SetInt32(0, expense.ExpenseID);
                        integers.Add(row);
                    }

                    databaseConnection.sqlexecute.Parameters["@associatedExpenses"].Value = integers;
                }

                databaseConnection.ExecuteProc("flagItem");
                flaggeditemid = (int)databaseConnection.sqlexecute.Parameters["@flaggeditemid"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            return flaggeditemid;
        }

        /// <summary>
        /// Checks an expense item to see if limits have been exceeded and a flag applies
        /// </summary>
        /// <param name="flag">The flag rule to check</param>
        /// <param name="item">The expense item to validate</param>
        /// <param name="emplimits">The limits applied to the employeeID to check</param>
        /// <param name="reqemp">The employee who saved the item</param>
        /// <returns>A flag result containing the flag and the custom flag text</returns>
        private List<FlaggedItem> CheckLimits(
            LimitFlag flag,
            cExpenseItem item,
            Dictionary<int, cRoleSubcat> emplimits,
            Employee reqemp)
        {
            decimal maximum = 0;
            decimal receiptmaximum = 0;

            decimal exceeded = 0;
            string comment = string.Empty;

            FlagColour flagColour = FlagColour.Red;

            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(this.AccountID);
            cAccountProperties reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties;
            bool increaseothers = flag.IncreaseByNumOthers;

            if ((flag.FlagType == FlagType.LimitWithReceipt && !item.normalreceipt) || (flag.FlagType == FlagType.LimitWithoutReceipt && item.normalreceipt))
            {
                return null;
            }

            // get limits
            if (emplimits.ContainsKey(item.subcatid))
            {
                cRoleSubcat rolesub = emplimits[item.subcatid];
                maximum = rolesub.maximum;
                receiptmaximum = rolesub.receiptmaximum;
            }

            SubcatBasic reqsubcat = this.GetSubcat(item.subcatid);
            if (reqsubcat.CalculationType == CalculationType.Meal)
            {
                // if it is a meal increase limts * number of staff
                if (increaseothers == false)
                {
                    maximum = maximum * item.staff;
                    receiptmaximum = receiptmaximum * item.staff;
                }
                else
                {
                    maximum = maximum * (item.staff + item.others);
                    receiptmaximum = receiptmaximum * (item.staff + item.others);
                }
            }

            if (item.nonights != 0)
            {
                // increase the limit by the number of nights
                maximum = maximum * item.nonights;
                receiptmaximum = receiptmaximum * item.nonights;
            }

            if (item.norooms != 0)
            {
                // increase the limit by the number of rooms
                maximum = maximum * item.norooms;
                receiptmaximum = receiptmaximum * item.norooms;
            }

            int basecurrency;

            if (reqemp.PrimaryCurrency != 0)
            {
                basecurrency = reqemp.PrimaryCurrency;
            }
            else
            {
                if (reqProperties.BaseCurrency != null)
                {
                    basecurrency = (int)reqProperties.BaseCurrency;
                }
                else
                {
                    basecurrency = 0;
                }
            }

            cCurrencies clscurrencies = new cCurrencies(this.AccountID, null);
            cCurrency reqcurrency = clscurrencies.getCurrencyById(basecurrency);
            string symbol;
            if (item.normalreceipt == false && receiptmaximum > 0 && flag.FlagType == FlagType.LimitWithoutReceipt)
            {
                if (item.total > receiptmaximum)
                {
                    symbol = this.GetGlobalCurrency(reqcurrency.globalcurrencyid).symbol;
                    exceeded = item.total - receiptmaximum;

                    flagColour = flag.CheckTolerance(item.total, receiptmaximum);
                    if (flagColour == FlagColour.None)
                    {
                        // falls in to no flag tolerance so don't flag
                        return null;
                    }

                    comment = "Our policy includes a limit of " + receiptmaximum.ToString(symbol + "###,###,##0.00") + " for this item when claimed without a receipt. You have exceeded this by " + exceeded.ToString(symbol + "###,###,##0.00") + ".";
                }
            }
            else
            {
                if ((item.total > maximum) && maximum > 0)
                {
                    symbol = this.GetGlobalCurrency(reqcurrency.globalcurrencyid).symbol;
                    exceeded = item.total - maximum;
                    flagColour = flag.CheckTolerance(item.total, maximum);
                    if (flagColour == FlagColour.None)
                    {
                        // falls in to no flag tolerance so don't flag
                        return null;
                    }

                    comment = "Our policy includes a limit of " + maximum.ToString(symbol + "###,###,##0.00") + " for this item when claimed with a receipt. You have exceeded this by " + exceeded.ToString(symbol + "###,###,##0.00") + ".";
                }
            }

            if (comment == string.Empty)
            {
                return null;
            }

            FlaggedItem flagResult = new LimitFlaggedItem(
                comment,
                flag.CustomFlagText,
                flag,
                flagColour,
                new List<AssociatedExpense>(),
                exceeded, flag.FlagTypeDescription, flag.NotesForAuthoriser, flag.AssociatedExpenseItems, flag.Action, flag.CustomFlagText, flag.ClaimantJustificationRequired, false);


            return new List<FlaggedItem>() { flagResult };
        }

        /// <summary>
        /// Gets any roles that apply to this user to determine which flags should be validated
        /// </summary>
        /// <param name="rolesubcats">A list of rolesubcats retrieved from the employees account</param>
        /// <returns>All roles that this user belongs to</returns>
        private List<int> GetEmployeeRoles(Dictionary<int, cRoleSubcat> rolesubcats)
        {
            List<int> roles = new List<int>();
            foreach (cRoleSubcat rolesubcat in rolesubcats.Values)
            {
                if (!roles.Contains(rolesubcat.roleid))
                {
                    roles.Add(rolesubcat.roleid);
                }
            }

            return roles;
        }

        /// <summary>
        /// Gets which flags in the system are applicable to this user based on their roles and the expense item
        /// </summary>
        /// <param name="subcatid">The subcatID of the expense to be checked</param>
        /// <param name="roles">The roles that the employee belongs to</param>
        /// <returns>A list of all flags that applies to the expense item and provided roles</returns>
        private IEnumerable<Flag> GetFlagsByExpenseItemAndRole(int subcatid, List<int> roles)
        {
            return
                this.lstFlags.Values.Where(
                    flag =>
                    (flag.ExpenseItemInclusionType == FlagInclusionType.All
                     || (flag.ExpenseItemInclusionType == FlagInclusionType.List
                         && (flag.AssociatedExpenseItems.FirstOrDefault(associatedItem => associatedItem.SubcatID == subcatid) != null)))
                    && (flag.ItemRoleInclusionType == FlagInclusionType.All
                        || (flag.ItemRoleInclusionType == FlagInclusionType.List && flag.ContainsItemRoles(roles))))
                    .ToList();
        }

        /// <summary>
        /// Initialises the cache with all flag rules
        /// </summary>
        private void InitialiseData()
        {
            this._addresses = new Addresses(this.AccountID);
            this.lstFlags = this.caching.Get(this.AccountID, CacheArea, "0") as Dictionary<int, Flag>
                            ?? this.CacheList();
        }

        /// <summary>
        /// The invalidate cache.
        /// </summary>
        private void InvalidateCache()
        {
            this.caching.Delete(this.AccountID, CacheArea, "0");
            this.lstFlags = null;
        }

        /// <summary>
        /// Gets the flag configuration from the database to be stored in cache
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// A list of all flags in the system
        /// </returns>
        private Dictionary<int, Flag> CacheList(IDBConnection connection = null)
        {
            Dictionary<int, List<AssociatedExpenseItem>> lstExpenseItems = this.GetAssociatedExpenseItems();

            Dictionary<int, List<int>> lstRoles = this.GetAssociatedItemRoles();

            Dictionary<int, List<Guid>> lstFields = this.GetAssociatedFields();

            Dictionary<int, List<int>> lstAggregates = this.GetAggregateFlags();

            Dictionary<int, Dictionary<int, cReportCriterion>> lstCriteria = this.GetCustomCriteria();

            Dictionary<int, Flag> lst = new Dictionary<int, Flag>();
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                using (
                    IDataReader reader =
                        databaseConnection.GetReader(
                            "select  flagID, flagType, flags.action, flagText, amberTolerance, frequency, frequencyType, period, periodType, financialYear, limit, dateComparisonType, dateToCompare, numberOfMonths, createdOn, createdBy, modifiedOn, modifiedBy, flags.description, flags.active, tipLimit, claimantJustificationRequired, displayFlagImmediately, noFlagTolerance, financialYears.YearStart, financialYears.YearEnd, flagLevel, approverJustificationRequired, increaseByNumOthers, displayLimit, notesForAuthoriser, itemroleinclusiontype, expenseiteminclusiontype, passengerLimit from flags left join financialYears on financialYears.financialyearid = flags.financialYear")
                    )
                {
                    while (reader.Read())
                    {
                        int flagid = reader.GetInt32(reader.GetOrdinal("flagID"));
                        FlagType flagtype = (FlagType)reader.GetByte(reader.GetOrdinal("flagtype"));
                        FlagAction flagaction = (FlagAction)reader.GetByte(reader.GetOrdinal("action"));
                        string flagtext = reader.IsDBNull(reader.GetOrdinal("flagtext"))
                                              ? string.Empty
                                              : reader.GetString(reader.GetOrdinal("flagtext"));

                        List<AssociatedExpenseItem> expenseitems;
                        lstExpenseItems.TryGetValue(flagid, out expenseitems);
                        if (expenseitems == null)
                        {
                            expenseitems = new List<AssociatedExpenseItem>();
                        }

                        List<int> roles;
                        lstRoles.TryGetValue(flagid, out roles);
                        if (roles == null)
                        {
                            roles = new List<int>();
                        }

                        List<Guid> fields;
                        lstFields.TryGetValue(flagid, out fields);
                        if (fields == null)
                        {
                            fields = new List<Guid>();
                        }

                        DateTime createdon = reader.GetDateTime(reader.GetOrdinal("createdOn"));
                        int? createdby;
                        if (reader.IsDBNull(reader.GetOrdinal("createdBy")))
                        {
                            createdby = null;
                        }
                        else
                        {
                            createdby = reader.GetInt32(reader.GetOrdinal("createdBy"));
                        }

                        int? modifiedby;
                        if (reader.IsDBNull(reader.GetOrdinal("modifiedBy")))
                        {
                            modifiedby = null;
                        }
                        else
                        {
                            modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedBy"));
                        }

                        DateTime? modifiedon;
                        if (reader.IsDBNull(reader.GetOrdinal("modifiedOn")))
                        {
                            modifiedon = null;
                        }
                        else
                        {
                            modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedOn"));
                        }

                        bool active = reader.GetBoolean(reader.GetOrdinal("active"));
                        string description = reader.IsDBNull(reader.GetOrdinal("description"))
                                                 ? string.Empty
                                                 : reader.GetString(reader.GetOrdinal("description"));

                        bool claimantJustificationRequired =
                            reader.GetBoolean(reader.GetOrdinal("claimantJustificationRequired"));
                        bool displayFlagImmediately = reader.GetBoolean(reader.GetOrdinal("displayFlagImmediately"));
                        FlagColour flagLevel = (FlagColour)reader.GetByte(reader.GetOrdinal("flagLevel"));
                        bool approverJustificationRequired =
                            reader.GetBoolean(reader.GetOrdinal("approverJustificationRequired"));
                        string notesforauthoriser = reader.IsDBNull(reader.GetOrdinal("notesForAuthoriser"))
                                                        ? string.Empty
                                                        : reader.GetString(reader.GetOrdinal("notesForAuthoriser"));
                        FlagInclusionType itemroleInclusionType =
                            (FlagInclusionType)reader.GetByte(reader.GetOrdinal("itemRoleInclusionType"));
                        FlagInclusionType expenseItemInclusionType =
                            (FlagInclusionType)reader.GetByte(reader.GetOrdinal("expenseItemInclusionType"));
                        decimal? ambertolerance;
                        decimal? limit;
                        decimal? noflagtolerance;
                        switch (flagtype)
                        {
                            case FlagType.Duplicate:
                                lst.Add(
                                    flagid,
                                    new DuplicateFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        fields,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.InvalidDate:
                                InvalidDateFlagType datecomparisontype =
                                    (InvalidDateFlagType)reader.GetByte(reader.GetOrdinal("dateComparisonType"));
                                DateTime? datetocompare;
                                byte? numberofmonths;
                                if (datecomparisontype == InvalidDateFlagType.SetDate)
                                {
                                    datetocompare = reader.GetDateTime(reader.GetOrdinal("dateToCompare"));
                                    numberofmonths = null;
                                }
                                else
                                {
                                    datetocompare = null;
                                    numberofmonths = reader.GetByte(reader.GetOrdinal("numberOfMonths"));
                                }

                                lst.Add(
                                    flagid,
                                    new InvalidDateFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        datecomparisontype,
                                        datetocompare,
                                        numberofmonths,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.GroupLimitWithoutReceipt:
                            case FlagType.GroupLimitWithReceipt:
                                if (reader.IsDBNull(reader.GetOrdinal("amberTolerance")))
                                {
                                    ambertolerance = null;
                                }
                                else
                                {
                                    ambertolerance = reader.GetDecimal(reader.GetOrdinal("amberTolerance"));
                                }

                                if (reader.IsDBNull(reader.GetOrdinal("noflagtolerance")))
                                {
                                    noflagtolerance = null;
                                }
                                else
                                {
                                    noflagtolerance = reader.GetDecimal(reader.GetOrdinal("noflagtolerance"));
                                }

                                limit = reader.GetDecimal(reader.GetOrdinal("limit"));
                                lst.Add(
                                    flagid,
                                    new GroupLimitFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        ambertolerance,
                                        (decimal)limit,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        noflagtolerance,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.LimitWithoutReceipt:
                            case FlagType.LimitWithReceipt:
                                bool increasebynumothers = reader.GetBoolean(reader.GetOrdinal("increaseByNumOthers"));
                                bool displaylimit = reader.GetBoolean(reader.GetOrdinal("displayLimit"));
                                if (reader.IsDBNull(reader.GetOrdinal("amberTolerance")))
                                {
                                    ambertolerance = null;
                                }
                                else
                                {
                                    ambertolerance = reader.GetDecimal(reader.GetOrdinal("amberTolerance"));
                                }

                                if (reader.IsDBNull(reader.GetOrdinal("noflagtolerance")))
                                {
                                    noflagtolerance = null;
                                }
                                else
                                {
                                    noflagtolerance = reader.GetDecimal(reader.GetOrdinal("noflagtolerance"));
                                }

                                lst.Add(
                                    flagid,
                                    new LimitFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        ambertolerance,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        noflagtolerance,
                                        flagLevel,
                                        approverJustificationRequired,
                                        increasebynumothers,
                                        displaylimit,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.MileageExceeded:
                                if (reader.IsDBNull(reader.GetOrdinal("amberTolerance")))
                                {
                                    ambertolerance = null;
                                }
                                else
                                {
                                    ambertolerance = reader.GetDecimal(reader.GetOrdinal("amberTolerance"));
                                }

                                if (reader.IsDBNull(reader.GetOrdinal("noflagtolerance")))
                                {
                                    noflagtolerance = null;
                                }
                                else
                                {
                                    noflagtolerance = reader.GetDecimal(reader.GetOrdinal("noflagtolerance"));
                                }

                                lst.Add(
                                    flagid,
                                    new MileageFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        ambertolerance,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        noflagtolerance,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.FrequencyOfItemCount:
                            case FlagType.FrequencyOfItemSum:
                                byte? frequency;
                                if (reader.IsDBNull(reader.GetOrdinal("frequency")))
                                {
                                    frequency = null;
                                }
                                else
                                {
                                    frequency = reader.GetByte(reader.GetOrdinal("frequency"));
                                }

                                FlagFrequencyType frequencyType =
                                    (FlagFrequencyType)reader.GetByte(reader.GetOrdinal("frequencyType"));
                                byte period = reader.GetByte(reader.GetOrdinal("period"));
                                FlagPeriodType periodtype =
                                    (FlagPeriodType)reader.GetByte(reader.GetOrdinal("periodType"));
                                if (reader.IsDBNull(reader.GetOrdinal("limit")))
                                {
                                    limit = null;
                                }
                                else
                                {
                                    limit = reader.GetDecimal(reader.GetOrdinal("limit"));
                                }

                                if (periodtype == FlagPeriodType.FinancialYears)
                                {
                                    DateTime financialYearStart = reader.GetDateTime(reader.GetOrdinal("YearStart"));
                                    DateTime financialYearEnd = reader.GetDateTime(reader.GetOrdinal("YearEnd"));
                                    int financialYear = reader.GetInt32(reader.GetOrdinal("financialYear"));
                                    lst.Add(
                                        flagid,
                                        new FrequencyFlag(
                                            flagid,
                                            flagtype,
                                            flagaction,
                                            flagtext,
                                            roles,
                                            expenseitems,
                                            createdon,
                                            createdby,
                                            modifiedon,
                                            modifiedby,
                                            frequency,
                                            frequencyType,
                                            period,
                                            periodtype,
                                            limit,
                                            description,
                                            active,
                                            this.AccountID,
                                            claimantJustificationRequired,
                                            displayFlagImmediately,
                                            financialYear,
                                            financialYearStart,
                                            financialYearEnd,
                                            flagLevel,
                                            approverJustificationRequired,
                                            notesforauthoriser,
                                            itemroleInclusionType,
                                            expenseItemInclusionType));
                                }
                                else
                                {
                                    lst.Add(
                                        flagid,
                                        new FrequencyFlag(
                                            flagid,
                                            flagtype,
                                            flagaction,
                                            flagtext,
                                            roles,
                                            expenseitems,
                                            createdon,
                                            createdby,
                                            modifiedon,
                                            modifiedby,
                                            frequency,
                                            frequencyType,
                                            period,
                                            periodtype,
                                            limit,
                                            description,
                                            active,
                                            this.AccountID,
                                            claimantJustificationRequired,
                                            displayFlagImmediately,
                                            flagLevel,
                                            approverJustificationRequired,
                                            notesforauthoriser,
                                            itemroleInclusionType,
                                            expenseItemInclusionType));
                                }

                                break;
                            case FlagType.Aggregate:
                                List<int> aggregates;
                                lstAggregates.TryGetValue(flagid, out aggregates);
                                if (aggregates == null)
                                {
                                    aggregates = new List<int>();
                                }

                                lst.Add(
                                    flagid,
                                    new AggregateFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        aggregates,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.TipLimitExceeded:
                                decimal? tipLimit;
                                if (reader.IsDBNull(reader.GetOrdinal("tipLimit")))
                                {
                                    tipLimit = null;
                                }
                                else
                                {
                                    tipLimit = reader.GetDecimal(reader.GetOrdinal("tipLimit"));
                                }

                                if (reader.IsDBNull(reader.GetOrdinal("amberTolerance")))
                                {
                                    ambertolerance = null;
                                }
                                else
                                {
                                    ambertolerance = reader.GetDecimal(reader.GetOrdinal("amberTolerance"));
                                }

                                if (reader.IsDBNull(reader.GetOrdinal("noflagtolerance")))
                                {
                                    noflagtolerance = null;
                                }
                                else
                                {
                                    noflagtolerance = reader.GetDecimal(reader.GetOrdinal("noflagtolerance"));
                                }

                                lst.Add(
                                    flagid,
                                    new TipFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        description,
                                        active,
                                        this.AccountID,
                                        (decimal)tipLimit,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        ambertolerance,
                                        noflagtolerance,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.ItemOnAWeekend:
                                lst.Add(
                                    flagid,
                                    new WeekendFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.ItemNotReimbursable:
                                lst.Add(
                                    flagid,
                                    new NonReimbursableFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.UnusedAllowanceAvailable:
                                lst.Add(
                                    flagid,
                                    new AllowanceAvailableFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.HomeToLocationGreater:
                                lst.Add(
                                    flagid,
                                    new HomeToOfficeFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.Custom:
                                Dictionary<int, cReportCriterion> criteria;
                                lstCriteria.TryGetValue(flagid, out criteria);
                                if (criteria == null)
                                {
                                    criteria = new Dictionary<int, cReportCriterion>();
                                }

                                lst.Add(
                                    flagid,
                                    new CustomFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        flagLevel,
                                        approverJustificationRequired,
                                        criteria,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.ReceiptNotAttached:
                                lst.Add(
                                    flagid,
                                    new ReceiptNotAttachedFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.NumberOfPassengersLimit:
                                int passengerLimit = reader.GetInt32(reader.GetOrdinal("passengerLimit"));
                                lst.Add(
                                    flagid,
                                    new NumberOfPassengersFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType,
                                        passengerLimit));
                                break;
                            case FlagType.OneItemInAGroup:
                                lst.Add(
                                    flagid,
                                    new OneInAGroupFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.JourneyDoesNotStartAndFinishAtHomeOrOffice:
                                lst.Add(
                                    flagid, 
                                    new JourneyDoesNotStartAndFinishAtHomeOrOfficeFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                 break;

                            case FlagType.ItemReimbursable:
                                lst.Add(
                                    flagid,
                                    new ReimbursableFlag(
                                        flagid,
                                        flagtype,
                                        flagaction,
                                        flagtext,
                                        roles,
                                        expenseitems,
                                        createdon,
                                        createdby,
                                        modifiedon,
                                        modifiedby,
                                        description,
                                        active,
                                        this.AccountID,
                                        claimantJustificationRequired,
                                        displayFlagImmediately,
                                        flagLevel,
                                        approverJustificationRequired,
                                        notesforauthoriser,
                                        itemroleInclusionType,
                                        expenseItemInclusionType));
                                break;
                            case FlagType.RestrictDailyMileage:
                                if (reader.IsDBNull(reader.GetOrdinal("limit")))
                                {
                                    limit = null;
                                }
                                else
                                {
                                    limit = reader.GetDecimal(reader.GetOrdinal("limit"));
                                }
                                lst.Add(
                                    flagid, new RestrictDailyMileageFlag(flagid,
                                    flagtype,
                                    flagaction,
                                    flagtext,
                                    roles,
                                    expenseitems,
                                    createdon,
                                    createdby,
                                    modifiedon,
                                    modifiedby,
                                    description,
                                    active,
                                    this.AccountID,
                                    claimantJustificationRequired,
                                    displayFlagImmediately,
                                    flagLevel,
                                    approverJustificationRequired,
                                    notesforauthoriser,
                                    itemroleInclusionType,
                                    expenseItemInclusionType,
                                    limit ?? 0m));
                                break;
                        }
                    }

                    reader.Close();
                }
            }

            this.caching.Add(this.AccountID, CacheArea, "0", lst);
            return lst;
        }

        /// <summary>
        /// Gets the criteria for the custom flags.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>SortedList</cref>
        ///     </see>
        ///     .
        /// </returns>
        private Dictionary<int, Dictionary<int, cReportCriterion>> GetCustomCriteria(IDBConnection connection = null)
        {
            Dictionary<int, Dictionary<int, cReportCriterion>> lstCriteria = new Dictionary<int, Dictionary<int, cReportCriterion>>();
            var joinVias = new JoinVias(new CurrentUser(this.AccountID, 0, 0, GlobalVariables.DefaultModule, 0));
            cFields clsfields = new cFields(this.AccountID);

            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                using (
                    IDataReader reader =
                        databaseConnection.GetReader(
                            "select criteriaid, flagid, condition, value1, value2, andor, [order], groupnumber, fieldid, joinviaid from flagCustomCriteria")
                    )
                {
                    while (reader.Read())
                    {
                        Guid criteriaId = reader.GetGuid(0);
                        int flagid = reader.GetInt32(1);
                        ConditionType condition = (ConditionType)reader.GetByte(2);
                        ConditionJoiner andor = (ConditionJoiner)reader.GetByte(5);
                        int order = reader.GetInt32(6);
                        cField field = reader.IsDBNull(reader.GetOrdinal("fieldid"))
                                           ? null
                                           : clsfields.GetFieldByID(reader.GetGuid(reader.GetOrdinal("fieldid")));
                        object[] value1 = new object[1];
                        object[] value2 = new object[2];
                        if (field != null && condition != ConditionType.ContainsData
                            && condition != ConditionType.DoesNotContainData)
                        {
                            switch (field.FieldType)
                            {
                                case "T":
                                case "S":
                                case "FS":
                                case "LT":
                                    if (reader.IsDBNull(reader.GetOrdinal("value1")))
                                    {
                                        value1[0] = string.Empty;
                                    }
                                    else
                                    {
                                        value1[0] = reader.GetString(reader.GetOrdinal("value1"));
                                    }

                                    break;
                                case "C":
                                case "M":
                                case "FD":
                                case "A":
                                case "F":
                                    if (!reader.IsDBNull(reader.GetOrdinal("value1")))
                                    {
                                        value1[0] = decimal.Parse(reader.GetString(reader.GetOrdinal("value1")));
                                    }
                                    else
                                    {
                                        value1[0] = string.Empty;
                                    }

                                    break;
                                case "N":
                                    if (field.ValueList
                                        || (field.FieldSource != cField.FieldSourceType.Metabase
                                            && field.GetRelatedTable() != null && field.IsForeignKey))
                                    {
                                        if (reader.IsDBNull(reader.GetOrdinal("value1")))
                                        {
                                            value1[0] = string.Empty;
                                        }
                                        else
                                        {
                                            value1[0] = reader.GetString(reader.GetOrdinal("value1"));
                                        }
                                    }
                                    else
                                    {
                                        value1[0] = int.Parse(reader.GetString(reader.GetOrdinal("value1")));
                                    }

                                    break;
                                case "X":
                                    value1[0] = byte.Parse(reader.GetString(reader.GetOrdinal("value1")));
                                    break;
                                case "Y":
                                    value1[0] = reader.GetString(reader.GetOrdinal("value1"));
                                    break;
                                case "D":
                                case "DT":
                                    switch (condition)
                                    {
                                        case ConditionType.LastXDays:
                                        case ConditionType.LastXMonths:
                                        case ConditionType.LastXWeeks:
                                        case ConditionType.LastXYears:
                                        case ConditionType.NextXDays:
                                        case ConditionType.NextXMonths:
                                        case ConditionType.NextXWeeks:
                                        case ConditionType.NextXYears:
                                            value1[0] = int.Parse(reader.GetString(reader.GetOrdinal("value1")));
                                            break;
                                        default:
                                            if (reader.IsDBNull(reader.GetOrdinal("value1")) == false)
                                            {
                                                value1[0] = DateTime.Parse(
                                                    reader.GetString(reader.GetOrdinal("value1")));
                                            }

                                            break;
                                    }

                                    break;
                            }

                            if (condition == ConditionType.Between)
                            {
                                switch (field.FieldType)
                                {
                                    case "T":
                                    case "S":
                                    case "FS":
                                    case "LT":
                                        value2[0] = reader.GetString(reader.GetOrdinal("value2"));
                                        break;
                                    case "C":
                                    case "M":
                                    case "FD":
                                    case "A":
                                    case "F":
                                        value2[0] = decimal.Parse(reader.GetString(reader.GetOrdinal("value2")));
                                        break;
                                    case "N":
                                        value2[0] = int.Parse(reader.GetString(reader.GetOrdinal("value2")));
                                        break;
                                    case "DT":
                                    case "D":
                                        value2[0] = DateTime.Parse(reader.GetString(reader.GetOrdinal("value2")));
                                        break;
                                }
                            }
                        }

                        JoinVia joinVia = null;

                        if (!reader.IsDBNull(reader.GetOrdinal("joinViaID")))
                        {
                            joinVia = joinVias.GetJoinViaByID(reader.GetInt32(reader.GetOrdinal("joinViaID")));
                        }

                        if (field != null)
                        {
                            var criterion = new cReportCriterion(
                                criteriaId,
                                Guid.Empty,
                                field,
                                condition,
                                value1,
                                value2,
                                andor,
                                order,
                                false,
                                0,
                                joinVia);

                            Dictionary<int, cReportCriterion> criteria;
                            lstCriteria.TryGetValue(flagid, out criteria);
                            if (criteria == null)
                            {
                                criteria = new Dictionary<int, cReportCriterion>();
                                lstCriteria.Add(flagid, criteria);
                            }

                            criteria.Add(order, criterion);
                        }
                    }
                }
            }

            return lstCriteria;
        }

        /// <summary>
        /// Retrieves all expense items from the flagAssociatedExpenseItem table as part of the caching
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// Returns a list of the expense items each flag applies to
        /// </returns>
        private Dictionary<int, List<AssociatedExpenseItem>> GetAssociatedExpenseItems(IDBConnection connection = null)
        {
            Dictionary<int, List<AssociatedExpenseItem>> lst = new Dictionary<int, List<AssociatedExpenseItem>>();
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                using (IDataReader reader = databaseConnection.GetReader("GetFlagAssociatedExpenseItems", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        int flagid = reader.GetInt32(0);
                        int subcatid = reader.GetInt32(1);
                        string subcat = reader.GetString(2);
                        List<AssociatedExpenseItem> expenseitems;
                        lst.TryGetValue(flagid, out expenseitems);
                        if (expenseitems == null)
                        {
                            expenseitems = new List<AssociatedExpenseItem>();
                            lst.Add(flagid, expenseitems);
                        }

                        expenseitems.Add(new AssociatedExpenseItem(subcatid, subcat));
                    }

                    reader.Close();
                }
            }

            return lst;
        }

        /// <summary>
        /// Retrieves all roles from the flagAssociatedRoles table as part of the caching
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// Returns a list of the item roles each flag applies to
        /// </returns>
        private Dictionary<int, List<int>> GetAssociatedItemRoles(IDBConnection connection = null)
        {
            Dictionary<int, List<int>> lst = new Dictionary<int, List<int>>();
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                using (
                    IDataReader reader = databaseConnection.GetReader("select flagID, roleID from flagAssociatedRoles"))
                {
                    while (reader.Read())
                    {
                        int flagid = reader.GetInt32(0);
                        int roleid = reader.GetInt32(1);
                        List<int> roles;
                        lst.TryGetValue(flagid, out roles);
                        if (roles == null)
                        {
                            roles = new List<int>();
                            lst.Add(flagid, roles);
                        }

                        roles.Add(roleid);
                    }

                    reader.Close();
                }
            }

            return lst;
        }

        /// <summary>
        /// Retrieves all flags that form an aggregrate flag from the flagAggregates table as part of the caching
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// Gets the flags that will be checked for the aggregate flags
        /// </returns>
        private Dictionary<int, List<int>> GetAggregateFlags(IDBConnection connection = null)
        {
            Dictionary<int, List<int>> lst = new Dictionary<int, List<int>>();
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                using (IDataReader reader = databaseConnection.GetReader("select flagID, aggregateFlagID from flagAggregates"))
                {
                    while (reader.Read())
                    {
                        int flagid = reader.GetInt32(0);
                        int aggregateflagid = reader.GetInt32(1);
                        List<int> aggregates;
                        lst.TryGetValue(flagid, out aggregates);
                        if (aggregates == null)
                        {
                            aggregates = new List<int>();
                            lst.Add(flagid, aggregates);
                        }

                        aggregates.Add(aggregateflagid);
                    }

                    reader.Close();
                }
            }

            return lst;
        }

        public void DeleteFlagField(int flagID, Guid fieldID, ICurrentUserBase currentUser, IDBConnection connection = null)
        {
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@flagID", flagID);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@fieldID", fieldID);
                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                databaseConnection.ExecuteProc("deleteFlagField");
                databaseConnection.sqlexecute.Parameters.Clear();
            }

            this.InvalidateCache();
        }

        /// <summary>
        /// Whether an authoriser justification is required.
        /// </summary>
        /// <param name="flag">
        /// The flag to check.
        /// </param>
        /// <param name="claimID">
        /// The id of the claim.
        /// </param>
        /// <param name="properties">
        /// The properties.
        /// </param>
        /// <returns>
        /// Whetheran authoriser justification is required.
        /// </returns>
        public bool AuthoriserJustificationRequired(Flag flag, int claimID, cAccountProperties properties)
        {
            if (!flag.ApproverJustificationRequired)
            {
                return false;
            }

            cClaims claims = new cClaims(this.AccountID);
            cClaim claim = claims.getClaimById(claimID);

            if (claim == null)
            {
                return false;
            }

            if (!claim.submitted)
            {
                return false;
            }

            cGroups _groups = new cGroups(this.AccountID);
            cGroup group = null;
            cEmployees employees = new cEmployees(this.AccountID);
            Employee employee;

            employee = employees.GetEmployeeById(claim.employeeid);

            //check the signoff group
            if (properties.OnlyCashCredit && properties.PartSubmit)
            {
                if (claim.claimtype == ClaimType.Credit)
                {
                    group = _groups.GetGroupById(employee.CreditCardSignOffGroup);
                }
                else if (claim.claimtype == ClaimType.Purchase)
                {
                    group = _groups.GetGroupById(employee.PurchaseCardSignOffGroup);
                }

                if (group == null)
                {
                    group = _groups.GetGroupById(employee.SignOffGroupID);
                }
            }
            else
            {
                group = _groups.GetGroupById(employee.SignOffGroupID);
            }

            if (group != null)
            {
                return group.stages.Values[claim.stage - 1].ApproverJustificationsRequired;
            }

            return false;
        }

        /// <summary>
        /// Retrieves all fields from the flagAssociatedFields table as part of the caching
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// Gets the fields to be checked for the duplicate flags
        /// </returns>
        private Dictionary<int, List<Guid>> GetAssociatedFields(IDBConnection connection = null)
        {
            Dictionary<int, List<Guid>> lst = new Dictionary<int, List<Guid>>();
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
            {
                const string Sql = "select flagID, fieldID from flagAssociatedFields";
                using (IDataReader reader = databaseConnection.GetReader(Sql))
                {
                    while (reader.Read())
                    {
                        int flagid = reader.GetInt32(0);
                        Guid fieldid = reader.GetGuid(1);
                        List<Guid> fields;
                        lst.TryGetValue(flagid, out fields);
                        if (fields == null)
                        {
                            fields = new List<Guid>();
                            lst.Add(flagid, fields);
                        }

                        fields.Add(fieldid);
                    }

                    reader.Close();
                }
            }

            return lst;
        }

        private SubcatBasic GetSubcat(int id)
        {
            SubcatBasic subcat;
            if (this._CachedSubcats == null)
            {
                this._CachedSubcats = new Dictionary<int, SubcatBasic>();
            }

            this._CachedSubcats.TryGetValue(id, out subcat);

            if (subcat == null)
            {
                cSubcats subcats = new cSubcats(this.AccountID);
                subcat = subcats.GetSubcatBasic(id);
                this._CachedSubcats.Add(id, subcat);
            }

            return subcat;
        }

        private cGlobalCurrency GetGlobalCurrency(int id)
        {
            cGlobalCurrency currency;

            if (this._CachedGlobalCurrencies == null)
            {
                this._CachedGlobalCurrencies = new Dictionary<int, cGlobalCurrency>();
            }

            this._CachedGlobalCurrencies.TryGetValue(id, out currency);

            if (currency == null)
            {
                cGlobalCurrencies currencies = new cGlobalCurrencies();
                currency = currencies.getGlobalCurrencyById(id);
                this._CachedGlobalCurrencies.Add(id, currency);
            }

            return currency;
        }

        private Dictionary<int, cRoleSubcat> GetRoleSubcats(Employee employee)
        {
            if (this._Rolesubcats == null)
            {
                cEmployees employees = new cEmployees(this.AccountID);
                this._Rolesubcats = employees.getResultantRoleSet(employee);
            }

            return this._Rolesubcats;
        }

        private List<int> GetRoles(int subcatID)
        {
            if (this._CachedRoles == null)
            {
                this._CachedRoles = new Dictionary<int, List<int>>();
            }


            List<int> roles;
            this._CachedRoles.TryGetValue(subcatID, out roles);

            if (roles == null)
            {
                roles = this.GetEmployeeRoles(this._Rolesubcats);


                //remove roles that aren't allowed on expense
                cSubcats subcats = new cSubcats(this.AccountID);
                List<int> allowedSubcatItemroles = subcats.GetItemRolesForSubcat(subcatID);
                for (int i = roles.Count - 1; i >= 0; i--)
                {
                    if (!allowedSubcatItemroles.Contains(roles[i]))
                    {
                        roles.RemoveAt(i);
                    }
                }

                this._CachedRoles.Add(subcatID, roles);
            }

            return roles;
        }

        public cClaim GetClaim(int id)
        {

            if (this.Claim == null)
            {
                cClaims claims = new cClaims(this.AccountID);
                this.Claim = claims.getClaimById(id);
            }

            return this.Claim;
        }

        /// <summary>
        /// The create flags grid.
        /// </summary>
        /// <param name="claimId">
        /// The claim id.
        /// </param>
        /// <param name="expenseIds">
        /// The exense id.
        /// </param>
        /// <param name="pageSource">
        /// The page source
        /// </param>
        /// <param name="employeeId">
        /// The employeeId
        /// </param>
        /// <returns>
        /// The <see cref="FlaggedItemsManager">FlaggedItemsManager</see>.
        /// </returns>
        public FlaggedItemsManager CreateFlagsGrid(int claimId, List<int> expenseIds, string pageSource, int employeeId)
        {
            int accountId = this.AccountID;
            var claims = new cClaims(accountId);
            var claim = claims.getClaimById(claimId);
            bool claimant = pageSource == "ClaimViewer" ? true : claim.employeeid == employeeId;
            int? stage = null;

            if (claim.stage > 0)
            {
                stage = claim.stage;
            }

            var expenseItems = new cExpenseItems(accountId);
            FlaggedItemsManager manager = expenseItems.GetFlaggedItems(expenseIds);

            manager.Claimant = claimant;
            manager.Stage = stage;
            manager.Authorising = pageSource == "CheckAndPay";
            manager.CanAccessClaim = claims.CheckClaimAndOwnership(claim, cMisc.GetCurrentUser(), false) == ClaimToAccessStatus.Success;
            manager.SubmittingClaim = false;
            manager.OnlyDisplayBlockedItems = false;

            cEmployees employees = new cEmployees(accountId);
            Employee employee = employees.GetEmployeeById(claim.employeeid);
            manager.ClaimantName = employee.FullName;

            return manager;
        }

        /// <summary>
        /// The save flag rule.
        /// </summary>
        /// <param name="flagid">
        ///     The flag id.
        /// </param>
        /// <param name="flagtype">
        ///     The type of flag. Duplicate limit with a receipt etc.
        /// </param>
        /// <param name="action">
        ///     The action to take if the flag is breached.
        /// </param>
        /// <param name="customFlagText">
        ///     The custom flag text provided by administrators to show to claimants in the event of a breach.
        /// </param>
        /// <param name="invaliddatetype">
        ///     Whether it is a set date or number of months
        /// </param>
        /// <param name="date">
        ///     The set date if using the Initial Date dateflagtype
        /// </param>
        /// <param name="months">
        ///     The number of months if using the number of months date flag type.
        /// </param>
        /// <param name="ambertolerance">
        ///     The percentage over the limit a claimant has gone where an amber flag is displayed rather than red.
        /// </param>
        /// <param name="frequency">
        ///     The frequency.
        /// </param>
        /// <param name="frequencyType">
        ///     The frequency the flag will be checked on. Every/In the last
        /// </param>
        /// <param name="period">
        ///     The period the flag will be checked against.
        /// </param>
        /// <param name="periodtype">
        ///     The period type. Daily, weekly, monthly etc
        /// </param>
        /// <param name="limit">
        ///     The monetary limit of the flag.
        /// </param>
        /// <param name="description">
        ///     The flag Description.
        /// </param>
        /// <param name="active">
        ///     Whether the flag is currently active.
        /// </param>
        /// <param name="claimantJustificationRequired">
        ///     The claimant justification required.
        /// </param>
        /// <param name="displayFlagImmediately">
        ///     The display flag immediately.
        /// </param>
        /// <param name="flagTolerancePercentage">
        ///     If the limit is breached, it will not be flagged if below the no flag tolerance percentage
        /// </param>
        /// <param name="financialYear">
        ///     The financial year.
        /// </param>
        /// <param name="tipLimit">
        ///     The tip Limit.
        /// </param>
        /// <param name="flagLevel">
        ///     The severity level of the flag.
        /// </param>
        /// <param name="approverJustificationRequired">
        ///     Whether an approver needs to provide a justification in order to authorise the claim.
        /// </param>
        /// <param name="increaseLimitByNumOthers">
        ///     Whether to increase the limit by the number of others fields as well as the number of employees field.
        /// </param>
        /// <param name="displayLimit">
        ///     Whether to display the claimant their limit when adding an expense.
        /// </param>
        /// <param name="reportCriteria">
        ///     The custom criteria to validate.
        /// </param>
        /// <param name="notesforauthoriser">
        ///     Notes seen by the authoriser to guide them on how to deal with the flag.
        /// </param>
        /// <param name="itemroleinclusiontype">
        ///     The item roles this flag applies to.
        /// </param>
        /// <param name="expenseiteminclusiontype">
        ///     The expense items this flag applies to.
        /// </param>
        /// <param name="passengerLimit">
        ///     The passenger limit that applied
        /// </param>
        /// <param name="validateSelectedExpenseItems">
        ///     Whether to validate the selected expense items
        /// </param>
        /// <param name="dailyMileageLimit">The daily mileage limit used for <see cref="RestrictDailyMileageFlag"/>flag type</param>
        /// <returns>
        /// The<see cref="int"/>.
        /// </returns>
        public int Save(int flagid, FlagType flagtype, FlagAction action, string customFlagText, InvalidDateFlagType invaliddatetype, DateTime? date, byte? months, decimal? ambertolerance, byte? frequency, FlagFrequencyType frequencyType, byte? period, FlagPeriodType periodtype, decimal? limit, string description, bool active, bool claimantJustificationRequired, bool displayFlagImmediately, decimal? flagTolerancePercentage, int? financialYear, decimal? tipLimit, FlagColour flagLevel, bool approverJustificationRequired, bool increaseLimitByNumOthers, bool displayLimit, JavascriptTreeData reportCriteria, string notesforauthoriser, FlagInclusionType itemroleinclusiontype, FlagInclusionType expenseiteminclusiontype, int? passengerLimit, bool validateSelectedExpenseItems, decimal? dailyMileageLimit)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            int? createdby;
            DateTime? modifiedon = null;
            int? modifiedby = null;
            DateTime createdon;

            FlagManagement clsflags = new FlagManagement(currentUser.AccountID);

            if (flagid > 0)
            {
                if (!currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.FlagsAndLimits, true))
                {
                    return -2;
                }

                Flag oldflag = clsflags.GetBy(flagid);
                createdon = oldflag.CreatedOn;
                createdby = oldflag.CreatedBy;
                modifiedon = DateTime.Now;
                modifiedby = currentUser.EmployeeID;

                if (oldflag.ExpenseItemSelectionMandatory && oldflag != null && oldflag.AssociatedExpenseItems.Count == 0
                    && validateSelectedExpenseItems)
                {
                    return -4;
                }
            }
            else
            {
                if (!currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.FlagsAndLimits, true))
                {
                    return -2;
                }

                createdon = DateTime.Now;
                createdby = currentUser.EmployeeID;
            }

            Flag flag = null;
            switch (flagtype)
            {
                case FlagType.Duplicate:
                    flag = new DuplicateFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        new List<Guid>(),
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.InvalidDate:
                    flag = new InvalidDateFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        invaliddatetype,
                        date,
                        months,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.GroupLimitWithoutReceipt:
                case FlagType.GroupLimitWithReceipt:
                    flag = new GroupLimitFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        ambertolerance,
                        (decimal)limit,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagTolerancePercentage,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.LimitWithoutReceipt:
                case FlagType.LimitWithReceipt:
                    flag = new LimitFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        ambertolerance,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagTolerancePercentage,
                        flagLevel,
                        approverJustificationRequired,
                        increaseLimitByNumOthers,
                        displayLimit,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.MileageExceeded:
                    flag = new MileageFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        ambertolerance,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagTolerancePercentage,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.FrequencyOfItemCount:
                case FlagType.FrequencyOfItemSum:
                    if (periodtype == FlagPeriodType.FinancialYears)
                    {
                        flag = new FrequencyFlag(
                            flagid,
                            flagtype,
                            action,
                            customFlagText,
                            new List<int>(),
                            new List<AssociatedExpenseItem>(),
                            createdon,
                            createdby,
                            modifiedon,
                            modifiedby,
                            frequency,
                            frequencyType,
                            (byte)period,
                            periodtype,
                            limit,
                            description,
                            active,
                            currentUser.AccountID,
                            claimantJustificationRequired,
                            displayFlagImmediately,
                            (int)financialYear,
                            DateTime.Now,
                            DateTime.Now,
                            flagLevel,
                            approverJustificationRequired,
                            notesforauthoriser,
                            itemroleinclusiontype,
                            expenseiteminclusiontype);
                    }
                    else
                    {
                        flag = new FrequencyFlag(
                            flagid,
                            flagtype,
                            action,
                            customFlagText,
                            new List<int>(),
                            new List<AssociatedExpenseItem>(),
                            createdon,
                            createdby,
                            modifiedon,
                            modifiedby,
                            frequency,
                            frequencyType,
                            (byte)period,
                            periodtype,
                            limit,
                            description,
                            active,
                            currentUser.AccountID,
                            claimantJustificationRequired,
                            displayFlagImmediately,
                            flagLevel,
                            approverJustificationRequired,
                            notesforauthoriser,
                            itemroleinclusiontype,
                            expenseiteminclusiontype);
                    }

                    break;
                case FlagType.Aggregate:
                    flag = new AggregateFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        new List<int>(),
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.ItemOnAWeekend:
                    flag = new WeekendFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.TipLimitExceeded:
                    flag = new TipFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        description,
                        active,
                        currentUser.AccountID,
                        (decimal)tipLimit,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        ambertolerance,
                        flagTolerancePercentage,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.ItemNotReimbursable:
                    flag = new NonReimbursableFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.UnusedAllowanceAvailable:
                    flag = new AllowanceAvailableFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.HomeToLocationGreater:
                    flag = new HomeToOfficeFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.Custom:
                    var joinVias = new JoinVias(currentUser);
                    Dictionary<int, cReportCriterion> criteria = new Dictionary<int, cReportCriterion>();
                    cFields clsFields = new cFields(currentUser.AccountID);
                    int order = 1;
                    foreach (JavascriptTreeData.JavascriptTreeNode treeNode in reportCriteria.data)
                    {
                        cField reportField = clsFields.GetFieldByID(Guid.Parse(treeNode.attr.fieldid));
                        int joinViaId = treeNode.attr.joinviaid;
                        string joinViaDescription = treeNode.attr.crumbs;
                        string joinViaIDs = treeNode.attr.id;
                        var joinViaList = new SortedList<int, JoinViaPart>();
                        if (joinViaId < 1)
                        {
                            joinViaList = joinVias.JoinViaPartsFromCompositeGuid(joinViaIDs, reportField);
                            joinViaId = 0; // 0 causes the save on the joinVia list
                        }

                        JoinVia joinVia = null;
                        const byte GroupNumber = 0;
                        const ConditionJoiner Joiner = ConditionJoiner.And;
                        object[] defaultValue = null;
                        object[] value2 = TreeViewNodes.GetMetadataValue(treeNode, "criterionTwo", defaultValue);
                        object[] value1 = TreeViewNodes.GetMetadataValue(treeNode, "criterionOne", defaultValue);
                        var conditionType =
                            (ConditionType)
                            TreeViewNodes.GetMetadataValue(treeNode, "conditionType", (int)ConditionType.ContainsData);
                        switch (conditionType)
                        {
                            case ConditionType.Yesterday:
                            case ConditionType.Today:
                            case ConditionType.Tomorrow:
                            case ConditionType.Next7Days:
                            case ConditionType.Last7Days:
                            case ConditionType.NextWeek:
                            case ConditionType.LastWeek:
                            case ConditionType.ThisWeek:
                            case ConditionType.NextMonth:
                            case ConditionType.LastMonth:
                            case ConditionType.ThisMonth:
                            case ConditionType.NextYear:
                            case ConditionType.LastYear:
                            case ConditionType.ThisYear:
                            case ConditionType.NextTaxYear:
                            case ConditionType.LastTaxYear:
                            case ConditionType.NextFinancialYear:
                            case ConditionType.LastFinancialYear:
                            case ConditionType.ThisFinancialYear:
                            case ConditionType.AnyTime:
                                value1 = null;
                                value2 = null;
                                break;
                        }

                        bool runtime = TreeViewNodes.GetMetadataValue(treeNode, "runtime", false);
                        Guid criteriaid = Guid.Empty;
                        if (joinViaList.Count > 0)
                        {
                            joinVia = new JoinVia(joinViaId, joinViaDescription, Guid.Empty, joinViaList);
                            joinViaId = joinVias.SaveJoinVia(joinVia);
                        }

                        if (joinViaId > 0)
                        {
                            joinVia = joinVias.GetJoinViaByID(joinViaId);
                        }

                        var newCriteria = new cReportCriterion(
                            criteriaid,
                            Guid.Empty,
                            reportField,
                            conditionType,
                            value1,
                            value2,
                            Joiner,
                            order,
                            runtime,
                            GroupNumber,
                            joinVia);
                        criteria.Add(order, newCriteria);
                        order++;
                    }

                    flag = new CustomFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        criteria,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.ReceiptNotAttached:
                    flag = new ReceiptNotAttachedFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.NumberOfPassengersLimit:
                    flag = new NumberOfPassengersFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype,
                        (int)passengerLimit);
                    break;
                case FlagType.OneItemInAGroup:
                    flag = new OneInAGroupFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.ItemReimbursable:
                    flag = new ReimbursableFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;

                case FlagType.JourneyDoesNotStartAndFinishAtHomeOrOffice:
                    flag = new JourneyDoesNotStartAndFinishAtHomeOrOfficeFlag(
                        flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype);
                    break;
                case FlagType.RestrictDailyMileage:
                    flag = new RestrictDailyMileageFlag(flagid,
                        flagtype,
                        action,
                        customFlagText,
                        new List<int>(),
                        new List<AssociatedExpenseItem>(),
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        description,
                        active,
                        currentUser.AccountID,
                        claimantJustificationRequired,
                        displayFlagImmediately,
                        flagLevel,
                        approverJustificationRequired,
                        notesforauthoriser,
                        itemroleinclusiontype,
                        expenseiteminclusiontype, 
                        (decimal)dailyMileageLimit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flagtype), flagtype, null);
            }

            if (flag.ExpenseItemSelectionMandatory && flag.ExpenseItemInclusionType == FlagInclusionType.All
                && validateSelectedExpenseItems)
            {
                return -1;
            }

            if (flag.FlagID == 0 && flag.ExpenseItemSelectionMandatory && validateSelectedExpenseItems)
            //saving new flag so can't have added any items
            {
                return -4;
            }

            flagid = flag.Save(currentUser);

            if (flagid > 0)
            {
                InvalidateCache();
            }

            return flagid;
        }
       
    }
   
}
