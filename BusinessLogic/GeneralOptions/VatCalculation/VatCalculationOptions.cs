namespace BusinessLogic.GeneralOptions.VatCalculation
{
    /// <summary>
    /// Defines a <see cref="VatCalculationOptions"/> and it's members
    /// </summary>
    public class VatCalculationOptions : IVatCalculationOptions
    {
        /// <summary>
        /// Gets or sets the enable calculations for allocating fuel receipt VAT to mileage
        /// </summary>
        public bool EnableCalculationsForAllocatingFuelReceiptVatToMileage { get; set; }
    }
}
