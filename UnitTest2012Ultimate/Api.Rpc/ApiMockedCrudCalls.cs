namespace UnitTest2012Ultimate.Api.Rpc
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Runtime.Caching;
    using System.Web.UI.MobileControls;

    using EsrGo2FromNhsWcfLibrary;
    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Crud;
    using EsrGo2FromNhsWcfLibrary.Enum;
    using EsrGo2FromNhsWcfLibrary.ESR;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Action = EsrGo2FromNhsWcfLibrary.Base.Action;
    using DataAccessMethod = EsrGo2FromNhsWcfLibrary.Enum.DataAccessMethod;

    /// <summary>
    /// The API mocked CRUD calls.
    /// </summary>
    [TestClass]
    public class ApiMockedCrudCalls
    {
        const long supervisorId = 99999;

        /// <summary>
        /// The test context instance.
        /// </summary>
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }

            set
            {
                this.testContextInstance = value;
            }
        }

        #region Additional test attributes

        /// <summary>
        /// The my class initialize.
        /// </summary>
        /// <param name="testContext">
        /// The test context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            cESRTrustObject.DeleteTrust();
            GlobalAsax.Application_End();
        }

        #endregion

        #region Tests

        /// <summary>
        /// API RPC get all persons Mock
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcGetAllPersonsMock()
        {
            var person = new EsrPerson
                               {
                                   ESRPersonId = 1,
                                   EmployeeNumber = "2",
                                   ActionResult = new ApiResult { Result = ApiActionResult.Success }
                               };
            var assignment = new EsrAssignment { ESRPersonId = 1, AssignmentID = 123 };
            var phone = new EsrPhone { ESRPersonId = 1, ESRPhoneId = 123 };
            var esrAddress = new EsrAddress { ESRPersonId = 1, ESRAddressId = 123 };
            var esrVehicle = new EsrVehicle { ESRPersonId = 1, ESRVehicleAllocationId = 123 };

            var apiCrudMock = new Mock<IDataAccess<EsrPerson>>();
            apiCrudMock.SetupAllProperties();
            apiCrudMock.Setup(x => x.ReadAll()).Returns(new List<EsrPerson> { person });

            
            var apiAssignmentMock = new Mock<IDataAccess<EsrAssignment>>();
            apiAssignmentMock.SetupAllProperties();
            apiAssignmentMock.Setup(x => x.ReadByEsrId(It.IsAny<int>()))
                             .Returns(
                                 new List<EsrAssignment> { assignment });

            var apiPhoneMock = new Mock<IDataAccess<EsrPhone>>();
            apiPhoneMock.SetupAllProperties();
            apiPhoneMock.Setup(x => x.ReadByEsrId(It.IsAny<int>()))
                        .Returns(new List<EsrPhone> { phone });

            var apiAddressMock = new Mock<IDataAccess<EsrAddress>>();
            apiAddressMock.SetupAllProperties();
            apiAddressMock.Setup(x => x.ReadByEsrId(It.IsAny<int>()))
                          .Returns(new List<EsrAddress> { esrAddress });

            var apiVehicleMock = new Mock<IDataAccess<EsrVehicle>>();
            apiVehicleMock.SetupAllProperties();
            apiVehicleMock.Setup(x => x.ReadByEsrId(It.IsAny<int>()))
                          .Returns(
                              new List<EsrVehicle> { esrVehicle });

            var esrApiMock = new Mock<IEsrApi>();
            esrApiMock.SetupAllProperties();
            esrApiMock.Setup(x => x.Execute(DataAccessMethod.ReadAll, "", It.IsAny<List<EsrPerson>>(), It.IsAny<DataAccessMethodReturnDefault>())).Returns(new List<EsrPerson> { person });

            esrApiMock.Setup(x => x.Execute<EsrAssignment>(DataAccessMethod.ReadByEsrId, It.IsAny<string>(), null, DataAccessMethodReturnDefault.NewObjectWithFailureFlag)).Returns(new List<EsrAssignment> { assignment });
            esrApiMock.Setup(x => x.Execute<EsrAddress>(DataAccessMethod.ReadByEsrId, It.IsAny<string>(), null, DataAccessMethodReturnDefault.NewObject)).Returns(new List<EsrAddress> { esrAddress });
            esrApiMock.Setup(x => x.Execute<EsrPhone>(DataAccessMethod.ReadByEsrId, It.IsAny<string>(), null, DataAccessMethodReturnDefault.NewObject)).Returns(new List<EsrPhone> { phone });
            esrApiMock.Setup(x => x.Execute<EsrVehicle>(DataAccessMethod.ReadByEsrId, It.IsAny<string>(), null, DataAccessMethodReturnDefault.NewObject)).Returns(new List<EsrVehicle> { esrVehicle });
            esrApiMock.Setup(x => x.Execute<EsrAssignmentCostings>(It.IsAny<string>(), DataAccessMethod.Read, DataAccessMethodReturnDefault.NewObject)).Returns(new EsrAssignmentCostings { Action = Action.Read, ESRAssignmentId = assignment.AssignmentID});

            var apiRpiSvc = new ApiRpc(GetMockDataLog(), esrApiMock.Object);
            var result = apiRpiSvc.GetAllPersons(GlobalTestVariables.AccountId);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success);
        }

        /// <summary>
        /// API RPC Save Location Mock
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcSaveLocationMock()
        {
            var esrLocation = new EsrLocation
                              {
                                  ESRLocationId = 1,
                                  AddressLine1 = "1",
                                  ActionResult = new ApiResult { Result = ApiActionResult.Success }
                              };

            var apiCrudMock = new Mock<IDataAccess<EsrLocation>>();
            apiCrudMock.SetupAllProperties();
            apiCrudMock.Setup(x => x.Create(It.IsAny<List<EsrLocation>>())).Returns(new List<EsrLocation>{ esrLocation });
            var esrLocations = new List<EsrLocationRecord>
                                  {
                                      new EsrLocationRecord()
                                          {
                                              Action = Action.Create,
                                              ESRLocationId = 1,
                                              AddressLine1 = "1",
                                              ActionResult =
                                                  new ApiResult
                                                      {
                                                          Result =
                                                              ApiActionResult
                                                              .NoAction
                                                      }
                                          }
                                  };

            var esrApiMock = new Mock<IEsrApi>();
            esrApiMock.SetupAllProperties();

            var actionResultSuccess = new ApiResult { LookupUpdateFailure = false, Result = ApiActionResult.Success, Message = string.Empty };
            var actionResultDeleted = new ApiResult { LookupUpdateFailure = false, Result = ApiActionResult.Deleted, Message = string.Empty };

            esrApiMock.Setup(x => x.Execute<Address>(DataAccessMethod.ReadByEsrId, "1", null, DataAccessMethodReturnDefault.Null))
                        .Returns(new List<Address> { new Address { Action = Action.Read, ActionResult = actionResultSuccess, AddressID = 1 } });

            esrApiMock.Setup(
                x => x.Execute<Address>("1", DataAccessMethod.Delete, DataAccessMethodReturnDefault.NewObject))
                .Returns(new Address { Action = Action.Delete, ActionResult = actionResultSuccess, AddressID = 1 });

            esrApiMock.Setup(x => x.Execute<EsrLocation>("1", DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null))
                .Returns(new EsrLocation { Action = Action.Delete, ActionResult = actionResultDeleted, AddressLine1 = "1", ESRLocationId = 1 });

            esrApiMock.Setup(x => x.Execute(DataAccessMethod.Create, It.IsAny<string>(), It.IsAny<List<EsrLocation>>(), It.IsAny<DataAccessMethodReturnDefault>()))
                .Returns(new List<EsrLocation> { esrLocation });

            var apiRpiSvc = new ApiRpc(GetMockDataLog(), esrApiMock.Object);
            var result = apiRpiSvc.UpdateEsrLocationRecords(GlobalTestVariables.AccountId, "371", esrLocations);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success);
        }

        /// <summary>
        /// API RPC Save Location Mock
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcSaveOrganisationMock()
        {
            var esrOrganisation = new EsrOrganisation
                                  {
                                      Action = Action.Create,
                                      ESRLocationId = 1111,
                                      CostCentreDescription = "desc",
                                      ActionResult =
                                          new ApiResult { Result = ApiActionResult.Success }
                                  };
            var apiCrudMock = new Mock<IDataAccess<EsrOrganisation>>();
            apiCrudMock.SetupAllProperties();
            apiCrudMock.Setup(x => x.Create(It.IsAny<List<EsrOrganisation>>())).Returns(new List<EsrOrganisation>{ esrOrganisation });
            var esrOrganisations = new List<EsrOrganisationRecord>
                                      {
                                          new EsrOrganisationRecord
                                              {
                                                  Action = Action.Create,
                                                  ESRLocationId = 1111,
                                                  CostCentreDescription
                                                      = "desc",
                                                  EffectiveFrom =
                                                      DateTime.Now,
                                                  ActionResult =
                                                      new ApiResult
                                                          {
                                                              Result
                                                                  =
                                                                  ApiActionResult
                                                                  .Success
                                                          }
                                              }
                                      };

            var esrApiMock = new Mock<IEsrApi>();
            esrApiMock.SetupAllProperties();

            var actionResultSuccess = new ApiResult { LookupUpdateFailure = false, Result = ApiActionResult.Success, Message = string.Empty };
            var actionResultDeleted = new ApiResult { LookupUpdateFailure = false, Result = ApiActionResult.Deleted, Message = string.Empty };

            esrApiMock.Setup(x => x.Execute<Address>(DataAccessMethod.ReadByEsrId, "1", null, DataAccessMethodReturnDefault.Null))
                        .Returns(new List<Address> { new Address { Action = Action.Read, ActionResult = actionResultSuccess, AddressID = 1 } });

            esrApiMock.Setup(
                x => x.Execute<Address>("1", DataAccessMethod.Delete, DataAccessMethodReturnDefault.NewObject))
                .Returns(new Address { Action = Action.Delete, ActionResult = actionResultSuccess, AddressID = 1 });

            esrApiMock.Setup(x => x.Execute<EsrLocation>("1", DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null))
                .Returns(new EsrLocation { Action = Action.Delete, ActionResult = actionResultDeleted, AddressLine1 = "1", ESRLocationId = 1 });

            esrApiMock.Setup(x => x.Execute(DataAccessMethod.Create, It.IsAny<string>(), It.IsAny<List<EsrOrganisation>>(), It.IsAny<DataAccessMethodReturnDefault>()))
                .Returns(new List<EsrOrganisation> { esrOrganisation });

            var apiRpiSvc = new ApiRpc(GetMockDataLog(), esrApiMock.Object);
            var result = apiRpiSvc.UpdateEsrOrganisations(GlobalTestVariables.AccountId, "371", esrOrganisations);

            Assert.IsTrue(
                result[0].ActionResult.Result == ApiActionResult.Success,
                string.Format(
                    "Was expecting {0} but returned {1} - {2}",
                    ApiActionResult.Success,
                    result[0].ActionResult.Result,
                    result[0].ActionResult.Message));
        }

        /// <summary>
        /// API RPC Save Position Mock
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcSavePositionMock()
        {
            var apiCrudMock = new Mock<IDataAccess<EsrPosition>>();
            apiCrudMock.SetupAllProperties();
            apiCrudMock.Setup(x => x.Create(It.IsAny<List<EsrPosition>>()))
                       .Returns(
                           new List<EsrPosition>
                               {
                                   new EsrPosition
                                       {
                                           ESRPositionId = 1,
                                           PositionName = "Position",
                                           ESRLastUpdateDate = new DateTime(2012, 12, 12),
                                           Action = Action.Create,
                                           ActionResult =
                                               new ApiResult
                                                   {
                                                       Result =
                                                           ApiActionResult.Success
                                                   }
                                       }
                               });
            var esrPosition = new List<EsrPositionRecord>
                                  {
                                      new EsrPositionRecord
                                          {
                                              ESRPositionId = 1,
                                              PositionName = "Position",
                                              ESRLastUpdateDate =
                                                  new DateTime(2012, 12, 12),
                                              Action = Action.Create,
                                              ActionResult =
                                                  new ApiResult
                                                      {
                                                          Result =
                                                              ApiActionResult
                                                              .NoAction
                                                      }
                                          }
                                  };

            var apiRpiSvc = new ApiRpc(GetMockDataLog());
            var result = apiRpiSvc.UpdateEsrPositions(GlobalTestVariables.AccountId, "371", esrPosition);

            Assert.IsTrue(
                result[0].ActionResult.Result == ApiActionResult.Success,
                string.Format("{0}", result[0].ActionResult.Message));
        }

        /// <summary>
        /// API RPC Delete Location Mock
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcDeleteLocationMock()
        {
            var esrLocation = new EsrLocationRecord
                                  {
                                      ESRLocationId = 1,
                                      AddressLine1 = "1",
                                      ActionResult = new ApiResult { Result = ApiActionResult.NoAction }
                                  };


            var esrApiMock = new Mock<IEsrApi>();
            esrApiMock.SetupAllProperties();

            var actionResultSuccess = new ApiResult { LookupUpdateFailure = false, Result = ApiActionResult.Success, Message = string.Empty };
            var actionResultDeleted = new ApiResult { LookupUpdateFailure = false, Result = ApiActionResult.Deleted, Message = string.Empty };

            esrApiMock.Setup(x => x.Execute<Address>(DataAccessMethod.ReadByEsrId, "1", null, DataAccessMethodReturnDefault.Null))
                        .Returns(new List<Address> { new Address { Action = Action.Read, ActionResult = actionResultSuccess, AddressID = 1 } });

            esrApiMock.Setup(
                x => x.Execute<Address>("1", DataAccessMethod.Delete, DataAccessMethodReturnDefault.NewObject))
                .Returns(new Address { Action = Action.Delete, ActionResult = actionResultSuccess, AddressID = 1 });

            esrApiMock.Setup(x => x.Execute<EsrLocation>("1", DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null))
                .Returns(new EsrLocation { Action = Action.Delete, ActionResult = actionResultDeleted, AddressLine1 = "1", ESRLocationId = 1 });

            esrApiMock.Setup(x => x.Execute(DataAccessMethod.Create, "", It.IsAny<List<EsrLocation>>(), It.IsAny<DataAccessMethodReturnDefault>()))
                .Returns(new List<EsrLocation>());

            var apiRpiSvc = new ApiRpc(GetMockDataLog(), esrApiMock.Object);
            esrLocation.Action = Action.Delete;
            var result = apiRpiSvc.UpdateEsrLocationRecords(GlobalTestVariables.AccountId, "371", new List<EsrLocationRecord> { esrLocation });

            Assert.IsTrue(result.Count == 1, "Records returned should equal 1");
            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Deleted);
        }

        /// <summary>
        /// The API RPC update car journey rates.
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcCarJourneyRateSingleRateFound()
        {
            var journeyRates = this.GetVJR();
            var car1 = new Car { carid = 100, employeeid = 10, make = "Ford", registration = "123" };

            var vehicle1 = new EsrVehicle { ESRVehicleAllocationId = 1000, Make = "Ford", RegistrationNumber = "123", EngineCC = 1500, UserRatesTable = "NHS1"};
            var vehicle2 = new EsrVehicle { ESRVehicleAllocationId = 1001, Make = "Kia", RegistrationNumber = "124", EngineCC = 1400, UserRatesTable = "NHS2" };

            var allocation1 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1000, CarId = 100 };
            
            var vehicles = new List<EsrVehicle> { vehicle1, vehicle2 };

            var allocations = new List<CarAssignmentNumberAllocation> { allocation1 };

            var carJourney = new CarVehicleJourneyRateCrud("expenses", GlobalTestVariables.AccountId);

            var testcarJourneyCrud = new PrivateObject(carJourney);
            var currentVehicles = testcarJourneyCrud.Invoke("GetVehiclesForCurrentCar", new object[] { car1, vehicles, allocations }) as List<EsrVehicle>;
            Assert.IsTrue(currentVehicles != null, "currentVehicles != null");
            Assert.IsTrue(currentVehicles.Count == 1, "Curent Vehicles.Count != 1");

            var rates = new List<CarVehicleJourneyRate>();
            var message = testcarJourneyCrud.Invoke(
                "FindNewVehicleJourneyRates", new object[] { 123, 100, currentVehicles, journeyRates, rates }) as string;
            Assert.IsTrue(message != null);
            Debug.WriteLine(message);
            Assert.IsTrue(message == string.Empty);
            Assert.IsTrue(rates.Count == 1, "Found rates should = 1");
            Assert.IsTrue(rates[0].MileageId == 1, "Mileage id should be 1");
        }

        /// <summary>
        /// The API RPC update car journey rates.
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcCarJourneyRateSingleRateNotFound()
        {
            var journeyRates = this.GetVJR();
            var car1 = new Car { carid = 100, employeeid = 10, make = "Ford", registration = "123" };

            var vehicle1 = new EsrVehicle { ESRVehicleAllocationId = 1000, Make = "Ford", RegistrationNumber = "123", EngineCC = 1500, UserRatesTable = "NHS1NotFound" };
            var vehicle2 = new EsrVehicle { ESRVehicleAllocationId = 1001, Make = "Kia", RegistrationNumber = "124", EngineCC = 1400, UserRatesTable = "NHS2NotFound" };

            var allocation1 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1000, CarId = 100 };

            var vehicles = new List<EsrVehicle> { vehicle1, vehicle2 };

            var allocations = new List<CarAssignmentNumberAllocation> { allocation1 };

            var carJourney = new CarVehicleJourneyRateCrud("expenses", GlobalTestVariables.AccountId);

            var testcarJourneyCrud = new PrivateObject(carJourney);
            var currentVehicles = testcarJourneyCrud.Invoke("GetVehiclesForCurrentCar", new object[] { car1, vehicles, allocations }) as List<EsrVehicle>;
            Assert.IsTrue(currentVehicles != null, "currentVehicles != null");
            Assert.IsTrue(currentVehicles.Count == 1, "Curent Vehicles.Count != 1");

            var rates = new List<CarVehicleJourneyRate>();
            var message = testcarJourneyCrud.Invoke(
                "FindNewVehicleJourneyRates", new object[] { 123, 100, currentVehicles, journeyRates, rates }) as string;
            Assert.IsTrue(message != null);
            Debug.WriteLine(message);
            Assert.IsTrue(message != string.Empty);
            Assert.IsTrue(rates.Count == 0, "Found rates should = 1");
        }

        /// <summary>
        /// The API RPC update car journey rates.
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcCarJourneyRateSingleRateLowerCase()
        {
            var journeyRates = this.GetVJR();
            var car1 = new Car { carid = 100, employeeid = 10, make = "Ford", registration = "123" };

            var vehicle1 = new EsrVehicle { ESRVehicleAllocationId = 1000, Make = "Ford", RegistrationNumber = "123", EngineCC = 1500, UserRatesTable = "nhs1" };
            var vehicle2 = new EsrVehicle { ESRVehicleAllocationId = 1001, Make = "Kia", RegistrationNumber = "124", EngineCC = 1400, UserRatesTable = "nhs2" };

            var allocation1 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1000, CarId = 100 };

            var vehicles = new List<EsrVehicle> { vehicle1, vehicle2 };

            var allocations = new List<CarAssignmentNumberAllocation> { allocation1 };

            var carJourney = new CarVehicleJourneyRateCrud("expenses", GlobalTestVariables.AccountId);

            var testcarJourneyCrud = new PrivateObject(carJourney);
            var currentVehicles = testcarJourneyCrud.Invoke("GetVehiclesForCurrentCar", new object[] { car1, vehicles, allocations }) as List<EsrVehicle>;
            Assert.IsTrue(currentVehicles != null, "currentVehicles != null");
            Assert.IsTrue(currentVehicles.Count == 1, "Curent Vehicles.Count != 1");

            var rates = new List<CarVehicleJourneyRate>();
            var message = testcarJourneyCrud.Invoke(
                "FindNewVehicleJourneyRates", new object[] { 123, 100, currentVehicles, journeyRates, rates }) as string;
            Assert.IsTrue(message != null);
            Debug.WriteLine(message);
            Assert.IsTrue(message == string.Empty);
            Assert.IsTrue(rates.Count == 1, "Found rates should = 1");
            Assert.IsTrue(rates[0].MileageId == 1, "Mileage id should be 1");
        }

        /// <summary>
        /// The API RPC update car journey rates.
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcCarJourneyRateTwoRates()
        {
            var journeyRates = this.GetVJR();
            var car1 = new Car { carid = 100, employeeid = 10, make = "Ford", registration = "123" };

            var vehicle1 = new EsrVehicle { ESRVehicleAllocationId = 1000, Make = "Ford", RegistrationNumber = "123", EngineCC = 1500, UserRatesTable = "nhs1" };
            var vehicle2 = new EsrVehicle { ESRVehicleAllocationId = 1001, Make = "Kia", RegistrationNumber = "124", EngineCC = 1400, UserRatesTable = "nhs2" };

            var allocation1 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1000, CarId = 100 };
            var allocation2 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1001, CarId = 100 };

            var vehicles = new List<EsrVehicle> { vehicle1, vehicle2 };

            var allocations = new List<CarAssignmentNumberAllocation> { allocation1, allocation2 };

            var carJourney = new CarVehicleJourneyRateCrud("expenses", GlobalTestVariables.AccountId);

            var testcarJourneyCrud = new PrivateObject(carJourney);
            var currentVehicles = testcarJourneyCrud.Invoke("GetVehiclesForCurrentCar", new object[] { car1, vehicles, allocations }) as List<EsrVehicle>;
            Assert.IsTrue(currentVehicles != null, "currentVehicles != null");
            Assert.IsTrue(currentVehicles.Count == 2, "Curent Vehicles.Count != 1");

            var rates = new List<CarVehicleJourneyRate>();
            var message = testcarJourneyCrud.Invoke(
                "FindNewVehicleJourneyRates", new object[] { 123, 100, currentVehicles, journeyRates, rates }) as string;
            Assert.IsTrue(message != null);
            Debug.WriteLine(message);
            Assert.IsTrue(message == string.Empty);
            Assert.IsTrue(rates.Count == 2, "Found rates should = 2");
            Assert.IsTrue(rates[0].MileageId == 1, "Mileage id of item 1 should be 1");
            Assert.IsTrue(rates[1].MileageId == 2, "Mileage id of item 2 should be 2");
        }

        /// <summary>
        /// The API RPC update car journey rates.
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcCarJourneyRateMatchEngineSize()
        {
            var journeyRates = this.GetVJR();
            var car1 = new Car { carid = 100, employeeid = 10, make = "Ford", registration = "123" };
            var car2 = new Car { carid = 101, employeeid = 10, make = "Kia", registration = "124" };

            var vehicle1 = new EsrVehicle { ESRVehicleAllocationId = 1000, Make = "Ford", RegistrationNumber = "123", EngineCC = 1500, UserRatesTable = "nhs_basic" };
            var vehicle2 = new EsrVehicle { ESRVehicleAllocationId = 1001, Make = "Kia", RegistrationNumber = "124", EngineCC = 1400, UserRatesTable = "nhs_basic" };

            var allocation1 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1000, CarId = 100 };
            var allocation2 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1001, CarId = 101 };

            var vehicles = new List<EsrVehicle> { vehicle1, vehicle2 };

            var allocations = new List<CarAssignmentNumberAllocation> { allocation1, allocation2 };

            var carJourney = new CarVehicleJourneyRateCrud("expenses", GlobalTestVariables.AccountId);

            var testcarJourneyCrud = new PrivateObject(carJourney);
            var currentVehicles = testcarJourneyCrud.Invoke("GetVehiclesForCurrentCar", new object[] { car1, vehicles, allocations }) as List<EsrVehicle>;
            Assert.IsTrue(currentVehicles != null, "currentVehicles != null");
            Assert.IsTrue(currentVehicles.Count == 1, "Curent Vehicles.Count != 1");

            var rates = new List<CarVehicleJourneyRate>();
            var message = testcarJourneyCrud.Invoke(
                "FindNewVehicleJourneyRates", new object[] { 123, 100, currentVehicles, journeyRates, rates }) as string;
            Assert.IsTrue(message != null);
            Debug.WriteLine(message);
            Assert.IsTrue(message == string.Empty);
            Assert.IsTrue(rates.Count == 1, "Found rates should = 1");
            Assert.IsTrue(rates[0].MileageId == 4, "Mileage id of item 1 should be 4");

            currentVehicles = testcarJourneyCrud.Invoke("GetVehiclesForCurrentCar", new object[] { car2, vehicles, allocations }) as List<EsrVehicle>;
            Assert.IsTrue(currentVehicles != null, "currentVehicles != null");
            Assert.IsTrue(currentVehicles.Count == 1, "Curent Vehicles.Count != 1");

            rates = new List<CarVehicleJourneyRate>();
            message = testcarJourneyCrud.Invoke(
                "FindNewVehicleJourneyRates", new object[] { 123, 101, currentVehicles, journeyRates, rates }) as string;
            Assert.IsTrue(message != null);
            Debug.WriteLine(message);
            Assert.IsTrue(message == string.Empty);
            Assert.IsTrue(rates.Count == 1, "Found rates should = 1");
            Assert.IsTrue(rates[0].MileageId == 3, "Mileage id of item 1 should be 3");
        }

        /// <summary>
        /// The API RPC update car journey rates.
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcCarJourneyRateMatchEngineSizeFail()
        {
            var journeyRates = this.GetVJR();
            var car1 = new Car { carid = 100, employeeid = 10, make = "Ford", registration = "123" };
            var car2 = new Car { carid = 101, employeeid = 10, make = "Kia", registration = "124" };

            var vehicle1 = new EsrVehicle { ESRVehicleAllocationId = 1000, Make = "Ford", RegistrationNumber = "123", EngineCC = 7500, UserRatesTable = "nhs_basic" };
            var vehicle2 = new EsrVehicle { ESRVehicleAllocationId = 1001, Make = "Kia", RegistrationNumber = "124", EngineCC = 7400, UserRatesTable = "nhs_basic" };

            var allocation1 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1000, CarId = 100 };
            var allocation2 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1001, CarId = 101 };

            var vehicles = new List<EsrVehicle> { vehicle1, vehicle2 };

            var allocations = new List<CarAssignmentNumberAllocation> { allocation1, allocation2 };

            var carJourney = new CarVehicleJourneyRateCrud("expenses", GlobalTestVariables.AccountId);

            var testcarJourneyCrud = new PrivateObject(carJourney);
            var currentVehicles = testcarJourneyCrud.Invoke("GetVehiclesForCurrentCar", new object[] { car1, vehicles, allocations }) as List<EsrVehicle>;
            Assert.IsTrue(currentVehicles != null, "currentVehicles != null");
            Assert.IsTrue(currentVehicles.Count == 1, "Curent Vehicles.Count != 1");

            var rates = new List<CarVehicleJourneyRate>();
            var message = testcarJourneyCrud.Invoke(
                "FindNewVehicleJourneyRates", new object[] { 123, 100, currentVehicles, journeyRates, rates }) as string;
            Assert.IsTrue(message != null);
            Debug.WriteLine(message);
            Assert.IsTrue(message != string.Empty);
            
            Assert.IsTrue(rates.Count == 0, "Found rates should = 0");

            currentVehicles = testcarJourneyCrud.Invoke("GetVehiclesForCurrentCar", new object[] { car2, vehicles, allocations }) as List<EsrVehicle>;
            Assert.IsTrue(currentVehicles != null, "currentVehicles != null");
            Assert.IsTrue(currentVehicles.Count == 1, "Curent Vehicles.Count != 1");

            rates = new List<CarVehicleJourneyRate>();
            message = testcarJourneyCrud.Invoke(
                "FindNewVehicleJourneyRates", new object[] { 123, 101, currentVehicles, journeyRates, rates }) as string;
            Assert.IsTrue(message != null);
            Debug.WriteLine(message);
            Assert.IsTrue(message != string.Empty);
            Assert.IsTrue(rates.Count == 0, "Found rates should = 0");
        }

        /// <summary>
        /// The API RPC update car journey rates.
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcCarJourneyRateMultipleVehicles()
        {
            var journeyRates = this.GetVJR();
            var car1 = new Car { carid = 100, employeeid = 10, make = "Ford", registration = "123" };

            var vehicle1 = new EsrVehicle { ESRVehicleAllocationId = 1000, Make = "Ford", RegistrationNumber = "123", EngineCC = 1100, UserRatesTable = "nhs_basic" };
            var vehicle2 = new EsrVehicle { ESRVehicleAllocationId = 1001, Make = "Ford", RegistrationNumber = "123", EngineCC = 1400, UserRatesTable = "nhs_basic" };
            var vehicle3 = new EsrVehicle { ESRVehicleAllocationId = 1002, Make = "Ford", RegistrationNumber = "123", EngineCC = 1800, UserRatesTable = "nhs_basic" };
            var vehicle4 = new EsrVehicle { ESRVehicleAllocationId = 1003, Make = "Ford", RegistrationNumber = "123", EngineCC = 2200, UserRatesTable = "nhs_basic" };
            var vehicle5 = new EsrVehicle { ESRVehicleAllocationId = 1004, Make = "Ford", RegistrationNumber = "123", EngineCC = 2600, UserRatesTable = "nhs_basic" };
            var vehicle6 = new EsrVehicle { ESRVehicleAllocationId = 1005, Make = "Ford", RegistrationNumber = "123", EngineCC = 3000, UserRatesTable = "nhs_basic" };
            var vehicle7 = new EsrVehicle { ESRVehicleAllocationId = 1006, Make = "Ford", RegistrationNumber = "123", EngineCC = 3400, UserRatesTable = "nhs_basic" };
            var vehicle8 = new EsrVehicle { ESRVehicleAllocationId = 1007, Make = "Ford", RegistrationNumber = "123", EngineCC = 3800, UserRatesTable = "nhs_basic" };
            var vehicle9 = new EsrVehicle { ESRVehicleAllocationId = 1008, Make = "Ford", RegistrationNumber = "123", EngineCC = 4000, UserRatesTable = "nhs_basic" };
            var vehicleA = new EsrVehicle { ESRVehicleAllocationId = 1009, Make = "Ford", RegistrationNumber = "123", EngineCC = 4400, UserRatesTable = "nhs_basic" };

            var allocation1 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1000, CarId = 100 };
            var allocation2 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1001, CarId = 100 };
            var allocation3 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1002, CarId = 100 };
            var allocation4 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1003, CarId = 100 };
            var allocation5 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1004, CarId = 100 };
            var allocation6 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1005, CarId = 100 };
            var allocation7 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1006, CarId = 100 };
            var allocation8 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1007, CarId = 100 };
            var allocation9 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1008, CarId = 100 };
            var allocationA = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1009, CarId = 100 };

            var vehicles = new List<EsrVehicle> { vehicle1, vehicle2, vehicle3, vehicle4, vehicle5, vehicle6, vehicle7, vehicle8, vehicle9, vehicleA };

            var allocations = new List<CarAssignmentNumberAllocation> { allocation1, allocation2, allocation3, allocation4, allocation5, allocation6, allocation7, allocation8, allocation9, allocationA };

            var carJourney = new CarVehicleJourneyRateCrud("expenses", GlobalTestVariables.AccountId);

            var testcarJourneyCrud = new PrivateObject(carJourney);
            var currentVehicles = testcarJourneyCrud.Invoke("GetVehiclesForCurrentCar", new object[] { car1, vehicles, allocations }) as List<EsrVehicle>;
            Assert.IsTrue(currentVehicles != null, "currentVehicles != null");
            Assert.IsTrue(currentVehicles.Count == 10, "Curent Vehicles.Count != 10");

            var rates = new List<CarVehicleJourneyRate>();
            var message = testcarJourneyCrud.Invoke(
                "FindNewVehicleJourneyRates", new object[] { 123, 100, currentVehicles, journeyRates, rates }) as string;
            Assert.IsTrue(message != null);
            Debug.WriteLine(message);
            Assert.IsTrue(message != string.Empty);

            Assert.IsTrue(rates.Count == 7, "Found rates should = 7");
        }

        /// <summary>
        /// The API RPC update car journey rates.
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcCarJourneyRateMatchMultipleVJR()
        {
            var journeyRates = this.GetVJR();
            journeyRates.Add(new VehicleJourneyRate{ MileageId = 5, UserRatesTable = "NHS_BASIC", UserRatesFromEngineSize = 0, UserRatesToEngineSize = 999999 });
            journeyRates.Add(new VehicleJourneyRate{ MileageId = 6, UserRatesTable = "NHS_BASIC", UserRatesFromEngineSize = 0, UserRatesToEngineSize = 999999 });
            var car1 = new Car { carid = 100, employeeid = 10, make = "Ford", registration = "123" };
            var car2 = new Car { carid = 101, employeeid = 10, make = "Kia", registration = "124" };

            var vehicle1 = new EsrVehicle { ESRVehicleAllocationId = 1000, Make = "Ford", RegistrationNumber = "123", EngineCC = 2000, UserRatesTable = "nhs_basic" };

            var allocation1 = new CarAssignmentNumberAllocation { ESRVehicleAllocationId = 1000, CarId = 100 };

            var vehicles = new List<EsrVehicle> { vehicle1};

            var allocations = new List<CarAssignmentNumberAllocation> { allocation1 };

            var carJourney = new CarVehicleJourneyRateCrud("expenses", GlobalTestVariables.AccountId);

            var testcarJourneyCrud = new PrivateObject(carJourney);
            var currentVehicles = testcarJourneyCrud.Invoke("GetVehiclesForCurrentCar", new object[] { car1, vehicles, allocations }) as List<EsrVehicle>;
            Assert.IsTrue(currentVehicles != null, "currentVehicles != null");
            Assert.IsTrue(currentVehicles.Count == 1, "Curent Vehicles.Count != 1");

            var rates = new List<CarVehicleJourneyRate>();
            var message = testcarJourneyCrud.Invoke(
                "FindNewVehicleJourneyRates", new object[] { 123, 100, currentVehicles, journeyRates, rates }) as string;
            Assert.IsTrue(message != null);
            Debug.WriteLine(message);
            Assert.IsTrue(message == string.Empty);

            Assert.IsTrue(rates.Count == 3, "Found rates should = 3");
        }



        /// <summary>
        /// The API RPC check rates changed tests. 
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcCheckRatesChanged()
        {
            var carJourney = new CarVehicleJourneyRateCrud("expenses", GlobalTestVariables.AccountId);
            var testcarJourneyCrud = new PrivateObject(carJourney);
            var currentRates = new List<CarVehicleJourneyRate> { new CarVehicleJourneyRate { CarId = 1, MileageId = 1 } };
            var newRates = new List<CarVehicleJourneyRate> { new CarVehicleJourneyRate { CarId = 1, MileageId = 1 } };
            var result = false;
            result = (bool)testcarJourneyCrud.Invoke("FindChangesInCarRates", new object[] { currentRates, newRates });
            Assert.IsFalse(result, "current and new are the same");

            currentRates = new List<CarVehicleJourneyRate> { new CarVehicleJourneyRate { CarId = 1, MileageId = 1 } };
            newRates = new List<CarVehicleJourneyRate> { new CarVehicleJourneyRate { CarId = 1, MileageId = 2 } };
            result = (bool)testcarJourneyCrud.Invoke("FindChangesInCarRates", new object[] { currentRates, newRates });
            Assert.IsFalse(!result, "current and new are not the same");

            currentRates = new List<CarVehicleJourneyRate>();
            newRates = new List<CarVehicleJourneyRate> { new CarVehicleJourneyRate { CarId = 1, MileageId = 2 } };
            result = (bool)testcarJourneyCrud.Invoke("FindChangesInCarRates", new object[] { currentRates, newRates });
            Assert.IsFalse(!result, "current and new are not the same");

            currentRates = new List<CarVehicleJourneyRate> { new CarVehicleJourneyRate { CarId = 1, MileageId = 1 } };
            newRates = new List<CarVehicleJourneyRate> { new CarVehicleJourneyRate { CarId = 1, MileageId = 1 }, new CarVehicleJourneyRate { CarId = 1, MileageId = 3 } };
            result = (bool)testcarJourneyCrud.Invoke("FindChangesInCarRates", new object[] { currentRates, newRates });
            Assert.IsFalse(!result, "current and new are not the same");

            currentRates = new List<CarVehicleJourneyRate> { new CarVehicleJourneyRate { CarId = 1, MileageId = 1 }, new CarVehicleJourneyRate { CarId = 1, MileageId = 2 }, new CarVehicleJourneyRate { CarId = 1, MileageId = 3 } };
            newRates = new List<CarVehicleJourneyRate> { new CarVehicleJourneyRate { CarId = 1, MileageId = 1 }, new CarVehicleJourneyRate { CarId = 1, MileageId = 5 }, new CarVehicleJourneyRate { CarId = 1, MileageId = 3 } };
            result = (bool)testcarJourneyCrud.Invoke("FindChangesInCarRates", new object[] { currentRates, newRates });
            Assert.IsFalse(!result, "current and new are not the same");
        }

        /// <summary>
        /// API RPC Submit a person record for update, should fail validation.
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcCreatePersonInvalidRecord()
        {
            var apiRpiSvc = new ApiRpc();
            List<EsrPersonRecord> result = null;
            result = apiRpiSvc.UpdateEsrPersonRecords(
                GlobalTestVariables.AccountId, "371", new List<EsrPersonRecord> { new EsrPersonRecord() });
            Assert.IsTrue(
                result[0].ActionResult.Result == ApiActionResult.ValidationFailed, "Record did not fail validation");
            Assert.IsTrue(result[0].ActionResult.Message != string.Empty, "Should return validation error message");
        }

        /// <summary>
        /// API RPC Get Single Location Mock
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcGetEsrLocationMock()
        {
            var esrLocation = new EsrLocation
                              {
                                  ESRLocationId = 1,
                                  AddressLine1 = "1",
                                  ActionResult = new ApiResult { Result = ApiActionResult.Success }
                              };
            var apiCrudMock = new Mock<IDataAccess<EsrLocation>>();
            apiCrudMock.SetupAllProperties();
            apiCrudMock.Setup(x => x.Read(It.IsAny<int>()))
                       .Returns(esrLocation);

            var esrApiMock = new Mock<IEsrApi>();
            esrApiMock.SetupAllProperties();
            esrApiMock.Setup(x => x.Execute<EsrLocation>(It.IsAny<string>(), DataAccessMethod.Read, DataAccessMethodReturnDefault.NewObject)).Returns(esrLocation);

            var apiRpiSvc = new ApiRpc(GetMockDataLog(), esrApiMock.Object);
            var result = apiRpiSvc.GetEsrLocationRecord(GlobalTestVariables.AccountId, 1);

            Assert.IsTrue(result != null, "Should return a record, not null");
            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
        }

        /// <summary>
        /// API RPC Get Single Organisation Mock
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcGetEsrOrganisationMock()
        {
            var esrOrganisation = new EsrOrganisation
                                  {
                                      ESROrganisationId = 1,
                                      CostCentreDescription = "1",
                                      ActionResult =
                                          new ApiResult { Result = ApiActionResult.Success }
                                  };
            var apiCrudMock = new Mock<IDataAccess<EsrOrganisation>>();
            apiCrudMock.SetupAllProperties();
            apiCrudMock.Setup(x => x.Read(It.IsAny<int>())).Returns(esrOrganisation);

            var esrApiMock = new Mock<IEsrApi>();
            esrApiMock.SetupAllProperties();
            esrApiMock.Setup(x => x.Execute<EsrOrganisation>(It.IsAny<string>(), DataAccessMethod.Read, DataAccessMethodReturnDefault.NewObject)).Returns(esrOrganisation);

            var apiRpiSvc = new ApiRpc(GetMockDataLog(), esrApiMock.Object);
            var result = apiRpiSvc.GetEsrOrganisation(GlobalTestVariables.AccountId, 1);

            Assert.IsTrue(result != null, "Should return a record, not null");
            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
        }

        /// <summary>
        /// API RPC Get Single Position Mock
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcGetEsrPositionMock()
        {
            var esrPosition = new EsrPosition
                              {
                                  ESRPositionId = 1,
                                  JobRole = "1",
                                  ActionResult = new ApiResult { Result = ApiActionResult.Success }
                              };

            var apiCrudMock = new Mock<IDataAccess<EsrPosition>>();
            apiCrudMock.SetupAllProperties();
            apiCrudMock.Setup(x => x.Read(It.IsAny<int>())).Returns(esrPosition);

            var esrApiMock = new Mock<IEsrApi>();
            esrApiMock.SetupAllProperties();
            esrApiMock.Setup(x => x.Execute<EsrPosition>(It.IsAny<string>(), DataAccessMethod.Read, DataAccessMethodReturnDefault.NewObject)).Returns(esrPosition);

            var apiRpiSvc = new ApiRpc(GetMockDataLog());
            var result = apiRpiSvc.GetEsrPosition(GlobalTestVariables.AccountId, 1);

            Assert.IsTrue(result != null, "Should return a record, not null");
            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
        }
        
        /// <summary>
        /// API deterime the primary assignment, where ESR person has two assignments, one without an end date. 
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcDeterminePrimaryAssignmentOneAssignmentWithNoEndDate()
        {                    
            var esrPerson = this.CreateEsrPersonOneAssignmentWithNoEndDate(1, supervisorId);
            var primaryAssignment = esrPerson.PrimaryAssignment;

            Assert.AreEqual(primaryAssignment.SupervisorPersonId, supervisorId);
        }

        /// <summary>
        /// API deterime the primary assignment, where ESR person has four assignments, two without an end date. 
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcDeterminePrimaryAssignmentTwoAssignmentsWithNoEndDate()
        {
            var esrPerson = this.CreateEsrPersonTwoAssignmentsWithNoEndDates(1, supervisorId);
            var primaryAssignment = esrPerson.PrimaryAssignment;

            Assert.AreEqual(primaryAssignment.SupervisorPersonId, supervisorId);
        }

        /// <summary>
        /// API deterime the primary assignment, where ESR person has four assignments, two without an end date of which only one is flaged as primary. 
        /// </summary>
        [TestMethod]
        [TestCategory("ApiRpc")]
        [TestCategory("ApiCrudInterface")]
        public void ApiRpcDeterminePrimaryAssignmentTwoAssignmentshWithNoEndDateOnePrimary()
        {     
            var esrPerson = this.CreateEsrPersonTwoAssignmentsWithNoEndDateOnePrimary(1, supervisorId);
            var primaryAssignment = esrPerson.PrimaryAssignment;

            Assert.AreEqual(primaryAssignment.SupervisorPersonId, supervisorId);
        }

        #endregion

        #region Private

        /// <summary>
        /// The cache Vehicle Journey Rates.
        /// </summary>
        private List<VehicleJourneyRate> GetVJR()
        {
            var vehicleJourneyRates = new List<VehicleJourneyRate>
                                          {
                                              new VehicleJourneyRate
                                                  {
                                                      MileageId = 1,
                                                      UserRatesTable = "NHS1",
                                                      UserRatesFromEngineSize = 0,
                                                      UserRatesToEngineSize = 3500
                                                  },
                                              new VehicleJourneyRate
                                                  {
                                                      MileageId = 2,
                                                      UserRatesTable = "NHS2",
                                                      UserRatesFromEngineSize = 0,
                                                      UserRatesToEngineSize = 3500
                                                  },
                                              new VehicleJourneyRate
                                                  {
                                                      MileageId = 3,
                                                      UserRatesTable = "NHS_BASIC",
                                                      UserRatesFromEngineSize = 0,
                                                      UserRatesToEngineSize = 1400
                                                  },
                                              new VehicleJourneyRate
                                                  {
                                                      MileageId = 4,
                                                      UserRatesTable = "NHS_BASIC",
                                                      UserRatesFromEngineSize = 1401,
                                                      UserRatesToEngineSize = 3500
                                                  }
                                          };
            if (MemoryCache.Default.Contains(string.Format("VJR_{0}", GlobalTestVariables.AccountId)))
            {
                MemoryCache.Default.Remove(string.Format("VJR_{0}", GlobalTestVariables.AccountId));
            }

            return vehicleJourneyRates;
        }

        /// <summary>
        /// The create ESR person.
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPersonRecord"/>.
        /// </returns>
        private EsrPersonRecord CreateEsrPerson(int number)
        {
            var esrPerson = new EsrPersonRecord
                                {
                                    Action = Action.Create,
                                    ESRPersonId = number,
                                    FirstName = string.Format("first{0}", number),
                                    LastName = string.Format("last{0}", number),
                                    Title = "mr",
                                    EsrAssignments =
                                        new List<EsrAssignment>
                                            {
                                              CreateAssignment(number,new DateTime (2012, 12, 12),new DateTime (2012, 12, 12),true, null)},                                               
                                    Phones =
                                        new List<EsrPhone>
                                            {
                                                new EsrPhone
                                                    {
                                                        Action = Action.Create,
                                                        ESRPersonId = number,
                                                        ESRPhoneId = number,
                                                        PhoneNumber = "01522 888888",
                                                        PhoneType = "blue",
                                                        EffectiveStartDate =
                                                            DateTime.Now
                                                    }
                                            }
                                };
            return esrPerson;
        }

        /// <summary>
        /// Creates an ESR person with two assignments.
        /// One assignment has a start and end date. The other assignment has a start date, but no end date.  
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <param name="supervisorId">
        /// The supervisorId.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPersonRecord"/>.
        /// </returns>
        private EsrPersonRecord CreateEsrPersonOneAssignmentWithNoEndDate(int number, long supervisorId)
        {
            var esrPerson = new EsrPersonRecord
            {
                Action = Action.Create,
                ESRPersonId = number,
                FirstName = string.Format("first{0}", number),
                LastName = string.Format("last{0}", number),
                Title = "mr",
                EsrAssignments =
                    new List<EsrAssignment>
                                            {
                                                CreateAssignment(number, new DateTime(2013, 01, 01), new DateTime(2013, 01, 01),true,88888),
                                                CreateAssignment(number, new DateTime(2013, 02, 02),null,true, supervisorId),                                        
                                            },
                Phones =
                    new List<EsrPhone>
                                            {
                                                new EsrPhone
                                                    {
                                                        Action = Action.Create,
                                                        ESRPersonId = number,
                                                        ESRPhoneId = number,
                                                        PhoneNumber = "01522 888888",
                                                        PhoneType = "blue",
                                                        EffectiveStartDate =
                                                            DateTime.Now
                                                    }
                                            }
            };
            return esrPerson;
        }

        /// <summary>
        /// Create an ESR person with four assignments.
        /// Two of the assignments have a start date but no end date.
        /// All assignments are primary.
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <param name="supervisorId">
        /// The supervisorId.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPersonRecord"/>.
        /// </returns>
        private EsrPersonRecord CreateEsrPersonTwoAssignmentsWithNoEndDates(int number, long supervisorId)
        {
            var esrPerson = new EsrPersonRecord
            {
                Action = Action.Create,
                ESRPersonId = number,
                FirstName = string.Format("first{0}", number),
                LastName = string.Format("last{0}", number),
                Title = "mr",
                EsrAssignments =
                    new List<EsrAssignment>
                                            {
                                                CreateAssignment(number, new DateTime(2013, 12, 12),null,true,88888),
                                                CreateAssignment(number, new DateTime(2014, 01 , 01), new DateTime(2014, 02 , 02),true,88888),   
                                                CreateAssignment(number, new DateTime(2014, 03, 03), new DateTime(2014, 04 , 04),true,88888),
                                                CreateAssignment(number, new DateTime(2014, 05, 05), null,true, supervisorId),    
                                            },
                Phones =
                    new List<EsrPhone>
                                            {
                                                new EsrPhone
                                                    {
                                                        Action = Action.Create,
                                                        ESRPersonId = number,
                                                        ESRPhoneId = number,
                                                        PhoneNumber = "01522 888888",
                                                        PhoneType = "blue",
                                                        EffectiveStartDate =
                                                            DateTime.Now
                                                    }
                                            }
            };
            return esrPerson;
        }


        /// <summary>
        /// Create an ESR person with four assignments.
        /// Two of the assignments have a start date but no end date.   
        /// All assignments are primary, apart from one of the assignments with no end date.
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <param name="supervisorId">
        /// The supervisorId.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPersonRecord"/>.
        /// </returns>
        private EsrPersonRecord CreateEsrPersonTwoAssignmentsWithNoEndDateOnePrimary(int number, long supervisorId)
        {
            var esrPerson = new EsrPersonRecord
            {
                Action = Action.Create,
                ESRPersonId = number,
                FirstName = string.Format("first{0}", number),
                LastName = string.Format("last{0}", number),
                Title = "mr",
                EsrAssignments =
                    new List<EsrAssignment>
                                            {
                                                CreateAssignment(number, new DateTime(2013, 12, 12),null,true,supervisorId),
                                                CreateAssignment(number, new DateTime(2014, 01 , 01), new DateTime(2014, 02 , 02),true,88888),   
                                                CreateAssignment(number, new DateTime(2014, 03, 03), new DateTime(2014, 04 , 04),true,88888),
                                                CreateAssignment(number, new DateTime(2014, 5, 05), null,false, 88888),    
                                            },
                Phones =
                    new List<EsrPhone>
                                            {
                                                new EsrPhone
                                                    {
                                                        Action = Action.Create,
                                                        ESRPersonId = number,
                                                        ESRPhoneId = number,
                                                        PhoneNumber = "01522 888888",
                                                        PhoneType = "blue",
                                                        EffectiveStartDate =
                                                            DateTime.Now
                                                    }
                                            }
            };
            return esrPerson;
        }

        /// <summary>
        /// Creates an ESR Assignment
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <param name="isPrimary">     
        /// Is a primary assignment
        /// </param>
        /// <param name="effectiveStartDate">
        /// The effective start date
        /// </param>
        /// <param name="effectiveEndDate">
        /// The effective end date
        /// </param>
        /// <param name="supervisorId">
        /// The supervisor Id
        ///</param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        private static EsrAssignment CreateAssignment(int number,DateTime effectiveStartDate,DateTime? effectiveEndDate,bool isPrimary,long? supervisorId)
  
        {
            return new EsrAssignment
            {
                Action = Action.Create,
                ESRPersonId = number,
                AssignmentID = number + 1000,
                EsrLastUpdate = new DateTime ( 2012, 12, 12),
                EarliestAssignmentStartDate = new DateTime(2012, 12, 12),
                FinalAssignmentEndDate = new DateTime(2014, 12, 12),
                EffectiveStartDate = effectiveStartDate,
                EffectiveEndDate = effectiveEndDate,           
                AssignmentNumber = "112233",
                PrimaryAssignment = isPrimary,
                SupervisorPersonId = supervisorId
            };
        }


       
        /// <summary>
        /// The get mock data log.
        /// </summary>
        /// <returns>
        /// The <see cref="IApiDbConnection"/>.
        /// </returns>
        private static IApiDbConnection GetMockDataLog()
        {
            var logDataMock = new Mock<IApiDbConnection>();
            logDataMock.Setup(x => x.ConnectionStringValid).Returns(true);
            logDataMock.Setup(x => x.DebugLevel).Returns(Log.MessageLevel.Debug);
            logDataMock.Setup(x => x.Sqlexecute).Returns(new SqlCommand());
            return logDataMock.Object;
        }

        /// <summary>
        /// Create an import template for ESR version 2
        /// </summary>
        /// <param name="trustVpd">
        /// The trust VPD.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        private List<TemplateMapping> CreateImportTemplate(string trustVpd)
        {
            return new List<TemplateMapping>
                                      {
                                          new TemplateMapping
                                              {
                                                  columnRef = 2,
                                                  dataType = 10,
                                                  destinationField = "Assignment ID",
                                                  fieldID =
                                                      new Guid(
                                                      "052237ae-fd91-4314-b3bf-6fb1aa5ba3fc"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "7c0d8eab-d9af-415f-9bb7-d1be01f69e2f"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a02e429-5073-4a8a-add9-2bb0c426d973"),
                                                  templateID = 242,
                                                  templateMappingID = 10694,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 11,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Last Update Date",
                                                  fieldID =
                                                      new Guid(
                                                      "2883f81c-e8a5-462e-b5ae-be1484703c1d"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a02e429-5073-4a8a-add9-2bb0c426d973"),
                                                  templateID = 242,
                                                  templateMappingID = 10703,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 1,
                                                  dataType = 10,
                                                  destinationField = "Person ID",
                                                  fieldID =
                                                      new Guid(
                                                      "50678a74-7df6-47e1-b304-9107dddbcf8f"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "59ebde1e-3e1d-408a-8d86-5c35caf7fd5f"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "d9141b31-b265-4ace-9525-e18d1b76bb63"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a02e429-5073-4a8a-add9-2bb0c426d973"),
                                                  templateID = 242,
                                                  templateMappingID = 10693,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 3,
                                                  dataType = 1,
                                                  destinationField =
                                                      "Vehicle Allocation ID",
                                                  fieldID =
                                                      new Guid(
                                                      "b7db17ec-4341-41c2-9abb-ab80877a0051"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a02e429-5073-4a8a-add9-2bb0c426d973"),
                                                  templateID = 242,
                                                  templateMappingID = 10695,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 14,
                                                  dataType = 0,
                                                  destinationField = "Fuel Type",
                                                  fieldID =
                                                      new Guid(
                                                      "e45c6ca7-7256-4332-a1fb-a037eb5b1a6f"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a02e429-5073-4a8a-add9-2bb0c426d973"),
                                                  templateID = 242,
                                                  templateMappingID = 10706,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 10,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Initial Registration Date",
                                                  fieldID =
                                                      new Guid(
                                                      "2c610772-c186-46c7-95d5-6684749fd569"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a02e429-5073-4a8a-add9-2bb0c426d973"),
                                                  templateID = 242,
                                                  templateMappingID = 10702,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 9,
                                                  dataType = 0,
                                                  destinationField = "Ownership",
                                                  fieldID =
                                                      new Guid(
                                                      "e8d2797a-6bd1-497f-8ab7-ad539c790dbd"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a02e429-5073-4a8a-add9-2bb0c426d973"),
                                                  templateID = 242,
                                                  templateMappingID = 10701,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 13,
                                                  dataType = 0,
                                                  destinationField =
                                                      "User Rates Table",
                                                  fieldID =
                                                      new Guid(
                                                      "176a01f3-9d6b-4e86-b997-b057413f9233"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a02e429-5073-4a8a-add9-2bb0c426d973"),
                                                  templateID = 242,
                                                  templateMappingID = 10705,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 14,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Cost Centre Description",
                                                  fieldID =
                                                      new Guid(
                                                      "9e2f9196-493f-44e0-909b-b9ef34acbbc7"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10647,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 9,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Default Cost Centre",
                                                  fieldID =
                                                      new Guid(
                                                      "e75f176c-53a3-41e8-8ba5-7a53af99dc6a"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10642,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 4,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective From Date",
                                                  fieldID =
                                                      new Guid(
                                                      "d4199a54-28ff-49c1-a482-f0af05750678"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10637,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 5,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective To Date",
                                                  fieldID =
                                                      new Guid(
                                                      "bf5fd119-d6de-4c51-9fbb-1e31ceb4810d"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10638,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 13,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Last Update Date",
                                                  fieldID =
                                                      new Guid(
                                                      "78411e5b-c069-4f92-af45-736e1cac9146"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10646,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 12,
                                                  dataType = 10,
                                                  destinationField = "Location ID",
                                                  fieldID =
                                                      new Guid(
                                                      "96f35f9a-b6da-496c-8885-e45d64260d23"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "ca5bc3a8-6583-4eb3-ad94-004557017dbd"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10645,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 1,
                                                  dataType = 1,
                                                  destinationField =
                                                      "Organisation ID",
                                                  fieldID =
                                                      new Guid(
                                                      "97c23760-b082-4400-94db-272180922ec8"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10634,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 7,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Hierarchy Version Date From",
                                                  fieldID =
                                                      new Guid(
                                                      "d0b53949-6c97-4931-9764-ca51890fb97a"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10640,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 6,
                                                  dataType = 1,
                                                  destinationField =
                                                      "Hierarchy Version ID",
                                                  fieldID =
                                                      new Guid(
                                                      "4cacfe56-6cdd-4e17-8039-0996c24c349f"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10639,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 8,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Hierarchy Version Date To",
                                                  fieldID =
                                                      new Guid(
                                                      "a6985961-260d-4424-849a-76cb1b7132da"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10641,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 11,
                                                  dataType = 0,
                                                  destinationField = "NACS Code",
                                                  fieldID =
                                                      new Guid(
                                                      "724a5693-c988-405e-8bcf-2db548bceca6"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10644,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 2,
                                                  dataType = 0,
                                                  destinationField = "Org Name",
                                                  fieldID =
                                                      new Guid(
                                                      "813a9eb8-9526-4189-b9bf-bc46a7a769d2"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10635,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 3,
                                                  dataType = 0,
                                                  destinationField = "Org Type",
                                                  fieldID =
                                                      new Guid(
                                                      "98940532-a1b5-4ae4-a2cc-cad5f474a1f1"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10636,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 10,
                                                  dataType = 10,
                                                  destinationField = "Parent Org ID",
                                                  fieldID =
                                                      new Guid(
                                                      "8156b391-85d7-49aa-ab79-6031f859e1c2"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)4,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "97c23760-b082-4400-94db-272180922ec8"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  templateID = 242,
                                                  templateMappingID = 10643,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 5,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Assignment Address 1st line",
                                                  fieldID =
                                                      new Guid(
                                                      "9086176a-dfb9-4151-94dc-ee345f0a594b"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10617,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 6,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Address 1st Line",
                                                  fieldID =
                                                      new Guid(
                                                      "9086176a-dfb9-4151-94dc-ee345f0a594b"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10683,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 6,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Assignment Address 2nd line",
                                                  fieldID =
                                                      new Guid(
                                                      "4a3c1107-4b9f-44ed-8c03-ec01772e1584"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10618,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 7,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Address 2nd Line",
                                                  fieldID =
                                                      new Guid(
                                                      "4a3c1107-4b9f-44ed-8c03-ec01772e1584"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10684,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 7,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Assignment Address 3rd line",
                                                  fieldID =
                                                      new Guid(
                                                      "4d8320b0-6b7c-441f-b9be-8386ea76f156"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10619,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 8,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Address 3rd Line",
                                                  fieldID =
                                                      new Guid(
                                                      "4d8320b0-6b7c-441f-b9be-8386ea76f156"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10685,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 8,
                                                  dataType = 0,
                                                  destinationField = "Town",
                                                  fieldID =
                                                      new Guid(
                                                      "3d633887-6a9d-4d7d-8782-81ea6e1cf1ba"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10620,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 9,
                                                  dataType = 0,
                                                  destinationField = "Address Town",
                                                  fieldID =
                                                      new Guid(
                                                      "3d633887-6a9d-4d7d-8782-81ea6e1cf1ba"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10686,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 3,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Location Description",
                                                  fieldID =
                                                      new Guid(
                                                      "f31f204a-e714-4f0b-9b3d-7170db58a30b"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10615,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 2,
                                                  dataType = 0,
                                                  destinationField = "Location Code",
                                                  fieldID =
                                                      new Guid(
                                                      "258118d7-9b57-4c16-a99a-b7d07edc9a54"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10614,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 12,
                                                  dataType = 10,
                                                  destinationField =
                                                      "Address Country",
                                                  fieldID =
                                                      new Guid(
                                                      "488ad114-e79d-43ca-a849-de68035b9c64"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "b1e9111b-aa3c-4cc0-ad99-6c9e4b3e70fc"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "078dd298-7ca6-4e52-8d57-e85444c8797a"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10689,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 9,
                                                  dataType = 0,
                                                  destinationField = "County",
                                                  fieldID =
                                                      new Guid(
                                                      "af280d90-97e6-4590-9241-6364d8bde4ef"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10621,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 10,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Address County",
                                                  fieldID =
                                                      new Guid(
                                                      "af280d90-97e6-4590-9241-6364d8bde4ef"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10687,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 10,
                                                  dataType = 0,
                                                  destinationField = "Postcode",
                                                  fieldID =
                                                      new Guid(
                                                      "b84c65c7-7140-4930-b90e-ca5ad9374017"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10622,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 11,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Address Postcode",
                                                  fieldID =
                                                      new Guid(
                                                      "b84c65c7-7140-4930-b90e-ca5ad9374017"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "d0335558-1ef5-449b-87cd-3e6bbf126205"),
                                                  templateID = 242,
                                                  templateMappingID = 10688,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 10,
                                                  dataType = 0,
                                                  destinationField = "Analysis 1",
                                                  fieldID =
                                                      new Guid(
                                                      "8379781e-057c-403d-8138-813932aa8e68"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)9,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "3efb3d39-0583-42f2-8557-580319f2082a"),
                                                  templateID = 242,
                                                  templateMappingID = 10716,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 11,
                                                  dataType = 0,
                                                  destinationField = "Analysis 2",
                                                  fieldID =
                                                      new Guid(
                                                      "8f9e933b-e596-4cce-b3e8-2689f97da51e"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)9,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "3efb3d39-0583-42f2-8557-580319f2082a"),
                                                  templateID = 242,
                                                  templateMappingID = 10717,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 7,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Charitable Indicator",
                                                  fieldID =
                                                      new Guid(
                                                      "0442c960-117b-4280-bd35-120cc7fb53a8"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)9,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "3efb3d39-0583-42f2-8557-580319f2082a"),
                                                  templateID = 242,
                                                  templateMappingID = 10713,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 8,
                                                  dataType = 0,
                                                  destinationField = "Cost Centre",
                                                  fieldID =
                                                      new Guid(
                                                      "aa2fc0b7-9d8d-48c3-aa79-c4780cc19453"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)9,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "3efb3d39-0583-42f2-8557-580319f2082a"),
                                                  templateID = 242,
                                                  templateMappingID = 10714,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 5,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective End Date",
                                                  fieldID =
                                                      new Guid(
                                                      "12fd9127-507b-4b3c-8505-69250614651d"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)9,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "3efb3d39-0583-42f2-8557-580319f2082a"),
                                                  templateID = 242,
                                                  templateMappingID = 10711,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 4,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective Start Date",
                                                  fieldID =
                                                      new Guid(
                                                      "f6fe871d-7d49-4ef8-8ff3-1e0a146be8e3"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)9,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "3efb3d39-0583-42f2-8557-580319f2082a"),
                                                  templateID = 242,
                                                  templateMappingID = 10710,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 6,
                                                  dataType = 0,
                                                  destinationField = "Entity Code",
                                                  fieldID =
                                                      new Guid(
                                                      "8dc6914e-1491-4544-b37a-f8a0be5580ae"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)9,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "3efb3d39-0583-42f2-8557-580319f2082a"),
                                                  templateID = 242,
                                                  templateMappingID = 10712,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 2,
                                                  dataType = 10,
                                                  destinationField = "Assignment ID",
                                                  fieldID =
                                                      new Guid(
                                                      "adb54acb-1b85-4ac0-bc19-bf7b1a012a17"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)9,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "7c0d8eab-d9af-415f-9bb7-d1be01f69e2f"),
                                                  overridePrimaryKey = true,
                                                  tableid =
                                                      new Guid(
                                                      "3efb3d39-0583-42f2-8557-580319f2082a"),
                                                  templateID = 242,
                                                  templateMappingID = 10708,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 3,
                                                  dataType = 1,
                                                  destinationField =
                                                      "Costing Allocation ID",
                                                  fieldID =
                                                      new Guid(
                                                      "2656eec9-8d4e-49d8-af4e-1c750b607ede"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)9,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "3efb3d39-0583-42f2-8557-580319f2082a"),
                                                  templateID = 242,
                                                  templateMappingID = 10709,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 15,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Last Update Date",
                                                  fieldID =
                                                      new Guid(
                                                      "86f8c14f-136d-48bc-917a-44609cdb2386"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)9,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "3efb3d39-0583-42f2-8557-580319f2082a"),
                                                  templateID = 242,
                                                  templateMappingID = 10719,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 1,
                                                  dataType = 10,
                                                  destinationField = "Person ID",
                                                  fieldID =
                                                      new Guid(
                                                      "ee5bc925-544d-4896-b101-532009110ae7"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)9,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "59ebde1e-3e1d-408a-8d86-5c35caf7fd5f"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "d9141b31-b265-4ace-9525-e18d1b76bb63"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "3efb3d39-0583-42f2-8557-580319f2082a"),
                                                  templateID = 242,
                                                  templateMappingID = 10707,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 14,
                                                  dataType = 2,
                                                  destinationField =
                                                      "Percentage Split",
                                                  fieldID =
                                                      new Guid(
                                                      "f7a90c80-3013-4ff5-8b61-4da6cf2eb58c"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)9,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "3efb3d39-0583-42f2-8557-580319f2082a"),
                                                  templateID = 242,
                                                  templateMappingID = 10718,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 9,
                                                  dataType = 0,
                                                  destinationField = "Subjective",
                                                  fieldID =
                                                      new Guid(
                                                      "1bfb53b4-5510-43b5-a56f-0101c1f075d3"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)9,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "3efb3d39-0583-42f2-8557-580319f2082a"),
                                                  templateID = 242,
                                                  templateMappingID = 10715,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 38,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Last Update Date",
                                                  fieldID =
                                                      new Guid(
                                                      "3ceb6d76-971a-405d-b60b-3cc14326380b"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "59ebde1e-3e1d-408a-8d86-5c35caf7fd5f"),
                                                  templateID = 242,
                                                  templateMappingID = 10587,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 1,
                                                  dataType = 1,
                                                  destinationField = "Person ID",
                                                  fieldID =
                                                      new Guid(
                                                      "d9141b31-b265-4ace-9525-e18d1b76bb63"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "59ebde1e-3e1d-408a-8d86-5c35caf7fd5f"),
                                                  templateID = 242,
                                                  templateMappingID = 10570,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 5,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective End Date",
                                                  fieldID =
                                                      new Guid(
                                                      "2ab21296-77ee-4b3d-807c-56edf936613d"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "a184192f-74b6-42f7-8fdb-6dcf04723cef"),
                                                  templateID = 242,
                                                  templateMappingID = 10697,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 12,
                                                  dataType = 1,
                                                  destinationField = "Engine CC",
                                                  fieldID =
                                                      new Guid(
                                                      "1f4e203c-1436-494e-9f53-9ff4fc6e2be3"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "a184192f-74b6-42f7-8fdb-6dcf04723cef"),
                                                  templateID = 242,
                                                  templateMappingID = 10704,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 7,
                                                  dataType = 0,
                                                  destinationField = "Make",
                                                  fieldID =
                                                      new Guid(
                                                      "b7961f43-e439-4835-9709-396fff9bbd0c"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "a184192f-74b6-42f7-8fdb-6dcf04723cef"),
                                                  templateID = 242,
                                                  templateMappingID = 10699,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 8,
                                                  dataType = 0,
                                                  destinationField = "Model",
                                                  fieldID =
                                                      new Guid(
                                                      "99a078d9-f82c-4474-bdde-6701d4bd51ea"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "a184192f-74b6-42f7-8fdb-6dcf04723cef"),
                                                  templateID = 242,
                                                  templateMappingID = 10700,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 6,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Registration Number",
                                                  fieldID =
                                                      new Guid(
                                                      "156ccca7-1f5c-45be-920c-5e5c199ee81a"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "a184192f-74b6-42f7-8fdb-6dcf04723cef"),
                                                  templateID = 242,
                                                  templateMappingID = 10698,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 4,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective Start Date",
                                                  fieldID =
                                                      new Guid(
                                                      "d226c1bd-ecc3-4f37-a5fe-58638b1bd66c"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)8,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "a184192f-74b6-42f7-8fdb-6dcf04723cef"),
                                                  templateID = 242,
                                                  templateMappingID = 10696,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 6,
                                                  dataType = 2,
                                                  destinationField = "Budgeted FTE",
                                                  fieldID =
                                                      new Guid(
                                                      "b1f1371d-755f-46e2-a57d-65279de48436"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10653,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 19,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Deanery Post Number",
                                                  fieldID =
                                                      new Guid(
                                                      "e7ab72c3-7b62-44af-bccb-38cacd836beb"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10666,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 2,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective From Date",
                                                  fieldID =
                                                      new Guid(
                                                      "4c9f18d4-8643-4fe1-94e1-477fc19cf54e"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10649,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 3,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective To Date",
                                                  fieldID =
                                                      new Guid(
                                                      "f177e812-da3b-47c7-9d90-93f28bcbd185"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10650,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 18,
                                                  dataType = 0,
                                                  destinationField =
                                                      "EPP Flag (LOV)",
                                                  fieldID =
                                                      new Guid(
                                                      "001fcccc-ff2e-44fa-bcf4-430fdc964f20"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10665,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 22,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Last Update Date",
                                                  fieldID =
                                                      new Guid(
                                                      "90ae30a1-7788-42f7-a6f8-49ccf2788a10"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10669,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 14,
                                                  dataType = 10,
                                                  destinationField =
                                                      "Organisation ID",
                                                  fieldID =
                                                      new Guid(
                                                      "e8a7efde-5af9-4853-a925-5902547dd07c"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "97c23760-b082-4400-94db-272180922ec8"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10661,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 1,
                                                  dataType = 1,
                                                  destinationField = "Position ID",
                                                  fieldID =
                                                      new Guid(
                                                      "4d98adea-68e6-42a4-8962-fff40c5a79a8"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10648,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 12,
                                                  dataType = 0,
                                                  destinationField = "Grade Step",
                                                  fieldID =
                                                      new Guid(
                                                      "931ac944-7f12-474f-807d-68c88bdccd1d"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10659,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 15,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Hiring Status (LOV)",
                                                  fieldID =
                                                      new Guid(
                                                      "510a84b9-f07d-4177-b635-e2a9920be042"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10662,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 13,
                                                  dataType = 0,
                                                  destinationField =
                                                      "ISA Regulated Post (LOV)",
                                                  fieldID =
                                                      new Guid(
                                                      "ac0b259d-7e14-4946-8d5e-8e342947c427"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10660,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 9,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Job Role (LOV)",
                                                  fieldID =
                                                      new Guid(
                                                      "61ce419a-e146-49a1-b534-7166d5b792ec"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10656,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 8,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Job Staff Group (LOV)",
                                                  fieldID =
                                                      new Guid(
                                                      "f622bad9-e202-4bfb-874b-679bf145a40f"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10655,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 20,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Managing Deanery Body (LOV)",
                                                  fieldID =
                                                      new Guid(
                                                      "98e7365b-27ba-4aec-a538-787710eb5126"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10667,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 10,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Occupation Code (LOV)",
                                                  fieldID =
                                                      new Guid(
                                                      "c238949a-e9e4-4e80-aaf2-4686a744a1ba"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10657,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 17,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Eligible for OH Processing (LOV)",
                                                  fieldID =
                                                      new Guid(
                                                      "9f797795-bc62-4d94-95ba-c92c67d7e0ec"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10664,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 11,
                                                  dataType = 0,
                                                  destinationField = "Payscale",
                                                  fieldID =
                                                      new Guid(
                                                      "49e9e0e2-2879-4229-bd34-468e3b5a91f4"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10658,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 5,
                                                  dataType = 0,
                                                  destinationField = "Position Name",
                                                  fieldID =
                                                      new Guid(
                                                      "ecca2ef5-1973-4fc7-bfa4-38d2f7bdf302"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10652,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 4,
                                                  dataType = 1,
                                                  destinationField =
                                                      "Position Number",
                                                  fieldID =
                                                      new Guid(
                                                      "357ce8fc-6d97-44d7-aa4b-eca4628a0664"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10651,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 16,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Position Type (LOV)",
                                                  fieldID =
                                                      new Guid(
                                                      "cba0156d-55a0-4789-924f-22e45b22c8c4"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10663,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 7,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Subjective Code (LOV)",
                                                  fieldID =
                                                      new Guid(
                                                      "bf41cb01-9755-4315-83e4-667d732e6b32"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10654,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 23,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Subjective Code Description (LOV)",
                                                  fieldID =
                                                      new Guid(
                                                      "55de1cd1-37a8-4815-bd01-499c83964c7d"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10670,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 21,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Workplace Org Code (LOV)",
                                                  fieldID =
                                                      new Guid(
                                                      "bfea0bd3-3934-483b-bd68-9eba7fa3c913"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)5,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  templateID = 242,
                                                  templateMappingID = 10668,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 2,
                                                  dataType = 1,
                                                  destinationField = "Assignment ID",
                                                  fieldID =
                                                      new Guid(
                                                      "7c0d8eab-d9af-415f-9bb7-d1be01f69e2f"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10589,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 6,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Assignment Type",
                                                  fieldID =
                                                      new Guid(
                                                      "4aa315a6-6472-4c71-8c85-d4f117daf2e1"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10593,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 18,
                                                  dataType = 10,
                                                  destinationField =
                                                      "Department Manager Person ID",
                                                  fieldID =
                                                      new Guid(
                                                      "081837aa-a9ef-4316-bf97-15507b7febfe"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "59ebde1e-3e1d-408a-8d86-5c35caf7fd5f"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "d9141b31-b265-4ace-9525-e18d1b76bb63"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10603,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 5,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Earliest Assignment Start Date",
                                                  fieldID =
                                                      new Guid(
                                                      "c53828af-99ff-463f-93f9-2721df44e5f2"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10592,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 4,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective End Date",
                                                  fieldID =
                                                      new Guid(
                                                      "9a0c99b6-da12-44dd-894f-dac34cab04fd"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10591,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 3,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective Start Date",
                                                  fieldID =
                                                      new Guid(
                                                      "438aa555-1748-4c26-91fe-8044138a2e76"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10590,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 10,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Employee Status Flag",
                                                  fieldID =
                                                      new Guid(
                                                      "d2b92c35-e369-4ce9-95a4-af2173840b01"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10595,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 44,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Last Update Date",
                                                  fieldID =
                                                      new Guid(
                                                      "7dc52316-924c-4a11-9fda-17595a48e7cd"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10611,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 13,
                                                  dataType = 10,
                                                  destinationField =
                                                      "Assignment Location ID",
                                                  fieldID =
                                                      new Guid(
                                                      "caee4043-b236-47eb-a067-c11422f38a17"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "ca5bc3a8-6583-4eb3-ad94-004557017dbd"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10598,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 27,
                                                  dataType = 10,
                                                  destinationField =
                                                      "Organisation ID",
                                                  fieldID =
                                                      new Guid(
                                                      "5d35ffe4-6915-474c-b195-93dd543474c0"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "7ec6044c-2f3d-4f3b-a348-38426c460bbc"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "97c23760-b082-4400-94db-272180922ec8"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10604,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 1,
                                                  dataType = 1,
                                                  destinationField = "Person ID",
                                                  fieldID =
                                                      new Guid(
                                                      "c08ff30b-997e-48f5-a6d1-e4c24ac38cc1"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10588,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 28,
                                                  dataType = 10,
                                                  destinationField = "Position ID",
                                                  fieldID =
                                                      new Guid(
                                                      "db2943c1-ef09-452c-b295-7cb64d6d0b85"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "26777f45-1d24-4623-9f18-8639e7d6227f"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "4d98adea-68e6-42a4-8962-fff40c5a79a8"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10605,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 35,
                                                  dataType = 0,
                                                  destinationField = "People Group",
                                                  fieldID =
                                                      new Guid(
                                                      "a213ccdb-a364-449b-814d-7f433a182dde"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10608,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 34,
                                                  dataType = 0,
                                                  destinationField = "Job Name",
                                                  fieldID =
                                                      new Guid(
                                                      "59a8b2fb-1874-41b8-aeaf-16e61ab01f8e"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10607,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 45,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Last Working Day",
                                                  fieldID =
                                                      new Guid(
                                                      "1d806ca8-dce1-4e72-a31f-c9ece41e27a6"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10612,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 11,
                                                  dataType = 0,
                                                  destinationField = "Payroll Name",
                                                  fieldID =
                                                      new Guid(
                                                      "e8ea6d07-ec23-4d41-8b42-416f0e139990"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10596,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 12,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Payroll Period Type",
                                                  fieldID =
                                                      new Guid(
                                                      "4fca7747-a09a-424f-a1e1-371c91367840"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10597,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 29,
                                                  dataType = 0,
                                                  destinationField = "Position Name",
                                                  fieldID =
                                                      new Guid(
                                                      "1a623436-381a-4508-a8cf-b6ac4cd50f00"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10606,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 38,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Projected Hire Date",
                                                  fieldID =
                                                      new Guid(
                                                      "772c17fa-a2c6-4562-835b-5162e397ca62"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10610,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 16,
                                                  dataType = 10,
                                                  destinationField =
                                                      "Supervisor Assignment ID",
                                                  fieldID =
                                                      new Guid(
                                                      "3a0a78fb-eed3-4d70-94c1-12b8de558cd1"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "7c0d8eab-d9af-415f-9bb7-d1be01f69e2f"),
                                                  overridePrimaryKey = true,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10601,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 17,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Supervisor Assignment Number",
                                                  fieldID =
                                                      new Guid(
                                                      "ed828976-f992-4adc-b461-27ee83ebfdc8"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10602,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 14,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Supervisor Flag",
                                                  fieldID =
                                                      new Guid(
                                                      "ac849695-59e2-46bf-bb19-5b57f830feeb"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10599,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 15,
                                                  dataType = 10,
                                                  destinationField =
                                                      "Supervisor Person ID",
                                                  fieldID =
                                                      new Guid(
                                                      "d68db9d1-0a73-4a76-aae9-f0de5f19f9ff"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "59ebde1e-3e1d-408a-8d86-5c35caf7fd5f"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "d9141b31-b265-4ace-9525-e18d1b76bb63"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10600,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 36,
                                                  dataType = 0,
                                                  destinationField = "T and A Flag",
                                                  fieldID =
                                                      new Guid(
                                                      "d1f40f3f-f14e-4082-b7d6-87decd3faa2b"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "bf9aa39a-82d6-4960-bfef-c5943bc0542d"),
                                                  templateID = 242,
                                                  templateMappingID = 10609,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 11,
                                                  dataType = 0,
                                                  destinationField = "Country",
                                                  fieldID =
                                                      new Guid(
                                                      "eefad055-e0b3-4961-b79c-62aaec38b71c"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  templateID = 242,
                                                  templateMappingID = 10623,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 21,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Last Update Date",
                                                  fieldID =
                                                      new Guid(
                                                      "d62db224-5c6c-4cc6-a8fd-930a1eb711f8"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  templateID = 242,
                                                  templateMappingID = 10633,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 1,
                                                  dataType = 1,
                                                  destinationField = "Location ID",
                                                  fieldID =
                                                      new Guid(
                                                      "ca5bc3a8-6583-4eb3-ad94-004557017dbd"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  templateID = 242,
                                                  templateMappingID = 10613,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 13,
                                                  dataType = 0,
                                                  destinationField = "Fax",
                                                  fieldID =
                                                      new Guid(
                                                      "1ae6707f-1bf3-4df1-b936-f2c6903edc29"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  templateID = 242,
                                                  templateMappingID = 10625,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 4,
                                                  dataType = 6,
                                                  destinationField = "Inactive Date",
                                                  fieldID =
                                                      new Guid(
                                                      "10a90941-1de1-48d1-a6aa-40e0d7a70dde"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  templateID = 242,
                                                  templateMappingID = 10616,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 14,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Payslip Delivery Point",
                                                  fieldID =
                                                      new Guid(
                                                      "26b4b106-4648-4aea-a3a0-08ed8470fb0a"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  templateID = 242,
                                                  templateMappingID = 10626,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 15,
                                                  dataType = 0,
                                                  destinationField = "Site Code",
                                                  fieldID =
                                                      new Guid(
                                                      "b16330af-670c-471a-bd9b-43c7713cc714"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  templateID = 242,
                                                  templateMappingID = 10627,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 12,
                                                  dataType = 0,
                                                  destinationField = "Telephone",
                                                  fieldID =
                                                      new Guid(
                                                      "907987b5-a9b1-4ad4-a810-e957d3ef4a77"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  templateID = 242,
                                                  templateMappingID = 10624,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 17,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Welsh Address Line 1",
                                                  fieldID =
                                                      new Guid(
                                                      "f648f51a-18a2-4662-9a36-0df33675b908"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  templateID = 242,
                                                  templateMappingID = 10629,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 18,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Welsh Address Line 2",
                                                  fieldID =
                                                      new Guid(
                                                      "fd9220f8-250a-45d7-b0ba-dc8ad14f3838"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  templateID = 242,
                                                  templateMappingID = 10630,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 19,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Welsh Address Line 3",
                                                  fieldID =
                                                      new Guid(
                                                      "94ef7bab-d2f5-4206-89eb-fb804d2fdcf9"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  templateID = 242,
                                                  templateMappingID = 10631,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 16,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Welsh Location Translation",
                                                  fieldID =
                                                      new Guid(
                                                      "404f17c8-537e-44f3-8b8f-ac34539e05c2"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  templateID = 242,
                                                  templateMappingID = 10628,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 20,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Welsh Town Translation",
                                                  fieldID =
                                                      new Guid(
                                                      "43b82f7a-259f-4a31-93ab-8f1e8d004e05"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)3,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "1f8a5a08-dea3-4ee2-a612-c79b9fb4670b"),
                                                  templateID = 242,
                                                  templateMappingID = 10632,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 6,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective End Date",
                                                  fieldID =
                                                      new Guid(
                                                      "cdd39326-ea61-4a3c-9188-21815ac6ad14"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)6,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "e663770c-fb8d-4ede-93d4-cb214e944d3a"),
                                                  templateID = 242,
                                                  templateMappingID = 10676,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 5,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective Start Date",
                                                  fieldID =
                                                      new Guid(
                                                      "14a17772-c54e-4013-a594-46adce3c9572"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)6,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "e663770c-fb8d-4ede-93d4-cb214e944d3a"),
                                                  templateID = 242,
                                                  templateMappingID = 10675,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 7,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Last Update Date",
                                                  fieldID =
                                                      new Guid(
                                                      "4f21e9b0-e14e-472f-b696-10e627eee8a7"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)6,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "e663770c-fb8d-4ede-93d4-cb214e944d3a"),
                                                  templateID = 242,
                                                  templateMappingID = 10677,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 1,
                                                  dataType = 10,
                                                  destinationField = "Person Id",
                                                  fieldID =
                                                      new Guid(
                                                      "87cb2610-3242-416c-a387-de7a0b39dd0e"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)6,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "59ebde1e-3e1d-408a-8d86-5c35caf7fd5f"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "d9141b31-b265-4ace-9525-e18d1b76bb63"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "e663770c-fb8d-4ede-93d4-cb214e944d3a"),
                                                  templateID = 242,
                                                  templateMappingID = 10671,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 2,
                                                  dataType = 1,
                                                  destinationField = "Phone Id",
                                                  fieldID =
                                                      new Guid(
                                                      "a5d8869a-7e28-4f43-a9db-9da06a1b6b73"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)6,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "e663770c-fb8d-4ede-93d4-cb214e944d3a"),
                                                  templateID = 242,
                                                  templateMappingID = 10672,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 4,
                                                  dataType = 0,
                                                  destinationField = "Phone Number",
                                                  fieldID =
                                                      new Guid(
                                                      "b8c22cc7-763e-483c-9af1-c85d3a9c575f"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)6,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "e663770c-fb8d-4ede-93d4-cb214e944d3a"),
                                                  templateID = 242,
                                                  templateMappingID = 10674,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 3,
                                                  dataType = 0,
                                                  destinationField = "Phone Type",
                                                  fieldID =
                                                      new Guid(
                                                      "0ba28c4f-6c01-4397-b21c-d9f8d8e65ac8"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)6,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "e663770c-fb8d-4ede-93d4-cb214e944d3a"),
                                                  templateID = 242,
                                                  templateMappingID = 10673,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 4,
                                                  dataType = 0,
                                                  destinationField = "Address Style",
                                                  fieldID =
                                                      new Guid(
                                                      "371a6bd0-cdb4-492b-bdab-af124b749a3a"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a793f19-e4a8-4dc9-8249-de20a53e5bcb"),
                                                  templateID = 242,
                                                  templateMappingID = 10681,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 3,
                                                  dataType = 0,
                                                  destinationField = "Address Type",
                                                  fieldID =
                                                      new Guid(
                                                      "c50c92da-7ea8-4038-ad48-c718b20e4890"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a793f19-e4a8-4dc9-8249-de20a53e5bcb"),
                                                  templateID = 242,
                                                  templateMappingID = 10680,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 14,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective End Date",
                                                  fieldID =
                                                      new Guid(
                                                      "37cd9f5e-8add-43d6-9f86-cc2f4e88c912"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a793f19-e4a8-4dc9-8249-de20a53e5bcb"),
                                                  templateID = 242,
                                                  templateMappingID = 10691,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 13,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective Start Date",
                                                  fieldID =
                                                      new Guid(
                                                      "bf28e2ef-a325-463c-b9a6-26f15ad104b1"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a793f19-e4a8-4dc9-8249-de20a53e5bcb"),
                                                  templateID = 242,
                                                  templateMappingID = 10690,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 2,
                                                  dataType = 1,
                                                  destinationField = "Address ID",
                                                  fieldID =
                                                      new Guid(
                                                      "c149b36c-d04a-48b0-bb0a-66abdbe76b8f"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a793f19-e4a8-4dc9-8249-de20a53e5bcb"),
                                                  templateID = 242,
                                                  templateMappingID = 10679,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 15,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Last Update Date",
                                                  fieldID =
                                                      new Guid(
                                                      "be7c11ff-bd09-4a1c-8046-b58054697a57"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a793f19-e4a8-4dc9-8249-de20a53e5bcb"),
                                                  templateID = 242,
                                                  templateMappingID = 10692,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 1,
                                                  dataType = 10,
                                                  destinationField = "Person ID",
                                                  fieldID =
                                                      new Guid(
                                                      "2ad23788-ed3b-4c67-8547-f57094c4f5e2"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "59ebde1e-3e1d-408a-8d86-5c35caf7fd5f"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "d9141b31-b265-4ace-9525-e18d1b76bb63"),
                                                      overridePrimaryKey = false,
                                                      tableid =
                                                      new Guid(
                                                          "4a793f19-e4a8-4dc9-8249-de20a53e5bcb"),
                                                      templateID = 242,
                                                      templateMappingID = 10678,
                                                      trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 5,
                                                  dataType = 0,
                                                  destinationField = "Primary Flag",
                                                  fieldID =
                                                      new Guid(
                                                      "71584448-aef9-4da7-987f-94dbbd045076"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)7,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "4a793f19-e4a8-4dc9-8249-de20a53e5bcb"),
                                                  templateID = 242,
                                                  templateMappingID = 10682,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 13,
                                                  dataType = 6,
                                                  destinationField = "Date of Birth",
                                                  fieldID =
                                                      new Guid(
                                                      "486554c0-75ed-4718-ba32-3c11eaa5ee79"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10580,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 35,
                                                  dataType = 0,
                                                  destinationField = "Office e-Mail",
                                                  fieldID =
                                                      new Guid(
                                                      "0f951c3e-29d1-49f0-ac13-4cfcabf21fda"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10586,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 4,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Employee Number",
                                                  fieldID =
                                                      new Guid(
                                                      "d7da409b-3e6d-4c92-b737-adb4d90c23e5"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10573,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 3,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective End Date",
                                                  fieldID =
                                                      new Guid(
                                                      "02b90a6a-d1f0-4b99-a3a4-06c6bae02560"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10572,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 2,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Effective Start Date",
                                                  fieldID =
                                                      new Guid(
                                                      "7626d350-d51a-42ee-84c9-127fb1edc580"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10571,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 34,
                                                  dataType = 0,
                                                  destinationField =
                                                      "User Person Type",
                                                  fieldID =
                                                      new Guid(
                                                      "1883ff7e-12ce-4e81-a1a1-fff6142ff13f"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10585,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 7,
                                                  dataType = 0,
                                                  destinationField = "First Name",
                                                  fieldID =
                                                      new Guid(
                                                      "6614acad-0a43-4e30-90ec-84de0792b1d6"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10576,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 16,
                                                  dataType = 6,
                                                  destinationField = "Hire Date",
                                                  fieldID =
                                                      new Guid(
                                                      "76473c0a-df08-40f9-8de0-632d0111a912"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10583,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 9,
                                                  dataType = 0,
                                                  destinationField = "Maiden Name",
                                                  fieldID =
                                                      new Guid(
                                                      "075fda2d-efd4-461f-928f-9ff582a8b6ac"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10578,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 8,
                                                  dataType = 0,
                                                  destinationField = "Middle Names",
                                                  fieldID =
                                                      new Guid(
                                                      "b3caf703-e72b-4eb8-9d5c-b389e16c8c43"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10577,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 15,
                                                  dataType = 0,
                                                  destinationField = "NHS Unique ID",
                                                  fieldID =
                                                      new Guid(
                                                      "ea2ecf94-ab38-475f-8454-c57da22c88ba"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10582,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 14,
                                                  dataType = 0,
                                                  destinationField =
                                                      "National Insurance Number",
                                                  fieldID =
                                                      new Guid(
                                                      "74391669-0070-4d19-af73-3ab4ea4d55bb"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10581,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 7,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Assignment Number",
                                                  fieldID =
                                                      new Guid(
                                                      "6a76898b-4052-416c-b870-61479ca15ac1"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)2,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10594,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 10,
                                                  dataType = 0,
                                                  destinationField =
                                                      "Preferred Name",
                                                  fieldID =
                                                      new Guid(
                                                      "3309e5e0-6105-4e2c-be90-a8ead03eacf2"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10579,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 6,
                                                  dataType = 0,
                                                  destinationField = "Last Name",
                                                  fieldID =
                                                      new Guid(
                                                      "9d70d151-5905-4a67-944f-1ad6d22cd931"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10575,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 17,
                                                  dataType = 6,
                                                  destinationField =
                                                      "Actual Termination Date",
                                                  fieldID =
                                                      new Guid(
                                                      "b7cbf994-4a23-4405-93df-d66d4c3ed2a3"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = false,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10584,
                                                  trustVPD = trustVpd
                                              },
                                          new TemplateMapping
                                              {
                                                  columnRef = 5,
                                                  dataType = 0,
                                                  destinationField = "Title",
                                                  fieldID =
                                                      new Guid(
                                                      "28471060-247d-461c-abf6-234bcb4698aa"),
                                                  importElementType =
                                                      (
                                                      TemplateMapping.
                                                          ImportElementType)1,
                                                  importField = true,
                                                  lookupTable =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  mandatory = true,
                                                  matchField =
                                                      new Guid(
                                                      "00000000-0000-0000-0000-000000000000"),
                                                  overridePrimaryKey = false,
                                                  tableid =
                                                      new Guid(
                                                      "618db425-f430-4660-9525-ebab444ed754"),
                                                  templateID = 242,
                                                  templateMappingID = 10574,
                                                  trustVPD = trustVpd
                                              }
                                      };
        }

        #endregion
    }
}

