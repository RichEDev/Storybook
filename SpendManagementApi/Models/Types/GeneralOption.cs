namespace SpendManagementApi.Models.Types
{
    using BusinessLogic.AccountProperties;
    
    /// <summary>
    /// Represents a global or account specific option in the system.
    /// </summary>
    public class GeneralOption : BaseExternalType
    {
        /// <summary>
        /// The sub account this general option is specific to.
        /// </summary>
        public int SubAccountId { get; set; }
        
        /// <summary>
        /// The look-up identifier for this general option.
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// The value to store in this general option.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The form post key.
        /// </summary>
        public string PostKey { get; set; }

        /// <summary>
        /// Specifies if this general option is applicable to all accounts.
        /// </summary>
        public bool IsGlobal { get; set; }

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <param name="accountProperty">The DAL type.</param>
        /// <returns>This, the API type.</returns>
        public GeneralOption From(IAccountProperty accountProperty)
        {
            SubAccountId = accountProperty.SubAccountId;
            Key = accountProperty.Key;
            Value = accountProperty.Value;
            PostKey = accountProperty.PostKey;
            IsGlobal = accountProperty.IsGlobal;
            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>This, the DAL type.</returns>
        public AccountProperty To()
        {
            var accountProperty = new AccountProperty(this.Key, this.Value, this.SubAccountId)
            {
                IsGlobal = this.IsGlobal,
                PostKey = this.PostKey
            };

            return accountProperty;
        }
    }
}
