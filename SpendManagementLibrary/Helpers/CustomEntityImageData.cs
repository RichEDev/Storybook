using System;
using System.Collections.Generic;
using SpendManagementLibrary.GreenLight;

namespace SpendManagementLibrary.Helpers
{
    public class CustomEntityImageData
    {
        private int _accountId;

        public CustomEntityImageData(int accountId)
        {
            this._accountId = accountId;
        }
        public HTMLImageData GetHtmlImageData(string fileId)
        {
            return GetData(this._accountId, fileId);
        }

        /// <summary>
        /// Gets the attachment data relating to the attachment type attribute or images uploaded in the formatted text boxes
        /// </summary>
        /// <param name="accountid">The account id</param>
        /// <param name="entityId">The entity id, or -1 if just using the file guid</param>
        /// <param name="attributeId">The attribute id, or -1 if just using the file guid</param>
        /// <param name="guidFileId">The file guid</param>
        /// <returns>List<HTMLImageData></HTMLImageData></returns> 
        public static HTMLImageData GetData(int accountid, string guidFileId = "")
        {
            HTMLImageData imageData = null;
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                var Sql = "select fileID, fileType, fileName from CustomEntityImageData where fileID = @fileID";
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@fileID", guidFileId);

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

                        imageData = new HTMLImageData(fileId, fileType, fileName, expdata);
                    }

                }
            }
            return imageData;
        }
    }
}
