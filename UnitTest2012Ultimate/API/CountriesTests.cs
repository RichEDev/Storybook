using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.API
{
    using System.Collections.Generic;

    using Moq;

    using SpendManagementApi.Controllers;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Repositories;

    using SpendManagementLibrary;

    using Spend_Management;

    using UnitTest2012Ultimate.API.Stubs;
    using UnitTest2012Ultimate.API.Utilities;

    using Currency = SpendManagementApi.Models.Types.Currency;
    using CurrencyType = SpendManagementApi.Common.Enums.CurrencyType;

    [TestClass]
    public class CountriesTests
    {
        private ControllerFactory<Country> controllerFactory;
        private static TestActionContext _actionContext;
        private Mock<ICurrentUser> _currentUser;

        [ClassInitialize]
        public static void MyClassInitialize(TestContext context)
        {
            _actionContext = new TestActionContext();
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// The my class cleanup.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Mock<cGlobalCountries> globalCountries = new Mock<cGlobalCountries>(MockBehavior.Strict);
            _actionContext.SetGlobalCountriesMock(globalCountries);

            Mock<cCountries> countries = new Mock<cCountries>(MockBehavior.Strict, GlobalTestVariables.AccountId, null, null);
            _actionContext.SetCountriesMock(countries);

            Mock<cSubcats> subcats = new Mock<cSubcats>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            _actionContext.SetSubCategoriesMock(subcats);

            var repository =
                RepositoryFactory.GetRepository<Country>(new object[] { Moqs.CurrentUser(), _actionContext });
            controllerFactory = new ControllerFactory<Country>(repository);
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void CountriesController_Post_ShouldReturnSuccessWhenValidRequest()
        {
            CountryResponse response = null;
            try
            {
                //Setup
                var repository = RepositoryFactory.GetRepository<Country>(new object[] { Moqs.CurrentUser(), null });
                controllerFactory = new ControllerFactory<Country>(repository);

                //Setup dependencies
                GetCountriesResponse getAllResponse = controllerFactory.GetAll<GetCountriesResponse>();
                int globalCountryId = getAllResponse.List.FirstOrDefault(curr => curr.CountryId == 0).GlobalCountryId;

                //Get request
                Country country = RequestStubCreator<Country>.GetValidCountry();
                country.CountryId = 0;
                country.GlobalCountryId = globalCountryId;

                country.VatRates.ToList()[0].ExpenseSubCategoryId =
                    _actionContext.SubCategories.GetSortedList().First().SubcatId;
                
                //Act
                response = controllerFactory.Post<CountryResponse>(country);

                //Assert equality
                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.AreEqual(country, response.Item);
            }
            finally
            {
                controllerFactory.Delete<CountryResponse>(response.Item.CountryId);
            }
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void CountriesController_Put_ShouldReturnSuccessWhenValidRequest()
        {
            CountryResponse response = null;
            try
            {
                //Setup
                var repository = RepositoryFactory.GetRepository<Country>(new object[] { Moqs.CurrentUser(), null });
                controllerFactory = new ControllerFactory<Country>(repository);

                //Setup dependencies
                GetCountriesResponse getAllResponse = controllerFactory.GetAll<GetCountriesResponse>();
                int globalCountryId = getAllResponse.List.FirstOrDefault(ctry => ctry.CountryId == 0).GlobalCountryId;

                //Get request
                Country country = RequestStubCreator<Country>.GetValidCountry();
                country.CountryId = 0;
                country.GlobalCountryId = globalCountryId;
                country.VatRates.ToList()[0].ExpenseSubCategoryId =
                    _actionContext.SubCategories.GetSortedList().First().SubcatId;

                //Add record
                response = controllerFactory.Post<CountryResponse>(country);

                //Set up for update
                Country original = response.Item;
                original.Archived = false;
                original.VatRates.ToList()[0].Vat = 200;
                original.VatRates.ToList()[0].VatPercent = 200;
                country.VatRates.ToList()[0].ExpenseSubCategoryId =
                    _actionContext.SubCategories.GetSortedList().Last().SubcatId;
                
                //Update record
                response = controllerFactory.Put<CountryResponse>(original);

                //Assert
                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.AreEqual(original, response.Item);
            }
            finally
            {
                controllerFactory.Delete<CountryResponse>(response.Item.CountryId);
            }
        }
    }
}
