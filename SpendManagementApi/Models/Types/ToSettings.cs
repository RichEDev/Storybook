﻿namespace SpendManagementApi.Models.Types
{
    using Interfaces;

    /// <summary>
    /// The fields settings for To field
    /// </summary>
    public class ToSettings : IFieldSetting
    {
        /// <summary>
        /// What to display in the field label
        /// </summary>
        public string DisplayAs { get; set; }

        /// <summary>
        /// Whether to show To field for cash items
        /// </summary>
        public bool DisplayForCash { get; set; }

        /// <summary>
        /// Whether to show To field for credit card items
        /// </summary>
        public bool DisplayForCreditCard { get; set; }

        /// <summary>
        /// Whether to show To field for purchase card items
        /// </summary>
        public bool DisplayForPurchaseCard { get; set; }

        /// <summary>
        /// Whether to show To field for individual expense items
        /// </summary>
        public bool DisplayOnIndividualItem { get; set; }

        /// <summary>
        /// Whether To field is mandatory for cash items
        /// </summary>
        public bool MandatoryForCash { get; set; }

        /// <summary>
        /// Whether To field is mandatory for credit card items
        /// </summary>
        public bool MandatoryForCreditCard { get; set; }

        /// <summary>
        // /Whether To field is mandatory for purchase card items
        /// </summary>
        public bool MandatoryForPurchaseCard { get; set; }

        /// <summary>
        /// Whether the account permits employees to add a new From field from Add/Edit Expense
        /// </summary>
        public bool CanAddToField { get; set; }
    }

}