namespace UnitTest2012Ultimate.BusinessLogic.FinancialYear
{
    using System;
    using System.Data;

    using global::BusinessLogic.Accounts;
    using global::BusinessLogic.CurrentUser;
    using global::BusinessLogic.Databases;
    using global::BusinessLogic.DataConnections;
    using global::BusinessLogic.Logging;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SQLDataAccess.FinancialYears;

    using Utilities.Cryptography;

    /// <summary>
    /// A suite of tests for the financial year repository 
    /// </summary>
    [TestClass]
    public class FinancialYearRepositoryTests
    {
        /// <summary>
        /// Tests the SQL element of the financial year repository
        /// </summary>
        [TestMethod]
        public void FinancialYearSQLTests()
        {
            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();

            var logger = new Mock<ILogger>();
            logger.SetupAllProperties();
            var dataParameters = new Mock<DataParameters>();
            dataParameters.SetupAllProperties();

            var accountRepository = new Mock<AccountRepository>();
            accountRepository.SetupAllProperties();
            var cryptography = new Mock<ICryptography>();
            cryptography.SetupAllProperties();
            var databaseCatalogue = new DatabaseCatalogue(new DatabaseServer(1, "localhost"), "test", "test", "test", cryptography.Object);

            accountRepository.Setup(x => x[It.IsAny<int>()]).Returns(new Account(1, databaseCatalogue));
      
            var ds = new DataSet();
            var testData = new DataTable("FinancialYears");
            testData.Columns.Add("yearstart");
            testData.Columns.Add("yearend");
            testData.Columns.Add("Primary", typeof(int));

            var dataRow = testData.NewRow();
            dataRow[0] = "06/04";
            dataRow[1] = "05/04";
            dataRow[2] = 1;

            testData.Rows.Add(dataRow);

            ds.Tables.Add(testData);

            var dataconnection = new TestDataConnection(dataParameters.Object, ds);

            var sqlFinancialYearRepo = new SqlFinancialYearFactory(dataconnection);

            DateTime[] result = sqlFinancialYearRepo.GetFinancialYear();

            int currentYear = DateTime.Now.Year;
            Assert.IsTrue(result[0].Date == new DateTime(currentYear, 04,06));
            Assert.IsTrue(result[1].Date == new DateTime(currentYear + 1, 04, 05));         
        }
    }
}
