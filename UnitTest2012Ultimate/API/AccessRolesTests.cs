using System.Linq;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Utilities;

namespace UnitTest2012Ultimate.API
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using Spend_Management;

    [TestClass]
    public class AccessRolesTests : BaseTests<AccessRole, AccessRoleResponse, cAccessRoles>
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("AccessRolesController")]
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
        [TestCategory("AccessRolesController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorRecordAlreadyExists)]
        public void AddAccessRoleWithExistingNameFailsWithNiceError()
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
                AddWithAssertions(item, added =>
                {
                    //Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                    //Assert.AreEqual(added.ResponseInformation.Errors[0].ErrorCode, ApiResources.ApiErrorRecordAlreadyExists);
                    //Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.ApiErrorRecordAlreadyExistsMessage);
                });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("AccessRolesController")]
        public void EditAccessRoleToChangeAccessDetailsSucceeds()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                // add a new one
                AddWithAssertions(item, added =>
                {
                    item = added.Item;
                    Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Success);
                    Assert.AreEqual(item.ElementAccess.Count(), 1);
                });

                var accessRights = new AccessRights(true, true, true, true);
                // now make the change to item, add another element access
                item.ElementAccess.Add(new ElementAccessDetail
                {
                    Id = (int)SpendManagementElement.Api,
                    AccessRights = accessRights
                });

                // add the same one again
                UpdateWithAssertions(item, updated =>
                {
                    item = updated.Item;
                    var returnedElement = item.ElementAccess.Last();
                    Assert.AreEqual(updated.ResponseInformation.Status, ApiStatusCode.Success);
                    Assert.AreEqual(item.ElementAccess.Count(), 2);
                    Assert.AreEqual(returnedElement.Id, (int)SpendManagementElement.Api);
                    Assert.AreEqual(returnedElement.AccessRights.CanAdd, accessRights.CanAdd);
                    Assert.AreEqual(returnedElement.AccessRights.CanEdit, accessRights.CanEdit);
                    Assert.AreEqual(returnedElement.AccessRights.CanDelete, accessRights.CanDelete);
                    Assert.AreEqual(returnedElement.AccessRights.CanView, accessRights.CanView);
                });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("AccessRolesController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorDeleteUnsuccessful)]
        public void DeleteAccessRoleInUseFails()
        {
            GenerateBasicWorkingItems();
            var dal = Dal;
            var id = dal.GetRoleIds().Last();
            try
            {
                DeleteWithAssertions(id, deleted =>
                {
                    //Assert.AreEqual(deleted.ResponseInformation.Status, ApiStatusCode.Failure);
                    //Assert.AreEqual(deleted.ResponseInformation.Errors[0].ErrorCode, ApiResources.ApiErrorDeleteUnsuccessful);
                    //Assert.AreEqual(deleted.ResponseInformation.Errors[0].Message,
                    //    ApiResources.ApiErrorDeleteUnsuccessfulMessage +
                    //    ApiResources.ApiErrorDeleteUnsuccessfulMessageAccessRoleEmployees);
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
        private AccessRole GenerateBasicWorkingItems()
        {
            CleanupAnyOutstanding();
            InitialIds = Dal.GetRoleIds();

            var accessRights = new AccessRights(true, true, true, true);

            var elementAccess = new ElementAccessDetail
            {
                Id = (int)SpendManagementElement.AccessRoles,
                Label = Label,
                AccessRights = accessRights,
            };

            return new AccessRole
            {
                Id = 0,
                AccountId = User.AccountID,
                Label = Label,
                Description = Description,
                EmployeeId = User.EmployeeID,
                ExpenseClaimMaximumAmount = 100,
                ExpenseClaimMinimumAmount = 10,
                CanEditCostCode = false,
                CanEditDepartment = false,
                CanEditProjectCode = false,
                MustHaveBankAccount = false,
                AccessLevel = AccessRoleLevel.AllData,
                AccessRoleLinks = new List<int>(),
                CustomEntityAccess = new List<CustomEntityGroupAccess>(),
                ElementAccess = new List<ElementAccessDetail> { elementAccess }
            };
        }

        private void CleanupAnyOutstanding()
        {
            var dal = Dal;

            var newList = dal.GetRoleIds();
            if (InitialIds != null)
            {
                newList.ForEach(id =>
                {
                    if (InitialIds.Contains(id)) return;
                    dal.DeleteAccessRole(id, User.EmployeeID,
                        User.isDelegate ? User.Delegate.EmployeeID : (int?)null);
                    dal = Dal;
                });
            }

            var item = dal.GetAccessRoleByName(Label);
            if (item != null)
            {
                dal.DeleteAccessRole(item.RoleID, User.EmployeeID, User.isDelegate ? User.Delegate.EmployeeID : (int?)null);
                dal = Dal;
            }

            item = dal.GetAccessRoleByName(Label + LabelDescriptionMod);
            if (item != null)
            {
                dal.DeleteAccessRole(item.RoleID, User.EmployeeID, User.isDelegate ? User.Delegate.EmployeeID : (int?)null);
            }

        }

        #endregion Utilities
    }
}
