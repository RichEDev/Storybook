namespace BusinessLogic.GeneralOptions.CompanyPolicy
{
    /// <summary>
    /// The company policy options.
    /// </summary>
    public class CompanyPolicyOptions: ICompanyPolicyOptions
    {
        /// <summary>
        /// Gets or sets the company policy.
        /// </summary>
        public string CompanyPolicy { get; set; }

        /// <summary>
        /// Gets or sets the policy type.
        /// </summary>
        public byte PolicyType { get; set; }
    }
}
