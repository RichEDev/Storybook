namespace SpendManagementApi.Models.Types
{
    using System.Web.Http.Description;

    using Spend_Management;
    using SpendManagementLibrary;

    /// <summary>
    /// The Custom Entity Form Field API Class
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CustomEntityFormField
    {
        /// <summary>
        /// Gets or sets the field's <see cref="CustomEntityAttribute">attribute</see>
        /// </summary>
        public CustomEntityAttribute Attribute { get; set; }

        /// <summary>
        /// Gets or sets the column position of this field
        /// </summary>
        public byte Column { get; set; }

        /// <summary>
        /// Gets or sets the row position of this field
        /// </summary>
        public byte Row { get; set; }

        /// <summary>
        /// Gets or sets the field's label text
        /// </summary>
        public string LabelText { get; set; }

        /// <summary>
        /// Populates this object's properties with values from a SpendManagementLibrary <see cref="cCustomEntityFormField">custom entity form field object</see>
        /// </summary>
        /// <param name="field">The SpendManagementLibrary <see cref="cCustomEntityFormField">custom entity form field object</see> to copy values from</param>
        /// <param name="user">The SpendManagementLibrary <see cref="ICurrentUser">user object</see></param>
        /// <returns>This object</returns>
        public CustomEntityFormField From(cCustomEntityFormField field, ICurrentUser user)
        {
            this.Column = field.column;
            this.Row = field.row;
            this.LabelText = field.labelText;
            this.Attribute = new CustomEntityAttribute().From(field.attribute);

            // if the field's attribute is a LookupDisplayField determine what it's original FieldType and AttributeFormat is
            if (field.attribute.GetType() == typeof(LookupDisplayField))
            {
                var fields = new cFields(user.AccountID);

                var attribute = (LookupDisplayField)field.attribute;
                if (attribute != null && attribute.TriggerDisplayFieldId.HasValue)
                {
                    var originalField = fields.GetFieldByID(attribute.TriggerDisplayFieldId.Value);
                    var originalAttribute = new cCustomEntities(user).getAttributeByFieldId(originalField.FieldID);

                    this.Attribute = new CustomEntityLookupDisplayAttribute().From(attribute, originalAttribute);
                }
            }

            return this;
        }
    }
}