using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using SpendManagementUnitTests.Global_Objects;
using Spend_Management;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cEmployeeTest and is intended
    ///to contain all cEmployeeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cEmployeeTest
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

        #region properties that have logic built in



        #endregion properties that have logic built in






        /// <summary>
        ///A test for readBroadcast
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_readBroadcastTest()
        {
            cEmployees emps = new cEmployees(cGlobalVariables.AccountID);
            cEmployeeObject.CreateUnreadBroadcastMessage(); // this appears to be an incorrect object currently
            Assert.Fail("The test is currently incorrect");
            cEmployee target = emps.GetEmployeeById(cGlobalVariables.EmployeeID);

            int broadcastid = cGlobalVariables.BroadcastID;
            bool expected = true;
            bool actual;
            actual = target.readBroadcast(broadcastid);
            Assert.AreEqual(expected, actual);
            cEmployeeObject.DeleteBroadcastMessage();

            cEmployee postread_target = emps.GetEmployeeById(cGlobalVariables.EmployeeID);
            expected = false;
            actual = target.readBroadcast(broadcastid);
            Assert.AreEqual(expected, actual);
        }



        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_passesDutyOfCareTest_trueWhenCheckingNothing_noLicencePresentNoCarsPresent()
        {
            cEmployee target = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
            List<cCar> lstCars = new List<cCar>();

            bool blocklicenceexpiry = false;
            bool blocktaxexpiry = false;
            bool blockmotexpiry = false;
            bool blockinsuranceexpiry = false;

            bool expected = true;
            bool actual;
            actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);

            Assert.AreEqual(expected, actual);
            cEmployeeObject.DeleteUTEmployee(target.employeeid, target.username);
        }

        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_passesDutyOfCareTest_falseWhenCheckingLicence_noLicencePresent()
        {
            cEmployee target = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
            List<cCar> lstCars = new List<cCar>();

            bool blocklicenceexpiry = true;
            bool blocktaxexpiry = false;
            bool blockmotexpiry = false;
            bool blockinsuranceexpiry = false;

            bool expected = false;
            bool actual;
            actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);

            Assert.AreEqual(expected, actual);
            cEmployeeObject.DeleteUTEmployee(target.employeeid, target.username);
        }

        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_passesDutyOfCareTest_trueWhenCheckingTax_noCarsPresent()
        {
            cEmployee target = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
            List<cCar> lstCars = new List<cCar>();

            bool blocklicenceexpiry = false;
            bool blocktaxexpiry = true;
            bool blockmotexpiry = false;
            bool blockinsuranceexpiry = false;

            bool expected = true;
            bool actual;
            actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);

            Assert.AreEqual(expected, actual);
            cEmployeeObject.DeleteUTEmployee(target.employeeid, target.username);
        }

        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_passesDutyOfCareTest_trueWhenCheckingMOT_noCarsPresent()
        {
            cEmployee target = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
            List<cCar> lstCars = new List<cCar>();

            bool blocklicenceexpiry = false;
            bool blocktaxexpiry = false;
            bool blockmotexpiry = true;
            bool blockinsuranceexpiry = false;

            bool expected = true;
            bool actual;
            actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);

            Assert.AreEqual(expected, actual);
            cEmployeeObject.DeleteUTEmployee(target.employeeid, target.username);
        }

        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_passesDutyOfCareTest_trueWhenCheckingInsurance_noCarsPresent()
        {
            cEmployee target = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
            List<cCar> lstCars = new List<cCar>();

            bool blocklicenceexpiry = false;
            bool blocktaxexpiry = false;
            bool blockmotexpiry = false;
            bool blockinsuranceexpiry = true;

            bool expected = true;
            bool actual;
            actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);

            Assert.AreEqual(expected, actual);
            cEmployeeObject.DeleteUTEmployee(target.employeeid, target.username);
        }

        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_passesDutyOfCareTest_trueWhenCheckingLicence_validLicenceDate()
        {
            cEmployee target = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject(licenceExpiryDate: DateTime.UtcNow.AddMonths(1)));
            List<cCar> lstCars = new List<cCar>();

            bool blocklicenceexpiry = true;
            bool blocktaxexpiry = false;
            bool blockmotexpiry = false;
            bool blockinsuranceexpiry = false;

            bool expected = true;
            bool actual;
            actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);

            Assert.AreEqual(expected, actual);
            cEmployeeObject.DeleteUTEmployee(target.employeeid, target.username);
        }

        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_passesDutyOfCareTest_falseWhenCheckingLicence_expiredLicenceDate()
        {
            cEmployee target = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject(licenceExpiryDate: DateTime.UtcNow.AddMonths(-1)));
            List<cCar> lstCars = new List<cCar>();

            bool blocklicenceexpiry = true;
            bool blocktaxexpiry = false;
            bool blockmotexpiry = false;
            bool blockinsuranceexpiry = false;

            bool expected = false;
            bool actual;
            actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);

            Assert.AreEqual(expected, actual);
            cEmployeeObject.DeleteUTEmployee(target.employeeid, target.username);
        }

        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_passesDutyOfCareTest_trueWhenCheckingLicence_validLicenceDate_invalidCarDutyOfCare()
        {
            cEmployee target = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject(licenceExpiryDate: DateTime.UtcNow.AddMonths(1)));
            List<cCar> lstCars = new List<cCar>();
            try
            {
                lstCars.Add(cCarObject.CreateObject(cCarObject.FailsDutyOfCareTemplate()));

                bool blocklicenceexpiry = true;
                bool blocktaxexpiry = false;
                bool blockmotexpiry = false;
                bool blockinsuranceexpiry = false;

                bool expected = true;
                bool actual;
                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);

                Assert.AreEqual(expected, actual);
            }
            finally
            {
                if (lstCars.Count > 0)
                {
                    foreach (cCar car in lstCars)
                    {
                        cCarObject.DeleteCar(car);
                    }
                }
                cEmployeeObject.DeleteUTEmployee(target.employeeid, target.username);
            }
        }

        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_passesDutyOfCareTest_falseWhenCheckingCarDetails_validLicenceDate_invalidCarDutyOfCare()
        {
            cEmployee target = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject(licenceExpiryDate: DateTime.UtcNow.AddMonths(1)));
            List<cCar> lstCars = new List<cCar>();

            try
            {
                lstCars.Add(cCarObject.CreateObject(cCarObject.FailsDutyOfCareTemplate()));

                bool blocklicenceexpiry = false;
                bool blocktaxexpiry = true;
                bool blockmotexpiry = false;
                bool blockinsuranceexpiry = false;

                bool expected = false;
                bool actual;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blocktaxexpiry = false;
                blockmotexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blockmotexpiry = false;
                blockinsuranceexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blocklicenceexpiry = true;
                blocktaxexpiry = true;
                blockmotexpiry = true;
                blockinsuranceexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                if (lstCars.Count > 0)
                {
                    foreach (cCar car in lstCars)
                    {
                        cCarObject.DeleteCar(car);
                    }
                }
                cEmployeeObject.DeleteUTEmployee(target.employeeid, target.username);
            }
        }

        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_passesDutyOfCareTest_trueWhenCheckingCarDetails_validLicenceDate_validCarDutyOfCare()
        {
            cEmployee target = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject(licenceExpiryDate: DateTime.UtcNow.AddMonths(1)));
            List<cCar> lstCars = new List<cCar>();

            try
            {
                lstCars.Add(cCarObject.CreateObject(cCarObject.PassesDutyOfCareTemplate()));

                bool blocklicenceexpiry = false;
                bool blocktaxexpiry = true;
                bool blockmotexpiry = false;
                bool blockinsuranceexpiry = false;

                bool expected = true;
                bool actual;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blocktaxexpiry = false;
                blockmotexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blockmotexpiry = false;
                blockinsuranceexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blocklicenceexpiry = true;
                blocktaxexpiry = true;
                blockmotexpiry = true;
                blockinsuranceexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                if (lstCars.Count > 0)
                {
                    foreach (cCar car in lstCars)
                    {
                        cCarObject.DeleteCar(car);
                    }
                }
                cEmployeeObject.DeleteUTEmployee(target.employeeid, target.username);
            }
        }

        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_passesDutyOfCareTest_trueWhenCheckingCarDetails_validLicenceDate_validCarsDutyOfCare()
        {
            cEmployee target = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject(licenceExpiryDate: DateTime.UtcNow.AddMonths(1)));
            List<cCar> lstCars = new List<cCar>();

            try
            {
                lstCars.Add(cCarObject.CreateObject(cCarObject.PassesDutyOfCareTemplate()));
                lstCars.Add(cCarObject.CreateObject(cCarObject.PassesDutyOfCareTemplate()));

                bool blocklicenceexpiry = false;
                bool blocktaxexpiry = true;
                bool blockmotexpiry = false;
                bool blockinsuranceexpiry = false;

                bool expected = true;
                bool actual;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blocktaxexpiry = false;
                blockmotexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blockmotexpiry = false;
                blockinsuranceexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blocklicenceexpiry = true;
                blocktaxexpiry = true;
                blockmotexpiry = true;
                blockinsuranceexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                if (lstCars.Count > 0)
                {
                    foreach (cCar car in lstCars)
                    {
                        cCarObject.DeleteCar(car);
                    }
                }
                cEmployeeObject.DeleteUTEmployee(target.employeeid, target.username);
            }
        }

        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_passesDutyOfCareTest_falseWhenCheckingCarDetails_validLicenceDate_mixedCarsDutyOfCare()
        {
            cEmployee target = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject(licenceExpiryDate: DateTime.UtcNow.AddMonths(1)));
            List<cCar> lstCars = new List<cCar>();

            try
            {
                lstCars.Add(cCarObject.CreateObject(cCarObject.FailsDutyOfCareTemplate()));
                lstCars.Add(cCarObject.CreateObject(cCarObject.PassesDutyOfCareTemplate()));

                bool blocklicenceexpiry = false;
                bool blocktaxexpiry = true;
                bool blockmotexpiry = false;
                bool blockinsuranceexpiry = false;

                bool expected = false;
                bool actual;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blocktaxexpiry = false;
                blockmotexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blockmotexpiry = false;
                blockinsuranceexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blocklicenceexpiry = true;
                blocktaxexpiry = true;
                blockmotexpiry = true;
                blockinsuranceexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                if (lstCars.Count > 0)
                {
                    foreach (cCar car in lstCars)
                    {
                        cCarObject.DeleteCar(car);
                    }
                }
                cEmployeeObject.DeleteUTEmployee(target.employeeid, target.username);
            }
        }

        /// <summary>
        ///A test for passesDutyOfCare
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_passesDutyOfCareTest_trueWhenCheckingCarDetails_validLicenceDate_invalidInactiveCarDutyOfCare()
        {
            cEmployee target = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject(licenceExpiryDate: DateTime.UtcNow.AddMonths(1)));
            List<cCar> lstCars = new List<cCar>();

            try
            {
                lstCars.Add(cCarObject.CreateObject(cCarObject.FailsDutyOfCareTemplate(active: false)));
                lstCars.Add(cCarObject.CreateObject(cCarObject.PassesDutyOfCareTemplate()));

                bool blocklicenceexpiry = false;
                bool blocktaxexpiry = true;
                bool blockmotexpiry = false;
                bool blockinsuranceexpiry = false;

                bool expected = true;
                bool actual;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blocktaxexpiry = false;
                blockmotexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blockmotexpiry = false;
                blockinsuranceexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);

                blocklicenceexpiry = true;
                blocktaxexpiry = true;
                blockmotexpiry = true;
                blockinsuranceexpiry = true;

                actual = target.passesDutyOfCare(blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, lstCars);
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                if (lstCars.Count > 0)
                {
                    foreach (cCar car in lstCars)
                    {
                        cCarObject.DeleteCar(car);
                    }
                }
                cEmployeeObject.DeleteUTEmployee(target.employeeid, target.username);
            }
        }




        /// <summary>
        ///A test for incrementRefnum
        ///</summary>
        [TestMethod()]
        public void cEmployeeTest_incrementRefnumTest()
        {
            int employeeID = cEmployeeObject.CreateUTEmployee();
            cEmployees clsEmployees = new cEmployees(cGlobalVariables.AccountID);
            cEmployee target = clsEmployees.GetEmployeeById(employeeID); // TODO: Initialize to an appropriate value
            target.incrementRefnum();
            Assert.IsTrue(target.currefnum == 1);
            clsEmployees.deleteEmployee(employeeID);
        }

        /// <summary>
        ///A test for incrementClaimNo
        ///</summary>
        [TestMethod()]
        public void incrementClaimNoTest()
        {
            cEmployee target = new cEmployee(); // TODO: Initialize to an appropriate value
            target.incrementClaimNo();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }


        /// <summary>
        ///A test for deleteUserView
        ///</summary>
        [TestMethod()]
        public void deleteUserViewTest()
        {
            cEmployee target = new cEmployee(); // TODO: Initialize to an appropriate value
            UserView viewtype = new UserView(); // TODO: Initialize to an appropriate value
            target.deleteUserView(viewtype);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for readBroadcast
        ///</summary>
        [TestMethod()]
        public void readBroadcastTest1()
        {
            cEmployee target = new cEmployee(); // TODO: Initialize to an appropriate value
            int broadcastid = 0; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.readBroadcast(broadcastid);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
