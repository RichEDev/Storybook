namespace BusinessLogic.JoinVia
{
    using System;
    using BusinessLogic.Fields.Type.Base;

    public class JoinViaPartField : JoinViaPart
    {
        /// <summary>
        /// Create an instance of a <see cref="IJoinViaPart"/> that joins to a <see cref="IField"/>
        /// </summary>
        /// <param name="viaId">The ID of the step <see cref="IField"/> </param>
        /// <param name="order">The order of this item.</param>
        public JoinViaPartField(Guid viaId, int order)
            : base(viaId, order)
        {
        }
    }
}