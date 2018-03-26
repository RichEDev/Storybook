﻿namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// Used for inbuilt currency lists
    /// </summary>
    public class cCurrencyListAttribute : cAttribute
    {
        /// <summary>
        /// Create a new instance of <see cref="cCurrencyListAttribute"/>
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
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute">Indicates whether the attribute is generated by the application and not a user</param>
        public cCurrencyListAttribute(int attributeid, string attributename, string displayname, string description, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, bool allowedit, bool allowdelete, bool displayInMobile, bool system_attribute )
            : base(attributeid, attributename, displayname, description, "", false, FieldType.CurrencyList, createdon, createdby, modifiedon, modifiedby, fieldid, false, false, false, allowedit, allowdelete, displayInMobile, false,false,  system_attribute, false)
        {
            this.sFieldIDPrefix = "ddl";
        }
    }
}