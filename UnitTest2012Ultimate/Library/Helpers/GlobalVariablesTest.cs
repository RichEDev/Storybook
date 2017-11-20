namespace UnitTest2012Ultimate.Library.Helpers
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary;

    using Spend_Management;

    /// <summary>
    /// This is a test class for GlobalVariablesTest and is intended
    /// to contain all GlobalVariablesTest Unit Tests
    /// </summary>
    [TestClass]
    public class GlobalVariablesTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        [TestInitialize()]
        public void MyTestInitialize()
        {
            GlobalVariables.MetabaseConnectionString =
                ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;
        }
        [TestCleanup]
        public void TestCleanup()
        {
            
        }
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext context)
        {
            GlobalAsax.Application_Start();
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        #endregion

        /// <summary>
        /// A test for GetModule where an empty module name is passed, the default value (Modules.SpendManagement) is expected
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
        public void GetModuleFromHostNoHostnameReturnsDefaultModule()
        {
            const string Hostname = "";
            const Modules Expected = Modules.SpendManagement;
            var actual = HostManager.GetModule(Hostname);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// A test for GetModule where a hostname is passed that doesn't match any modules, the default value (Modules.SpendManagement) is expected
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
        public void GetModuleFromHostUnknownHostnameReturnsDefaultModule()
        {
            const string Hostname = "421aa90e079fa326b6494f812ad13e79";
            const Modules Expected = Modules.SpendManagement;
            var actual = HostManager.GetModule(Hostname);
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// A test for GetModule where the first known hostname in the metabase is used, the hostname's own moduleID is expected
        /// </summary>
        [TestMethod, TestCategory("Spend Management Library"), TestCategory("Helpers")]
        public void GetModuleFromHostFirstHostnameInMetabaseReturnsOwnModule()
        {
            var db = new DBConnection(GlobalVariables.MetabaseConnectionString);
            using (SqlDataReader reader = db.GetStoredProcReader("GetHostnames"))
            {
                while (reader.Read())
                {
                    if (!reader.IsDBNull(1))
                    {
                        string hostname = reader.GetString(0);
                        var expected = (Modules)reader.GetInt32(1);
                        var actual = HostManager.GetModule(hostname);

                        Assert.AreEqual(expected, actual);
                    }

                    // do as the test says, only test the first.
                    break;
                }

                db.sqlexecute.Parameters.Clear();
                reader.Close();
            }
        }            
    }
}
