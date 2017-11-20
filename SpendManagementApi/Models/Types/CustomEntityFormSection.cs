namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Description;

    using Spend_Management;
    using SpendManagementLibrary;

    /// <summary>
    /// Enumerator for custom entity for section types
    /// </summary>
    public enum CustomEntityFormSectionType
    {
        /// <summary>
        /// Section contains CustomEntityFormFields
        /// </summary>
        Fields = 1,

        /// <summary>
        /// Section contains a list of Torch Attachments
        /// </summary>
        TorchAttachments = 2,

        /// <summary>
        /// Section contains a list of Attachments
        /// </summary>
        Attachments = 3
    }

    /// <summary>
    /// Custom Entity Form Section API Class
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CustomEntityFormSection
    {
        /// <summary>
        /// Gets or sets a list of <see cref="CustomEntityFormField">custom entity form fields</see>
        /// </summary>
        public List<CustomEntityFormField> Fields { get; set; }

        /// <summary>
        /// Gets or sets the identifier of this section
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the header caption text for this section
        /// </summary>
        public string HeaderCaption { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CustomEntityFormSectionType"/> type
        /// </summary>
        public CustomEntityFormSectionType Type { get; set; }

        /// <summary>
        /// Populates this object's properties with values from a SpendManagementLibrary <see cref="cCustomEntityFormSection">custom entity form section object</see>
        /// </summary>
        /// <param name="section">The SpendManagementLibrary <see cref="cCustomEntityFormSection">custom entity form section object</see> to copy values from</param>
        /// <param name="user">The SpendManagementLibrary <see cref="ICurrentUser">user object</see></param>
        /// <returns>This object</returns>
        public CustomEntityFormSection From(cCustomEntityFormSection section, ICurrentUser user)
        {
            this.Id = section.sectionid;
            this.Fields = new List<CustomEntityFormField>();
            this.HeaderCaption = section.headercaption;
            this.Type = CustomEntityFormSectionType.Fields;

            if (section.fields.Any())
            {
                foreach (cCustomEntityFormField field in section.fields.Where(formField => formField.attribute.DisplayInMobile || formField.attribute.fieldtype == FieldType.LookupDisplayField))
                {
                    this.Fields.Add(new CustomEntityFormField().From(field, user));
                }
            }

            return this;
        }
    }
}