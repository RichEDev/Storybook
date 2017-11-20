namespace FuelReceiptToVATCalculation
{
    using SpendManagementLibrary;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a response containing a list of account ids.
    /// </summary>
    public class GetAccountVatCalculationEnabledResponses
    {       
        /// <summary>
        /// Gets or sets the list of accountids.
        /// </summary>
        public List<int> List { get; set; }
    }
    /// <summary>
    /// Represents a response  process status of vat calculation
    /// </summary>
    public class FuelReceiptToVATCalculationProcessResponse 
    {
        /// <summary>
        /// Gets or sets processed status for a request
        /// </summary>
        public int isProcessed { get; set; }

    }   
}
