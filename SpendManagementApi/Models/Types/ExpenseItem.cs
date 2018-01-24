namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;
    using Interfaces;
    using Utilities;

    using SpendManagementLibrary;
    using SpendManagementApi.Common.Enums;
    using System.Linq;

    using Expedite;

    using SpendManagementApi.Common.Enum;
    using SpendManagementApi.Models.Types.Employees;

    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Flags;
    using SpendManagementLibrary.Helpers;

    using Spend_Management.expenses.code;

    /// <summary>
    /// An Expense Item is part of a User's <see cref="Claim">Claim</see>. 
    /// An <see cref="ExpenseSubCategory">ExpenseSubCategory</see> is like a template against which
    /// a User submits expenses. ExpenseItem is like an instance of that template.
    /// </summary>
    public class ExpenseItem : BaseExternalType, IApiFrontForDbObject<cExpenseItem, ExpenseItem>
    {
        #region Core Properties

        /// <summary>
        /// The unique Id of this item in the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The friendly code for this item.
        /// </summary>
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// The Id of the <see cref="ExpenseSubCategory">ExpenseSubCategory</see> (or template) that this item is an instance of.
        /// </summary>
        public int ExpenseSubCategoryId { get; set; }

        /// <summary>
        /// Gets or set the expense sub category description
        /// </summary>
        public string ExpenseSubCategoryDescription { get; set; }

        /// <summary>
        /// The Id of the parent ExpenseItem.
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// If this item has been split then this contains a list of <see cref="ExpenseItem">ExpenseItem</see> making up the split expense
        /// </summary>
        public List<ExpenseItem> SplitItems { get; set; }

        /// <summary>
        /// The Id of the claim to which this item belongs.
        /// </summary>
        public int ClaimId { get; set; }

        /// <summary>
        /// The Id of the <see cref="ClaimReason">ClaimReason</see> applicable to this item.
        /// </summary>
        public int ClaimReasonId { get; set; }

        /// <summary>
        /// The value for the reasonId lookup.
        /// </summary>
        public string ClaimReasonInfo { get; set; }

        /// <summary>
        /// The Id of the currency this item is to be claimed in.
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// The symbol of the currency this item is to be claimed in.
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// The Id of the country this item is to be claimed in.
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// The type of this item.
        /// </summary>
        public ExpenseItemType ItemType { get; set; }

        /// <summary>
        /// The date of this item.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Whether this item is the primary item.
        /// </summary>
        public bool PrimaryItem { get; set; }

        /// <summary>
        /// Whether this item is returned to the claimant.
        /// </summary>
        public bool Returned { get; set; }

        /// <summary>
        /// The notes for disputing this item.
        /// </summary>
        public string DisputeNotes { get; set; }

        /// <summary>
        /// Whether the item should be temporarily allowed.
        /// </summary>
        public bool TempAllow { get; set; }

        /// <summary>
        /// Whether this item has been corrected.
        /// </summary>
        public bool Corrected { get; set; }

        /// <summary>
        /// The Id of the Item Checker.
        /// </summary>
        public int? ItemCheckerId { get; set; }

        /// <summary>
        /// The ID of the Team who is checking this expense item
        /// </summary>
        public int? ItemCheckerTeamId { get; set; }

        /// <summary>
        /// The Id of the float that has been configured.
        /// </summary>
        public int FloatId { get; set; }

        /// <summary>
        /// Gets or sets the description for the advance.
        /// </summary>
        public string AdvanceDescription { get; set; }

        /// <summary>
        /// The Id of the transaction.
        /// </summary>
        public int TransactionId { get; set; }

        #endregion Core Properties


        #region Financial Totals Properties

        /// <summary>
        /// The quantity of items
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// The Net value of this item.
        /// </summary>
        public decimal Net { get; set; }

        /// <summary>
        /// The amount of VAT on this item.
        /// </summary>
        public decimal VAT { get; set; }

        /// <summary>
        /// The total value of this item.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// The amount payable.
        /// </summary>
        public decimal AmountPayable { get; set; }

        /// <summary>
        /// The VAT number of the receipt issuer.
        /// </summary>
        public string VATNumber { get; set; }

        /// <summary>
        /// The account code.
        /// </summary>
        public string AccountCode { get; set; }

        /// <summary>
        /// The Base Currency (in case a conversion is needed).
        /// </summary>
        public int BaseCurrency { get; set; }

        /// <summary>
        /// The Global Base Currency.
        /// </summary>
        public int BaseCurrencyGlobal { get; set; }

        /// <summary>
        /// The VAT value for the foreign country.
        /// </summary>
        public decimal ForeignVAT { get; set; }

        /// <summary>
        /// The exchange rate between the home country and the item's target country.
        /// </summary>
        public double ExchangeRate { get; set; }

        /// <summary>
        /// The exchange rate, but the global value.
        /// </summary>
        public double ExchangeRateGlobal { get; set; }

        /// <summary>
        /// The total after conversion.
        /// </summary>
        public decimal ConvertedTotal { get; set; }

        /// <summary>
        /// The Grand Total.
        /// </summary>
        public decimal GrandTotal { get; set; }

        /// <summary>
        /// The Global Total.
        /// </summary>
        public decimal GlobalTotal { get; set; }

        /// <summary>
        /// The Global Grand Total.
        /// </summary>
        public decimal GrandTotalGlobal { get; set; }

        /// <summary>
        /// The Converted Global Grand Total.
        /// </summary>
        public decimal GrandTotalConverted { get; set; }

        /// <summary>
        /// The Net Grand Total.
        /// </summary>
        public decimal GrandTotalNet { get; set; }

        /// <summary>
        /// The Grand Total VAT.
        /// </summary>
        public decimal GrandTotalVAT { get; set; }

        /// <summary>
        /// The amount of the Grand Total that is payable.
        /// </summary>
        public decimal GrandTotalAmountPayable { get; set; }

        /// <summary>
        /// Another grand total.
        /// </summary>
        public int GrandTotalOther { get; set; }

        /// <summary>
        /// The grand total for remote workers.
        /// </summary>
        public int GrandTotalRemoteWorkers { get; set; }

        /// <summary>
        /// The grand total for personal guests.
        /// </summary>
        public int GrandTotalPersonalGuests { get; set; }

        #endregion Financial Totals Properties


        #region Receipt Properties

        /// <summary>
        /// Whether the receipt has a VAT number and VAT rate 
        /// </summary>
        public bool UserSaysHasReceipts { get; set; }

        /// <summary>
        /// Whether the expense item will have a receipt.
        /// </summary>
        public bool NormalReceipt { get; set; }

        /// <summary>
        /// Whether this item has a receipt attached.
        /// </summary>
        public bool ReceiptAttached { get; set; }

        #endregion Receipt Properties


        #region Travel Properties

        /// <summary>
        /// The Id of the vehicle used in this item.
        /// </summary>
        public int CarId { get; set; }

        /// <summary>
        /// Gets or sets the vehicle description.
        /// </summary>
        public string VehicleDescription { get; set; }

        /// <summary>
        /// The Id of the Mileage item.
        /// </summary>
        public int MileageId { get; set; }

        /// <summary>
        /// The sorted list of int,<see cref="JourneyStep"> JourneyStep</see> that apply to this item.
        /// </summary>
        public List<JourneyStep> JourneySteps { get; set; }

        /// <summary>
        /// The distance's unit of measure of measure.
        /// </summary>
        public MileageUom DistanceUom { get; set; }

        /// <summary>
        /// The method by which mileage from home to office is deducted from travel for this item.
        /// </summary>
        public SpendManagementApi.Common.Enums.HomeToLocationType HomeToOfficeDeductionMethod { get; set; }

        /// <summary>
        /// Number of passengers present in the vehicle for this item.
        /// </summary>
        public byte NumPassengers { get; set; }

        /// <summary>
        /// The number of miles for this item (if this is a travelling expense).
        /// </summary>
        public decimal Miles { get; set; }

        /// <summary>
        /// The number of business miles travelled (if this is a travelling expense).
        /// </summary>
        public decimal MileageBusiness { get; set; }

        /// <summary>
        /// The number of personal miles travelled (if this is a travelling expense).
        /// </summary>
        public decimal MileagePersonal { get; set; }

        /// <summary>
        /// The number of litres of fuel for personal usage (if this is a travelling expense).
        /// </summary>
        public int FuelLitresPersonal { get; set; }

        /// <summary>
        /// The number of litres of fuel for business usage (if this is a travelling expense).
        /// </summary>
        public int FuelLitresBusiness { get; set; }

        #endregion Travel Properties


        #region Meals Properties

        /// <summary>
        /// A string list of people who were present.
        /// </summary>
        public string Attendees { get; set; }

        /// <summary>
        /// The number of staff present for this item.
        /// </summary>
        public byte NumStaff { get; set; }

        /// <summary>
        /// The number of directors present for this item.
        /// </summary>
        public byte NumDirectors { get; set; }

        /// <summary>
        /// The number of directors present for this item.
        /// </summary>
        public byte NumPersonalGuests { get; set; }

        /// <summary>
        /// The number of directors present for this item.
        /// </summary>
        public byte NumRemoteWorkers { get; set; }

        /// <summary>
        /// The number of others (non staff) for this item.
        /// </summary>
        public byte NumOthers { get; set; }

        /// <summary>
        /// Whether this item was incurred in claimant's home town.
        /// </summary>
        public bool EventInHomeCity { get; set; }

        /// <summary>
        /// The tip given for a meal type item.
        /// </summary>
        public decimal Tip { get; set; }

        #endregion Meals Properties


        #region Accommodation Properties

        /// <summary>
        /// Number of rooms for an accommodation item.
        /// </summary>
        public byte NumRooms { get; set; }

        /// <summary>
        /// The number of nights stayed.
        /// </summary>
        public byte NumNights { get; set; }

        /// <summary>
        /// The Id of the hotel.
        /// </summary>
        public int HotelId { get; set; }

        /// <summary>
        /// The name of the hotel.
        /// </summary>
        public string HotelName { get; set; }

        #endregion Accommodation Properties


        #region General Properties

        /// <summary>
        /// The reason given for the existence of this item.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Notes
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// The company id of the other company related to this item.
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// The name of the company related to this item.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// The Id of the From.
        /// </summary>
        public int FromId { get; set; }

        /// <summary>
        /// Gets or sets the From address description
        /// </summary>
        public string FromDescription { get; set; }

        /// <summary>
        /// The Id of the To.
        /// </summary>
        public int ToId { get; set; }

        /// <summary>
        /// Gets or sets the To address description
        /// </summary>
        public string ToDescription { get; set; }

        #endregion General Properties


        #region Allowance Properties

        /// <summary>
        /// The ID of the allowance for this item.
        /// </summary>
        public int AllowanceId { get; set; }

        /// <summary>
        /// The start date of the allowance.
        /// </summary>
        public DateTime AllowanceStartDate { get; set; }

        /// <summary>
        /// The end date of the allowance.
        /// </summary>
        public DateTime AllowanceEndDate { get; set; }

        /// <summary>
        /// The ammount deductable from the allowance for this item.
        /// </summary>
        public decimal AllowanceDeduction { get; set; }

        #endregion Allowance Properties


        #region Misc Properties

        /// <summary>
        /// The list of <see cref="Employees.CostCentreBreakdown">Cost Centre Breakdowns</see> that apply to this item.
        /// </summary>
        public List<Employees.CostCentreBreakdown> CostCentreBreakdowns { get; set; }


        /// <summary>
        /// The list of flags for this item.
        /// </summary>
        public List<FlagSummary> Flags { get; set; }

        /// <summary>
        /// The list blocked flags for this item
        /// </summary>
        public FlaggedItemsManager BlockedFlags { get; set; }
        
        /// <summary>
        /// User defined data for this item.
        /// </summary>
        public List<UserDefinedFieldValue> UserDefined { get; set; }

        /// <summary>
        /// Whether this was added as a mobile expense.
        /// </summary>
        public bool AddedAsMobileExpense { get; set; }

        /// <summary>
        /// The Id of the type of mobile device this item was added from.
        /// </summary>
        public int AddedByMobileDeviceTypeId { get; set; }

        /// <summary>
        /// The Id for the attached ESR (if there is one)
        /// </summary>
        public long ESRAssignmentId { get; set; }

        /// <summary>
        /// Gets or sets the ESR Assignment description.
        /// </summary>
        public string ESRAssignmentDescription { get; set; }

        /// <summary>
        /// The current progress of this item in validation.
        /// </summary>
        public ExpenseValidationProgress ValidationProgress { get; set; }

        /// <summary>
        /// A list of the Ids of the Results of validation on this expense item.
        /// See <see cref="ExpenseValidationResult">ExpenseValidationResults</see>.
        /// </summary>
        public List<int> ValidationResults { get; internal set; }


        /// <summary>
        /// Gets or sets a value indicating whether the expense item has been edited.
        /// </summary>
        public bool Edited { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the expense item is paid.
        /// </summary>
        public bool Paid { get; set; }

        /// <summary>
        /// Gets or sets the original expense id if this is a copied item from a previously paid then edited expense item.
        /// </summary>
        public int? OriginalExpenseId { get; set; }

        /// <summary>
        /// Gets or sets the bank account Id for the expense item
        /// </summary>
        public int BankAccountId { get; set; }

        /// <summary>
        /// Gets or sets the validation progress status by any expedite operator
        /// </summary>
        public ExpediteOperatorValidationProgress OperatorValidationProgress { get; set; }

        /// <summary>
        /// Gets or sets the expense action outcome.
        /// </summary>
        public ExpenseActionOutcome ExpenseActionOutcome { get; set; }

        /// <summary>
        /// Gets or sets the mobile metric device id, which is the internal identifier for the device.
        /// </summary>
        public int? MobileMetricDeviceId { get; set; }

        #endregion Misc Properties


        /// <summary>
        /// Initializes a new instance of the<see cref="ExpenseItem">ExpenseItem</see> class. 
        /// </summary>
        public ExpenseItem()
        {           
        }   

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <param name="user">The current user</param>
        /// <returns>An api Type</returns>
        public ExpenseItem From(cExpenseItem dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            Id = dbType.expenseid;
            ExpenseSubCategoryId = dbType.subcatid;
            ParentId = dbType.parent != null ? dbType.parent.expenseid : (int?)null;

            List<cDepCostItem> costcodes = actionContext.ExpenseItems.getCostCodeBreakdown(dbType.expenseid);
            List<CostCentreBreakdown> costcodeBreakdowns = costcodes.Cast<List<CostCentreBreakdown>>();
                  
            var splitItems = new List<ExpenseItem>();

            foreach (cExpenseItem item in dbType.splititems)
            {
                splitItems.Add(new ExpenseItem().From(item, actionContext));
            }

            SplitItems = splitItems;
            ClaimId = dbType.claimid;
            ClaimReasonId = dbType.reasonid;
            CurrencyId = dbType.currencyid;
            CountryId = dbType.countryid;
            ItemType = (ExpenseItemType)dbType.itemtype;
            Date = dbType.date;
            PrimaryItem = dbType.primaryitem;
            ESRAssignmentId = dbType.ESRAssignmentId;
            Returned = dbType.returned;
            DisputeNotes = dbType.Dispute;
            TempAllow = dbType.tempallow;
            Corrected = dbType.corrected;
            ItemCheckerId = dbType.itemCheckerId;
            ItemCheckerTeamId = dbType.ItemCheckerTeamId;
            FloatId = dbType.floatid;
            TransactionId = dbType.transactionid;
            Quantity = dbType.quantity;
            Net = dbType.net;
            VAT = dbType.vat;
            Total = dbType.total;
            AmountPayable = dbType.amountpayable;
            VATNumber = dbType.vatnumber;
            AccountCode = dbType.accountcode;
            BaseCurrency = dbType.basecurrency;
            BaseCurrencyGlobal = dbType.globalbasecurrency;
            ForeignVAT = dbType.foreignvat;
            ExchangeRate = dbType.exchangerate;
            ExchangeRateGlobal = dbType.globalexchangerate;
            ConvertedTotal = dbType.convertedtotal;
            GrandTotal = dbType.grandtotal;
            GlobalTotal = dbType.globaltotal;
            GrandTotalGlobal = dbType.grandGlobalTotal;
            GrandTotalConverted = dbType.convertedgrandtotal;
            GrandTotalNet = dbType.grandnettotal;
            GrandTotalVAT = dbType.grandvattotal;
            GrandTotalAmountPayable = dbType.grandamountpayabletotal;
            GrandTotalOther = dbType.othergrandtotal;
            GrandTotalRemoteWorkers = dbType.remoteworkersgrandtotal;
            GrandTotalPersonalGuests = dbType.personalguestsgrandtotal;
            UserSaysHasReceipts = dbType.receipt;
            NormalReceipt = dbType.normalreceipt;
            ReceiptAttached = dbType.receiptattached;
            CarId = dbType.carid;
            MileageId = dbType.mileageid;
            DistanceUom = (MileageUom)dbType.journeyunit;
            HomeToOfficeDeductionMethod = (SpendManagementApi.Common.Enums.HomeToLocationType)dbType.homeToOfficeDeductionMethod;
            NumPassengers = dbType.nopassengers;
            Miles = dbType.miles;
            MileageBusiness = dbType.bmiles;
            MileagePersonal = dbType.pmiles;
            FuelLitresPersonal = dbType.plitres;
            FuelLitresBusiness = dbType.blitres;
            Attendees = dbType.attendees;
            NumStaff = dbType.staff;
            NumDirectors = dbType.directors;
            NumPersonalGuests = dbType.personalguests;
            NumRemoteWorkers = dbType.remoteworkers;
            NumOthers = dbType.others;
            EventInHomeCity = dbType.home;
            Tip = dbType.tip;
            ReferenceNumber = dbType.refnum;
            NumRooms = dbType.norooms;
            NumNights = dbType.nonights;
            HotelId = dbType.hotelid;
            Reason = dbType.reason;
            Notes = dbType.Note;
            CompanyId = dbType.companyid;
            FromId = dbType.fromid;
            ToId = dbType.toid;
            AllowanceId = dbType.allowanceid;
            AllowanceStartDate = dbType.allowancestartdate;
            AllowanceEndDate = dbType.allowanceenddate;
            AllowanceDeduction = dbType.allowancededuct;
            CostCentreBreakdowns = costcodeBreakdowns;

        
            if (dbType.flags != null)
            {
                List<FlagSummary> flagSummaries = new List<FlagSummary>();

                foreach (var flags in dbType.flags)
                {
                    var flagSummary = new FlagSummary().From(flags, actionContext);

                    var flagManager = new FlagManagement(actionContext.AccountId);              
                    var flagDetails = flagManager.GetBy(flags.FlagID);
                    flagSummary.FlaggedItem.DisplayFlagImmediately = flagDetails.DisplayFlagImmediately;

                    flagSummaries.Add(flagSummary);
                }

                Flags = flagSummaries;
            }

            if (dbType.journeysteps != null)
            {
                List<JourneyStep> itemJourneySteps = dbType.journeysteps.Select(journey => new JourneyStep(actionContext.AccountId).From(journey.Value, actionContext)).ToList();

                JourneySteps = itemJourneySteps.Count > 0 ? itemJourneySteps : null;
            }
            else
            {
                JourneySteps = null;
            }

            if (dbType.userdefined != null)
            {
                List<UserDefinedFieldValue> udfList = dbType.userdefined.ToUserDefinedFieldValueList();
                UserDefined = udfList.Count > 0 ? udfList : null;
            }
            else
                {
                UserDefined = null;
            }

            AddedAsMobileExpense = dbType.addedAsMobileExpense;
            AddedByMobileDeviceTypeId = dbType.addedByMobileDeviceTypeId;

            ValidationProgress = (ExpenseValidationProgress)dbType.ValidationProgress;
            OperatorValidationProgress = (ExpediteOperatorValidationProgress)dbType.OperatorValidationProgress;

            var subcat = actionContext.SubCategories.GetSubcatById(ExpenseSubCategoryId);

            if (subcat.Validate && subcat.ValidationRequirements.Any())
            {
                var validationManager = new ExpenseValidationManager(actionContext.AccountId);
                dbType.ValidationResults = validationManager.GetResultsForExpenseItem(dbType.expenseid).ToList();
                if (dbType.ValidationResults != null)
                {
                    ValidationResults = dbType.ValidationResults.Select(r => r.Id).ToList();
                }
            }

            this.Edited = dbType.Edited;
            this.Paid = dbType.Paid;
            this.OriginalExpenseId = dbType.OriginalExpenseId;

            this.BankAccountId = dbType.bankAccountId == null ? 0 : dbType.bankAccountId.Value;
            this.MobileMetricDeviceId = dbType.MobileMetricDeviceId;

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public cExpenseItem To(IActionContext actionContext)
        {
            var item = new cExpenseItem(Id, (ItemType)ItemType, MileageBusiness, MileagePersonal, Reason, UserSaysHasReceipts,
                Net, VAT, Total, ExpenseSubCategoryId, Date, NumStaff, NumOthers, CompanyId, Returned, EventInHomeCity, ReferenceNumber, ClaimId,
                FuelLitresPersonal, FuelLitresBusiness, CurrencyId, Attendees, Tip, CountryId, ForeignVAT, ConvertedTotal, ExchangeRate, TempAllow, ClaimReasonId,
                NormalReceipt, AllowanceStartDate, AllowanceEndDate, CarId, AllowanceDeduction, AllowanceId, NumNights, Quantity, NumDirectors, AmountPayable,
                HotelId, PrimaryItem, NumRooms, VATNumber, NumPersonalGuests, NumRemoteWorkers, AccountCode, BaseCurrency, BaseCurrencyGlobal, ExchangeRateGlobal,
                GlobalTotal, FloatId, Corrected, ReceiptAttached, TransactionId, CreatedOn, CreatedById, ModifiedOn ?? DateTime.UtcNow, ModifiedById ?? 0, MileageId, 
                (MileageUOM)DistanceUom, ESRAssignmentId, (SpendManagementLibrary.HomeToLocationType)HomeToOfficeDeductionMethod,
                AddedAsMobileExpense, AddedByMobileDeviceTypeId, Notes, DisputeNotes, ItemCheckerId, ItemCheckerTeamId, (SpendManagementLibrary.Enumerators.Expedite.ExpenseValidationProgress)ValidationProgress, edited: this.Edited, paid: this.Paid, originalExpenseId: this.OriginalExpenseId, bankAccountId: this.BankAccountId, mobileMetricDeviceId: this.MobileMetricDeviceId);

            var account = actionContext.Accounts.GetAccountByID(actionContext.AccountId);
            var subcat = actionContext.SubCategories.GetSubcatById(item.subcatid);
            var employee = actionContext.Employees.GetEmployeeById(actionContext.EmployeeId);
            var validateStageInSignoff = false;

            if (CostCentreBreakdowns != null)
            {
                var itemCostCodeBreakdown = CostCentreBreakdowns.Select(costcode => new cDepCostItem((int)costcode.DepartmentId, (int)costcode.CostCodeId, (int)costcode.ProjectCodeId, costcode.Percentage)).ToList();
                item.costcodebreakdown = itemCostCodeBreakdown;
            }

            if (UserDefined != null)
            {
                item.userdefined = UserDefined.ToSortedList();
            }

            if (JourneySteps != null)
            {
                var itemJourneySteps = new SortedList<int, cJourneyStep>();
                var vehicle = actionContext.EmployeeCars.GetCarByID(CarId);

                foreach (var journeyStep in JourneySteps)
                {
                    cJourneyStep journey = journeyStep.To(actionContext);

                    if ((!subcat.fromapp && !subcat.toapp) && vehicle.defaultuom == MileageUOM.KM)
                    {
                        //only convert if claimant has entered mileage as journey step builder takes care of conversion when entering a start, stop and end locations.
                        var miles = ConvertKilometersToMiles.PerformConversion(journey.NumActualMiles);
                        journey.NumActualMiles = miles;
                        journey.nummiles = miles;                     
                    }

                    itemJourneySteps.Add(journeyStep.StepNumber, journey);
                }

                item.journeysteps = itemJourneySteps;
            }

            if (employee.SignOffGroupID > 0)
            {
            var claimStages = actionContext.SignoffGroups.GetGroupById(employee.SignOffGroupID).stages.Values.Select(s => s.signofftype).ToList();
                validateStageInSignoff = claimStages.Contains(SpendManagementLibrary.SignoffType.SELValidation);
            }

    
            item.DetermineValidationProgress(account, subcat.Validate, validateStageInSignoff, actionContext.ExpenseValidation);

            return item;
        }
    }
}