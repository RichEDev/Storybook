using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SpendManagementLibrary.GreenLight;
using SpendManagementLibrary.Interfaces;

namespace SpendManagementLibrary.Helpers
{
    public class CustomEntityImageData
    {
        private int _accountId;

        public CustomEntityImageData(int accountId)
        {
            this._accountId = accountId;
        }
        public HtmlImageData GetHtmlImageData(string fileId)
        {
            return GetData(this._accountId, fileId);
        }

        /// <summary>
        /// Gets the attachment data relating to the attachment type attribute or images uploaded in the formatted text boxes
        /// </summary>
        /// <param name="accountid">The account id</param>
        /// <param name="guidFileId">The file guid</param>
        /// <returns>List<HTMLImageData></HTMLImageData></returns> 
        public static HtmlImageData GetData(int accountid, string guidFileId = "")
        {
            HtmlImageData imageData = null;
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                var Sql = "select customentities.entity_name, cea.display_name,fileID, fileType, fileName from CustomEntityImageData inner join customEntities on customEntities.entityid = CustomEntityImageData.entityID inner join customEntityAttributes as cea on cea.attributeid = CustomEntityImageData.attributeID where fileID = @fileID";
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@fileID", guidFileId);

                imageData = GetImages(expdata, Sql).FirstOrDefault();
            }

            return imageData;
        }

        /// <summary>
        /// Get a list of <see cref="HtmlImageData"/> for the gfiven entity id and attribute id
        /// </summary>
        /// <param name="accountid">The current Account ID</param>
        /// <param name="entityId">The Entity ID</param>
        /// <param name="attributeId">The attribute ID</param>
        /// <returns>A <see cref="List{T}"/>of <seealso cref="HtmlImageData"/> for the given entity and attribute IDs</returns>
        public static List<HtmlImageData> GetData(int accountid, int entityId, int attributeId)
        {
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                var Sql =
                    "select customentities.entity_name, cea.display_name,fileID, fileType, fileName from CustomEntityImageData inner join customEntities on customEntities.entityid = CustomEntityImageData.entityID inner join customEntityAttributes as cea on cea.attributeid = CustomEntityImageData.attributeID where CustomEntityImageData.entityID = @entityID AND CustomEntityImageData.attributeID = @attributeID";
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@entityID", entityId);
                expdata.sqlexecute.Parameters.AddWithValue("@attributeID", attributeId);

                return GetImages(expdata, Sql);
            }
        }


        private static List<HtmlImageData> GetImages(IDBConnection expdata, string sql)
        {
            var result = new List<HtmlImageData>();
            using (var reader = expdata.GetReader(sql))
            {
                var entityNameOrdinal = reader.GetOrdinal("entity_name");
                var displayNameOrdinal = reader.GetOrdinal("display_name");
                var fileIdOrdinal = reader.GetOrdinal("fileID");
                var fileNameOrdinal = reader.GetOrdinal("fileName");
                var fileTypeOrdinal = reader.GetOrdinal("fileType");

                while (reader.Read())
                {
                    var fileGuid = reader.GetGuid(fileIdOrdinal);
                    var fileId = fileGuid.ToString();
                    var fileType = reader.GetString(fileTypeOrdinal);
                    string fileName;
                    if (reader.IsDBNull(fileNameOrdinal))
                    {
                        fileName = string.Empty;
                    }
                    else
                    {
                        fileName = reader.GetString(fileNameOrdinal);
                    }

                    var entityName = reader.GetString(entityNameOrdinal);
                    var displayName = reader.GetString(displayNameOrdinal);

                    result.Add(new HtmlImageData(fileId, fileType, fileName, expdata, entityName, displayName));
                }

            }

            return result;
        }
    }
}
