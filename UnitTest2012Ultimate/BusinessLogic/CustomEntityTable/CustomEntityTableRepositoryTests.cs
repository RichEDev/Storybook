using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.BusinessLogic.Table
{
    using global::BusinessLogic.Accounts.Elements;
    using global::BusinessLogic.CustomEntities;
    using global::BusinessLogic.Tables.Type;

    [TestClass]
    public class CustomEntityTableRepositoryTests
    {
        [TestMethod]
        public void BaseTableRepositoryTest()
        {
            var tableRepository = new TestTableRepository();

            var guidId = Guid.NewGuid();
            var result = tableRepository[guidId];

            var table = new MetabaseTable(
                "TestTable",
                0,
                "TestTableDescription",
                false,
                false,
                false,
                false,
                guidId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                ModuleElements.AccessKeys,
                false,
                "paramLabel");

            result = tableRepository.Add(table);
            Assert.IsTrue(result.Id == guidId);
            Assert.IsTrue(result.Name == "TestTable");

            result = tableRepository[guidId];
            Assert.IsTrue(result.Id == guidId);
            Assert.IsTrue(result.Name == "TestTable");
        }



        [TestMethod]
        public void SqlCustomEntityTableRepositoryTest()
        {


        }
    }

    public class TestCustomEntityTableRepository : CustomEntityTableRepository
    {
    }
}
