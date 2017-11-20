using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auto_Tests.Coded_UI_Tests.Expenses.System_Options.Attachment_Types
{
    class AttachmentTypesMethods
    {
        /// <summary>
        /// Deletes PNG from the testing database
        ///</summary>
        public void DeleteAttachmentTypeFromDB(string attachmentType, ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));

            string strSQL = "DELETE FROM mimetypes WHERE globalmimeid = @globalMimeID";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@globalMimeID", ReturnMimeHeaderIDFromMetabase(attachmentType, executingProduct));
            dbex_CodedUI.ExecuteSQL(strSQL);
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Enter PNG to the testing database 
        ///</summary>
        public void AddAttachmentTypeToDB(string attachmentType, ProductType executingProduct)
        {
            DateTime currentDateTime = DateTime.Now;

            int subAccountId = AutoTools.GetSubAccountId(executingProduct);
            int employeeID = AutoTools.GetEmployeeIDByUsername(executingProduct);
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));

            string strSQL = "INSERT INTO mimetypes VALUES (@subAccountID, 0, @currentDateTime, @employeeID, NULL, NULL, @mimeHeaderID)";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@subAccountID", subAccountId);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@currentDateTime", currentDateTime);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@mimeHeaderID", ReturnMimeHeaderIDFromMetabase(attachmentType, executingProduct));
            dbex_CodedUI.ExecuteSQL(strSQL);
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Reads the ID for a specific attachment type from the metabase and returns it
        ///</summary>
        public Guid ReturnMimeHeaderIDFromMetabase(string mimeHeader, ProductType executingProduct)
        {
            Guid mimeHeaderID = Guid.Empty;
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.MetabaseConnectionString(executingProduct));
            string strSQL = "SELECT mimeid FROM mime_headers WHERE fileExtension = @fileExtension";
            db.sqlexecute.Parameters.AddWithValue("@fileExtension", mimeHeader);

            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    mimeHeaderID = reader.GetGuid(0);
                }
                reader.Close();
            }
            db.sqlexecute.Parameters.Clear();
            return mimeHeaderID;
        }
    }
}
