using System;

namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.IO;

    using SpendManagementApi.Common;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;

    using Spend_Management;
    using Interfaces;
    using Utilities;

    /// <summary>
    /// Represents an Employee within the system. The employee is the base of Spend Management.
    /// </summary>
    [MetadataType(typeof(IEmployee))]
    public class Employee : ArchivableBaseExternalType, IEmployee, IRequiresValidation, IEquatable<Employee>
    {
        /// <summary>
        /// The unique Id of this Employee.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The title of the employee. Mr, Ms etc.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// The user name of this Employee.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// The forename of the employee.
        /// </summary>
        [Required]
        public string Forename { get; set; }

        /// <summary>
        /// The surname of the employee.
        /// </summary>
        [Required]
        public string Surname { get; set; }

        /// <summary>
        /// General employee information.
        /// </summary>
        public EmployeeDetails EmployeeDetails { get; set; }

        /// <summary>
        /// The list of associated vehicles for this Employee.
        /// <strong>Do not try to modify this user's owned cars by changing this list.</strong>
        /// Instead use the <see cref="Vehicle">Vehicles</see> resource.
        /// </summary>
        public List<int> OwnedVehicles { get; internal set; }

        /// <summary>
        /// The list of associated pool cars for this Employee.
        /// <strong>Do not try to modify this user's pool cars by changing this list.</strong>
        /// Instead use the <see cref="Vehicle">Vehicles</see> resource.
        /// </summary>
        public List<int> PoolCars { get; internal set; }

        /// <summary>
        /// The list of associated corporate cards for this Employee.
        /// <strong>Do not try to modify this user's cards by changing this list.</strong>
        /// Instead use the <see cref="CorporateCard">CorporateCards</see> resources.
        /// </summary>
        public List<int> CorporateCards { get; internal set; }

        /// <summary>
        /// The list of work addresses at which this Employee works.
        /// These come in the form of an <see cref="WorkAddressLinkage">WorkAddressLinkage</see>.
        /// <strong>Do not try to modify this user's WorkAddresses by changing this list.</strong>
        /// Instead use the <see cref="Address">Addresses</see> resource.
        /// </summary>
        public List<WorkAddressLinkage> WorkAddresses { get; internal set; }

        /// <summary>
        /// The list of home addresses for this Employee.
        /// These come in the form of an <see cref="HomeAddressLinkage">HomeAddressLinkage</see>.
        /// <strong>Do not try to modify this user's HomeAddresses by changing this list.</strong>
        /// Instead use the <see cref="Address">Addresses</see> resource.
        /// </summary>
        public List<HomeAddressLinkage> HomeAddresses { get; internal set; }
        
        /// <summary>
        /// The list of Mobile devices for this Employee.
        /// <strong>Do not try to modify this user's MobileDevices by changing this list.</strong>
        /// Instead use the <see cref="MobileDevice">MobileDevices</see> resource.
        /// </summary>
        public List<int> MobileDevices { get; internal set; }

        /// <summary>
        /// Whether the employee status is set to active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value consent date of DVLA.
        /// The DVLA consent date is the consent provided date by the claimant.
        /// </summary>
        public DateTime? DvlaConsentDate { get; set; }

        /// <summary>
        /// Gets or sets a value of driverId.
        /// The Driver ID of the licence holder.
        /// </summary>
        public int? DriverId { get; set; }

        /// <summary>
        /// Whether the employee is locked.
        /// </summary>
        internal bool IsLocked { get; set; }

        /// <summary>
        /// The logon count for this Employee.
        /// </summary>
        internal int LogonCount { get; set; }

        /// <summary>
        /// The logon retry count for this Employee.
        /// </summary>
        internal int LogonRetryCount { get; set; }

        /// <summary>
        /// Whether this employee is verified.
        /// </summary>
        internal bool IsVerified { get; set; }
        
        /// <summary>
        /// The password of the employee.
        /// </summary>
        internal string Password { get; set; }
        
        /// <summary>
        /// The password encryption method of the employee.
        /// </summary>
        internal PasswordEncryptionMethod PasswordEncryptionMethod { get; set; }

        /// <summary>
        /// The datetime of the last change.
        /// </summary>
        public DateTime LastPasswordChange { get; internal set; }
        
        /// <summary>
        /// The current (most recent) claim number for this employee.
        /// </summary>
        public int? CurrentClaimReference { get; internal set; }
        
        /// <summary>
        /// The current (most recent) expense item reference number for this employee.
        /// </summary>
        public int? CurrentExpenseItemReference { get; internal set; }

        /// <summary>
        /// Any user defined fields.
        /// </summary>
        public List<UserDefinedFieldValue> UserDefinedFields { get; set; }


        internal static Employee Merge(Employee data, Employee existingEmployee)
        {
            data.Id = existingEmployee.Id;
            data.AccountId = existingEmployee.AccountId;
            data.IsLocked = existingEmployee.IsLocked;
            data.LogonCount = existingEmployee.LogonCount;
            data.LogonRetryCount = existingEmployee.LogonRetryCount;
            data.Archived = existingEmployee.Archived;
            data.Password = existingEmployee.Password;
            data.LastPasswordChange = existingEmployee.LastPasswordChange;
            data.PasswordEncryptionMethod = existingEmployee.PasswordEncryptionMethod;
            data.CurrentClaimReference = existingEmployee.CurrentClaimReference;
            data.CurrentExpenseItemReference = existingEmployee.CurrentExpenseItemReference;
            data.EmployeeDetails = EmployeeDetails.Merge(data.EmployeeDetails, existingEmployee.EmployeeDetails);
            return data;
        }

        /// <summary>
        /// Validation method
        /// </summary>
        public void Validate(IActionContext actionContext)
        {
            if (Id == 0 && actionContext.Employees.getEmployeeidByUsername(AccountId.Value, UserName) > 0)
            {
                throw new InvalidDataException(ApiResources.ApiErrorExistingUsername);
            }
            
            Helper.ValidateIfNotNull(EmployeeDetails, actionContext, AccountId);

            if (WorkAddresses != null && WorkAddresses.Count > 0)
            {
                WorkAddresses.ForEach(add => Helper.ValidateIfNotNull(add, actionContext, AccountId.Value));
            }

            if (HomeAddresses != null && HomeAddresses.Count > 0)
            {
                HomeAddresses.ForEach(add => Helper.ValidateIfNotNull(add, actionContext, AccountId.Value));
            }

            this.UserDefinedFields = UdfValidator.Validate(this.UserDefinedFields, actionContext, "userdefined_employees");
        }

        public bool Equals(Employee other)
        {
            if (other == null)
            {
                return false;
            }
            return AccountId.Equals(other.AccountId) 
                   && EmployeeDetails.Equals(other.EmployeeDetails)
                   && CorporateCards.SequenceEqual(other.CorporateCards) 
                   && PoolCars.SequenceEqual(other.PoolCars)
                   && OwnedVehicles.SequenceEqual(other.OwnedVehicles)
                   && MobileDevices.SequenceEqual(other.MobileDevices ?? new List<int>())
                   && IsActive.Equals(other.IsActive)
                   && Archived.Equals(other.Archived);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Employee);
        }

        /// <summary>
        /// Gets or sets a field indicating whether to send a wecome email and password key.
        /// Only applicable when creating a new employee or activating an existing employee.
        /// </summary>
        public bool SendPasswordKeyAndWelcomeEmail { get; set; }
    }

    /// <summary>
    /// Employee Conversion
    /// </summary>
    internal static class EmployeeConversion
    {
        internal static TResult Cast<TResult>(this SpendManagementLibrary.Employees.Employee employee, ICurrentUser user, IActionContext actionContext)
            where TResult : Employee, new()
        {
            if (employee == null)
            {
                return null;
            }

            actionContext.EmployeeId = employee.EmployeeID;
            var employeeCorporateCards = actionContext.EmployeeCorporateCards;
            var employeeCorporateCardList = new List<cEmployeeCorporateCard>(Helper.NullIf(employeeCorporateCards.GetEmployeeCorporateCards(employee.EmployeeID)).Values);
            var locales = actionContext.Locales;
            var cars = actionContext.EmployeeCars;
            var employees = actionContext.Employees;
            var globalCountries = actionContext.GlobalCountries;
            var globalCurrencies = actionContext.GlobalCurrencies;
            var accountSubAccounts = actionContext.SubAccounts;
            var esrAssignments = actionContext.EsrAssignments;
            var userDefinedFields = actionContext.UserDefinedFields;
            var tables = actionContext.Tables;
            var table = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            var mobileDevices = actionContext.MobileDevices;

            var homeLocations = employee.GetHomeAddresses().HomeLocations as IReadOnlyCollection<cEmployeeHomeLocation>;
            var homeLocationArray = homeLocations.ToArray();

            IDictionary<int, cEmployeeWorkLocation> workLocations = Helper.NullIf(employee.GetWorkAddresses().WorkLocations);
            var workLocationArray = new cEmployeeWorkLocation[workLocations.Count];
            workLocations.Values.CopyTo(workLocationArray, 0);

            var userDefinedFieldValues =
                employees.GetUserDefinedFields(employee.EmployeeID).ToUserDefinedFieldValueList();

            return new TResult
                       {
                           Id = employee.EmployeeID,
                           Title = employee.Title,
                           Forename = employee.Forename,
                           Surname = employee.Surname,
                           UserName = employee.Username,
                           AccountId = employee.AccountID,
                           IsActive = employee.Active,
                           Archived = employee.Archived,
                           CreatedById = employee.CreatedBy,
                           CreatedOn = employee.CreatedOn,
                           ModifiedById = employee.ModifiedBy,
                           ModifiedOn = employee.ModifiedOn,
                           EmployeeId = employee.EmployeeID,
                           IsVerified = employee.Verified,
                           IsLocked = employee.Locked,
                           UserDefinedFields = userDefinedFieldValues,
                           CorporateCards = employeeCorporateCardList.ConvertAll(cc => cc.corporatecardid),
                           MobileDevices = mobileDevices.GetMobileDevicesByEmployeeId(employee.EmployeeID).Keys.ToList(),
                           PoolCars = cars.Cars.Where(c => c.employeeid == 0).Select(c => c.carid).ToList(),
                           OwnedVehicles = cars.Cars.Where(c => c.employeeid == employee.EmployeeID).ToList().ConvertAll(c => c.carid),
                           HomeAddresses = new List<cEmployeeHomeLocation>(homeLocationArray).ConvertAll(address => address.Cast<HomeAddressLinkage>(employee.AccountID)),
                           WorkAddresses = new List<cEmployeeWorkLocation>(workLocationArray).ConvertAll(address => address.Cast<WorkAddressLinkage>(employee.AccountID)),
                           EmployeeDetails = employee.Cast<EmployeeDetails>(locales, employees, globalCountries, globalCurrencies, accountSubAccounts, esrAssignments, userDefinedFields, table, user, actionContext),
                           LogonCount = employee.LogonCount,
                           LogonRetryCount = employee.LogonRetryCount,
                           Password = employee.Password,
                           PasswordEncryptionMethod = employee.PasswordMethod,
                           LastPasswordChange = employee.LastChange,
                           CurrentExpenseItemReference = employee.CurrentReferenceNumber,
                           CurrentClaimReference = employee.CurrentClaimNumber,
                           DvlaConsentDate = employee.DvlaConsentDate,
                           DriverId = employee.DriverId
            };
        }

        internal static SpendManagementLibrary.Employees.Employee Cast<TResult>(this Employee employee, ICurrentUser user, IActionContext actionContext)
            where TResult : SpendManagementLibrary.Employees.Employee, new()
        {
            employee.EmployeeDetails = Helper.NullIf(employee.EmployeeDetails);
            employee.EmployeeDetails.ContactDetails = Helper.NullIf(employee.EmployeeDetails.ContactDetails);
            employee.EmployeeDetails.EmployeePermissions = Helper.NullIf(employee.EmployeeDetails.EmployeePermissions);
            employee.EmployeeDetails.EmployeePermissions.AccessRoles = Helper.NullIf(employee.EmployeeDetails.EmployeePermissions.AccessRoles);
            employee.EmployeeDetails.WorkDetails = Helper.NullIf(employee.EmployeeDetails.WorkDetails);
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns = Helper.NullIf(employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns);
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns = Helper.NullIf(employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns);
            employee.EmployeeDetails.NhsDetails = Helper.NullIf(employee.EmployeeDetails.NhsDetails);
            employee.EmployeeDetails.PersonalDetails = Helper.NullIf(employee.EmployeeDetails.PersonalDetails);
            employee.EmployeeDetails.PersonalDetails.BasicInfo = Helper.NullIf(employee.EmployeeDetails.PersonalDetails.BasicInfo);
            employee.EmployeeDetails.PersonalDetails.HomeContactDetails = Helper.NullIf(employee.EmployeeDetails.PersonalDetails.HomeContactDetails);
            employee.EmployeeDetails.PersonalDetails.BankAccount = Helper.NullIf(employee.EmployeeDetails.PersonalDetails.BankAccount);
            employee.EmployeeDetails.ClaimSignOffDetails = Helper.NullIf(employee.EmployeeDetails.ClaimSignOffDetails);
            employee.EmployeeDetails.ClaimSignOffDetails.ItemRoles = Helper.NullIf(employee.EmployeeDetails.ClaimSignOffDetails.ItemRoles);
            employee.UserDefinedFields = Helper.NullIf(employee.UserDefinedFields);
            employee.OwnedVehicles = Helper.NullIf(employee.OwnedVehicles);
            employee.PoolCars = Helper.NullIf(employee.PoolCars);
            employee.CorporateCards = Helper.NullIf(employee.CorporateCards);
            employee.WorkAddresses = Helper.NullIf(employee.WorkAddresses);
            employee.HomeAddresses = Helper.NullIf(employee.HomeAddresses);

            cAccountProperties properties = actionContext.SubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            
            //employee.EmployeeDetails.NhsDetails.EsrAssignments = Helper.NullIf(employee.EmployeeDetails.NhsDetails.EsrAssignments);
            
            var convertedEmployee =
                new SpendManagementLibrary.Employees.Employee(
                    employee.AccountId.Value,
                    employee.Id,
                    employee.UserName,
                    employee.Password ?? string.Empty,
                    employee.EmployeeDetails.ContactDetails.EmailAddress ?? string.Empty,
                    employee.Title,
                    employee.Forename ?? string.Empty,
                    employee.EmployeeDetails.PersonalDetails.BasicInfo.MiddleName ?? string.Empty,
                    employee.EmployeeDetails.PersonalDetails.BasicInfo.MaidenName ?? string.Empty,
                    employee.Surname ?? string.Empty,
                    employee.IsActive,
                    employee.IsVerified,
                    employee.Archived,
                    employee.IsLocked,
                    employee.LogonCount,
                    employee.LogonRetryCount,
                    employee.CreatedOn,
                    employee.CreatedById,
                    employee.ModifiedOn,
                    employee.ModifiedById,
                    employee.EmployeeDetails.PersonalDetails.BankAccount.Cast<SpendManagementLibrary.Employees.BankAccount>(),
                    employee.EmployeeDetails.ClaimSignOffDetails.SignOffGroupId ?? 0,
                    employee.EmployeeDetails.ContactDetails.ExtensionNumber ?? string.Empty,
                    employee.EmployeeDetails.ContactDetails.MobileNumber ?? string.Empty,
                    employee.EmployeeDetails.ContactDetails.PagerNumber ?? string.Empty,
                    employee.EmployeeDetails.PersonalDetails.HomeContactDetails.FaxNumber ?? string.Empty,
                    employee.EmployeeDetails.PersonalDetails.HomeContactDetails.EmailAddress ?? string.Empty,
                    employee.EmployeeDetails.WorkDetails.LineManagerUserId ?? 0,
                    employee.EmployeeDetails.ClaimSignOffDetails.AdvancesSignOffGroupId ?? 0,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                    employee.EmployeeDetails.PersonalDetails.BasicInfo.PreferredName ?? string.Empty,
                    employee.EmployeeDetails.PersonalDetails.BasicInfo.Gender ?? string.Empty,
                    employee.EmployeeDetails.PersonalDetails.BasicInfo.DateOfBirth,
                    employee.EmployeeDetails.WorkDetails.HireDate,
                    employee.EmployeeDetails.WorkDetails.TerminationDate,
                    employee.EmployeeDetails.WorkDetails.PayRollNumber ?? string.Empty,
                    employee.EmployeeDetails.WorkDetails.Position ?? string.Empty,
                    employee.EmployeeDetails.PersonalDetails.HomeContactDetails.TelephoneNumber ?? string.Empty,
                    employee.EmployeeDetails.WorkDetails.CreditAccount ?? string.Empty,
                    CreationMethod.Api,
                    employee.PasswordEncryptionMethod,
                    employee.EmployeeDetails.PersonalDetails.BasicInfo.FirstLogon,
                    false, // should not be allowed to set admin override from the API.
                    employee.EmployeeDetails.EmployeePermissions.DefaultSubAccountId ?? 1,
                    (!employee.EmployeeDetails.WorkDetails.PrimaryCurrencyId.HasValue ||
                        (employee.EmployeeDetails.WorkDetails.PrimaryCurrencyId.HasValue &&
                            employee.EmployeeDetails.WorkDetails.PrimaryCurrencyId.Value == 0)) ? 
                            properties.BaseCurrency.Value : 
                            employee.EmployeeDetails.WorkDetails.PrimaryCurrencyId.Value,
                    (!employee.EmployeeDetails.WorkDetails.PrimaryCountryId.HasValue || 
                        (employee.EmployeeDetails.WorkDetails.PrimaryCountryId.HasValue && 
                            employee.EmployeeDetails.WorkDetails.PrimaryCountryId.Value == 0)) 
                                ? properties.HomeCountry : employee.EmployeeDetails.WorkDetails.PrimaryCountryId.Value,
                    employee.EmployeeDetails.ClaimSignOffDetails.CreditCardSignOffGroupId ?? 0,
                    employee.EmployeeDetails.ClaimSignOffDetails.PurchaseCardSignOffGroupId ?? 0,
                    employee.EmployeeDetails.PersonalDetails.BasicInfo.HasCustomisedAddItems,
                    employee.EmployeeDetails.PersonalDetails.BasicInfo.LocaleId,
                    employee.EmployeeDetails.NhsDetails.TrustId,
                    employee.EmployeeDetails.WorkDetails.NationalInsuranceNumber ?? string.Empty,
                    employee.EmployeeDetails.WorkDetails.EmployeeNumber ?? string.Empty,
                    employee.EmployeeDetails.NhsDetails.NhsUniqueId ?? string.Empty,
                    employee.EmployeeDetails.WorkDetails.EsrPersonId,
                    employee.EmployeeDetails.WorkDetails.EsrEffectiveStartDate,
                    employee.EmployeeDetails.WorkDetails.EsrEffectiveEndDate,
                    employee.CurrentClaimReference ?? 0,
                    DateTime.UtcNow,
                    employee.CurrentExpenseItemReference ?? 0,
                    employee.EmployeeDetails.WorkDetails.StartMileage,
                    employee.EmployeeDetails.WorkDetails.StartMileageDate,
                    false,
                    dvlaConsentDate: employee.DvlaConsentDate,
                    driverId: employee.DriverId);

            return convertedEmployee;
        }
    }
}
