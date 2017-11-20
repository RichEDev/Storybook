namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Web.Http.Description;

    using SpendManagementLibrary;

    /// <summary>
    /// The Custom Entity Attribute API Class
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CustomEntityAttribute : BaseExternalType
    {
        /// <summary>
        /// Gets or sets the description of the field
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the field's name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the field's unique identifier
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// Gets or sets the field's database identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the field's tooltip
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FieldType">field type</see> enumerator
        /// </summary>
        public FieldType Type { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AttributeFormat">attribute format</see> enumerator
        /// </summary>
        public AttributeFormat Format { get; set; }

        /// <summary>
        /// Populates this object's properties with values from a SpendManagementLibrary <see cref="cAttribute">attribute object</see>
        /// </summary>
        /// <param name="attribute">The SpendManagementLibrary <see cref="cAttribute">attribute object</see> to copy values from</param>
        /// <returns>This object</returns>
        public CustomEntityAttribute From(cAttribute attribute)
        {
            this.Id = attribute.attributeid;
            this.DisplayName = attribute.displayname;
            this.Description = attribute.description;
            this.Tooltip = attribute.tooltip;
            this.FieldId = attribute.fieldid;
            this.Type = attribute.fieldtype;

            switch (this.Type)
            {
                case FieldType.Contact:
                {
                    this.Format = ((cContactAttribute)attribute).format;
                    break;
                }
            }

            return this;
        }
    }

    /// <summary>
    /// The Custom Entity Lookup Display Attribute API Class
    /// </summary>
    public class CustomEntityLookupDisplayAttribute : CustomEntityAttribute
    {
        /// <summary>
        /// Gets or sets the lookup display attribute's underlying <see cref="FieldType">field type</see>
        /// </summary>
        public FieldType OriginalType { get; set; }

        /// <summary>
        /// Gets or sets the lookup display attribute's underlying <see cref="AttributeFormat">format</see>
        /// </summary>
        public AttributeFormat OriginalFormat { get; set; }

        /// <summary>
        /// Populates this object's properties with values from a SpendManagementLibrary <see cref="LookupDisplayField">attribute object</see>
        /// Adding properties from the LookupDisplayField's original <see cref="cAttribute">attribute object</see>.
        /// </summary>
        /// <param name="attribute">The SpendManagementLibrary <see cref="LookupDisplayField">attribute</see> to copy values from</param>
        /// <param name="originalAttribute">The underlying SpendManagementLibrary <see cref="cAttribute">attribute object</see> to copy values from</param>
        /// <returns>This object</returns>
        public CustomEntityLookupDisplayAttribute From(LookupDisplayField attribute, cAttribute originalAttribute)
        {
            base.From(attribute);
            this.DisplayName = this.Description;

            if (originalAttribute != null)
            {
                this.OriginalType = originalAttribute.fieldtype;

                switch (this.OriginalType)
                {
                    case FieldType.Contact:
                    {
                        this.OriginalFormat = ((cContactAttribute)originalAttribute).format;
                        break;
                    }
                }
            }

            return this;
        }
    }
}