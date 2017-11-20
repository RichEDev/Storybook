namespace UnitTest2012Ultimate.Claims
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Data.SqlClient;

    using SpendManagementLibrary;
    using cOrganisations = SpendManagementLibrary.Addresses.Organisations;

    [TestClass]
    public class Organisations
    {
        private const int TestOrganisationId = -1;

        [ClassInitialize]
        public static void Initialise(TestContext tc)
        {
            GlobalAsax.Application_Start();
            Organisations.CreateOrganisation(Organisations.TestOrganisationId);
        }

        private static void CreateOrganisation(int organisationId)
        {
            using (var cn = new SqlConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountId)))
            {
                cn.Open();
                using (var insertOrg = new SqlCommand("SET IDENTITY_INSERT dbo.Organisations ON;" +
                                                      "IF NOT EXISTS (SELECT OrganisationID FROM dbo.Organisations WHERE OrganisationID = @OrganisationId)" +
                                                      " INSERT dbo.Organisations (OrganisationID, OrganisationName, IsArchived)" +
                                                      " VALUES (@OrganisationId, 'Test Organisation Name' + cast(@OrganisationId as varchar), 0);" +
                                                      "SET IDENTITY_INSERT dbo.Organisations OFF;", cn))
                {
                    insertOrg.Parameters.AddWithValue("@OrganisationId", organisationId);
                    insertOrg.ExecuteNonQuery();
                }
            }
        }

        [TestMethod, TestCategory("Spend Management"), TestCategory("Organisations")]
        public void TestGetAll()
        {
            var orgs = cOrganisations.GetAll(GlobalTestVariables.AccountId);
            Assert.IsTrue(orgs.Count > 0);
        }

        [TestMethod, TestCategory("Spend Management"), TestCategory("Organisations")]
        public void TestGet_Success()
        {
            var org = cOrganisations.Get(GlobalTestVariables.AccountId, Organisations.TestOrganisationId);
            Assert.IsNotNull(org);
        }

        [TestMethod, TestCategory("Spend Management"), TestCategory("Organisations")]
        public void TestGet_Failure()
        {
            var org = cOrganisations.Get(GlobalTestVariables.AccountId, -2);
            Assert.IsNull(org);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            Organisations.DeleteOrganisation(Organisations.TestOrganisationId);
        }

        private static void DeleteOrganisation(int organisationId)
        {
            using (var cn = new SqlConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountId)))
            {
                cn.Open();
                using (var deleteOrg = new SqlCommand("DELETE FROM dbo.Organisations" +
                                                      " WHERE OrganisationID = @OrganisationID;", cn))
                {
                    deleteOrg.Parameters.AddWithValue("@OrganisationId", organisationId);
                    deleteOrg.ExecuteNonQuery();
                }
            }
        }
    }
}
