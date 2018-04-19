namespace BusinessLogic.GeneralOptions.Framework.Contracts
{
    /// <summary>
    /// Defines a <see cref="IContractOptions"/> and it's members
    /// </summary>
    public interface IContractOptions
    {
        /// <summary>
        /// Gets or sets the auto update licence total
        /// </summary>
        bool AutoUpdateLicenceTotal { get; set; }

        /// <summary>
        /// Gets or sets the recharge unrecovered title
        /// </summary>
        string RechargeUnrecoveredTitle { get; set; }

        /// <summary>
        /// Gets or sets the auto update CV recharge live
        /// </summary>
        bool AutoUpdateCVRechargeLive { get; set; }

        /// <summary>
        /// Gets or sets the penalty clause title
        /// </summary>
        string PenaltyClauseTitle { get; set; }

        /// <summary>
        /// Gets or sets the contract schedule default
        /// </summary>
        string ContractScheduleDefault { get; set; }

        /// <summary>
        /// Gets or sets the use CP extra info
        /// </summary>
        bool UseCPExtraInfo { get; set; }

        /// <summary>
        /// Gets or set the enable contract savings
        /// </summary>
        bool EnableContractSavings { get; set; }

        /// <summary>
        /// Gets or sets the enable recharge
        /// </summary>
        bool EnableRecharge { get; set; }

        /// <summary>
        /// Gets or sets the contract dates mandatory
        /// </summary>
        bool ContractDatesMandatory { get; set; }

        /// <summary>
        /// Gets or sets the contract key
        /// </summary>
        string ContractKey { get; set; }

        /// <summary>
        /// Gets or sets the auto update annual contract value
        /// </summary>
        bool AutoUpdateAnnualContractValue { get; set; }

        /// <summary>
        /// Gets or sets the contract category title
        /// </summary>
        string ContractCategoryTitle { get; set; }

        /// <summary>
        /// Gets or sets the inflator active
        /// </summary>
        bool InflatorActive { get; set; }

        /// <summary>
        /// Gets or sets the invoice freq active
        /// </summary>
        bool InvoiceFreqActive { get; set; }

        /// <summary>
        /// Gets or sets the term type active
        /// </summary>
        bool TermTypeActive { get; set; }

        /// <summary>
        /// Gets or sets the value comments
        /// </summary>
        string ValueComments { get; set; }

        /// <summary>
        /// Gets or sets the contract description title
        /// </summary>
        string ContractDescTitle { get; set; }

        /// <summary>
        /// Gets or sets the contract description short title
        /// </summary>
        string ContractDescShortTitle { get; set; }

        /// <summary>
        /// Gets or sets the contract number gen
        /// </summary>
        bool ContractNumGen { get; set; }

        /// <summary>
        /// Gets or sets the contract number sequence
        /// </summary>
        int ContractNumSeq { get; set; }

        /// <summary>
        /// Gets or sets the contract category mandatory
        /// </summary>
        bool ContractCatMandatory { get; set; }

        /// <summary>
        /// Gets or set the enable flashing icon
        /// </summary>
        bool EnableFlashingNotesIcon { get; set; }

        /// <summary>
        /// Gets or sets the enable contract num update
        /// </summary>
        bool EnableContractNumUpdate { get; set; }

        /// <summary>
        /// Gets or sets the allow menu contract add
        /// </summary>
        bool AllowMenuContractAdd { get; set; }
    }
}
