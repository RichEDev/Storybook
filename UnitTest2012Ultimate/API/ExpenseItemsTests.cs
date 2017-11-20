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
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary;

    using Spend_Management;

    using UnitTest2012Ultimate.API.Stubs;
    using UnitTest2012Ultimate.API.Utilities;

    using Currency = SpendManagementApi.Models.Types.Currency;
    
    [TestClass]
    public class ExpenseItemTests
    {
        private ControllerFactory<ExpenseSubCategory> controllerFactory;
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
            Mock<cSubcats> subcats = new Mock<cSubcats>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            _actionContext.SetSubCategoriesMock(subcats);

            var repository =
                RepositoryFactory.GetRepository<ExpenseSubCategory>(new object[] { Moqs.CurrentUser(), _actionContext });
            controllerFactory = new ControllerFactory<ExpenseSubCategory>(repository);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidUdfMessage)]
        public void ExpenseItemsController_Post_ShouldReturnErrorWhenInvalidUdfId()
        {
            ExpenseSubCategoryResponse response = null;
                
            //Setup
            var repository = RepositoryFactory.GetRepository<ExpenseSubCategory>(new object[] { Moqs.CurrentUser(), null });
            controllerFactory = new ControllerFactory<ExpenseSubCategory>(repository);

            //Get request
            ExpenseSubCategory expenseSubCategory = RequestStubCreator<ExpenseSubCategory>.GetValidExpenseSubCategory();
            expenseSubCategory.ParentCategoryId = _actionContext.ExpenseCategories.CachedList().First().categoryid;
            expenseSubCategory.PdCatId = _actionContext.P11DCategories.GetAll().First().pdcatid;
            expenseSubCategory.MileageCategory = _actionContext.MileageCategories.GetMileageIDs().First();

            expenseSubCategory.Split = new List<ExpenseSubCategory>
                                            {
                                                repository.Get(expenseSubCategory.Split.First().SubCatId)
                                                             
                                            };

            expenseSubCategory.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(-1, -1) };
            expenseSubCategory.AssociatedUdfs = new List<int> { -1 };
            int subcatid = _actionContext.SubCategories.GetSortedList().First().SubcatId;
            expenseSubCategory.EntertainmentId = subcatid;
            expenseSubCategory.PersonalId = subcatid;
            expenseSubCategory.ReimbursableSubCatId = subcatid;
            expenseSubCategory.RemoteId = subcatid;

            //Act
           response = controllerFactory.Post<ExpenseSubCategoryResponse>(expenseSubCategory);

            Assert.Equals(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, ApiResources.MandatoryUdfMissingMessage);
        
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidUdfListItemMessage)]
        public void ExpenseItemsController_Post_ShouldReturnErrorWhenInvalidUdfListItemId()
        {

            ExpenseSubCategoryResponse response = null;
            try
            {
            //Setup
            var repository = RepositoryFactory.GetRepository<ExpenseSubCategory>(new object[] { Moqs.CurrentUser(), null });
            controllerFactory = new ControllerFactory<ExpenseSubCategory>(repository);

            //Get request
            ExpenseSubCategory expenseSubCategory = RequestStubCreator<ExpenseSubCategory>.GetValidExpenseSubCategory();
            expenseSubCategory.ParentCategoryId = _actionContext.ExpenseCategories.CachedList().First().categoryid;
            expenseSubCategory.PdCatId = _actionContext.P11DCategories.GetAll().First().pdcatid;
            expenseSubCategory.MileageCategory = _actionContext.MileageCategories.GetMileageIDs().First();
            List<SubcatBasic> subcats = _actionContext.SubCategories.GetSortedList(true);
            expenseSubCategory.Split = new List<ExpenseSubCategory>
                                        {
                                            repository.Get(expenseSubCategory.Split.First().SubCatId)

                                        };

                cUserDefinedField listUdf =
                    _actionContext.UserDefinedFields.UserdefinedFields.FirstOrDefault(
                        udf => (udf.Value.table.TableName.ToLower().Equals("userdefinedsubcats") && 
                            udf.Value.attribute.fieldtype == FieldType.List)).Value;

            if (listUdf == null)
            {
                Assert.Inconclusive("No user defined field is associated with sub categories");
            }

            cUserDefinedField mandatoryUdf =
                    _actionContext.UserDefinedFields.UserdefinedFields.FirstOrDefault(
                        udf => (udf.Value.table.TableName.ToLower().Equals("userdefinedsubcats") &&
                            udf.Value.mandatory)).Value;

            if (mandatoryUdf == null)
            {
                Assert.Fail("No mandatory user defined field is associated with sub categories");
            }

            expenseSubCategory.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, -1), new UserDefinedFieldValue(mandatoryUdf.userdefineid, "Text") };
            expenseSubCategory.AssociatedUdfs = new List<int> { listUdf.userdefineid, mandatoryUdf.userdefineid };
            int subcatid = subcats.First().SubcatId;
            expenseSubCategory.EntertainmentId = subcatid;
            expenseSubCategory.PersonalId = subcatid;
            expenseSubCategory.ReimbursableSubCatId = subcatid;
            expenseSubCategory.RemoteId = subcatid;

            //Act
        response =  controllerFactory.Post<ExpenseSubCategoryResponse>(expenseSubCategory);
           }

            finally
            {
                if (response != null) controllerFactory.Delete<ExpenseSubCategoryResponse>(response.Item.SubCatId);
            }

        }


        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.MandatoryUdfMissingMessage)]
        public void ExpenseItemsController_Post_ShouldReturnErrorWhenMissingUdf()
        {
            ExpenseSubCategoryResponse response = null;

            try
            {
                      
            //Setup
            var repository = RepositoryFactory.GetRepository<ExpenseSubCategory>(new object[] { Moqs.CurrentUser(), null });
            controllerFactory = new ControllerFactory<ExpenseSubCategory>(repository);

            //Get request
            ExpenseSubCategory expenseSubCategory = RequestStubCreator<ExpenseSubCategory>.GetValidExpenseSubCategory();
            expenseSubCategory.ParentCategoryId = _actionContext.ExpenseCategories.CachedList().First().categoryid;
            expenseSubCategory.PdCatId = _actionContext.P11DCategories.GetAll().First().pdcatid;
            expenseSubCategory.MileageCategory = _actionContext.MileageCategories.GetMileageIDs().First();
            List<SubcatBasic> subcats = _actionContext.SubCategories.GetSortedList(true);
            expenseSubCategory.Split = new List<ExpenseSubCategory>
                                    {
                                        repository.Get(expenseSubCategory.Split.First().SubCatId)

                                    };

                cUserDefinedField listUdf =
                     _actionContext.UserDefinedFields.UserdefinedFields.FirstOrDefault(
                         udf => (udf.Value.table.TableName.ToLower().Equals("userdefinedsubcats") &&
                             udf.Value.attribute.fieldtype == FieldType.List)).Value;

            if (listUdf == null)
            {
                Assert.Fail("No user defined field is associated with sub categories");
            }

            expenseSubCategory.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, -1) };
            expenseSubCategory.AssociatedUdfs = new List<int> { listUdf.userdefineid };
            int subcatid = subcats.First().SubcatId;
            expenseSubCategory.EntertainmentId = subcatid;
            expenseSubCategory.PersonalId = subcatid;
            expenseSubCategory.ReimbursableSubCatId = subcatid;
            expenseSubCategory.RemoteId = subcatid;

            //Act
           response = controllerFactory.Post<ExpenseSubCategoryResponse>(expenseSubCategory);
        }
              finally
            {
                if (response != null) controllerFactory.Delete<ExpenseSubCategoryResponse>(response.Item.SubCatId);
            }

        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void ExpenseItemsController_Post_ShouldReturnSuccessWhenValidRequest()
        {
            ExpenseSubCategoryResponse response = null;
            try
            {           
                //Setup
                var repository = RepositoryFactory.GetRepository<ExpenseSubCategory>(new object[] { Moqs.CurrentUser(), null });                   
                controllerFactory = new ControllerFactory<ExpenseSubCategory>(repository);
   
                  //Get request
                ExpenseSubCategory expenseSubCategory = RequestStubCreator<ExpenseSubCategory>.GetValidExpenseSubCategory();
                expenseSubCategory.ParentCategoryId = _actionContext.ExpenseCategories.CachedList().First().categoryid;
                expenseSubCategory.PdCatId = _actionContext.P11DCategories.GetAll().First().pdcatid;
                expenseSubCategory.MileageCategory = _actionContext.MileageCategories.GetMileageIDs().First();
                List<SubcatBasic> subcats = _actionContext.SubCategories.GetSortedList(true);
                expenseSubCategory.Split = new List<ExpenseSubCategory>
                                            {
                                                repository.Get(expenseSubCategory.Split.First().SubCatId)

                                            };
                expenseSubCategory.UserDefined = null;
                int subcatid = subcats.First().SubcatId;
                expenseSubCategory.EntertainmentId = subcatid;
                expenseSubCategory.PersonalId = subcatid;
                expenseSubCategory.ReimbursableSubCatId = subcatid;
                expenseSubCategory.RemoteId = subcatid;
                expenseSubCategory.AssociatedUdfs = null;

                //Act
                response = controllerFactory.Post<ExpenseSubCategoryResponse>(expenseSubCategory);

                //Assert equality
                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.AreEqual(expenseSubCategory, response.Item);
            }
            finally
            {
                controllerFactory.Delete<ExpenseSubCategoryResponse>(response.Item.SubCatId);
            }
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void ExpenseItemsController_Put_ShouldReturnSuccessWhenValidRequest()
        {
            ExpenseSubCategoryResponse response = null;
            ExpenseSubCategory original = null;
            try
            {
                //Setup
                var repository = RepositoryFactory.GetRepository<ExpenseSubCategory>(new object[] { Moqs.CurrentUser(), null });
                controllerFactory = new ControllerFactory<ExpenseSubCategory>(repository);

                //Get request
                ExpenseSubCategory expenseSubCategory = RequestStubCreator<ExpenseSubCategory>.GetValidExpenseSubCategory();
                expenseSubCategory.ParentCategoryId = _actionContext.ExpenseCategories.CachedList().First().categoryid;
                expenseSubCategory.PdCatId = _actionContext.P11DCategories.GetAll().First().pdcatid;
                expenseSubCategory.MileageCategory = _actionContext.MileageCategories.GetMileageIDs().First();
                List<SubcatBasic> subcats = _actionContext.SubCategories.GetSortedList(true);
                expenseSubCategory.Split = new List<ExpenseSubCategory>
                                            {
                                                repository.Get(expenseSubCategory.Split.First().SubCatId)

                                            };

                cUserDefinedField listUdf =
                    _actionContext.UserDefinedFields.UserdefinedFields.First(
                        udf => udf.Value.attribute.fieldtype == FieldType.List).Value;
                int val = listUdf.items.First().Value.elementValue;
                expenseSubCategory.AssociatedUdfs = new List<int> { listUdf.userdefineid };
                expenseSubCategory.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, val) };

                int subcatid = subcats.First().SubcatId;
                expenseSubCategory.EntertainmentId = subcatid;
                expenseSubCategory.PersonalId = subcatid;
                expenseSubCategory.ReimbursableSubCatId = subcatid;
                expenseSubCategory.RemoteId = subcatid;

                //Act
                response = controllerFactory.Post<ExpenseSubCategoryResponse>(expenseSubCategory);

                //Update record
                original = response.Item;
                original.SubCat = "Subcat Modified";
                original.Description = "Subcat desc modified";
                original.ParentCategoryId = _actionContext.ExpenseCategories.CachedList().Last().categoryid;
                original.PdCatId = _actionContext.P11DCategories.GetAll().Last().pdcatid;
                original.MileageCategory = _actionContext.MileageCategories.GetMileageIDs().Last();
                expenseSubCategory.Split = new List<ExpenseSubCategory>
                                            {
                                                repository.Get(expenseSubCategory.Split.First().SubCatId)

                                            };
                expenseSubCategory.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, val) };

                subcatid = subcats.Last().SubcatId;
                expenseSubCategory.EntertainmentId = subcatid;
                expenseSubCategory.PersonalId = subcatid;
                expenseSubCategory.ReimbursableSubCatId = subcatid;
                expenseSubCategory.RemoteId = subcatid;

                //Act
                response = controllerFactory.Put<ExpenseSubCategoryResponse>(original);

                //Assert equality
                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.AreEqual(original, response.Item);
            }
            finally
            {
                controllerFactory.Delete<ExpenseSubCategoryResponse>(original.SubCatId);
            }
        }
    }
}
