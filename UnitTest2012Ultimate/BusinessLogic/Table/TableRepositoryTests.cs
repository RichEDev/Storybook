namespace UnitTest2012Ultimate.BusinessLogic.Table
{
    using System;

    using global::BusinessLogic.Accounts;
    using global::BusinessLogic.Accounts.Elements;
    using global::BusinessLogic.Cache;
    using global::BusinessLogic.CurrentUser;
    using global::BusinessLogic.Tables;
    using global::BusinessLogic.Tables.Type;

    using CacheDataAccess.Tables;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class TableRepositoryTests
    {
        [TestMethod]
        public void BaseTableRepositoryTest()
        {
            var tableRepository = new TestTableRepository();

            var guidId = Guid.NewGuid();
            var result = tableRepository[guidId];

            var table = new MetabaseTable("TestTable", 0, "TestTableDescription", false, false, false, false, guidId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ModuleElements.AccessKeys, false, "paramLabel");

            result = tableRepository.Add(table);
            Assert.IsTrue(result.Id == guidId);
            Assert.IsTrue(result.Name == "TestTable");

            result = tableRepository[guidId];
            Assert.IsTrue(result.Id == guidId);
            Assert.IsTrue(result.Name == "TestTable");
        }

        [TestMethod]
        public void CacheTableRepositoryTest()
        {
            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();

            var account  = new Mock<IAccount>();
            account.SetupAllProperties();

            var cache = new Mock<ICache>();
            cache.SetupAllProperties();
            
            cache.Setup(x => x.Add(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Guid>()), It.IsAny<object>())).Returns(true);
            cache.Setup(x => x.Add(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Guid>()), It.IsAny<int>())).Returns(true);

            var guidId = Guid.NewGuid();

            var table = new MetabaseTable("TestTable", 0, "TestTableDescription", false, false, false, false, guidId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ModuleElements.AccessKeys, false, "paramLabel");

            cache.Setup(x => x.Get(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), guidId))).Returns(table);

            var cacheTableFactory = new CacheTableFactory(account.Object, cache.Object);

            var result = cacheTableFactory[guidId];
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Description == "TestTableDescription");

            var guidId2 = Guid.NewGuid();
            var table2 = new MetabaseTable("TestTable2", 0, "TestTableDescription2", false, false, false, false, guidId2, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ModuleElements.AccessKeys, false, "paramLabel");
                    
            result = cacheTableFactory.Add(table2);
            Assert.IsTrue(result.Id == guidId2);
            Assert.IsTrue(result.Description == "TestTableDescription2");

        }

        [TestMethod]
        public void SqlTableRepositoryTest()
        {
        }
    }

    public class TestTableRepository : TableRepository
    {
        /// <summary>
        /// Gets an instance of <see cref="ITable"/> with a matching name from memory if possible.
        /// </summary>
        /// <param name="name">The name of the <see cref="ITable"/> you want to retrieve</param>
        /// <returns>The required <see cref="ITable"/> or null if it cannot be found</returns>
        public override ITable this[string name]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="ITable"/> which is the parent of the <see cref="System.Guid"/> given.
        /// </summary>
        /// <param name="id">The </param>
        /// <returns></returns>
        public override ITable GetParentTable(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
