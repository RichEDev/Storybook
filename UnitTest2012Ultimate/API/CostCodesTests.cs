namespace UnitTest2012Ultimate.API
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementApi.Controllers;
    using SpendManagementApi.Models.Types.Employees;
    using SpendManagementApi.Repositories;

    using Spend_Management;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Utilities;
    using SpendManagementLibrary;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class CostCodesTests : BaseTests<CostCode, CostCodeResponse, cCostcodes>
    {

        #region Test Methods

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("CostCodesController")]
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
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidUdfListItemMessage)]
        public void CostCodesController_Post_ShouldReturnErrorWhenInvalidListUdf()
        {
            cUserDefinedField listUdf = GetCostCodeUdf(FieldType.List);

            if (listUdf == null)
            {
                Assert.Inconclusive("No field type {0} user defined field is associated with cost codes", FieldType.List);
            }      
   
                var item = this.GenerateBasicWorkingItems();
                item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, null) };

                try
                {
                    // add on with test names first
                    AddWithAssertions(item,
                        added =>
                        {
                            Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                            Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.InvalidUdfListItemMessage);
                        });
                }
                finally
                {
                    CleanupAnyOutstanding();
                }
            
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidNumberValueMessage)]
        public void CostCodesController_Post_ShouldReturnErrorWhenInvalidValueForNumber()
        {
            cUserDefinedField listUdf = GetCostCodeUdf(FieldType.Number);

            if (listUdf == null)
            {
                Assert.Inconclusive("No field type {0} user defined field is associated with cost codes", FieldType.Number);
            }         

            var item = this.GenerateBasicWorkingItems();
            item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, "text") };

            try
            {
                // add on with test names first
                AddWithAssertions(item,
                    added =>
                    {
                        Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                        Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.InvalidNumberValueMessage);
                    });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidCurrencyValueMessage)]
        public void CostCodesController_Post_ShouldReturnErrorWhenInvalidValueForCurrency()
        {
            cUserDefinedField listUdf = GetCostCodeUdf(FieldType.Currency);

            if (listUdf == null)
            {
                Assert.Inconclusive("No field type {0} user defined field is associated with cost codes", FieldType.Currency);
            }      

            var item = this.GenerateBasicWorkingItems();
            item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, "text") };


            try
            {
                // add on with test names first
                AddWithAssertions(item,
                    added =>
                    {
                        Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                        Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.InvalidCurrencyValueMessage);
                    });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidTickboxValueMessage)]
        public void CostCodesController_Post_ShouldReturnErrorWhenInvalidValueForTickbox()
        {
            cUserDefinedField listUdf = GetCostCodeUdf(FieldType.TickBox);

            if (listUdf == null)
            {
                Assert.Inconclusive("No field type {0} user defined field is associated with cost codes", FieldType.TickBox);
            }            

            var item = this.GenerateBasicWorkingItems();

            item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, "text") };

            try
            {
                // add on with test names first
                AddWithAssertions(item,
                    added =>
                    {
                        Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                        Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.InvalidTickboxValueMessage);
                    });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.MandatoryUdfMissingMessage)]
        public void CostCodesController_Post_ShouldReturnErrorWhenMandatoryElementsNotSupplied()
        {

            cUserDefinedField listUdf = GetCostCodeUdf(FieldType.List);

            if (listUdf == null)
            {
                Assert.Inconclusive("No field type {0} user defined field is associated with cost codes", FieldType.List);
            }

            var item = this.GenerateBasicWorkingItems();
            item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, -1) };
   
            try
            {
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
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidDateOnlyValueMessage)]
        public void CostCodesController_Post_ShouldReturnErrorWhenInvalidValueForDate()
        {
            cUserDefinedField listUdf = GetCostCodeUdf(FieldType.DateTime, AttributeFormat.DateOnly);

            if (listUdf == null)
            {
                Assert.Inconclusive("No field type {0} user defined field is associated with cost codes", FieldType.DateTime);
            }        

            var item = this.GenerateBasicWorkingItems();
            item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, "23-01-2001 12:00:00") };

            try
            {
                // add on with test names first
                AddWithAssertions(item,
                    added =>
                    {
                        Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                        Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.InvalidDateOnlyValueMessage);
                    });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }



        

      

        

        

       
        
        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidUdfMessage)]
        public void CostCodesController_Post_ShouldReturnErrorWhenInvalidUdfId()
        {
            var item = this.GenerateBasicWorkingItems();
            item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(-1, -1) };


            try
            {
                // add on with test names first
                AddWithAssertions(item,
                    added =>
                        {
                            //Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                            //Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.InvalidUdfMessage);
                        });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.MandatoryUdfMissingMessage)]
        public void CostCodesController_Post_ShouldReturnErrorWhenMissingMandatoryUdf()
        {            
            cUserDefinedField listUdf = GetCostCodeUdf(FieldType.List);
            
            if (listUdf == null)
            {
                Assert.Inconclusive("No field type {0} user defined field is associated with cost codes", FieldType.List);
            }  
    
            var item = this.GenerateBasicWorkingItems();
            item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, -1) };


            try
            {
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
                CleanupAnyOutstanding();
            }
        }


        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidTimeOnlyValueMessage)]
        public void CostCodesController_Post_ShouldReturnErrorWhenInvalidValueForTime()
        {
            cUserDefinedField listUdf = GetCostCodeUdf(FieldType.DateTime, AttributeFormat.TimeOnly);

            if (listUdf == null)
            {
                Assert.Inconclusive("No field type {0} user defined field is associated with cost codes", FieldType.DateTime);
            }          

            var item = this.GenerateBasicWorkingItems();
            item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, "23-01-2001 12:00:00") };

    
            try
            {
                // add on with test names first
                AddWithAssertions(item,
                    added =>
                    {
                        Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                        Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.InvalidTimeOnlyValueMessage);
                    });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

      

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidDateTimeValueMessage)]
        public void CostCodesController_Post_ShouldReturnErrorWhenInvalidValueForDateTime()
        {
            cUserDefinedField listUdf = GetCostCodeUdf(FieldType.DateTime, AttributeFormat.DateTime);
         
            if (listUdf == null)
            {
                Assert.Inconclusive("No field type {0} user defined field is associated with cost codes", FieldType.DateTime);
            }          
    
            var item = this.GenerateBasicWorkingItems();

            item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, "text") };


            try
            {
                // add on with test names first
                AddWithAssertions(item,
                    added =>
                    {
                        Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                        Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.InvalidDateTimeValueMessage);
                    });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidListItemIdValueMessage)]
        public void CostCodesController_Post_ShouldReturnErrorWhenInvalidTypeForListItemId()
        {
            cUserDefinedField listUdf = GetCostCodeUdf(FieldType.List);

            if (listUdf == null)
            {
                Assert.Inconclusive("Must have a user defined field associated with cost codes");
            }

            var item = this.GenerateBasicWorkingItems();
            item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, "text") };


            try
            {
                // add on with test names first
                AddWithAssertions(item,
                    added =>
                    {
                        Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                        Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.InvalidListItemIdValueMessage);
                    });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }


        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidRelationshipValueMessage)]
        public void CostCodesController_Post_ShouldReturnErrorWhenInvalidTypeForRelationship()
        {
            cUserDefinedField listUdf = GetCostCodeUdf(FieldType.Relationship);

            if (listUdf == null)
            {
                Assert.Inconclusive("No field type {0} user defined field is associated with cost codes", FieldType.Relationship);
            }      

            var item = this.GenerateBasicWorkingItems();
            item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, "69") };


            try
            {
                // add on with test names first
                AddWithAssertions(item,
                    added =>
                    {
                        Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                        Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.InvalidRelationshipValueMessage);
                    });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }



        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidIntValueMessage)]
        public void CostCodesController_Post_ShouldReturnErrorWhenInvalidValueForIntUdf()
        {
            cUserDefinedField listUdf = GetCostCodeUdf(FieldType.Integer);

            if (listUdf == null)
            {
                Assert.Inconclusive("No field type {0} user defined field is associated with cost codes", FieldType.Integer);
            }    

            var item = this.GenerateBasicWorkingItems();
            item.UserDefined = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(listUdf.userdefineid, "Text") };

            try
            {
                // add on with test names first
                AddWithAssertions(item,
                    added =>
                    {
                        Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                        Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.InvalidIntValueMessage);
                    });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }
        
        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("CostCodesController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorInvalidNameAlreadyExists)]
        public void AddCostCodeWithExistingNameFails()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                // add on with test names first
                AddWithAssertions(item, added => Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Success));

                // try to add another
                AddWithAssertions(item, added =>
                {
                    //Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                    //Assert.AreEqual(added.ResponseInformation.Errors[0].ErrorCode, ApiResources.ApiErrorSaveUnsuccessful);
                    //Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.ApiErrorInvalidNameAlreadyExists);
                });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("CostCodesController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorInvalidDescriptionAlreadyExists)]
        public void AddCostCodeWithExistingDescriptionFails()
        {
            var item = GenerateBasicWorkingItems();

            try
            {
                // add on with test names first
                AddWithAssertions(item, added => Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Success));

                item.Label = Label + LabelDescriptionMod;

                // try to add another
                AddWithAssertions(item, added =>
                {
                    //Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                    //Assert.AreEqual(added.ResponseInformation.Errors[0].ErrorCode, ApiResources.ApiErrorSaveUnsuccessful); 
                    //Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.ApiErrorInvalidDescriptionAlreadyExists);
                });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }
        
        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("CostCodesController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorWrongEmployeeIdMessage)]
        public void AddCostCodeWithBadOwnerFails()
        {
            var item = GenerateBasicWorkingItems();
            var employees = ActionContext.Employees.GetEmployeeIdList();
            var nonExistentId = employees.Last() + 10;
            item.OwnerId = nonExistentId;

            try
            {
                // try to add another
                AddWithAssertions(item, added =>
                {
                    //Assert.AreEqual(added.ResponseInformation.Status, ApiStatusCode.Failure);
                    //Assert.AreEqual(added.ResponseInformation.Errors[0].ErrorCode, ApiResources.ApiErrorSaveUnsuccessful);
                    //Assert.AreEqual(added.ResponseInformation.Errors[0].Message, ApiResources.ApiErrorWrongEmployeeIdMessage);
                });
            }
            finally
            {
                CleanupAnyOutstanding();
            }
        }

        [TestMethod]
        [TestCategory("Api")]
        [TestCategory("CostCodesController")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorInvalidLinkedToEmployee)]
        public void CannotDeleteWhenOwnedByEmmployee()
        {
            GenerateBasicWorkingItems();

            var costCodes = new cCostcodes(User.AccountID);
            var costCode = costCodes.GetCostcodeByString(Label);
            if (costCode == null)
            {
                costCode = new cCostCode(0, Label, Description, false, DateTime.UtcNow, User.EmployeeID,
                    DateTime.UtcNow, User.EmployeeID, new SortedList<int, object>(), null, null, null);
                costCode.CostcodeId = costCodes.SaveCostcode(costCode);
            }

            var departments = new cDepartments(User.AccountID);
            var department = departments.GetDepartmentByName(Label);
            if (department == null)
            {
                department = new cDepartment(0, Label, Description, false, DateTime.UtcNow, User.EmployeeID,
                    DateTime.UtcNow, User.EmployeeID, new SortedList<int, object>());
                department.DepartmentId = departments.SaveDepartment(department);
            }

            var projectCodes = new cProjectCodes(User.AccountID);
            var projectCode = projectCodes.getProjectCodeByString(Label);
            if (projectCode == null)
            {
                projectCode = new cProjectCode(0, Label, Description, false, false, DateTime.UtcNow, User.EmployeeID,
                    DateTime.UtcNow, User.EmployeeID, new SortedList<int, object>());
                projectCode.projectcodeid = projectCodes.saveProjectCode(projectCode);
            }

            var ccbd = new cDepCostItem(department.DepartmentId, costCode.CostcodeId, projectCode.projectcodeid, 100);
            var employees = new cEmployees(User.AccountID);
            var employeeId = employees.getEmployeeidByUsername(User.AccountID, Label);
            var employee = employees.GetEmployeeById(employeeId);
            if (employee == null)
            {
                employee = cEmployeeObject.GetUTEmployeeTemplateObject();
                employee.Username = Label;
            }

            employeeId = employees.SaveEmployee(employee, new[] { ccbd }, new List<int>(), new SortedList<int, object>());
            
            try
            {
                if (employeeId > 1)
                {
                    // attempt to delete
                    DeleteWithAssertions(costCode.CostcodeId, deleted =>
                    {
                        //Assert.AreEqual(deleted.ResponseInformation.Status, ApiStatusCode.Failure);
                        //Assert.AreEqual(deleted.ResponseInformation.Errors[0].ErrorCode, ApiResources.ApiErrorDeleteUnsuccessful);
                        //Assert.AreEqual(deleted.ResponseInformation.Errors[0].Message, ApiResources.ApiErrorInvalidLinkedToEmployee);
                    });
                }
            }
            finally
            {
                //employees.SaveEmployee(employee, new cDepCostItem[]{}, new List<int>(), new SortedList<int, object>());
                employee.GetCostBreakdown().Clear();
                employee.Save(User);
                var id = projectCodes.deleteProjectCode(projectCode.projectcodeid, User.EmployeeID);
                id = costCodes.DeleteCostCode(costCode.CostcodeId, User.EmployeeID);
                id = departments.DeleteDepartment(department.DepartmentId, User.EmployeeID);
                id = employee.Delete(User);

                CleanupAnyOutstanding();
            }
        }

        #endregion Test Methods

        #region Utilities

        private CostCode GenerateBasicWorkingItems()
        {
            CleanupAnyOutstanding();
            InitialIds = Dal.GetCostcodeIds().OfType<int>().ToList();

            return new CostCode
            {
                Label = Label,
                Description = Description,
                Archived = false,
                AccountId = User.AccountID,
                OwnerId = User.EmployeeID,
                OwnerType = SpendManagementElement.Employees,
                UserDefined = new List<UserDefinedFieldValue>(),
                EmployeeId = User.EmployeeID
            };
        }

        private void CleanupAnyOutstanding()
        {
            var dal = Dal;

            var newList = dal.GetCostcodeIds();
            if (InitialIds != null)
            {
                foreach (var id in newList)
                {
                    if (!InitialIds.Contains((int) id))
                    {
                        dal.DeleteCostCode((int) id, User.EmployeeID);
                        dal = Dal;
                    }
                }
            }

            var item = dal.GetCostcodeByString(Label);
            if (item != null)
            {
                dal.DeleteCostCode(item.CostcodeId, User.EmployeeID);
                dal = Dal;
            }

            item = dal.GetCostcodeByString(Label + LabelDescriptionMod);
            if (item != null)
            {
                dal.DeleteCostCode(item.CostcodeId, User.EmployeeID);
            }
        }

        private cUserDefinedField GetCostCodeUdf(FieldType fieldType)
        {
            return ActionContext.UserDefinedFields.UserdefinedFields.FirstOrDefault(
                udf => (udf.Value.table.TableName.ToLower().Equals("userdefined_costcodes") &&
                        udf.Value.attribute.fieldtype == fieldType)).Value;
        }

        private cUserDefinedField GetCostCodeUdf(FieldType fieldType, AttributeFormat attributeFormat)
        {
            return ActionContext.UserDefinedFields.UserdefinedFields.FirstOrDefault(
                udf => (udf.Value.table.TableName.ToLower().Equals("userdefined_costcodes") &&
                        udf.Value.attribute.fieldtype == fieldType &&
                        ((cDateTimeAttribute)udf.Value.attribute).format == attributeFormat)).Value;
        }

        #endregion Utilities
    }
}
