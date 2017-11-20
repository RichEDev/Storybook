using System.Linq;

namespace UnitTest2012Ultimate.API
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spend_Management;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Utilities;
    using System.Collections.Generic;

    [TestClass]
    public class ExpenseCategoryTests : BaseTests<ExpenseCategory, ExpenseCategoryResponse, cCategories>
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("ExpenseCategoriesController")]
        public void RoundTrip()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                AddEditDeleteFullCycle(
                    item,
                    added =>
                    {
                        Assert.IsNotNull(added);
                        Assert.AreNotEqual(added.Id, 0);
                        item = added;
                    },
                    toMod =>
                    {
                        toMod.Label += LabelDescriptionMod;
                        toMod.Description += LabelDescriptionMod;
                        return toMod;
                    },
                    modified =>
                    {
                        Assert.AreEqual(modified.Label, Label + LabelDescriptionMod);
                        Assert.AreEqual(modified.Description, Description + LabelDescriptionMod);
                    },
                    toDelete =>
                    {
                        InitialIds.Remove(toDelete.Id);
                        return toDelete.Id;
                    },
                    Assert.IsNull);
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("ExpenseCategorysController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorInvalidNameAlreadyExists)]
        public void ExpenseCategoryWithExistingNameFails()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                // add the first
                AddWithAssertions(item);

                item.Description += LabelDescriptionMod;

                // attempt to add the other
                AddWithAssertions(item, added =>
                {
                    Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                    Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.ApiErrorInvalidNameAlreadyExists);
                    Assert.IsNull(added.Item);
                });

            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        #endregion Test Methods

        #region Utilities

        /// <summary>
        /// Returns and allowance with correct properties for AccountId, CurrencyId, EmployeeId, Label, and Description.
        /// The AllowanceRates property is set to null. The instance variables dal and intiialAllowanceIds are also set...
        /// </summary>
        /// <returns>A new Allowance object populated as per summary.</returns>
        private ExpenseCategory GenerateBasicWorkingItems()
        {
            CleanupAnyOutstanding();
            InitialIds = Dal.getCategoryIds();

            return new ExpenseCategory
            {
                AccountId = User.AccountID,
                Label = Label,
                Description = Description,
                EmployeeId = User.EmployeeID,
            };
        }

        private void CleanupAnyOutstanding()
        {
            var dal = Dal;

            // remove the original add.
            var newList = dal.getCategoryIds();
            var toRemove = new List<int>();
            if (InitialIds != null)
            {
                newList.ForEach(id =>
                {
                    if (!InitialIds.Contains(id))
                    {
                        dal.deleteCategory(id, User);
                        toRemove.Add(id);
                        dal = Dal;
                    }
                });
            }

            toRemove.ForEach(i => newList.Remove(i));
            
            var item = dal.getCategoryByName(Label);
            int did;
            if (item != null)
            {
                did = dal.deleteCategory(item.categoryid, User);
                if (did != 0)
                {
                    var subcats = new cSubcats(User.AccountID);
                    subcats.GetSortedList().Where(s => s.CategoryId == item.categoryid)
                        .ToList()
                        .ForEach(s => subcats.DeleteSubcat(s.SubcatId));
                    dal.deleteCategory(item.categoryid, User);
                }
            }
            item = dal.getCategoryByName(Label+LabelDescriptionMod);
            if (item != null)
            {
                did = dal.deleteCategory(item.categoryid, User);
                if (did != 0)
                {
                    var subcats = new cSubcats(User.AccountID);
                    subcats.GetSortedList().Where(s => s.CategoryId == item.categoryid)
                        .ToList()
                        .ForEach(s => subcats.DeleteSubcat(s.SubcatId));
                    dal.deleteCategory(item.categoryid, User);
                }
            }
            
        }

        #endregion Utilities
    }
}
