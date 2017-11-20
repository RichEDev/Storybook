namespace Spend_Management.shared.code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using SpendManagementLibrary;

    /// <summary>
    /// The AccessRoleCheck.
    /// </summary>
    public static class AccessRoleCheck
    {
        private static System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;


        /// <summary>
        /// Gets a value indicating whether the user is delegate.
        /// </summary>
        public static bool IsDelegate
        {
            get
            {
                if (session["myid"] != null)
                {
                    return (int)session["delegatetype"] == 1;
                }

                return false;
            }
        }

        /// <summary>
        /// Check whether the user can access administrative settings.
        /// </summary>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        public static bool CanAccessAdminSettings(CurrentUser user)
        {
            var customEntitiesByUser = new cCustomEntities(user);
            var userMiscellaneousInstance = new cMisc(user.AccountID);
            cGlobalProperties accountGlobalProperties = userMiscellaneousInstance.GetGlobalProperties(user.AccountID);
            bool showAdminForCustomEntities = false;
            bool isDelegateUser = IsDelegate;

            List<cCustomEntityView> menuItems;

            // 1 = home - no need to check this
            // 2 = Admin
            // 3 = category (base information)
            // 4 = tailoring
            // 5 = policy
            // 6 = user management
            for (int i = 2; i <= 6; i++)
            {
                menuItems = customEntitiesByUser.getViewsByMenuId(i);
                if ((from view in menuItems let entity = customEntitiesByUser.getEntityById(view.entityid) select view).Any(view => user.CheckAccessRole(AccessRoleType.View, CustomEntityElementType.View, view.entityid, view.viewid, false)))
                {
                    showAdminForCustomEntities = true;
                }

                if (showAdminForCustomEntities)
                {
                    break;
                }
            }

            if (showAdminForCustomEntities || CheckIfUserCanAccessAdmin(user) ||
                ((isDelegateUser && accountGlobalProperties.delauditlog &&
                  user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuditLog, true)) ||
                 (!isDelegateUser && user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuditLog, false))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Framework - Check whether the user can access administrative settings.
        /// </summary>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        public static bool FWCanAccessAdminSettings(CurrentUser user, cAccountProperties accountProperties)
        {
            if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Colours, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractCategories, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractStatus, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractTypes, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Currencies, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Countries, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomEntities, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentConfigurations, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentTemplates, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.GeneralOptions, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.InflatorMetrics, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportDataWizard, true) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Locations, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductCategories, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SalesTax, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Sites, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierCategory, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SupplierStatus, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.TaskTypes, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Teams, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.TermTypes, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Units, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserDefinedFields, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserdefinedGroupings, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductLicenceTypes, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Products, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tooltips, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.LicenceRenewalTypes, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Countries, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Locations, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AccessRoles, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuditLog, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.IPAdressFiltering, false) | (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SingleSignOn, false) && user.Account.HasLicensedElement(SpendManagementElement.SingleSignOn)) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractSupplierReassignment, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ContractProductReassignment, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportDataWizard, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AttachmentMimeTypes, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomMimeHeaders, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Emails, false) | (accountProperties.EnableRecharge & (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeClients, false) | user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.RechargeAccountCodes, false))))
            {
                return true;
            }
                return false;
            
        }

        /// <summary>
        /// Check if the user can access base info.
        /// </summary>
        /// <param name="currentUser">
        /// The cu.
        /// </param>
        /// <returns>
        /// The System.Boolean. TRUE if it is accessible else FALSE
        /// </returns>
        private static bool CheckIfUserCanAccessBaseInfo(ICurrentUser currentUser)
        {
            return currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CostCodes, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Currencies, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Countries, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Locations, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ExpenseItems, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reasons, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProjectCodes, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.P11D, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Departments, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.VehicleJourneyRateCategories, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.VehicleEngineType, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Allowances, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ExpenseCategories, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.PoolCars, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Workflows, true);
        }

        /// <summary>
        /// Check if the user can access tailoring.
        /// </summary>
        /// <param name="currentUser">
        /// The cu.
        /// </param>
        /// <returns>
        /// The System.Boolean. TRUE if it is accessible else FALSE
        /// </returns>
        private static bool CheckIfUserCanAccessTailoring(ICurrentUser currentUser)
        {
            return currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.PrintOut, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DefaultView, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DefaultPrintView, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Colours, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyLogo, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Emails, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EmailSuffixes, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyDetails, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.GeneralOptions, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserDefinedFields, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserdefinedGroupings, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.QuickEntryForms, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FilterRules, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tooltips, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AttachmentMimeTypes, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomMimeHeaders, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FAQS, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyHelpAndSupportInformation, true);
        }

        /// <summary>
        /// Check if the user can access policy info.
        /// </summary>
        /// <param name="currentUser">
        /// The cu.
        /// </param>
        /// <returns>
        /// The System.Boolean. TRUE if it is accessible else FALSE
        /// </returns>
        private static bool CheckIfUserCanAccessPolicyInfo(ICurrentUser currentUser)
        {
            return currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CompanyPolicy, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.BroadcastMessages, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FlagsAndLimits, true);
        }

        /// <summary>
        /// Check if the user can access user management.
        /// </summary>
        /// <param name="currentUser">
        /// The cu.
        /// </param>
        /// <returns>
        /// The System.Boolean. TRUE if it is accessible else FALSE
        /// </returns>
        private static bool CheckIfUserCanAccessUserManagement(ICurrentUser currentUser)
        {
            return currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AccessRoles, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ItemRoles, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SignOffGroups, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Employees, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Teams, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.BudgetHolders, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Audiences, true);
        }

        /// <summary>
        /// Check if the user can access system options.
        /// </summary>
        /// <param name="currentUser">
        /// The cu.
        /// </param>
        /// <returns>
        /// The System.Boolean. TRUE if it is accessible else FALSE
        /// </returns>
        private static bool CheckIfUserCanAccessSystemOptions(ICurrentUser currentUser)
        {
            return currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuditLog, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AttachmentMimeTypes, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomMimeHeaders, true) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.IPAdressFiltering, true) ||
                   (currentUser.Account.HasLicensedElement(SpendManagementElement.SingleSignOn) &&
                        currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SingleSignOn, true));
        }

        /// <summary>
        /// Check if the user can access imports exports.
        /// </summary>
        /// <param name="currentUser">
        /// The cu.
        /// </param>
        /// <returns>
        /// The System.Boolean. TRUE if it is accessible else FALSE
        /// </returns>
        private static bool CheckIfUserCanAccessImportsExports(ICurrentUser currentUser)
        {
            if (currentUser.CurrentActiveModule != Modules.Greenlight && currentUser.CurrentActiveModule != Modules.GreenlightWorkforce)
            {
                return currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CorporateCards, true) ||
                       currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FinancialExports, true) ||
                       currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportDataWizard, true) ||
                       currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ESRTrustDetails, true) ||
                       currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportHistory, true) ||
                       currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportTemplates, true);
            }

            return currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FinancialExports, true)
                    || currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportDataWizard, true)
                    ||
                    (currentUser.Account.IsNHSCustomer
                     &&
                     (currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportHistory, true)
                      || currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ESRTrustDetails, true)
                      || currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportTemplates, true)));
        }

        /// <summary>
        /// Check if the user can access admin.
        /// </summary>
        /// <param name="currentUser">
        /// The cu.
        /// </param>
        /// <returns>
        /// The System.Boolean. TRUE if it is accessible else FALSE
        /// </returns>
        private static bool CheckIfUserCanAccessAdmin(ICurrentUser currentUser)
        {
            return CheckIfUserCanAccessImportsExports(currentUser) ||
                   CheckIfUserCanAccessPolicyInfo(currentUser) ||
                   CheckIfUserCanAccessUserManagement(currentUser) ||
                   CheckIfUserCanAccessTailoring(currentUser) ||
                   CheckIfUserCanAccessBaseInfo(currentUser) ||
                   CheckIfUserCanAccessSystemOptions(currentUser) ||
                   currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AuditLog, true);
        }

    }

}