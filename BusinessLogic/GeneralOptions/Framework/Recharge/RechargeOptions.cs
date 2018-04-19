namespace BusinessLogic.GeneralOptions.Framework.Recharge
{
    /// <summary>
    /// Defines a <see cref="RechargeOptions"/> and it's members
    /// </summary>
    public class RechargeOptions : IRechargeOptions
    {
        /// <summary>
        /// Gets or sets the reference as
        /// </summary>
        public string ReferenceAs { get; set; }

        /// <summary>
        /// Gets or sets the staff rep as
        /// </summary>
        public string StaffRepAs { get; set; }

        /// <summary>
        /// Gets or sets the recharge period
        /// </summary>
        public int RechargePeriod { get; set; }

        /// <summary>
        /// Gets or sets the fin year commence
        /// </summary>
        public int FinYearCommence { get; set; }

        /// <summary>
        /// Gets or sets the CP delete action
        /// </summary>
        public int CPDeleteAction { get; set; }
    }
}
