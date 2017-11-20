namespace UnitTest2012Ultimate.Claims
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Data.SqlClient;

    using SpendManagementLibrary;
    using cVehicleEngineType = SpendManagementLibrary.VehicleEngineType;

    [TestClass]
    public class VehicleEngineTypes
    {
        private const int TestVehicleEngineTypeId = -1;

        [ClassInitialize]
        public static void Initialise(TestContext tc)
        {
            GlobalAsax.Application_Start();
            VehicleEngineTypes.CreateVehicleEngineType(VehicleEngineTypes.TestVehicleEngineTypeId);
        }

        private static void CreateVehicleEngineType(int VehicleEngineTypeId)
        {
            using (var cn = new SqlConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountId)))
            {
                cn.Open();
                using (var insertVet = new SqlCommand("SET IDENTITY_INSERT dbo.VehicleEngineTypes ON;" +
                                                      "IF NOT EXISTS (SELECT VehicleEngineTypeID FROM dbo.VehicleEngineTypes WHERE VehicleEngineTypeID = @VehicleEngineTypeId)" +
                                                      " INSERT dbo.VehicleEngineTypes (VehicleEngineTypeID, CreatedBy, CreatedOn, Name)" +
                                                      " VALUES (@VehicleEngineTypeId, @CuEmployeeId, GETUTCDATE(), 'Test Vehicle engine type Name' + cast(@VehicleEngineTypeId as varchar));" +
                                                      "SET IDENTITY_INSERT dbo.VehicleEngineTypes OFF;", cn))
                {
                    insertVet.Parameters.AddWithValue("@CuEmployeeId", GlobalTestVariables.EmployeeId);
                    insertVet.Parameters.AddWithValue("@VehicleEngineTypeId", VehicleEngineTypeId);
                    insertVet.ExecuteNonQuery();
                }
            }
        }

        [TestMethod, TestCategory("Spend Management"), TestCategory("Vehicle engine types")]
        public void TestGetAll()
        {
            var orgs = cVehicleEngineType.GetAll(Moqs.CurrentUser());
            Assert.IsTrue(orgs.Count > 0);
        }

        [TestMethod, TestCategory("Spend Management"), TestCategory("Vehicle engine types")]
        public void TestGet_Success()
        {
            var org = cVehicleEngineType.Get(Moqs.CurrentUser(), VehicleEngineTypes.TestVehicleEngineTypeId);
            Assert.IsNotNull(org);
        }

        [TestMethod, TestCategory("Spend Management"), TestCategory("Vehicle engine types")]
        public void TestGet_Failure()
        {
            var org = cVehicleEngineType.Get(Moqs.CurrentUser(), -2);
            Assert.IsNull(org);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            VehicleEngineTypes.DeleteVehicleEngineType(VehicleEngineTypes.TestVehicleEngineTypeId);
        }

        private static void DeleteVehicleEngineType(int VehicleEngineTypeId)
        {
            using (var cn = new SqlConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountId)))
            {
                cn.Open();
                using (var deleteVet = new SqlCommand("DELETE FROM dbo.VehicleEngineTypes" +
                                                      " WHERE VehicleEngineTypeID = @VehicleEngineTypeID;", cn))
                {
                    deleteVet.Parameters.AddWithValue("@VehicleEngineTypeId", VehicleEngineTypeId);
                    deleteVet.ExecuteNonQuery();
                }
            }
        }
    }
}
