namespace SpendManagementLibrary
{
    using System;

    using SpendManagementLibrary.Employees;

    [Serializable()]
    public abstract class cCurrentUserBase : ICurrentUserBase
    {
        protected int nAccountid;
        protected int nEmployeeid;
        protected Lazy<Employee> clsEmployee;
        protected cAccount clsAccount;
        [NonSerialized()]
        public Lazy<Employee> clsDelegate;
        protected int nActiveSubAccountId;
        protected Modules nActiveModuleId;
        protected int nHighestAccessRoleID;
        

        #region properties

        /// <summary>
        /// Currently Active SubAccount
        /// </summary>
        public int CurrentSubAccountId
        {
            get
            {
                if (this.nActiveSubAccountId < 0)
                {
                    this.nActiveSubAccountId = Employee?.DefaultSubAccount ?? new cAccountSubAccountsBase(this.AccountID).getFirstSubAccount().SubAccountID;
                }

                return nActiveSubAccountId;
            }
            set { nActiveSubAccountId = value; }
        }

        /// <summary>
        /// Account ID
        /// </summary>
        public int AccountID
        {
            get { return nAccountid; }
            set { nAccountid = value; }
        }

        /// <summary>
        /// Employee ID
        /// </summary>
        public int EmployeeID
        {
            get { return nEmployeeid; }
            set { nEmployeeid = value; }
        }

        /// <summary>
        /// Employee object for the currently active user
        /// </summary>
        public Employee Employee
        {
            get
            {
                return this.clsEmployee.Value;
            }
            set
            {
            }
        }

        public object EmployeeUncast { get; set; }

        /// <summary>
        /// cEmployee object of the user logging in as a delegate
        /// </summary>
        public Employee Delegate
        {
            get { return clsDelegate.Value; }
        }

        /// <summary>
        /// Is the current user account being used by someone it was delegated to
        /// </summary>
        public bool isDelegate
        {
            get
            {
                if (clsDelegate.Value == null)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Spend Management Module currently being used
        /// </summary>
        public Modules CurrentActiveModule
        {
            get { return nActiveModuleId; }
            set { nActiveModuleId = value; }
        }

        /// <summary>
        /// Returns the relevant Support Portal Product name that matches the Current Module
        /// </summary>
        public string CurrentSupportPortalProductName
        {
            get
            {
                string helpProdName = string.Empty;
                // select the appropriate support portal product name to match the current spend management module to
                switch (this.CurrentActiveModule)
                {
                    case Modules.contracts:
                        helpProdName = "framework";
                        break;
                    case Modules.expenses:
                        helpProdName = "expenses";
                        break;
                    default:
                        helpProdName = "none";
                        break;
                }
                return helpProdName;
            }
            set { }
        }


        #endregion properties
    }


    public interface ICurrentUserBase
    {
        int CurrentSubAccountId { get; set; }
        int AccountID { get; set; }
        int EmployeeID { get; set; }
        Employee Employee { get; set; }
        object EmployeeUncast { get; set; }
        Employee Delegate { get; }
        bool isDelegate { get; }
        Modules CurrentActiveModule { get; set; }
        string CurrentSupportPortalProductName { get; set; }
    }
}
