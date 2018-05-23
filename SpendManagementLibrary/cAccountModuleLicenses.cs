namespace SpendManagementLibrary
{
    using System;

    using BusinessLogic.ProductModules;

    public class cAccountModuleLicenses
    {
        private int nModuleID;
        private int nAccountID;
        private int nMaxUsers;
        private DateTime dtExpiryDate;
        private IProductModule clsModule;


        /// <summary>
        /// Constructor for cAccountModuleLicenses
        /// </summary>
        public cAccountModuleLicenses(int moduleID, int accountID, int maxUsers, DateTime expiryDate, IProductModule module)
        {
            nModuleID = moduleID;
            nAccountID = accountID;
            nMaxUsers = maxUsers;
            dtExpiryDate = expiryDate;
            clsModule = module;
        }

        /// <summary>
        /// Returns an instance of cModule
        /// </summary>
        public IProductModule Module { get { return clsModule; } }

        /// <summary>
        /// Gets or sets the moduleID
        /// </summary>
        public int ModuleID { get { return nModuleID; } }

        /// <summary>
        /// Gets or sets the accountID
        /// </summary>
        public int AccountID { get { return nAccountID; } }

        /// <summary>
        /// Gets or sets the maximum users allowed for this module
        /// </summary>
        public int MaxUsers { get { return nMaxUsers; } }

        /// <summary>
        /// Gets or sets the expiry date
        /// </summary>
        public DateTime ExpiryDate { get { return dtExpiryDate; } }
    }
}
