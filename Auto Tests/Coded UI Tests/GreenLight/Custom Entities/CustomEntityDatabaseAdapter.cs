using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Auto_Tests.Tools;

namespace Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities
{
    internal class CustomEntityDatabaseAdapter
    {
        internal static int CreateCustomEntity(CustomEntity customEntityToCreate, ProductType executingProduct)
        {
            customEntityToCreate.entityId = 0;
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            //cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.expenses));

            db.sqlexecute.Parameters.AddWithValue("@entityid", customEntityToCreate.entityId);
            if (string.IsNullOrEmpty(customEntityToCreate.modifiedBy))
            {
                db.sqlexecute.Parameters.AddWithValue("@userid", customEntityToCreate.userId);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@userid", customEntityToCreate.modifiedBy);
            }
            db.sqlexecute.Parameters.AddWithValue("@entityname", customEntityToCreate.entityName);
            db.sqlexecute.Parameters.AddWithValue("@pluralname", customEntityToCreate.pluralName);
            if (string.IsNullOrEmpty(customEntityToCreate.description))
            {
                db.sqlexecute.Parameters.AddWithValue("@description", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@description", customEntityToCreate.description);
            }
            db.sqlexecute.Parameters.AddWithValue("@enableattachments", Convert.ToByte(customEntityToCreate.enableAttachments));
            db.sqlexecute.Parameters.AddWithValue("@audienceViewType", Convert.ToByte(customEntityToCreate.AudienceViewType));
            db.sqlexecute.Parameters.AddWithValue("@allowdocmergeaccess", Convert.ToByte(customEntityToCreate.allowDocumentMerge));
            db.sqlexecute.Parameters.AddWithValue("@enableCurrencies", Convert.ToByte(customEntityToCreate.enableCurrencies));
            db.sqlexecute.Parameters.AddWithValue("@enablePopupWindow", Convert.ToByte(customEntityToCreate.EnablePopupWindow));

            if (customEntityToCreate.DefaultPopupView == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@defaultPopupView", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@defaultPopupView", customEntityToCreate.DefaultPopupView);
            }
            
            if (string.IsNullOrEmpty(customEntityToCreate.defaultCurrencyId))
            {
                db.sqlexecute.Parameters.AddWithValue("@defaultCurrencyID", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@defaultCurrencyID", getCurrencyIDByCurrencyName(customEntityToCreate.defaultCurrencyId, executingProduct));
            }
            db.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
            db.sqlexecute.Parameters.AddWithValue("@formSelectionAttribute", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@ownerId", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@supportContactId", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@supportQuestion", DBNull.Value);
            db.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            db.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("saveCustomEntity");
            customEntityToCreate.entityId = (int)db.sqlexecute.Parameters["@identity"].Value;
            customEntityToCreate.OldTableId = customEntityToCreate.tableId;
            customEntityToCreate.tableId = GetTableIDbyTableName(customEntityToCreate.pluralName, executingProduct);
            db.sqlexecute.Parameters.Clear();
            var lastDatabaseUpdate = db.ExecuteScalar<DateTime>("select getdate()"); //ideally should be transactioned but if anything will be later
            CacheUtilities.DeleteCachedTablesAndFields(lastDatabaseUpdate);
            return customEntityToCreate.entityId;
        }

        internal static Guid GetTableIDbyTableName(string displayname, ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));

            string strSQL2 = "select tableid from tables where parentTableID is null and description = @displayName";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@displayName", displayname);
            Guid displaytable = dbex_CodedUI.getGuidValue(strSQL2);

            return displaytable;
        }

        internal static Guid GetTableIDbyPluralName(string pluralName, ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));

            string strSQL2 = "select tableid from customEntities where plural_name = @displayName";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@displayName", pluralName);
            Guid displaytable = dbex_CodedUI.getGuidValue(strSQL2);

            return displaytable;
        }

        /// <summary>
        /// Gets the currency ID from the database based on the currency name
        ///</summary>
        internal static int getCurrencyIDByCurrencyName(string currencyName, ProductType executingProduct)
        {
            int currencyID;
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            string strSQL = "SELECT currencyid FROM currencyview WHERE currencyName = @currency";
            db.sqlexecute.Parameters.AddWithValue("@currency", currencyName);
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
            {
                reader.Read();
                currencyID = reader.GetInt32(0);
                reader.Close();
            }
            db.sqlexecute.Parameters.Clear();
            return currencyID;
        }

        /* Returns boolean */
        internal static int DeleteCustomEntity(int customEntityId, ProductType executingProduct, int? employeeId = null, int? delegateId = null)
        {
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            db.sqlexecute.Parameters.AddWithValue("@entityid", customEntityId);
            if (employeeId == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeId);
            }
            if (delegateId == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateId);
            }
            db.sqlexecute.Parameters.Add("@returnVal", SqlDbType.Int);
            db.sqlexecute.Parameters["@returnVal"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("deleteCustomEntity");
            int result = (int)db.sqlexecute.Parameters["@returnVal"].Value;
            db.sqlexecute.Parameters.Clear();

            return result;
        }

        internal static int GetCustomEntityIdByName(string entityName, ProductType executingProduct)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                throw new NoNullAllowedException("Entity name cannot be null or empty!");
            }

            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            db.sqlexecute.Parameters.AddWithValue("@entityname", entityName);
            string sqlQuery = "select entityid from customentities where entity_name = @entityname";
            int id = 0;
            using (SqlDataReader reader = db.GetReader(sqlQuery))
            {
                try
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        id = reader.GetInt32(0);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return id;
        }
    }
}
