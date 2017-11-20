namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using System.Collections.Generic;
    using SpendManagementApi.Common;
    using Interfaces;
    using Repositories;

    using SpendManagementApi.Models.Common;

    using Utilities;

    using SpendManagementLibrary;
    using Spend_Management;

    /// <summary>
    /// Represents the more general, Expenses-related inforamtion for an <see cref="Employee">Employee</see>.
    /// </summary>
    public class EmployeeDetails : BaseExternalType, IRequiresValidation, IEquatable<EmployeeDetails>
    {
        /// <summary>
        /// The business contact details for this employee.
        /// </summary>
        public EmploymentContactDetails ContactDetails { get; set; }

        /// <summary>
        /// An EmployeePermissions object, which contains access roles associated with the user.
        /// </summary>
        public EmployeePermissions EmployeePermissions { get; set; }

        /// <summary>
        /// The Work details of the employee.
        /// </summary>
        public WorkDetails WorkDetails { get; set; }

        /// <summary>
        /// The NHS details of the employee.
        /// </summary>
        public NhsDetails NhsDetails { get; set; }

        /// <summary>
        /// The personal details of the employee.
        /// </summary>
        public PersonalDetails PersonalDetails { get; set; }

        /// <summary>
        /// The claim sign off details for the employee.
        /// </summary>
        public ClaimSignOffDetails ClaimSignOffDetails { get; set; }
        
        /// <summary>
        /// A list of the Ids of Email Notifications applied to this Employee.
        /// </summary>
        public List<int> EmailNotifications { get; internal set; }

        /// <summary>
        /// Validates this object.
        /// </summary>
        /// <param name="actionContext"></param>
        public void Validate(IActionContext actionContext)
        {
            Helper.ValidateIfNotNull(EmployeePermissions, actionContext, AccountId);

            Helper.ValidateIfNotNull(ClaimSignOffDetails, actionContext, AccountId);
            
            Helper.ValidateIfNotNull(WorkDetails, actionContext, AccountId);

            Helper.ValidateIfNotNull(this.PersonalDetails, actionContext, this.AccountId);

            Helper.ValidateIfNotNull(this.NhsDetails, actionContext, this.AccountId);
        }

        internal static EmployeeDetails Merge(EmployeeDetails dataToUpdate, EmployeeDetails existing)
        {
            if (dataToUpdate == null)
            {
                dataToUpdate = new EmployeeDetails { AccountId = existing.AccountId, CreatedById = existing.CreatedById };
            }
            
            dataToUpdate.ContactDetails = EmploymentContactDetails.Merge(dataToUpdate.ContactDetails, existing.ContactDetails);
            dataToUpdate.EmployeePermissions = EmployeePermissions.Merge(dataToUpdate.EmployeePermissions, existing.EmployeePermissions);
            dataToUpdate.WorkDetails = WorkDetails.Merge(dataToUpdate.WorkDetails, existing.WorkDetails);
            dataToUpdate.NhsDetails = NhsDetails.Merge(dataToUpdate.NhsDetails, existing.NhsDetails);
            dataToUpdate.PersonalDetails = PersonalDetails.Merge(dataToUpdate.PersonalDetails, existing.PersonalDetails);
            dataToUpdate.ClaimSignOffDetails = ClaimSignOffDetails.Merge(dataToUpdate.ClaimSignOffDetails, existing.ClaimSignOffDetails);

            return dataToUpdate;
        }

        public bool Equals(EmployeeDetails other)
        {
            if (other == null)
            {
                return false;
            }
            return AccountId.Equals(other.AccountId)
                && ContactDetails.Equals(other.ContactDetails)
                && ClaimSignOffDetails.Equals(other.ClaimSignOffDetails)
                && NhsDetails.Equals(other.NhsDetails)
                && PersonalDetails.Equals(other.PersonalDetails)
                && WorkDetails.Equals(other.WorkDetails);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as EmployeeDetails);
        }
    }

    internal static class EmployeeGeneralDetailsConversion
    {
        internal static TResult Cast<TResult>(this SpendManagementLibrary.Employees.Employee employee, 
            cLocales locales, cEmployees employees, cGlobalCountries globalCountries, 
            cGlobalCurrencies globalCurrencies, cAccountSubAccounts accountSubAccounts, 
            cESRAssignments esrAssignments, cUserdefinedFields userdefinedFields, cTable table, ICurrentUser user, IActionContext actionContext) 
            where TResult : EmployeeDetails, new()
        {
            var accessRoleRepository = new AccessRoleRepository(user, actionContext);
            return new TResult
                       {
                           //Generic
                           AccountId = employee.AccountID,
                           
                           CreatedById = employee.CreatedBy,
                           CreatedOn = employee.CreatedOn,
                           EmployeeId = employee.EmployeeID,

                           EmailNotifications = employee.GetEmailNotificationList().EmailNotificationIDs,
                           
                           ModifiedById = employee.ModifiedBy,
                           ModifiedOn = employee.ModifiedOn,

                    ContactDetails = new EmploymentContactDetails
                    {
                        EmailAddress = employee.EmailAddress,
                        ExtensionNumber = employee.TelephoneExtensionNumber,
                        FaxNumber = employee.FaxNumber,
                        MobileNumber = employee.MobileTelephoneNumber,
                        PagerNumber = employee.PagerNumber,
                        TelephoneNumber = employee.TelephoneNumber
                    },
                           
                           NhsDetails = new NhsDetails
                                            {
                                                TrustId = employee.NhsTrustID,
                                                NhsUniqueId = employee.NhsUniqueID,
                                                //EsrAssignments = esrAssignments.getAssignmentsAssociated().Values.Select(e => e.Cast<EsrAssignment>(employee.AccountID)).ToList()
                                            },

                           //Admin Tab 2
                           EmployeePermissions = employee.Cast<EmployeePermissions>(accessRoleRepository, accountSubAccounts),

                           //Admin Tab 3
                           WorkDetails = employee.Cast<WorkDetails>(employees, globalCountries, globalCurrencies),

                           //Admin Tab 4
                           PersonalDetails = employee.Cast<PersonalDetails>(employees, locales),

                           //Admin Tab 5
                           ClaimSignOffDetails = employee.Cast<ClaimSignOffDetails>(employee.GetItemRoles())
                       };
        }
    }
}