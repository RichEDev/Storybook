namespace BusinessLogic.GeneralOptions.SelfRegistration
{
    /// <summary>
    /// Defines a <see cref="ISelfRegistrationOptions"/> and it's members
    /// </summary>
    public interface ISelfRegistrationOptions
    {
        /// <summary>
        /// Gets or set the allow self registration
        /// </summary>
        bool AllowSelfReg { get; set; }

        /// <summary>
        /// Gets or sets the allow self registration employee contact
        /// </summary>
        bool AllowSelfRegEmployeeContact { get; set; }

        /// <summary>
        /// Gets or sets the allow self registration home address
        /// </summary>
        bool AllowSelfRegHomeAddress { get; set; }

        /// <summary>
        /// Gets or sets the allow self registration employee info
        /// </summary>
        bool AllowSelfRegEmployeeInfo { get; set; }

        /// <summary>
        /// Get or sets the allow self registration role
        /// </summary>
        bool AllowSelfRegRole { get; set; }

        /// <summary>
        /// Get or sets the allow self registration sign off
        /// </summary>
        bool AllowSelfRegSignOff { get; set; }

        /// <summary>
        /// Get or sets the allow self registration item role
        /// </summary>
        bool AllowSelfRegItemRole { get; set; }

        /// <summary>
        /// Get or sets the allow self registration advances sign off
        /// </summary>
        bool AllowSelfRegAdvancesSignOff { get; set; }

        /// <summary>
        /// Get or sets the allow self registration department and cost code
        /// </summary>
        bool AllowSelfRegDepartmentCostCode { get; set; }

        /// <summary>
        /// Get or sets the allow self registration bank details
        /// </summary>
        bool AllowSelfRegBankDetails { get; set; }

        /// <summary>
        /// Get or sets the allow self registration car details
        /// </summary>
        bool AllowSelfRegCarDetails { get; set; }

        /// <summary>
        /// Get or sets the allow self registration UDF
        /// </summary>
        bool AllowSelfRegUDF { get; set; }

        /// <summary>
        /// Gets or sets the default role
        /// </summary>
        int? DefaultRole { get; set; }

        /// <summary>
        /// Gets or sets the default item role
        /// </summary>
        int? DefaultItemRole { get; set; }
    }
}
