using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Responses;
using SpendManagementApi.Models.Types;
using SpendManagementApi.Utilities;
using Spend_Management;

namespace UnitTest2012Ultimate.API
{
    [TestClass]
    public class BudgetHolderTests : BaseTests<BudgetHolder, BudgetHolderResponse, cBudgetholders>
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("BudgetHoldersController")]
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
        [TestCategory("BudgetHoldersController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorWrongEmployeeIdMessage)]
        public void BudgetHolderWithBadEmployeeFails()
        {
            var item = GenerateBasicWorkingItems();
            item.EmployeeId = -1000;

            try
            {
                AddWithAssertions(item, r =>
                {
                    //Assert.AreEqual(r.ResponseInformation.Status, ApiStatusCode.Failure);
                    //Assert.AreEqual(r.ResponseInformation.Errors[0].ErrorCode, ApiResources.ApiErrorWrongEmployeeId);
                });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("BudgetHoldersController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorSaveUnsuccessfulMessage)]
        public void BudgetHolderWithExistingNameFails()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                // add the first
                AddWithAssertions(item);

                item.Id = 0;

                // attempt to add the other
                AddWithAssertions(item, added =>
                {
                    Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
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
        private BudgetHolder GenerateBasicWorkingItems()
        {
            CleanupAnyOutstanding();
            InitialIds = Dal.GetList().Select(x => x.budgetholderid).ToList();

            return new BudgetHolder
            {
                AccountId = User.AccountID,
                Label = Label,
                Description = Description,
                EmployeeId = User.EmployeeID
            };
        }

        private void CleanupAnyOutstanding()
        {
            var dal = Dal;

            // remove the original add.
            var newList = dal.GetList().Select(x => x.budgetholderid).ToList();
            var toRemove = new List<int>();
            if (InitialIds != null)
            {
                newList.ForEach(id =>
                {
                    if (!InitialIds.Contains(id))
                    {
                        dal.deleteBudgetHolder(id);
                        toRemove.Add(id);
                        dal = Dal;
                    }
                });
            }

            toRemove.ForEach(i => newList.Remove(i));
            newList.ForEach(id =>
            {
                var item = dal.getBudgetHolderById(id);
                if (item != null &&
                    (item.budgetholder == Label || item.budgetholder == Label + LabelDescriptionMod))
                {
                    dal.deleteBudgetHolder(id);
                }
            });
        }

        #endregion Utilities
    }
}
