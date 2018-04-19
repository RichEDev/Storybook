namespace BusinessLogic.GeneralOptions.Framework.Recharge
{
    /// <summary>
    /// Defines a <see cref="IRechargeOptions"/> and it's members
    /// </summary>
    public interface IRechargeOptions
    {
        /// <summary>
        /// Gets or sets the reference as
        /// </summary>
        string ReferenceAs { get; set; }

        /// <summary>
        /// Gets or sets the staff rep as
        /// </summary>
        string StaffRepAs { get; set; }

        /// <summary>
        /// Gets or sets the recharge period
        /// </summary>
        int RechargePeriod { get; set; }

        /// <summary>
        /// Gets or sets the fin year commence
        /// </summary>
        int FinYearCommence { get; set; }

        /// <summary>
        /// Gets or sets the CP delete action
        /// </summary>
        int CPDeleteAction { get; set; }
    }
}
