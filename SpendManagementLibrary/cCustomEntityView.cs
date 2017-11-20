
namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SpendManagementLibrary.GreenLight;

    /// <summary>
    /// Represents custom entity view 
    /// </summary>
    public class cCustomEntityView
    {
        private int nViewid;
        private int nEntityid;
        private string sViewName;
        private string sDescription;
        private DateTime dtCreatedOn;
        private int nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;
        private int? nMenuid;
        private string _menuDescription;
        private bool bAllowAdd;
        private bool bShowRecordCount;
        private cCustomEntityForm clsAddForm;
        private bool bAllowEdit;
        private cCustomEntityForm clsEditForm;
        private bool bAllowDelete;
        private bool bAllowApproval;
        private GreenLightSortColumn oSortColumn;
        private string _MenuIcon;
        private bool bAllowArchive;
        SortedList<byte, cCustomEntityViewField> lstFields;
        SortedList<byte, FieldFilter> lstFilters;
        private readonly List<FormSelectionMapping> _formSelectionAttributeMappings;

        /// <summary>
        ///Initialises a new instance of the cCustomEntityView class 
        /// </summary>        
        /// <param name="viewid"></param>
        /// <param name="entityid"></param>
        /// <param name="viewname"></param>
        /// <param name="description"></param>
        /// <param name="builtIn"></param>
        /// <param name="systemCustomEntityViewId"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="menuid"></param>
        /// <param name="menuDescription"></param>
        /// <param name="showRecordCount"></param>
        /// <param name="fields"></param>
        /// <param name="allowadd"></param>
        /// <param name="addform"></param>
        /// <param name="allowedit"></param>
        /// <param name="editform"></param>
        /// <param name="allowdelete"></param>
        /// <param name="filters"></param>
        /// <param name="allowapproval"></param>
        /// <param name="allowarchive"></param>
        /// <param name="SortColumn"></param>
        /// <param name="menuIcon"></param>
        /// <param name="addFormMappings"></param>
        /// <param name="editFormMappings"></param>
        /// <param name="menuDisabledModuleIds"></param>
        public cCustomEntityView(int viewid, int entityid, string viewname, string description, bool builtIn, Guid? systemCustomEntityViewId, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, int? menuid, string menuDescription, bool showRecordCount, SortedList<byte, cCustomEntityViewField> fields, bool allowadd, cCustomEntityForm addform, bool allowedit, cCustomEntityForm editform, bool allowdelete, SortedList<byte, FieldFilter> filters, bool allowapproval, bool allowarchive, GreenLightSortColumn SortColumn, string menuIcon = null, List<FormSelectionMapping> addFormMappings = null, List<FormSelectionMapping> editFormMappings = null, List<int> menuDisabledModuleIds = null)
        {
            this.nViewid = viewid;
            this.nEntityid = entityid;
            this.sViewName = viewname;
            this.sDescription = description;
            this.dtCreatedOn = createdon;
            this.nCreatedBy = createdby;
            this.dtModifiedOn = modifiedon;
            this.nModifiedBy = modifiedby;
            this.nMenuid = menuid;
            this._menuDescription = menuDescription;
            this.bShowRecordCount = showRecordCount;
            this.bAllowAdd = allowadd;
            this.clsAddForm = addform;
            this.bAllowEdit = allowedit;
            this.clsEditForm = editform;
            this.bAllowDelete = allowdelete;
            this.bAllowApproval = allowapproval;
            this.oSortColumn = SortColumn;
            this.BuiltIn = builtIn;
            this.SystemCustomEntityViewId = systemCustomEntityViewId;

            this.lstFields = fields ?? new SortedList<byte, cCustomEntityViewField>();
            this.lstFilters = filters ?? new SortedList<byte, FieldFilter>();

            this._MenuIcon = menuIcon ?? "window_dialog.png";

            this.AddFormMappings = addFormMappings ?? new List<FormSelectionMapping>();
            this.EditFormMappings = editFormMappings ?? new List<FormSelectionMapping>();
            this.MenuDisabledModuleIds = menuDisabledModuleIds ?? new List<int>();
            this.bAllowArchive = allowarchive;
        }

        #region properties
        public int entityid
        {
            get { return this.nEntityid; }
        }
        public int viewid
        {
            get { return this.nViewid; }
        }
        public string viewname
        {
            get { return this.sViewName; }
        }
        public string description
        {
            get { return this.sDescription; }
        }
        public DateTime createdon
        {
            get { return this.dtCreatedOn; }
        }
        public int createdby
        {
            get { return this.nCreatedBy; }
        }
        public DateTime? modifiedon
        {
            get { return this.dtModifiedOn; }
        }
        public int? modifiedby
        {
            get { return this.nModifiedBy; }
        }
        public int? menuid
        {
            get { return this.nMenuid; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this is a system view
        /// </summary>
        public bool BuiltIn { get; private set; }

        /// <summary>
        /// Gets or sets a unique system identifier for this view, for System GreenLights
        /// </summary>
        public Guid? SystemCustomEntityViewId { get; set; }

        /// <summary>
        /// The flavour text on the underside of the menu option for the view
        /// </summary>
        public string MenuDescription
        {
            get { return this._menuDescription; }
        }

        /// <summary>
        /// Record count in view
        /// </summary>
        public bool showRecordCount
        {
            get { return this.bShowRecordCount; }
        }

        /// <summary>
        /// A list of module ids for which this view's menu item should be disabled
        /// </summary>
        public List<int> MenuDisabledModuleIds { get; private set; }

        public bool allowadd
        {
            get { return this.bAllowAdd; }
        }
        public cCustomEntityForm DefaultAddForm
        {
            get { return this.clsAddForm; }
        }
        public bool allowedit
        {
            get { return this.bAllowEdit; }
        }
        public cCustomEntityForm DefaultEditForm
        {
            get { return this.clsEditForm; }
        }
        public bool allowdelete
        {
            get { return this.bAllowDelete; }
        }       

        /// <summary>
        /// The forms that have been mapped for the FormSelectionAttribute on the Add for this view
        /// </summary>
        public List<FormSelectionMapping> AddFormMappings
        {
            get;
            set;
        }

        /// <summary>
        /// The forms that have been mapped for the FormSelectionAttribute on the Edit for this view
        /// </summary>
        public List<FormSelectionMapping> EditFormMappings
        {
            get;
            set;
        }

        /// <summary>
        /// The column the sort is used for
        /// </summary>
        public GreenLightSortColumn SortColumn
        {
            get { return this.oSortColumn; }
        }

        public SortedList<byte, cCustomEntityViewField> fields
        {
            get { return this.lstFields; }
        }
        public SortedList<byte, FieldFilter> filters
        {
            get { return this.lstFilters; }
        }
        public bool allowapproval
        {
            get { return this.bAllowApproval; }
        }
        /// <summary>
        /// Gets allow archive flag for View
        /// </summary>
        public bool allowarchive
        {
            get { return this.bAllowArchive; }
        }
        
        /// <summary>
        /// Gets the View's menu icon name
        /// </summary>
        public string MenuIcon
        {
            get { return this._MenuIcon; }
        }

        /// <summary>
        /// Gets the form to attribute value mappings
        /// </summary>
        public List<FormSelectionMapping> FormSelectionAttributeMappings
        {
            get
            {
                return this._formSelectionAttributeMappings;
            }
        }

        #endregion

        public bool containsField(Guid fieldid)
        {
            return (from x in this.lstFields.Values
                       where x.Field.FieldID == fieldid
                       select x).Count() > 0;
        }
               
    }
}