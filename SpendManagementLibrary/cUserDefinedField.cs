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

        public cUserDefinedField(int userdefineid, cTable table, int order, List<int> subcats, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, cAttribute attribute, cUserdefinedFieldGrouping grouping, bool archived, bool specific, bool allowsearch, bool allowemployeetopopulate)
        {
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

        public SortedList<int, cListAttributeElement> items
        {
            get
            {
                if (_attribute.GetType() == typeof(cListAttribute))
                {
                    var lstatt = (cListAttribute)_attribute;
                    return lstatt.items;
                }

                return null;
            }
        }

        public List<int> selectedSubcats
        {
            get { return _subcatIds; }
        }


        public List<System.Web.UI.WebControls.ListItem> CreateDropDown()
        {
            var items = new List<System.Web.UI.WebControls.ListItem>();
            var listatt = (cListAttribute)_attribute;
            items.Add(new System.Web.UI.WebControls.ListItem("", "0"));
            
            foreach (KeyValuePair<int,cListAttributeElement> curItem in listatt.items)
            {
                cListAttributeElement s = curItem.Value;
                
                items.Add(new System.Web.UI.WebControls.ListItem(s.elementText, s.elementValue.ToString(CultureInfo.InvariantCulture)));
            }

            return items;
        }

        public DataSet getUserDefinedList()
        {
            return new DataSet();
        }
        public cListItem getListItemByID(int itemid)
        {
            return null;
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

        #endregion

         }

    

    public enum AppliesTo
    {
        CONTRACT_DETAILS = 1,
        CONTRACT_PRODUCTS = 2,
        PRODUCT_DETAILS = 3,
        VENDOR_DETAILS = 4,
        CONTRACT_GROUPING = 5,
        RECHARGE_GROUPING = 6,
        CONPROD_GROUPING = 7,
        STAFF_DETAILS = 8,
        INVOICE_DETAILS = 9,
        INVOICE_FORECASTS = 10,
        VENDOR_CONTACTS = 11,
        VENDOR_GROUPING = 12,
        Employee,
        ExpenseItem,
        Claim,
        ItemCategory,
        Car,
        Company,
        Costcode,
        Department,
        Projectcode
    }

    [Serializable()]
    public class cListItem
    {
        private readonly int _itemId;
        private readonly int _userDefinedId;
        private readonly string _item;
        private readonly string _comment;
        private readonly DateTime _createdOn;
        private readonly int _createdBy;
        private readonly DateTime _modifiedOn;
        private readonly int _modifiedBy;

        public cListItem(int userdefineid, int itemid, string item, string comment, DateTime createdon, int createdby, DateTime modifiedon, int modifiedby)
        {
            _itemId = itemid;
            _userDefinedId = userdefineid;
            _item = item;
            _comment = comment;
            _createdOn = createdon;
            _createdBy = createdby;
            _modifiedOn = modifiedon;
            _modifiedBy = modifiedby;
        }

        #region properties
        public int itemid
        {
            get { return _itemId; }
        }
        public int userdefineid
        {
            get { return _userDefinedId; }
        }
        public string item
        {
            get { return _item; }
        }
        public string comment
        {
            get { return _comment; }
        }
        public DateTime createdon
        {
            get { return _createdOn; }
        }
        public int createdby
        {
            get { return _createdBy; }
        }
        public DateTime modifiedon
        {
            get { return _modifiedOn; }
        }
        public int modifiedby
        {
            get { return _modifiedBy; }
        }
        #endregion
    }

    [Serializable()]
    public struct sOnlineUserdefinedInfo
    {
        public Dictionary<int, cUserDefinedField> lstUserdefined;
        public List<int> lstUserdefinedids;
        public Dictionary<int, cListItem> lstListitems;
        public List<int> lstListitemids;
    }
}
