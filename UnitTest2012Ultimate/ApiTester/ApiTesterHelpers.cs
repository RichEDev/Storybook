namespace UnitTest2012Ultimate.ApiTester
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Web;

    using APITester;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    //A collection of Static methods to assist with the API tester unit tests
    static class ApiTesterHelpers
    {
        /// <summary>
        /// Turns a API request object into a URL parameter string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>A URL parameter string i.e. ?Id=432&Name=test&active=1</returns>
        public static string ObjectToParameterString(object obj)
        {
            var properties =
            obj.GetType()
                .GetProperties()
                .Where(p => p.GetValue(obj, null) != null)
                .Select(p => p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString()));
            string queryString = String.Join("&", properties.ToArray());

            return "?" + queryString;
        }

        /// <summary>
        /// Create a dataset with the database table and the api data table then compare them, Assert they are the same.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal static DataSet GetDataBaseDataAndApiDataAndCompareThem(JsonDeserializer json, string sql)
        {
            DataSet databaseList;
            var result = new DataSet();
            using (
                IDBConnection databaseConnection =
                    new DatabaseConnection(ConfigurationManager.ConnectionStrings["expenses"].ConnectionString))
            {
                databaseList =
                    databaseConnection.GetDataSet(sql);
            }

            int currencyCount = json.ArrayItemCount("List");

            var columns = databaseList.Tables[0].Columns;

            var apiList = databaseList.Tables[0].Clone();
            apiList.TableName = "ApiList";

            for (int i = 0; i < currencyCount; i++)
            {
                var newRow = apiList.NewRow();
                foreach (DataColumn dataColumn in columns)
                {
                    var apiItem = json.GetObject(string.Format("List.{0}.{1}", i, dataColumn.ColumnName));
                    if (apiItem != null)
                    {
                        newRow[dataColumn.ColumnName] = apiItem;
                    }
                    else
                    {
                        newRow[dataColumn.ColumnName] = DBNull.Value;
                    }
                }

                apiList.Rows.Add(newRow);
            }

            // At the point we have to datatables so we should be able to compare them if they are in the same order.
            databaseList.Tables[0].DefaultView.Sort = string.Format("{0} asc", columns[0].ColumnName);
            apiList.DefaultView.Sort = string.Format("{0} asc", columns[0].ColumnName);

            var sortedDatabase = databaseList.Tables[0].DefaultView.ToTable();
            var sortedApi = apiList.DefaultView.ToTable();

            Assert.IsTrue(AreTablesTheSame(sortedDatabase, sortedApi), "Lists not the same");

            Assert.AreEqual(currencyCount, databaseList.Tables[0].Rows.Count);

            result.Tables.Add(sortedDatabase);
            result.Tables.Add(sortedApi);
            return result;
        }

        internal static bool AreTablesTheSame(DataTable tbl1, DataTable tbl2)
        {
            if (tbl1.Rows.Count != tbl2.Rows.Count || tbl1.Columns.Count != tbl2.Columns.Count)
            {
                Assert.Fail("List and database row count not the same.");
                return false;
            }


            for (int i = 0; i < tbl1.Rows.Count; i++)
            {
                for (int c = 0; c < tbl1.Columns.Count; c++)
                {
                    Assert.IsTrue(Equals(tbl1.Rows[i][c], tbl2.Rows[i][c]), string.Format("Row {0} , Column {1} the data does not match {2} != {3}", i, c, tbl1.Rows[i][c], tbl2.Rows[i][c]));
                }
            }
            return true;
        }

        public static void CompareSingleItemToDataTableRow(JsonDeserializer singleItem, DataRow dataRow)
        {
            var objectList = (IDictionary<string, object>)singleItem.GetObject("Item");
            foreach (KeyValuePair<string, object> keyValuePair in objectList)
            {
                Assert.IsTrue(Equals(dataRow[keyValuePair.Key], keyValuePair.Value));
            }
            
        }

        public static DataTable FilterTable(this DataTable validCurrencies, string rowFilter)
        {
            validCurrencies.DefaultView.RowFilter = rowFilter;
            return validCurrencies.DefaultView.ToTable();
        }
    }
}
