using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Responses;
using SpendManagementApi.Models.Types;
using SpendManagementApi.Utilities;
using Spend_Management;

namespace UnitTest2012Ultimate.API
{
    using System.IO;

    [TestClass]
    public class AllowanceTests : BaseTests<Allowance, AllowanceResponse, cAllowances>
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("AllowancesController")]
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
                    toDelete => toDelete.Id,
                    Assert.IsNull);
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("AllowancesController")]
        public void NewAllowanceRatesGetSavedWithNewAllowance()
        {
            var allowanceRateAId = -1;
            var allowanceRateBId = -1;

            var item = GenerateBasicWorkingItems();
            item.AllowanceRates = new List<AllowanceRate>
            {
                new AllowanceRate {HourThreshold = 2, HourlyRate = 2}, // A
                new AllowanceRate {HourThreshold = 4, HourlyRate = 4} // B
            };


            try
            {
                AddWithAssertions(item, added =>
                {
                    item = added.Item;

                    Assert.IsNotNull(added.Item.AllowanceRates);
                    Assert.AreEqual(added.Item.AllowanceRates.Count, 2);

                    Assert.IsNotNull(added.Item.AllowanceRates[0]);
                    allowanceRateAId = added.Item.AllowanceRates[0].Id;
                    Assert.AreNotEqual(allowanceRateAId, 0);
                    Assert.AreNotEqual(allowanceRateAId, -1);

                    Assert.IsNotNull(added.Item.AllowanceRates[1]);
                    allowanceRateBId = added.Item.AllowanceRates[1].Id;
                    Assert.AreNotEqual(allowanceRateBId, 0);
                    Assert.AreNotEqual(allowanceRateBId, -1);
                });
                ActionContext.SetAllowancesMock(new Mock<cAllowances>(User.AccountID));
                var dbItem = Dal.getAllowanceById(item.Id);
                Assert.IsNotNull(dbItem.getAllowanceBreakdownByID(allowanceRateAId));
                Assert.IsNotNull(dbItem.getAllowanceBreakdownByID(allowanceRateBId));
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("AllowancesController")]
        public void AllowanceRatesGetAddedUpdatedAndDeletedWithUpdateAllowance()
        {
            var allowanceRateAId = -1;
            var allowanceRateBId = -1;
            var allowanceRateCId = -1;

            var item = GenerateBasicWorkingItems();
            item.AllowanceRates = new List<AllowanceRate>
            {
                new AllowanceRate {HourThreshold = 2, HourlyRate = 2}, // A
                new AllowanceRate {HourThreshold = 4, HourlyRate = 4} // B
            };

            try
            {
                AddWithAssertions(item, added =>
                {
                    item = added.Item;
                    allowanceRateAId = added.Item.AllowanceRates[0].Id;
                    allowanceRateBId = added.Item.AllowanceRates[1].Id;
                });

                // make edits
                item.Label += LabelDescriptionMod;
                item.Description += LabelDescriptionMod;
                item.AllowanceRates[0].HourlyRate = 2000;
                item.AllowanceRates[0].HourThreshold = 2000;
                item.AllowanceRates.RemoveAt(1);
                item.AllowanceRates.Add(new AllowanceRate { HourThreshold = 10, HourlyRate = 10 });


                UpdateWithAssertions(item, updated =>
                {
                    item = updated.Item;

                    // should have 2 allowance rates still as one should be removed.
                    Assert.IsNotNull(updated.Item.AllowanceRates);
                    Assert.AreEqual(updated.Item.AllowanceRates.Count, 2);

                    // the second allowance rate should be a different one, grab it's id
                    Assert.IsNotNull(updated.Item.AllowanceRates[1]);
                    allowanceRateCId = updated.Item.AllowanceRates[1].Id;
                    Assert.AreNotEqual(allowanceRateBId, 0);
                    Assert.AreNotEqual(allowanceRateBId, -1);
                });
                ActionContext.SetAllowancesMock(new Mock<cAllowances>(User.AccountID));
                // now make sure the one that the list reflects the changes.
                var dbItem = Dal.getAllowanceById(item.Id);
                Assert.IsNotNull(dbItem.getAllowanceBreakdownByID(allowanceRateAId));
                Assert.IsNull(dbItem.getAllowanceBreakdownByID(allowanceRateBId));
                Assert.IsNotNull(dbItem.getAllowanceBreakdownByID(allowanceRateCId));

            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("AllowancesController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorWrongCurrencyIdMessage)]
        public void AllowanceWithWrongCurrencyIdFails()
        {
            var item = GenerateBasicWorkingItems();
            item.CurrencyId = -1;

            try
            {
                AddWithAssertions(item);
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
        private Allowance GenerateBasicWorkingItems()
        {
            var currencies = new cCurrencies(User.AccountID, null);
            var currency = currencies.getCurrencyByAlphaCode("GBP");

            CleanupAnyOutstanding();
            InitialIds = Dal.getAllowanceIds();

            return new Allowance
            {
                AccountId = User.AccountID,
                Label = Label,
                Description = Description,
                EmployeeId = User.EmployeeID,
                CurrencyId = currency.currencyid,
                AllowanceRates = null
            };
        }

        private void CleanupAnyOutstanding()
        {
            var dal = Dal;

            var newList = dal.getAllowanceIds();
            var toRemove = new List<int>();
            if (InitialIds != null)
            {
                newList.ForEach(id =>
                {
                    if (InitialIds.Contains(id)) return;
                    toRemove.Add(id);
                    dal.deleteAllowance(id);
                    dal = Dal;
                });
            }

            toRemove.ForEach(i => newList.Remove(i));
            newList.ForEach(id =>
            {
                var allowance = dal.getAllowanceById(id);
                if (allowance != null &&
                    (allowance.allowance == Label || allowance.allowance == Label + LabelDescriptionMod))
                {
                    dal.deleteAllowance(id);
                }

            });
        }

        #endregion Utilities
    }
}
