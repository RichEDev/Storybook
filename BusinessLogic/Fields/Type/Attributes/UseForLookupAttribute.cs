namespace BusinessLogic.Fields.Type.Attributes
{
    using System;

    using BusinessLogic.Fields.Type.Base;

    /// <summary>
    /// The associated <see cref="IField"/> can be used for lookup.
    /// </summary>
    public class UseForLookupAttribute : IFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseForLookupAttribute"/> class.
        /// </summary>
        /// <param name="lookupFieldId">
        /// The lookup field id.
        /// </param>
        /// <param name="lookupTableId">
        /// The lookup table id.
        /// </param>
        public UseForLookupAttribute(Guid lookupFieldId, Guid lookupTableId)
        {
            this.LookupFieldId = lookupFieldId;
            this.LookupTableId = lookupTableId;
        }

        /// <summary>
        /// Gets the lookup field id.
        /// </summary>
        public Guid LookupFieldId { get; }

        /// <summary>
        /// Gets the lookup table id.
        /// </summary>
        public Guid LookupTableId { get; }
    }
}
