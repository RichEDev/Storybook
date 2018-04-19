namespace BusinessLogic.GeneralOptions.Framework.Suppliers
{
    /// <summary>
    /// Defines a <see cref="SupplierOptions"/> and it's members
    /// </summary>
    public class SupplierOptions : ISupplierOptions
    {
        /// <summary>
        /// Gets or sets the supplier region title
        /// </summary>
        public string SupplierRegionTitle { get; set; }

        /// <summary>
        /// Gets or sets the supplier primary title
        /// </summary>
        public string SupplierPrimaryTitle { get; set; }

        /// <summary>
        /// Gets or sets the supplier variation title
        /// </summary>
        public string SupplierVariationTitle { get; set; }

        /// <summary>
        /// Gets or sets the supplier category mandatory
        /// </summary>
        public bool SupplierCatMandatory { get; set; }

        /// <summary>
        /// Gets or sets the supplier status enforced
        /// </summary>
        public bool SupplierStatusEnforced { get; set; }

        /// <summary>
        /// Gets or sets the supplier last finish status enabled
        /// </summary>
        public bool SupplierLastFinStatusEnabled { get; set; }

        /// <summary>
        /// Gets or sets the supplier last finish check enabled
        /// </summary>
        public bool SupplierLastFinCheckEnabled { get; set; }

        /// <summary>
        /// Gets or sets the supplier financial year end enabled
        /// </summary>
        public bool SupplierFYEEnabled { get; set; }

        /// <summary>
        /// Gets or sets the supplier number employees enabled
        /// </summary>
        public bool SupplierNumEmployeesEnabled { get; set; }

        /// <summary>
        /// Gets or sets the supplier turnover enabled
        /// </summary>
        public bool SupplierTurnoverEnabled { get; set; }

        /// <summary>
        /// Gets or sets the supplier Int contact enabled
        /// </summary>
        public bool SupplierIntContactEnabled { get; set; }

        /// <summary>
        /// Gets or sets the supplier category title
        /// </summary>
        public string SupplierCatTitle { get; set; }
    }
}
