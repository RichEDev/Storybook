namespace BusinessLogic.GeneralOptions.CompanyPolicy
{
    /// <summary>
    /// Defines a <see cref="ICompanyPolicyOptions"/> and it's members
    /// </summary>
    public interface ICompanyPolicyOptions
    {
        /// <summary>
        /// Gets or sets the company policy.
        /// </summary>
        string CompanyPolicy { get; set; }

        /// <summary>
        /// Gets or sets the policy type.
        /// </summary>
        byte PolicyType { get; set; }
    }
}