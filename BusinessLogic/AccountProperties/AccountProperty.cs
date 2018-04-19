namespace BusinessLogic.AccountProperties
{
    using System;

    using BusinessLogic.Enums;
    using BusinessLogic.GeneralOptions;

    /// <summary>
    /// Defines a basic <see cref="AccountProperty"/> and its members.
    /// </summary>
    [Serializable]
    public class AccountProperty : IAccountProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountProperty"/> class.
        /// </summary>
        /// <param name="key">The key of <see cref="AccountProperty"/></param>
        /// <param name="value">The value of the <see cref="AccountProperty"/></param>
        /// <param name="subAccountId">The sub account id of the <see cref="AccountProperty"/></param>
        public AccountProperty(string key, string value, int subAccountId)
        {
            Guard.ThrowIfNullOrWhiteSpace(key, nameof(key));
            Guard.ThrowIfNull(subAccountId, nameof(subAccountId));

            this.Id = $"{subAccountId}/{key}";
            this.Key = key;
            this.Value = value;
            this.SubAccountId = subAccountId;
            this.AccountPropertyKey = EnumHelper.GetEnumValueFromDescription<AccountPropertyKeys>(key);
        }

        /// <summary>
        /// Gets the <see cref="AccountPropertyKeys"/>
        /// </summary>
        public AccountPropertyKeys? AccountPropertyKey { get; }

        /// <summary>
        /// Gets or sets the Id for this <see cref="AccountProperty"/>.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the key for this <see cref="AccountProperty"/>.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name for this <see cref="AccountProperty"/>.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the sub account id for this <see cref="AccountProperty"/>.
        /// </summary>
        public int SubAccountId { get; set; }

        /// <summary>
        /// Gets or sets the post key for this <see cref="AccountProperty"/>.
        /// </summary>
        public string PostKey { get; set; }

        /// <summary>
        /// Gets or sets the is global for this <see cref="AccountProperty"/>.
        /// </summary>
        public bool IsGlobal { get; set; }
    }
}
