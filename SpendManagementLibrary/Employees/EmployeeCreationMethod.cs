namespace SpendManagementLibrary.Employees
{
    using System;

    /// <summary>
    /// The method in which the employee was added to spend management
    /// </summary>
    [Serializable]
    public enum CreationMethod
    {
        /// <summary>
        /// Via add/edit expenses
        /// </summary>
        Manually = 0,

        /// <summary>
        /// Via the ESR Outbound import
        /// </summary>
        ESROutbound,

        /// <summary>
        /// Via the Excel implementation import
        /// </summary>
        ImplementationImport,

        /// <summary>
        /// Via the data import wizard
        /// </summary>
        ImportWizard,

        /// <summary>
        /// Via Self Registration
        /// </summary>
        SelfRegistration,

        /// <summary>
        /// Api
        /// </summary>
        Api
    }
}
