namespace BusinessLogic.JoinVia
{
    using System;
    using BusinessLogic.Tables.Type;

    public class JoinViaPartTable : JoinViaPart
    {
        /// <summary>
        /// Create a new instance of <see cref="IJoinViaPart"/> which joins to a <see cref="ITable"/> via a Foreign key in the current table
        /// </summary>
        /// <param name="viaId">The ID of the step <see cref="ITable"/> </param>
        /// <param name="order">The order of this item.</param>

        public JoinViaPartTable(Guid viaId, int order)
            : base(viaId, order)
        {
        }
    }
}