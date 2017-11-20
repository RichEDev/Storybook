namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Web.Security;

    using Microsoft.SqlServer.Server;

    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

using Utilities.DistributedCaching;

    //// <summary>
    //// Join Via class
    //// </summary>
    public class JoinVias
{
    /// <summary>
        /// The _ current user.
    /// </summary>
        private ICurrentUserBase _CurrentUser;

        /// <summary>
        /// The _ cache.
        /// </summary>
        private static readonly Cache _Cache;

        /// <summary>
        /// Initialises static members of the <see cref="JoinVias"/> class.
        /// </summary>
        static JoinVias()
        {
            _Cache = new Cache();
        }

        /// <summary>
        /// Constructor for the JoinVias class
        /// </summary>
        /// <param name="currentUser">CurrentUser object</param>
        public JoinVias(ICurrentUserBase currentUser)
        {
            this._CurrentUser = currentUser;
        }
    
        private string CacheKey { 
            get
        {
            return "joinViaPaths_";
                } 
        }

        /// <summary>
        /// Forces removal and re-initialisation of cache
        /// </summary>
        /// <param name="joinVia">
        /// The join Via.
        /// </param>
        private void ExpireCache(int joinViaId)
            {
            _Cache.Delete(this._CurrentUser.AccountID, this.CacheKey, joinViaId.ToString());
        }

        /// <summary>
        /// Returns a list of the JoinVias indexed by JoinViaID
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, JoinVia> GetJoinVias(IDBConnection connection = null)
        {
            Dictionary<int, JoinVia> joinViaList;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._CurrentUser.AccountID)))
            {
                joinViaList = new Dictionary<int, JoinVia>();

                expdata.sqlexecute.Parameters.Clear(); 
                IDataReader reader;

                using (reader = expdata.GetReader("GetJoinVia", CommandType.StoredProcedure))
                {
                    #region Ordinals
                    int joinViaID_ord = reader.GetOrdinal("joinViaID");
                    int joinViaDescription_ord = reader.GetOrdinal("joinViaDescription");
                    int joinViaAS_ord = reader.GetOrdinal("joinViaAS");
                    int relatedID_ord = reader.GetOrdinal("relatedID");
                    int relatedType_ord = reader.GetOrdinal("relatedType");
                    int order_ord = reader.GetOrdinal("order");
                    #endregion

                    while (reader.Read())
                    {
                        var joinViaID = reader.GetInt32(joinViaID_ord);
                        var joinViaDescription = reader.GetString(joinViaDescription_ord);
                        var joinViaAs = reader.GetGuid(joinViaAS_ord);
                        var relatedID = reader.GetGuid(relatedID_ord);
                        var relatedType = (JoinViaPart.IDType)reader.GetByte(relatedType_ord);
                        var joinViaOrder = reader.GetInt32(order_ord);

                        JoinVia joinVia = null;
                        joinViaList.TryGetValue(joinViaID, out joinVia);

                        if (joinVia == null)
                        {
                            // create the joinvia with a joinviapart (may not be the first "order" if database is loading out of order)
                            joinViaList.Add(joinViaID, new JoinVia(joinViaID, joinViaDescription, joinViaAs, new SortedList<int, JoinViaPart>() { { joinViaOrder, new JoinViaPart(relatedID, relatedType) } }));
                        }
                        else
                        {
                            // add the joinviapart to the existing joinvia
                            joinViaList[joinViaID].JoinViaList.Add(joinViaOrder, new JoinViaPart(relatedID, relatedType));
                        }
                    }
                }
            }

            foreach (KeyValuePair<int, JoinVia> keyValuePair in joinViaList)
            {
                _Cache.Add(this._CurrentUser.AccountID, this.CacheKey, keyValuePair.Key.ToString(), keyValuePair.Value);
            }

            return joinViaList;
        }


        /// <summary>
        /// Save a JoinVia to the database after checking for duplicates
        /// </summary>
        /// <param name="joinVia"></param>
        /// <returns></returns>
        public int SaveJoinVia(JoinVia joinVia, IDBConnection connection = null)
        {
            int savedJoinViaID;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this._CurrentUser.AccountID)))
            {
                var lstViaIDs = new List<SqlDataRecord>();
                var pathForHashing = new StringBuilder();

                // Generate a sql Int_UniqueIdentifier_TinyInt table param and pass into the stored proc
                SqlMetaData[] viaPart = { new SqlMetaData("c1", System.Data.SqlDbType.Int), new SqlMetaData("c2", System.Data.SqlDbType.UniqueIdentifier), new SqlMetaData("c3", System.Data.SqlDbType.TinyInt) };

                foreach (KeyValuePair<int, JoinViaPart> kvp in joinVia.JoinViaList)
                {
                    // Int_UniqueIdentifier_TinyInt
                    var row = new SqlDataRecord(viaPart);
                    row.SetInt32(0, kvp.Key);
                    row.SetGuid(1, kvp.Value.ViaID);
                    row.SetByte(2, (byte)kvp.Value.ViaIDType);

                    lstViaIDs.Add(row);

                    // build up the comparison string for hashing
                    pathForHashing.Append(kvp.Value.ViaID);
                }

                // this allows us to compare all parts of a via path at once
                string pathHash = FormsAuthentication.HashPasswordForStoringInConfigFile(pathForHashing.ToString(), "MD5");

                expdata.sqlexecute.Parameters.Clear();

                expdata.sqlexecute.Parameters.AddWithValue("@joinViaID", joinVia.JoinViaID);
                expdata.sqlexecute.Parameters.AddWithValue("@joinViaDescription", joinVia.Description);
                expdata.sqlexecute.Parameters.AddWithValue("@joinViaPathHash", pathHash);
                expdata.sqlexecute.Parameters.Add("@joinViaParts_Int_Unique_TinyInt", System.Data.SqlDbType.Structured);
                expdata.sqlexecute.Parameters["@joinViaParts_Int_Unique_TinyInt"].Direction = System.Data.ParameterDirection.Input;
                expdata.sqlexecute.Parameters["@joinViaParts_Int_Unique_TinyInt"].Value = lstViaIDs;

                expdata.sqlexecute.Parameters.Add("@savedJoinViaID", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@savedJoinViaID"].Direction = ParameterDirection.ReturnValue;

                expdata.ExecuteProc("SaveJoinVia");

                savedJoinViaID = 0;
                savedJoinViaID = (int)expdata.sqlexecute.Parameters["@savedJoinViaID"].Value;
                expdata.sqlexecute.Parameters.Clear();
            }

            ExpireCache(savedJoinViaID);

            // if savedJoinViaID = -1 there were MORE than one joinvia with that ID (thats not good)
            // so it couldn't decide which one to choose and won't have saved the joinvia

            return savedJoinViaID;
        }

        /// <summary>
        /// Deserialization method from javascript object
        /// </summary>
        /// <param name="joinVia">Javascript JoinVia object</param>
        /// <returns>JoinVia class object</returns>
        public static JoinVia ConvertJSToC(JSJoinVia joinVia)
        {
            SortedList<int, JoinViaPart> viaList = null;
            int i = 0;

            if (joinVia.Parts.Count > 0)
            {
                viaList = new SortedList<int, JoinViaPart>();
                foreach (JSJoinVia.Part p in joinVia.Parts)
                {
                    viaList.Add(i, new JoinViaPart(p.ViaID, (JoinViaPart.IDType)p.ViaType, (JoinViaPart.JoinType)p.JoinType));
                    i++;
                }
            }

            return new JoinVia(joinVia.JoinViaID, joinVia.JoinViaDescription, joinVia.JoinViaAS, viaList);
        }

        /// <summary>
        /// Serializes JoinVia into javascript JoinVia object
        /// </summary>
        /// <param name="joinVia">JoinVia class object</param>
        /// <returns>Javascript JoinVia object</returns>
        public static JSJoinVia ConvertCToJS(JoinVia joinVia)
        {
            JSJoinVia jsJoinVia = new JSJoinVia();
            jsJoinVia.JoinViaID = joinVia.JoinViaID;
            jsJoinVia.JoinViaDescription = joinVia.Description;
            jsJoinVia.JoinViaAS = joinVia.JoinViaAS;

            List<JSJoinVia.Part> viaList = new List<JSJoinVia.Part>();
            JSJoinVia.Part jsJoinViaPart;
            int i = 0;

            if (joinVia.JoinViaList.Count > 0)
            {
                foreach (JoinViaPart p in joinVia.JoinViaList.Values)
                {
                    jsJoinViaPart = new JSJoinVia.Part();
                    jsJoinViaPart.ViaID = p.ViaID;
                    jsJoinViaPart.ViaType = p.ViaIDType;
                    jsJoinViaPart.JoinType = p.TypeOfJoin;

                    viaList.Add(jsJoinViaPart);
                    i++;
                }
            }

            jsJoinVia.Parts = viaList;

            return jsJoinVia;
        }

        /// <summary>
        /// Retrieves a particular JoinVia by its ID
        /// </summary>
        /// <param name="joinViaID">ID of JoinVia to retrieve</param>
        /// <returns>JoinVia class or NULL if not found</returns>
        public JoinVia GetJoinViaByID(int joinViaID)
        {
            JoinVia retJv = null;
            retJv = (JoinVia)_Cache.Get(this._CurrentUser.AccountID, this.CacheKey, joinViaID.ToString());
            if (retJv == null && joinViaID > 0)
            {
                var result = this.GetJoinVias();
                result.TryGetValue(joinViaID, out retJv);
        }

            return retJv;
        }

        /// <summary>
        /// This turns a formatted composite series of guids into a list of join via parts for use in a join via
        /// </summary>
        /// <param name="joinViaIDs"></param>
        /// <returns></returns>
        public static SortedList<int, JoinViaPart> JoinViaPartsFromCompositeGuid(string joinViaIDs)
        {
            int joinViaOrder = 0;
            var joinViaList = new SortedList<int, JoinViaPart>();

            foreach (string tmpStr in joinViaIDs.Split(new string[] {"_"}, StringSplitOptions.RemoveEmptyEntries))
            {
                if (tmpStr == "copy")
                {
                    continue;
                }

                string _prefix = tmpStr.Substring(0, 1);
                string _guid = tmpStr.Substring(1);

                Guid joinViaPartID;
                if (Guid.TryParseExact(_guid, "D", out joinViaPartID))
                {
                    JoinViaPart.IDType joinViaType;
                    switch (_prefix)
                    {
                        case "g":
                            joinViaType = JoinViaPart.IDType.Table;
                            break;
                        case "k":
                            joinViaType = JoinViaPart.IDType.Field;
                            break;
                        case "x":
                            joinViaType = JoinViaPart.IDType.RelatedTable;
                            break;
                        case "n":
                            continue;
                        default:
                            return null;
                                //throw new ArgumentOutOfRangeException("oJsonFields", "The joinVia list for field: " + _field.FieldName + " - contained an invalid prefix to a guid");
                    }
                    joinViaList.Add(joinViaOrder, new JoinViaPart(joinViaPartID, joinViaType));
                    joinViaOrder++;
                }
            }

            return joinViaList;
        }

        public SortedList<int, JoinViaPart> JoinViaPartsFromCompositeGuid(string joinViaIDs, cField reportField)
        {
            int joinViaOrder = 0;
            var joinViaList = new SortedList<int, JoinViaPart>();

            foreach (string tmpStr in joinViaIDs.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (tmpStr == "copy")
                {
                    continue;
                }

                string _prefix = tmpStr.Substring(0, 1);
                string _guid = tmpStr.Substring(1);

                Guid joinViaPartID;
                if (Guid.TryParseExact(_guid, "D", out joinViaPartID))
                {
                    JoinViaPart.IDType joinViaType;
                    switch (_prefix)
                    {
                        case "g":
                            joinViaType = JoinViaPart.IDType.Table;
                            break;
                        case "k":
                            joinViaType = JoinViaPart.IDType.Field;
                            break;
                        case "x":
                            joinViaType = JoinViaPart.IDType.RelatedTable;
                            break;
                        case "n":
                            this.LastField = joinViaPartID;
                            continue;
                        default:
                            return null;
                                //throw new ArgumentOutOfRangeException("oJsonFields", "The joinVia list for field: " + _field.FieldName + " - contained an invalid prefix to a guid");
                    }
                    joinViaList.Add(joinViaOrder, new JoinViaPart(joinViaPartID, joinViaType));
                    joinViaOrder++;
                }
            }

            return joinViaList;
        }

        public Guid LastField { get; set; }
}
}
