﻿namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// cTextAttribute class which inherits cAttribute
    /// </summary>
    public class cTextAttribute : cAttribute
    {
        private AttributeFormat afFormat;
        private int? nMaxLength;

        /// <summary>
        /// Create a new instance of <see cref="cTextAttribute"/>
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
        /// <param name="removeFonts">True to remove font information from any formatted text.</param>
        /// <param name="system_attribute">Indicates whether the attribute is generated by the application and not a user</param>
        /// <param name="encrypted">Indicates that the data for this attribute is encrypted.</param>
        public cTextAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, int? maxlength, AttributeFormat format, Guid fieldid, bool isauditidentity, bool isunique, bool allowedit, bool allowdelete, bool removeFonts, bool displayInMobile, bool builtIn, bool system_attribute, bool encrypted )
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, isunique, allowedit, allowdelete, displayInMobile, builtIn, removeFonts, system_attribute, encrypted)
        {
            this.nMaxLength = maxlength;
            this.afFormat = format;

            if (format == AttributeFormat.FormattedText || format == AttributeFormat.MultiLine)
            {
                this.sCssClass = (format == AttributeFormat.FormattedText) ? "onecolumnlarge" : "onecolumn";
                this.nMaxLabelTextLength = 50;
            }
            if (format == AttributeFormat.SingleLineWide)
            {
                this.sCssClass = "onecolumnsmall";
            }
        }

        #region properties
        /// <summary>
        /// Gets format type of text attribute
        /// </summary>
        public AttributeFormat format
        {
            get { return this.afFormat; }
        }
        /// <summary>
        /// Gets maximum length permitted for text attribute
        /// </summary>
        public int? maxlength
        {
            get { return this.nMaxLength; }
        }

        public bool RemoveFonts
        {
            get { return base.BoolAttribute; }
        }

        #endregion
    }
}