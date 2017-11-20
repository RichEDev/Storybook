using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// cCustomEntity class
    /// </summary>
    public class cCustomEntity
    {
        private int nEntityId;
        private string sEntityName;
        private string sPluralName;
        private string sDescription;
        private DateTime dtCreatedOn;
        private int nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedby;
        private bool bEnableAttachments;
        private AudienceViewType _audienceViewType;
        private bool bAllowMergeConfig;
        private bool bIsSystemView;
        private int? nSystemView_DerivedEntityId;
        private int? nSystemView_EntityId;
        private bool bEnableCurrencies;
        private int? nDefaultCurrencyID;
        private bool _enablePopupWindow;
        private int? _defaultPopupView;
        private int? _ownerId;
        private int? _supportContactId;
        private string _supportQuestion;

        private cTable clstable;
        private cTable clsAudienceTable;
        private SortedList<int, cAttribute> lstAttributes;
        private SortedList<int, cCustomEntityForm> lstForms;
        private SortedList<int, cCustomEntityView> lstViews;

        /// <summary>
        /// Initializes a new instance of the <see cref="cCustomEntity"/> class. 
        /// cCustomEntity constructor
        /// </summary>
        /// <param name="entityid">
        /// </param>
        /// <param name="entityname">
        /// </param>
        /// <param name="pluralname">
        /// </param>
        /// <param name="description">
        /// </param>
        /// <param name="createdon">
        /// </param>
        /// <param name="createdby">
        /// </param>
        /// <param name="modifiedon">
        /// </param>
        /// <param name="modifiedby">
        /// </param>
        /// <param name="attributes">
        /// </param>
        /// <param name="forms">
        /// </param>
        /// <param name="views">
        /// </param>
        /// <param name="table">
        /// </param>
        /// <param name="audienceTable">
        /// </param>
        /// <param name="enableattachments">
        /// </param>
        /// <param name="audienceViewType">
        /// </param>
        /// <param name="allowdocmergeaccess">
        /// </param>
        /// <param name="systemview">
        /// </param>
        /// <param name="system_derivedentityid">
        /// </param>
        /// <param name="systemview_entityid">
        /// </param>
        /// <param name="enableCurrencies">
        ///     The enable Currencies.
        /// </param>
        /// <param name="defaultCurrencyID">
        ///     The default Currency ID.
        /// </param>
        /// <param name="enablePopupWindow">
        ///     The enable popup window
        /// </param>
        /// <param name="defaultPopupView">
        ///     The default view for the popup window
        /// </param>
        /// <param name="formSelectionAttributeId"></param>
        /// <param name="ownerId"></param>
        /// <param name="supportContactId"></param>
        /// <param name="supportQuestion"></param>
        /// <param name="enableLocking"></param>
        /// <param name="builtIn"></param>
        public cCustomEntity(int entityid, string entityname, string pluralname, string description, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, SortedList<int, cAttribute> attributes, SortedList<int, cCustomEntityForm> forms, SortedList<int, cCustomEntityView> views, cTable table, cTable audienceTable, bool enableattachments, AudienceViewType audienceViewType, bool allowdocmergeaccess, bool systemview, int? system_derivedentityid, int? systemview_entityid, bool enableCurrencies, int? defaultCurrencyID, bool enablePopupWindow, int? defaultPopupView, int? formSelectionAttributeId, int? ownerId, int? supportContactId, string supportQuestion, bool enableLocking, bool builtIn, Guid? systemCustomEntityId = null)
        {
            this.nEntityId = entityid;
            this.sEntityName = entityname;
            this.sPluralName = pluralname;
            this.sDescription = description;
            this.dtCreatedOn = createdon;
            this.nCreatedBy = createdby;
            this.dtModifiedOn = modifiedon;
            this.nModifiedby = modifiedby;
            this.clstable = table;
            this.clsAudienceTable = audienceTable;
            this.bEnableAttachments = enableattachments;
            this._audienceViewType = audienceViewType;
            this.bAllowMergeConfig = allowdocmergeaccess;
            this.bIsSystemView = systemview;
            this.nSystemView_DerivedEntityId = system_derivedentityid;
            this.nSystemView_EntityId = systemview_entityid;
            this.bEnableCurrencies = enableCurrencies;
            this.nDefaultCurrencyID = defaultCurrencyID;
            this._enablePopupWindow = enablePopupWindow;
            this._defaultPopupView = defaultPopupView;
            this.FormSelectionAttributeId = formSelectionAttributeId;
            this.EnableLocking = enableLocking;
            this._ownerId = ownerId;
            this._supportContactId = supportContactId;
            this._supportQuestion = supportQuestion;
            this.BuiltIn = builtIn;
            this.SystemCustomEntityId = systemCustomEntityId;

            this.lstAttributes = attributes ?? new SortedList<int, cAttribute>();
            this.lstForms = forms ?? new SortedList<int, cCustomEntityForm>();
            this.lstViews = views ?? new SortedList<int, cCustomEntityView>();
        }

        /// <summary>
        /// getAttributeById: get an attribute by it's ID
        /// </summary>
        /// <param name="id">ID of attibute to retrieve</param>
        /// <returns>cAttribute entity</returns>
        public cAttribute getAttributeById(int id)
        {
            cAttribute attribute = null;
            lstAttributes.TryGetValue(id, out attribute);
            return attribute;
        }

        /// <summary>
        /// CreateFormDropDown: obtain available forms for drop down list
        /// </summary>
        /// <param name="includeNone">Optionally include a [None] option at the top of the list with ID zero</param>
        /// <returns>List of list items</returns>
        public List<ListItem> CreateFormDropDown(bool includeNone = false)
        {
            //SortedList<string, cCustomEntityForm> forms = sortForms();

            List<ListItem> items = (from x in lstForms.Values
                                    orderby x.formname
                                    select new ListItem(x.formname, x.formid.ToString())).ToList();

            if (includeNone)
                items.Insert(0, new ListItem("[None]", "0"));

            //foreach (cCustomEntityForm form in forms.Values)
            //{
            //    items.Add(new ListItem(form.formname, form.formid.ToString()));
            //}
            return items;
        }

        /// <summary>
        /// sortForms: Sort entity forms by form name
        /// </summary>
        /// <returns>A collection of forms with the form name as the key</returns>
        public SortedList<string, cCustomEntityForm> sortForms()
        {
            SortedList<string, cCustomEntityForm> sorted = new SortedList<string, cCustomEntityForm>((from x in lstForms.Values
                                                                                                      orderby x.formname
                                                                                                      select x).ToDictionary(a => a.formname));
            //foreach (cCustomEntityForm form in lstForms.Values)
            //{
            //    sorted.Add(form.formname, form);
            //}
            return sorted;
        }

        /// <summary>
        /// getFormById: Get a custom entity form by its id
        /// </summary>
        /// <param name="id">ID of the form to be retrieved</param>
        /// <returns>A Custom Entity Form class object</returns>
        public cCustomEntityForm getFormById(int id)
        {
            cCustomEntityForm form = null;
            lstForms.TryGetValue(id, out form);
            return form;
        }

        /// <summary>
        /// getFormById: Get a custom entity form by its system guid
        /// </summary>
        /// <param name="systemFormId">guid of the form to be retrieved</param>
        /// <returns>A Custom Entity Form class object</returns>
        public cCustomEntityForm GetFormByGuid(Guid systemFormId)
        {
            cCustomEntityForm customEntityForm = null;
            customEntityForm = (from form in this.lstForms.Values
                                where form.SystemCustomEntityFormId == systemFormId
                                select form).FirstOrDefault();
            return customEntityForm;
        }

        /// <summary>
        /// getViewsByMenuId: Get views associated with a particular menu option
        /// </summary>
        /// <param name="id">ID of menu option associated</param>
        /// <param name="forMobile">Optionally only return views that contain at least one field (attribute) that can be displayed on a mobile device.</param>
        /// <returns>List of view associated with the particular menu option</returns>
        public List<cCustomEntityView> getViewsByMenuId(int id, bool forMobile = false)
        {
            var views = new List<cCustomEntityView>();

            foreach (cCustomEntityView view in this.lstViews.Values.Where(view => view.menuid == id))
            {
                if (!forMobile || this.lstAttributes.Any(attribute => attribute.Value.DisplayInMobile && view.containsField(attribute.Value.fieldid)))
                {
                    views.Add(view);
                }
            }
            return views;
        }

        /// <summary>
        /// Returns the number of views contained in the menu with the given menuId.
        /// </summary>
        /// <param name="menuId">
        /// Id value of the menu.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetViewCountForMenuId(int menuId)
        {
            return (from x in this.lstViews.Values
                    where x.menuid == menuId
                    select x).Count();
        }

        /// <summary>
        /// getViewById: Get a view by it's id
        /// </summary>
        /// <param name="id">ID of view to retrieve</param>
        /// <returns>Custom Entity View class object</returns>
        public cCustomEntityView getViewById(int id)
        {
            cCustomEntityView view = null;
            lstViews.TryGetValue(id, out view);
            return view;
        }

        /// <summary>
        /// getKeyField: Get the attribute tagged as the key field
        /// </summary>
        /// <returns>cAttribute entity that is the key field</returns>
        public cAttribute getKeyField()
        {
            cAttribute att = (from x in lstAttributes.Values
                              where x.iskeyfield
                              select x).FirstOrDefault();
            return att;
        }

        /// <summary>
        /// getKeyField: Get the attribute tagged as the key field
        /// </summary>
        /// <returns>cAttribute entity that is the key field</returns>
        public cAttribute getGreenLightCurrencyAttribute()
        {
            cAttribute att = (from x in lstAttributes.Values
                              where x.IsSystemAttribute && x.attributename == "GreenLightCurrency"
                              select x).FirstOrDefault();
            return att;
        }

        /// <summary>
        /// Get the attribute tagged as the audit field identifier
        /// </summary>
        /// <returns>cAttribute entity that is the audit field identifier</returns>
        public cAttribute getAuditIdentifier()
        {
            cAttribute reqAttribute = (from x in lstAttributes.Values
                                       where x.isauditidentifer
                                       select x).FirstOrDefault();

            if (reqAttribute == null)
            {
                // not defined, so return the key field
                reqAttribute = (from x in lstAttributes.Values
                                where x.iskeyfield == true
                                select x).FirstOrDefault();
            }

            return reqAttribute;
        }

        /// <summary>
        /// Checks to see if any views have mappings
        /// </summary>
        /// <returns>boolean indicating whether any mappings are present</returns>
        public bool HasFormSelectionMappings()
        {
            return lstViews.Values.Any(x => x.AddFormMappings.Count > 0 || x.EditFormMappings.Count > 0);
        }

        #region properties
        /// <summary>
        /// Entity ID
        /// </summary>
        public int entityid
        {
            get { return nEntityId; }
        }
        /// <summary>
        /// Entity name
        /// </summary>
        public string entityname
        {
            get { return sEntityName; }
        }
        /// <summary>
        /// Pluralised definition of the entity
        /// </summary>
        public string pluralname
        {
            get { return sPluralName; }
        }
        /// <summary>
        /// Entity Description
        /// </summary>
        public string description
        {
            get { return sDescription; }
        }
        /// <summary>
        /// Date entity was created
        /// </summary>
        public DateTime createdon
        {
            get { return dtCreatedOn; }
        }
        /// <summary>
        /// User who created the entity record
        /// </summary>
        public int createdby
        {
            get { return nCreatedBy; }
        }
        /// <summary>
        /// Date entity record was last modified
        /// </summary>
        public DateTime? modifiedon
        {
            get { return dtModifiedOn; }
        }
        /// <summary>
        /// Last user to modify the entity record
        /// </summary>
        public int? modifiedby
        {
            get { return nModifiedby; }
        }
        /// <summary>
        /// Table class for the entity
        /// </summary>
        public cTable table
        {
            get { return clstable; }
        }
        /// <summary>
        /// Audience Table class for the entity
        /// </summary>
        public cTable AudienceTable
        {
            get { return clsAudienceTable; }
        }
        /// <summary>
        /// Collection of attributes associated with the entity
        /// </summary>
        public SortedList<int, cAttribute> attributes
        {
            get { return lstAttributes; }
        }
        /// <summary>
        /// Permit access to the file attachment capability
        /// </summary>
        public bool EnableAttachments
        {
            get { return bEnableAttachments; }
        }
        /// <summary>
        /// Permit access to the audiences capability
        /// </summary>
        public AudienceViewType AudienceView
        {
            get { return _audienceViewType; }
        }
        /// <summary>
        /// Permit access to the Torch tab
        /// </summary>
        public bool AllowMergeConfigAccess
        {
            get { return bAllowMergeConfig; }
        }
        /// <summary>
        /// Indicates that entity is a filtered view of a donor entity
        /// </summary>
        public bool IsSystemView
        {
            get { return bIsSystemView; }
        }
        /// <summary>
        /// SystemView_DerivedEntityId is the donor entity when entity is a system view
        /// </summary>
        public int? SystemView_DerivedEntityId
        {
            get { return nSystemView_DerivedEntityId; }
        }
        /// <summary>
        /// Gets the parent entity that the systemview is representing
        /// </summary>
        public int? SystemView_EntityId
        {
            get { return nSystemView_EntityId; }
        }
        /// <summary>
        /// Gets the list of forms associated with this entity
        /// </summary>
        public SortedList<int, cCustomEntityForm> Forms
        {
            get { return lstForms; }
        }
        /// <summary>
        /// Gets the list of views associated with this entity
        /// </summary>
        public SortedList<int, cCustomEntityView> Views
        {
            get { return lstViews; }
        }
        /// <summary>
        /// Indicates whether the entity has currency fields that need to be formatted to a different currency or format than the default
        /// </summary>
        public bool EnableCurrencies
        {
            get { return bEnableCurrencies; }
        }
        /// <summary>
        /// Default currency ID to use if one is not specified
        /// </summary>
        public int? DefaultCurrencyID
        {
            get { return nDefaultCurrencyID; }
        }
        /// <summary>
        /// Indicates if the Open in New Window is enabled on the custom entity
        /// </summary>
        public bool EnablePopupWindow
        {
            get { return _enablePopupWindow; }
        }
        /// <summary>
        /// Default view for the Pop-up window grid.
        /// </summary>
        public int? DefaultPopupView
        {
            get { return _defaultPopupView; }
        }

        /// <summary>
        /// The attributes that may be used as a form selection mapping attribute
        /// </summary>
        public List<cAttribute> FormSelectionAttributes
        {
            get
            {
                return attributes.Values.Where(a => a.fieldtype == FieldType.List || (a.fieldtype == FieldType.Text && (((cTextAttribute)a).format == AttributeFormat.SingleLine || ((cTextAttribute)a).format == AttributeFormat.SingleLineWide))).ToList();
            }
        }

        /// <summary>
        /// The attribute that, the value of which at the time of use, is used for the selection of which form to use from a view
        /// </summary>
        public int? FormSelectionAttributeId
        {
            get;
            set;
        }

        /// <summary>
        /// Enable single user pessimistic locking on this custom entity.
        /// </summary>
        public bool EnableLocking { get; set; }


        /// <param name="ownerId">
        /// The owner of the custom entity
        /// </param>
        public int? OwnerId
        {
            get { return _ownerId; }
        }

        /// <param name="supportContactId">
        /// The user who is the support contact for this custom entity (used in Help & Support)
        /// </param>
        public int? SupportContactId
        {
            get { return _supportContactId; }
        }

        /// <param name="supportQuestion">
        /// The support question/statement for this custom entity (used in Help & Support)
        /// </param>
        public string SupportQuestion
        {
            get { return _supportQuestion; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this GreenLight is built-in.
        /// Built in GreenLights (and their built-in sub components) can be copied between customer databases, although the functionality to do so is not provided within this product.
        /// Only "adminonly" employees can set this value, once set it cannot be un-set.
        /// </summary>
        public bool BuiltIn { get; set; }

        /// <summary>
        /// The Unique ID for the System Green Lights , will be comomon across all databases.
        /// </summary>
        public Guid? SystemCustomEntityId { get; set; }

        #endregion

        /// <summary>
        /// Returns a collection of views for the entity sorted by view name
        /// </summary>
        /// <returns></returns>
        public SortedList<string, cCustomEntityView> sortViews()
        {
            SortedList<string, cCustomEntityView> sorted = new SortedList<string, cCustomEntityView>((from x in this.lstViews.Values
                                                                                                      orderby x.viewname
                                                                                                      select x).ToDictionary(a => a.viewname));
            //foreach (cCustomEntityView view in lstViews.Values)
            //{
            //    sorted.Add(view.viewname, view);
            //}
            return sorted;
        }

        /// <summary>
        /// Retrieve a 1:n relationship entity for a particular entity
        /// </summary>
        /// <param name="entityid">ID of the entity to retrieve the relationship for</param>
        /// <returns>cOneToManyRelationship class element</returns>
        public List<cOneToManyRelationship> findOneToManyRelationship(int entityid)
        {
            List<cAttribute> onetomany = (from x in this.lstAttributes.Values
                                    where x.GetType() == typeof(cOneToManyRelationship) && ((cOneToManyRelationship)x).entityid == entityid
                                    select x).ToList();

            var oneToManyRelationships = new List<cOneToManyRelationship>();
            if (onetomany.Count > 0)
            {
                foreach (var item in onetomany)
                {
                    oneToManyRelationships.Add((cOneToManyRelationship)item);
                }
            }

            //If oneToManyRelationships has 0 count then return null
            if (oneToManyRelationships.Count > 0)
            {
                return oneToManyRelationships;
            }
            
            return null;

            //foreach (cAttribute att in lstAttributes.Values)
            //{
            //    if (att.GetType() == typeof(cOneToManyRelationship))
            //    {
            //        onetomany = (cOneToManyRelationship)att;
            //        if (onetomany.entityid == entityid)
            //        {
            //            return onetomany;
            //        }
            //    }
            //}

            //return null;
        }

        /// <summary>
        /// Returns an array of attribute IDs where the is_unique is true
        /// </summary>
        /// <returns>Array of attribute IDs</returns>
        public int[] getUniqueAttributes()
        {
            bool addID = false;
            List<int> retIDs = new List<int>();

            foreach (KeyValuePair<int, cAttribute> i in this.lstAttributes)
            {
                cAttribute curAtt = (cAttribute)i.Value;
                addID = false;

                switch (curAtt.fieldtype)
                {
                    case FieldType.LargeText:
                    case FieldType.Text:
                        if (((cTextAttribute)curAtt).isunique)
                        {
                            addID = true;
                        }
                        break;
                    case FieldType.DateTime:
                        if (((cDateTimeAttribute)curAtt).isunique)
                        {
                            addID = true;
                        }
                        break;
                    case FieldType.List:
                        if (((cListAttribute)curAtt).isunique)
                        {
                            addID = true;
                        }
                        break;
                    case FieldType.Currency:
                    case FieldType.Number:
                    case FieldType.Integer:
                        if (((cNumberAttribute)curAtt).isunique)
                        {
                            addID = true;
                        }
                        break;
                    case FieldType.TickBox:
                        if (((cTickboxAttribute)curAtt).isunique)
                        {
                            addID = true;
                        }
                        break;
                    case FieldType.Contact:
                        if (((cContactAttribute) curAtt).isunique)
                        {
                            addID = true;
                        }
                        break;
                    default:
                        break;
                }

                if (addID)
                {
                    retIDs.Add(curAtt.attributeid);
                }

            }

            return retIDs.ToArray();
        }

        /// <summary>
        /// Gets an array for field control ids for unique controls in an entity that MAY be on the form
        /// </summary>
        /// <returns>String array of field names</returns>
        public string[] getUniqueAttributesFieldNames()
        {
            List<string> retIDs = new List<string>();

            foreach (KeyValuePair<int, cAttribute> i in this.lstAttributes)
            {
                cAttribute curAtt = (cAttribute)i.Value;

                switch (curAtt.fieldtype)
                {
                    case FieldType.LargeText:
                    case FieldType.Text:
                        if (((cTextAttribute)curAtt).isunique)
                        {
                            retIDs.Add("txt" + ((cTextAttribute)curAtt).attributeid.ToString());
                        }
                        break;
                    case FieldType.DateTime:
                        if (((cDateTimeAttribute)curAtt).isunique)
                        {
                            retIDs.Add("txt" + ((cDateTimeAttribute)curAtt).attributeid.ToString());
                        }
                        break;
                    case FieldType.List:
                        if (((cListAttribute)curAtt).isunique)
                        {
                            retIDs.Add("cmb" + ((cListAttribute)curAtt).attributeid.ToString());
                        }
                        break;
                    case FieldType.Number:
                    case FieldType.Integer:
                        if (((cNumberAttribute)curAtt).isunique)
                        {
                            retIDs.Add("txt" + ((cNumberAttribute)curAtt).attributeid.ToString());
                        }
                        break;
                    case FieldType.TickBox:
                        if (((cTickboxAttribute)curAtt).isunique)
                        {
                            retIDs.Add("cmb" + ((cTickboxAttribute)curAtt).attributeid.ToString());
                        }
                        break;
                    default:
                        break;
                }
            }

            return retIDs.ToArray();
        }

        /// <summary>
        /// Get the attribute given by the name specified
        /// </summary>
        /// /// <param name="name">The name of the entity to return</param>
        /// <returns>Attribute found or null if not found</returns>
        public cAttribute getAttributeByName(string name)
        {
            return (cAttribute)(from x in this.lstAttributes.Values
                                where x.attributename.ToLower() == name.ToLower()
                                select x).FirstOrDefault();
        }

        /// <summary>
        /// Get the attribute given by the field id specified
        /// </summary>
        /// <param name="fieldId">The <see cref="Guid"/> identifier of the attribute to return</param>
        /// <returns>Attribute found or null if not found</returns>
        public cAttribute GetAttributeByFieldId(Guid fieldId)
        {
            return (cAttribute)(from x in this.lstAttributes.Values
                                where x.fieldid == fieldId
                                select x).FirstOrDefault();
        }

        /// <summary>
        /// Get the attribute given by the displayname specified
        /// </summary>
        /// <param name="fieldId">The <see cref="Guid"/> identifier of the attribute to return</param>
        /// <returns>Attribute found or null if not found</returns>
        public cAttribute GetAttributeByDisplayName(string displayname)
        {
            return (cAttribute)(from x in this.lstAttributes.Values
                                where x.displayname == displayname
                                select x).FirstOrDefault();
        }


        /// <summary>
        /// get lookup display field by id.
        /// </summary>
        /// <param name="attributeId">
        /// The attribute id to find.
        /// </param>
        /// <returns>
        /// The <see cref="cAttribute"/>.
        /// </returns>
        public cAttribute GetLookupDisplayFieldById(int attributeId)
        {
            return this.attributes.Where(attribute => attribute.Value.GetType() == typeof(cManyToOneRelationship)).Select(attribute => (cManyToOneRelationship)attribute.Value).SelectMany(manyToOne => manyToOne.TriggerLookupFields).FirstOrDefault(lookupDisplayField => lookupDisplayField.attributeid == attributeId);
        }

        /// <summary>
        /// Retrieves the ID of the employee who is listed as the "support contact" for a particular greenlight
        /// </summary>
        /// <param name="customEntityId">The identifier of the custom entity</param>
        /// <param name="user">The current user</param>
        /// <returns>An employee ID, or null if the greenlight has no support contact defined</returns>
        public static int? GetSupportContactIdByEntityId(int customEntityId, ICurrentUserBase user)
        {
            int? ownerId = null;

            using (IDBConnection connection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID)))
            {
                const string Sql = "SELECT supportContactId FROM customEntities WHERE entityid = @entityId";
                connection.AddWithValue("@entityId", customEntityId);

                using (IDataReader reader = connection.GetReader(Sql))
                {
                    int ownerIdOrdinal = reader.GetOrdinal("supportContactId");

                    if (reader.Read() && reader.IsDBNull(ownerIdOrdinal) == false)
                    {
                        ownerId = reader.GetInt32(ownerIdOrdinal);
                    }
                }
            }

            return ownerId;
        }
    }

    /// <summary>
    /// cAttribute base class
    /// </summary>
    public abstract class cAttribute
    {
        protected int nAttributeid;
        protected string sAttributeName;
        protected string sDisplayName;
        protected string sDescription;
        protected string sTooltip;
        protected bool bMandatory;
        protected FieldType ftFieldType;
        protected DateTime dtCreatedOn;
        protected int nCreatedBy;
        protected DateTime? dtModifiedOn;
        protected int? nModifiedBy;
        protected Guid gFieldid;
        protected bool bIsKeyField;
        protected bool bIsAuditIdentity;
        protected bool bIsUnique;
        protected bool bAllowEdit;
        protected bool bAllowDelete;
        protected bool bSystemAttribute;
        private readonly bool _boolAttribute;

        /// <summary>
        /// cAttribute constructor
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
        /// <param name="iskeyfield">Is attribute a key field</param>
        /// <param name="isauditidentity">Is attribute used as field identifier in the audit log</param>
        /// <param name="isunique">Is value to be unique</param>
        /// <param name="allowEdit"></param>
        /// <param name="allowDelete"></param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="builtIn">Whether the attribute is a system attribute</param>
        /// <param name="boolAttribute">A generic boolean attribute used for different attributes.</param>
        /// <param name="system_attribute">Indicates whether the attribute is generated by the application and not a user</param>
        public cAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, bool iskeyfield, bool isauditidentity, bool isunique, bool allowEdit, bool allowDelete, bool displayInMobile, bool builtIn, bool boolAttribute = false, bool system_attribute = false)
        {
            nAttributeid = attributeid;
            sAttributeName = attributename;
            sDisplayName = displayname;
            sDescription = description;
            sTooltip = tooltip;
            bMandatory = mandatory;
            this.DisplayInMobile = displayInMobile;
            ftFieldType = fieldtype;
            dtCreatedOn = createdon;
            nCreatedBy = createdby;
            dtModifiedOn = modifiedon;
            nModifiedBy = modifiedby;
            gFieldid = fieldid;
            bIsKeyField = iskeyfield;
            bIsAuditIdentity = isauditidentity;
            bIsUnique = isunique;
            bAllowEdit = allowEdit;
            bAllowDelete = allowDelete;
            bSystemAttribute = system_attribute;
            _boolAttribute = boolAttribute;
            BuiltIn = builtIn;

        }
        #region properties
        /// <summary>
        /// Get attribute Id
        /// </summary>
        public int attributeid
        {
            get { return nAttributeid; }
        }
        /// <summary>
        /// Get attribute name
        /// </summary>
        public string attributename
        {
            get { return sAttributeName; }
        }
        /// <summary>
        /// Get friendly display name of attribute
        /// </summary>
        public string displayname
        {
            get { return sDisplayName; }
        }
        /// <summary>
        /// Get attribute description
        /// </summary>
        public string description
        {
            get { return sDescription; }
        }
        /// <summary>
        /// Get associated tool tip
        /// </summary>
        public string tooltip
        {
            get { return sTooltip; }
        }
        /// <summary>
        /// Get mandatory indication
        /// </summary>
        public bool mandatory
        {
            get { return bMandatory; }
        }

        /// <summary>
        ///  Gets a value indicating whether the attribute should be displayed to mobile users.
        /// </summary>
        public bool DisplayInMobile { get; private set; }

        /// <summary>
        /// Get field type
        /// </summary>
        public FieldType fieldtype
        {
            get { return ftFieldType; }
        }
        /// <summary>
        /// Get attribute creation date
        /// </summary>
        public DateTime createdon
        {
            get { return dtCreatedOn; }
        }
        /// <summary>
        /// Get User ID of attribute creator
        /// </summary>
        public int createdby
        {
            get { return nCreatedBy; }
        }
        /// <summary>
        /// Get attribute last modification date
        /// </summary>
        public DateTime? modifiedon
        {
            get { return dtModifiedOn; }
        }
        /// <summary>
        /// Get User ID of attributes modifer
        /// </summary>
        public int? modifiedby
        {
            get { return nModifiedBy; }
        }
        /// <summary>
        /// Get field id for reporting of attribute
        /// </summary>
        public Guid fieldid
        {
            get { return gFieldid; }
        }
        /// <summary>
        /// Get whether the attribute is a key field
        /// </summary>
        public bool iskeyfield
        {
            get { return bIsKeyField; }
        }
        /// <summary>
        /// Get whether attribute is used as audit log field identifier
        /// </summary>
        public bool isauditidentifer
        {
            get { return bIsAuditIdentity; }
        }
        /// <summary>
        /// Gets whether the attribute value is to be unique
        /// </summary>
        public bool isunique
        {
            get { return bIsUnique; }
        }
        /// <summary>
        /// Gets whether the attribute can be edited
        /// </summary>
        public bool AllowEdit
        {
            get { return bAllowEdit; }
        }
        /// <summary>
        /// Gets whether the attribute can be deleted
        /// </summary>
        public bool AllowDelete
        {
            get { return bAllowDelete; }
        }
        /// <summary>
        /// Gets whether the attribute was generated by the application and not a user
        /// </summary>
        public bool IsSystemAttribute
        {
            get { return bSystemAttribute; }
        }

        /// <summary>
        /// A generic boolean attribute for the attribute.
        /// Attachment attribute - include image library
        /// Formatted text box - strip font tags from html before save.
        /// </summary>
        public bool BoolAttribute
        {
            get { return _boolAttribute; }
        }

        /// <summary>
        /// Gets or sets whether the attribute is a system attribute
        /// </summary>
        public bool BuiltIn { get; set; }
        #endregion


        #region GenerateField Fields
        protected string sFieldIDPrefix = "txt";
        protected int nMaxLabelTextLength = 20;
        protected string sCssClass = "twocolumn";

        #endregion GenerateField Fields

        #region GenerateField Properties
        public string ControlID { get { return sFieldIDPrefix + nAttributeid; } }
        public int MaxLabelTextLength { get { return nMaxLabelTextLength; } }
        public string DivCssClass { get { return sCssClass; } }
        #endregion GenerateField Properties

    }

    /// <summary>
    /// cHyperlinkAttribute class which inherits cAttribute
    /// </summary>
    public class cHyperlinkAttribute : cAttribute
    {
        private string sHyperlinkText;
        private string sHyperlinkPath;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="fieldtype"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="fieldid"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="isunique"></param>
        /// <param name="hyperlinkPath"></param>
        /// <param name="hyperlinkText"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute"></param>
        public cHyperlinkAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, bool isauditidentity, bool isunique, string hyperlinkText, string hyperlinkPath, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn, bool system_attribute = false)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, isunique, allowedit, allowdelete, displayInMobile, builtIn, system_attribute: system_attribute)
        {
            sHyperlinkText = hyperlinkText;
            sHyperlinkPath = hyperlinkPath;
        }

        #region properties
        /// <summary>
        /// Gets the URL or file path value for the hyperlink 
        /// </summary>
        public string hyperlinkText
        {
            get { return sHyperlinkText; }
        }

        /// <summary>
        /// Gets the URL or file path value for the hyperlink 
        /// </summary>
        public string hyperlinkPath
        {
            get { return sHyperlinkPath; }
        }
        #endregion
    }

    /// <summary>
    /// cTextAttribute class which inherits cAttribute
    /// </summary>
    public class cTextAttribute : cAttribute
    {
        private AttributeFormat afFormat;
        private int? nMaxLength;

        /// <summary>
        /// cTextAttribute constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="fieldtype"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="maxlength"></param>
        /// <param name="format"></param>
        /// <param name="fieldid"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="isunique"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="removeFonts">If true a formatted text box will remove the foncts on save.</param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute"></param>
        public cTextAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, int? maxlength, AttributeFormat format, Guid fieldid, bool isauditidentity, bool isunique, bool allowedit, bool allowdelete, bool removeFonts, bool displayInMobile, bool builtIn, bool system_attribute = false)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, isunique, allowedit, allowdelete, displayInMobile, builtIn, boolAttribute: removeFonts, system_attribute: system_attribute)
        {
            nMaxLength = maxlength;
            afFormat = format;

            if (format == AttributeFormat.FormattedText || format == AttributeFormat.MultiLine)
            {
                sCssClass = (format == AttributeFormat.FormattedText) ? "onecolumnlarge" : "onecolumn";
                nMaxLabelTextLength = 50;
            }
            if (format == AttributeFormat.SingleLineWide)
            {
                sCssClass = "onecolumnsmall";
            }
        }

        #region properties
        /// <summary>
        /// Gets format type of text attribute
        /// </summary>
        public AttributeFormat format
        {
            get { return afFormat; }
        }
        /// <summary>
        /// Gets maximum length permitted for text attribute
        /// </summary>
        public int? maxlength
        {
            get { return nMaxLength; }
        }

        public bool RemoveFonts
        {
            get { return base.BoolAttribute; }
        }

        #endregion
    }

    /// <summary>
    /// cDateTimeAttribute class which inherits cAttribute
    /// </summary>
    public class cDateTimeAttribute : cAttribute
    {
        private AttributeFormat afFormat;

        /// <summary>
        /// cDateTimeAttribute constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="fieldtype"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="isunique"></param>
        /// <param name="format"></param>
        /// <param name="fieldid"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute"></param>
        public cDateTimeAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, AttributeFormat format, Guid fieldid, bool isauditidentity, bool isunique, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn, bool system_attribute = false)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, isunique, allowedit, allowdelete, displayInMobile, builtIn, system_attribute: system_attribute)
        {
            afFormat = format;
        }

        #region properties
        /// <summary>
        /// Gets the date format for attribute
        /// </summary>
        public AttributeFormat format
        {
            get { return afFormat; }
        }
        #endregion
    }

    /// <summary>
    /// cNumberAttribute class which inherits cAttribute
    /// </summary>
    public class cNumberAttribute : cAttribute
    {
        private byte nPrecision;

        /// <summary>
        /// cNumberAttribute constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="fieldtype"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="precision"></param>
        /// <param name="fieldid"></param>
        /// <param name="iskeyfield"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="isunique"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute"></param>
        public cNumberAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, byte precision, Guid fieldid, bool iskeyfield, bool isauditidentity, bool isunique, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn, bool system_attribute = false)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, fieldid, iskeyfield, isauditidentity, isunique, allowedit, allowdelete, displayInMobile, builtIn, system_attribute: system_attribute)
        {
            nPrecision = precision;
        }

        #region properties
        /// <summary>
        /// Gets the precision of the number attribute
        /// </summary>
        public byte precision
        {
            get { return nPrecision; }
        }
        #endregion
    }

    [Serializable]
    public class cListAttributeElement
    {
        /// <summary>
        /// The n value.
        /// </summary>
        [NonSerialized]
        private int nValue;

        /// <summary>
        /// The s text.
        /// </summary>
        [NonSerialized]
        private string sText;

        /// <summary>
        /// The n sequence.
        /// </summary>
        [NonSerialized]
        private int nSequence;

        #region properties

        /// <summary>
        /// Gets the element value.
        /// </summary>
        public int elementValue
        {
            get { return nValue; }
        }

        /// <summary>
        /// Gets the element text.
        /// </summary>
        public string elementText
        {
            get { return sText; }
        }

        /// <summary>
        /// Gets the element order.
        /// </summary>
        public int elementOrder
        {
            get { return nSequence; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether archived.
        /// </summary>
        public bool Archived { get; set; }
        #endregion

        /// <summary>
        /// Initialises a new instance of the <see cref="cListAttributeElement"/> class.
        /// </summary>
        /// <param name="elementValue">
        /// The element value.
        /// </param>
        /// <param name="elementText">
        /// The element text.
        /// </param>
        /// <param name="sequence">
        /// The sequence.
        /// </param>
        /// <param name="Archived">
        /// The Archived flag.
        /// </param>
        public cListAttributeElement(int elementValue, string elementText, int sequence, bool Archived = false)
        {
            this.Archived = Archived;
            this.sText = elementText;
            this.nValue = elementValue;
            this.nSequence = sequence;
        }
    }

    /// <summary>
    /// cListAttribute class which inherits cAttribute
    /// </summary>
    public class cListAttribute : cAttribute
    {
        [NonSerialized()]
        private SortedList<int, cListAttributeElement> lstItems;

        /// <summary>
        /// cListAttribute constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="fieldtype"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="items"></param>
        /// <param name="fieldid"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="isunique"></param>
        /// <param name="listformat"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute"></param>
        public cListAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, SortedList<int, cListAttributeElement> items, Guid fieldid, bool isauditidentity, bool isunique, AttributeFormat listformat, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn, bool system_attribute = false)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, isunique, allowedit, allowdelete, displayInMobile, builtIn, system_attribute: system_attribute)
        {
            format = listformat;
            lstItems = items;
            sFieldIDPrefix = "cmb";
            if (format == AttributeFormat.ListWide)
            {
                sCssClass = "onecolumnsmall";
            }
            else
            {
                sCssClass = "twocolumn";
            }
        }

        #region properties
        /// <summary>
        /// Get the list of items for the attribute
        /// </summary>
        public SortedList<int, cListAttributeElement> items
        {
            get { return lstItems; }
        }
        public AttributeFormat format { get; set; }
        #endregion
    }

    /// <summary>
    /// cTickboxAttribute class which inherits cAttribute
    /// </summary>
    public class cTickboxAttribute : cAttribute
    {
        private string sDefaultValue;

        /// <summary>
        /// cTickboxAttribute constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="fieldtype"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="defaultvalue"></param>
        /// <param name="fieldid"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="isunique"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute"></param>
        public cTickboxAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, string defaultvalue, Guid fieldid, bool isauditidentity, bool isunique, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn, bool system_attribute = false)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, isunique, allowedit, allowdelete, displayInMobile, builtIn, system_attribute: system_attribute)
        {
            sDefaultValue = defaultvalue;

            sFieldIDPrefix = "cmb";
        }

        #region properties
        /// <summary>
        /// Get default setting for tickbox
        /// </summary>
        public string defaultvalue
        {
            get { return sDefaultValue; }
        }
        #endregion
    }

    /// <summary>
    /// cRelationshipTextBoxAttribute class which inherits cAttribute
    /// </summary>
    public class cRelationshipTextBoxAttribute : cAttribute
    {
        [NonSerialized()]
        private cTable tblRelatedTable;

        /// <summary>
        /// cRelationshipTextBoxAttribute constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="fieldtype"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="relatedtable"></param>
        /// <param name="fieldid"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute"></param>
        public cRelationshipTextBoxAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, cTable relatedtable, Guid fieldid, bool isauditidentity, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn, bool system_attribute = false)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, false, allowedit, allowdelete, displayInMobile, builtIn, system_attribute: system_attribute)
        {
            tblRelatedTable = relatedtable;
        }

        #region properties
        /// <summary>
        /// Gets the related table definition
        /// </summary>
        public cTable relatedtable
        {
            get { return tblRelatedTable; }
        }
        #endregion
    }

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
        /// cManyToOneRelationship constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="relatedtable"></param>
        /// <param name="fieldid"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="AliasTable"></param>
        /// <param name="autoCompleteDisplayFieldID">Field ID of field to display in the auto complete return list</param>
        /// <param name="autoCompleteMatchFieldIDs">Field ID collection of fields to match search text against</param>
        /// <param name="maxRows">Maximum number of rows to return in the auto complete options list</param>
        /// <param name="autocompleteDisplayFields">Field ID collection of fields todisplay in the search results in autocomplete fields</param>
        /// <param name="filters">A list of FieldFilters to be applied to the ManyToOne Relationship</param>
        /// <param name="system_attribute"></param>
        public cManyToOneRelationship(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, bool builtIn, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, cTable relatedtable, Guid fieldid, bool isauditidentity, bool allowedit, bool allowdelete, cTable AliasTable, Guid autoCompleteDisplayFieldID, List<Guid> autoCompleteMatchFieldIDs, int maxRows, List<Guid> autocompleteDisplayFields = null, SortedList<int, FieldFilter> filters = null, bool system_attribute = false, List<LookupDisplayField> triggerFields = null)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, FieldType.Relationship, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, false, allowedit, allowdelete, displayInMobile: false, builtIn: builtIn, system_attribute: system_attribute)
        {
            tblRelatedTable = relatedtable;
            tblAliasTable = AliasTable;
            gDisplayFieldID = autoCompleteDisplayFieldID;
            lstMatchFieldIDs = autoCompleteMatchFieldIDs;
            this.lstAutocompleteDisplayFieldIDs = autocompleteDisplayFields;
            nMaxRows = maxRows;
            lstFilters = filters ?? new SortedList<int, FieldFilter>();
            lstTriggerFields = triggerFields ?? new List<LookupDisplayField>();
        }

        #region properties
        /// <summary>
        /// Gets the related table definition
        /// </summary>
        public cTable relatedtable
        {
            get { return tblRelatedTable; }
        }

        /// <summary>
        /// Gets the alias table definition
        /// </summary>
        public cTable AliasTable
        {
            get { return tblAliasTable; }
        }

        /// <summary>
        /// Gets the field ID of the field to display in the text box following auto complete selection
        /// </summary>
        public Guid AutoCompleteDisplayField
        {
            get { return gDisplayFieldID; }
        }

        /// <summary>
        /// Gets list of field IDs used for matching wildcard against during auto complete
        /// </summary>
        public List<Guid> AutoCompleteMatchFieldIDList
        {
            get { return lstMatchFieldIDs; }
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
            get { return nMaxRows; }
        }

        /// <summary>
        /// Gets the filters that are applied to the auto complete
        /// </summary>
        public SortedList<int, FieldFilter> filters
        {
            get { return lstFilters; }
        }

        /// <summary>
        /// Gets a collection of Lookup Display Fields that are populated automatically by this Many To One Relationship attribute
        /// </summary>
        public List<LookupDisplayField> TriggerLookupFields
        {
            get { return lstTriggerFields; }
        }
        #endregion
    }

    /// <summary>
    /// Trigger display field, displays other data from a ManyToOne object
    /// </summary>
    public class LookupDisplayField : cAttribute
    {
        /// <summary>
        /// The attribute id. of the Trigger (Many to One) Object.
        /// </summary>
        private int? _triggerAttributeId;

        /// <summary>
        /// The join via id. from the source data
        /// </summary>
        private int? _triggerJoinViaId;

        /// <summary>
        /// The display field id. from the source data
        /// </summary>
        private Guid? _triggerDisplayFieldId;

        /// <summary>
        /// The _trigger join via.
        /// </summary>
        private JoinVia _triggerJoinVia;

        /// <summary>
        /// Initializes a new instance of the <see cref="LookupDisplayField"/> class.
        /// </summary>
        /// <param name="attributeid">
        /// The attribute id.
        /// </param>
        /// <param name="attributename">
        /// The attribute name.
        /// </param>
        /// <param name="displayname">
        /// The display name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="tooltip">
        /// The tooltip.
        /// </param>
        /// <param name="createdon">
        /// The created on.
        /// </param>
        /// <param name="createdby">
        /// The created by.
        /// </param>
        /// <param name="modifiedon">
        /// The modified on.
        /// </param>
        /// <param name="modifiedby">
        /// The modified by.
        /// </param>
        /// <param name="fieldid">
        /// The field id.
        /// </param>
        /// <param name="triggerAttributeId">
        /// The trigger attribute id.
        /// </param>
        /// <param name="triggerJoinVia">
        /// The trigger join via object.
        /// </param>
        /// <param name="triggerDisplayFieldId">
        /// The trigger display field id.
        /// </param>
        public LookupDisplayField(int attributeid, string attributename, string displayname, string description, string tooltip, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, int? triggerAttributeId, JoinVia triggerJoinVia, Guid? triggerDisplayFieldId)
            : base(attributeid, attributename, displayname, description, tooltip, false, FieldType.LookupDisplayField, createdon, createdby, modifiedon, modifiedby, fieldid, false, false, false, false, false, displayInMobile: false, boolAttribute: false, builtIn: false, system_attribute: false)
        {
            this._triggerAttributeId = triggerAttributeId;
            this._triggerDisplayFieldId = triggerDisplayFieldId;
            if (triggerJoinVia != null)
            {
                this._triggerJoinViaId = triggerJoinVia.JoinViaID;
                this._triggerJoinVia = triggerJoinVia;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LookupDisplayField"/> class.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to use as a lookup source.
        /// </param>
        /// <param name="triggerAttributeId">
        /// attribute id.
        /// </param>
        /// <param name="triggerJoinVia">
        /// join via object.
        /// </param>
        /// <param name="triggerDisplayFieldId">
        /// display field id.
        /// </param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        public LookupDisplayField(cAttribute attribute, int? triggerAttributeId, JoinVia triggerJoinVia, Guid? triggerDisplayFieldId, bool displayInMobile, bool builtIn)
            : base(attribute.attributeid, attribute.attributename, attribute.displayname, attribute.description, attribute.tooltip, attribute.mandatory, attribute.fieldtype, attribute.createdon, attribute.createdby, attribute.modifiedon, attribute.modifiedby, attribute.fieldid, attribute.iskeyfield, attribute.isauditidentifer, attribute.isunique, attribute.AllowEdit, attribute.AllowDelete, displayInMobile, builtIn, system_attribute: attribute.IsSystemAttribute)
        {
            this._triggerAttributeId = triggerAttributeId;
            this._triggerDisplayFieldId = triggerDisplayFieldId;
            if (triggerJoinVia != null)
            {
                this._triggerJoinViaId = triggerJoinVia.JoinViaID;
                this._triggerJoinVia = triggerJoinVia;
            }
        }

        /// <summary>
        /// Gets or sets the trigger attribute id.
        /// </summary>
        public int? TriggerAttributeId
        {
            get { return this._triggerAttributeId; }
            set { this._triggerAttributeId = value; }
        }

        /// <summary>
        /// Gets or sets the trigger join via id.
        /// </summary>
        public int? TriggerJoinViaId
        {
            get { return this._triggerJoinViaId; }
            set { this._triggerJoinViaId = value; }
        }

        /// <summary>
        /// Gets or sets the trigger display field id.
        /// </summary>
        public Guid? TriggerDisplayFieldId
        {
            get { return this._triggerDisplayFieldId; }
            set { this._triggerDisplayFieldId = value; }
        }

        /// <summary>
        /// Gets or sets the trigger field join via.
        /// </summary>
        public JoinVia TriggerJoinVia
        {
            get { return this._triggerJoinVia; }
            set { this._triggerJoinVia = value; }
        }
    }

    /// <summary>
    /// cOneToManyRelationship class which inherits cAttribute
    /// </summary>
    public class cOneToManyRelationship : cAttribute
    {
        [NonSerialized()]
        private cTable tblRelatedTable;
        private int nViewID;
        private int nEntityid;
        private int nParentEntityId;

        /// <summary>
        /// cOneToManyRelationship constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="relatedtable"></param>
        /// <param name="fieldid"></param>
        /// <param name="viewid"></param>
        /// <param name="entityid"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="parent_entityid"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="system_attribute"></param>
        public cOneToManyRelationship(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, bool builtIn, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, cTable relatedtable, Guid fieldid, int viewid, int entityid, bool isauditidentity, int parent_entityid, bool allowedit, bool allowdelete, bool system_attribute = false)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, FieldType.Relationship, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, false, allowedit, allowdelete, displayInMobile: false, builtIn: builtIn, system_attribute: system_attribute)
        {
            tblRelatedTable = relatedtable;
            nViewID = viewid;
            nEntityid = entityid;
            nParentEntityId = parent_entityid;

            sCssClass = "";
        }

        #region properties
        /// <summary>
        /// Gets the table definition attribute relates to
        /// </summary>
        public cTable relatedtable
        {
            get { return tblRelatedTable; }
        }
        /// <summary>
        /// Gets the custom entity view id for relationship
        /// </summary>
        public int viewid
        {
            get { return nViewID; }
        }
        /// <summary>
        /// Gets the related entity id field
        /// </summary>
        public int entityid
        {
            get { return nEntityid; }
        }
        /// <summary>
        /// Gtes Entity ID that attribute belongs to
        /// </summary>
        public int parent_entityid
        {
            get { return nParentEntityId; }
        }
        #endregion
    }

    /// <summary>
    /// cRunWorkflowAttribute class which inherits cAttribute
    /// </summary>
    public class cRunWorkflowAttribute : cAttribute
    {
        cWorkflow clsworkflow;

        /// <summary>
        /// cRunWorkflowAttribute constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="fieldid"></param>
        /// <param name="workflow"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute"></param>
        public cRunWorkflowAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, cWorkflow workflow, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn, bool system_attribute = false)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, FieldType.RunWorkflow, createdon, createdby, modifiedon, modifiedby, fieldid, false, false, false, allowedit, allowdelete, displayInMobile, builtIn, system_attribute: system_attribute)
        {
            clsworkflow = workflow;
        }

        #region properties
        /// <summary>
        /// Gets the workflow class associated to the attribute
        /// </summary>
        public cWorkflow workflow
        {
            get { return clsworkflow; }
        }
        #endregion
    }

    /// <summary>
    /// Used for inbuilt currency lists
    /// </summary>
    public class cCurrencyListAttribute : cAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="fieldid"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute"></param>
        public cCurrencyListAttribute(int attributeid, string attributename, string displayname, string description, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, bool allowedit, bool allowdelete, bool displayInMobile, bool system_attribute = false)
            : base(attributeid, attributename, displayname, description, "", false, FieldType.CurrencyList, createdon, createdby, modifiedon, modifiedby, fieldid, false, false, false, allowedit, allowdelete, displayInMobile, false, system_attribute: system_attribute)
        {
            sFieldIDPrefix = "ddl";
        }
    }

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
            get { return dicSummaryElements; }
        }
        public Dictionary<int, cSummaryAttributeColumn> SummaryColumns
        {
            get { return dicSummaryColumns; }
        }
        public int SourceEntityID
        {
            get { return nSourceEntityId; }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="fieldid"></param>
        /// <param name="summaryElements"></param>
        /// <param name="summaryColumns"></param>
        /// <param name="source_entityid"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="system_attribute"></param>
        public cSummaryAttribute(int attributeid, string attributename, string displayname, string description, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, Dictionary<int, cSummaryAttributeElement> summaryElements, Dictionary<int, cSummaryAttributeColumn> summaryColumns, int source_entityid, bool allowedit, bool allowdelete, bool system_attribute = false)
            : base(attributeid, attributename, displayname, description, "", false, FieldType.OTMSummary, createdon, createdby, modifiedon, modifiedby, fieldid, false, false, false, allowedit, allowdelete, displayInMobile: false, builtIn: false, system_attribute: system_attribute)
        {
            dicSummaryElements = summaryElements;
            dicSummaryColumns = summaryColumns;
            nSourceEntityId = source_entityid;

            sCssClass = "";
        }
    }

    /// <summary>
    /// Used to identify the individual one to many attributes to be summarised in a cSummaryAttribute
    /// </summary>
    public class cSummaryAttributeElement
    {
        private int nSummaryAttributeId;
        private int nAttributeId;
        private int nOTMAttributeId;
        private int nOrder;

        #region properties
        public int SummaryAttributeId
        {
            get { return nSummaryAttributeId; }
        }
        public int AttributeId
        {
            get { return nAttributeId; }
        }
        public int OTM_AttributeId
        {
            get { return nOTMAttributeId; }
        }
        public int Order
        {
            get { return nOrder; }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="summaryattributeid"></param>
        /// <param name="attributeid"></param>
        /// <param name="otm_attributeid"></param>
        /// <param name="order"></param>
        public cSummaryAttributeElement(int summaryattributeid, int attributeid, int otm_attributeid, int order)
        {
            nSummaryAttributeId = summaryattributeid;
            nAttributeId = attributeid;
            nOTMAttributeId = otm_attributeid;
            nOrder = order;
        }
    }

    /// <summary>
    /// Used to identify a column to be displayed from the related entity in a cSummaryAttribute
    /// </summary>
    public class cSummaryAttributeColumn
    {
        private int nColumnId;
        private int nColumnAttributeID;
        private string sAlternateHeader;
        private int nWidth;
        private int nOrder;
        private bool bDefaultSort;
        private string sFilterValue;
        private bool bIsManyToOne;
        private Guid gDisplayFieldId;
        private JoinVia oJoinVia;

        #region properties
        public int ColumnAttributeID
        {
            get { return nColumnAttributeID; }
        }
        public int ColumnId
        {
            get { return nColumnId; }
        }
        public string AlternateHeader
        {
            get { return sAlternateHeader; }
        }
        public int Width
        {
            get { return nWidth; }
        }
        public int Order
        {
            get { return nOrder; }
        }
        public bool DefaultSort
        {
            get { return bDefaultSort; }
        }
        public string FilterValue
        {
            get { return sFilterValue; }
        }

        public bool IsMTOField
        {
            get { return bIsManyToOne; }
        }

        public Guid DisplayFieldId
        {
            get { return gDisplayFieldId; }
        }

        public JoinVia JoinViaObj
        {
            get { return oJoinVia; }
            set { oJoinVia = value; }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnid"></param>
        /// <param name="columnattributeid"></param>
        /// <param name="alternate_header"></param>
        /// <param name="width"></param>
        /// <param name="order"></param>
        /// <param name="sortColumn"></param>
        /// <param name="filter_value"></param>
        /// <param name="ismanytooneattribute"></param>
        /// <param name="displayfieldid"></param>
        /// <param name="joinviaObj"></param>
        public cSummaryAttributeColumn(int columnid, int columnattributeid, string alternate_header, int width, int order, bool sortColumn, string filter_value, bool ismanytooneattribute, Guid displayfieldid, JoinVia joinviaObj)
        {
            nColumnAttributeID = columnattributeid;
            nColumnId = columnid;
            sAlternateHeader = alternate_header;
            nWidth = width;
            nOrder = order;
            bDefaultSort = sortColumn;
            sFilterValue = filter_value;
            bIsManyToOne = ismanytooneattribute;
            gDisplayFieldId = displayfieldid;
            oJoinVia = joinviaObj;
        }
    }

    /// <summary>
    /// cCommentAttribute class which inherits cAttribute
    /// </summary>
    public class cCommentAttribute : cAttribute
    {
        private string sCommentText;

        /// <summary>
        /// cCommentAttribute constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="commentText"></param>
        /// <param name="fieldid"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="isunique"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute"></param>
        public cCommentAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, string commentText, Guid fieldid, bool isauditidentity, bool isunique, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn, bool system_attribute = false)
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, FieldType.Comment, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, isunique, allowedit, allowdelete, displayInMobile, builtIn, system_attribute: system_attribute)
        {
            sCommentText = commentText;

            nMaxLabelTextLength = 50;
            sCssClass = "onecolumnpanel customentitycomment";
        }

        #region properties

        /// <summary>
        /// The information to be shown for the comment panel text
        /// </summary>
        public string commentText
        {
            get { return sCommentText; }
        }

        #endregion
    }

    /// <summary>
    /// cAttachmentAttribute class which inherits cAttribute
    /// </summary>
    public class cAttachmentAttribute : cAttribute
    {
        private AttributeFormat afFormat;

        /// <summary>
        /// cAttachmentAttribute constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="fieldtype"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="fieldid"></param>
        /// <param name="format"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="isunique"></param>
        /// <param name="includeImageLibrary">Specifies whether a user will have access to the image library when uploading an attachment</param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="displayInMobile">Whether the attribute should be displayed on mobile.</param>
        /// <param name="system_attribute"></param>
        public cAttachmentAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, AttributeFormat format, bool isauditidentity, bool isunique, bool includeImageLibrary, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn, bool system_attribute = false)
            : base(
                attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon,
                createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, isunique, allowedit,
                allowdelete, displayInMobile, builtIn, includeImageLibrary, system_attribute)
        {
            afFormat = format;
        }
        #region properties
        /// <summary>
        /// Gets format type of text attribute
        /// </summary>
        public AttributeFormat format
        {
            get { return afFormat; }
        }

        public bool IncludeImageLibrary
        {
            get { return base.BoolAttribute; }
        }
        #endregion
    }

    /// <summary>
    /// Represents a "Contact" custom-entity attribute type
    /// </summary>
    public class cContactAttribute : cAttribute
    {
        private AttributeFormat afFormat;

        /// <summary>
        /// cContactAttribute constructor
        /// </summary>
        /// <param name="attributeid"></param>
        /// <param name="attributename"></param>
        /// <param name="displayname"></param>
        /// <param name="description"></param>
        /// <param name="tooltip"></param>
        /// <param name="mandatory"></param>
        /// <param name="fieldtype"></param>
        /// <param name="createdon"></param>
        /// <param name="createdby"></param>
        /// <param name="modifiedon"></param>
        /// <param name="modifiedby"></param>
        /// <param name="fieldid"></param>
        /// <param name="format"></param>
        /// <param name="isauditidentity"></param>
        /// <param name="isunique"></param>
        /// <param name="allowedit"></param>
        /// <param name="allowdelete"></param>
        /// <param name="displayInMobile"></param>
        public cContactAttribute(int attributeid, string attributename, string displayname, string description, string tooltip, bool mandatory, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, Guid fieldid, AttributeFormat format, bool isauditidentity, bool isunique, bool allowedit, bool allowdelete, bool displayInMobile, bool builtIn) 
            : base(attributeid, attributename, displayname, description, tooltip, mandatory, FieldType.Contact, createdon, createdby, modifiedon, modifiedby, fieldid, false, isauditidentity, isunique, allowedit, allowdelete, displayInMobile, builtIn, false, false)
        {
            this.afFormat = format;
        }

        public AttributeFormat format
        {
            get { return this.afFormat; }
        }
    }
    
    /// <summary>
    /// AttributeFormat enumeration
    /// </summary>
    public enum AttributeFormat
    {
        NotSet = 0,
        SingleLine = 1,
        MultiLine,
        DateTime,
        DateOnly,
        TimeOnly,
        FormattedText,
        SingleLineWide,
        ListStandard,
        ListWide,
        ContactEmail = 11,
        ContactPhone = 12,
        ContactSMS = 13
    }

    /// <summary>
    /// cCustomEntityForm class
    /// </summary>
    public class cCustomEntityForm
    {
        private int nEntityid;
        private int nFormid;
        private string sFormName;
        private string sDescription;
        private DateTime dtCreatedOn;
        private int nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;
        private string sSaveAndDuplicateButtonText;
        private string saveAndNewButtonText;
        private string sSaveAndStayButtonText;
        private string sSaveButtonText;
        private string sCancelButtonText;
        private bool bShowBreadcrumbs;
        private bool bShowSaveAndDuplicate;
        private bool showSaveAndNew;
        private bool bShowSaveAndStay;
        private bool bShowSave;
        private bool bShowSubMenus;
        private bool bShowCancel;
        private SortedList<int, cCustomEntityFormTab> lstTabs;
        private SortedList<int, cCustomEntityFormSection> lstSections;
        private SortedList<int, cCustomEntityFormField> lstFields;

        /// <summary>
        /// cCustomEntityForm constructor
        /// </summary>
        /// <param name="formid">
        /// Custom Entity Form ID
        /// </param>
        /// <param name="entityid">
        /// Custom Entity ID
        /// </param>
        /// <param name="formname">
        /// Custom Entity Form Name
        /// </param>
        /// <param name="description">
        /// Form Description
        /// </param>
        /// <param name="showSave">
        /// Show the Save Button on the form
        /// </param>
        /// <param name="saveText">
        /// Text to display in the save button
        /// </param>
        /// <param name="showSaveAndDuplicate">
        /// Show the Save and New button on the form
        /// </param>
        /// <param name="saveAndDuplicateText">
        /// Text to display in the save and new button
        /// </param>
        /// <param name="showSaveAndStay">
        /// Show the Save and Stay button on the form
        /// </param>
        /// <param name="saveAndStayText">
        /// Text to display in the save and stay button
        /// </param>
        /// <param name="showCancel">
        /// Show the Cancel button on the form
        /// </param>
        /// <param name="cancelText">
        /// Text to display in the cancel button
        /// </param>
        /// <param name="showSubMenus">
        /// Show Sub-menu panel (Page Options) panel on the screen
        /// </param>
        /// <param name="showBreadcrumbs">
        /// Show the breadcrumbs on the form page
        /// </param>
        /// <param name="createdon">
        /// Date the custom entity form record was created
        /// </param>
        /// <param name="createdby">
        /// Employee ID who created the custom entity form record
        /// </param>
        /// <param name="modifiedon">
        /// Date the custom entity form was last modified
        /// </param>
        /// <param name="modifiedby">
        /// Employee ID who last modified the custom entity form
        /// </param>
        /// <param name="tabs">
        /// Collection of tabs that make up the form
        /// </param>
        /// <param name="sections">
        /// Collection of sections used on the form
        /// </param>
        /// <param name="fields">
        /// Fields to be rendered on the custom entity form
        /// </param>
        /// <param name="saveAndNewText">
        /// The save And New button Text.
        /// </param>
        /// <param name="showSaveAndNew">
        /// The show Save And New button.
        /// </param>
        /// <param name="checkDefaultValues">
        /// Go through each field within the form and check for default values
        /// </param>
        /// <param name="hideTorch"></param>
        /// <param name="hideAttachments"></param>
        /// <param name="hideAudiences"></param>
        /// <param name="builtIn"></param>
        /// <param name="systemCustomEntityFormId">A unique system identifier for this form</param>
        public cCustomEntityForm(int formid, int entityid, string formname, string description, bool showSave, string saveText, bool showSaveAndDuplicate, string saveAndDuplicateText, bool showSaveAndStay, string saveAndStayText, bool showCancel, string cancelText, bool showSubMenus, bool showBreadcrumbs, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, SortedList<int, cCustomEntityFormTab> tabs, SortedList<int, cCustomEntityFormSection> sections, SortedList<int, cCustomEntityFormField> fields, string saveAndNewText, bool showSaveAndNew, bool checkDefaultValues = false, bool hideTorch = false, bool hideAttachments = false, bool hideAudiences = false, bool builtIn = false, Guid? systemCustomEntityFormId = null)
        {
            nEntityid = entityid;
            nFormid = formid;
            sFormName = formname;
            sDescription = description;
            bShowBreadcrumbs = showBreadcrumbs;
            bShowCancel = showCancel;
            sCancelButtonText = cancelText;
            bShowSave = showSave;
            sSaveButtonText = saveText;
            this.bShowSaveAndDuplicate = showSaveAndDuplicate;
            this.sSaveAndDuplicateButtonText = saveAndDuplicateText;
            this.showSaveAndNew = showSaveAndNew;
            this.saveAndNewButtonText = saveAndNewText;
            bShowSaveAndStay = showSaveAndStay;
            sSaveAndStayButtonText = saveAndStayText;
            bShowSubMenus = showSubMenus;
            dtCreatedOn = createdon;
            nCreatedBy = createdby;
            dtModifiedOn = modifiedon;
            nModifiedBy = modifiedby;
            lstTabs = tabs;
            lstSections = sections;
            lstFields = fields;
            this.CheckDefaultValues = checkDefaultValues;
            this.HideTorch = hideTorch;
            this.HideAttachments = hideAttachments;
            this.HideAudiences = hideAudiences;
            this.BuiltIn = builtIn;
            this.SystemCustomEntityFormId = systemCustomEntityFormId;
        }

        /// <summary>
        /// getTabsForForm: Retrieve tabs associated with the current form
        /// </summary>
        /// <returns>Sorted list collection of tab definitions</returns>
        public SortedList<byte, cCustomEntityFormTab> getTabsForForm()
        {
            return sortTabs();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionheader"></param>
        /// <returns></returns>
        public cCustomEntityFormTab getTabBySection(string sectionheader)
        {
            return (cCustomEntityFormTab)(from x in sections.Values
                                          where x.headercaption == sectionheader
                                          select x.tab).FirstOrDefault();

            //foreach (cCustomEntityFormSection section in sections.Values)
            //{
            //    if (section.headercaption == sectionheader)
            //    {
            //        return section.tab;
            //    }
            //}
            //return null;
        }

        public cCustomEntityFormTab getTabByName(string name)
        {
            return (cCustomEntityFormTab)(from x in lstTabs.Values
                                          where x.headercaption == name
                                          select x).FirstOrDefault();

            //foreach (cCustomEntityFormTab tab in lstTabs.Values)
            //{
            //    if (tab.headercaption == name)
            //    {
            //        return tab;
            //    }
            //}
            //return null;
        }
        private SortedList<byte, cCustomEntityFormTab> sortTabs()
        {
            SortedList<byte, cCustomEntityFormTab> tabs = new SortedList<byte, cCustomEntityFormTab>((from x in lstTabs.Values
                                                                                                      select x).ToDictionary(a => a.order));
            //foreach (cCustomEntityFormTab tab in lstTabs.Values)
            //{
            //    tabs.Add(tab.order, tab);
            //}
            return tabs;
        }
        #region properties
        public int entityid
        {
            get { return nEntityid; }
        }
        public int formid
        {
            get { return nFormid; }
        }
        public string formname
        {
            get { return sFormName; }
        }
        public string description
        {
            get { return sDescription; }
        }
        public bool ShowSaveAndStayButton
        {
            get { return bShowSaveAndStay; }
        }
        public string SaveAndStayButtonText
        {
            get { return sSaveAndStayButtonText; }
        }
        public DateTime createdon
        {
            get { return dtCreatedOn; }
        }
        public int createdby
        {
            get { return nCreatedBy; }
        }
        public DateTime? modifiedon
        {
            get { return dtModifiedOn; }
        }
        public int? modifiedby
        {
            get { return nModifiedBy; }
        }
        public SortedList<int, cCustomEntityFormField> fields
        {
            get { return lstFields; }
        }
        public SortedList<int, cCustomEntityFormTab> tabs
        {
            get { return lstTabs; }
        }
        public SortedList<int, cCustomEntityFormSection> sections
        {
            get { return lstSections; }
        }

        /// <summary>
        /// Gets if the save button should be shown
        /// </summary>
        public bool ShowSaveButton
        {
            get { return bShowSave; }
        }

        /// <summary>
        /// Gets if the cancel button should be shown
        /// </summary>
        public bool ShowCancelButton
        {
            get { return bShowCancel; }
        }

        /// <summary>
        /// Gets what text will be displayed for this button action
        /// </summary>
        public string SaveButtonText
        {
            get { return sSaveButtonText; }
        }

        /// <summary>
        /// Gets what text will be displayed for this button action
        /// </summary>
        public string SaveAndDuplicateButtonText
        {
            get { return this.sSaveAndDuplicateButtonText; }
        }

        /// <summary>
        /// Gets what text will be displayed for this button action
        /// </summary>
        public string SaveAndNewButtonText
        {
            get
            {
                return this.saveAndNewButtonText;
            }
        }

        /// <summary>
        /// Gets if the Save and New button should be shown
        /// </summary>
        public bool ShowSaveAndNew
        {
            get
            {
                return this.showSaveAndNew;
            }
        }

        /// <summary>
        /// Gets what text will be displayed for this button action
        /// </summary>
        public string CancelButtonText
        {
            get { return sCancelButtonText; }
        }

        /// <summary>
        /// Determines if the page options menu is shown via the master page.
        /// </summary>
        public bool ShowSubMenus
        {
            get { return bShowSubMenus; }
            set { bShowSubMenus = value; }
        }

        /// <summary>
        /// Gets and sets Determines if the Save And Duplicate button will be displayed.
        /// </summary>
        public bool ShowSaveAndDuplicate
        {
            get { return this.bShowSaveAndDuplicate; }
            set { this.bShowSaveAndDuplicate = value; }
        }


        /// <summary>
        /// Determines if breadcrumbs are shown or not.
        /// </summary>
        public bool ShowBreadCrumbs
        {
            get { return bShowBreadcrumbs; }
            set { bShowBreadcrumbs = value; }
        }

        /// <summary>
        /// Gets a value indicating whether to check for default values on form fields.
        /// </summary>
        public bool CheckDefaultValues { get; private set; }

        /// <summary>
        /// Optionally hide the torch tab when the form is displayed.
        /// </summary>
        public bool HideTorch { get; set; }

        /// <summary>
        /// Optionally hide the attachments tab when the form is displayed.
        /// </summary>
        public bool HideAttachments { get; set; }

        /// <summary>
        /// Optionally hide the audience tab when the form is displayed.
        /// </summary>
        public bool HideAudiences { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this GreenLight Form is built-in A.K.A "System".
        /// Built in GreenLights (and their built-in sub components) can be copied between customer databases, although the functionality to do so is not provided within this product.
        /// Only "adminonly" employees can set this value, once set it cannot be un-set.
        /// </summary>
        public bool BuiltIn { get; set; }

        /// <summary>
        /// Gets or sets a unique system identifier for this form, for System GreenLights
        /// </summary>
        public Guid? SystemCustomEntityFormId { get; set; }

        #endregion
    }


    public class cCustomEntityFormTab
    {
        private int nTabid;
        private int nFormid;
        private string sHeaderCaption;
        private byte nOrder;
        private List<cCustomEntityFormSection> lstSections = new List<cCustomEntityFormSection>();

        public cCustomEntityFormTab(int tabid, int formid, string headercaption, byte order)
        {
            nTabid = tabid;
            nFormid = formid;
            sHeaderCaption = headercaption;
            nOrder = order;
        }

        public SortedList<byte, cCustomEntityFormSection> getSectionsForTab()
        {
            return sortSections();
        }

        private SortedList<byte, cCustomEntityFormSection> sortSections()
        {
            SortedList<byte, cCustomEntityFormSection> sections = new SortedList<byte, cCustomEntityFormSection>((from x in lstSections
                                                                                                                  select x).ToDictionary(a => a.order));
            //foreach (cCustomEntityFormSection section in lstSections)
            //{
            //    sections.Add(section.order, section);
            //}
            return sections;
        }
        #region properties
        public int tabid
        {
            get { return nTabid; }
            set { nTabid = value; }
        }
        public int formid
        {
            get { return nFormid; }
        }
        public string headercaption
        {
            get { return sHeaderCaption; }
        }
        public byte order
        {
            get { return nOrder; }
        }
        public List<cCustomEntityFormSection> sections
        {
            get { return lstSections; }
        }
        #endregion
    }

    public class cCustomEntityFormSection
    {
        private int nSectionid;
        private int nFormid;
        private string sHeaderCaption;
        private byte nOrder;
        [NonSerialized()]
        private cCustomEntityFormTab clsTab;
        private List<cCustomEntityFormField> lstFields = new List<cCustomEntityFormField>();

        public cCustomEntityFormSection(int sectionid, int formid, string headercaption, byte order, cCustomEntityFormTab tab)
        {
            nSectionid = sectionid;
            nFormid = formid;
            sHeaderCaption = headercaption;
            nOrder = order;
            clsTab = tab;
        }

        public List<cCustomEntityFormField> getFieldsForSection()
        {
            return lstFields;
        }
        #region properties
        public int sectionid
        {
            get { return nSectionid; }
            set { nSectionid = value; }
        }
        public int formid
        {
            get { return nFormid; }
        }
        public string headercaption
        {
            get { return sHeaderCaption; }
        }
        public byte order
        {
            get { return nOrder; }
        }

        public cCustomEntityFormTab tab
        {
            get { return clsTab; }
        }
        public List<cCustomEntityFormField> fields
        {
            get { return lstFields; }
        }
        #endregion
    }

    /// <summary>
    /// Class for custom entity field properties at form level
    /// </summary>
    public class cCustomEntityFormField
    {
        private int nFormid;
        private cAttribute clsAttribute;
        private bool bReadOnly;
        private cCustomEntityFormSection clsSection;
        private byte nColumn;
        private byte nRow;
        private string sLabelText;
        private byte nColumnSpan;

        /// <summary>
        /// Mandatory value of an atribute at form level
        /// </summary>
        private bool? fieldMandatoryCheck;

        /// <summary>
        /// Initializes a new instance of the <see cref="cCustomEntityFormField"/> class. 
        /// Initialisation of form fields.
        /// </summary>
        /// <param name="formid">
        /// current form id.
        /// </param>
        /// <param name="attribute">
        /// current attribute.
        /// </param>
        /// <param name="breadonly">
        /// breadcrumb of the form.
        /// </param>
        /// <param name="section">
        /// sections in a form.
        /// </param>
        /// <param name="column">
        /// columns in a form.
        /// </param>
        /// <param name="row">
        /// rows in a form.
        /// </param>
        /// <param name="labeltext">
        /// Label of an attribute.
        /// </param>
        /// <param name="isMandatory">
        /// Mandatory check of an attribute at form level.
        /// </param>
        /// <param name="defaultValue">
        /// The default Value text.
        /// </param>
        /// <param name="columnSpan">
        /// columnSpan.
        /// </param>
        public cCustomEntityFormField(int formid, cAttribute attribute, bool breadonly, cCustomEntityFormSection section, byte column, byte row, string labeltext = "", bool? isMandatory = null, string defaultValue = null, byte columnSpan = 1)
        {
            nFormid = formid;
            clsAttribute = attribute;
            bReadOnly = breadonly;
            clsSection = section;
            nColumn = column;
            nRow = row;
            sLabelText = labeltext;
            nColumnSpan = columnSpan;
            this.DefaultValue = defaultValue;
            this.fieldMandatoryCheck = isMandatory;
        }

        #region properties
        public int formid
        {
            get { return nFormid; }
        }
        public cAttribute attribute
        {
            get { return clsAttribute; }
        }
        public bool isReadOnly
        {
            get { return bReadOnly; }
        }
        public cCustomEntityFormSection section
        {
            get { return clsSection; }
        }
        public byte column
        {
            get { return nColumn; }
        }
        public byte row
        {
            get { return nRow; }
        }
        public string labelText
        {
            get { return sLabelText; }
        }

        /// <summary>
        /// Gets attributes mandatory status at form level
        /// </summary>
        public bool? IsMandatory
        {
            get { return this.fieldMandatoryCheck; }
        }
        public byte columnSpan
        {
            get
            {
                byte columnSpan = 1;

                if (this.attribute.GetType() == typeof(cCommentAttribute) || this.attribute.GetType() == typeof(cSummaryAttribute) || this.attribute.GetType() == typeof(cOneToManyRelationship))
                {
                    columnSpan = 2;
                }

                if (this.attribute.GetType() == typeof(cTextAttribute))
                {
                    switch (((cTextAttribute)this.attribute).format)
                    {
                        case AttributeFormat.FormattedText:
                            columnSpan = 2;
                            break;
                        case AttributeFormat.MultiLine:
                            columnSpan = 2;
                            break;
                        case AttributeFormat.SingleLineWide:
                            columnSpan = 2;
                            break;
                        default:
                            break;
                    }
                }

                if (this.attribute.GetType() == typeof(cListAttribute))
                {
                    if (((cListAttribute)this.attribute).format == AttributeFormat.ListWide)
                    {
                        columnSpan = 2;
                    }
                }

                return columnSpan;
            }
        }

        /// <summary>
        /// Gets the default value for the form field. This needs updating so we can have different types of default value.
        /// </summary>
        public string DefaultValue { get; private set; }

        #endregion
    }
    /// <summary>
    /// View field with unique guid to use when a custom join is needed and joinVia is populated
    /// </summary>
    public class cCustomEntityViewField
    {
        /// <summary>
        /// The field to display in the view
        /// </summary>
        public cField Field { get; set; }
        /// <summary>
        /// A list of join table/field guids that the field was picked via
        /// </summary>
        public JoinVia JoinVia { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field">The field to display on the view</param>
        /// <param name="joinVia">Populated if the field is from a joined table (from MtO relationship(s))</param>
        public cCustomEntityViewField(cField field, JoinVia joinVia = null)
        {
            Field = field;
            JoinVia = joinVia;
        }
    }

    ///// <summary>
    ///// A filter to be applied to a view
    ///// </summary>    
    //public class FieldFilter
    //{
    //    private readonly cField _field;
    //    private readonly ConditionType _conditionType;
    //    private readonly string _valueOne;
    //    private readonly string _valueTwo;
    //    private readonly byte _order;
    //    private readonly JoinVia _joinVia;

    //    /// <summary>
    //    /// Default constructor
    //    /// </summary>
    //    /// <param name="field"></param>
    //    /// <param name="conditiontype"></param>
    //    /// <param name="valueOne"></param>
    //    /// <param name="valueTwo"></param>
    //    /// <param name="order"></param>
    //    /// <param name="joinVia"></param>
    //    public FieldFilter(cField field, ConditionType conditiontype, string valueOne, string valueTwo, byte order, JoinVia joinVia)
    //    {
    //        _field = field;
    //        _conditionType = conditiontype;
    //        _valueOne = valueOne;
    //        _valueTwo = valueTwo;
    //        _order = order;
    //        _joinVia = joinVia;
    //    }

    //    #region properties
    //    /// <summary>
    //    /// The field to filter using
    //    /// </summary>
    //    public cField Field
    //    {
    //        get { return _field; }
    //    }
    //    /// <summary>
    //    /// The expression operator to use
    //    /// </summary>
    //    public ConditionType Conditiontype
    //    {
    //        get { return _conditionType; }
    //    }
    //    /// <summary>
    //    /// The first criterion, required
    //    /// </summary>
    //    public string ValueOne
    //    {
    //        get { return _valueOne; }
    //    }
    //    /// <summary>
    //    /// The second criterion, only required with some operators - eg. between
    //    /// </summary>
    //    public string ValueTwo
    //    {
    //        get { return _valueTwo; }
    //    }
    //    /// <summary>
    //    /// The order in which the filters should be applied
    //    /// </summary>
    //    public byte Order
    //    {
    //        get { return _order; }
    //    }
    //    /// <summary>
    //    /// A join path to use with the field
    //    /// </summary>
    //    public JoinVia JoinVia
    //    {
    //        get { return _joinVia; }
    //    }
    //    #endregion
    //}

    /// <summary>
    /// Sort columns for greenlight view
    /// </summary>
    public class GreenLightSortColumn
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="fieldID"></param>
        /// <param name="sortDirection"></param>
        /// <param name="joinVia"></param>
        public GreenLightSortColumn(Guid fieldID, SortDirection sortDirection, JoinVia joinVia)
        {
            FieldID = fieldID;
            SortDirection = sortDirection;
            JoinVia = joinVia;
        }

        #region properties
        /// <summary>
        /// The fieldid of the column to sort
        /// </summary>
        public Guid FieldID { get; set; }

        /// <summary>
        /// The direction of the sort whether it be ascending or descending
        /// </summary>
        public SortDirection SortDirection { get; set; }

        /// <summary>
        /// The JoinViaID for the column if one is present, otherwise 0
        /// </summary>
        public JoinVia JoinVia { get; set; }

        #endregion
    }

    /// <summary>
    /// Access role details for a user about a specific view
    /// </summary>
    /// 
    [Serializable()]
    public class cCustomEntityViewAccess
    {
        private int nCustomEntityID;
        private int nCustomEntityViewID;
        private bool bCanView;
        private bool bCanAdd;
        private bool bCanEdit;
        private bool bCanDelete;

        /// <summary>
        /// Constructor for storing a role specific custom entity view access details
        /// </summary>
        /// <param name="customEntityID"></param>
        /// <param name="customEntityViewID"></param>
        /// <param name="canView">If this user can view records belonging to this element</param>
        /// <param name="canAdd">If this user can add records belonging to this element</param>
        /// <param name="canEdit">If this user can edit records belonging to this element</param>
        /// <param name="canDelete">If this user can delete records belonging to this element</param>
        public cCustomEntityViewAccess(int customEntityID, int customEntityViewID, bool canView, bool canAdd, bool canEdit, bool canDelete)
        {
            nCustomEntityID = customEntityID;
            nCustomEntityViewID = customEntityViewID;
            bCanView = canView;
            bCanAdd = canAdd;
            bCanEdit = canEdit;
            bCanDelete = canDelete;
        }

        /// <summary>
        /// Returns the related custom entity id
        /// </summary>
        public int CustomEntityID { get { return nCustomEntityID; } }

        /// <summary>
        /// Returns the custom entity view id
        /// </summary>
        public int CustomEntityViewID { get { return nCustomEntityViewID; } }

        /// <summary>
        /// Boolean stating if a user can view or not, if CanEdit, CanAdd or CanDelete are true CanView will return true
        /// </summary>
        public bool CanView
        {
            get
            {
                if (bCanAdd == true || bCanEdit == true || bCanDelete == true)
                {
                    return true;
                }
                else
                {
                    return bCanView;
                }
            }

            set { bCanView = value; }
        }

        /// <summary>
        /// Boolean stating if a user can add or not
        /// </summary>
        public bool CanAdd
        {
            get { return bCanAdd; }
            set { bCanAdd = value; }
        }

        /// <summary>
        /// Boolean stating if a user can edit or not
        /// </summary>
        public bool CanEdit
        {
            get { return bCanEdit; }
            set { bCanEdit = value; }
        }

        /// <summary>
        /// Boolean stating if a user can delete or not
        /// </summary>
        public bool CanDelete
        {
            get { return bCanDelete; }
            set { bCanDelete = value; }
        }

    }

    /// <summary>
    /// Access role details for a user about a specific form
    /// </summary>
    /// 
    [Serializable()]
    public class cCustomEntityFormAccess
    {
        private int nCustomEntityID;
        private int nCustomEntityFormID;
        private bool bCanView;
        private bool bCanAdd;
        private bool bCanEdit;
        private bool bCanDelete;

        /// <summary>
        /// Constructor for storing a role specific custom entity form access details
        /// </summary>
        /// <param name="customEntityFormID"></param>
        /// <param name="customEntityID"></param>
        /// <param name="canView">If this user can view records belonging to this element</param>
        /// <param name="canAdd">If this user can add records belonging to this element</param>
        /// <param name="canEdit">If this user can edit records belonging to this element</param>
        /// <param name="canDelete">If this user can delete records belonging to this element</param>
        public cCustomEntityFormAccess(int customEntityID, int customEntityFormID, bool canView, bool canAdd, bool canEdit, bool canDelete)
        {
            nCustomEntityID = customEntityID;
            nCustomEntityFormID = customEntityFormID;
            bCanView = canView;
            bCanAdd = canAdd;
            bCanEdit = canEdit;
            bCanDelete = canDelete;
        }

        /// <summary>
        /// Returns the related custom entity id
        /// </summary>
        public int CustomEntityID { get { return nCustomEntityID; } }

        /// <summary>
        /// Returns the custom entity view id
        /// </summary>
        public int CustomEntityFormID { get { return nCustomEntityFormID; } }

        /// <summary>
        /// Boolean stating if a user can view or not, if CanEdit, CanAdd or CanDelete are true CanView will return true
        /// </summary>
        public bool CanView
        {
            get
            {
                if (bCanAdd == true || bCanEdit == true || bCanDelete == true)
                {
                    return true;
                }
                else
                {
                    return bCanView;
                }
            }

            set { bCanView = value; }
        }

        /// <summary>
        /// Boolean stating if a user can add or not
        /// </summary>
        public bool CanAdd
        {
            get { return bCanAdd; }
            set { bCanAdd = value; }
        }

        /// <summary>
        /// Boolean stating if a user can edit or not
        /// </summary>
        public bool CanEdit
        {
            get { return bCanEdit; }
            set { bCanEdit = value; }
        }

        /// <summary>
        /// Boolean stating if a user can delete or not
        /// </summary>
        public bool CanDelete
        {
            get { return bCanDelete; }
            set { bCanDelete = value; }
        }

    }

    [Serializable()]
    public class cCustomEntityAccess
    {
        private int nCustomEntityID;
        private bool bCanView;
        private bool bCanAdd;
        private bool bCanEdit;
        private bool bCanDelete;
        private SortedList<int, cCustomEntityViewAccess> lstViewAccess;
        private SortedList<int, cCustomEntityFormAccess> lstFormAccess;

        /// <summary>
        /// Constructor for storing a role specific custom entity access details
        /// </summary>
        /// <param name="customEntityID">The custom entity id</param>
        /// <param name="canView">If this user can view records belonging to this element</param>
        /// <param name="canAdd">If this user can add records belonging to this element</param>
        /// <param name="canEdit">If this user can edit records belonging to this element</param>
        /// <param name="canDelete">If this user can delete records belonging to this element</param>
        /// <param name="formAccess"></param>
        /// <param name="viewAccess"></param>
        public cCustomEntityAccess(int customEntityID, bool canView, bool canAdd, bool canEdit, bool canDelete, SortedList<int, cCustomEntityViewAccess> viewAccess, SortedList<int, cCustomEntityFormAccess> formAccess)
        {
            nCustomEntityID = customEntityID;
            bCanView = canView;
            bCanAdd = canAdd;
            bCanEdit = canEdit;
            bCanDelete = canDelete;
            lstViewAccess = viewAccess;
            lstFormAccess = formAccess;
        }

        /// <summary>
        /// Returns the custom entity id
        /// </summary>
        public int CustomEntityID { get { return nCustomEntityID; } }

        /// <summary>
        /// Boolean stating if a user can view or not, if CanEdit, CanAdd or CanDelete are true CanView will return true
        /// </summary>
        public bool CanView
        {
            get
            {
                if (bCanAdd == true || bCanEdit == true || bCanDelete == true)
                {
                    return true;
                }
                else
                {
                    return bCanView;
                }
            }

            set { bCanView = value; }
        }

        /// <summary>
        /// Boolean stating if a user can add or not
        /// </summary>
        public bool CanAdd
        {
            get { return bCanAdd; }
            set { bCanAdd = value; }
        }

        /// <summary>
        /// Boolean stating if a user can edit or not
        /// </summary>
        public bool CanEdit
        {
            get { return bCanEdit; }
            set { bCanEdit = value; }
        }

        /// <summary>
        /// Boolean stating if a user can delete or not
        /// </summary>
        public bool CanDelete
        {
            get { return bCanDelete; }
            set { bCanDelete = value; }
        }

        /// <summary>
        /// List of view access details
        /// </summary>
        public SortedList<int, cCustomEntityViewAccess> ViewAccess
        {
            get { return lstViewAccess; }
            set { lstViewAccess = value; }
        }

        /// <summary>
        /// List of form access details
        /// </summary>
        public SortedList<int, cCustomEntityFormAccess> FormAccess
        {
            get { return lstFormAccess; }
            set { lstFormAccess = value; }
        }
    }

    /// <summary>
    /// Stores the field details for the custom entity forms
    /// </summary>
    public struct sCEFieldDetails
    {
        public int AttributeID;
        public string ControlName;
        public string DisplayName;
        public string Description;
        public string Tooltip;
        public string LabelText;
        public bool Mandatory;
        public FieldType FieldType;
        public bool ReadOnly;
        public byte Row;
        public byte Column;
        public AttributeFormat Format;
        public string CommentText;
        public int RelationshipType;
        public int ColumnSpan;
        public string FieldValue;
        public string SortDisplayName;
        public string DefaultValue;
        public int MaxLength;
        public bool? MandatoryCheckOverride;
    }


    /// <summary>
    /// Store the custom entity form tab and its associated sections
    /// </summary>
    public struct sCEFormTab
    {
        public string TabName;
        public string TabControlName;
        public List<sCEFormSection> Sections;
        public byte Order;
    }


    /// <summary>
    /// Store the custom entity form section and its associated fields
    /// </summary>
    public struct sCEFormSection
    {
        public string SectionName;
        public string SectionControlName;
        public List<sCEFieldDetails> Fields;
        public byte Order;
    }

    /// <summary>
    /// Stores all the form details and controls
    /// </summary>
    public struct sForm
    {
        public string formName;
        public string description;
        public bool showSave;
        public string saveButtonText;
        public bool showSaveAndDuplicate;
        public string saveAndDuplicateButtonText;
        public bool showSaveAndNew;
        public string saveAndNewButtonText;
        public bool showSaveAndStay;
        public string saveAndStayButtonText;
        public bool showCancel;
        public string cancelButtonText;
        public bool showSubMenus;
        public bool showBreadcrumbs;
        public bool hideTorch;
        public bool hideAttachments;
        public bool hideAudiences;
        public bool builtIn;
        public List<sCEFormTab> tabs;
    }

    /// <summary>
    /// View Menu Icon results struct
    /// </summary>
    [Serializable]
    public struct ViewMenuIconResults
    {
        /// <summary>
        /// Actual menu icons
        /// </summary>
        public List<ViewMenuIcon> MenuIcons;

        /// <summary>
        /// The result number to start from
        /// </summary>
        public int ResultStartNumber;

        /// <summary>
        /// The result number we ended on
        /// </summary>
        public int ResultEndNumber;

        /// <summary>
        /// Indicates if there are further results to be returned
        /// </summary>
        public bool FurtherResults;
    }

    /// <summary>
    /// View Menu Icon struct
    /// </summary>
    [Serializable]
    public struct ViewMenuIcon
    {
        /// <summary>
        /// URL for the icon
        /// </summary>
        public string IconUrl;

        /// <summary>
        /// Name of the icon
        /// </summary>
        public string IconName;
    }

    #region View Sort Column Info
    /// <summary>
    /// The information required for a JS view sort column
    /// </summary>
    public class jsGreenLightViewSortColumn
    {
        public string SortID = string.Empty;
        public Guid FieldID = Guid.Empty;
        public int JoinViaID = 0;
        public string JoinViaPath = string.Empty;
        public string JoinViaCrumbs = string.Empty;
    }
    #endregion View Sort Column Info

    [Serializable()]
    public struct sSummary
    {
        public int summary_attributeid;
        public int attributeid;
        public int otm_attributeid;
        public int order;
    }

    [Serializable()]
    public struct sSummaryColumn
    {
        public int columnid;
        public int attributeid;
        public int columnAttributeID;
        public string alt_header;
        public int width;
        public int order;
        public bool default_sort;
        public string filterVal;
        public bool ismtoattribute;
        public string displayFieldId;
        public string displayFieldName;
        public int JoinViaID;
    }


    public struct jsSummaryColumnData
    {
        public int columnid;
        public string JoinViaPath;
        public string JoinViaCrumbs;
    }

    /// <summary>
    /// Structure that contains information relevant to custom fields
    /// </summary>
    [Serializable()]
    public struct sCustomField
    {
        public string FieldID;
        public string FieldName;
        public string Description;
        public string DataType;
        public string TableID;
        public FieldCategory FieldCat;
        public string RelatedFieldID;
    }

    /// <summary>
    /// The category of the field
    /// </summary>
    public enum FieldCategory
    {
        ViewField = 0,
        AliasField,
        FunctionField,
        AliasTableField
    }

    /// <summary>
    /// The relationship type of the attribute
    /// </summary>
    public enum RelationshipType
    {
        None = 0,
        ManyToOne = 1,
        OneToMany = 2
    }
}
