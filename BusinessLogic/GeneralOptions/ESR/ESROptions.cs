namespace BusinessLogic.GeneralOptions.ESR
{
    /// <summary>
    /// Defines a <see cref="ESROptions"/> and it's members
    /// </summary>
    public class ESROptions : IESROptions
    {
        /// <summary>
        /// Gets or sets the enable ESR diagnostics
        /// </summary>
        public bool EnableEsrDiagnostics { get; set; }

        /// <summary>
        /// Gets or set the <see cref="AutoActivateType"/>
        /// </summary>
        public AutoActivateType AutoActivateType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AutoArchiveType"/>
        /// </summary>
        public AutoArchiveType AutoArchiveType { get; set; }

        /// <summary>
        /// Gets or sets the archive grace period
        /// </summary>
        public short ArchiveGracePeriod { get; set; }

        /// <summary>
        /// Gets or sets the import username format
        /// </summary>
        public string ImportUsernameFormat { get; set; }

        /// <summary>
        /// Gets or sets the import home address format
        /// </summary>
        public string ImportHomeAddressFormat { get; set; }

        /// <summary>
        /// Gets or sets the check ESR assignment on employee add
        /// </summary>
        public bool CheckESRAssignmentOnEmployeeAdd { get; set; }

        /// <summary>
        /// Gets or sets the ESR auto activate car
        /// </summary>
        public bool EsrAutoActivateCar { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EsrRoundingType"/>
        /// </summary>
        public EsrRoundingType EsrRounding { get; set; }

        /// <summary>
        /// Gets or sets the summary ESR inbound file
        /// </summary>
        public bool SummaryEsrInboundFile { get; set; }

        /// <summary>
        /// Gets or sets the enable ESR manual assignment supervisor
        /// </summary>
        public bool EnableESRManualAssignmentSupervisor { get; set; }

        /// <summary>
        /// Gets or sets the ESR primary address only
        /// </summary>
        public bool EsrPrimaryAddressOnly { get; set; }
    }
}
