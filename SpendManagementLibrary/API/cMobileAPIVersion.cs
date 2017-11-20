using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    public class cMobileAPIVersion
    {
        private Guid gVersionId;
        private cMobileAPIType clAPIType;
        private string sVersionNumber;
        private bool bDisableUsage;
        private string sNotifyMessage;
        private string sTitle;
        private string sSyncMessage;
        private string sAppStoreURL;
        private DateTime? dtModifiedOn;

        #region properties

        public Guid VersionId
        {
            get { return gVersionId; }
            set { gVersionId = value; }
        }

        public cMobileAPIType APIType
        {
            get { return clAPIType; }
            set { clAPIType = value; }
        }

        public string VersionNumber
        {
            get { return sVersionNumber; }
            set { sVersionNumber = value; }
        }

        public bool DisableUsage
        {
            get { return bDisableUsage; }
            set { bDisableUsage = value; }
        }

        public string NotifyMessage
        {
            get { return sNotifyMessage; }
            set { sNotifyMessage = value; }
        }

        public string Title
        {
            get { return sTitle; }
            set { sTitle = value; }
        }

        public string SyncMessage
        {
            get { return sSyncMessage; }
            set { sSyncMessage = value; }
        }

        public string AppStoreURL
        {
            get { return sAppStoreURL; }
            set { sAppStoreURL = value; }
        }

        public DateTime? ModifiedOn
        {
            get { return dtModifiedOn; }
            set { dtModifiedOn = value; }
        }


        #endregion

        public cMobileAPIVersion(Guid versionid, cMobileAPIType apitype, string versionnumber, bool disableapp, string notifymsg, string title, string syncmsg, string appstoreurl, DateTime? modifiedon)
        {
            gVersionId = versionid;
            clAPIType = apitype;
            sVersionNumber = versionnumber;
            bDisableUsage = disableapp;
            sNotifyMessage = notifymsg;
            sSyncMessage = syncmsg;
            sTitle = title;
            sAppStoreURL = appstoreurl;
            dtModifiedOn = modifiedon;
        }
    }


    public class cMobileAPIVersions
    {
        private string sConnStr;
        private Dictionary<string, cMobileAPIVersion> versionList;

        public cMobileAPIVersions()
        {
            sConnStr = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;
            Caching vCaching = new Caching();

            if (vCaching.Cache.Contains("apiVersions"))
            {
                versionList = (Dictionary<string, cMobileAPIVersion>)vCaching.Cache.Get("apiVersions");
            }
            else
            {
                versionList = GetVersions();    
            }
            
        }

        private Dictionary<string, SpendManagementLibrary.cMobileAPIVersion> GetVersions()
        {
            Caching vCaching = new Caching();
            DBConnection db = new DBConnection(sConnStr);
            cMobileAPITypes apiTypes = new cMobileAPITypes();
            Dictionary<string, SpendManagementLibrary.cMobileAPIVersion> versions = new Dictionary<string, cMobileAPIVersion>();

            const string sql = "select versionId, mobileAPITypeId, versionNumber, disableAppUsage, notifyMessage, title, syncMessage, appStoreURL, modifiedOn from dbo.mobileAPIVersion";

            using(SqlDataReader reader = db.GetReader(sql))
            {
                #region Ordinals

                int versionOrd = reader.GetOrdinal("versionId");
                int typeIdOrd = reader.GetOrdinal("mobileAPITypeId");
                int verNumOrd = reader.GetOrdinal("versionNumber");
                int disableOrd = reader.GetOrdinal("disableAppUsage");
                int notifyOrd = reader.GetOrdinal("notifyMessage");
                int titleOrd = reader.GetOrdinal("title");
                int syncOrd = reader.GetOrdinal("syncMessage");
                int appstoreOrd = reader.GetOrdinal("appStoreURL");
                int modonOrd = reader.GetOrdinal("modifiedOn");

                #endregion

                while(reader.Read())
                {
                    Guid versionId = reader.GetGuid(versionOrd);
                    Guid typeId = reader.GetGuid(typeIdOrd);
                    string versionNumber = "";
                    if(!reader.IsDBNull(verNumOrd))
                        versionNumber = reader.GetString(verNumOrd);

                    bool disableApp = reader.GetBoolean(disableOrd);
                    string notifyMsg = "";
                    if(!reader.IsDBNull(notifyOrd))
                        notifyMsg = reader.GetString(notifyOrd);
                    string title = "";
                    if(!reader.IsDBNull(titleOrd))
                        title = reader.GetString(titleOrd);
                    string syncMsg = "";
                    if(!reader.IsDBNull(syncOrd))
                        syncMsg = reader.GetString(syncOrd);
                    string appStoreURL = "";
                    if(!reader.IsDBNull(appstoreOrd))
                        appStoreURL = reader.GetString(appstoreOrd);
                    DateTime? modifiedOn = null;
                    if(!reader.IsDBNull(modonOrd))
                        modifiedOn = reader.GetDateTime(modonOrd);

                    cMobileAPIType apiType = apiTypes.GetTypeById(typeId);
                    cMobileAPIVersion version = new cMobileAPIVersion(versionId, apiType, versionNumber, disableApp, notifyMsg, title, syncMsg, appStoreURL, modifiedOn);

                    if(!versions.ContainsKey(apiType.TypeKey))
                        versions.Add(apiType.TypeKey, version);

                }
                reader.Close();

                const string monitorSql = "select modifiedOn from dbo.mobileAPIVersion";
                const string monitorTypesSql = "select modifiedOn from dbo.mobileAPITypes";
                vCaching.Add("apiVersions", versions, new Dictionary<string, Dictionary<string, object>> { { monitorSql, null }, { monitorTypesSql, null } }, Caching.CacheTimeSpans.Medium, Caching.CacheDatabaseType.Metabase);
            }
            return versions;
        }

        public cMobileAPIVersion GetVersionByTypeKey(string typeKey)
        {
            return versionList.ContainsKey(typeKey) ? versionList[typeKey] : null;
        }
    }
}
