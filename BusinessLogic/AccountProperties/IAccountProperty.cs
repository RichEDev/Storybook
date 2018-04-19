namespace BusinessLogic.AccountProperties
{
    using BusinessLogic.Interfaces;
    using BusinessLogic.GeneralOptions;

    /// <summary>
    /// Interface defining common field of a Account Property.
    /// </summary>
    public interface IAccountProperty : IIdentifier<string>
    {
        /// <summary>
        /// Gets the <see cref="AccountPropertyKeys"/>
        /// </summary>
        AccountPropertyKeys? AccountPropertyKey { get; }

        /// <summary>
        /// Gets or sets the key for this <see cref="IAccountProperty"/>.
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Gets or sets the value for this <see cref="IAccountProperty"/>.
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Gets or sets the sub account id for this <see cref="IAccountProperty"/>.
        /// </summary>
        int SubAccountId { get; set; }

        /// <summary>
        /// Gets or sets the post key for this <see cref="IAccountProperty"/>.
        /// </summary>
        string PostKey { get; set; }

        /// <summary>
        /// Gets or sets the is global for this <see cref="IAccountProperty"/>.
        /// </summary>
        bool IsGlobal { get; set; }
    }
}
