namespace UnitTest2012Ultimate.API
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementApi.Controllers;
    using SpendManagementApi.Controllers.V1;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Repositories;
    using Stubs;
    using Utilities;
    using Currency = SpendManagementApi.Models.Types.Currency;
   
    [TestClass]
    public class CurrenciesTests
    {
        private ControllerFactory<Currency> controllerFactory;
        private static TestActionContext _actionContext;
        private static CurrenciesV1Controller _currenciesController;

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
            var repository =
                RepositoryFactory.GetRepository<Currency>(new object[] { Moqs.CurrentUser(), _actionContext });
            controllerFactory = new ControllerFactory<Currency>(repository);
            _currenciesController = (CurrenciesV1Controller)controllerFactory.GetController();
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void CurrenciesController_Post_ShouldReturnSuccessWhenValidRequest()
        {
            CurrencyResponse response = null;
            try
            {
                //Setup
                var repository = RepositoryFactory.GetRepository<Currency>(new object[] { Moqs.CurrentUser(), null });
                controllerFactory = new ControllerFactory<Currency>(repository);

                //Setup dependencies
                GetCurrenciesResponse getAllResponse = controllerFactory.GetAll<GetCurrenciesResponse>();
                int globalCurrencyId = getAllResponse.List.FirstOrDefault(curr => curr.CurrencyId == 0).GlobalCurrencyId;

                //Get request
                Currency currency = RequestStubCreator<Currency>.GetValidCurrency();
                currency.CurrencyId = 0;
                currency.GlobalCurrencyId = globalCurrencyId;

                //Act
                response = controllerFactory.Post<CurrencyResponse>(currency);

                //Assert equality
                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.IsTrue(response.Item.CurrencyId > 0);
            }
            finally
            {
                controllerFactory.Delete<CurrencyResponse>(response.Item.CurrencyId);
            }
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void CurrenciesController_Put_ShouldReturnSuccessWhenValidRequest()
        {
            CurrencyResponse response = null;
            try
            {
                //Setup
                var repository = RepositoryFactory.GetRepository<Currency>(new object[] { Moqs.CurrentUser(), null });
                controllerFactory = new ControllerFactory<Currency>(repository);

                //Setup dependencies
                GetCurrenciesResponse getAllResponse = controllerFactory.GetAll<GetCurrenciesResponse>();
                int globalCurrencyId = getAllResponse.List.FirstOrDefault(curr => curr.CurrencyId == 0).GlobalCurrencyId;

                //Get request
                Currency currency = RequestStubCreator<Currency>.GetValidCurrency();
                currency.CurrencyId = 0;
                currency.GlobalCurrencyId = globalCurrencyId;

                //Add record
                response = controllerFactory.Post<CurrencyResponse>(currency);

                //Set up for update
                Currency original = response.Item;
                original.Archived = false;

                //Update record
                response = controllerFactory.Put<CurrencyResponse>(original);

                //Assert
                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.IsTrue(response.Item.CurrencyId > 0);
            }
            finally
            {
                controllerFactory.Delete<CurrencyResponse>(response.Item.CurrencyId);
            }
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void CurrenciesController_GetGlobalCurrencyId_ShouldReturnValidRequest()
        {
            CurrencyResponse response = null;
            try
            {
                //Setup
                var repository = RepositoryFactory.GetRepository<Currency>(new object[] { Moqs.CurrentUser(), null });
                controllerFactory = new ControllerFactory<Currency>(repository);

                //Setup dependencies
                GetCurrenciesResponse getAllResponse = controllerFactory.GetAll<GetCurrenciesResponse>();
                int globalCurrencyId = getAllResponse.List.FirstOrDefault(curr => curr.CurrencyId == 0).GlobalCurrencyId;

                //Get request
                Currency currency = RequestStubCreator<Currency>.GetValidCurrency();
                currency.CurrencyId = 0;
                currency.GlobalCurrencyId = globalCurrencyId;

                //Save currency
                response = controllerFactory.Post<CurrencyResponse>(currency);

                //Assert currency save
                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.IsTrue(response.Item.CurrencyId > 0);

                //Get currency by global Id
                CurrencyResponse globalCurrencyResponse = _currenciesController.ByGlobalCurrencyId(globalCurrencyId);

                //Assert currencyId equality 
                Assert.AreEqual(response.Item.CurrencyId, globalCurrencyResponse.Item.CurrencyId);
            }
            finally
            {
                controllerFactory.Delete<CurrencyResponse>(response.Item.CurrencyId);
            }
        }
    }
}
