using System;
namespace SpendManagementLibrary.Expedite
{
    /// <summary>
    /// The expedite validation type.
    /// </summary>
    public enum ExpediteValidationType
    {
        /// <summary>
        /// Validation is related to business
        /// </summary>
        Business = 0,

        /// <summary>
        /// Validation is related to VAT
        /// </summary>
        VAT = 1,

        /// <summary>
        /// Custom validation 
        /// </summary>
        Custom = 2,

        /// <summary>
        /// Validation is related to fraud
        /// </summary>
        Fraud = 3

    }
}
