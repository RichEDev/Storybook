using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cCarTest and is intended
    ///to contain all cCarTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cCarTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for cCar Constructor
        ///</summary>
        [TestMethod()]
        public void cCarConstructorTest1()
        {
            int accountid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int carid = 0; // TODO: Initialize to an appropriate value
            string make = string.Empty; // TODO: Initialize to an appropriate value
            string model = string.Empty; // TODO: Initialize to an appropriate value
            string regno = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<DateTime> startdate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> enddate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            bool active = false; // TODO: Initialize to an appropriate value
            List<int> mileagecats = null; // TODO: Initialize to an appropriate value
            byte cartypeid = 0; // TODO: Initialize to an appropriate value
            long odometer = 0; // TODO: Initialize to an appropriate value
            bool fuelcard = false; // TODO: Initialize to an appropriate value
            int endodometer = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> taxexpiry = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> taxlastchecked = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            int taxcheckedby = 0; // TODO: Initialize to an appropriate value
            string mottestnumber = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<DateTime> motexpiry = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> motlastchecked = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            int motcheckedby = 0; // TODO: Initialize to an appropriate value
            string insurancenumber = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<DateTime> insuranceexpiry = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> insurancelastchecked = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            int insurancecheckedby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> serviceexpiry = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> servicelastchecked = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            int servicecheckedby = 0; // TODO: Initialize to an appropriate value
            SortedList<int, object> userdefined = null; // TODO: Initialize to an appropriate value
            cOdometerReading[] odometerreadings = null; // TODO: Initialize to an appropriate value
            string insurancepath = string.Empty; // TODO: Initialize to an appropriate value
            string motpath = string.Empty; // TODO: Initialize to an appropriate value
            string servicepath = string.Empty; // TODO: Initialize to an appropriate value
            string taxpath = string.Empty; // TODO: Initialize to an appropriate value
            MileageUOM defaultuom = new MileageUOM(); // TODO: Initialize to an appropriate value
            int engineSize = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> createdon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            int createdby = 0; // TODO: Initialize to an appropriate value
            Nullable<DateTime> modifiedon = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<int> modifiedby = new Nullable<int>(); // TODO: Initialize to an appropriate value
            bool approved = false; // TODO: Initialize to an appropriate value
            bool exemptfromhometooffice = false; // TODO: Initialize to an appropriate value
            Nullable<int> TaxAttachID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> MOTAttachID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> InsuranceAttachID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> ServiceAttachID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cCar target = new cCar(accountid, employeeid, carid, make, model, regno, startdate, enddate, active, mileagecats, cartypeid, odometer, fuelcard, endodometer, taxexpiry, taxlastchecked, taxcheckedby, mottestnumber, motexpiry, motlastchecked, motcheckedby, insurancenumber, insuranceexpiry, insurancelastchecked, insurancecheckedby, serviceexpiry, servicelastchecked, servicecheckedby, userdefined, odometerreadings, insurancepath, motpath, servicepath, taxpath, defaultuom, engineSize, createdon, createdby, modifiedon, modifiedby, approved, exemptfromhometooffice, TaxAttachID, MOTAttachID, InsuranceAttachID, ServiceAttachID);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cCar Constructor
        ///</summary>
        [TestMethod()]
        public void cCarConstructorTest()
        {
            cCar target = new cCar();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Clone
        ///</summary>
        [TestMethod()]
        public void CloneTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = target.Clone();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getLastOdometerReading
        ///</summary>
        [TestMethod()]
        public void getLastOdometerReadingTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            cOdometerReading expected = null; // TODO: Initialize to an appropriate value
            cOdometerReading actual;
            actual = target.getLastOdometerReading();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void passesDutyOfCareTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            bool blocktaxexpiry = false; // TODO: Initialize to an appropriate value
            bool blockmotexpiry = false; // TODO: Initialize to an appropriate value
            bool blockinsuranceexpiry = false; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.passesDutyOfCare(blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for updateDocumentPaths
        ///</summary>
        [TestMethod()]
        public void updateDocumentPathsTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            string insurancepath = string.Empty; // TODO: Initialize to an appropriate value
            string motpath = string.Empty; // TODO: Initialize to an appropriate value
            string servicepath = string.Empty; // TODO: Initialize to an appropriate value
            string taxpath = string.Empty; // TODO: Initialize to an appropriate value
            target.updateDocumentPaths(insurancepath, motpath, servicepath, taxpath);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for accountid
        ///</summary>
        [TestMethod()]
        public void accountidTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.accountid;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for active
        ///</summary>
        [TestMethod()]
        public void activeTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.active;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Approved
        ///</summary>
        [TestMethod()]
        public void ApprovedTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Approved = expected;
            actual = target.Approved;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for carid
        ///</summary>
        [TestMethod()]
        public void caridTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.carid = expected;
            actual = target.carid;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cartypeid
        ///</summary>
        [TestMethod()]
        public void cartypeidTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            byte actual;
            actual = target.cartypeid;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for createdby
        ///</summary>
        [TestMethod()]
        public void createdbyTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.createdby;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for createdon
        ///</summary>
        [TestMethod()]
        public void createdonTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.createdon;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for defaultuom
        ///</summary>
        [TestMethod()]
        public void defaultuomTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            MileageUOM actual;
            actual = target.defaultuom;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for employeeid
        ///</summary>
        [TestMethod()]
        public void employeeidTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.employeeid;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for enddate
        ///</summary>
        [TestMethod()]
        public void enddateTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.enddate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for endodometer
        ///</summary>
        [TestMethod()]
        public void endodometerTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.endodometer;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EngineSize
        ///</summary>
        [TestMethod()]
        public void EngineSizeTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.EngineSize;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ExemptFromHomeToOffice
        ///</summary>
        [TestMethod()]
        public void ExemptFromHomeToOfficeTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ExemptFromHomeToOffice;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for fuelcard
        ///</summary>
        [TestMethod()]
        public void fuelcardTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.fuelcard;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InsuranceAttachID
        ///</summary>
        [TestMethod()]
        public void InsuranceAttachIDTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            actual = target.InsuranceAttachID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for insurancecheckedby
        ///</summary>
        [TestMethod()]
        public void insurancecheckedbyTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.insurancecheckedby;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for insuranceexpiry
        ///</summary>
        [TestMethod()]
        public void insuranceexpiryTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.insuranceexpiry;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for insurancelastchecked
        ///</summary>
        [TestMethod()]
        public void insurancelastcheckedTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.insurancelastchecked;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for insurancenumber
        ///</summary>
        [TestMethod()]
        public void insurancenumberTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.insurancenumber;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for insurancepath
        ///</summary>
        [TestMethod()]
        public void insurancepathTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.insurancepath;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for make
        ///</summary>
        [TestMethod()]
        public void makeTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.make;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for mileagecats
        ///</summary>
        [TestMethod()]
        public void mileagecatsTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            List<int> actual;
            actual = target.mileagecats;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for model
        ///</summary>
        [TestMethod()]
        public void modelTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.model;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for modifiedby
        ///</summary>
        [TestMethod()]
        public void modifiedbyTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            actual = target.modifiedby;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for modifiedon
        ///</summary>
        [TestMethod()]
        public void modifiedonTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.modifiedon;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MOTAttachID
        ///</summary>
        [TestMethod()]
        public void MOTAttachIDTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            actual = target.MOTAttachID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for motcheckedby
        ///</summary>
        [TestMethod()]
        public void motcheckedbyTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.motcheckedby;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for motexpiry
        ///</summary>
        [TestMethod()]
        public void motexpiryTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.motexpiry;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for motlastchecked
        ///</summary>
        [TestMethod()]
        public void motlastcheckedTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.motlastchecked;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for motpath
        ///</summary>
        [TestMethod()]
        public void motpathTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.motpath;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for mottestnumber
        ///</summary>
        [TestMethod()]
        public void mottestnumberTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.mottestnumber;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for odometer
        ///</summary>
        [TestMethod()]
        public void odometerTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            long expected = 0; // TODO: Initialize to an appropriate value
            long actual;
            target.odometer = expected;
            actual = target.odometer;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for odometerreadings
        ///</summary>
        [TestMethod()]
        public void odometerreadingsTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            cOdometerReading[] expected = null; // TODO: Initialize to an appropriate value
            cOdometerReading[] actual;
            target.odometerreadings = expected;
            actual = target.odometerreadings;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for registration
        ///</summary>
        [TestMethod()]
        public void registrationTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.registration;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ServiceAttachID
        ///</summary>
        [TestMethod()]
        public void ServiceAttachIDTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            actual = target.ServiceAttachID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for servicecheckedby
        ///</summary>
        [TestMethod()]
        public void servicecheckedbyTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.servicecheckedby;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for serviceexpiry
        ///</summary>
        [TestMethod()]
        public void serviceexpiryTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.serviceexpiry;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for servicelastchecked
        ///</summary>
        [TestMethod()]
        public void servicelastcheckedTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.servicelastchecked;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for servicepath
        ///</summary>
        [TestMethod()]
        public void servicepathTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.servicepath;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for startdate
        ///</summary>
        [TestMethod()]
        public void startdateTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.startdate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for TaxAttachID
        ///</summary>
        [TestMethod()]
        public void TaxAttachIDTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            actual = target.TaxAttachID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for taxcheckedby
        ///</summary>
        [TestMethod()]
        public void taxcheckedbyTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.taxcheckedby;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for taxexpiry
        ///</summary>
        [TestMethod()]
        public void taxexpiryTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.taxexpiry;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for taxlastchecked
        ///</summary>
        [TestMethod()]
        public void taxlastcheckedTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            actual = target.taxlastchecked;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for taxpath
        ///</summary>
        [TestMethod()]
        public void taxpathTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.taxpath;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for userdefined
        ///</summary>
        [TestMethod()]
        public void userdefinedTest()
        {
            cCar target = new cCar(); // TODO: Initialize to an appropriate value
            SortedList<int, object> expected = null; // TODO: Initialize to an appropriate value
            SortedList<int, object> actual;
            target.userdefined = expected;
            actual = target.userdefined;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
