
namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// Represents a "Contact" custom-entity attribute type
    /// </summary>
    public class cContactAttribute : cAttribute
    {
        private AttributeFormat afFormat;

        /// <summary>
        /// Create a new instance of <see cref="cContactAttribute"/>
        /// </summary>
        /// <param name="attributeid">Attribute ID</param>
        /// <param name="attributename">Attribute name</param>
        /// <param name="displayname">Friendly attribute name to display</param>
        /// <param name="description">Attribute description</param>
        /// <param name="tooltip">Tooltip associated with attribute</param>
        /// <param name="mandatory">Is attribute mandatory</param>
        /// <param name="fieldtype">Field type of attribute</param>
        /// <param name="createdon">Date attribute created</param>
        /// <param name="createdby">User ID of attribute creator</param>
        /// <param name="modifiedon">Date attribute last modified</param>
        /// <param name="modifiedby">User ID of last user to modify attribute</param>
        /// <param name="fieldid">Field ID for reporting</param>
        /// <param name="isauditidentity">Is attribute used as field identifier in the audit log</param>
        /// <param name="isunique">Is value to be unique</param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="builtIn">Whether the attribute is a system attribute</param>
        /// <param name="encrypted">Indicates that the data for this attribute is encrypted.</param>
        public cContactAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, AttributeFormat format, bool isauditidentity, bool isunique, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn, bool encrypted) 
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, FieldType.Contact, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, isunique, allowedit, allowdelete, displayInMobile, builtIn, false, false, encrypted)
        {
            this.afFormat = format;
        }

        /// <summary>
        /// Gets or sets the <see cref="AttributeFormat"/> of the <seealso cref="cContactAttribute"/> type
        /// </summary>
        public AttributeFormat format
        {
            get { return this.afFormat; }
        }
    }
}