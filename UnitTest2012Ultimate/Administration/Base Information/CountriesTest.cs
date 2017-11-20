using System.Collections.Generic;
using System.Data;
using Moq;
using SpendManagementLibrary;
using SpendManagementLibrary.Interfaces;
using UnitTest2012Ultimate.DatabaseMock;
using Utilities.DistributedCaching;

namespace UnitTest2012Ultimate.Administration.Base_Information
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Spend_Management;

    /// <summary>
    /// Unit Tests for Countries.
    /// </summary>
    [TestClass]
    public class CountriesTest
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

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        [TestCleanup()]
        public void MyTestCleanUp()
        {
            HelperMethods.ClearTestDelegateID();
        }

        #endregion

        /// <summary>
        /// The get 2 character country code test.
        /// Get known valid country code and then known invalid country code.
        /// The assumption is made that United Kingdom and Ireland are valid countries in the test database.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void CountriesGetCountryCode()
        {
            var countries = new cCountries(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
            try
            {
                //// test known valid values
                var gbTest = countries.getCountryByCode("GB");
                Assert.IsNotNull(gbTest);
                var gbId = gbTest.CountryId;
                var countryById = countries.getCountryById(gbId);
                Assert.IsNotNull(countryById);
                var gbrTest = countries.getCountryByCode("GBR");
                Assert.IsNotNull(gbrTest);
                var twoThreeTwotest = countries.getCountryByGlobalCountryId(232);
                Assert.IsNotNull(twoThreeTwotest);
                var irltest = countries.getCountryByCode("IRL");
                Assert.IsNotNull(irltest);
                var numeric3Test = countries.getCountryByCode("826");
                Assert.IsNotNull(numeric3Test);
                //// test invalid values
                var invalid2Digit = countries.getCountryByCode("XX");
                Assert.IsNull(invalid2Digit);
                var invalid3Digit = countries.getCountryByCode("XXX");
                Assert.IsNull(invalid3Digit);
                var invalid4Digit = countries.getCountryByCode("XXXX");
                Assert.IsNull(invalid4Digit);
                var invalidglobal = countries.getCountryByCode("999");
                Assert.IsNull(invalidglobal);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        /// <summary>
        /// Checks that it only calls the database once if nothing is saved.
        /// </summary>
        [TestMethod]
        public void CountriesCachesList()
        {
            const string countriesSql = "SELECT countryid, globalcountryid, archived, CreatedOn, CreatedBy, subAccountId FROM dbo.countries";
            const string vatRatesSql = "SELECT countrysubcatid, countryid, subcatid, vat, vatpercent, createdon, createdby FROM dbo.countrysubcats";

            var countriesData =
                Reader.MockReaderDataFromClassData(
                    countriesSql,
                    new List<object>
                        {
                            new cCountry(1, 42, false, new Dictionary<int, ForeignVatRate>(), new DateTime(2013, 1, 1), 1),
                            new cCountry(2, 43, false, new Dictionary<int, ForeignVatRate>(), new DateTime(2013, 1, 2), 2),
                        });
            
            var vatRate = new ForeignVatRate() {CountryId = 1, SubcatId = 1, Vat = 17.5, VatPercent = 17.5};
            var vatRatesData = Reader
                .MockReaderDataFromClassData(vatRatesSql, new List<object> {vatRate})
                .AddField("countrysubcatid", new List<object>{1});
            Mock<IDBConnection> dbConnection = Reader.NormalDatabase(new[] {countriesData, vatRatesData});

            //clear the cache
            var cache = new Cache();
            cache.Delete(1, string.Empty, "countries");
            
            var countries1 = new cCountries(1, null, dbConnection.Object);
            //the database should have been called once:
            dbConnection.Verify(d => d.GetReader(countriesSql, CommandType.Text), Times.Once());
            var country1_1 = countries1.getCountryById(1);
            var country1_2 = countries1.getCountryById(2);
            Assert.IsNotNull(country1_1);
            Assert.IsNotNull(country1_2);
            Assert.AreEqual(42, country1_1.GlobalCountryId);
            Assert.AreEqual(43, country1_2.GlobalCountryId);

            //now create a new cCountries...
            var countries2 = new cCountries(1, null, dbConnection.Object);
            var country2_1 = countries2.getCountryById(1);
            Assert.IsNotNull(country2_1);
            Assert.AreEqual(42, country2_1.GlobalCountryId);
            //...and check that the total calls to the DB is still only one:
            dbConnection.Verify(d => d.GetReader(countriesSql, CommandType.Text), Times.Once());

        }

        /// <summary>
        /// Checks that if you save a country, a subsequent Get retrieves the updated value
        ///  (not the old value because it's just getting it out of the cache)
        /// </summary>
        [TestMethod]
        public void CountriesSaveCountryIsReflected()
        {
            const string countriesSql = "SELECT countryid, globalcountryid, archived, CreatedOn, CreatedBy, subAccountId FROM dbo.countries";
            const string vatRatesSql = "SELECT countrysubcatid, countryid, subcatid, vat, vatpercent, createdon, createdby FROM dbo.countrysubcats";

            var countries = new List<object>
                {
                    new cCountry(1, 42, false, new Dictionary<int, ForeignVatRate>(), new DateTime(2013, 1, 1), 1), 
                    new cCountry(2, 43, false, new Dictionary<int, ForeignVatRate>(), new DateTime(2013, 1, 2), 2),
                };
            var countriesData =
                Reader.MockReaderDataFromClassData(
                    countriesSql,
                    countries);
            
            var vatRate = new ForeignVatRate() { CountryId = 1, SubcatId = 1, Vat = 17.5, VatPercent = 17.5};
            var vatRatesData = Reader
                .MockReaderDataFromClassData(vatRatesSql, new List<object> {vatRate})
                .AddField("countrysubcatid", new List<object> {1});
            Mock<IDBConnection> dbConnection = Reader.NormalDatabase(new[] {countriesData, vatRatesData});

            //clear the cache
            var cache = new Cache();
            cache.Delete(1, string.Empty, "countries");
            
            var countries1 = new cCountries(1, null, dbConnection.Object);
            var country1_1 = countries1.getCountryById(1);
            Assert.IsNotNull(country1_1);
            Assert.AreEqual(42, country1_1.GlobalCountryId);

            //it should have hit the db once by this point
            dbConnection.Verify(c => c.GetReader(countriesSql, CommandType.Text), Times.Once());

            //now save a new country... (with id 0, so it inserts it)
            var newCountry = new cCountry(0, 44, false, new Dictionary<int, ForeignVatRate>(), new DateTime(2013, 06, 01), 1);

            //tell it that when a country is saved, add that country to the ones that are returned by the reader
            dbConnection.Setup(c => c.ExecuteProc("saveCountry")).Callback(() =>
                {
                    countries.Add(newCountry);
                    countriesData = Reader.MockReaderDataFromClassData(countriesSql, countries);
                    dbConnection.Setup(c => c.GetReader(countriesSql, CommandType.Text)).Returns(() => Reader.CreateMockReader(countriesData).Object);
                    //mock the sql identity column id generation:
                    dbConnection.Object.sqlexecute.Parameters["@id"].Value = 3;
                });
            countries1.saveCountry(newCountry, dbConnection.Object);

            //...and check it can get it (from a new countries class, so it doesn't fluke)
            var countries2 = new cCountries(2, null, dbConnection.Object);
            dbConnection.Verify(c => c.GetReader(countriesSql, CommandType.Text), Times.Exactly(1)); //once more, since it's been re-set up
            Assert.AreEqual(3, countries2.list.Count);

        }
    }
}
