namespace BusinessLogic.GeneralOptions.VatCalculation
{
    /// <summary>
    /// Defines a <see cref="IVatCalculationOptions"/> and it's members
    /// </summary>
    public interface IVatCalculationOptions
    {
        /// <summary>
        /// Gets or sets the enable calculations for allocating fuel receipt VAT to mileage
        /// </summary>
        bool EnableCalculationsForAllocatingFuelReceiptVatToMileage { get; set; }
    }
}
