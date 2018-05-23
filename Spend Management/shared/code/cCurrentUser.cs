namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;

    using BusinessLogic.Modules;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Enumerators;

    /// <summary>
    /// Information about the current user as determined by the User.Identity.Name credentials
    /// </summary>
    [Serializable()]
    public class CurrentUser : cCurrentUserBase, ICurrentUser
    {
        /// <summary>
        /// Are we coming from the scheduler
        /// </summary>
        public bool IgnoreCache { get; set; }

        /// <summary>
        /// A private instance of <see cref="cAccessRoles"/>
        /// </summary>
        private cAccessRoles _accessRoles;

        /// <summary>
        /// A private list of <see cref="cAccessRole"/> for the current user
        /// </summary>
        private List<cAccessRole> _accessRoleList;

        /// <summary>
        /// Gets or sets the access role object for this user
        /// </summary>
        public cAccessRoles AccessRoles
        {
            get
            {
                if (this.AccountID != 0 && this._accessRoles == null)
                {
                    this._accessRoles = new cAccessRoles(this.AccountID, cAccounts.getConnectionString(this.AccountID));
                }

                return this._accessRoles;
            }

        }

        /// <summary>
        /// A list of <see cref="cAccessRole"/> for the current user.
        /// </summary>
        public List<cAccessRole> EmployeeAccessRoles
        {
            get
            {
                if (this._accessRoleList == null)
                {
                    var accessRoleIds = this.Employee.GetAccessRoles().GetBy(this.CurrentSubAccountId);
                    this._accessRoleList = new List<cAccessRole>();
                    foreach (var accessRoleId in accessRoleIds)
                    {
                        this._accessRoleList.Add(this.AccessRoles.GetAccessRoleByID(accessRoleId));
                    }
                }

                return this._accessRoleList;
            }
        }

        public CurrentUser()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CurrentUser"/> class.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="employeeid">
        /// The employeeid.
        /// </param>
        /// <param name="employeeDelegateId">
        /// The employee delegate id.
        /// </param>
        /// <param name="active_module">
        /// The active_module.
        /// </param>
        /// <param name="subAccountId">
        /// The sub account id.
        /// </param>
        /// <param name="ignoreCache">Are we coming from scheduler. Defaults to false</param>
        public CurrentUser(
            int accountid,
            int employeeid,
            int employeeDelegateId,
            Modules active_module,
            int subAccountId,
            bool ignoreCache = false)
        {
            this.IgnoreCache = ignoreCache;
            this.nAccountid = accountid;
            this.nEmployeeid = employeeid;
            this.nActiveModuleId = active_module;
            this.nActiveSubAccountId = subAccountId;
            this.clsDelegate =
                new Lazy<Employee>(
                    () =>
                    new cEmployees(this.AccountID).GetEmployeeById(employeeDelegateId == 0 ? -1 : employeeDelegateId));
            this.clsEmployee = new Lazy<Employee>(() => new cEmployees(this.AccountID).GetEmployeeById(employeeid));
        }

        /// <summary>
        /// Returns if they have access to a specific element with a specific role type (add, edit, delete, view);=
        /// </summary>
        /// <param name="type">The type of access you want to check if they can do</param>
        /// <param name="element">The element you want to check access for</param>
        /// <param name="checkIfDDelegate">State if you allow the user to access this page if he is logged in as a delegate</param>
        /// <returns></returns>
        public bool CheckAccessRole(AccessRoleType type, SpendManagementElement element, bool checkIfDDelegate)
        {
            return this.CheckAccessRole(
                type,
                element,
                checkIfDDelegate,
                false,
                CustomEntityElementType.None,
                null,
                null,
                AccessRequestType.Website);
        }

        /// <summary>
        /// Returns if the user has access to a custom entity form or view with a specific role type
        /// </summary>
        /// <param name="type">Access Type view, add, edit, delete, approve</param>
        /// <param name="customEntityElementType">The page type this item gives access to</param>
        /// <param name="customEntityID">Entity ID check access to</param>
        /// <param name="customEntityRelatedID">The form or view ID</param>
        /// <param name="redirectToErrorHome">Redirect to error home page if error encountered or access denied</param>
        /// <param name="requestType">The source of the request</param>
        /// <returns></returns>
        public bool CheckAccessRole(
            AccessRoleType type,
            CustomEntityElementType customEntityElementType,
            int customEntityID,
            int customEntityRelatedID,
            bool redirectToErrorHome,
            AccessRequestType requestType = AccessRequestType.Website)
        {
            return this.CheckAccessRole(
                type,
                SpendManagementElement.CustomEntityInstances,
                true,
                redirectToErrorHome,
                customEntityElementType,
                customEntityID,
                customEntityRelatedID,
                requestType);
        }

        /// <summary>
        /// Returns if they have access to a specific element with a specific role type (add, edit, delete, view);=
        /// </summary>
        /// <param name="type">The type of access you want to check if they can do</param>
        /// <param name="element">The element you want to check access for</param>
        /// <param name="checkIfDDelegate">State if you allow the user to access this page if he is logged in as a deleage</param>
        /// <param name="autoRedirectToHome">If the employee does not have access, do you want to redirect them to the home page?</param>
        /// <returns></returns>
        public bool CheckAccessRole(
            AccessRoleType type,
            SpendManagementElement element,
            bool checkIfDDelegate,
            bool autoRedirectToHome)
        {
            return this.CheckAccessRole(
                type,
                element,
                checkIfDDelegate,
                autoRedirectToHome,
                CustomEntityElementType.None,
                null,
                null,
                AccessRequestType.Website);
        }


        /// <summary>
        /// For an ApiUser, returns if they have access to a specific element with a specific role type (add, edit, delete, view);=
        /// </summary>
        /// <param name="type">The type of access you want to check if they can do</param>
        /// <param name="element">The element you want to check access for</param>
        /// <param name="requestType">Whether the request came from mobile. If true, stops this method checking the user has an access role containing <see cref="SpendManagementElement.Api" /></param>
        /// <returns>True if access is granted, false if not.</returns>
        public bool CheckAccessRoleApi(
            SpendManagementElement element,
            AccessRoleType type,
            AccessRequestType requestType)
        {

            if (Employee == null) return false;

            #region SEL adminonly check

            if (Employee.AdminOverride) return true;

            #endregion

            if (element == SpendManagementElement.Api)
            {
                element = SpendManagementElement.None;
            }

            if (requestType == AccessRequestType.Api)
            {
                if (GetBypassedElements().FirstOrDefault(el => el == element) == element)
                {
                    return true;
                }
            }

            if (this.EmployeeAccessRoles == null || this.EmployeeAccessRoles.Count <= 0) return false;

            // set up bools
            var allowApi = false;
            var allowMobile = false;
            var allowWebsite = false;
            var containsElement = false;

            // now check that they have access to the SpendManagementElement provided.
            foreach (var accessRole in this.EmployeeAccessRoles)
            {
                if (accessRole.ElementAccess != null)
                {
                    // check in case one of the element access is API.
                    if (!allowApi && accessRole.AllowApiAccess)
                    {
                        allowApi = true;
                    }

                    if (!allowMobile && accessRole.AllowMobileAccess)
                    {
                        allowMobile = true;
                    }

                    if (!allowWebsite && accessRole.AllowWebsiteAccess)
                    {
                        allowWebsite = true;
                    }

                    if (element != SpendManagementElement.None
                        && ((requestType == AccessRequestType.Api && accessRole.AllowApiAccess)
                            || requestType == AccessRequestType.Mobile && accessRole.AllowMobileAccess))
                    {
                        // now check the specific area
                        var target = accessRole.ElementAccess.ContainsKey(element)
                                         ? accessRole.ElementAccess[element]
                                         : null;

                        if (target != null)
                        {
                            switch (type)
                            {
                                case AccessRoleType.View:
                                    if (target.CanView)
                                    {
                                        containsElement = true;
                                    }
                                    break;
                                case AccessRoleType.Add:
                                    if (target.CanAdd)
                                    {
                                        containsElement = true;
                                    }
                                    break;
                                case AccessRoleType.Edit:
                                    if (target.CanEdit)
                                    {
                                        containsElement = true;
                                    }
                                    break;
                                case AccessRoleType.Delete:
                                    if (target.CanDelete)
                                    {
                                        containsElement = true;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            switch (requestType)
            {
                case AccessRequestType.Website:
                    if (!allowWebsite)
                    {
                        return false;
                    }
                    break;
                case AccessRequestType.Mobile:
                    if (!allowMobile)
                    {
                        return false;
                    }
                    break;
                case AccessRequestType.Api:
                    if (!allowApi)
                    {
                        return false;
                    }
                    break;
            }

            if (element == SpendManagementElement.None)
            {
                return true;
            }

            var accessRolePermitsAccess = requestType == AccessRequestType.Api ? allowApi : allowMobile;        

            return accessRolePermitsAccess && containsElement;
        }

        /// <summary>
        /// Returns if they have access to a specific element with a specific role type (add, edit, delete, view);=
        /// </summary>
        /// <param name="type">The type of access you want to check if they can do</param>
        /// <param name="element">The element you want to check access for</param>
        /// <param name="checkIfDDelegate">State if you allow the user to access this page if he is logged in as a deleage</param>
        /// <param name="autoRedirectToHome">If the employee does not have access, do you want to redirect them to the home page?</param>
        /// <param name="customEntityElementType">The custom entity element type</param>
        /// <param name="requestType">The source of the request</param>
        /// <param name="customEntityID">Optional Greenlight ID</param>
        /// <returns></returns>
        public bool CheckAccessRole(
            AccessRoleType type,
            SpendManagementElement element,
            bool checkIfDDelegate,
            bool autoRedirectToHome,
            CustomEntityElementType customEntityElementType,
            int? customEntityID,
            int? customEntityRelatedID,
            AccessRequestType requestType = AccessRequestType.Website)
        {
            int? delegatetype = null;
            bool result = false;

            if (HttpContext.Current.Session != null && HttpContext.Current.Session["delegatetype"] != null)
            {
                delegatetype = (int)HttpContext.Current.Session["delegatetype"];
            }

            #region SEL adminonly check

            if (Employee != null && Employee.AdminOverride && (!delegatetype.HasValue))
            {
                return true;
            }

            #endregion

            if (this.Employee != null)
            {
                if (this.EmployeeAccessRoles != null && this.EmployeeAccessRoles.Count > 0)
                {

                    foreach (var accessRole in this.EmployeeAccessRoles)
                    {
                        switch (requestType)
                        {
                            case AccessRequestType.Website:
                                if (!accessRole.AllowWebsiteAccess)
                                {
                                    continue;
                                }
                                break;
                            case AccessRequestType.Mobile:
                                if (!accessRole.AllowMobileAccess)
                                {
                                    continue;
                                }
                                break;
                            case AccessRequestType.Api:
                                if (!accessRole.AllowApiAccess)
                                {
                                    continue;
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("requestType");
                        }

                        #region custom entity access role check

                        // new block to handle custom entity views and forms, currently simplistic and will probably need structuring changes when access roles for custom entities are properly formulated as currently if nothing is set it should not prevent access
                        if (element == SpendManagementElement.CustomEntityInstances
                            && accessRole.CustomEntityAccess != null && customEntityID.HasValue == true
                            && accessRole.CustomEntityAccess.ContainsKey(customEntityID.Value) == true)
                        {
                            if (customEntityElementType == CustomEntityElementType.Entity)
                            {
                                if (type == AccessRoleType.Delete
                                    && accessRole.CustomEntityAccess[customEntityID.Value].CanDelete == true)
                                {
                                    return true;
                                }
                                else if (type == AccessRoleType.Edit
                                         && accessRole.CustomEntityAccess[customEntityID.Value].CanEdit == true)
                                {
                                    return true;
                                }
                                else if (type == AccessRoleType.Add
                                         && accessRole.CustomEntityAccess[customEntityID.Value].CanAdd == true)
                                {
                                    return true;
                                }
                                else if (type == AccessRoleType.View
                                         && (accessRole.CustomEntityAccess[customEntityID.Value].CanView == true
                                             || accessRole.CustomEntityAccess[customEntityID.Value].CanAdd
                                             == true
                                             || accessRole.CustomEntityAccess[customEntityID.Value].CanEdit
                                             == true
                                             || accessRole.CustomEntityAccess[customEntityID.Value].CanDelete
                                             == true))
                                {
                                    return true;
                                }
                            }
                            else if (customEntityElementType == CustomEntityElementType.View
                                     && customEntityRelatedID.HasValue == true
                                     && accessRole.CustomEntityAccess[customEntityID.Value].ViewAccess.ContainsKey(
                                         customEntityRelatedID.Value))
                            {
                                if (type == AccessRoleType.Delete
                                    && accessRole.CustomEntityAccess[customEntityID.Value].ViewAccess[
                                        customEntityRelatedID.Value].CanDelete == true)
                                {
                                    return true;
                                }
                                else if (type == AccessRoleType.Edit
                                         && accessRole.CustomEntityAccess[customEntityID.Value].ViewAccess[
                                             customEntityRelatedID.Value].CanEdit == true)
                                {
                                    return true;
                                }
                                else if (type == AccessRoleType.Add
                                         && accessRole.CustomEntityAccess[customEntityID.Value].ViewAccess[
                                             customEntityRelatedID.Value].CanAdd == true)
                                {
                                    return true;
                                }
                                else if (type == AccessRoleType.View
                                         && (accessRole.CustomEntityAccess[customEntityID.Value].ViewAccess[
                                             customEntityRelatedID.Value].CanView == true
                                             || accessRole.CustomEntityAccess[customEntityID.Value]
                                                    .ViewAccess[customEntityRelatedID.Value].CanAdd == true
                                             || accessRole.CustomEntityAccess[customEntityID.Value]
                                                    .ViewAccess[customEntityRelatedID.Value].CanEdit == true
                                             || accessRole.CustomEntityAccess[customEntityID.Value]
                                                    .ViewAccess[customEntityRelatedID.Value].CanDelete
                                             == true))
                                {
                                    return true;
                                }
                            }
                        }

                        #endregion custom entity access role check

                        if ((checkIfDDelegate == true && delegatetype.HasValue != null && delegatetype == 1)
                            || (accessRole.ElementAccess != null
                                && accessRole.ElementAccess.ContainsKey(element) == true
                                && element != SpendManagementElement.CustomEntityInstances
                                && ((type == AccessRoleType.Delete
                                     && accessRole.ElementAccess[element].CanDelete == true)
                                    || (type == AccessRoleType.Add && accessRole.ElementAccess[element].CanAdd == true)
                                    || (type == AccessRoleType.Edit && accessRole.ElementAccess[element].CanEdit == true)
                                    || (type == AccessRoleType.View
                                        && (accessRole.ElementAccess[element].CanView == true
                                            || accessRole.ElementAccess[element].CanAdd == true
                                            || accessRole.ElementAccess[element].CanDelete == true
                                            || accessRole.ElementAccess[element].CanEdit == true)))))
                        {
                            if (checkIfDDelegate == true && delegatetype.HasValue)
                            {
                                cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(AccountID);
                                cAccountProperties reqProperties =
                                    clsSubAccounts.getFirstSubAccount().SubAccountProperties;

                                if (delegatetype == 2 && reqProperties.EnableDelegateOptionsForDelegateAccessRole == false)
                                {
                                    return true;
                                }
                                else if (delegatetype == 1 || (delegatetype == 2 && reqProperties.EnableDelegateOptionsForDelegateAccessRole == true))
                                {
                                    if (element == SpendManagementElement.QuickEntryForms
                                        && reqProperties.DelQEDesign == true)
                                    {
                                        return true;
                                    }
                                    else if ((element == SpendManagementElement.UserDefinedFields
                                              || element == SpendManagementElement.BroadcastMessages
                                              || element == SpendManagementElement.Colours
                                              || element == SpendManagementElement.CompanyDetails
                                              || element == SpendManagementElement.CompanyLogo
                                              || element == SpendManagementElement.EmailSuffixes
                                              || element == SpendManagementElement.FlagsAndLimits
                                              || element == SpendManagementElement.GeneralOptions
                                              || element == SpendManagementElement.EmailServer
                                              || element == SpendManagementElement.MainAdministrator
                                              || element == SpendManagementElement.PasswordOptions
                                              || element == SpendManagementElement.PrintOut
                                              || element == SpendManagementElement.RegionalSettings
                                              || element == SpendManagementElement.ExpenseItems
                                              || element == SpendManagementElement.CostCodes
                                              || element == SpendManagementElement.Countries
                                              || element == SpendManagementElement.Currencies
                                              || element == SpendManagementElement.Locations
                                              || element == SpendManagementElement.Reasons
                                              || element == SpendManagementElement.ProjectCodes
                                              || element == SpendManagementElement.P11D
                                              || element == SpendManagementElement.Departments
                                              || element == SpendManagementElement.VehicleJourneyRateCategories
                                              || element == SpendManagementElement.Allowances
                                              || element == SpendManagementElement.ExpenseCategories
                                              || element == SpendManagementElement.PoolCars
                                              || element == SpendManagementElement.Workflows
                                              || element == SpendManagementElement.DefaultView
                                              || element == SpendManagementElement.DefaultPrintView
                                              || element == SpendManagementElement.Emails
                                              || element == SpendManagementElement.CompanyDetails
                                              || element == SpendManagementElement.FilterRules
                                              || element == SpendManagementElement.EnvelopeManagement
                                              || element == SpendManagementElement.CompanyPolicy)
                                             && reqProperties.DelSetup == true)
                                    {
                                        return true;
                                    }
                                    else if ((element == SpendManagementElement.BudgetHolders
                                              || element == SpendManagementElement.Employees
                                              || element == SpendManagementElement.Groups
                                              || element == SpendManagementElement.AccessRoles
                                              || element == SpendManagementElement.SignOffGroups
                                              || element == SpendManagementElement.Teams
                                              || element == SpendManagementElement.CarDocuments
                                              || element == SpendManagementElement.GeneralOptions
                                              || element == SpendManagementElement.EnvelopeManagement
                                              || element == SpendManagementElement.ItemRoles)
                                             && reqProperties.DelEmployeeAdmin == true)
                                    {
                                        return true;
                                    }
                                    else if ((element == SpendManagementElement.CheckAndPay)
                                             && reqProperties.DelCheckAndPay == true)
                                    {
                                        return true;
                                    }
                                    else if ((element == SpendManagementElement.Advances)
                                             && reqProperties.DelApprovals == true)
                                    {
                                        return true;
                                    }
                                    else if ((element == SpendManagementElement.AuditLog)
                                             && reqProperties.DelAuditLog == true)
                                    {
                                        return true;
                                    }
                                    else if (element == SpendManagementElement.ClaimantReports
                                             && reqProperties.DelReportsClaimants)
                                    {
                                        return true;
                                    }
                                    else if (element == SpendManagementElement.Reports
                                             && reqProperties.DelReports)
                                    {
                                        return true;
                                    }
                                    else if (element
                                             == SpendManagementElement.FinancialExports
                                             && reqProperties.DelExports)
                                    {
                                        return true;
                                    }
                                    else if (element
                                             == SpendManagementElement.CorporateCards
                                             && reqProperties.DelCorporateCards)
                                    {
                                        return true;
                                    }
                                    else if (element 
                                        == SpendManagementElement.MobileDevices
                                         && reqProperties.UseMobileDevices)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                result = true;
                            }

                        }
                    }
                }
            }

            if (autoRedirectToHome == true && result == false)
            {
                System.Web.HttpContext.Current.Response.Redirect(
                    "~/shared/restricted.aspx?reason=Current%20access%20role%20does%20not%20permit%20you%20to%20view%20this%20page.",
                    true);
            }

            return result;
        }

        #region Properties

        public cAccount Account
        {
            get
            {
                if (clsAccount == null)
                {
                    var clsAccounts = new cAccounts();
                    clsAccount = clsAccounts.GetAccountByID(this.AccountID);

                }

                return clsAccount;
            }
        }

        #endregion

        /// <summary>
        /// Returns the highest AccessLevel found in all AccessRoles assigned to this employee. (for the current subaccount)
        /// </summary>
        public AccessRoleLevel HighestAccessLevel
        {
            get
            {
                Int16 highestAccessLevel = 0;
                var highestAccessLevelEnum = AccessRoleLevel.EmployeesResponsibleFor;

                if (this.EmployeeAccessRoles != null && this.EmployeeAccessRoles.Count > 0)
                {

                    foreach (cAccessRole accessRole in this.EmployeeAccessRoles)
                    {
                        if ((Int16)accessRole.AccessLevel > highestAccessLevel)
                        {
                            highestAccessLevel = (Int16)accessRole.AccessLevel;
                            this.nHighestAccessRoleID = accessRole.RoleID;
                            highestAccessLevelEnum = accessRole.AccessLevel;
                        }
                    }
                }

                return highestAccessLevelEnum;
            }
        }

        /// <summary>
        /// Returns the highest maximum claimable value a claim can be for all cAccessRoles
        /// </summary>
        public decimal? ExpenseClaimMaximumValue
        {
            get
            {
                decimal? maximumValue = null;

                if (this.EmployeeAccessRoles != null)
                {
                    foreach (cAccessRole accessRole in this.EmployeeAccessRoles)
                    {
                        if (maximumValue == null || accessRole.ExpenseClaimMaximumAmount > maximumValue)
                        {
                            maximumValue = accessRole.ExpenseClaimMaximumAmount;
                        }
                    }
                }

                return maximumValue;
            }
        }

        /// <summary>
        /// Returns the lowest claimable value a claim can be for all cAccessRoles
        /// </summary>
        public decimal? ExpenseClaimMinimumValue
        {
            get
            {
                decimal? maximumValue = null;
                
                if (this.EmployeeAccessRoles != null)
                {
                    foreach (cAccessRole accessRole in this.EmployeeAccessRoles)
                    {
                        if (maximumValue == null || accessRole.ExpenseClaimMinimumAmount > maximumValue)
                        {
                            maximumValue = accessRole.ExpenseClaimMinimumAmount;
                        }
                    }
                }

                return maximumValue;
            }
        }

        /// <summary>
        /// Generates additional SQL forming part of a where clause when SQL queries need to use this users AccessRole's to determine what data can be used
        /// </summary>
        /// <returns></returns>
        public string GetAccessRoleWhereClause()
        {
            System.Text.StringBuilder sbSQL = new System.Text.StringBuilder();

            switch (this.HighestAccessLevel)
            {
                case AccessRoleLevel.EmployeesResponsibleFor:

                    #region workout who this employee is responsible for - uses dbo.resolveReport function

                    DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.AccountID));
                    System.Data.SqlClient.SqlDataReader reader;
                    string strsql;
                    strsql = "select distinct employeeid from dbo.resolveReport(" + this.EmployeeID + ",'')";
                    using (reader = expdata.GetReader(strsql))
                    {
                        while (reader.Read())
                        {
                            sbSQL.Append(reader.GetInt32(0) + ",");
                        }
                        reader.Close();
                    }
                    if (sbSQL.Length != 0)
                    {
                        sbSQL.Remove(sbSQL.Length - 1, 1);
                        sbSQL = new System.Text.StringBuilder("employees.employeeid in (" + sbSQL.ToString() + ")");
                    }
                    break;

                    #endregion

                case AccessRoleLevel.SelectedRoles:

                    #region work out accessRoleLinks

                    sbSQL.Append("(");

                    if (this.EmployeeAccessRoles != null)
                    {
                        foreach (cAccessRole accessRole in this.EmployeeAccessRoles)
                        {
                            foreach (int accessRoleID in accessRole.AccessRoleLinks)
                            {
                                sbSQL.Append("(employeeAccessRoles.employeeID = employees.employeeid AND employeeAccessRoles.accessRoleID = ").Append(accessRoleID).Append(") or ");
                            }

                            sbSQL.Append("(employeeAccessRoles.employeeID = employees.employeeid AND employeeAccessRoles.accessRoleID = ").Append(accessRole.RoleID).Append(") or ");
                        }
                    }

                    sbSQL.Append(" 1 = 0)");
                    break;

                    #endregion

                case AccessRoleLevel.AllData:
                    break;
                default:
                    throw new Exception("Unhandled AccessRoleLevel enum value (" + this.HighestAccessLevel + ")");

            }
            return sbSQL.ToString();
        }

        public bool CanEditDepartments
        {
            get
            {
                if (this.EmployeeAccessRoles != null)
                {
                    return this.EmployeeAccessRoles.Any(accessRole => accessRole.CanEditDepartment == true);
                }

                return false;
            }
        }

        public bool CanEditProjectCodes
        {
            get
            {
                if (this.EmployeeAccessRoles != null)
                {
                    return this.EmployeeAccessRoles.Any(accessRole => accessRole.CanEditProjectCode == true);
                }

                return false;
            }
        }

        public bool CanEditCostCodes
        {
            get
            {
                if (this.EmployeeAccessRoles != null)
                {
                    return this.EmployeeAccessRoles.Any(accessRole => accessRole.CanEditCostCode == true);
                }

                return false;
            }
        }

        /// <summary>
        /// Whether the Employee requires a Bank Account to claim for Expenses.
        /// </summary>
        public bool MustHaveBankAccount
        {
            get
            {
                if (this.EmployeeAccessRoles != null)
                {
                    return this.EmployeeAccessRoles.Any(accessRole => accessRole.MustHaveBankAccount == true);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the CurrentUserInfo javascript variable as a script for use on product pages.
        /// </summary>
        public string CurrentUserInfoJavascriptVariable
        {
            get
            {
                var currentUserInfo = new StringBuilder();
                currentUserInfo.AppendLine();
                currentUserInfo.AppendLine();
                currentUserInfo.AppendLine("var CurrentUserInfo = {");
                currentUserInfo.AppendLine(string.Format("'AccountID':{0},", this.AccountID));
                currentUserInfo.AppendLine("'SubAccountID':" + this.CurrentSubAccountId + ",");
                currentUserInfo.AppendLine(string.Format("'EmployeeID':{0},", this.EmployeeID));
                currentUserInfo.AppendLine(string.Format("'IsDelegate':{0},", this.isDelegate.ToString().ToLower()));
                currentUserInfo.AppendLine(
                    string.Format("'DelegateEmployeeID':{0},", this.isDelegate ? this.Delegate.EmployeeID : 0));
                currentUserInfo.AppendLine(
                    string.Format(
                        "'Module': {{'ID':{0}, 'Name':'{1}'}}",
                        (int)this.CurrentActiveModule,
                        this.CurrentActiveModule));
                currentUserInfo.AppendLine("};");
                string javascript =
                    string.Format(
                        "<script language=\"javascript\" type=\"text/javascript\">{0}{1}</script>",
                        Environment.NewLine,
                        currentUserInfo);

                return javascript;
            }
        }

        /// <summary>
        /// Check the user has access to the product via the website.
        /// </summary>
        /// <returns>
        /// True, if user has access.
        /// </returns>
        public bool CheckUserHasAccesstoWebsite()
        {
            if (this.Employee != null && this.Employee.AdminOverride)
            {
                return true;
            }

            if (this.Employee != null)
            {
                if (this.EmployeeAccessRoles != null && this.EmployeeAccessRoles.Count > 0)
                {
                    return this.EmployeeAccessRoles.Any(val => val.AllowWebsiteAccess);
                }
            }

            return false;
        }

        private static IEnumerable<SpendManagementElement> GetBypassedElements()
        {
            var bypassed = new List<SpendManagementElement> { SpendManagementElement.Claims };

            return bypassed;
        }
    }
    
    public interface ICurrentUser : ICurrentUserBase
    {
        bool CheckAccessRoleApi(SpendManagementElement element, AccessRoleType type, AccessRequestType requestType);

        // additional to ICurrentUser
        bool CheckAccessRole(AccessRoleType type, SpendManagementElement element, bool checkIfDDelegate);
        bool CheckAccessRole(AccessRoleType type, CustomEntityElementType customEntityElementType, int customEntityID, int customEntityRelatedID, bool redirectToErrorHome, AccessRequestType requestType = AccessRequestType.Website);
        bool CheckAccessRole(AccessRoleType type, SpendManagementElement element, bool checkIfDDelegate, bool autoRedirectToHome);
        bool CheckAccessRole(AccessRoleType type, SpendManagementElement element, bool checkIfDDelegate, bool autoRedirectToHome, CustomEntityElementType customEntityElementType, int? customEntityID, int? customEntityRelatedID, AccessRequestType requestType = AccessRequestType.Website);

        bool CheckUserHasAccesstoWebsite();
        cAccount Account { get; }
        AccessRoleLevel HighestAccessLevel { get; }
        decimal? ExpenseClaimMaximumValue { get; }
        decimal? ExpenseClaimMinimumValue { get; }
        string GetAccessRoleWhereClause();
        bool CanEditDepartments { get; }
        bool CanEditProjectCodes { get; }
        bool CanEditCostCodes { get; }

        /// <summary>
        /// Gets a value indicating whether the employee must have a bank account to claim for an expense 
        /// </summary>
        bool MustHaveBankAccount { get; }
    }
    
    /// <summary>
    /// Defines an item that requires a CurrentUSer object.
    /// </summary>
    public interface IRequireCurrentUser
    {
        /// <summary>
        /// The Current User
        /// </summary>
        ICurrentUser CurrentUser { get; set; }
    }
}
