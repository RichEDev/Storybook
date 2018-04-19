namespace BusinessLogic.GeneralOptions.Framework.Suppliers
{
    /// <summary>
    /// Defines a <see cref="ISupplierOptions"/> and it's members
    /// </summary>
    public interface ISupplierOptions
    {
        /// <summary>
        /// Gets or sets the supplier region title
        /// </summary>
        string SupplierRegionTitle { get; set; }

        /// <summary>
        /// Gets or sets the supplier primary title
        /// </summary>
        string SupplierPrimaryTitle { get; set; }

        /// <summary>
        /// Gets or sets the supplier variation title
        /// </summary>
        string SupplierVariationTitle { get; set; }

        /// <summary>
        /// Gets or sets the supplier category mandatory
        /// </summary>
        bool SupplierCatMandatory { get; set; }

        /// <summary>
        /// Gets or sets the supplier status enforced
        /// </summary>
        bool SupplierStatusEnforced { get; set; }

        /// <summary>
        /// Gets or sets the supplier last finish status enabled
        /// </summary>
        bool SupplierLastFinStatusEnabled { get; set; }

        /// <summary>
        /// Gets or sets the supplier last finish check enabled
        /// </summary>
        bool SupplierLastFinCheckEnabled { get; set; }

        /// <summary>
        /// Gets or sets the supplier financial year end enabled
        /// </summary>
        bool SupplierFYEEnabled { get; set; }

        /// <summary>
        /// Gets or sets the supplier number employees enabled
        /// </summary>
        bool SupplierNumEmployeesEnabled { get; set; }

        /// <summary>
        /// Gets or sets the supplier turnover enabled
        /// </summary>
        bool SupplierTurnoverEnabled { get; set; }

        /// <summary>
        /// Gets or sets the supplier Int contact enabled
        /// </summary>
        bool SupplierIntContactEnabled { get; set; }

        /// <summary>
        /// Gets or sets the supplier category title
        /// </summary>
        string SupplierCatTitle { get; set; }
    }
}
