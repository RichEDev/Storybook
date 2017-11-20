namespace UnitTest2012Ultimate
{
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Interfaces;

    using Spend_Management;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using SpendManagementLibrary;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using System.Reflection;
    using SpendManagementLibrary.Enumerators;

    /// <summary>
    ///This is a test class for cAccountSubAccountsTest and is intended
    ///to contain all cAccountSubAccountsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cAccountSubAccountsTest
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
            cSubAccountObject.DeleteSubAccount();
            GlobalAsax.Application_End();
        }

        [TestInitialize()]
        public void MyTestInitialise()
        {
            cSubAccountObject.CreateSubAccount();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            cSubAccountObject.DeleteSubAccount();
        }

        #endregion


        /// <summary>
        ///A test for SaveAccountProperties
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Sub Accounts"), TestMethod()]
        public void cAccountSubAccountsTest_SaveAccountPropertiesTest()
        {
            int accountId = GlobalTestVariables.AccountId;
            ICurrentUser currentUser = Moqs.CurrentUser();

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subaccount = target.getSubAccountById(cSubAccountObject.AlternateSubAccountID);

            cAccountProperties safeProperties = subaccount.SubAccountProperties.Clone();
            cAccountProperties subAccountProperties = subaccount.SubAccountProperties.Clone();
            int EmployeeID = GlobalTestVariables.EmployeeId;

            #region update values using reflection
            Type t = subAccountProperties.GetType();

            object oldVal;
            object newVal;
            string uniq;

            foreach (PropertyInfo pi in t.GetProperties())
            {
                oldVal = pi.GetValue(safeProperties, null);
                newVal = null;
                uniq = DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString();

                // DON'T CHANGE THE SUBACCOUNT!!
                if (pi.Name == "SubAccountID")
                {
                    continue;
                }


                // Should be an entry here for each type used in a property to create a value different to the one in the standard/current properties
                if (pi.PropertyType == typeof(Boolean))
                {
                    newVal = !((Boolean)oldVal);
                }
                else if (pi.PropertyType == typeof(Int16))
                {
                    newVal = (Int16)(((Int16)oldVal) + (Int16)1);
                }
                else if (pi.PropertyType == typeof(Int16?))
                {
                    if (((Int16?)oldVal).HasValue)
                    {
                        newVal = null;
                    }
                    else
                    {
                        newVal = (Int16)1;
                    }
                }
                else if (pi.PropertyType == typeof(Int32))
                {
                    newVal = (Int32)(((Int32)oldVal) + (Int32)1);
                }
                else if (pi.PropertyType == typeof(Int32?))
                {
                    if (((Int32?)oldVal).HasValue)
                    {
                        newVal = null;
                    }
                    else
                    {
                        newVal = (Int32)1;
                    }
                }
                else if (pi.PropertyType == typeof(String))
                {
                    newVal = (String)oldVal + (String)uniq;
                }
                else if (pi.PropertyType.BaseType == typeof(Enum))
                {
                    newVal = null;
                    Array tmpEnumVals = pi.PropertyType.GetEnumValues(); // get a list of the enum integer values as object array from the type
                    foreach (object o in tmpEnumVals)
                    {
                        if ((int)o != (int)oldVal)
                        {
                            newVal = Enum.ToObject(pi.PropertyType, (int)o);
                        }
                    }
                }
                else if (pi.PropertyType == typeof(DateTime))
                {
                    newVal = DateTime.UtcNow;
                }
                else if (pi.PropertyType == typeof(DateTime?))
                {
                    newVal = DateTime.UtcNow;
                }
                else if (pi.PropertyType == typeof(cRechargeSetting))
                {
                    newVal = null;
                }
                else if (pi.PropertyType == typeof(Decimal))
                {
                    newVal = (Decimal)(((Decimal)oldVal) + (Decimal)0.1);
                }
                else if (pi.PropertyType == typeof(Byte))
                {
                    newVal = ((Byte)oldVal > (Byte)0) ? (Byte)(((Byte)oldVal) - (Byte)1) : (Byte)1;
                }
                else if (pi.PropertyType == typeof(Guid))
                {
                    newVal = Guid.NewGuid();
                }
                else if (pi.PropertyType == typeof(Guid?))
                {
                    newVal = Guid.NewGuid();
                }
                else if (pi.PropertyType == typeof(IOwnership))
                {
                    newVal = new Employee();
                }
                else if (pi.PropertyType == typeof(IncludeEsrDetails))
                {
                    newVal = (byte)IncludeEsrDetails.PayPoint;
                }
                else
                {
                    // let us know we've forgotten a type
                    throw new Exception("Unhandled Type = Name:" + pi.PropertyType.Name + "; BaseType:" + pi.PropertyType.BaseType + "; FullName:" + pi.PropertyType.FullName + ";");
                }

                pi.SetValue(subAccountProperties, newVal, null);
            }
            #endregion

            target.SaveAccountProperties(subAccountProperties, EmployeeID, currentUser.Delegate != null ? currentUser.Delegate.EmployeeID : (int?)null);
            target.InvalidateCache(cSubAccountObject.AlternateSubAccountID);

            cAccountSubAccounts new_subaccs = new cAccountSubAccounts(accountId);
            cAccountProperties newproperties = new_subaccs.getSubAccountById(cSubAccountObject.AlternateSubAccountID).SubAccountProperties.Clone();
            newproperties.AccountID++;
            cCompareAssert.AreNothingEqual(safeProperties, newproperties, new List<string>() { "SubAccountID", "NumRows", "ThresholdType", "RechargeSettings", "EnableNotesUpdate", "MigrateUF", "EnableContractSavings" });
            // reset properties back to original values
            target.SaveAccountProperties(safeProperties, EmployeeID, currentUser.Delegate != null ? currentUser.Delegate.EmployeeID : (int?)null);
            target.InvalidateCache(safeProperties.SubAccountID);
        }

        /// <summary>
        ///A test for CreateDropDown
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Sub Accounts"), TestMethod()]
        public void cAccountSubAccountsTest_CreateDropDownTest()
        {
            int accountId = GlobalTestVariables.AccountId;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subacc = target.getSubAccountById(cSubAccountObject.AlternateSubAccountID);

            ListItem[] actual = target.CreateDropDown(cSubAccountObject.AlternateSubAccountID);
            Assert.IsTrue(actual.Length > 0);

            int cnt = 0;
            foreach (ListItem li in actual)
            {
                if (li.Value == subacc.SubAccountID.ToString())
                {
                    cnt++;
                }
            }

            Assert.IsTrue(cnt == 1);
        }

        /// <summary>
        ///A test for cAccountSubAccounts Constructor
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Sub Accounts"), TestMethod()]
        public void cAccountSubAccountsTest_cAccountSubAccountsConstructorTest()
        {
            int accountId = GlobalTestVariables.AccountId;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subacc = target.getSubAccountById(cSubAccountObject.AlternateSubAccountID);

            Assert.IsNotNull(target);
            Assert.IsTrue(target.Count > 0);
            Assert.IsNotNull(subacc);
        }

        /// <summary>
        ///A test for CreateFilteredDropDown
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Sub Accounts"), TestMethod()]
        public void cAccountSubAccountsTest_CreateFilteredDropDownTest()
        {
            int accountId = GlobalTestVariables.AccountId;
            int subAccountId = GlobalTestVariables.SubAccountId;
            cSubAccountObject.GrantAccessRole(subAccountId);

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subacc = target.getSubAccountById(subAccountId);

            int employeeId = GlobalTestVariables.EmployeeId;
            var employee = new cEmployees(accountId).GetEmployeeById(employeeId);

            ListItem[] actual;
            actual = target.CreateFilteredDropDown(employee, subAccountId);

            cSubAccountObject.CleanupAccessRoles();

            Assert.IsNotNull(actual);

            int cnt = 0;
            foreach (ListItem li in actual)
            {
                if (li.Value == subAccountId.ToString())
                {
                    cnt++;
                }
            }

            Assert.IsTrue(cnt == 1);
        }

        /// <summary>
        ///A test for SaveProperties
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Sub Accounts"), TestMethod()]
        public void cAccountSubAccountsTest_SavePropertiesTest()
        {
            int accountId = GlobalTestVariables.AccountId;
            int SubAccountID = cSubAccountObject.AlternateSubAccountID;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subacc = target.getSubAccountById(SubAccountID);

            cAccountProperties clsproperties = target.getSubAccountById(SubAccountID).SubAccountProperties;

            Dictionary<string, string> properties = new Dictionary<string, string>();
            bool safeAMAC = clsproperties.AllowMenuContractAdd;
            string safeSPT = clsproperties.SupplierPrimaryTitle;
            bool newAMAC = !clsproperties.AllowMenuContractAdd;
            string newSPT = "** Supplier **";

            properties.Add("allowMenuAddContract", (newAMAC ? "1" : "0"));
            properties.Add("supplierPrimaryTitle", newSPT);

            int modifiedBy = GlobalTestVariables.EmployeeId;
            target.SaveProperties(SubAccountID, properties, modifiedBy, null);
            target.InvalidateCache(SubAccountID);

            target = new cAccountSubAccounts(accountId);
            clsproperties = target.getSubAccountById(SubAccountID).SubAccountProperties;

            Assert.AreEqual(clsproperties.AllowMenuContractAdd, newAMAC);
            Assert.AreEqual(clsproperties.SupplierPrimaryTitle, newSPT);
        }

        /// <summary>
        ///A test for getFirstSubAccount
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Sub Accounts"), TestMethod()]
        public void cAccountSubAccountsTest_getFirstSubAccountTest()
        {
            int accountId = GlobalTestVariables.AccountId;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subacc = target.getSubAccountById(cSubAccountObject.AlternateSubAccountID);

            cAccountSubAccount actual;
            actual = target.getFirstSubAccount();

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for getPropertiesModified
        ///</summary>
        //[TestCategory("Spend Management"), TestCategory("Sub Accounts"), TestMethod()] // this method does not appear to be used anywhere
        //public void cAccountSubAccountsTest_getPropertiesModifiedTest()
        //{
        //    int accountId = GlobalTestVariables.AccountID;
        //    int modifiedBy = GlobalTestVariables.EmployeeID;
        //    int alternateSubAccountID = cSubAccountObject.AlternateSubAccountID;

        //    cAccountSubAccounts target = new cAccountSubAccounts(accountId);
        //    cAccountSubAccount subacc = target.getSubAccountById(alternateSubAccountID);

        //    cAccountProperties clsProperties = target.getSubAccountById(alternateSubAccountID).SubAccountProperties.Clone();

        //    DateTime modifiedSince = DateTime.UtcNow;

        //    // create a new list of items changed from the existing
        //    Dictionary<string, string> lstChangedProperties = new Dictionary<string, string>();
        //    bool newBTE = !clsProperties.BlockTaxExpiry; // opposite to existing
        //    string newSPT = clsProperties.SupplierPrimaryTitle + "** Supplier **";
        //    // add them to the list to be saved
        //    lstChangedProperties.Add("blockTaxExpiry", (newBTE ? "1" : "0"));
        //    lstChangedProperties.Add("supplierPrimaryTitle", newSPT);

        //    // save the new properties
        //    target.SaveProperties(alternateSubAccountID, lstChangedProperties, modifiedBy);

        //    // new balls please
        //    target = new cAccountSubAccounts(accountId);

        //    // get the list of chaged properties
        //    DateTime beforeGet = DateTime.UtcNow;
        //    Dictionary<string, string> actual = target.getPropertiesModified(modifiedSince, alternateSubAccountID);

        //    // check they contain what we were expecting
        //    Assert.IsNotNull(actual);
        //    Assert.IsTrue(actual.Count == 2, "AlternateSubAccountID: " + cSubAccountObject.AlternateSubAccountID + " - alternateSubAccountID: " + alternateSubAccountID + " - Actual Count: " + actual.Count + " - Start Time: " + modifiedSince.ToString(CultureInfo.InvariantCulture) + "." + modifiedSince.Millisecond.ToString(CultureInfo.InvariantCulture) + " - End Time: " + beforeGet.ToString(CultureInfo.InvariantCulture) + "." + beforeGet.Millisecond.ToString(CultureInfo.InvariantCulture));
        //    Assert.IsTrue(actual.ContainsKey("blockTaxExpiry"));
        //    Assert.IsTrue(actual.ContainsKey("supplierPrimaryTitle"));
        //}

        /// <summary>
        ///A test for getSubAccountById
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Sub Accounts"), TestMethod()]
        public void cAccountSubAccountsTest_getSubAccountByIdTest()
        {
            int accountId = GlobalTestVariables.AccountId;
            cAccountSubAccounts target = new cAccountSubAccounts(accountId);

            cAccountSubAccount actual;
            actual = target.getSubAccountById(cSubAccountObject.AlternateSubAccountID);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for getSubAccountsCollection
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Sub Accounts"), TestMethod()]
        public void cAccountSubAccountsTest_getSubAccountsCollectionTest()
        {
            int accountId = GlobalTestVariables.AccountId;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            SortedList<int, cAccountSubAccount> actual;
            actual = target.getSubAccountsCollection();

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        ///A test for InvalidateCache
        ///</summary>
		[TestCategory("Spend Management"), TestCategory("Sub Accounts"), TestMethod()]
		public void cAccountSubAccountsTest_InvalidateCacheTest()
        {
        	Caching cache = new Caching();
        	int accountId = GlobalTestVariables.AccountId;

        	cAccountSubAccounts target = new cAccountSubAccounts(accountId);

        	SortedList<int, cAccountSubAccount> tmpList =
        		(SortedList<int, cAccountSubAccount>) cache.Cache.Get("accountsubaccounts_" + accountId.ToString());

        	Assert.IsNotNull(tmpList);

        	target.InvalidateCache(cSubAccountObject.AlternateSubAccountID);

        	tmpList = (SortedList<int, cAccountSubAccount>) cache.Cache.Get("accountsubaccounts_" + accountId.ToString());

        	Assert.IsNull(tmpList);
        }

    	/// <summary>
        ///A test for DeleteSubAccount
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Sub Accounts"), TestMethod()]
        public void cAccountSubAccountsTest_DeleteSubAccountTest()
        {
            int accountId = GlobalTestVariables.AccountId;
            int subAccountId = cSubAccountObject.AlternateSubAccountID;
            int employeeId = GlobalTestVariables.EmployeeId;
            cAccountSubAccounts target = new cAccountSubAccounts(accountId);

            Assert.AreNotEqual(subAccountId, -1);

            cAccountSubAccount subAccount = target.getSubAccountById(subAccountId);//cSubAccountObject.CreateSubAccount(); this is done in the pre-test setup

            Assert.IsNotNull(subAccount);
            Assert.AreEqual(cSubAccountObject.AlternateSubAccountID, subAccount.SubAccountID);

            int actual = target.DeleteSubAccount(subAccountId, employeeId);

            target = new cAccountSubAccounts(accountId);

            Assert.IsNull(target.getSubAccountById(subAccountId));
        }

        /// <summary>
        ///A test for UpdateSubAccount
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Sub Accounts"), TestMethod()]
        public void cAccountSubAccountsTest_UpdateSubAccountTest()
        {
            int accountId = GlobalTestVariables.AccountId;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subacc = target.getSubAccountById(cSubAccountObject.AlternateSubAccountID);

            cAccountSubAccount altered_subacc = new cAccountSubAccount(subacc.SubAccountID, "Test " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), subacc.IsArchived, subacc.SubAccountProperties, subacc.CreatedOn, subacc.CreatedBy, DateTime.UtcNow, GlobalTestVariables.EmployeeId);
            target.UpdateSubAccount(altered_subacc, GlobalTestVariables.EmployeeId, 0, GlobalTestVariables.AccountId, subacc.SubAccountID);

            target = new cAccountSubAccounts(accountId);
            subacc = target.getSubAccountById(cSubAccountObject.AlternateSubAccountID);

            Assert.IsNotNull(subacc);
            Assert.IsTrue(subacc.SubAccountID == cSubAccountObject.AlternateSubAccountID);
            Assert.AreEqual(subacc.Description, altered_subacc.Description);
        }
    }
}
