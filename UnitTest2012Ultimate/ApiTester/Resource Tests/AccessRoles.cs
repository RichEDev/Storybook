namespace UnitTest2012Ultimate.ApiTester.Resource_Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Configuration;
    using APITester;

    using System;

    using Spend_Management;

    [TestClass]
    public class AccessRoles
    {
        private string token;
        private const string Reference = "AccessRoles";

        private const int databaseTable = 0;
        private const int apiTable = 1;
        private int accountId;
        private int employeeId;
        private string AccessRoleDescription = "AccessRoleDescription";

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
        [TestCategory("AccessRoles"),TestCategory("ApiEndToEnd"), TestMethod]
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

                var sortedDatabase = ApiTesterHelpers.GetDataBaseDataAndApiDataAndCompareThem(json, "select roleID as Id, rolename as Label,description as Description from accessroles");

                var testRow = sortedDatabase.Tables[databaseTable].Rows.Count / 2;

                var testId = sortedDatabase.Tables[databaseTable].Rows[testRow][0];

                var singleItem = this.GetById((int)testId);

                ApiTesterHelpers.CompareSingleItemToDataTableRow(
                    singleItem,
                    sortedDatabase.Tables[databaseTable].Rows[testRow]);

                sortedDatabase.Tables[databaseTable].DefaultView.RowFilter = "AccessRoleId > 0";
                var validAccessRoles = sortedDatabase.Tables[databaseTable].DefaultView.ToTable();

                testRow = validAccessRoles.Rows.Count / 3;

                testId = validAccessRoles.Rows[testRow]["GlobalAccessRoleId"];



                // Find
                var items = this.Find(label: sortedDatabase.Tables[databaseTable].Rows[testRow][1].ToString());
                Assert.IsTrue(items.ArrayItemCount("List") == 1);


                // Now test for non-existant codes.
                singleItem = this.GetById(int.MaxValue);
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

        private JsonDeserializer GetByGlobalAccessRoleId(int id)
        {
            string requestUrl = ConfigurationManager.AppSettings["ApiAddress"] + Reference;
            JsonDeserializer json = RestClient.MakeRequest(
                requestUrl,
                HttpVerb.GET,
                authToken: token,
                parameters: String.Format(@"/ByGlobalAccessRoleId/{0}", id));

            return json;

        }

        private JsonDeserializer Find(int id = 0, int globalAccessRoleId = 0, bool? archived = null, string label = "", string symbol = "", byte op = 0)
        {
            string requestUrl = ConfigurationManager.AppSettings["ApiAddress"] + Reference;
            var AccessRoleId = id > 0 ? string.Format("AccessRoleId={0}", id) : string.Empty;
            var global = globalAccessRoleId > 0 ? string.Format("GlobalAccessRoleId={0}", globalAccessRoleId) : string.Empty;
            var arch = archived.HasValue ? string.Format("Archived={0}", archived.Value ? "True" : "False") : string.Empty;
            var lab = !string.IsNullOrEmpty(label) ? string.Format("Label={0}", label) : string.Empty;
            var symb = !string.IsNullOrEmpty(symbol) ? string.Format("Symbol={0}", symbol) : string.Empty;


            JsonDeserializer json = RestClient.MakeRequest(
                requestUrl,
                HttpVerb.GET,
                authToken: token,
                parameters: String.Format(@"/Find?{0}{1}{2}{3}{4}&SearchOperator={5}", AccessRoleId, global, arch, lab, symb, op));

            return json;

        }
    }
}

