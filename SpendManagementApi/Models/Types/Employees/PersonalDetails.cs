namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using SpendManagementApi.Common;
    using Spend_Management;
    using Interfaces;
    using Attributes.Validation;
    using Utilities;

    /// <summary>
    /// Represents a collection of information about an Employee.
    /// </summary>
    public class PersonalDetails : BaseExternalType, IRequiresValidation, IEquatable<PersonalDetails>
    {
        /// <summary>
        /// The optional basic information for the employee.
        /// </summary>
        public OptionalGeneralDetails BasicInfo { get; set; }

        /// <summary>
        /// The home contact details for the employee.
        /// </summary>
        public HomeContactDetails HomeContactDetails { get; set; }

        /// <summary>
        /// The Bank account for the employee.
        /// </summary>
        public BankAccount BankAccount { get; set; }
        
        public void Validate(IActionContext actionContext)
        {
            Helper.ValidateIfNotNull(BasicInfo, actionContext, AccountId);
           Helper.ValidateIfNotNull(BankAccount, actionContext, AccountId);
        }

        internal static PersonalDetails Merge(PersonalDetails dataToUpdate, PersonalDetails existingData)
        {
            if (dataToUpdate == null)
            {
                dataToUpdate = new PersonalDetails
                                   {
                                       AccountId = existingData.AccountId,
                                       CreatedById = existingData.CreatedById,
                                       CreatedOn = existingData.CreatedOn,
                                   };
            }

            dataToUpdate.BasicInfo = OptionalGeneralDetails.Merge(dataToUpdate.BasicInfo, existingData.BasicInfo);
            dataToUpdate.HomeContactDetails = HomeContactDetails.Merge(dataToUpdate.HomeContactDetails, existingData.HomeContactDetails);
            dataToUpdate.BankAccount = BankAccount.Merge(dataToUpdate.BankAccount, existingData.BankAccount);
            return dataToUpdate;
        }

        public bool Equals(PersonalDetails other)
        {
            if (other == null)
            {
                return false;
            }
            return BasicInfo.Equals(other.BasicInfo)
            && HomeContactDetails.Equals(other.HomeContactDetails);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PersonalDetails);
        }
    }


    /// <summary>
    /// Represents the basic contact details for an emnployee.
    /// </summary>
    public class EmploymentContactDetails : HomeContactDetails
    {
        /// <summary>
        /// The extension number of the employee.
        /// </summary>
        public string ExtensionNumber { get; set; }

        /// <summary>
        /// The mobile number of the employee.
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// The mobile number of the employee.
        /// </summary>
        public string PagerNumber { get; set; }


        internal static EmploymentContactDetails Merge(EmploymentContactDetails dataToUpdate, EmploymentContactDetails existingData)
        {
            return dataToUpdate ?? (new EmploymentContactDetails
            {
                EmailAddress = existingData.EmailAddress,
                ExtensionNumber = existingData.ExtensionNumber,
                FaxNumber = existingData.FaxNumber,
                MobileNumber = existingData.MobileNumber,
                PagerNumber = existingData.PagerNumber,
                TelephoneNumber = existingData.TelephoneNumber
            });
        }

        public bool Equals(EmploymentContactDetails other)
        {
            if (other == null)
            {
                return false;
            }
            return this.EmailAddress.Equals(other.EmailAddress)
                   && this.ExtensionNumber.Equals(other.ExtensionNumber) && this.FaxNumber.Equals(other.FaxNumber)
                   && this.MobileNumber.Equals(other.MobileNumber) && this.PagerNumber.Equals(other.PagerNumber);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as EmploymentContactDetails);
        }
    }


    /// <summary>
    /// Contact Details for the employee.
    /// </summary>
    public class HomeContactDetails : IEquatable<HomeContactDetails>
    {
        /// <summary>
        /// The email address of the employee.
        /// </summary>
        [OptionalEmailAddress(ErrorMessage = ApiResources.ApiErrorOptionalEmailWrong)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The fax number of the employee.
        /// </summary>
        public string FaxNumber { get; set; }

        /// <summary>
        /// The telephone number of the employee.
        /// </summary>
        public string TelephoneNumber { get; set; }


        internal static HomeContactDetails Merge(HomeContactDetails dataToUpdate, HomeContactDetails existingData)
        {
            return dataToUpdate ?? (new HomeContactDetails
            {
                EmailAddress = existingData.EmailAddress,
                FaxNumber = existingData.FaxNumber,
                TelephoneNumber = existingData.TelephoneNumber
            });
        }

        public bool Equals(HomeContactDetails other)
        {
            if (other == null)
            {
                return false;
            }
            return this.EmailAddress.Equals(other.EmailAddress)
                   && this.FaxNumber.Equals(other.FaxNumber)
                   && this.TelephoneNumber.Equals(other.TelephoneNumber);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as HomeContactDetails);
        }
    }

    internal static class PersonalDetailsConversion
    {
        internal static TResult Cast<TResult>(this SpendManagementLibrary.Employees.Employee employee, cEmployees employees, cLocales locales)
            where TResult : PersonalDetails, new()
        {
            return new TResult
            {
                AccountId = employee.AccountID,
                CreatedById = employee.CreatedBy,
                CreatedOn = employee.CreatedOn,
                EmployeeId = employee.EmployeeID,
                HomeContactDetails = new HomeContactDetails
                {
                    EmailAddress = employee.HomeEmailAddress,
                    FaxNumber = employee.FaxNumber,
                    TelephoneNumber = employee.TelephoneNumber
                },
                BankAccount = new BankAccount
                {
                    AccountHolderName = employee.BankAccountDetails.AccountHolderName,
                    AccountNumber = employee.BankAccountDetails.AccountNumber,
                    AccountReference = employee.BankAccountDetails.AccountReference,
                    AccountType = employee.BankAccountDetails.AccountType,
                    SortCode = employee.BankAccountDetails.SortCode,
                },
                ModifiedById = employee.ModifiedBy,
                ModifiedOn = employee.ModifiedOn,
                BasicInfo = new OptionalGeneralDetails
                {
                    LocaleId = employee.LocaleID,
                    MaidenName = employee.MaidenName,
                    MiddleName = employee.MiddleNames,
                    PreferredName = employee.PreferredName,
                    DateOfBirth = employee.DateOfBirth,
                    Gender = employee.Gender,
                    AdminOverride = employee.AdminOverride,
                    FirstLogon = employee.FirstLogon,
                    HasCustomisedAddItems = employee.HasCustomisedAddItems
                }
            };
        }

    }

    /// <summary>
    /// Represents a User's Bank Account information.
    /// </summary>
    public class BankAccount : IRequiresValidation
    {
        /// <summary>
        /// The AccountId
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// The Account Name
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// The account holder name.
        /// </summary>
        public string AccountHolderName { get; set; }

        /// <summary>
        /// The account number.
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// The account type.
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// The sort code.
        /// </summary>
        public string SortCode { get; set; }

        /// <summary>
        /// The account reference.
        /// </summary>
        public string AccountReference { get; set; }

        internal static BankAccount Merge(BankAccount dataToUpdate, BankAccount existingData)
        {
            if (dataToUpdate == null)
            {
                dataToUpdate = new BankAccount
                {
                    AccountId = existingData.AccountId,
                    AccountName = existingData.AccountName,
                    AccountHolderName = existingData.AccountHolderName,
                    AccountNumber = existingData.AccountNumber,
                    AccountReference = existingData.AccountReference,
                    AccountType = existingData.AccountType,
                    SortCode = existingData.SortCode
                };
            }

            return dataToUpdate;
        }

        public void Validate(IActionContext actionContext)
        {
            
        }
    }


    internal static class BankAccountConversion
    {
        internal static SpendManagementLibrary.Employees.BankAccount Cast<TResult>(
            this BankAccount bankAccount)
            where TResult : SpendManagementLibrary.Employees.BankAccount
        {
            if (bankAccount == null)
            {
                return null;
            }
            return new SpendManagementLibrary.Employees.BankAccount(
                !string.IsNullOrEmpty(bankAccount.AccountHolderName) ? bankAccount.AccountHolderName : string.Empty,
                !string.IsNullOrEmpty(bankAccount.AccountNumber) ? bankAccount.AccountNumber : string.Empty,
                !string.IsNullOrEmpty(bankAccount.AccountType) ? bankAccount.AccountType : string.Empty,
                !string.IsNullOrEmpty(bankAccount.SortCode) ? bankAccount.SortCode : string.Empty,
                !string.IsNullOrEmpty(bankAccount.AccountReference) ? bankAccount.AccountReference : string.Empty);
        }
    }
}