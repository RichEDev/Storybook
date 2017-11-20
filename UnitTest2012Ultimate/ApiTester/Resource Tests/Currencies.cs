namespace UnitTest2012Ultimate.ApiTester.Resource_Tests
{
    using System.Diagnostics;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Configuration;
    using APITester;

    using System;

    using Newtonsoft.Json;

    using SpendManagementHelpers;

    using SpendManagementLibrary;

    using Spend_Management;

    [TestClass]
    public class Currencies
    {
        private string token;
        private const string Reference = "Currencies";

        private const int databaseTable = 0;
        private const int apiTable = 1;
        private int accountId;
        private int employeeId;
        private string currencyDescription = "currencyDescription";

        [TestInitialize]
        public void TestInitialize()
        {
            GlobalAsax.Application_Start();
            token = AuthorisationToken.GetAuthToken();
            accountId = int.Parse(ConfigurationManager.AppSettings.Get("AccountID"));
            employeeId = int.Parse(ConfigurationManager.AppSettings.Get("EmployeeID"));
        }

        /// <summary>
        /// Tests the GetAll costcodes endpoint.
        /// </summary>
        [ TestCategory("Currencies"),TestCategory("ApiEndToEnd"), TestMethod]
        public void GetAll()
        {

            string requestUrl = ConfigurationManager.AppSettings["ApiAddress"] + Reference;
            JsonDeserializer json = RestClient.MakeRequest(requestUrl, HttpVerb.GET, authToken: token);

            if (json.jsonData.ContainsKey("error"))
            {
                Assert.Fail("Error with GetAll");
            }
            else
            {

                var globalCurrencies = new cGlobalCurrencies();
                
                var sortedDatabase = ApiTesterHelpers.GetDataBaseDataAndApiDataAndCompareThem(json, "select ISNULL(currencyid,0) as CurrencyId, global_currencies.globalcurrencyid as GlobalCurrencyId, ISNULL(archived, 0) as Archived from global_currencies left join currencies on global_currencies.globalcurrencyid = currencies.globalcurrencyid WHERE symbol IS NOT NULL");

                var testRow = sortedDatabase.Tables[databaseTable].Rows.Count / 2;

                var testId = sortedDatabase.Tables[databaseTable].Rows[testRow][0];

                var singleItem = this.GetById((int)testId);

                

                ApiTesterHelpers.CompareSingleItemToDataTableRow(
                    singleItem,
                    sortedDatabase.Tables[databaseTable].Rows[testRow]);

                sortedDatabase.Tables[databaseTable].DefaultView.RowFilter = "CurrencyId > 0";
                var validCurrencies = sortedDatabase.Tables[databaseTable].DefaultView.ToTable();

                testRow = validCurrencies.Rows.Count / 3;

                testId = validCurrencies.Rows[testRow]["GlobalCurrencyId"];

                var globalCurrency = globalCurrencies.globalcurrencies[(int)testId];

                singleItem = this.GetByNumericCode(globalCurrency.numericcode);

                ApiTesterHelpers.CompareSingleItemToDataTableRow(
                    singleItem,
                    validCurrencies.Rows[testRow]);
                testRow = validCurrencies.Rows.Count / 4;

                testId = validCurrencies.Rows[testRow]["GlobalCurrencyId"];

                globalCurrency = globalCurrencies.globalcurrencies[(int)testId];

                singleItem = this.GetByAlphaCode(globalCurrency.alphacode);

                ApiTesterHelpers.CompareSingleItemToDataTableRow(
                    singleItem,
                    validCurrencies.Rows[testRow]);

                testRow = validCurrencies.Rows.Count / 5;

                testId = validCurrencies.Rows[testRow]["GlobalCurrencyId"];

                globalCurrency = globalCurrencies.globalcurrencies[(int)testId];

                singleItem = this.GetByGlobalCurrencyId(globalCurrency.globalcurrencyid);

                ApiTesterHelpers.CompareSingleItemToDataTableRow(
                    singleItem,
                    validCurrencies.Rows[testRow]);


                // Find
                var filteredTable = validCurrencies.FilterTable(string.Format("GlobalCurrencyId = {0}", globalCurrency.globalcurrencyid));
                var items = this.Find(globalCurrencyId: globalCurrency.globalcurrencyid);
                Assert.IsTrue(items.ArrayItemCount("List") == filteredTable.Rows.Count);

                filteredTable = sortedDatabase.Tables[databaseTable].FilterTable("Archived = 0");
                items = this.Find(archived: false);
                Assert.IsTrue(items.ArrayItemCount("List") == filteredTable.Rows.Count);

                filteredTable = sortedDatabase.Tables[databaseTable].FilterTable("Archived = 1");
                items = this.Find(archived: true);
                Assert.IsTrue(items.ArrayItemCount("List") == filteredTable.Rows.Count);

                items = this.Find(symbol: "£" );
                Assert.IsTrue(items.GetObject("error") == null);
                items = this.Find(label: "label");
                Assert.IsTrue(items.GetObject("error") == null);

                // Now test for non-existant codes.
                singleItem = this.GetById(int.MaxValue);
                Assert.IsTrue(singleItem.GetObject("0,Item") == null);
                singleItem = this.GetByAlphaCode("XXXX");
                Assert.IsTrue(singleItem.GetObject("0,Item") == null);
                singleItem = this.GetByNumericCode("9999");
                Assert.IsTrue(singleItem.GetObject("0,Item") == null);
            }
        }

        private JsonDeserializer GetById(int id)
        {
            string requestUrl = ConfigurationManager.AppSettings["ApiAddress"] + Reference;
            JsonDeserializer json = RestClient.MakeRequest(
                requestUrl,
                HttpVerb.GET,
                authToken: token,
                parameters: String.Format(@"/{0}", id));

            return json;

        }

        private JsonDeserializer GetByNumericCode(string id)
        {
            string requestUrl = ConfigurationManager.AppSettings["ApiAddress"] + Reference;
            JsonDeserializer json = RestClient.MakeRequest(
                requestUrl,
                HttpVerb.GET,
                authToken: token,
                parameters: String.Format(@"/ByNumericCode/{0}", id));

            return json;

        }

        private JsonDeserializer GetByAlphaCode(string id)
        {
            string requestUrl = ConfigurationManager.AppSettings["ApiAddress"] + Reference;
            JsonDeserializer json = RestClient.MakeRequest(
                requestUrl,
                HttpVerb.GET,
                authToken: token,
                parameters: String.Format(@"/ByAlphaCode/{0}", id));

            return json;

        }

        private JsonDeserializer GetByGlobalCurrencyId(int id)
        {
            string requestUrl = ConfigurationManager.AppSettings["ApiAddress"] + Reference;
            JsonDeserializer json = RestClient.MakeRequest(
                requestUrl,
                HttpVerb.GET,
                authToken: token,
                parameters: String.Format(@"/ByGlobalCurrencyId/{0}", id));

            return json;

        }

        private JsonDeserializer Find(int id = 0, int globalCurrencyId = 0, bool? archived = null, string label = "", string symbol = "", byte op = 0)
        {
            string requestUrl = ConfigurationManager.AppSettings["ApiAddress"] + Reference;
            var currencyId = id > 0 ? string.Format("CurrencyId={0}", id) : string.Empty;
            var global = globalCurrencyId > 0 ? string.Format("GlobalCurrencyId={0}", globalCurrencyId) : string.Empty;
            var arch = archived.HasValue  ? string.Format("Archived={0}", archived.Value ? "True" : "False") : string.Empty;
            var lab = !string.IsNullOrEmpty(label)  ? string.Format("Label={0}", label ) : string.Empty;
            var symb = !string.IsNullOrEmpty(symbol)  ? string.Format("Symbol={0}", symbol) : string.Empty;
            

            JsonDeserializer json = RestClient.MakeRequest(
                requestUrl,
                HttpVerb.GET,
                authToken: token,
                parameters: String.Format(@"/Find?{0}{1}{2}{3}{4}&SearchOperator={5}", currencyId, global, arch, lab, symb, op));

            return json;

        }

        [TestCategory("Currencies")]
        [TestCategory("ApiEndToEnd")]
        [TestMethod]
        public void Bobbins()
        {
            var sqlHierarchy = new TreeNode<string>("root");
            sqlHierarchy.AddChild("APIUnitTestGetAccessRoles");
            sqlHierarchy.Children[0].AddChild("APIUnitTestElementsBase");

           // var serializer = new DatabaseObjectsSerializer(ConfigurationManager.ConnectionStrings["expenses"].ConnectionString);
          //  Debug.WriteLine(serializer.Serialize(sqlHierarchy));
        }
    }
}

