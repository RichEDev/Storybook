using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.BusinessLogic.CustomTable
{
    using global::BusinessLogic.Accounts.Elements;
    using global::BusinessLogic.CustomTables;
    using global::BusinessLogic.Tables.Type;

    [TestClass]
    public class CustomTableRepositoryTests
    {
        [TestMethod]
        public void BaseCustomerTableRepositoryTest()
        {
            var customTableRepository = new TestCustomTableRepository();

            var guidId = Guid.NewGuid();
            var result = customTableRepository[guidId];

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

            result = customTableRepository.Add(table);
            Assert.IsTrue(result.Id == guidId);
            Assert.IsTrue(result.Name == "TestTable");

            result = customTableRepository[guidId];
            Assert.IsTrue(result.Id == guidId);
            Assert.IsTrue(result.Name == "TestTable");
        }

        public void SQLCustomTableRepository()
        {
        
        }

    }

    public class TestCustomTableRepository : CustomTableRepository
    {
        
    }
}
