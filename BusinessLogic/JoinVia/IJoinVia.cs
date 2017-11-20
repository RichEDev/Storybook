
namespace BusinessLogic.JoinVia
{
    using System;
    using BusinessLogic.Interfaces;

    public interface IJoinVia : IIdentifier<int>
    {
        /// <summary>
        /// The "alias" to use when constructing an SQL statement using this <see cref="IJoinVia"/>
        /// </summary>
        Guid Alias { get; }

        /// <summary>
        /// The description of the Join
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The <see cref="JoinViaParts"/> for this join
        /// </summary>
        JoinViaParts JoinViaList { get; }
    }
}