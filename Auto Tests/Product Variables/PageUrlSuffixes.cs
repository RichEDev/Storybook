using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auto_Tests.Product_Variables.PageUrlSurfices
{
    internal class EmployeeUrlSuffixes
    {
        #region url suffixes relating to employees
        /// <summary>
        /// Url suffix to edit an Employee 
        /// </summary>
        internal const string EditEmployeeUrl = "/shared/admin/aeemployee.aspx?employeeid=";
        /// <summary>
        /// Url suffix to search employees 
        /// </summary>
        internal const string SearchEmployeeUrl = "/shared/admin/selectemployee.aspx";
        #endregion
    }

    internal class GreenlightUrlSuffixes
    {
        #region url suffixes relating to greenlight
        /// <summary>
        /// Suffix to navigate to greenlight administration
        /// </summary>
        internal const string GreenlightAdministrationUrl = "/shared/admin/custom_entities.aspx";
        /// <summary>
        /// Url suffix to edit a greenlight 
        /// </summary>
        internal const string EditGreenlightUrl = "/shared/admin/aecustomentity.aspx?entityid=";
        #endregion
    }

    internal class NHSTustUrlSuffixes
    {
        #region url suffixes relating to trusts
        /// <summary>
        /// Suffix to navigate to NHS Tust
        /// </summary>
        internal const string NHSTustUrl = "/expenses/nhs/trusts.aspx";
        #endregion
    }

    internal class FinancialExportsUrlSuffixes
    {
        #region url suffixes relating to financial exports
        /// <summary>
        /// Suffix to navigate to financial exports
        /// </summary>
        internal const string FinancialExportUrl = "/expenses/admin/financialexports.aspx";
        #endregion
    }

    internal class BudgetHoldersUrlSuffixes
    {
        #region url suffixes relating to budget holder
        /// <summary>
        /// Suffix to navigate to budget holders
        /// </summary>
        internal const string BudgetHolderAdministrationUrl = "/shared/admin/adminbudget.aspx";
        #endregion
    }

    internal class TeamUrlSuffixes
    {
        #region url suffixes relating to teams
        /// <summary>
        /// The team url.
        /// </summary>
        internal const string TeamUrl = "/shared/admin/adminteams.aspx";
        #endregion
    }

    internal class ApprovalMatricesUrlSuffixes
    {
        #region url suffixes relating to approval matrices
        /// <summary>
        /// The approval matrices url.
        /// </summary>
        internal const string ApprovalMatricesUrl = "/shared/admin/ApprovalMatrices.aspx";
        #endregion
    }

    internal class UserDefinedFieldsUrlSuffixes
    {
        #region url suffixes relating to user defined fields
        /// <summary>
        /// The user defined fields url.
        /// </summary>
        internal const string UserDefinedFieldsUrl = "/shared/admin/adminuserdefined.aspx";
        #endregion
    }

    internal class CostCodesUrlSuffixes
    {
        #region url suffixes relating to cost codes
        /// <summary>
        /// The cost codes url.
        /// </summary>
        internal const string CostCodesUrl = "/shared/admin/admincostcodes.aspx";
        #endregion
    }
}
