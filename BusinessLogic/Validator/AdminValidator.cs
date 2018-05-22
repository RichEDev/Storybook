namespace BusinessLogic.Validator
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic.Accounts;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Employees.AccessRoles;
    using BusinessLogic.Identity;

    /// <summary>
    /// A class to validate that a given <see cref="UserIdentity"/> has Admin menu access
    /// </summary>
    [Serializable]
    public class AdminValidator : IValidator
    {
        /// <summary>
        /// A list of <see cref="ModuleElements"/> that define an admin user.
        /// </summary>
        private static readonly List<ModuleElements> ElementsList = new List<ModuleElements>
                                                                        {
                                                                            ModuleElements.CorporateCards,
                                                                            ModuleElements.FinancialExports,
                                                                            ModuleElements.ImportDataWizard,
                                                                            ModuleElements.EsrTrustDetails,
                                                                            ModuleElements.ImportHistory,
                                                                            ModuleElements.ImportTemplates,
                                                                            ModuleElements.CompanyPolicy,
                                                                            ModuleElements.BroadcastMessages,
                                                                            ModuleElements.FlagsAndLimits,
                                                                            ModuleElements.AccessRoles,
                                                                            ModuleElements.ItemRoles,
                                                                            ModuleElements.SignOffGroups,
                                                                            ModuleElements.Employees,
                                                                            ModuleElements.Teams,
                                                                            ModuleElements.BudgetHolders,
                                                                            ModuleElements.Audiences,
                                                                            ModuleElements.PrintOut,
                                                                            ModuleElements.DefaultView,
                                                                            ModuleElements.DefaultPrintView,
                                                                            ModuleElements.Colours,
                                                                            ModuleElements.CompanyLogo,
                                                                            ModuleElements.Emails,
                                                                            ModuleElements.EmailSuffixes,
                                                                            ModuleElements.CompanyDetails,
                                                                            ModuleElements.GeneralOptions,
                                                                            ModuleElements.UserDefinedFields,
                                                                            ModuleElements.UserdefinedGroupings,
                                                                            ModuleElements.QuickEntryForms,
                                                                            ModuleElements.FilterRules,
                                                                            ModuleElements.Tooltips,
                                                                            ModuleElements.AttachmentMimeTypes,
                                                                            ModuleElements.CustomMimeHeaders,
                                                                            ModuleElements.Faqs,
                                                                            ModuleElements.CompanyHelpAndSupportInformation,
                                                                            ModuleElements.CostCodes,
                                                                            ModuleElements.Currencies,
                                                                            ModuleElements.Countries,
                                                                            ModuleElements.Locations,
                                                                            ModuleElements.ExpenseItems,
                                                                            ModuleElements.Reasons,
                                                                            ModuleElements.ProjectCodes,
                                                                            ModuleElements.P11D,
                                                                            ModuleElements.Departments,
                                                                            ModuleElements.VehicleJourneyRateCategories,
                                                                            ModuleElements.VehicleEngineType,
                                                                            ModuleElements.Allowances,
                                                                            ModuleElements.ExpenseCategories,
                                                                            ModuleElements.PoolCars,
                                                                            ModuleElements.Workflows,
                                                                            ModuleElements.AuditLog,
                                                                            ModuleElements.IpAdressFiltering,
                                                                            ModuleElements.SingleSignOn,
                                                                        };

        /// <summary>
        /// Is this object valid, based on the given <see cref="IAccount"/> and <see cref="UserIdentity"/>.
        /// </summary>
        /// <param name="account">
        /// An instance of <see cref="IAccount"/>.
        /// </param>
        /// <param name="userIdentity">
        /// An instance of <see cref="UserIdentity"/>.
        /// </param>
        /// <param name="employeeCombinedAccessRoles">
        /// An instance of <see cref="IEmployeeCombinedAccessRoles"/>.
        /// </param>
        /// <returns>
        /// <see cref="bool"/> true if this object is valid..
        /// </returns>
        public bool Valid(IAccount account, UserIdentity userIdentity, IEmployeeCombinedAccessRoles employeeCombinedAccessRoles)
        {
            var userAccessScope = employeeCombinedAccessRoles.Get(userIdentity.EmployeeId, userIdentity.SubAccountId);

            foreach (var scope in userAccessScope.Scopes)
            {
                if (ElementsList.Contains(scope.Element))
                {
                    return true;
                }
            }

            return false;
        }
    }
}