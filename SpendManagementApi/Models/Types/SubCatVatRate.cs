namespace SpendManagementApi.Models.Types
{
    using System;
    using SpendManagementLibrary;
    using DateRangeType = SpendManagementApi.Common.Enums.DateRangeType;

    /// <summary>
    /// Links an <see cref="ExpenseSubCategory">ExpenseSubCategory</see> to a <see cref="VatRate">VAT Rate</see>.
    /// </summary>
    public class SubCatVatRate : BaseExternalType, IEquatable<SubCatVatRate>
    {
        /// <summary>
        /// The related Vat Rate Id.
        /// </summary>
        public int VatRateId { get; internal set; }

        /// <summary>
        /// The related Sub Category Id.
        /// </summary>
        public int SubCatId { get;  set; }

        /// <summary>
        /// The VAT Amount for this SubCatVatRate.
        /// </summary>
        public double VatAmount { get; set; }

        /// <summary>
        /// Whether this has a receipt.
        /// </summary>
        public bool VatReceipt { get; set; }

        /// <summary>
        /// The VAT limit without receipt.
        /// </summary>
        public decimal? VatLimitWithout { get; set; }

        /// <summary>
        /// The VAT limit with receipt.
        /// </summary>
        public decimal? VatLimitWith { get; set; }

        /// <summary>
        /// The VAT percentage.
        /// </summary>
        public byte VatPercent { get; set; }

        /// <summary>
        /// The DateRangeType for this SubCatVatRate.
        /// </summary>
        public DateRangeType RangeType { get; set; }

        /// <summary>
        /// The first date value for this SubCatVatRate.
        /// </summary>
        public DateTime? DateValue1 { get; set; }

        /// <summary>
        /// 
        /// The second date value for this SubCatVatRate.
        /// </summary>
        public DateTime? DateValue2 { get; set; }

        /// <summary>
        /// Whether to mark this for delete in a bulk update.
        /// </summary>
        public bool ForDelete { internal get; set; }

        internal bool Validate()
        {
            switch (this.RangeType)
            {
                case DateRangeType.AfterOrEqualTo:
                case DateRangeType.Before:
                    if (DateValue1 == null)
                        return false;
                    break;
                    
                case DateRangeType.Between:
                    if (DateValue1 == null || DateValue2 == null)
                        return false;
                    break;
                    
                default:
                    return true;
            }
            return true;
        }

        public bool Equals(SubCatVatRate other)
        {
            if (other == null)
            {
                return false;
            }
            return this.RangeType.Equals(other.RangeType) &&
                this.DateCompare(this.DateValue1, other.DateValue1) && 
                this.DateCompare(this.DateValue2, other.DateValue2) && 
                this.VatAmount.Equals(other.VatAmount) &&
                this.VatLimitWith.Equals(other.VatLimitWith) &&
                this.VatLimitWithout.Equals(other.VatLimitWithout) &&
                this.VatPercent.Equals(other.VatPercent) &&
                this.VatReceipt.Equals(other.VatReceipt);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SubCatVatRate);
        }
    }

    internal static class SubCatVatRateExtension
    {
        internal static TResult Cast<TResult>(this cSubcatVatRate cSubcatVatRate)
            where TResult : SubCatVatRate, new()
        {
            if (cSubcatVatRate == null)
                return null;
            return new TResult
                {
                    RangeType = (DateRangeType)Enum.Parse(typeof(DateRangeType), cSubcatVatRate.daterangetype.ToString()),
                    DateValue1 = cSubcatVatRate.datevalue1,
                    DateValue2 = cSubcatVatRate.datevalue2,
                    SubCatId = cSubcatVatRate.subcatid,
                    VatAmount = cSubcatVatRate.vatamount,
                    VatLimitWith = cSubcatVatRate.vatlimitwith,
                    VatLimitWithout = cSubcatVatRate.vatlimitwithout,
                    VatPercent = cSubcatVatRate.vatpercent,
                    VatRateId = cSubcatVatRate.vatrateid,
                    VatReceipt = cSubcatVatRate.vatreceipt
                };
        }

        internal static cSubcatVatRate Cast(this SubCatVatRate subCatVatRate)
        {
            if (subCatVatRate == null)
                return null;
            return new cSubcatVatRate(
                subCatVatRate.VatRateId, 
                subCatVatRate.SubCatId, 
                subCatVatRate.VatAmount, 
                subCatVatRate.VatReceipt, 
                subCatVatRate.VatLimitWithout, 
                subCatVatRate.VatLimitWith, 
                subCatVatRate.VatPercent, 
                (SpendManagementLibrary.DateRangeType)Enum.Parse(typeof(SpendManagementLibrary.DateRangeType), subCatVatRate.RangeType.ToString()), 
                subCatVatRate.DateValue1, 
                subCatVatRate.DateValue2);
        }
    }
}