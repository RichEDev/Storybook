using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using SpendManagementUnitTests.Global_Objects;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cGlobalMimeTypesTest and is intended
    ///to contain all cGlobalMimeTypesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cGlobalMimeTypesTest
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
        ///A test for cGlobalMimeTypes Constructor
        ///</summary>    
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_cGlobalMimeTypesConstructorTest()
        {
            cGlobalMimeTypes target = new cGlobalMimeTypes(cGlobalVariables.AccountID);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for CacheCustomMimeTypeList
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_CacheCustomMimeTypeList()
        {
            cGlobalMimeType gMime = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
                Cache.Remove("customMimeTypes" + cGlobalVariables.AccountID);

                gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();

                SortedList<Guid, cGlobalMimeType> expected = (SortedList<Guid, cGlobalMimeType>)Cache["customMimeTypes" + cGlobalVariables.AccountID];

                Assert.IsNotNull(expected);
                Assert.IsTrue(expected.Count > 0);
                Cache.Remove("customMimeTypes" + cGlobalVariables.AccountID);
            }
            finally
            {
                if (gMime != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(gMime.GlobalMimeID);
                }
            }
        }

        /// <summary>
        ///A test for CacheList
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_CacheList()
        {
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
            Cache.Remove("MimeTypes");

            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);
            SortedList<Guid, cGlobalMimeType> expected = (SortedList<Guid, cGlobalMimeType>)Cache["MimeTypes"];

            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);
            Cache.Remove("MimeTypes");
            
        }

        /// <summary>
        ///A test for DeleteCustomMimeHeader with a valid ID
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_DeleteCustomMimeHeaderWithValidID()
        {
            cGlobalMimeType gMime = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();
                clsGlobalMimeTypes.DeleteCustomMimeHeader(gMime.GlobalMimeID);
                Assert.IsNull(clsGlobalMimeTypes.getMimeTypeById(gMime.GlobalMimeID));
            }
            finally
            {
                if (gMime != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(gMime.GlobalMimeID);
                }
            }
        }

        /// <summary>
        ///A test for DeleteCustomMimeHeader with an invalid ID
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_DeleteCustomMimeHeaderWithInvalidID()
        {
            cGlobalMimeType gMime = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();
                clsGlobalMimeTypes.DeleteCustomMimeHeader(Guid.Empty);
                Assert.IsNotNull(clsGlobalMimeTypes.getMimeTypeById(gMime.GlobalMimeID));
            }
            finally
            {
                if (gMime != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(gMime.GlobalMimeID);
                }
            }
        }

        /// <summary>
        ///A test for DeleteCustomMimeHeader with a valid ID as a delegate
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_DeleteCustomMimeHeaderWithValidIDASADelegate()
        {
            cGlobalMimeType gMime = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                //Set the delegate for the current user
                cEmployeeObject.CreateUTDelegateEmployee();
                System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

                gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();
                clsGlobalMimeTypes.DeleteCustomMimeHeader(gMime.GlobalMimeID);
                Assert.IsNull(clsGlobalMimeTypes.getMimeTypeById(gMime.GlobalMimeID));
            }
            finally
            {
                if (gMime != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(gMime.GlobalMimeID);
                    System.Web.HttpContext.Current.Session["myid"] = null;
                }
            }
        }

        /// <summary>
        ///A test for GetGlobalMimeTypes
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_GetGlobalMimeTypes()
        {
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);
            SortedList<Guid, cGlobalMimeType> globalList = clsGlobalMimeTypes.GetGlobalMimeTypes();
            Assert.IsNotNull(globalList);
            Assert.IsTrue(globalList.Count > 0);
        }

        /// <summary>
        ///A test for adding a Custom Mime Header with all values set
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_AddCustomMimeHeaderWithAllValuesSet()
        {
            cGlobalMimeType actual = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                cGlobalMimeType expected = cGlobalMimeTypeObject.GetGlobalMimeType();
                clsGlobalMimeTypes.SaveCustomMimeHeader(expected);
                actual = clsGlobalMimeTypes.getMimeTypeByExtension(expected.FileExtension);
                Assert.IsNotNull(actual);
                List<string> lstProps = new List<string>();
                lstProps.Add("GlobalMimeID");
                cCompareAssert.AreEqual(expected, actual, lstProps);
            }
            finally
            {
                if (actual != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(actual.GlobalMimeID);
                }
            }
        }

        /// <summary>
        ///A test for adding a Custom Mime Header with all values set to null or nothing
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_AddCustomMimeHeaderWithValuesSetToNullOrNothingThatCanBe()
        {
            cGlobalMimeType actual = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                cGlobalMimeType expected = cGlobalMimeTypeObject.GetGlobalMimeTypeWithValuesSetToNullOrNothingThatCanBe();
                clsGlobalMimeTypes.SaveCustomMimeHeader(expected);
                actual = clsGlobalMimeTypes.getMimeTypeByExtension(expected.FileExtension);
                Assert.IsNotNull(actual);
                List<string> lstProps = new List<string>();
                lstProps.Add("GlobalMimeID");
                cCompareAssert.AreEqual(expected, actual, lstProps);
            }
            finally
            {
                if (actual != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(actual.GlobalMimeID);
                }
            }
        }

        /// <summary>
        ///A test for adding a Custom Mime Header with all values set as a delegate
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_AddCustomMimeHeaderWithAllValuesSetAsADelegate()
        {
            cGlobalMimeType actual = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                //Set the delegate for the current user
                cEmployeeObject.CreateUTDelegateEmployee();
                System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

                cGlobalMimeType expected = cGlobalMimeTypeObject.GetGlobalMimeType();
                clsGlobalMimeTypes.SaveCustomMimeHeader(expected);
                actual = clsGlobalMimeTypes.getMimeTypeByExtension(expected.FileExtension);
                Assert.IsNotNull(actual);
                List<string> lstProps = new List<string>();
                lstProps.Add("GlobalMimeID");
                cCompareAssert.AreEqual(expected, actual, lstProps);
            }
            finally
            {
                if (actual != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(actual.GlobalMimeID);
                    System.Web.HttpContext.Current.Session["myid"] = null;
                }
            }
        }

        /// <summary>
        ///A test for editing a Custom Mime Header with all values set
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_EditCustomMimeHeaderWithAllValuesSet()
        {
            cGlobalMimeType actual = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                cGlobalMimeType expected = cGlobalMimeTypeObject.CreateGlobalMimeType();
                expected.MimeHeader = "Edited";
                expected.FileExtension = "Edit" + DateTime.Now.Ticks.ToString().Substring(0, 16);
                expected.Description = "Edited";
                clsGlobalMimeTypes.SaveCustomMimeHeader(expected);
                actual = clsGlobalMimeTypes.getMimeTypeById(expected.GlobalMimeID);
                Assert.IsNotNull(actual);
                cCompareAssert.AreEqual(expected, actual);
            }
            finally
            {
                if (actual != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(actual.GlobalMimeID);
                }
            }
        }

        /// <summary>
        ///A test for editing a Custom Mime Header with all values set to null or nothing
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_EditCustomMimeHeaderWithValuesSetToNullOrNothingThatCanBe()
        {
            cGlobalMimeType actual = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                cGlobalMimeType expected = cGlobalMimeTypeObject.CreateGlobalMimeTypeWithValuesSetToNullOrNothingThatCanBe();
                expected.MimeHeader = "Edited";
                expected.FileExtension = "Edit" + DateTime.Now.Ticks.ToString().Substring(0, 16);
                clsGlobalMimeTypes.SaveCustomMimeHeader(expected);
                actual = clsGlobalMimeTypes.getMimeTypeById(expected.GlobalMimeID);
                Assert.IsNotNull(actual);
                cCompareAssert.AreEqual(expected, actual);
            }
            finally
            {
                if (actual != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(actual.GlobalMimeID);
                }
            }
        }

        /// <summary>
        ///A test for editing a Custom Mime Header with all values set as a delegate
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_EditCustomMimeHeaderWithAllValuesSetAsADelegate()
        {
            cGlobalMimeType actual = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                //Set the delegate for the current user
                cEmployeeObject.CreateUTDelegateEmployee();
                System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

                cGlobalMimeType expected = cGlobalMimeTypeObject.CreateGlobalMimeType();
                expected.MimeHeader = "Edited";
                expected.FileExtension = "Edit" + DateTime.Now.Ticks.ToString().Substring(0, 16);
                expected.Description = "Edited";
                clsGlobalMimeTypes.SaveCustomMimeHeader(expected);
                actual = clsGlobalMimeTypes.getMimeTypeById(expected.GlobalMimeID);
                Assert.IsNotNull(actual);
                cCompareAssert.AreEqual(expected, actual);
            }
            finally
            {
                if (actual != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(actual.GlobalMimeID);
                    System.Web.HttpContext.Current.Session["myid"] = null;
                }
            }
        }

        /// <summary>
        ///A test for getMimeTypeByExtension with a valid extension
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_getMimeTypeByExtensionWithValidExtension()
        {
            cGlobalMimeType gMime = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();
                clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);
                cGlobalMimeType actual = clsGlobalMimeTypes.getMimeTypeByExtension(gMime.FileExtension);
                Assert.IsNotNull(actual);
            }
            finally
            {
                if (gMime != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(gMime.GlobalMimeID);
                }
            }
        }

        /// <summary>
        ///A test for getMimeTypeByExtension with an invalid extension
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_getMimeTypeByExtensionWithInvalidExtension()
        {
            cGlobalMimeType gMime = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();
                Assert.IsNull(clsGlobalMimeTypes.getMimeTypeByExtension("None"));
            }
            finally
            {
                if (gMime != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(gMime.GlobalMimeID);
                }
            }
        }

        /// <summary>
        ///A test for getMimeTypeById with a valid ID
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_getMimeTypeByIdWithValidID()
        {
            cGlobalMimeType gMime = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();
                clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);
                cGlobalMimeType actual = clsGlobalMimeTypes.getMimeTypeById(gMime.GlobalMimeID);
                Assert.IsNotNull(actual);
            }
            finally
            {
                if (gMime != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(gMime.GlobalMimeID);
                }
            }
        }

        /// <summary>
        ///A test for getMimeTypeById with an invalid ID
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cGlobalMimeTypes_getMimeTypeByIdWithInvalidID()
        {
            cGlobalMimeType gMime = null;
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);

            try
            {
                gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();
                Assert.IsNull(clsGlobalMimeTypes.getMimeTypeById(Guid.Empty));
            }
            finally
            {
                if (gMime != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(gMime.GlobalMimeID);
                }
            }
        }
    }
}
