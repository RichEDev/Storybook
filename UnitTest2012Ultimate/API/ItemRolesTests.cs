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
    
    [TestClass]
    public class ItemRolesTests
    {
        private ControllerFactory<ItemRole> controllerFactory;
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
            Mock<cItemRoles> itemRoles = new Mock<cItemRoles>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            _actionContext.SetItemRolesMock(itemRoles);

            Mock<cSubcats> subcats = new Mock<cSubcats>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            _actionContext.SetSubCategoriesMock(subcats);

            var repository =
                RepositoryFactory.GetRepository<ItemRole>(new object[] { Moqs.CurrentUser(), _actionContext });
            controllerFactory = new ControllerFactory<ItemRole>(repository);
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void ItemRolesController_Post_ShouldReturnSuccessWhenValidRequest()
        {
            ItemRoleResponse response = null;
            try
            {
                //Setup
                var repository = RepositoryFactory.GetRepository<ItemRole>(new object[] { Moqs.CurrentUser(), null });
                controllerFactory = new ControllerFactory<ItemRole>(repository);

                //Get request
                ItemRole itemRole = RequestStubCreator<ItemRole>.GetValidItemRole();
                itemRole.SubCatItemRoles = new List<SubCatItemRole> { 
                    new SubCatItemRole
                        {
                            AddToTemplate = true,
                            SubCatId = _actionContext.SubCategories.GetSortedList().First().SubcatId,
                            MaximumAllowedWithoutReceipt = 100,
                            MaximumAllowedWithReceipt = 100
                        }
                };
                
                //Act
                response = controllerFactory.Post<ItemRoleResponse>(itemRole);

                //Assert equality
                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.AreEqual(itemRole, response.Item);
            }
            finally
            {
                controllerFactory.Delete<ItemRoleResponse>(response.Item.ItemRoleId);
            }
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void ItemRolesController_Put_ShouldReturnSuccessWhenValidRequest()
        {
            ItemRoleResponse response = null;
            try
            {
                //Setup
                var repository = RepositoryFactory.GetRepository<ItemRole>(new object[] { Moqs.CurrentUser(), null });
                controllerFactory = new ControllerFactory<ItemRole>(repository);

                //Get request
                ItemRole itemRole = RequestStubCreator<ItemRole>.GetValidItemRole();
                itemRole.SubCatItemRoles = new List<SubCatItemRole> { 
                    new SubCatItemRole
                        {
                            AddToTemplate = true,
                            SubCatId = _actionContext.SubCategories.GetSortedList().First().SubcatId,
                            MaximumAllowedWithoutReceipt = 100,
                            MaximumAllowedWithReceipt = 100
                        }
                };

                //Act
                response = controllerFactory.Post<ItemRoleResponse>(itemRole);

                //Update record
                ItemRole original = response.Item;
                original.Description = "Modified description";
                original.RoleName = "Modified role name";
                original.SubCatItemRoles[0].SubCatId =
                    _actionContext.SubCategories.GetSortedList().OrderByDescending(r => r.SubcatId).First().SubcatId;

                //Act
                response = controllerFactory.Put<ItemRoleResponse>(response.Item);
                
                //Assert equality
                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.AreEqual(original, response.Item);
            }
            finally
            {
                controllerFactory.Delete<ItemRoleResponse>(response.Item.ItemRoleId);
            }
        }

        //[TestMethod]
        //[TestCategory("EndToEnd")]
        //public void CountriesController_Put_ShouldReturnSuccessWhenValidRequest()
        //{
        //    CountryResponse response = null;
        //    try
        //    {
        //        //Setup
        //        var repository = RepositoryFactory.GetRepository<Country>(new object[] { Moqs.CurrentUser(), null });
        //        controllerFactory = new ControllerFactory<Country>(repository);

        //        //Setup dependencies
        //        GetCountriesResponse getAllResponse = controllerFactory.GetAll<GetCountriesResponse>();
        //        int globalCountryId = getAllResponse.List.FirstOrDefault(ctry => ctry.CountryId == 0).GlobalCountry.GlobalCountryid;

        //        //Get request
        //        Country country = RequestStubCreator<Country>.GetValidCountry();
        //        country.CountryId = 0;
        //        country.GlobalCountryId = globalCountryId;
        //        country.VatRates.ToList()[0].ExpenseSubCategoryId =
        //            _actionContext.SubCategories.subcats.First().Value.subcatid;

        //        //Add record
        //        response = controllerFactory.Post<CountryResponse>(country);

        //        //Set up for update
        //        Country original = response.Item;
        //        original.Archived = false;
        //        original.VatRates.ToList()[0].Vat = 200;
        //        original.VatRates.ToList()[0].VatPercent = 200;
        //        country.VatRates.ToList()[0].ExpenseSubCategoryId =
        //            _actionContext.SubCategories.subcats.Last().Value.subcatid;
                
        //        //Update record
        //        response = controllerFactory.Put<CountryResponse>(original);

        //        //Assert
        //        Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
        //        Assert.AreEqual(original, response.Item);
        //    }
        //    finally
        //    {
        //        controllerFactory.Delete<CountryResponse>(response.Item.CountryId);
        //    }
        //}
    }
}
