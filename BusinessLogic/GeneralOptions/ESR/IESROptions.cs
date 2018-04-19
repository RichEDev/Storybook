namespace BusinessLogic.GeneralOptions.ESR
{
    /// <summary>
    /// Defines a <see cref="IESROptions"/> and it's members
    /// </summary>
    public interface IESROptions
    {
        /// <summary>
        /// Gets or sets the enable ESR diagnostics
        /// </summary>
        bool EnableEsrDiagnostics { get; set; }

        /// <summary>
        /// Gets or set the <see cref="AutoActivateType"/>
        /// </summary>
        AutoActivateType AutoActivateType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AutoArchiveType"/>
        /// </summary>
        AutoArchiveType AutoArchiveType { get; set; }

        /// <summary>
        /// Gets or sets the archive grace period
        /// </summary>
        short ArchiveGracePeriod { get; set; }

        /// <summary>
        /// Gets or sets the import username format
        /// </summary>
        string ImportUsernameFormat { get; set; }

        /// <summary>
        /// Gets or sets the import home address format
        /// </summary>
        string ImportHomeAddressFormat { get; set; }

        /// <summary>
        /// Gets or sets the check ESR assignment on employee add
        /// </summary>
        bool CheckESRAssignmentOnEmployeeAdd { get; set; }

        /// <summary>
        /// Gets or sets the ESR auto activate car
        /// </summary>
        bool EsrAutoActivateCar { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EsrRoundingType"/>
        /// </summary>
        EsrRoundingType EsrRounding { get; set; }

        /// <summary>
        /// Gets or sets the summary ESR inbound file
        /// </summary>
        bool SummaryEsrInboundFile { get; set; }

        /// <summary>
        /// Gets or sets the enable ESR manual assignment supervisor
        /// </summary>
        bool EnableESRManualAssignmentSupervisor { get; set; }

        /// <summary>
        /// Gets or sets the ESR primary address only
        /// </summary>
        bool EsrPrimaryAddressOnly { get; set; }
    }
}
