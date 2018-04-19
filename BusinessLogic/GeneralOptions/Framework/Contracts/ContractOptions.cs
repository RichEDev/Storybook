namespace BusinessLogic.GeneralOptions.Framework.Contracts
{
    /// <summary>
    /// Defines a <see cref="ContractOptions"/> and it's members
    /// </summary>
    public class ContractOptions : IContractOptions
    {
        /// <summary>
        /// Gets or sets the auto update licence total
        /// </summary>
        public bool AutoUpdateLicenceTotal { get; set; }

        /// <summary>
        /// Gets or sets the recharge unrecovered title
        /// </summary>
        public string RechargeUnrecoveredTitle { get; set; }

        /// <summary>
        /// Gets or sets the auto update CV recharge live
        /// </summary>
        public bool AutoUpdateCVRechargeLive { get; set; }

        /// <summary>
        /// Gets or sets the penalty clause title
        /// </summary>
        public string PenaltyClauseTitle { get; set; }

        /// <summary>
        /// Gets or sets the contract schedule default
        /// </summary>
        public string ContractScheduleDefault { get; set; }

        /// <summary>
        /// Gets or sets the use CP extra info
        /// </summary>
        public bool UseCPExtraInfo { get; set; }

        /// <summary>
        /// Gets or set the enable contract savings
        /// </summary>
        public bool EnableContractSavings { get; set; }

        /// <summary>
        /// Gets or sets the enable recharge
        /// </summary>
        public bool EnableRecharge { get; set; }

        /// <summary>
        /// Gets or sets the contract dates mandatory
        /// </summary>
        public bool ContractDatesMandatory { get; set; }

        /// <summary>
        /// Gets or sets the contract key
        /// </summary>
        public string ContractKey { get; set; }

        /// <summary>
        /// Gets or sets the auto update annual contract value
        /// </summary>
        public bool AutoUpdateAnnualContractValue { get; set; }

        /// <summary>
        /// Gets or sets the contract category title
        /// </summary>
        public string ContractCategoryTitle { get; set; }

        /// <summary>
        /// Gets or sets the inflator active
        /// </summary>
        public bool InflatorActive { get; set; }

        /// <summary>
        /// Gets or sets the invoice freq active
        /// </summary>
        public bool InvoiceFreqActive { get; set; }

        /// <summary>
        /// Gets or sets the term type active
        /// </summary>
        public bool TermTypeActive { get; set; }

        /// <summary>
        /// Gets or sets the value comments
        /// </summary>
        public string ValueComments { get; set; }

        /// <summary>
        /// Gets or sets the contract description title
        /// </summary>
        public string ContractDescTitle { get; set; }

        /// <summary>
        /// Gets or sets the contract description short title
        /// </summary>
        public string ContractDescShortTitle { get; set; }

        /// <summary>
        /// Gets or sets the contract number gen
        /// </summary>
        public bool ContractNumGen { get; set; }

        /// <summary>
        /// Gets or sets the contract number sequence
        /// </summary>
        public int ContractNumSeq { get; set; }

        /// <summary>
        /// Gets or sets the contract category mandatory
        /// </summary>
        public bool ContractCatMandatory { get; set; }

        /// <summary>
        /// Gets or set the enable flashing icon
        /// </summary>
        public bool EnableFlashingNotesIcon { get; set; }

        /// <summary>
        /// Gets or sets the enable contract num update
        /// </summary>
        public bool EnableContractNumUpdate { get; set; }

        /// <summary>
        /// Gets or sets the allow menu contract add
        /// </summary>
        public bool AllowMenuContractAdd { get; set; }
    }
}
