namespace BusinessLogic.Reasons
{
    using System;

    using BusinessLogic.Interfaces;

    /// <summary>
    /// Defines a <see cref="IReason"/> and all it's members
    /// </summary>
    public interface IReason : IIdentifier<int>, IArchivable, IDescription, IName, ICreated, IModified
    {
        /// <summary>
        /// Gets or sets the account code vat for this <see cref="IReason"/>.
        /// </summary>
        string AccountCodeVat { get; set; }

        /// <summary>
        /// Gets or sets the account code no vat for this <see cref="IReason"/>.
        /// </summary>
        string AccountCodeNoVat { get; set; }
    }
}
