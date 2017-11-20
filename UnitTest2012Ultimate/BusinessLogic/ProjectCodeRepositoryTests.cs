namespace UnitTest2012Ultimate.BusinessLogic
{
    using System;
    using System.Data;

    using CacheDataAccess.ProjectCodes;

    using global::BusinessLogic.Accounts;
    using global::BusinessLogic.Cache;
    using global::BusinessLogic.CurrentUser;
    using global::BusinessLogic.Databases;
    using global::BusinessLogic.DataConnections;
    using global::BusinessLogic.Logging;
    using global::BusinessLogic.ProjectCodes;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SQLDataAccess.ProjectCodes;

    using Utilities.Cryptography;

    [TestClass]
    public class ProjectCodeRepositoryTests
    {
        [TestMethod]
        public void BaseProjectCodeRepositoryTests()
        {
            var projectCodes = new TestProjectCodeRepository();

            var result = projectCodes[666];
            Assert.IsNull(result);

            result = projectCodes.Add(new ProjectCode(1, "ref", "desc", false, false));
            Assert.IsTrue(result.Id == 1);

            result = projectCodes.Add(new ProjectCode(666, "ref666", "desc666", false, false));
            Assert.IsTrue(result.Id == 666);

            var result2 = projectCodes[666];

            Assert.IsTrue(result == result2);

            result = projectCodes[1];
            Assert.IsTrue(result.Id == 1);

        }

        internal class TestProjectCodeRepository : ProjectCodeRepository
        {
        }

        [TestMethod]
        public void CacheProjectCodeRepositoryTest()
        {
            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();
            var cache = new Mock<ICache>();
            cache.SetupAllProperties();
            cache.Setup(x => x.Add(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), It.IsAny<object>())).Returns(true);
            cache.Setup(x => x.Add(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), It.IsAny<object>(), It.IsAny<int>())).Returns(true);

             cache.Setup(x => x.Get(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), 666))).Returns(new ProjectCode(666, "ref666fromcache", "desc666fromcache", false, false));

            var projectCodes = new CacheProjectCodesFactory(currentUser.Object, cache.Object);

            var result = projectCodes[666];
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Description == "desc666fromcache");

            result = projectCodes.Add(new ProjectCode(1, "ref", "desc", false, false));
            Assert.IsTrue(result.Id == 1);

            result = projectCodes.Add(new ProjectCode(666, "ref666", "desc666", false, false));
            Assert.IsTrue(result.Id == 666);

            var result2 = projectCodes[666];
            Assert.IsTrue(result.Description == "desc666");
            Assert.IsTrue(result == result2);

            result = projectCodes[1];
            Assert.IsTrue(result.Id == 1);
        }

        [TestMethod]
        public void SqlProjectCodeRepositoryTest()
        {
            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();
            var cache = new Mock<ICache>();
            cache.SetupAllProperties();
            cache.Setup(x => x.Add(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), It.IsAny<object>())).Returns(true);
            cache.Setup(x => x.Add(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), It.IsAny<int>())).Returns(true);
            cache.Setup(x => x.Get(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), 666))).Returns(new ProjectCode(666, "ref666fromcache", "desc666fromcache", false, false));

            var logger = new Mock<ILogger>();
            logger.SetupAllProperties();
            var dataParameters = new Mock<DataParameters>();
            dataParameters.SetupAllProperties();

            var accountRepository = new Mock<AccountRepository>();
            accountRepository.SetupAllProperties();
            var cryptography = new Mock<ICryptography>();
            cryptography.SetupAllProperties();
            var databaseCatalogue = new DatabaseCatalogue(new global::BusinessLogic.Databases.DatabaseServer(1, "localhost"), "test", "test", "test", cryptography.Object);

            accountRepository.Setup(x => x[It.IsAny<int>()]).Returns(new global::BusinessLogic.Accounts.Account(666, databaseCatalogue));

            var testData = new DataTable();
            testData.Columns.Add("projectcode");
            testData.Columns.Add("description");
            testData.Columns.Add("archived", typeof(bool));
            testData.Columns.Add("rechargeable", typeof(bool));
            testData.Columns.Add("projectCodeId", typeof(int));
            testData.Columns.Add("udf123", typeof(int));

            var dataRow = testData.NewRow();
            dataRow[0] = "1";
            dataRow[1] = "desc1";
            dataRow[2] = false;
            dataRow[3] = false;
            dataRow[4] = 1;
            dataRow[5] = DBNull.Value;
            testData.Rows.Add(dataRow);

            //----------------------------
            dataRow = testData.NewRow();
            dataRow[0] = "1235";
            dataRow[1] = "desc1235";
            dataRow[2] = false;
            dataRow[3] = false;
            dataRow[4] = 1235;
            dataRow[5] = 1235811;
            testData.Rows.Add(dataRow);

            var dataconnection = new TestDataConnection(dataParameters.Object, testData);
            
            var projectCodes = new SqlProjectCodesFactory(dataconnection, currentUser.Object, cache.Object);
            var result = projectCodes[1];
            Assert.IsTrue(result.Id == 1);
            Assert.IsTrue(result.Description == "desc1");
            result = projectCodes[666];
            Assert.IsTrue(result.Id == 666);
            Assert.IsTrue(result.Description == "desc666fromcache");
            Assert.IsTrue(result.UserDefinedFieldValues.Count == 0);
            result = projectCodes[1235];
            Assert.IsTrue(result.Id == 1235);
            Assert.IsTrue(result.Description == "desc1235");
            Assert.IsTrue(result.UserDefinedFieldValues.Count == 1);
            Assert.IsTrue(result.UserDefinedFieldValues[123].ToString() == "1235811");
        }
    }
}
