using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Models.Responses
{
    using System.Runtime.Serialization;
    using Common;

    using SpendManagementApi.Interfaces;

    using Spend_Management.shared.code.AccessRoleElementPermission.Interfaces;

    /// <summary>
    /// LoginResponse encapsulates the data the consumer needs to make subsequent requests to the API.
    /// </summary>
    [KnownType(typeof(MobileLoginResponse))]
    [KnownType(typeof(LoginResponse))]
    [KnownType(typeof(ApiResponse))]
    public class LoginResponse : ApiResponse
    {
        /// <summary>
        /// An authentication token that must be present in the header of every request.
        /// Use the string "AuthToken" when adding this token to the request header.
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// Can the user access the check and pay page?
        /// </summary>
        public bool CanAccessCheckAndPay { get; set; }

        /// <summary>
        /// The number of claims the employee has awating approval
        /// </summary>
        public int NumberOfClaimsAwaitingApproval { get; set; }

        /// <summary>
        /// Does the account permit multiple step journeys?
        /// </summary>
        public bool AllowMultipleStepJourneys { get; set; }

        /// <summary>
        /// Is the postcode mandatory when manipulating addresses?
        /// </summary>
        public bool MandatoryPostcodeForAddresses{ get; set; }

        /// <summary>
        /// Does the Employee has any active journeys?
        /// </summary>
        public bool HasActiveJourneys { get; set; }

        /// <summary>
        /// Is electronic declaration switched on for the account?
        /// </summary>
        public bool HasElectronicDeclaration { get; set; }

        /// <summary>
        /// The message to display for the approver declaration
        /// </summary>
        public string ApproveDeclarationMessage { get; set; }

        /// <summary>
        /// The message to display for the claimant declaration when submitting a claim
        /// </summary>
        public string ClaimantDeclarationMessage { get; set; }

        /// <summary>
        /// Whether the account only allows for one current claim at a time.
        /// </summary>
        public bool SingleClaimOnly { get; set; }

        /// <summary>
        /// Whether the claimant can upload receipts against an expense item
        /// </summary>
        public bool AllowReceiptsForExpenseItems { get; set; }

        /// <summary>
        /// Whether the employee has credit cards assigned
        /// </summary>
        public bool HasCreditCard { get; set; }

        /// <summary>
        /// Whether the employee has purchase cards assigned
        /// </summary>
        public bool HasPurchaseCard { get; set; }

        /// <summary>
        /// Gets or sets whether the employee's access roles requires the employee to have a bank account to record an expense
        /// </summary>
        public bool BankAccountRequiredForExpense { get; set; }

        /// <summary>
        /// Gets or sets the element access permissions the user has for bank accounts.
        /// </summary>
        public IAccessRoleElementPermissions BankAccountElementAccessPermissions { get; set; }

        /// <summary>
        /// Gets or sets whether a claimant can add a manual address
        /// </summary>
        public bool AllowClaimantsToAddManualAddresses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is address name mandatory.
        /// </summary>
        public bool IsAddressNameMandatory { get; set; }

        /// <summary>
        /// Gets or sets whether the receipt service is enabled.
        /// </summary>
        public bool ReceiptServiceEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the validation service is enabled.
        /// </summary>
        public bool ValidationServiceEnabled { get; set; }

        /// <summary>
        /// Whether to exclude expired vehicle when adding an expense
        /// </summary>
        public bool WhetherExcludeExpiredVehicles { get; set; }

        /// <summary>
        /// Gets or sets where home to office is enabled
        /// </summary>
        public bool IsHomeToOfficeEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether user may edit details
        /// </summary>
        public bool UserMayEditDetails { get; set; }

        /// <summary>
        /// Costcode breakdown settings
        /// </summary>
        public CostcodeBreakdownSettings CostcodeBreakdownSettings { get; set; }

        /// <summary>
        /// Gets or sets a whether the account permits employees to notify the admin of changes to their details
        /// </summary>
        public bool CanNotifyAdminOfChanges { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current user is an NHS customer
        /// </summary>
        public bool IsNHSCustomer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether claimant may add vehicles.
        /// </summary>
        public bool UserMayAddVehicles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether claimant may select vehicle rates when adding a vehicle
        /// </summary>
        public bool UserMaySelectVehicleRates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user can specify vehicle start date when adding a vehicle
        /// </summary>
        public bool UserCanSpecifyVehicleStartDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether start date is mandatory when adding a vehicle
        /// </summary>
        public bool StartDateMandatoryWhenAddingVehicle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the vehicle is activated when an employee adds their own vehicle
        /// </summary>
        public bool ActivateCarOnUserAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user can see full home address or not
        /// </summary>
        public bool ShowFullHomeAddressOnClaims { get; set; }

        /// <summary>
        /// Gets or sets the home address keyword
        /// </summary>
        public string HomeAddressKeyword { get; set; }

        /// <summary>
        /// Gets or sets the active vehicle count
        /// </summary>
        public int ActiveVehicleCount { get; set; }

        /// <summary>
        /// Gets or sets the post code anywhere key for the account.
        /// </summary>
        public string PostCodeAnywhereKey { get; set; }

        /// <summary>
        /// Gets or sets whether the Duty of Care checks are based on the expense date.
        /// </summary>
        public bool UseDateOfExpenseForDutyOfCareChecks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has the ability to see holidays.
        /// </summary>
        public bool ShowHolidays { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an ESR assignment must be associated with an expense item.
        /// </summary>
        public bool EsrAssignmentRequriedForExpense { get; set; }

        /// <summary>
        /// Gets or sets whether employee has access to My Advances
        /// </summary>
        public bool ShowMyAdvances { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Maps are enabled for the account.
        /// </summary>
        public bool IsMapsEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the employee has access to Advances
        /// </summary>
        public bool ShowAdvances { get; set; }

        /// <summary>
        /// Gets or sets the custom message that needs to be displayed when an account is locked
        /// </summary>
        public string AccountLockedMessage { get; set; }

        /// <summary>
        /// Gets or sets the custom message that the user exceeds login attempts
        /// </summary>
        public string AccountCurrentlyLockedMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the app can ask the employee for app reviews.
        /// </summary>
        public bool CanAskForReviews { get; set; }

        /// <summary>
        /// Gets or sets the current employee Id.
        /// </summary>
        public int EmployeeId { get; set; }
    }
}