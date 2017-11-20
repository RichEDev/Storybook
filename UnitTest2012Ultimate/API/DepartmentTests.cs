using Moq;
using SpendManagementApi.Utilities;

namespace UnitTest2012Ultimate.API
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;

    using SpendManagementLibrary;

    using Spend_Management;
    using SpendManagementLibrary;

    [TestClass]
    public class DepartmentTests : BaseTests<Department, DepartmentResponse, cDepartments>
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("DepartmentsController")]
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
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidUdfMessage)]
        public void DepartmentsController_Post_ShouldReturnErrorWhenInvalidUdfId()
        {
            var item = this.GenerateBasicWorkingItems();
            item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(-1, -1) };

                // add on with test names first
                AddWithAssertions(item,
                    added =>
                    {
                        Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                        Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.InvalidUdfMessage);
                    });
            
           
            }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.MandatoryUdfMissingMessage)]
        public void DepartmentsController_Post_ShouldReturnErrorWhenMissingMandatoryUdf()
        {
            var item = this.GenerateBasicWorkingItems();

            var listItems = new SortedList<int, cListAttributeElement>();
            cListAttributeElement attribute = cListAttributeElementObject.Template(elementValue: 20, elementText: "UDFDeptItem", sequence: 300);
            listItems.Add(attribute.elementOrder, attribute);
            cUserDefinedField listUdf = null;

            try
            {
                
           

            int udfId = ActionContext.UserDefinedFields.SaveUserDefinedField(
               new cUserDefinedField(
                   0,
                   ActionContext.Tables.GetTableByName("userdefinedDepartments"),
                   1,
                   null,
                   DateTime.UtcNow,
                   User.EmployeeID,
                   null,
                   null,
                   new cListAttribute(
                       0,
                       "DeptListAttrib",
                       "DeptListAttrib",
                       string.Empty,
                       "tooltip",
                       false,
                       FieldType.List,
                       DateTime.UtcNow,
                       User.EmployeeID,
                       null,
                       null,
                       listItems,
                       Guid.NewGuid(),
                       false,
                       true,
                       AttributeFormat.DateTime,
                       false,
                       false,
                       false,
                       false),
                   null,
                   false,
                   false,
                   false,
                   false));

            ActionContext.SetUserDefinedFieldsMock(new Mock<cUserdefinedFields>(User.AccountID));
            listUdf = ActionContext.UserDefinedFields.GetUserDefinedById(udfId);

            if (listUdf != null)
            {
                item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, -1) };
            }

                // add on with test names first
                AddWithAssertions(item,
                    added =>
                    {
                        Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                        Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.MandatoryUdfMissingMessage);
                    });

            }
            finally
            {
                if (listUdf != null) ActionContext.UserDefinedFields.DeleteUserDefined(listUdf.userdefineid);
            }
                      
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidUdfListItemMessage)]
        public void DepartmentsController_Post_ShouldReturnErrorWhenInvalidListUdf()
        {
            var item = new Department
            {
                AccountId = User.AccountID,
                Label = Label,
                Description = Description,
                EmployeeId = User.EmployeeID
            };

            var listItems = new SortedList<int, cListAttributeElement>();
            cListAttributeElement attribute = cListAttributeElementObject.Template(elementValue: 20, elementText: "UDFDeptItem", sequence: 300);
            listItems.Add(attribute.elementOrder, attribute);
            cUserDefinedField listUdf = null;

            try
            {
            int udfId = ActionContext.UserDefinedFields.SaveUserDefinedField(
                new cUserDefinedField(
                    0,
                    ActionContext.Tables.GetTableByName("userdefinedDepartments"),
                    1,
                    null,
                    DateTime.UtcNow,
                    User.EmployeeID,
                    null,
                    null,
                    new cListAttribute(
                        0,
                        "DeptListAttrib",
                        "DeptListAttrib",
                        string.Empty,
                        "tooltip",
                        false,
                        FieldType.List,
                        DateTime.UtcNow,
                        User.EmployeeID,
                        null,
                        null,
                        listItems,
                        Guid.NewGuid(),
                        false,
                        true,
                        AttributeFormat.DateTime,
                        false,
                        false,
                        false,
                        false),
                    null,
                    false,
                    false,
                    false,
                    false));

          ActionContext.SetUserDefinedFieldsMock(new Mock<cUserdefinedFields>(User.AccountID));
           listUdf = ActionContext.UserDefinedFields.GetUserDefinedById(udfId);

            if (listUdf != null)
            {
                item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, listUdf) };
            }            
                AddWithAssertions(item,
                    added =>
                    {
                                      Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                                      Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.InvalidUdfListItemMessage);
                    });
            }
            finally
            {
                if (listUdf != null) ActionContext.UserDefinedFields.DeleteUserDefined(listUdf.userdefineid);
               
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("DepartmentsController")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.ApiErrorInvalidDescriptionAlreadyExists)]
        public void DepartmentWithExistingDescriptionFails()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                // add the first
                AddWithAssertions(item);

                item.Label += LabelDescriptionMod;

                // attempt to add the other
      
                AddWithAssertions(item, added =>
                {
                    Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                    Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.ApiErrorRecordAlreadyExists);
                    Assert.IsNull(added.Item);
                });


            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("DepartmentsController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorInvalidNameAlreadyExists)]
        public void DepartmentWithExistingNameFails()
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
        private Department GenerateBasicWorkingItems()
        {
            CleanupAnyOutstanding();
            InitialIds = Dal.GetDepartmentIds().OfType<int>().ToList();

            return new Department
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
            var newList = dal.GetDepartmentIds().OfType<int>().ToList();
            var toRemove = new List<int>();
            if (InitialIds != null)
            {
                newList.ForEach(id =>
                {
                    if (!InitialIds.Contains(id))
                    {
                        dal.DeleteDepartment(id, User.EmployeeID);
                        toRemove.Add(id);
                        dal = Dal;
                    }
                });
            }

            toRemove.ForEach(i => newList.Remove(i));
            newList.ForEach(id =>
            {
                var item = dal.GetDepartmentById(id);
                if (item != null &&
                    (item.Department == Label || item.Department == Label + LabelDescriptionMod))
                {
                    dal.DeleteDepartment(id, User.EmployeeID);
                }
            });
        }

        #endregion Utilities
    }
}
