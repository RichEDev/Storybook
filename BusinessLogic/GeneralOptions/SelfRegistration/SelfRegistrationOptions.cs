namespace BusinessLogic.GeneralOptions.SelfRegistration
{
    /// <summary>
    /// Defines a <see cref="SelfRegistrationOptions"/> and it's members
    /// </summary>
    public class SelfRegistrationOptions : ISelfRegistrationOptions
    {
        /// <summary>
        /// Gets or set the allow self registration
        /// </summary>
        public bool AllowSelfReg { get; set; }

        /// <summary>
        /// Gets or sets the allow self registration employee contact
        /// </summary>
        public bool AllowSelfRegEmployeeContact { get; set; }

        /// <summary>
        /// Gets or sets the allow self registration home address
        /// </summary>
        public bool AllowSelfRegHomeAddress { get; set; }

        /// <summary>
        /// Gets or sets the allow self registration employee info
        /// </summary>
        public bool AllowSelfRegEmployeeInfo { get; set; }

        /// <summary>
        /// Get or sets the allow self registration role
        /// </summary>
        public bool AllowSelfRegRole { get; set; }

        /// <summary>
        /// Get or sets the allow self registration sign off
        /// </summary>
        public bool AllowSelfRegSignOff { get; set; }

        /// <summary>
        /// Get or sets the allow self registration item role
        /// </summary>
        public bool AllowSelfRegItemRole { get; set; }

        /// <summary>
        /// Get or sets the allow self registration advances sign off
        /// </summary>
        public bool AllowSelfRegAdvancesSignOff { get; set; }

        /// <summary>
        /// Get or sets the allow self registration department and cost code
        /// </summary>
        public bool AllowSelfRegDepartmentCostCode { get; set; }

        /// <summary>
        /// Get or sets the allow self registration bank details
        /// </summary>
        public bool AllowSelfRegBankDetails { get; set; }

        /// <summary>
        /// Get or sets the allow self registration car details
        /// </summary>
        public bool AllowSelfRegCarDetails { get; set; }

        /// <summary>
        /// Get or sets the allow self registration UDF
        /// </summary>
        public bool AllowSelfRegUDF { get; set; }

        /// <summary>
        /// Gets or sets the default role
        /// </summary>
        public int? DefaultRole { get; set; }

        /// <summary>
        /// Gets or sets the default item role
        /// </summary>
        public int? DefaultItemRole { get; set; }
    }
}
