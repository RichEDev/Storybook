namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

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
}
