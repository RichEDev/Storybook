
namespace BusinessLogic.JoinVia
{
    using System;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Tables.Type;

    /// <summary>
    /// Defines a <see cref="IJoinVia"/> step
    /// </summary>
    public interface IJoinViaPart
    {
        /// <summary>
        /// The order of this item.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// The ID of the step <see cref="IField"/> or <see cref="ITable"/>
        /// </summary>
        Guid ViaId { get; }
    }
}