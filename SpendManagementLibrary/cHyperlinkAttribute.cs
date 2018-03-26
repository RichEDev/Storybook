﻿namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// cHyperlinkAttribute class which inherits cAttribute
    /// </summary>
    public class cHyperlinkAttribute : cAttribute
    {
        private string sHyperlinkText;
        private string sHyperlinkPath;
        
        /// <summary>
        /// Create a new instance of <see cref="cHyperlinkAttribute"/>
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
        /// <param name="system_attribute">Indicates whether the attribute is generated by the application and not a user</param>
        public cHyperlinkAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, bool isauditidentity, bool isunique, string hyperlinkText, string hyperlinkPath, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn, bool system_attribute)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, isunique, allowedit, allowdelete, displayInMobile, builtIn, false, system_attribute, false)
        {
            this.sHyperlinkText = hyperlinkText;
            this.sHyperlinkPath = hyperlinkPath;
        }

        #region properties
        /// <summary>
        /// Gets the URL or file path value for the hyperlink 
        /// </summary>
        public string hyperlinkText
        {
            get { return this.sHyperlinkText; }
        }

        /// <summary>
        /// Gets the URL or file path value for the hyperlink 
        /// </summary>
        public string hyperlinkPath
        {
            get { return this.sHyperlinkPath; }
        }
        #endregion
    }
}