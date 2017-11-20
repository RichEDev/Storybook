using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SpendManagementLibrary;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace UnitTest2012Ultimate
{
    /// <summary>
    ///This is a test class for cMimeTypesTest and is intended
    ///to contain all cMimeTypesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cMimeTypesTest
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

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext context)
        {
            GlobalAsax.Application_Start();
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        [TestCleanup()]
        public void MyTestCleanUp()
        {
            HelperMethods.ClearTestDelegateID();
        }

        #endregion


        /// <summary>
        ///A test for cMimeTypes Constructor
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Mime Types"), TestMethod()]
        public void cMimeTypes_cMimeTypesConstructorTest()
        {
            cMimeTypes target = new cMimeTypes(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for ArchiveMimeType
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Mime Types"), TestMethod()]
        public void cMimeTypes_ArchiveMimeType()
        {
            cMimeType mime = null;

            try
            {
                mime = cMimeTypeObject.CreateMimeType();
                cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
                clsMimeTypes.ArchiveMimeType(mime.MimeID);

                mime = clsMimeTypes.GetMimeTypeByID(mime.MimeID);
                Assert.IsTrue(mime.Archived);

                clsMimeTypes.ArchiveMimeType(mime.MimeID);
                mime = clsMimeTypes.GetMimeTypeByID(mime.MimeID);
                Assert.IsFalse(mime.Archived);
            }
            finally
            {
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }

        /// <summary>
        ///A test for CacheList
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Mime Types"), TestMethod()]
        public void cMimeTypes_CacheListTest()
        {
            cMimeType mime = null;
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;

            try
            {
                mime = cMimeTypeObject.CreateMimeType();
                Cache.Remove("MimeTypes" + GlobalTestVariables.AccountId);

                cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
                SortedList<int, cMimeType> expected = (SortedList<int, cMimeType>)Cache["MimeTypes" + GlobalTestVariables.AccountId];

                Assert.IsNotNull(expected);
                Assert.IsTrue(expected.Count > 0);
            }
            finally
            {
                Cache.Remove("MimeTypes" + GlobalTestVariables.AccountId);
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }

        /// <summary>
        ///A test for CreateMimeTypeDropdown
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Mime Types"), TestMethod()]
        public void cMimeTypes_CreateMimeTypeDropdown()
        {
            cMimeType mime = null;

            try
            {
                mime = cMimeTypeObject.CreateMimeType();
                cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
                ListItem[] lstItems = clsMimeTypes.CreateMimeTypeDropdown();

                Assert.IsTrue(lstItems.Length > 0);
            }
            finally
            {
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }

        /// <summary>
        ///A test for DeleteMimeType with a valid ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Mime Types"), TestMethod()]
        public void cMimeTypes_DeleteMimeTypeWithValidID()
        {
            cMimeType mime = null;

            try
            {
                mime = cMimeTypeObject.CreateMimeType();
                cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
                clsMimeTypes.DeleteMimeType(mime.MimeID);

                Assert.IsNull(clsMimeTypes.GetMimeTypeByID(mime.MimeID));
            }
            finally
            {
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }

        /// <summary>
        ///A test for DeleteMimeType with an invalid ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Mime Types"), TestMethod()]
        public void cMimeTypes_DeleteMimeTypeWithInvalidID()
        {
            cMimeType mime = null;

            try
            {
                mime = cMimeTypeObject.CreateMimeType();
                cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
                clsMimeTypes.DeleteMimeType(0);

                Assert.IsNotNull(clsMimeTypes.GetMimeTypeByID(mime.MimeID));
            }
            finally
            {
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }

        /// <summary>
        ///A test for DeleteMimeType with a valid ID as a delegate
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Mime Types"), TestMethod()]
        public void cMimeTypes_DeleteMimeTypeWithValidIDAsADelegate()
        {
            cMimeType mime = null;
            HelperMethods.SetTestDelegateID();

            try
            {

                mime = cMimeTypeObject.CreateMimeType();
                cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
                clsMimeTypes.DeleteMimeType(mime.MimeID);

                Assert.IsNull(clsMimeTypes.GetMimeTypeByID(mime.MimeID));
            }
            finally
            {
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }

        /// <summary>
        ///A test for GetMimeTypeByGlobalID with valid ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Mime Types"), TestMethod()]
        public void cMimeTypes_GetMimeTypeByGlobalIDWithValidID()
        {
            cMimeType mime = null;

            try
            {
                mime = cMimeTypeObject.CreateMimeType();
                cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
                cMimeType actual = clsMimeTypes.GetMimeTypeByGlobalID(mime.GlobalMimeID);

                Assert.IsNotNull(actual);
            }
            finally
            {
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }

        /// <summary>
        ///A test for GetMimeTypeByGlobalID with invalid ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Mime Types"), TestMethod()]
        public void cMimeTypes_GetMimeTypeByGlobalIDWithInvalidID()
        {
            cMimeType mime = null;

            try
            {
                mime = cMimeTypeObject.CreateMimeType();
                cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
                cMimeType actual = clsMimeTypes.GetMimeTypeByGlobalID(Guid.Empty);

                Assert.IsNull(actual);
            }
            finally
            {
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }

        /// <summary>
        ///A test for GetMimeTypeByID with valid ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Mime Types"), TestMethod()]
        public void cMimeTypes_GetMimeTypeByIDWithValidID()
        {
            cMimeType mime = null;

            try
            {
                mime = cMimeTypeObject.CreateMimeType();
                cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
                cMimeType actual = clsMimeTypes.GetMimeTypeByID(mime.MimeID);

                Assert.IsNotNull(actual);
            }
            finally
            {
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }

        /// <summary>
        ///A test for GetMimeTypeByID with invalid ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Mime Types"), TestMethod()]
        public void cMimeTypes_GetMimeTypeByIDWithInvalidID()
        {
            cMimeType mime = null;

            try
            {
                mime = cMimeTypeObject.CreateMimeType();
                cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
                cMimeType actual = clsMimeTypes.GetMimeTypeByID(0);

                Assert.IsNull(actual);
            }
            finally
            {
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }

        /// <summary>
        ///A test for SaveMimeType
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Mime Types"), TestMethod()]
        public void cMimeTypes_SaveMimeType()
        {
            cMimeType mime = null;

            try
            {
                cGlobalMimeType gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();

                cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
                int ID = clsMimeTypes.SaveMimeType(gMime.GlobalMimeID);

                mime = clsMimeTypes.GetMimeTypeByID(ID);

                Assert.IsNotNull(mime);
            }
            finally
            {
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }

        /// <summary>
        ///A test for SaveMimeType as a delgate
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Mime Types"), TestMethod()]
        public void cMimeTypes_SaveMimeTypeAsADelegate()
        {
            cMimeType mime = null;
            HelperMethods.SetTestDelegateID();

            try
            {
                cGlobalMimeType gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();

                cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId);
                int ID = clsMimeTypes.SaveMimeType(gMime.GlobalMimeID);

                mime = clsMimeTypes.GetMimeTypeByID(ID);

                Assert.IsNotNull(mime);
            }
            finally
            {
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }
    }
}
