namespace BusinessLogic.Accounts.Elements
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a <see cref="IAccountWithElement"/>
    /// </summary>
    public interface IAccountWithElement : IAccount
    {
        /// <summary>
        /// Gets or sets the <see cref="IElement"/> for this <see cref="IAccountWithElement"/>.
        /// </summary>
        IList<IElement> LicencedElements { get; }
    }
}
