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
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
    public class cInformationMessages
    {
        System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
        System.Collections.Generic.List<cInformationMessage> list;

        public cInformationMessages()
        {
            InitialiseData();
        }

        public void InitialiseData()
        {
            list = (System.Collections.Generic.List<cInformationMessage>)Cache["informationMessages"];
            if (list == null)
            {
                list = CacheList();
            }
        }


        public System.Collections.Generic.List<cInformationMessage> CacheList()
        {
            DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            string strSQL = "SELECT informationID, title, message, administratorID, dateAdded, displayOrder, deleted FROM dbo.information_messages WHERE deleted = 0";
            expdata.sqlexecute.CommandText = strSQL;
            

            System.Collections.Generic.List<cInformationMessage> listMessages = new System.Collections.Generic.List<cInformationMessage>();

            System.Data.SqlClient.SqlDataReader reader;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
                Cache.Insert("informationMessages", listMessages, dep, Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Short),
                    CacheItemPriority.Default, null);
            }
            using (reader = expdata.GetReader(strSQL))
            {
                cInformationMessage tmpMessage;
                while (reader.Read())
                {
                    int informationID = reader.GetInt32(reader.GetOrdinal("informationID"));
                    int administratorID = reader.GetInt32(reader.GetOrdinal("administratorID"));
                    int displayOrder = reader.GetInt32(reader.GetOrdinal("displayOrder"));
                    bool deleted = reader.GetBoolean(reader.GetOrdinal("deleted"));
                    string title = reader.GetString(reader.GetOrdinal("title"));
                    string message = reader.GetString(reader.GetOrdinal("message"));
                    DateTime dateAdded = reader.GetDateTime(reader.GetOrdinal("dateAdded"));

                    tmpMessage = new cInformationMessage(informationID, title, message, administratorID, dateAdded, displayOrder, deleted);
                    listMessages.Add(tmpMessage);
                }
                reader.Close();
            }

            return listMessages;
        }

        public System.Collections.Generic.List<cInformationMessage> GetMessages()
        {
            System.Collections.Generic.List<cInformationMessage> listMessages = new System.Collections.Generic.List<cInformationMessage>();

            cInformationMessage tmpMessage;
            for (int i = 0; i < list.Count; i++)
            {
                tmpMessage = (cInformationMessage)list[i];
                if (tmpMessage.Deleted == false)
                {
                    listMessages.Add(tmpMessage);
                }
            }
            return listMessages;
        }

        public void DeleteMessage(int messageID)
        {
            DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            expdata.sqlexecute.Parameters.AddWithValue("@informationID", messageID);
            string strSQL = "UPDATE information_messages SET deleted=1 WHERE informationID = @informationID";
            expdata.ExecuteSQL(strSQL);
        }

        public void UpdateMessage(int messageID, string title, string message, int displayOrder)
        {

        }

        public void AddMessage(string title, string message, int administratorID)
        {

        }
    }

    public class cInformationMessage
    {
        private int nInformationID;
        private string sTitle;
        private string sMessage;
        private int nAdministratorID;
        private DateTime dtDateAdded;
        private int nDisplayOrder;
        private bool nDeleted;

        public cInformationMessage(int informationID, string title, string message, int adminitratorID, DateTime dateAdded, int displayOrder, bool deleted)
        {
            InformationID = informationID;
            Title = title;
            Message = message;
            AdministratorID = adminitratorID;
            DateAdded = dateAdded;
            DisplayOrder = displayOrder;
            Deleted = deleted;
        }

        public int InformationID
        {
            get { return nInformationID; }
            set { nInformationID = value; }
        }

        public string Title
        {
            get { return sTitle; }
            set { sTitle = value; }
        }

        public string Message
        {
            get { return sMessage; }
            set { sMessage = value; }
        }

        public int AdministratorID
        {
            get { return nAdministratorID; }
            set { nAdministratorID = value; }
        }

        public DateTime DateAdded
        {
            get { return dtDateAdded; }
            set { dtDateAdded = value; }
        }

        public int DisplayOrder
        {
            get { return nDisplayOrder; }
            set { nDisplayOrder = value; }
        }

        public bool Deleted
        {
            get { return nDeleted; }
            set { nDeleted = value; }
        }
    }
}
