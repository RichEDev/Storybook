namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// cManyToOneRelationship class which inherits cAttribute
    /// </summary>
    public class cManyToOneRelationship : cAttribute
    {
        [NonSerialized()]
        private cTable tblRelatedTable;
        private cTable tblAliasTable;
        private Guid gDisplayFieldID;
        private List<Guid> lstMatchFieldIDs;
        private List<Guid> lstAutocompleteDisplayFieldIDs;
        private int nMaxRows;
        private SortedList<int, FieldFilter> lstFilters;
        private List<LookupDisplayField> lstTriggerFields;

        /// <summary>
        /// Create a new instance of <see cref="cManyToOneRelationship"/>
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
        /// <param name="builtIn">Whether the attribute is a system attribute</param>
        /// <param name="autoCompleteDisplayFieldID">Field ID of field to display in the auto complete return list</param>
        /// <param name="autoCompleteMatchFieldIDs">Field ID collection of fields to match search text against</param>
        /// <param name="maxRows">Maximum number of rows to return in the auto complete options list</param>
        /// <param name="autocompleteDisplayFields">Field ID collection of fields todisplay in the search results in autocomplete fields</param>
        /// <param name="filters">A list of FieldFilters to be applied to the ManyToOne Relationship</param>
        /// <param name="system_attribute"></param>
        public cManyToOneRelationship(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, bool builtIn, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, cTable relatedtable, Guid fieldid, bool isauditidentity, bool allowedit, bool allowdelete, cTable AliasTable, Guid autoCompleteDisplayFieldID, List<Guid> autoCompleteMatchFieldIDs, int maxRows, List<Guid> autocompleteDisplayFields, SortedList<int, FieldFilter> filters , bool system_attribute , List<LookupDisplayField> triggerFields )
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, FieldType.Relationship, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, false, allowedit, allowdelete, false, builtIn, false, system_attribute, false)
        {
            this.tblRelatedTable = relatedtable;
            this.tblAliasTable = AliasTable;
            this.gDisplayFieldID = autoCompleteDisplayFieldID;
            this.lstMatchFieldIDs = autoCompleteMatchFieldIDs;
            this.lstAutocompleteDisplayFieldIDs = autocompleteDisplayFields;
            this.nMaxRows = maxRows;
            this.lstFilters = filters ?? new SortedList<int, FieldFilter>();
            this.lstTriggerFields = triggerFields ?? new List<LookupDisplayField>();
        }

        #region properties
        /// <summary>
        /// Gets the related table definition
        /// </summary>
        public cTable relatedtable
        {
            get { return this.tblRelatedTable; }
        }

        /// <summary>
        /// Gets the alias table definition
        /// </summary>
        public cTable AliasTable
        {
            get { return this.tblAliasTable; }
        }

        /// <summary>
        /// Gets the field ID of the field to display in the text box following auto complete selection
        /// </summary>
        public Guid AutoCompleteDisplayField
        {
            get { return this.gDisplayFieldID; }
        }

        /// <summary>
        /// Gets list of field IDs used for matching wildcard against during auto complete
        /// </summary>
        public List<Guid> AutoCompleteMatchFieldIDList
        {
            get { return this.lstMatchFieldIDs; }
        }

        /// <summary>
        /// Gets list of field IDs used for matching wildcard against during auto complete
        /// </summary>
        public List<Guid> AutoCompleteDisplayFieldIDList
        {
            get { return this.lstAutocompleteDisplayFieldIDs; }
        }

        /// <summary>
        /// Gets the maximum number of return items for the autocomplete options
        /// </summary>
        public int AutoCompleteMatchRows
        {
            get { return this.nMaxRows; }
        }

        /// <summary>
        /// Gets the filters that are applied to the auto complete
        /// </summary>
        public SortedList<int, FieldFilter> filters
        {
            get { return this.lstFilters; }
        }

        /// <summary>
        /// Gets a collection of Lookup Display Fields that are populated automatically by this Many To One Relationship attribute
        /// </summary>
        public List<LookupDisplayField> TriggerLookupFields
        {
            get { return this.lstTriggerFields; }
        }
        #endregion
    }
}