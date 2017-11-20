namespace UnitTest2012Ultimate.BusinessLogic.Module
{
    using System.Data;

    using global::BusinessLogic.Accounts.Modules;
    using global::BusinessLogic.CurrentUser;
    using global::BusinessLogic.DataConnections;
    using global::BusinessLogic.ProductModules;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SQLDataAccess.Modules;

    using Utilities.Cryptography;

    /// <summary>
    /// Tests the Module Repository
    /// </summary>
    [TestClass]
    public class ModuleRepositoryTests
    {


        /// <summary>
        /// Tests the base repository of the Element Repository
        /// </summary>
        [TestMethod]
        public void BaseModuleRepositoryTests()
        {
            var moduleRepo = new TestModuleRepository();

            var greenlightModule = new GreenLightProductModule(1, "GreenLight", "GreenLightDesc", "GreenLightBrand");
            var corpDModule = new CorporateDilligenceProductModule(2, "CorpD", "CorpDDesc", "CorpDBrand");
            var expenseModule = new ExpensesProductModule(3, "Expenses", "ExpensesDesc", "ExpensesBrand");
            var frameworkModule = new FrameworkProductModule(4, "Framework", "FrameworkDesc", "FrameworkBrand");
            var greenLightWorkForceModule = new GreenLightWorkForceProductModule(5, "GreenLightWorkForce", "GreenLightWorkForceDesc", "GreenLightWorkForceBrand");

            var result = moduleRepo[666];
            Assert.IsNull(result);

            result = moduleRepo.Add(greenlightModule);
            Assert.IsTrue(result.Id == 1);

            result = moduleRepo[1];
            Assert.IsTrue(result.Id == 1);

            result = moduleRepo.Add(corpDModule);
            Assert.IsTrue(result.Id == 2);

            result = moduleRepo[2];
            Assert.IsTrue(result.Id == 2);

            result = moduleRepo.Add(expenseModule);
            Assert.IsTrue(result.Id == 3);

            result = moduleRepo[3];
            Assert.IsTrue(result.Id == 3);

            result = moduleRepo.Add(frameworkModule);
            Assert.IsTrue(result.Id == 4);

            result = moduleRepo[4];
            Assert.IsTrue(result.Id == 4);

            result = moduleRepo.Add(greenLightWorkForceModule);
            Assert.IsTrue(result.Id == 5);

            result = moduleRepo[5];
            Assert.IsTrue(result.Id == 5);
        }

        /// <summary>
        /// Tests the SQL aspect of the Element Repository
        /// </summary>
        [TestMethod]
        public void SqlModuleRepositoryTest()
        {  
            var cryptography = new Mock<ICryptography>();
            cryptography.SetupAllProperties();
       
            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();

            var dataParameters = new Mock<DataParameters>();
            dataParameters.SetupAllProperties();

            var testData = new DataTable();
            testData.Columns.Add("moduleID", typeof(int));
            testData.Columns.Add("moduleName");
            testData.Columns.Add("description");
            testData.Columns.Add("brandName");
        
            var dataRow = testData.NewRow();
            dataRow[0] = "1";
            dataRow[1] = "Module1";
            dataRow[2] = "Description1";
            dataRow[3] = "Brand1";       
            testData.Rows.Add(dataRow);

            dataRow = testData.NewRow();
            dataRow[0] = "2";
            dataRow[1] = "Module2";
            dataRow[2] = "Description2";
            dataRow[3] = "Brand2";
            testData.Rows.Add(dataRow);

            var dataconnection = new TestDataConnection(dataParameters.Object, testData);
            var moduleFactory = new SqlModulesFactory(dataconnection);
            
            IProductModule result = moduleFactory[1];
            Assert.IsTrue(result.Id == 1);
            Assert.IsTrue(result.BrandName == "Brand1");
           
            result = moduleFactory[2];
            Assert.IsTrue(result.Id == 2);
            Assert.IsTrue(result.BrandName == "Brand2");
        }
    }


    internal class TestModuleRepository : ModuleRepository
    {

    }
}
