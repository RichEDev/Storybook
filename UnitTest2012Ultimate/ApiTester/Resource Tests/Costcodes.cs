namespace UnitTest2012Ultimate.ApiTester.Resource_Tests
{
    using System.Configuration;
    using APITester;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    [TestClass]
    public class Costcodes
    {
        private string token;
        private const string Reference = "CostCodes";

        [TestInitialize]
        public void TestInitialize()
        {
            GlobalAsax.Application_Start();
            token = AuthorisationToken.GetAuthToken();
        }

        /// <summary>
        /// Tests the GetAll costcodes endpoint.
        /// </summary>
        [TestCategory("Spend Management API"), TestCategory("CostCodes"), TestMethod]
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
                IDBConnection databaseConnection = new DatabaseConnection(ConfigurationManager.ConnectionStrings["expenses"].ConnectionString);
                var databasecostCodes = databaseConnection.GetDataSet("SELECT costcodeId from costcodes");
                int costCodeCount = json.ArrayItemCount("List");

                Assert.AreEqual(costCodeCount, databasecostCodes.Tables[0].Rows.Count);
            }
        }
    }
}



