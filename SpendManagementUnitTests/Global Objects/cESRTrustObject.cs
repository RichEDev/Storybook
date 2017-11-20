using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;
using System.Configuration;
using SpendManagementLibrary.ESRTransferServiceClasses;
using System.Data.SqlClient;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cESRTrustObject
    {
        /// <summary>
        /// Global static variable for a trust object to be used in the unit tests for the ESR Trusts
        /// </summary>
        /// <returns></returns>
        public static cESRTrust CreateESRTrustGlobalVariable()
        {
            cESRTrusts clsTrusts = new cESRTrusts(cGlobalVariables.AccountID);
            int tempTrustID = clsTrusts.SaveTrust(new cESRTrust(0, "TestTrust" + DateTime.Now.Ticks.ToString(), "123", "M", "N", 1, "ftp", "UnitTestAccount", "UnitTestPassword", false, new DateTime?(DateTime.Now), new DateTime?(DateTime.Now), ","));
            cGlobalVariables.NHSTrustID = tempTrustID;
            clsTrusts = new cESRTrusts(cGlobalVariables.AccountID);
            cESRTrust trust = clsTrusts.GetESRTrustByID(tempTrustID);
            return trust;
        }

        /// <summary>
        /// Global static variable for a trust object to be used in the unit tests for the ESR Trusts where its values that 
        /// can be set to null or nothing are
        /// </summary>
        /// <returns></returns>
        public static cESRTrust CreateESRTrustGlobalVariableWithValuesThatCanBeSetToNullOrNothing()
        {
            cESRTrusts clsTrusts = new cESRTrusts(cGlobalVariables.AccountID);
            int tempTrustID = clsTrusts.SaveTrust(new cESRTrust(0, "TestTrust" + DateTime.Now.Ticks.ToString(), "123", "M", "N", 1, "", "", "", false, null, null, ","));
            cGlobalVariables.NHSTrustID = tempTrustID;
            clsTrusts = new cESRTrusts(cGlobalVariables.AccountID);
            cESRTrust trust = clsTrusts.GetESRTrustByID(tempTrustID);
            return trust;
        }

        /// <summary>
        /// Delete the static trust object from the database
        /// </summary>
        public static void DeleteTrust()
        {
            cESRTrusts clsTrusts = new cESRTrusts(cGlobalVariables.AccountID);
            clsTrusts.DeleteTrust(cGlobalVariables.NHSTrustID);
        }

        /// <summary>
        /// Global static variable for a trust object used in the inbound and outbound services to be used in the unit tests
        /// </summary>
        /// <returns></returns>
        public static void CreateServiceESRTrustGlobalVariable()
        {
            cNHSTrusts clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            cSecureData clsSecure = new cSecureData();
            clsTrusts.SaveTrust(new cESRTrust(0, 999, cGlobalVariables.AccountID, "UnitTestTrust" + DateTime.Now.Ticks.ToString(), "123", "ftp", "UnitTestAccount", clsSecure.Encrypt("UnitTestPassword"), false, new DateTime?(DateTime.Now), new DateTime?(DateTime.Now)), cGlobalVariables.AccountID);
            
            clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);

            #region Set the Global Inbound Data ID Variable

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "SELECT trustID FROM NHSTrustDetails WHERE TrustName = @TrustName";
            smData.sqlexecute.Parameters.AddWithValue("@TrustName", "UnitTestTrust");

            int TrustID = 0;

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    TrustID = reader.GetInt32(0);
                }

                reader.Close();
            }

            smData.sqlexecute.Parameters.Clear();

            cGlobalVariables.NHSTrustID = TrustID;

            #endregion

        }

        /// <summary>
        /// Delete the service trust from the ESRFileTransfer database
        /// </summary>
        public static void DeleteServiceTrust()
        {
            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "DELETE FROM NHSTrustDetails WHERE trustID = @TrustID";
            smData.sqlexecute.Parameters.AddWithValue("@TrustID", cGlobalVariables.NHSTrustID);
            smData.ExecuteSQL(strSQL);

            if (cNHSTrusts.lstTrusts != null)
            {
                cNHSTrusts.lstTrusts.Remove(cGlobalVariables.NHSTrustID);
            }
        }

    }
}
