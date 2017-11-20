namespace MetabaseDatabaseUnitTest
{
    using Microsoft.Data.Tools.Schema.Sql.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class DatabaseSetup
    {
        [AssemblyInitialize()]
        public static void InitializeAssembly(TestContext ctx)
        {
            //   Setup the test database based on setting in the
            // configuration file
            SqlDatabaseTestClass.TestService.DeployDatabaseProject();
            SqlDatabaseTestClass.TestService.GenerateData();
        }

    }
}
