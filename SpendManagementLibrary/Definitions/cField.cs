using System;
using System.Collections.Generic;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cField : IRelabel
    {
        [NonSerialized]
        private cTable _LookupTable;
        [NonSerialized]
        private cTable _RelatedTable;
        [NonSerialized]
        private cField _LookupField;
        [NonSerialized]
        private cTable _ParentTable;

        private int accountID;

        #region Properties - get/set for ajax

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public int AccountID
        {
            get
            {
                return this.accountID;
            }
            set
            {
                this.accountID = value;
            }
        }

        public Guid FieldID { get; set; }
        public Guid TableID { get; set; }
        public Guid ViewGroupID { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
        public string Comment { get; set; }
        public bool NormalView { get; set; }
        public bool IDField { get; set; }
        public bool GenList { get; set; }
        public bool CanTotal { get; set; }
        public int Width { get; set; }
        public bool ValueList { get; set; }
        public bool AllowImport { get; set; }
        public bool Mandatory { get; set; }
        public bool PrintOut { get; set; }
        public int Length { get; set; }
        public bool WorkflowUpdate { get; set; }
        public bool WorkflowSearch { get; set; }
        public bool ReLabel
        {
            get { return (this.RelabelParam != string.Empty) ? true : false; }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the relabel item to use (if any) used in <see cref="IRelabler{T}"/>
        /// </summary>
        public string RelabelParam { get; set; }
        public string ClassPropertyName { get; set; }
        public SortedList<object, string> ListItems { get; set; }

        public Guid LookupFieldID { get; set; }
        public bool UseForLookup { get; set; }
        public Guid RelatedTableID { get; set; }
        public Guid LookupTableID { get; set; }
        public FieldSourceType FieldSource { get; set; }
        public FieldCategory FieldCategory { get; set; }
        public bool IsForeignKey { get; set; }

        /// <summary>
        /// Gets or sets the friendly name to the node which is a foreign key in the tree
        /// </summary>
        public string FriendlyNameTo { get; set; }

        /// <summary>
        /// Gets or sets the friendly name to the node which is a foreign key in the tree
        /// </summary>
        public string FriendlyNameFrom { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Guid"/>ID of the group used in the tree view.
        /// </summary>
        public Guid? TreeGroup { get; set; }

        #region cTable/cField properties

        public cTable GetRelatedTable()
        {
            if (this._RelatedTable == null)
            {
                this._RelatedTable = this.GetTable(this.RelatedTableID);
            }
            return this._RelatedTable;
        }

        public cTable GetLookupTable()
        {
            if (this._LookupTable == null)
            {
                this._LookupTable = this.GetTable(this.LookupTableID);
            }

            return this._LookupTable;
        }

        public cField GetLookupField()
        {
            //cFields clsFields = new cFields(this.AccountID);
            if (this._LookupField == null)
            {
                this._LookupField = this.GetField(this.LookupFieldID);
            }
            return this._LookupField;
        }

        public cTable GetParentTable()
        {
            if (this._ParentTable == null)
            {
                this._ParentTable = this.GetTable(this.TableID);
            }
            return this._ParentTable;
        }

        #endregion cTable/cField properties
        #endregion Properties - get/set for ajax

        public enum FieldSourceType
        {
            Metabase = 0,
            CustomEntity = 1,
            Userdefined = 2
        }

        private cTable GetTable(Guid tableID)
        {
            cTables clsTables = null;
            if (this.AccountID == 0)
            {
                clsTables = new cTables();
            }
            else
            {
                clsTables = new cTables(this.AccountID);
            }

            return clsTables.GetTableByID(tableID);
        }

        private cField GetField(Guid fieldID)
        {
            cFields clsFields = null;
            if (this.AccountID == 0) // metabase field
            {
                clsFields = new cFields();
            }
            else
            {
                clsFields = new cFields(this.AccountID);
            }

            return clsFields.GetFieldByID(fieldID);
        }

        /// <summary>
        /// Returns true if the current <see cref="cField"/> is a string type
        /// </summary>
        /// <returns>True is a string type.</returns>
        public bool StringField()
        {
            switch (this.FieldType)
            {
                case "S":
                case "LT":
                case "FS":
                    return true;
            }

            return false;
        }

        public cField() { }

        public cField(int accountID, Guid fieldID, Guid tableID, Guid viewGroupID, Guid lookupFieldID, Guid relatedTableID, Guid lookupTableID, string fieldName, string fieldType, string description, string comment, bool normalView, bool idField, bool genList, bool canTotal, int width, bool valueList, bool allowImport, bool mandatory, bool printOut, int length, bool workflowUpdate, bool workflowSearch, string relabelParam, string classPropertyName, SortedList<object, string> listItems, bool useForLookup, FieldSourceType fieldSource, FieldCategory fieldCat, bool isForeignKey, string friendlyNameTo, string friendlyNameFrom, Guid? treeGroup)
        {
            this.AccountID = accountID;
            this.FieldID = fieldID;
            this.TableID = tableID;
            this.ViewGroupID = viewGroupID;
            this.LookupFieldID = lookupFieldID;
            this.RelatedTableID = relatedTableID;
            this.LookupTableID = lookupTableID;
            this.FieldName = fieldName;
            this.FieldType = fieldType;
            this.Description = description;
            this.Comment = comment;
            this.NormalView = normalView;
            this.IDField = idField;
            this.GenList = genList;
            this.CanTotal = canTotal;
            this.Width = width;
            this.ValueList = valueList;
            this.AllowImport = allowImport;
            this.Mandatory = mandatory;
            this.PrintOut = printOut;
            this.Length = length;
            this.WorkflowUpdate = workflowUpdate;
            this.WorkflowSearch = workflowSearch;
            this.RelabelParam = relabelParam;
            this.ClassPropertyName = classPropertyName;
            this.ListItems = listItems;
            this.UseForLookup = useForLookup; // Whats this for? the strings will be set if it is, can check these in the useforlookup property?
            this.FieldSource = fieldSource;
            this.FieldCategory = fieldCat;
            this.IsForeignKey = isForeignKey;
            this.FriendlyNameTo = friendlyNameTo;
            this.FriendlyNameFrom = friendlyNameFrom;
            this.TreeGroup = treeGroup;
        }

        /// <summary>
        /// Instatiate a new version of <see cref="cField"/> based on an existing field
        /// </summary>
        /// <param name="field">The <see cref="cField"/> to copy</param>
        public cField(cField field)
        {
            if (field == null)
            {
                return;
            }

            this.AccountID = field.AccountID;
            this.FieldID = field.FieldID;
            this.TableID = field.TableID;
            this.ViewGroupID = field.ViewGroupID;
            this.LookupFieldID = field.LookupFieldID;
            this.RelatedTableID = field.RelatedTableID;
            this.LookupTableID = field.LookupTableID;
            this.FieldName = field.FieldName;
            this.FieldType = field.FieldType;
            this.Description = field.Description;
            this.Comment = field.Comment;
            this.NormalView = field.NormalView;
            this.IDField = field.IDField;
            this.GenList = field.GenList;
            this.CanTotal = field.CanTotal;
            this.Width = field.Width;
            this.ValueList = field.ValueList;
            this.AllowImport = field.AllowImport;
            this.Mandatory = field.Mandatory;
            this.PrintOut = field.PrintOut;
            this.Length = field.Length;
            this.WorkflowUpdate = field.WorkflowUpdate;
            this.WorkflowSearch = field.WorkflowSearch;
            this.RelabelParam = field.RelabelParam;
            this.ClassPropertyName = field.ClassPropertyName;
            this.ListItems = field.ListItems;
            this.UseForLookup = field.UseForLookup;
            this.FieldSource = field.FieldSource;
            this.FieldCategory = field.FieldCategory;
            this.IsForeignKey = field.IsForeignKey;
            this.FriendlyNameTo = field.FriendlyNameTo;
            this.FriendlyNameFrom = field.FriendlyNameFrom;
            this.TreeGroup = field.TreeGroup;
        }
    }
}
