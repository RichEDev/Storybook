namespace SpendManagementApi.Utilities
{
    // Todo: This is only here until we finalise a resources / i18n soltion.

    public class ApiResources
    {
        public const string HttpStatusCodeUnauthorised = "The credentials you have supplied are invalid";
        public const string HttpStatusCodeUnauthorisedArchived = "The account you are trying to access has been archived.";
        public const string HttpStatusCodeForbidden = "You are not allowed access to this. Please ensure you have API access, and sufficient access roles.";
        public const string HttpStatusCodeForbiddenLockedPassword = "Your account has currently been locked or your password has expired. Please contact your administrator.";
        public const string HttpStatusCodeForbiddenInvalidIP = "Your IP Address is not allowed access, according to the IP filters. ";
        public const string HttpStatusCodeForbiddenSubAccount = "Your default sub account is invalid.";
        public const string HttpStatusCodeForbiddenInactiveAccount = "Your account is inactive. Please contact support.";
        public const string HttpStatusCodeNotImplemented = "This resource is inaccessible or incomplete.";
        
        public const string ResponseForbiddenNoAccountId = "The account Id is missing from the request header.";
        public const string ResponseForbiddenNoAuthToken = "The authentication token is missing from the request header.";
        public const string ResponseForbiddenOldAuthToken = "The authentication token is is out of date. Please log in.";
        public const string ResponseForbiddenAdmin = "Admin only. This call can only be made from the local server.";


        public const string ApiErrorGeneralError = "General Error";
        public const string ApiErrorSaveUnsuccessful = "Save Unsuccessful";
        public const string ApiErrorSaveUnsuccessfulMessage = "Data not saved successfully";
        public const string ApiErrorUpdateUnsuccessfulMessage = "Update unsuccessful";
        public const string ApiErrorXUnsuccessfulMessage = "{0} unsuccessful";
        public const string ApiErrorSaveUnsuccessfulDepartmentLabelMessage = "Department descriptions are in use for this account. Therefore, department label must be changed in order for this to be a successful edit.";
        public const string ApiErrorSaveUnsuccessfulDepartmentDescriptionMessage = "Department descriptions are in use for this account. Please either change the description slightly, or delete and recreate a department.";
        public const string ApiErrorDeleteUnsuccessful = "Record not deleted.";
        public const string ApiErrorDeleteUnsuccessfulMessage = "Record not deleted. This may be because it has dependencies in other areas";
        public const string ApiErrorDeleteUnsuccessfulMessageAccessRoleEmployees = ": One or more Employees have this AccessRole assigned to them.";
        public const string ApiErrorDeleteUnsuccessfulMessageUserDefinedClaimReasons = ": One or more User Defined Fields have links to this Claim Reason.";
        public const string ApiErrorDeleteUnsuccessfulMessageEmployee = ": One or more Employees use this Cost Code.";        
        public const string ApiErrorDeleteUnsuccessfulInsufficientPermission = "You do not have permission to delete this record.";

        public const string ApiErrorRecordAlreadyExists = "Record Already Exists";
        public const string ApiErrorRecordAlreadyExistsMessage = "The Id must be set to 0 when adding a new object.";
        public const string ApiErrorRecordDoesntExist = "Record Doesn't Exist";
        public const string ApiErrorRecordDoesntExistMessage = "The record with the provided Id doesn't exist.";
        public const string ApiErrorUpdateObjectWithWrongId = "Wrong Id";
        public const string ApiErrorUpdateObjectWithWrongIdMessage = "The Id is wrong. Are you attempting to update a record with an Id of zero?";
        public const string ApiErrorArchiveFailed = "Archived failed.";

        public const string ApiErrorInvalidName = "Invalid Name";
        public const string ApiErrorInvalidNameMessage = "No name provided.";
        public const string ApiErrorInvalidNameAlreadyExists = "An object already exists with this Label.";
        public const string ApiErrorInvalidDescriptionAlreadyExists = "An object already exists with this Description.";
        public const string ApiErrorOptionalEmailWrong = "The email address you supplied is in an incorrect format.";
        
        
        public const string ApiErrorInvalidLinkedToExpenseItem = "This cost code cannot be deleted because it is in use by an expense item.";
        public const string ApiErrorCategoryInvalidLinkedToSubcat = "This category cannot be deleted because it is in use by an expense subcategory.";
        public const string ApiErrorInvalidLinkedToEmployee = "This cost code cannot be deleted because it is in use by an employee.";
        public const string ApiErrorInvalidLinkedToSignoff = "This cost code cannot be deleted because it is in use by a sign-off group.";


        public const string ApiErrorMissingCritera = "You must provide at least one criterion to search with.";
        public const string ApiErrorAllowanceRateAdditionFailure = "Addition of an AllowanceRate failed.";
        public const string ApiErrorAllowanceRateDeletionFailure = "Deletion of an AllowanceRate failed.";
        public const string ApiErrorWrongTeamLeaderIdMessage = "An employee with an Id matching the TeamLeaderId could not be found.";


        public const string AuditMessageTotalRemainingIsZero = "Your account has run out of Api Credits. Please contact your administrator to get more.";
        public const string AuditMessageThresholdLimitNotEnough = "You do not have enough Free or Licensed calls combined to complete this call. Please wait until your free daily calls reset or contact us to get more.";
        public const string AuditMessageThresholdLimitHitHour = "You have exceeded the API call limit for calls this hour ";
        public const string AuditMessageThresholdLimitHitMinute = "You have exceeded the API call limit for calls this minute ";
        public const string HttpStatusCodeForbiddenLicense = "There are no thresholds set up for this account.";


        public const string ApiErrorGender = "Gender must be set to either Male or Female";
        public const string ValidationError = "There is a problem with the request";
        public const string ErrorMaxLength = "Too many characters, must not exceed: ";
        public const string ApiErrorSpendManagementElementEnum = "The value of this must be either: ";
        public const string ApiErrorEnum = "This enum value is invalid.";

        public const string ApiErrorANonExistentX = "A {0} doesn't exist with this Id: ";
        public const string ApiErrorAnNonExistentX = "An {0} doesn't exist with this Id: ";

        #region GeneralOption
        public const string ApiErrorGeneralOptionDoesntExist = "GeneralOption doesn't exist";
        public const string ApiErrorGeneralOptionDoesntExistMessageKey = "The GeneralOption with the provided {0} doesn't exist.";
        public const string ApiErrorGeneralOptionDoesntExistMessageKeySubAccount = "The GeneralOption with the provided key and subAccountId doesn't exist.";
        #endregion

        #region ExpenseItem

        public const string ApiErrorCreditCardReconcile = "The item cannot be added to this claim as credit card items from the same statement have already been reconciled on other claims.";
        public const string ApiErrorPurchaseCardReconcile = "The item cannot be added to this claim as purchase card items from the same statement have already been reconciled on other claims.";
        public const string ApiErrorVehicleDates = "The mileage item cannot be added as the date of the expense item is not between the range of the car start date and the car end date.";
        public const string ApiErrorMileage = "The mileage item cannot be added as your recommended mileage has been exceeded.";
        public const string ApiErrorAddressMatch = "The mileage item cannot be added as you have some addresses that could not be matched automatically and require correcting.";
        public const string ApiErrorVATGreaterThanTotal = "The VAT amount cannot be greater than the total.";
        public const string ApiErrorEpenseItemApprovalPermission = "You do not have the correct access roles to approve this expense item.";
        public const string ApiErrorEpenseItemEmployeeDoesNotOwnClaim = "You cannot add/edit an Expense Item to a Claim you do not own.";
        public const string ApiErrorCreateExpenseItemMessage = "An error occured while creating the {0}.";
        public const string ApiErrorEditExpenseItemMessage = "An error occured while editing the {0}.";
        public const string ApiErrorExpenseItemClaimSubmitted = "You cannot add/edit an Expense Item to a Claim that is currently submitted.";
        public const string ApiErrorExpenseItemClaimWithApprover = "You cannot add/edit an Expense Item to a Claim that is with the approver.";
        public const string ApiErrorExpenseItemEdited = "You cannot edit the Expense Item when it has been edited after Pay Before Validate.";
        public const string ApiErrorExpenseItemClaimApproved = "You do not have permission to edit the Expense Item when it has been approved.";
        public const string ApiErrorExpenseItemViewExpenseItem = "You do not have permission to view the Expense Item.";
        public const string ApiErrorExpenseItemViewCostCodeBreakdown = "Unable to retrieve the Cost Code Brekadown as you do not have permission to view the releated Expense Item.";
        public const string ApiErrorExpenseItemViewJorneySteps = "Unable to retrieve the Jorney Steps as you do not have permission to view the releated Expense Item.";
        public const string ApiErrorExpenseItemUnapprovalApproved = "Unable to unapprove Expense Item as the Expense Item has not been Approved.";
        public const string ApiErrorExpenseItemUnapprovalReturned = "Unable to unapprove Expense Item as the Expense Item has been returned.";
        public const string ApiErrorExpenseItemUnapprovalClaimNotSubmitted = "Unable to unapprove Expense Item as the associated Claim has not been Submitted.";
        public const string ApiErrorExpenseItemUnapprovalClaimApproved = "Unable to unapprove Expense Item as the associated Claim has been Approved.";
        public const string ApiErrorExpenseItemUnapprovalClaimNotAtSubmittedStage = "Unable to unapprove Expense Item as the associated Claim is not at the Submitted Stage.";
        public const string ApiErrorExpenseItemUnapprovalCannotSelfApprove = "Unable to unapprove Expense Item as you do not have permission to unapprove your own Expense Items.";
        public const string ApiErrorExpenseItemDoesNotBelongToClaim = "An Expense Item does not belong to the ClaimId provided.";
        public const string ApiErrorExpenseItemAlreadyApproved = "Expense Item has already been Approved.";
        public const string ApiErrorExpenseItemAlreadyReturned = "The Expense Item has been returned.";
        public const string ApiErrorExpenseItemNoFlagsFound = "No Flags are associated with the Expense Item";
        public const string APIErrorValidationPointNotFound = "Validation Point not found.";
        public const string APIErrorValidationTypeNotFound = "Validation Type not found.";
        public const string ApiErrorDeleteExpenseItemMessage = "An error occured while deleting the {0}.";
        public const string ApiErrorExpenseItemCannotDeleteExpenseItem = "{0} cannot be deleted as its associated claim has been submitted or paid.";
        public const string ApiErrorExpenseItemCannotDisputeExpenseItemAlreadyPaid = "{0} cannot be disputed as its associated claim has been paid.";
        public const string ApiErrorExpenseItemCannotDisputeExpenseItemNotReturned = "{0} cannot be disputed as it has not been returned by the approver.";
        public const string ApiErrorGetClaimantExpenseDataUnSuccessful = "Get claimant expense item data unsuccessfull";
        public const string ApiErrorNoPermissionExpenseData = "You do not have permission to view the requested expense item data";
        public const string ApiErrorExpenseItemNoReasonForAmendmentProvidedByApprover = "The approver must provide a reason for amendment when editing a claimant's Expense Item";
        public const string ApiErrorNotExpenseItemOwner = "You do not have permission to perform this action aginast the expense item.";

        #endregion

        #region Currencies

        public const string ApiErrorWrongCurrencyId = "Invalid CurrencyId.";
        public const string ApiErrorWrongGlobalCurrencyId = "Invalid GlobalCurrencyId.";
        public const string ApiErrorWrongCurrencyIdMessage = "The CurrencyId could not be found or resulted in the wrong currency.";
        public const string ApiErrorWrongGlobalCurrencyIdMessage = "The GlobalCurrencyId could not be found or resulted in the wrong GlobalCurrency.";
        public const string ApiErrorCurrencyArchiveFailedPrimary = "The selected currency cannot be archived as it is currently set as the primary currency for your company.";
        public const string ApiErrorCurrencyArchiveFailedExpenseClaim = "The selected currency cannot be archived as it has been used on one or more expense claims.";
        public const string ApiErrorCurrencyArchiveFailedContracts = "The selected currency cannot be archived as it has been used on one or more contracts.";
        public const string ApiErrorCurrencyArchiveFailedSuppliers = "The selected currency cannot be archived as it has been used on one or more supplier records.";
        public const string ApiErrorCurrencyArchiveFailedProducts = "The selected currency cannot be archived as it has been used on one or more contract product records.";
        public const string ApiErrorCurrencyArchiveFailedBankAccounts = "The selected currency cannot be archived as it has been used on one or more bank accounts.";
        public const string ApiErrorCurrencyAlreadyExistsForGlobal = "A Currency already exists to match the GlobalCurrency with the supplied Id.";
        public const string ApiErrorCurrencyExchangeRatesDeleteUnsuccessful = "Delete Exchange Rate Unsuccessful";
        public const string ApiErrorCurrencyDeleteUnsuccessful = "Specified currency is in use as a base currency";
        public const string ApiErrorCurrencyErrorMessage1 = "Specified currency is in use as a base currency";
        public const string ApiErrorCurrencyErrorMessage2 = "Specified currency is associated with a savedexpense or a claim";
        public const string ApiErrorCurrencyErrorMessage3 = "Specified currency is associated with a contract";
        public const string ApiErrorCurrencyErrorMessage4 = "Specified currency is associated with a supplier";
        public const string ApiErrorCurrencyErrorMessage5 = "Specified currency is associated with a contract product";
        public const string ApiErrorCurrencyErrorMessage6 = "Specified currency is associated with a custom entity";
        public const string ApiErrorCurrencyErrorMessage9 = "Specified currency is associated with one or more bank accounts";
        public const string ApiErrorCurrencyExchangeRateDeleteMessage1 = "Specified currency month could not be located";

        #endregion Currencies

        #region Countries

        public const string ApiErrorWrongCountryId = "Invalid CountryId.";
        public const string ApiErrorWrongGlobalCountryId = "Invalid GlobalCountryId.";
        public const string ApiErrorWrongCountryIdMessage = "The CountryId could not be found or resulted in the wrong Country.";
        public const string ApiErrorWrongGlobalCountryIdMessage = "The GlobalCountryId could not be found or resulted in the wrong GlobalCountry.";
        public const string ApiErrorCountryArchiveFailedPrimary = "The selected Country cannot be archived as it is currently set as the primary Country for your company.";
        public const string ApiErrorCountryAlreadyExistsForGlobal = "A Country already exists to match the GlobalCountry with the supplied Id.";

        #endregion Countries

        #region Mileage Categories

        public const string MileageCategoriesNullRequest = "Missing MileageCategory request";
        public const string MileageCategoriesNullRequestMessage = "The request for MileageCategory cannot be Null";
        public const string MileageCategoriesDateRangeNullRequest = "Missing DateRanges request";
        public const string MileageCategoriesDateRangeNullRequestMessage = "The request for DateRanges cannot be Null";
        public const string MileageCategoriesDateRangeThresholdNullRequest = "Missing Thresholds request";
        public const string MileageCategoriesDateRangeThresholdNullRequestMessage = "The request for Thresholds cannot be Null";
        public const string MileageCategoriesThresholdFuelRateNullRequest = "Missing FuelRates request";
        public const string MileageCategoriesThresholdFuelRateNullRequestMessage = "The request for FuelRates cannot be Null";
        public const string MileageCategories_InvalidCarSize = "Invalid Car Size";
        public const string MileageCategories_InvalidCarSizeMessage = "A mileage category with specified car size already exists";
        public const string MileageCategories_InvalidNhsMileageCode = "Invalid Nhs Mileage Code";
        public const string MileageCategories_InvalidNhsMileageCodeMessage = "Please provide a valid nhs mileage code";
        public const string MileageCategories_InvalidDateRange = "Invalid Date Range";
        public const string MileageCategories_InvalidDateRangeMessage = "You cannot add a date range of type 'Any' if other date ranges already exist for this category.";
        public const string MileageCategories_DateRangeNotSaved = "Date Range could not be saved.";
        public const string MileageCategories_InvalidDateRangeCountMessage = "Please provide only one date range";
        public const string MileageCategories_InvalidThresholdRange = "Invalid Threshold Range";
        public const string MileageCategories_InvalidThresholdRangeMessage = "You cannot add a threshold of type 'Any' if other thresholds already exist for this date range.";

        public const string MileageCategoriesDateRangeIdMismatch = "DateRangeId/MileageCategoryId mismatch ";
        public const string MileageCategoriesDateRangeIdMismatchMessage = "The DateRangeId supplied is not related to the MileageCategoryId supplied";
        public const string MileageCategoriesThresholdIdMismatch = "ThresholdIdId/MileageCategoryId mismatch ";
        public const string MileageCategoriesThresholdIdMismatchMessage = "The ThresholdId supplied is not related to the MileageCategoryId supplied";
        public const string MileageCategoriesFuelRateThresholdIdMismatch = "ThresholdId/FuelRate mismatch ";
        public const string MileageCategoriesFuelRateThresholdIdMismatchMessage = "The ThresholdId supplied is not related to the FuelRate supplied";
        public const string MileageCategoriesDateRangeThresholdIdMismatch =  "ThresholdIdId/DateRangeId mismatch ";
        public const string MileageCategoriesDateRangeThresholdIdMismatchMessage = "The ThresholdId supplied is not related to the DateRangeId supplied";
        public const string MileageCategoriesDateRangeNotFound = "Date Range not found ";
        public const string MileageCategoriesDateRangeNotFoundMessage = "The Date Range could not be found using the supplied MileageCategoryId";
        public const string MileageCategoriesWithLabelAlreadyExists = "Mileage Category with label already exists";
        public const string MileageCategoriesWithLabelAlreadyExistsMessage = "You cannot update the label Mileage Category as a Mileage Category with this label already exists";
        public const string MileageCategoriesDateValidationFailed = "Date validation failed";
        public const string MileageCategoriesDateValue1NullMessage = "DateValue1 must be supplied for this type of Date Range Type";     
        public const string MileageCategoriesDateValue1And2NullMessage = "DateValue1 and DateValue2 must be supplied for this type of Date Range Type";
        public const string MileageCategoriesDateValue2GreaterThanDate1Message = "DateValue2 cannot be greater than DateValue1";
        public const string MileageCategoriesRangeValue1ValidationFailed = "Range validation failed";
        public const string MileageCategoriesRangeValue1ValidationFailedMessage = "RangeValue1 must be supplied for this type of Range Type";
        public const string MileageCategoriesRangeValue1And2NullMessage = "RangeValue1 and RangeValue2 must be supplied for a Betweem Range Type";
        public const string MileageCategoriesDateRangeAddPermissionError = "You do not have the correct access roles to add a Date Range";
        public const string MileageCategoriesDateRangeEditPermissionError = "You do not have the correct access roles to edit a Date Range";
        public const string MileageCategoriesThresholdAddPermissionError = "You do not have the correct access roles to add a Threshold";
        public const string MileageCategoriesThresholdEditPermissionError = "You do not have the correct access roles to edit a Threshold";

        #endregion 

        #region Item Roles

        public const string ItemRoles_InvalidRoleName = "Invalid Role Name";
        public const string ItemRoles_InvalidRoleNameMessage = "A role with name specified already exists";
        
        public const string ItemRoles_InvalidRoleDescription = "Invalid Role Description";
        public const string ItemRoles_InvalidRoleDecriptionMessage = "No role description provided";

        public const string ItemRoles_MissingRoleSubCats = "Missing Expense Sub Category Data";
        public const string ItemRoles_MissingRoleSubCatsMessage = "No expense sub categories present in request";

        public const string ItemRoles_InvalidRoleSubCats = "Invalid Expense Sub Category Data";
        public const string ItemRoles_InvalidRoleSubCatsMessage = "Invalid expense sub categories present in request";

        public const string ItemRoles_InvalidItemRoleId = "Invalid Item Role id";
        public const string ItemRoles_InvalidItemRoleIdMessage = "You must provide a valid Item Role Id.";

        public const string ItemRole_ErrorUpdatingItemRolesForSubcats = "An error has occured while assoicating Sub Categories with the Item Role";
     
        #endregion

        #region Employees

        public const string ApiErrorWrongEmployeeId = "Invalid Employee Id";
        public const string ApiErrorWrongEmployeeIdMessage = "The EmployeeId could not be found or resulted in the wrong employee.";
        public const string ApiErrorEmployeeIdDoesNotBelongToCurrentUser = "The EmployeeId does not match that of the current user.";
        public const string ApiErrorAccountDoesNotPermitEditOfMyDetails = "The current account does not permit employees to update their details";
        public const string ApiErrorExistingUsername = "The username specified already exists";
        public const string ApiErrorNonExistentEmployeeCar = "An employee owned car doesn't exist with this Id: ";
        public const string ApiErrorNonExistentPoolCar = "A Pool car doesn't exist with this Id: ";
        public const string ApiErrorNonExistentCorporateCard = "A Corporate Card doesn't exist with this Id: ";
        public const string ApiErrorNonExistentMobileDevice = "A Mobile Device doesn't exist with this Id: ";
        public const string ApiErrorCostCentreNonExistentDepartment = "A Department doesn't exist with this Id: ";
        public const string ApiErrorCostCentreArchivedDepartment = "The Department with the following Id is Archived: ";
        public const string ApiErrorCostCentreNonExistentCostCode = "A CostCode doesn't exist with this Id: ";
        public const string ApiErrorCostCentreArchivedCostCode = "The CostCode with the following Id is Archived: ";
        public const string ApiErrorCostCentreNonExistentProjectCode = "A ProjectCode doesn't exist with this Id: ";
        public const string ApiErrorCostCentreArchivedProjectCode = "The ProjectCode with the following Id is Archived: ";
        public const string ApiErrorCostCentrePercentageAllocated = "Percentage needs to be allocated to either a department id, cost centre id or project code id";
        public const string ApiErrorCostCentrePercentageOutOfBounds = "Percentage specified must be between 1 and 100";
        public const string ApiErrorCostCentrePercentageMustAddUp = "Percentages must total 100";
        public const string ApiErrorEmployeeIsArchived = "The following employee is archived or inactive: ";
        public const string ApiErrorEmployeeLineManager = "The LineManagerId supplied must be result in a valid employee.";
        public const string ApiErrorEmployeePrimaryCountry = "The PrimaryCountryId supplied must be result in a valid country.";
        public const string ApiErrorEmployeePrimaryCurrency = "The PrimaryCurrencyId supplied must be result in a valid currency.";
        public const string ApiErrorEmployeeHireDateAfterMileageDate = "Start mileage date must be later than the employee's hire date.";
        public const string ApiErrorEmployeeTerminationBeforeHire = "Termination date cannot be before hire date.";
        public const string ApiErrorNhsTrustNonExistent = "The NHS Trust for the supplied id doens't exist: ";
        public const string ApiErrorNhsTrustArchived = "The NHS Trust is archived.";
        public const string ApiErrorNhsTrustExists = "An NHS Trust with this Id already exists."; 
        public const string ApiErrorNhsTrustVPDExists = "An NHS Trust by this VPD already exists.";
        public const string ApiErrorBankAccountHolder = "The holder name for the bank account must be provided.";
        public const string ApiErrorBankAccountNumber = "The account number for the bank account must be provided.";
        public const string ApiErrorBankAccountSortCode = "The sort code for the bank account must be provided.";
        public const string ApiErrorLocaleInvalid = "The Locale with the provided Id doesn't exist.";
        public const string ApiErrorLocaleInactive = "The Locale with the provided Id is Inactive, meaning it cannot be used on an employee.";
        public const string ApiErrorResetPasswordEmployeeInactive = "The action could not be completed. The employee is not currently active.";
        public const string ApiErrorResetPasswordEmployeeArchived = "The action could not be completed. The employee is not currently archived.";
        
        public const string ApiErrorInEmployeeUpdateMessage = "An error occured while updating the Employee details.";

        #endregion

        #region Vehicles

        public const string Vehicles_MakeOfTheCarMustBeProvided = "Make of the car must be provided";
        public const string Vehicles_ModelOfTheCarMustBeProvided = "Model of the car must be provided";
        public const string Vehicles_RegistrationNumberOfTheCarMustBeProvided = "Registration number of the car must be provided";
        public const string Vehicles_UnitOfMeasureProvidedIsNotValid = "Unit of Measure provided is not valid";
        public const string Vehicles_FuelTypeProvidedIsNotValid = "Fuel type provided is not valid";
        public const string Vehicles_MileageCategoryDoesNotExist = "No mileage category exists for the mileage category id provided";
        public const string Vehicles_MileageCategoryInvalid = "A MileageCategory provided is not valid";
        public const string Vehicles_MileageCategoryInvalidFinancialYearId = "MileageCategoryIds provided must all be associated with the financial year provided";
        public const string Vehicles_MileageCategoryInvalidUOM = "MileageCategoryIds provided must all be associated with the unit of measure provided";
        public const string Vehicles_NewReadingMustBeGreaterThanOldReading = "New reading must be greater than old reading";
        public const string Vehicles_CannotBeBothPoolCarAndOwned = "A vehicle cannot be both a pool car and owned solely by an employee.";
        public const string Vehicles_CannotUnlinkAnOwnedCar = "You cannot unlink a car that is owned by an employee.";
        public const string Vehicles_OdometerReadingInvalidCarId = "A vehicle with this Id doesn't exist.";
        public const string Vehicles_OdometerReadingsRequired = "OdometerReadings must be poplulated if OdometerReadingsRequired is set to true.";
        public const string ApiErrorOdometerReading = " - (OdometerReading ?)";
        public const string ApiErrorDutyOfCareCheckedBy = " - (CheckedBy ?)";
        public const string Vehicles_ExpiryDateInvalid = "Expiry date is not valid";
        public const string Vehicles_IncorrectEmployeeId = "An employee does not exist for the specified id";
        public const string APIErrorAccountDoesNotPermitEmployeesToSpecifyStartDateForVehicle = "The account does not allow employees to specify a start date when adding their own vehicle";
        public const string APIErrorAccountDoesNotPermitEmployeesVehicles = "The account does not permit employees to add their own vehicles";
        public const string APIErrorAccountRequiresStartDateToBeSpecifiedForAVehicle = "The account requires that a start date is specified when employees add their own vehicle";
        public const string APIErrorAccountDoesNotPermitEmployeesToSetMileageCategories = "The account does not permit employees to specify mileage categories when adding their own vehicle";

        #endregion Vehicles

        #region ExpenseCategory

        public const string ApiErrorInvalidExpenseCategory = "Invalid Parent Category";
        public const string ApiErrorInvalidExpenseCategoryMessage = "No parent category exists with specified category id";
        public const string ApiErrorWrongSubCatId = "Invalid sub category Id";
        public const string ApiErrorInvalidExpenseSubCategoryMessage = "No sub category exists with the specified sub category Id  ";

        #endregion

        #region ExpenseSubCat

        public const string APIErrorNoMileageSubCategoriesForAccount = "No Expense Sub Categories for Mileage";
        public const string APIErrorNoMileageSubCategoriesForAccountMessage = "There are no mileage related Expense Item Sub Categories for the account";

        #endregion
        
        #region User Defined Functions

        public const string InvalidUdf = "Invalid User Defined Field Id";
        public const string InvalidUdfMessage = "One or more of the user defined fields specified does not apply to this element";
        public const string MandatoryUdfMissingMessage = "One or more of the mandatory user defined fields required has not been provided";
        public const string InvalidUdfListItemMessage = "Invalid List Item Id provided for one or more of the specified User Defined Fields";
        public const string ArchivedUdfListItemMessage = "List Item Id provided for one or more of the specified User Defined Fields has been archived";
        public const string InvalidIntValueMessage = "One or more of the user defined values provided requires an integer value";
        public const string InvalidNumberValueMessage = "One or more of the user defined values provided requires a decimal value";
        public const string InvalidCurrencyValueMessage = "One or more of the user defined values provided requires a currency value in decimal";
        public const string InvalidDateOnlyValueMessage = "One or more of the user defined values provided requires a date only value in the format yyyy-MM-dd";
        public const string InvalidTimeOnlyValueMessage = "One or more of the user defined values provided requires a time only value in the format HH:mm:ss";
        public const string InvalidDateTimeValueMessage = "One or more of the user defined values provided requires a date time value in the format yyyy-MM-dd HH:mm:ss";
        public const string InvalidListItemIdValueMessage = "One or more of the user defined values provided requires a valid list item id";
        public const string InvalidHyperlinkValueMessage = "One or more of the user defined values provided requires a valid hyperlink";
        public const string InvalidRelationshipValueMessage = "One or more of the user defined values provided requires a valid relationship";
        public const string InvalidTickboxValueMessage = "One or more of the user defined values provided requires a boolean value";
        public const string InvalidNullValueForMandatory = "Value cannot be Null for a mandatory user defined field";
        public const string UdfLabelAlreadyExists = "The Display Name set for the UDF already exists";

        #endregion

        #region Corporate Cards and Providers

        public const string ApiErrorValidCardId = "No Corporate card was found with this Id.";
        public const string ApiErrorValidCardProvider = "A valid CardProviderId must be provided";
        public const string ApiErrorValidCardNumber = "A valid Card number must be provided";
        public const string ApiErrorCardNumberExists = "This Card number already exists,";
        public const string ApiErrorCannotReAssignCardByChangingEmployee = "You cannot re-assign a corporate card by changing the EmployeeId.";
        public const string ApiErrorNoCardStatements = "No card statements found for given employee.";
        public const string ApiErrorNoUnAllocatedCards = "No unallocated cards found for given statement.";
        public const string ApiErrorUnsuccessfulTransactionMatch = "An error has occured when matching a transaction to an expense";
        public const string ApiErrorUnsuccessfulRetrievalOfAdditionalTransactionInformation = "An error has occured when getting the additional transaction information";
        public const string ApiErrorTransactionInsufficientPermission = "You are not the owner of the supplied transactionId";
        public const string ApiErrorTransactionImportFailedToConvertTransactionData = "An error occured when attempting to convert the transaction import data to a Byte Array.";
        public const string ApiErrorTransactionImportFailed = "The following error occured when trying to import credit card transaction data ";
        public const string ApiErrorStatementNotFound = "No statement was found for the provided statement Id.";
        public const string ApiErrorStatementTransactionsAlreadyReconciled = "The statement cannot be deleted as one or more transactions have already been reconciled.";
        public const string ApiErrorTransactionDoesNotBelongToExpense = "The supplied Transaction Id does not belong to the Expense.";

        /// <summary>
        /// Warning literal for showing No Transaction found.
        /// </summary>
        public const string ApiErrorNoTransactionFound = "No transaction found.Please use valid transaction id.";
        public const string ApiErrorExpenseItemAssociatedWithAnotherTransaction = "The Expense Item is already associated with another transaction";
        public const string ApiErrorTransactionMatchCouldNotBeMade = "The match could not be made. Check transaction country status, currency or that the transaction total does not exceed the unallocated amount.";   
        
        #endregion

        #region Access Roles / Account

        public const string ApiErrorSubAccount = "Default sub account must be greater then zero.";
        public const string ApiErrorWrongIdAccessRole = "An AccessRole with this Id could not be found.";
        public const string ApiErrorWrongAccountIdMessage = "The Id could not be found or resulted in the wrong Account or SubAccount.";

        #endregion Access Roles / Account

        #region EmailNotifications

        public const string ApiErrorWrongEmailNotificationId = "Invalid Email Notification Id";
        public const string ApiErrorWrongEmailNotificationIdMessage = "The Email Notification Id could not be found or resulted in the wrong Email Notification.";

        #endregion EmailNotifications

        #region MobileDevice

        public const string ApiErrorCannotReAssignMobileByChangingEmployee = "You cannot re-assign a mobile device by changing the EmployeeId.";
        public const string ApiErrorCannotChangeDeviceType = "You cannot change the type of a mobile device once created.";
        public const string ApiErrorDeviceTypeInvalid = "A mobile device type does not exist with this Id. Use the MobileDevices/MobileDeviceTypes resource.";
        public const string ApiErrorDeviceNameExists = "A mobile device with this DeviceName already exists. Please choose another name.";

        #endregion MobileDevice

        #region Addresses

        public const string ApiErrorAddressNotFound = "An Address with the supplied Id could not be found.";
        public const string ApiErrorAddressANotFound = "No Address with the supplied AddressAId could not be found.";
        public const string ApiErrorAddressBNotFound = "An Address with the supplied AddressBId could not be found.";
        public const string ApiErrorAddressExists = "An Address already exists with this Line1, City, and Postcode.";
        public const string ApiErrorAddressLine1 = "You must supply a Line 1 value that is not empy or whitespace.";
        public const string ApiErrorAddressPostCode = "You must supply a Postcode value that is not empty or whitespace.";
        public const string ApiErrorAddressAddressName = "You must supply an AddressName value that is not empty or whitespace.";
        public const string ApiErrorAddressCleanseFail = "Address could not be cleansed.";
        public const string ApiErrorAddressCleanseFailMessage = "One of more items failed to be cleansed. Please repeat the get.";
        public const string ApiErrorAddressCleanseFailJourneySteps = "This address is being used in the journey stages of an expense claim.";
        public const string ApiErrorAddressCleanseFailEsr = "This address is being used by ESR.";
        public const string ApiErrorAddressCleanseFailEmployeeHome = "This address is being used as a home location by an Employee.";
        public const string ApiErrorAddressCleanseFailEmployeeWork = "This address is being used as a work location by an Employee.";
        public const string ApiErrorAddressCleanseFailOrganisation = "This address is being used as by an Organisation.";
        public const string ApiErrorAddressLabelExists = "An Address label with the same name already exists.";
        public const string ApiErrorAddressLabelExistsPersonal = "This Address already has a personal label attached.";
        public const string ApiErrorAddressLabelReservedWord = "Cannot save an Address label which is a reserved word (HOME or OFFICE)";
        public const string ApiErrorAddressLabelNotFound = "An AddressLabel with the supplied Id could not be found.";
        public const string ApiErrorAddressDistanceGetAllNotImplmented = "Get All for Address Distances is not implmented. Use ByAddress instead.";
        public const string ApiErrorAddressDistanceAttemptEndPointEdit = "You cannot modify the two Address Ids when editing an existing recommended distance.";
        public const string ApiErrorAddressDistanceExists = "A recommended distance already exists between these two addresses.";
        public const string ApiErrorAddressDistanceCannotBeNegative = "The recommended distance between two addresses cannot be negative.";
        public const string ApiErrorAddressLinkageId = "An AddressLinkage with this Id could not be found.";
        public const string ApiErrorAddressLinkageIdEmployee = "An AddressLinkage with this Id could not be found for the supplied Employee.";
        public const string ApiErrorAddressStartEndDateMismatch = "AddressLinkage EndDate cannot be before the StartDate.";
        public const string ApiErrorWorkAddressLinkageMustHaveTemp = "The IsTemporary flag must be set for WorkAddressLinkages.";
        public const string ApiErrorWorkAddressLinkageMustHaveActive = "The IsActive flag must be set for WorkAddressLinkages.";
        public const string ApiErrorAddressLinkageMustHaveEmployee = "The EmployeeId must be set to a valid Employee Id for AddressLinkages.";
        public const string ApiErrorAddressLinkageCannotEditEmployee = "You cannot change the EmployeeId of a created AddressLinkage.";
        public const string ApiErrorAddressLinkageMustHaveAddress = "The AddressId must be set to a valid Address Id for AddressLinkages.";
        public const string ApiErrorAddressLinkageCannotEditAddress = "You cannot change the AddressId of a created AddressLinkage.";
        public const string ApiErrorAddressDistanceIdMustBeValid = "The AddressDistanceId must be set to a valid Address Distance Id.";
        public const string ApiErrorAddressEditManualOnly = "You may only edit manually created addresses.";
        public const string APIErrorAddressInvalidWayPointsSequence = "Invalid waypoint sequence.";
        public const string APIErrorAddressInvalidWayPoints = "Invalid waypoints supplied.";
        public const string APIErrorAddressNoPermission = "You do not have permission to view the route.";
        public const string APIErrorMappingNotEnabledForAccount = "Your account does not have the ability to view the recommended route and directions on a map.";


        #endregion Addresses

        #region Receipts

        public const string ApiErrorNotImplementedReceiptGetById = "As yet there is no need to get a single Receipt by Id.";
        public const string ApiErrorNotImplementedReceiptGetAll = "Cannot get all receipts";
        public const string ApiErrorExtensionAndDataRequired = "The file extension and the actual data are required when adding a receipt.";
        public const string ApiErrorReceiptDoesntExist = "No receipt was found with the supplied Id.";
        public const string ApiErrorClaimDoesntExist = "No claim was found with the supplied Id.";
        public const string ApiErrorClaimLineDoesntExist = "No claimline (savedexpense) was found with the supplied Id.";
        public const string ApiErrorEnvelopeDoesntExist = "No Envelope was found with the supplied Id.";
        public const string ApiErrorEnvelopeDoesntHaveClaim = "This Envelope doesn't have a claim associated.";
        public const string ApiErrorClaimIsNotInScanAttachStage = "The Claim attached to this Envelope is not currently at the Scan & Attach stage.";
        public const string ApiErrorAccountDoesntExist = "There is no account matching the supplied Id.";

        #endregion Receipts

        #region ExpenseValidation

        public const string ApiErrorValidationResultDoesntExist = "No ExpenseValidationResult was found matching this Id.";
        public const string ApiErrorValidationResultGetAll = "Cannot get all ExpenseValidationResults in the system - only per expense item.";
        public const string ApiErrorValidationResultGetById = "Cannot get an ExpenseValidationResult without providing the expense Id.";
        public const string ApiErrorValidationResultInvalidExpense = "The Expense Id provided is invalid.";
        public const string ApiErrorValidationReasonDoesntExist = "No ExpenseValidationReason was found matching this Id.";
        public const string ApiErrorValidationCriterionDoesntExist = "No ExpenseValidationCriterion was found matching this Id.";
        public const string ApiErrorValidationNotEnoughResults = "Not enough Results provided to cover all the Criteria for this expense item.";
        public const string ApiErrorValidationCriterionDisabled = "This ExpenseValidationCriterion is disabled, so you cannot add results to it.";
        public const string ApiErrorValidationResultValidationDisabledAccount = "Expedite Expense Validation is disabled for this Account, therefore you cannot submit validation results for any ExpenseItem in this Account.";
        public const string ApiErrorValidationResultValidationDisabledSubcat = "Expedite Expense Validation is disabled for the ExpenseSubCategory from which this ExpenseItem is templated, therefore you cannot submit validation results for this ExpenseItem.";
        public const string ApiErrorValidationResultValidationDisabledSignoff = "The owner of this expense item is not in a Signoff Group containing a Validation stage.";
        public const string ApiErrorValidationResultValidationCompleted = "Expedite Expense Validation is marked as complete for this ExpenseItem - every Criterion has a corresponding Result. Consider editing an existing Result instead.";
        public const string ApiErrorValidationResultValidationNotRequired = "Expedite Expense Validation is not required for this ExpenseItem - there are no applicable Criteria for which to create Results.";
        public const string ApiErrorValidationResultResultExistsForCriterion = "There is already a Result (id:{0}) for this Criterion (id:{1}) on this ExpenseItem. For every ExpenseItem there must be a 1:1 relationship between Criteria and Results.";
        public const string ApiErrorValidationResultResultDoesntExistForCriterion = "There is no result for this Criterion (id:{0}) on this ExpenseItem. For every ExpenseItem there must be a 1:1 relationship between Criteria and Results. Consider adding instead.";
        public const string ApiErrorValidationResultNoTargetCriterion = "The target Criterion you are attempting to assign to (id:{0}) doesn't exist.";
        


        #endregion ExpenseValidation

        #region CompanyPolicy

        public const string ApiErrorGetCompanyPolicyUnSucessful = "Get Company Policy Unsuccessful";
        public const string ApiErrorGetCompanyPolicyMessage = "An error occured while retrieving the Company Policy";
        public const string ApiErrorCompanyPolicyNotFound = "Company Policy not found";

        #endregion CompanyPolicy

        #region claims
        public const string ApiErrorClaimNotFound = "{0} not found";
        public const string ApiErrorGetClaimMessage = "An error occurred while retrieving the {0}";
        public const string ApiErrorCreateClaimMessage = "An error occurred while creating the {0}";
        public const string ApiErrorUpdateClaimMessage = "An error occurred while updating the {0}";
        public const string ApiErrorCreateClaimIdNonZeroMessage = "The Id number for a new {0} must be 0";
        public const string ApiErrorCreateUpdateClaimAccountSingleClaim = "The account only permits one active claim at a time";
        public const string ApiErrorCreateClaimDuplicateName= "The claim name already exists in the system for this employee";
        public const string ApiErrorSaveUnsuccessfulInsufficientPermission = "You do not have permission to update this {0}";
        public const string ApiErrorSaveUnsuccessfulUnrecognisedUdf = "An unrecognised user defined field for this {0} has been found";
        public const string ApiErrorCheckAndPay = "You do not have permission to access check and pay details of this {0}";
        public const string ApiErrorInsufficientPermission = "Insufficient permission";
        public const string ApiErrorInvalidClaimId = "Invalid Claim Id";
        public const string ApiErrorProvideValidClaimIdMessage = "Please provide valid Claim Id";
        public const string ApiErrorInvalidRequest = "Invalid request";
        public const string ApiErrorInvalidClaimSubmissionMessage = "Invalid Claim Submission request";
        public const string ApiErrorClaimSubmission = "An error occurred while submitting the Claim";
        public const string ApiErrorClaimSubmissionUnsuccessfulMessage = "An error occurred while submitting the Claim";
        public const string ApiErrorInvalidClaimantJustificationsSaveMessage = "Invalid request to Save Claimant Flag Justifications";
        public const string ApiErrorInvalidApproverJustificationsSaveMessage = "Invalid request to Save Approver Flag Justifications";
        public const string ApiErrorInvalidRequestToGetMessage = "Invalid request to Get Flag Grid";
        public const string ApiErrorFlagIsNotAssociatedWithExpense = "The Flag Id submitted is not associated with the Expense Id submitted"; 
        #endregion

        #region Versioning

        public const string ApiErrorGetVersionUnSucessful = "Get Version Unsuccessful";
        public const string ApiErrorGetVersionMessage = "An error occured while retrieving the Version";

        #endregion 
                
        #region Journey
        public const string InvalidJourney = "Invalid journey steps";
        public const string InvalidJourneyMessage = "An error occurred while saving the journey";
        public const string ApiErrorGetEmployeeJourneysMessage = "An error occurred while retrieving the Employee's active journeys";
        public const string ApiErrorDeleteMobileJourneyMessage = "An error occurred while deleting the journey";
        public const string ApiErrorJourneyDoesNotBelongToEmployee = "The journey cannot be deleted as the journey does not belong to the employee";


        #endregion

        #region FilterRules

        public const string ApiErrorFilterRulesNotFound = "{0} not found";

        #endregion  FilterRules

        #region ClaimableItem
        public const string ApiErrorClaimableItemError = "Claimable Item not found";
        public const string ApiErrorClaimableItemNotAddedMessage = "An error occurred while adding the Claimable Items";
        public const string ApiErrorInvalidClaimableItemError = "Invalid Claimable Item";
        public const string ApiErrorInvalidClaimableItemMessage = "The Sub Category Ids list can not be found";
        public const string ApiErrorClaimableItemsError = "Claimable Items not found";
        public const string ApiErrorClaimableItemNotFoundMessage = "The Claimable Items not found.";
        #endregion

        #region receipt
        public const string ApiErrorReceiptUploadInvalid = "Invalid Receipt Upload request";
        public const string ApiErrorReceiptUploadMessage = "An error occurred while uploading the Receipt";

        public const string ApiErrorReceiptNotFound = "Receipt not found";
        public const string ApiErrorGetReceiptMessage = "An error occurred while getting the Receipt";
        #endregion

        #region SystemGreenLight
        public const string ResponseForbidddenUnAuthorisedUser = "Authentication failed or user does not have permissions for the requested operation.";
        public const string ResponseSystemGreenLightOnly = "Only System GreenLights can be copied.";
        public const string ResponseGreenLightNameExists = "The GreenLight name already exists.";
        public const string ResponseGreenLightPluralNameExists = "The Plural name you have provided already exists.";
        public const string ResponseGreenLightNameAndPluralNameExists = "The GreenLight name and Plural name you have provided already exists.";
        public const string ResponseGreenLightUpdatedSuccessfully ="The GreenLight - {0} has been updated successfully to {1}.";
        public const string ResponseGreenLightCopiedSuccessfully = "The GreenLight - {0} has been copied successfully to {1}.";
        public const string ResponseFailedToCopyGreenLight = "Could not complete the copy operation please contact the API administrator.";
        public const string ResponseCurrencyNotExists = "Currency does not exist in target account.";

        #endregion

        #region Advances

        public const string ApiErrorAdvanceNotFound = "The Advance could not be found for the AdvanceId provided.";
        public const string ApiErrorNotAdvanceApprover = "The Advance cannot be approved as you are no the approver of the Advance";
        public const string ApiErrorNotAdvanceOwner = "The action cannot take place as you are not the owner of the Advance";

        #endregion

        #region Holidays


        public const string ApiErrorInvalidHolidayPermission = "You do not have permission to view Holidays.";
        public const string ApiErrorNotHolidayOwner = "You do not have permission to perform this action as you are not the owner of the holiday.";
        public const string ApiErrorHolidayStartDateAfterEndDate = "The holiday start date cannot be after the holiday end date.";
        public const string ApiErrorHolidayEndDateBeforeStartDate = "The holiday end date cannot be before the holiday start date.";

        #endregion

        #region BankAccount

        public const string ApiErrorNotBankAccountOwner = "You do not have permission to perform this action, as you are not the owner of the Bank Account";

        #endregion

        #region EmailNotifications

        public const string ApiErrorEmailNotificationsFailed = "An error occured while trying to send email notifications";

        #endregion

    }

}