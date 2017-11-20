using System;
using SpendManagementLibrary;

namespace UnitTest2012Ultimate.API
{
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spend_Management;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Utilities;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class P11DCategoryTests : BaseTests<P11DCategory, P11DCategoryResponse, cP11dcats>
    {
        #region Consts / Vars

        private const string Label2 = "Unit Test Label 2";
        private cSubcat _subcatA; 
        private cSubcat _subcatB;
        private cCategory _category;

        #endregion Consts / Vars

        #region Test Methods

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("P11DCategoriesController")]
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
                        return toMod;
                    },
                    modified => Assert.AreEqual(modified.Label, Label + LabelDescriptionMod),
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
        [TestCategory("P11DCategorysController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorRecordAlreadyExistsMessage)]
        public void P11DCategoryWithExistingNameFails()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                // add the first
                AddWithAssertions(item);

                // attempt to add the other
                AddWithAssertions(item);

            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("P11DCategorysController")]
        public void UpdateP11DCategoryUpdatesSubCatIds()
        {
            var item = GenerateBasicWorkingItems();
            
            try
            {
                item.ExpenseSubCategoryIds.Add(_subcatA.subcatid);
                
                // add the first
                AddWithAssertions(item, added =>
                {
                    item = added.Item;
                    Assert.AreEqual(item.ExpenseSubCategoryIds.Count, 1);
                });

                item.ExpenseSubCategoryIds.Add(_subcatB.subcatid);
                
                // attempt to update
                UpdateWithAssertions(item, updated =>
                {
                    item = updated.Item;
                    Assert.AreEqual(item.ExpenseSubCategoryIds.Count, 2);
                });

                item.ExpenseSubCategoryIds.RemoveAt(1);

                // attempt to update
                UpdateWithAssertions(item, updated =>
                {
                    item = updated.Item;
                    Assert.AreEqual(item.ExpenseSubCategoryIds.Count, 1);
                });


                item.ExpenseSubCategoryIds.RemoveAt(0);

                // attempt to update again
                UpdateWithAssertions(item, updated => Assert.AreEqual(item.ExpenseSubCategoryIds.Count, 0));

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
        private P11DCategory GenerateBasicWorkingItems()
        {
            CleanupAnyOutstanding();
            InitialIds = Dal.GetAll().Select(x => x.pdcatid).ToList();

            var categories = new cCategories(User.AccountID);
            _category = categories.getCategoryByName(Label);
            if (_category == null)
            {
                categories.addCategory(Label, Description, User.EmployeeID);
                _category = categories.getCategoryByName(Label);
            }

            var subCats = new cSubcats(User.AccountID);
            _subcatA = subCats.GetSubcatByString(Label);
            _subcatB = subCats.GetSubcatByString(Label2);

            if (_subcatA == null)
            {
                _subcatA = new cSubcat(0, _category.categoryid, Label, Description, false, false, false, false, false, false, 10, "",
                    false, false, 0, false, false, CalculationType.NormalItem, false, false, false, "", false, 0, false,
                    false, false, false, false, false, false, false, false, false, false, "", false, false, 0, 0, false,
                    false, new SortedList<int, object>(), DateTime.UtcNow, User.EmployeeID, DateTime.UtcNow,
                    User.EmployeeID, "", false, false, new List<cCountrySubcat>(), new List<int>(), new List<int>(), new List<int>(), false, new List<cSubcatVatRate>(), false,
                    HomeToLocationType.None, null, false, null, false, false, null, null);
                
                _subcatA.updateID(subCats.SaveSubcat(_subcatA));
            }

            if (_subcatB == null)
            {
                _subcatB = new cSubcat(0, _category.categoryid, Label2, Description, false, false, false, false, false, false, 10, "",
                    false, false, 0, false, false, CalculationType.NormalItem, false, false, false, "", false, 0, false,
                    false, false, false, false, false, false, false, false, false, false, "", false, false, 0, 0, false,
                    false, new SortedList<int, object>(), DateTime.UtcNow, User.EmployeeID, DateTime.UtcNow,
                    User.EmployeeID, "", false, false, new List<cCountrySubcat>(), new List<int>(), new List<int>(), new List<int>(), false, new List<cSubcatVatRate>(), false,
                    HomeToLocationType.None, null, false, null, false, false, null, null);

                _subcatB.updateID(subCats.SaveSubcat(_subcatB));
            }
            
            return new P11DCategory
            {
                AccountId = User.AccountID,
                Label = Label,
                ExpenseSubCategoryIds = new List<int>(),
                EmployeeId = User.EmployeeID,
            };
        }

        private void CleanupAnyOutstanding()
        {
            var dal = Dal;

            // remove the original add.
            var newList = dal.GetAll().Select(x => x.pdcatid).ToList();
            var toRemove = new List<int>();
            if (InitialIds != null)
            {
                newList.ForEach(id =>
                {
                    if (!InitialIds.Contains(id))
                    {
                        dal.deleteP11dCat(id);
                        toRemove.Add(id);
                        dal = Dal;
                    }
                });
            }

            toRemove.ForEach(i => newList.Remove(i));
            
            dal.deleteP11dCat(dal.getP11DByName(Label).pdcatid);
            dal.deleteP11dCat(dal.getP11DByName(Label+LabelDescriptionMod).pdcatid);
        }

        #endregion Utilities
    }
}
