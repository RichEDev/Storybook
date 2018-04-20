namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;

    [Serializable()]
    public class cUserDefinedField
    {
        private readonly int _userDefinedId;
        private readonly cTable _table;
        private readonly cAttribute _attribute;
        private readonly List<int> _subcatIds;
        private readonly int _order;
        private readonly DateTime _createdOn;
        private readonly int _createdBy;
        private readonly DateTime? _modifiedOn;
        private readonly int? _modifiedBy;
        private readonly bool _specific;
        private readonly cUserdefinedFieldGrouping _fieldGrouping;
        private readonly bool _archived;
        private readonly bool _allowSearch;
        private readonly bool _allowEMployeeToPopulate;

        /// <summary>
        /// Initializes a new instance of the <see cref="cUserDefinedField"/> class.
        /// </summary>
        /// <param name="userdefineid">
        /// The userdefineid.
        /// </param>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="subcats">
        /// The subcats.
        /// </param>
        /// <param name="createdon">
        /// The createdon.
        /// </param>
        /// <param name="createdby">
        /// The createdby.
        /// </param>
        /// <param name="modifiedon">
        /// The modifiedon.
        /// </param>
        /// <param name="modifiedby">
        /// The modifiedby.
        /// </param>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <param name="grouping">
        /// The grouping.
        /// </param>
        /// <param name="archived">
        /// The archived.
        /// </param>
        /// <param name="specific">
        /// The specific.
        /// </param>
        /// <param name="allowsearch">
        /// The allowsearch.
        /// </param>
        /// <param name="allowemployeetopopulate">
        /// The allowemployeetopopulate.
        /// </param>
        /// <param name="encrypted">
        /// The encrypted.
        /// </param>
        public cUserDefinedField(int userdefineid, cTable table, int order, List<int> subcats, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, cAttribute attribute, cUserdefinedFieldGrouping grouping, bool archived, bool specific, bool allowsearch, bool allowemployeetopopulate, bool encrypted)
        {
            this.Encrypted = encrypted;
            _userDefinedId = userdefineid;
            _table = table;
            _attribute = attribute;
            _order = order;

            if (this.itemspecific)
            {
                _subcatIds = subcats;
            }

            _createdOn = createdon;
            _createdBy = createdby;
            _modifiedOn = modifiedon;
            _modifiedBy = modifiedby;
            _fieldGrouping = grouping;
            _archived = archived;
            _specific = specific;
            _allowSearch = allowsearch;
            _allowEMployeeToPopulate = allowemployeetopopulate;
        }

        #region properties
        
        public int userdefineid
        {
            get { return _userDefinedId; }
        }
        public string label
        {
            get { return _attribute.displayname; }
        }
        public string description
        {
            get { return _attribute.description; }
        }
        public FieldType fieldtype
        {
            get { return _attribute.fieldtype; }
        }
        public cTable table
        {
            get { return _table; }
        }
        public bool itemspecific
        {
            get { return true; }
        }
        public bool mandatory
        {
            get { return _attribute.mandatory; }
        }
        public int order
        {
            get { return _order; }
        }
        public DateTime createdon
        {
            get { return _createdOn; }
        }
        public int createdby
        {
            get { return _createdBy; }
        }
        public DateTime? modifiedon
        {
            get { return _modifiedOn; }
        }
        public int? modifiedby
        {
            get { return _modifiedBy; }
        }
        public string tooltip
        {
            get { return _attribute.tooltip; }
        }
        public cAttribute attribute
        {
            get { return _attribute; }
        }
        
        public bool Archived
        {
            get { return _archived; }
        }
        public cUserdefinedFieldGrouping Grouping
        {
            get { return _fieldGrouping; }
        }
        /// <summary>
        /// Should be displayed on specific expense items rather than in the general details section
        /// </summary>
        public bool Specific
        {
            get {return _specific;}
        }
        public bool AllowSearch
        {
            get { return _allowSearch; }
        }

        public bool AllowEmployeeToPopulate
        {
            get
            {
                return _allowEMployeeToPopulate;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the field is encrypted in the database.
        /// </summary>
        public bool Encrypted { get; }

        public SortedList<int, cListAttributeElement> items
        {
            get
            {
                if (this._attribute.GetType() == typeof(cListAttribute))
                {
                    var lstatt = (cListAttribute)this._attribute;
                    return lstatt.items;
                }

                return null;
            }
        }

        public List<int> selectedSubcats
        {
            get { return this._subcatIds; }
        }

        #endregion

         }
}
