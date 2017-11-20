namespace UnitTest2012Ultimate.BusinessLogic.CustomEntityField
{
    using System;

    using CacheDataAccess.CustomFields;

    using global::BusinessLogic.Accounts;
    using global::BusinessLogic.Cache;
    using global::BusinessLogic.CurrentUser;
    using global::BusinessLogic.CustomEntities;
    using global::BusinessLogic.Fields.Type.Attributes;
    using global::BusinessLogic.Fields.Type.Base;
    using global::BusinessLogic.Interfaces;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// A suite of tests for the CustomEntityFieldRepository
    /// </summary>
    [TestClass]
    public class CustomEntityFieldRepositoryTests
    {
        /// <summary>
        /// Tests the base repository for Custom Entity Fields
        /// </summary>
        [TestMethod]
        public void CustomEntityFieldRepositoryBaseTest()
        {
            var customEntityFieldRepo = new TestCustomEntityFieldRepository();
            var result = customEntityFieldRepo[Guid.NewGuid()];
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

            result = customEntityFieldRepo.Add(field);
            Assert.IsTrue(result.Id == fieldGuid);

            result = customEntityFieldRepo[fieldGuid];
            Assert.IsTrue(result.Id == fieldGuid);
            Assert.IsTrue(result == field);

        }

        [TestMethod]
        public void CustomEntityFieldRepositoryCacheTest()
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

            var cacheCustomEntityFieldsFactory = new CacheCustomEntityFieldsFactory(currentUser.Object, cache.Object);

            var result = cacheCustomEntityFieldsFactory[fieldGuid];
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

            result = cacheCustomEntityFieldsFactory.Add(field2);
            Assert.IsTrue(result.Id == guidId2);
            Assert.IsTrue(result.Description == "A Decimal Field2");
        }
    }

    internal class TestCustomEntityFieldRepository : CustomEntityFieldRepository
    {
        /// <summary>
        /// Gets the <see cref="IField">IField</see> that matches the supplied field name
        /// </summary>
        /// <param name="name">the field name</param>
        /// <returns>the <see cref="IField">IField</see></returns>
        public override IField this[string name]
        {
            get
            {
                return null;
            }
        }
    }

}
