using System.Linq;

namespace UnitTest2012Ultimate.API
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpendManagementApi.Controllers;
    using SpendManagementApi.Controllers.V1;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Models.Types.Employees;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Interfaces;

    using Spend_Management;

    using Stubs;
    using Utilities;
    using SpendManagementApi.Repositories;

    using CostCentreBreakdown = SpendManagementApi.Models.Types.Employees.CostCentreBreakdown;

    [TestClass]
    public class EmployeeTests
    {
        private ControllerFactory<Employee> controllerFactory;
        private int _testAccessRoleId;
        private static TestActionContext _actionContext;

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
            var currentUser = new Mock<ICurrentUser>();
            int accountId = GlobalTestVariables.AccountId;
            currentUser.SetupGet(u => u.AccountID).Returns(accountId);
            currentUser.SetupGet(u => u.CurrentSubAccountId).Returns(1);
            var repository = RepositoryFactory.GetRepository<Employee>(new object[] { currentUser.Object, _actionContext });

            var locales = new Mock<cLocales>(MockBehavior.Strict);
            locales.Setup(l => l.getLocaleByID(It.IsAny<int>())).Returns<cLocale>(null);
            _actionContext.SetLocalesMock(locales);

            var countries = new Mock<cCountries>(MockBehavior.Strict, accountId, null, null);
            var country = new Mock<cCountry>(MockBehavior.Strict);
            country.SetupAllProperties();
            countries.Setup(c => c.getCountryById(It.IsAny<int>())).Returns(country.Object);
            _actionContext.SetCountriesMock(countries);

            var currencies = new Mock<cCurrencies>(MockBehavior.Strict, accountId, null);
            var currency = new Mock<cCurrency>(MockBehavior.Strict);
            currency.SetupAllProperties();
            currencies.Setup(c => c.getCurrencyById(It.IsAny<int>())).Returns(currency.Object);
            _actionContext.SetCurrenciesMock(currencies);

            var groups = new Mock<cGroups>(MockBehavior.Strict, accountId);
            var group = new Mock<cGroup>(MockBehavior.Strict);
            group.SetupAllProperties();
            groups.Setup(g => g.GetGroupById(It.IsAny<int>())).Returns(group.Object);
            _actionContext.SetGroupsMock(groups);

            Mock<CardProviders> cardProviders = new Mock<CardProviders>(MockBehavior.Strict, null);
            Mock<cCardProvider> cardProvider = new Mock<cCardProvider>(MockBehavior.Strict);
            cardProvider.SetupAllProperties();
            cardProviders.Setup(c => c.getProviderByID(It.IsAny<int>())).Returns(cardProvider.Object);
            _actionContext.SetCardProvidersMock(cardProviders);

            var employee = new Mock<SpendManagementLibrary.Employees.Employee>(MockBehavior.Strict);
            employee.SetupAllProperties();

            var employees = new Mock<cEmployees>(GlobalTestVariables.AccountId);
            employees.Setup(emp => emp.GetEmployeeById(It.IsAny<int>(), null))
                     .Returns(employee.Object);
            employees.Setup(emp => emp.getEmployeeidByUsername(It.IsAny<int>(), It.IsAny<string>(), null))
                     .Returns(0);
            _actionContext.SetEmployeesMock(employees);

            _actionContext.SetMileageCategoriesMock(new Mock<cMileagecats>(accountId));

            controllerFactory = new ControllerFactory<Employee>(repository);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorRecordAlreadyExistsMessage)]
        public void EmployeeController_Post_ShouldReturnErrorWhenIdNotZero()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.Id = 10;
            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            //Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            //Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            //Assert.AreEqual(response.ResponseInformation.Errors[0].Message, ApiResources.ApiErrorRecordAlreadyExistsMessage);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.ApiErrorInvalidNameMessage)]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidUserName()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.UserName = null;
            controllerFactory.Post<EmployeeResponse>(employee);
            //Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            //Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            //Assert.AreEqual(response.ResponseInformation.Errors[0].Message, ApiResources.ApiErrorInvalidNameMessage);
        }

        //Test no longer valid
        [TestMethod]
        [TestCategory("Validation")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidAddressLocationId()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.WorkAddresses[0].Id = 5;

            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "Valid AddressLocationId must be provided");
        }

        //Test no longer valid
        [TestMethod]
        [TestCategory("Validation")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenMissingAddressLocationId()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.WorkAddresses[0].Id = -1;
            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "AddressLocationId must be provided");
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.ApiErrorCostCentrePercentageAllocated)]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidCostCentreId()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].CostCodeId = 0;
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].DepartmentId = 0;
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].ProjectCodeId = 0;
            controllerFactory.Post<EmployeeResponse>(employee);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.ApiErrorCostCentrePercentageOutOfBounds)]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidCostCentrePercentage()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].Percentage = 200;
            controllerFactory.Post<EmployeeResponse>(employee);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.ApiErrorGender)]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidGender()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns = null;
            employee.EmployeeDetails.PersonalDetails.BasicInfo.Gender = "Unknown";
            controllerFactory.Post<EmployeeResponse>(employee);
        }

        //Telephone number no longer mandatory 
        [TestMethod]
        [TestCategory("Validation")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidTelNumber()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.PersonalDetails.HomeContactDetails.TelephoneNumber = null;
            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "Valid TelephoneNumber must be provided");
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(InvalidDataException), "Valid LocaleId must be provided")]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidLocaleId()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns = null;
            employee.EmployeeDetails.NhsDetails = null;
            employee.EmployeeDetails.PersonalDetails.BasicInfo.LocaleId = 100;
            controllerFactory.Post<EmployeeResponse>(employee);
        }

        //Payroll number no longer mandatory
        [TestMethod]
        [TestCategory("Validation")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidWorkDetailPayRollNumber()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.PayRollNumber = null;
            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "Payroll number must be provided");
        }

        //Position no longer mandatory
        [TestMethod]
        [TestCategory("Validation")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidWorkDetailPosition()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.Position = null;
            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "Position must be provided");
        }

        //National insurance format validation no longer required
        [Ignore]
        [TestMethod]
        [TestCategory("Validation")]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidWorkDetailNationalInsuranceNumber()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.NationalInsuranceNumber = "123";
            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "Valid NationalInsuranceNumber must be provided");
        }

        //National Insurance Number no longer mandatory
        [Ignore]
        [TestMethod]
        [TestCategory("Validation")]
        public void EmployeeController_Post_ShouldReturnErrorWhenMissingWorkDetailNationalInsuranceNumber()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.NationalInsuranceNumber = null;
            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "NationalInsuranceNumber must be provided");
        }

        //Employee number no longer mandatory
        [TestMethod]
        [TestCategory("Validation")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenMissingWorkDetailEmployeeNumber()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.EmployeeNumber = null;
            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "EmployeeNumber must be provided");
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.ApiErrorCostCentreArchivedDepartment + "1")]
        public void EmployeeController_Post_ShouldReturnErrorWhenArchivedDepartmentInCostCentreBreakdown()
        {
            Mock<cDepartments> departmentsMock = new Mock<cDepartments>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            departmentsMock.Setup(d => d.GetDepartmentById(It.IsAny<int>())).Returns(new cDepartment(1, "Test", "Test", true, DateTime.UtcNow, GlobalTestVariables.EmployeeId, null, null, null));
            _actionContext.SetDepartmentsMock(departmentsMock);

            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].CostCodeId = null;
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].DepartmentId = 1;
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].ProjectCodeId = null;
            employee.UserDefinedFields = null;

            controllerFactory.Post<EmployeeResponse>(employee);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.ApiErrorNhsTrustNonExistent + "1")]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidTrustId()
        {
            Mock<cESRTrusts> trustsMock = new Mock<cESRTrusts>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            trustsMock.Setup(t => t.GetESRTrustByID(It.IsAny<int>())).Returns<cESRTrust>(null);
            _actionContext.SetNhsTrustsMock(trustsMock);

            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();

            employee.EmployeeDetails.NhsDetails.TrustId = null;

            controllerFactory.Post<EmployeeResponse>(employee);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.ApiErrorLocaleInvalid + "1")]
        public void EmployeeController_Post_ShouldReturnErrorWhenNonExistentLocaleId()
        {
            Mock<cESRTrusts> trustsMock = new Mock<cESRTrusts>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            trustsMock.Setup(t => t.GetESRTrustByID(It.IsAny<int>())).Returns<cESRTrust>(null);
            _actionContext.SetNhsTrustsMock(trustsMock);

            Mock<cDepartments> departmentsMock = new Mock<cDepartments>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            departmentsMock.Setup(d => d.GetDepartmentById(It.IsAny<int>())).Returns(new cDepartment(1, "Test", "Test", false, DateTime.UtcNow, GlobalTestVariables.EmployeeId, null, null, null));
            _actionContext.SetDepartmentsMock(departmentsMock);

            Mock<cLocales> localesMock = new Mock<cLocales>(MockBehavior.Strict);
            localesMock.Setup(d => d.getLocaleByID(It.IsAny<int>())).Returns<cLocale>(null);
            _actionContext.SetLocalesMock(localesMock);

            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.PersonalDetails.BasicInfo.LocaleId = 1;
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].CostCodeId = null;
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].DepartmentId = 1;
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].ProjectCodeId = null;
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].Percentage = 100;

            employee.EmployeeDetails.NhsDetails.TrustId = 1;

            controllerFactory.Post<EmployeeResponse>(employee);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.ApiErrorCostCentrePercentageMustAddUp)]
        public void EmployeeController_Post_ShouldReturnErrorWhenCostCentreBreakdownPercentageNot100()
        {
            Mock<cDepartments> departmentsMock = new Mock<cDepartments>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            departmentsMock.Setup(d => d.GetDepartmentById(It.IsAny<int>())).Returns(new cDepartment(1, "Test", "Test", false, DateTime.UtcNow, GlobalTestVariables.EmployeeId, null, null, null));
            _actionContext.SetDepartmentsMock(departmentsMock);

            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].CostCodeId = null;
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].DepartmentId = 1;
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].ProjectCodeId = null;
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns[0].Percentage = 1;

            controllerFactory.Post<EmployeeResponse>(employee);
        }

        //Line Manager no longer mandatory
        [TestMethod]
        [TestCategory("Validation")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenMissingWorkDetailLineManager()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.LineManagerUserId = -1;
            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "LineManagerId must be provided");
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.ApiErrorEmployeeLineManager)]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidWorkDetailLineManager()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();

            var employees = new Mock<cEmployees>(GlobalTestVariables.AccountId);
            employees.Setup(emp => emp.GetEmployeeById(It.IsAny<int>(), It.IsAny<IDBConnection>()))
                     .Returns<SpendManagementLibrary.Employees.Employee>(null);
            _actionContext.SetEmployeesMock(employees);

            controllerFactory.Post<EmployeeResponse>(employee);
        }

        //Hire date no longer mandatory
        [TestMethod]
        [TestCategory("Validation")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenWorkDetailsInvalidHireDate()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.HireDate = null;
            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "Hire Date must be provided");
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.ApiErrorEmployeeHireDateAfterMileageDate)]
        public void EmployeeController_Post_ShouldReturnErrorWhenWorkDetailsStartMileageDateBeforeHireDate()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.StartMileageDate = new DateTime(2014, 3, 3);
            employee.EmployeeDetails.WorkDetails.HireDate = new DateTime(2014, 4, 4);
            controllerFactory.Post<EmployeeResponse>(employee);
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(InvalidDataException), ApiResources.ApiErrorEmployeeTerminationBeforeHire)]
        public void EmployeeController_Post_ShouldReturnErrorWhenWorkDetailsTerminationDateBeforeStartDate()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.TerminationDate = new DateTime(2012, 2, 2);
            employee.EmployeeDetails.WorkDetails.HireDate = new DateTime(2012, 3, 3);
            controllerFactory.Post<EmployeeResponse>(employee);
        }

        //Validation of sign off groups has been removed from the code. Was there a reason for it being taken out?
        [TestMethod]
        [TestCategory("Validation")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenMissingClaimSignOffGroupId()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.ClaimSignOffDetails.SignOffGroupId = 0;

            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "SignOffGroupId must be provided");
        }

        //Validation of sign off groups has been removed from the code. Was there a reason for it being taken out?
        [TestMethod]
        [TestCategory("Validation")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidClaimSignOffGroupId()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.ClaimSignOffDetails.SignOffGroupId = 10;

            var groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            groups.Setup(g => g.GetGroupById(It.IsAny<int>())).Returns<cGroup>(null);
            _actionContext.SetGroupsMock(groups);

            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "Valid SignOffGroupId must be provided");
        }

        //Advances Sign off group no longer mandatory
        [TestMethod]
        [TestCategory("Validation")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenMissingAdvancesSignOffGroupId()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.ClaimSignOffDetails = new ClaimSignOffDetails
                                                                      {
                                                                          AdvancesSignOffGroupId = 0,
                                                                          CreditCardSignOffGroupId =
                                                                              10,
                                                                          PurchaseCardSignOffGroupId =
                                                                              10,
                                                                          SignOffGroupId = 10
                                                                      };

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cGroup> group = new Mock<cGroup>(MockBehavior.Strict);
            groups.Setup(g => g.GetGroupById(10)).Returns(group.Object);
            _actionContext.SetGroupsMock(groups);

            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "AdvancesSignOffGroupId must be provided");
        }

        //Validation of sign off groups has been removed from the code. Was there a reason for it being taken out?
        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), "Valid AdvancesSignOffGroupId must be provided")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidAdvancesSignOffGroupId()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns = null;
            employee.EmployeeDetails.PersonalDetails.BasicInfo = null;
            employee.EmployeeDetails.NhsDetails = null;
            employee.EmployeeDetails.ClaimSignOffDetails = new ClaimSignOffDetails
            {
                AdvancesSignOffGroupId = 1,
                CreditCardSignOffGroupId =
                    10,
                PurchaseCardSignOffGroupId =
                    10,
                SignOffGroupId = 10
            };

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cGroup> group = new Mock<cGroup>(MockBehavior.Strict);
            groups.Setup(g => g.GetGroupById(10)).Returns(group.Object);
            groups.Setup(g => g.GetGroupById(1)).Returns<cGroup>(null);
            _actionContext.SetGroupsMock(groups);

            controllerFactory.Post<EmployeeResponse>(employee);
            //Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            //Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            //Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "Valid AdvancesSignOffGroupId must be provided");
        }

        //Credit card sign off group no longer mandatory
        [TestMethod]
        [TestCategory("Validation")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenMissingCreditCardSignOffGroupId()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.ClaimSignOffDetails = new ClaimSignOffDetails
            {
                AdvancesSignOffGroupId = 10,
                CreditCardSignOffGroupId =
                    0,
                PurchaseCardSignOffGroupId =
                    10,
                SignOffGroupId = 10
            };

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cGroup> group = new Mock<cGroup>(MockBehavior.Strict);
            groups.Setup(g => g.GetGroupById(10)).Returns(group.Object);
            _actionContext.SetGroupsMock(groups);

            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "CreditCardSignOffGroupId must be provided");
        }

        //Validation of sign off groups has been removed from the code. Was there a reason for it being taken out?
        [TestMethod]
        [TestCategory("Validation")]
        [Ignore]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidCreditCardSignOffGroupId()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.EmployeeDetails.ClaimSignOffDetails = new ClaimSignOffDetails
            {
                AdvancesSignOffGroupId = 10,
                CreditCardSignOffGroupId =
                    1,
                PurchaseCardSignOffGroupId =
                    10,
                SignOffGroupId = 10
            };

            Mock<cGroups> groups = new Mock<cGroups>(MockBehavior.Strict, GlobalTestVariables.AccountId);
            Mock<cGroup> group = new Mock<cGroup>(MockBehavior.Strict);
            groups.Setup(g => g.GetGroupById(10)).Returns(group.Object);
            groups.Setup(g => g.GetGroupById(1)).Returns<cGroup>(null);
            _actionContext.SetGroupsMock(groups);

            EmployeeResponse response = controllerFactory.Post<EmployeeResponse>(employee);
            Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Failure);
            Assert.AreEqual(response.ResponseInformation.Errors.Count, 1);
            Assert.AreEqual(response.ResponseInformation.Errors[0].Message, "Valid CreditCardSignOffGroupId must be provided");
        }

        [TestMethod]
        [TestCategory("Validation")]
        [ExpectedException(typeof(ApiException), ApiResources.InvalidUdfMessage)]
        public void EmployeeController_Post_ShouldReturnErrorWhenInvalidUdfId()
        {
            Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
            employee.UserDefinedFields = new List<UserDefinedFieldValue> { new UserDefinedFieldValue(-1, -1) };
            employee.EmployeeDetails.PersonalDetails.BasicInfo = null;
            employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns = null;
            employee.EmployeeDetails.NhsDetails = null;

            controllerFactory.Post<EmployeeResponse>(employee);
        }

        [TestMethod]
        [TestCategory("EndToEnd")]
        public void EmployeeController_Post_ShouldReturnValidResponseWithValidRequest()
        {
            EmployeeResponse response = null;
            try
            {
                this.SetupValidEmployeeDependencies();

                var currentUser = new Mock<ICurrentUser>();
                int accountId = GlobalTestVariables.AccountId;
                currentUser.SetupGet(u => u.AccountID).Returns(accountId);
                currentUser.SetupGet(u => u.EmployeeID).Returns(GlobalTestVariables.EmployeeId);
                currentUser.SetupGet(u => u.CurrentSubAccountId).Returns(GlobalTestVariables.SubAccountId);
                
                var repository = RepositoryFactory.GetRepository<Employee>(new object[] { currentUser.Object, null });
                controllerFactory = new ControllerFactory<Employee>(repository);
                cImportTemplateObject.CreateImportTemplate();
                Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
                employee.IsActive = false;
                employee.Archived = true;
                cDepartment department = _actionContext.Departments.InitialiseData().First(d => !d.Value.Archived).Value;
                employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns = new List<CostCentreBreakdown>
                                                                                {
                                                                                    new CostCentreBreakdown
                                                                                        {
                                                                                            DepartmentId
                                                                                                =
                                                                                                department
                                                                                                .DepartmentId,
                                                                                            Percentage
                                                                                                =
                                                                                                100
                                                                                        }
                                                                                };
                response = controllerFactory.Post<EmployeeResponse>(employee);

                Assert.AreEqual(1, (controllerFactory.Controller as EmployeesV1Controller).TransactionCount);

                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.AreEqual(response.ResponseInformation.Errors.Count, 0);
                Assert.IsTrue(response.Item.EmployeeId > 0);
            }
            finally
            {
                if (response.Item != null)
                {
                    //Archive employee record before deletion
                    response.Item.Archived = true;
                    response = controllerFactory.Put<EmployeeResponse>(response.Item);

                    //Delete new record and it's dependencies
                    controllerFactory.Delete<EmployeeResponse>((response.Item).Id);
                }
                cImportTemplateObject.DeleteImportTemplate();

                this.DeleteValidEmployeeDependencies();
            }
        }


        [TestMethod]
        [TestCategory("EndToEnd")]
        public void EmployeeController_Put_ShouldReturnValidResponseWithValidRequest()
        {
            EmployeeResponse response = null;
            try
            {
                this.SetupValidEmployeeDependencies();

                var currentUser = new Mock<ICurrentUser>();
                int accountId = GlobalTestVariables.AccountId;
                currentUser.SetupGet(u => u.AccountID).Returns(accountId);
                currentUser.SetupGet(u => u.EmployeeID).Returns(GlobalTestVariables.EmployeeId);
                currentUser.SetupGet(u => u.CurrentSubAccountId).Returns(GlobalTestVariables.SubAccountId);

                var repository = RepositoryFactory.GetRepository<Employee>(new object[] { currentUser.Object, null });
                controllerFactory = new ControllerFactory<Employee>(repository);
                cImportTemplateObject.CreateImportTemplate();
                Employee employee = RequestStubCreator<Employee>.GetValidEmployee();
                employee.IsActive = false;
                employee.Archived = true;
                cDepartment department = _actionContext.Departments.InitialiseData().First(d => !d.Value.Archived).Value;
           
                employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns = new List<CostCentreBreakdown>
                                                                                {
                                                                                    new CostCentreBreakdown
                                                                                        {
                                                                                            DepartmentId
                                                                                                =
                                                                                                department
                                                                                                .DepartmentId,
                                                                                            Percentage
                                                                                                =
                                                                                                100
                                                                                        }
                                                                                };

                //Save initial request
                response = controllerFactory.Post<EmployeeResponse>(employee);
                var originalRequest = controllerFactory.Get<EmployeeResponse>(employee.Id);
                var originalEmployee = originalRequest.Item;
                employee.Surname = "UpdatedSurname";
                response = controllerFactory.Put<EmployeeResponse>(employee);

                Assert.AreEqual(1, (controllerFactory.Controller as EmployeesV1Controller).TransactionCount);
                Assert.AreEqual(response.ResponseInformation.Status, ApiStatusCode.Success);
                Assert.AreEqual(response.ResponseInformation.Errors.Count, 0);
                Assert.AreNotEqual(response.Item.Surname, originalEmployee.Surname);
            }
            finally
            {
                if (response.Item != null)
                {
                    //Archive employee record before deletion
                    response.Item.Archived = true;
                    response = controllerFactory.Put<EmployeeResponse>(response.Item);

                    //Delete new record and it's dependencies
                    controllerFactory.Delete<EmployeeResponse>((response.Item).Id);
                }
                cImportTemplateObject.DeleteImportTemplate();

                this.DeleteValidEmployeeDependencies();
            }
        }

        private void SetupValidEmployeeDependencies()
        {
            int accountId = GlobalTestVariables.AccountId;
            cAccessRoles accessRoles = new cAccessRoles(accountId, cAccounts.getConnectionString(accountId));
            _testAccessRoleId = accessRoles.SaveAccessRole(
                GlobalTestVariables.EmployeeId,
                0,
                "TestAccessRole",
                string.Empty,
                (int)AccessRoleLevel.EmployeesResponsibleFor,
                new object[,] { { null }, { null } },
                null,
                null,
                false,
                false,
                false,
                false,
                new object[0],
                null,
                null,
                true,
                true,
                true);
            if (_testAccessRoleId == -1)
            {
                _testAccessRoleId = accessRoles.GetAccessRoleByName("TestAccessRole").RoleID;
            }

        }

        private void DeleteValidEmployeeDependencies()
        {
            int accountId = GlobalTestVariables.AccountId;
            cAccessRoles accessRoles = new cAccessRoles(accountId, cAccounts.getConnectionString(accountId));
            accessRoles.DeleteAccessRole(_testAccessRoleId, GlobalTestVariables.EmployeeId, null);
        }

    }
}
