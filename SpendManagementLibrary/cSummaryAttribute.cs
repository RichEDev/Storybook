﻿namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Allow summarisation of multiple one to many attributes related to the same entity
    /// </summary>
    public class cSummaryAttribute : cAttribute
    {
        private Dictionary<int, cSummaryAttributeElement> dicSummaryElements;
        private Dictionary<int, cSummaryAttributeColumn> dicSummaryColumns;
        private int nSourceEntityId;

        #region properties
        public Dictionary<int, cSummaryAttributeElement> SummaryElements
        {
            get { return this.dicSummaryElements; }
        }
        public Dictionary<int, cSummaryAttributeColumn> SummaryColumns
        {
            get { return this.dicSummaryColumns; }
        }
        public int SourceEntityID
        {
            get { return this.nSourceEntityId; }
        }
        #endregion

        /// <summary>
        /// Create a new instance of <see cref="cSummaryAttribute"/>
        /// </summary>
        /// <param name="attributeid">Attribute ID</param>
        /// <param name="attributename">Attribute name</param>
        /// <param name="displayname">Friendly attribute name to display</param>
        /// <param name="description">Attribute description</param>
        /// <param name="createdon">Date attribute created</param>
        /// <param name="createdby">User ID of attribute creator</param>
        /// <param name="modifiedon">Date attribute last modified</param>
        /// <param name="modifiedby">User ID of last user to modify attribute</param>
        /// <param name="fieldid">Field ID for reporting</param>
        /// <param name="system_attribute">Indicates whether the attribute is generated by the application and not a user</param>
        public cSummaryAttribute(int attributeid, string attributename, string displayname, string description, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, Dictionary<int, cSummaryAttributeElement> summaryElements, Dictionary<int, cSummaryAttributeColumn> summaryColumns, int source_entityid, bool allowedit, bool allowdelete, bool system_attribute)
            : base(attributeid, attributename, displayname, description, "", false, FieldType.OTMSummary, createdon, createdby, modifiedon, modifiedby, fieldid, false, false, false, allowedit, allowdelete, false, false, false, system_attribute, false)
        {
            this.dicSummaryElements = summaryElements;
            this.dicSummaryColumns = summaryColumns;
            this.nSourceEntityId = source_entityid;

            this.sCssClass = "";
        }
    }
}