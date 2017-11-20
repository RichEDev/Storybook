namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;

    /// <summary>
    /// The vehicle definition containing data to help build the add vehicle form
    /// </summary>
    public class VehicleDefinition
    {
        /// <summary>
        /// Gets or sets the vehicle engine types.
        /// </summary>
        public List<VehicleEngineType> VehicleEngineTypes {get; set; }

        /// <summary>
        /// Gets or sets a list of valid vehicle journey rates.
        /// </summary>
        public IList<MileageCategory> VehicleJourneyRates { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="UserDefinedFieldValue">user defined fields</see>./>
        /// </summary>
        public List<UserDefinedFieldType> UserDefinedFields { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="FinancialYear">FinancialYear</see>s
        /// </summary>
        public List<FinancialYear> FinancialYears { get; set; }
    }
}