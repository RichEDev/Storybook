using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cAddressTest and is intended
    ///to contain all cAddressTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cAddressTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for cAddress Constructor
        ///</summary>
        [TestMethod()]
        public void cAddressConstructorTest()
        {
            int addressid = 99;
            string address_title = "Nibley House";
            string addr1 = "Low Moor Road";
            string addr2 = "Doddington Road";
            string town = "Lincoln";
            string county = "Lincs";
            string postcode = "LN6 3JY";
            int countryid = 77;
            string switchboard = "01522 881300";
            string fax = "01522 881355";
            bool private_address = true;
            DateTime createddate = new DateTime(2008, 12, 25, 1, 2, 3);
            int createdbyid = 66;
            DateTime modifieddate = new DateTime(2008, 1, 1, 1, 4, 5, 6);
            int modifiedbyid = 55;
            cAddress target = new cAddress(addressid, address_title, addr1, addr2, town, county, postcode, countryid, switchboard, fax, private_address, createddate, createdbyid, modifieddate, modifiedbyid);

            Assert.AreEqual(target.AddressId, 99);
            Assert.AreEqual(target.AddressTitle, "Nibley House");
            Assert.AreEqual(target.AddressLine1, "Low Moor Road");
            Assert.AreEqual(target.AddressLine2, "Doddington Road");
            Assert.AreEqual(target.Town, "Lincoln");
            Assert.AreEqual(target.County, "Lincs");
            Assert.AreEqual(target.PostCode, "LN6 3JY");
            Assert.AreEqual(target.CountryId, 77);
            Assert.AreEqual(target.Switchboard, "01522 881300");
            Assert.AreEqual(target.Fax, "01522 881355");
            Assert.AreEqual(target.IsPrivateAddress, true);
            Assert.AreEqual(target.CreatedDate, new DateTime(2008, 12, 25, 1, 2, 3));
            Assert.AreEqual(target.CreatedById, 66);
            Assert.AreEqual(target.LastModifiedDate, new DateTime(2008, 1, 1, 1, 4, 5, 6));
            Assert.AreEqual(target.LastModifiedById, 55);
        }
    }
}
