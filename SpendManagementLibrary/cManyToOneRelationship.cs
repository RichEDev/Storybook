namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// cManyToOneRelationship class which inherits cAttribute
    /// </summary>
    public class cManyToOneRelationship : cAttribute
    {
        /// <summary>
        /// The _related table.
        /// </summary>
        [NonSerialized()]
        private readonly cTable _relatedTable;

        /// <summary>
        /// The _alias table.
        /// </summary>
        private readonly cTable _aliasTable;

        /// <summary>
        /// The _display field id.
        /// </summary>
        private readonly Guid _displayFieldId;

        /// <summary>
        /// The _match field ids.
        /// </summary>
        private readonly List<Guid> _matchFieldIds;

        /// <summary>
        /// The _autocomplete display field i ds.
        /// </summary>
        private readonly List<Guid> _autocompleteDisplayFieldIDs;

        /// <summary>
        /// The _max rows.
        /// </summary>
        private readonly int _maxRows;

        /// <summary>
        /// The _filters.
        /// </summary>
        private readonly SortedList<int, FieldFilter> _filters;

        /// <summary>
        /// The _trigger fields.
        /// </summary>
        private readonly List<LookupDisplayField> _triggerFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="cManyToOneRelationship"/> class.
        /// </summary>
        /// <param name="attributeid">
        /// Attribute ID
        /// </param>
        /// <param name="attributename">
        /// Attribute name
        /// </param>
        /// <param name="displayname">
        /// Friendly attribute name to display
        /// </param>
        /// <param name="description">
        /// Attribute description
        /// </param>
        /// <param name="tooltip">
        /// Tooltip associated with attribute
        /// </param>
        /// <param name="mandatory">
        /// Is attribute mandatory
        /// </param>
        /// <param name="builtIn">
        /// Whether the attribute is a system attribute
        /// </param>
        /// <param name="createdon">
        /// Date attribute created
        /// </param>
        /// <param name="createdby">
        /// User ID of attribute creator
        /// </param>
        /// <param name="modifiedon">
        /// Date attribute last modified
        /// </param>
        /// <param name="modifiedby">
        /// User ID of last user to modify attribute
        /// </param>
        /// <param name="relatedtable">
        /// The relatedtable.
        /// </param>
        /// <param name="fieldid">
        /// Field ID for reporting
        /// </param>
        /// <param name="isauditidentity">
        /// Is attribute used as field identifier in the audit log
        /// </param>
        /// <param name="allowedit">
        /// Is edit allowed.
        /// </param>
        /// <param name="allowdelete">
        /// Is delete allowed.
        /// </param>
        /// <param name="aliasTable">
        /// The Alias <see cref="cTable"/>.
        /// </param>
        /// <param name="autoCompleteDisplayFieldID">
        /// Field ID of field to display in the auto complete return list
        /// </param>
        /// <param name="autoCompleteMatchFieldIDs">
        /// Field ID collection of fields to match search text against
        /// </param>
        /// <param name="maxRows">
        /// Maximum number of rows to return in the auto complete options list
        /// </param>
        /// <param name="autocompleteDisplayFields">
        /// Field ID collection of fields todisplay in the search results in autocomplete fields
        /// </param>
        /// <param name="filters">
        /// A list of FieldFilters to be applied to the ManyToOne Relationship
        /// </param>
        /// <param name="systemAttribute">
        /// True if this is a system attribute
        /// </param>
        /// <param name="triggerFields">
        /// a <see cref="List{T}"/> if <seealso cref="LookupDisplayField"/>.
        /// </param>
        public cManyToOneRelationship(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, bool builtIn, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, cTable relatedtable, Guid fieldid, bool isauditidentity, bool allowedit, bool allowdelete, cTable aliasTable, Guid autoCompleteDisplayFieldID, List<Guid> autoCompleteMatchFieldIDs, int maxRows, List<Guid> autocompleteDisplayFields, SortedList<int, FieldFilter> filters, bool systemAttribute, List<LookupDisplayField> triggerFields)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, FieldType.Relationship, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, false, allowedit, allowdelete, false, builtIn, false, systemAttribute, false)
        {
            this._relatedTable = relatedtable;
            this._aliasTable = aliasTable;
            this._displayFieldId = autoCompleteDisplayFieldID;
            this._matchFieldIds = autoCompleteMatchFieldIDs;
            this._autocompleteDisplayFieldIDs = autocompleteDisplayFields;
            this._maxRows = maxRows;
            this._filters = filters ?? new SortedList<int, FieldFilter>();
            this._triggerFields = triggerFields ?? new List<LookupDisplayField>();
        }

        #region properties
        /// <summary>
        /// Gets the related table definition
        /// </summary>
        public cTable relatedtable
        {
            get { return this._relatedTable; }
        }

        /// <summary>
        /// Gets the alias table definition
        /// </summary>
        public cTable AliasTable
        {
            get { return this._aliasTable; }
        }

        /// <summary>
        /// Gets the field ID of the field to display in the text box following auto complete selection
        /// </summary>
        public Guid AutoCompleteDisplayField
        {
            get { return this._displayFieldId; }
        }

        /// <summary>
        /// Gets list of field IDs used for matching wildcard against during auto complete
        /// </summary>
        public List<Guid> AutoCompleteMatchFieldIDList
        {
            get { return this._matchFieldIds; }
        }

        /// <summary>
        /// Gets list of field IDs used for matching wildcard against during auto complete
        /// </summary>
        public List<Guid> AutoCompleteDisplayFieldIDList
        {
            get { return this._autocompleteDisplayFieldIDs; }
        }

        /// <summary>
        /// Gets the maximum number of return items for the autocomplete options
        /// </summary>
        public int AutoCompleteMatchRows
        {
            get { return this._maxRows; }
        }

        /// <summary>
        /// Gets the filters that are applied to the auto complete
        /// </summary>
        public SortedList<int, FieldFilter> filters
        {
            get { return this._filters; }
        }

        /// <summary>
        /// Gets a collection of Lookup Display Fields that are populated automatically by this Many To One Relationship attribute
        /// </summary>
        public List<LookupDisplayField> TriggerLookupFields
        {
            get { return this._triggerFields; }
        }
        #endregion
    }
}