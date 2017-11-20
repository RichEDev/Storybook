using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Web.Caching;
using SpendManagementLibrary;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;

namespace Spend_Management
{
    public class Databases
    {
        internal static Dictionary<int, cDatabase> lstDatabases;

        public Databases(IDBConnection connection = null)
        {
            InitialiseData(connection);
        }

        public void InitialiseData(IDBConnection connection = null)
        {
            if (lstDatabases == null)
            {
                lstDatabases = CacheList(connection);
            }
        }

        public Dictionary<int, cDatabase> CacheList(IDBConnection connection = null) 
        {
            IDBConnection expdata = connection ?? new DatabaseConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            const string sql = "SELECT databaseID, receiptpath, logopath FROM dbo.databases";
            using (IDataReader reader = expdata.GetReader(sql))
            {
                lstDatabases = new Dictionary<int, cDatabase>();

                while (reader.Read())
                {
                    int databaseID = (int) reader["databaseID"];
                    string receiptPath = reader.GetValueOrDefault("receiptpath", "");
                    string logoPath = reader.GetValueOrDefault("logopath", "");
                    cDatabase database = new cDatabase(databaseID, receiptPath, logoPath);

                    lstDatabases.Add(databaseID, database);
                }
                reader.Close();
            }

            return lstDatabases;
        }

        public cDatabase GetDatabaseByID(int databaseID)
        {
            cDatabase reqDatabase = null;
            reqDatabase = lstDatabases[databaseID];
            return reqDatabase;
        }
    }

    public class cDatabase
    {
        private int nDatabaseID;
        private string sReceiptPath;
        private string sLogoPath;

        public cDatabase(int databaseID, string receiptPath, string logoPath)
        {
            nDatabaseID = databaseID;
            sReceiptPath = receiptPath;
            sLogoPath = logoPath;
        }

        #region Properties
        public int DatabaseID
        {
            get { return nDatabaseID; }
        }

        public string ReceiptPath
        {
            get { return sReceiptPath; }
        }

        public string LogoPath
        {
            get { return sLogoPath; }
        }
        #endregion
    }
}


