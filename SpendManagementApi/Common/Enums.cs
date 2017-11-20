namespace SpendManagementApi.Common.Enums
{

    using System.ComponentModel;

    /// <summary>
    /// Used to allow users to specify a combiner for search queries
    /// </summary>
    public enum SearchOperator
    {
        /// <summary>
        /// Default operator for searches - And
        /// </summary>
        And,

        /// <summary>
        /// Or operator
        /// </summary>
        Or
    }

    /// <summary>
    /// Type of currency
    /// </summary>
    public enum CurrencyType
    {
        /// <summary>
        /// Static Currency Type
        /// </summary>
        Static, 

        /// <summary>
        /// Monthly Currency Type
        /// </summary>
        Monthly, 
        
        /// <summary>
        /// Range Currency Type
        /// </summary>
        Range
    }

    /// <summary>
    /// Gender Enum
    /// </summary>
    public enum Gender
    {
        /// <summary>
        /// Male Gender
        /// </summary>
        Male,

        /// <summary>
        /// Female Gender
        /// </summary>
        Female
    }

    /// <summary>
    /// Corporate Card Type
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// Purchase Card
        /// </summary>
        CreditCard = 1,

        /// <summary>
        /// Credit Card
        /// </summary>
        PurchaseCard
    }

    /// <summary>
    /// Address Type
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// Work Address
        /// </summary>
        Work,

        /// <summary>
        /// Home Address
        /// </summary>
        Home
    }

    /// <summary>
    /// Duty Of Care Document Type
    /// </summary>
    internal enum DutyOfCareDocumentType
    {
        /// <summary>
        /// Driving License Document
        /// </summary>
        DrivingLicense,

        /// <summary>
        /// Insurance Document
        /// </summary>
        Insurance,

        /// <summary>
        /// Mot Document
        /// </summary>
        Mot,

        /// <summary>
        /// Service Document
        /// </summary>
        Service
    }

    /// <summary>
    /// Notification Type
    /// </summary>
    public enum Notify
    {
        /// <summary>
        /// Just notify user of claim
        /// </summary>
        JustNotifyUserOfClaim = 1,

        /// <summary>
        /// User to check claim
        /// </summary>
        UserIsToCheckClaim
    }

    /// <summary>
    /// Holiday Provision
    /// </summary>
    public enum HolidayProvision
    {
        /// <summary>
        /// Take no action
        /// </summary>
        TakeNoAction = 1,
        
        /// <summary>
        /// Skip stage
        /// </summary>
        SkipStage,
        
        /// <summary>
        /// Assign claim to someone else
        /// </summary>
        AssignClaimToSomeoneElse
    }

    /// <summary>
    /// Stage Inclusion Type
    /// </summary>
    public enum StageInclusionType
    {
        /// <summary>
        /// None = 0
        /// </summary>
        None,

        /// <summary>
        /// Always
        /// </summary>
        Always = 1,

        /// <summary>
        /// Claim Total Exceeds
        /// </summary>
        ClaimTotalExceeds = 2,
        
        /// <summary>
        /// Expense Item Exceeds
        /// </summary>
        ExpenseItemExceeds = 3,
        
        /// <summary>
        /// Includes cost code
        /// </summary>
        IncludesCostCode = 4,
        
        /// <summary>
        /// Claim total below
        /// </summary>
        ClaimTotalBelow = 5,
        
        /// <summary>
        /// Includes expense item
        /// </summary>
        IncludesExpenseItem = 6,
        
        /// <summary>
        /// Older than days
        /// </summary>
        OlderThanDays = 7,

        /// <summary>
        /// Includes department
        /// </summary>
        IncludesDepartment = 8,

        /// <summary>
        /// Any expense item has failed validation twice or more
        /// </summary>
        ValidationFailedTwice = 9
    }

    /// <summary>
    /// Sign off type
    /// </summary>
    public enum SignoffType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Budget Holder
        /// </summary>
        BudgetHolder = 1,

        /// <summary>
        /// Employee
        /// </summary>
        Employee = 2,

        /// <summary>
        /// Team
        /// </summary>
        Team = 3,

        /// <summary>
        /// Line Manager
        /// </summary>
        LineManager = 4,

        /// <summary>
        /// Claimant selects own checker
        /// </summary>
        ClaimantSelectsOwnChecker = 5,

        /// <summary>
        /// Approval matrix
        /// </summary>
        ApprovalMatrix = 6,

        /// <summary>
        /// Determines by claimant from aproval matrix
        /// </summary>
        DeterminedByClaimantFromApprovalMatrix = 7,

        /// <summary>
        /// Cost code owner
        /// </summary>
        CostCodeOwner = 8,

        /// <summary>
        /// Assignment supervisor
        /// </summary>
        AssignmentSupervisor = 9,

        /// <summary>
        /// SEL Scan & Attach
        /// </summary>
        SELScanAttach = 100,

        /// <summary>
        /// SEL Validation
        /// </summary>
        SELValidation = 101
    }

    /// <summary>
    /// Date Range Type
    /// </summary>
    public enum DateRangeType
    {
        /// <summary>
        /// Before
        /// </summary>
        Before = 0,
        
        /// <summary>
        /// After or Equals To
        /// </summary>
        AfterOrEqualTo,

        /// <summary>
        /// Between
        /// </summary>
        Between,

        /// <summary>
        /// Any
        /// </summary>
        Any
    }

    /// <summary>
    /// Range Type
    /// </summary>
    public enum RangeType
    {
        /// <summary>
        /// Greater than or Equal to
        /// </summary>
        GreaterThanOrEqualTo = 0,

        /// <summary>
        /// Between
        /// </summary>
        Between,

        /// <summary>
        /// Less than
        /// </summary>
        LessThan,

        /// <summary>
        /// Any
        /// </summary>
        Any
    }

    /// <summary>
    /// Threshold Type
    /// </summary>
    public enum ThresholdType
    {
        /// <summary>
        /// Annual
        /// </summary>
        Annual = 0,

        /// <summary>
        /// Journey
        /// </summary>
        Journey
    }

    /// <summary>
    /// Calculation Type
    /// </summary>
    public enum CalculationType
    {
        /// <summary>
        /// Normal Item
        /// </summary>
        NormalItem = 1,

        /// <summary>
        /// Meal
        /// </summary>
        Meal,

        /// <summary>
        /// Pence per mile
        /// </summary>
        PencePerMile,

        /// <summary>
        /// Daily Allowance
        /// </summary>
        DailyAllowance,

        /// <summary>
        /// Fuel Receipt
        /// </summary>
        FuelReceipt,

        /// <summary>
        /// Pence per Mile Receipt
        /// </summary>
        PencePerMileReceipt,

        /// <summary>
        /// Fixed Allowance
        /// </summary>
        FixedAllowance,

        /// <summary>
        /// Fuel Card Mileage
        /// </summary>
        FuelCardMileage,

        /// <summary>
        /// Item Reimburse
        /// </summary>
        ItemReimburse
    }

    /// <summary>
    /// The area of the system the UDF applies to 
    /// </summary>
    public enum UDFAppliesTo
    {
        /// <summary>
        /// Employees
        /// </summary>
        Employees = 0,

        /// <summary>
        /// Expenses Items
        /// </summary>
        ExpenseItems = 1,

        /// <summary>
        /// Claims
        /// </summary>
        Claims = 2,

        /// <summary>
        /// Expense Item Categories
        /// </summary>
        ExpenseItemCategories = 3,

        /// <summary>
        /// Cars
        /// </summary>
        Cars = 4,

        /// <summary>
        /// Companies
        /// </summary>
        Companies = 5,

        /// <summary>
        /// Cost Codes
        /// </summary>
        CostCodes = 6,

        /// <summary>
        /// Documents
        /// </summary>
        Departments = 7,

        /// <summary>
        /// Project Codes
        /// </summary>
        ProjectCodes = 8  
    }

    /// <summary>
    /// Represents the way in which calculations between addresses are done.
    /// </summary>
    public enum HomeToLocationType
    {
        /// <summary>
        /// no home to location set.
        /// </summary>
        None = 0,

        /// <summary>
        /// calculate home and office to location diff.
        /// </summary>
        CalculateHomeAndOfficeToLocationDiff = 1,

        /// <summary>
        /// flag home and office to location diff.
        /// </summary>
        FlagHomeAndOfficeToLocationDiff = 2,

        /// <summary>
        /// deduct home to office from every journey.
        /// </summary>
        DeductHomeToOfficeFromEveryJourney = 3,

        /// <summary>
        /// deduct home to office every time home is visited.
        /// </summary>
        DeductHomeToOfficeEveryTimeHomeIsVisited = 4,

        /// <summary>
        /// deduct home to office if start or finish home.
        /// </summary>
        DeductHomeToOfficeIfStartOrFinishHome = 5,

        /// <summary>
        /// deduct first and last home.
        /// </summary>
        DeductFirstAndLastHome = 6,

        /// <summary>
        /// deduct full home to office every time home is visited.
        /// </summary>
        DeductFullHomeToOfficeEveryTimeHomeIsVisited = 7,

        /// <summary>
        /// deduct full home to office if start or finish home.
        /// </summary>
        DeductFullHomeToOfficeIfStartOrFinishHome = 8
    }

    /// <summary>
    /// Represents the type of an expense item.
    /// </summary>
    public enum ExpenseItemType
    {
        /// <summary>
        /// A Cash Expense.
        /// </summary>
        Cash = 1,

        /// <summary>
        /// A Credit Card Expense.
        /// </summary>
        CreditCard = 2,

        /// <summary>
        /// A Purchase Card Expense.
        /// </summary>
        PurchaseCard = 3
    }

    /// <summary>
    /// Represents the unit of measure for a mileage expense.
    /// </summary>
    public enum MileageUom
    {
        /// <summary>
        /// Miles.
        /// </summary>
        Mile = 0,

        /// <summary>
        /// Kilometres.
        /// </summary>
        Km = 1
    }

    /// <summary>
    /// Represents the stages a claim can go through.
    /// </summary>
    public enum ClaimStage
    {
        /// <summary>
        /// Current.
        /// </summary>
        Current = 1,

        /// <summary>
        /// Submitted.
        /// </summary>
        Submitted = 2,

        /// <summary>
        /// Previous.
        /// </summary>
        Previous = 3,

        /// <summary>
        /// Any.
        /// </summary>
        Any = 4
    }

    /// <summary>
    /// Represents a type of claim.
    /// </summary>
    public enum ClaimType
    {
        /// <summary>
        /// Mixed between Cash, Credit or Purchase.
        /// </summary>
        Mixed = 0,

        /// <summary>
        /// Cash Claim.
        /// </summary>
        Cash = 1,

        /// <summary>
        /// Credit Claim.
        /// </summary>
        Credit = 2,

        /// <summary>
        /// Purchase Claim.
        /// </summary>
        Purchase = 3
    }

    /// <summary>
    /// Represents the current status of a claim.
    /// </summary>
    public enum ClaimStatus
    {
        /// <summary>
        /// No status.
        /// </summary>
        None = 0,

        /// <summary>
        /// The claim is submitted.
        /// </summary>
        Submitted = 1,

        /// <summary>
        /// The claim is being processed.
        /// </summary>
        BeingProcessed = 2,

        /// <summary>
        /// The claim is at the next stage, awaiting action.
        /// </summary>
        NextStageAwaitingAction = 3,

        /// <summary>
        /// An item in the claim is returned and awaiting input from the claimant.
        /// </summary>
        ItemReturnedAwaitingEmployee = 4,
        
        /// <summary>
        /// An item has been corrected by the claimant and is awaiting from the approver.
        /// </summary>
        ItemCorrectedAwaitingApprover = 5,
        
        /// <summary>
        /// The claim has been approved.
        /// </summary>
        ClaimApproved = 6,
        
        /// <summary>
        /// The claim has been paid.
        /// </summary>
        ClaimPaid = 7,

        /// <summary>
        /// The claim is awaiting allocation.
        /// </summary>
        AwaitingAllocation = 8
    }

    /// <summary>
    /// The result of saving or editing a claim
    /// </summary>
    public enum SaveEditClaimOutcome
    {
        /// <summary>
        /// A duplicate claim name exists for the employee
        /// </summary>
        DuplicateClaimName = 0,

        /// <summary>
        /// The action completed successfully 
        /// </summary>
        Success = 1
    }

    /// <summary>
    /// The result of a ClaimDefinition action
    /// </summary>
    public enum ClaimDefinitionOutcome
    {
        /// <summary>
        /// The account only permits one active claim at a time
        /// </summary>
        SingleClaimOnly = 0,

        /// <summary>
        /// A duplicate claim name exists for the employee
        /// </summary>
        DuplicateClaimName = 1,

        /// <summary>
        /// The action on the claim definition was a success
        /// </summary>
        Success = 2,

        /// <summary>
        /// The action on the claim definition was a failure
        /// </summary>
        Fail = 3
    }
    

    /// <summary>
    /// The result of a Claim action
    /// </summary>
    public enum ClaimBasicOutcome
    {
        /// <summary>
        /// The account only permits one active claim at a time
        /// </summary>
        SingleClaimOnly = 0,

        /// <summary>
        /// A duplicate claim name exists for the employee
        /// </summary>
        DuplicateClaimName = 1,

        /// <summary>
        /// The action on the claim definition was a success
        /// </summary>
        Success = 2,

        /// <summary>
        /// The action on the claim definition was a failure
        /// </summary>
        Fail = 3,

        /// <summary>
        /// New claimId must be zero when adding a claim
        /// </summary>
        NewClaimIdMustBeZero = 4
    }

    /// <summary>
    /// Represents the areas filter types are applicable to
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// Filter is applicable to all areas
        /// </summary>
        All = 0,

        /// <summary>
        /// Filter is applicable to all cost codes
        /// </summary>
        Costcode = 1,

        /// <summary>
        /// Filter is applicable to all departments
        /// </summary>
        Department = 2,

        /// <summary>
        /// Filter is applicable to all locations
        /// </summary>
        Location = 3,

        /// <summary>
        /// Filter is applicable to all project codes
        /// </summary>
        Projectcode = 4,

        /// <summary>
        /// Filter is applicable to all reasons
        /// </summary>
        Reason = 5,

        /// <summary>
        /// Filter is applicable to all Udfs
        /// </summary>
        Userdefined = 6
    }

    /// <summary>
    /// Represents the permission results for an expense item
    /// </summary>
    public enum ExpenseItemPermissionResult
    {
        /// <summary>
        /// Expense item has passed validation
        /// </summary>
        Pass = 0,

        /// <summary>
        /// The employee does not own the claim
        /// </summary>
        EmployeeDoesNotOwnClaim = 1,

        /// <summary>
        /// The expense item belongs to an claim that has been submitted
        /// </summary>
        ClaimHasBeenSubmitted = 2,

        /// <summary>
        /// The expense item belongs to an claim that has been approved
        /// </summary>
        ClaimHasBeenApproved = 3,

        /// <summary>
        ///Expense item has been edited after Pay Before Validate       
        /// </summary>
        ExpenseItemHasBeenEdited = 4,

        /// <summary>
        /// The no reason for amendment provided by approver.
        /// </summary>
        NoReasonForAmendmentProvidedByApprover = 5
    }

    /// <summary>
    /// The flag colour.
    /// </summary>
    public enum FlagColour
    {
        /// <summary>
        /// No Colour.
        /// </summary>
        None,

        /// <summary>
        /// For information only.
        /// </summary>
        Information = 1,

        /// <summary>
        /// Amber colour.
        /// </summary>
        Amber = 2,

        /// <summary>
        /// Red colour.
        /// </summary>
        Red = 3,
    }

    /// <summary>
    /// The flag failure reason.
    /// </summary>
    public enum FlagFailureReason
    {
        /// <summary>
        /// No failure.
        /// </summary>
        None,

        /// <summary>
        /// Flag causing the failure has its action set to block.
        /// </summary>
        Blocked,

        /// <summary>
        /// The claimant must provide a justification.
        /// </summary>
        ClaimantJustificationRequired
    }

    /// <summary>
    /// The flag action.
    /// </summary>
    public enum FlagAction
    {
        /// <summary>
        /// No action.
        /// </summary>
        None = 0,

        /// <summary>
        /// Flag the expense item.
        /// </summary>
        FlagItem = 1,

        /// <summary>
        /// Block the expense item from being added.
        /// </summary>
        BlockItem
    }

    /// <summary>
    /// Where are items being checked from
    /// </summary>
    public enum ValidationPoint
    {
        /// <summary>
        /// On the add expense page.
        /// </summary>
        AddExpense,

        /// <summary>
        /// When the claimant is submitting the claim.
        /// </summary>
        SubmitClaim
    }

    /// <summary>
    /// Whether a flag can be validated at any point or needs the expense saving to the database first.
    /// </summary>
    public enum ValidationType
    {
        /// <summary>
        /// Can be validated at any time.
        /// </summary>
        Any,

        /// <summary>
        /// Expense does not require saving before validation for flags.
        /// </summary>
        DoesNotRequireSave,

        /// <summary>
        /// Expense requires saving to database before validation for flags.
        /// </summary>
        RequiresSave
    }

    /// <summary>
    /// The check and pay action
    /// </summary>
    public enum CheckAndPayAction
    {
        /// <summary>
        /// Set the expense items to approved
        /// </summary>
        ApproveItems = 0,

        /// <summary>
        /// Set the expense item to unapproved
        /// </summary>
        UnapproveItem = 1,

        /// <summary>
        /// Set the expense items to returned
        /// </summary>
        ReturnItems = 2
    }

    /// <summary>
    /// The vehicle type.
    /// </summary>
    public enum VehicleType
    {
        /// <summary>
        /// No vehicle type set.
        /// </summary>
        [Description("None")]
        None = 0,

        /// <summary>
        /// The bicycle.
        /// </summary>
        [Description("Bicycle")]
        Bicycle = 1,

        /// <summary>
        /// The car.
        /// </summary>
        [Description("Car")]
        Car = 2,

        /// <summary>
        /// The motorcycle.
        /// </summary>
        [Description("Motorcycle")]
        Motorcycle = 3,

        /// <summary>
        /// The moped.
        /// </summary>
        [Description("Moped")]
        Moped = 4,

        /// <summary>
        /// LGV
        /// </summary>
        [Description("Light Goods Vehicle (LGV)")]
        LGV = 5,

        /// <summary>
        /// HGV
        /// </summary>
        [Description("Heavy Goods Vehicle (HGV)")]
        HGV = 6,

        /// <summary>
        /// Minibus
        /// </summary>
        [Description("Minibus")]
        Minibus = 7,

        /// <summary>
        /// Bus
        /// </summary>
        [Description("Bus")]
        Bus = 8
    }

    /// <summary>
    /// The vehicle engine type.
    /// </summary>
    public enum VehicleEngineType
    {
        /// <summary>
        /// No engine type set.
        /// </summary>
        [Description("None")]
        None = 0,

        /// <summary>
        /// Petrol engine.
        /// </summary>
        [Description("Petrol")]
        Petrol = 1,

        /// <summary>
        /// Diesel engine.
        /// </summary>
        [Description("Diesel")]
        Diesel = 2,

        /// <summary>
        /// LPG engine.
        /// </summary>
        [Description("LPG")]
        LPG = 3,

        /// <summary>
        /// Hybrid engine.
        /// </summary>
        [Description("Hybrid")]
        Hybrid = 4,

        /// <summary>
        /// Electric engine.
        /// </summary>
        [Description("Electric")]
        Electric = 5,

        /// <summary>
        /// Diesel euro v engine.
        /// </summary>
        [Description("Diesel Euro V")]
        DieselEuroV = 6,

        /// <summary>
        /// Bi-Fuel engine.
        /// </summary>
        [Description("Bi-Fuel")]
        BiFuel = 7,

        /// <summary>
        /// Conversion engine.
        /// </summary>
        [Description("Conversion")]
        Conversion = 8,

        /// <summary>
        /// E85 engine.
        /// </summary>
        [Description("E85")]
        E85 = 9,

        /// <summary>
        /// Hybrid Electric engine.
        /// </summary
        [Description("Hybrid Electric")]
        HybridElectric = 10,

        /// <summary>
        /// All Other engines.
        /// </summary>
        [Description("Other")]
        Other = 99
    }

    /// <summary>
    /// Field Types relevant to UDFs
    /// </summary>
    public enum TypeOfUserDefinedField
    {  
        /// <summary>
        /// The text field type
        /// </summary>
        Text = 1,
        /// <summary>
        /// The integer field type
        /// </summary>
        Integer = 2,
        /// <summary>
        /// The datetime field type
        /// </summary>
        DateTime = 3,
        /// <summary>
        /// The list field type
        /// </summary>
        List = 4,
        /// <summary>
        /// The tickbox field type
        /// </summary>
        TickBox = 5,
        /// <summary>
        /// The currency field type
        /// </summary>
        Currency = 6,
        /// <summary>
        /// The number field type
        /// </summary>
        Number = 7,
        /// <summary>
        /// The hyperlink field type
        /// </summary>
        Hyperlink = 8,
        /// <summary>
        /// The relationship field type
        /// </summary>
        Relationship = 9,
        /// <summary>
        /// The large text field type
        /// </summary>
        LargeText = 10,
           
    }

    /// <summary>
    /// The format of the user defined field
    /// </summary>
    public enum FieldFormat
    {
        /// <summary>
        /// No field format set
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// Field is single line
        /// </summary>
        SingleLine = 1,

        /// <summary>
        /// Field is multiline
        /// </summary>
        MultiLine = 2,

        /// <summary>
        /// Field is datetime
        /// </summary>
        DateTime = 3,

        /// <summary>
        /// Field is date only
        /// </summary>
        DateOnly = 4,

        /// <summary>
        /// /Field is time only
        /// </summary>
        TimeOnly = 5,

        /// <summary>
        /// Field is formatted
        /// </summary>
        FormattedText = 6 
    }

    /// <summary>
    /// The outcome of an action relating to Organisations
    /// </summary>
    public enum OrganisationActionOutcome
    {
        /// <summary>
        /// Success
        /// </summary>
        Success = 0,

        /// <summary>
        /// An unexpected error was thrown
        /// </summary>
        UnexpectedError = 1,

        /// <summary>
        /// The company name already exists
        /// </summary>
        OrganisationNameAlreadyExists =2
    }
}

