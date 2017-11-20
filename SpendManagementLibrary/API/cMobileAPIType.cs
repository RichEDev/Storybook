using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    public class cMobileAPIType
    {
        private Guid gTypeId;
        private string sTypeKey;
        private string sTypeDescription;
        private DateTime? dtModifiedOn;

        #region properties

        public Guid TypeId
        {
            get { return gTypeId; }
            set { gTypeId = value; }
        }

        public string TypeKey
        {
            get { return sTypeKey; }
            set { sTypeKey = value; }
        }

        public string TypeDescription
        {
            get { return sTypeDescription; }
            set { sTypeDescription = value; }
        }

        public DateTime? ModifiedOn
        {
            get { return dtModifiedOn; }
            set { dtModifiedOn = value; }
        }

        #endregion

        public cMobileAPIType(Guid typeid, string typekey, string description, DateTime? modifiedon)
        {
            gTypeId = typeid;
            sTypeKey = typekey;
            sTypeDescription = description;
            dtModifiedOn = modifiedon;
        }
    }

    public class cMobileAPITypes
    {
        private string sConnStr;
        private Dictionary<string, cMobileAPIType> typesList;

        public cMobileAPITypes()
        {
            sConnStr = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;

            typesList = GetAPITypes();
        }

        private Dictionary<string, cMobileAPIType> GetAPITypes()
        {
            DBConnection db = new DBConnection(sConnStr);
            Dictionary<string, cMobileAPIType> lst = new Dictionary<string, cMobileAPIType>();
            const string sql = "select API_TypeId, typeKey, typeDescription, modifiedOn from dbo.mobileAPITypes";

            using(SqlDataReader reader = db.GetReader(sql))
            {
                #region Ordinals

                int idOrd = reader.GetOrdinal("API_TypeId");
                int keyOrd = reader.GetOrdinal("typeKey");
                int descOrd = reader.GetOrdinal("typeDescription");
                int modOrd = reader.GetOrdinal("modifiedOn");

                #endregion

                while(reader.Read())
                {
                    Guid id = reader.GetGuid(idOrd);
                    string key = reader.GetString(keyOrd);
                    string desc = "";
                    if(!reader.IsDBNull(descOrd))
                        desc = reader.GetString(descOrd);
                    DateTime? modon = null;
                    if(!reader.IsDBNull(modOrd))
                        modon = reader.GetDateTime(modOrd);

                    cMobileAPIType apiType = new cMobileAPIType(id, key, desc, modon);

                    if(!lst.ContainsKey(key))
                        lst.Add(key, apiType);
                }
                reader.Close();
            }

            return lst;
        }

        public cMobileAPIType GetTypeByKey(string key)
        {
            return typesList.ContainsKey(key) ? typesList[key] : null;
        }

        public cMobileAPIType GetTypeById(Guid apiTypeId)
        {
            cMobileAPIType t = (from x in typesList.Values
                                where x.TypeId == apiTypeId
                                select x).FirstOrDefault();
            return t;
        }
    }
}
