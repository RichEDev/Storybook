using System;

using SpendManagementLibrary;

namespace Spend_Management
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Microsoft.SqlServer.Server;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Enumerators.Expedite;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.JourneyDeductionRules;
    using SpendManagementLibrary.Mileage;
    using SpendManagementLibrary.Flags;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.UserDefinedFields;

    using Common.Logging;

    using expenses.code;
    using expenses.code.Claims;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    /// <summary>
    /// Summary description for items.
    /// </summary>
    public class cExpenseItems
    {
        /// <summary>
        /// An instance of <see cref="ILog"/> for logging information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<cExpenseItems>().GetLogger();

        private int nAccountid;
        private List<cExpenseItem> expenseItems;

        string strsql;
        cMisc clsmisc;
        
        private readonly IDataFactory<IGeneralOptions, int> _generalOptionsFactory = FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>();

        cFields fields;

        private cStage currentStage;

        private cStage payBeforeValidateStage;

        /// <summary>
        /// A private instance of <see cref="Addresses"/>
        /// </summary>
        private Addresses _addresses;

        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion

        public cExpenseItems()
        {
        }
        public cExpenseItems(int accountid)
        {
            nAccountid = accountid;
            clsmisc = new cMisc(accountid);

            fields = new cFields(accountid);
            this._addresses = new Addresses(accountid);
        }

        /// <summary>
        /// Add an expense item
        /// </summary>
        /// <param name="expitem">Expense item to be added</param>
        /// <param name="employeeid">Id of the employee entering the expense</param>
        /// <param name="claim">An optional parameter for if the claim object which the expense is being added to is already created</param>
        /// <returns>The id of the newly created expense</returns>
        public int addItem(cExpenseItem expitem, Employee reqemp, cClaim claim = null)
        {
            cMisc clsmisc = new cMisc(accountid);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cClaims claims = new cClaims(accountid);
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(accountid);
            cAccountSubAccount subAccount = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId);
            cAccountProperties accountProperties = subAccount.SubAccountProperties.Clone(); // clsSubAccounts.getFirstSubAccount().SubAccountProperties.Clone();

            bool amountpayableCalculated = false;
            decimal total = 0;
            decimal amountpay = 0;
            decimal originalTotal = 0;
            decimal originalGlobalTotal = 0;
            
            if (claim == null)
            {
                claim = claims.getClaimById(expitem.claimid);
            }

            if (expitem.reason.Length >= 2499)
            {
                expitem.reason = expitem.reason.Substring(0, 2498);
            }

            if (expitem.transactionid > 0)
            {
                cCardStatements clsstatements = new cCardStatements(accountid);
                cCardTransaction transaction = clsstatements.getTransactionById(expitem.transactionid);
                cCardStatement statement = clsstatements.getStatementById(transaction.statementid);
                if (statement.Corporatecard.singleclaim)
                {
                    if (clsstatements.canReconcileItem(expitem.claimid, transaction.statementid, reqemp.EmployeeID) == false)
                    {
                        return -1;

                    }
                }
            }

            int expenseid;
            cUserdefinedFields clsuserdefined = new cUserdefinedFields(accountid);
            cSubcats clssubcats = new cSubcats(accountid);
            cSubcat subcat = clssubcats.GetSubcatById(expitem.subcatid);
            if (subcat == null)
            {
                return 0;
            }

            if (subcat.calculation == CalculationType.ExcessMileage && expitem.journeysteps.ContainsKey(0))
            {
                expitem.journeysteps[0].NumActualMiles = (decimal)expitem.quantity * (decimal)reqemp.ExcessMileage;
                expitem.journeysteps[0].nummiles = (decimal)expitem.quantity * (decimal)reqemp.ExcessMileage;

            }

            cMileagecats clsmileagecats = new cMileagecats(accountid);



            cVat clsvat = new cVat(accountid, ref expitem, reqemp, clsmisc, null);

            cMileageCat reqmileage = clsmileagecats.GetMileageCatById(expitem.mileageid);

            cEmployeeCars clsEmployeeCars = new cEmployeeCars(accountid, reqemp.EmployeeID);

            var generalOptions = this._generalOptionsFactory[currentUser.CurrentSubAccountId].WithDutyOfCare().WithCurrency();

            if (reqmileage != null)
            {
                #region Check if mileage should not be claimable if you exceed the recommended distances

                int i;


                #endregion Check if mileage should not be claimable if you exceed the recommended distances

                if (subcat.calculation != CalculationType.FuelCardMileage)
                {
                    int carid = expitem.carid;

                    if (carid == 0)
                    {
                        carid = clsEmployeeCars.GetDefaultCarID(generalOptions.DutyOfCare.BlockTaxExpiry, generalOptions.DutyOfCare.BlockMOTExpiry, generalOptions.DutyOfCare.BlockInsuranceExpiry, generalOptions.DutyOfCare.BlockBreakdownCoverExpiry, accountProperties.DisableCarOutsideOfStartEndDate, expitem.date);
                    }

                    cCar car = clsEmployeeCars.GetValidCarWithinExpenseDate(expitem.carid, expitem.date);
                    if (subcat.calculation == CalculationType.PencePerMile)
                    {
                        //Make sure there is a valid car before allowing to claim mileage

                        if (car == null) 
                        {
                            return -3;
                        }

                        if (subcat.EnableHomeToLocationMileage && !car.ExemptFromHomeToOffice)
                        {
                            if (expitem.journeysteps.Count > 0)
                            {
                                foreach (KeyValuePair<int, cJourneyStep> kvp in expitem.journeysteps)
                                {
                                    Address start = kvp.Value.startlocation;
                                    Address end = kvp.Value.endlocation;

                                    if (start == null || end == null)
                                    {
                                        return -5;
                                    }

                                    if (start.Identifier == -100 || end.Identifier == -100)
                                    {
                                        return -5;
                                    }
                                }
                                calculateHomeToOfficeMileage(ref expitem, ref amountpay, ref total, ref clsvat, subcat, car, reqmileage, reqemp.EmployeeID);
                                amountpayableCalculated = true;
                            }
                        }
                    }

                    if (reqmileage.thresholdType == ThresholdType.Journey) //Journey calculation
                    {
                        total = clsmileagecats.CalculateVehicleJourneyTotal(currentUser, subcat, expitem, reqemp.EmployeeID, clsvat, ThresholdType.Journey);
                    }
                    else //Annual calculation
                    {
                        total = clsmileagecats.CalculateVehicleJourneyTotal(currentUser, subcat, expitem, reqemp.EmployeeID, clsvat, ThresholdType.Annual);
                    }

                    if (subcat.IsRelocationMileage)
                    {
                        if (expitem.quantity == 0)
                        {
                            total = 0;
                            expitem.updateVAT(0, 0, 0);
                            return 0;
                        }

                        amountpay = amountpay * (decimal)expitem.quantity;
                        total = total * (decimal)expitem.quantity;
                    }
                }

                if (total != 0)
                {
                    expitem.total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
                }
                if (amountpay != 0)
                {
                    expitem.amountpayable = Math.Round(amountpay, 2, MidpointRounding.AwayFromZero);
                }
            }



            if ((expitem.total == 0 && (subcat.calculation != CalculationType.PencePerMile && subcat.calculation != CalculationType.DailyAllowance && subcat.calculation != CalculationType.PencePerMileReceipt)) || ((expitem.miles == 0 || expitem.carid == 0) && (subcat.calculation == CalculationType.PencePerMile || subcat.calculation == CalculationType.PencePerMileReceipt)))
            {
                return 0;
            }

            if (expitem.primaryitem)
            {
                #region Convert the grandtotal for an expense item to sterling for use to compare against the converted split items total

                if (expitem.splititems.Count > 0)
                {
                    // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
                    cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
                    int subAccountID = subaccs.getFirstSubAccount().SubAccountID;

                    cCurrencies clscurrencies = new cCurrencies(accountid, subAccountID);
                    int basecurrency = reqemp.PrimaryCurrency != 0 ? reqemp.PrimaryCurrency : (int)generalOptions.Currency.BaseCurrency;

                    if (expitem.currencyid != basecurrency)
                    {
                        originalTotal = Math.Round(expitem.grandtotal * (1 / (decimal)expitem.exchangerate), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        originalTotal = expitem.grandtotal;
                    }

                    if (generalOptions.Currency.BaseCurrency == expitem.currencyid || generalOptions.Currency.BaseCurrency == expitem.basecurrency)
                    {
                        originalGlobalTotal = expitem.currencyid != generalOptions.Currency.BaseCurrency ? expitem.convertedgrandtotal : expitem.grandtotal;
                    }
                    else
                    {
                        double exchangeRate = 0;
                        if (expitem.floatid > 0)
                        {
                            cFloats clsFloats = new cFloats(accountid);
                            cFloat flt = clsFloats.GetFloatById(expitem.floatid);
                            exchangeRate = flt.exchangerate;
                        }
                        else if (basecurrency == expitem.currencyid)
                        {
                            exchangeRate = clscurrencies.getExchangeRate((int)generalOptions.Currency.BaseCurrency, expitem.currencyid, expitem.date);
                        }
                        else
                        {
                            exchangeRate = clscurrencies.getExchangeRate(basecurrency, expitem.currencyid, expitem.date);
                        }

                        originalGlobalTotal = Math.Round(expitem.grandtotal * (1 / (decimal)exchangeRate), 2, MidpointRounding.AwayFromZero);
                    }
                }

                #endregion

                //convert net to gross
                ConvertNetToGross(ref expitem);
                //convert back to base currency
                convertTotals(ref expitem, reqemp);
                //convert to global base currency
                convertGlobalTotals(ref expitem);


                if (expitem.splititems.Count > 0)
                {
                    foreach (cExpenseItem splititem in expitem.splititems)
                    {
                        cExpenseItem tempItem = splititem;
                        //convert net to gross
                        ConvertNetToGross(ref tempItem);
                        //convert back to base currency
                        convertTotals(ref tempItem, reqemp);
                        //convert to global base currency
                        convertGlobalTotals(ref tempItem);
                    }

                    decimal tempTotal = originalTotal;
                    decimal tempGlobalTotal = originalGlobalTotal;

                    foreach (cExpenseItem splititem in expitem.splititems)
                    {
                        tempTotal -= splititem.total;
                        tempGlobalTotal -= splititem.globaltotal;
                    }

                    expitem.total = tempTotal;
                    expitem.globaltotal = tempGlobalTotal;
                }
            }

            var flagManagement = new FlagManagement(nAccountid);
            List<FlagSummary> flags = flagManagement.CheckItemForFlags(expitem, reqemp, ValidationPoint.AddExpense, ValidationType.DoesNotRequireSave, subAccount, currentUser, claim);

            if (flagManagement.ContainsBlockedItem(flags) && !claim.submitted)
            {
                expitem.flags = flags;
                return -6;
            }

            splitEntertainment(ref expitem, null);
            string refnum = generateRefnum(reqemp.EmployeeID, currentUser);
            expitem.refnum = refnum;

            setAccountCode(ref expitem, subcat);
            if (expitem.allowanceid != 0)
            {
                cAllowances clsallowances = new cAllowances(accountid);
                clsallowances.calculateDailyAllowance(reqemp.EmployeeID, ref expitem, this._generalOptionsFactory);
            }

            clsvat.calculateVAT();



            if (amountpayableCalculated == false)
            {
                calculateAmountPayable(ref expitem, null, subcat);
            }

            decimal floatToAllocate = 0;
            cFloats clsfloats = new cFloats(accountid);

            if (expitem.floatid != 0)
            {
                cFloat reqfloat = clsfloats.GetFloatById(expitem.floatid);
                decimal floatallocation;

                if (!subcat.reimbursable) //If the item is non-reiumbursable, the user's float should not be affected
                {
                    floatallocation = 0;
                }
                else if (expitem.amountpayable == 0)
                {
                    floatallocation = expitem.total;
                }
                else
                {
                    floatallocation = expitem.total - expitem.amountpayable;
                }

                if (floatallocation > reqfloat.floatavailable)
                {
                    floatToAllocate = reqfloat.floatavailable;
                    expitem.amountpayable = floatallocation - reqfloat.floatavailable;
                }
                else
                {
                    floatToAllocate = floatallocation;
                    //expitem.amountpayable = reqfloat.floatavailable - floatallocation;
                }

                floatToAllocate = Math.Round(floatToAllocate, 2, MidpointRounding.AwayFromZero);
            }

            // make sure the validation progress matches the subcat.
            var claimStages = claims.GetSignoffStagesAsTypes(claim);
            expitem.DetermineValidationProgress(currentUser.Account, subcat.Validate, claimStages.Contains(SignoffType.SELValidation), new ExpenseValidationManager(accountid));

            if ((expitem.total != 0 || subcat.calculation == CalculationType.PencePerMileReceipt) || (expitem.total == 0 && subcat.calculation == CalculationType.PencePerMile))
            {
                expenseid = RunInsertSQL(ref expitem, reqemp.EmployeeID);
            }
            else
            {
                expenseid = 0;
            }

            if (expenseid == -7)
            {
                return expenseid;
            }

            if (expenseid != 0)
            {
                expitem.expenseid = expenseid;
                InsertCostCodeBreakdown(false, expitem);
                saveJourneySteps(expitem);
                if (expitem.floatid != 0)
                {
                    clsfloats.addAllocation(expitem.floatid, expitem.expenseid, floatToAllocate);
                }

                List<FlagSummary> requireSaveFlags = flagManagement.CheckItemForFlags(
                    expitem,
                    reqemp,
                    ValidationPoint.AddExpense,
                    ValidationType.RequiresSave, subAccount, currentUser);

                if (requireSaveFlags != null)
                {
                    foreach (FlagSummary i in requireSaveFlags)
                {
                        flags.Add(i);
                    }
                }

                if (flags.Count > 0)
                    {
                    expitem.flags = flags;
                    }

                expitem.flags = flags;

                cTables clstables = new cTables(accountid);
                cFields clsfields = new cFields(accountid);
                cTable tbl = clstables.GetTableByID(new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8"));

                clsuserdefined.SaveValues(clstables.GetTableByID(tbl.UserDefinedTableID), expenseid, expitem.userdefined, clstables, clsfields, currentUser);

                //if (expitem.itemtype == ItemType.CreditCard) //need to match items
                //{
                //    cCardUploads clsuploads = new cCardUploads(accountid);

                //    clsuploads.matchItems(expitem, expitem.transactionid);
                //}

                if (expitem.splititems.Count > 0)
                {
                    decimal tempTotal = expitem.grandtotal;
                    int counter = expitem.splititems.Count;

                    foreach (cExpenseItem splititem in expitem.splititems)
                    {
                        addItem(splititem, reqemp);
                        linkSplitItem(splititem);

                        tempTotal -= splititem.total;
                    }

                    decimal roundedTotal = (expitem.grandtotal - tempTotal);
                    expitem.splititems[counter - 1].total = decimal.Round(roundedTotal, 2);
                }
                cAuditLog clsaudit = addAuditRecord(reqemp.EmployeeID);
                //cAuditLog clsaudit = new cAuditLog(employeeid);
                clsaudit.addRecord(SpendManagementElement.Expenses, expitem.expenseid + "_" + expitem.date.ToShortDateString() + "_" + subcat.subcat + "_" + expitem.total.ToString("£###,##,##0.00"), expenseid);
            }

            if (expitem.mobileID != null)
            {
                cMobileDevices clsmobile = new cMobileDevices(accountid);
                clsmobile.saveReceiptFromMobile((int)expitem.mobileID, expitem.expenseid, expitem.claimid);
            }
            return expenseid;
        }



        /// <summary>
        /// Calculate Home to Office Mileage and amount to pay
        /// </summary>
        /// <param name="expItem">Expense Item to calculate</param>
        /// <param name="amountpay">Returns amount to pay</param>
        /// <param name="total">Returns Total</param>
        /// <param name="clsvat">Vat Class</param>
        /// <param name="subcat">Expense Category ti calculate against</param>
        /// <param name="car">The Employees Car</param>
        /// <param name="reqmileage">Mileage category for this caklculation</param>
        /// <param name="employeeid">Employee id to calculate for</param>
        public void calculateHomeToOfficeMileage(ref cExpenseItem expItem, ref decimal amountpay, ref decimal total, ref cVat clsvat, cSubcat subcat, cCar car, cMileageCat reqmileage, int employeeid)
        {
            cEmployees employees = new cEmployees(accountid);
            Employee employee = employees.GetEmployeeById(employeeid);
            var clsmileagecats = new cMileagecats(this.accountid);

            CurrentUser currentUser = cMisc.GetCurrentUser();
            var baseAddress = employee.GetWorkAddresses().GetBy(expItem.date, true);
            var homeAddress = employee.GetHomeAddresses().GetBy(expItem.date);
            int employeeHomeLocationId = homeAddress?.LocationID ?? 0;

            int employeeWorkLocationId = expItem.WorkAddressId;
            if (employeeWorkLocationId == 0)
            {
                var workAddress = employee.GetWorkAddresses().GetBy(currentUser, expItem.date, (int)expItem.ESRAssignmentId);
                employeeWorkLocationId = workAddress?.LocationID ?? 0;
            }
            

            var subAccount = new cAccountSubAccounts(currentUser.AccountID).getSubAccountById(currentUser.CurrentSubAccountId);
            cAccountProperties subAccountProperties = subAccount.SubAccountProperties;

            decimal? homeToOfficeDistance = null, officeToHomeDistance = null;
            if (subcat.HomeToLocationType == HomeToLocationType.JuniorDoctorRotation )
            {
                if (employeeHomeLocationId > 0 && baseAddress != null)
                {
                    var homeAddressRef = this._addresses.GetAddressById(employeeHomeLocationId);
                    var baseAddressRef = this._addresses.GetAddressById(baseAddress.LocationID);
                    homeToOfficeDistance = AddressDistance.GetRecommendedOrCustomDistance(homeAddressRef, baseAddressRef, currentUser.AccountID, subAccount, currentUser);
                    officeToHomeDistance = AddressDistance.GetRecommendedOrCustomDistance(baseAddressRef, homeAddressRef, currentUser.AccountID, subAccount, currentUser);
                }
            }
            else
            {
                if (employeeHomeLocationId > 0 && employeeWorkLocationId > 0)
                {
                    var homeAddressRef = this._addresses.GetAddressById(employeeHomeLocationId);
                    var workAddressRef = this._addresses.GetAddressById(employeeWorkLocationId);
                    homeToOfficeDistance = AddressDistance.GetRecommendedOrCustomDistance(homeAddressRef, workAddressRef, currentUser.AccountID, subAccount, currentUser);
                    officeToHomeDistance = AddressDistance.GetRecommendedOrCustomDistance(workAddressRef, homeAddressRef, currentUser.AccountID, subAccount, currentUser);
                }
            }

            if (homeToOfficeDistance.HasValue && officeToHomeDistance.HasValue)
            {
                this.IterateThroughStepsToCalculateHomeToOfficeMileage(ref expItem, subcat, car, employeeHomeLocationId, employeeWorkLocationId, (decimal)homeToOfficeDistance, (decimal)officeToHomeDistance, clsmileagecats, currentUser, baseAddress);
            }

            total = clsmileagecats.CalculateVehicleJourneyTotal(currentUser, subcat, expItem, employeeid, clsvat, reqmileage.thresholdType == ThresholdType.Journey ? ThresholdType.Journey : ThresholdType.Annual);

            amountpay = !subcat.reimbursable ? 0 : total;
        }

        /// <summary>
        /// Iterate through the journey steps in the Expense item and calculate mileage
        /// </summary>
        /// <param name="expItem">Expense Item to calculate</param>
        /// <param name="subcat">Expense Category ti calculate against</param>
        /// <param name="car">The Employees Car</param>
        /// <param name="employeeHomeLocationID">Employees Home Location ID</param>
        /// <param name="employeeWorkLocationID">Employees Work Location ID</param>
        /// <param name="homeToOfficeDistance"></param>
        /// <param name="officeToHomeDistance"></param>
        /// <param name="clsmileagecats">Mileage category to use</param>
        /// <param name="currentUser">The current user object</param>
        /// <param name="baseAddress">The base (or nominated base) for Junior Doctors Rotations.</param>
        public void IterateThroughStepsToCalculateHomeToOfficeMileage(ref cExpenseItem expItem, cSubcat subcat, cCar car, int employeeHomeLocationID, int employeeWorkLocationID, decimal homeToOfficeDistance, decimal officeToHomeDistance, cMileagecats clsmileagecats, ICurrentUser currentUser, cEmployeeWorkLocation baseAddress)
        {
            bool homeDiffDeductedOut = false;
            bool homeDiffDeductedReturn = false;

            Address homeAddress = Address.Get(currentUser.AccountID, employeeHomeLocationID);
            var stepHomeAddresses = new List<Tuple<int, Address, Address>>();

            bool international = currentUser.Account.AddressInternationalLookupsAndCoordinates;
            AddressLookupProvider provider = currentUser.Account.AddressLookupProvider;

            // if postcodes are being used to locate addresses,
            // replace any addresses with postcodes matching the home address's postcode
            // with the home address until the end of the method
            if (homeAddress != null && !string.IsNullOrWhiteSpace(homeAddress.Postcode))
            {
                string homePostcode = homeAddress.Postcode.Replace(" ", string.Empty);

                foreach (KeyValuePair<int, cJourneyStep> kvp in expItem.journeysteps)
                {
                    bool replaceStart = false;
                    bool replaceEnd = false;
                    Address start = kvp.Value.startlocation;
                    Address end = kvp.Value.endlocation;

                    if (start.Identifier != homeAddress.Identifier && start.GetLocatorType(international, provider) == Address.LocatorType.Postcode && start.Postcode.Replace(" ", string.Empty).Equals(homePostcode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        replaceStart = true;
                        expItem.journeysteps[kvp.Key].startlocation = homeAddress;
                    }

                    if (end.Identifier != homeAddress.Identifier && end.GetLocatorType(international, provider) == Address.LocatorType.Postcode && end.Postcode.Replace(" ", string.Empty).Equals(homePostcode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        replaceEnd = true;
                        expItem.journeysteps[kvp.Key].endlocation = homeAddress;
                    }

                    if (replaceStart || replaceEnd)
                    {
                        stepHomeAddresses.Add(new Tuple<int, Address, Address>(kvp.Key, replaceStart ? start : null, replaceEnd ? end : null));
                    }
                }
            }

            // perform the same replacement where any addresses have a postcode that matches the office address
            Address officeAddress = Address.Get(currentUser.AccountID, employeeWorkLocationID);
            var stepOfficeAddresses = new List<Tuple<int, Address, Address>>();

            if (officeAddress != null && !string.IsNullOrWhiteSpace(officeAddress.Postcode))
            {
                string officePostcode = officeAddress.Postcode.Replace(" ", string.Empty);

                foreach (KeyValuePair<int, cJourneyStep> kvp in expItem.journeysteps)
                {
                    bool replaceStart = false;
                    bool replaceEnd = false;
                    Address start = kvp.Value.startlocation;
                    Address end = kvp.Value.endlocation;

                    if (start.Identifier != officeAddress.Identifier && start.GetLocatorType(international, provider) == Address.LocatorType.Postcode && start.Postcode.Replace(" ", string.Empty).Equals(officePostcode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        replaceStart = true;
                        expItem.journeysteps[kvp.Key].startlocation = officeAddress;
                    }

                    if (end.Identifier != officeAddress.Identifier && end.GetLocatorType(international, provider) == Address.LocatorType.Postcode && end.Postcode.Replace(" ", string.Empty).Equals(officePostcode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        replaceEnd = true;
                        expItem.journeysteps[kvp.Key].endlocation = officeAddress;
                    }

                    if (replaceStart || replaceEnd)
                    {
                        stepOfficeAddresses.Add(new Tuple<int, Address, Address>(kvp.Key, replaceStart ? start : null, replaceEnd ? end : null));
                    }
                }
            }

            switch (subcat.HomeToLocationType)
            {
                case HomeToLocationType.DeductFullHomeToOfficeIfStartOrFinishHome:
                    expItem.journeysteps = new FullHomeToOfficeIfStartOrFinishHome(expItem.journeysteps, homeToOfficeDistance, officeToHomeDistance, employeeHomeLocationID, employeeWorkLocationID, subcat).Deduct();
                    break;
                case HomeToLocationType.DeductHomeToOfficeFromEveryJourney:
                    expItem.journeysteps = new HomeToOfficeOnce(expItem.journeysteps, homeToOfficeDistance, officeToHomeDistance, employeeHomeLocationID, employeeWorkLocationID, subcat).Deduct();
                    break;
                case HomeToLocationType.DeductHomeToOfficeEveryTimeHomeIsVisited:
                    expItem.journeysteps = new HomeToOfficeForEveryHomeVisit(expItem.journeysteps, homeToOfficeDistance, officeToHomeDistance, employeeHomeLocationID, employeeWorkLocationID, subcat).Deduct();
                    break;
                case HomeToLocationType.DeductHomeToOfficeIfStartOrFinishHome:
                    expItem.journeysteps = new HomeToOfficeIfStartOrFinishHome(expItem.journeysteps, homeToOfficeDistance, officeToHomeDistance, employeeHomeLocationID, employeeWorkLocationID, subcat).Deduct();
                    break;
                case HomeToLocationType.DeductFirstAndLastHome:
                    expItem.journeysteps = new HomeToOfficeForFirstAndLastHome(expItem.journeysteps, homeToOfficeDistance, officeToHomeDistance, employeeHomeLocationID, employeeWorkLocationID, subcat).Deduct();
                    break;
                case HomeToLocationType.DeductFullHomeToOfficeEveryTimeHomeIsVisited:
                    expItem.journeysteps = new FullHomeToOfficeForEveryHomeVisit(expItem.journeysteps, homeToOfficeDistance, officeToHomeDistance, employeeHomeLocationID, employeeWorkLocationID, subcat).Deduct();
                    break;
                case HomeToLocationType.DeductFixedMilesIfStartOrFinishHome:
                    expItem.journeysteps = new FixedDeductionIfStartOrFinishHome(expItem.journeysteps, homeToOfficeDistance, officeToHomeDistance, employeeHomeLocationID, employeeWorkLocationID, subcat).Deduct();
                    break;
                case HomeToLocationType.JuniorDoctorRotation:
                    expItem.journeysteps = new JuniorDoctorRotation(expItem.journeysteps, homeToOfficeDistance, officeToHomeDistance, employeeHomeLocationID, employeeWorkLocationID, subcat, baseAddress).Deduct();
                    break;
            }

            var subAccount = new cAccountSubAccounts(currentUser.AccountID).getSubAccountById(currentUser.CurrentSubAccountId);

            foreach (cJourneyStep step in expItem.journeysteps.Values)
            {
                if (step.startlocation != null && step.endlocation != null)
                {
                    switch (subcat.HomeToLocationType)
                    {
                        case HomeToLocationType.CalculateHomeAndOfficeToLocationDiff:
                            {
                                if (subcat.HomeToOfficeAlwaysZero)
                                {
                                    if ((step.startlocation.Identifier == employeeHomeLocationID && step.endlocation.Identifier == employeeWorkLocationID) || (step.startlocation.Identifier == employeeWorkLocationID && step.endlocation.Identifier == employeeHomeLocationID))
                                    {
                                        step.nummiles = 0;
                                    }
                                }

                                EnforceHomeToOfficeMileageCap(subcat, employeeHomeLocationID, employeeWorkLocationID, step);
                                decimal diff = 0;

                                if (step.startlocation.Identifier == employeeHomeLocationID)
                                {
                                    decimal hometolocationdist = step.nummiles;
                                    decimal officetolocationdist = AddressDistance.GetRecommendedOrCustomDistance(this._addresses.GetAddressById(employeeWorkLocationID), step.endlocation, currentUser.AccountID, subAccount, currentUser) ?? 0;

                                    if (hometolocationdist <= 0 || officetolocationdist <= 0)
                                    {
                                        break;
                                    }

                                    if (car.defaultuom == MileageUOM.KM)
                                    {
                                        hometolocationdist = clsmileagecats.convertMilesToKM(hometolocationdist);
                                        officetolocationdist = clsmileagecats.convertMilesToKM(officetolocationdist);
                                    }

                                    if (homeDiffDeductedOut == false)
                                    {
                                        if (hometolocationdist > officetolocationdist)
                                        {
                                            homeDiffDeductedOut = true;
                                            diff = hometolocationdist - officetolocationdist;
                                            step.nummiles = step.nummiles - diff;
                                        }
                                    }

                                }
                                else if (step.endlocation.Identifier == employeeHomeLocationID)
                                {
                                    decimal hometolocationdist = step.nummiles;
                                    decimal officetolocationdist = AddressDistance.GetRecommendedOrCustomDistance(step.startlocation, this._addresses.GetAddressById(employeeWorkLocationID), currentUser.AccountID, subAccount, currentUser) ?? 0;

                                    if (hometolocationdist <= 0 || officetolocationdist <= 0)
                                    {
                                        break;
                                    }
                                    if (car.defaultuom == MileageUOM.KM)
                                    {
                                        hometolocationdist = clsmileagecats.convertMilesToKM(hometolocationdist);
                                        officetolocationdist = clsmileagecats.convertMilesToKM(officetolocationdist);
                                    }

                                    if (homeDiffDeductedReturn == false)
                                    {
                                        if (hometolocationdist > officetolocationdist)
                                        {
                                            homeDiffDeductedReturn = true;
                                            diff = hometolocationdist - officetolocationdist;
                                            step.nummiles = step.nummiles - diff;
                                        }
                                    }
                                }
                                break;
                            }

                        case HomeToLocationType.FlagHomeAndOfficeToLocationDiff:

                            {
                                if (subcat.HomeToOfficeAlwaysZero)
                                {
                                    if ((step.startlocation.Identifier == employeeHomeLocationID && step.endlocation.Identifier == employeeWorkLocationID) || (step.startlocation.Identifier == employeeWorkLocationID && step.endlocation.Identifier == employeeHomeLocationID))
                                    {
                                        step.nummiles = 0;
                                    }
                                }

                                EnforceHomeToOfficeMileageCap(subcat, employeeHomeLocationID, employeeWorkLocationID, step);
                                break;
                            }
                        case HomeToLocationType.None:
                        {
                            EnforceHomeToOfficeMileageCap(subcat, employeeHomeLocationID, employeeWorkLocationID, step);
                            break;
                        }
                    }
                }
            }

            if (stepHomeAddresses.Count <= 0 && stepOfficeAddresses.Count <= 0)
            {
                return;
            }

            // put any substituted home addresses back
            foreach (Tuple<int, Address, Address> tuple in stepHomeAddresses)
            {
                if (tuple.Item2 != null)
                {
                    expItem.journeysteps[tuple.Item1].startlocation = tuple.Item2;
                }

                if (tuple.Item3 != null)
                {
                    expItem.journeysteps[tuple.Item1].endlocation = tuple.Item3;
                }
            }

            // put any substituted office addresses back
            foreach (Tuple<int, Address, Address> tuple in stepOfficeAddresses)
            {
                if (tuple.Item2 != null)
                {
                    expItem.journeysteps[tuple.Item1].startlocation = tuple.Item2;
                }

                if (tuple.Item3 != null)
                {
                    expItem.journeysteps[tuple.Item1].endlocation = tuple.Item3;
                }
            }

        }

      
        public cAuditLog addAuditRecord(int employeeid)
        {
            if (employeeid != 0)
            {
                cAuditLog clsaudit = new cAuditLog(accountid, employeeid);
                return clsaudit;
            }
            else
            {
                cAuditLog clsaudit = new cAuditLog();
                return clsaudit;
            }
        }

        private void linkSplitItem(cExpenseItem item)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "insert into savedexpenses_splititems (primaryitem, splititem) values (@primaryitem, @splititem)";
            expdata.sqlexecute.Parameters.AddWithValue("@primaryitem", item.parent.expenseid);
            expdata.sqlexecute.Parameters.AddWithValue("@splititem", item.expenseid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Updates an expense item.
        /// </summary>
        /// <param name="expitem">The expense item.</param>
        /// <param name="employeeid">The employee id.</param>
        /// <param name="oldclaimid">The old claim id</param>
        /// <param name="offlineitem">Whether it is offline.</param>
        /// <param name="reqclaim">An optional parameter for if the claim object which the expense is being updated to is already created</param>
        /// <returns>A code based on the state of the creation i.e. sucess, mileage exceeded etc.</returns>
        public int updateItem(cExpenseItem expitem, int employeeid, int oldclaimid, bool offlineitem, cClaim reqclaim = null)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cUserdefinedFields clsuserdefined = new cUserdefinedFields(accountid);
            cSubcats clssubcats = new cSubcats(accountid);
            cSubcat subcat = clssubcats.GetSubcatById(expitem.subcatid);
            bool vatchanged = false;
            cClaims clsclaims = new cClaims(accountid);

            if (reqclaim == null)
            {
                reqclaim = clsclaims.getClaimById(expitem.claimid);
            }

            cClaim oldclaim = null;
            bool amountpayableCalculated = false;
            decimal dtotal = 0;
            decimal amountpay = 0;
            decimal originalTotal = 0;
            decimal originalGlobalTotal = 0;
            cEmployees employees = new cEmployees(accountid);
            Employee reqemp = employees.GetEmployeeById(employeeid);

            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(accountid);
            cAccountSubAccount subAccount = clsSubAccounts.getFirstSubAccount();
            cAccountProperties clsProperties = subAccount.SubAccountProperties; // NEEDS SUBACCOUNTING

            if (expitem.reason.Length >= 2499)
            {
                expitem.reason = expitem.reason.Substring(0, 2498);
            }

            if (oldclaimid > 0)
            {
                oldclaim = (oldclaimid == reqclaim.claimid) ? reqclaim : clsclaims.getClaimById(oldclaimid);
            }

            cExpenseItem olditem = null;
            FlagManagement clsflags = new FlagManagement(nAccountid);
            if (expitem.primaryitem)
            {
                if (oldclaimid != expitem.claimid)
                {
                    moveExpenseItemToNewClaim(expitem, expitem.claimid, new List<int>());
                }
                if (oldclaim != null)
                {
                    olditem = clsclaims.getExpenseItemById(expitem.expenseid);
                }
                else
                {
                    olditem = clsclaims.getExpenseItemById(expitem.expenseid);
                }
            }
            else
            {
                if (oldclaim != null)
                {
                    olditem = clsclaims.getExpenseItemById(expitem.parent.expenseid);
                }
                else
                {
                    olditem = clsclaims.getExpenseItemById(expitem.parent.expenseid);
                }
                if (olditem == null) //split of split
                {
                    if (oldclaim != null)
                    {
                        olditem = clsclaims.getExpenseItemById(expitem.parent.expenseid);
                        if (olditem == null) //parent item moved
                        {
                            olditem = clsclaims.getExpenseItemById(expitem.parent.parent.expenseid);
                        }
                    }
                    else
                    {
                        olditem = clsclaims.getExpenseItemById(expitem.parent.parent.expenseid);
                    }
                    foreach (cExpenseItem splititem in olditem.splititems)
                    {
                        if (splititem.expenseid == expitem.parent.expenseid)
                        {
                            olditem = splititem;
                            break;
                        }
                    }

                }
                foreach (cExpenseItem splititem in olditem.splititems)
                {
                    if (splititem.expenseid == expitem.expenseid)
                    {
                        olditem = splititem;
                        break;
                    }
                }
            }

            if (olditem == null)
            {
                return 0;
            }
            List<int> arrexpenseids = new List<int>();
            arrexpenseids.Add(olditem.expenseid);
            FlaggedItemsManager oldflags = GetFlaggedItems(arrexpenseids);
            if (oldflags.Count > 0)
            {
                olditem.flags = oldflags.First;
            }

            if (olditem.vat.ToString("########0.00") != expitem.vat.ToString("########0.00") && subcat.calculation != CalculationType.PencePerMile || (olditem.date != expitem.date))
            {
                vatchanged = true;
            }

            if (expitem.allowanceid != 0)
            {
                cAllowances clsallowances = new cAllowances(accountid);
                clsallowances.calculateDailyAllowance(reqemp.EmployeeID, ref expitem, this._generalOptionsFactory);
            }

            if (subcat.calculation == CalculationType.ExcessMileage && expitem.journeysteps.ContainsKey(0))
            {
                expitem.journeysteps[0].NumActualMiles = (decimal)expitem.quantity * (decimal)reqemp.ExcessMileage;
                expitem.journeysteps[0].nummiles = (decimal)expitem.quantity * (decimal)reqemp.ExcessMileage;
            }

            cMileagecats clsmileagecats = new cMileagecats(accountid);

            cVat clsvat = new cVat(accountid, ref expitem, reqemp, clsmisc, olditem);

            cMileageCat reqmileage = clsmileagecats.GetMileageCatById(expitem.mileageid);

            cEmployeeCars clsEmployeeCars = new cEmployeeCars(accountid, employeeid);

            var generalOptions = this._generalOptionsFactory[cMisc.GetCurrentUser().CurrentSubAccountId].WithCurrency().WithDutyOfCare().WithMileage();

            if (reqmileage != null)
            {
                if (subcat.calculation != CalculationType.FuelCardMileage)
                {
                    int carid = expitem.carid;

                    if (carid == 0)
                    {
                        carid = clsEmployeeCars.GetDefaultCarID(clsProperties.BlockTaxExpiry, clsProperties.BlockMOTExpiry, clsProperties.BlockInsuranceExpiry, generalOptions.DutyOfCare.BlockBreakdownCoverExpiry, clsProperties.DisableCarOutsideOfStartEndDate, expitem.date);
                    }

                    cFieldToDisplay from = clsmisc.GetGeneralFieldByCode("from");
                    cFieldToDisplay to = clsmisc.GetGeneralFieldByCode("to");
                    var itemtype = expitem.itemtype;

                    if (subcat.mileageapp == true && (generalOptions.Mileage.AllowMultipleDestinations && subcat.fromapp && from.individual && subcat.toapp && to.individual && ((itemtype == ItemType.Cash && from.display && to.display) || (itemtype == ItemType.CreditCard && from.displaycc && to.displaycc) || (itemtype == ItemType.PurchaseCard && from.displaypc && to.displaypc))))
                    {
                        //Make sure there is a valid car before allowing to claim mileage
                        cCar car = clsEmployeeCars.GetValidCarWithinExpenseDate(expitem.carid, expitem.date);

                        if (car == null)
                        {
                            return -3;
                        }

                        int i;


                        if (subcat.EnableHomeToLocationMileage && !car.ExemptFromHomeToOffice)
                        {
                            if (expitem.journeysteps.Count > 0)
                            {
                                calculateHomeToOfficeMileage(ref expitem, ref amountpay, ref dtotal, ref clsvat, subcat, car, reqmileage, employeeid);
                                amountpayableCalculated = true;
                            }
                        }

                    }

                    if (reqmileage.thresholdType == ThresholdType.Journey) //Journey calculation
                    {
                        dtotal = clsmileagecats.CalculateVehicleJourneyTotal(currentUser, subcat, expitem, employeeid, clsvat, ThresholdType.Journey);
                    }
                    else //Annual calculation
                    {
                        dtotal = clsmileagecats.CalculateVehicleJourneyTotal(currentUser, subcat, expitem, employeeid, clsvat, ThresholdType.Annual);
                    }

                    if (subcat.IsRelocationMileage)
                    {
                        if (expitem.quantity == 0)
                        {
                            dtotal = 0;
                            expitem.updateVAT(0, 0, 0);

                        }
                        else
                        {
                            amountpay = amountpay * (decimal)expitem.quantity;
                            dtotal = dtotal * (decimal)expitem.quantity;
                        }
                    }
                }
            }

            bool recalculateVat = (olditem.vat != expitem.vat || olditem.receipt != expitem.receipt || olditem.home != expitem.home
                                   || olditem.total != expitem.total || olditem.tip != expitem.tip || olditem.miles != expitem.miles
                                   || olditem.nopassengers != expitem.nopassengers || olditem.countryid != expitem.countryid
                                   || olditem.currencyid != expitem.currencyid || olditem.foreignvat != expitem.foreignvat 
                                   || olditem.others != expitem.others) 
                                   && vatchanged == false
                                  && (subcat.calculation != CalculationType.PencePerMileReceipt || olditem.date != expitem.date);

            if (!recalculateVat)
            {
            if (expitem.total > 0)
            {
                if (expitem.vat > expitem.total)
                {
                    return -1;
                }
            }
            else
            {
                if (expitem.vat < expitem.total || expitem.vat > 0)
                {
                    return -1;
                }
            }
            }


            if (dtotal != 0)
            {
                expitem.total = Math.Round(dtotal, 2, MidpointRounding.AwayFromZero);
            }
            if (amountpay != 0)
            {
                expitem.amountpayable = Math.Round(amountpay, 2, MidpointRounding.AwayFromZero);
            }

            if (expitem.primaryitem)
            {
                #region Convert the grandtotal for an expense item to sterling for use to compare against the converted split items total

                if (expitem.splititems.Count > 0)
                {
                    int basecurrency;
                    double exchangeRate = 0;
                    // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
                    cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
                    int subAccountID = subaccs.getFirstSubAccount().SubAccountID;

                    cCurrencies clscurrencies = new cCurrencies(accountid, subAccountID);

                    if (reqemp.PrimaryCurrency != 0)
                    {
                        basecurrency = reqemp.PrimaryCurrency;
                    }
                    else
                    {
                        basecurrency = (int)generalOptions.Currency.BaseCurrency;
                    }

                    if (expitem.currencyid != basecurrency)
                    {
                        originalTotal = Math.Round(expitem.grandtotal * (1 / (decimal)expitem.exchangerate), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        originalTotal = expitem.grandtotal;
                    }

                    if (generalOptions.Currency.BaseCurrency == expitem.currencyid || generalOptions.Currency.BaseCurrency == expitem.basecurrency)
                    {
                        if (expitem.currencyid != generalOptions.Currency.BaseCurrency)
                        {
                            originalGlobalTotal = Math.Round(expitem.grandtotal * (1 / (decimal)expitem.exchangerate), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            originalGlobalTotal = expitem.grandtotal;
                        }
                    }
                    else
                    {
                        if (expitem.floatid > 0)
                        {
                            cFloats clsFloats = new cFloats(accountid);
                            cFloat flt = clsFloats.GetFloatById(expitem.floatid);
                            exchangeRate = flt.exchangerate;
                        }
                        else
                        {
                            exchangeRate = clscurrencies.getExchangeRate((int)generalOptions.Currency.BaseCurrency, expitem.currencyid, expitem.date);
                        }

                        if (exchangeRate > 0)
                        {
                            originalGlobalTotal = Math.Round(expitem.grandtotal * (1 / (decimal)exchangeRate), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            originalGlobalTotal = expitem.grandtotal;
                        }
                    }
                }

                #endregion

                convertTotals(ref expitem, reqemp);  //convert to sterling if necessary
                convertGlobalTotals(ref expitem);

                cExpenseItem tempItem;

                if (expitem.splititems.Count > 0)
                {
                    foreach (cExpenseItem splititem in expitem.splititems)
                    {
                        tempItem = splititem;
                        convertTotals(ref tempItem, reqemp);
                        convertGlobalTotals(ref tempItem);
                    }

                    decimal tempTotal = originalTotal;
                    decimal tempGlobalTotal = originalGlobalTotal;

                    foreach (cExpenseItem splititem in expitem.splititems)
                    {
                        tempTotal -= splititem.total;
                        tempGlobalTotal -= splititem.globaltotal;
                    }

                    expitem.total = tempTotal;
                    expitem.globaltotal = tempGlobalTotal;
                }
            }

            splitEntertainment(ref expitem, olditem);
            setAccountCode(ref expitem, subcat);

            if (recalculateVat)
            {
                clsvat.calculateVAT();
            }
            else
            {
                decimal total = expitem.total;
                decimal vat = expitem.vat;
                decimal net = total - vat;
                expitem.updateVAT(net, vat, total);

            }

            if (amountpayableCalculated == false)
            {
                calculateAmountPayable(ref expitem, olditem, subcat);
            }

            
            cFloats clsfloats = new cFloats(accountid);
            cFloat reqfloat;

            if (expitem.floatid == 0)
            {
                if (olditem.floatid > 0)
                {
                    reqfloat = clsfloats.GetFloatById(olditem.floatid);
                    clsfloats.deleteAllocation(expitem.expenseid, reqfloat.floatid);
                }
            }
            else
            {
                reqfloat = clsfloats.GetFloatById(expitem.floatid);

                decimal floatallocation;
                if (expitem.amountpayable == 0)
                {
                    floatallocation = expitem.total;
                }
                else
                {
                    floatallocation = expitem.total - expitem.amountpayable;
                }

                decimal floatToAllocate = 0;

                if (floatallocation-olditem.total > reqfloat.floatavailable) //Ensure the check takes into account the old item allocation that will be returned
                {
                    floatToAllocate = reqfloat.floatavailable;
                    expitem.amountpayable = floatallocation - reqfloat.floatavailable;
                }
                else
                {
                    floatToAllocate = floatallocation;
                    //expitem.amountpayable = reqfloat.floatavailable - floatallocation;
                }

                floatToAllocate = Math.Round(floatToAllocate, 2, MidpointRounding.AwayFromZero);

                if (olditem.floatid > 0 && olditem.floatid != expitem.floatid)
                {
                    clsfloats.deleteAllocation(olditem.expenseid, olditem.floatid);
                }

                if (reqfloat.allocations.ContainsKey(expitem.expenseid))
                {
                    ////add already used float value to total
                    //floatToAllocate = floatToAllocate + reqfloat.floatused;

                    ////If greater than available only allow the portion available to be allocated and the rest added to amount payable
                    //if (floatToAllocate > reqfloat.floatavailable)
                    //{
                    //    decimal floatForAmountPay = floatToAllocate - reqfloat.floatavailable;
                    //    expitem.amountpayable += floatForAmountPay;
                    //    floatToAllocate = reqfloat.floatavailable;
                    //}
                    //floatToAllocate = Math.Round(floatToAllocate, 2);
                    clsfloats.updateAllocation(expitem.floatid, expitem.expenseid, floatToAllocate);
                }
                else
                {
                    clsfloats.addAllocation(expitem.floatid, expitem.expenseid, floatToAllocate);
                }
            }

            this.ReturnToValidationStageIfAny(expitem, currentUser, clsclaims, reqclaim, subcat);

            var returnVal = RunUpdateSQL(ref expitem, employeeid);
            if (returnVal == -7)
            {
                return returnVal;
            }
            saveJourneySteps(expitem);
            InsertCostCodeBreakdown(true, expitem);


            if (expitem.returned && reqclaim.splitApprovalStage)
            {
                // may need to update the item checker for cost code ownership or assignment supervisors
                // only one will run as they check the signoff type
                clsclaims.UpdateItemCheckers(reqclaim);

                clsclaims.UpdateItemCheckers(reqclaim, ItemCheckerType.AssignmentSupervisor);
            }




            if (expitem.returned) //add return comment
            {
                clsclaims.updateReturned(reqclaim, expitem, currentUser);
            }
            cTables clstables = new cTables(accountid);
            cFields clsfields = new cFields(accountid);
            cTable tbl = clstables.GetTableByID(new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8"));

            string itemDesc = string.Format("{0}_{1}_{2}_{3}", olditem.expenseid, olditem.date.ToShortDateString(), subcat.subcat, olditem.total.ToString("£###,##,##0.00"));
            clsuserdefined.SaveValues(clstables.GetTableByID(tbl.UserDefinedTableID), expitem.expenseid, expitem.userdefined, clstables, clsfields, currentUser, elementId: (int)SpendManagementElement.Expenses, record: itemDesc);

            if (expitem.splititems.Count != 0)
            {
                System.Data.SqlClient.SqlDataReader reader;
                int expid = 0;
                decimal tempTotal = expitem.grandtotal;
                int counter = expitem.splititems.Count;

                foreach (cExpenseItem splititem in expitem.splititems)
                {
                    if (offlineitem)
                    {
                        strsql = "SELECT expenseid FROM savedexpenses WHERE expenseid = @expenseid";
                        expdata.sqlexecute.Parameters.AddWithValue("@expenseid", splititem.expenseid);
                        using (reader = expdata.GetReader(strsql))
                        {
                            while (reader.Read())
                            {
                                expid = reader.GetInt32(reader.GetOrdinal("expenseid"));
                            }
                            reader.Close();
                        }
                    }

                    tempTotal -= splititem.total;

                    if (splititem.expenseid == 0 && !offlineitem || expid == 0 && offlineitem) //new item
                    {
                        addItem(splititem, reqemp);
                        if (splititem.expenseid > 0)
                        {
                            linkSplitItem(splititem);
                        }
                    }
                    else
                    {
                        updateItem(splititem, employeeid, oldclaimid, offlineitem);
                    }
                }
                decimal roundedTotal = (expitem.grandtotal - tempTotal);
                expitem.splititems[counter - 1].total = decimal.Round(roundedTotal, 2);
            }

            if (expitem.splititems.Count < olditem.splititems.Count) //split has reduced so delete
            {
                bool delete = false;
                foreach (cExpenseItem splititem in olditem.splititems)
                {
                    delete = true;
                    foreach (cExpenseItem checkitem in expitem.splititems)
                    {
                        if (checkitem.expenseid == splititem.expenseid)
                        {
                            delete = false;
                            break;
                        }
                    }
                    if (delete)
                    {
                        clsclaims.deleteExpense(reqclaim, splititem, false, currentUser, true);
                    }
                }
            }

            List<FlagSummary> flags = clsflags.CheckItemForFlags(expitem, reqemp, ValidationPoint.AddExpense, ValidationType.Any, subAccount, currentUser, reqclaim);

            if (clsflags.ContainsBlockedItem(flags) && !reqclaim.submitted)
            {
                expitem.flags = flags;
                //put the item back as it was
                RunUpdateSQL(ref olditem, employeeid);
                saveJourneySteps(olditem);
                InsertCostCodeBreakdown(true, olditem);
                return -6;
            }

            if (flags.Count > 0)
            {
                clsflags.FlagItem(expitem.expenseid, flags);
            }

            if (olditem.flags != null)
            {
                clsflags.CheckExpenseForRedundantItems(olditem, ref flags);
            }

            expitem.flags = flags;
            //if (expitem.transactionid > 0) //need to match items
            //{
            //    cCardStatements clsstatements = new cCardStatements(accountid);
            //    cCardTransaction transaction = clsstatements.getTransactionById(expitem.transactionid);
            //    cCardStatement statement = clsstatements.getStatementById(transaction.transactionid);
            //    clsstatements.matchTransaction(statement, transaction, expitem);

            //}

            updateAuditLog(olditem, ref expitem, employeeid, itemDesc);
            return 0;
        }

        /// <summary>
        /// The return to validation stage if any.
        /// </summary>
        /// <param name="expenseitem">
        /// The expense item.
        /// </param>
        /// <param name="currentUser">
        /// The current user.
        /// </param>
        /// <param name="claims">
        /// The claims object
        /// </param>
        /// <param name="claim">
        /// The claim to check.
        /// </param>
        /// <param name="subcat">
        /// The current subcategory.
        /// </param>
        public void ReturnToValidationStageIfAny(
            cExpenseItem expenseitem,
            ICurrentUser currentUser,
            cClaims claims,
            cClaim claim,
            cSubcat subcat)
        {
            // if the item has validation enabled, update the claim history to indicate invalidation of validation results.
            if (currentUser.Account.ValidationServiceEnabled
                && (expenseitem.ValidationProgress >= ExpenseValidationProgress.WaitingForClaimant))
            {
                var validationManager = new ExpenseValidationManager(this.accountid);
                var claimStages = claims.GetSignoffStagesAsTypes(claim);
                var isCurrentlyInValidation = claims.HasClaimPreviouslyPassedSelStage(
                    claim,
                    SignoffType.SELValidation,
                    claimStages,
                    true);
                var result = expenseitem.DetermineValidationProgress(
                    currentUser.Account,
                    subcat.Validate,
                    claimStages.Contains(SignoffType.SELValidation),
                    validationManager);

                if (isCurrentlyInValidation)
                {
                    expenseitem.corrected = true;

                    // Attempt to reset the validation - the result will dictate whether this was allowed or not.
                    result = expenseitem.ResetValidation(validationManager, isCurrentlyInValidation);

                    // if it got reset, then clear the corrected flag.
                    if (result == ExpenseValidationProgress.Required)
                    {
                        expenseitem.corrected = false;
                    }
                }
                else
                {
                    // see if the item has previously passed validation
                    if (claims.HasClaimPreviouslyPassedSelStage(claim, SignoffType.SELValidation, claimStages))
                    {
                        var oldResult = expenseitem.ValidationProgress;
                        result = expenseitem.DetermineValidationProgressValidity(true);
                        validationManager.UpdateProgressForExpenseItem(expenseitem.expenseid, oldResult, result);
                    }
                }

                // update the history
                var notice = result == ExpenseValidationProgress.Required
                                 ? "Expense will be marked for receipt re-validation."
                                 : "Previous validation results are no longer applicable.";
                claims.UpdateClaimHistory(claim, notice, currentUser.EmployeeID, expenseitem.refnum);
            }
        }

        private void moveExpenseItemToNewClaim(cExpenseItem item, int claimid, List<int> ids)
        {

            ids.Add(item.expenseid);
            foreach (cExpenseItem splitItem in item.splititems)
            {
                moveExpenseItemToNewClaim(splitItem, claimid, ids);
            }

            if (item.primaryitem)
            {
                DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
                string sql = "update savedexpenses set claimid = @claimid where ";
                foreach (int i in ids)
                {
                    sql += "expenseid = " + i + " or ";
                }
                if (ids.Count > 0)
                {
                    sql = sql.Remove(sql.Length - 4, 4);
                }
                data.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
                data.ExecuteSQL(sql);
                data.sqlexecute.Parameters.Clear();
            }
        }


        private void updateAuditLog(cExpenseItem olditem, ref cExpenseItem newitem, int employeeid, string itemDesc)
        {
            cAuditLog clsaudit = addAuditRecord(employeeid);

            // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
            int subAccountID = subaccs.getFirstSubAccount().SubAccountID;

            cSubcats clssubcats = new cSubcats(accountid);
            string oldval = string.Empty, newval = string.Empty;

            SubcatBasic reqsubcat = clssubcats.GetSubcatBasic(olditem.subcatid);
            SubcatBasic newsubcat = clssubcats.GetSubcatBasic(newitem.subcatid);
            string itemdesc = olditem.expenseid + "_" + olditem.date.ToShortDateString() + "_" + reqsubcat.Subcat + "_" + olditem.total.ToString("£###,##,##0.00");
            if (olditem.claimid != newitem.claimid)
            {
                cClaims clsclaims = new cClaims(accountid);
                if (olditem.claimid == 0)
                {
                    oldval = string.Empty;
                }
                else
                {
                    cClaim oldclaim = clsclaims.getClaimById(olditem.claimid);
                    oldval = oldclaim.name;
                }
                if (newitem.claimid == 0)
                {
                    newval = string.Empty;
                }
                else
                {
                    cClaim newclaim = clsclaims.getClaimById(newitem.claimid);
                    newval = newclaim.name;
                }
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("71474E06-AB80-482C-88E4-5941B71D06B1"), oldval, newval);
            }

            if (reqsubcat.CategoryId != newsubcat.CategoryId)
            {
                cCategories clscategories = new cCategories(accountid);

                cCategory oldcat = clscategories.FindById(reqsubcat.CategoryId);
                cCategory newcat = clscategories.FindById(newsubcat.CategoryId);
                clsaudit.editRecord(olditem.expenseid, itemdesc, SpendManagementElement.Expenses, new Guid("44C2A53A-DB33-45AF-AF66-D0055AAD48EC"), oldcat.category, newcat.category);
            }
            if (olditem.miles != newitem.miles)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("B8102A2A-9471-4327-B325-46DF1B550B8B"), olditem.miles.ToString(), newitem.miles.ToString());
            }
            if (olditem.bmiles != newitem.bmiles)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("31737A96-DF40-45E3-9BBE-1DF96FEA5E1A"), olditem.bmiles.ToString(), newitem.bmiles.ToString());
            }
            if (olditem.pmiles != newitem.pmiles)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("B66687B3-5D65-4F87-AAF2-D1B86CE4A5EC"), olditem.pmiles.ToString(), newitem.pmiles.ToString());
            }
            if (olditem.reason != newitem.reason)
            {
                string sNewReason = newitem.reason;
                string sOldReason = olditem.reason;

                if (sOldReason.Length >= 1999)
                {
                    sOldReason = sOldReason.Substring(0, 1999);
                }

                if (sNewReason.Length >= 1999)
                {
                    sNewReason = sNewReason.Substring(0, 1999);
                }
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("7CF61909-8D25-4230-84A9-F5701268F94B"), sOldReason, sNewReason);
            }
            if (olditem.receipt != newitem.receipt)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("A4FB5521-728A-45D0-8669-C36724FCDF46"), olditem.receipt.ToString(), newitem.receipt.ToString());
            }
            if (olditem.net != newitem.net)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("28B402FB-E030-49A2-9128-825859CE0A11"), olditem.net.ToString("£###,##,##0.00"), newitem.net.ToString("£###,##,##0.00"));
            }
            if (olditem.vat != newitem.vat)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("EBC742BC-7D95-448B-87E1-7464F4D279DD"), olditem.vat.ToString("£###,##,##0.00"), newitem.vat.ToString("£###,##,##0.00"));
            }
            if (olditem.total != newitem.total)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("C3C64EB9-C0E1-4B53-8BE9-627128C55011"), olditem.total.ToString("£###,##,##0.00"), newitem.total.ToString("£###,##,##0.00"));
            }
            if (olditem.subcatid != newitem.subcatid)
            {
                SubcatBasic oldsubcat = clssubcats.GetSubcatBasic(olditem.subcatid);

                clsaudit.editRecord(olditem.expenseid, itemdesc, SpendManagementElement.Expenses, new Guid("ABFE0BB2-E6AC-40D0-88CE-C5F7B043924D"), oldsubcat.Subcat, newsubcat.Subcat);
            }
            if (olditem.date != newitem.date)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("A52B4423-C766-47BB-8BF3-489400946B4C"), olditem.date.ToShortDateString(), newitem.date.ToShortDateString());
            }
            if (olditem.staff != newitem.staff)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("D40D314B-DBF3-4B43-83EF-1A895ED6CF4D"), olditem.staff.ToString(), newitem.staff.ToString());
            }
            if (olditem.others != newitem.others)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("22A8321D-52E6-479F-81F1-9BE511AD53C4"), olditem.others.ToString(), newitem.others.ToString());
            }

            if (olditem.companyid != newitem.companyid)
            {
                if (olditem.companyid == 0)
                {
                    oldval = string.Empty;
                }
                else
                {
                    Organisation oldOrganisation = Organisation.Get(accountid, olditem.companyid);

                    if (oldOrganisation != null)
                    {
                        oldval = oldOrganisation.Name;
                    }
                }

                if (newitem.companyid == 0)
                {
                    newval = string.Empty;
                }
                else
                {
                    Organisation newOrganisation = Organisation.Get(accountid, newitem.companyid);

                    if (newOrganisation != null)
                    {
                        newval = newOrganisation.Name;
                    }
                }

                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("41539361-DB17-4427-85CA-5C9221720AD6"), oldval, newval);
            }
            if (olditem.home != newitem.home)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("CBCD077A-C8BC-494E-85B1-9AF2CC6E16F9"), olditem.home.ToString(), newitem.home.ToString());
            }

            if (olditem.plitres != newitem.plitres)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("05E4BC81-30F2-4BA3-AFD3-97EC303E90B7"), olditem.plitres.ToString(), newitem.plitres.ToString());
            }
            if (olditem.blitres != newitem.blitres)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("1233C7BD-A1A4-4D5D-9ED3-5D1531E0F338"), olditem.blitres.ToString(), newitem.blitres.ToString());
            }
            //			if (olditem.allowanceamount != newitem.allowanceamount)
            //			{
            //				clsaudit.editRecord(itemdesc,"Allowance Amount",auditcat,olditem.allowanceamount.ToString(),newitem.allowanceamount.ToString());
            //			}
            if (olditem.currencyid != newitem.currencyid)
            {
                cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
                cCurrencies clscurrencies = new cCurrencies(accountid, subAccountID);
                if (olditem.currencyid == 0)
                {
                    oldval = string.Empty;
                }
                else
                {
                    cCurrency oldcur = clscurrencies.getCurrencyById(olditem.currencyid);
                    oldval = clsglobalcurrencies.getGlobalCurrencyById(oldcur.globalcurrencyid).label;
                }
                if (newitem.currencyid == 0)
                {
                    newval = string.Empty;
                }
                else
                {
                    cCurrency newcur = clscurrencies.getCurrencyById(newitem.currencyid);
                    newval = clsglobalcurrencies.getGlobalCurrencyById(newcur.globalcurrencyid).label;
                }
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("1EE53AE2-2CDF-41B4-9081-1789ADF03459"), oldval, newval);
            }
            if (olditem.attendees != newitem.attendees && newitem.attendees != null)
            {

                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("392CE424-50EF-42EE-8BDB-27BFC7EBFF92"), olditem.attendees, newitem.attendees);
            }
            if (olditem.tip != newitem.tip)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("591FAEF1-EABE-4EA9-B095-BC2473801B85"), olditem.tip.ToString("£###,##,##0.00"), newitem.tip.ToString("£###,##,##0.00"));
            }
            if (olditem.countryid != newitem.countryid)
            {
                cCountries clscountries = new cCountries(accountid, subAccountID);
                cGlobalCountries clsglobalcountries = new cGlobalCountries();
                if (olditem.countryid == 0)
                {
                    oldval = string.Empty;
                }
                else
                {
                    cCountry oldcountry = clscountries.getCountryById(olditem.countryid);
                    oldval = clsglobalcountries.getGlobalCountryById(oldcountry.GlobalCountryId).Country;
                }
                if (newitem.countryid == 0)
                {
                    newval = string.Empty;
                }
                else
                {
                    cCountry newcountry = clscountries.getCountryById(newitem.countryid);
                    newval = clsglobalcountries.getGlobalCountryById(newcountry.GlobalCountryId).Country;
                }
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("EC527561-DFEE-48C7-A126-0910F8E031B0"), oldval, newval);
            }
            if (olditem.foreignvat != newitem.foreignvat)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("629E54C7-D9AD-47CB-9E0F-D7865F60F695"), olditem.foreignvat.ToString(), newitem.foreignvat.ToString());
            }
            if (olditem.convertedtotal != newitem.convertedtotal)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("1D92D5D0-64A7-4136-B4F3-B0B7F1055B35"), olditem.convertedtotal.ToString(), newitem.convertedtotal.ToString());
            }
            if (olditem.exchangerate != newitem.exchangerate)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("4091311D-4363-46CB-95B6-6EDA2261EACF"), olditem.exchangerate.ToString(), newitem.exchangerate.ToString());
            }
            if (olditem.normalreceipt != newitem.normalreceipt)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("B52C86F2-FEFC-465C-9163-A6E29A57061A"), olditem.normalreceipt.ToString(), newitem.normalreceipt.ToString());
            }
            if (olditem.floatid != newitem.floatid)
            {
                cFloats clsfloats = new cFloats(accountid);
                if (olditem.floatid == 0)
                {
                    oldval = string.Empty;
                }
                else
                {
                    cFloat oldfloat = clsfloats.GetFloatById(olditem.floatid);
                    oldval = oldfloat.name;
                }
                if (newitem.floatid == 0)
                {
                    newval = string.Empty;
                }
                else
                {
                    cFloat newfloat = clsfloats.GetFloatById(newitem.floatid);
                    newval = newfloat.name;
                }
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("32DC3E56-B72E-460E-A707-646A7541AC70"), oldval, newval);
            }

            if (olditem.reasonid != newitem.reasonid)
            {
                cReasons clsreasons = new cReasons(accountid);
                if (olditem.reasonid == 0)
                {
                    oldval = string.Empty;
                }
                else
                {
                    cReason oldreas = clsreasons.getReasonById(olditem.reasonid);
                    oldval = oldreas.reason;
                }
                if (newitem.reasonid == 0)
                {
                    newval = string.Empty;
                }
                else
                {
                    cReason newreas = clsreasons.getReasonById(newitem.reasonid);
                    newval = newreas.reason;
                }
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("AF839FE7-8A52-4BD1-962C-8A87F22D4A10"), oldval, newval);
            }

            if (olditem.fromid != newitem.fromid)
            {

                if (olditem.fromid == 0)
                {
                    oldval = string.Empty;
                }
                else
                {
                    Address oldfrom = Address.Get(accountid, olditem.fromid);
                    if (oldfrom != null)
                    {
                        oldval = oldfrom.FriendlyName;
                    }
                    else
                    {
                        oldval = string.Empty;
                    }
                }
                if (newitem.fromid == 0)
                {
                    newval = string.Empty;
                }
                else
                {
                    Address newfrom = Address.Get(accountid, newitem.fromid);
                    newval = newfrom.FriendlyName;
                }
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("7CD556BF-57F8-45CC-8E20-87CE33A14552"), oldval, newval);
            }

            if (olditem.allowancestartdate != newitem.allowancestartdate)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("F934CFFE-E79F-4242-9DE8-76A6BB086A41"), olditem.allowancestartdate.ToString(), newitem.allowancestartdate.ToString());
            }
            if (olditem.allowanceenddate != newitem.allowanceenddate)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("DF9BD0E9-DEA2-4CED-A0E4-0FD19594C74E"), olditem.allowanceenddate.ToString(), newitem.allowanceenddate.ToString());
            }
            if (olditem.nopassengers != newitem.nopassengers)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("1987DE58-39DF-43BE-8E5E-FD5EB6519B00"), olditem.nopassengers.ToString(), newitem.nopassengers.ToString());
            }
            if (olditem.carid != newitem.carid)
            {
                cEmployeeCars clsEmployeeCars = new cEmployeeCars(accountid, employeeid);

                if (olditem.carid == 0)
                {
                    oldval = string.Empty;
                }
                else
                {
                    cCar oldcar = clsEmployeeCars.GetCarByID(olditem.carid);
                    if (oldcar != null)
                    {
                        oldval = oldcar.make + " " + oldcar.model;
                    }
                    else
                    {
                        oldval = string.Empty;
                    }
                }
                if (newitem.carid == 0)
                {
                    newval = string.Empty;
                }
                else
                {
                    cCar newcar = clsEmployeeCars.GetCarByID(newitem.carid);
                    if (newcar != null)
                    {
                        newval = newcar.make + " " + newcar.model;
                    }
                    else
                    {
                        newval = string.Empty;
                    }
                }
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("BCEBD759-DCC2-4C0A-96D7-7B64DD752CCC"), oldval, newval);
            }
            if (olditem.allowancededuct != newitem.allowancededuct)
            {
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("61A41EB9-A050-4C06-A9CA-FA0B3E741B1C"), olditem.allowancededuct.ToString("£###,##,##0.00"), newitem.allowancededuct.ToString("£###,##,##0.00"));
            }
            if (olditem.allowanceid != newitem.allowanceid)
            {
                cAllowances clsallowances = new cAllowances(accountid);
                cAllowance oldallowance = clsallowances.getAllowanceById(olditem.allowanceid);
                cAllowance newallowance = clsallowances.getAllowanceById(newitem.allowanceid);
                if (newallowance != null)
                {
                    newval = newallowance.allowance;
                }
                else
                {
                    newval = string.Empty;
                }
                if (oldallowance != null)
                {
                    oldval = oldallowance.allowance;
                }
                else
                {
                    oldval = string.Empty;
                }
                clsaudit.editRecord(olditem.expenseid, itemDesc, SpendManagementElement.Expenses, new Guid("74BEC6A1-5520-46BC-96D1-759200BC206F"), oldval, newval);
            }

        }
        private string generateRefnum(int employeeid, CurrentUser currentUser)
        {
            cEmployees clsemployees = new cEmployees(accountid);
            Employee reqemp = clsemployees.GetEmployeeById(employeeid);
            string strcurrefnum = string.Empty;
            string refnum = string.Empty;
            refnum = reqemp.EmployeeID + "-";
            strcurrefnum = reqemp.CurrentReferenceNumber.ToString("000000");
            refnum += strcurrefnum;
            reqemp.IncrementAndSaveCurrentExpenseItemReferenceNumber(currentUser);
            return refnum;
        }

        private void saveJourneySteps(cExpenseItem expenseitem)
        {
            if (expenseitem.journeysteps != null)
            {
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                string strsql;
                strsql = "delete from savedexpenses_journey_steps_passengers where expenseid = @expenseid " +
                         "delete from savedexpenses_journey_steps where expenseid = @expenseid";
                expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseitem.expenseid);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();

                foreach (cJourneyStep step in expenseitem.journeysteps.Values)
                {
                    strsql = "insert into savedexpenses_journey_steps (expenseid, step_number, StartAddressID, EndAddressID, num_miles,num_passengers,numActualMiles, heavyBulkyEquipment) " +
                        "values (@expenseid, @stepnumber, @startlocation, @endlocation, @nummiles, @numpassengers, @numactualmiles, @heavyBulkyEquipment)";
                    expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseitem.expenseid);
                    expdata.sqlexecute.Parameters.AddWithValue("@stepnumber", step.stepnumber);
                    if (step.startlocation != null)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@startlocation", step.startlocation.Identifier);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@startlocation", DBNull.Value);
                    }
                    if (step.endlocation != null)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@endlocation", step.endlocation.Identifier);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@endlocation", DBNull.Value);
                    }
                    expdata.AddWithValue("@nummiles", step.nummiles, 18, 2);
                    if (step.numpassengers > 0)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@numpassengers", step.numpassengers);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@numpassengers", DBNull.Value);
                    }
                    expdata.AddWithValue("@numactualmiles", step.NumActualMiles, 18, 2);
                    expdata.sqlexecute.Parameters.AddWithValue("@heavyBulkyEquipment", step.heavyBulkyEquipment);
                    expdata.ExecuteSQL(strsql);

                    if (step.passengers != null) foreach (var passenger in step.passengers)
                        {
                            const string insertPassengerSql =
                                @"insert savedexpenses_journey_steps_passengers " +
                                    "(expenseid, step_number, employeeid, name) " +
                                @"values(@expenseid,@step_number,@employeeid,@name)";
                            expdata.sqlexecute.Parameters.Clear();
                            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseitem.expenseid);
                            expdata.sqlexecute.Parameters.AddWithValue("@step_number", step.stepnumber);
                            object employeeIdParamValue;
                            object passengerNameParamValue;
                            if (passenger.EmployeeId == 0 || passenger.EmployeeId == null)
                            {
                                employeeIdParamValue = DBNull.Value;
                                passengerNameParamValue = (object)passenger.Name;
                            }
                            else
                            {
                                employeeIdParamValue = passenger.EmployeeId;
                                passengerNameParamValue = DBNull.Value;
                            }
                            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeIdParamValue);
                            expdata.sqlexecute.Parameters.AddWithValue("@name", passengerNameParamValue);
                            expdata.ExecuteSQL(insertPassengerSql);
                        }
                    expdata.sqlexecute.Parameters.Clear();
                }
            }
        }

        /// <summary>
        /// Insert an expense item to the database
        /// </summary>
        /// <param name="expenseitem">The expense item</param>
        /// <param name="userid">The user Id</param>
        /// <returns>The expense Id</returns>
        private int RunInsertSQL(ref cExpenseItem expenseitem, int userid)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("Method RunInsertSQL has started");
            }

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            DateTime createdon = DateTime.Now.ToUniversalTime();
            expenseitem.createdon = createdon;

            expdata.sqlexecute.Parameters.AddWithValue("@claimid", expenseitem.claimid);
            expdata.AddWithValue("@bmiles", expenseitem.bmiles, 18, 2);
            //expdata.sqlexecute.Parameters.AddWithValue("@bmiles", expenseitem.bmiles);
            expdata.AddWithValue("@pmiles", expenseitem.pmiles, 18, 2);
            //expdata.sqlexecute.Parameters.AddWithValue("@pmiles", expenseitem.pmiles);

            if (expenseitem.reason == null)
            {
                expdata.AddWithValue("@reason", string.Empty, fields.GetFieldSize("savedexpenses", "reason"));
            }
            else
            {
                expdata.AddWithValue("@reason", expenseitem.reason, fields.GetFieldSize("savedexpenses", "reason"));
            }
            expdata.sqlexecute.Parameters.AddWithValue("@receipt", Convert.ToByte(expenseitem.receipt));
            expdata.sqlexecute.Parameters.AddWithValue("@net", expenseitem.net);
            expdata.sqlexecute.Parameters.AddWithValue("@vat", expenseitem.vat);
            expdata.sqlexecute.Parameters.AddWithValue("@total", expenseitem.total);
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", expenseitem.subcatid);
            expdata.sqlexecute.Parameters.AddWithValue("@date", expenseitem.date); //.Year + "/" + expenseitem.date.Month + "/" + expenseitem.date.Day);
            expdata.sqlexecute.Parameters.AddWithValue("@staff", expenseitem.staff);
            expdata.sqlexecute.Parameters.AddWithValue("@others", expenseitem.others);

            if (expenseitem.companyid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@companyid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@companyid", expenseitem.companyid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@home", Convert.ToByte(expenseitem.home));
            expdata.AddWithValue("@refnum", expenseitem.refnum, fields.GetFieldSize("savedexpenses", "refnum"));
            expdata.sqlexecute.Parameters.AddWithValue("@plitres", expenseitem.plitres);
            expdata.sqlexecute.Parameters.AddWithValue("@blitres", expenseitem.blitres);
            expdata.sqlexecute.Parameters.AddWithValue("@allowanceamount", expenseitem.total);
            if (expenseitem.currencyid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", expenseitem.currencyid);
            }
            expdata.sqlexecute.Parameters.Add("@attendees", SqlDbType.NVarChar, 1000);
            if (expenseitem.attendees == null)
            {
                expdata.sqlexecute.Parameters["@attendees"].Value = DBNull.Value;
            }
            else
            {
                expdata.sqlexecute.Parameters["@attendees"].Value = expenseitem.attendees;
            }
            expdata.sqlexecute.Parameters.AddWithValue("@tip", expenseitem.tip);
            if (expenseitem.countryid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@countryid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@countryid", expenseitem.countryid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@foreignvat", expenseitem.foreignvat);
            expdata.sqlexecute.Parameters.AddWithValue("@convertedtotal", expenseitem.convertedtotal);
            expdata.sqlexecute.Parameters.AddWithValue("@exchangerate", expenseitem.exchangerate);
            expdata.sqlexecute.Parameters.AddWithValue("@normalreceipt", Convert.ToByte(expenseitem.normalreceipt));

            if (expenseitem.reasonid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@reasonid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@reasonid", expenseitem.reasonid);
            }


            if (expenseitem.allowancestartdate.Date <= DateTime.Parse("01/01/1900"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowancestartdate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowancestartdate", expenseitem.allowancestartdate); //.Year + "/" + expenseitem.allowancestartdate.Month + "/" + expenseitem.allowancestartdate.Day + " " + expenseitem.allowancestartdate.Hour + ":" + expenseitem.allowancestartdate.Minute);
            }
            if (expenseitem.allowanceenddate.Date <= DateTime.Parse("01/01/1900"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceenddate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceenddate", expenseitem.allowanceenddate); //.Year + "/" + expenseitem.allowanceenddate.Month + "/" + expenseitem.allowanceenddate.Day + " " + expenseitem.allowanceenddate.Hour + ":" + expenseitem.allowanceenddate.Minute);
            }

            if (expenseitem.carid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@carid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@carid", expenseitem.carid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@allowancededuct", expenseitem.allowancededuct);
            if (expenseitem.allowanceid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceid", expenseitem.allowanceid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@amountpayable", expenseitem.amountpayable);
            expdata.sqlexecute.Parameters.AddWithValue("@nonights", expenseitem.nonights);
            expdata.sqlexecute.Parameters.AddWithValue("@quantity", expenseitem.quantity);
            expdata.sqlexecute.Parameters.AddWithValue("@directors", expenseitem.directors);
            if (expenseitem.hotelid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hotelid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hotelid", expenseitem.hotelid);
            }

            if (expenseitem.vatnumber == null)
            {
                expdata.AddWithValue("@vatnumber", DBNull.Value, fields.GetFieldSize("savedexpenses", "vatnumber"));
            }
            else
            {
                expdata.AddWithValue("@vatnumber", expenseitem.vatnumber, fields.GetFieldSize("savedexpenses", "vatnumber"));
            }
            expdata.sqlexecute.Parameters.AddWithValue("@primaryitem", Convert.ToByte(expenseitem.primaryitem));
            expdata.sqlexecute.Parameters.AddWithValue("@norooms", expenseitem.norooms);
            expdata.sqlexecute.Parameters.AddWithValue("@personalguests", expenseitem.personalguests);
            expdata.sqlexecute.Parameters.AddWithValue("@remoteworkers", expenseitem.remoteworkers);

            if (expenseitem.accountcode != null)
            {
                expdata.AddWithValue("@accountcode", expenseitem.accountcode, fields.GetFieldSize("savedexpenses", "accountcode"));
            }
            else
            {

                expdata.AddWithValue("@accountcode", DBNull.Value, fields.GetFieldSize("savedexpenses", "accountcode"));
            }

            //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;


            expdata.sqlexecute.Parameters.AddWithValue("@basecurrency", expenseitem.basecurrency);
            expdata.sqlexecute.Parameters.AddWithValue("@globalexchangerate", expenseitem.globalexchangerate);
            expdata.sqlexecute.Parameters.AddWithValue("@globalbasecurrency", expenseitem.globalbasecurrency);
            expdata.sqlexecute.Parameters.AddWithValue("@globaltotal", expenseitem.globaltotal);

            expdata.sqlexecute.Parameters.AddWithValue("@itemtype", (byte)expenseitem.itemtype);
            expdata.sqlexecute.Parameters.AddWithValue("@savedDate", createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", userid);
            if (expenseitem.mileageid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mileageid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mileageid", expenseitem.mileageid);
            }
            if (expenseitem.transactionid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@transactionid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@transactionid", expenseitem.transactionid);
            }
            if (expenseitem.mileageid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@journey_unit", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@journey_unit", expenseitem.journeyunit);
            }
            if (expenseitem.ESRAssignmentId == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@assignmentnum", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@assignmentnum", expenseitem.ESRAssignmentId);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@hometooffice_deduction_method", (byte)expenseitem.homeToOfficeDeductionMethod);
            expdata.sqlexecute.Parameters.AddWithValue("@addedAsMobileItem", expenseitem.addedAsMobileExpense);
            if (expenseitem.addedByMobileDeviceTypeId == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@addedByDeviceTypeId", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@addedByDeviceTypeId", expenseitem.addedByMobileDeviceTypeId);
            }

            // add the validation progress marker.
            expdata.sqlexecute.Parameters.AddWithValue("@validationProgress", (int)expenseitem.ValidationProgress);
            expdata.sqlexecute.Parameters.AddWithValue("@validationCount", DBNull.Value);

            if (expenseitem.bankAccountId == 0 || expenseitem.bankAccountId == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@BankAccountId", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@BankAccountId", expenseitem.bankAccountId);
            }

            if (expenseitem.WorkAddressId == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@WorkAddressId", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@WorkAddressId", expenseitem.WorkAddressId);
            }

            if (expenseitem.MobileMetricDeviceId == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@MobileMetricDeviceId", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@MobileMetricDeviceId", expenseitem.MobileMetricDeviceId);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", 0);

            expdata.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("saveExpenseItem");
            int expenseid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            if (Log.IsDebugEnabled)
            {
                Log.Debug($"An expense item has been added the expense Id is {expenseid} and the bank account Id is {expenseitem.bankAccountId}");
                Log.Debug("Method RunInsertSQL has completed");
            }

            return expenseid;
        }


        /// <summary>
        /// Updates an expense item
        /// </summary>
        /// <param name="expenseitem">The expense item</param>
        /// <param name="userid">The user Id</param>
        /// <returns>A return code from the store proc</returns>
        private int RunUpdateSQL(ref cExpenseItem expenseitem, int userid)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug("Method RunUpdateSQL has started");
            }
            
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            DateTime modifiedon = DateTime.Now.ToUniversalTime();

            expdata.sqlexecute.Parameters.AddWithValue("@claimid", expenseitem.claimid);
            expdata.AddWithValue("@bmiles", expenseitem.bmiles, 18, 2);
            expdata.AddWithValue("@pmiles", expenseitem.pmiles, 18, 2);
            if (expenseitem.reason.Length > 2499)
            {
                expdata.AddWithValue("@reason", expenseitem.reason.Substring(0, 2499), fields.GetFieldSize("savedexpenses", "reason"));
            }
            else
            {
                expdata.AddWithValue("@reason", expenseitem.reason, fields.GetFieldSize("savedexpenses", "reason"));
            }
            expdata.sqlexecute.Parameters.AddWithValue("@receipt", Convert.ToByte(expenseitem.receipt));
            expdata.sqlexecute.Parameters.AddWithValue("@net", expenseitem.net);
            expdata.sqlexecute.Parameters.AddWithValue("@vat", expenseitem.vat);
            expdata.sqlexecute.Parameters.AddWithValue("@total", expenseitem.total);
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", expenseitem.subcatid);
            expdata.sqlexecute.Parameters.AddWithValue("@date", expenseitem.date); //.Year + "/" + expenseitem.date.Month + "/" + expenseitem.date.Day);
            expdata.sqlexecute.Parameters.AddWithValue("@staff", expenseitem.staff);
            expdata.sqlexecute.Parameters.AddWithValue("@others", expenseitem.others);

            if (expenseitem.companyid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@companyid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@companyid", expenseitem.companyid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@home", Convert.ToByte(expenseitem.home));
            expdata.AddWithValue("@refnum", expenseitem.refnum, fields.GetFieldSize("savedexpenses", "refnum"));

            expdata.sqlexecute.Parameters.AddWithValue("@plitres", expenseitem.plitres);
            expdata.sqlexecute.Parameters.AddWithValue("@blitres", expenseitem.blitres);
            expdata.sqlexecute.Parameters.AddWithValue("@allowanceamount", expenseitem.total);
            if (expenseitem.currencyid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", expenseitem.currencyid);
            }

            expdata.sqlexecute.Parameters.Add("@attendees", SqlDbType.NVarChar, 1000);
            if (expenseitem.attendees == null)
            {
                expdata.sqlexecute.Parameters["@attendees"].Value = DBNull.Value;
            }
            else
            {
                expdata.sqlexecute.Parameters["@attendees"].Value = expenseitem.attendees;
            }


            expdata.sqlexecute.Parameters.AddWithValue("@tip", expenseitem.tip);
            if (expenseitem.countryid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@countryid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@countryid", expenseitem.countryid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@foreignvat", expenseitem.foreignvat);
            expdata.sqlexecute.Parameters.AddWithValue("@convertedtotal", expenseitem.convertedtotal);
            expdata.sqlexecute.Parameters.AddWithValue("@exchangerate", expenseitem.exchangerate);
            expdata.sqlexecute.Parameters.AddWithValue("@normalreceipt", Convert.ToByte(expenseitem.normalreceipt));
            if (expenseitem.vatnumber == null)
            {
                expdata.AddWithValue("@vatnumber", DBNull.Value, fields.GetFieldSize("savedexpenses", "vatnumber"));
            }
            else
            {
                expdata.AddWithValue("@vatnumber", expenseitem.vatnumber, fields.GetFieldSize("savedexpenses", "vatnumber"));
            }

            if (expenseitem.reasonid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@reasonid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@reasonid", expenseitem.reasonid);
            }


            if (expenseitem.allowancestartdate.Date <= DateTime.Parse("01/01/1900"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowancestartdate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowancestartdate", expenseitem.allowancestartdate); //.Year + "/" + expenseitem.allowancestartdate.Month + "/" + expenseitem.allowancestartdate.Day + " " + expenseitem.allowancestartdate.Hour + ":" + expenseitem.allowancestartdate.Minute);
            }
            if (expenseitem.allowanceenddate.Date <= DateTime.Parse("01/01/1900"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceenddate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceenddate", expenseitem.allowanceenddate); //.Year + "/" + expenseitem.allowanceenddate.Month + "/" + expenseitem.allowanceenddate.Day + " " + expenseitem.allowanceenddate.Hour + ":" + expenseitem.allowanceenddate.Minute);
            }
            if (expenseitem.carid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@carid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@carid", expenseitem.carid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@allowancededuct", expenseitem.allowancededuct);
            if (expenseitem.allowanceid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceid", expenseitem.allowanceid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@nonights", expenseitem.nonights);
            expdata.sqlexecute.Parameters.AddWithValue("@quantity", expenseitem.quantity);
            expdata.sqlexecute.Parameters.AddWithValue("@directors", expenseitem.directors);
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseitem.expenseid);
            expdata.sqlexecute.Parameters.AddWithValue("@amountpayable", expenseitem.amountpayable);
            expdata.sqlexecute.Parameters.AddWithValue("@personalguests", expenseitem.personalguests);
            expdata.sqlexecute.Parameters.AddWithValue("@remoteworkers", expenseitem.remoteworkers);
            if (expenseitem.hotelid != 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hotelid", expenseitem.hotelid);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hotelid", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@norooms", expenseitem.norooms);
            expdata.sqlexecute.Parameters.AddWithValue("@primaryitem", Convert.ToByte(expenseitem.primaryitem));
            if (expenseitem.accountcode == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@accountcode", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@accountcode", expenseitem.accountcode);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@basecurrency", expenseitem.basecurrency);
            expdata.sqlexecute.Parameters.AddWithValue("@globalexchangerate", expenseitem.globalexchangerate);
            expdata.sqlexecute.Parameters.AddWithValue("@globalbasecurrency", expenseitem.globalbasecurrency);
            expdata.sqlexecute.Parameters.AddWithValue("@globaltotal", expenseitem.globaltotal);

            expdata.sqlexecute.Parameters.AddWithValue("@savedDate", modifiedon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", userid);
            if (expenseitem.mileageid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mileageid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mileageid", expenseitem.mileageid);
            }
            if (expenseitem.transactionid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@transactionid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@transactionid", expenseitem.transactionid);
            }
            if (expenseitem.mileageid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@journey_unit", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@journey_unit", expenseitem.journeyunit);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@itemtype", (byte)expenseitem.itemtype);
            if (expenseitem.ESRAssignmentId == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@assignmentnum", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@assignmentnum", expenseitem.ESRAssignmentId);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@hometooffice_deduction_method", (byte)expenseitem.homeToOfficeDeductionMethod);
            expdata.sqlexecute.Parameters.AddWithValue("@addedAsMobileItem", expenseitem.addedAsMobileExpense);
            expdata.sqlexecute.Parameters.AddWithValue("@addedByDeviceTypeId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@MobileMetricDeviceId", DBNull.Value);

            // add the validation progress marker.
            expdata.sqlexecute.Parameters.AddWithValue("@validationProgress", (int)expenseitem.ValidationProgress);
            expdata.sqlexecute.Parameters.AddWithValue("@validationCount", expenseitem.ValidationCount);

            if (expenseitem.bankAccountId == 0 || expenseitem.bankAccountId == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@BankAccountId", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@BankAccountId", expenseitem.bankAccountId);
            }

            if (expenseitem.WorkAddressId == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@WorkAddressId", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@WorkAddressId", expenseitem.WorkAddressId);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("saveExpenseItem");
            int returnVal = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            if (Log.IsDebugEnabled)
            {
                Log.Debug($"An expense item has been updated the expense Id is {expenseitem.expenseid}, the bank account Id is {expenseitem.bankAccountId}");
                Log.Debug("Method RunUpdateSQL has completed");
            }

            return returnVal;
        }

        /// <summary>
        /// Generates a claim grid.
        /// </summary>
        /// <param name="employeeId">The employeeID of the user generating the grid</param>
        /// <param name="claim">The claim the grid is generated for</param>
        /// <param name="gridName">The name of the grid</param>
        /// <param name="viewType">Determines which view the claim is on, current, submitted, previous</param>
        /// <param name="filter">Filter by cash, credit card, purchase card</param>
        /// <param name="printView">Whether it is the view for printing</param>
        /// <param name="enableReceiptAttachments">Sets whether the receipt attachment link should be shown on the grid</param>
        /// <param name="enableJourneyDetailsLink">Sets whether the journey details link should be shown on the grid</param>
        /// <param name="enableCorporateCardLink">Sets whether the corporate card view/unmatch transaction link should be shown on the grid</param>
        /// <param name="defaultCurrencySymbol">Symbol for the default curreency</param>
        /// <param name="itemState">State of the item</param>
        /// <param name="claimEmployeeId">The employeeId of the claim owner</param>
        /// <param name="fromClaimSelector">States whether from the claim selector or not</param>
        /// <param name="claimStage">Current stage of the claim</param>
        /// <returns></returns>
        public string[] generateClaimGrid(int employeeId, cClaim claim, string gridName, UserView viewType, Filter filter, bool printView, bool enableReceiptAttachments, bool enableJourneyDetailsLink, bool enableCorporateCardLink, string defaultCurrencySymbol, ItemState itemState = ItemState.Approved, int claimEmployeeId = 0, bool fromClaimSelector = false, ClaimStage claimStage = ClaimStage.Any)
        {
            var claimId = claim.claimid;
            bool defaultView;
            var clsemployees = new cEmployees(accountid);
            SortedList<int, cField> viewFields = clsemployees.getFieldsForView(printView, employeeId, viewType, out defaultView, true);
            var account = new cAccounts().GetAccountByID(accountid);

            if (enableJourneyDetailsLink) //remove from, to
            {
                for (int x = viewFields.Count - 1; x >= 0; x--)
                {
                    if (viewFields.Values[x].FieldID == new Guid("2CF623AE-B9CA-4298-95EB-E43B201C9EB6") || viewFields.Values[x].FieldID == new Guid("B0A89FBD-641B-4D77-967B-2702CFC1787F"))
                    {
                        viewFields.RemoveAt(x);
                    }
                }
            }

            var claims = new cClaims(this.accountid);
            var employee = clsemployees.GetEmployeeById(claim.employeeid);
            var claimStages = claims.GetSignoffStagesAsTypes(claim, null, employee);
            var groupContainsValidation = claimStages.Contains(SignoffType.SELValidation);

            var columns = new List<cNewGridColumn>();
            cFieldToDisplay clsfield;
            var clsmisc = new cMisc(accountid);


            var fields = new cFields(accountid);
            columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("A528DE93-3037-46F6-974C-A76BD0C8642A")))); //expenseid
            columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("1993e202-4cda-4941-ac75-76a97d1beb45")))); //hasflags

            if (account.ValidationServiceEnabled && groupContainsValidation)
            {
                if (viewType == UserView.CheckAndPay)
                {
                    var claimExpenseItems = claims.getExpenseItemsFromDB(claim.claimid);
                    var groups = new cGroups(this.accountid);
                    var group = claims.GetGroupForClaim(groups, claim);
                    this.currentStage = group.stages.Values[claim.stage - 1];
                    this.payBeforeValidateStage = ClaimSubmission.GetPayBeforeValidateStage(group);
                    this.expenseItems = claimExpenseItems.Values.ToList();
                }

                var validationColumn = new cFieldColumn(fields.GetFieldByID(Guid.Parse("B7F7A4AB-0016-4207-A17D-6CBC114884AB"))); //ValidationProgress
                columns.Add(validationColumn);
            }

            columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("C144109C-84E4-4005-A967-8857CF398041")))); //maxflagcolour
            columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("f7e1e306-0f56-4237-ac06-a72eaa79f6b1")))); //hasSplitItems
            columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("9045c0d2-ceb1-4e5f-8fc4-557b5c60c192")))); //floatid
            columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("b4e34975-5615-4be9-ba71-146e3a65e471")))); //transactionid

            if (viewType == UserView.Submitted || (viewType == UserView.CheckAndPay && itemState == ItemState.Returned))
            {
                columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("d351f38d-0046-42ec-9571-36152cc02119")))); //return
                columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("45c5f424-1e30-4047-a683-de8cc710b0ec")))); //corrected
            }

            cField fromField = null, fromIDField = null;
            cField toField = null, toIDField = null;

            foreach (cField field in viewFields.Values)
            {
                var column = new cFieldColumn(field, field.FieldID.ToString());

                if (field.ListItems.Count > 0)
                {
                    foreach (KeyValuePair<object, string> item in field.ListItems)
                    {
                        column.addValueListItem(Convert.ToInt32(item.Key), item.Value);
                    }
                }

                columns.Add(column);

                if (field.FieldID.Equals(Guid.Parse("2CF623AE-B9CA-4298-95EB-E43B201C9EB6"))) //addressFrom
                {
                    fromField = field;
                    fromIDField = fields.GetFieldByID(Guid.Parse("45167B8B-281B-4F2A-B728-9AE90B72A43A"));
                    var fromIdColumn = new cFieldColumn(fromIDField);
                    columns.Add(fromIdColumn); //addressFromId
                }
                if (field.FieldID.Equals(Guid.Parse("B0A89FBD-641B-4D77-967B-2702CFC1787F"))) //addressTo
                {
                    toField = field;
                    toIDField = fields.GetFieldByID(Guid.Parse("93E97A44-0F8E-43E2-B7BA-1772AD80003B"));
                    columns.Add(new cFieldColumn(toIDField)); //addressToId
                }
            }

            if (viewType == UserView.CheckAndPay && this.currentStage != null && this.currentStage.IsPostValidationCleanupStage && account.ValidationServiceEnabled && groupContainsValidation)
            {
                columns.Add(new cFieldColumn(fields.GetFieldByID(new Guid("591BBE93-B190-47BA-9739-52E3C0B4FA53"))));
                columns[columns.Count - 1].HeaderText = "Previously Paid";
            }

            var tables = new cTables(accountid);
            cTable baseTable = tables.GetTableByID(Guid.Parse("d70d9e5f-37e2-4025-9492-3bcf6aa746a8"));

            var grid = new cGridNew(accountid, employeeId, gridName, baseTable, columns);

            var gridInfo = new SerializableDictionary<string, object> { { "claimId", claimId }, { "viewType", (int)viewType }, { "gridName", gridName }, { "claimEmployeeId", claimEmployeeId }, { "fromClaimSelector", fromClaimSelector } };
            grid.InitialiseRowGridInfo = gridInfo;
            grid.InitialiseRow += this.claimGrid_InitialiseRow;
            grid.ServiceClassForInitialiseRowEvent = "Spend_Management.cExpenseItems";
            grid.ServiceClassMethodForInitialiseRowEvent = "claimGrid_InitialiseRow";

            grid.DefaultCurrencySymbol = defaultCurrencySymbol;

            if (viewType == UserView.CheckAndPay)
            {
                switch (itemState)
                {
                    case ItemState.Unapproved:
                        grid.WhereClause = "savedexpenses.claimid = @claimid and savedexpenses.tempallow = 0 and savedexpenses.returned = 0 and savedexpenses.primaryitem = 1 and ISNULL(edited, 0) <> @edit";
                        grid.EmptyText = "There are no items to approve on this claim.";
                        break;
                    case ItemState.Returned:
                        grid.WhereClause = "savedexpenses.claimid = @claimid and savedexpenses.returned = 1 and savedexpenses.primaryitem = 1 and ISNULL(edited, 0) <> @edit";
                        grid.EmptyText = "No items have been returned for amendment on this claim.";
                        break;
                    case ItemState.Approved:
                        grid.WhereClause = "savedexpenses.claimid = @claimid and savedexpenses.tempallow = 1 and savedexpenses.returned = 0 and savedexpenses.primaryitem = 1 and ISNULL(edited, 0) <> @edit";
                        grid.EmptyText = "No items have been approved on this claim.";
                        break;
                }

                grid.WhereClause += "  and ((claims.splitApprovalStage = 0 and itemCheckerId is null) or (claims.splitApprovalStage = 1 and itemCheckerId = @checkerId))";
                grid.addFilter(fields.GetFieldByID(Guid.Parse("E3AF2B67-A613-437E-AABF-6853C4553977")), "@claimId", claimId);
                grid.addFilter(fields.GetFieldByID(Guid.Parse("A48C7721-BDEC-4BF8-A390-DC626730D3D8")), "@checkerId", employeeId);
                grid.addFilter(fields.GetFieldByID(Guid.Parse("AD41FEE2-D25C-4615-901A-A27B329D9EC1")), "@edit", 1);
            }
            else
            {
                grid.addFilter(fields.GetFieldByID(Guid.Parse("34012174-7CE8-4F67-8B91-6C44AC1A4845")), ConditionType.Equals, new object[] { claimId }, null, ConditionJoiner.None);
                grid.addFilter(fields.GetFieldByID(Guid.Parse("B1A38E6B-1F4C-4429-AF1E-195B75E6081C")), ConditionType.Equals, new object[] { 1 }, null, ConditionJoiner.And); // primary item
                if (!fromClaimSelector)
                {
                    grid.addFilter(
                        fields.GetFieldByID(Guid.Parse("AD41FEE2-D25C-4615-901A-A27B329D9EC1")),
                        ConditionType.DoesNotEqual,
                        new object[] { 1 },
                        null,
                        ConditionJoiner.And); // Edited Paid items
                }

                if (filter != Filter.None)
                {
                    grid.addFilter(fields.GetFieldByID(Guid.Parse("975454BF-3269-4903-8C3E-6962C5AFC181")), ConditionType.Equals, new object[] { (byte)filter }, null, ConditionJoiner.And);
                }

                grid.EmptyText = "There are no expense items to display.";
            }

            grid.KeyField = "expenseid";
            grid.getColumnByName("expenseid").hidden = true;
            grid.getColumnByName("hasFlags").hidden = true;

            if (account.ValidationServiceEnabled && groupContainsValidation)
            {
                grid.getColumnByName("ValidationProgress").hidden = true;
            }

            grid.getColumnByName("hasSplitItems").hidden = true;
            grid.getColumnByName("floatid").hidden = true;
            grid.getColumnByName("transactionid").hidden = true;
            grid.getColumnByName("flagColour").hidden = true;

            if (viewType == UserView.Submitted)
            {
                grid.getColumnByName("returned").hidden = true;
                grid.getColumnByName("corrected").hidden = true;
            }

            var clsSubAccounts = new cAccountSubAccounts(this.accountid);
            cAccountProperties reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties;
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (viewType == UserView.Current || viewType == UserView.Submitted ||
                (viewType == UserView.CheckAndPay && (itemState == ItemState.Unapproved || itemState == ItemState.Returned)) ||
                 (fromClaimSelector && reqProperties.EditPreviousClaims && claimStage == ClaimStage.Previous &&
                    currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ClaimViewer, true, false) && employeeId != claimEmployeeId))
            {
                grid.enableupdating = true;
                if (viewType != UserView.Previous && !claim.PayBeforeValidate)
                {
                    grid.enabledeleting = true;
                }

                if (viewType == UserView.CheckAndPay)
                {
                    grid.editlink = "../aeexpense.aspx?returnto=3&claimid=" + claimId + "&employeeid=" + employeeId + "&action=2&expenseid={expenseid}";
                }
                else
                {
                    grid.editlink = "../aeexpense.aspx?action=2&claimid=" + claimId + "&expenseid={expenseid}" + (fromClaimSelector ? "&claimSelector=1" : string.Empty);
                }

                if (viewType != UserView.Previous)
                {
                    if (viewType == UserView.CheckAndPay)
                    {
                        grid.deletelink = "javascript:SEL.Claims.ClaimViewer.DeleteExpenseAsApprover({expenseid});";
                    }
                    else
                    {
                        grid.deletelink = "javascript:SEL.Claims.ClaimViewer.DeleteExpense({expenseid}, true);";
                    }
                }
            }

            if (viewType == UserView.CheckAndPayPrint || viewType == UserView.CurrentPrint || viewType == UserView.PreviousPrint || viewType == UserView.SubmittedPrint)
            {
                grid.enablepaging = false;
            }



            grid.EnableSorting = true;
            grid.showfooters = true;

            #region custom headers
            if (grid.getColumnByID(Guid.Parse("4D0F2409-0705-4F0F-9824-42057B25AEBE")) != null)
            {
                clsfield = clsmisc.GetGeneralFieldByCode("organisation");
                grid.getColumnByID(Guid.Parse("4D0F2409-0705-4F0F-9824-42057B25AEBE")).HeaderText = clsfield.description;
            }

            if (grid.getColumnByID(Guid.Parse("2cf623ae-b9ca-4298-95eb-e43b201c9eb6")) != null)
            {
                clsfield = clsmisc.GetGeneralFieldByCode("from");
                grid.getColumnByID(Guid.Parse("2cf623ae-b9ca-4298-95eb-e43b201c9eb6")).HeaderText = clsfield.description;
            }

            if (grid.getColumnByID(Guid.Parse("b0a89fbd-641b-4d77-967b-2702cfc1787f")) != null)
            {
                clsfield = clsmisc.GetGeneralFieldByCode("to");
                grid.getColumnByID(Guid.Parse("b0a89fbd-641b-4d77-967b-2702cfc1787f")).HeaderText = clsfield.description;
            }

            if (grid.getColumnByID(Guid.Parse("1ee53ae2-2cdf-41b4-9081-1789adf03459")) != null)
            {
                clsfield = clsmisc.GetGeneralFieldByCode("currency");
                grid.getColumnByID(Guid.Parse("1ee53ae2-2cdf-41b4-9081-1789adf03459")).HeaderText = clsfield.description;
            }

            if (grid.getColumnByID(Guid.Parse("ec527561-dfee-48c7-a126-0910f8e031b0")) != null)
            {
                clsfield = clsmisc.GetGeneralFieldByCode("country");
                grid.getColumnByID(Guid.Parse("ec527561-dfee-48c7-a126-0910f8e031b0")).HeaderText = clsfield.description;
            }

            if (grid.getColumnByID(Guid.Parse("af839fe7-8a52-4bd1-962c-8a87f22d4a10")) != null)
            {
                clsfield = clsmisc.GetGeneralFieldByCode("reason");
                grid.getColumnByID(Guid.Parse("af839fe7-8a52-4bd1-962c-8a87f22d4a10")).HeaderText = clsfield.description;
            }

            if (grid.getColumnByID(Guid.Parse("7cf61909-8d25-4230-84a9-f5701268f94b")) != null)
            {
                clsfield = clsmisc.GetGeneralFieldByCode("otherdetails");
                grid.getColumnByID(Guid.Parse("7cf61909-8d25-4230-84a9-f5701268f94b")).HeaderText = clsfield.description;
            }

            if (grid.getColumnByID(Guid.Parse("359dfac9-74e6-4be5-949f-3fb224b1cbfc")) != null)
            {
                clsfield = clsmisc.GetGeneralFieldByCode("costcode");
                grid.getColumnByID(Guid.Parse("359dfac9-74e6-4be5-949f-3fb224b1cbfc")).HeaderText = clsfield.description;
            }

            if (grid.getColumnByID(Guid.Parse("9617a83e-6621-4b73-b787-193110511c17")) != null)
            {
                clsfield = clsmisc.GetGeneralFieldByCode("department");
                grid.getColumnByID(Guid.Parse("9617a83e-6621-4b73-b787-193110511c17")).HeaderText = clsfield.description;
            }

            if (grid.getColumnByID(Guid.Parse("6d06b15e-a157-4f56-9ff2-e488d7647219")) != null)
            {
                clsfield = clsmisc.GetGeneralFieldByCode("projectcode");
                grid.getColumnByID(Guid.Parse("6d06b15e-a157-4f56-9ff2-e488d7647219")).HeaderText = clsfield.description;
            }
            #endregion

            if (enableJourneyDetailsLink)
            {
                columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("415ae9c3-a06b-4152-95de-5206da31dcd2")))); //hasJourneySteps
                grid.getColumnByName("hasJourneySteps").hidden = true;
                grid.addEventColumn("journeyStepsLink", GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/car_sedan_blue.png", string.Empty, "Journey Steps", "Journey Steps");
            }

            if (enableReceiptAttachments)
            {
                columns.Add(new cFieldColumn(fields.GetFieldByID(Guid.Parse("64604b7c-a2ea-4a61-b0b1-e4fb55e1b438")))); //receiptAttached
                grid.getColumnByName("receiptAttached").hidden = true;
                grid.addEventColumn("receiptAttachmentsLink", string.Format("{0}/{1}/{2}", GlobalVariables.StaticContentLibrary, "icons/16/new-icons", "scroll.png"), string.Empty, "Receipt", "Receipt");
            }

            if (enableCorporateCardLink)
            {
                grid.addEventColumn("corporateCardLink", "/shared/images/icons/16/plain/creditcards.png", string.Empty, "Flag Information", "Flag Information");
            }

            if (viewType == UserView.CheckAndPay && (itemState == ItemState.Unapproved || itemState == ItemState.Returned))
            {
                grid.EnableSelect = true;
                grid.GridSelectType = GridSelectType.CheckBox;
            }

            if (!printView)
            {
                grid.addEventColumn("splitItemLink", "/static/icons/16/new-icons/zoom_in_blue.png", "",
                    "Is the expense split into multiple items", "Is the expense split into multiple items");

                // if the validation service is enabled, check for any expenseitems that have a validation status of in progress or higher.
                if (account.ValidationServiceEnabled && groupContainsValidation)
                {
                    const string openValidationModalJs = "javascript:SEL.Claims.ClaimViewer.DisplayValidation('{expenseid}');";
                    var validationIconPrefix = GlobalVariables.StaticContentLibrary + "/icons/16/plain/validation_";
                    var iconOptions = new List<ValueIconEventColumnOptions>
                    {
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.SubcatValidationDisabled,      IconPath = validationIconPrefix + "not_applicable.png",   Tooltip = "Receipt validation not applicable, expense item not configured for validation" },
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.StageNotInSignoffGroup,        IconPath = validationIconPrefix + "not_applicable.png",   Tooltip = "Receipt validation not applicable, no validation stage in signoff group" },
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.NoReceipts,                    IconPath = validationIconPrefix + "not_applicable.png",   Tooltip = "No receipts to validate" },
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.NotRequired,                   IconPath = validationIconPrefix + "not_applicable.png",   Tooltip = "Receipt validation not required" },
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.NotValidated,                  IconPath = validationIconPrefix + "not_applicable.png",   Tooltip = "Item skipped validation" },
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.Required,                      IconPath = validationIconPrefix + "hourglass.png",        Tooltip = "Receipt validation pending",                                       OnClickCommand = openValidationModalJs },
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.NotSelectedForValidation,      IconPath = validationIconPrefix + "hourglass.png",        Tooltip = "Receipt validation pending",                                       OnClickCommand = openValidationModalJs  },
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.InProgress,                    IconPath = validationIconPrefix + "hourglass.png",        Tooltip = "Receipt validation in progress",                                   OnClickCommand = openValidationModalJs  },
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.WaitingForClaimant,            IconPath = validationIconPrefix + "return.png",           Tooltip = "Receipt validation failed, item returned to claimant",             OnClickCommand = openValidationModalJs  }, 
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.CompletedFailed,               IconPath = validationIconPrefix + "cross.png",            Tooltip = "Receipt validation failed",                                        OnClickCommand = openValidationModalJs  }, 
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.CompletedWarning,              IconPath = validationIconPrefix + "warning.png",          Tooltip = "Receipt validation warning",                                       OnClickCommand = openValidationModalJs  }, 
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.CompletedPassed,               IconPath = validationIconPrefix + "tick.png",             Tooltip = "Receipt validation passed",                                        OnClickCommand = openValidationModalJs  },
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.InvalidatedFailed,             IconPath = validationIconPrefix + "cross_invalid.png",    Tooltip = "Receipt validation invalidated. Previously 'Completed: Failed'",   OnClickCommand = openValidationModalJs  }, 
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.InvalidatedWarning,            IconPath = validationIconPrefix + "warning_invalid.png",  Tooltip = "Receipt validation invalidated. Previously 'Completed: Warning'",  OnClickCommand = openValidationModalJs  }, 
                        new ValueIconEventColumnOptions { ExpectedValue = (int)ExpenseValidationProgress.InvalidatedPassed,             IconPath = validationIconPrefix + "tick_invalid.png",     Tooltip = "Receipt validation invalidated. Previously 'Completed: Passed'",   OnClickCommand = openValidationModalJs  }
                    };

                    var column = grid.getColumnByName("ValidationProgress");
                    grid.AddValueIconColumn("ValidationProgress", (cFieldColumn)column, validationIconPrefix + "header.png", "Validation Progress", validationIconPrefix + "header.png", "Validation Not Applicable", "#", iconOptions);
            }

                grid.addEventColumn("flagLink", GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/signal_flag_red.png",
                    "javascript:SEL.Claims.ClaimViewer.DisplayFlags('{expenseid}');", "Flag Information",
                    "Flag Information");


            }

            if (viewType == UserView.Submitted && !fromClaimSelector)
            {
                grid.addEventColumn("disputeLink", "../icons/redo_blue.gif", "../dispute.aspx?returned=1&expenseid={expenseid}&claimid=" + claimId, "Dispute", "Dispute");
            }

            if (viewType == UserView.CheckAndPay && itemState == ItemState.Approved && !fromClaimSelector)
            {
                grid.addEventColumn("unapproveLink", "../shared/images/icons/16/plain/undo.png", "javascript:SEL.Claims.CheckExpenseList.UnapproveItem({expenseid});", "Un-approve Item", "Un-approve Item");
            }

            if (viewType == UserView.Current)
            {
                grid.addEventColumn("copyLink", GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/copy.png", "javascript:SEL.Claims.ClaimViewer.CopyExpense({expenseid});", "Copy Expense Item", "Copy Expense Item");
            }

            string[] generatedGrid = grid.generateGrid();
            var homeAddressModifier = new ExpenseItemsGridHomeAddressModifier(accountid, claim.employeeid);
            if (fromIDField != null && fromField != null)
            {
                var fromFieldName = clsmisc.GetGeneralFieldByCode("from").description;
                generatedGrid[1] = homeAddressModifier.ReplaceTextFieldWithHomeIfIdFieldIsHomeAddress(generatedGrid[1], fromIDField, fromFieldName);
            }
            if (toIDField != null && toField != null)
            {
                var toFieldName = clsmisc.GetGeneralFieldByCode("to").description;
                generatedGrid[1] = homeAddressModifier.ReplaceTextFieldWithHomeIfIdFieldIsHomeAddress(generatedGrid[1], toIDField, toFieldName);
            }
            return generatedGrid;
        }

        private void claimGrid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
        {
            int claimId = (int)gridInfo["claimId"];
            int expenseId = (int)row.getCellByID("expenseid").Value;
            UserView viewType = (UserView)gridInfo["viewType"];
            string gridName = (string)gridInfo["gridName"];
            int claimEmployeeId = (int)gridInfo["claimEmployeeId"];
            var fromClaimSelector = (bool)gridInfo["fromClaimSelector"];

            string pageSource;
            pageSource = viewType == UserView.CheckAndPay ? "CheckAndPay" : "ClaimViewer";

            if ((bool)row.getCellByID("hasFlags").Value == false && row.getCellByID("flagLink") != null)
            {
                row.getCellByID("flagLink").Value = "&nbsp;";
            }
            else
            {
                if (row.getCellByID("flagColour").Value != DBNull.Value)
                {
                    FlagColour colour = (FlagColour)(byte)row.getCellByID("flagColour").Value;
                    string icon = GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/";
                    switch (colour)
                    {
                        case FlagColour.Red:
                            icon += "signal_flag_red.png";
                            break;
                        case FlagColour.Amber:
                            icon += "signal_flag_amber.png";
                            break;
                        case FlagColour.Information:
                            icon += "about.png";
                            break;
                    }

                    if (row.getCellByID("flagLink") != null)
                    {
                        row.getCellByID("flagLink").Value =
                            "<a href=\"javascript:SEL.Claims.ClaimViewer.DisplayFlags('{expenseid}', '" + pageSource
                            + "');\"><img src=\"" + icon + "\" alt=\"Flag Information\" /></a>";
                    }
                }
            }
            if ((bool)row.getCellByID("hasSplitItems").Value == true)
            {
                if (row.getCellByID("splitItemLink") != null)
                {
                    row.getCellByID("splitItemLink").Value = "<img src=\"" + "../shared/images/icons/16/plain/zoom_in.png\" title=\"This expense is split into multiple items\" alt=\"This expense is split into multiple items\">";
                }
            }
            else
            {
                if (row.getCellByID("splitItemLink") != null)
                {
                    row.getCellByID("splitItemLink").Value = "&nbsp;";
                }
            }

            if (row.getCellByID("receiptAttachmentsLink") != null)
            {
                var receiptAttached = (bool)row.getCellByID("receiptattached").Value;
                var receiptAnchorTitle = fromClaimSelector ? "View Receipts" : "Manage Receipts";
                var iconType = !fromClaimSelector ? (receiptAttached ? "scroll.png" : "scroll_add.png") : "scroll_information.png";
                var receiptIcon = string.Format("{0}/{1}/{2}", GlobalVariables.StaticContentLibrary, "icons/16/new-icons", iconType);

                var returnTo = viewType == UserView.CheckAndPay ? 3 : 1;
                var uploadLink =
                    string.Format("href=\"../ReceiptManagement.aspx?returnto={0}&expenseid={1}&claimid={2}{3}{4}\"",
                        returnTo, expenseId, claimId,
                        claimEmployeeId > 0 ? "&viewowner=" + claimEmployeeId : string.Empty,
                        fromClaimSelector ? "&claimSelector=true" : string.Empty);
                
                row.getCellByID("receiptAttachmentsLink").Value = string.Format("<a title=\"{0}\" {1}><img src=\"{2}\"></a>", receiptAnchorTitle, uploadLink, receiptIcon);

                if (receiptAttached)
                {
                    var receiptPreviewIcon = string.Format("{0}/{1}/{2}", GlobalVariables.StaticContentLibrary, "icons/16/plain", "view.png");
                    const string ReceiptPreviewTitle = "Quick view receipts";
                    row.getCellByID("receiptAttachmentsLink").Value += string.Format("<a class=\"receiptPreview\" title=\"{0}\" href=\"#{1}\"><img src=\"{2}\"></a>", ReceiptPreviewTitle, expenseId, receiptPreviewIcon);
                }
            }

            if (row.getCellByID("journeyStepsLink") != null)
            {
                if ((bool)row.getCellByID("hasJourneySteps").Value == true)
                {
                    row.getCellByID("journeyStepsLink").Value = "<a id=\"viewjourney{expenseid}\" href=\"javascript:viewJourney(" + claimId + ",{expenseid},'viewjourney{expenseid}');\"><img alt=\"View Journey Steps\" src=\"" + GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/car_sedan_blue.png\"></a>";
                }
                else
                {
                    row.getCellByID("journeyStepsLink").Value = "&nbsp;";
                }
            }

            if (row.getCellByID("name") != null)
            {
                if (row.getCellByID("name").Value != DBNull.Value)
                {
                    row.getCellByID("name").Value = "<a href=\"javascript:SEL.Claims.ClaimViewer.ShowAdvanceAllocation(" + row.getCellByID("floatid").Value + "," + (int)viewType + ");\">" + row.getCellByID("name").Value + "</a>";
                }
            }

            if (row.getCellByID("exchangerate") != null)
            {
                if (row.getCellByID("exchangerate").Value != DBNull.Value)
                {
                    row.getCellByID("exchangerate").Value = Math.Round(Convert.ToDouble(row.getCellByID("exchangerate").Value), 5, MidpointRounding.AwayFromZero).ToString();
                }
            }

            if (row.getCellByID("dbo.getCostcodeSplit(savedexpenses.expenseid)") != null)
            {
                if (row.getCellByID("dbo.getCostcodeSplit(savedexpenses.expenseid)").Value.ToString() == "Split")
                {
                    row.getCellByID("dbo.getCostcodeSplit(savedexpenses.expenseid)").Value = "<a style=\"cursor:hand;\" onclick=\"window.open('../codesplit.aspx?claimid=" + claimId + "&expenseid=" + expenseId + "&field=1',null,'width=300,height=200,resizeable=yes,scrollbars=yes');\">Split</a>";
                }
            }

            if (row.getCellByID("dbo.getDepartmentSplit(savedexpenses.expenseid)") != null)
            {
                if (row.getCellByID("dbo.getDepartmentSplit(savedexpenses.expenseid)").Value.ToString() == "Split")
                {
                    row.getCellByID("dbo.getDepartmentSplit(savedexpenses.expenseid)").Value = "<a style=\"cursor:hand;\" onclick=\"window.open('../codesplit.aspx?claimid=" + claimId + "&expenseid=" + expenseId + "&field=2',null,'width=300,height=200,resizeable=yes,scrollbars=yes');\">Split</a>";
                }
            }

            if (row.getCellByID("dbo.getProjectcodeSplit(savedexpenses.expenseid)") != null)
            {
                if (row.getCellByID("dbo.getProjectcodeSplit(savedexpenses.expenseid)").Value.ToString() == "Split")
                {
                    row.getCellByID("dbo.getProjectcodeSplit(savedexpenses.expenseid)").Value = "<a style=\"cursor:hand;\" onclick=\"window.open('../codesplit.aspx?claimid=" + claimId + "&expenseid=" + expenseId + "&field=3',null,'width=300,height=200,resizeable=yes,scrollbars=yes');\">Split</a>";
                }
            }

            if (row.getCellByID("corporateCardLink") != null)
            {
                if (row.getCellByID("transactionid").Value != DBNull.Value)
                {
                    int transactionId = (int)row.getCellByID("transactionid").Value;

                    row.getCellByID("corporateCardLink").Value = "<a href=\"javascript:SEL.Claims.ClaimViewer.DisplayTransactionDetails('" + gridName + "'," + transactionId + ")\"><img id=\"viewTransaction" + gridName + transactionId + "\" alt=\"View Transaction Details\" src=\"../shared/images/icons/16/plain/zoom_in.png\" /></a>";
                    if (viewType == UserView.Current)
                    {
                        row.getCellByID("corporateCardLink").Value += "<a id=\"unmatchtransaction" + transactionId + "\" href=\"javascript:SEL.Claims.ClaimViewer.UnmatchTransaction(" + transactionId + "," + expenseId + ")\"><img alt=\"Un-match Transaction\" src=\"../shared/images/icons/16/plain/undo.png\" /></a>";
                    }
                }
                else
                {
                    row.getCellByID("corporateCardLink").Value = "&nbsp;";
                }
            }

            if (row.getCellByID("disputeLink") != null)
            {
                if ((bool)row.getCellByID("returned").Value == false && (row.getCellByID("corrected").Value == DBNull.Value || (row.getCellByID("corrected").Value != DBNull.Value && (bool)row.getCellByID("corrected").Value == false)) || ((bool)row.getCellByID("returned").Value == true && (bool)row.getCellByID("corrected").Value == true))
                {
                    row.enableupdating = false;
                    row.enabledeleting = false;
                    row.getCellByID("disputeLink").Value = "&nbsp;";
                }
                else
                {
                    row.CssClass = "errorRow";
                }
            }

            if (row.getCellByID("Edited") != null)
            {
                row.enableupdating = false;
            }

            if (row.getCellByID("OriginalExpenseId") != null)
            {
                var originalExpenseId = row.getCellByID("OriginalExpenseId").Value;
                if (originalExpenseId != DBNull.Value)
                {
                    if (this.expenseItems == null)
                    {
                        var user = cMisc.GetCurrentUser();
                        var claims = new cClaims(user.AccountID);
                        var claim = claims.getClaimById(claimId);
                        var claimExpenseItems = claims.getExpenseItemsFromDB(claim.claimid);
                        this.expenseItems = claimExpenseItems.Values.ToList();
                        var groups = new cGroups(user.AccountID);
                        var group = claims.GetGroupForClaim(groups, claim);
                        this.currentStage = group.stages.Values[claim.stage - 1];
                        this.payBeforeValidateStage = ClaimSubmission.GetPayBeforeValidateStage(group);
                    }

                    var paidValue =
                        this.expenseItems.Where(
                            expenseItem =>
                            expenseItem.expenseid == (int)originalExpenseId && expenseItem.Paid && expenseItem.Edited)
                            .Sum(expenseItem => expenseItem.amountpayable);
                    var unpaidTotal =
                        this.expenseItems.Where(
                            expenseItem =>
                            expenseItem.OriginalExpenseId == (int)originalExpenseId && !expenseItem.Paid
                            && !expenseItem.Edited).Sum(expenseItem => expenseItem.amountpayable);
                    

                    if (unpaidTotal != 0)
                    {
                        var difference = ((unpaidTotal - paidValue) / paidValue) * 100;
                        var overTolerance = string.Empty;
                        if (difference > this.payBeforeValidateStage.ValidationCorrectionThreshold)
                        {
                            row.CssClass = "errorRow";
                            overTolerance = string.Format(
                                "The changes for this Expense Item exceed the tolerance of {0}%.",
                                this.payBeforeValidateStage.ValidationCorrectionThreshold);
                        }
                        else
                        {
                            row.enableupdating = false;
                        }

                        var previouslyPaidIcon =
                        string.Format(
                            "<img title='The previously paid amount is {0}.  {1}' src='/static/icons/16/plain/money_bills.png' style='margin-left: 22px;'>",
                            paidValue.ToString("###,##0.00"), 
                            overTolerance);
                        row.getCellByID("OriginalExpenseId").Value = previouslyPaidIcon;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the journey steps for an expense
        /// </summary>
        /// <param name="expenseId">The id of the expense.</param>
        /// <returns>A sorted list of journey syteps</returns>
        public SortedList<int, cJourneyStep> GetJourneySteps(int expenseId)
        {
            var steps = new SortedList<int, cJourneyStep>();

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                var expenseItemStepAddressIds = new List<Tuple<byte, int?, int?>>();
                var addressIdentifiers = new List<int>();
                const string Sql = "dbo.GetJourneyStepsByExpenseId";

                connection.sqlexecute.Parameters.AddWithValue("@expenseid", expenseId);

                var passengers = new List<Passenger>();

                using (var passengersConnection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
                {
                    passengersConnection.sqlexecute.Parameters.AddWithValue("@expenseid", expenseId);

                    using (var passengersReader = passengersConnection.GetReader("dbo.GetPassengerInformationByExpenseId", CommandType.StoredProcedure))
                    {
                        while (passengersReader.Read())
                        {
                            int? employeeId = passengersReader.GetNullable<int>("employeeid");
                            string name = passengersReader.GetValueOrDefault("name", (string)null);
                            byte? stepNumber = passengersReader.GetNullable<byte>("step_number");

                            passengers.Add(new Passenger { EmployeeId = employeeId, Name = name, StepNumber = stepNumber });
                        }
                    }
                }

                using (var reader = connection.GetReader(Sql, CommandType.StoredProcedure))
                {
                    connection.sqlexecute.Parameters.Clear();

                    int expenseIdOrdinal = reader.GetOrdinal("expenseid");
                    int stepNumberOrdinal = reader.GetOrdinal("step_number");
                    int numMilesOrdinal = reader.GetOrdinal("num_miles");
                    int startLocationOrdinal = reader.GetOrdinal("StartAddressID");
                    int endLocationOrdinal = reader.GetOrdinal("EndAddressID");
                    int numPassengersOrdinal = reader.GetOrdinal("num_passengers");
                    int numActualMilesOrdinal = reader.GetOrdinal("numActualMiles");
                    int heavyBulkyEquipmentOrdinal = reader.GetOrdinal("heavyBulkyEquipment");

                    while (reader.Read())
                    {
                        const decimal RecMiles = 0;
                        int id = reader.GetInt32(expenseIdOrdinal);
                        byte stepnumber = reader.GetByte(stepNumberOrdinal);
                        decimal nummiles = reader.GetDecimal(numMilesOrdinal);
                        int? startLocationId = reader.IsDBNull(startLocationOrdinal) ? (int?)null : reader.GetInt32(startLocationOrdinal);
                        int? endLocationId = reader.IsDBNull(endLocationOrdinal) ? (int?)null : reader.GetInt32(endLocationOrdinal);
                        byte numpassengers = reader.IsDBNull(numPassengersOrdinal) ? (Byte)0 : reader.GetByte(numPassengersOrdinal);
                        decimal numactualmiles = reader.GetDecimal(numActualMilesOrdinal);
                        bool heavyBulkyEquipment = reader.GetBoolean(heavyBulkyEquipmentOrdinal);

                        if (startLocationId.HasValue || endLocationId.HasValue)
                        {
                            expenseItemStepAddressIds.Add(new Tuple<byte, int?, int?>(stepnumber, startLocationId, endLocationId));
                            if (startLocationId.HasValue)
                            {
                                addressIdentifiers.Add(startLocationId.Value);
                            }
                            if (endLocationId.HasValue)
                            {
                                addressIdentifiers.Add(endLocationId.Value);
                            }
                        }

                        var journeyStepPassengers = passengers.Where(p => p.StepNumber == stepnumber);

                        steps.Add(stepnumber, new cJourneyStep(id, null, null, nummiles, RecMiles, numpassengers, stepnumber, numactualmiles, heavyBulkyEquipment) { passengers = journeyStepPassengers.ToArray() });

                    }

                    reader.Close();
                }

                #region Populate the company objects in the journeysteps

                if (addressIdentifiers.Count > 0)
                {
                    List<Address> addresses = Address.Get(this.accountid, addressIdentifiers.Distinct().ToList(), true);

                    foreach (Tuple<byte, int?, int?> t in expenseItemStepAddressIds)
                    {
                        if (t.Item2.HasValue)
                        {
                            steps[t.Item1].startlocation = addresses.FirstOrDefault(x => t.Item2 != null && x.Identifier == t.Item2.Value);
                        }

                        if (t.Item3.HasValue)
                        {
                            steps[t.Item1].endlocation = addresses.FirstOrDefault(x => t.Item3 != null && x.Identifier == t.Item3.Value);
                        }
                    }
                }

                #endregion Populate the company objects in the journeysteps

                return steps;
            }
        }

        /// <summary>
        /// Clone the current object journey step
        /// </summary>
        /// <param name="expenseId">The expense id to clone journey steps from</param>
        /// <param name="expenseItem">The expense item to clone journey steps from</param>
        /// <param name="accountProperties">The account properties</param>
        /// <param name="user">the current user</param>
        /// <param name="item">The cloned item</param>
        public void CloneJourneySteps(int expenseId, cExpenseItem expenseItem, cAccountProperties accountProperties, CurrentUser user, ref cExpenseItem item)
        {
            var result = this.GetJourneySteps(expenseId);

            var officeAddresses = Address.GetAllByReservedKeyword(user, "office", DateTime.Today.AddDays(-1).Date, accountProperties, accountProperties.MultipleWorkAddress ? null : (int?)expenseItem.ESRAssignmentId);
            var workAdressId = item.WorkAddressId;

            foreach (cJourneyStep step in result.Values)
            {
                step.Clone(officeAddresses, workAdressId, ref item);
            }

            item.journeysteps = result;
        }

        #region Utility Functions
        private void ConvertNetToGross(ref cExpenseItem expenseitem)
        {

            cSubcat reqsubcat;
            cSubcats clssubcats = new cSubcats(accountid);

            reqsubcat = clssubcats.GetSubcatById(expenseitem.subcatid);
            if (reqsubcat == null)
            {
                return;
            }
            if (reqsubcat.addasnet == false)
            {
                return;
            }

            decimal net;
            decimal total;
            double vatamount;

            cSubcatVatRate clsvatrate = reqsubcat.getVatRateByDate(expenseitem.date);

            if (clsvatrate != null && ((clsvatrate.vatreceipt == true && expenseitem.receipt == true) || clsvatrate.vatreceipt == false)) //vat exists so calculate total
            {
                vatamount = clsvatrate.vatamount;
                net = expenseitem.total;
                total = Math.Round(((net / 100) * (decimal)vatamount) + net, 2, MidpointRounding.AwayFromZero);
                expenseitem.total = total;
                expenseitem.amountpayable = total;

            }

        }

        private void convertTotals(ref cExpenseItem expenseitem, Employee reqemp)
        {
            int basecurrency;

            if (reqemp.PrimaryCurrency != 0)
            {
                basecurrency = reqemp.PrimaryCurrency;
            }
            else
            {
                basecurrency = (int)this._generalOptionsFactory[cMisc.GetCurrentUser().CurrentSubAccountId].WithCurrency().Currency.BaseCurrency;
            }

            if (expenseitem.currencyid == basecurrency)
            {
                return;
            }

            double exchangerate;
            decimal convertedtotal;
            decimal convertedNet, net;
            decimal convertedVat, vat;
            decimal total;

            convertedtotal = expenseitem.total;
            convertedNet = expenseitem.net;
            convertedVat = expenseitem.vat;

            if (expenseitem.floatid > 0)
            {
                cFloats clsfloats = new cFloats(accountid);
                cFloat reqfloat = clsfloats.GetFloatById(expenseitem.floatid);
                exchangerate = reqfloat.exchangerate;
                total = Math.Round(convertedtotal * (decimal)exchangerate, 2, MidpointRounding.AwayFromZero);
                net = Math.Round(convertedNet * (decimal)exchangerate, 2, MidpointRounding.AwayFromZero);
                vat = Math.Round(convertedVat * (decimal)exchangerate, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                exchangerate = expenseitem.exchangerate;
                total = Math.Round(convertedtotal * (1 / (decimal)exchangerate), 2, MidpointRounding.AwayFromZero);
                net = Math.Round(convertedNet * (1 / (decimal)exchangerate), 2, MidpointRounding.AwayFromZero);
                vat = Math.Round(convertedVat * (1 / (decimal)exchangerate), 2, MidpointRounding.AwayFromZero);
            }


            if (exchangerate == 0)
            {
                exchangerate = 1;
            }

            expenseitem.convertedtotal = Math.Round(convertedtotal, 2, MidpointRounding.AwayFromZero);
            expenseitem.total = total;

            //If this is a mileage item the Net and Vat doesn't get converted when calculated so needs to here.
            expenseitem.updateVAT(net, vat, total);
            expenseitem.amountpayable = total;
        }
        private void convertGlobalTotals(ref cExpenseItem expenseitem)
        {
            // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
            int subAccountID = subaccs.getFirstSubAccount().SubAccountID;

            cCurrencies clscurrencies = new cCurrencies(accountid, subAccountID);
            int basecurrency;
            int globalcurrency;
            double globalexchangerate;
            decimal globaltotal;

            basecurrency = (int)this._generalOptionsFactory[subAccountID].WithCurrency().Currency.BaseCurrency;

            if (expenseitem.currencyid == basecurrency || expenseitem.basecurrency == basecurrency)
            {
                globalcurrency = basecurrency;
                globalexchangerate = 1;
                if (expenseitem.basecurrency != basecurrency)
                {
                    globaltotal = expenseitem.convertedtotal;
                }
                else
                {
                    globaltotal = expenseitem.total;
                }
            }
            else
            {
                double exchangerate;
                decimal convertedtotal;
                decimal total;

                if (expenseitem.floatid > 0)
                {
                    cFloats clsfloats = new cFloats(accountid);
                    cFloat reqfloat = clsfloats.GetFloatById(expenseitem.floatid);
                    exchangerate = reqfloat.exchangerate;
                }
                else
                {
                    exchangerate = clscurrencies.getExchangeRate(basecurrency, expenseitem.basecurrency, expenseitem.date);
                }

                convertedtotal = expenseitem.total;

                if (exchangerate > 0)
                {
                    total = Math.Round(convertedtotal * (1 / (decimal)exchangerate), 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    total = convertedtotal;
                }


                globalcurrency = basecurrency;
                globalexchangerate = exchangerate;
                globaltotal = total;
            }


            expenseitem.setGlobalTotal(globalcurrency, globalexchangerate, globaltotal);


        }
        private void splitEntertainment(ref cExpenseItem item, cExpenseItem olditem)
        {
            cSubcats clssubcats = new cSubcats(accountid);

            cMisc clsmisc = new cMisc(accountid);
            cSubcat reqsubcat = clssubcats.GetSubcatById(item.subcatid);
            cExpenseItem splititem;
            cExpenseItem tempitem;
            decimal staffportion, otherportion, personalportion, remoteportion;
            decimal staffportionconverted, otherportionconverted, personalportionconverted, remoteportionconverted;
            decimal staffportionglobal, otherportionglobal, personalportionglobal, remoteportionglobal;

            byte numstaff, numothers, numpersonal, numremote;
            int expenseid;
            DateTime nulldate = DateTime.Parse("01/01/1900");

            if (reqsubcat == null)
            {
                return;
            }
            if (item.total == 0)
            {
                return;
            }
            if (reqsubcat.calculation != CalculationType.Meal || (reqsubcat.splitentertainment == false && reqsubcat.splitpersonal == false && reqsubcat.splitremote == false))
            {
                return;
            }

            numstaff = (byte)(item.staff + item.directors);
            numothers = item.others;

            if (reqsubcat.splitpersonal == false)
            {
                numpersonal = 0;
                numothers += item.personalguests;
            }
            else
            {
                numpersonal = item.personalguests;
            }
            if (reqsubcat.splitremote == false)
            {
                numremote = 0;
                numstaff += item.remoteworkers;
            }
            else
            {
                numremote = item.remoteworkers;
            }
            //first item is staff portion

            decimal amounttoallocate = item.total;
            decimal amounttoallocateconverted = item.convertedtotal;
            decimal amounttoallocateglobal = item.globaltotal;
            staffportion = Math.Round((amounttoallocate / (item.staff + item.others + item.directors + item.personalguests + item.remoteworkers)) * numstaff, 2, MidpointRounding.AwayFromZero);
            if (item.convertedtotal > 0)
            {
                staffportionconverted = Math.Round((amounttoallocateconverted / (item.staff + item.others + item.directors + item.personalguests + item.remoteworkers)) * numstaff, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                staffportionconverted = 0;
            }
            staffportionglobal = Math.Round((amounttoallocateglobal / (item.staff + item.others + item.directors + item.personalguests + item.remoteworkers)) * numstaff, 2, MidpointRounding.AwayFromZero);
            amounttoallocate -= staffportion;
            amounttoallocateconverted -= staffportionconverted;
            amounttoallocateglobal -= staffportionglobal;

            item.total = staffportion;
            item.convertedtotal = staffportionconverted;
            item.globaltotal = staffportionglobal;

            if (amounttoallocate > 0)
            {
                otherportion = Math.Round((amounttoallocate / (item.others + item.personalguests + numremote)) * numothers, 2, MidpointRounding.AwayFromZero);
                if (item.convertedtotal > 0)
                {
                    otherportionconverted = Math.Round((amounttoallocateconverted / (item.others + item.personalguests + numremote)) * numothers, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    otherportionconverted = 0;
                }
                otherportionglobal = Math.Round((amounttoallocateglobal / (item.others + item.personalguests + numremote)) * numothers, 2, MidpointRounding.AwayFromZero);
                amounttoallocate -= otherportion;
                amounttoallocateconverted -= otherportionconverted;
                amounttoallocateglobal -= otherportionglobal;
            }
            else
            {
                otherportion = 0;
                otherportionconverted = 0;
                otherportionglobal = 0;
            }
            if (amounttoallocate > 0)
            {
                personalportion = Math.Round((amounttoallocate / (item.personalguests + numremote)) * numpersonal, 2, MidpointRounding.AwayFromZero);
                if (item.convertedtotal > 0)
                {
                    personalportionconverted = Math.Round((amounttoallocateconverted / (item.personalguests + numremote)) * numpersonal, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    personalportionconverted = 0;
                }
                personalportionglobal = Math.Round((amounttoallocateglobal / (item.personalguests + numremote)) * numpersonal, 2, MidpointRounding.AwayFromZero);
                amounttoallocate -= personalportion;
                amounttoallocateconverted -= personalportionconverted;
                amounttoallocateglobal -= personalportionglobal;
            }
            else
            {
                personalportion = 0;
                personalportionconverted = 0;
                personalportionglobal = 0;
            }

            if (amounttoallocate > 0)
            {
                remoteportion = (amounttoallocate / (item.remoteworkers)) * numremote;
                if (item.convertedtotal > 0)
                {
                    remoteportionconverted = (amounttoallocateconverted / (item.remoteworkers)) * numremote;
                }
                else
                {
                    remoteportionconverted = 0;
                }
                remoteportionglobal = (amounttoallocateglobal / (item.remoteworkers)) * numremote;
                item.total = staffportion;
                item.convertedtotal = staffportionconverted;
                item.globaltotal = staffportionglobal;
            }
            else
            {
                remoteportion = 0;
                remoteportionconverted = 0;
                remoteportionglobal = 0;
            }
            //no of others split
            if (reqsubcat.splitentertainment == true && numothers != 0)
            {
                if (olditem != null)
                {
                    tempitem = olditem.entertainmentsplititem();
                    if (tempitem != null)
                    {
                        expenseid = tempitem.expenseid;
                    }
                    else
                    {
                        //delete old item as a new one will be added when changing claim
                        string sql = "delete from savedexpenses where expenseid in (select splititem from savedexpenses_splititems where primaryitem = @primaryitem); delete from savedexpenses_splititems where primaryitem = @primaryitem";
                        DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
                        data.sqlexecute.Parameters.AddWithValue("@primaryitem", item.expenseid);
                        data.ExecuteSQL(sql);
                        data.sqlexecute.Parameters.Clear();
                        expenseid = 0;
                    }
                }
                else
                {
                    expenseid = 0;
                }


                splititem = new cExpenseItem(expenseid, item.itemtype, item.miles, item.pmiles, item.reason, item.receipt, item.net, 0, otherportion, reqsubcat.entertainmentid, item.date, 0, item.others, item.companyid, item.returned, item.home, item.refnum, item.claimid, item.plitres, item.blitres, item.currencyid, item.attendees, 0, item.countryid, item.foreignvat, otherportionconverted, item.exchangerate, item.tempallow, item.reasonid, item.normalreceipt, item.allowancestartdate, item.allowanceenddate, item.carid, item.allowancededuct, item.allowanceid, item.nonights, item.quantity, 0, item.amountpayable, item.hotelid, false, item.norooms, item.vatnumber, 0, 0, item.accountcode, item.basecurrency, item.globalbasecurrency, item.globalexchangerate, personalportionglobal, item.floatid, item.corrected, item.receiptattached, item.transactionid, item.createdon, item.createdby, item.modifiedon, item.modifiedby, item.mileageid, item.journeyunit, item.ESRAssignmentId, reqsubcat.HomeToLocationType, bankAccountId: (item.bankAccountId ?? 0));
                splititem.userdefined = item.userdefined;
                splititem.costcodebreakdown = item.costcodebreakdown;
                splititem.setPrimaryItem(item);
                item.addSplitItem(splititem);

                item.others = 0;

            }

            if (reqsubcat.splitpersonal == true && numpersonal != 0)
            {
                if (olditem != null)
                {
                    tempitem = olditem.personalguestssplititem();
                    if (tempitem != null)
                    {
                        expenseid = tempitem.expenseid;
                    }
                    else
                    {
                        expenseid = 0;
                    }
                }
                else
                {
                    expenseid = 0;
                }
                splititem = new cExpenseItem(expenseid, item.itemtype, item.miles, item.pmiles, item.reason, item.receipt, item.net, 0, personalportion, reqsubcat.personalid, item.date, 0, 0, item.companyid, item.returned, item.home, item.refnum, item.claimid, item.plitres, item.blitres, item.currencyid, item.attendees, 0, item.countryid, item.foreignvat, personalportionconverted, item.exchangerate, item.tempallow, item.reasonid, item.normalreceipt, item.allowancestartdate, item.allowanceenddate, item.carid, item.allowancededuct, item.allowanceid, item.nonights, item.quantity, 0, item.amountpayable, item.hotelid, false, item.norooms, item.vatnumber, numpersonal, 0, item.accountcode, item.basecurrency, item.globalbasecurrency, item.globalexchangerate, personalportionglobal, item.floatid, item.corrected, item.receiptattached, item.transactionid, item.createdon, item.createdby, item.modifiedon, item.modifiedby, item.mileageid, item.journeyunit, item.ESRAssignmentId, reqsubcat.HomeToLocationType);
                splititem.userdefined = item.userdefined;
                splititem.costcodebreakdown = item.costcodebreakdown;
                item.personalguests = 0;
                splititem.setPrimaryItem(item);
                item.addSplitItem(splititem);

            }


            if (reqsubcat.splitremote == true && numremote != 0)
            {
                if (olditem != null)
                {
                    tempitem = olditem.remoteworkerssplititem();
                    if (tempitem != null)
                    {
                        expenseid = tempitem.expenseid;
                    }
                    else
                    {
                        expenseid = 0;
                    }
                }
                else
                {
                    expenseid = 0;
                }
                splititem = new cExpenseItem(expenseid, item.itemtype, item.miles, item.pmiles, item.reason, item.receipt, item.net, 0, remoteportion, reqsubcat.remoteid, item.date, 0, 0, item.companyid, item.returned, item.home, item.refnum, item.claimid, item.plitres, item.blitres, item.currencyid, item.attendees, 0, item.countryid, item.foreignvat, remoteportionconverted, item.exchangerate, item.tempallow, item.reasonid, item.normalreceipt, item.allowancestartdate, item.allowanceenddate, item.carid, item.allowancededuct, item.allowanceid, item.nonights, item.quantity, 0, item.amountpayable, item.hotelid, false, item.norooms, item.vatnumber, 0, numremote, item.accountcode, item.basecurrency, item.globalbasecurrency, item.globalexchangerate, remoteportionglobal, item.floatid, item.corrected, item.receiptattached, item.transactionid, item.createdon, item.createdby, item.modifiedon, item.modifiedby, item.mileageid, item.journeyunit, item.ESRAssignmentId, reqsubcat.HomeToLocationType, bankAccountId: (item.bankAccountId ?? 0));
                splititem.setPrimaryItem(item);
                splititem.costcodebreakdown = item.costcodebreakdown;
                splititem.userdefined = item.userdefined;

                item.addSplitItem(splititem);
                item.remoteworkers = 0;

            }
        }

        /// <summary>
        /// Gets the flags for the expenses.
        /// </summary>
        /// <param name="expenseIds">
        /// The ids of the expense items to retrieve the flags for.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// Returns a list of all the flagged items for the expense ids given.
        /// </returns>
        public FlaggedItemsManager GetFlaggedItems(List<int> expenseIds, IDBConnection connection = null)
        {
            SerializableDictionary<int, SerializableDictionary<int, FlaggedItem>> lstFlags = new SerializableDictionary<int, SerializableDictionary<int, FlaggedItem>>();

            SerializableDictionary<int, List<AssociatedExpense>> lstAssociatedItems = this.getAssociatedExpenses(expenseIds);
            SerializableDictionary<int, List<AuthoriserJustification>> lstJustifications = this.GetAuthoriserJustifictions(expenseIds);

            FlagManagement flagman = new FlagManagement(this.accountid);
            Flag flag;
            FlaggedItemsManager manager = new FlaggedItemsManager();
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(accountid);
            cAccountSubAccount subAccount = clsSubAccounts.getFirstSubAccount();
            cAccountProperties properties = subAccount.SubAccountProperties;
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                databaseConnection.sqlexecute.Parameters.Add("@expenses", SqlDbType.Structured);
                List<SqlDataRecord> integers = new List<SqlDataRecord>();
                SqlMetaData[] rowMetaData = { new SqlMetaData("c1", SqlDbType.Int) };
                foreach (int i in expenseIds)
                {
                    var row = new SqlDataRecord(rowMetaData);
                    row.SetInt32(0, i);
                    integers.Add(row);
                }

                databaseConnection.sqlexecute.Parameters["@expenses"].Value = integers;
                using (IDataReader reader = databaseConnection.GetReader("GetSavedExpenseFlags", CommandType.StoredProcedure))
                {
                    databaseConnection.sqlexecute.Parameters.Clear();

                    while (reader.Read())
                    {

                        int flagid = reader.GetInt32(reader.GetOrdinal("flagID"));
                        flag = flagman.GetBy(flagid);
                        int flaggeditemid = reader.GetInt32(reader.GetOrdinal("flaggeditemid"));
                        int expenseid = reader.GetInt32(reader.GetOrdinal("expenseid"));
                        FlagType flagtype = (FlagType)reader.GetByte(reader.GetOrdinal("flagtype"));
                        string flagdescription = reader.GetString(reader.GetOrdinal("flagDescription"));
                        string flagtext = reader.IsDBNull(reader.GetOrdinal("flagText"))
                                              ? string.Empty
                                              : reader.GetString(reader.GetOrdinal("flagText"));

                        FlagColour flagcolour = (FlagColour)reader.GetByte(reader.GetOrdinal("flagColour"));
                        string claimantJustification = reader.IsDBNull(reader.GetOrdinal("claimantJustification"))
                                                           ? string.Empty
                                                           : reader.GetString(
                                                               reader.GetOrdinal("claimantJustification"));
                        int? claimantJustificationDelegateID = null;
                        string delegateName = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal("claimantJustificationDelegateID")))
                        {
                            claimantJustificationDelegateID = reader.GetInt32(reader.GetOrdinal("claimantJustificationDelegateID"));
                            delegateName = reader.GetString(reader.GetOrdinal("delegateFullName"));
                        }

                        string subcat = reader.GetString(reader.GetOrdinal("subcat"));
                        string currencySymbol = reader.GetString(reader.GetOrdinal("currencySymbol"));
                        decimal total = reader.GetDecimal(reader.GetOrdinal("total"));
                        DateTime date = reader.GetDateTime(reader.GetOrdinal("date"));
                        int claimid = reader.GetInt32(reader.GetOrdinal("claimid"));
                        bool submitted = reader.GetBoolean(reader.GetOrdinal("submitted"));
                        List<AssociatedExpense> associatedItems;
                        lstAssociatedItems.TryGetValue(flaggeditemid, out associatedItems);
                        if (associatedItems == null)
                        {
                            associatedItems = new List<AssociatedExpense>();
                        }

                        List<AuthoriserJustification> justifications;
                        lstJustifications.TryGetValue(flaggeditemid, out justifications);
                        if (justifications == null)
                        {
                            justifications = new List<AuthoriserJustification>();
                        }

                        decimal exceededLimit;

                        FlagSummary flagSummary;
                        ExpenseItemFlagSummary expense;
                        manager.TryGetValue(expenseid, out expense);
                        if (expense == null)
                        {
                            expense = new ExpenseItemFlagSummary(expenseid);
                            manager.Add(expense);
                        }

                        FlaggedItem item = null;

                        bool authoriserJustificationRequired = (flag.ApproverJustificationRequired && submitted
                                                                && flagman.AuthoriserJustificationRequired(
                                                                    flag,
                                                                    claimid,
                                                                    properties));
                        switch (flagtype)
                        {
                            case FlagType.Duplicate:
                               
                                AssociatedExpense duplicateExpense = associatedItems[0];
                                DuplicateFlag duplicateFlag = (DuplicateFlag)flag;
                                List<string> duplicateFields = duplicateFlag.AssociatedFields.Select(fieldId => this.fields.GetFieldByID(fieldId).Description).ToList();
                                duplicateFields.Sort();
                                duplicateFields.Sort();
                                item = new DuplicateFlaggedItem(
                                    flaggeditemid,
                                    flagtype,
                                    flagdescription,
                                    flagtext,
                                    flagid,
                                    flagcolour,
                                    claimantJustification,
                                    associatedItems,
                                    justifications,
                                    duplicateExpense,
                                    flag.FlagTypeDescription,
                                    flag.NotesForAuthoriser,
                                    flag.AssociatedExpenseItems,
                                    flag.Action,
                                    flag.CustomFlagText,
                                    duplicateFields,
                                    claimantJustificationDelegateID,
                                    delegateName, 
                                    flag.ClaimantJustificationRequired,
                                    authoriserJustificationRequired,
                                    flag.RequiresRevalidationOnClaimSubmittal);
                                break;
                            case FlagType.GroupLimitWithReceipt:
                            case FlagType.GroupLimitWithoutReceipt:
                            case FlagType.LimitWithReceipt:
                            case FlagType.LimitWithoutReceipt:
                            case FlagType.TipLimitExceeded:

                                exceededLimit = reader.GetDecimal(reader.GetOrdinal("exceededLimit"));

                                item = new LimitFlaggedItem(
                                    flaggeditemid,
                                    flagtype,
                                    flagdescription,
                                    flagtext,
                                    flagid,
                                    flagcolour,
                                    claimantJustification,
                                    associatedItems,
                                    justifications,
                                    exceededLimit,
                                    flag.FlagTypeDescription,
                                    flag.NotesForAuthoriser,
                                    flag.AssociatedExpenseItems,
                                    flag.Action,
                                    flag.CustomFlagText,
                                    claimantJustificationDelegateID,
                                    delegateName, 
                                    flag.ClaimantJustificationRequired,
                                    authoriserJustificationRequired,
                                    flag.RequiresRevalidationOnClaimSubmittal);
                                break;
                            case FlagType.MileageExceeded:
                            case FlagType.NumberOfPassengersLimit:
                            case FlagType.HomeToLocationGreater:
                                int stepnumber = reader.GetByte(reader.GetOrdinal("stepnumber"));
                                exceededLimit = reader.GetDecimal(reader.GetOrdinal("exceededLimit"));

                                // add the justification per journey step so that in js will ignore the one in the root 
                                // and we will show the one per journey step instead
                                string currentStepClaimantJustification = claimantJustification;
                                   
                                //does the item already exist
                                expense.TryGetValue(flagid, out flagSummary);

                                if (flagSummary == null || flagSummary.FlaggedItem == null)
                                {
                                    item = new MileageFlaggedItem(
                                        flaggeditemid,
                                        flagtype,
                                        string.Empty,
                                        flagtext,
                                        flagid,
                                        flagcolour,
                                        claimantJustification,
                                        associatedItems,
                                        justifications,
                                        flag.FlagTypeDescription,
                                        flag.NotesForAuthoriser,
                                        flag.AssociatedExpenseItems,
                                        flag.Action,
                                        flag.CustomFlagText,
                                        claimantJustificationDelegateID,
                                    delegateName, 
                                    flag.ClaimantJustificationRequired, 
                                    flag.ApproverJustificationRequired,
                                    flag.RequiresRevalidationOnClaimSubmittal);
                                    expense.Add(flagid, item);
                                    expense.TryGetValue(flagid, out flagSummary);
                                }
                                else
                                {
                                    item = flagSummary.FlaggedItem;
                                }

                                ((MileageFlaggedItem)flagSummary.FlaggedItem).AddFlaggedJourneyStep(stepnumber, exceededLimit, flagdescription, flaggeditemid, currentStepClaimantJustification);
                                break;
                            default:
                                item = new FlaggedItem(
                                    flaggeditemid,
                                    flagtype,
                                    flagdescription,
                                    flagtext,
                                    flagid,
                                    flagcolour,
                                    claimantJustification,
                                    associatedItems,
                                    justifications,
                                    flag.FlagTypeDescription,
                                    flag.NotesForAuthoriser,
                                    flag.AssociatedExpenseItems,
                                    flag.Action,
                                    flag.CustomFlagText,
                                    claimantJustificationDelegateID,
                                    delegateName, 
                                    flag.ClaimantJustificationRequired,
                                    authoriserJustificationRequired,
                                    flag.RequiresRevalidationOnClaimSubmittal);
                                break;
                        }

                        if (!expense.ContainsKey(flagid))
                        {
                            expense.Add(flagid, item);
                        }
                        
                        item.ExpenseSubcat = subcat;
                        item.ExpenseDate = date;
                        item.ExpenseTotal = total;
                        item.ExpenseCurrencySymbol = currencySymbol;
                    }

                    reader.Close();
                }
            }

            return manager;
        }

        /// <summary>
        /// Checks a <see cref="cExpenseItem">expense item</see> for blocked flags
        /// </summary>
        /// <param name="accountID">The account Id</param>
        /// <param name="employeeID">The employee Id</param>
        /// <param name="item">The expense item</param>
        /// <param name="validationType">The point at which the validation is taking place</param>
        /// <param name="blockedFlags">The blocked items flag manager</param>
        /// <param name="index">The index of the expense item in the collection of expense items.</param>
        public void CheckItemForBlockedFlags(int accountID, int employeeID, cExpenseItem item, ValidationType validationType, FlaggedItemsManager blockedFlags, int index)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cAccountSubAccounts subAccounts = new cAccountSubAccounts(accountID);
            cAccountSubAccount subAccount = subAccounts.getSubAccountById(user.CurrentSubAccountId);
            FlagManagement flagManagement = new FlagManagement(accountID);
            List<FlagSummary> flags = flagManagement.CheckItemForFlags(item, employeeID, ValidationPoint.AddExpense, validationType, subAccount, user);

            if (flagManagement.ContainsBlockedItem(flags))
            {
                item.flags = flags;
                blockedFlags.Add(new ExpenseItemFlagSummary(index / -1, flags));
            }
        }


        private SerializableDictionary<int, List<AssociatedExpense>> getAssociatedExpenses(List<int> expenseIds, IDBConnection connection = null)
        {
            
            SerializableDictionary<int, List<AssociatedExpense>> items = new SerializableDictionary<int, List<AssociatedExpense>>();
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                databaseConnection.sqlexecute.Parameters.Add("@expenses", SqlDbType.Structured);
                List<SqlDataRecord> integers = new List<SqlDataRecord>();
                SqlMetaData[] rowMetaData = { new SqlMetaData("c1", SqlDbType.Int) };
                foreach (int i in expenseIds)
        {
                    var row = new SqlDataRecord(rowMetaData);
                    row.SetInt32(0, i);
                    integers.Add(row);
                }

                databaseConnection.sqlexecute.Parameters["@expenses"].Value = integers;
                using (
                    IDataReader reader = databaseConnection.GetReader(
                        "GetSavedExpensesFlagsAssociatedExpenses",
                        CommandType.StoredProcedure))
            {
                while (reader.Read())
                {
                        int flaggeditemid = reader.GetInt32(0);
                        int associatedexpenseid = reader.GetInt32(1);

                        List<AssociatedExpense> associatedexpenses;
                        items.TryGetValue(flaggeditemid, out associatedexpenses);
                        if (associatedexpenses == null)
                        {
                            associatedexpenses = new List<AssociatedExpense>();
                            items.Add(flaggeditemid, associatedexpenses);
                        }
                    string claimName = reader.GetString(2);
                    DateTime date = reader.GetDateTime(3);
                    string refnum = reader.GetString(4);
                    decimal total = reader.GetDecimal(5);
                    string subcat = reader.GetString(6);
                    string symbol = reader.GetString(7);
                        associatedexpenses.Add(new AssociatedExpense(claimName, date.ToShortDateString(), refnum, symbol + total.ToString("####,###,##0.00"), subcat, associatedexpenseid));
                    }
                    reader.Close();
                }
            }
            return items;
        }

        /// <summary>
        /// Get the authoriser justifictions for the given expense items.
        /// </summary>
        /// <param name="expenseIds">
        /// The ids of the expenses to retrieve the justifications for.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        private SerializableDictionary<int, List<AuthoriserJustification>> GetAuthoriserJustifictions(IEnumerable<int> expenseIds, IDBConnection connection = null)
                    {
            SerializableDictionary<int, List<AuthoriserJustification>> lstJustifications = new SerializableDictionary<int, List<AuthoriserJustification>>();

            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                databaseConnection.sqlexecute.Parameters.Add("@expenses", SqlDbType.Structured);
                List<SqlDataRecord> integers = new List<SqlDataRecord>();
                SqlMetaData[] rowMetaData = { new SqlMetaData("c1", SqlDbType.Int) };
                foreach (int i in expenseIds)
                {
                    var row = new SqlDataRecord(rowMetaData);
                    row.SetInt32(0, i);
                    integers.Add(row);
                }

                databaseConnection.sqlexecute.Parameters["@expenses"].Value = integers;
                cEmployees employees = new cEmployees(this.accountid);

                using (IDataReader reader = databaseConnection.GetReader("GetSavedExpensesFlagsAuthoriserJustifications", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        int flaggeditemid = reader.GetInt32(0);
                        int stage = reader.GetByte(1);
                        int employeeid = reader.GetInt32(2);
                        string justification = reader.GetString(3);
                        DateTime datestamp = reader.GetDateTime(4);
                        int? delegateID = null;
                        if (!reader.IsDBNull(5))
                        {
                            delegateID = reader.GetInt32(5);
                        }
                        string fullname = reader.GetString(6);
                        List<AuthoriserJustification> justifications;
                        lstJustifications.TryGetValue(flaggeditemid, out justifications);
                        if (justifications == null)
                    {
                            justifications = new List<AuthoriserJustification>();
                            lstJustifications.Add(flaggeditemid, justifications);
                    }
                       
                        justifications.Add(new AuthoriserJustification(flaggeditemid, stage, fullname, justification, datestamp, delegateID));
                }

                reader.Close();
            }

                databaseConnection.sqlexecute.Parameters.Clear();
        }

            return lstJustifications;
        }

        private void setAccountCode(ref cExpenseItem item, cSubcat subcat)
        {
            string accountcode = string.Empty;
            cReasons clsreasons = new cReasons(accountid);


            cReason reason = clsreasons.getReasonById(item.reasonid);

            if (reason == null)
            {
                if (subcat.alternateaccountcode != string.Empty)
                {
                    accountcode = subcat.alternateaccountcode;
                    item.accountcode = accountcode;
                }
                return;
            }

            if (subcat.alternateaccountcode != string.Empty)
            {
                accountcode = subcat.alternateaccountcode;
            }
            else if (item.receipt == true && reason.accountcodevat != string.Empty)
            {
                accountcode = reason.accountcodevat;
            }
            else if (item.receipt == false && reason.accountcodenovat != string.Empty)
            {
                accountcode = reason.accountcodenovat;
            }
            item.accountcode = accountcode;

        }

        public List<cDepCostItem> getCostCodeBreakdown(int expenseId)
        {

            List<cDepCostItem> breakdown = new List<cDepCostItem>(); ;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            int departmentid, costcodeid, percentused, projectcodeid, expenseid;
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseId);

            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select savedexpenses_costcodes.expenseid, departmentid, costcodeid, percentused, projectcodeid from [savedexpenses_costcodes] where expenseid = @expenseid";

            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    expenseid = reader.GetInt32(reader.GetOrdinal("expenseid"));
                    if (reader.IsDBNull(reader.GetOrdinal("departmentid")) == false)
                    {
                        departmentid = reader.GetInt32(reader.GetOrdinal("departmentid"));
                    }
                    else
                    {
                        departmentid = 0;
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("costcodeid")) == false)
                    {
                        costcodeid = reader.GetInt32(reader.GetOrdinal("costcodeid"));
                    }
                    else
                    {
                        costcodeid = 0;
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("projectcodeid")) == false)
                    {
                        projectcodeid = reader.GetInt32(reader.GetOrdinal("projectcodeid"));
                    }
                    else
                    {
                        projectcodeid = 0;
                    }
                    percentused = reader.GetInt32(reader.GetOrdinal("percentused"));



                    breakdown.Add(new cDepCostItem(departmentid, costcodeid, projectcodeid, percentused));

                }
                reader.Close();
            }

            expdata.sqlexecute.Parameters.Clear();
            return breakdown;
        }

        /// <summary>
        /// Get all the UDFs associated with expense items.
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <returns>Returns UDFs for expense items</returns>
        public List<UserDefinedFieldValue> GetExpenseItemDefinitionUDFs(int accountId)
        {
            SortedList<int, cUserDefinedField> userDefinedList = this.GetUserDefinedFieldsForExpenseItems(accountId);
            var userDefinedFieldValues = new List<UserDefinedFieldValue>();

            foreach (KeyValuePair<int, cUserDefinedField> keyValuePair in userDefinedList)
            {
                userDefinedFieldValues.Add(new UserDefinedFieldValue(keyValuePair.Key, keyValuePair.Value));
            }

            return userDefinedFieldValues;
        }

        /// <summary>
        /// Gets the user defined fields for expense items.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <returns>A sorted list of udf ids, and their <see cref="cUserDefinedField">cUserDefinedField</see></returns>
        private SortedList<int, cUserDefinedField> GetUserDefinedFieldsForExpenseItems(int accountId)
        {
            var userDefinedFields = new cUserdefinedFields(accountId);
            var tables = new cTables(accountId);
            const string ExpenseTableId = "65394331-792e-40b8-af8b-643505550783";
            cTable table = tables.GetTableByID(new Guid(ExpenseTableId));
            SortedList<int, cUserDefinedField> expenseItemDefinitionUdf = new SortedList<int, cUserDefinedField>();
            foreach (var field in userDefinedFields.UserdefinedFields.Values)
            {
                if (field.table.TableID == table.TableID)
                {
                    expenseItemDefinitionUdf.Add(field.order, field);
                }
            }

            return expenseItemDefinitionUdf;
        }

        private void InsertCostCodeBreakdown(bool edit, cExpenseItem item)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            List<cDepCostItem> breakdown = item.costcodebreakdown;
            if (item.parent != null)
            {
                breakdown = item.parent.costcodebreakdown;
            }

            if (breakdown == null || breakdown.Count == 0) //no items
            {
                return;
            }

            if (edit) //delete current breakdown
            {
                deleteCostCodeBreakdown(item.expenseid);
            }

            foreach (cDepCostItem costcode in breakdown)
            {
                if (costcode.percentused > 0)
                {
                    if (costcode.departmentid == 0)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@departmentid", DBNull.Value);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@departmentid", costcode.departmentid);
                    }
                    if (costcode.costcodeid == 0)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@costcodeid", DBNull.Value);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@costcodeid", costcode.costcodeid);
                    }
                    if (costcode.projectcodeid == 0)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@projectcodeid", DBNull.Value);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@projectcodeid", costcode.projectcodeid);
                    }
                    expdata.sqlexecute.Parameters.AddWithValue("@expenseid", item.expenseid);
                    expdata.sqlexecute.Parameters.AddWithValue("@percentused", costcode.percentused);
                    strsql = "insert into [savedexpenses_costcodes] (expenseid, departmentid, costcodeid, percentused,projectcodeid) values (@expenseid,@departmentid,@costcodeid,@percentused,@projectcodeid)";

                    expdata.ExecuteSQL(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                }
            }
            return;
        }

        private void deleteCostCodeBreakdown(int expenseid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseid);
            strsql = "delete from [savedexpenses_costcodes] where expenseid = @expenseid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        private void calculateAmountPayable(ref cExpenseItem item, cExpenseItem olditem, cSubcat subcat)
        {
            decimal amountpayable;
            bool ccusersettles = false;

            if ((item.itemtype == ItemType.CreditCard || item.itemtype == ItemType.PurchaseCard) && item.transactionid > 0)
            {
                cCardStatements clsstatements = new cCardStatements(accountid);
                cCardTransaction transaction = clsstatements.getTransactionById(item.transactionid);
                cCardStatement statement = clsstatements.getStatementById(transaction.statementid);
                ccusersettles = statement.Corporatecard.claimantsettlesbill;
            }
            if (((item.itemtype == ItemType.CreditCard || item.itemtype == ItemType.PurchaseCard) && !ccusersettles) || !subcat.reimbursable)
            {
                amountpayable = 0;
            }

            else if (item.floatid != 0)
            {
                decimal allocatedamount = 0;
                cFloats clsfloats = new cFloats(accountid);
                cFloat reqfloat = clsfloats.GetFloatById(item.floatid);
                if (olditem != null)
                {
                    if (olditem.floatid == item.floatid) //add allocation back on, adding exisitng app
                    {
                        allocatedamount = olditem.total;
                    }
                }
                amountpayable = reqfloat.calculateFloatValue(item.expenseid, item.total, allocatedamount);
            }
            else
            {
                amountpayable = item.total;
            }

            item.amountpayable = Math.Round(amountpayable, 2, MidpointRounding.AwayFromZero);
        }

        #endregion

        /// <summary>
        /// Duplicate the given expense item as -ve and +ve.
        /// </summary>
        /// <param name="item">
        /// The item to duplicate.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> new expense ID.
        /// </returns>
        public int DuplicateExpense(cExpenseItem item)
        {
            var result = 0;
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@expenseId", item.expenseid);
                connection.AddReturn("@returnValue", SqlDbType.Int);
                connection.ExecuteProc("dbo.DuplicateExpense");
                result = connection.GetReturnValue<int>("@returnValue");
            }

            return result;
        }

        /// <summary>
        /// Runs the expense item and claim through a series of validations to determine if it can be edited, or added to a claim.
        /// </summary>
        /// <param name="stage">
        /// The stage the claim is presently at
        /// </param>
        /// <param name="claimEmployeeId">
        /// The Id of the employee who owns the claim
        /// </param>
        /// <param name="claimCheckerId">
        /// The Id of the employee who is checking the claim
        /// </param>
        /// <param name="itemReturned">
        /// Has the item been returned?
        /// </param>
        /// <param name="itemCheckerId">
        /// The Id of the employee who is checking the expense item
        /// </param>
        /// <param name="itemEdited">
        /// Has the item been edited?
        /// </param>
        /// <param name="canEditPreviousClaim">
        /// Can previous claims be exited?
        /// </param>
        /// <param name="user">
        /// The currenct user
        /// </param>
        /// <param name="reasonForAmendment">
        /// The reason For amendment.
        /// </param>
        /// <param name="accountProperties">
        /// An instance of <see cref="cAccountProperties"/> 
        /// </param>
        /// <returns>
        /// The <see cref="ExpenseItemPermissionResult"/>ExpenseItemPermissionResult
        /// </returns>
        public ExpenseItemPermissionResult ExpenseItemPermissionCheck(ClaimStage stage, int claimEmployeeId, int claimCheckerId, bool itemReturned, int? itemCheckerId, bool itemEdited, bool canEditPreviousClaim, ICurrentUser user, string reasonForAmendment, cAccountProperties accountProperties)
        {
            switch (stage)
            {
                case ClaimStage.Current:
                    if (claimEmployeeId != user.EmployeeID)
                    {
                        return ExpenseItemPermissionResult.EmployeeDoesNotOwnClaim;
                    }

                    break;
                case ClaimStage.Submitted:

                    if (claimEmployeeId == user.EmployeeID)
                    {
                        if (!itemReturned && !accountProperties.AllowEmployeeInOwnSignoffGroup)
                        {
                            //employee attempting to edit a submitted expense item that has not been returned
                            //and the account property does not permit approver to approver own claim
                            return ExpenseItemPermissionResult.ClaimHasBeenSubmitted;
                        }

                        break;
                    }

                    if (claimCheckerId != user.EmployeeID && itemCheckerId != user.EmployeeID)
                    {
                        return ExpenseItemPermissionResult.ClaimHasBeenSubmitted;
                    }

                    //editing as approver, so check that the reason for amendment has been provided
                    if (string.IsNullOrEmpty(reasonForAmendment))
                    {
                        return ExpenseItemPermissionResult.NoReasonForAmendmentProvidedByApprover;
                    }

                    break;
                    
                case ClaimStage.Previous:
                    if (!canEditPreviousClaim || claimEmployeeId == user.EmployeeID ||
                       !user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ClaimViewer, true, false))
                    {
                        return ExpenseItemPermissionResult.ClaimHasBeenApproved;
                    }
                    break;
            }

            if (itemEdited)
            {
                return ExpenseItemPermissionResult.ExpenseItemHasBeenEdited;
            }

            return ExpenseItemPermissionResult.Pass;
        }

        /// <summary>
        /// This method apply the mileage cap on journey step if the Enforce mileage cap on Home to Office journeys is enabled
        /// </summary>
        /// <param name="subcat">Expense item for which  the Enforce mileage cap on Home to Office journeys is enabled</param>
        /// <param name="employeeHomeLocationId">Home Address Location set for employee</param>
        /// <param name="employeeWorkLocationId">Employee Work Location set for employee</param>
        /// <param name="step">Journey step for which the mileage cap on Home to Office journeys is applied </param>
        private static void EnforceHomeToOfficeMileageCap(cSubcat subcat, int employeeHomeLocationId, int employeeWorkLocationId,
            cJourneyStep step)
        {
            if (!subcat.HomeToOfficeAlwaysZero && subcat.EnforceToOfficeMileageCap)
            {
                if (subcat.HomeToOfficeMileageCap.HasValue)
                {
                    var mileageCap = Convert.ToDecimal(subcat.HomeToOfficeMileageCap.Value);
                    if ((step.startlocation.Identifier == employeeHomeLocationId &&
                         step.endlocation.Identifier == employeeWorkLocationId) ||
                        (step.startlocation.Identifier == employeeWorkLocationId &&
                         step.endlocation.Identifier == employeeHomeLocationId))
                    {
                        if (step.nummiles > mileageCap)
                        {
                            step.nummiles = mileageCap;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Represents a receipt that will be shown in WebUIs and mobile receipts.
    /// At some point in future this will be changed for the <see cref="Receipt"/> class.
    /// </summary>
    public class AttachedReceipt
    {
        public int receiptid { get; set; }
        public int claimid { get; set; }
        public List<int> expenseids { get; set; }
        public string filename { get; set; }
        public DateTime createdon { get; set; }
        public int createdby { get; set; }
        public string mimeType { get; set; }
        public string extension { get; set; }
        public bool validImageForBrowser { get; set; }

        /// <summary>
        /// Converts from a <see cref="SpendManagementLibrary.Expedite.Receipt"/> to this AttachedReceipt.
        /// </summary>
        /// <param name="receipt">The eceipt to convert.</param>
        /// <param name="accountId">The account Id to use when determining the GlobalMimeTypes.</param>
        /// <returns></returns>
        public static AttachedReceipt FromReceipt(Receipt receipt, int accountId)
        {
            return new AttachedReceipt
            {
                expenseids = receipt.OwnershipInfo.ClaimLines,
                receiptid = receipt.ReceiptId,
                filename = receipt.TemporaryUrl,
                createdon = receipt.CreatedOn,
                createdby = receipt.CreatedBy,
                mimeType = GetMimeType(receipt.Extension, accountId),
                extension = receipt.Extension,
                validImageForBrowser = (".jpg.jpeg.gif.png.bmp".Contains(receipt.Extension))
            };
        }

        /// <summary>
        /// Returns mimetype from globalMimeTypes for the supplied filename.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMimeType(string fileName, int accountId)
        {
            var result = String.Empty;
            if (fileName != null)
            {
                if (!fileName.EndsWith("."))
                {
                    int periodLocation = fileName.IndexOf(".") + 1;
                    string extention = fileName.Substring(periodLocation).ToUpper();
                    var globalMime = new cGlobalMimeTypes(accountId);
                    cGlobalMimeType globalType = globalMime.getMimeTypeByExtension(extention);
                    if (globalType != null)
                    {
                        result = globalType.MimeHeader;
                    }
                }
            }

            return result;
        }
    }
}

public struct sExpenseItemDetails
{
    public int expenseid;
    public ItemType itItemtype;
    public bool receipt;
    public decimal net;
    public decimal vat;
    public decimal total;
    public int subcatid;
    public decimal miles;
    public decimal bmiles;
    public decimal pmiles;
    public byte staff;
    public byte others;
    public int floatid;
    public int companyid;

    public bool home;
    public int plitres;
    public int blitres;
    public string attendees;
    public decimal tip;
    public bool normalreceipt;
    public bool receiptattached;
    public DateTime allowancestartdate;
    public DateTime allowanceenddate;
    public int nopassengers;
    public int carid;
    public decimal allowancededuct;
    public int allowanceid;
    public byte nonights;
    public byte norooms;
    public double quantity;
    public byte directors;
    public decimal amountpayable;
    public int hotelid;

    public string vatnumber;
    public byte personalguests;
    public byte remoteworkers;
    public string accountcode;
    public bool purchasecard;
    public bool creditcard;
    public int transactionid;
    public System.Collections.Generic.SortedList<int, object> userdefined;
    public int mileageid;
    public System.Collections.Generic.SortedList<int, cJourneyStep> journeysteps;
    public int currencyid;
    public MileageUOM unit;
    public double exchangerate;
    public DateTime date;
    public int reasonid;
    public string otherdetails;
    //public string assignmentnum;
    public long assignmentid;
    public int ValidationProgress;
    public int ValidationCount;
}
