namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// A simple structure to allow three important pieces of the subcat information to be represented.
    /// </summary>
    public class SubcatItemRoleBasic
    {
        /// <summary>
        /// The subcat id
        /// </summary>
        public int SubcatId { get; set; }

        /// <summary>
        /// The subcat name
        /// </summary>
        public string Subcat { get; set; }

        /// <summary>
        /// The Short Subcat Name
        /// </summary>
        public string ShortSubcat { get; set; }

        /// <summary>
        /// Maximum
        /// </summary>
        public decimal Maximum { get; set; }

        /// <summary>
        /// Receipt Maximum
        /// </summary>
        public decimal ReceiptMaximum { get; set; }

        /// <summary>
        /// CategoryId
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// The calculation type
        /// </summary>
        public CalculationType CalculationType { get; set; }

        /// <summary>
        /// From App
        /// </summary>
        public bool FromApp { get; set; }

        /// <summary>
        /// To App
        /// </summary>
        public bool ToApp { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The item role ID
        /// </summary>
        public int ItemRoleID { get; set; }

        /// <summary>
        /// The symbol of the currency
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// The Date this Item Role becomes valid.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The Date this Item Role expires.
        /// </summary>
        public DateTime EndDate { get; set; }

    }
}

