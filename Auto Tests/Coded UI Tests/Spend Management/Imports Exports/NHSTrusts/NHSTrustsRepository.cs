namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Imports_Exports.NHSTrusts
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;

    using Auto_Tests.Tools;

    class NHSTrustSQLStatements
    {
        public static string GET_ALL_NHSTrusts = "SELECT trustName, trustVPD, runSequenceNumber FROM esrTrusts ORDER BY {0} {1}";
        public static string DELETE_ALL_NHSTrusts;
    }

    /// <summary>
    /// The nhs trusts repository.
    /// </summary>
    public class NHSTrustsRepository
    {
        //private cDatabaseConnection db;
        //public NHSTrustsRepository(cDatabaseConnection db)
        //{
        //    this.db = db;
        //}

        /* public List<IPFiltersDTO> GetAll(SortIPFiltersByColumn sortby, EnumHelper.TableSortOrder order)
         {
             List<IPFiltersDTO> ipAddressFilters = new List<IPFiltersDTO>();
             SqlDataReader reader = db.GetReader(String.Format(IPFiltersSQLStatements.GET_ALL_IPFILTERS, EnumHelper.GetEnumDescription(sortby), EnumHelper.GetEnumDescription(order)));
             while (reader.Read())
             {
                 ipAddressFilters.Add(new IPFiltersDTO(reader.GetString(0), reader.GetString(1), reader.GetBoolean(2)));
             }
             reader.Close();
             return ipAddressFilters;
         }*/

        /// <summary>
        /// The create nhs trusts.
        /// </summary>
        /// <param name="nhsTrustToCreate">
        /// The nhs trust to create.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        /// <param name="adminId">
        /// The admin id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int CreateNhsTrusts(NHSTrusts nhsTrustToCreate, ProductType executingProduct, int adminId)
        {
            nhsTrustToCreate.TrustID = 0;
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@trustID", nhsTrustToCreate.TrustID);
            expdata.sqlexecute.Parameters.AddWithValue("@trustName", nhsTrustToCreate.TrustName);
            expdata.sqlexecute.Parameters.AddWithValue("@trustVPD", nhsTrustToCreate.TrustVPD);
            expdata.sqlexecute.Parameters.AddWithValue("@periodType", nhsTrustToCreate.PeriodType);
            expdata.sqlexecute.Parameters.AddWithValue("@periodRun", nhsTrustToCreate.PeriodRun);

            if (nhsTrustToCreate.FtpAddress == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@ftpAddress", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@ftpAddress", nhsTrustToCreate.FtpAddress);
            }

            if (nhsTrustToCreate.FtpUsername == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@ftpUsername", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@ftpUsername", nhsTrustToCreate.FtpUsername);
            }

            if (nhsTrustToCreate.FtpPassword == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@ftpPassword", DBNull.Value);
            }
            else
            {
                cSecureData clsSecure = new cSecureData();
                string encryptedPassword = clsSecure.Encrypt(nhsTrustToCreate.FtpPassword);
                expdata.sqlexecute.Parameters.AddWithValue("@ftpPassword", encryptedPassword);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@runSequenceNumber", nhsTrustToCreate.RunSequenceNumber);

            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", adminId);
            expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);

            expdata.AddWithValue("@delimiterCharacter", nhsTrustToCreate.DelimiterCharacter, 5);
            expdata.sqlexecute.Parameters.AddWithValue("@esrTrustVersionNumber", nhsTrustToCreate.ESRInterfaceVersion);
            expdata.sqlexecute.Parameters.AddWithValue("@currentOutboundSequence", DBNull.Value);
            expdata.sqlexecute.Parameters.Add("@versionNumberChanged", SqlDbType.TinyInt);
            expdata.sqlexecute.Parameters["@versionNumberChanged"].Direction = ParameterDirection.Output;

            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("SaveESRTrust");

            int trustId = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            nhsTrustToCreate.TrustID = trustId;
            ChangeStatus(nhsTrustToCreate.TrustID, executingProduct, nhsTrustToCreate.Archived);
            expdata.sqlexecute.Parameters.Clear();
            return nhsTrustToCreate.TrustID;
        }

        /// <summary>
        /// The delete nhsTrusts.
        /// </summary>
        /// <param name="nhsTrustId">
        /// The nhs trust id.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        /// <param name="adminId">
        /// The admin Id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal static int DeleteNhsTrusts(int nhsTrustId, ProductType executingProduct, int adminId)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));

            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", adminId);

            expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);

            expdata.sqlexecute.Parameters.AddWithValue("@trustID", nhsTrustId);

            expdata.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;

            expdata.ExecuteProc("deleteESRTrust");
            int returnValue = (int)expdata.sqlexecute.Parameters["@returnvalue"].Value;
            expdata.sqlexecute.Parameters.Clear();

            return returnValue;
        }

        /// <summary>
        /// The delete financial export.
        /// </summary>
        /// <param name="trustId">
        /// The trustid.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        /// <param name="associatedTrust">
        /// The associated trust.
        /// </param>
        internal static void DeleteFinancialExport(
            int trustId, ProductType executingProduct, bool associatedTrust = false)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@trustID", trustId);
            string strsql;
            if (associatedTrust)
            {
                strsql = "delete from financial_exports where NHSTrustID = @trustID";

                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// The change status.
        /// </summary>
        /// <param name="trustId">
        /// The trustid.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        /// <param name="archive">
        /// The archive.
        /// </param>
        internal static void ChangeStatus(int trustId, ProductType executingProduct, bool archive = false)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@trustID", trustId);
            string strsql;
            if (archive)
            {
                strsql = "update esrTrusts set archived = 1 where trustID = @trustID";

                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// The populate trust.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        internal static List<NHSTrusts> PopulateNhsTrust()
        {
            #region Read Data From Lithium

            cDatabaseConnection db =
                new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());
            List<NHSTrusts> _NHSTrusts = new List<NHSTrusts>();
            using (
                SqlDataReader reader =
                    db.GetReader(
                        "SELECT trustVPD, periodType, periodRun, runSequenceNumber, ftpAddress, ftpUsername, ftpPassword, archived, trustName, delimiterCharacter, ESRVersionNumber FROM esrTrusts")
                )
            {
                int trustVPDOrdinal = reader.GetOrdinal("trustVPD");
                int periodTypeOrdinal = reader.GetOrdinal("periodType");
                int periodRunOrdinal = reader.GetOrdinal("periodRun");
                int runSequenceNumberOrdinal = reader.GetOrdinal("runSequenceNumber");
                int ftpAddressOrdinal = reader.GetOrdinal("ftpAddress");
                int ftpUsernameOrdinal = reader.GetOrdinal("ftpUsername");
                int ftpPasswordOrdinal = reader.GetOrdinal("ftpPassword");
                int archivedOrdinal = reader.GetOrdinal("archived");
                int trustNameOrdinal = reader.GetOrdinal("trustName");
                int delimiterCharacterOrdinal = reader.GetOrdinal("delimiterCharacter");
                int ESRVersionNumberOrdinal = reader.GetOrdinal("ESRVersionNumber");

                while (reader.Read())
                {
                    #region Set variables

                    NHSTrusts trust = new NHSTrusts();
                    trust.TrustVPD = reader.IsDBNull(trustVPDOrdinal) ? null : reader.GetString(trustVPDOrdinal);
                    trust.PeriodType = reader.GetString(periodTypeOrdinal);
                    trust.PeriodRun = reader.GetString(periodRunOrdinal);
                    trust.RunSequenceNumber = reader.GetInt32(runSequenceNumberOrdinal);
                    trust.FtpAddress = reader.IsDBNull(trustVPDOrdinal) ? null : reader.GetString(ftpAddressOrdinal);
                    trust.FtpUsername = reader.IsDBNull(trustVPDOrdinal) ? null : reader.GetString(ftpUsernameOrdinal);
                    trust.FtpPassword = reader.IsDBNull(trustVPDOrdinal) ? null : reader.GetString(ftpPasswordOrdinal);
                    trust.Archived = reader.GetBoolean(archivedOrdinal);
                    trust.TrustName = reader.GetString(trustNameOrdinal);
                    trust.DelimiterCharacter = reader.GetString(delimiterCharacterOrdinal);
                    trust.ESRInterfaceVersion = reader.GetByte(ESRVersionNumberOrdinal);
                    _NHSTrusts.Add(trust);

                    #endregion
                }
                reader.Close();
            }
            return _NHSTrusts;

            #endregion
        }
    }
}
