namespace BusinessLogic.JoinVia
{
    using System;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Tables.Type;

    /// <summary>
    /// A base class for <see cref="IJoinViaPart"/>
    /// </summary>
    public abstract class JoinViaPart : IJoinViaPart
    {
        /// <summary>
        /// Create a new instance of <see cref="JoinViaPart"/>
        /// </summary>
        /// <param name="viaId">The ID of the step <see cref="IField"/> or <see cref="ITable"/></param>
        /// <param name="order">The order of this item.</param>
        protected JoinViaPart(Guid viaId, int order)
        {
            this.ViaId = viaId;
            this.Order = order;
        }

        /// <summary>
        /// The order of this item.
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// The ID of the step <see cref="IField"/> or <see cref="ITable"/>
        /// </summary>
        public Guid ViaId { get; }
    }
}