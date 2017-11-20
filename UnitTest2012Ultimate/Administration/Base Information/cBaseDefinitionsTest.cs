using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI.WebControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spend_Management;
using SpendManagementLibrary;

namespace UnitTest2012Ultimate
{
    /// <summary>
    ///This is a test class for cBaseDefinitionsTest and is intended
    ///to contain all cBaseDefinitionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cBaseDefinitionsTest
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
        public static void MyClassInitialize(TestContext testContext)
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
        ///A test for GetBaseDefinitionRecord where a record is being added as new so all the base definition values should be empty as the new
        ///base definition has not been added yet
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_GetBaseDefinitionRecordForANewDefinition()
        {
            cBaseDefinitions clsBaseDefs = null;
            cBaseDefinitionValues[] defValues;
            List<Guid> FieldIDs = null;
            List<cNewGridColumn> lstColumns = null;

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    lstColumns = new List<cNewGridColumn>();
                    FieldIDs = new List<Guid>();
                    
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);

                    #region Get the field IDs for the element 

                    List<cField> lstFields = clsBaseDefs.SetBaseDefinitionFields(cBaseDefinitionObject.GetBaseDefTableID(element), ref lstColumns);

                    if(lstFields != null)
                    {
                        foreach(cField field in lstFields)
                        {
                            FieldIDs.Add(field.FieldID);
                        }
                    }

                    #endregion

                    defValues = clsBaseDefs.GetBaseDefinitionRecord(-1, FieldIDs.ToArray());

                    Assert.IsTrue(defValues.Length > 0);

                    foreach (cBaseDefinitionValues val in defValues)
                    {
                        Assert.IsTrue(val.fieldValue == "");
                    }

                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for GetBaseDefinitionRecord where there is an existing record and all the base definition values should match the values of the 
        ///existing base definition object
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_GetBaseDefinitionRecordForAnExistingDefinition()
        {
            cBaseDefinitions clsBaseDefs = null;
            cBaseDefinitionValues[] defValues;
            List<Guid> FieldIDs = null;
            List<cNewGridColumn> lstColumns = null;
            cFields clsFields = new cFields(GlobalTestVariables.AccountId);

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    //Blank list so it can be passed as a parameter by reference to the SetBaseDefinitionFields method
                    lstColumns = new List<cNewGridColumn>();
                    cBaseDefinition nullDef = null;
                    FieldIDs = new List<Guid>();
                    cBaseDefinition def = cBaseDefinitionObject.CreateBaseDefinition(element, ref nullDef);
                    PropertyInfo[] piLst = def.GetType().GetProperties();
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);

                    #region Get the field IDs for the element

                    List<cField> lstFields = clsBaseDefs.SetBaseDefinitionFields(cBaseDefinitionObject.GetBaseDefTableID(element), ref lstColumns);

                    if (lstFields != null)
                    {
                        foreach (cField field in lstFields)
                        {
                            FieldIDs.Add(field.FieldID);
                        }
                    }

                    #endregion

                    defValues = clsBaseDefs.GetBaseDefinitionRecord(cBaseDefinitionObject.GlobalID, FieldIDs.ToArray());

                    Assert.IsTrue(defValues.Length > 0);

                    foreach (cBaseDefinitionValues val in defValues)
                    {
                        cField field = clsFields.GetFieldByID(new Guid(val.fieldID));

                        foreach (PropertyInfo pi in piLst)
                        {
                            if (field.ClassPropertyName == pi.Name)
                            {
                                Assert.AreEqual(pi.GetValue(def, null).ToString(), val.fieldValue);
                            }
                        }
                    }

                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for archiving and unarchiving a base definition
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_ArchiveDefinition()
        {
            cBaseDefinitions clsBaseDefs = null;

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    cBaseDefinition nullDef = null;
                    cBaseDefinition def = cBaseDefinitionObject.CreateBaseDefinition(element, ref nullDef);
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);
                    clsBaseDefs.ArchiveDefinition(def.ID);
                    def = clsBaseDefs.GetDefinitionByID(def.ID);

                    Assert.IsTrue(def.Archived);

                    clsBaseDefs.ArchiveDefinition(def.ID);
                    def = clsBaseDefs.GetDefinitionByID(def.ID);

                    Assert.IsFalse(def.Archived);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for cBaseDefinitions Constructor
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_cBaseDefinitionsConstructorTest()
        {
             cBaseDefinitions clsBaseDefs = null;

             foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
             {
                 clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);
             }
        }

        /// <summary>
        ///A test for CreateDropDown where the none value is populated into the dropdown
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_CreateDropDownWithAddNoneValue()
        {
            cBaseDefinitions clsBaseDefs = null;

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    cBaseDefinition nullDef = null;
                    cBaseDefinition def = cBaseDefinitionObject.CreateBaseDefinition(element, ref nullDef);
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);

                    ListItem[] lstItems = clsBaseDefs.CreateDropDown(true, 0);

                    Assert.IsTrue(lstItems.Length > 0);
                    Assert.IsTrue(lstItems[0].Text == "[None]");
                    // check the base definition dropdown to check that it contains the new element
                    bool foundElement = false;
                    foreach (ListItem li in lstItems)
                    {
                        if (def.Description == li.Text)
                        {
                            foundElement = true;
                            break;
                        }
                    }
                    Assert.IsTrue(foundElement);
                    //Assert.AreEqual(def.Description, lstItems[1].Text);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for CreateDropDown where the none value isn't populated into the dropdown
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_CreateDropDownWithNoNoneValue()
        {
            cBaseDefinitions clsBaseDefs = null;

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    cBaseDefinition nullDef = null;
                    cBaseDefinition def = cBaseDefinitionObject.CreateBaseDefinition(element, ref nullDef);
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);

                    ListItem[] lstItems = clsBaseDefs.CreateDropDown(false, 0);

                    Assert.IsTrue(lstItems.Length > 0);
                    Assert.IsFalse(lstItems[0].Text == "[None]");
                    // check the base definition dropdown to check that it contains the new element
                    bool foundElement = false;
                    foreach (ListItem li in lstItems)
                    {
                        if (def.Description == li.Text)
                        {
                            foundElement = true;
                            break;
                        }
                    }
                    Assert.IsTrue(foundElement);
                    //Assert.AreEqual(def.Description, lstItems[0].Text);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for CreateDropDown where the selected item is archived as this should be in the dropdown if it is the archived value.
        ///Archived values that are not selected should not be contained in the dropdown
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_CreateDropDownWithSelectedItemArchived()
        {
            cBaseDefinitions clsBaseDefs = null;

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    cBaseDefinition nullDef = null;
                    cBaseDefinition def = cBaseDefinitionObject.CreateBaseDefinition(element, ref nullDef);
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);
                    clsBaseDefs.ArchiveDefinition(def.ID);
                    ListItem[] lstItems = clsBaseDefs.CreateDropDown(false, def.ID);

                    Assert.IsTrue(lstItems.Length > 0);
                    // check the base definition dropdown to check that it contains the selected element even though it is archived
                    bool foundElement = false;
                    foreach (ListItem li in lstItems)
                    {
                        if (def.Description == li.Text)
                        {
                            foundElement = true;
                            break;
                        }
                    }
                    Assert.IsTrue(foundElement);
                    //Assert.AreEqual(def.Description, lstItems[0].Text);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for CreateDropDown where the item is archived but not selected as this should not be contained in the dropdown
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_CreateDropDownWithItemArchivedButNotSelected()
        {
            cBaseDefinitions clsBaseDefs = null;

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    cBaseDefinition nullDef = null;
                    cBaseDefinition def = cBaseDefinitionObject.CreateBaseDefinition(element, ref nullDef);
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);
                    clsBaseDefs.ArchiveDefinition(def.ID);
                    ListItem[] lstItems = clsBaseDefs.CreateDropDown(false, 0);

                    // check the base definition dropdown to check that does not contains the new element as it is archived and not selected
                    bool foundElement = false;
                    foreach (ListItem li in lstItems)
                    {
                        if (def.Description == li.Text)
                        {
                            foundElement = true;
                            break;
                        }
                    }
                    Assert.IsFalse(foundElement);
                    //Assert.IsTrue(lstItems.Length == 0);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for DeleteDefinition with a valid base definition
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_DeleteDefinitionWithAValidID()
        {
            cBaseDefinitions clsBaseDefs = null;

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    cBaseDefinition nullDef = null;
                    cBaseDefinition def = cBaseDefinitionObject.CreateBaseDefinition(element, ref nullDef);
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);
                    clsBaseDefs.DeleteDefinition(def.ID);
                    def = clsBaseDefs.GetDefinitionByID(def.ID);

                    Assert.IsNull(def);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for DeleteDefinition with an invalid base definition ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_DeleteDefinitionWithAnInvalidID()
        {
            cBaseDefinitions clsBaseDefs = null;

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    cBaseDefinition nullDef = null;
                    cBaseDefinition def = cBaseDefinitionObject.CreateBaseDefinition(element, ref nullDef);
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);
                    clsBaseDefs.DeleteDefinition(0);
                    
                    def = clsBaseDefs.GetDefinitionByID(def.ID);

                    Assert.IsNotNull(def);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for DeleteDefinition with a valid base definition as a delegate
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_DeleteDefinitionAsADelegate()
        {
            cBaseDefinitions clsBaseDefs = null;
            HelperMethods.SetTestDelegateID();

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    cBaseDefinition nullDef = null;
                    cBaseDefinition def = cBaseDefinitionObject.CreateBaseDefinition(element, ref nullDef);
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);
                    clsBaseDefs.DeleteDefinition(def.ID);
                    def = clsBaseDefs.GetDefinitionByID(def.ID);

                    Assert.IsNull(def);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for GetDefinitionByID with a valid ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_GetDefinitionByIDWithAValidID()
        {
            cBaseDefinitions clsBaseDefs = null;

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    cBaseDefinition nullDef = null;
                    cBaseDefinition def = cBaseDefinitionObject.CreateBaseDefinition(element, ref nullDef);
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);

                    cBaseDefinition actual = clsBaseDefs.GetDefinitionByID(def.ID);

                    Assert.IsNotNull(actual);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for GetDefinitionByID with an invalid ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_GetDefinitionByIDWithAnInvalidID()
        {
            cBaseDefinitions clsBaseDefs = null;

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);

                cBaseDefinition actual = clsBaseDefs.GetDefinitionByID(0);

                Assert.IsNull(actual);
            }
        }

        /// <summary>
        ///A test for adding a new Definition
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_AddDefinition()
        {
            cBaseDefinitions clsBaseDefs = null;
            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    cBaseDefinition expectedDef = null;
                    cBaseDefinition def = cBaseDefinitionObject.CreateBaseDefinition(element, ref expectedDef);
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);

                    cBaseDefinition actual = clsBaseDefs.GetDefinitionByID(def.ID);

                    cCompareAssert.AreEqual(expectedDef, actual, cBaseDefinitionElements.lstOmittedProperties);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for adding a new Definition as a delegate
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_AddDefinitionAsADelegate()
        {
            cBaseDefinitions clsBaseDefs = null;
            HelperMethods.SetTestDelegateID();

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    cBaseDefinition expectedDef = null;                    

                    cBaseDefinition def = cBaseDefinitionObject.CreateBaseDefinition(element, ref expectedDef);
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);

                    cBaseDefinition actual = clsBaseDefs.GetDefinitionByID(def.ID);

                    cCompareAssert.AreEqual(expectedDef, actual, cBaseDefinitionElements.lstOmittedProperties);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for editing a Definition
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_EditDefinition()
        {
            cBaseDefinitions clsBaseDefs = null;

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    cBaseDefinition def = cBaseDefinitionObject.EditBaseDefinition(element);
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);

                    cBaseDefinition actual = clsBaseDefs.GetDefinitionByID(def.ID);

                    cCompareAssert.AreEqual(def, actual, cBaseDefinitionElements.lstOmittedProperties);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for editing a Definition
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_EditDefinitionAsADelegate()
        {
            cBaseDefinitions clsBaseDefs = null;
            HelperMethods.SetTestDelegateID();

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    cBaseDefinition def = cBaseDefinitionObject.EditBaseDefinition(element);

                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);

                    cBaseDefinition actual = clsBaseDefs.GetDefinitionByID(def.ID);

                    cCompareAssert.AreEqual(def, actual, cBaseDefinitionElements.lstOmittedProperties);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for SetBaseDefinitionFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_SetBaseDefinitionFields()
        {
            cBaseDefinitions clsBaseDefs = null;

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);

                    List<cNewGridColumn> columns = new List<cNewGridColumn>();

                    List<cField> bdColumns = clsBaseDefs.SetBaseDefinitionFields(cBaseDefinitionObject.GetBaseDefTableID(element), ref columns);

                    Assert.IsTrue(columns.Count > 0);
                    Assert.IsTrue(bdColumns.Count > 0);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }

        /// <summary>
        ///A test for SetBaseDefinitionFields with an invalid Table ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Base Definitions"), TestMethod()]
        public void cBaseDefinitions_SetBaseDefinitionFieldsWithAnInvalidTableID()
        {
            cBaseDefinitions clsBaseDefs = null;

            foreach (SpendManagementElement element in cBaseDefinitionElements.lstBaseDefinitions)
            {
                try
                {
                    clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountId, GlobalTestVariables.SubAccountId, element);

                    List<cNewGridColumn> columns = new List<cNewGridColumn>();

                    List<cField> bdColumns = clsBaseDefs.SetBaseDefinitionFields(Guid.Empty, ref columns);

                    Assert.IsTrue(columns.Count == 0);
                    Assert.IsTrue(bdColumns.Count == 0);
                }
                finally
                {
                    cBaseDefinitionObject.DeleteBaseDefinition(element);
                }
            }
        }
    }
}
