
using System.ComponentModel;
using System.Web.Services.Protocols;
using Newtonsoft.Json.Linq;
using SpendManagementLibrary.GreenLight;
using Spend_Management.shared.code.Helpers;


namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Script.Serialization;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using AjaxControlToolkit;
    using Microsoft.SqlServer.Server;
    using shared.usercontrols;
    using SpendManagementHelpers.TreeControl;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Interfaces;
    using SortDirection = SpendManagementLibrary.SortDirection;
    using SpendManagementLibrary.Helpers;
    using System.Net;
    using Spend_Management.shared.code;
    using shared.code;
    using shared.code.GreenLight;
    using shared.code.EasyTree;
    using System.Text;
    /// <summary>
    /// Primary custom entities collection class
    /// </summary>
    public class cCustomEntities : ICustomEntities
    {
        private Cache Cache = HttpRuntime.Cache;
        private int nAccountid;
        private int nActiveDisplayTabId;
        private int nRecordid = 0;
        private SortedList<int, cCustomEntity> lst;
        private static ConcurrentDictionary<int, SortedList<int, cCustomEntity>> allLists = new ConcurrentDictionary<int, SortedList<int, cCustomEntity>>();
        private static ConcurrentDictionary<int, long> lastReadFromDatabaseTicks = new ConcurrentDictionary<int, long>();
        private ICurrentUser oCurrentUser;
        private List<int> lstViewId;
        private List<sEntityBreadCrumb> lstCrumbs;
        private Dictionary<int, CustomMenuStructureItem> customMenuList;
        CustomMenuStructure customMenu;
        //private IDBConnection expdata;

        private cFields fields;

        private cTables tables;

        private bool bclearCache;

        /// <summary>
        /// Initialises a new instance of the <see cref="cCustomEntities"/> class. 
        /// cCustomEntities, constructor initialises the caching of all of the custom entities in the database.
        /// </summary>
        /// <param name="currentUser">
        /// The current user object
        /// </param>
        /// <param name="connection">
        /// The database connection.
        /// </param>
        public cCustomEntities(ICurrentUser currentUser, IDBConnection connection = null)
        {
            oCurrentUser = currentUser;
            nAccountid = currentUser.AccountID;
            //this.expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(nAccountid));
            this.fields = new cFields(nAccountid);
            this.tables = new cTables(nAccountid);
            InitialiseData();
        }


        public cCustomEntities(ICurrentUser currentUser,bool clearCache, IDBConnection connection = null)
        {
            oCurrentUser = currentUser;
            nAccountid = currentUser.AccountID;
            //this.expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(nAccountid));
            this.fields = new cFields(nAccountid);
            this.tables = new cTables(nAccountid);
            ClearCache = clearCache;
            InitialiseData();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="cCustomEntities"/> class. 
        /// Parameter-less constructor for the class
        /// </summary>
        public cCustomEntities()
        {
        }

        #region properties

        /// <summary>
        /// Gets Customer Account ID
        /// </summary>
        public int _accountid
        {
            get { return nAccountid; }
        }

        /// <summary>
        /// Gets or sets active display tab
        /// </summary>
        public int activeDisplayTab
        {
            get { return nActiveDisplayTabId; }
            set { nActiveDisplayTabId = value; }
        }

        /// <summary>
        /// Gets the list of custom entities
        /// </summary>
        public SortedList<int, cCustomEntity> CustomEntities
        {
            get { return lst; }
        }

        public string ImageLibraryModalPopupExtenderId { get; set; }

        public bool ClearCache { get; set; }

        #endregion

        /// <summary>
        /// Initialises data collection into memory
        /// </summary>
        private void InitialiseData()
        {
            if (nAccountid > 0)
            {
                long lastUpdatedAllServers = cUserDefinedFieldsBase.GetLastUpdatedFromCache(nAccountid);
                long lastReadFromDatabaseThisServer = lastReadFromDatabaseTicks.GetOrAdd(nAccountid, 0);
                var forceUpdateFromDatabase = lastUpdatedAllServers > lastReadFromDatabaseThisServer;
                if (forceUpdateFromDatabase || ClearCache)
                {
                    SortedList<int, cCustomEntity> oldValue;
                    allLists.TryRemove(nAccountid, out oldValue);
                    ClearCache = false;
                }
            }

            lst = allLists.GetOrAdd(nAccountid, CacheList);
        }

        private SortedList<int, cCustomEntity> CacheList(int accountId)
        {
            return CacheList();
        }

        /// <summary>
        /// Cache entities from database into collection
        /// </summary>
        /// <returns>SortedList of entities with entity id as key</returns>
        private SortedList<int, cCustomEntity> CacheList(IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            var list = new SortedList<int, cCustomEntity>();
            SortedList<int, SortedList<int, cAttribute>> lstattributes = this.getAttributes();

            SortedList<int, SortedList<int, cCustomEntityForm>> lstforms = this.getForms(lstattributes);

            SortedList<int, SortedList<int, cCustomEntityView>> lstviews = this.getViews(lstforms);

            var strsql = "select getdate() select entityid, entity_name, plural_name, description, createdon, createdby, modifiedon, modifiedby, tableid, enableAttachments, audienceViewType, allowdocmergeaccess, systemview, systemview_derivedentityid, audienceTableID, systemview_entityid, enableCurrencies, defaultCurrencyID, enablePopupWindow, defaultPopupView, formSelectionAttribute, ownerId, supportContactId, supportQuestion, enableLocking, builtIn, SystemCustomEntityID from dbo.customEntities";
            const string recordsetError = "The SQL should return the date before the user defined fields.";

            expdata.sqlexecute.CommandText = strsql;
            DateTime lastUpdated;

            using (IDataReader reader = expdata.GetReader(strsql))
            {
                if (!reader.Read()) throw new Exception(recordsetError);
                lastUpdated = reader.GetDateTime(0);
                if (!reader.NextResult()) throw new Exception(recordsetError);

                var entityidOrd = reader.GetOrdinal("entityid");
                var entityNameOrd = reader.GetOrdinal("entity_name");
                var pluralNameOrd = reader.GetOrdinal("plural_name");
                var descriptionOrd = reader.GetOrdinal("description");
                var createdOnOrd = reader.GetOrdinal("createdon");
                var createdByOrd = reader.GetOrdinal("createdby");
                var modifiedOnOrd = reader.GetOrdinal("modifiedon");
                var modifiedByOrd = reader.GetOrdinal("modifiedby");
                var tableidOrd = reader.GetOrdinal("tableid");
                var audienceTableidOrd = reader.GetOrdinal("audienceTableID");
                var enableAttachmentsOrd = reader.GetOrdinal("enableAttachments");
                var audiencesOrd = reader.GetOrdinal("audienceViewType");
                var allowDocMergeAccessOrd = reader.GetOrdinal("allowdocmergeaccess");
                var sysViewOrd = reader.GetOrdinal("systemview");
                var systemViewDerivedEntityidOrd = reader.GetOrdinal("systemview_derivedentityid");
                var systemViewEntityidOrd = reader.GetOrdinal("systemview_entityid");
                var enableCurrenciesOrd = reader.GetOrdinal("enableCurrencies");
                var defaultCurrencyidOrd = reader.GetOrdinal("defaultCurrencyID");
                var enablePopupWindowOrd = reader.GetOrdinal("enablePopupWindow");
                var defaultPopupViewOrd = reader.GetOrdinal("defaultPopupView");
                var formSelectionAttributeIdOrd = reader.GetOrdinal("formSelectionAttribute");
                var ownerIdOrd = reader.GetOrdinal("ownerId");
                var supportContactIdOrd = reader.GetOrdinal("supportContactId");
                var supportQuestionOrd = reader.GetOrdinal("supportQuestion");
                var enableLockingOrd = reader.GetOrdinal("enableLocking");
                var builtInOrd = reader.GetOrdinal("builtIn");
                var systemCustomEntityIdOrd = reader.GetOrdinal("SystemCustomEntityID");

                while (reader.Read())
                {
                    int entityid = reader.GetInt32(entityidOrd);
                    string entityname = reader.GetString(entityNameOrd);
                    string pluralname = reader.GetString(pluralNameOrd);
                    string description = reader.IsDBNull(descriptionOrd) ? string.Empty : reader.GetString(descriptionOrd);
                    DateTime createdon = reader.GetDateTime(createdOnOrd);
                    int createdby = reader.GetInt32(createdByOrd);
                    DateTime? modifiedon = reader.IsDBNull(modifiedOnOrd) ? (DateTime?)null : reader.GetDateTime(modifiedOnOrd);
                    int? modifiedby = reader.IsDBNull(modifiedByOrd) ? (int?)null : reader.GetInt32(modifiedByOrd);

                    Guid tableid = reader.GetGuid(tableidOrd);
                    Guid audienceTableID = reader.GetGuid(audienceTableidOrd);
                    bool enableattachments = reader.GetBoolean(enableAttachmentsOrd);
                    AudienceViewType audienceViewType = (AudienceViewType)reader.GetByte(audiencesOrd);
                    bool allowdocmergeaccess = reader.GetBoolean(allowDocMergeAccessOrd);
                    bool sysview = reader.GetBoolean(sysViewOrd);
                    int? systemviewDerivedentityid = !reader.IsDBNull(systemViewDerivedEntityidOrd) ? (int?)reader.GetInt32(systemViewDerivedEntityidOrd) : null;

                    int? systemviewEntityid = !reader.IsDBNull(systemViewEntityidOrd) ? (int?)reader.GetInt32(systemViewEntityidOrd) : null;

                    bool enableCurrencies = reader.GetBoolean(enableCurrenciesOrd);
                    int? defaultCurrencyID = reader.IsDBNull(defaultCurrencyidOrd) == true ? (int?)null : reader.GetInt32(defaultCurrencyidOrd);

                    bool enablePopupWindow = reader.GetBoolean(enablePopupWindowOrd);
                    int? defaultPopupView = reader.IsDBNull(defaultPopupViewOrd) == true ? (int?)null : reader.GetInt32(defaultPopupViewOrd);

                    int? formSelectionAttributeId = reader.IsDBNull(formSelectionAttributeIdOrd) ? (int?)null : reader.GetInt32(formSelectionAttributeIdOrd);

                    int? ownerId = reader.IsDBNull(ownerIdOrd) ? null : (int?)reader.GetInt32(ownerIdOrd);
                    int? supportContactId = reader.IsDBNull(supportContactIdOrd) ? null : (int?)reader.GetInt32(supportContactIdOrd);
                    string supportQuestion = reader.IsDBNull(supportQuestionOrd) ? string.Empty : reader.GetString(supportQuestionOrd);
                    bool enableLocking = reader.IsDBNull(enableLockingOrd) ? false : reader.GetBoolean(enableLockingOrd);
                    bool builtIn = reader.GetBoolean(builtInOrd);
                    Guid? systemCustomEntityId = reader.IsDBNull(systemCustomEntityIdOrd) ? (Guid?)null : (Guid?)reader.GetGuid(systemCustomEntityIdOrd);

                    cTable table = tables.GetTableByID(tableid);
                    cTable audienceTable = tables.GetTableByID(audienceTableID);
                    SortedList<int, cAttribute> lstentityattributes;
                    SortedList<int, cCustomEntityForm> forms;
                    SortedList<int, cCustomEntityView> views;
                    if (sysview)
                    {
                        lstattributes.TryGetValue(systemviewDerivedentityid.Value, out lstentityattributes);
                        lstforms.TryGetValue(systemviewDerivedentityid.Value, out forms);
                        lstviews.TryGetValue(systemviewDerivedentityid.Value, out views);
                    }
                    else
                    {
                        lstattributes.TryGetValue(entityid, out lstentityattributes);
                        lstforms.TryGetValue(entityid, out forms);
                        lstviews.TryGetValue(entityid, out views);
                    }

                    list.Add(entityid, new cCustomEntity(entityid, entityname, pluralname, description, createdon, createdby, modifiedon, modifiedby, lstentityattributes, forms, views, table, audienceTable, enableattachments, audienceViewType, allowdocmergeaccess, sysview, systemviewDerivedentityid, systemviewEntityid, enableCurrencies, defaultCurrencyID, enablePopupWindow, defaultPopupView, formSelectionAttributeId, ownerId, supportContactId, supportQuestion, enableLocking, builtIn, systemCustomEntityId));
                }

                reader.Close();
            }
            lastReadFromDatabaseTicks.AddOrUpdate(nAccountid, addValueFactory: accId => lastUpdated.Ticks,
                                                  updateValueFactory: (accId, old) => lastUpdated.Ticks);

            return list;
        }

        /// <summary>
        /// Refresh the cache 
        /// </summary>
        private void refreshCache(DateTime lastUpdated)
        {
            cUserDefinedFieldsBase.SaveLastUpdatedToCache(nAccountid, lastUpdated);
        }

        /// <summary>
        /// Sorts entity collection into list by entity name key
        /// </summary>
        /// <returns>Sorted list with entity name as key</returns>
        private SortedList<string, cCustomEntity> SortList()
        {
            SortedList<string, cCustomEntity> sorted = new SortedList<string, cCustomEntity>((from x in lst.Values
                                                                                              select x).ToDictionary(a => a.entityname));

            return sorted;
        }

        /// <summary>
        /// Saves the entity to the database. Used for adding and updating entities
        /// </summary>
        /// <param name="entity">The entity instance that is being updated</param>
        /// <returns>The ID of the new/updated entity or -1 if the entity name already exists</returns>
        public int saveEntity(cCustomEntity entity, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@entityid", entity.entityid);
                expdata.sqlexecute.Parameters.AddWithValue("@entityname", entity.entityname);
                expdata.sqlexecute.Parameters.AddWithValue("@pluralname", entity.pluralname);
                if (entity.description == string.Empty)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@description", entity.description);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@enableattachments", Convert.ToByte(entity.EnableAttachments));
                expdata.sqlexecute.Parameters.AddWithValue("@audienceViewType", Convert.ToByte(entity.AudienceView));
                expdata.sqlexecute.Parameters.AddWithValue("@allowdocmergeaccess", Convert.ToByte(entity.AllowMergeConfigAccess));
                expdata.sqlexecute.Parameters.AddWithValue("@enableCurrencies", Convert.ToByte(entity.EnableCurrencies));
                if (entity.DefaultCurrencyID.HasValue == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@defaultCurrencyID", entity.DefaultCurrencyID.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@defaultCurrencyID", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@enablePopupWindow", Convert.ToByte(entity.EnablePopupWindow));

                if (entity.DefaultPopupView.HasValue == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@defaultPopupView", entity.DefaultPopupView.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@defaultPopupView", DBNull.Value);
                }

                if (entity.FormSelectionAttributeId.HasValue)
                {
                    expdata.AddWithValue("@formSelectionAttribute", entity.FormSelectionAttributeId.Value);
                }
                else
                {
                    expdata.AddWithValue("@formSelectionAttribute", DBNull.Value);
                }


                if (entity.OwnerId.HasValue)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@ownerId", entity.OwnerId.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@ownerId", DBNull.Value);
                }

                if (entity.SupportContactId.HasValue)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@supportContactId", entity.SupportContactId.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@supportContactId", DBNull.Value);
                }

                if (entity.SupportQuestion == string.Empty)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@supportQuestion", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@supportQuestion", entity.SupportQuestion);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@enableLocking", entity.EnableLocking);
                expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
                if (entity.modifiedby == null)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@userid", entity.createdby);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@userid", entity.modifiedby);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@builtIn", Convert.ToByte(entity.BuiltIn));

                expdata.sqlexecute.Parameters.AddWithValue("@systemCustomEntityId", entity.SystemCustomEntityId);

                expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("saveCustomEntity");
                int id = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                expdata.sqlexecute.Parameters.Clear();
                var lastDatabaseUpdate = expdata.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
                refreshCache(lastDatabaseUpdate);

                return id;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewentity"></param>
        /// <param name="parententityid"></param>
        /// <param name="relativeAttributeId"></param>
        /// <param name="donorentity"></param>
        /// <returns></returns>
        public int saveEntitySystemView(cCustomEntity viewentity, int parententityid, int relativeAttributeId, cCustomEntity donorentity, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@entityid", viewentity.entityid);
            expdata.sqlexecute.Parameters.AddWithValue("@parententityid", donorentity.entityid);
            expdata.sqlexecute.Parameters.AddWithValue("@systemViewEntityName", viewentity.pluralname);
            expdata.sqlexecute.Parameters.AddWithValue("@relativeattributeid", relativeAttributeId);
            expdata.sqlexecute.Parameters.AddWithValue("@entityname", viewentity.entityname);
            expdata.sqlexecute.Parameters.AddWithValue("@pluralname", viewentity.pluralname);
            if (viewentity.description == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@description", viewentity.description);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@audienceViewType", Convert.ToByte(donorentity.AudienceView));
            expdata.sqlexecute.Parameters.AddWithValue("@allowdocmergeaccess", Convert.ToByte(donorentity.AllowMergeConfigAccess));
            expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
            if (viewentity.modifiedby == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@userid", viewentity.createdby);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@userid", viewentity.modifiedby);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@enablePopupWindow", viewentity.EnablePopupWindow);


            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveCustomEntitySystemView");
            int id = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            var lastDatabaseUpdate = expdata.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
            this.refreshCache(lastDatabaseUpdate);
            this.InitialiseData();

            return id;
        }

        /// <summary>
        /// Delete entity and all related tables, will fail if it is referenced by another entity
        /// </summary>
        /// <param name="entityid">Entity id to delete</param>
        /// <param name="employeeid"></param>
        /// <param name="delegateid"></param>
        /// <returns>Success boolean</returns>
        public int deleteEntity(int entityid, int employeeid, int delegateid, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            var entity = this.getEntityById(entityid);
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@entityid", entityid);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateid);

            expdata.sqlexecute.Parameters.Add("@returnVal", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@returnVal"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("deleteCustomEntity");
            int retVal = (int)expdata.sqlexecute.Parameters["@returnVal"].Value;
            expdata.sqlexecute.Parameters.Clear();

            var lastDatabaseUpdate = expdata.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
            refreshCache(lastDatabaseUpdate);

            return retVal;
        }

        /// <summary>
        /// Delete an attribute from the custom entity
        /// </summary>
        /// <param name="attributeid">Id of the attribute</param>
        /// <param name="employeeid">Id of the entity</param>
        /// <param name="delegateid">delagate id if any</param>
        public int deleteAttribute(int attributeid, int employeeid, int delegateid)
        {
            cCustomEntity entity = getEntityById(getEntityIdByAttributeId(attributeid));
            if (entity != null)
            {
                var attribute = entity.getAttributeById(attributeid);
                return deleteAttributeStoredProcedure(entity, attribute.fieldtype, attributeid, employeeid, delegateid);
            }

            return -1;
        }

        /// <summary>
        /// Delete lookup display field.
        /// </summary>
        /// <param name="triggerFieldAttributeid">
        /// Trigger field attribute id.
        /// </param>
        /// <param name="attributeid">
        /// Attribute id.
        /// </param>
        /// <param name="employeeid">
        /// Employee id.
        /// </param>
        /// <param name="delegateid">
        /// Delegate id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int deleteLookupDisplayField(int triggerFieldAttributeid, int attributeid, int employeeid, int delegateid)
        {

            cCustomEntity entity = getEntityById(getEntityIdByAttributeId(triggerFieldAttributeid));
            if (entity != null)
            {
                return deleteAttributeStoredProcedure(entity, entity.getAttributeById(triggerFieldAttributeid).fieldtype, attributeid, employeeid, delegateid);
            }

            return -1;
        }

        /// <summary>
        /// Call the Delete attribute stored procedure.
        /// </summary>
        /// <param name="entity">
        /// The entity to delete the attribute from.
        /// </param>
        /// <param name="attributeFieldType">
        /// The attribute Field Type.
        /// </param>
        /// <param name="attributeid">
        /// Attribute id.
        /// </param>
        /// <param name="employeeid">
        /// Employee id.
        /// </param>
        /// <param name="delegateid">
        /// Delegate id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int deleteAttributeStoredProcedure(cCustomEntity entity, FieldType attributeFieldType, int attributeid, int employeeid, int delegateid, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            int retCode;
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@attributeid", attributeid);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateid);
            expdata.sqlexecute.Parameters.Add("@retCode", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@retCode"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc(attributeFieldType == FieldType.OTMSummary ? "deleteCustomOneToManySummaryAttribute" : "deleteCustomAttribute");

            retCode = (int)expdata.sqlexecute.Parameters["@retCode"].Value;
            expdata.sqlexecute.Parameters.Clear();

            var lastDatabaseUpdate = expdata.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
            this.refreshCache(lastDatabaseUpdate);

            if (attributeFieldType == FieldType.Attachment)
            {
                var tmpJoin = new cJoin(this._accountid, Guid.Empty, entity.table.TableID, new Guid("92a15f16-bcec-4666-8478-1ef83ed3d076"), "tmpJoin", DateTime.Now);
                cJoins joins = new cJoins(this._accountid);
                joins.CacheDelete(tmpJoin);
            }

            return retCode;
        }

        /// <summary>
        /// Returns the attribute id of the audit identifer attribute for the entity.
        /// </summary>
        /// <param name="entityid">entityid</param>
        /// <param name="accountid">accountid</param>
        /// <returns></returns>
        public int getAuditIdentifierAttributeIDForEntity(int entityid, int accountid)
        {
            cCustomEntity entity = getEntityById(entityid);
            cAttribute auditatt = entity.getAuditIdentifier();
            int attid = 0;
            if (auditatt != null)
            {
                attid = auditatt.attributeid;
            }

            return attid;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewid"></param>
        public int deleteView(int viewid, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@viewid", viewid);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID",
                                                       oCurrentUser.isDelegate
                                                           ? oCurrentUser.Delegate.EmployeeID
                                                           : 0);

            expdata.sqlexecute.Parameters.Add("@retcode", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@retcode"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("deleteCustomEntityView");

            int retcode = (int)expdata.sqlexecute.Parameters["@retcode"].Value;
            expdata.sqlexecute.Parameters.Clear();

            if (retcode == 0)
            {
                var lastDatabaseUpdate = expdata.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
                refreshCache(lastDatabaseUpdate);
            }

            return retcode;
        }

        /// <summary>
        /// Checks if the view is the default view for a new pop-up window.
        /// </summary>
        /// <param name="viewid"></param>
        /// <param name="accountid"></param>
        /// <returns>Count of views</returns> 
        public int checkViewDoesNotBelongToPopupView(int viewid, int accountid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int retcode;

            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@viewid", viewid);

            expdata.sqlexecute.Parameters.Add("@retcode", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@retcode"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("checkIfPopupViewIsInUse");

            retcode = (int)expdata.sqlexecute.Parameters["@retcode"].Value;
            expdata.sqlexecute.Parameters.Clear();

            return retcode;
        }

        /// <summary>
        /// Saves Entity Image data relating to the entity/attribute
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="attributeID"></param>
        /// <param name="imageBinary"></param>
        /// <param name="fileID"></param>
        /// <param name="fileType"></param>
        /// <param name="fileName"></param>
        /// <param name="accountid"></param>
        /// <returns>result</returns> 
        public int SaveCustomeEntityImageData(int entityID, int attributeID, byte[] imageBinary, string fileID, string fileType, string fileName, int accountid)
        {
            int id = 0;
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {

                expdata.sqlexecute.Parameters.Clear();

                expdata.sqlexecute.Parameters.AddWithValue("@entityID", entityID);
                expdata.sqlexecute.Parameters.AddWithValue("@attributeID", attributeID);
                expdata.sqlexecute.Parameters.AddWithValue("@fileID", fileID);
                expdata.sqlexecute.Parameters.AddWithValue("@imageBinary", imageBinary);
                expdata.sqlexecute.Parameters.AddWithValue("@fileType", fileType);
                expdata.sqlexecute.Parameters.AddWithValue("@fileName", fileName);
                expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("saveCustomEntityImageData");

                if (expdata.sqlexecute.Parameters["@identity"].Value != null)
                {
                    id = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                }
            }
            return id;
        }

        /// <summary>
        /// Deletes the image data (attachment) for the specified entity and attribute record
        /// </summary>
        /// <param name="fileId">The file guid</param>
        /// <param name="attributeId">The attribute </param>
        /// <param name="entityId">The entity id</param>
        /// <param name="entityRecordId">The entity record id</param>
        /// <param name="entityPrimaryKey">The entity table primary key</param>
        /// <param name="accountId">The account id</param>
        /// <returns>0 for success</returns>
        public static int DeleteCustomEntityImageData(Guid fileId, int attributeId, int entityId, int entityRecordId, string entityPrimaryKey, int accountId)
        {
            int returnId = 0;

            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@fileID", fileId);
                expdata.sqlexecute.Parameters.AddWithValue("@attributeId", attributeId);
                expdata.sqlexecute.Parameters.AddWithValue("@entityId", entityId);
                expdata.sqlexecute.Parameters.AddWithValue("@entityRecordId", entityRecordId);
                expdata.sqlexecute.Parameters.AddWithValue("@entityPrimaryKey", entityPrimaryKey);
                expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("DeleteCustomEntityImageData");

                if (expdata.sqlexecute.Parameters["@identity"].Value != null)
                {
                    returnId = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                }
            }

            return returnId;
        }

        /// <summary>
        /// Gets the attachment data relating to the attachment type attribute or images uploaded in the formatted text boxes
        /// </summary>
        /// <param name="accountid">The account id</param>
        /// <param name="GuidFileID">The file guid</param>
        /// <returns>List<HTMLImageData></returns> 
        public HTMLImageData GetCustomEntityAttachmentData(int accountid, string GuidFileID = "")
        {
            return CustomEntityImageData.GetData(accountid, GuidFileID);
        }

        /// <summary>
        /// Writes all images to disk relating to an entity/attribute
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="entityID"></param>
        /// <param name="attributeID"></param>    
        public void SaveHTMLEditorImagesToDisk(int accountID, int entityID, int attributeID)
        {
            List<HTMLImageData> imgList;
            System.Drawing.Image newImage;
            string path;

            imgList = GetCustomEntityAttachmentDataByEntityAndAttribute(accountID, entityID, attributeID);

            if (imgList.Count > 0)
            {
                foreach (HTMLImageData image in imgList)
                {
                    using (var stream = new MemoryStream(image.imageData))
                    {
                        try
                        {
                            newImage = System.Drawing.Image.FromStream(stream);
                            path = string.Format("{0}\\{1}.{2}", ConfigurationManager.AppSettings["HostedEntityImageLocation"], image.fileID, image.fileType);
                            newImage.Save(path);
                        }
                        catch (Exception ex)
                        {
                            cEventlog.LogEntry("Failed to save image\n\n" + ex.Message);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Gets the attachment data relating to the attachment type attribute or images uploaded in the formatted text boxes via the entity and attribute ID
        /// </summary>
        /// <param name="accountid">The account id of the current user</param>
        /// <param name="enttiyID">The entity id of the greenlight you want to get the images are for</param>
        /// <param name="attributeID">The attribute id of the field on the greenlight to get the images for</param>
        /// <returns>List<HTMLImageData></returns> 
        private List<HTMLImageData> GetCustomEntityAttachmentDataByEntityAndAttribute(int accountID, int entityID, int attributeID)
        {
            List<HTMLImageData> imageList = new List<HTMLImageData>();
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountID)))
            {
                var Sql = "select fileID, fileType, fileName from CustomEntityImageData where entityID = @entityID AND attributeID = @attributeID";
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@entityID", entityID);
                expdata.sqlexecute.Parameters.AddWithValue("@attributeID", attributeID);

                using (var reader = expdata.GetReader(Sql))
                {
                    var fileNameOrd = reader.GetOrdinal("fileName");

                    while (reader.Read())
                    {
                        var fileGuid = (Guid)reader["fileID"];
                        var fileId = fileGuid.ToString();
                        var fileType = (string)reader["fileType"];
                        string fileName;
                        if (reader.IsDBNull(fileNameOrd))
                        {
                            fileName = string.Empty;
                        }
                        else
                        {
                            fileName = (string)reader["fileName"];
                        }

                        imageList.Add(new HTMLImageData(fileId, fileType, fileName, expdata));
                    }

                }
            }

            return imageList;
        }    

        /// <summary>
        /// Writes an image to disk based on its fileID
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="fileID"></param>
        public string SaveHTMLEditorImagesToDisk(int accountID, string fileID)
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(accountID));
            byte[] imgData;
            string fileType;
            var fileName = string.Empty;
            Guid fileGuid;
            System.Drawing.Image newImage;

            const string Sql = "select fileID, imageBinary, fileType, fileName from CustomEntityImageData where fileID=@fileID";
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@fileID", fileID);

            using (IDataReader reader = expdata.GetReader(Sql))
            {
                while (reader.Read())
                {
                    int fileNameOrd = reader.GetOrdinal("fileName");

                    imgData = (byte[])reader["imageBinary"];
                    fileType = (string)reader["fileType"];
                    if (!reader.IsDBNull(fileNameOrd))
                    {
                        fileName = (string)reader["fileName"];
                    }
                    using (var stream = new MemoryStream(imgData))
                    {
                        string path;
                        newImage = System.Drawing.Image.FromStream(stream);
                        path = ConfigurationManager.AppSettings["tempDocMergeImageLocation"] + fileID + "." + fileType;
                        try
                        {
                            newImage.Save(path);
                        }
                        catch (Exception ex)
                        {
                            cEventlog.LogEntry("Failed to create image\n\n" + ex.Message);
                        }

                    }
                }
            }

            return fileName;
        }

        /// <summary>
        /// Creates duplicate attachment file data against a new FileID, if the fileID passed in already exists
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="connection"></param>
        /// <returns>New Guid ID if attachment data has been duplicate, else returns empty Guid </returns>

        public Guid DuplicateCustomEntityAttributeAttachmentData(Guid fileID, IDBConnection connection = null)
        {
            Guid retcode;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@fileID", fileID);
                expdata.sqlexecute.Parameters.Add("@newfileID", SqlDbType.UniqueIdentifier);
                expdata.sqlexecute.Parameters["@newfileID"].Direction = ParameterDirection.Output;
                expdata.ExecuteProc("duplicateCustomEntityAttributeAttachmentData");
                retcode = (Guid)expdata.sqlexecute.Parameters["@newfileID"].Value;
                expdata.sqlexecute.Parameters.Clear();
            }

            return retcode;
        }

        public byte[] GetWordAttachmentsData(int accountID, string fileID)
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(accountID));
            var result = new byte[0];
            const string Sql = "select fileID, imageBinary, fileType, fileName from CustomEntityImageData where fileID=@fileID";
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@fileID", fileID);

            using (IDataReader reader = expdata.GetReader(Sql))
            {
                while (reader.Read())
                {
                    result = (byte[])reader["imageBinary"];
                }

                reader.Close();
            }

            return result;
        }

        /// <summary>
        /// Deletes file from specified path
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="filetype"></param>
        public void deleteTemporaryImages(string filename, string filetype)
        {
            var physicalPath = ConfigurationManager.AppSettings["HostedEntityImageLocation"] + "\\" + filename + "." + filetype;
            try
            {
                File.Delete(physicalPath);
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry("Failed to delete image\n\n" + ex.Message);
            }

        }

        /// <summary>
        /// checks if file type is an image
        /// </summary>
        /// <param name="filetype"></param>
        /// <returns>true/false</returns>
        public bool CheckFileTypeIsImage(string fileType)
        {
            if (Enum.IsDefined(typeof(ImageFileExtentions), fileType.ToUpper()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formid"></param>
        /// <param name="employeeid"></param>
        /// <param name="delegateid"></param>
        public int deleteForm(int formid, int employeeid, int delegateid, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@formid", formid);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateid);
            expdata.sqlexecute.Parameters.Add("@retcode", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@retcode"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("deleteCustomEntityForm");
            int retcode = (int)expdata.sqlexecute.Parameters["@retcode"].Value;
            expdata.sqlexecute.Parameters.Clear();

            if (retcode == 0)
            {
                var lastDatabaseUpdate = expdata.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
                refreshCache(lastDatabaseUpdate);
            }

            return retcode;
        }

        /// <summary>
        /// Checks if a specified List Attributes' List Item is in use on a CustomEntityViewFilter
        /// </summary>
        /// <param name="listItemID"></param>
        /// <param name="fieldID"></param>
        /// <returns></returns>
        public int CheckListItemIsNotUsedWithinFilter(Guid fieldID, int listItemID, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@listItemID", listItemID);
            expdata.sqlexecute.Parameters.AddWithValue("@fieldID", fieldID);
            expdata.sqlexecute.Parameters.Add("@retcode", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@retcode"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("CheckIfListItemIsUsedWithinFieldFilter");
            int retcode = (int)expdata.sqlexecute.Parameters["@retcode"].Value;
            expdata.sqlexecute.Parameters.Clear();

            return retcode;
        }

        /// <summary>
        /// Get an instance of <see cref="cCustomEntity"/> by it's ID
        /// </summary>
        /// <param name="id">The <see cref="int"/>ID to retrieve</param>
        /// <returns>An instance of <see cref="cCustomEntity"/>or NULL if none found to match</returns>
        public cCustomEntity getEntityById(int id)
        {
            cCustomEntity entity = null;
            lst.TryGetValue(id, out entity);
            return entity;
        }

        /// <summary>
        /// Get entity by name
        /// </summary>
        /// <param name="entityName">entityname</param>
        /// <returns>custom entity</returns>
        public cCustomEntity getEntityByName(string entityName)
        {
            return (from x in lst.Values where x.entityname == entityName select x).FirstOrDefault();
        }


        /// <summary>
        /// Get an instance of <see cref="cCustomEntity"/> by it's table ID
        /// </summary>
        /// <param name="id">The <see cref="Guid"/>ID of the table to retrieve</param>
        /// <returns>An instance of <see cref="cCustomEntity"/>if found, otherwise null</returns>
        public cCustomEntity getEntityByTableId(Guid id)
        {
            cCustomEntity entity = (from x in lst.Values
                                    where x.table.TableID == id
                                    select x).FirstOrDefault();

            return entity;
        }

        /// <summary>
        /// Get an instance of <see cref="cAttribute"/> by it's ID
        /// </summary>
        /// <param name="fieldID">The <see cref="Guid"/>ID of the <seealso cref="cAttribute"/>to retrieve</param>
        /// <returns>An instance of <see cref="cAttribute"/>if found, or null.</returns>
        public cAttribute getAttributeByFieldId(Guid fieldID)
        {
            cAttribute retVal = (from x in lst.Values
                                 from y in x.attributes.Values
                                 where y.fieldid == fieldID
                                 select y).FirstOrDefault();

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstattributes"></param>
        /// <returns></returns>
        private SortedList<int, SortedList<int, cCustomEntityForm>> getForms(SortedList<int, SortedList<int, cAttribute>> lstattributes, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            var lst = new SortedList<int, SortedList<int, cCustomEntityForm>>();
            SortedList<int, cCustomEntityForm> forms;

            SortedList<int, SortedList<int, cCustomEntityFormTab>> lsttabs = getFormTabs();
            SortedList<int, cCustomEntityFormTab> tabs;


            SortedList<int, SortedList<int, cCustomEntityFormSection>> lstsections = getFormSections(lsttabs);
            SortedList<int, cCustomEntityFormSection> sections;

            SortedList<int, SortedList<int, cCustomEntityFormField>> lstfields = getFormFields(lstsections, lstattributes);
            SortedList<int, cCustomEntityFormField> fields;

            const string Strsql = "select entityid, formid, form_name, description, showSave, showSaveAndDuplicate, showSaveAndStay, showSubMenus, showCancel, showBreadcrumbs, saveButtonText, saveAndDuplicateButtonText, saveAndStayButtonText, cancelButtonText, createdon,  createdby, modifiedon, modifiedby, CheckDefaultValues, showSaveAndNew, saveAndNewButtonText, hideTorch, hideAudiences, hideAttachments, builtIn, SystemCustomEntityFormId from dbo.customEntityForms";

            using (IDataReader reader = expdata.GetReader(Strsql))
            {
                var entityIdOrd = reader.GetOrdinal("entityid");
                var formIdOrd = reader.GetOrdinal("formid");
                var formNameOrd = reader.GetOrdinal("form_name");
                var descriptionOrd = reader.GetOrdinal("description");
                var showSaveOrd = reader.GetOrdinal("showSave");
                var showSaveAndDuplicateOrd = reader.GetOrdinal("showSaveAndDuplicate");
                var showSaveAndStayOrd = reader.GetOrdinal("showSaveAndStay");
                var showSubMenusOrd = reader.GetOrdinal("showSubMenus");
                var showCancelOrd = reader.GetOrdinal("showCancel");
                var showBreadcrumbsOrd = reader.GetOrdinal("showBreadcrumbs");
                var saveButtonTextOrd = reader.GetOrdinal("saveButtonText");
                var saveAndDuplicateButtonTextOrd = reader.GetOrdinal("saveAndDuplicateButtonText");
                var saveAndStayButtonTextOrd = reader.GetOrdinal("saveAndStayButtonText");
                var cancelButtonTextOrd = reader.GetOrdinal("cancelButtonText");
                var createdonOrd = reader.GetOrdinal("createdon");
                var createdbyOrd = reader.GetOrdinal("createdby");
                var modifiedonOrd = reader.GetOrdinal("modifiedon");
                var modifiedbyOrd = reader.GetOrdinal("modifiedby");
                var checkDefaultOrdId = reader.GetOrdinal("CheckDefaultValues");
                var showSaveAndNewOrd = reader.GetOrdinal("showSaveAndNew");
                var saveAndNewButtonTextOrd = reader.GetOrdinal("saveAndNewButtonText");
                var hideTorchOrd = reader.GetOrdinal("hideTorch");
                var hideAttachmentsOrd = reader.GetOrdinal("hideAttachments");
                var hideAudiencesOrd = reader.GetOrdinal("hideAudiences");
                var builtInOrdinal = reader.GetOrdinal("builtIn");
                var systemCustomEntityFormIdOrdinal = reader.GetOrdinal("SystemCustomEntityFormId");

                while (reader.Read())
                {
                    int entityid = reader.GetInt32(entityIdOrd);
                    int formid = reader.GetInt32(formIdOrd);
                    string formname = reader.GetString(formNameOrd);
                    string description = reader.IsDBNull(descriptionOrd) ? string.Empty : reader.GetString(descriptionOrd);
                    bool showSave = reader.GetBoolean(showSaveOrd);
                    bool showSaveAndDuplicate = reader.GetBoolean(showSaveAndDuplicateOrd);
                    bool showSaveAndStay = reader.GetBoolean(showSaveAndStayOrd);
                    bool showSubMenus = reader.GetBoolean(showSubMenusOrd);
                    bool showCancel = reader.GetBoolean(showCancelOrd);
                    bool showBreadcrumbs = reader.GetBoolean(showBreadcrumbsOrd);
                    string saveText = reader.GetString(saveButtonTextOrd);
                    string saveAndDuplicateText = reader.GetString(saveAndDuplicateButtonTextOrd);
                    string saveAndStayText = reader.GetString(saveAndStayButtonTextOrd);
                    string cancelText = reader.GetString(cancelButtonTextOrd);
                    DateTime createdon = reader.GetDateTime(createdonOrd);
                    int createdby = reader.GetInt32(createdbyOrd);
                    DateTime? modifiedon = reader.IsDBNull(modifiedonOrd) ? (DateTime?)null : reader.GetDateTime(modifiedonOrd);
                    int? modifiedby = reader.IsDBNull(modifiedbyOrd) ? (int?)null : reader.GetInt32(modifiedbyOrd);
                    var checkDefaultValues = reader.GetBoolean(checkDefaultOrdId);
                    var showSaveAndNew = reader.GetBoolean(showSaveAndNewOrd);
                    var saveAndNewButtontext = reader.GetString(saveAndNewButtonTextOrd);

                    var hideTorch = reader.GetBoolean(hideTorchOrd);
                    var hideAttachments = reader.GetBoolean(hideAttachmentsOrd);
                    var hideAudiences = reader.GetBoolean(hideAudiencesOrd);
                    var builtIn = reader.GetBoolean(builtInOrdinal);
                    Guid? systemCustomEntityFormId = reader.IsDBNull(systemCustomEntityFormIdOrdinal) ? (Guid?)null : reader.GetGuid(systemCustomEntityFormIdOrdinal);

                    lsttabs.TryGetValue(formid, out tabs);
                    if (tabs == null)
                    {
                        tabs = new SortedList<int, cCustomEntityFormTab>();
                    }

                    lstsections.TryGetValue(formid, out sections);
                    if (sections == null)
                    {
                        sections = new SortedList<int, cCustomEntityFormSection>();
                    }

                    lst.TryGetValue(entityid, out forms);
                    lstfields.TryGetValue(formid, out fields);
                    if (fields == null)
                    {
                        fields = new SortedList<int, cCustomEntityFormField>();
                    }

                    if (forms == null)
                    {
                        forms = new SortedList<int, cCustomEntityForm>();
                        lst.Add(entityid, forms);
                    }

                    forms.Add(formid, new cCustomEntityForm(formid, entityid, formname, description, showSave, saveText, showSaveAndDuplicate, saveAndDuplicateText, showSaveAndStay, saveAndStayText, showCancel, cancelText, showSubMenus, showBreadcrumbs, createdon, createdby, modifiedon, modifiedby, tabs, sections, fields, saveAndNewButtontext, showSaveAndNew, checkDefaultValues, hideTorch, hideAttachments, hideAudiences, builtIn, systemCustomEntityFormId));
                }

                reader.Close();
            }

            return lst;
        }

        /// <summary>
        /// Gets fields inside a GreenLight form
        /// </summary>
        /// <param name="lstsections"></param>
        /// <param name="lstattributes"></param>
        /// <returns>List of custom entity form field objects</returns>
        private SortedList<int, SortedList<int, cCustomEntityFormField>> getFormFields(SortedList<int, SortedList<int, cCustomEntityFormSection>> lstsections, SortedList<int, SortedList<int, cAttribute>> lstattributes, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            SortedList<int, SortedList<int, cCustomEntityFormField>> lst = new SortedList<int, SortedList<int, cCustomEntityFormField>>();
            SortedList<int, cCustomEntityFormField> fields;
            SortedList<int, cCustomEntityFormSection> sections;
            SortedList<int, cAttribute> attributes;
            cCustomEntityFormField formfield;
            int formid, attributeid, sectionid;
            byte column, row;
            bool isreadonly;
            int entityid;
            cCustomEntityFormSection section;
            cAttribute attribute;

            ////                        0                           1               2       3       4          5           6        7           8
            string strsql = "select entityid, customEntityFormFields.formid, readonly, [row], [column], attributeid, sectionid, labelText, DefaultValue, FormMandatory  from customEntityFormFields inner join customEntityForms on customEntityForms.formid = customEntityFormFields.formid order by [row], [column]";

            using (IDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    entityid = reader.GetInt32(0);
                    formid = reader.GetInt32(1);
                    isreadonly = reader.GetBoolean(2);
                    row = reader.GetByte(3);
                    column = reader.GetByte(4);
                    attributeid = reader.GetInt32(5);
                    var isMandatory = reader.IsDBNull(9) ? (bool?)null : reader.GetBoolean(9);
                    lstattributes.TryGetValue(entityid, out attributes);
                    attribute = null;
                    if (attributes != null)
                    {
                        attributes.TryGetValue(attributeid, out attribute);
                        if (attribute == null)
                        {
                            foreach (LookupDisplayField lookupDisplayField in from keyValuePair in attributes where keyValuePair.Value.GetType() == typeof(cManyToOneRelationship) select (cManyToOneRelationship)keyValuePair.Value into manyToOne from lookupDisplayField in manyToOne.TriggerLookupFields where lookupDisplayField.attributeid == attributeid select lookupDisplayField)
                            {
                                attribute = lookupDisplayField;
                            }
                        }
                    }

                    section = null;

                    if (!reader.IsDBNull(6))
                    {
                        lstsections.TryGetValue(formid, out sections);
                        if (sections != null)
                        {
                            sectionid = reader.GetInt32(6);
                            sections.TryGetValue(sectionid, out section);
                        }
                    }

                    lst.TryGetValue(formid, out fields);
                    if (fields == null)
                    {
                        fields = new SortedList<int, cCustomEntityFormField>();
                        lst.Add(formid, fields);
                    }

                    string labeltext = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);

                    string defaultValue = reader.IsDBNull(8) ? null : reader.GetString(8);

                    formfield = new cCustomEntityFormField(formid, attribute, isreadonly, section, column, row, labeltext, isMandatory, defaultValue);
                    section.fields.Add(formfield);
                    fields.Add(attributeid, formfield);
                }

                reader.Close();
            }

            return lst;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private SortedList<int, SortedList<int, cCustomEntityFormTab>> getFormTabs(IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            SortedList<int, SortedList<int, cCustomEntityFormTab>> lst = new SortedList<int, SortedList<int, cCustomEntityFormTab>>();
            SortedList<int, cCustomEntityFormTab> tabs;
            int tabid, formid;
            string headercaption;
            byte order;

            IDataReader reader;

            // :                      0       1         2             3
            string strsql = "select tabid, formid, header_caption, [order] from customEntityFormTabs";
            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    tabid = reader.GetInt32(0);
                    formid = reader.GetInt32(1);
                    headercaption = reader.GetString(2);
                    order = reader.GetByte(3);
                    lst.TryGetValue(formid, out tabs);
                    if (tabs == null)
                    {
                        tabs = new SortedList<int, cCustomEntityFormTab>();
                        lst.Add(formid, tabs);
                    }

                    tabs.Add(tabid, new cCustomEntityFormTab(tabid, formid, headercaption, order));
                }

                reader.Close();
            }

            return lst;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lsttabs"></param>
        /// <returns></returns>
        private SortedList<int, SortedList<int, cCustomEntityFormSection>> getFormSections(SortedList<int, SortedList<int, cCustomEntityFormTab>> lsttabs, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            SortedList<int, SortedList<int, cCustomEntityFormSection>> lst = new SortedList<int, SortedList<int, cCustomEntityFormSection>>();
            SortedList<int, cCustomEntityFormSection> sections;
            SortedList<int, cCustomEntityFormTab> tabs;
            int sectionid, tabid, formid;
            string headercaption;
            byte order;
            cCustomEntityFormTab tab;
            cCustomEntityFormSection section;
            IDataReader reader;
            //                          0         1          2            3       4
            string strsql = "select sectionid, formid, header_caption, [order], tabid from customEntityFormSections";

            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    sectionid = reader.GetInt32(0);
                    formid = reader.GetInt32(1);
                    headercaption = reader.GetString(2);
                    order = reader.GetByte(3);
                    tab = null;
                    if (!reader.IsDBNull(4))
                    {
                        tabid = reader.GetInt32(4);
                        lsttabs.TryGetValue(formid, out tabs);
                        if (tabs != null)
                        {
                            tabs.TryGetValue(tabid, out tab);
                        }
                    }

                    lst.TryGetValue(formid, out sections);
                    if (sections == null)
                    {
                        sections = new SortedList<int, cCustomEntityFormSection>();
                        lst.Add(formid, sections);
                    }

                    section = new cCustomEntityFormSection(sectionid, formid, headercaption, order, tab);
                    tab.sections.Add(section);
                    sections.Add(sectionid, section);
                }

                reader.Close();
            }

            return lst;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private SortedList<int, SortedList<int, cListAttributeElement>> getAttributeListItems(IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            const string Strsql = "select valueid, item, [order], attributeid, archived from customEntityAttributeListItems";
            var lst = new SortedList<int, SortedList<int, cListAttributeElement>>();
            SortedList<int, cListAttributeElement> items;

            using (IDataReader reader = expdata.GetReader(Strsql))
            {
                var valueidOrd = reader.GetOrdinal("valueid");
                var itemOrd = reader.GetOrdinal("item");
                var orderOrd = reader.GetOrdinal("order");
                var attributeidOrd = reader.GetOrdinal("attributeid");
                var archivedOrd = reader.GetOrdinal("archived");
                while (reader.Read())
                {
                    int valueid = reader.GetInt32(valueidOrd);
                    int attributeid = reader.GetInt32(attributeidOrd);
                    string item = reader.GetString(itemOrd);
                    int order = reader.GetInt32(orderOrd);
                    var archived = reader.GetBoolean(archivedOrd);
                    lst.TryGetValue(attributeid, out items);
                    if (items == null)
                    {
                        items = new SortedList<int, cListAttributeElement>();
                        lst.Add(attributeid, items);
                    }

                    cListAttributeElement element = new cListAttributeElement(valueid, item, order, archived);
                    items.Add(order, element);
                }

                reader.Close();
            }

            return lst;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private SortedList<int, SortedList<int, cAttribute>> getAttributes(IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            SortedList<int, List<LookupDisplayField>> triggerAttributes = getLookupAttributes();
            SortedList<int, SortedList<int, cListAttributeElement>> lstitems = getAttributeListItems();
            SortedList<int, cListAttributeElement> items;
            SortedList<int, SortedList<int, cAttribute>> lst = new SortedList<int, SortedList<int, cAttribute>>();
            SortedList<int, cAttribute> attributes;
            AttributeFormat format;
            RelationshipType relationshiptype;
            cWorkflows clsworkflows = new cWorkflows(this.oCurrentUser);
            int attributeid, entityid;
            int viewid;
            int relatedentity;
            int? maxlength;
            string attributename, displayname, description, tooltip;
            Guid fieldid;
            DateTime createdon;
            int createdby;
            Guid relatedtableid = Guid.Empty;
            DateTime? modifiedon;
            int? modifiedby;
            bool mandatory, iskeyfield, isauditidentifier, isunique, allowEdit, allowDelete, displayInMobile, builtIn;
            byte precision;
            string defaultvalue;
            FieldType fieldtype;
            string commentText;
            Guid aliasTableID;
            int maxRows = 15;
            bool sysAttribute = false;
            bool boolAttribute = false;

            const string Sql = "select entityid, attributeid, display_name, description, tooltip, mandatory, DisplayInMobile, createdon, createdby, modifiedon, modifiedby, fieldtype, fieldid, is_key_field, is_audit_identity, is_unique, maxlength, format, precision, defaultvalue, relatedtable, relationshiptype, viewid, related_entity, workflowid, allowEdit, allowDelete, advicePanelText, aliasTableID, relationshipDisplayField, maxRows, system_attribute, BoolAttribute, BuiltIn from customEntityAttributes where fieldtype <> @lookupType";
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@lookupType", (int)FieldType.LookupDisplayField);

            using (IDataReader reader = expdata.GetReader(Sql))
            {
                #region reader ordinals

                int entityID_Ord = reader.GetOrdinal("entityid");
                int attributeID_Ord = reader.GetOrdinal("attributeid");
                int displayname_Ord = reader.GetOrdinal("display_name");
                int description_Ord = reader.GetOrdinal("description");
                int tootip_Ord = reader.GetOrdinal("tooltip");
                int mandatory_Ord = reader.GetOrdinal("mandatory");
                int displayInMobile_Ord = reader.GetOrdinal("DisplayInMobile");
                int createdon_Ord = reader.GetOrdinal("createdon");
                int createdby_Ord = reader.GetOrdinal("createdby");
                int modifiedon_Ord = reader.GetOrdinal("modifiedon");
                int modifiedby_Ord = reader.GetOrdinal("modifiedby");
                int fieldtype_Ord = reader.GetOrdinal("fieldtype");
                int fieldid_Ord = reader.GetOrdinal("fieldid");
                int iskeyfield_Ord = reader.GetOrdinal("is_key_field");
                int isauditidentity_Ord = reader.GetOrdinal("is_audit_identity");
                int isunique_Ord = reader.GetOrdinal("is_unique");
                int maxlength_Ord = reader.GetOrdinal("maxlength");
                int format_Ord = reader.GetOrdinal("format");
                int precision_Ord = reader.GetOrdinal("precision");
                int defaultvalue_Ord = reader.GetOrdinal("defaultvalue");
                int relatedtable_Ord = reader.GetOrdinal("relatedtable");
                int relationshiptype_Ord = reader.GetOrdinal("relationshiptype");
                int viewid_Ord = reader.GetOrdinal("viewid");
                int relatedentity_Ord = reader.GetOrdinal("related_entity");
                int workflowid_Ord = reader.GetOrdinal("workflowid");
                int allowEdit_Ord = reader.GetOrdinal("allowEdit");
                int allowDelete_Ord = reader.GetOrdinal("allowDelete");
                int advicePanelText_Ord = reader.GetOrdinal("advicePanelText");
                int aliasTableID_Ord = reader.GetOrdinal("aliasTableID");
                int relationshipDisplayField_Ord = reader.GetOrdinal("relationshipDisplayField");
                int maxRows_Ord = reader.GetOrdinal("maxRows");
                int systemAttribute_Ord = reader.GetOrdinal("system_attribute");
                int boolAttribute_Ord = reader.GetOrdinal("BoolAttribute");
                int builtIn_Ord = reader.GetOrdinal("BuiltIn");

                #endregion

                expdata.sqlexecute.Parameters.Clear();
                var lstfilters = getManyToOneAttributeFilters();

                while (reader.Read())
                {
                    entityid = reader.GetInt32(entityID_Ord);
                    attributeid = reader.GetInt32(attributeID_Ord);
                    displayname = reader.GetString(displayname_Ord);

                    if (reader.IsDBNull(description_Ord))
                    {
                        description = string.Empty;
                    }
                    else
                    {
                        description = reader.GetString(description_Ord);
                    }

                    if (reader.IsDBNull(tootip_Ord))
                    {
                        tooltip = string.Empty;
                    }
                    else
                    {
                        // escape any apostrophes to prevent javascript errors
                        tooltip = reader.GetString(tootip_Ord);
                        tooltip = tooltip.Replace("'", "\\'").Replace("\\\\'", "\\'");
                        tooltip = tooltip.Replace("\"", "&quot;");
                    }

                    mandatory = reader.GetBoolean(mandatory_Ord);
                    displayInMobile = reader.GetBoolean(displayInMobile_Ord);
                    builtIn = reader.GetBoolean(builtIn_Ord);
                    createdon = reader.GetDateTime(createdon_Ord);
                    createdby = reader.GetInt32(createdby_Ord);
                    if (reader.IsDBNull(modifiedon_Ord))
                    {
                        modifiedon = null;
                    }
                    else
                    {
                        modifiedon = reader.GetDateTime(modifiedon_Ord);
                    }

                    if (reader.IsDBNull(modifiedby_Ord))
                    {
                        modifiedby = null;
                    }
                    else
                    {
                        modifiedby = reader.GetInt32(modifiedby_Ord);
                    }

                    lst.TryGetValue(entityid, out attributes);
                    if (attributes == null)
                    {
                        attributes = new SortedList<int, cAttribute>();
                        lst.Add(entityid, attributes);
                    }

                    fieldtype = (FieldType)reader.GetByte(fieldtype_Ord);
                    fieldid = reader.GetGuid(fieldid_Ord);
                    iskeyfield = reader.GetBoolean(iskeyfield_Ord);
                    isauditidentifier = reader.GetBoolean(isauditidentity_Ord);
                    isunique = reader.GetBoolean(isunique_Ord);
                    allowEdit = reader.GetBoolean(allowEdit_Ord);
                    allowDelete = reader.GetBoolean(allowDelete_Ord);
                    if (!reader.IsDBNull(advicePanelText_Ord))
                    {
                        commentText = reader.GetString(advicePanelText_Ord);
                    }
                    else
                    {
                        commentText = string.Empty;
                    }

                    boolAttribute = reader.GetBoolean(boolAttribute_Ord);

                    if (!reader.IsDBNull(aliasTableID_Ord))
                    {
                        aliasTableID = reader.GetGuid(aliasTableID_Ord);
                    }
                    else
                    {
                        aliasTableID = Guid.Empty;
                    }

                    sysAttribute = reader.GetBoolean(systemAttribute_Ord);

                    if (sysAttribute)
                    {
                        switch (displayname.ToLower())
                        {
                            case "created on":
                            case "created by":
                            case "modified on":
                            case "modified by":
                                attributename = displayname.Replace(" ", string.Empty);
                                break;
                            default:
                                if (fieldtype == FieldType.CurrencyList && displayname.ToLower() == "greenlight currency")
                                    attributename = "GreenLightCurrency";
                                else
                                    attributename = "att" + attributeid.ToString();
                                break;
                        }
                    }
                    else
                    {
                        attributename = "att" + attributeid.ToString();
                    }

                    switch (fieldtype)
                    {
                        case FieldType.Text:
                        case FieldType.LargeText:
                            if (reader.IsDBNull(maxlength_Ord))
                            {
                                maxlength = null;
                            }
                            else
                            {
                                maxlength = reader.GetInt32(maxlength_Ord);
                            }

                            format = (AttributeFormat)reader.GetByte(format_Ord);
                            attributes.Add(attributeid, new cTextAttribute(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, maxlength, format, fieldid, isauditidentifier, isunique, allowEdit, allowDelete, boolAttribute, displayInMobile, builtIn, sysAttribute));
                            break;
                        case FieldType.Integer:
                            attributes.Add(attributeid, new cNumberAttribute(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, 0, fieldid, iskeyfield, isauditidentifier, isunique, allowEdit, allowDelete, displayInMobile, builtIn, sysAttribute));
                            break;
                        case FieldType.Number:
                            if (reader.IsDBNull(precision_Ord))
                            {
                                precision = 0;
                            }
                            else
                            {
                                precision = reader.GetByte(precision_Ord);
                            }

                            attributes.Add(attributeid, new cNumberAttribute(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, precision, fieldid, false, isauditidentifier, isunique, allowEdit, allowDelete, displayInMobile, builtIn, sysAttribute));
                            break;
                        case FieldType.Currency:
                            attributes.Add(attributeid, new cNumberAttribute(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, 2, fieldid, false, isauditidentifier, isunique, allowEdit, allowDelete, displayInMobile, builtIn, sysAttribute));
                            break;
                        case FieldType.DateTime:
                            format = (AttributeFormat)reader.GetByte(format_Ord);
                            attributes.Add(attributeid, new cDateTimeAttribute(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, format, fieldid, isauditidentifier, isunique, allowEdit, allowDelete, displayInMobile, builtIn, system_attribute: sysAttribute));
                            break;
                        case FieldType.TickBox:
                            if (reader.IsDBNull(defaultvalue_Ord))
                            {
                                defaultvalue = string.Empty;
                            }
                            else
                            {
                                defaultvalue = reader.GetString(defaultvalue_Ord);
                            }

                            attributes.Add(attributeid, new cTickboxAttribute(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, defaultvalue, fieldid, isauditidentifier, isunique, allowEdit, allowDelete, displayInMobile, builtIn, system_attribute: sysAttribute));
                            break;
                        case FieldType.Relationship:
                            relatedtableid = reader.GetGuid(relatedtable_Ord);
                            relationshiptype = (RelationshipType)reader.GetByte(relationshiptype_Ord);
                            cTable relatedtable = tables.GetTableByID(relatedtableid);

                            if (relationshiptype == RelationshipType.ManyToOne)
                            {
                                Guid displayField = Guid.Empty;
                                if (!reader.IsDBNull(relationshipDisplayField_Ord))
                                {
                                    displayField = reader.GetGuid(relationshipDisplayField_Ord);
                                }

                                if (!reader.IsDBNull(maxRows_Ord))
                                {
                                    maxRows = reader.GetInt32(maxRows_Ord);
                                }

                                List<Guid> matchFields = getRelationshipMatchFields(attributeid);
                                var autocompleteResultsDisplayFields  = this.GetAutocompleteLookupDisplayFields(attributeid);

                                SortedList<int, FieldFilter> filters;
                                lstfilters.TryGetValue(attributeid, out filters);

                                List<LookupDisplayField> triggerFields = triggerAttributes.ContainsKey(attributeid) ? triggerAttributes[attributeid] : new List<LookupDisplayField>();

                                cTable aliasTable = tables.GetTableByID(aliasTableID);
                                attributes.Add(attributeid, new cManyToOneRelationship(attributeid, attributename, displayname, description, tooltip, mandatory, builtIn, createdon, createdby, modifiedon, modifiedby, relatedtable, fieldid, isauditidentifier, allowEdit, allowDelete, aliasTable, displayField, matchFields,maxRows, autocompleteResultsDisplayFields,filters, sysAttribute, triggerFields));
                            }
                            else
                            {
                                viewid = reader.GetInt32(viewid_Ord);
                                relatedentity = reader.GetInt32(relatedentity_Ord);
                                attributes.Add(attributeid, new cOneToManyRelationship(attributeid, attributename, displayname, description, tooltip, mandatory, builtIn, createdon, createdby, modifiedon, modifiedby, relatedtable, fieldid, viewid, relatedentity, isauditidentifier, entityid, allowEdit, allowDelete, sysAttribute));
                            }

                            break;
                        case FieldType.List:
                            lstitems.TryGetValue(attributeid, out items);
                            if (items == null)
                            {
                                items = new SortedList<int, cListAttributeElement>();
                            }

                            if (reader.IsDBNull(format_Ord))
                            {
                                format = AttributeFormat.ListStandard;
                            }
                            else
                            {
                                format = (AttributeFormat)reader.GetByte(format_Ord);
                            }

                            attributes.Add(attributeid, new cListAttribute(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, items, fieldid, isauditidentifier, isunique, format, allowEdit, allowDelete, displayInMobile, builtIn, sysAttribute));
                            break;
                        case FieldType.RunWorkflow:
                            int workflowid = reader.GetInt32(workflowid_Ord);
                            cWorkflow workflow = clsworkflows.GetWorkflowByID(workflowid);
                            attributes.Add(attributeid, new cRunWorkflowAttribute(attributeid, attributename, displayname, description, tooltip, mandatory, createdon, createdby, modifiedon, modifiedby, fieldid, workflow, allowEdit, allowDelete, displayInMobile, builtIn, sysAttribute));
                            break;
                        case FieldType.OTMSummary:
                            relatedentity = reader.GetInt32(relatedentity_Ord);
                            attributes.Add(attributeid, new cSummaryAttribute(attributeid, attributename, displayname, description, createdon, createdby, modifiedon, modifiedby, fieldid, getSummaryAttributeElements(attributeid), getSummaryAttributeColumns(attributeid), relatedentity, allowEdit, allowDelete, sysAttribute));
                            break;
                        case FieldType.CurrencyList:
                            attributes.Add(attributeid, new cCurrencyListAttribute(attributeid, attributename, displayname, description, createdon, createdby, modifiedon, modifiedby, fieldid, allowEdit, allowDelete, displayInMobile, sysAttribute));
                            break;
                        case FieldType.Comment:
                            attributes.Add(attributeid, new cCommentAttribute(attributeid, attributename, displayname, description, tooltip, mandatory, createdon, createdby, modifiedon, modifiedby, commentText, fieldid, isauditidentifier, isunique, allowEdit, allowDelete, displayInMobile, builtIn, sysAttribute));
                            break;
                        case FieldType.Attachment:
                            format = (AttributeFormat)reader.GetByte(format_Ord);
                            attributes.Add(attributeid, new cAttachmentAttribute(attributeid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, fieldid, format, isauditidentifier, isunique, boolAttribute, allowEdit, allowDelete, displayInMobile, builtIn, sysAttribute));
                            break;
                        case FieldType.Contact:
                            format = (AttributeFormat) reader.GetByte(format_Ord);
                            attributes.Add(attributeid, new cContactAttribute(attributeid, attributename, displayname, description, tooltip, mandatory, createdon, createdby, modifiedon, modifiedby, fieldid, format, isauditidentifier, isunique, allowEdit, allowDelete, displayInMobile, builtIn));
                            break;
                    }
                }

                reader.Close();
            }

            return lst;
        }

        /// <summary>
        /// Gets the Lookup Display (Trigger) attributes
        /// </summary>
        /// <param name="expdata">Database connection object</param>
        /// <returns>Collection of lookup display attributes for each </returns>
        private SortedList<int, List<LookupDisplayField>> getLookupAttributes(IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            SortedList<int, List<LookupDisplayField>> lookupDisplayFields = new SortedList<int, List<LookupDisplayField>>();
            int attributeid;
            string attributename, displayname, description, tooltip;
            Guid fieldid;
            DateTime createdon;
            int createdby;
            DateTime? modifiedon;
            int? modifiedby;
            int? triggerAttributeID;
            int? triggerJoinViaID;
            Guid? triggerDisplayFieldID;
            JoinVia triggerJoinVia;

            const string Sql = "select attributeid, display_name, description, tooltip, createdon, createdby, modifiedon, modifiedby, fieldid, TriggerAttributeId, TriggerJoinViaId, TriggerDisplayFieldId from customEntityAttributes where fieldtype = @lookupType";
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@lookupType", (int)FieldType.LookupDisplayField);

            using (IDataReader reader = expdata.GetReader(Sql))
            {
                #region reader ordinals

                int attributeID_Ord = reader.GetOrdinal("attributeid");
                int displayname_Ord = reader.GetOrdinal("display_name");
                int description_Ord = reader.GetOrdinal("description");
                int tootip_Ord = reader.GetOrdinal("tooltip");
                int createdon_Ord = reader.GetOrdinal("createdon");
                int createdby_Ord = reader.GetOrdinal("createdby");
                int modifiedon_Ord = reader.GetOrdinal("modifiedon");
                int modifiedby_Ord = reader.GetOrdinal("modifiedby");
                int fieldid_Ord = reader.GetOrdinal("fieldid");
                int triggerAttributeID_Ord = reader.GetOrdinal("TriggerAttributeID");
                int triggerJoinViaID_Ord = reader.GetOrdinal("TriggerJoinViaID");
                int triggerDisplayFieldID_Ord = reader.GetOrdinal("TriggerDisplayFieldID");

                #endregion

                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    attributeid = reader.GetInt32(attributeID_Ord);
                    displayname = reader.GetString(displayname_Ord);

                    if (reader.IsDBNull(description_Ord))
                    {
                        description = string.Empty;
                    }
                    else
                    {
                        description = reader.GetString(description_Ord);
                    }

                    if (reader.IsDBNull(tootip_Ord))
                    {
                        tooltip = string.Empty;
                    }
                    else
                    {
                        // escape any apostrophes to prevent javascript errors
                        tooltip = reader.GetString(tootip_Ord);
                        tooltip = tooltip.Replace("'", "\\'").Replace("\\\\'", "\\'");
                        tooltip = tooltip.Replace("\"", "&quot;");
                    }

                    createdon = reader.GetDateTime(createdon_Ord);
                    createdby = reader.GetInt32(createdby_Ord);
                    if (reader.IsDBNull(modifiedon_Ord))
                    {
                        modifiedon = null;
                    }
                    else
                    {
                        modifiedon = reader.GetDateTime(modifiedon_Ord);
                    }

                    if (reader.IsDBNull(modifiedby_Ord))
                    {
                        modifiedby = null;
                    }
                    else
                    {
                        modifiedby = reader.GetInt32(modifiedby_Ord);
                    }

                    fieldid = reader.GetGuid(fieldid_Ord);
                    attributename = "att" + attributeid.ToString();

                    if (!reader.IsDBNull(triggerAttributeID_Ord))
                    {
                        triggerAttributeID = reader.GetInt32(triggerAttributeID_Ord);
                    }
                    else
                    {
                        triggerAttributeID = null;
                    }

                    if (!reader.IsDBNull(triggerJoinViaID_Ord))
                    {
                        triggerJoinViaID = reader.GetInt32(triggerJoinViaID_Ord);
                        JoinVias jvs = new JoinVias(this.oCurrentUser);
                        triggerJoinVia = jvs.GetJoinViaByID((int)triggerJoinViaID);
                    }
                    else
                    {
                        triggerJoinViaID = null;
                        triggerJoinVia = null;
                    }

                    if (!reader.IsDBNull(triggerDisplayFieldID_Ord))
                    {
                        triggerDisplayFieldID = reader.GetGuid(triggerDisplayFieldID_Ord);
                    }
                    else
                    {
                        triggerDisplayFieldID = null;
                    }

                    List<LookupDisplayField> ludf = null;

                    if (!triggerAttributeID.HasValue)
                    {
                        continue;
                    }

                    ludf = new List<LookupDisplayField>();
                    if (lookupDisplayFields.ContainsKey(triggerAttributeID.Value))
                    {
                        lookupDisplayFields.TryGetValue(triggerAttributeID.Value, out ludf);
                        lookupDisplayFields.Remove(triggerAttributeID.Value);
                    }

                    ludf.Add(new LookupDisplayField(attributeid, attributename, displayname, description, tooltip, createdon, createdby, modifiedon, modifiedby, fieldid, triggerAttributeID, triggerJoinVia, triggerDisplayFieldID));

                    lookupDisplayFields.Add(triggerAttributeID.Value, ludf);
                }

                reader.Close();
            }

            return lookupDisplayFields;
        }

        /// <summary>
        /// Retrieves type information for the view filter modal. It remaps the type into a more finite set.
        /// </summary>
        /// <param name="fieldid">Field ID</param>
        /// <param name="accountid"> </param>
        /// <param name="subaccountid"> </param>
        /// <returns>Returns type, list or precision</returns>
        public object[] GetFieldInfoForFilter(Guid fieldid, int accountid, int subaccountid)
        {
            cField field = fields.GetFieldByID(fieldid);
            List<ListItem> list = new List<ListItem>();
            bool isList = false;
            int precision = 2;

            string type = field.FieldType;

            if (field.FieldSource != cField.FieldSourceType.Metabase && type == "M")
            {
                type = "FD";
            }

            if (field.ValueList)
            {
                isList = true;
                type = "L";
                list.AddRange(field.ListItems.OrderBy(listitem => listitem.Value).Select(o => new ListItem(o.Value, o.Key.ToString())));
            }

            if (field.GenList && !field.ValueList)
            {
                isList = true;
                type = "GL";
                list = FieldFilters.GetGenListItemsForField(fieldid, oCurrentUser);
            }

            if (field.FieldType == "CL")
            {
                isList = true;
                list = Currencies.GetCurrencyList(accountid, subaccountid);
            }

            if (type == "M" && field.FieldSource == cField.FieldSourceType.Metabase)
            {
                type = "C";
            }

            if (type == "F" || type == "FD" || type == "M")
            {
                cNumberAttribute na;
                switch (field.FieldSource)
                {
                    case cField.FieldSourceType.Metabase:
                        precision = 2;
                        break;
                    case cField.FieldSourceType.CustomEntity:
                        na = (cNumberAttribute)getAttributeByFieldId(fieldid);
                        precision = na.precision;
                        break;
                    case cField.FieldSourceType.Userdefined:
                        cUserDefinedFieldsBase udfs = new cUserdefinedFields(accountid);
                        cUserDefinedField udf = udfs.GetUserdefinedFieldByFieldID(fieldid);
                        na = (cNumberAttribute)udf.attribute;
                        precision = na.precision;
                        break;
                    default:
                        precision = 2;
                        break;
                }

                if (type == "F")
                {
                    type = "FD";
                }
            }

            if (type == "FC" || type == "M" || type == "A")
            {
                type = "C";
            }

            if (type == "I" || type == "FI")
            {
                type = "N";
            }

            if (type == "Y")
            {
                type = "X";
            }

            // shouldn't receive guids here
            if (type == "G" || type == "U" || type == "FU")
            {
                type = "S";
            }

            object[] ret = new object[2];
            ret[0] = type;
            ret[1] = isList ? (object)list : (object)precision;
            return ret;
        }

        private List<Guid> getRelationshipMatchFields(int attributeid, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            List<Guid> matchIDs = new List<Guid>();
            string sql = "select fieldid from customEntityAttributeMatchFields where attributeId = @attId";
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@attId", attributeid);
            using (IDataReader reader = expdata.GetReader(sql))
            {
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        matchIDs.Add(reader.GetGuid(0));
                }

                reader.Close();
            }

            return matchIDs;
        }

        /// <summary>
        /// This method get lists of fields that should display in the autocomplete search results
        /// </summary>
        /// <param name="attributeid">Attribute for which the lookup fields should be fetched</param>
        /// <returns>The <see cref="Guid"/>List of Fields ids</returns>
        private List<Guid> GetAutocompleteLookupDisplayFields(int attributeid)
        {
            var autoCompleteFieldIds = new List<Guid>();
            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue("@attributeId", attributeid);
                using (var reader = databaseConnection.GetReader("GetAutocompleteSearchResultsFields", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            autoCompleteFieldIds.Add(reader.GetGuid(0));
                        }
                    }
                }

                return autoCompleteFieldIds;
            }
        }

        private Dictionary<int, cSummaryAttributeElement> getSummaryAttributeElements(int attributeid, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            Dictionary<int, cSummaryAttributeElement> retSA = new Dictionary<int, cSummaryAttributeElement>();
            expdata.sqlexecute.Parameters.Clear();
            ////                          0                   1           2
            string sql = "select summary_attributeid, otm_attributeid, [order] from customEntityAttributeSummary where attributeid = @attId order by [order]";
            expdata.sqlexecute.Parameters.AddWithValue("@attId", attributeid);

            using (IDataReader reader = expdata.GetReader(sql))
            {
                while (reader.Read())
                {
                    int sid = reader.GetInt32(0);
                    int otmid = reader.GetInt32(1);

                    int ord = 0;
                    if (!reader.IsDBNull(2))
                    {
                        ord = reader.GetByte(2);
                    }

                    cSummaryAttributeElement element = new cSummaryAttributeElement(sid, attributeid, otmid, ord);
                    retSA.Add(sid, element);
                }

                reader.Close();
            }

            return retSA;
        }

        private Dictionary<int, cSummaryAttributeColumn> getSummaryAttributeColumns(int attributeid, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            Dictionary<int, cSummaryAttributeColumn> retSA = new Dictionary<int, cSummaryAttributeColumn>();
            JoinVias lstJoinVias = new JoinVias(cMisc.GetCurrentUser());

            ////                    0               1               2            3      4            5           6                            7                              8             9            10
            const string sql = "select columnid, columnAttributeId, alternate_header, width, [order], default_sort, filterVal, customEntityAttributes.relationshiptype, displayFieldId, joinViaID, columnAttributeId from customEntityAttributeSummaryColumns inner join customEntityAttributes on customEntityAttributes.attributeid = customEntityAttributeSummaryColumns.columnAttributeId where customEntityAttributeSummaryColumns.attributeid = @attId order by [order] ";
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@attId", attributeid);

            using (IDataReader reader = expdata.GetReader(sql))
            {
                while (reader.Read())
                {
                    int colid = reader.GetInt32(0);
                    int columnAttributeID = reader.GetInt32(1);
                    string header = reader.GetString(2);
                    int width = 0;
                    if (!reader.IsDBNull(3))
                    {
                        width = reader.GetInt32(3);
                    }

                    int ord = 0;
                    if (!reader.IsDBNull(4))
                    {
                        ord = reader.GetByte(4);
                    }

                    bool defsort = reader.GetBoolean(5);
                    string filterVal = string.Empty;
                    if (!reader.IsDBNull(6))
                    {
                        filterVal = reader.GetString(6);
                    }

                    bool isMTO = false;
                    if (!reader.IsDBNull(7))
                    {
                        if (reader.GetByte(7) == 1)
                            isMTO = true;
                    }

                    Guid displayFieldId = Guid.Empty;
                    if (!reader.IsDBNull(8))
                    {
                        displayFieldId = reader.GetGuid(8);
                    }

                    JoinVia joinViaObj = null;
                    if (!reader.IsDBNull(9))
                    {
                        int joinViaID = reader.GetInt32(9);
                        joinViaObj = lstJoinVias.GetJoinViaByID(joinViaID);
                    }

                    cSummaryAttributeColumn col = new cSummaryAttributeColumn(colid, columnAttributeID, header, width, ord, defsort, filterVal, isMTO, displayFieldId, joinViaObj);

                    retSA.Add(columnAttributeID, col);
                }

                reader.Close();
            }

            return retSA;
        }

        /// <summary>
        /// This method get the Autocomplete Display selection for an attribute
        /// </summary>
        /// <param name="attribute">The <see cref="cAttribute"/>ID of the <see cref="cAttribute"/>to get the autolookup display fields</param>
        /// <param name="fields">The <see cref="cFields"/>ID of the <seealso cref="cAttribute"/>to retrieve</param> 
        /// <returns>
        /// The <see cref="ListItem"/>List of fields with field id and description
        /// </returns>
        public List<ListItem> GetAutocompleteResultDisplaySelection(cAttribute attribute, cFields fields)
        {
            return attribute.GetType() != typeof(cManyToOneRelationship) ? null : (from fieldid in ((cManyToOneRelationship)attribute).AutoCompleteDisplayFieldIDList let field = fields.GetFieldByID(fieldid) where field != null select new ListItem(field.Description, fieldid.ToString())).ToList();
        }

        /// <summary>
        /// The custom entity and its associated 'one to many link' is created with a system view containing the sub entities details,
        /// a jointable link from the parent entity to the sub entity and the associated join breakdown as well as a view group.
        /// If there are other parent entities above the parent in this method a recursive routine creates the joins from the
        /// sub level entity to the parent entities of the parent in this method, this is so there is a full link 
        /// for reporting between all the entities and their parents.
        /// </summary>
        /// <param name="employeeid"></param>
        /// <param name="presaveAttributeId"></param>
        /// <param name="relationshipAttributeId"></param>
        /// <param name="entity"></param>
        /// <param name="relatedtable"></param>
        /// <param name="relatedentity"></param>
        public void createDerivedTable(int employeeid, int presaveAttributeId, int relationshipAttributeId, cCustomEntity entity, cTable relatedtable, cCustomEntity relatedentity)
        {
            string viewname = entity.pluralname + "_" + relatedentity.pluralname;
            cCustomEntity sysViewEntity = null;

            if (presaveAttributeId == 0)
            {
                cCustomEntity viewentity = new cCustomEntity(entity.entityid, entity.entityname + "_" + relatedentity.entityname, viewname,
                                                             string.Empty, DateTime.Now, employeeid, null, null,
                                                             new SortedList<int, cAttribute>(),
                                                             new SortedList<int, cCustomEntityForm>(),
                                                             new SortedList<int, cCustomEntityView>(), null, null,
                                                             entity.EnableAttachments, entity.AudienceView,
                                                             entity.AllowMergeConfigAccess, true, relatedentity.entityid,
                                                             entity.entityid, entity.EnableCurrencies,
                                                             entity.DefaultCurrencyID, entity.EnablePopupWindow, entity.DefaultPopupView, entity.FormSelectionAttributeId,
                                                             entity.OwnerId, entity.SupportContactId, entity.SupportQuestion, entity.EnableLocking, entity.BuiltIn, entity.SystemCustomEntityId);

                int entityViewId = this.saveEntitySystemView(viewentity, entity.entityid, relationshipAttributeId,
                                                             relatedentity);
                if (entityViewId != 0)
                {
                    sysViewEntity = lst[entityViewId];
                }
            }
            else
            {
                sysViewEntity = (from x in lst.Values
                                 where
                                     x.IsSystemView && x.SystemView_EntityId.Value == entity.entityid &&
                                     x.SystemView_DerivedEntityId.Value == relatedentity.entityid
                                 select x).FirstOrDefault();
            }

            if (sysViewEntity != null)
            {
                // need to create jointable entry and join breakdown for each entity and it's parent entities to the new system view
                List<cCustomEntity> lstEntity = this.createParentEntityReportConfig(entity, null, 1, null);

                if (lstEntity.Count > 1)
                {
                    Dictionary<int, List<cJoinStep>> entityJoins = new Dictionary<int, List<cJoinStep>>();

                    int topEntityIdx = 0;
                    cJoins joins = new cJoins(this.oCurrentUser.AccountID);

                    while (topEntityIdx < lstEntity.Count)
                    {
                        cCustomEntity topEntity = lstEntity[topEntityIdx];

                        if (topEntity.table != null && sysViewEntity.table != null)
                        {
                            if (joins.GetJoin(topEntity.table.TableID, sysViewEntity.table.TableID) == null)
                            {
                                // join doesn't exist, so need to create it
                                joins.ConstructJoin(lstEntity.Skip(topEntityIdx).ToList(), sysViewEntity, relationshipAttributeId);
                            }
                        }

                        topEntityIdx++;
                    }
                }
            }
        }

        /// <summary>
        /// createAttributeGrid: displays the data grid for the provided custom entity
        /// </summary>
        /// <param name="entity">Entity class structure of the entity to display the data for</param>
        /// <returns>Array containing cNewGrid elements</returns>
        public string[] createAttributeGrid(cCustomEntity entity)
        {
            cGridNew clsgrid = new cGridNew(this.oCurrentUser.AccountID, oCurrentUser.EmployeeID, "gridAttributes", "select entityid, attributeid, display_name, description, fieldtype, is_audit_identity, system_attribute from customEntityAttributes");

            if (entity != null)
            {
                clsgrid.addFilter(fields.GetFieldByID(new Guid("091d83f7-c4c1-4fea-9779-784028d489d0")), ConditionType.Equals, new object[] { entity.entityid }, null, ConditionJoiner.None);
                clsgrid.addFilter(fields.GetFieldByID(new Guid("654706A1-05E9-475A-9A0C-0197B162BF5C")), ConditionType.DoesNotEqual, new object[] { (int)FieldType.LookupDisplayField }, null, ConditionJoiner.And);
            }
            else
            {
                clsgrid.addFilter(fields.GetFieldByID(new Guid("091d83f7-c4c1-4fea-9779-784028d489d0")), ConditionType.Equals, new object[] { -1 }, null, ConditionJoiner.None);
            }

            clsgrid.enabledeleting = true;
            clsgrid.enableupdating = true;
            clsgrid.editlink = "javascript:editAttribute({attributeid},{fieldtype},false);";
            clsgrid.deletelink = "javascript:deleteAttribute({attributeid},{fieldtype});";

            cColumnList fieldtypes = new cColumnList();
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Currency, "Currency");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.DateTime, "Date/Time");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Hyperlink, "Hyperlink");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Integer, "Integer");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.LargeText, "Large Text");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.List, "List");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Number, "Number");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Relationship, "Relationship");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Text, "Text");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.TickBox, "Tickbox");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.OTMSummary, "Summary");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Comment, "Comment");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.CurrencyList, "Currency List");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Attachment, "Attachment");
            ((cFieldColumn)clsgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Contact, "Contact");
            clsgrid.KeyField = "attributeid";
            clsgrid.CssClass = "datatbl";
            clsgrid.getColumnByName("entityid").hidden = true;
            clsgrid.getColumnByName("attributeid").hidden = true;
            clsgrid.getColumnByName("system_attribute").hidden = true;
            clsgrid.EmptyText = "There are currently no attributes defined for this GreenLight.";
            clsgrid.InitialiseRow += new cGridNew.InitialiseRowEvent(clsAttributeGridgrid_InitialiseRow);
            clsgrid.ServiceClassForInitialiseRowEvent = "Spend_Management.cCustomEntities";
            clsgrid.ServiceClassMethodForInitialiseRowEvent = "clsAttributeGridgrid_InitialiseRow";

            cEmployees clsEmployees = new cEmployees(this.oCurrentUser.AccountID);

            if (oCurrentUser.Employee.GetNewGridSortOrders().GetBy("gridAttributes") == null)
            {
                clsgrid.SortedColumn = clsgrid.getColumnByName("display_name");
                clsgrid.SortDirection = SortDirection.Ascending;
            }

            return clsgrid.generateGrid();
        }

        private void clsAttributeGridgrid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
        {
            if ((bool)row.getCellByID("system_attribute").Value == true)
            {
                row.enablearchiving = false;
                row.enabledeleting = false;
                row.enableupdating = false;
            }
        }

        /// <summary>
        /// The forms grid for an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string[] createFormGrid(cCustomEntity entity)
        {
            cGridNew clsgrid = new cGridNew(this.oCurrentUser.AccountID, oCurrentUser.EmployeeID, "gridForms", "select formid, form_name, description from customEntityForms");
            if (entity != null)
            {
                clsgrid.addFilter(fields.GetFieldByID(new Guid("b4662049-d92a-4b8b-8dba-69acd883cebb")), ConditionType.Equals, new object[] { entity.entityid }, null, ConditionJoiner.None);
            }
            else
            {
                clsgrid.addFilter(fields.GetFieldByID(new Guid("b4662049-d92a-4b8b-8dba-69acd883cebb")), ConditionType.Equals, new object[] { -1 }, null, ConditionJoiner.None);
            }

            clsgrid.getColumnByName("formid").hidden = true;
            clsgrid.enabledeleting = true;
            clsgrid.enableupdating = true;
            clsgrid.editlink = "javascript:SEL.CustomEntityAdministration.Forms.EditForm({formid});";
            clsgrid.deletelink = "javascript:SEL.CustomEntityAdministration.Forms.DeleteForm({formid});";
            clsgrid.CssClass = "datatbl";
            clsgrid.KeyField = "formid";
            clsgrid.EmptyText = "There are currently no forms defined for this GreenLight.";

            cEmployees clsEmployees = new cEmployees(this.oCurrentUser.AccountID);

            if (oCurrentUser.Employee.GetNewGridSortOrders().GetBy("gridForms") == null)
            {
                clsgrid.SortedColumn = clsgrid.getColumnByName("form_name");
                clsgrid.SortDirection = SortDirection.Ascending;
            }

            clsgrid.addEventColumn("CreateCopy", GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/copy.png", "javascript:SEL.CustomEntityAdministration.Forms.ShowCopyModal({formid});", "Create a copy of this Form", "Create a copy of this Form");

            return clsgrid.generateGrid();
        }
        /// <summary>
        /// Creates the data for the new pop-up window views drop down list
        /// </summary>
        /// <param name="entityid">id of custom entity</param>
        /// <param name="formid">id of the pop-up view, is null then default entity view is selected</param>
        /// <param name="recordid">id of individual record</param>
        /// <returns>Array of listitems to be bound to the pop-up view drop down list</returns>
        public ListItem[] createViewDropDown(int entityID, int? defaultPopupView, int accountID)
        {
            string strsql;
            int count = 0;
            int i = 0;

            var expdata = new DBConnection(cAccounts.getConnectionString(accountID));

            strsql = "select count(viewid) from customEntityViews where entityid=" + entityID;
            count = expdata.getcount(strsql);

            if (count > 0)
            {
                var items = new ListItem[count];
                strsql = "select viewid, view_name from customEntityViews where entityid=" + entityID;
                items[0] = new ListItem();

                using (IDataReader viewDataReader = expdata.GetReader(strsql))
                {
                    while (viewDataReader.Read())
                    {
                        items[i] = new ListItem();
                        items[i].Text = viewDataReader.GetString(viewDataReader.GetOrdinal("view_name"));
                        items[i].Value = viewDataReader.GetInt32(viewDataReader.GetOrdinal("viewID")).ToString();
                        if (viewDataReader.GetInt32(viewDataReader.GetOrdinal("viewID")) == defaultPopupView)
                        {
                            items[i].Selected = true;
                        }
                        i++;
                    }
                    viewDataReader.Close();
                    expdata.sqlexecute.Parameters.Clear();
                    return items;
                }
            }
            else
            //entity has no views associated
            {
                var items = new ListItem[1];
                items[0] = new ListItem();
                items[0].Text = "[None]";
                items[0].Value = "-1";
                return items;
            }
        }


        /// <summary>
        /// The views grid for an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string[] createViewGrid(cCustomEntity entity)
        {
            cGridNew clsgrid = new cGridNew(this.oCurrentUser.AccountID, oCurrentUser.EmployeeID, "gridViews", "select viewid, view_name, description from customEntityViews");
            if (entity != null)
            {
                clsgrid.addFilter(fields.GetFieldByID(new Guid("135b0d53-0123-4154-b467-a499df1c7b84")), ConditionType.Equals, new object[] { entity.entityid }, null, ConditionJoiner.None);
            }
            else
            {
                clsgrid.addFilter(fields.GetFieldByID(new Guid("135b0d53-0123-4154-b467-a499df1c7b84")), ConditionType.Equals, new object[] { -1 }, null, ConditionJoiner.None);
            }

            clsgrid.enabledeleting = true;
            clsgrid.enableupdating = true;
            clsgrid.editlink = "javascript:SEL.CustomEntityAdministration.Views.Edit({viewid});";
            if (entity != null)
            {
                clsgrid.deletelink = "javascript:SEL.CustomEntityAdministration.Views.Delete({viewid}," + this.oCurrentUser.AccountID + "," + entity.entityid + ");";
            }
            clsgrid.getColumnByName("viewid").hidden = true;
            clsgrid.KeyField = "viewid";
            clsgrid.CssClass = "datatbl";
            clsgrid.EmptyText = "There are currently no views defined for this GreenLight.";

            cEmployees clsEmployees = new cEmployees(this.oCurrentUser.AccountID);

            if (oCurrentUser.Employee.GetNewGridSortOrders().GetBy("gridViews") == null)
            {
                clsgrid.SortedColumn = clsgrid.getColumnByName("view_name");
                clsgrid.SortDirection = SortDirection.Ascending;
            }

            return clsgrid.generateGrid();

        }

        /// <summary>
        /// Method to save an attribute
        /// </summary>
        /// <param name="employeeid">EmployeeID</param>
        /// <param name="entityid">EntityID</param>
        /// <param name="attributeid">AttributeID</param>
        /// <param name="displayname">Dispplay Name</param>
        /// <param name="description">Description</param>
        /// <param name="tooltip">Tooltip Text</param>
        /// <param name="mandatory">Mandatory Attribute</param>
        /// <param name="fieldtype">Type Of Attribute</param>
        /// <param name="maxlength">Max Length of Text</param>
        /// <param name="format">Format of Attribute (Text or Date Format)</param>
        /// <param name="defaultvalue">Default Value</param>
        /// <param name="precision">Precision for Decimal</param>
        /// <param name="lstitems">Items if List Type</param>
        /// <param name="workflowid">Workflow ID</param>
        /// <param name="advicePanelText">Advice Text</param>
        /// <param name="auditidentifier">Attribute to be used as Audit Identifier</param>
        /// <param name="isunique">Is Unique in Value</param>
        /// <param name="populateExistingRecordsDefault">Populate any existing GreenLight records with the default value chosen for this new attribute</param>
        /// <param name="boolAttribute">Specifies whether a user will have access to the image library when uploading an attachment or whether the fonts will be stripped from a formatted text box.</param>
        /// <param name="displayInMobile">Specifies whether this attribute will show to users of the mobile application.</param>
        /// <param name="builtIn">Specifies whether this attribute will be a system attribute</param>
        /// <returns>Object array containing attribute id, 0 or 1 if Audit Identifier and Display Name</returns>
        /// <exception cref="Exception"></exception>
        public static string[] SaveGeneralAttribute(int employeeid, int entityid, int attributeid, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, int? maxlength, AttributeFormat format, string defaultvalue, byte precision, string[] lstitems, int workflowid, string advicePanelText, bool auditidentifier, bool isunique, bool populateExistingRecordsDefault, bool boolAttribute, bool displayInMobile, bool builtIn)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            string attributename = displayname;
            DateTime createdon = DateTime.Now;
            int createdby = employeeid;
            DateTime? modifiedon = null;
            int? modifiedby = null;
            Guid fieldid = Guid.Empty;
            bool currentBuiltIn = false;
            cCustomEntities clsentities = new cCustomEntities(user);
            cCustomEntity entity = clsentities.getEntityById(entityid);
            cAttribute attribute = null;

            string[] arrReturn = new string[3];

            if (attributeid > 0)
            {
                attribute = entity.getAttributeById(attributeid);
                attributename = attribute.attributename;
                modifiedon = DateTime.Now;
                modifiedby = employeeid;
                createdby = attribute.createdby;
                createdon = attribute.createdon;
                fieldid = attribute.fieldid;
                currentBuiltIn = attribute.BuiltIn;
            }

            // only allow the attribute to be set as built-in/system if the user is "adminonly"
            if (!user.Employee.AdminOverride && !currentBuiltIn && builtIn)
            {
                builtIn = false;
            }

            switch (fieldtype)
            {
                case FieldType.Text:
                case FieldType.LargeText:
                    attribute = new cTextAttribute(attributeid, attributename, displayname, description, tooltip, mandatory,
                                                   fieldtype, createdon, createdby, modifiedon, modifiedby, maxlength, format,
                                                   fieldid, auditidentifier, isunique, false, false, boolAttribute, displayInMobile, builtIn);
                    break;
                case FieldType.DateTime:
                    attribute = new cDateTimeAttribute(attributeid, attributename, displayname, description, tooltip, mandatory,
                                                       fieldtype, createdon, createdby, modifiedon, modifiedby, format, fieldid,
                                                       auditidentifier, isunique, false, false, displayInMobile, builtIn);
                    break;
                case FieldType.TickBox:
                    attribute = new cTickboxAttribute(attributeid, attributename, displayname, description, tooltip, mandatory,
                                                      fieldtype, createdon, createdby, modifiedon, modifiedby, defaultvalue,
                                                      fieldid, auditidentifier, isunique, false, false, displayInMobile, builtIn);
                    break;
                case FieldType.Integer:
                    attribute = new cNumberAttribute(attributeid, attributename, displayname, description, tooltip, mandatory,
                                                     FieldType.Integer, createdon, createdby, modifiedon, modifiedby, 0, fieldid,
                                                     false, auditidentifier, isunique, false, false, displayInMobile, builtIn);
                    break;
                case FieldType.Currency:
                    attribute = new cNumberAttribute(attributeid, attributename, displayname, description, tooltip, mandatory,
                                                     FieldType.Currency, createdon, createdby, modifiedon, modifiedby, 2,
                                                     fieldid, false, auditidentifier, isunique, false, false, displayInMobile, builtIn);
                    break;
                case FieldType.Number:
                    attribute = new cNumberAttribute(attributeid, attributename, displayname, description, tooltip, mandatory,
                                                     FieldType.Number, createdon, createdby, modifiedon, modifiedby, precision,
                                                     fieldid, false, auditidentifier, isunique, false, false, displayInMobile, builtIn);
                    break;
                case FieldType.List:
                    var list = new SortedList<int, cListAttributeElement>();
                    var jss = new JavaScriptSerializer();

                    for (int i = 0; i < lstitems.GetLength(0); i++)
                    {
                        var arrVal = (CustomEntityListItem)jss.Deserialize(lstitems[i], typeof(CustomEntityListItem));
                        if (arrVal != null)
                        {
                            arrVal.elementOrder = i;
                            list.Add(i, arrVal.ToListAttributeElement());
                        }
                    }

                    attribute = new cListAttribute(
                        attributeid,
                        attributename,
                        displayname,
                        description,
                        tooltip,
                        mandatory,
                        fieldtype,
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        list,
                        fieldid,
                        auditidentifier,
                        isunique,
                        format,
                        false,
                        false,
                        displayInMobile,
                        builtIn);
                    break;
                case FieldType.RunWorkflow:
                    cWorkflows clsworkflows = new cWorkflows(user);
                    cWorkflow workflow = clsworkflows.GetWorkflowByID(workflowid);
                    attribute = new cRunWorkflowAttribute(attributeid, attributename, displayname, description, tooltip,
                                                          mandatory, createdon, createdby, modifiedon, modifiedby, fieldid,
                                                          workflow, false, false, displayInMobile, builtIn);
                    break;
                case FieldType.Comment:
                    attribute = new cCommentAttribute(attributeid, attributename, displayname, description, tooltip, mandatory,
                                                      createdon, createdby, modifiedon, modifiedby, advicePanelText, fieldid,
                                                      auditidentifier, isunique, false, false, displayInMobile, builtIn);
                    break;
                case FieldType.Attachment:
                    attribute = new cAttachmentAttribute(attributeid, attributename, displayname, description, tooltip, mandatory,
                                                     FieldType.Attachment, createdon, createdby, modifiedon, modifiedby, fieldid, format,
                                                     auditidentifier, isunique, boolAttribute, false, false, displayInMobile, builtIn);


                    break;
                case FieldType.Contact:
                    attribute = new cContactAttribute(attributeid, attributename, displayname, description, tooltip,
                                                    mandatory, createdon, createdby, modifiedon, modifiedby, fieldid, format, auditidentifier,
                                                    isunique, false, false, displayInMobile, builtIn);
                    break;
                default:
                    throw new Exception("FieldType." + fieldtype +
                                        ", is not handled saveGeneralAttribute");
            }

            attributeid = clsentities.saveAttribute(entityid, attribute, populateExistingRecordsDefault);

            if (fieldtype == FieldType.Attachment && attributeid != 0)
            {
                clsentities.InitialiseData();
                entity = clsentities.getEntityById(entityid);
                attribute = entity.getAttributeById(attributeid);
                Guid tableID = entity.table.TableID;
                // create join using tableID / attribute ID
                clsentities.saveAttachmentLevelRelationship(tableID, attribute.fieldid, entity.entityname);
            }

            arrReturn[0] = attributeid.ToString();
            arrReturn[1] = "0";
            if (auditidentifier)
            {
                arrReturn[1] = "1";
                arrReturn[2] = displayname;
            }

            // if the attribute is set as built-in/system but the GreenLight isn't, make the GreenLight built-in/system too
            if (attributeid > 0 && builtIn && !entity.BuiltIn)
            {
                entity.BuiltIn = true;
                clsentities.saveEntity(entity);
            }

            return arrReturn;
        }

        /// <summary>
        /// Creates the join between the attachment type attribute and the EntityImageData table which stores the file info
        /// </summary>
        /// <param name="tableID"></param>
        /// <param name="attributeID"></param>
        /// <param name="entityName"></param>
        /// <param name="connection"></param>
        public void saveAttachmentLevelRelationship(Guid tableID, Guid attFieldID, string entityName, IDBConnection connection = null)
        {

            //using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            //{
            //    expdata.sqlexecute.Parameters.Clear();
            //    expdata.sqlexecute.Parameters.AddWithValue("@baseTable", tableID);
            //    expdata.sqlexecute.Parameters.AddWithValue("@destinationKey", attFieldID);
            //    expdata.sqlexecute.Parameters.AddWithValue("@entityName", entityName);
            //    expdata.ExecuteProc("[saveCustomFieldLevelAttachmentConfig]");
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityid"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public int saveAttribute(int entityid, cAttribute attribute, bool populateExistingRecordsWithDefault = false, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            bool isNew = false;
            expdata.sqlexecute.Parameters.Clear();

            expdata.sqlexecute.Parameters.AddWithValue("@entityid", entityid);
            expdata.sqlexecute.Parameters.AddWithValue("@attributeid", attribute.attributeid);
            expdata.sqlexecute.Parameters.AddWithValue("@displayname", WebUtility.HtmlDecode(attribute.displayname));
            expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@precision", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@workflowid", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@populateExistingWithDefault", false);

            if (attribute.attributeid == 0)
            {
                isNew = true;
            }

            expdata.sqlexecute.Parameters.AddWithValue("@boolAttribute", attribute.BoolAttribute);

            if (string.IsNullOrEmpty(attribute.description))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@description", attribute.description);
            }

            if (string.IsNullOrEmpty(attribute.tooltip))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@tooltip", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@tooltip", attribute.tooltip);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@fieldtype", (byte)attribute.fieldtype);
            expdata.sqlexecute.Parameters.AddWithValue("@mandatory", Convert.ToByte(attribute.mandatory));
            expdata.sqlexecute.Parameters.AddWithValue("@displayInMobile", Convert.ToByte(attribute.DisplayInMobile));

            if (attribute.modifiedby == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@userid", attribute.createdby);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@userid", attribute.modifiedby);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@maxlength", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@commentText", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@relationshipType", (int)RelationshipType.None);
            expdata.sqlexecute.Parameters.AddWithValue("@builtIn", Convert.ToByte(attribute.BuiltIn));

            if (attribute.GetType() == typeof(cTextAttribute))
            {
                cTextAttribute textattribute = (cTextAttribute)attribute;
                if (textattribute.maxlength != null)
                {
                    expdata.sqlexecute.Parameters["@maxlength"].Value = textattribute.maxlength;
                }

                expdata.sqlexecute.Parameters.AddWithValue("@format", (byte)textattribute.format);
            }
            else if (attribute.GetType() == typeof(cListAttribute))
            {
                cListAttribute listattribute = (cListAttribute)attribute;
                expdata.sqlexecute.Parameters.AddWithValue("@format", (byte)listattribute.format);
            }
            else if (attribute.GetType() == typeof(cDateTimeAttribute))
            {
                cDateTimeAttribute dateattribute = (cDateTimeAttribute)attribute;
                expdata.sqlexecute.Parameters.AddWithValue("@format", (byte)dateattribute.format);
            }
            else if (attribute.GetType() == typeof(cAttachmentAttribute))
            {
                cAttachmentAttribute attachmentattribute = (cAttachmentAttribute)attribute;
                expdata.sqlexecute.Parameters.AddWithValue("@format", (byte)attachmentattribute.format);
                expdata.sqlexecute.Parameters.AddWithValue("@relatedtable", DBNull.Value);
            }
            else if (attribute.GetType() == typeof(cTickboxAttribute))
            {
                cTickboxAttribute tickboxattribute = (cTickboxAttribute)attribute;
                expdata.sqlexecute.Parameters["@defaultvalue"].Value = tickboxattribute.defaultvalue;
                expdata.sqlexecute.Parameters["@populateExistingWithDefault"].Value = populateExistingRecordsWithDefault;
            }
            else if (attribute.GetType() == typeof(cNumberAttribute))
            {
                cNumberAttribute numberattribute = (cNumberAttribute)attribute;
                if (numberattribute.fieldtype == FieldType.Number)
                {
                    expdata.sqlexecute.Parameters["@precision"].Value = numberattribute.precision;
                }
            }
            else if (attribute.GetType() == typeof(cRunWorkflowAttribute))
            {
                cRunWorkflowAttribute workflowattribute = (cRunWorkflowAttribute)attribute;
                expdata.sqlexecute.Parameters["@workflowid"].Value = workflowattribute.workflow.workflowid;
            }
            else if (attribute.GetType() == typeof(cCommentAttribute))
            {
                cCommentAttribute adviceAttribute = (cCommentAttribute)attribute;
                expdata.sqlexecute.Parameters["@commentText"].Value = adviceAttribute.commentText;
            }
            else if (attribute.GetType() == typeof(cOneToManyRelationship))
            {
                expdata.sqlexecute.Parameters["@relationshipType"].Value = (int)RelationshipType.OneToMany;
            }
            else if (attribute.GetType() == typeof(cManyToOneRelationship))
            {
                expdata.sqlexecute.Parameters["@relationshipType"].Value = (int)RelationshipType.ManyToOne;
            }
            else if (attribute.GetType() == typeof (cContactAttribute))
            {
                var contactAttribute = (cContactAttribute) attribute;
                expdata.sqlexecute.Parameters.AddWithValue("@format", (byte)contactAttribute.format);
            }

            if (!expdata.sqlexecute.Parameters.Contains("@format"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@format", DBNull.Value);
            }

            if (!expdata.sqlexecute.Parameters.Contains("@relatedtable"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@relatedtable", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@isauditidentity", attribute.isauditidentifer);
            expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
            expdata.sqlexecute.Parameters.AddWithValue("@related_entityid", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@isunique", attribute.isunique);
            expdata.sqlexecute.Parameters.AddWithValue("@triggerAttributeId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@triggerJoinViaId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@triggerDisplayFieldId", DBNull.Value);

            if (attribute.GetType() == typeof(LookupDisplayField))
            {
                LookupDisplayField ldf = (LookupDisplayField)attribute;
                if (ldf.TriggerAttributeId.HasValue)
                {
                    expdata.sqlexecute.Parameters["@triggerAttributeId"].Value = ldf.TriggerAttributeId.Value;
                }

                if (ldf.TriggerJoinViaId.HasValue)
                {
                    expdata.sqlexecute.Parameters["@triggerJoinViaId"].Value = ldf.TriggerJoinViaId.Value;
                }

                if (ldf.TriggerDisplayFieldId.HasValue)
                {
                    expdata.sqlexecute.Parameters["@triggerDisplayFieldId"].Value = ldf.TriggerDisplayFieldId.Value;
                }
            }

            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveCustomEntityAttribute");

            int attributeid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            if (attribute.fieldtype == FieldType.List)
            {
                if (attributeid > 0)
                {
                    this.saveListItems(entityid, (cListAttribute)attribute, attributeid, isNew);
                }
            }
            // fieldid default constraint newid()
            ////clearCacheForGetEntityIdByAttributeId();
            var lastDatabaseUpdate = expdata.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
            this.refreshCache(lastDatabaseUpdate);
            return attributeid;
        }


        /// <summary>
        /// Save list items for a custom attribute (list) field
        /// </summary>
        /// <param name="entityid">
        /// ID of parent entity that list element is attributed to
        /// </param>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <param name="attributeID">
        /// List attribute record
        /// </param>
        /// <param name="isNew">
        /// Indicates whether to check for possible deleted list items
        /// </param>
        private void saveListItems(int entityid, cListAttribute attribute, int attributeID, bool isNew, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            SortedList<int, cListAttributeElement> lstItems = attribute.items;
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@attributeid", attributeID);

            // If new no need to compare against the existing items
            if (!isNew)
            {
                // Get the currently cached attribute and the newly created attribute so that we can see which list items have been removed
                cListAttribute curListAttribute = (cListAttribute)getEntityById(entityid).attributes[attributeID];

                if (curListAttribute != null)
                {
                    List<int> deleteValueIDs = new List<int>();

                    deleteValueIDs.AddRange(from i in curListAttribute.items
                                            select i.Value
                                                into item
                                            where attribute.items.Select(newItems => newItems.Value).Any(
                                                newItem => newItem.elementValue == item.elementValue) == false
                                            select item.elementValue);

                    expdata.sqlexecute.Parameters.Clear();

                    foreach (int deleteID in deleteValueIDs)
                    {
                        expdata.sqlexecute.Parameters.Clear();

                        expdata.sqlexecute.Parameters.Add("@valueid", SqlDbType.Int);
                        expdata.sqlexecute.Parameters["@valueid"].Value = deleteID;
                        expdata.sqlexecute.Parameters.Add("@CUemployeeID", SqlDbType.Int);
                        expdata.sqlexecute.Parameters["@CUemployeeID"].Value = oCurrentUser.EmployeeID;
                        expdata.sqlexecute.Parameters.Add("@CUdelegateID", SqlDbType.Int);
                        if (this.oCurrentUser.Delegate != null)
                        {
                            expdata.sqlexecute.Parameters["@CUdelegateID"].Value = oCurrentUser.Delegate.EmployeeID;
                        }
                        else
                        {
                            expdata.sqlexecute.Parameters["@CUdelegateID"].Value = DBNull.Value;
                        }

                        expdata.ExecuteProc("deleteCustomEntityListAttributeItem");
                    }
                }
            }

            foreach (KeyValuePair<int, cListAttributeElement> i in lstItems)
            {
                expdata.sqlexecute.Parameters.Clear();
                var listelement = (cListAttributeElement)i.Value;

                expdata.sqlexecute.Parameters.AddWithValue("@valueid", listelement.elementValue);
                expdata.sqlexecute.Parameters.AddWithValue("@attributeid", attributeID);
                expdata.sqlexecute.Parameters.AddWithValue("@item", listelement.elementText);
                expdata.sqlexecute.Parameters.AddWithValue("@order", listelement.elementOrder);
                expdata.sqlexecute.Parameters.AddWithValue("@archived", listelement.Archived);
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", this.oCurrentUser.EmployeeID);
                if (this.oCurrentUser.Delegate != null)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", this.oCurrentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                expdata.ExecuteProc("addCustomEntityListAttributeItem");
            }
        }

        /// <summary>
        ///  The save many to one relationship.
        /// </summary>
        /// <param name="accountid">
        ///  The account id.
        /// </param>
        /// <param name="employeeid">
        ///  The employee id.
        /// </param>
        /// <param name="entityid">
        ///  The entity id.
        /// </param>
        /// <param name="attributeid">
        ///  The attribute id.
        /// </param>
        /// <param name="displayname">
        ///  The display name.
        /// </param>
        /// <param name="description">
        ///  The description.
        /// </param>
        /// <param name="tooltip">
        ///  The tooltip.
        /// </param>
        /// <param name="mandatory">
        ///  The mandatory.
        /// </param>
        /// <param name="builtIn">
        ///  The built in.
        /// </param>
        /// <param name="tableid">
        ///  The table id.
        /// </param>
        /// <param name="displayFieldId">
        ///  The display field id.
        /// </param>
        /// <param name="matchFieldIds">
        ///  The match field ids.
        /// </param>
        /// <param name="autocompleteFieldIds">
        ///  The list of fields that should appear in autocomplete search results.
        /// </param>
        /// <param name="maxRows">
        ///  The max rows.
        /// </param>
        /// <param name="oJsonFilters">
        ///  The o json filters.
        /// </param>
        /// <param name="oJsonTriggerFields">
        ///  The o json trigger fields.
        /// </param>
        /// <param name="isParentFilter">
        /// Is this parent and child relationship
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int saveManyToOneRelationship(int accountid, int employeeid, int entityid, int attributeid, string displayname, string description, string tooltip, bool mandatory, bool builtIn, Guid tableid, Guid displayFieldId, string[] matchFieldIds, string[] autocompleteFieldIds, int maxRows, JavascriptTreeData oJsonFilters, JavascriptTreeData oJsonTriggerFields, int formid, bool isParentFilter)
        {
            // forced merge
            string attributename = displayname;
            DateTime createdon = DateTime.Now;
            int createdby = employeeid;
            DateTime? modifiedon = null;
            int? modifiedby = null;
            bool auditidentifier = false;
            bool currentBuiltIn = false;
            cTable relatedtable = tables.GetTableByID(tableid);
            cTable aliasTable = null;
            Guid tmpGuid = Guid.Empty;
            Guid fieldid = Guid.Empty;
            cAttribute attribute = null;
            List<Guid> matchFields = (from x in matchFieldIds
                                      where Guid.TryParse(x, out tmpGuid)
                                      select new Guid(x)).ToList();

            cFields clsFields = new cFields(accountid);
            List<Guid> autocompleteFields = new List<Guid>();
            autocompleteFieldIds = autocompleteFieldIds.Where(fieldName => !string.IsNullOrEmpty(fieldName)).ToArray();

            foreach (string autocompleteDisplay in autocompleteFieldIds)
            {
                cField autolookupDisplayField = clsFields.GetFieldByTableAndDescription(tableid, autocompleteDisplay);
                if (autolookupDisplayField != null)
                {
                    autocompleteFields.Add(autolookupDisplayField.FieldID);
                }
            }

            cCustomEntity entity = this.getEntityById(entityid);

            if (attributeid > 0)
            {
                attribute = entity.getAttributeById(attributeid);
                attributename = attribute.attributename;
                modifiedon = DateTime.Now;
                modifiedby = employeeid;
                createdby = attribute.createdby;
                createdon = attribute.createdon;
                fieldid = attribute.fieldid;
                auditidentifier = attribute.isauditidentifer;
                currentBuiltIn = attribute.BuiltIn;
            }
            else
            {
                // Create aliased table for the relationship
                cCustomTables clsCustTables = new cCustomTables(accountid);
                aliasTable = clsCustTables.SaveCustomTable(entityid, relatedtable);
            }

            #region parse filters

            cField field;
            byte order = (byte)0;
            SortedList<int, JoinViaPart> joinViaList = new SortedList<int, JoinViaPart>();
            int joinViaID;
            string joinViaDescription;
            string joinViaIDs;
            Guid joinViaPartID;
            SortedList<int, FieldFilter> lstFilters = new SortedList<int, FieldFilter>();
            ConditionType tmpCT;
            int tmpCTint = 0;
            string tmpVal1;
            string tmpVal2;

            foreach (JavascriptTreeData.JavascriptTreeNode jsNode in oJsonFilters.data)
            {
                if (Guid.TryParseExact(jsNode.attr.fieldid, "D", out joinViaPartID))
                {
                    field = fields.GetFieldByID(joinViaPartID);

                    if (field != null)
                    {
                        joinViaID = jsNode.attr.joinviaid;
                        joinViaDescription = jsNode.attr.crumbs;
                        joinViaIDs = jsNode.attr.id;
                        joinViaList = new SortedList<int, JoinViaPart>();

                        if (jsNode.metadata.ContainsKey("conditionType"))
                        {
                            int.TryParse(jsNode.metadata["conditionType"].ToString(), out tmpCTint);
                        }

                        tmpCT = (jsNode.metadata.ContainsKey("conditionType"))
                                    ? (ConditionType)tmpCTint
                                    : ConditionType.Equals;
                        tmpVal1 = (jsNode.metadata.ContainsKey("criterionOne"))
                                      ? jsNode.metadata["criterionOne"].ToString()
                                      : string.Empty;
                        tmpVal2 = (jsNode.metadata.ContainsKey("criterionTwo"))
                                      ? jsNode.metadata["criterionTwo"].ToString()
                                      : string.Empty;

                        // for saving, we only need to parse out the via parts if we don't have a saved joinvia id already
                        if (joinViaID < 1)
                        {
                            joinViaList = JoinVias.JoinViaPartsFromCompositeGuid(joinViaIDs);
                            joinViaID = 0; // 0 causes the save on the joinVia list
                        }

                        // if we already have a valid joinviaid, the joinviaAS and joinvialist will be empty at this point, that's fine
                        lstFilters.Add(order, new FieldFilter(field, tmpCT, tmpVal1, tmpVal2, order, new JoinVia(joinViaID, joinViaDescription, Guid.Empty, joinViaList), formid, isParentFilter: isParentFilter));
                        order++;
                    }
                }
            }

            #endregion parse filters

            #region parse trigger fields
            Guid triggerFieldId;
            List<LookupDisplayField> triggerFields = new List<LookupDisplayField>();
            LookupDisplayField lookupDisplayField = null;
            int triggerAttributeId;

            foreach (JavascriptTreeData.JavascriptTreeNode jsNode in oJsonTriggerFields.data)
            {
                if (Guid.TryParseExact(jsNode.attr.fieldid, "D", out triggerFieldId))
                {
                    field = fields.GetFieldByID(triggerFieldId);

                    if (field != null)
                    {
                        joinViaID = jsNode.attr.joinviaid;
                        joinViaDescription = jsNode.attr.crumbs;
                        joinViaIDs = jsNode.attr.id;
                        joinViaList = new SortedList<int, JoinViaPart>();

                        // for saving, we only need to parse out the via parts if we don't have a saved joinvia id already
                        if (joinViaID < 1)
                        {
                            joinViaList = JoinVias.JoinViaPartsFromCompositeGuid(joinViaIDs);
                            joinViaID = 0; // 0 causes the save on the joinVia list
                        }
                        else
                        {
                            joinViaList = new JoinVias(cMisc.GetCurrentUser()).GetJoinViaByID(joinViaID).JoinViaList;
                        }

                        lookupDisplayField = null;
                        if (attributeid > 0)
                        {
                            attribute = entity.getAttributeById(attributeid);
                            cManyToOneRelationship manyToOne = (cManyToOneRelationship)attribute;
                            foreach (var triggerLookupField in manyToOne.TriggerLookupFields)
                            {
                                if (triggerLookupField.TriggerJoinViaId == null)
                                {
                                    triggerLookupField.TriggerJoinViaId = 0;
                                }

                                if (triggerLookupField.TriggerDisplayFieldId == field.FieldID)
                                {
                                    if (triggerLookupField.TriggerJoinVia != null)
                                    {
                                        if (triggerLookupField.TriggerJoinViaId == joinViaID)
                                        {
                                            lookupDisplayField = triggerLookupField;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (joinViaList.Count == 0)
                                        {
                                            lookupDisplayField = triggerLookupField;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (lookupDisplayField == null)
                        {
                            var attributeFieldid = new Guid();
                            if (attribute == null)
                            {
                                attributeFieldid = Guid.Empty;
                            }
                            else
                            {
                                attributeFieldid = attribute.fieldid;
                            }

                            lookupDisplayField = new LookupDisplayField(0, field.FieldName, field.Description, field.Description, field.Comment, createdon, createdby, modifiedon, modifiedby, attributeFieldid, attributeid, new JoinVia(joinViaID, joinViaDescription, Guid.Empty, joinViaList), triggerFieldId);
                        }

                        triggerFields.Add(lookupDisplayField);
                    }
                }
            }

            #endregion

            attribute = new cManyToOneRelationship(attributeid, attributename, displayname, description, tooltip, mandatory, builtIn, createdon, createdby, modifiedon, modifiedby, relatedtable, fieldid, auditidentifier, false, false, aliasTable, displayFieldId, matchFields, maxRows, autocompleteFields, lstFilters, false, triggerFields);

            // only allow the attribute to be set as built-in/system if the user is "adminonly"
            if (!this.oCurrentUser.Employee.AdminOverride && !currentBuiltIn && builtIn)
            {
                builtIn = false;
            }

            attributeid = this.saveRelationship(entityid, attribute, isParentFilter, formid);
            // if the attribute is set as built-in/system but the GreenLight isn't, make the GreenLight built-in/system too
            if (attributeid > 0 && builtIn && !entity.BuiltIn)
            {
                entity.BuiltIn = true;
                this.saveEntity(entity);
            }

            return attributeid;
        }

        /// <summary>
        /// Save filter relationship
        /// </summary>
        /// <param name="entityid">
        /// Greenlight entity used as filter
        /// </param>
        /// <param name="attribute">
        /// Greenlight attribute used as filter
        /// </param>
        /// <param name="isParentFilter">
        /// Is this parent and child relationship filter.
        /// </param>
        /// <param name="formid">
        /// Greenlight form used as filter
        /// The formid.
        /// </param>
        /// <param name="connection">
        /// Connection override
        /// </param>
        /// <returns>
        /// Returns interger value of filterid
        /// </returns>
        public int saveRelationship(int entityid, cAttribute attribute, bool isParentFilter, int formid = 0, IDBConnection connection = null)
        {
            using (IDBConnection expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                int attributeid;
                List<LookupDisplayField> currentLookupDisplayFields = null;

                if (attribute.attributeid > 0 && attribute.GetType() == typeof(cManyToOneRelationship))
                {
                    // get any existing trigger fields before updates applied
                    cCustomEntity curEntity = lst[entityid];
                    cManyToOneRelationship currentAttribute = (cManyToOneRelationship)curEntity.attributes[attribute.attributeid];

                    currentLookupDisplayFields = (from x in currentAttribute.TriggerLookupFields select x).ToList();
                }

                currentLookupDisplayFields = currentLookupDisplayFields ?? new List<LookupDisplayField>();
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@entityid", entityid);
                expdata.sqlexecute.Parameters.AddWithValue("@attributeid", attribute.attributeid);
                expdata.sqlexecute.Parameters.AddWithValue("@displayname", attribute.displayname);
                expdata.sqlexecute.Parameters.AddWithValue("@mandatory", Convert.ToByte(attribute.mandatory));
                expdata.sqlexecute.Parameters.AddWithValue("@builtIn", Convert.ToByte(attribute.BuiltIn));

                if (attribute.description == string.Empty)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@description", attribute.description);
                }

                if (attribute.tooltip == string.Empty)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@tooltip", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@tooltip", attribute.tooltip);
                }

                if (attribute.modifiedby == null)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@userid", attribute.createdby);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@userid", attribute.modifiedby);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@viewid", DBNull.Value);
                expdata.sqlexecute.Parameters.AddWithValue("@relatedentity", DBNull.Value);
                expdata.sqlexecute.Parameters.AddWithValue("@maxRows", DBNull.Value);
                expdata.sqlexecute.Parameters.AddWithValue("@displayFieldID", DBNull.Value);
                expdata.sqlexecute.Parameters.AddWithValue("@aliasTableID", DBNull.Value);

                if (attribute.GetType() == typeof(cManyToOneRelationship))
                {
                    cManyToOneRelationship manytoone = (cManyToOneRelationship)attribute;
                    expdata.sqlexecute.Parameters.AddWithValue("@relatedtableid", manytoone.relatedtable.TableID);
                    expdata.sqlexecute.Parameters.AddWithValue("@relationshiptype", RelationshipType.ManyToOne);
                    if (manytoone.AliasTable != null)
                    {
                        expdata.sqlexecute.Parameters["@aliasTableID"].Value = manytoone.AliasTable.TableID;
                    }

                    expdata.sqlexecute.Parameters["@displayFieldID"].Value = manytoone.AutoCompleteDisplayField;
                    expdata.sqlexecute.Parameters["@maxRows"].Value = (manytoone.AutoCompleteMatchRows == 0) ? 15 : manytoone.AutoCompleteMatchRows;
                }
                else if (attribute.GetType() == typeof(cOneToManyRelationship))
                {
                    cOneToManyRelationship onetomany = (cOneToManyRelationship)attribute;
                    expdata.sqlexecute.Parameters.AddWithValue("@relatedtableid", onetomany.relatedtable.TableID);
                    expdata.sqlexecute.Parameters["@viewid"].Value = onetomany.viewid;
                    expdata.sqlexecute.Parameters.AddWithValue("@relationshiptype", RelationshipType.OneToMany);
                    cCustomEntity entity = getEntityByTableId(onetomany.relatedtable.TableID);

                    expdata.sqlexecute.Parameters["@relatedentity"].Value = entity.entityid;
                }

                expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
                expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("saveCustomEntityRelationship");
                attributeid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                expdata.sqlexecute.Parameters.Clear();

                if (attributeid > 0)
                {
                    if (attribute.GetType() == typeof(cManyToOneRelationship))
                    {
                        var attributeFieldId = attribute.fieldid;
                        if (attributeFieldId == Guid.Empty)
                        {
                            attributeFieldId = this.GetFieldIdOfAttribute(attributeid);
                        }

                        #region Save the match field ids

                        expdata.sqlexecute.Parameters.Clear();

                        expdata.AddWithValue("@attributeid", attributeid);
                        expdata.AddWithValue("@fieldIdList", ((cManyToOneRelationship)attribute).AutoCompleteMatchFieldIDList);

                        expdata.ExecuteProc("saveCustomEntityRelationshipMatchFields");

                        #endregion Save the match field ids

                        #region Save the field filters

                        expdata.sqlexecute.Parameters.Clear();
                        
                        this.saveFieldFilters(FieldFilterProductArea.Attributes, attributeid, ((cManyToOneRelationship)attribute).filters, isParentFilter, formid);

                        #endregion Save the field filters

                        #region Save / Delete trigger fields

                        JoinVias clsJoinVias = null;
                        foreach (LookupDisplayField lookupDisplayField in ((cManyToOneRelationship)attribute).TriggerLookupFields)
                        {
                            if (lookupDisplayField.TriggerJoinVia == null)
                            {
                                lookupDisplayField.TriggerJoinVia = new JoinVia(0, string.Empty, new Guid());
                            }
                            string desc = "attr - " + attributeid.ToString() + "_" + lookupDisplayField.TriggerDisplayFieldId.ToString();
                            int currentJoinViaId = 0;
                            if (lookupDisplayField.TriggerJoinViaId == 0 && lookupDisplayField.TriggerJoinVia.JoinViaList.Count > 0)
                            {
                                if (clsJoinVias == null)
                                {
                                    clsJoinVias = new JoinVias(this.oCurrentUser);
                                }

                                currentJoinViaId = clsJoinVias.SaveJoinVia(lookupDisplayField.TriggerJoinVia);
                            }
                            else
                            {
                                currentJoinViaId = (int)lookupDisplayField.TriggerJoinViaId;
                            }

                            LookupDisplayField newField = new LookupDisplayField(lookupDisplayField.attributeid, lookupDisplayField.attributename, lookupDisplayField.TriggerJoinVia.Description == string.Empty ? desc : desc + "_" + currentJoinViaId, lookupDisplayField.description, lookupDisplayField.tooltip, lookupDisplayField.attributeid == 0 ? DateTime.Now : lookupDisplayField.createdon, lookupDisplayField.attributeid == 0 ? this.oCurrentUser.EmployeeID : lookupDisplayField.createdby, DateTime.Now, this.oCurrentUser.EmployeeID, attributeFieldId, attributeid, lookupDisplayField.TriggerJoinVia, lookupDisplayField.TriggerDisplayFieldId);
                            if (currentJoinViaId != 0)
                            {
                                newField.TriggerJoinViaId = currentJoinViaId;
                            }

                            this.saveAttribute(entityid, newField);

                            if (lookupDisplayField.attributeid > 0)
                            {
                                // remove from current list, as edited, so not deleted
                                LookupDisplayField existingField = (from x in currentLookupDisplayFields where x.attributeid == lookupDisplayField.attributeid && x.TriggerAttributeId == lookupDisplayField.TriggerAttributeId select x).FirstOrDefault();

                                if (existingField != null)
                                {
                                    currentLookupDisplayFields.Remove(existingField);
                                }
                            }
                        }

                        expdata.sqlexecute.Parameters.Clear();

                        expdata.AddWithValue("@attributeid", attributeid);
                        expdata.AddWithValue("@fieldIdList", CustomEntityFieldsToDataTable(((cManyToOneRelationship)attribute).AutoCompleteDisplayFieldIDList));
                        expdata.ExecuteProc("SaveCustomEntityAttributeLookupSearchResultFields");

                        // entries left in currentLookupDisplayFields must have been deleted
                        foreach (LookupDisplayField displayField in currentLookupDisplayFields)
                        {
                            this.deleteLookupDisplayField((int)displayField.TriggerAttributeId, displayField.attributeid, this.oCurrentUser.EmployeeID, this.oCurrentUser.isDelegate ? this.oCurrentUser.Delegate.EmployeeID : 0);
                        }

                        #endregion
                    }

                    expdata.sqlexecute.Parameters.Clear();
                    var lastDatabaseUpdate = expdata.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
                    this.refreshCache(lastDatabaseUpdate);
                }

                return attributeid;
            }
        }

        /// <summary>
        /// The convert list Guid to data table.
        /// </summary>
        /// <param name="list">
        /// The List of Guid id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        private DataTable CustomEntityFieldsToDataTable(List<Guid> list)
        {
            var table = new DataTable();
            table.Columns.Add();
            foreach (var array in list)
            {
                table.Rows.Add(array);
            }
            return table;
        }

        /// <summary>
        /// The get field id of attribute from Database.
        /// </summary>
        /// <param name="attributeid">
        /// The attribute id.
        /// </param>
        /// <param name="connection">Connection override</param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        private Guid GetFieldIdOfAttribute(int attributeid, IDBConnection connection = null)
        {
            using (IDBConnection expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                Guid result = new Guid();
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@attributeid", attributeid);
                using (IDataReader reader = expdata.GetReader("select fieldid from customEntityAttributes where attributeid=@attributeid"))
                {
                    while (reader.Read())
                    {
                        result = reader.GetGuid(0);
                    }

                    reader.Close();
                }

                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startEntity"></param>
        /// <param name="curEntity"></param>
        /// <param name="level"></param>
        /// <param name="recHistory"></param>
        /// <returns></returns>
        public List<cCustomEntity> createParentEntityReportConfig(cCustomEntity startEntity, cCustomEntity curEntity, int level, List<cCustomEntity> recHistory)
        {
            if (recHistory == null)
            {
                recHistory = new List<cCustomEntity>();
            }

            if (startEntity == null)
            {
                // the start entity should never be null, so abort recursive procedure
                return recHistory;
            }

            if (curEntity == null)
            {
                curEntity = lst[startEntity.entityid];
            }

            recHistory.Insert(0, curEntity);
            bool called = false;

            foreach (KeyValuePair<int, cCustomEntity> e in lst)
            {
                cCustomEntity cmpEntity = (cCustomEntity)e.Value;

                if (!recHistory.Contains(cmpEntity))
                {
                    foreach (KeyValuePair<int, cAttribute> a in cmpEntity.attributes)
                    {
                        cAttribute curAttribute = (cAttribute)a.Value;

                        if (curAttribute.GetType() == typeof(cOneToManyRelationship))
                        {
                            // check for parent (related) entities
                            cOneToManyRelationship onetomany = (cOneToManyRelationship)curAttribute;
                            if (onetomany.entityid == (curEntity.IsSystemView ? curEntity.SystemView_DerivedEntityId : curEntity.entityid))
                            {
                                // make recursive call to find if further parent level exists
                                if (curEntity.IsSystemView && curEntity.SystemView_DerivedEntityId.HasValue)
                                {
                                    cCustomEntity relEntity = getEntityById(curEntity.SystemView_DerivedEntityId.Value);
                                    recHistory = createParentEntityReportConfig(startEntity, relEntity, level, recHistory);
                                }
                                else
                                {
                                    recHistory = createParentEntityReportConfig(startEntity, cmpEntity, level, recHistory);
                                }

                                called = true;
                                break;
                            }
                        }
                    }
                }

                if (called)
                {
                    break;
                }
            }

            return recHistory;
        }

        #region summary attribute

        /// <summary>
        /// Summary Attribute (Many to Many)
        /// </summary>
        /// <param name="entityid"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public int saveSummaryAttribute(int entityid, cSummaryAttribute attribute, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@attributeid", attribute.attributeid);
            expdata.sqlexecute.Parameters.AddWithValue("@entityid", entityid);
            expdata.sqlexecute.Parameters.AddWithValue("@displayname", attribute.displayname);
            expdata.sqlexecute.Parameters.AddWithValue("@description", attribute.description);
            expdata.sqlexecute.Parameters.AddWithValue("@tooltip", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@mandatory", false);
            expdata.sqlexecute.Parameters.AddWithValue("@fieldtype", (byte)FieldType.OTMSummary);
            expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.UtcNow);
            expdata.sqlexecute.Parameters.AddWithValue("@maxlength", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@format", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@defaultvalue", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@precision", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@workflowid", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@isauditidentity", false);
            expdata.sqlexecute.Parameters.AddWithValue("@commentText", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@relationshipType", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@related_entityid", attribute.SourceEntityID);
            expdata.sqlexecute.Parameters.AddWithValue("@isunique", attribute.isunique);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", this.oCurrentUser.EmployeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@populateExistingWithDefault", false);
            expdata.sqlexecute.Parameters.AddWithValue("@triggerAttributeId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@triggerJoinViaId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@triggerDisplayFieldId", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@relatedtable", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@boolAttribute", false);
            expdata.sqlexecute.Parameters.AddWithValue("@displayInMobile", false);
            expdata.sqlexecute.Parameters.AddWithValue("@builtIn", false);
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("saveCustomEntityAttribute"); // saveCustomSummaryAttribute

            int attributeid = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            // Get a list of current elements stored to detect any changes
            List<int> currentElementID = new List<int>();

            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@attributeid", attributeid);
            using (IDataReader reader = expdata.GetReader("select summary_attributeid from customEntityAttributeSummary where attributeid=@attributeid"))
            {
                while (reader.Read())
                {
                    currentElementID.Add(reader.GetInt32(0));
                }

                reader.Close();
            }

            foreach (KeyValuePair<int, cSummaryAttributeElement> kvp in attribute.SummaryElements)
            {
                cSummaryAttributeElement se = (cSummaryAttributeElement)kvp.Value;
                expdata.sqlexecute.Parameters.Clear();

                expdata.sqlexecute.Parameters.AddWithValue("@summary_attributeid", se.SummaryAttributeId);
                expdata.sqlexecute.Parameters.AddWithValue("@attributeid", attributeid);
                expdata.sqlexecute.Parameters.AddWithValue("@otm_attributeid", se.OTM_AttributeId);
                expdata.sqlexecute.Parameters.AddWithValue("@order", se.Order);
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
                if (this.oCurrentUser.Delegate != null)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("saveCustomSummaryAttributeElement");

                int summary_attributeid = (int)expdata.sqlexecute.Parameters["@identity"].Value;

                if (currentElementID.Contains(summary_attributeid))
                {
                    currentElementID.Remove(summary_attributeid);
                }
            }

            // delete elements no longer used
            foreach (int e in currentElementID)
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@summary_attributeid", e);
                expdata.ExecuteProc("deleteCustomOneToManySummaryAttributeElement");
            }

            List<int> currentCol = new List<int>();
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@attributeid", attributeid);
            using (IDataReader reader = expdata.GetReader("select columnid from customEntityAttributeSummaryColumns where attributeid=@attributeid"))
            {
                while (reader.Read())
                {
                    currentCol.Add(reader.GetInt32(0));
                }

                reader.Close();
            }

            foreach (KeyValuePair<int, cSummaryAttributeColumn> kvp in attribute.SummaryColumns)
            {
                cSummaryAttributeColumn sc = (cSummaryAttributeColumn)kvp.Value;
                expdata.sqlexecute.Parameters.Clear();

                int? jViaID = null;
                if (sc.JoinViaObj != null && sc.JoinViaObj.JoinViaID > 0)
                {
                    jViaID = sc.JoinViaObj.JoinViaID;
                }
                else if (sc.JoinViaObj != null && sc.JoinViaObj.JoinViaID == 0 &&
                        sc.JoinViaObj.JoinViaList.Count > 0)
                {
                    JoinVias clsJoinVias = new JoinVias(this.oCurrentUser);
                    jViaID = clsJoinVias.SaveJoinVia(sc.JoinViaObj);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@columnid", sc.ColumnId);
                expdata.sqlexecute.Parameters.AddWithValue("@attributeid", attributeid);
                expdata.sqlexecute.Parameters.AddWithValue("@columnAttributeID", sc.ColumnAttributeID);
                expdata.sqlexecute.Parameters.AddWithValue("@alternate_header", sc.AlternateHeader);
                expdata.sqlexecute.Parameters.AddWithValue("@width", sc.Width);
                expdata.sqlexecute.Parameters.AddWithValue("@order", sc.Order);
                expdata.sqlexecute.Parameters.AddWithValue("@default_sort", sc.DefaultSort);
                expdata.sqlexecute.Parameters.AddWithValue("@filterVal", sc.FilterValue);
                if (sc.DisplayFieldId == Guid.Empty)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@displayFieldId", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@displayFieldId", sc.DisplayFieldId);
                }

                if (!jViaID.HasValue)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@joinViaId", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@joinViaId", jViaID.Value);
                }

                expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

                expdata.ExecuteProc("saveCustomSummaryAttributeColumn");

                int columnid = (int)expdata.sqlexecute.Parameters["@identity"].Value;

                if (currentCol.Contains(columnid))
                {
                    currentCol.Remove(columnid);
                }
            }

            // delete columns no longer used
            foreach (int c in currentCol)
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@columnid", c);
                expdata.ExecuteProc("deleteCustomOneToManySummaryAttributeColumn");
            }

            //clearCacheForGetEntityIdByAttributeId();
            var lastDatabaseUpdate = expdata.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
            refreshCache(lastDatabaseUpdate);
            return attributeid;
        }

        /// <summary>
        /// Returns a string array of summary sources for the entity.
        /// </summary>
        /// <param name="entityid">Entity ID</param>
        /// <param name="related_entitytableid">Related Entity Table ID</param>
        /// <returns></returns>
        public static string[] SummarySources(int entityid, Guid related_entitytableid)
        {
            List<string> retGrids = new List<string>();
            CurrentUser curUser = cMisc.GetCurrentUser();
            StringBuilder sbRels = new StringBuilder();
            StringBuilder sbRelArray = new StringBuilder();
            StringBuilder sbCols = new StringBuilder();

            Dictionary<int, cOneToManyRelationship> rels = new Dictionary<int, cOneToManyRelationship>();

            // get available relationships to the selected entity
            cCustomEntities clsentities = new cCustomEntities(curUser);
            cCustomEntity entity = clsentities.getEntityByTableId(related_entitytableid);
            int related_entityid = entity.entityid;
            clsentities.getRelationshipAttributes(entityid, related_entityid, ref rels);

            StringBuilder sbRelHeader = new StringBuilder();
            bool relFound = false;

            sbRelHeader.Append("<div class=\"onecolumnpanel\">");
            sbRelHeader.Append("Select your relationships for the summary.&nbsp;");
            sbRelHeader.Append("<img class=\"tooltipicon\" id=\"imgSumRelTTip\" onmouseover=\"SEL.Tooltip.Show('cf17cc4f-9894-4ef8-97ad-dfccfb5227d5', 'sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" />");
            sbRelHeader.Append("</div>");
            sbRelHeader.Append("<table class=\"datatbl\">");
            sbRelHeader.Append("<tr>");
            sbRelHeader.Append("<th><input type=\"checkbox\" id=\"chkRelSAll\" onclick=\"SEL.CustomEntityAdministration.Attributes.Summary.RelSelectAll()\" /></th>");
            sbRelHeader.Append("<th>Relationship Name</th>");
            sbRelHeader.Append("<th>Sequence</th>");
            sbRelHeader.Append("</tr>");

            sbRelArray.Append("<script language=\"javascript\" type=\"text/javascript\">SEL.CustomEntityAdministration.Attributes.Summary.availableRels = []; SEL.CustomEntityAdministration.Attributes.Summary.columnRels = [];\n");

            bool rowalt = false;
            int seq = 1;
            foreach (KeyValuePair<int, cOneToManyRelationship> kvp in rels)
            {
                rowalt = rowalt ^ true;
                relFound = true;
                cOneToManyRelationship rel = (cOneToManyRelationship)kvp.Value;
                sbRels.Append("<tr>");
                sbRels.Append("<td class=\"");
                sbRels.Append(rowalt ? "row1" : "row2");
                sbRels.Append("\"><input type=\"checkbox\" id=\"chkRel_");
                sbRels.Append(rel.attributeid.ToString());
                sbRels.Append("\" onclick=\"SEL.CustomEntityAdministration.Attributes.Summary.SwapRelState('");
                sbRels.Append(rel.attributeid.ToString());
                sbRels.Append("')\" /></td>\n");
                sbRels.Append("<td class=\"");
                sbRels.Append(rowalt ? "row1" : "row2");
                sbRels.Append("\">");
                sbRels.Append(rel.displayname + "</td>\n");
                sbRels.Append("<td class=\"");
                sbRels.Append(rowalt ? "row1" : "row2");
                sbRels.Append("\"><input type=\"text\" id=\"txtRelSeq_");
                sbRels.Append(rel.attributeid.ToString());
                sbRels.Append("\" maxlength=\"3\" value=\"");
                sbRels.Append(seq.ToString());
                sbRels.Append("\" disabled=\"true\" />");
                sbRels.Append("<input type=\"hidden\" id=\"hiddenRelSAID_");
                sbRels.Append(rel.attributeid.ToString());
                sbRels.Append("\" value=\"0\" /></td>\n");
                sbRels.Append("</tr>");

                sbRelArray.Append("SEL.CustomEntityAdministration.Attributes.Summary.availableRels.push(");
                sbRelArray.Append(rel.attributeid.ToString());
                sbRelArray.Append(");\n");
                seq++;
            }

            sbRels.Append("</table>");

            // get columns for selection in the selected entity
            sbCols.Append("<div class=\"onecolumnpanel\">");
            sbCols.Append("Use the editor to select and adjust styling of your columns.&nbsp;");
            sbCols.Append("<img class=\"tooltipicon\" id=\"imgSumColTTip\" onmouseover=\"SEL.Tooltip.Show('6de1a2ca-8d28-4f92-8de2-bb6386e4d4f7', 'sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" />");
            sbCols.Append("</div>");
            sbCols.Append("<table class=\"datatbl\">");
            sbCols.Append("<tr>");
            sbCols.Append("<th><input type=\"checkbox\" id=\"chkColSAll\" onClick=\"SEL.CustomEntityAdministration.Attributes.Summary.ColSelectAll()\" /></th>");
            sbCols.Append("<th style=\"width: 150px;\">Field Name</th>");
            sbCols.Append("<th style=\"width: 150px;\">Display Field</th>");
            sbCols.Append("<th style=\"width: 75px;\">Sequence</th>");
            sbCols.Append("<th style=\"width: 150px;\">Alternate Header</th>");
            sbCols.Append("<th style=\"width: 75px;\">Column Width</th>");
            sbCols.Append("<th style=\"width: 150px;\">Filter Value</th>");
            sbCols.Append("<th style=\"width: 75px;\">Default Sort</th>");
            sbCols.Append("</tr>");

            rowalt = false;
            seq = 1;
            foreach (KeyValuePair<int, cAttribute> kvp in entity.attributes)
            {
                cAttribute att = (cAttribute)kvp.Value;
                if (!att.iskeyfield)
                {
                    string sAttId = att.attributeid.ToString();
                    rowalt = rowalt ^ true;
                    sbCols.Append("<tr>");
                    sbCols.Append("<td class=\"");
                    sbCols.Append(rowalt ? "row1" : "row2");
                    sbCols.Append("\"><input type=\"checkbox\" id=\"chkCol_");
                    sbCols.Append(sAttId);
                    sbCols.Append("\" onclick=\"SEL.CustomEntityAdministration.Attributes.Summary.RelColRowSwapState('");
                    sbCols.Append(sAttId + "', " + (att.GetType() == typeof(cManyToOneRelationship)).ToString().ToLower());
                    sbCols.Append(")\" /></td>\n");
                    sbCols.Append("<td class=\"");
                    sbCols.Append(rowalt ? "row1" : "row2");
                    sbCols.Append("\">");
                    sbCols.Append(att.displayname);
                    sbCols.Append("</td>\n");
                    sbCols.Append("<td class=\"");
                    sbCols.Append(rowalt ? "row1" : "row2");
                    sbCols.Append("\" />");
                    if (att.fieldtype == FieldType.Relationship && att.GetType() == typeof(cManyToOneRelationship))
                    {
                        // show icon to allow field selection from tree for field to display
                        sbCols.Append("<span id=\"mtoFieldLink_" + sAttId + "\" style=\"display: none;\"><img src=\"" + GlobalVariables.StaticContentLibrary + "/icons/16/plain/add2.png\" alt=\"Select\" onclick=\"SEL.CustomEntityAdministration.Attributes.Summary.SelectSummaryMTODisplayField(" + att.attributeid.ToString() + ");\" style=\"cursor: hand;\" />&nbsp;<span id=\"txtMTODisplayField_" + sAttId + "\" style=\"vertical-align: top;\">Not selected</span><hidden id=\"hidMTODisplayFieldId_" + sAttId + "\" value=\"\" /><hidden id=\"hidMTOJoinViaId_" + sAttId + "\" value=\"\" /><hidden id=\"hidMTOJoinViaCrumbs_" + sAttId + "\" value=\"\" /><hidden id=\"hidMTOJoinViaPath_" + sAttId + "\" value=\"\" />");
                    }
                    else
                    {
                        sbCols.Append("&nbsp;");
                    }

                    sbCols.Append("</td>\n");

                    sbCols.Append("<td class=\"");
                    sbCols.Append(rowalt ? "row1" : "row2");
                    sbCols.Append("\"><input style=\"width: 75px;\" type=\"textbox\" id=\"txtColSequence_");
                    sbCols.Append(sAttId);
                    sbCols.Append("\" maxlength=\"3\"  value=\"");
                    sbCols.Append(seq.ToString());
                    sbCols.Append("\" disabled=\"true\" /></td>\n");
                    sbCols.Append("<td class=\"");
                    sbCols.Append(rowalt ? "row1" : "row2");
                    sbCols.Append("\"><input style=\"width: 150px;\" type=\"textbox\" id=\"txtColAltHeader_");
                    sbCols.Append(sAttId);
                    sbCols.Append("\" maxlength=\"50\" disabled=\"true\" /></td>\n");
                    sbCols.Append("<td class=\"");
                    sbCols.Append(rowalt ? "row1" : "row2");
                    sbCols.Append("\"><input style=\"width: 75px;\" type=\"textbox\" id=\"txtColWidth_");
                    sbCols.Append(sAttId);
                    sbCols.Append("\" maxlength=\"3\" disabled=\"true\" /></td>\n");
                    sbCols.Append("<td class=\"");
                    sbCols.Append(rowalt ? "row1" : "row2");
                    sbCols.Append("\"><input style=\"width: 150px;\" type=\"text\" id=\"txtColFilter_");
                    sbCols.Append(sAttId);
                    sbCols.Append("\" maxlength=\"50\" disabled=\"true\" /></td>");
                    sbCols.Append("<td class=\"");
                    sbCols.Append(rowalt ? "row1" : "row2");
                    sbCols.Append("\" align=\"center\"><input type=\"radio\" id=\"optDefaultSort_");
                    sbCols.Append(sAttId);
                    sbCols.Append("\" name=\"rdoDefaultSort\" disabled=\"true\"  value=\"");
                    sbCols.Append(sAttId);
                    sbCols.Append("\" />");
                    sbCols.Append("<input type=\"hidden\" id=\"hiddenColID_" + sAttId + "\" value=\"0\" /></td>\n");
                    sbCols.Append("<input type=\"hidden\" id=\"hiddenDisplayFieldID_" + sAttId + "\" value=\"\" /></td>\n");
                    sbCols.Append("</tr>");

                    sbRelArray.Append("SEL.CustomEntityAdministration.Attributes.Summary.columnRels.push(");
                    sbRelArray.Append(sAttId);
                    sbRelArray.Append(");\n");
                    seq++;
                }
            }

            if (relFound)
            {
                sbCols.Append("</table>");
                sbRelArray.Append("</script>\n");
                retGrids.Add(sbRelHeader.ToString() + sbRels.ToString());
                retGrids.Add(sbCols.ToString());
                retGrids.Add(sbRelArray.ToString());
            }

            return retGrids.ToArray();
        }

        /// <summary>
        /// Gets the current selections for match fields when editing a n:1 attribute
        /// </summary>
        /// <param name="attributeid">Attribute ID</param>
        /// <returns>A list of items representing relationship match selections</returns>
        public static List<ListItem> getRelationshipMatchSelections(int attributeid)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            var fields = new cFields(curUser.AccountID);
            var clsEntities = new cCustomEntities(curUser);

            cCustomEntity curEntity = clsEntities.getEntityById(clsEntities.getEntityIdByAttributeId(attributeid));
            cAttribute att = curEntity.getAttributeById(attributeid);
            if (att.GetType() != typeof(cManyToOneRelationship))
            {
                return null;
            }

            var items = new List<ListItem>();
            foreach (Guid g in ((cManyToOneRelationship)att).AutoCompleteMatchFieldIDList)
            {
                cField matchField = fields.GetFieldByID(g);
                if (matchField != null)
                {
                    items.Add(new ListItem(matchField.Description, g.ToString()));
                }
            }

            return items;
        }

        /// <summary>
        /// Gets tree node data for a many to one relationship attribute
        /// </summary>
        /// <param name="attributeId">many to one attribute ID</param>
        /// <returns>jsTreeData object</returns>
        public JavascriptTreeData getDisplayFieldData(int attributeId)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            JavascriptTreeData javascriptData = new JavascriptTreeData();
            JavascriptTreeData.JavascriptTreeNode node;
            List<JavascriptTreeData.JavascriptTreeNode> lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();

            cCustomEntity curEntity = getEntityById(getEntityIdByAttributeId(attributeId));
            cAttribute relationshipAttribute = curEntity.getAttributeById(attributeId);

            if (relationshipAttribute.GetType() == typeof(cManyToOneRelationship))
            {
                // g for Group - table join, k for node linK - foreign key join, n for node - field
                string sourcePrefix = "k" + relationshipAttribute.fieldid.ToString();
                string guidPrefix = string.Empty;

                cTable relatedTable = ((cManyToOneRelationship)relationshipAttribute).relatedtable;
                if (relatedTable.TableSource == cTable.TableSourceType.CustomEntites)
                {
                    cCustomEntity oCustomEntity = this.getEntityByTableId(relatedTable.TableID);

                    if (oCustomEntity != null)
                    {
                        foreach (cAttribute a in oCustomEntity.attributes.Values)
                        {
                            if (a.GetType() == typeof(cCommentAttribute) || a.GetType() == typeof(cOneToManyRelationship) || a.GetType() == typeof(cSummaryAttribute))
                            {
                                continue;
                            }

                            node = TreeViewNodes.CreateJavascriptNode(a, sourcePrefix, relationshipAttribute);

                            lstNodes.Add(node);
                        }
                    }
                }
                else
                {
                    // standard field
                    foreach (cField field in fields.GetFieldsByTableID(relatedTable.TableID))
                    {
                        if (!field.IDField && !(field.FieldType.Length == 2 && field.FieldType.StartsWith("F")) && !field.FieldName.Contains("password")) // excludes function field types (not handled yet by joinvias)
                        {
                            node = TreeViewNodes.CreateJavascriptNode(field, sourcePrefix, relationshipAttribute);

                            lstNodes.Add(node);
                        }
                    }
                }

                lstNodes = lstNodes.OrderBy(x => x.data).ToList();
                javascriptData.data = lstNodes;
            }

            return javascriptData;
        }

        #endregion

        public string formatFieldData(cAttribute attribute, string rawData)
        {
            string fieldData = string.Empty;

            if (rawData != string.Empty)
            {
                switch (attribute.fieldtype)
                {
                    case FieldType.Currency:
                        decimal currencyAttribute = decimal.Parse(rawData);
                        fieldData = currencyAttribute.ToString("########0.00");
                        break;

                    case FieldType.DateTime:
                        cDateTimeAttribute dateatt = (cDateTimeAttribute)attribute;
                        DateTime date = DateTime.Parse(rawData);
                        switch (dateatt.format)
                        {
                            case AttributeFormat.DateOnly:
                                fieldData = date.ToShortDateString();
                                break;
                            case AttributeFormat.TimeOnly:
                                fieldData = date.ToShortTimeString();
                                break;
                            case AttributeFormat.DateTime:
                                fieldData = date.ToShortDateString() + " " + date.ToShortTimeString();
                                break;
                        }

                        break;

                    case FieldType.List:
                        cListAttribute listAttribute = (cListAttribute)attribute;
                        fieldData = (from x in listAttribute.items.Values
                                     where x.elementValue.ToString() == rawData
                                     select x.elementText).FirstOrDefault();
                        break;

                    case FieldType.TickBox:
                        fieldData = rawData == "True" ? "Yes" : "No";
                        break;

                    default:
                        fieldData = rawData;
                        break;
                }
            }

            return fieldData ?? string.Empty;
        }

        /// <summary>
        /// Checks that the current employee has access to ANY view which allows access to the current form.
        /// Please note that this should be replaced by Form-level access roles in the future.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="form"></param>
        /// <param name="recordID"></param>
        /// <returns>True if the current employee is allowed access to the form</returns>
        public bool CheckEmployeeHasAccessToForm(cCustomEntity entity, cCustomEntityForm form, int recordID)
        {
            bool hasAccess = false;

            if (recordID == 0)
            {
                foreach (cCustomEntityView view in entity.Views.Values)
                {
                    if (view.DefaultAddForm != null && view.DefaultAddForm.formid == form.formid)
                    {
                        if (this.oCurrentUser.CheckAccessRole(AccessRoleType.View, CustomEntityElementType.View, entity.entityid, view.viewid, false))
                        {
                            hasAccess = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (cCustomEntityView view in entity.Views.Values)
                {
                    if ((view.DefaultEditForm != null && view.DefaultEditForm.formid == form.formid) || view.DefaultAddForm != null && view.DefaultAddForm.formid == form.formid )
                    {
                        if (this.oCurrentUser.CheckAccessRole(AccessRoleType.View, CustomEntityElementType.View, entity.entityid, view.viewid, false))
                        {
                            hasAccess = true;
                            break;
                        }
                    }
                }

            }

            return hasAccess;
        }

        #region form generation

        /// <summary>
        /// generateForm: 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="formid"></param>
        /// <param name="viewid"></param>
        /// <param name="recordid"></param>
        /// <param name="tabid"></param>
        /// <param name="crumbs"></param>
        /// <param name="otmTableID"></param>
        /// <param name="summaryTableID"></param>
        /// <returns></returns>
        public Panel generateForm(cCustomEntity entity, int formid, int viewid, int? recordid, int tabid, List<sEntityBreadCrumb> crumbs, ref Dictionary<int, List<string>> otmTableID, ref Dictionary<int, List<string>> summaryTableID, ref List<string> scriptCmds)
        {
            var holder = new Panel();
            var form = entity.getFormById(formid);

            if (recordid != null)
            {
                nRecordid = (int)recordid;
            }

            lstCrumbs = crumbs;

            holder.Controls.Add(generateTabs(form, entity, viewid, tabid, ref otmTableID, ref summaryTableID, ref scriptCmds));
            return holder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="entity"></param>
        /// <param name="viewid"></param>
        /// <param name="activeTabId"></param>
        /// <param name="otmTableID"></param>
        /// <param name="summaryTableID"></param>
        /// <returns></returns>
        private TabContainer generateTabs(cCustomEntityForm form, cCustomEntity entity, int viewid, int activeTabId, ref Dictionary<int, List<string>> otmTableID, ref Dictionary<int, List<string>> summaryTableID, ref List<string> scriptCmds)
        {
            //generate the tabs
            TabContainer tabContainer = new TabContainer();
            TabPanel tabpnl;
            tabContainer.ID = "tabs" + form.formid;


            SortedList<byte, cCustomEntityFormTab> lstTabs = form.getTabsForForm();

            foreach (cCustomEntityFormTab tab in lstTabs.Values)
            {
                tabpnl = new TabPanel();
                tabpnl.ID = "tab" + tab.tabid;
                tabpnl.HeaderText = tab.headercaption;

                // overwrite last crumbs tabid with the current active tabid
                sEntityBreadCrumb lastCrumb = lstCrumbs[lstCrumbs.Count - 1];
                lastCrumb.TabID = tab.tabid;
                lstCrumbs[lstCrumbs.Count - 1] = lastCrumb;

                List<string> tab_otmTableID = new List<string>();
                List<string> tab_summaryTableID = new List<string>();

                generateSections(ref tabpnl, tab, entity, viewid, form, tab.tabid, ref tab_otmTableID, ref tab_summaryTableID, ref scriptCmds);

                otmTableID.Add(tab.tabid, tab_otmTableID);
                summaryTableID.Add(tab.tabid, tab_summaryTableID);

                tabContainer.Tabs.Add(tabpnl);
            }

            return tabContainer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabpnl"></param>
        /// <param name="tab"></param>
        /// <param name="entity"></param>
        /// <param name="viewid"></param>
        /// <param name="form"></param>
        /// <param name="activeTabId"></param>
        /// <param name="otmTableID"></param>
        /// <param name="summaryTableID"></param>
        private void generateSections(ref TabPanel tabpnl, cCustomEntityFormTab tab, cCustomEntity entity, int viewid, cCustomEntityForm form, int activeTabId, ref List<string> otmTableID, ref List<string> summaryTableID, ref List<string> scriptCmds)
        {
            Literal lit = new Literal();
            SortedList<byte, cCustomEntityFormSection> lstSections = tab.getSectionsForTab();

            lit.Text = "<div class=\"formpanel\">";
            tabpnl.Controls.Add(lit);
            foreach (cCustomEntityFormSection section in lstSections.Values)
            {
                PlaceHolder pnlSection = new PlaceHolder();
                lit = new Literal();
                lit.Text = "<div class=\"sectiontitle\">" + section.headercaption + "</div>";
                pnlSection.Controls.Add(lit);

                if (section.fields.Count > 0)
                {
                    generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);
                }

                tabpnl.Controls.Add(pnlSection);
            }

            lit = new Literal();
            lit.Text = "</div>";
            tabpnl.Controls.Add(lit);
        }

        /// <summary>
        /// A method to generate Fields
        /// </summary>
        /// <param name="pnlSection">
        /// The asp:placeholder section to add the fields to
        /// </param>
        /// <param name="section">
        /// The custom entity section object
        /// </param>
        /// <param name="entity">
        /// The entity object
        /// </param>
        /// <param name="viewid">
        /// The view id
        /// </param>
        /// <param name="form">
        /// The form object being generated
        /// </param>
        /// <param name="activeTabId">
        /// The current tab id
        /// </param>
        /// <param name="otmTableID">
        /// The 1:n table id
        /// </param>
        /// <param name="summaryTableID">
        /// The summary table id
        /// </param>
        /// <param name="scriptCmds">
        /// The script Commands.
        /// </param>
        public void generateFields(ref PlaceHolder pnlSection, cCustomEntityFormSection section, cCustomEntity entity, int viewid, cCustomEntityForm form, int activeTabId, ref List<string> otmTableID, ref List<string> summaryTableID, ref List<string> scriptCmds)
        {
            List<cCustomEntityFormField> formFields = section.getFieldsForSection();
            formFields.Sort(
                (ceff1, ceff2) =>
                    Convert.ToInt32(ceff1.row.ToString(CultureInfo.InvariantCulture) + ceff1.column.ToString(CultureInfo.InvariantCulture))
                    .CompareTo(Convert.ToInt32(ceff2.row + ceff2.column.ToString(CultureInfo.InvariantCulture))));

            string validationGroup = "vgCE_" + entity.entityid.ToString(CultureInfo.InvariantCulture);

            Literal cell;
            string controlid;
            Label lbl;
            Literal row;
            Literal lit;
            Literal spacer;

            bool first_sectionfield = true;
            string currentDivClass = "twocolumn"; // pnlSection will be the "formpanel"
            string previousDivClass = string.Empty;
            bool added = false;
            bool divStateOpen = false;
            bool firstField = true;
            int currentRow = 0;
            int currentColumn = 0;

            foreach (cCustomEntityFormField field in formFields)
            {
                if (formFields.IndexOf(field) > 0)
                {
                    firstField = false;
                }

                added = false;

                #region Add any required Spacers for fields that span two columns

                if (field.columnSpan == 2)
                {
                    while (currentRow < field.row)
                    {
                        row = new Literal();

                        spacer = new Literal { Text = "<span class=\"spacerpanel\"></span>" };

                        if (currentColumn == 0)
                        {
                            // Create a new div, insert a spacer then end the div
                            row.Text = "<div class=\"twocolumn\">";
                            pnlSection.Controls.Add(row);

                            pnlSection.Controls.Add(spacer);

                            row = new Literal { Text = "</div>" };
                            pnlSection.Controls.Add(row);

                            currentRow++;

                            firstField = false;
                            divStateOpen = false;
                        }
                        else
                        {
                            // End the current div
                            currentRow++;
                            currentColumn = 0;

                            row.Text = "</div>";
                            pnlSection.Controls.Add(row);
                            firstField = false;
                            divStateOpen = false;
                        }
                    }
                }

                #endregion Add any required Spacers for fields that span two columns

                row = new Literal();

                #region Determine the row style and open/close div

                previousDivClass = currentDivClass;

                currentDivClass = field.attribute.DivCssClass;

                bool isMultiLineOrWide = (field.attribute.GetType() == typeof(cTextAttribute)
                                          &&
                                             (((cTextAttribute)field.attribute).format == AttributeFormat.FormattedText
                                             || ((cTextAttribute)field.attribute).format == AttributeFormat.MultiLine
                                             || ((cTextAttribute)field.attribute).format == AttributeFormat.SingleLineWide))
                                          ||
                                             (field.attribute.GetType() == typeof(cListAttribute)
                                              && ((cListAttribute)field.attribute).format == AttributeFormat.ListWide)
                                              || (field.attribute.GetType() == typeof(cCommentAttribute));

                if (!first_sectionfield)
                {
                    if (divStateOpen && ((formFields.IndexOf(field) > 0 && isMultiLineOrWide) || (currentColumn == 2 && currentDivClass != "twocolumn") || (currentColumn == 2 && currentDivClass == "twocolumn" && previousDivClass != "twocolumn")))
                    {
                        // close div if next field is multiline text or if on second column
                        row.Text = "</div>";
                        currentColumn = 1;
                        divStateOpen = false;
                    }
                }

                if (currentColumn == 0)
                {
                    // only output div if first column field
                    row.Text += string.IsNullOrWhiteSpace(currentDivClass) ? "<div>" : "<div class=\"" + currentDivClass + "\">";
                    pnlSection.Controls.Add(row);
                    divStateOpen = true;
                }

                #endregion Determine the row style and open/close div

                #region Add any required Spacers for fields that span a single column

                if (field.columnSpan == 1)
                {
                    while (currentRow < field.row || currentColumn < field.column)
                    {
                        row = new Literal();

                        spacer = new Literal { Text = "<span class=\"spacerpanel\"></span>" };

                        if (currentColumn == 0)
                        {
                            // Insert a spacer
                            pnlSection.Controls.Add(spacer);
                            currentColumn = 1;
                            firstField = false;
                        }
                        else
                        {
                            // End the div and create a new one
                            currentRow++;
                            currentColumn = 0;

                            // If this is not the first field, end the current div and create a new one
                            if (firstField == false)
                            {
                                row.Text = "</div>";
                            }

                            row.Text += "<div class=\"twocolumn\">";
                            pnlSection.Controls.Add(row);
                            firstField = false;
                        }
                    }
                }

                #endregion Add any required Spacers for fields that span a single column

                #region Add the field to the page

                #region One to many relationships and Summarys

                switch (field.attribute.fieldtype)
                {
                    case FieldType.Relationship:

                        #region one to many relationship

                        if (field.attribute.GetType() == typeof(cOneToManyRelationship))
                        {
                            cOneToManyRelationship onetomany = (cOneToManyRelationship)field.attribute;

                            cCustomEntity onetomanyentity = getEntityById(onetomany.entityid);

                            cCustomEntityView onetomanyview = onetomanyentity.getViewById(onetomany.viewid);
                            UpdatePanel upnlonetomany = new UpdatePanel { ID = "upnlonetomany" + field.attribute.attributeid };

                            Table tblonetomany = new Table();
                            TableCell cellonetomany;
                            TableRow rowonetomany;

                            rowonetomany = new TableRow();
                            cellonetomany = new TableCell();

                            string relentityids = string.Empty;
                            string relformids = string.Empty;
                            string relrecordids = string.Empty;
                            string reltabids = string.Empty;
                            string relviewids = string.Empty;

                            lstCrumbs = this.lstCrumbs ?? new List<sEntityBreadCrumb>();

                            foreach (sEntityBreadCrumb crumb in lstCrumbs)
                            {
                                relentityids += crumb.EntityID + "_";
                                relformids += crumb.FormID + "_";
                                relrecordids += crumb.RecordID + "_";
                                reltabids += crumb.TabID + "_";
                                relviewids += crumb.ViewID + "_";
                            }

                            if (relentityids.Length > 0)
                            {
                                relentityids = relentityids.Substring(0, relentityids.Length - 1);
                            }

                            if (relformids.Length > 0)
                            {
                                relformids = relformids.Substring(0, relformids.Length - 1);
                            }

                            if (relrecordids.Length > 0)
                            {
                                relrecordids = relrecordids.Substring(0, relrecordids.Length - 1);
                            }

                            if (reltabids.Length > 0)
                            {
                                reltabids = reltabids.Substring(0, reltabids.Length - 1);
                            }

                            if (relviewids.Length > 0)
                            {
                                relviewids = relviewids.Substring(0, relviewids.Length - 1);
                            }
                            if (onetomanyview.allowadd && oCurrentUser.CheckAccessRole(AccessRoleType.Add, CustomEntityElementType.View, onetomanyentity.entityid, onetomany.viewid, false) && (onetomanyview.DefaultAddForm != null && onetomanyview.DefaultAddForm.fields.Count > 0))
                            {
                                LinkButton lnkonetomany = new LinkButton
                                {
                                    Text = "New " + onetomanyentity.entityname,
                                    ID = "lnkonetomany" + onetomany.attributeid,
                                    ClientIDMode = ClientIDMode.Static,
                                    ValidationGroup = validationGroup,
                                    CommandArgument =
                                        relentityids + "," + relformids + "," + relrecordids + "," + reltabids + ","
                                        + relviewids + "," + onetomany.viewid + "," + onetomany.entityid + "," + onetomanyview.DefaultAddForm.formid
                                        + ",0",
                                    OnClientClick =
                                            "javascript:if (!validateform('" + validationGroup + "')) { return false; }"
                                };

                                // lnkonetomany.OnClientClick = "javascript:if(!validateform('" + validationGroup + "')) { return false; } else { CheckForDuplicate(" + entity.entityid + ", '" + lnkonetomany.ClientID + "', " + activeTabId.ToString() + ", -1); return false; }";
                                lnkonetomany.Click += new EventHandler(lnkonetomany_Click);

                                cellonetomany.Controls.Add(lnkonetomany);
                                rowonetomany.Cells.Add(cellonetomany);
                                tblonetomany.Rows.Add(rowonetomany);
                            }

                            string formId = "0";
                            formId = onetomanyview.DefaultAddForm != null
                                                ? onetomanyview.DefaultAddForm.formid.ToString()
                                                : formId;

                            if (onetomanyentity.EnablePopupWindow)
                            {
                                // add spacing between the New Entity and Open in New Window links
                                var lblspacer = new Label { Text = "&nbsp;&nbsp;&nbsp;" };
                                cellonetomany.Controls.Add(lblspacer);
                                rowonetomany.Cells.Add(cellonetomany);
                                tblonetomany.Rows.Add(rowonetomany);

                                var lnkPopup = new LinkButton
                                {
                                    Text = "Open in New Window",
                                    ID = "lnkPopupWindow" + onetomany.attributeid,
                                    ClientIDMode = ClientIDMode.Static,
                                    ValidationGroup = validationGroup,
                                    OnClientClick =
                                        string.Format("javascript:var w=window.open('aeEntityPopup.aspx?entityid={0}&viewid={1}&relformid={2}&relentityid={3}&reltabid={4}&relrecordid={5}&attributeid={6}&relviewid={7} ','newWindow','width=1000,height=725, scrollbars=yes, resizable=yes');w.focus();return false;", onetomanyentity.entityid, onetomanyentity.DefaultPopupView, relformids, relentityids, reltabids, relrecordids, onetomany.attributeid, relviewids)
                                };

                                cellonetomany.Controls.Add(lnkPopup);
                                rowonetomany.Cells.Add(cellonetomany);
                                tblonetomany.Rows.Add(rowonetomany);
                            }

                            // create the grid
                            rowonetomany = new TableRow();
                            cellonetomany = new TableCell();
                            lit = new Literal
                            {
                                ID = "lit" + field.attribute.attributeid,
                                Text =
                                    "<div id=\"otmGrid" + field.attribute.attributeid.ToString(CultureInfo.InvariantCulture)
                                    +
                                    "\"><img alt=\"Table loading...\" src=\"/shared/images/ajax-loader.gif\" /></div>"
                            };
                            otmTableID.Add(entity.entityid.ToString(CultureInfo.InvariantCulture) + "," + field.attribute.attributeid.ToString(CultureInfo.InvariantCulture) + "," + viewid.ToString(CultureInfo.InvariantCulture) + "," + activeTabId.ToString(CultureInfo.InvariantCulture) + "," + form.formid.ToString(CultureInfo.InvariantCulture) + ",{0},'" + relentityids + "','" + relformids + "','" + relrecordids + "','" + reltabids + "','otmGrid" + field.attribute.attributeid.ToString(CultureInfo.InvariantCulture) + "','" + relviewids + "'");

                            cellonetomany.Controls.Add(lit);
                            rowonetomany.Cells.Add(cellonetomany);
                            tblonetomany.Rows.Add(rowonetomany);

                            upnlonetomany.ContentTemplateContainer.Controls.Add(tblonetomany);
                            pnlSection.Controls.Add(upnlonetomany);

                            added = true;
                        }

                        #endregion

                        break;

                    case FieldType.OTMSummary:

                        #region Summary Table

                        lit = new Literal
                        {
                            ID = "lit" + field.attribute.attributeid,
                            Text =
                                "<div id=\"summaryGrid" + field.attribute.attributeid.ToString(CultureInfo.InvariantCulture)
                                + "\"><img alt=\"Table loading...\" src=\"/shared/images/ajax-loader.gif\" /></div>"
                        };

                        summaryTableID.Add(
                            entity.entityid.ToString(CultureInfo.InvariantCulture) + ","
                            + field.attribute.attributeid.ToString(CultureInfo.InvariantCulture) + ","
                            + viewid.ToString(CultureInfo.InvariantCulture) + ","
                            + activeTabId.ToString(CultureInfo.InvariantCulture) + ","
                            + form.formid.ToString(CultureInfo.InvariantCulture) + ","
                            + nRecordid.ToString(CultureInfo.InvariantCulture) + ",'summaryGrid"
                            + field.attribute.attributeid.ToString(CultureInfo.InvariantCulture) + "'");

                        pnlSection.Controls.Add(lit);

                        added = true;

                        #endregion

                        break;

                    default:
                        break;
                }

                #endregion One to many relationships and Summarys

                if (!added)
                {
                    #region Inputs

                    controlid = field.attribute.ControlID;

                    #region Label

                    if (field.attribute.GetType() != typeof(cCommentAttribute) && field.attribute.GetType() != typeof(cManyToOneRelationship))
                    {
                        lbl = new Label();

                        if (field.attribute.fieldtype != FieldType.RunWorkflow)
                        {
                            lbl.AssociatedControlID = controlid;

                            if (string.IsNullOrWhiteSpace(field.labelText) == false)
                            {
                                lbl.ToolTip = field.labelText;
                                lbl.Text = field.labelText;
                            }
                            else if (field.attribute.fieldtype == FieldType.LookupDisplayField)
                            {
                                string labelText = ((LookupDisplayField)field.attribute).TriggerDisplayFieldId.HasValue ? fields.GetFieldByID(((LookupDisplayField)field.attribute).TriggerDisplayFieldId.Value).Description : field.attribute.displayname;
                                lbl.ToolTip = labelText;
                                lbl.Text = labelText;
                            }
                            else
                            {
                                lbl.ToolTip = field.attribute.displayname;
                                lbl.Text = field.attribute.displayname;
                            }

                            if (this.GetMandatoryValueOfAnAttribute(field))
                            {
                                    lbl.Text += @"*";
                                    lbl.CssClass = "mandatory";
                            }
                        }

                        pnlSection.Controls.Add(lbl);
                    }
                    else
                    {
                        if (field.attribute.GetType() == typeof(cManyToOneRelationship))
                        {
                            var manyToOne = (cManyToOneRelationship)field.attribute;
                            pnlSection.Page = new Page();

                            var selectinator = (Selectinator)pnlSection.Page.LoadControl("~/shared/usercontrols/Selectinator.ascx");
                            selectinator.Filters = new List<JSFieldFilter>();
                            selectinator.ID = field.attribute.ControlID;
                            selectinator.Name = string.IsNullOrWhiteSpace(field.labelText) == false ? field.labelText : field.attribute.displayname;
                            selectinator.TableGuid = manyToOne.relatedtable.TableID;
                            selectinator.DisplayField = manyToOne.AutoCompleteDisplayField;
                            selectinator.MatchFields = manyToOne.AutoCompleteMatchFieldIDList;
                            selectinator.AutocompleteFields = manyToOne.AutoCompleteDisplayFieldIDList;
                            selectinator.SetAutocompleteMultipleResultFields = true;
                            selectinator.Tooltip = HttpUtility.HtmlEncode(cMisc.EscapeLinebreaks(field.attribute.tooltip));
                            selectinator.AutoCompleteTriggerFields =
                                        manyToOne.TriggerLookupFields.Select(
                                            x =>
                                            new AutoCompleteTriggerField
                                            {
                                                ControlId = "txt" + x.attributeid,
                                                DisplayFieldId = x.TriggerDisplayFieldId ?? Guid.Empty,
                                                JoinViaId = x.TriggerJoinViaId ?? 0,
                                                DisplayValue = string.Empty
                                            }).ToList();
                            selectinator.IsEnabled = !field.isReadOnly;
                            foreach (FieldFilter filter in manyToOne.filters.Values)
                            {
                                if (filter.FormId == form.formid && !filter.IsParentFilter)
                                {
                                    selectinator.Filters.Add(FieldFilters.CsToJs(filter, oCurrentUser));
                                }

                                // Check if it is editing record
                                if (this.nRecordid > 0)
                                {
                                SortedList<int, object> record = this.getEntityRecord(entity, this.nRecordid, form);

                                    // Check for parent filter
                                    if (filter.FormId == form.formid && filter.IsParentFilter && filter.ValueOne != null)
                                    {
                                    string recordValues = record[Convert.ToInt32(filter.ValueOne)].ToString();
                                    var fieldToBuild = Convert.ToInt32(field.attribute.attributeid);

                                        if (fieldToBuild > 0)
                                        {
                                            var filterBy = (cManyToOneRelationship)entity.getAttributeById(Convert.ToInt32(filter.ValueOne));
                                            selectinator.Filters.Add(new JSFieldFilter { ConditionType = ConditionType.Equals, FieldID = filterBy.relatedtable.PrimaryKeyID, ValueOne = recordValues, Joiner = ConditionJoiner.And, FilterOnEdit = true});
                                        }
                                    }
                            }
                        }
                            //looping through filter twice because if a particular form does not have any filter added to it (which is when count will be 0), it should take the basic filter added to the attribute(where formid is 0).
                            if (selectinator.Filters.Count == 0)
                            {
                                foreach (FieldFilter filter in manyToOne.filters.Values)
                                {
                                    if (filter.FormId == 0)
                                    {
                                        selectinator.Filters.Add(FieldFilters.CsToJs(filter, oCurrentUser));
                                    }
                                }
                            }

                            selectinator.ValidationControlGroup = validationGroup;
                            selectinator.Mandatory = this.GetMandatoryValueOfAnAttribute(field);
                            pnlSection.Controls.Add(selectinator);

                        }
                    }

                    #endregion Label

                    #region Input
                    if (field.attribute.GetType() != typeof(cManyToOneRelationship))
                    {
                        cell = new Literal { Text = "<span class=\"inputs\">" };
                        pnlSection.Controls.Add(cell);

                        this.GenerateFieldsInput(ref pnlSection, field, validationGroup, entity, form, activeTabId);

                        cell = new Literal { Text = "</span>" };

                        #endregion Input

                        #region Icon

                        cell.Text += "<span class=\"inputicon\">";
                        pnlSection.Controls.Add(cell);

                        switch (field.attribute.fieldtype)
                        {
                            case FieldType.DateTime:
                                var calImg = new Image { ImageUrl = "~/shared/images/icons/cal.gif", ID = "img" + field.attribute.attributeid };

                                switch (((cDateTimeAttribute)field.attribute).format)
                                {
                                    case AttributeFormat.DateOnly:
                                    case AttributeFormat.DateTime:
                                        calImg.CssClass = "dateCalImg";
                                        break;
                                    case AttributeFormat.TimeOnly:
                                        calImg.CssClass = "timeCalImg";
                                        break;
                                }

                                pnlSection.Controls.Add(calImg);
                                break;
                            case FieldType.Attachment:
                                var deleteImage = new Image { ImageUrl = "~/shared/images/icons/delete2.png", ID = "img" + field.attribute.attributeid };

                                pnlSection.Controls.Add(deleteImage);
                                break;
                        }

                        cell = new Literal { Text = "</span>" };

                        #endregion Icon

                        #region ToolTip

                        cell.Text += "<span class=\"inputtooltipfield\">";

                        string tooltipText = cMisc.EscapeLinebreaks(field.attribute.tooltip);
                        if (string.IsNullOrWhiteSpace(tooltipText) == false)
                        {
                            cell.Text += string.Format("<img id=\"imgToolTip{0}\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('{1}','sm', this);\" src=\"{2}/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" />", field.attribute.attributeid, HttpUtility.HtmlEncode(tooltipText), cMisc.Path);
                        }

                        cell.Text += "</span>";

                        #endregion ToolTip

                        #region Validators

                        cell.Text += "<span class=\"inputvalidatorfield\" id=\"spanvalidate" + field.attribute.attributeid + "\">";
                        pnlSection.Controls.Add(cell);

                        #region Mandatory

                        if (this.GetMandatoryValueOfAnAttribute(field))
                        {
                            this.GenerateFieldsMandatoryValidator(ref pnlSection, field, validationGroup);
                        }

                        #endregion Mandatory

                        #region Formatting

                        this.GenerateFieldsFormattingValidators(ref pnlSection, field, validationGroup);

                        #endregion Formatting

                        cell = new Literal { Text = "</span>" };
                        pnlSection.Controls.Add(cell);
                    }
                    #endregion Validators

                    first_sectionfield = false;

                    #endregion Inputs
                }

                if (currentColumn == 1 || field.columnSpan == 2)
                {
                    row = new Literal { Text = "</div>" };
                    pnlSection.Controls.Add(row);
                    divStateOpen = false;
                }

                #endregion Add the field to the page

                #region Update the currentRow and currentColumn before the next iteration

                if (field.columnSpan == 1)
                {
                    if (currentColumn == 0)
                    {
                        currentColumn = 1;
                    }
                    else
                    {
                        currentRow++;
                        currentColumn = 0;
                    }
                }
                else
                {
                    if (currentColumn == 1)
                    {
                        currentRow++;
                        currentColumn = 0;
                    }

                    currentRow++;
                }

                #endregion Update the currentRow and currentColumn before the next iteration
            }

            #region Close final div

            if (!divStateOpen)
            {
                return;
            }

            lit = new Literal { Text = "</div>" };
            pnlSection.Controls.Add(lit);

            #endregion Close final div
        }

        /// <summary>
        /// Creates the input field on the form for the custom entity field type
        /// </summary>
        /// <param name="ph">The asp:placeholder</param>
        /// <param name="field">The field object</param>
        /// <param name="validationGroup">The validation group</param>
        /// <param name="entity">The entity object</param>
        /// <param name="form">The form object</param>
        /// <param name="activeTabID">The active tab id</param>
        public void GenerateFieldsInput(ref PlaceHolder ph, cCustomEntityFormField field, string validationGroup, cCustomEntity entity, cCustomEntityForm form, int activeTabID)
        {
            cCurrencies clsCurrencies = new cCurrencies(oCurrentUser.AccountID, oCurrentUser.CurrentSubAccountId);
            WebControl webControl;
            cAttribute attr;
            Literal lit;

            switch (field.attribute.fieldtype)
            {
                case FieldType.DateTime:
                    cDateTimeAttribute datetimeatt = (cDateTimeAttribute)field.attribute;
                    webControl = new TextBox { ID = "txt" + field.attribute.attributeid };

                    switch (datetimeatt.format)
                    {
                        case AttributeFormat.DateOnly:
                            webControl.CssClass = "fillspan dateField";
                            break;

                        case AttributeFormat.TimeOnly:
                            webControl.CssClass = "fillspan timeField";
                            break;

                        case AttributeFormat.DateTime:
                            webControl.CssClass = "fillspan dateField smallDateField";
                            if (field.isReadOnly)
                            {
                                webControl.Enabled = false;
                            }
                            ph.Controls.Add(webControl);

                            webControl = new TextBox
                            {
                                ID = "txt" + field.attribute.attributeid + "_time",
                                CssClass = "fillspan timeField smallTimeField"
                            };
                            break;
                    }

                    if (field.isReadOnly)
                    {
                        webControl.Enabled = false;
                    }

                    ph.Controls.Add(webControl);

                    break;
                case FieldType.Text:
                case FieldType.Currency:
                case FieldType.Number:
                case FieldType.Integer:
                case FieldType.Contact:
                    webControl = new TextBox { ID = "txt" + field.attribute.attributeid, CssClass = "fillspan" };

                    if (field.attribute is cTextAttribute)
                    {
                        attr = field.attribute;

                        if (((cTextAttribute)attr).maxlength.HasValue && ((cTextAttribute)attr).maxlength > 0)
                        {
                            ((TextBox)webControl).MaxLength = ((cTextAttribute)attr).maxlength.Value;
                            webControl.Attributes.Add("textareamaxlength", ((cTextAttribute)attr).maxlength.ToString());
                        }
                        else
                        {
                            if (((cTextAttribute)attr).format == AttributeFormat.MultiLine)
                            {
                                ((TextBox)webControl).MaxLength = 4000;
                                webControl.Attributes.Add("textareamaxlength", "4000");
                            }
                            else
                            {
                                ((TextBox)webControl).MaxLength = 500;
                                webControl.Attributes.Add("textareamaxlength", "500");
                            }
                        }

                        if (((cTextAttribute)attr).format == AttributeFormat.MultiLine)
                        {
                            ((TextBox)webControl).TextMode = TextBoxMode.MultiLine;
                        }
                    }
                    else if (field.attribute is cContactAttribute)
                    {
                        ((TextBox) webControl).MaxLength = 255;
                        webControl.Attributes.Add("textareamaxlength", "255");
                    }

                    if (field.isReadOnly)
                    {
                        webControl.Enabled = false;
                    }

                    ph.Controls.Add(webControl);
                    break;

                case FieldType.LargeText:
                    attr = field.attribute;

                    switch (((cTextAttribute)attr).format)
                    {
                        case AttributeFormat.MultiLine:
                            webControl = new TextBox { ID = "txt" + field.attribute.attributeid };
                            ((TextBox)webControl).TextMode = TextBoxMode.MultiLine;

                            if (((cTextAttribute)attr).maxlength.HasValue && ((cTextAttribute)attr).maxlength > 0)
                            {
                                ((TextBox)webControl).MaxLength = ((cTextAttribute)attr).maxlength.Value;
                                webControl.Attributes.Add("textareamaxlength", ((cTextAttribute)attr).maxlength.ToString());
                            }
                            else
                            {
                                ((TextBox)webControl).MaxLength = 4000;
                                webControl.Attributes.Add("textareamaxlength", "4000");
                            }

                            if (field.isReadOnly)
                            {
                                webControl.Enabled = false;
                            }

                            ph.Controls.Add(webControl);
                            break;

                        case AttributeFormat.FormattedText:
                            Panel pnl = new Panel { ID = "rtepanel" + field.attribute.attributeid, CssClass = "rtePanel" };

                            lit = new Literal { ID = "rteliteral" + field.attribute.attributeid };
                            pnl.Controls.Add(lit);

                            HiddenField hidden = new HiddenField { ID = "txthidden" + field.attribute.attributeid };

                            webControl = new TextBox { ID = "txt" + field.attribute.attributeid };
                            webControl.Style.Add(HtmlTextWriterStyle.Display, "none");

                            if (field.isReadOnly)
                            {
                                webControl.Enabled = false;
                                pnl.CssClass = "rtePanelReadOnly";
                            }
                            else
                            {
                                Literal litRtEditLink = new Literal
                                {
                                    Text = new StringBuilder("<div style=\"vertical-align: middle;\"><a style=\"cursor: hand; vertical-align:center;\" onclick=\"EditRichTextEditor('tabs")
                                        .Append(form.formid)
                                        .Append("','tab")
                                        .Append(activeTabID)
                                        .Append("','")
                                        .Append(webControl.UniqueID)
                                        .Append("','")
                                        .Append(hidden.UniqueID)
                                        .Append("','")
                                        .Append(pnl.UniqueID)
                                        .Append("','" + ((cTextAttribute)field.attribute).RemoveFonts)
                                        .Append("');\"><img src=\"/shared/images/icons/edit.png\" alt=\"Edit\" style=\"vertical-align:bottom;\" />&nbsp;&nbsp;Edit Text</a></div>").ToString()
                                };

                                ph.Controls.Add(litRtEditLink);
                            }

                            ph.Controls.Add(pnl);
                            ph.Controls.Add(webControl);
                            ph.Controls.Add(hidden);
                            break;
                    }

                    break;

                case FieldType.TickBox:
                    webControl = new DropDownList();
                    webControl.ID = "cmb" + field.attribute.attributeid;
                    webControl.CssClass = "fillspan";
                    cTickboxAttribute tickatt = (cTickboxAttribute)field.attribute;

                    if (tickatt.mandatory)
                    {
                        ((DropDownList)webControl).Items.Add(new ListItem("[None]", "-1"));
                    }

                    ((DropDownList)webControl).Items.Add(new ListItem("Yes", "1"));
                    ((DropDownList)webControl).Items.Add(new ListItem("No", "0"));

                    if (tickatt.mandatory == false && tickatt.defaultvalue != "")
                    {
                        if (((DropDownList)webControl).Items.FindByText(tickatt.defaultvalue) != null)
                        {
                            ((DropDownList)webControl).Items.FindByText(tickatt.defaultvalue).Selected = true;
                        }
                    }

                    if (field.isReadOnly)
                    {
                        webControl.Enabled = false;
                    }

                    ph.Controls.Add((DropDownList)webControl);
                    break;

                case FieldType.List:
                    attr = field.attribute;
                    webControl = new DropDownList { ID = "cmb" + field.attribute.attributeid, CssClass = "fillspan" };
                    ((DropDownList)webControl).Items.Add(new ListItem("[None]", "-1"));

                    foreach (KeyValuePair<int, cListAttributeElement> listitem in ((cListAttribute)attr).items)
                    {
                        cListAttributeElement listelement = listitem.Value;
                        ((DropDownList)webControl).Items.Add(
                            new ListItem(listelement.elementText, listelement.elementValue.ToString(), !listelement.Archived));
                    }

                    if (field.isReadOnly)
                    {
                        webControl.Enabled = false;
                    }

                    ph.Controls.Add(webControl);
                    break;

                case FieldType.Relationship:
                    break;

                case FieldType.RunWorkflow:
                    cRunWorkflowAttribute reqRunWorkflowAtt = (cRunWorkflowAttribute)field.attribute;
                    Literal litRunWorkflow = new Literal { ID = "litRunWorkflow" + field.attribute.attributeid, Text = new StringBuilder("<a href=\"javascript:runWorkflow(").Append(reqRunWorkflowAtt.workflow.workflowid).Append(", ").Append(this.nRecordid).Append(");\">").Append(field.attribute.description).Append("</a>").ToString() };

                    ph.Controls.Add(litRunWorkflow);
                    break;

                case FieldType.CurrencyList:
                    webControl = new DropDownList { ID = "ddl" + field.attribute.attributeid, CssClass = "fillspan" };
                    if (entity.DefaultCurrencyID != null)
                    {
                        ((DropDownList)webControl).Items.AddRange(clsCurrencies.CreateDropDown(entity.DefaultCurrencyID.Value));
                    }

                    ph.Controls.Add(webControl);
                    break;

                case FieldType.Comment:
                    lit = new Literal { ID = "litAdvice" + field.attribute.attributeid, Text = ((cCommentAttribute)field.attribute).commentText };

                    ph.Controls.Add(lit);
                    break;

                case FieldType.LookupDisplayField:
                    webControl = new Label { ID = "txt" + field.attribute.attributeid, CssClass = "lookupdisplayvalue", Text = "&nbsp;" };

                    ph.Controls.Add(webControl);
                    break;

                case FieldType.Attachment:
                    ph.Page = new Page();
                    var fileUploader = (FileUploader)ph.Page.LoadControl("~/shared/usercontrols/FileUploader.ascx");
                    //webControl = new FileUploader { ID = "txt" + field.attribute.attributeid, CssClass = "fillspan" };
                    fileUploader.ID = field.attribute.ControlID;
                    fileUploader.Mandatory = this.GetMandatoryValueOfAnAttribute(field);
                    fileUploader.IncludeImageLibrary = ((cAttachmentAttribute)field.attribute).IncludeImageLibrary;
                    fileUploader.ValidationControlGroup = validationGroup;
                    fileUploader.Name = field.attribute.displayname;
                    fileUploader.ImageLibraryModalPopupExtenderId = ImageLibraryModalPopupExtenderId;
                    fileUploader.AttributeId = field.attribute.attributeid.ToString();
                    ph.Controls.Add(fileUploader);
                    break;

                default:
                    throw new Exception("Couldn't build generateFieldsInput WebControl(s)");
            }
        }

        /// <summary>
        /// Creates a mandatory field validator based upon the custom entity field type and adds it to the validation group
        /// </summary>
        /// <param name="ph">The asp:placeholder object</param>
        /// <param name="field">The custom entity form field object</param>
        /// <param name="validationGroup">The validation group</param>
        public void GenerateFieldsMandatoryValidator(ref PlaceHolder ph, cCustomEntityFormField field, string validationGroup)
        {
            BaseValidator val;

            switch (field.attribute.fieldtype)
            {
                case FieldType.TickBox:
                case FieldType.List:
                    val = new CompareValidator { ID = "req" + field.attribute.attributeid, ControlToValidate = "cmb" + field.attribute.attributeid, Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic, ErrorMessage = ValidatorMessages.MandatoryDropdown(field.attribute.displayname) };

                    ((CompareValidator)val).Type = ValidationDataType.Integer;
                    ((CompareValidator)val).ValueToCompare = "-1";
                    ((CompareValidator)val).Operator = ValidationCompareOperator.GreaterThan;

                    ph.Controls.Add(val);
                    break;

                case FieldType.Relationship:
                case FieldType.Attachment:
                    break;
                default:
                    val = new RequiredFieldValidator { ID = "req" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic, ErrorMessage = ValidatorMessages.MandatoryDropdown(field.attribute.displayname) };

                    if (field.attribute.fieldtype == FieldType.DateTime)
                    {
                        val.CssClass = "reqDateTimeVal";
                    }

                    ph.Controls.Add(val);
                    break;
            }
        }

        /// <summary>
        /// Creates a set of format validators based upon the custom entity field type and adds it to the validation group
        /// </summary>
        /// <param name="ph">The asp:placeholder object</param>
        /// <param name="field">The custom entity form field object</param>
        /// <param name="validationGroup">The validation group</param>
        public void GenerateFieldsFormattingValidators(ref PlaceHolder ph, cCustomEntityFormField field, string validationGroup)
        {
            CompareValidator compval;
            RegularExpressionValidator regCompval;

            switch (field.attribute.fieldtype)
            {
                case FieldType.Currency:
                    compval = new CompareValidator { ID = "comp" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, Type = ValidationDataType.Currency, ErrorMessage = ValidatorMessages.FormatCurrency(field.attribute.displayname), Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic, Operator = ValidationCompareOperator.DataTypeCheck };
                    ph.Controls.Add(compval);
                    break;
                case FieldType.Integer:
                    compval = new CompareValidator { ID = "comp" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, Type = ValidationDataType.Integer, ErrorMessage = ValidatorMessages.FormatInteger(field.attribute.displayname), Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic, Operator = ValidationCompareOperator.DataTypeCheck };
                    ph.Controls.Add(compval);

                    compval = new CompareValidator { ID = "compgte" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, Type = ValidationDataType.Integer, ValueToCompare = "-2147483648", Operator = ValidationCompareOperator.GreaterThanEqual, ErrorMessage = ValidatorMessages.FormatIntegerGreaterThan(field.attribute.displayname, -2147483648), Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic };
                    ph.Controls.Add(compval);

                    compval = new CompareValidator { ID = "complte" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, Type = ValidationDataType.Integer, ValueToCompare = "2147483647", Operator = ValidationCompareOperator.LessThanEqual, ErrorMessage = ValidatorMessages.FormatIntegerLessThan(field.attribute.displayname, 2147483647), Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic };
                    ph.Controls.Add(compval);

                    break;
                case FieldType.Number:
                    compval = new CompareValidator { ID = "comp" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, Type = ValidationDataType.Double, ErrorMessage = ValidatorMessages.FormatNumber(field.attribute.displayname), Text = "*", Operator = ValidationCompareOperator.DataTypeCheck, Display = ValidatorDisplay.Dynamic, ValidationGroup = validationGroup };
                    ph.Controls.Add(compval);

                    break;

                case FieldType.DateTime:
                    cDateTimeAttribute datetimeatt = (cDateTimeAttribute)field.attribute;
                    string mindate;
                    string maxdate;

                    switch (datetimeatt.format)
                    {
                        case AttributeFormat.DateOnly:
                            compval = new CompareValidator { ID = "comp" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, Type = ValidationDataType.Date, ErrorMessage = ValidatorMessages.FormatDate(field.attribute.displayname), Text = "*", Operator = ValidationCompareOperator.DataTypeCheck, ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic };

                            ph.Controls.Add(compval);

                            mindate = new DateTime(1753, 1, 1).ToShortDateString();
                            compval = new CompareValidator { ID = "compgte" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, Type = ValidationDataType.Date, ValueToCompare = mindate, Operator = ValidationCompareOperator.GreaterThanEqual, ErrorMessage = ValidatorMessages.FormatDateMinimum(field.attribute.displayname, mindate), Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic };

                            ph.Controls.Add(compval);

                            maxdate = new DateTime(3000, 12, 31).ToShortDateString();
                            compval = new CompareValidator { ID = "complte" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, Type = ValidationDataType.Date, ValueToCompare = maxdate, Operator = ValidationCompareOperator.LessThanEqual, ErrorMessage = ValidatorMessages.FormatDateMaximum(field.attribute.displayname, maxdate), Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic };

                            ph.Controls.Add(compval);
                            break;

                        case AttributeFormat.DateTime:
                            compval = new CompareValidator { ID = "comp" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, Type = ValidationDataType.Date, ErrorMessage = ValidatorMessages.FormatDateAndTime(field.attribute.displayname), Text = "*", Operator = ValidationCompareOperator.DataTypeCheck, ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic };

                            ph.Controls.Add(compval);

                            mindate = new DateTime(1753, 1, 1).ToShortDateString();
                            compval = new CompareValidator { ID = "compgte" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, Type = ValidationDataType.Date, ValueToCompare = mindate, Operator = ValidationCompareOperator.GreaterThanEqual, ErrorMessage = ValidatorMessages.FormatDateAndTimeMinimum(field.attribute.displayname, mindate), Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic };

                            ph.Controls.Add(compval);

                            maxdate = new DateTime(3000, 12, 31).ToShortDateString();
                            compval = new CompareValidator { ID = "complte" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, Type = ValidationDataType.Date, ValueToCompare = maxdate, Operator = ValidationCompareOperator.LessThanEqual, ErrorMessage = ValidatorMessages.FormatDateAndTimeMaximum(field.attribute.displayname, maxdate), Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic };

                            ph.Controls.Add(compval);

                            regCompval = new RegularExpressionValidator { ID = "comp" + field.attribute.attributeid + "_time", ControlToValidate = "txt" + field.attribute.attributeid + "_time", ValidationExpression = "^(([0-9])|([0-1][0-9])|([2][0-3])):(([0-9])|([0-5][0-9]))$", ErrorMessage = ValidatorMessages.FormatDateAndTime(field.attribute.displayname), Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic };

                            ph.Controls.Add(regCompval);

                            RequiredFieldValidator reqVal = new RequiredFieldValidator { ID = "reqDate" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic, ErrorMessage = ValidatorMessages.MandatoryDateFromDateTime(field.attribute.displayname), CssClass = "reqDateVal", Enabled = false };

                            ph.Controls.Add(reqVal);

                            reqVal = new RequiredFieldValidator { ID = "reqTime" + field.attribute.attributeid + "_time", ControlToValidate = "txt" + field.attribute.attributeid + "_time", Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic, ErrorMessage = ValidatorMessages.MandatoryTimeFromDateTime(field.attribute.displayname), CssClass = "reqTimeVal", Enabled = false };

                            ph.Controls.Add(reqVal);
                            break;

                        case AttributeFormat.TimeOnly:
                            regCompval = new RegularExpressionValidator { ID = "comp" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, ValidationExpression = "^(([0-9])|([0-1][0-9])|([2][0-3])):(([0-9])|([0-5][0-9]))$", ErrorMessage = ValidatorMessages.FormatTime(field.attribute.displayname), Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic };

                            ph.Controls.Add(regCompval);
                            break;
                    }

                    break;

                case FieldType.Contact:
                    var contactAttribute = (cContactAttribute) field.attribute;
                    if (contactAttribute.format == AttributeFormat.ContactEmail)
                    {
                        regCompval = new RegularExpressionValidator { ID = "comp" + field.attribute.attributeid, ControlToValidate = "txt" + field.attribute.attributeid, ValidationExpression = @"^\S+@\S+\.\S+$", ErrorMessage = ValidatorMessages.FormatContactEmail(field.attribute.displayname), Text = "*", ValidationGroup = validationGroup, Display = ValidatorDisplay.Dynamic };
                        ph.Controls.Add(regCompval);
                    }
                    break;
            }
        }

        private void lnkonetomany_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            aeentity pgEntity = (aeentity)lnk.Page;

            int recordid;
            int entityid, formid;
            int viewid;
            int tabid;

            string[] temp = lnk.CommandArgument.Split(',');
            string[] arrRelentityid = temp[0].Split('_');
            string[] arrRelformid = temp[1].Split('_');
            string[] arrRelrecordid = temp[2].Split('_');
            string[] arrReltabid = temp[3].Split('_');
            string[] arrRelviewid = temp[4].Split('_');

            if (arrReltabid.Length > 0)
            {
                HiddenField curTab = (HiddenField)pgEntity.hiddenCETab;
                TabContainer tabs = (TabContainer)pgEntity.FindControl("tabs" + arrRelformid[arrRelformid.Length - 1]);

                if (tabs != null)
                {
                    for (int i = 0; i < tabs.Tabs.Count; i++)
                    {
                        if (tabs.Tabs[i].ID == "tab" + arrReltabid[arrReltabid.Length - 1])
                        {
                            arrReltabid[arrReltabid.Length - 1] = tabs.Tabs[i].ID.Replace("tab", string.Empty);
                            curTab.Value = i.ToString();
                            break;
                        }
                    }
                }
            }

            string returnurl;
            List<sEntityBreadCrumb> crumbs = new List<sEntityBreadCrumb>();
            for (int i = 0; i < arrRelentityid.GetLength(0); i++)
            {
                sEntityBreadCrumb crumb = new sEntityBreadCrumb(Convert.ToInt32(arrRelentityid[i]),
                                                                Convert.ToInt32(arrRelformid[i]),
                                                                Convert.ToInt32(arrRelrecordid[i]),
                                                                Convert.ToInt32(arrReltabid[i]),
                                                                Convert.ToInt32(arrRelviewid[i]));
                crumbs.Add(crumb);
            }

            viewid = Convert.ToInt32(temp[5]);
            entityid = Convert.ToInt32(temp[6]);
            formid = Convert.ToInt32(temp[7]);
            tabid = Convert.ToInt32(temp[8]);

            if (crumbs[crumbs.Count - 1].RecordID == 0) //save base record
            {
                recordid = pgEntity.SaveCustomEntity(crumbs[crumbs.Count - 1].EntityID, crumbs[crumbs.Count - 1].FormID);
                sEntityBreadCrumb crumb = crumbs[crumbs.Count - 1];
                crumb.RecordID = recordid;
                crumbs[crumbs.Count - 1] = crumb;
            }
            else
            {
                recordid = pgEntity.SaveCustomEntity(crumbs[crumbs.Count - 1].EntityID, crumbs[crumbs.Count - 1].FormID);
            }

            if (recordid < 0)
            {
                string message = "Cannot save record as one of the field values already exists in another record.";
                string title = "Message from ";

                switch (oCurrentUser.CurrentActiveModule)
                {
                    case Modules.expenses:
                        title += "Expenses";
                        break;
                    case Modules.contracts:
                        title += "Framework";
                        break;
                    case Modules.SmartDiligence:
                        title += "Smart Diligence";
                        break;
                    case Modules.Greenlight:
                        title += "Greenlight";
                        break;
                    case Modules.CorporateDiligence:
                        title += "Corporate Diligence";
                        break;
                    case Modules.GreenlightWorkforce:
                        title += "Greenlight Workforce";
                        break;
                    default:
                        title += "Spend Management";
                        break;
                }

                var divTitle = (HtmlGenericControl)pgEntity.SaveErrorMessageModal.FindControl("divErrorTitle");
                divTitle.InnerText = title;

                switch (recordid)
                {
                    case (int)ReturnValues.InvalidFile:
                        message = "Cannot save record as one of the attachments is an invalid file.";
                        break;
                    case (int)ReturnValues.InvalidFileType:
                        message = "Cannot save record as one of the attachments has an unsupported file type.";
                        break;
                }

                var divError = (HtmlGenericControl)pgEntity.SaveErrorMessageModal.FindControl("divErrorText");
                divError.InnerText = message;
                pgEntity.SaveErrorMessageModal.Show();
            }
            else
            {
                returnurl = "~/shared/aeentity.aspx?";
                string entityurl = string.Empty;
                string formurl = string.Empty;
                string recordurl = string.Empty;
                string taburl = string.Empty;
                foreach (sEntityBreadCrumb c in crumbs)
                {
                    entityurl += "relentityid=" + c.EntityID + "&";
                    formurl += "relformid=" + c.FormID + "&";
                    recordurl += "relrecordid=" + c.RecordID + "&";
                    taburl += "reltabid=" + c.TabID + "&";
                    taburl += "relviewid=" + c.ViewID + "&";
                }

                returnurl += entityurl + formurl + recordurl + taburl + "viewid=" + viewid + "&entityid=" + entityid + "&formid=" + formid + "&tabid=" + tabid + "&attributeid=0&attributetext=";
                HttpContext.Current.Response.Redirect(returnurl, true);
            }
        }

        #endregion

        #region views

        /// <summary>
        /// generateOneToManyGrid: Outputs HTML for rendering 1:n relationship table
        /// </summary>
        /// <param name="onetomany">Relationship to compile table for</param>
        /// <param name="id">Custom Entity Id</param>
        /// <param name="viewid">Custom Entity View Id</param>
        /// <param name="tabid"></param>
        /// <param name="formid"></param>
        /// <param name="recid"></param>
        /// <param name="relentityids"></param>
        /// <param name="relformids"></param>
        /// <param name="relrecordids"></param>
        /// <param name="reltabids"></param>
        /// <returns>String of HTML data</returns>
        public string[] generateOneToManyGrid(cOneToManyRelationship onetomany, int id, int viewid, int tabid, int formid, int recid, string relentityids, string relformids, string relrecordids, string reltabids, string relviewids)
        {
            cCustomEntity entity = this.getEntityById(onetomany.entityid);
            cCustomEntityView view = entity.getViewById(onetomany.viewid);

            cCustomEntity systemviewentity = getEntityByTableId(onetomany.relatedtable.TableID);

            cTable display_table;
            cAttribute keyatt;
            cField keyfield;
            if (systemviewentity.table.TableID != entity.table.TableID)
            {
                // must have a filtered system view
                display_table = systemviewentity.table;
                keyatt = entity.getKeyField();

                keyfield = fields.GetBy(display_table.TableID, keyatt.attributename);
            }
            else
            {
                display_table = entity.table;
                keyatt = entity.getKeyField();
                keyfield = fields.GetFieldByID(keyatt.fieldid);
            }

            List<cNewGridColumn> columns = new List<cNewGridColumn>();

            List<cField> lstValueListFields = new List<cField>();

            var bContainsKeyField = false;
            var containsAttachmentFieldType = false;
            var attachmentColumn = string.Empty;
            foreach (cCustomEntityViewField viewField in view.fields.Values)
            {
                cField tmpField = viewField.Field;

                if (containsAttachmentFieldType && viewField.Field.FieldType == "AT")
                {
                    continue;
                }

                if (viewField.Field.FieldName == keyfield.FieldName && viewField.Field.TableID == keyfield.TableID && viewField.JoinVia == null)
                {
                    bContainsKeyField = true;
                }

                if (viewField.JoinVia == null)
                {
                    // presumably whatever this is doing doesn't need to be done if it's a via joined field
                    // either that or it'll also need to compare the JoinVia
                    // either way, the relatedtable probably isn't the one we need to compare so refactoring in here for now

                    tmpField = fields.GetBy(onetomany.relatedtable.TableID, viewField.Field.FieldName);

                    tmpField = (tmpField != null) ? tmpField : viewField.Field;

                    if (tmpField.GenList && tmpField.ListItems.Count == 0)
                    {
                        // n:1 field, so need related field for reporting
                        columns.Add(new cFieldColumn(viewField.Field, tmpField.GetLookupField()));
                    }
                    else
                    {
                        columns.Add(new cFieldColumn(tmpField));
                    }
                }
                else
                {
                    columns.Add(new cFieldColumn(viewField.Field, viewField.JoinVia));
                }

                if (tmpField.ValueList)
                {
                    lstValueListFields.Add(tmpField);
                }

                if (viewField.Field.FieldType == "CL")
                {
                    cGlobalCurrencies clsGCurrencies = new cGlobalCurrencies();
                    cCurrencies clsCurrencies = new cCurrencies(this.oCurrentUser.AccountID, oCurrentUser.CurrentSubAccountId);

                    foreach (cCurrency c in clsCurrencies.currencyList.Values)
                    {
                        ((cFieldColumn)columns[columns.Count - 1]).addValueListItem(c.currencyid, clsGCurrencies.getGlobalCurrencyById(c.globalcurrencyid).label);
                    }
                }

                //if (viewField.Field.FieldType == "AT" && containsAttachmentFieldType == false)
                //{
                // attachmentColumn = viewField.Field.FieldName;
                // containsAttachmentFieldType = true;
                // //Adds the file name of the attachment to the grid
                // var fileName = fields.GetFieldByID(new Guid("7186B7FB-487C-4D4D-A975-799FA423A86C"));
                // fileName.Description = viewField.Field.Description;
                // columns.Add(new cFieldColumn(fileName, viewField.Field.FieldName + "_Alias"));
                //}
            }

            if (entity.EnableCurrencies && entity.DefaultCurrencyID.HasValue)
            {
                cField greenLightCurrencyField = fields.GetFieldByID(entity.getGreenLightCurrencyAttribute().fieldid);
                columns.Insert(0, new cFieldColumn(greenLightCurrencyField));
            }

            if (bContainsKeyField == false)
            {
                columns.Add(new cFieldColumn(keyfield));
            }

            SerializableDictionary<string, object> gridInfo = new SerializableDictionary<string, object>();

            cGridNew clsgrid = new cGridNew(this.oCurrentUser.AccountID, oCurrentUser.EmployeeID, "grid" + entity.entityid + view.viewid + onetomany.attributeid, display_table, columns);

            if (entity.AudienceView != AudienceViewType.NoAudience)
            {
                SerializableDictionary<string, object> audienceStatus = GetAudienceRecords(entity.entityid, oCurrentUser.EmployeeID);
                gridInfo.Add("employeeid", oCurrentUser.EmployeeID);
                gridInfo.Add("entityid", entity.entityid);
                gridInfo.Add("keyfield", keyfield.FieldName);
                gridInfo.Add("gridid", clsgrid.GridID);

                clsgrid.HiddenRecords = (from x in audienceStatus.Values
                                         where ((cAudienceRecordStatus)x).Status == 0
                                         select ((cAudienceRecordStatus)x).RecordID).ToList();
            }

            clsgrid.InitialiseRowGridInfo = gridInfo;
            clsgrid.InitialiseRow += new cGridNew.InitialiseRowEvent(clsOTMgrid_InitialiseRow);
            clsgrid.ServiceClassForInitialiseRowEvent = "Spend_Management.cCustomEntities";
            clsgrid.ServiceClassMethodForInitialiseRowEvent = "clsOTMgrid_InitialiseRow";

            clsgrid.CssClass = "datatbl";
            clsgrid.EmptyText = "There are currently no " + entity.pluralname.ToLower() + " defined.";

            if (entity.EnableCurrencies && entity.DefaultCurrencyID.HasValue)
            {
                clsgrid.CurrencyColumnName = "GreenLightCurrency";
                clsgrid.getColumnByName("GreenLightCurrency").hidden = true;
                clsgrid.CurrencyId = entity.DefaultCurrencyID.Value;
            }

            clsgrid.KeyField = keyfield.FieldName;

            #region Set the sorting of the grid

            cEmployees clsEmployees = new cEmployees(this.oCurrentUser.AccountID);
            cNewGridSort employeeSort = oCurrentUser.Employee.GetNewGridSortOrders().GetBy("grid" + entity.entityid + view.viewid + onetomany.attributeid);

            // if default sort set for view, use this. Need to let this get overridden by user default though
            if (employeeSort == null && view.SortColumn.FieldID != Guid.Empty)
            {
                cNewGridColumn SortCol = null;

                SortCol = clsgrid.getColumnByID(view.SortColumn.FieldID, (view.SortColumn.JoinVia != null ? view.SortColumn.JoinVia.JoinViaID : 0));

                if (SortCol != null)
                {
                    clsgrid.SortedColumn = SortCol;
                    clsgrid.SortDirection = view.SortColumn.SortDirection;
                }
            }

            #endregion

            clsgrid.addFilter(fields.GetFieldByID(onetomany.fieldid), ConditionType.Equals, new object[] { recid }, new object[] { }, ConditionJoiner.None);

            if (bContainsKeyField == false)
            {
                clsgrid.getColumnByName(clsgrid.KeyField).hidden = true;
            }

            #region Create Value list items for grid

            if (lstValueListFields.Count > 0)
            {
                SortedList<int, SortedList<int, cListAttributeElement>> lstitems = getAttributeListItems();

                foreach (cField fld in lstValueListFields)
                {
                    cAttribute tempatt = null;

                    foreach (cAttribute att in entity.attributes.Values)
                    {
                        if (att.fieldid == fld.FieldID)
                        {
                            tempatt = att;
                            break;
                        }
                    }

                    if (tempatt == null)
                    {
                        continue;
                    }

                    SortedList<int, cListAttributeElement> items = null;

                    lstitems.TryGetValue(tempatt.attributeid, out items);

                    if (items != null)
                    {
                        foreach (cListAttributeElement element in items.Values)
                        {
                            ((cFieldColumn)clsgrid.getColumnByName(fld.FieldName)).addValueListItem(element.elementValue, element.elementText);
                        }
                    }
                }
            }

            #endregion

            if (view.allowdelete)
            {
                clsgrid.enabledeleting = oCurrentUser.CheckAccessRole(AccessRoleType.Delete, CustomEntityElementType.View, view.entityid, view.viewid, false);
                clsgrid.deletelink = "javascript:deleteRecord(" + view.entityid + ",{" + keyfield.FieldName + "}, " + view.viewid + "," + onetomany.attributeid.ToString() + ")";
            }           

            if (view.allowedit)
            {
                if (view.DefaultEditForm != null && view.DefaultEditForm.fields.Count > 0)
                {
                    clsgrid.enableupdating = oCurrentUser.CheckAccessRole(AccessRoleType.Edit, CustomEntityElementType.View, view.entityid, view.viewid, false);

                    if (clsgrid.enableupdating)
                    {
                        string returnurl = "aeentity.aspx?";
                        string entityurl = string.Empty;
                        string formurl = string.Empty;
                        string recordurl = string.Empty;
                        string taburl = string.Empty;
                        string[] relentities = relentityids.Split('_');
                        string[] relforms = relformids.Split('_');
                        string[] relrecs = relrecordids.Split('_');
                        string[] reltabs = reltabids.Split('_');
                        string[] relviews = relviewids.Split('_');

                        for (int i = 0; i < relentities.Length; i++)
                        {
                            entityurl += "relentityid=" + relentities[i] + "&";
                            formurl += "relformid=" + relforms[i] + "&";
                            recordurl += "relrecordid=" + relrecs[i] + "&";
                            taburl += "reltabid=" + reltabs[i] + "&";
                            taburl += "relviewid=" + relviews[i] + "&";
                        }

                        returnurl += entityurl + formurl + recordurl + taburl + "viewid=" + view.viewid + "&entityid=" + view.entityid + "&formid=" + view.DefaultEditForm.formid + "&tabid=0&id={" + keyfield.FieldName + "}";

                        clsgrid.editlink = returnurl;
                    }
                }
                else
                {
                    clsgrid.enableupdating = false;
                }
            }

            if (view.filters != null)
            {
                foreach (KeyValuePair<byte, FieldFilter> kvp in view.filters)
                {
                    FieldFilter curFilter = kvp.Value;
                    FieldFilters.FieldFilterValues filterValues = FieldFilters.GetFilterValuesFromFieldFilter(curFilter, oCurrentUser);

                    clsgrid.addFilter(curFilter.Field, filterValues.conditionType, filterValues.valueOne,
                                      filterValues.valueTwo, ConditionJoiner.And, curFilter.JoinVia);
                }
            }

            //if (containsAttachmentFieldType)                   
            //{
            //    var fileName = fields.GetFieldByID(new Guid("7186B7FB-487C-4D4D-A975-799FA423A86C"));
            //    clsgrid.addTwoStateEventColumn("viewattachment", (cFieldColumn)clsgrid.getColumnByName(fileName.FieldName), null, null, "/shared/images/icons/16/plain/zoom_in.png","javascript:viewFieldLevelAttachment('{"+ attachmentColumn + "}');", "show image","","","","","", false);
            //    clsgrid.getColumnByName(attachmentColumn).hidden = true;
            //}

            List<string> retVals = new List<string>();
            retVals.Add(clsgrid.GridID);
            retVals.AddRange(clsgrid.generateGrid());
            return retVals.ToArray();
        }


        private void clsOTMgrid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
        {
            if (row.getCellByID("formfieldcount") != null)
            {
                if (Convert.ToUInt32(row.getCellByID("formfieldcount").Value) == 0)
                {
                    // no form fields exist, so prevent access by hiding the edit icon
                    row.enableupdating = false;
                }
            }

            if (gridInfo.ContainsKey("keyfield") && gridInfo.ContainsKey("gridid"))
            {
                oCurrentUser = oCurrentUser ?? cMisc.GetCurrentUser();
                nAccountid = oCurrentUser.AccountID;
                InitialiseData();

                SerializableDictionary<string, object> audienceStatus = GetAudienceRecords((int)gridInfo["entityid"], (int)gridInfo["employeeid"]);
                cGridNew.InitialiseRowAudienceCheck(ref row, gridInfo["gridid"].ToString(), gridInfo["keyfield"].ToString(), audienceStatus);
            }
        }

        /// <summary>
        /// getViewFields: Returns a List containing List of fields to be used in view
        /// </summary>
        /// <returns>SortedList&lt;int, SortedList&lt;byte, cField&gt;&gt; collection</returns>
        private SortedList<int, SortedList<byte, cCustomEntityViewField>> getViewFields(IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            SortedList<int, SortedList<byte, cCustomEntityViewField>> lstfields = new SortedList<int, SortedList<byte, cCustomEntityViewField>>();
            SortedList<byte, cCustomEntityViewField> entityViewFields;
            cField field;

            int viewID;
            Guid fieldID;
            byte order;
            int? joinViaID;

            cCustomEntityViewField cevField;
            JoinVias clsJoinVia = new JoinVias(this.oCurrentUser);
            JoinVia joinVia;

            IDataReader reader;

            //                         0        1       2          3
            string strsql = "select viewid, fieldid, [order], joinViaID from customEntityViewFields";
            using (reader = expdata.GetReader(strsql))
            {
                int viewid_ord = reader.GetOrdinal("viewid");
                int fieldid_ord = reader.GetOrdinal("fieldid");
                int order_ord = reader.GetOrdinal("order");
                int joinViaID_ord = reader.GetOrdinal("joinViaID");

                while (reader.Read())
                {
                    viewID = reader.GetInt32(viewid_ord);
                    fieldID = reader.GetGuid(fieldid_ord);
                    order = reader.GetByte(order_ord);
                    if (reader.IsDBNull(joinViaID_ord))
                    {
                        joinViaID = null;
                    }
                    else
                    {
                        joinViaID = reader.GetInt32(joinViaID_ord);
                    }

                    field = fields.GetFieldByID(fieldID);
                    lstfields.TryGetValue(viewID, out entityViewFields);
                    if (entityViewFields == null)
                    {
                        entityViewFields = new SortedList<byte, cCustomEntityViewField>();
                        lstfields.Add(viewID, entityViewFields);
                    }

                    if (joinViaID.HasValue)
                    {
                        joinVia = clsJoinVia.GetJoinViaByID(joinViaID.Value);
                        cevField = new cCustomEntityViewField(field, joinVia);
                    }
                    else
                    {
                        cevField = new cCustomEntityViewField(field);
                    }

                    entityViewFields.Add(order, cevField);
                }

                reader.Close();
            }

            return lstfields;
        }

        /// <summary>
        /// getViewFilters: Get any filters required for custom entity view
        /// </summary>
        /// <returns>A sorted list containing sorted list of entity view filters</returns>
        private SortedList<int, SortedList<byte, FieldFilter>> getViewFilters(IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            SortedList<int, SortedList<byte, FieldFilter>> lst = new SortedList<int, SortedList<byte, FieldFilter>>();
            SortedList<byte, FieldFilter> filters;

            Guid fieldid;
            ConditionType condition;
            int viewid;
            string value;
            string valueTwo;
            byte order;
            int? joinViaID;

            cField field;

            JoinVias clsJoinVia = new JoinVias(this.oCurrentUser);
            JoinVia joinVia = null;

            // :                      0         1        2        3       4        5
            string strsql = "select viewid, fieldid, condition, value, valueTwo, [order], joinViaID from dbo.fieldFilters where viewid is not null";

            using (IDataReader reader = expdata.GetReader(strsql))
            {
                int viewid_ord = reader.GetOrdinal("viewid");
                int fieldid_ord = reader.GetOrdinal("fieldid");
                int condition_ord = reader.GetOrdinal("condition");
                int value_ord = reader.GetOrdinal("value");
                int valuetwo_ord = reader.GetOrdinal("valueTwo");
                int order_ord = reader.GetOrdinal("order");
                int joinViaID_ord = reader.GetOrdinal("joinViaID");

                while (reader.Read())
                {
                    joinVia = null;

                    viewid = reader.GetInt32(viewid_ord); //reader.GetOrdinal("viewid"));
                    fieldid = reader.GetGuid(fieldid_ord); //reader.GetOrdinal("fieldid"));
                    condition = (ConditionType)reader.GetByte(condition_ord); //reader.GetOrdinal("condition"));
                    value = reader.GetString(value_ord); //reader.GetOrdinal("value"));
                    valueTwo = reader.GetString(valuetwo_ord); //reader.GetOrdinal("value"));
                    order = reader.GetByte(order_ord); //reader.GetOrdinal("order"));
                    if (reader.IsDBNull(joinViaID_ord))
                    {
                        joinViaID = null;
                    }
                    else
                    {
                        joinViaID = reader.GetInt32(joinViaID_ord);
                    }

                    field = fields.GetFieldByID(fieldid);

                    lst.TryGetValue(viewid, out filters);
                    if (filters == null)
                    {
                        filters = new SortedList<byte, FieldFilter>();
                        lst.Add(viewid, filters);
                    }

                    if (joinViaID.HasValue)
                    {
                        joinVia = clsJoinVia.GetJoinViaByID(joinViaID.Value);
                    }

                    filters.Add(order, new FieldFilter(field, condition, value, valueTwo, order, joinVia));
                }

                reader.Close();
            }

            return lst;
        }

        private SortedList<int, SortedList<int, FieldFilter>> getManyToOneAttributeFilters(IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            SortedList<int, SortedList<int, FieldFilter>> lst = new SortedList<int, SortedList<int, FieldFilter>>();
            SortedList<int, FieldFilter> filters;

            Guid fieldid;
            ConditionType condition;
            int formid;
            int attributeid;
            string value;
            string valueTwo;
            byte order;
            int? joinViaID;
            int viewFilterID;
            bool isParentFilter;

            cField field;

            JoinVias clsJoinVia = new JoinVias(this.oCurrentUser);
            JoinVia joinVia = null;

            const string strsql = "select formid, attributeid, fieldid, condition, value, valueTwo, [order], joinViaID, viewFilterID, isParentFilter from dbo.fieldFilters where attributeid is not null";

            using (IDataReader reader = expdata.GetReader(strsql))
            {
                int formid_ord = reader.GetOrdinal("formid");
                int attributeid_ord = reader.GetOrdinal("attributeid");
                int fieldid_ord = reader.GetOrdinal("fieldid");
                int condition_ord = reader.GetOrdinal("condition");
                int value_ord = reader.GetOrdinal("value");
                int valuetwo_ord = reader.GetOrdinal("valueTwo");
                int order_ord = reader.GetOrdinal("order");
                int joinViaID_ord = reader.GetOrdinal("joinViaID");
                int viewFilterID_ord = reader.GetOrdinal("viewFilterID");
                int isParentFilterOrd = reader.GetOrdinal("isParentFilter");

                while (reader.Read())
                {
                    joinVia = null;

                    attributeid = reader.GetInt32(attributeid_ord);
                    if (reader.IsDBNull(formid_ord))
                    {
                        formid = 0;
                    }
                    else
                    {
                        formid = reader.GetInt32(formid_ord);
                    }
                    fieldid = reader.GetGuid(fieldid_ord);
                    condition = (ConditionType)reader.GetByte(condition_ord);
                    value = reader.GetString(value_ord);
                    valueTwo = reader.GetString(valuetwo_ord);
                    order = reader.GetByte(order_ord);
                    viewFilterID = reader.GetInt32(viewFilterID_ord);
                    isParentFilter = reader.GetBoolean(isParentFilterOrd);
                    if (reader.IsDBNull(joinViaID_ord))
                    {
                        joinViaID = null;
                    }
                    else
                    {
                        joinViaID = reader.GetInt32(joinViaID_ord);
                    }

                    field = fields.GetFieldByID(fieldid);

                    lst.TryGetValue(attributeid, out filters);
                    if (filters == null)
                    {
                        filters = new SortedList<int, FieldFilter>();
                        lst.Add(attributeid, filters);
                    }

                    if (joinViaID.HasValue)
                    {
                        joinVia = clsJoinVia.GetJoinViaByID(joinViaID.Value);
                    }

                    filters.Add(viewFilterID, new FieldFilter(field, condition, value, valueTwo, order, joinVia, formid, isParentFilter: isParentFilter));
                }

                reader.Close();
            }

            return lst;
        }

        /// <summary>
        /// Get the form selections for the views
        /// </summary>
        /// <returns>A list of the form selection mappings</returns>
        private List<FormSelectionMapping> GetFormSelectionMappings(IDBConnection connection = null)
        {
            List<FormSelectionMapping> mappings = new List<FormSelectionMapping>();

            using (IDBConnection db = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                using (IDataReader reader = db.GetReader("SELECT [viewFormSelectionMappingId], [isAdd], [viewId], [formId], [listValue], [textValue] FROM [dbo].[customEntityViewFormSelectionMappings]"))
                {
                    int viewFormSelectionMappingIdOrd = reader.GetOrdinal("viewFormSelectionMappingId");
                    int isAddOrd = reader.GetOrdinal("isAdd");
                    int viewIdOrd = reader.GetOrdinal("viewId");
                    int formIdOrd = reader.GetOrdinal("formId");
                    int listValueOrd = reader.GetOrdinal("listValue");
                    int textValueOrd = reader.GetOrdinal("textValue");

                    while (reader.Read())
                    {
                        int viewFormSelectionMappingId = reader.GetInt32(viewFormSelectionMappingIdOrd);
                        bool isAdd = reader.GetBoolean(isAddOrd);
                        int viewId = reader.GetInt32(viewIdOrd);
                        int formId = reader.GetInt32(formIdOrd);
                        int listValue = reader.IsDBNull(listValueOrd) ? -1 : reader.GetInt32(listValueOrd);
                        string textValue = reader.IsDBNull(textValueOrd) ? null : reader.GetString(textValueOrd);

                        mappings.Add(new FormSelectionMapping
                        {
                            FormSelectionMappingId = viewFormSelectionMappingId,
                            IsAdd = isAdd,
                            ViewId = viewId,
                            FormId = formId,
                            ListValue = listValue,
                            TextValue = textValue
                        });
                    }
                }
            }

            return mappings;
        }

        /// <summary>
        /// getViews: Gets view collections for provided custom entity forms
        /// </summary>
        /// <param name="lstforms">Collection of custom entity forms to retrieve views for</param>
        /// <returns></returns>
        private SortedList<int, SortedList<int, cCustomEntityView>> getViews(SortedList<int, SortedList<int, cCustomEntityForm>> lstforms, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            SortedList<int, SortedList<int, cCustomEntityView>> lstviews = new SortedList<int, SortedList<int, cCustomEntityView>>();
            SortedList<int, cCustomEntityView> views;
            int viewid, entityid;
            int addformid, editformid;
            string viewname, description, _menuDescription;
            string MenuIcon = null;
            int? menuid;
            List<int> menuDisabledModuleIds;
            bool builtIn, allowadd, allowedit, allowdelete, allowapproval, allowArchive,showRecordCount;          
            cCustomEntityForm addform, editform;
            DateTime createdon;
            int createdby;
            DateTime? modifiedon;
            int? modifiedby;
            Guid SortColumn = Guid.Empty;
            SortDirection SortOrderDirection;
            int sortColumnJoinViaID = 0;
            SortedList<int, cCustomEntityForm> forms;
            SortedList<int, SortedList<byte, cCustomEntityViewField>> lstfields = getViewFields();
            SortedList<int, SortedList<byte, FieldFilter>> lstfilters = getViewFilters();
            SortedList<byte, cCustomEntityViewField> fields;
            SortedList<byte, FieldFilter> filters;
            List<FormSelectionMapping> formSelectionMappings = this.GetFormSelectionMappings();

            GreenLightSortColumn oSortColumn = null;
            JoinVias joinVias = new JoinVias(this.oCurrentUser);
            JoinVia joinVia = new JoinVia(0, string.Empty, Guid.Empty);
            string strsql = "select viewid, entityid, view_name, description, menuid, MenuDescription,ShowRecordCount ,createdon, createdby, modifiedon, modifiedby, allowadd, add_formid, allowedit, edit_formid, allowdelete, allowapproval, SortColumn, SortOrder, SortColumnJoinViaID, MenuIcon, BuiltIn, SystemCustomEntityViewId,allowarchive from dbo.customEntityViews";

            using (IDataReader reader = expdata.GetReader(strsql))
            {
                #region ordinals

                int viewid_ord = reader.GetOrdinal("viewid");
                int entityid_ord = reader.GetOrdinal("entityid");
                int view_name_ord = reader.GetOrdinal("view_name");
                int description_ord = reader.GetOrdinal("description");
                int menuid_ord = reader.GetOrdinal("menuid");
                int menudescription_ord = reader.GetOrdinal("MenuDescription");
                int showrecordcount_ord = reader.GetOrdinal("ShowRecordCount");
                int createdon_ord = reader.GetOrdinal("createdon");
                int createdby_ord = reader.GetOrdinal("createdby");
                int modifiedon_ord = reader.GetOrdinal("modifiedon");
                int modifiedby_ord = reader.GetOrdinal("modifiedby");
                int allowadd_ord = reader.GetOrdinal("allowadd");
                int add_formid_ord = reader.GetOrdinal("add_formid");
                int allowedit_ord = reader.GetOrdinal("allowedit");
                int edit_formid_ord = reader.GetOrdinal("edit_formid");
                int allowdelete_ord = reader.GetOrdinal("allowdelete");
                int allowapproval_ord = reader.GetOrdinal("allowapproval");
                int allowarchive_ord = reader.GetOrdinal("allowarchive"); 
                int SortColumn_ord = reader.GetOrdinal("SortColumn");
                int SortOrder_ord = reader.GetOrdinal("SortOrder");
                int SortColumnJoinViaID_ord = reader.GetOrdinal("SortColumnJoinViaID");
                int MenuIcon_ord = reader.GetOrdinal("MenuIcon");
                int BuiltIn_ord = reader.GetOrdinal("BuiltIn");
                var SystemCustomEntityViewId_Ordinal = reader.GetOrdinal("SystemCustomEntityViewId");

                #endregion ordinals

                while (reader.Read())
                {
                    viewid = reader.GetInt32(viewid_ord);
                    entityid = reader.GetInt32(entityid_ord);
                    viewname = reader.GetString(view_name_ord);
                    if (reader.IsDBNull(description_ord))
                    {
                        description = string.Empty;
                    }
                    else
                    {
                        description = reader.GetString(description_ord);
                    }

                    builtIn = reader.GetBoolean(BuiltIn_ord);
                    Guid? systemCustomEntityViewId = reader.IsDBNull(SystemCustomEntityViewId_Ordinal) ? (Guid?)null : reader.GetGuid(SystemCustomEntityViewId_Ordinal);
                    menuid = null;

                    if (!reader.IsDBNull(menuid_ord))
                    {
                        menuid = reader.GetInt32(menuid_ord);
                    }

                    if (reader.IsDBNull(menudescription_ord))
                    {
                        _menuDescription = string.Empty;
                    }
                    else
                    {
                        _menuDescription = reader.GetString(menudescription_ord);
                    }
                    showRecordCount = reader.GetBoolean(showrecordcount_ord);
                    menuDisabledModuleIds = new DisabledModuleMenuViews(oCurrentUser.AccountID, (int)oCurrentUser.CurrentActiveModule).GetDisabledModuleIds(viewid);

                    createdon = reader.GetDateTime(createdon_ord);
                    createdby = reader.GetInt32(createdby_ord);
                    modifiedon = null;
                    if (!reader.IsDBNull(modifiedon_ord))
                    {
                        modifiedon = reader.GetDateTime(modifiedon_ord);
                    }

                    modifiedby = null;
                    if (!reader.IsDBNull(modifiedby_ord))
                    {
                        modifiedby = reader.GetInt32(modifiedby_ord);
                    }

                    allowadd = reader.GetBoolean(allowadd_ord);
                    addform = null;
                    if (!reader.IsDBNull(add_formid_ord))
                    {
                        addformid = reader.GetInt32(add_formid_ord);
                        lstforms.TryGetValue(entityid, out forms);
                        if (forms != null)
                        {
                            forms.TryGetValue(addformid, out addform);
                        }
                    }

                    allowedit = reader.GetBoolean(allowedit_ord);
                    editform = null;
                    if (!reader.IsDBNull(edit_formid_ord))
                    {
                        editformid = reader.GetInt32(edit_formid_ord);
                        lstforms.TryGetValue(entityid, out forms);
                        if (forms != null)
                        {
                            forms.TryGetValue(editformid, out editform);
                        }
                    }

                    allowdelete = reader.GetBoolean(allowdelete_ord);
                    allowapproval = reader.GetBoolean(allowapproval_ord);
                    allowArchive = reader.GetBoolean(allowarchive_ord);
                    if (!reader.IsDBNull(SortColumn_ord))
                    {
                        SortColumn = reader.GetGuid(SortColumn_ord);
                    }
                    else
                    {
                        SortColumn = Guid.Empty;
                    }

                    SortOrderDirection = (SortDirection)reader.GetByte(SortOrder_ord);

                    if (!reader.IsDBNull(SortColumnJoinViaID_ord))
                    {
                        sortColumnJoinViaID = reader.GetInt32(SortColumnJoinViaID_ord);
                        joinVia = joinVias.GetJoinViaByID(sortColumnJoinViaID);
                    }
                    else
                    {
                        joinVia = null;
                    }

                    oSortColumn = new GreenLightSortColumn(SortColumn, SortOrderDirection, joinVia);

                    if (!reader.IsDBNull(MenuIcon_ord))
                    {
                        MenuIcon = reader.GetString(MenuIcon_ord);
                    }

                    lstviews.TryGetValue(entityid, out views);
                    if (views == null)
                    {
                        views = new SortedList<int, cCustomEntityView>();
                        lstviews.Add(entityid, views);
                    }

                    lstfields.TryGetValue(viewid, out fields);
                    lstfilters.TryGetValue(viewid, out filters);
                    List<FormSelectionMapping> addMappings = formSelectionMappings.Where(x => x.ViewId == viewid && x.IsAdd).ToList();
                    List<FormSelectionMapping> editMappings = formSelectionMappings.Where(x => x.ViewId == viewid && !x.IsAdd).ToList();

                    views.Add(viewid, new cCustomEntityView(viewid, entityid, viewname, description, builtIn, systemCustomEntityViewId, createdon, createdby, modifiedon, modifiedby, menuid, _menuDescription, showRecordCount, fields, allowadd, addform, allowedit, editform, allowdelete, filters, allowapproval, allowArchive, oSortColumn, MenuIcon, addMappings, editMappings, menuDisabledModuleIds));
                }

                reader.Close();
            }

            return lstviews;
        }

        /// <summary>
        /// getViewsByMenuId: Gets any entity views that are tagged to a particular menu option
        /// </summary>
        /// <param name="id">ID of menu option to retrieve views for</param>
        /// <param name="forMobile">Optionally only return views that contain at least one field (attribute) that can be displayed on a mobile device.</param>
        /// <returns></returns>
        public List<cCustomEntityView> getViewsByMenuId(int id, bool forMobile = false)
        {
            List<cCustomEntityView> views = new List<cCustomEntityView>();
            foreach (cCustomEntity entity in lst.Values)
            {
                if (!entity.IsSystemView)
                {
                    List<cCustomEntityView> temp = entity.getViewsByMenuId(id, forMobile);
                    views.AddRange(temp);
                }
            }

            return views;
        }

        /// <summary>
        /// Gets the view count for the menu id.
        /// </summary>
        /// <param name="menuId">
        /// The menuId.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetViewCountForMenuId(int menuId)
        {
            return this.lst.Values.Where(entity => !entity.IsSystemView).Sum(entity => entity.GetViewCountForMenuId(menuId));
        }

        //TODO: Move this out of cCustomEntities and into a better place

        /// <summary>
        /// saveForm: Saves a custom entity form definition
        /// </summary>
        /// <param name="entityid">Custom entity ID to associate the form to</param>
        /// <param name="form">Custom Entity Form entity to save</param>
        /// <param name="formNameForCopy">If specified, a copy of the form will be created using this form name</param>
        /// <returns>Database ID of the form saved</returns>
        public int saveForm(int entityid, cCustomEntityForm form, string formNameForCopy = "", IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            int formid;
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@formid", formNameForCopy == string.Empty ? form.formid : 0);
            expdata.sqlexecute.Parameters.AddWithValue("@entityid", entityid);
            expdata.sqlexecute.Parameters.AddWithValue("@formname", formNameForCopy == string.Empty ? form.formname : formNameForCopy);

            if (form.description == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@description", form.description);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@showSave", form.ShowSaveButton);
            if (form.SaveButtonText == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@saveButtonText", String.Empty);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@saveButtonText", form.SaveButtonText);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@showSaveAndDuplicate", form.ShowSaveAndDuplicate);
            if (form.SaveAndDuplicateButtonText == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@saveAndDuplicateButtonText", String.Empty);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@saveAndDuplicateButtonText", form.SaveAndDuplicateButtonText);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@showSaveAndStay", form.ShowSaveAndStayButton);
            if (form.SaveAndStayButtonText == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@saveAndStayButtonText", String.Empty);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@saveAndStayButtonText", form.SaveAndStayButtonText);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@showSaveAndNew", form.ShowSaveAndNew);
            expdata.sqlexecute.Parameters.AddWithValue("@saveAndNewButtonText", form.SaveAndNewButtonText);

            expdata.sqlexecute.Parameters.AddWithValue("@showCancel", form.ShowCancelButton);
            if (form.CancelButtonText == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@cancelButtonText", String.Empty);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@cancelButtonText", form.CancelButtonText);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@showSubMenus", form.ShowSubMenus);
            expdata.sqlexecute.Parameters.AddWithValue("@showBreadcrumbs", form.ShowBreadCrumbs);
            if (form.modifiedby == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", form.createdon);
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", form.createdby);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", form.modifiedon);
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", form.modifiedby);
            }

            if (this.oCurrentUser.Delegate == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
            }

            bool checkDefaults = form.fields != null && form.fields.Values.Any(
                field => string.IsNullOrEmpty(field.DefaultValue) == false);

            expdata.sqlexecute.Parameters.AddWithValue("@checkDefaultVales", checkDefaults);
            expdata.sqlexecute.Parameters.AddWithValue("@hideTorch", form.HideTorch);
            expdata.sqlexecute.Parameters.AddWithValue("@hideAttachments", form.HideAttachments);
            expdata.sqlexecute.Parameters.AddWithValue("@hideAudiences", form.HideAudiences);
            expdata.sqlexecute.Parameters.AddWithValue("@builtIn", form.BuiltIn);

            if (form.SystemCustomEntityFormId.HasValue)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@SystemCustomEntityFormId", form.SystemCustomEntityFormId.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@SystemCustomEntityFormId", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveCustomEntityForm");
            formid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            if (formid == -1)
            {
                return -1;
            }

            int delegateid = 0;
            if (this.oCurrentUser.Delegate != null)
            {
                delegateid = oCurrentUser.Delegate.EmployeeID;
            }

            expdata.sqlexecute.Parameters.AddWithValue("@formid", formid);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateid);
            expdata.ExecuteProc("deleteCustomEntityFormDesign");
            //Tabs
            saveFormTabs(formid, form);
            saveFormSections(formid, form);
            saveFormFields(formid, form);

            var lastDatabaseUpdate = expdata.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
            refreshCache(lastDatabaseUpdate);

            return formid;
        }

        /// <summary>
        /// saveFormFields: Save fields collection for a give custom entity form
        /// </summary>
        /// <param name="formid">Form ID to associate fields to</param>
        /// <param name="form">Custom Entity Form definition entity</param>
        private void saveFormFields(int formid, cCustomEntityForm form, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            if (form.fields != null)
            {
                foreach (cCustomEntityFormField field in form.fields.Values)
                {
                    expdata.sqlexecute.Parameters.Clear();
                    expdata.sqlexecute.Parameters.AddWithValue("@formid", formid);
                    expdata.sqlexecute.Parameters.AddWithValue("@attributeid", field.attribute.attributeid);
                    expdata.sqlexecute.Parameters.AddWithValue("@sectionid", field.section.sectionid);
                    expdata.sqlexecute.Parameters.AddWithValue("@readonly", field.isReadOnly);
                    expdata.sqlexecute.Parameters.AddWithValue("@row", field.row);
                    expdata.sqlexecute.Parameters.AddWithValue("@column", field.column);

                    if (string.IsNullOrWhiteSpace(field.labelText))
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@labeltext", DBNull.Value);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@labeltext", field.labelText);
                    }

                    if (string.IsNullOrWhiteSpace(field.DefaultValue))
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@defaultValue", DBNull.Value);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@defaultValue", field.DefaultValue);
                    }

                    if (field.IsMandatory.HasValue)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@isMandatory", field.IsMandatory);
                    }

                    expdata.ExecuteProc("saveCustomEntityFormField");
                    expdata.sqlexecute.Parameters.Clear();

                    expdata.sqlexecute.Parameters.AddWithValue("@formid", formid);
                    expdata.sqlexecute.Parameters.AddWithValue("@attributeid", field.attribute.attributeid);
                    expdata.ExecuteProc("UpdateFormIdForManyToOneAttributeFieldFilter");
                    expdata.sqlexecute.Parameters.Clear();

                    var lastDatabaseUpdate = expdata.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
                    this.refreshCache(lastDatabaseUpdate);
                }
            }
        }

        /// <summary>
        /// saveFormTabs: Save Tab definitions against a particular custom entity form
        /// </summary>
        /// <param name="formid">Form ID to associate tabs to</param>
        /// <param name="form">Custom Entity Form definition entity</param>
        private void saveFormTabs(int formid, cCustomEntityForm form, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            int tabid;
            if (form.tabs != null)
            {
                foreach (cCustomEntityFormTab tab in form.tabs.Values)
                {
                    expdata.sqlexecute.Parameters.Clear();
                    expdata.sqlexecute.Parameters.AddWithValue("@formid", formid);
                    expdata.sqlexecute.Parameters.AddWithValue("@header", tab.headercaption);
                    expdata.sqlexecute.Parameters.AddWithValue("@order", tab.order);
                    expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                    expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                    expdata.ExecuteProc("saveCustomEntityFormTab");
                    tabid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                    expdata.sqlexecute.Parameters.Clear();
                    tab.tabid = tabid;
                }
            }
        }

        /// <summary>
        /// saveFormSections: Save section definitions against a particular custom entity form
        /// </summary>
        /// <param name="formid">Form ID to associate sections to</param>
        /// <param name="form">Custom Entity Form definition entity</param>
        private void saveFormSections(int formid, cCustomEntityForm form, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            int sectionid;
            if (form.sections != null)
            {
                foreach (cCustomEntityFormSection section in form.sections.Values)
                {
                    expdata.sqlexecute.Parameters.Clear();
                    expdata.sqlexecute.Parameters.AddWithValue("@formid", formid);
                    expdata.sqlexecute.Parameters.AddWithValue("@header", section.headercaption);
                    expdata.sqlexecute.Parameters.AddWithValue("@order", section.order);
                    expdata.sqlexecute.Parameters.AddWithValue("@tabid", section.tab.tabid);
                    expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                    expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                    expdata.ExecuteProc("saveCustomEntityFormSection");
                    sectionid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                    expdata.sqlexecute.Parameters.Clear();
                    section.sectionid = sectionid;
                }
            }
        }

        /// <summary>
        /// Save a GreenLight View
        /// </summary>
        /// <param name="accountid">Account ID</param>
        /// <param name="employeeid">Employee ID</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="viewid">View ID</param>
        /// <param name="viewname">View name</param>
        /// <param name="description">Descriptin</param>
        /// <param name="menuid">Menu ID</param>
        /// <param name="oJsonFields">View fields</param>
        /// <param name="addformid">Add Form</param>
        /// <param name="editformid">Edit Form</param>
        /// <param name="allowdelete">Allow delete</param>
        /// <param name="allowapproval">Allow approval</param>
        /// <param name="oJsonFilters">View filters</param>
        /// <param name="oSortColumn">Sort column</param>
        /// <param name="sortOrderDirection">Sort direction</param>
        /// <param name="menuIconName">Menu Icon</param>
        /// <param name="addMappings">form selection mappings for adding a greenlight</param>
        /// <param name="editMappings">form selection mappings for editing a greenlight</param>
        /// <returns>An int, indicating the success or failure of the View save</returns>
        public int SaveView(int accountid, int employeeid, int entityid, int viewid, string viewname, string description, bool builtIn, int menuid, string menuDescription, bool showRecordCount, JavascriptTreeData oJsonFields, int addformid, int editformid, bool allowdelete, bool allowapproval,bool allowarchive, JavascriptTreeData oJsonFilters, jsGreenLightViewSortColumn oSortColumn, SortDirection sortOrderDirection, string menuIconName, List<FormSelectionMapping> addMappings, List<FormSelectionMapping> editMappings, List<int> menuDisabledModuleIds)
       {
            bool allowadd = false;
            bool allowedit = false;           
            DateTime? modifiedon = null;
            int? modifiedby = null;
            DateTime createdon = DateTime.Now;
            int createdby = employeeid;
            bool currentBuiltIn = false;
            Guid? systemCustomEntityViewId = null;
            cCustomEntityForm addform, editform;
            string menuIcon = null;
            Guid _sortColumnID = Guid.Empty;
            string _sortCrumbs = string.Empty;
            SortedList<int, JoinViaPart> _joinViaList = new SortedList<int, JoinViaPart>();
            int _joinViaID = 0;
            GreenLightSortColumn sortColumn = new GreenLightSortColumn(Guid.Empty, 0, null);

            if (oSortColumn != null && oSortColumn.FieldID != Guid.Empty)
            {
                _sortColumnID = oSortColumn.FieldID;
                _joinViaID = oSortColumn.JoinViaID;
                _sortCrumbs = oSortColumn.JoinViaCrumbs;

                if (_joinViaID < 1)
                {
                    _joinViaList = JoinVias.JoinViaPartsFromCompositeGuid(oSortColumn.JoinViaPath);
                    _joinViaID = 0;
                }

                sortColumn = new GreenLightSortColumn(_sortColumnID, sortOrderDirection,
                                                      new JoinVia(_joinViaID, _sortCrumbs, Guid.NewGuid(), _joinViaList));
            }

            if (menuIconName != null && (menuIconName.EndsWith(".png") || menuIconName.EndsWith(".gif") || menuIconName.EndsWith(".jpg")))
            {
                menuIcon = menuIconName;
            }

            cCustomEntity entity = this.getEntityById(entityid);
            if (viewid > 0)
            {
                cCustomEntityView oldview = entity.getViewById(viewid);
                createdon = oldview.createdon;
                createdby = oldview.createdby;
                modifiedby = employeeid;
                modifiedon = DateTime.Now;
                currentBuiltIn = oldview.BuiltIn;
                systemCustomEntityViewId = oldview.SystemCustomEntityViewId;
            }

            // only allow the attribute to be set as built-in/system if the user is "adminonly"
            if (!this.oCurrentUser.Employee.AdminOverride && !currentBuiltIn && builtIn)
            {
                builtIn = false;
            }

            // if the form is being set as "system", create a system identifier for it
            if (builtIn && (viewid == 0 || currentBuiltIn == false))
            {
                systemCustomEntityViewId = Guid.NewGuid();
            }

            cField _field;
            SortedList<byte, cCustomEntityViewField> lstFields = new SortedList<byte, cCustomEntityViewField>();

            byte _order = (byte)0;

            string _joinViaDescription;
            string _joinViaIDs;
            Guid _joinViaPartID;
            //cCustomEntities.FilterJsTreeMetadata _joinViaMetadata;


            #region parse fields

            foreach (JavascriptTreeData.JavascriptTreeNode jsNode in oJsonFields.data)
            {
                if (Guid.TryParseExact(jsNode.attr.fieldid, "D", out _joinViaPartID))
                {
                    _field = fields.GetFieldByID(_joinViaPartID);

                    if (_field != null)
                    {
                        _joinViaID = jsNode.attr.joinviaid;
                        _joinViaDescription = jsNode.attr.crumbs;
                        _joinViaIDs = jsNode.attr.id;
                        _joinViaList = new SortedList<int, JoinViaPart>();

                        // for saving, we only need to parse out the via parts if we don't have a saved joinvia id already
                        if (_joinViaID < 1)
                        {
                            _joinViaList = JoinVias.JoinViaPartsFromCompositeGuid(_joinViaIDs);
                            _joinViaID = 0; // 0 causes the save on the joinVia list
                        }

                        // if we already have a valid joinviaid, the joinviaAS and joinvialist will be empty at this point, that's fine
                        lstFields.Add(_order, new cCustomEntityViewField(_field, new JoinVia(_joinViaID, _joinViaDescription, Guid.Empty, _joinViaList)));
                        _order++;
                    }
                }
            }

            #endregion parse fields

            SortedList<byte, FieldFilter> lstFilters = new SortedList<byte, FieldFilter>();
            // reset order
            _order = (byte)0;
            ConditionType tmpCT;
            int tmpCTint = 0;
            string tmpVal1;
            string tmpVal2;

            #region parse filters

            foreach (JavascriptTreeData.JavascriptTreeNode jsNode in oJsonFilters.data)
            {
                if (Guid.TryParseExact(jsNode.attr.fieldid, "D", out _joinViaPartID))
                {
                    _field = fields.GetFieldByID(_joinViaPartID);

                    if (_field != null)
                    {
                        _joinViaID = jsNode.attr.joinviaid;
                        _joinViaDescription = jsNode.attr.crumbs;
                        _joinViaIDs = jsNode.attr.id;
                        _joinViaList = new SortedList<int, JoinViaPart>();

                        if (jsNode.metadata.ContainsKey("conditionType"))
                        {
                            int.TryParse(jsNode.metadata["conditionType"].ToString(), out tmpCTint);
                        }

                        tmpCT = (jsNode.metadata.ContainsKey("conditionType"))
                                    ? (ConditionType)tmpCTint
                                    : ConditionType.Equals;
                        tmpVal1 = (jsNode.metadata.ContainsKey("criterionOne"))
                                      ? jsNode.metadata["criterionOne"].ToString()
                                      : string.Empty;
                        tmpVal2 = (jsNode.metadata.ContainsKey("criterionTwo"))
                                      ? jsNode.metadata["criterionTwo"].ToString()
                                      : string.Empty;

                        // for saving, we only need to parse out the via parts if we don't have a saved joinvia id already
                        if (_joinViaID < 1)
                        {
                            _joinViaList = JoinVias.JoinViaPartsFromCompositeGuid(_joinViaIDs);
                            _joinViaID = 0; // 0 causes the save on the joinVia list
                        }

                        // if we already have a valid joinviaid, the joinviaAS and joinvialist will be empty at this point, that's fine
                        lstFilters.Add(_order, new FieldFilter(_field, tmpCT, tmpVal1, tmpVal2, _order, new JoinVia(_joinViaID, _joinViaDescription, Guid.Empty, _joinViaList)));
                        _order++;
                    }
                }
            }

            #endregion parse filters

            if (addformid > 0)
            {
                addform = entity.getFormById(addformid);
                if (addform != null)
                {
                    allowadd = true;
                }
            }
            else
            {
                addform = null;
            }

            if (editformid > 0)
            {
                editform = entity.getFormById(editformid);
                if (editform != null)
                {
                    allowedit = true;
                }
            }
            else
            {
                editform = null;
            }
            cCustomEntityView view = new cCustomEntityView(viewid, entityid, viewname, description, builtIn, systemCustomEntityViewId, createdon, createdby, modifiedon, modifiedby, menuid, menuDescription, showRecordCount, lstFields, allowadd, addform, allowedit, editform, allowdelete, lstFilters, allowapproval, allowarchive, sortColumn, menuIcon, addMappings, editMappings, menuDisabledModuleIds);
            viewid = this.saveView(entityid, view);

            // if the attribute is set as built-in/system but the GreenLight isn't, make the GreenLight built-in/system too
            if (viewid > 0 && builtIn && !entity.BuiltIn)
            {
                entity.BuiltIn = true;
                this.saveEntity(entity);
            }

            return viewid;
        }

        /// <summary>
        /// saveView: Save a custom entity view definition
        /// </summary>
        /// <param name="entityid">Custom Entity ID to save view against</param>
        /// <param name="view">Custom Entity View definition to save</param>
        /// <returns></returns>
        public int saveView(int entityid, cCustomEntityView view, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@viewid", view.viewid);
            expdata.sqlexecute.Parameters.AddWithValue("@entityid", entityid);
            expdata.sqlexecute.Parameters.AddWithValue("@viewname", view.viewname);
            if (view.menuid.HasValue && view.menuid > 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@menuid", view.menuid);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@menuid", DBNull.Value);
            }

            if (view.description == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@description", view.description);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@builtIn", view.BuiltIn);

            if (view.SystemCustomEntityViewId.HasValue)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@SystemCustomEntityViewId", view.SystemCustomEntityViewId.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@SystemCustomEntityViewId", DBNull.Value);
            }

            if (view.MenuDescription == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@menuDescription", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@menuDescription", view.MenuDescription);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@showRecordCount", Convert.ToByte(view.showRecordCount));
            if (view.modifiedby == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", view.createdon);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", view.createdby);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", view.modifiedon);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", view.modifiedby);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@allowadd", Convert.ToByte(view.allowadd));
            if (view.allowadd)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@addformid", view.DefaultAddForm.formid);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@addformid", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@allowedit", Convert.ToByte(view.allowedit));
            if (view.allowedit)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@editformid", view.DefaultEditForm.formid);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@editformid", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@allowdelete", Convert.ToByte(view.allowdelete));
            expdata.sqlexecute.Parameters.AddWithValue("@allowarchive", Convert.ToByte(view.allowarchive));
            expdata.sqlexecute.Parameters.AddWithValue("@SortColumn", view.SortColumn.FieldID);

            expdata.sqlexecute.Parameters.AddWithValue("@SortOrder", Convert.ToInt32(view.SortColumn.SortDirection));

            int? jViaID = null;
            if (view.SortColumn.JoinVia != null && view.SortColumn.JoinVia.JoinViaID > 0)
            {
                jViaID = view.SortColumn.JoinVia.JoinViaID;
            }
            else if (view.SortColumn.JoinVia != null && view.SortColumn.JoinVia.JoinViaID == 0 &&
                    view.SortColumn.JoinVia.JoinViaList.Count > 0)
            {
                JoinVias clsJoinVias = new JoinVias(this.oCurrentUser);
                jViaID = clsJoinVias.SaveJoinVia(view.SortColumn.JoinVia);
            }

            if (jViaID.HasValue)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@SortJoinViaID", jViaID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@SortJoinViaID", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@AllowApproval", Convert.ToByte(view.allowapproval));
      
            if (String.IsNullOrWhiteSpace(view.MenuIcon))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@MenuIcon", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@MenuIcon", view.MenuIcon);
            }

            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveCustomEntityView");
            int viewid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            if (viewid == -1)
            {
                return -1;
            }

            if (view.menuid.HasValue)
            {
                var disabledModuleMenuViews = new DisabledModuleMenuViews(oCurrentUser.AccountID, (int)oCurrentUser.CurrentActiveModule);
                disabledModuleMenuViews.DisableForModules(view.menuid.Value, viewid, view.MenuDisabledModuleIds);
            }

            saveViewFields(viewid, view.fields);
            saveFieldFiltersforView(FieldFilterProductArea.Views, viewid, view.filters);

            List<FormSelectionMapping> allMappings = view.AddFormMappings;
            allMappings.AddRange(view.EditFormMappings);

            SaveFormSelectionMappings(viewid, allMappings);

            var lastDatabaseUpdate = expdata.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
            refreshCache(lastDatabaseUpdate);

            return viewid;
        }

        /// <summary>
        /// saveViewFields: Save custom entity fields to a view
        /// </summary>
        /// <param name="viewid">View ID to associate the fields to</param>
        /// <param name="fields">Collection of fields to associate to the view</param>
        private void saveViewFields(int viewid, SortedList<byte, cCustomEntityViewField> fields, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            int delegateid = 0;
            int? jViaID;
            JoinVias clsJoinVias;

            if (this.oCurrentUser.Delegate != null)
            {
                delegateid = oCurrentUser.Delegate.EmployeeID;
            }

            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@viewid", viewid);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateid);
            expdata.ExecuteProc("deleteCustomEntityViewFields");
            expdata.sqlexecute.Parameters.Clear();

            foreach (KeyValuePair<byte, cCustomEntityViewField> kvp in fields)
            {
                jViaID = null;

                if (kvp.Value.JoinVia != null && kvp.Value.JoinVia.JoinViaID > 0)
                {
                    jViaID = kvp.Value.JoinVia.JoinViaID;
                }
                else if (kvp.Value.JoinVia != null && kvp.Value.JoinVia.JoinViaID == 0 &&
                        kvp.Value.JoinVia.JoinViaList.Count > 0)
                {
                    clsJoinVias = new JoinVias(this.oCurrentUser);
                    jViaID = clsJoinVias.SaveJoinVia(kvp.Value.JoinVia);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@viewid", viewid);
                expdata.sqlexecute.Parameters.AddWithValue("@fieldid", kvp.Value.Field.FieldID);
                expdata.sqlexecute.Parameters.AddWithValue("@order", kvp.Key);
                if (jViaID.HasValue)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@joinViaID", jViaID.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@joinViaID", DBNull.Value);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateid);
                expdata.ExecuteProc("addCustomEntityViewField");
                expdata.sqlexecute.Parameters.Clear();
            }
        }


        // MOVE THE FOLLOWING METHOD OUT OF cCustomEntities ! ! ! ! ! ! !

        /// <summary>
        /// saveViewFilters: Save custom entity filter to a view
        /// </summary>
        /// <param name="recordid">ID to associate the fields to</param>
        /// <param name="filters">Collection of filters to associate to the view</param>
        /// <param name="productArea">"The product area for which the filter is applied"</param>
        private void saveFieldFilters(FieldFilterProductArea productArea, int recordid, SortedList<int, FieldFilter> filters, bool isParentFilter, int formid = 0, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            int delegateid = 0;
            int? jViaID;
            JoinVias clsJoinVias;

            if (this.oCurrentUser.Delegate != null)
            {
                delegateid = oCurrentUser.Delegate.EmployeeID;
            }

            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@filterType", (int)productArea);
            expdata.sqlexecute.Parameters.AddWithValue("@recordID", recordid);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateid);
            expdata.sqlexecute.Parameters.AddWithValue("@isParentFilter", isParentFilter);
            if (formid != 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@formid", formid);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@formid", DBNull.Value);
            }
            expdata.ExecuteProc("DeleteFieldFilters");
            expdata.sqlexecute.Parameters.Clear();

            foreach (KeyValuePair<int, FieldFilter> kvp in filters)
            {
                jViaID = null;

                if (kvp.Value.JoinVia != null && kvp.Value.JoinVia.JoinViaID > 0)
                {
                    jViaID = kvp.Value.JoinVia.JoinViaID;
                }
                else if (kvp.Value.JoinVia != null && kvp.Value.JoinVia.JoinViaID == 0 && kvp.Value.JoinVia.JoinViaList.Count > 0)
                {
                    clsJoinVias = new JoinVias(this.oCurrentUser);
                    jViaID = clsJoinVias.SaveJoinVia(kvp.Value.JoinVia);
                }

                if (productArea == FieldFilterProductArea.Attributes)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@attributeid", recordid);
                    if (formid != 0)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@formid", formid);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@formid", DBNull.Value);
                    }
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@attributeid", DBNull.Value);
                }

                if (productArea == FieldFilterProductArea.UserDefinedFields)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@userdefineid", recordid);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@userdefineid", DBNull.Value);
                }

                if (productArea == FieldFilterProductArea.Views)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@viewid", recordid);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@viewid", DBNull.Value);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@fieldid", kvp.Value.Field.FieldID);
                expdata.sqlexecute.Parameters.AddWithValue("@order", kvp.Key);
                expdata.sqlexecute.Parameters.AddWithValue("@operator", kvp.Value.Conditiontype);
                expdata.sqlexecute.Parameters.AddWithValue("@valueOne", kvp.Value.ValueOne);
                expdata.sqlexecute.Parameters.AddWithValue("@valueTwo", kvp.Value.ValueTwo);
                if (jViaID.HasValue)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@joinViaID", jViaID.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@joinViaID", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@isParentFilter", kvp.Value.IsParentFilter);
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateid);
                expdata.ExecuteProc("addFieldFilter");
                expdata.sqlexecute.Parameters.Clear();
            }

        }

        private void saveFieldFiltersforView(FieldFilterProductArea productArea, int recordid, SortedList<byte, FieldFilter> filters, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            int delegateid = 0;
            int? jViaID;
            JoinVias clsJoinVias;

            if (this.oCurrentUser.Delegate != null)
            {
                delegateid = oCurrentUser.Delegate.EmployeeID;
            }

            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@filterType", (int)productArea);
            expdata.sqlexecute.Parameters.AddWithValue("@recordID", recordid);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateid);
            expdata.sqlexecute.Parameters.AddWithValue("@formid", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@isParentFilter", 0);
            expdata.ExecuteProc("DeleteFieldFilters");
            expdata.sqlexecute.Parameters.Clear();

            foreach (KeyValuePair<byte, FieldFilter> kvp in filters)
            {
                jViaID = null;

                if (kvp.Value.JoinVia != null && kvp.Value.JoinVia.JoinViaID > 0)
                {
                    jViaID = kvp.Value.JoinVia.JoinViaID;
                }
                else if (kvp.Value.JoinVia != null && kvp.Value.JoinVia.JoinViaID == 0 && kvp.Value.JoinVia.JoinViaList.Count > 0)
                {
                    clsJoinVias = new JoinVias(this.oCurrentUser);
                    jViaID = clsJoinVias.SaveJoinVia(kvp.Value.JoinVia);
                }

                if (productArea == FieldFilterProductArea.Attributes)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@attributeid", recordid);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@attributeid", DBNull.Value);
                }

                if (productArea == FieldFilterProductArea.UserDefinedFields)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@userdefineid", recordid);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@userdefineid", DBNull.Value);
                }

                if (productArea == FieldFilterProductArea.Views)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@viewid", recordid);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@viewid", DBNull.Value);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@formid", DBNull.Value);
                expdata.sqlexecute.Parameters.AddWithValue("@fieldid", kvp.Value.Field.FieldID);
                expdata.sqlexecute.Parameters.AddWithValue("@order", kvp.Key);
                expdata.sqlexecute.Parameters.AddWithValue("@operator", kvp.Value.Conditiontype);
                expdata.sqlexecute.Parameters.AddWithValue("@valueOne", kvp.Value.ValueOne);
                expdata.sqlexecute.Parameters.AddWithValue("@valueTwo", kvp.Value.ValueTwo);
                if (jViaID.HasValue)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@joinViaID", jViaID.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@joinViaID", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateid);
                expdata.sqlexecute.Parameters.AddWithValue("@isParentFilter", 0);
                expdata.ExecuteProc("addFieldFilter");
                expdata.sqlexecute.Parameters.Clear();
            }

        }

        #endregion

        /// <summary>
        /// getEntityRecord: 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public SortedList<int, object> getEntityRecord(cCustomEntity entity, int id, cCustomEntityForm form)
        {
            SortedList<int, object> record = new SortedList<int, object>();

            cCustomEntity query_entity = entity;

            if (query_entity.IsSystemView)
            {
                query_entity = this.getEntityById(entity.SystemView_DerivedEntityId.Value);

            }

            cQueryBuilder clsquery = new cQueryBuilder(nAccountid, cAccounts.getConnectionString(nAccountid), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, query_entity.table, this.tables, this.fields);
            foreach (cCustomEntityFormField field in form.fields.Values)
            {
                if (field.attribute.GetType() != typeof(cOneToManyRelationship) && field.attribute.GetType() != typeof(cCommentAttribute) && field.attribute.GetType() != typeof(LookupDisplayField))
                {
                    clsquery.addColumn(fields.GetFieldByID(field.attribute.fieldid), field.attribute.attributeid.ToString());
                }
            }

            clsquery.addFilter(query_entity.table.GetPrimaryKey(), ConditionType.Equals, new object[] { id }, null, ConditionJoiner.None, null); // null as pk? !!!!!!!

            if (clsquery.lstColumns.Count > 0)
            {

                using (IDataReader reader = clsquery.getReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (!reader.GetName(i).EndsWith("_text"))
                            {
                                record.Add(Convert.ToInt32(reader.GetName(i).Replace("att", string.Empty)), reader.GetValue(i));
                            }
                        }
                    }

                    reader.Close();
                }
            }

            return record;
        }

        /// <summary>
        /// Looks up an EntityID using a child attribute ID
        /// </summary>
        /// <param name="attributeid">Child attribute ID</param>
        /// <returns>Parent Entity ID of the Attribute</returns>
        public int getEntityIdByAttributeId(int attributeid)
        {
            int entityID = (from e in lst.Values
                            from a in e.attributes.Values
                            where a.attributeid == attributeid
                            select e.entityid).FirstOrDefault();
            return entityID;
        }

        public cCustomEntity getSystemViewEntity(int derived_entityid, int parent_entityid)
        {
            cCustomEntity retCE = (from sv in lst.Values
                                   where sv.IsSystemView && sv.SystemView_DerivedEntityId == derived_entityid && sv.SystemView_EntityId == parent_entityid
                                   select sv).FirstOrDefault();
            return retCE;
        }

        public string[] generateSummaryGridNew(int entityId, int attributeId, int viewId, int activeTab, int formId, int recordId, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            cCustomEntity entity = getEntityById(getEntityIdByAttributeId(attributeId));
            cAttribute summaryAttribute = entity.getAttributeById(attributeId);
            cSummaryAttribute sAtt = (cSummaryAttribute)summaryAttribute;
            cCustomEntity summarySourceEntity = getEntityById(sAtt.SourceEntityID);

            #region calculate OTM Relationship paths

            // get relationship paths for all summarised otm attributes for use by joinVias
            Dictionary<int, Dictionary<int, cOneToManyRelationship>> attributePaths = new Dictionary<int, Dictionary<int, cOneToManyRelationship>>();

            foreach (cSummaryAttributeElement attributeElement in sAtt.SummaryElements.Values)
            {
                int otmEntityId = getEntityIdByAttributeId(attributeElement.OTM_AttributeId);
                Dictionary<int, cOneToManyRelationship> tmpRel = getRelationshipPath(entityId, otmEntityId, null);
                if (tmpRel.Count > 0)
                {
                    // must have path, so append implicit relationship to summarySource
                    cCustomEntity tmpEntity = getEntityById(otmEntityId);
                    cOneToManyRelationship tmpAttribute = (cOneToManyRelationship)tmpEntity.attributes.Values.FirstOrDefault(x => x.GetType() == typeof(cOneToManyRelationship) && ((cOneToManyRelationship)x).entityid == sAtt.SourceEntityID);

                    if (tmpAttribute != null)
                        tmpRel.Add(sAtt.SourceEntityID, tmpAttribute);
                }

                attributePaths.Add(attributeElement.OTM_AttributeId, tmpRel);
            }

            #endregion

            string metabaseConnStr = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;

            #region pre-cache ce form data

            Dictionary<int, int> ceForms = (Dictionary<int, int>)Cache["ceForms_" + nAccountid.ToString()];
            if (ceForms == null)
            {
                // only cache for 1 minute
                ceForms = new Dictionary<int, int>();

                string formSQL = "select entityid, formid from customEntityForms";
                using (IDataReader reader = expdata.GetReader(formSQL))
                {
                    while (reader.Read())
                    {
                        int ceform_entityid = reader.GetInt32(0);
                        int ceform_formid = reader.GetInt32(1);

                        if (!ceForms.ContainsKey(ceform_entityid))
                        {
                            ceForms.Add(ceform_entityid, ceform_formid);
                        }
                    }

                    reader.Close();

                    Cache.Insert("ceForms_" + nAccountid.ToString(), ceForms, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.UltraShort), CacheItemPriority.Default, null);
                }
            }

            #endregion

            string union = string.Empty;
            string filterStr = string.Empty;
            string filterParam = "@filter1_1";
            int srcViewId = 0;

            StringBuilder sbQuery = new StringBuilder();
            JoinVia baseToSummaryJoinVia = null;

            foreach (KeyValuePair<int, cSummaryAttributeElement> kvp in sAtt.SummaryElements)
            {
                cSummaryAttributeElement e = (cSummaryAttributeElement)kvp.Value;

                cQueryBuilder qb = new cQueryBuilder(this.oCurrentUser.AccountID, cAccounts.getConnectionString(this.oCurrentUser.AccountID), metabaseConnStr, entity.table, tables, fields);

                bool firstcolumn = true;
                cSummaryAttributeColumn filterCol = null;

                cCustomEntity otmEntity = getEntityById(getEntityIdByAttributeId(e.OTM_AttributeId));
                cOneToManyRelationship otmAttribute = (cOneToManyRelationship)otmEntity.getAttributeById(e.OTM_AttributeId);
                JoinVia baseToParentJoinVia = null;
                cCustomEntity parent_entity = null;

                foreach (KeyValuePair<int, cSummaryAttributeColumn> sc_kvp in sAtt.SummaryColumns)
                {
                    cSummaryAttributeColumn col = (cSummaryAttributeColumn)sc_kvp.Value;

                    if (col.DefaultSort && col.FilterValue != string.Empty)
                    {
                        filterCol = col;
                    }

                    if (firstcolumn)
                    {
                        srcViewId = otmAttribute.viewid;

                        int parent_formid;

                        if (ceForms.ContainsKey(otmAttribute.parent_entityid))
                        {
                            parent_formid = ceForms[otmAttribute.parent_entityid];
                        }
                        else
                        {
                            cQueryBuilder tmpQB = new cQueryBuilder(nAccountid, cAccounts.getConnectionString(nAccountid), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, tables.GetCustomTableByName("customEntityForms"), tables, fields);
                            tmpQB.addColumn(fields.GetCustomFieldByTableAndFieldName(tables.GetCustomTableByName("customEntityForms").TableID, "formid"));
                            tmpQB.addFilter(new cQueryFilter(fields.GetCustomFieldByTableAndFieldName(tables.GetCustomTableByName("customEntityForms").TableID, "entityid"), ConditionType.Equals, new List<object> { otmAttribute.parent_entityid }, null, ConditionJoiner.None, null)); // null as appears to be the entityid on the forms table?  !!!!!!
                            parent_formid = tmpQB.GetCount();
                        }

                        parent_entity = getEntityById(otmAttribute.parent_entityid);
                        cTable parent_table = tables.GetTableByID(parent_entity.table.TableID);
                        JoinVia currentJoinVia = null;

                        SortedList<int, JoinViaPart> tmpParts = new SortedList<int, JoinViaPart>();

                        // create base table to summary entity joinvia
                        int stepIdx = 0;

                        foreach (KeyValuePair<int, cOneToManyRelationship> pathSteps in attributePaths[e.OTM_AttributeId])
                        {
                            cOneToManyRelationship pathStep = (cOneToManyRelationship)pathSteps.Value;

                            tmpParts.Add(stepIdx++, new JoinViaPart(getEntityById(pathStep.entityid).table.TableID, JoinViaPart.IDType.Table, JoinViaPart.JoinType.INNER));
                        }

                        currentJoinVia = new JoinVia(0, string.Empty, Guid.NewGuid(), tmpParts);

                        baseToSummaryJoinVia = currentJoinVia;

                        // for base to selected summary element, reuse base to summary target minus the final step
                        if (attributePaths.ContainsKey(e.OTM_AttributeId))
                        {
                            tmpParts = new SortedList<int, JoinViaPart>();

                            foreach (KeyValuePair<int, JoinViaPart> jvp in baseToSummaryJoinVia.JoinViaList)
                            {
                                if (jvp.Key != baseToSummaryJoinVia.JoinViaList.Count - 1)
                                {
                                    tmpParts.Add(jvp.Key, new JoinViaPart(jvp.Value.ViaID, jvp.Value.ViaIDType, jvp.Value.TypeOfJoin));
                                }
                            }

                            currentJoinVia = new JoinVia(0, string.Empty, Guid.NewGuid(), tmpParts);

                            baseToParentJoinVia = currentJoinVia;
                        }

                        // insert id and source area identification column
                        qb.addColumn(fields.GetFieldByID(parent_table.GetPrimaryKey().FieldID), baseToParentJoinVia, "parent_id");
                        qb.addColumn(fields.GetCustomFieldByTableAndFieldName(summarySourceEntity.table.TableID, summarySourceEntity.table.GetPrimaryKey().FieldName), baseToSummaryJoinVia);
                        qb.addStaticColumn(otmAttribute.entityid.ToString(), "entityid");
                        qb.addStaticColumn(summarySourceEntity.Views[otmAttribute.viewid].DefaultEditForm.formid.ToString(), "formid");
                        qb.addStaticColumn(otmAttribute.viewid.ToString(), "viewid");
                        qb.addStaticColumn(summarySourceEntity.Views[otmAttribute.viewid].DefaultEditForm.fields.Count.ToString(), "formfieldcount");
                        qb.addStaticColumn(otmAttribute.parent_entityid.ToString(), "parent_entityid");
                        qb.addStaticColumn(otmAttribute.attributeid.ToString(), "attributeid");
                        qb.addStaticColumn(parent_formid.ToString(), "parent_formid");
                        qb.addStaticColumn(otmAttribute.displayname, "Summary Source");
                        firstcolumn = false;
                    }

                    cAttribute columnAttribute = summarySourceEntity.getAttributeById(col.ColumnAttributeID);
                    cField gridField = fields.GetFieldByID(columnAttribute.fieldid);

                    //cField gridField = fields.GetCustomFieldByTableAndFieldName(summarySourceEntity.table.TableID, col.ColumnAttributeID);

                    if (col.JoinViaObj != null && col.DisplayFieldId != Guid.Empty)
                    {
                        SortedList<int, JoinViaPart> tmpParts = new SortedList<int, JoinViaPart>();
                        // force joinviaPart for basetable to systemview
                        int stepIdx = 0;

                        foreach (JoinViaPart jvp in baseToSummaryJoinVia.JoinViaList.Values)
                        {
                            tmpParts.Add(stepIdx++, new JoinViaPart(jvp.ViaID, jvp.ViaIDType, jvp.TypeOfJoin));
                        }

                        foreach (JoinViaPart jvp in col.JoinViaObj.JoinViaList.Values)
                        {
                            tmpParts.Add(stepIdx++, jvp);
                        }

                        // set gridField to be the displayFieldId for the n:1 selection
                        gridField = fields.GetFieldByID(col.DisplayFieldId);

                        JoinVia tmpJoinVia = new JoinVia(col.JoinViaObj.JoinViaID, col.JoinViaObj.Description, col.JoinViaObj.JoinViaAS, tmpParts);
                        qb.addColumn(gridField, tmpJoinVia);
                    }
                    else if (gridField.GenList || gridField.ValueList)
                    {
                        qb.addColumn(gridField, true);
                    }
                    else if (gridField.TableID == otmEntity.table.TableID)
                    {
                        col.JoinViaObj = baseToParentJoinVia;
                        qb.addColumn(gridField, baseToParentJoinVia);
                    }
                    else if (gridField.TableID == summarySourceEntity.table.TableID)
                    {
                        col.JoinViaObj = baseToSummaryJoinVia;
                        qb.addColumn(gridField, baseToSummaryJoinVia);
                    }
                    else
                    {
                        qb.addColumn(gridField);
                    }
                }

                // add query filter to ensure only summary data for current record returned
                qb.addFilter(new cQueryFilter(entity.table.GetPrimaryKey(), ConditionType.Equals, new List<object> { recordId }, null, ConditionJoiner.None, null));

                if (filterCol != null)
                {
                    cAttribute filterColumnAttribute = summarySourceEntity.getAttributeById(filterCol.ColumnAttributeID);
                    cField filterField = fields.GetFieldByID(filterColumnAttribute.fieldid);

                    if (filterField.ValueList || filterField.GenList)
                    {
                        foreach (int s in filterField.ListItems.Keys)
                        {
                            if (filterField.ListItems[s].ToString() == filterCol.FilterValue)
                            {
                                filterStr = s.ToString();
                                filterParam = "@filter1_1_0";
                                qb.addFilter(new cQueryFilter(filterField, ConditionType.Equals, new List<object> { filterStr }, null, ConditionJoiner.And, null)); // null, but may need enhancement to let summaries use manytoone?    !!!!!!!!
                                break;
                            }
                        }
                    }
                    else
                    {
                        filterStr = "%" + filterCol.FilterValue + "%";
                        filterParam = "@filter1_1";
                        qb.addFilter(new cQueryFilter(filterField, ConditionType.Like, new List<object> { filterStr }, null, ConditionJoiner.And, null)); // null, but may need enhancement to let summaries use manytoone?    !!!!!!!!
                    }
                }

                if (parent_entity != null && parent_entity.AudienceView != AudienceViewType.NoAudience)
                {
                    SerializableDictionary<string, object> parentAudienceStatus = GetAudienceRecords(entity.SystemView_EntityId.Value, oCurrentUser.EmployeeID);

                    string excludeList = String.Join(",", (from n in parentAudienceStatus.Values where ((cAudienceRecordStatus)n).Status == 0 select ((cAudienceRecordStatus)n).RecordID).ToArray());
                    if (excludeList.Length > 0)
                    {
                        qb.addFilterString(new cQueryFilterString(parent_entity.table.GetPrimaryKey().FieldName + " NOT IN (" + excludeList + ")", ConditionJoiner.And));
                    }
                }

                sbQuery.Append(union);
                sbQuery.Append(qb.sql);

                union = " UNION ALL ";
            }

            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cStaticGridColumn("parent_id", Guid.NewGuid()));
            if (baseToSummaryJoinVia != null)
                columns.Add(new cFieldColumn(summarySourceEntity.table.GetPrimaryKey(), baseToSummaryJoinVia));
            else
                columns.Add(new cFieldColumn(summarySourceEntity.table.GetPrimaryKey()));

            columns.Add(new cStaticGridColumn("entityid", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("formid", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("viewid", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("formfieldcount", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("parent_entityid", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("attributeid", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("parent_formid", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("Summary Source", Guid.NewGuid()));
            //string sortColumn = "";

            foreach (KeyValuePair<int, cSummaryAttributeColumn> sc_kvp in sAtt.SummaryColumns)
            {
                cSummaryAttributeColumn col = (cSummaryAttributeColumn)sc_kvp.Value;
                cAttribute columnAttribute = summarySourceEntity.getAttributeById(col.ColumnAttributeID);
                cField curField = fields.GetFieldByID(columnAttribute.fieldid);

                cNewGridColumn gridCol;

                if (col.IsMTOField)
                {
                    cManyToOneRelationship relAtt = (cManyToOneRelationship)columnAttribute;

                    if (col.JoinViaObj != null)
                    {
                        gridCol = new cFieldColumn(fields.GetFieldByID(col.DisplayFieldId), col.JoinViaObj);
                    }
                    else
                    {
                        gridCol = new cFieldColumn(fields.GetFieldByID(columnAttribute.fieldid), relAtt.relatedtable.GetKeyField());
                    }
                }
                else
                {
                    if (col.JoinViaObj != null)
                    {
                        gridCol = new cFieldColumn(fields.GetFieldByID(columnAttribute.fieldid), col.JoinViaObj);
                    }
                    else
                    {
                        gridCol = new cFieldColumn(fields.GetFieldByID(columnAttribute.fieldid));
                    }
                }

                columns.Add(gridCol);
            }
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@filter1_0_0", recordId); // qbuilder adds this to each part of the union
            expdata.sqlexecute.Parameters.AddWithValue(filterParam, filterStr);
            DataSet dset = expdata.GetDataSet(sbQuery.ToString());

            cCustomEntityView view = summarySourceEntity.getViewById(srcViewId);
            cAttribute keyatt = summarySourceEntity.getKeyField();
            cField keyfield = fields.GetCustomFieldByTableAndFieldName(summarySourceEntity.table.TableID, keyatt.attributename);
            string keyFieldColumnID = keyfield.FieldName;
            cFieldColumn keyColumn = (cFieldColumn)columns.FirstOrDefault(x => x.GetType() == typeof(cFieldColumn) && ((cFieldColumn)x).field.FieldName == "att" + keyatt.attributeid.ToString());
            if (keyColumn != null && !string.IsNullOrWhiteSpace(keyColumn.Alias))
            {
                keyFieldColumnID = keyColumn.Alias;
            }

            cGridNew grid = new cGridNew(this.oCurrentUser.AccountID, oCurrentUser.EmployeeID, "grid" + view.entityid + view.viewid + attributeId, entity.table, columns, dset)
            {
                enablepaging = true,
                EnableSorting = true,
                CssClass = "datatbl",
                KeyField = keyFieldColumnID,
                EmptyText = "No summary records to display"
            };

            SerializableDictionary<string, object> gridInfo = new SerializableDictionary<string, object>();
            if (summarySourceEntity.AudienceView != AudienceViewType.NoAudience)
            {
                SerializableDictionary<string, object> audienceStatus = GetAudienceRecords(summarySourceEntity.entityid, oCurrentUser.EmployeeID);
                gridInfo.Add("employeeid", oCurrentUser.EmployeeID);
                gridInfo.Add("entityid", entity.entityid);
                gridInfo.Add("keyfield", keyfield.FieldName);
                gridInfo.Add("gridid", grid.GridID);

                grid.HiddenRecords = (from x in audienceStatus.Values
                                      where ((cAudienceRecordStatus)x).Status == 0
                                      select ((cAudienceRecordStatus)x).RecordID).ToList();
            }

            grid.InitialiseRowGridInfo = gridInfo;
            grid.InitialiseRow += new cGridNew.InitialiseRowEvent(clsOTMgrid_InitialiseRow);
            grid.ServiceClassForInitialiseRowEvent = "Spend_Management.cCustomEntities";
            grid.ServiceClassMethodForInitialiseRowEvent = "clsOTMgrid_InitialiseRow";

            if (view.allowdelete)
            {
                grid.enabledeleting = true;
                grid.deletelink = "javascript:deleteRecord(" + view.entityid + ",{" + keyfield.FieldName + "}, " + view.viewid + "," + attributeId.ToString() + ")";
            }           

            string returnurl = "aeentity.aspx?";
            string entityurl = "relentityid=" + entityId.ToString() + "&";
            string formurl = "relformid=" + formId.ToString() + "&";
            string recordurl = "relrecordid=" + recordId.ToString() + "&";
            string taburl = "reltabid=" + activeTab.ToString() + "&";
            string viewurl = "relviewid=" + viewId.ToString() + "&";

            if (view.allowedit)
            {
                grid.enableupdating = true;

                returnurl += entityurl + formurl + recordurl + taburl + viewurl + "entityid=" + view.entityid + "&formid=" + view.DefaultEditForm.formid + "&tabid=0&id={" + keyfield.FieldName + "}&fromsummary=1";

                grid.editlink = returnurl;
            }

            returnurl = "aeentity.aspx?";
            returnurl += entityurl + formurl + recordurl + taburl + "viewid=" + viewId.ToString() + "&entityid={parent_entityid}&formid={parent_formid}&tabid=0&id={parent_id}&fromsummary=1";
            grid.addEventColumn("viewrec", "/shared/images/icons/view.png", "javascript:document.location.href='" + returnurl + "';", "View", "View Record");

            // hide the 8 id columns
            for (int x = 1; x < 10; x++)
            {
                grid.getColumnByIndex(x).hidden = true;
            }

            List<string> retVals = new List<string>();
            retVals.Add(grid.GridID);
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="attributeId"></param>
        /// <param name="viewId"></param>
        /// <param name="activeTab"></param>
        /// <param name="formId"></param>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public string[] generateSummaryGrid(int entityId, int attributeId, int viewId, int activeTab, int formId, int recordId, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            cCustomEntity entity = getEntityById(getEntityIdByAttributeId(attributeId));
            StringBuilder sbQuery = new StringBuilder();

            cAttribute summary_attribute = entity.getAttributeById(attributeId);
            cSummaryAttribute s_att = (cSummaryAttribute)summary_attribute;
            cCustomEntity summarySourceEntity = getEntityById(s_att.SourceEntityID);

            string metabaseConnStr = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;

            cQueryBuilder qb;
            string union = string.Empty;
            string filterStr = string.Empty;
            string filterParam = "@filter1_1";
            int srcViewId = 0;

            // pre-cache ce form data
            Dictionary<int, int> ceForms = (Dictionary<int, int>)Cache["ceForms_" + nAccountid.ToString()];
            if (ceForms == null)
            {
                // only cache for 1 minute
                ceForms = new Dictionary<int, int>();

                string formSQL = "select entityid, formid from customEntityForms";
                using (IDataReader reader = expdata.GetReader(formSQL))
                {
                    while (reader.Read())
                    {
                        int ceform_entityid = reader.GetInt32(0);
                        int ceform_formid = reader.GetInt32(1);

                        if (!ceForms.ContainsKey(ceform_entityid))
                        {
                            ceForms.Add(ceform_entityid, ceform_formid);
                        }
                    }

                    reader.Close();

                    Cache.Insert("ceForms_" + nAccountid.ToString(), ceForms, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.UltraShort), CacheItemPriority.Default, null);
                }
            }

            JoinVia baseJoinVia = null;
            Guid baseJoinViaAS = Guid.NewGuid();
            JoinVia svJoinVia = null;
            Guid svJoinViaAS = Guid.NewGuid();

            foreach (KeyValuePair<int, cSummaryAttributeElement> kvp in s_att.SummaryElements)
            {
                cSummaryAttributeElement e = (cSummaryAttributeElement)kvp.Value;

                qb = new cQueryBuilder(this.oCurrentUser.AccountID, cAccounts.getConnectionString(this.oCurrentUser.AccountID), metabaseConnStr, entity.table, tables, fields);

                bool firstcolumn = true;
                cSummaryAttributeColumn filterCol = null;

                cCustomEntity otmEntity = getEntityById(getEntityIdByAttributeId(e.OTM_AttributeId));
                cOneToManyRelationship otmAttribute = (cOneToManyRelationship)otmEntity.getAttributeById(e.OTM_AttributeId);

                cCustomEntity sysview_entity = getSystemViewEntity(otmAttribute.entityid, otmAttribute.parent_entityid);
                if (sysview_entity == null)
                {
                    sysview_entity = otmEntity;
                }

                cCustomEntity parent_entity = null;
                JoinVia defaultJoinVia = null;

                foreach (KeyValuePair<int, cSummaryAttributeColumn> sc_kvp in s_att.SummaryColumns)
                {
                    cSummaryAttributeColumn col = (cSummaryAttributeColumn)sc_kvp.Value;

                    if (col.DefaultSort && col.FilterValue != string.Empty)
                    {
                        filterCol = col;
                    }

                    if (firstcolumn)
                    {
                        srcViewId = otmAttribute.viewid;

                        int parent_formid;

                        if (ceForms.ContainsKey(otmAttribute.parent_entityid))
                        {
                            parent_formid = ceForms[otmAttribute.parent_entityid];
                        }
                        else
                        {
                            cQueryBuilder tmpQB = new cQueryBuilder(nAccountid, cAccounts.getConnectionString(nAccountid), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, tables.GetCustomTableByName("customEntityForms"), tables, fields);
                            tmpQB.addColumn(fields.GetCustomFieldByTableAndFieldName(tables.GetCustomTableByName("customEntityForms").TableID, "formid"));
                            tmpQB.addFilter(new cQueryFilter(fields.GetCustomFieldByTableAndFieldName(tables.GetCustomTableByName("customEntityForms").TableID, "entityid"), ConditionType.Equals, new List<object>() { otmAttribute.parent_entityid }, null, ConditionJoiner.None, null)); // null as appears to be the entityid on the forms table?  !!!!!!
                            parent_formid = tmpQB.GetCount();
                        }

                        parent_entity = getEntityById(otmAttribute.parent_entityid);
                        cTable parent_table = tables.GetTableByID(parent_entity.table.TableID);

                        // create default JoinVia
                        SortedList<int, JoinViaPart> tmpParts = new SortedList<int, JoinViaPart>();
                        // force joinviaPart for basetable to systemview
                        tmpParts.Add(0, new JoinViaPart(sysview_entity.table.TableID, JoinViaPart.IDType.Table, JoinViaPart.JoinType.INNER));

                        defaultJoinVia = new JoinVia(0, string.Empty, svJoinViaAS, tmpParts);
                        if (svJoinVia == null)
                            svJoinVia = defaultJoinVia;

                        // create a joinVia for use from base table to current table
                        tmpParts = new SortedList<int, JoinViaPart>();
                        tmpParts.Add(0, new JoinViaPart(parent_table.TableID, JoinViaPart.IDType.Table, JoinViaPart.JoinType.INNER));
                        JoinVia parentTableJoinVia = new JoinVia(0, string.Empty, baseJoinViaAS, tmpParts);
                        if (baseJoinVia == null)
                            baseJoinVia = parentTableJoinVia;

                        // insert id and source area identification column
                        qb.addColumn(fields.GetFieldByID(parent_table.GetPrimaryKey().FieldID), parentTableJoinVia); // "parent_id"
                        qb.addColumn(fields.GetCustomFieldByTableAndFieldName(sysview_entity.table.TableID, sysview_entity.table.GetPrimaryKey().FieldName), defaultJoinVia);
                        qb.addStaticColumn(otmAttribute.entityid.ToString(), "entityid");
                        qb.addStaticColumn(summarySourceEntity.Views[otmAttribute.viewid].DefaultEditForm.formid.ToString(), "formid");
                        qb.addStaticColumn(otmAttribute.viewid.ToString(), "viewid");
                        qb.addStaticColumn(summarySourceEntity.Views[otmAttribute.viewid].DefaultEditForm.fields.Count.ToString(), "formfieldcount");
                        qb.addStaticColumn(otmAttribute.parent_entityid.ToString(), "parent_entityid");
                        qb.addStaticColumn(otmAttribute.attributeid.ToString(), "attributeid");
                        qb.addStaticColumn(parent_formid.ToString(), "parent_formid");
                        qb.addStaticColumn(otmAttribute.displayname, "Summary Source");
                        firstcolumn = false;
                    }

                    cAttribute columnAttribute = entity.getAttributeById(col.ColumnAttributeID);
                    cField gridField = fields.GetFieldByID(columnAttribute.fieldid);

                    if (col.JoinViaObj != null && col.DisplayFieldId != Guid.Empty)
                    {
                        SortedList<int, JoinViaPart> tmpParts = new SortedList<int, JoinViaPart>();
                        // force joinviaPart for basetable to systemview
                        tmpParts.Add(-1, new JoinViaPart(sysview_entity.table.TableID, JoinViaPart.IDType.Table, JoinViaPart.JoinType.INNER));

                        foreach (KeyValuePair<int, JoinViaPart> kvpParts in col.JoinViaObj.JoinViaList)
                        {
                            // replace the first joinViaPart fieldID with the sysview fieldID for the same field
                            tmpParts.Add(kvpParts.Key, new JoinViaPart(tmpParts.Count == 1 ? gridField.FieldID : kvpParts.Value.ViaID, kvpParts.Value.ViaIDType, kvpParts.Value.TypeOfJoin));
                        }

                        // set gridField to be the displayFieldId for the n:1 selection
                        gridField = fields.GetFieldByID(col.DisplayFieldId);

                        JoinVia tmpJoinVia = new JoinVia(col.JoinViaObj.JoinViaID, col.JoinViaObj.Description, col.JoinViaObj.JoinViaAS, tmpParts);
                        qb.addColumn(gridField, tmpJoinVia);
                    }
                    else if (gridField.GenList || gridField.ValueList)
                    {
                        qb.addColumn(gridField, true);
                    }
                    else if (gridField.TableID == sysview_entity.table.TableID)
                    {
                        col.JoinViaObj = defaultJoinVia;
                        qb.addColumn(gridField, defaultJoinVia);
                    }
                    else if (gridField.TableID == parent_entity.table.TableID)
                    {
                        col.JoinViaObj = baseJoinVia;
                        qb.addColumn(gridField, baseJoinVia);
                    }
                    else
                    {
                        qb.addColumn(gridField);
                    }
                }

                // add query filter to ensure only summary data for current record returned
                qb.addFilter(new cQueryFilter(entity.table.GetPrimaryKey(), ConditionType.Equals, new List<object> { recordId }, null, ConditionJoiner.None, null));

                if (filterCol != null)
                {
                    cAttribute filterColumnAttribute = entity.getAttributeById(filterCol.ColumnAttributeID);
                    cField filterField = fields.GetFieldByID(filterColumnAttribute.fieldid);
                    if (filterField.ValueList || filterField.GenList)
                    {
                        foreach (int s in filterField.ListItems.Keys)
                        {
                            if (filterField.ListItems[s].ToString() == filterCol.FilterValue)
                            {
                                filterStr = s.ToString();
                                filterParam = "@filter1_1_0";
                                qb.addFilter(new cQueryFilter(filterField, ConditionType.Equals, new List<object> { filterStr }, null, ConditionJoiner.And, null)); // null, but may need enhancement to let summaries use manytoone?    !!!!!!!!
                                break;
                            }
                        }
                    }
                    else
                    {
                        filterStr = "%" + filterCol.FilterValue + "%";
                        filterParam = "@filter1_1";
                        qb.addFilter(new cQueryFilter(filterField, ConditionType.Like, new List<object> { filterStr }, null, ConditionJoiner.And, null)); // null, but may need enhancement to let summaries use manytoone?    !!!!!!!!
                    }
                }

                if (parent_entity != null && parent_entity.AudienceView != AudienceViewType.NoAudience)
                {
                    SerializableDictionary<string, object> parentAudienceStatus = GetAudienceRecords(sysview_entity.SystemView_EntityId.Value, oCurrentUser.EmployeeID);

                    string excludeList = String.Join(",", (from n in parentAudienceStatus.Values where ((cAudienceRecordStatus)n).Status == 0 select ((cAudienceRecordStatus)n).RecordID).ToArray());
                    if (excludeList.Length > 0)
                    {
                        qb.addFilterString(new cQueryFilterString(parent_entity.table.GetPrimaryKey().FieldName + " NOT IN (" + excludeList + ")", ConditionJoiner.And));
                    }
                }

                sbQuery.Append(union);
                sbQuery.Append(qb.sql);

                union = " UNION ALL ";
            }

            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cStaticGridColumn("parent_id", Guid.NewGuid()));
            if (svJoinVia != null)
                columns.Add(new cFieldColumn(summarySourceEntity.table.GetPrimaryKey(), svJoinVia));
            else
                columns.Add(new cFieldColumn(summarySourceEntity.table.GetPrimaryKey()));

            columns.Add(new cStaticGridColumn("entityid", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("formid", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("viewid", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("formfieldcount", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("parent_entityid", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("attributeid", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("parent_formid", Guid.NewGuid()));
            columns.Add(new cStaticGridColumn("Summary Source", Guid.NewGuid()));
            //string sortColumn = "";

            foreach (KeyValuePair<int, cSummaryAttributeColumn> sc_kvp in s_att.SummaryColumns)
            {
                cSummaryAttributeColumn col = (cSummaryAttributeColumn)sc_kvp.Value;
                cAttribute curAtt = summarySourceEntity.getAttributeById(col.ColumnAttributeID);
                cField curField = fields.GetFieldByID(curAtt.fieldid);

                cNewGridColumn gridCol;

                if (col.IsMTOField)
                {
                    cManyToOneRelationship relAtt = (cManyToOneRelationship)curAtt;

                    if (col.JoinViaObj != null)
                    {
                        gridCol = new cFieldColumn(fields.GetFieldByID(col.DisplayFieldId), col.JoinViaObj);
                    }
                    else
                    {
                        gridCol = new cFieldColumn(fields.GetFieldByID(curAtt.fieldid), relAtt.relatedtable.GetKeyField());
                    }
                }
                else
                {
                    gridCol = new cFieldColumn(fields.GetFieldByID(curAtt.fieldid));
                }

                columns.Add(gridCol);
            }

            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@filter1_0_0", recordId); // qbuilder adds this to each part of the union
            expdata.sqlexecute.Parameters.AddWithValue(filterParam, filterStr);
            DataSet dset = expdata.GetDataSet(sbQuery.ToString());

            cCustomEntityView view = summarySourceEntity.getViewById(srcViewId);
            cAttribute keyatt = summarySourceEntity.getKeyField();
            cField keyfield = fields.GetCustomFieldByTableAndFieldName(summarySourceEntity.table.TableID, keyatt.attributename);

            cGridNew grid = new cGridNew(this.oCurrentUser.AccountID, oCurrentUser.EmployeeID, "grid" + view.entityid + view.viewid + attributeId, entity.table, columns, dset)
            {
                enablepaging = true,
                EnableSorting = true,
                CssClass = "datatbl",
                KeyField = keyfield.FieldName,
                EmptyText = "No summary records to display"
            };

            SerializableDictionary<string, object> gridInfo = new SerializableDictionary<string, object>();
            if (summarySourceEntity.AudienceView != AudienceViewType.NoAudience)
            {
                SerializableDictionary<string, object> audienceStatus = GetAudienceRecords(summarySourceEntity.entityid, oCurrentUser.EmployeeID);
                gridInfo.Add("employeeid", oCurrentUser.EmployeeID);
                gridInfo.Add("entityid", entity.entityid);
                gridInfo.Add("keyfield", keyfield.FieldName);
                gridInfo.Add("gridid", grid.GridID);

                grid.HiddenRecords = (from x in audienceStatus.Values
                                      where ((cAudienceRecordStatus)x).Status == 0
                                      select ((cAudienceRecordStatus)x).RecordID).ToList();
            }

            grid.InitialiseRowGridInfo = gridInfo;
            grid.InitialiseRow += new cGridNew.InitialiseRowEvent(clsOTMgrid_InitialiseRow);
            grid.ServiceClassForInitialiseRowEvent = "Spend_Management.cCustomEntities";
            grid.ServiceClassMethodForInitialiseRowEvent = "clsOTMgrid_InitialiseRow";

            if (view.allowdelete)
            {
                grid.enabledeleting = true;
                grid.deletelink = "javascript:deleteRecord(" + view.entityid + ",{" + keyfield.FieldName + "}, " + view.viewid + "," + ", " + attributeId.ToString() + ")";
            }

          
            string returnurl = "aeentity.aspx?";
            string entityurl = "relentityid=" + entityId.ToString() + "&";
            string formurl = "relformid=" + formId.ToString() + "&";
            string recordurl = "relrecordid=" + recordId.ToString() + "&";
            string taburl = "reltabid=" + activeTab.ToString() + "&";
            string viewurl = "relviewid=" + viewId.ToString() + "&";

            if (view.allowedit)
            {
                grid.enableupdating = true;

                returnurl += entityurl + formurl + recordurl + taburl + viewurl + "viewid=" + viewId.ToString() + "&entityid=" + view.entityid + "&formid=" + view.DefaultEditForm.formid + "&tabid=0&id={" + keyfield.FieldName + "}&fromsummary=1";

                grid.editlink = returnurl;
            }

            returnurl = "aeentity.aspx?";
            returnurl += entityurl + formurl + recordurl + taburl + "viewid=" + viewId.ToString() + "&entityid={parent_entityid}&formid={parent_formid}&tabid=0&id={parent_id}&fromsummary=1";
            grid.addEventColumn("viewrec", "/shared/images/icons/view.png", "javascript:document.location.href='" + returnurl + "';", "View", "View Record");

            // hide the 8 id columns
            for (int x = 1; x < 10; x++)
            {
                grid.getColumnByIndex(x).hidden = true;
            }

            List<string> retVals = new List<string>();
            retVals.Add(grid.GridID);
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

        /// <summary>
        /// Returns a path of one to many relationships to get from a source entity to a destination entity (i.e. A --> B --> C)
        /// </summary>
        /// <param name="sourceEntityId"></param>
        /// <param name="destinationEntityId"></param>
        public Dictionary<int, cOneToManyRelationship> getRelationshipPath(int sourceEntityId, int destinationEntityId, Dictionary<int, cOneToManyRelationship> currentPath)
        {
            cCustomEntity srcEntity = getEntityById(sourceEntityId);
            if (currentPath == null)
                currentPath = new Dictionary<int, cOneToManyRelationship>();

            Dictionary<int, cOneToManyRelationship> rels = new Dictionary<int, cOneToManyRelationship>(currentPath);

            foreach (cOneToManyRelationship relAttribute in srcEntity.attributes.Values.Where(x => x.GetType() == typeof(cOneToManyRelationship)))
            {
                rels.Add(relAttribute.parent_entityid, relAttribute);

                if (relAttribute.entityid != destinationEntityId)
                {
                    rels = getRelationshipPath(relAttribute.entityid, destinationEntityId, rels);
                }

                // if got source and destination, then a valid path, otherwise clear and try next

                if ((rels.Values.Count(x => x.entityid == destinationEntityId) > 0) && rels.ContainsKey(sourceEntityId))
                {
                    break;
                }

                rels.Clear();
                rels = new Dictionary<int, cOneToManyRelationship>(currentPath);
            }

            return rels;
        }

        public void getRelationshipAttributes(int entityid, int relationship_entityid, ref Dictionary<int, cOneToManyRelationship> otmRelationships)
        {
            cCustomEntity entity = this.getEntityById(entityid);

            if (otmRelationships == null)
                otmRelationships = new Dictionary<int, cOneToManyRelationship>();

            foreach (KeyValuePair<int, cAttribute> kvp in entity.attributes)
            {
                cAttribute att = (cAttribute)kvp.Value;
                if (att.fieldtype == FieldType.Relationship)
                {
                    if (typeof(cOneToManyRelationship) == att.GetType())
                    {
                        cOneToManyRelationship otm = (cOneToManyRelationship)att;
                        if (otm.entityid == relationship_entityid)
                        {
                            if (!otmRelationships.ContainsKey(otm.attributeid))
                            {
                                otmRelationships.Add(otm.attributeid, otm);
                            }
                        }
                        else
                        {
                            this.getRelationshipAttributes(otm.entityid, relationship_entityid, ref otmRelationships);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of items representing an entity's relationship views
        /// </summary>
        /// <param name="entityid">Entity ID</param>
        /// <returns>Returns a list of items representing an entity's relationship views</returns>
        public static List<ListItem> GetRelationshipViews(string entityid,int AttrId)
        {
            List<ListItem> items = new List<ListItem>();
            if (entityid != null && entityid != "0")
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                cCustomEntities clsentities = new cCustomEntities(currentUser);
                cFields clsFields = new cFields(currentUser.AccountID);
                cCustomEntity entity = clsentities.getEntityByTableId(new Guid(entityid));

                cField reqField = clsFields.GetFieldByID(new Guid(entityid));
                if (entity != null)
                {
                    SortedList<string, cCustomEntityView> views = entity.sortViews();
                    List<int> lstviewid = getViewsId(views);
                    List<string> lstviewname = getViewName(views);
                    DBConnection expdata = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));
                    int count = 0;
                    string allviewid = "";
                    foreach (int exsitviewid in lstviewid)
                    {
                        if (allviewid!="")
                        {
                            allviewid += ",";

                        }
                        allviewid += exsitviewid;
                        count ++;
                    }

                    var strSQL = "SELECT  distinct viewid,view_name from customEntityViews where  customEntityViews.viewid not  in(select viewid from customEntityAttributes where customEntityViews.viewid=customEntityAttributes.viewid) and viewid in ("+allviewid+")";

                    using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
                    {
                        if (reader != null && reader.HasRows)
                        {
                            while (reader.Read())
                            {

                                items.AddRange(
                                    views.Values.Select(
                                        view =>
                                            new ListItem(reader["view_name"].ToString(), reader["viewid"].ToString()))
                                        .Distinct());
                            }
                        }
                        reader.Close();

                    }

                    if (AttrId != 0)
                    {
                        var Query = "SELECT distinct  customEntityAttributes.viewid, customEntityViews.view_name FROM customEntityAttributes  JOIN customEntityViews ON customEntityAttributes.viewid = customEntityViews.viewid where customEntityAttributes.viewid in(" + allviewid + ") and customEntityAttributes.attributeid=" + AttrId;
                        using (System.Data.SqlClient.SqlDataReader reader1 = expdata.GetReader(Query))
                        {
                            if (reader1 != null && reader1.HasRows)
                            {
                                while (reader1.Read())
                                {
                                    items.AddRange(
                            views.Values.Select(
                                view =>
                                    new ListItem(reader1["view_name"].ToString(), reader1["viewid"].ToString()))
                                .Distinct());

                                }

                            }
                            reader1.Close();
                        }

                    }
                }
            }
            items.Insert(0, new ListItem("[None]", "0"));
            return items;

        }

        private static List<int> getViewEntityId(SortedList<string, cCustomEntityView> views)
        {
            var lstViewId = new List<int>();
            foreach (var view in views)
            {

                lstViewId.Add(view.Value.entityid);
            }
            return lstViewId;
        }


        private static List<int> getViewsId(SortedList<string, cCustomEntityView> views)
        {
            var lstViewId = new List<int>();

            foreach (var view in views)
            {
                lstViewId.Add(view.Value.viewid);
            }
            return lstViewId;
        }

        private static List<string> getViewName(SortedList<string, cCustomEntityView> views)
        {
            var lstViewId = new List<string>();

            foreach (var view in views)
            {
                lstViewId.Add(view.Value.viewname);
            }
            return lstViewId;
        }

        public sEntityBreadCrumb getParentBreadcrumb(int entityid, sEntityBreadCrumb topCrumb, int id)
        {
            sEntityBreadCrumb crumb = new sEntityBreadCrumb();
            cCustomEntity curEntity = this.getEntityById(entityid);
            //SortedList<int, object> rec = clsentities.getEntityRecord(curEntity, id, curEntity.getFormById(formid));

            Dictionary<int, cOneToManyRelationship> rels = new Dictionary<int, cOneToManyRelationship>();
            this.getRelationshipAttributes(topCrumb.EntityID, entityid, ref rels);

            int parent_attributeid = 0;
            int parent_recordid = 0;
            bool matched = false;

            foreach (KeyValuePair<int, cOneToManyRelationship> kvp in rels)
            {
                cOneToManyRelationship rel = (cOneToManyRelationship)kvp.Value;
                cQueryBuilder qb = new cQueryBuilder(nAccountid, cAccounts.getConnectionString(nAccountid), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, curEntity.table, tables, fields);
                qb.addColumn(fields.GetFieldByID(rel.fieldid));
                qb.addFilter(new cQueryFilter(fields.GetFieldByID(curEntity.getKeyField().fieldid), ConditionType.Equals, new List<object>() { id }, null, ConditionJoiner.None, null)); // null as on the query bt ?   !!!!!!
                using (IDataReader r = qb.getReader())
                {
                    while (r.Read())
                    {
                        if (!r.IsDBNull(0))
                        {
                            parent_attributeid = rel.attributeid;
                            parent_recordid = r.GetInt32(0);
                            matched = true;
                            break;
                        }
                    }

                    r.Close();
                }

                if (matched)
                {
                    int parent_entityid = this.getEntityIdByAttributeId(parent_attributeid);
                    cCustomEntity parent_entity = this.getEntityById(parent_entityid);

                    cOneToManyRelationship parent_otm = (cOneToManyRelationship)parent_entity.getAttributeById(parent_attributeid);
                    Guid attributeTableId = tables.GetTableByName("customEntityForms").TableID;
                    qb = new cQueryBuilder(nAccountid, cAccounts.getConnectionString(nAccountid), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, tables.GetTableByName("customEntityForms"), tables, fields);
                    qb.addColumn(fields.GetBy(attributeTableId, "formid"));
                    qb.addFilter(new cQueryFilter(fields.GetBy(attributeTableId, "entityid"), ConditionType.Equals, new List<object>() { parent_entityid }, null, ConditionJoiner.None, null)); // null as on bt ? !!!!!!
                    int parent_formid = qb.GetCount();
                    int parent_tabid = 0;
                    int parent_viewid = 0;
                    bool foundtab = false;

                    SortedList<byte, cCustomEntityFormTab> parent_tabs = parent_entity.getFormById(parent_formid).getTabsForForm();
                    foreach (KeyValuePair<byte, cCustomEntityFormTab> t in parent_tabs)
                    {
                        cCustomEntityFormTab curTab = (cCustomEntityFormTab)t.Value;
                        foreach (cCustomEntityFormSection s in curTab.sections)
                        {
                            foreach (cCustomEntityFormField f in s.fields)
                            {
                                if (f.attribute.attributeid == parent_attributeid)
                                {
                                    parent_tabid = curTab.tabid;
                                    foundtab = true;
                                }
                            }

                            if (foundtab)
                                break;
                        }

                        if (foundtab)
                            break;
                    }

                    foreach (var customEntityView in parent_entity.Views)
                    {
                        if (customEntityView.Value.DefaultAddForm != null && customEntityView.Value.DefaultAddForm.formid == parent_formid)
                        {
                            parent_viewid = customEntityView.Value.viewid;
                            break;
                        }

                        if (customEntityView.Value.DefaultEditForm != null && customEntityView.Value.DefaultEditForm.formid == parent_formid)
                        {
                            parent_viewid = customEntityView.Value.viewid;
                            break;
                        }
                    }

                    crumb = new sEntityBreadCrumb(parent_entityid, parent_formid, parent_recordid, parent_tabid, parent_viewid);
                    break;
                }
            }

            return crumb;
        }

        /// <summary>
        /// Obtains a collection of audience access status for each record in the specified table that has an audience applied
        /// </summary>
        /// <param name="entityID">Entity ID that audience access is being checked</param>
        /// <param name="employeeID">Employee ID of user whose access is being checked</param>
        /// <param name="recordID">Optional parameter that will return status for a single record if required</param>
        /// <returns>Dictionary collection of recordID to AudienceStatus record</returns>
        public SerializableDictionary<string, object> GetAudienceRecords(int entityID, int employeeID, int recordID = 0, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            expdata.sqlexecute.Parameters.Clear();
            SerializableDictionary<string, object> audienceRecords = null;
            var ceCache = new Caching();
            string cacheKey = string.Format("audienceRecords_{0}_{1}_{2}", nAccountid.ToString(), entityID.ToString(), employeeID.ToString());
            if (ceCache.Cache.Contains(cacheKey))
            {
                var allAudienceRecords = (SerializableDictionary<string, object>)ceCache.Cache.Get(cacheKey);

                if (recordID != 0)
                {
                    // only retrieving a single record
                    if (allAudienceRecords.ContainsKey(recordID.ToString()))
                    {
                        audienceRecords = new SerializableDictionary<string, object>
                        {
                            {recordID.ToString(), allAudienceRecords[recordID.ToString()]}
                        };
                    }
                    else
                    {
                        audienceRecords = allAudienceRecords;
                    }
                }
                else
                {
                    audienceRecords = allAudienceRecords;
                }
            }
            else
            {
                audienceRecords = new SerializableDictionary<string, object>();
                cCustomEntity entity = getEntityById(entityID);
                if (entity == null || entity.AudienceTable == null)
                    return null;

                cField keyfield = fields.GetFieldByID(entity.getKeyField().fieldid);

                var sql =
                    new StringBuilder(
                        string.Format(
                            "select CustomEntityAudiences.audienceid, CustomEntityAudiences.employeeid, ISNULL(custom_{0}_audiences.canView,(case WHEN customentities.audienceViewType = 2 and custom_{0}.createdby = @employeeid then 1 ELSE 0 END))   as CanView, ISNULL(custom_{0}_audiences.canEdit, (case WHEN customentities.audienceViewType = 2 and custom_{0}.createdby = @employeeid then 1 ELSE 0 END))   as CanEdit, ISNULL(custom_{0}_audiences.canDelete, (case WHEN customentities.audienceViewType = 2 and custom_{0}.createdby = @employeeid then 1 ELSE 0 END)) as CanDelete, customentities.AudienceViewType , parentid , custom_{0}.{1} as customEntityKey from custom_{0} inner join customEntities on customEntities.entityid = {0} left join custom_{0}_audiences on custom_{0}_audiences.parentID = custom_{0}.{1} left join CustomEntityAudiences on CustomEntityAudiences.audienceID = custom_{0}_audiences.audienceID where (employeeid = @employeeid or (customentities.AudienceViewType = 2 and custom_{0}.createdby = @employeeid)) ",
                            entityID, keyfield.FieldName));

                if (recordID != 0)
                {
                    sql.AppendFormat(" and custom_{0}.{1} = @recordID", entityID, keyfield.FieldName);
                    expdata.sqlexecute.Parameters.AddWithValue("@recordID", recordID);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
                sql.AppendFormat(
                    " SELECT {1}, parentid from custom_{0} left join custom_{0}_audiences on custom_{0}_audiences.parentID = custom_{0}.{1}",
                    entityID, keyfield.FieldName);

                using (IDataReader reader = expdata.GetReader(sql.ToString()))
                {
                    var audienceIdOrd = reader.GetOrdinal("audienceId");
                    var canViewOrd = reader.GetOrdinal("canView");
                    var canEditOrd = reader.GetOrdinal("canEdit");
                    var canDeleteOrd = reader.GetOrdinal("canDelete");
                    var customEntityKeyOrd = reader.GetOrdinal("customEntityKey");

                    while (reader.Read())
                    {
                        int recordid = reader.GetInt32(customEntityKeyOrd);
                        int? audienceId = reader.IsDBNull(audienceIdOrd) ? 0 : reader.GetInt32(audienceIdOrd);
                        bool canView = reader.GetBoolean(canViewOrd);
                        bool canEdit = reader.GetBoolean(canEditOrd);
                        bool canDelete = reader.GetBoolean(canDeleteOrd);

                        AddAudienceToList(audienceRecords, recordid, audienceId, canView, canEdit, canDelete, 1);
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            var recordId = reader.GetInt32(0);
                            var audienceStatus = reader.IsDBNull(1) ? 1 : 0;
                            if (!audienceRecords.ContainsKey(recordId.ToString()))
                            {
                                if (entity.AudienceView == AudienceViewType.AllowAllIfNoneExist)
                                {
                                    AddAudienceToList(audienceRecords, recordId, null, true, true, true, audienceStatus);
                                }
                                else
                                {
                                    AddAudienceToList(audienceRecords, recordId, null, false, false, false, 0);
                                }
                            }
                        }
                    }

                    reader.Close();
                }


                if (audienceRecords.Count > 0 && recordID == 0)
                {
                    string cachedep =
                        string.Format("select CacheExpiry from dbo.[{0}] where {1} = {1} and {2} = {2} and {3} = {3}",
                            entity.AudienceTable.TableName, nAccountid.ToString(), entity.entityid.ToString(),
                            employeeID.ToString());

                    string audienceMemberDep = string.Format("select modifiedon from dbo.audiences where {0} = {0}",
                        nAccountid.ToString());
                    string customEntityDep =
                        string.Format("select modifiedon from dbo.customEntities where {0} = {0} AND entityID = {1}",
                            nAccountid.ToString(), entityID);

                    var customTableDep = string.Format("SELECT {0} FROM dbo.custom_{1} WHERE {2} = {2}", keyfield.FieldName, entityID, _accountid);
                    ceCache.Add(cacheKey, audienceRecords,
                        new List<string>() {cachedep, audienceMemberDep, customEntityDep, customTableDep}, Caching.CacheTimeSpans.Short,
                        Caching.CacheDatabaseType.Customer, nAccountid);
                }
            }

            return audienceRecords;
        }

        private static void AddAudienceToList(SerializableDictionary<string, object> audienceRecords, int recordid, int? audienceId,
            bool canView, bool canEdit, bool canDelete, int status)
        {
            if (!audienceRecords.ContainsKey(recordid.ToString()))
            {
                var audRec = new cAudienceRecordStatus(recordid, audienceId, status, canView, canEdit, canDelete);
                audienceRecords.Add(recordid.ToString(), audRec);
            }
            else
            {
                var curStatus = (cAudienceRecordStatus) audienceRecords[recordid.ToString()];

                if (canView)
                {
                    curStatus.Status += 1;
                    if (canDelete)
                    {
                        curStatus.CanDelete = true;
                    }

                    if (canEdit)
                    {
                        curStatus.CanEdit = true;
                    }

                    curStatus.CanView = true;

                    audienceRecords[recordid.ToString()] = curStatus;
                }
            }
        }

        /// <summary>
        /// Delete an individual record for an entity
        /// </summary>
        /// <param name="entity">The entity type</param>
        /// <param name="recordid">The record id to delete</param>
        /// <param name="viewid">The view id the record came from</param>
        /// <returns></returns>
        public int DeleteCustomEntityRecord(cCustomEntity entity, int recordid, int viewid)
        {
            // Check any audience applied permission revoke
            if (entity.AudienceView != AudienceViewType.NoAudience)
            {
                SerializableDictionary<string, object> audienceStatus = this.GetAudienceRecords(entity.entityid, oCurrentUser.EmployeeID, recordid);
                if (audienceStatus.ContainsKey(recordid.ToString()))
                {

                    cAudienceRecordStatus audRecStatus = (cAudienceRecordStatus)audienceStatus[recordid.ToString()];
                    if (audRecStatus.Status == 0 || (audRecStatus.Status > 0 && !audRecStatus.CanDelete))
                    {
                        return -1;
                    }
                }
            }

            foreach (KeyValuePair<int, cAttribute> a in entity.attributes)
            {
                cAttribute att = (cAttribute)a.Value;
                if (att.fieldtype == FieldType.Relationship && att.GetType() == typeof(cOneToManyRelationship))
                {
                    // need to remove child record(s) for this record
                    cOneToManyRelationship rel = (cOneToManyRelationship)att;
                    cCustomEntity related_entity = this.getEntityByTableId(rel.relatedtable.TableID);

                    cCustomEntityView onetomanyview = related_entity.getViewById(rel.viewid);
                    cCustomEntity query_entity = this.getEntityById(related_entity.entityid);

                    cQueryBuilder qry = new cQueryBuilder(this.oCurrentUser.AccountID, cAccounts.getConnectionString(this.oCurrentUser.AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, query_entity.table, this.tables, this.fields);

                    cField tmpField;
                    string alias = query_entity.getKeyField().attributeid.ToString();

                    tmpField = fields.GetBy(query_entity.table.TableID, query_entity.getKeyField().attributename);

                    qry.addFilter(fields.GetBy(query_entity.table.TableID, "att" + rel.attributeid.ToString()), ConditionType.Equals, new object[] { recordid }, null, ConditionJoiner.None, null); // null as always on bt ? !!!!!!!

                    qry.addColumn(tmpField, alias);

                    int related_recordid;
                    List<int> relatedRecordList = new List<int>();

                    using (IDataReader reader = qry.getReader())
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                related_recordid = Convert.ToInt32(reader.GetValue(0));
                                relatedRecordList.Add(related_recordid);
                            }
                        }

                        reader.Close();
                    }

                    if (relatedRecordList.Count > 0)
                    {
                        try
                        {
                            // need to recursively call delete routine to ensure all child records are deleted.
                            foreach (int recordID in relatedRecordList)
                            {
                                DeleteCustomEntityRecord(related_entity, recordID, onetomanyview.viewid);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }

            //string sql = "delete from " + tablename + " where " + keyfield.attributename + " = @recordid";
            int retVal = deleteEntityRecord(entity, entity.getKeyField().attributeid, recordid, viewid);
            return retVal;
        }

        /// <summary>
        /// deleteEntityRecord: delete the record from the database
        /// </summary>        
        /// <param name="entity">Entity class of the record's parent entity</param>
        /// <param name="recordMatchAttributeId">Attribute to match record id against for deletion condition (attnnn = @recordid)</param>
        /// <param name="recordid">ID of the data record to be deleted</param>
        /// <param name="viewid">ID of the custom entity view</param>
        /// <returns>Zero if record deleted successfully, -10 if record referenced in CE or UDF, -99 Error occurred</returns>
        private int deleteEntityRecord(cCustomEntity entity, int recordMatchAttributeId, int recordid, int viewid, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            expdata.sqlexecute.Parameters.Clear();
            try
            {
                cCustomEntityView view = entity.getViewById(viewid);
                string auditIdentifierData = string.Empty;

                if (view != null && view.DefaultEditForm != null)
                {
                    SortedList<int, object> record = getEntityRecord(entity, recordid, view.DefaultEditForm);
                    var locked = CustomEntityRecordLocking.IsRecordLocked(entity.entityid, recordid,
                        cMisc.GetCurrentUser());
                    if (locked.EmployeeId != 0)
                    {
                        return -100;
                    }
                    cAttribute auditAtt = entity.getAuditIdentifier();

                    if (record.ContainsKey(auditAtt.attributeid))
                    {
                        string fieldData = formatFieldData(auditAtt, record[auditAtt.attributeid].ToString());

                        if (fieldData != string.Empty)
                        {
                            auditIdentifierData = fieldData;
                        }
                    }
                    else if (auditAtt.IsSystemAttribute && auditAtt.iskeyfield)
                    {
                        auditIdentifierData = recordid.ToString();
                    }
                }

                if (auditIdentifierData == string.Empty)
                {
                    // Audit identifier data is of no use, display the record ID
                    auditIdentifierData = entity.entityname + " (Record ID: " + recordid.ToString() + ")";
                }
                else
                {
                    auditIdentifierData = entity.entityname + " (" + cMisc.DotonateString(auditIdentifierData, 400) + ")";
                }

                if (entity.IsSystemView)
                {
                    cCustomEntity derived_entity = this.getEntityById(entity.SystemView_DerivedEntityId.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@entityid", derived_entity.entityid);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@entityid", entity.entityid);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@recordMatchAttributeId", recordMatchAttributeId);
                expdata.sqlexecute.Parameters.AddWithValue("@recordid", recordid);
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
                if (this.oCurrentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@recordTitle", auditIdentifierData);
                expdata.sqlexecute.Parameters.Add("@retCode", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@retCode"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("deleteCustomEntityRecord");

                int retCode = (int)expdata.sqlexecute.Parameters["@retCode"].Value;

                return retCode;
            }
            catch
            {
                return -99;
            }
        }

        /// <summary>
        /// ArchiveCustomEntityRecord: set the archive status of the record true/false
        /// </summary>        
        /// <param name="entity">Entity class of the record's parent entity</param>
        /// <param name="recordMatchAttributeId">Attribute to match record id against for deletion condition (attnnn = @recordid)</param>
        /// <param name="recordid">ID of the data record to be deleted</param>
        /// <param name="viewid">ID of the custom entity view</param>
        /// <param name="archive">ID of the custom entity view</param>
        /// <returns>Zero if record deleted successfully, -10 if record referenced in CE or UDF, -99 Error occurred</returns>
        public int ArchiveCustomEntityRecord(int entityId, int recordMatchAttributeId, int recordId, int viewId,bool archive)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities customEntities = new cCustomEntities(currentUser);
            cCustomEntity entity = customEntities.getEntityById(entityId);          
            var expdata = new DatabaseConnection(cAccounts.getConnectionString(this._accountid));        
            expdata.sqlexecute.Parameters.Clear();
            try
            {
                cCustomEntityView view = entity.getViewById(viewId);
                string auditIdentifierData = string.Empty;

                if (view != null && view.DefaultEditForm != null)
                {
                    SortedList<int, object> record = getEntityRecord(entity, recordId, view.DefaultEditForm);
                   
                    cAttribute auditAtt = entity.getAuditIdentifier();               
                 
                    if (record.ContainsKey(auditAtt.attributeid))
                    {
                        string fieldData = formatFieldData(auditAtt, record[auditAtt.attributeid].ToString());

                        if (fieldData != string.Empty)
                        {
                            auditIdentifierData = fieldData;
                        }
                    }
                    else if (auditAtt.IsSystemAttribute && auditAtt.iskeyfield)
                    {
                        auditIdentifierData = recordId.ToString();
                    }
                }

                if (auditIdentifierData == string.Empty)
                {
                    // Audit identifier data is of no use, display the record ID
                    auditIdentifierData = entity.entityname + " (Record ID: " + recordId.ToString() + ")";
                }
                else
                {
                    auditIdentifierData = entity.entityname + " (" + cMisc.DotonateString(auditIdentifierData, 400) + ")";
                }

                if (entity.IsSystemView)
                {
                    cCustomEntity derived_entity = this.getEntityById(entity.SystemView_DerivedEntityId.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@entityid", derived_entity.entityid);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@entityid", entity.entityid);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@recordMatchAttributeId", recordMatchAttributeId);
                expdata.sqlexecute.Parameters.AddWithValue("@recordid", recordId);
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", oCurrentUser.EmployeeID);
                expdata.sqlexecute.Parameters.AddWithValue("@archived", Convert.ToByte(archive));
                if (this.oCurrentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", oCurrentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@recordTitle", auditIdentifierData);
                expdata.sqlexecute.Parameters.Add("@retCode", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@retCode"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("ChangeCustomEntityRecordArchiveStatus");

                int retCode = (int)expdata.sqlexecute.Parameters["@retCode"].Value;
                return retCode;


            }
            catch(Exception ex)
            {
                return -99;
            }
        }

        /// <summary>
        /// Performs deletion of an individual custom entity data record
        /// </summary>
        /// <param name="entityid">ID of the record's parent entity</param>
        /// <param name="recordid">ID of the record to be deleted</param>
        /// <param name="viewid">ID of the view the record belongs to</param>
        /// <param name="attributeid">ID of the custom entity form</param>
        /// <param name="curUser">Current User</param>
        /// <param name="dsGrid">Dataset representing the grid of entities</param>
        /// <returns></returns>
        public static object[] DeleteCustomEntityRecord(int entityid, int recordid, int viewid, int attributeid, CurrentUser curUser, ref DataSet dsGrid)
        {
            cCustomEntities clsentities = new cCustomEntities(curUser);
            cCustomEntity entity = clsentities.getEntityById(entityid);
            string sGridID = "grid" + entityid.ToString() + viewid.ToString() + attributeid.ToString();

            int successCode = clsentities.DeleteCustomEntityRecord(entity, recordid, viewid);
            if (successCode != 0)
            {
                return new object[] { null, successCode, null }; //new object[] {};
            }
            else
            {
                int firstaffectedtabid = getAffectedTabIds(attributeid, curUser.AccountID).FirstOrDefault();

                // alter grid in session memory
                if (dsGrid == null)
                    return new object[] { sGridID, recordid, firstaffectedtabid };

                foreach (DataRow row in dsGrid.Tables[0].Rows.Cast<DataRow>().Where(row => row[entity.getKeyField().attributename].ToString() == recordid.ToString()))
                {
                    dsGrid.Tables[0].Rows.Remove(row);
                    break;
                }

                return new object[] { sGridID, recordid, firstaffectedtabid };
            }
        }

        /// <summary>
        /// Evaluates the entity instances to see if the data for a particular attribute is unique or not.
        /// </summary>
        /// <param name="entityid">Entity ID</param>
        /// <param name="attributeid">Attribute ID</param>
        /// <returns></returns>
        public bool IsCustomEntityAttributeUniqueInInstances(int entityid, int attributeid, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid));
            IDataReader reader;
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@entityid", entityid);
            expdata.sqlexecute.Parameters.AddWithValue("@attributeid", attributeid);
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            int count = 0;

            using (reader = expdata.GetReader("customEntityAttributeIsDataUnique", CommandType.StoredProcedure))
            {
                while (reader.Read())
                {
                    count = reader.GetInt32(0);
                    break;
                }

                expdata.sqlexecute.Parameters.Clear();
            }

            if (count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns True if the user is allowed access to the speicified recordID
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="recordID"></param>
        /// <returns></returns>
        public bool IsUserAllowedAccessToAudienceRecord(cCustomEntity entity, int recordID)
        {
            cAudienceRecordStatus currentRecordAudienceStatus = GetAudienceRecordStatus(entity, recordID);

            if (currentRecordAudienceStatus != null)
            {
                if (currentRecordAudienceStatus.Status == 0 || (currentRecordAudienceStatus.Status > 0 && (!currentRecordAudienceStatus.CanView || !currentRecordAudienceStatus.CanEdit)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the cAudienceRecordStatus for the specified recod
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="recordID"></param>
        /// <returns></returns>
        public cAudienceRecordStatus GetAudienceRecordStatus(cCustomEntity entity, int recordID)
        {
            cAudienceRecordStatus currentRecordAudienceStatus = null;
            if (entity.AudienceView != AudienceViewType.NoAudience && recordID > 0)
            {
                SerializableDictionary<string, object> audienceStatus = GetAudienceRecords(entity.entityid, oCurrentUser.EmployeeID, recordID);
                if (audienceStatus.ContainsKey(recordID.ToString()))
                {
                    currentRecordAudienceStatus = (cAudienceRecordStatus)audienceStatus[recordID.ToString()];
                }
            }

            return currentRecordAudienceStatus;
        }


        /// <summary>
        /// Returns the Master Page Title for the given recordID
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="form"></param>
        /// <param name="entityCrumbs"></param>
        /// <returns></returns>
        public string GetMasterPageTitle(cCustomEntity entity, cCustomEntityForm form, List<sEntityBreadCrumb> entityCrumbs)
        {
            string title = "New " + entity.entityname;

            int recordID = entityCrumbs.Last().RecordID;

            if (recordID > 0)
            {
                title = "Edit";

                if (entityCrumbs != null)
                {
                    SortedList<int, object> topRecord = getEntityRecord(entity, recordID, form);

                    cAttribute auditAtt = entity.getAuditIdentifier();
                    if (topRecord.ContainsKey(auditAtt.attributeid))
                    {
                        string fieldData = formatFieldData(auditAtt, topRecord[auditAtt.attributeid].ToString());

                        if (fieldData.Trim() != string.Empty)
                        {
                            title = entity.entityname + ": " + cMisc.DotonateString(fieldData, 36);
                        }
                    }

                    // If the audit identifier is not on the form, check to see if it is the ID field
                    else if (auditAtt.IsSystemAttribute && auditAtt.iskeyfield && recordID > 0)
                    {
                        title = entity.entityname + ": " + recordID.ToString();
                    }
                }
            }

            return title;
        }

        /// <summary>
        /// Gets breadcrumbs links HTML between the Home page and Custom Entity according to the menu it is loaded from
        /// </summary>
        /// <param name="rootEntity">Custom Entity being accessed</param>
        /// <param name="rootViewID">ID of theCustom Entity View accessed from the menu</param>
        /// <returns>HTML links to amend into the sitemap</returns>
        public string GetInnerBreadcrumbs(cCustomEntity rootEntity, int rootViewID)
        {
            cCustomEntityView rootView = rootEntity.getViewById(rootViewID);
            customMenu = new CustomMenuStructure(oCurrentUser.AccountID);
            if (rootView == null && rootEntity.Views.Count > 0)
            {
                rootView = rootEntity.Views.Values[0];
            }

            string bcrumbs = String.Empty;

            if (rootView != null && rootView.menuid.HasValue)
            {
                customMenu = new CustomMenuStructure(oCurrentUser.AccountID);
                var customMenuItem = customMenu.GetCustomMenuById(rootView.menuid.Value);
                var customMenuNodes = new CustomMenuNodes();
                int breadcrumbLength = 0;
                bool isTruncated = false;
                if (customMenuItem.CustomParentId > 0)
                {
                    bcrumbs = customMenuNodes.GenerateCustomMenuBreadcrumb(customMenuItem, ref breadcrumbLength, ref isTruncated);
                }
                cCustomEntities entities = new cCustomEntities(oCurrentUser);
                var title = customMenuItem.CustomMenuName;
                var menuUrl = string.Empty;
                entities.GetMenuUrl(customMenuItem.CustomMenuId, ref title, ref menuUrl, customMenuItem.SystemMenu);

                if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(menuUrl))
                {
                    breadcrumbLength +=  title.Trim().Length;

                    if (isTruncated)
                    {
                        return bcrumbs;
                    }

                    if (breadcrumbLength > 75)
                    {
                        bcrumbs += "<li><label class='breadcrumb_arrow'>/</label><span>...</span></li>";
                    }
                    else
                    {
                        bcrumbs += string.Format("<li><label class='breadcrumb_arrow'>/</label><a class='breadcrumbtitle' href='{0}'>{1}</a></li>", menuUrl, title);
                    }
                }
            }
            return bcrumbs;
        }

        /// <summary>
        /// Iterates up through the menu item parents and returns links until top menu hit
        /// </summary>
        /// <param name="menu">Menu to start from</param>
        /// <returns>HTML string of links</returns>
        private string GenerateBreadcrumb(cMenuElement menu)
        {
            var menuStringBuilder = new StringBuilder();

            if (menu.parent != null)
            {
                menuStringBuilder.Append(this.GenerateBreadcrumb(menu.parent));
            }

            var title = string.Empty;
            var menuUrl = string.Empty;
            this.GetMenuUrl(menu.menuid, ref title, ref menuUrl, true);
            if (title != string.Empty && menuUrl != string.Empty)
            {
                menuStringBuilder.Append("<li><label class=\"breadcrumb_arrow\">/</label><a title=\"" + title + "\" style=\"text-decoration:none;\" href=\"" + menuUrl + "\">" + title + "</a></li>");
            }
            return menuStringBuilder.ToString();
        }

        /// <summary>
        /// Gets the menu title and URL to a menu item depending on the active module
        /// </summary>
        /// <param name="menuId">Menu Id to retrieve title and url for</param>
        /// <param name="title">Contains the title set by the routine</param>
        /// <param name="menuURL">Contains the url set by the routine</param>
        /// <param name="isSystemMenu">If the Menu is SystemMenu</param>
        public void GetMenuUrl(int menuId, ref string title, ref string menuURL,bool isSystemMenu= true)
        {
            string menuTitle =string.Empty;
            menuTitle = new CustomMenuStructure(this.nAccountid).GetCustomMenuNameById(menuId, true);

            switch (this.oCurrentUser.CurrentActiveModule)
            {
                case Modules.contracts:
                case Modules.SmartDiligence:
                    switch (menuId)
                    {
                        case 1: // home - already done
                            break;
                        case 2: // admin
                            title = "Administrative Settings";
                            menuURL = cMisc.Path + "/MenuMain.aspx?menusection=admin";
                            break;
                        case 3: // base info
                            title = "Base Information";
                            menuURL = cMisc.Path + "/MenuMain.aspx?menusection=baseinfo";
                            break;
                        case 4: // tailoring
                            title = "Tailoring";
                            menuURL = cMisc.Path + "/MenuMain.aspx?menusection=tailoring";
                            break;
                        case 5: // policy information - n/a
                            break;
                        case 6: // user management
                            title = "Employee Management";
                            menuURL = cMisc.Path + "/MenuMain.aspx?menusection=employee";
                            break;
                        case 7: // system options
                            title = "System Options";
                            menuURL = cMisc.Path + "/MenuMain.aspx?menusection=sysoptions";
                            break;
                    }
                    break;
                case Modules.expenses:
                case Modules.Greenlight:
                case Modules.GreenlightWorkforce:
                    switch (menuId)
                    {
                        case 1: // home - already done
                            break;
                        case 2: // admin
                            title = "Administrative Settings";
                            menuURL = cMisc.Path + "/adminmenu.aspx";
                            break;
                        case 3: // base info
                            title = "Base Information";
                            menuURL = cMisc.Path + "/categorymenu.aspx";
                            break;
                        case 4: // tailoring
                            title = "Tailoring";
                            menuURL = cMisc.Path + "/tailoringmenu.aspx";
                            break;
                        case 5: // policy information - n/a
                            title = "Policy Information";
                            menuURL = cMisc.Path + "/policymenu.aspx";
                            break;
                        case 6: // user management
                            title = "User Management";
                            menuURL = cMisc.Path + "/usermanagementmenu.aspx";
                            break;
                        case 7: // system options
                            title = "System Options";
                            menuURL = cMisc.Path + "/shared/menu.aspx?area=systemoptions";
                            break;
                        default:
                            if (menuTitle == "My Details" && isSystemMenu)
                            {
                                menuURL = cMisc.Path + "/mydetailsmenu.aspx";
                            }
                            else
                            {
                                menuURL = string.Format("/shared/ViewCustomMenu.aspx?menuid={0}", menuId);
                            }
                            break;
                    }
                    break;
            }
        }
        

        /// <summary>
        /// Get the initial entity tree
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public JavascriptTreeData GetEntityData(int entityID)
        {
            JavascriptTreeData javascriptData = new JavascriptTreeData();
            JavascriptTreeData.JavascriptTreeNode node;
            List<JavascriptTreeData.JavascriptTreeNode> lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();

            cCustomEntity oCustomEntity = this.getEntityById(entityID);

            // g for Group - table join, k for node linK - foreign key join, n for node - field
            string guidPrefix = string.Empty;

            if (oCustomEntity != null)
            {
                foreach (cAttribute a in oCustomEntity.attributes.Values)
                {
                    if (a.GetType() == typeof(cCommentAttribute) || a.GetType() == typeof(cOneToManyRelationship) || a.GetType() == typeof(cSummaryAttribute) || a.fieldtype == FieldType.LookupDisplayField)
                    {
                        continue;
                    }

                    node = TreeViewNodes.CreateJavascriptNode(a);
                    lstNodes.Add(node);
                }

                lstNodes = lstNodes.OrderBy(x => x.data).ToList();
            }

            javascriptData.data = lstNodes;

            return javascriptData;
        }

        /// <summary>
        /// Get the initial nodes for a One to Many relationship
        /// </summary>
        /// <param name="relatedTable"></param>
        /// <returns></returns>
        public JavascriptTreeData GetManyToOneData(string relatedTable)
        {
            JavascriptTreeData javascriptData = new JavascriptTreeData();
            Guid tableID;

            if (Guid.TryParse(relatedTable, out tableID))
            {
                CurrentUser curUser = cMisc.GetCurrentUser();
                JavascriptTreeData.JavascriptTreeNode node;
                List<JavascriptTreeData.JavascriptTreeNode> lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();

                // g for Group - table join, k for node linK - foreign key join, n for node - field
                string guidPrefix = string.Empty;

                foreach (cField f in fields.GetFieldsByTableIDForViews(tableID))
                {
                    if (f.IDField)
                    {
                        continue;
                    }

                    if (f.FieldName == "Password")
                    {
                        continue;
                    }

                    node = TreeViewNodes.CreateJavascriptNode(f);

                    if (node != null)
                    {
                        lstNodes.Add(node);
                    }
                }

                lstNodes = lstNodes.OrderBy(x => x.data).ToList();

                #region Output a User Defined Fields folder if needed

                cTable relTable = tables.GetTableByID(tableID);

                if (relTable.HasUserdefinedFields)
                {
                    if ((from x in fields.GetFieldsByTableIDForViews(relTable.UserDefinedTableID)
                         where x.FieldSource != cField.FieldSourceType.Metabase
                         select x).Any())
                    {
                        node = TreeViewNodes.CreateGroupJavascriptNode(relTable);

                        lstNodes.Insert(0, node);
                    }
                }

                #endregion Output a User Defined Fields folder if needed

                javascriptData.data = lstNodes;
            }

            return javascriptData;
        }



        /// <summary>
        /// Returns a list of tables/keys/fields for a table/key expansion
        /// </summary>
        /// <param name="AssociatedID">This can be the tableID if not a sub node or the viewgroupID if is a sub node</param>
        /// <param name="prefixID"></param>
        /// <returns></returns>
        public JavascriptTreeData GetNodeData(Guid AssociatedID, string prefixCrumbs, string prefixID, bool filterComments = false)
        {
            JavascriptTreeData javascriptData = new JavascriptTreeData();
            JavascriptTreeData.JavascriptTreeNode node;
            List<JavascriptTreeData.JavascriptTreeNode> lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();
            var customEntityImageData = new Guid("92A15F16-BCEC-4666-8478-1EF83ED3D076");
            cField field = null;
            Guid relatedTabledID = Guid.Empty;
            string crumbDescription = String.Empty;
            bool skipIDField = false;

            // g for Group - table join, k for node linK - foreign key join, n for node - field, x related table link
            string guidPrefix = String.Empty;

            switch (prefixID.Split('_').Last().Substring(0, 1))
            {
                case "g":
                    relatedTabledID = AssociatedID;
                    crumbDescription = "User Defined Fields";
                    skipIDField = true;
                    break;
                case "x":
                    var relatedFieldId = AssociatedID;
                    var relatedField = fields.GetFieldByID(relatedFieldId);
                    var relatedTable = relatedField.GetParentTable();
                    relatedTabledID = relatedTable.TableID;
                    crumbDescription = relatedTable.Description;
                    skipIDField = true;
                    if (!relatedTable.AllowReportOn)
                    {
                        relatedTabledID = Guid.Empty;
                    }

                    break;
                default:
                    field = fields.GetFieldByID(AssociatedID);

                    if (field != null)
                    {
                        relatedTabledID = field.RelatedTableID;
                        crumbDescription = field.Description;
                    }
                    break;
            }

            if (relatedTabledID != Guid.Empty)
            {
                List<cField> fieldsByTableIDForViews = fields.GetFieldsByTableIDForViews(relatedTabledID);
                string preCrumb = (String.IsNullOrWhiteSpace(prefixCrumbs)) ? String.Empty : new StringBuilder(prefixCrumbs).Append(": ").ToString();

                foreach (cField f in fieldsByTableIDForViews)
                {
                    if (f.FieldType == "L")
                    {
                        continue;
                    }

                    if (filterComments && relatedTabledID != customEntityImageData)
                    {
                        var ce = this.getAttributeByFieldId(f.FieldID);
                        if (ce != null && ce.fieldtype == FieldType.Comment)
                        {
                            continue;
                        }
                    }
                    if (field != null)
                    {
                        if (skipIDField && field.IDField)
                        {
                            continue;
                        }

                        if (field.FieldName == "Password")
                        {
                            continue;
                        }
                    }

                    node = TreeViewNodes.CreateJavascriptNode(prefixID, f, preCrumb, crumbDescription);

                    lstNodes.Add(node);
                }

                lstNodes = lstNodes.DistinctBy(x => x.data).OrderBy(x => x.data).ToList();

                if (field != null && field.GetRelatedTable() != null && field.GetRelatedTable().HasUserdefinedFields)
                {
                    List<cField> userDefinedFields = fields.GetFieldsByTableIDForViews(field.GetRelatedTable().UserDefinedTableID);

                    if ((from x in userDefinedFields where x.FieldSource != cField.FieldSourceType.Metabase select x).Any())
                    {
                        node = TreeViewNodes.CreateGroupJavascriptNode(prefixID, field, preCrumb);

                        lstNodes.Insert(0, node);
                    }
                }
            }

            javascriptData.data = lstNodes;

            return javascriptData;
        }

        /// <summary>
        /// Get the initial selected columns tree
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="viewID"></param>
        /// <returns></returns>
        public JavascriptTreeData GetEntitySelectedColumnData(int entityID, int viewID)
        {
            JavascriptTreeData javascriptData = new JavascriptTreeData();
            JavascriptTreeData.JavascriptTreeNode node;
            List<JavascriptTreeData.JavascriptTreeNode> lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();

            cCustomEntity oCustomEntity = this.getEntityById(entityID);

            if (oCustomEntity == null)
            {
                return null;
            }

            cCustomEntityView oView = oCustomEntity.getViewById(viewID);

            if (oView == null)
            {
                return null;
            }

            // g for Group - table join, k for node linK - foreign key join, n for node - field
            StringBuilder nodeID;

            foreach (cCustomEntityViewField vf in oView.fields.Values)
            {

                if (vf.Field != null)
                {
                    node = TreeViewNodes.CreateCustomEntityViewFieldJavascriptNode(vf, oCustomEntity.table.TableID);

                    lstNodes.Add(node);
                }
            }

            //lstNodes = lstNodes.OrderBy(x => x.data).ToList();


            javascriptData.data = lstNodes;

            return javascriptData;
        }


        /// <summary>
        /// Get the initial selected columns tree for Many to One attributes
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public JavascriptTreeData GetManyToOneSelectedColumnData(int entityID, int attributeID, int formid, bool IsParentFilter)
        {
            JavascriptTreeData javascriptData = new JavascriptTreeData();
            JavascriptTreeData.JavascriptTreeNode node;
            List<JavascriptTreeData.JavascriptTreeNode> lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();

            cCustomEntity oCustomEntity = getEntityById(entityID);
            if (oCustomEntity == null)
            {
                return null;
            }

            cAttribute attribute = oCustomEntity.getAttributeById(attributeID);
            if (attribute == null)
            {
                return null;
            }

            cManyToOneRelationship manyToOne = (cManyToOneRelationship)attribute;

            // g for Group - table join, k for node linK - foreign key join, n for node - field
            StringBuilder nodeID;
            Dictionary<string, object> metadata;

            List<string> duplicateFilters = new List<string>();

            foreach (FieldFilter vf in manyToOne.filters.Values)
            {
                if (vf.Field != null && vf.FormId == formid && vf.IsParentFilter == IsParentFilter)
                {
                    node = TreeViewNodes.CreateJavascriptNode(vf, duplicateFilters, this.fields, this.oCurrentUser);

                    lstNodes.Add(node);
                }
            }

            javascriptData.data = lstNodes;

            return javascriptData;
        }

        /// <summary>
        /// Get the initial selected filters tree
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="viewID"></param>
        /// <param name="accountID"></param>
        /// <param name="subAccountID"></param>
        /// <returns></returns>
        public JavascriptTreeData GetEntitySelectedFilterData(int entityID, int viewID, int accountID, int subAccountID)
        {
            JavascriptTreeData javascriptData = new JavascriptTreeData();
            JavascriptTreeData.JavascriptTreeNode node;
            List<JavascriptTreeData.JavascriptTreeNode> lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();

            cCustomEntity oCustomEntity = this.getEntityById(entityID);

            if (oCustomEntity == null)
            {
                return null;
            }

            cCustomEntityView oView = oCustomEntity.getViewById(viewID);

            if (oView == null)
            {
                return null;
            }

            // g for Group - table join, k for node linK - foreign key join, n for node - field
            StringBuilder nodeID;
            Dictionary<string, object> metadata;

            List<string> duplicateFilters = new List<string>();

            foreach (FieldFilter vf in oView.filters.Values)
            {
                if (vf.Field != null)
                {
                    node = TreeViewNodes.CreateJavascriptNode(vf, duplicateFilters, this.fields, this.oCurrentUser);
                    lstNodes.Add(node);
                }
            }

            javascriptData.data = lstNodes;

            return javascriptData;
        }

        /// <summary>
        /// Get the initial selected Lookup Display Fields tree for Many to One attributes
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public JavascriptTreeData GetManyToOneSelectedLookupDisplayFields(int entityID, int attributeID)
        {
            JavascriptTreeData javascriptData = new JavascriptTreeData();
            JavascriptTreeData.JavascriptTreeNode node;
            List<JavascriptTreeData.JavascriptTreeNode> lstNodes = new List<JavascriptTreeData.JavascriptTreeNode>();

            cCustomEntity oCustomEntity = getEntityById(entityID);
            if (oCustomEntity == null)
            {
                return null;
            }

            cAttribute attribute = oCustomEntity.getAttributeById(attributeID);
            if (attribute == null)
            {
                return null;
            }

            var manyToOne = (cManyToOneRelationship)attribute;

            // g for Group - table join, k for node linK - foreign key join, n for node - field

            var duplicateLookupDisplayFields = new List<string>();

            foreach (LookupDisplayField ldf in manyToOne.TriggerLookupFields)
            {
                node = TreeViewNodes.CreateLookupDisplayFieldJavascriptNode(ldf, duplicateLookupDisplayFields, this.fields);
                lstNodes.Add(node);
            }

            javascriptData.data = lstNodes;

            return javascriptData;
        }

        #region filter tree metadata

        /// <summary>
        /// The javascript metadata for a jsTree filter object
        /// </summary>
        public class FilterJsTreeMetadata
        {
            public string value = string.Empty;

            public ConditionType conditionType = ConditionType.Equals;
        }

        #endregion filter tree metadata

        /// <summary>
        /// Gets a list of icons from the Static Library collection
        /// </summary>
        /// <param name="fileName">The file name to match</param>
        /// <param name="searchStartNumber">The search number to start from</param>
        /// <param name="staticContentFolderPath">The folder path for Static Library</param>
        /// <param name="staticContentLibrary">The URL path for Static Library</param>
        /// <returns>A wildcard-matched list of icons</returns>
        public static ViewMenuIconResults GetViewIconsByName(string fileName, int searchStartNumber, string staticContentFolderPath, string staticContentLibrary)
        {
            var iconResults = new ViewMenuIconResults
            {
                MenuIcons = new List<ViewMenuIcon>(),
                ResultStartNumber = (searchStartNumber < 0) ? 0 : searchStartNumber
            };

            Regex regFixFileName = new Regex("[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]");

            if (searchStartNumber >= 0 && regFixFileName.IsMatch(fileName) == false)
            {
                var folderPath = staticContentFolderPath + @"\icons\48\new\";

                var urlPath = staticContentLibrary + "/icons/48/new/";

                fileName = "*" + fileName + "*";

                if (Directory.Exists(folderPath))
                {
                    var iterationNumber = 0;

                    foreach (var s in Directory.GetFiles(folderPath, fileName, SearchOption.TopDirectoryOnly))
                    {
                        if (iterationNumber >= searchStartNumber)
                        {
                            if (iconResults.MenuIcons.Count > 29)
                            {
                                iconResults.FurtherResults = true;
                                break;
                            }

                            var iconName = s.Replace(folderPath, string.Empty);

                            iconResults.MenuIcons.Add(new ViewMenuIcon { IconUrl = urlPath + iconName, IconName = iconName });
                        }

                        iterationNumber++;
                    }

                    iconResults.ResultEndNumber = iterationNumber;
                }
            }

            return iconResults;
        }

        /// <summary>
        /// Returns an integer list of affected tabs for a given attribute.
        /// </summary>
        /// <param name="attributeid">Attribute ID</param>
        /// <param name="curUser"></param>
        /// <returns></returns>
        public static List<int> getAffectedTabIds(int attributeid, int accountid, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid));
            List<int> tmpTabIds = new List<int>();
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@attributeId", attributeid);
            using (IDataReader reader = expdata.GetReader("getCERefreshTabs", CommandType.StoredProcedure))
            {
                while (reader.Read())
                {
                    tmpTabIds.Add(reader.GetInt32(0));
                }

                reader.Close();
            }

            expdata.sqlexecute.Parameters.Clear();
            var affectedTabIds = (from x in tmpTabIds
                                  select x).ToList();
            return affectedTabIds;
        }

        /// <summary>
        /// Gets properties for population on the CE and UDF admin screens when adding a N:1 relationship field type
        /// </summary>
        /// <param name="exclusions"></param>
        /// <param name="relTableID"></param>
        /// <returns></returns>
        public List<ListItem> GetTablesFieldListItemsForManyToOneRelationship(Guid relTableID, List<Guid> exclusions)
        {
            List<ListItem> items = new List<ListItem>();
            List<string> reservedFields = new List<string>() { "createdon", "createdby", "modifiedon", "modifiedby", "id", "password" };

            cTable relTable = tables.GetTableByID(relTableID);

            if (relTable != null)
            {
                List<cField> fieldsInTable = fields.GetFieldsByTableID(relTableID).Where(field => field.FieldType != "L").ToList();

                // if relTable is standard table, only "useforlookup" fields are output
                items = (from x in fieldsInTable
                         where
                             (relTable.TableSource == cTable.TableSourceType.Metabase ? x.UseForLookup == true : true) &&
                             x.IDField == false && x.IsForeignKey == false &&
                             !reservedFields.Contains(x.FieldName.ToLower()) && !exclusions.Contains(x.FieldID) &&
                             x.RelatedTableID != relTableID && x.FieldType.ToLower() != "o" && x.FieldType.ToLower() != "rw"
                         orderby x.Description
                         select new ListItem(x.Description, x.FieldID.ToString())).ToList();
            }

            return items;
        }

        /// <summary>
        /// Gets properties for population on autocomplete display selection fields in CE admin screens when adding a N:1 relationship field type
        /// </summary>
        /// <param name="relatedTableID">Related table from which the fields need to be retrieved</param>
        /// <param name="exclusions">Fields need to be excluded from the selection list</param>
        /// <returns>A <see cref="ListItem"/>List of fields from the related tables</returns>
        public List<ListItem> GetTablesFieldListItemsForAutoCompleteDisplay(Guid relatedTableID, List<Guid> exclusions)
        {
            var items = new List<ListItem>();
            var reservedFields = new List<string>() { "createdon", "createdby", "modifiedon", "modifiedby", "id", "password" };
            var relTable = this.tables.GetTableByID(relatedTableID);

            if (relTable == null)
            {
                return items;
            }

            var fieldsInTable = this.fields.GetFieldsByTableID(relatedTableID).Where(field => field.FieldType != "L").ToList();
            items = (from x in fieldsInTable
                     where
                         x.IDField == false && x.IsForeignKey == false &&
                         !reservedFields.Contains(x.FieldName.ToLower()) && !exclusions.Contains(x.FieldID) &&
                         x.RelatedTableID != relatedTableID && x.FieldType.ToLower() != "o" && x.FieldType.ToLower() != "rw"
                     orderby x.Description
                     select new ListItem(x.Description, x.FieldID.ToString())).ToList();

            return items;
        }

        /// <summary>
        /// Constructs a grid of currently defined GreenLight entities
        /// </summary>
        /// <returns></returns>
        public static string[] createEntityGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridEntities", "select entityid, entity_name, description from customEntities");
            cFields fields = new cFields(user.AccountID);
            cField sysview_field = fields.GetFieldByID(new Guid("3FDA2F02-BDD3-4A4D-AF30-F2264F33E4DC"));
            if (sysview_field != null)
            {
                clsgrid.addFilter(sysview_field, ConditionType.DoesNotEqual, new object[] { 1 }, new object[] { }, ConditionJoiner.None);
            }

            clsgrid.getColumnByName("entityid").hidden = true;
            clsgrid.editlink = "aecustomentity.aspx?entityid={entityid}";
            clsgrid.deletelink = "javascript:deleteEntity({entityid});";
            clsgrid.KeyField = "entityid";
            clsgrid.CssClass = "datatbl";
            clsgrid.EmptyText = "There are no GreenLights to display.";
            clsgrid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.CustomEntities, true);
            clsgrid.enableupdating = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.CustomEntities, true);
          
            cEmployees clsEmployees = new cEmployees(user.AccountID);

            if (user.Employee.GetNewGridSortOrders().GetBy("gridEntities") == null)
            {
                clsgrid.SortedColumn = clsgrid.getColumnByName("entity_name");
                clsgrid.SortDirection = SortDirection.Ascending;
            }

            return clsgrid.generateGrid();
        }

        /// <summary>
        /// Checks for 1:n circular references between two Custom Entities
        /// </summary>
        /// <param name="entityToCheck">The Custom Entity being checked</param>
        /// <param name="curEntity">The Custom Entity that the 1:n is being created on</param>
        /// <returns></returns>
        public bool CheckCircularReferencesForOneToManyEntity(cCustomEntity entityToCheck, cCustomEntity curEntity)
        {
            foreach (cAttribute attribute in entityToCheck.attributes.Values)
            {
                if (attribute.GetType() == typeof(cOneToManyRelationship))
                {
                    cOneToManyRelationship otmRel = (cOneToManyRelationship)attribute;

                    if (otmRel.entityid == curEntity.entityid)
                    {
                        return true;
                    }

                    // Check the next level of 1:n's
                    if (CheckCircularReferencesForOneToManyEntity(getEntityById(otmRel.entityid), curEntity)) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Save a list of form mappings
        /// </summary>
        /// <param name="viewId">The view</param>
        /// <param name="formSelectionMappings">The form selection mappings</param>
        /// <param name="connection">The override connection object for testing</param>
        /// <returns>0 is Success</returns>
        public static int SaveFormSelectionMappings(int viewId, List<FormSelectionMapping> formSelectionMappings, IDBConnection connection = null)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            using (IDBConnection db = connection ?? new DatabaseConnection(currentUser.Account.ConnectionString))
            {
                List<SqlDataRecord> mappings = new List<SqlDataRecord>();
                // Generate a sql table param and pass into the sql
                SqlMetaData[] rowMetaData = {
                                                new SqlMetaData("formSelectionMappingId", SqlDbType.Int),
                                                new SqlMetaData("viewId", SqlDbType.Int),
                                                new SqlMetaData("isAdd", SqlDbType.Bit),
                                                new SqlMetaData("formId", SqlDbType.Int),
                                                new SqlMetaData("listValue", SqlDbType.Int),
                                                new SqlMetaData("textValue", SqlDbType.NVarChar, 4000)
                                            };

                foreach (FormSelectionMapping f in formSelectionMappings)
                {
                    var row = new SqlDataRecord(rowMetaData);
                    row.SetInt32(0, f.FormSelectionMappingId);
                    row.SetInt32(1, f.ViewId);
                    row.SetBoolean(2, f.IsAdd);
                    row.SetInt32(3, f.FormId);
                    row.SetInt32(4, f.ListValue);

                    if (f.ListValue < 0)
                    {
                        row.SetDBNull(4);
                    }
                    else
                    {
                        row.SetInt32(4, f.ListValue);
                    }

                    if (f.TextValue == null)
                    {
                        row.SetDBNull(5);
                    }
                    else
                    {
                        row.SetString(5, f.TextValue);
                    }

                    mappings.Add(row);
                }

                db.AddWithValue("@viewId", viewId);

                if (mappings.Count > 0)
                {
                    db.AddWithValue("@formSelectionMappings", "dbo.FormSelectionMapping", mappings);
                }

                db.AddWithValue("@employeeId", currentUser.EmployeeID);

                if (currentUser.isDelegate)
                {
                    db.AddWithValue("@delegateId", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    db.AddWithValue("@delegateId", DBNull.Value);
                }

                db.AddReturn("@returnValue", SqlDbType.Int);

                db.ExecuteProc("SaveFormSelectionMappings");

                return db.GetReturnValue<int>("@returnValue");
            }
        }

        /// <summary>
        /// Gets a <see cref="cGridNew" /> object for displaying custom entity records for a given <see cref="cCustomEntityView"/>, ready to be used on a page
        /// </summary>
        /// <param name="user">The current user object</param>
        /// <param name="accountId">The account identifier</param>
        /// <param name="employeeId">The employee identifier</param>
        /// <param name="view">The <see cref="cCustomeEntityView"/> view object</param>
        /// <param name="entity">The <see cref="cCustomEntity"/> custom entity object</param>
        /// <param name="forMobile">The flag to only include attributes (columns/data) which have "Show in Mobile" checked</param>
        /// <returns>A <see cref="cGridNew"/></returns>
        public cGridNew getEntityRecordsGrid(ICurrentUser user, cCustomEntityView view, cCustomEntity entity, bool forMobile)
        {
            cFields fields = new cFields(user.AccountID);
            cCustomEntities clsEntities = new cCustomEntities(user);
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            cAttribute keyatt = entity.getKeyField();
           

            cAttribute archiveFieldAttribute = entity.GetAttributeByDisplayName("Archived");
            cField archiveField = fields.GetFieldByID(archiveFieldAttribute.fieldid);
            // ID Field Check -- added to see if the view.fields list already contains the keyatt field
            bool bContainsKeyField = false;
            bool containsAttachmentFieldType = false;
            string attachmentColumn = string.Empty;
            bool containsArchiveField = false;
            cField keyfield = fields.GetFieldByID(keyatt.fieldid);

            cFieldColumn attributeFilterColumn = null;
                
            foreach (cCustomEntityViewField viewField in view.fields.Values)
            {
                // mobile app requests need to exclude any attributes not for mobile
                var attribute = entity.GetAttributeByFieldId(viewField.Field.FieldID);
                if (forMobile && (attribute == null || attribute.DisplayInMobile == false))
                {
                    continue;
                }

                if (containsAttachmentFieldType && viewField.Field.FieldType == "AT")
                {
                    continue;
                }

                // ID Field Check -- checking the field rather than fieldid as it's the field name in the SQL that we are trying not to duplicate on the table
                if (viewField.Field.FieldName == keyfield.FieldName && viewField.Field.TableID == keyfield.TableID && viewField.JoinVia == null)
                {
                    bContainsKeyField = true;
                }
                if (viewField.Field.FieldName == archiveField.FieldName && viewField.Field.TableID == archiveField.TableID && viewField.JoinVia == null)
                {
                    containsArchiveField = true;
                }

                if (viewField.JoinVia != null)
                {
                    columns.Add(new cFieldColumn(viewField.Field, viewField.JoinVia));
                }
                else if (viewField.Field.GenList && viewField.Field.ListItems.Count == 0)
                {
                    columns.Add(new cFieldColumn(viewField.Field, viewField.Field.GetLookupField()));
                }
                else
                {
                    columns.Add(new cFieldColumn(viewField.Field));
                }

                if (viewField.Field.ListItems.Count > 0)
                {
                    foreach (int s in viewField.Field.ListItems.Keys)
                    {
                        ((cFieldColumn)columns[columns.Count - 1]).addValueListItem(s, viewField.Field.ListItems[s].ToString());
                    }
                }

                if (viewField.Field.FieldType == "CL")
                {
                    cGlobalCurrencies clsGCurrencies = new cGlobalCurrencies();
                    cCurrencies clsCurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);

                    foreach (cCurrency c in clsCurrencies.currencyList.Values)
                    {
                        ((cFieldColumn)columns[columns.Count - 1]).addValueListItem(c.currencyid, clsGCurrencies.getGlobalCurrencyById(c.globalcurrencyid).label);
                    }
                }
            }

            

            bool bHideCurrencyField = false;
            if (entity.EnableCurrencies && entity.DefaultCurrencyID.HasValue)
            {
                cField greenLightCurrencyField = fields.GetFieldByID(entity.getGreenLightCurrencyAttribute().fieldid);
                if (!(from x in view.fields.Values where x.Field.FieldID == greenLightCurrencyField.FieldID select x).Any())
                {
                    columns.Insert(0, new cFieldColumn(greenLightCurrencyField));
                    bHideCurrencyField = true;
                }
            }
            if (containsArchiveField == false)
            {
                columns.Insert(0, new cFieldColumn(archiveField));
            }

            // ID Field Check -- if the id field is not already in the columns list insert it now
            if (bContainsKeyField == false)
            {
                columns.Insert(0, new cFieldColumn(keyfield));
            }

            if (entity.FormSelectionAttributeId.HasValue && entity.FormSelectionAttributeId.Value > 0)
            {
                var formSelectionAttribute = entity.attributes[entity.FormSelectionAttributeId.Value];
                attributeFilterColumn = new cFieldColumn(fields.GetFieldByID(formSelectionAttribute.fieldid))
                {
                    hidden = true,
                    Alias = "FilterAttribute"
                };

                columns.Add(attributeFilterColumn);
            }

            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "grid" + entity.entityid + view.viewid + keyatt.attributeid, entity.table, columns);
            clsgrid.KeyField = keyfield.FieldName;
            clsgrid.ArchiveField = archiveField.FieldName;

            #region Set the sorting of the grid
            cNewGridSort employeeSort = user.Employee.GetNewGridSortOrders().GetBy("grid" + entity.entityid + view.viewid + keyatt.attributeid);

            if (entity.FormSelectionAttributeId.HasValue && entity.FormSelectionAttributeId.Value > 0)
            {
                SerializableDictionary<string, object> gridInfo = new SerializableDictionary<string, object>();
                gridInfo.Add("defaultFormId", view.DefaultEditForm.formid);
                gridInfo.Add("selectionMappings", view.EditFormMappings);
                gridInfo.Add("columnName", attributeFilterColumn.field.FieldName);
                clsgrid.InitialiseRowGridInfo = gridInfo;
                clsgrid.InitialiseRow += new cGridNew.InitialiseRowEvent(clsgrid_InitialiseRowForAttributeFilter);
                clsgrid.ServiceClassForInitialiseRowEvent = "Spend_Management.cCustomEntities";
                clsgrid.ServiceClassMethodForInitialiseRowEvent = "clsgrid_InitialiseRowForAttributeFilter";
            }

            // if default sort set for view, use this. Need to let this get overridden by user default though
            if (employeeSort == null && view.SortColumn.FieldID != Guid.Empty)
            {
                cNewGridColumn SortCol = null;

                SortCol = clsgrid.getColumnByID(view.SortColumn.FieldID, (view.SortColumn.JoinVia != null ? view.SortColumn.JoinVia.JoinViaID : 0));

                if (SortCol != null)
                {
                    clsgrid.SortedColumn = SortCol;
                    clsgrid.SortDirection = view.SortColumn.SortDirection;
                }
            }

            #endregion

            clsgrid.CssClass = "datatbl";
            if (bContainsKeyField == false)  // ID Field Check -- if the key field should not be in the grid, hide it
            {
                clsgrid.getColumnByName(clsgrid.KeyField).hidden = true;
            }
          

            //clsgrid.getColumnByName("att" + keyfield.attributeid).hidden = true;
            if (view.allowdelete && user.CheckAccessRole(AccessRoleType.Delete, CustomEntityElementType.View, entity.entityid, view.viewid, false))
            {
                clsgrid.enabledeleting = true;
                clsgrid.deletelink = "javascript:deleteRecord(" + entity.entityid + ",{" + keyfield.FieldName + "}," + view.viewid + "," + keyatt.attributeid + ");";
            }

            if (view.allowedit && user.CheckAccessRole(AccessRoleType.Edit, CustomEntityElementType.View, entity.entityid, view.viewid, false) && (view.DefaultEditForm != null && view.DefaultEditForm.fields.Count > 0))
            {
                clsgrid.enableupdating = true;
                var editMappings = (from mapping in view.EditFormMappings let form = entity.getFormById(mapping.FormId) where form.fields.Count > 0 select mapping).ToList();
                if (editMappings.Any() && attributeFilterColumn != null)
                {
                    clsgrid.editlink =
                        $"aeentity.aspx?entityid={entity.entityid}&viewid={view.viewid}&formid={{FilterAttribute}}&id={{{keyfield.FieldName}}}";
                }
                else
                {
                    clsgrid.editlink =
                        $"aeentity.aspx?entityid={entity.entityid}&viewid={view.viewid}&formid={view.DefaultEditForm.formid}&id={{{keyfield.FieldName}}}";
                }
            }

            if (view.allowarchive)
            {
               
                clsgrid.enablearchiving = user.CheckAccessRole(AccessRoleType.Edit, CustomEntityElementType.View, view.entityid, view.viewid, false);
                clsgrid.archivelink = "javascript:archiveRecord(" + view.entityid + ",{" + keyfield.FieldName + "}, " + view.viewid + ", '{" + archiveField.FieldName + "}'," + keyatt.attributeid.ToString() + ")";
            }
            if (containsArchiveField == false)  
            {
                clsgrid.getColumnByName(clsgrid.ArchiveField).hidden = true;
            }

            if (view.allowapproval)
            {
                clsgrid.addEventColumn("approverecord", "Approve", string.Format("javascript:approveRecord({{{0}}}, {1});", keyfield.FieldName, entity.entityid), "Approve");
                clsgrid.addEventColumn("rejectrecord", "Reject", string.Format("javascript:RejectReason({{{0}}}, {1});", keyfield.FieldName, entity.entityid), "Reject");
            }

            if (entity.AudienceView != AudienceViewType.NoAudience)
            {
                SerializableDictionary<string, object> audienceRecStatus = clsEntities.GetAudienceRecords(entity.entityid, user.EmployeeID);
                SerializableDictionary<string, object> gridInfo = null;
                if (clsgrid.InitialiseRowGridInfo == null)
                {
                    gridInfo = new SerializableDictionary<string, object>();
                }
                else
                {
                    gridInfo = clsgrid.InitialiseRowGridInfo;
                }
                 
                gridInfo.Add("keyfield", keyfield.FieldName);
                gridInfo.Add("employeeid", user.EmployeeID);
                gridInfo.Add("accountid", user.AccountID);
                gridInfo.Add("gridid", clsgrid.GridID);
                gridInfo.Add("entityid", entity.entityid);
                clsgrid.InitialiseRowGridInfo = gridInfo;
                clsgrid.InitialiseRow += new cGridNew.InitialiseRowEvent(clsgrid_InitialiseRow);
                clsgrid.ServiceClassForInitialiseRowEvent = "Spend_Management.cCustomEntities";
                clsgrid.ServiceClassMethodForInitialiseRowEvent = "clsgrid_InitialiseRow";
                List<int> hiddenRecs = new List<int>();
                foreach (KeyValuePair<string, object> kvp in audienceRecStatus)
                {
                    if (((cAudienceRecordStatus)kvp.Value).Status == 0)
                    {
                        hiddenRecs.Add(((cAudienceRecordStatus)kvp.Value).RecordID);
                    }
                }
                clsgrid.HiddenRecords = hiddenRecs;
                //clsgrid.addAudienceFilter(entity.table.tableid, entity.AudienceTable.tableid, entity.table.primarykeyfield.field, user.EmployeeID);
            }
            if (view.filters != null)
            {
                foreach (KeyValuePair<byte, FieldFilter> kvp in view.filters)
                {
                    FieldFilter curFilter = kvp.Value;
                    FieldFilters.FieldFilterValues filterValues = FieldFilters.GetFilterValuesFromFieldFilter(curFilter, user);

                    clsgrid.addFilter(curFilter.Field, filterValues.conditionType, filterValues.valueOne,
                                      filterValues.valueTwo, ConditionJoiner.And, curFilter.JoinVia);
                }
            }

            if (entity.EnableCurrencies && entity.DefaultCurrencyID.HasValue)
            {
                clsgrid.CurrencyColumnName = "GreenLightCurrency";
                clsgrid.getColumnByName("GreenLightCurrency").hidden = bHideCurrencyField;
                clsgrid.CurrencyId = entity.DefaultCurrencyID.Value;
            }

            if (containsAttachmentFieldType)
            {
                //clsgrid.addEventColumn("viewattachment", "/shared/images/icons/16/plain/zoom_in.png", "javascript:viewFieldLevelAttachment('{" + attachmentColumn + "}');", "", "");
                clsgrid.addTwoStateEventColumn("viewattachment", (cFieldColumn)clsgrid.getColumnByName(attachmentColumn), null, null, "/shared/images/icons/16/plain/zoom_in.png", "javascript:viewFieldLevelAttachment('{" + attachmentColumn + "}');", "show image", "", "", "", "", "", false);
                clsgrid.getColumnByName(attachmentColumn).hidden = true;
            }

            clsgrid.EmptyText = "There are currently no " + entity.pluralname + " defined.";

            return clsgrid;
        }

        /// <summary>
        /// Checks whether the current record is accessible for the user logged in.
        /// </summary>
        /// <param name="view">Current view object</param>
        /// <param name="entity">Current entity object</param>
        /// <param name="recordId">Current record ID</param>
        /// <returns>The bool value whether an entity can be accessible</returns>
        public bool IsTheDataAccessibleToTheUser(cCustomEntityView view, cCustomEntity entity, int recordId)
        {
            if (!this.oCurrentUser.CheckAccessRole(AccessRoleType.Edit, CustomEntityElementType.View, entity.entityid, view.viewid, false))
                return false;

            var queryToCheckTheAccessLevel = new cQueryBuilder(nAccountid, cAccounts.getConnectionString(nAccountid), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, entity.table, tables, this.fields);
            queryToCheckTheAccessLevel.addColumn(fields.GetFieldByID(entity.getKeyField().fieldid));
            if (view.filters != null)
            {
                foreach (KeyValuePair<byte, FieldFilter> kvp in view.filters)
                {
                    var curFilter = kvp.Value;
                    var filterValues = FieldFilters.GetFilterValuesFromFieldFilter(curFilter, this.oCurrentUser);

                    queryToCheckTheAccessLevel.addFilter(new cQueryFilter(curFilter.Field, filterValues.conditionType, filterValues.valueOne.ToList(), filterValues.valueTwo.ToList(), ConditionJoiner.And, curFilter.JoinVia));
                }
            }

            queryToCheckTheAccessLevel.addFilter(new cQueryFilter(fields.GetFieldByID(entity.getKeyField().fieldid), ConditionType.Equals, new List<object> { recordId }, null, ConditionJoiner.And, null));
            if (queryToCheckTheAccessLevel.getDataset().Tables[0].Rows.Count > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Used by <see cref="cGridNew"/> when constructing each row of a grid
        /// </summary>
        /// <param name="row">The <see cref="cNewGridRow"/> row</param>
        /// <param name="gridInfo">A set of key/value pairs</param>
        void clsgrid_InitialiseRow(cNewGridRow row, Dictionary<string, object> gridInfo)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            // called for each row. Allows checking of audience accessibility
            cCustomEntities entities = new cCustomEntities(currentUser);

            SerializableDictionary<string, object> audienceRecStatus = entities.GetAudienceRecords((int)gridInfo["entityid"], (int)gridInfo["employeeid"]);

            if (gridInfo.ContainsKey("keyfield") && gridInfo.ContainsKey("gridid"))
            {
                cGridNew.InitialiseRowAudienceCheck(ref row, gridInfo["gridid"].ToString(), gridInfo["keyfield"].ToString(), audienceRecStatus);
            }

            return;
        }

        /// <summary>
        /// Change the edit url depending on the value of the filter attribute
        /// </summary>
        /// <param name="row">The current <see cref="cNewGridRow"/>instance</param>
        /// <param name="gridInfo">A <see cref="SerializableDictionary{TKey,TValue}"/>of key and value where the values are;
        /// 'columnName' - The ID of the AttributeFilter Column
        /// 'selectionMappings' - An instance of <see cref="List{T}"/>where T is <seealso cref="FormSelectionMapping"/>for the current view
        /// 'defaultFormId' - The default edit form ID, used if the current value of the filter attribute does not match any of the <seealso cref="FormSelectionMapping"/>values</param>
        private void clsgrid_InitialiseRowForAttributeFilter(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
        {
            var columnName = gridInfo["columnName"].ToString();
            var value = row.getCellByID(columnName).Value;
            var selectionMappings = this.GetMappings(gridInfo["selectionMappings"]);
            gridInfo["selectionMappings"] = selectionMappings;
            var formId = selectionMappings.Where(x => string.IsNullOrEmpty(x.TextValue) ? x.ListValue.ToString() == value.ToString() : x.TextValue.ToString() == value.ToString()).Select(x => x.FormId).FirstOrDefault();
            row.getCellByID("FilterAttribute").Value = formId > 0 ? formId : gridInfo["defaultFormId"];
        }

        /// <summary>
        /// Cast an object returned from "gridInfo" into a <see cref="List{T}"/> of <seealso cref="FormSelectionMapping"/>
        /// </summary>
        /// <param name="selectionMappings">The object returned from gridInfo to cast.</param>
        /// <returns>A <see cref="List{T}"/> of <seealso cref="FormSelectionMapping"/> or null</returns>
        private List<FormSelectionMapping> GetMappings(object selectionMappings)
        {
            if (selectionMappings.GetType() == typeof(List<FormSelectionMapping>))
            {
                return (List<FormSelectionMapping>) selectionMappings;
            }

            if (selectionMappings.GetType() != typeof(object[]))
            {
                return null;
            }

            var result = new List<FormSelectionMapping>();

            var list = (object[]) selectionMappings;
            foreach (Dictionary<string, object> fields in list)
            {
                var item = new FormSelectionMapping
                {
                    FormId = int.Parse(fields["FormId"].ToString()),
                    FormSelectionMappingId = int.Parse(fields["FormSelectionMappingId"].ToString()),
                    IsAdd = bool.Parse(fields["IsAdd"].ToString()),
                    ListValue = int.Parse(fields["ListValue"].ToString()),
                    TextValue = fields["TextValue"] == null ? string.Empty : fields["TextValue"].ToString(),
                    ViewId = int.Parse(fields["ViewId"].ToString())
                };

                result.Add(item);
            }

            return result;
        }



        /// <summary>
        /// Creates dropdown with greenlight attributes
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="accountId"></param>
        /// <param name="showNoneOption"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public ListItem[] CreateGreenLightAttributeDropDown(Guid tableId, int accountId,
            IDBConnection connection = null)
        {
            var lstAttributes = new List<ListItem>();
            using (var expData = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                var sql = "GetGreenLightAttributeByEntityId";
                if (tableId != Guid.Empty)
                {
                    expData.sqlexecute.Parameters.AddWithValue("@tableId", tableId);
                    using (var attributeDataSet = expData.GetProcDataSet(sql))
                    {
                        if (attributeDataSet != null && attributeDataSet.Tables.Count > 0)
                        {
                            foreach (DataRow dataRow in attributeDataSet.Tables[0].Rows)
                            {
                                var attributeId = new Guid(Convert.ToString(dataRow["fieldid"]));

                                lstAttributes.Add(new ListItem(dataRow["display_name"].ToString(), attributeId.ToString()));
                            }
                        }
                    }
                }
            }

            return lstAttributes.ToArray();
        }
        /// <summary>
        /// Get attribute value by id
        /// </summary>
        /// <param name="attributeid">attributeId</param>
        /// <param name="accountId">accountId</param>
        /// <param name="connection">data connection</param>
        /// <param name="filterValue">The entity record that should be matched.</param>
        /// <returns></returns>
        public string GetAttributeValueByAttributeId(object filterValue, int attributeid, int accountId, IDBConnection connection = null)
        {
            var attributeValue = string.Empty;

            using (var expData = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                var sql = "GetAttributeDataByAttributeId";
                if (attributeid > 0)
                {
                    expData.sqlexecute.Parameters.AddWithValue("@attributeId", attributeid);
                    if (filterValue != null && filterValue is int)
                    {
                        expData.sqlexecute.Parameters.AddWithValue("@recId", (int)filterValue);
                    }
                    using (var dtSet = expData.GetProcDataSet(sql))
                    {
                        if (dtSet != null && dtSet.Tables.Count > 0)
                        {
                            attributeValue = Convert.ToString(dtSet.Tables[0].Rows[0]["data"]);
                        }
                    }
                }
            }

            return attributeValue;
        }

        /// <summary>
        /// Get greenlight entity id by passing entity name, plural name and builtin as parameter.
        /// </summary>
        /// <param name="entityName">Name of greenlight entity</param>
        /// <param name="pluralName">Plural name  of greenlight entity</param>
        ///  <param name="builtIn">Is this a system greenlight</param>
        /// <param name="connection">Database connection</param>
        /// <returns>Identity of greenlight entity</returns>
        public int GetEntityIdByName(string entityName, string pluralName, bool builtIn, IDBConnection connection = null)
        {
            int returnvalue;
            using (var expData = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                expData.sqlexecute.Parameters.AddWithValue("@entityName ", entityName);
                expData.sqlexecute.Parameters.AddWithValue("@pluralName ", pluralName);
                expData.sqlexecute.Parameters.AddWithValue("@builtIn ", builtIn);
                expData.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                expData.ExecuteProc("GetEntityIdByName");
                returnvalue = Convert.ToInt32(expData.sqlexecute.Parameters["@returnvalue"].Value);
            }
            return returnvalue;
        }

        /// <summary>
        /// Get mandatory value of GreenLight attributes per form.
        /// </summary>
        /// <param name="field">Custom entity form attribute object</param>
        /// <returns>Bool value of mandatory</returns>
        public bool GetMandatoryValueOfAnAttribute(cCustomEntityFormField field)
        {
            if (field.IsMandatory.HasValue)
            {
                return field.IsMandatory.Value;
            }

             return field.attribute.mandatory;
        } 
    }

    [Serializable()]
    public struct sEntityBreadCrumb
    {
        private int nEntityID;
        private int nFormID;
        private int nRecordID;
        private int nTabID;
        private int viewID;

        /// <summary>
        /// sEntityBreadCrumb constructor
        /// </summary>
        /// <param name="entityid">id of custom entity</param>
        /// <param name="formid">id of form</param>
        /// <param name="recordid">id of individual record</param>
        /// <param name="tabid">id of tab</param>
        public sEntityBreadCrumb(int entityid, int formid, int recordid, int tabid, int viewid)
        {
            nEntityID = entityid;
            nFormID = formid;
            nRecordID = recordid;
            nTabID = tabid;
            viewID = viewid;
        }

        #region properties

        /// <summary>
        /// Get or Set the active entity id of the element
        /// </summary>
        public int EntityID
        {
            get { return nEntityID; }
        }

        /// <summary>
        /// Get or Set the active form id of the element
        /// </summary>
        public int FormID
        {
            get { return nFormID; }
        }

        /// <summary>
        /// Get or Set the active record id of the element
        /// </summary>
        public int RecordID
        {
            get { return nRecordID; }
            set { nRecordID = value; }
        }

        /// <summary>
        /// Get or Set the active tab of the element
        /// </summary>
        public int TabID
        {
            get { return nTabID; }
            set { nTabID = value; }
        }

        /// <summary>
        /// Get or set the active view of the element.
        /// </summary>
        public int ViewID
        {
            get { return viewID; }
            set { viewID = value; }
        }
        #endregion
    }

    #region cCustomEntityRelationship

    public class cCustomEntityRelationship
    {
        public struct ParentEntity
        {
            public int AttributeId;
            public int EntityId;
            public string MasterTableName;
            public Guid TableId;
            public Guid FieldId;
            public string FieldName;
        }

        public struct RelatedEntity
        {
            public string PluralName;
            public Guid TableId;
            public Guid FieldId;
            public string FieldName;
            public int EntityId;
        }

        private ParentEntity pEntity;
        private RelatedEntity rEntity;


        public cCustomEntityRelationship(int attributeid, int entityid, string parent_mastertablename, Guid parent_tableid, Guid parent_fieldid, string parent_fieldname, string target_pluralname, Guid target_tableid, Guid target_fieldid, string target_fieldname, int target_entityid)
        {
            pEntity = new ParentEntity();
            pEntity.AttributeId = attributeid;
            pEntity.EntityId = entityid;
            pEntity.FieldId = parent_fieldid;
            pEntity.FieldName = parent_fieldname;
            pEntity.MasterTableName = parent_mastertablename;
            pEntity.TableId = parent_tableid;

            rEntity = new RelatedEntity();
            rEntity.EntityId = target_entityid;
            rEntity.FieldId = target_fieldid;
            rEntity.FieldName = target_fieldname;
            rEntity.PluralName = target_pluralname;
            rEntity.TableId = target_tableid;
        }

        #region properties

        public ParentEntity SourceEntity
        {
            get { return pEntity; }
        }

        public RelatedEntity RelationEntity
        {
            get { return rEntity; }
        }

        #endregion
    }

    #endregion

    #region cCustomEntityRelationships

    public class cCustomEntityRelationships
    {
        private int nAccountId;
        private Dictionary<int, cCustomEntityRelationship> dicRelationships;
        private readonly IDBConnection expdata;
        public cCustomEntityRelationships(int accountid, IDBConnection connection = null)
        {
            this.expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid));
            dicRelationships = new Dictionary<int, cCustomEntityRelationship>();
            nAccountId = accountid;
            //                      0            1              2                3               4                5                 6            7              8               9                   10
            const string sql = "select attributeid, entityid, parent_mastertablename, parent_tableid, parent_fieldid, parent_fieldname, display_name, relatedtable, related_field, related_fieldname, related_entity from customEntityRelationships";

            using (IDataReader reader = expdata.GetReader(sql))
            {
                while (reader.Read())
                {
                    int attid = reader.GetInt32(0);
                    int entityid = reader.GetInt32(1);
                    string pmtname = reader.GetString(2);
                    Guid ptableid = reader.GetGuid(3);
                    Guid pfieldid = reader.GetGuid(4);
                    string pfieldname = reader.GetString(5);
                    string rpname = reader.GetString(6);
                    Guid rtableid = reader.GetGuid(7);
                    Guid rfieldid = reader.GetGuid(8);
                    string rfieldname = reader.GetString(9);
                    int rentityid = reader.GetInt32(10);

                    dicRelationships.Add(attid, new cCustomEntityRelationship(attid, entityid, pmtname, ptableid, pfieldid, pfieldname, rpname, rtableid, rfieldid, rfieldname, rentityid));
                }
                reader.Close();
            }
        }

        public cCustomEntityRelationship getRelationshipByAttributeId(int attributeId)
        {
            cCustomEntityRelationship ret = null;
            if (dicRelationships.ContainsKey(attributeId))
            {
                ret = dicRelationships[attributeId];
            }

            return ret;
        }

        public cCustomEntityRelationship getRelationship(int source_entityid, int target_entityid)
        {
            cCustomEntityRelationship ret = null;

            foreach (KeyValuePair<int, cCustomEntityRelationship> kvp in dicRelationships)
            {
                cCustomEntityRelationship cer = (cCustomEntityRelationship)kvp.Value;
                if (cer.SourceEntity.EntityId == source_entityid && cer.RelationEntity.EntityId == target_entityid)
                {
                    ret = cer;
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns an item list for a dropdown list with tables or entities that can have a relationship to them.
        /// </summary>
        /// <param name="entityid">Entity ID</param>
        /// <param name="relationshipType">Relationship Type (1 - n:1, 2 - 1:n)</param>
        /// <param name="isSummary">Summary relationship</param>
        /// <param name="excludeExistingOneToManys">Exclude any 1:n entities that already have a relationship on this Entity</param>
        /// <returns></returns>
        public static List<ListItem> getRelationshipDropDownByRelationshipType(int entityid, int relationshipType, bool isSummary, bool excludeExistingOneToManys)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            cCustomEntities clsEntities = new cCustomEntities(curUser);
            cCustomEntity curEntity = clsEntities.getEntityById(entityid);

            List<ListItem> items = new cTables(curUser.AccountID).CreateEntityRelationshipDropDown(includeNone: true,
                                                                              customTablesOnly:
                                                                                  (relationshipType != 1),
                                                                              excludeTableList:
                                                                                  new List<Guid>() { curEntity.table.TableID },
                                                                              filterModule: curUser.CurrentActiveModule);

            // for n:1, remove ce items where there are no non-excluded fields
            if (relationshipType == 1)
            {
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    Guid tableID;

                    if (Guid.TryParseExact(items[i].Value, "D", out tableID))
                    {
                        List<ListItem> subitems = clsEntities.GetTablesFieldListItemsForManyToOneRelationship(tableID, new List<Guid>());

                        if (subitems.Count == 0)
                        {
                            items.Remove(items[i]);
                        }
                    }
                }
            }

            // for 1:n, remove items where viewcount is zero or don't have a relationship to the current entity
            if (relationshipType == 2)
            {
                List<cTable> manyToOnesInUse = new List<cTable>();

                if (excludeExistingOneToManys)
                {
                    // Remove any entities that have already been used in a 1:n
                    //manyToOnesInUse = (from curAttribute in curEntity.attributes
                    //                   where curAttribute.Value.GetType() == typeof(cOneToManyRelationship)
                    //                   select ((cOneToManyRelationship)curAttribute.Value).relatedtable).ToList();
                }
                // remove those entries that do not have 1:n relationships TO them
                Dictionary<int, cOneToManyRelationship> otms = new Dictionary<int, cOneToManyRelationship>();

                for (int i = items.Count - 1; i >= 0; i--)
                {
                    if (String.IsNullOrWhiteSpace(items[i].Value) || items[i].Value == "0") continue;

                    otms.Clear();
                    cCustomEntity entity = clsEntities.getEntityByTableId(new Guid(items[i].Value));
                    if (entity != null)
                    {
                        clsEntities.getRelationshipAttributes(entityid, entity.entityid, ref otms);
                    }

                    if (
                            entity == null
                            ||
                            manyToOnesInUse.Contains(entity.table)
                            ||
                            entity.Views.Count == 0 // can't create 1:N if there's no view to show
                            ||
                            (isSummary && otms.Count == 0) // summary, no relationships to this entity
                            ||
                            (!isSummary && (entity.attributes.Values.FirstOrDefault(x => x.GetType() == typeof(cOneToManyRelationship) && ((cOneToManyRelationship)x).entityid == entityid) != null))
                            ||
                            clsEntities.CheckCircularReferencesForOneToManyEntity(entity, curEntity) // Check for circular references
                            )
                    {
                        items.Remove(items[i]);
                    }

                }
            }

            return items;
        }
    }

    #endregion
}