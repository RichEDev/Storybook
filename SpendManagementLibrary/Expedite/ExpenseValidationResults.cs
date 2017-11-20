
namespace SpendManagementLibrary.Expedite
{
    using System.Collections.Generic;

    /// <summary>
    /// A class to hold the expense validation results
    /// </summary>
    public class ExpenseValidationResults
    {
        /// <summary>
        /// Gets or sets a list of <see cref="ValidationResult"/> pertaining to the business results
        /// </summary>
        public List<ValidationResult> BusinessResults { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="ValidationResult"/> pertaining to the VAT results
        /// </summary>
        public List<ValidationResult> VatResults { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="ValidationResult"/> pertaining to the fraud results
        /// </summary>
        public List<ValidationResult> FraudResults { get; set; }
    }
}
