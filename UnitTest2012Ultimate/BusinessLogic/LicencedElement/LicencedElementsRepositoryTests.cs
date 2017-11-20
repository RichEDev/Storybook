using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.BusinessLogic.LicencedElement
{
    using global::BusinessLogic.Accounts.Elements;
    using global::BusinessLogic.ProductModules.Licensing;

    [TestClass]
    public class LicencedElementsRepositoryTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var LicencedElementRepository = new TestLicencedElementRepository();

            var result = LicencedElementRepository[0];
            Assert.IsNull(result);

            var element = new Element(1, 1, "TestElement", "TestElementDescription", false, false, false, "TestFriendlyName", false);

            result = LicencedElementRepository.Add(element);
            Assert.IsTrue(((Element)result).Id == 1);
            Assert.IsTrue(result.Description == "TestElementDescription");

            result = LicencedElementRepository[1];
            Assert.IsTrue(((Element)result).Id == 1);
            Assert.IsTrue(result.Description == "TestElementDescription");

        }
    }

    internal class TestLicencedElementRepository : LicencedElementRepository
    {
     
    }
}
