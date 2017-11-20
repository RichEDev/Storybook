namespace SpendManagementApi.Models.Types
{
    using Interfaces;
    using GeneralOptions = SpendManagementLibrary.GeneralOptions.GeneralOption;

    /// <summary>
    /// Represents a global or account specific option in the system.
    /// </summary>
    public class GeneralOption : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.GeneralOptions.GeneralOption, GeneralOption>
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
        /// <param name="dbType">The DAL type.</param>
        /// <param name="actionContext">The IActionContext</param>
        /// <returns>This, the API type.</returns>
        public GeneralOption From(SpendManagementLibrary.GeneralOptions.GeneralOption dbType, IActionContext actionContext)
        {
            SubAccountId = dbType.SubaccountId;
            Key = dbType.Key;
            Value = dbType.Value;
            PostKey = dbType.PostKey;
            IsGlobal = dbType.IsGlobal;
            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public SpendManagementLibrary.GeneralOptions.GeneralOption To(IActionContext actionContext)
        {
            return new GeneralOptions(SubAccountId, Key, Value, PostKey,IsGlobal);
        }
    }
}
