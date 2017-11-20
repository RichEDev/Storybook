namespace BusinessLogic.Fields.Type.Attributes
{
    using System;

    using BusinessLogic.Fields.Type.Base;

    /// <summary>
    /// The associated <see cref="IField"/> Is a foreign key.
    /// </summary>
    public class ForeignKeyAttribute : IFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKeyAttribute"/> class.
        /// </summary>
        /// <param name="relatedTableId">
        /// The related table id.
        /// </param>
        public ForeignKeyAttribute(Guid relatedTableId)
        {
            this.RelatedTableId = relatedTableId;
        }

        /// <summary>
        /// Gets the related table id.
        /// </summary>
        public Guid RelatedTableId { get; }
    }
}
