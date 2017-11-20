namespace UnitTest2012Ultimate.API
{
    using System.Linq;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using Spend_Management;

    [TestClass]
    public class ClaimReasonsTests : BaseTests<ClaimReason, ClaimReasonResponse, cReasons>
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("ClaimReasonsController")]
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
        [TestCategory("ClaimReasonsController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorRecordAlreadyExistsMessage)]
        public void AddReasonWithExistingNameFailsWithNiceError()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                // add a new one
                AddWithAssertions(item, added =>
                {
                    Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Success);
                    item = added.Item;
                });

                // add the same one again
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
        private ClaimReason GenerateBasicWorkingItems()
        {
            CleanupAnyOutstanding();
            InitialIds = Dal.getReasonIds().OfType<int>().ToList();

            return new ClaimReason
            {
                Id = 0,
                Label = Label,
                Description = Description,
                AccountCodeNoVat = "AC_NO_VAT",
                AccountCodeVat = "AC_VAT"
            };

        }

        private void CleanupAnyOutstanding()
        {
            var dal = Dal;

            var newList = dal.getReasonIds();
            if (InitialIds != null)
            {
                foreach (var id in newList)
                {
                    if (!InitialIds.Contains((int) id))
                    {
                        dal.deleteReason((int) id);
                        dal = Dal;
                    }
                }
            }

            var item = dal.getReasonByString(Label);
            if (item != null)
            {
                dal.deleteReason(item.reasonid);
                dal = Dal;
            }                

            item = dal.getReasonByString(Label + LabelDescriptionMod);
            if (item != null)
            {
                dal.deleteReason(item.reasonid);
            }
        }

        #endregion Utilities
    }
}
