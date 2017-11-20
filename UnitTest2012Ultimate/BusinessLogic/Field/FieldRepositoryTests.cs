using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.BusinessLogic.Field
{
    using CacheDataAccess.Fields;
    using CacheDataAccess.Tables;

    using global::BusinessLogic.Accounts;
    using global::BusinessLogic.Accounts.Elements;
    using global::BusinessLogic.Cache;
    using global::BusinessLogic.CurrentUser;
    using global::BusinessLogic.Fields;
    using global::BusinessLogic.Fields.Type.Attributes;
    using global::BusinessLogic.Fields.Type.Base;
    using global::BusinessLogic.Interfaces;
    using global::BusinessLogic.Tables.Type;

    using Moq;

    [TestClass]
    public class FieldRepositoryTests
    {
        
 
        [TestMethod]
        public void FieldRepositoryBaseTest()
        {
            var fieldRepository = new TestFieldRepository();
            var result = fieldRepository[Guid.NewGuid()];
            Assert.IsNull(result);

            var fieldGuid = Guid.NewGuid();

            var field = new DecimalField(
                fieldGuid,
                "DecimalField",
                "A Decimal Field",
                "Comment",
                Guid.NewGuid(),
                "property",
                new FieldAttributes(),
                Guid.NewGuid(),
                10,
                10);

            result = fieldRepository.Add(field);
            Assert.IsTrue(result.Id == fieldGuid);

            result = fieldRepository[fieldGuid];
            Assert.IsTrue(result.Id == fieldGuid);
            Assert.IsTrue(result == field);
        }

        [TestMethod]
        public void CacheFieldRepositoryTest()
        {

            var currentUser = new Mock<ICurrentUser>();
            currentUser.SetupAllProperties();

            var account = new Mock<IAccount>();
            account.SetupAllProperties();

            var cache = new Mock<ICache>();
            cache.SetupAllProperties();
            
            cache.Setup(x => x.Add(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Guid>()), It.IsAny<object>())).Returns(true);
            cache.Setup(x => x.Add(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Guid>()), It.IsAny<object>(), It.IsAny<int>())).Returns(true);


            var fieldGuid = Guid.NewGuid();

            var field = new DecimalField(
                fieldGuid,
                "DecimalField",
                "A Decimal Field",
                "Comment",
                Guid.NewGuid(),
                "property",
                new FieldAttributes(),
                Guid.NewGuid(),
                10,
                10);

            cache.Setup(x => x.Get(new AccountCacheKey(It.IsAny<int>(), It.IsAny<string>(), fieldGuid))).Returns(field);

            var cacheFieldFactory = new CacheFieldFactory(account.Object, cache.Object);

            var result = cacheFieldFactory[fieldGuid];
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Description == "A Decimal Field");

            var guidId2 = Guid.NewGuid();
            var field2 = new DecimalField(
             guidId2,
             "DecimalField2",
             "A Decimal Field2",
             "Comment2",
             Guid.NewGuid(),
             "property2",
             new FieldAttributes(),
             Guid.NewGuid(),
             10,
             10);

            result = cacheFieldFactory.Add(field2);
            Assert.IsTrue(result.Id == guidId2);
            Assert.IsTrue(result.Description == "A Decimal Field2");

        }
    }

    internal class TestFieldRepository : FieldRepository
    {
        
    }
}
