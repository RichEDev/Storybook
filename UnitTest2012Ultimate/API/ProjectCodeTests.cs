namespace UnitTest2012Ultimate.API
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using Spend_Management;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Utilities;
    using System.Collections.Generic;

    [TestClass]
    public class ProjectCodeTests : BaseTests<ProjectCode, ProjectCodeResponse, cProjectCodes>
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("ProjectCodesController")]
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
        [TestCategory("ProjectCodesController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorInvalidNameAlreadyExists)]
        public void AddProjectCodeWithExistingNameFails()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                // add on with test names first
                AddWithAssertions(item, added => Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Success));

                item.Id = 0;
                item.Description = Description + LabelDescriptionMod;

                // try to add another
                AddWithAssertions(item);
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("ProjectCodesController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorInvalidDescriptionAlreadyExists)]
        public void AddProjectCodeWithExistingDescriptionFails()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                // add on with test names first
                AddWithAssertions(item, added => Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Success));

                item.Id = 0;
                item.Label = Label + LabelDescriptionMod;

                // try to add another
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
        private ProjectCode GenerateBasicWorkingItems()
        {
            CleanupAnyOutstanding();
            InitialIds = Dal.getProjectcodeIds().OfType<int>().ToList();

            return new ProjectCode
            {
                AccountId = User.AccountID,
                EmployeeId = User.EmployeeID,
                Label = Label,
                Description = Description,
                UserDefined = new List<UserDefinedFieldValue>(),
                Archived = false,
                Rechargeable = false
            };
        }

        private void CleanupAnyOutstanding()
        {
            var dal = Dal;

            // remove the original add.
            var newList = dal.getProjectcodeIds().OfType<int>().ToList();
            var toRemove = new List<int>();
            if (InitialIds != null)
            {
                newList.ForEach(id =>
                {
                    if (!InitialIds.Contains(id))
                    {
                        dal.deleteProjectCode(id, User.EmployeeID);
                        toRemove.Add(id);
                        dal = Dal;
                    }
                });
            }

            toRemove.ForEach(i => newList.Remove(i));
            var pc = dal.getProjectCodeByDesc(Description);
            int did;
            if (pc != null)
            {
                did = dal.deleteProjectCode(pc.projectcodeid, User.EmployeeID);
                if (did != 0)
                {

                    
                }
            }
            pc = dal.getProjectCodeByDesc(Description + LabelDescriptionMod);
            if (pc != null)
            {
                did = dal.deleteProjectCode(pc.projectcodeid, User.EmployeeID);
                if (did != 0)
                {


                }
            }
        }

        #endregion Utilities
    }
}
