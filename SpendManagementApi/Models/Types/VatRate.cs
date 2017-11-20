namespace SpendManagementApi.Models.Types
{
    using System;

    /// <summary>
    /// Defines a rate of VAT (value added tax).
    /// </summary>
    public class VatRate : BaseExternalType, IEquatable<VatRate>
    {
        /// <summary>
        /// The Id of the ExpenseSubCategory.
        /// </summary>
        public int ExpenseSubCategoryId { get; set; }

        /// <summary>
        /// The VAT rate itself.
        /// </summary>
        public double Vat { get; set; }

        /// <summary>
        /// The VAT percentage.
        /// </summary>
        public double VatPercent { get; set; }

        /// <summary>
        /// Whether to delete this item during a bulk update.
        /// </summary>
        public bool ForDelete { internal get; set; }


        public bool Equals(VatRate other)
        {
            if (other == null)
            {
                return false;
            }
            return this.ExpenseSubCategoryId.Equals(other.ExpenseSubCategoryId) && this.Vat.Equals(other.Vat)
                   && this.VatPercent.Equals(other.VatPercent);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as VatRate);
        }
    }
}