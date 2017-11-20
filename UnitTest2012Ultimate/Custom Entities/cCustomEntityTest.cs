using System.IO;


namespace UnitTest2012Ultimate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Web.UI.WebControls;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary;

    using Spend_Management;

    /// <summary>
    ///This is a test class for cCustomEntityTest and is intended
    ///to contain all cCustomEntityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cCustomEntityTests
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #region GetTabByName

        /// <summary>
        ///A test for getTabByName using a valid tab name
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getTabByName_UsingValidTabName()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));

            try
            {
                #region Create required form with tab, section, field and form attribute

                SortedList<int, cCustomEntityFormTab> tabList = new SortedList<int, cCustomEntityFormTab>();
                cCustomEntityFormTab reqTab = cCustomEntityFormTabObject.Template();
                tabList.Add(0, reqTab);

                SortedList<int, cCustomEntityFormSection> sectionList = new SortedList<int, cCustomEntityFormSection>();
                cCustomEntityFormSection reqSection = cCustomEntityFormSectionObject.Template(tab: reqTab);
                sectionList.Add(0, reqSection);

                cAttribute reqAttribute = cNumberAttributeObject.Template(createdBy: EmployeeID);

                SortedList<int, cCustomEntityFormField> fieldList = new SortedList<int, cCustomEntityFormField>();
                cCustomEntityFormField reqField = cCustomEntityFormFieldObject.Template(section: reqSection, attribute: reqAttribute);
                fieldList.Add(0, reqField);

                SortedList<int, cCustomEntityForm> formList = new SortedList<int, cCustomEntityForm>();
                cCustomEntityForm reqForm = cCustomEntityFormObject.Template(createdBy: EmployeeID, tabs: tabList, sections: sectionList, fields: fieldList);

                #endregion

                int nCustomEntityID = reqCustomEntity.entityid;

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());            

                int formID = customEntities.saveForm(nCustomEntityID, reqForm);
                formList.Add(formID, reqForm);

                reqCustomEntity = cCustomEntityObject.Template(entityID: nCustomEntityID, createdBy: EmployeeID, forms: formList);

                cCustomEntityFormTab actual = reqForm.getTabByName(reqTab.headercaption);

                cCompareAssert.AreEqual(reqTab, actual);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getTabByName_UsingValidTabNameMocked()
        {
            var tabs = new SortedList<int, cCustomEntityFormTab>();
            tabs.Add(1, new cCustomEntityFormTab(1,1,"caption", 0));
            tabs.Add(2, new cCustomEntityFormTab(2,1,"caption2", 1));
            var form = new cCustomEntityForm(
                1,
                1,
                "",
                "",
                false,
                "",
                false,
                "",
                false,
                "",
                false,
                "",
                false,
                false,
                DateTime.Now,
                0,
                null,
                null,
                tabs,
                new SortedList<int, cCustomEntityFormSection>(),
                new SortedList<int, cCustomEntityFormField>(), 
                "", 
                false);
            var result = form.getTabByName("caption2");
            Assert.IsTrue(result.tabid == 2, result.tabid.ToString());
        }
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getTabByName_UsingIncorrectTabNameMocked()
        {
            var tabs = new SortedList<int, cCustomEntityFormTab>();
            tabs.Add(1, new cCustomEntityFormTab(1, 1, "caption", 0));
            tabs.Add(2, new cCustomEntityFormTab(2, 1, "caption2", 1));
            var form = new cCustomEntityForm(
                1,
                1,
                "",
                "",
                false,
                "",
                false,
                "",
                false,
                "",
                false,
                "",
                false,
                false,
                DateTime.Now,
                0,
                null,
                null,
                tabs,
                new SortedList<int, cCustomEntityFormSection>(),
                new SortedList<int, cCustomEntityFormField>(),
                "",
                false);
            var result = form.getTabByName("incorrect");
            Assert.IsTrue(result == null);
        }

        

        /// <summary>
        ///A test for getTabByName using an incorrect tab name
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getTabByName_UsingIncorrectTabName()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));

            try
            {
                #region Create required form with tab, section, field and form attribute

                SortedList<int, cCustomEntityFormTab> tabList = new SortedList<int, cCustomEntityFormTab>();
                cCustomEntityFormTab reqTab = cCustomEntityFormTabObject.Template();
                tabList.Add(0, reqTab);

                SortedList<int, cCustomEntityFormSection> sectionList = new SortedList<int, cCustomEntityFormSection>();
                cCustomEntityFormSection reqSection = cCustomEntityFormSectionObject.Template(tab: reqTab);
                sectionList.Add(0, reqSection);

                cAttribute reqAttribute = cNumberAttributeObject.Template(createdBy: EmployeeID);

                SortedList<int, cCustomEntityFormField> fieldList = new SortedList<int, cCustomEntityFormField>();
                cCustomEntityFormField reqField = cCustomEntityFormFieldObject.Template(section: reqSection, attribute: reqAttribute);
                fieldList.Add(0, reqField);

                SortedList<int, cCustomEntityForm> formList = new SortedList<int, cCustomEntityForm>();
                cCustomEntityForm reqForm = cCustomEntityFormObject.Template(createdBy: EmployeeID, tabs: tabList, sections: sectionList, fields: fieldList);

                #endregion

                int nCustomEntityID = reqCustomEntity.entityid;

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int formID = customEntities.saveForm(nCustomEntityID, reqForm);
                formList.Add(formID, reqForm);

                reqCustomEntity = cCustomEntityObject.Template(entityID: nCustomEntityID, createdBy: EmployeeID, forms: formList);

                cCustomEntityFormTab actual = reqForm.getTabByName(reqTab.headercaption + "incorrect name" + DateTime.UtcNow.Ticks.ToString());

                Assert.AreEqual(null, actual);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        #endregion

        #region GetFormByID

        /// <summary>
        ///A test for getFormById using a valid FormID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getFormById_UsingValidFormID()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));

            try
            {
                #region Create required form with tab, section, field and form attribute

                SortedList<int, cCustomEntityFormTab> tabList = new SortedList<int,cCustomEntityFormTab>();
                cCustomEntityFormTab reqTab = cCustomEntityFormTabObject.Template();
                tabList.Add(0, reqTab);

                SortedList<int, cCustomEntityFormSection> sectionList = new SortedList<int, cCustomEntityFormSection>();
                cCustomEntityFormSection reqSection = cCustomEntityFormSectionObject.Template(tab: reqTab);
                sectionList.Add(0, reqSection);

                cAttribute reqAttribute = cNumberAttributeObject.Template(createdBy: EmployeeID);

                SortedList<int, cCustomEntityFormField> fieldList = new SortedList<int, cCustomEntityFormField>();
                cCustomEntityFormField reqField = cCustomEntityFormFieldObject.Template(section: reqSection, attribute: reqAttribute);
                fieldList.Add(0, reqField);

                SortedList<int, cCustomEntityForm> formList = new SortedList<int,cCustomEntityForm>();
                cCustomEntityForm reqForm = cCustomEntityFormObject.Template(createdBy: EmployeeID, tabs: tabList, sections: sectionList, fields: fieldList);
            
                #endregion

                int nCustomEntityID = reqCustomEntity.entityid;                       
            
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());
           
                int formID = customEntities.saveForm(nCustomEntityID, reqForm);
                formList.Add(formID, reqForm);

                reqCustomEntity = cCustomEntityObject.Template(entityID: nCustomEntityID, createdBy: EmployeeID, forms: formList);

                cCustomEntityForm actual = reqCustomEntity.getFormById(formID);

                cCompareAssert.AreEqual(reqForm, actual);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for getFormById using an incorrect FormID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getFormById_UsingIncorrectFormID()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));

            try
            {
                #region Create required form with tab, section, field and form attribute

                SortedList<int, cCustomEntityFormTab> tabList = new SortedList<int, cCustomEntityFormTab>();
                cCustomEntityFormTab reqTab = cCustomEntityFormTabObject.Template();
                tabList.Add(0, reqTab);

                SortedList<int, cCustomEntityFormSection> sectionList = new SortedList<int, cCustomEntityFormSection>();
                cCustomEntityFormSection reqSection = cCustomEntityFormSectionObject.Template(tab: reqTab);
                sectionList.Add(0, reqSection);

                cAttribute reqAttribute = cNumberAttributeObject.Template(createdBy: EmployeeID);

                SortedList<int, cCustomEntityFormField> fieldList = new SortedList<int, cCustomEntityFormField>();
                cCustomEntityFormField reqField = cCustomEntityFormFieldObject.Template(section: reqSection, attribute: reqAttribute);
                fieldList.Add(0, reqField);

                SortedList<int, cCustomEntityForm> formList = new SortedList<int, cCustomEntityForm>();
                cCustomEntityForm reqForm = cCustomEntityFormObject.Template(createdBy: EmployeeID, tabs: tabList, sections: sectionList, fields: fieldList);

                #endregion

                int nCustomEntityID = reqCustomEntity.entityid;

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int formID = customEntities.saveForm(nCustomEntityID, reqForm);
                formList.Add(formID, reqForm);

                reqCustomEntity = cCustomEntityObject.Template(entityID: nCustomEntityID, createdBy: EmployeeID, forms: formList);

                reqCustomEntity.Forms.Remove(formID);

                cCustomEntityForm actual = reqCustomEntity.getFormById(formID);

                Assert.AreEqual(null, actual);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        #endregion

        #region GetAttributeByName

        /// <summary>
        ///A test for getAttributeByName using a valid attribute name
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getAttributeByName_UsingValidAttributeName()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));
       
            try
            {   
                cAttribute expected = reqCustomEntity.attributes.Values[0];

                cAttribute actual = reqCustomEntity.getAttributeByName(expected.attributename);
                
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for getAttributeByName using an incorrect attribute name
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getAttributeByName_UsingIncorrectAttributeName()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;
            
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));
            
            try
            {
                cAttribute actual = reqCustomEntity.getAttributeByName("ThisAttributeShouldNotExist");

                Assert.AreEqual(null, actual);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        #endregion

        #region GetViewByID

        /// <summary>
        ///A test for getViewById using a valid view ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getViewById_UsingValidViewID()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                int nCustomEntityID = reqCustomEntity.entityid;

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                SortedList<int, cCustomEntityView> lstCustomViews = new SortedList<int, cCustomEntityView>();
                cCustomEntityView expected = cCustomEntityViewObject.Template(entityID: nCustomEntityID, createdBy: nEmployeeID);

                int nCustomViewID = customEntities.saveView(nCustomEntityID, expected);
                lstCustomViews.Add(nCustomViewID, expected);

                reqCustomEntity = cCustomEntityObject.Template(entityID: nCustomEntityID, createdBy: nEmployeeID, views: lstCustomViews);

                cCustomEntityView actual = reqCustomEntity.getViewById(nCustomViewID);

                cCompareAssert.AreEqual(expected, actual);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for getViewById using an incorrect view ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getViewById_UsingIncorrectViewID()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                int nCustomEntityID = reqCustomEntity.entityid;

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                SortedList<int, cCustomEntityView> lstCustomViews = new SortedList<int, cCustomEntityView>();
                cCustomEntityView reqCustomView = cCustomEntityViewObject.Template(entityID: nCustomEntityID, createdBy: nEmployeeID);

                int nCustomViewID = customEntities.saveView(nCustomEntityID, reqCustomView);
                lstCustomViews.Add(nCustomViewID, reqCustomView);

                reqCustomEntity = cCustomEntityObject.Template(entityID: nCustomEntityID, createdBy: nEmployeeID, views: lstCustomViews);

                reqCustomEntity.Views.Remove(nCustomViewID);

                cCustomEntityView actual = reqCustomEntity.getViewById(nCustomViewID);

                Assert.AreEqual(null, actual);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        #endregion

        #region getViewsByMenuId

        /// <summary>
        ///A test for getViewsByMenuId using a valid menu ID, where views exists on the menu
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getViewsByMenuId_UsingValidMenuIDWhereViewsExist()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                int nCustomEntityID = reqCustomEntity.entityid;

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());
                      
                SortedList<int, cCustomEntityView> lstCustomViews = new SortedList<int, cCustomEntityView>();
                cCustomEntityView reqFirstCustomViewMenu2 = cCustomEntityViewObject.Template(entityID: nCustomEntityID, createdBy: nEmployeeID, menuID: 2);
                cCustomEntityView reqSecondCustomViewMenu2 = cCustomEntityViewObject.Template(entityID: nCustomEntityID, createdBy: nEmployeeID, menuID: 2);
                cCustomEntityView reqFirstCustomViewMenu3 = cCustomEntityViewObject.Template(entityID: nCustomEntityID, createdBy: nEmployeeID, menuID: 3);

                int nMenu2FirstCustomViewID = customEntities.saveView(nCustomEntityID, reqFirstCustomViewMenu2);
                lstCustomViews.Add(nMenu2FirstCustomViewID, reqFirstCustomViewMenu2);
                int nMenu2SecondCustomViewID = customEntities.saveView(nCustomEntityID, reqSecondCustomViewMenu2);
                lstCustomViews.Add(nMenu2SecondCustomViewID, reqSecondCustomViewMenu2);
                int nMenu3FirstCustomViewID = customEntities.saveView(nCustomEntityID, reqFirstCustomViewMenu3);

                reqCustomEntity = cCustomEntityObject.Template(entityID: nCustomEntityID, createdBy: nEmployeeID, views: lstCustomViews);

                List<cCustomEntityView> actual = reqCustomEntity.getViewsByMenuId(2);

                // Assert that the cCustomEntityView added to menu 3 has not been returned
                Assert.AreEqual(false, actual.Contains(reqFirstCustomViewMenu3));

                // Assert that the cCustomEntityViews added to menu 2 have been returned
                Assert.AreEqual(true, actual.Contains(reqFirstCustomViewMenu2));
                Assert.AreEqual(true, actual.Contains(reqSecondCustomViewMenu2));

                // Assert that the cCustomEntityViews added to menu 2 are correct
                foreach(cCustomEntityView currentView in actual)
                {
                    if (currentView == reqFirstCustomViewMenu2)
                    {
                        cCompareAssert.AreEqual(reqFirstCustomViewMenu2, currentView);
                    }
                    else if (currentView == reqSecondCustomViewMenu2)
                    {
                        cCompareAssert.AreEqual(reqSecondCustomViewMenu2, currentView);
                    }
                }
                
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for getViewsByMenuId using an incorrect menu ID, where no views should exists on the menu
        ///</summary>        
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getViewsByMenuId_UsingInvalidMenuID()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                int nCustomEntityID = reqCustomEntity.entityid;

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                List<cCustomEntityView> actual = reqCustomEntity.getViewsByMenuId(99999);                

                // Assert that no cCustomEntityViews exist for the incorrect menu ID 
                Assert.AreEqual(0, actual.Count);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        #endregion

        #region containsField

        /// <summary>
        ///A test for containsField using valid field Guid
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_containsField_UsingValidFieldGuid()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                Guid reqFieldID = reqCustomEntity.attributes.Values[0].fieldid;

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                cFields clsFields = new cFields(Moqs.CurrentUser().AccountID);

                SortedList<byte, cCustomEntityViewField> lstFields = new SortedList<byte, cCustomEntityViewField>();
                lstFields.Add(0, new cCustomEntityViewField(clsFields.GetFieldByID(reqFieldID)));
                                               
                cCustomEntityView reqCustomEntityView = cCustomEntityViewObject.Template(entityID: reqCustomEntity.entityid, fields: lstFields);
                

                Assert.AreEqual(true, reqCustomEntityView.containsField(reqFieldID));                               
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for containsField using an incorrect field Guid
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_containsField_UsingIncorrectFieldGuid()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            Guid reqFieldID = new Guid("12345678-1234-1234-1234-123456789123");

            cFields clsFields = new cFields(Moqs.CurrentUser().AccountID);

            cCustomEntityView reqCustomEntityView = cCustomEntityViewObject.Template();

            Assert.AreEqual(false, reqCustomEntityView.containsField(reqFieldID));
        }

        #endregion

        #region getKeyField
        
        /// <summary>
        ///A list of one attribute with no keyfield set should return null
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getKeyField_OneNonKeyFieldAttribute()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cAttribute reqAttribute = cNumberAttributeObject.Template(isKeyField: false);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            cAttribute att = reqCustomEntity.getKeyField();

            Assert.IsNull(att);

        }

        /// <summary>
        ///A list of many attributes with no keyfield set should return null
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getKeyField_MultipleAttributesWithNoKeyFieldSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cAttribute reqAttribute1 = cNumberAttributeObject.Template(isKeyField: false);
            cAttribute reqAttribute2 = cNumberAttributeObject.Template(isKeyField: false);
            cAttribute reqAttribute3 = cNumberAttributeObject.Template(isKeyField: false);
            cAttribute reqAttribute4 = cNumberAttributeObject.Template(isKeyField: false);
            cAttribute reqAttribute5 = cNumberAttributeObject.Template(isKeyField: false);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute1);
            attributes_list.Add(1, reqAttribute2);
            attributes_list.Add(2, reqAttribute3);
            attributes_list.Add(3, reqAttribute4);
            attributes_list.Add(4, reqAttribute5);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            cAttribute att = reqCustomEntity.getKeyField();

            Assert.IsNull(att);

        }

        /// <summary>
        ///A list of one attribute with keyfield set true should return the attribute
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getKeyField_OneAttributeWithKeyFieldSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cAttribute reqAttribute1 = cNumberAttributeObject.Template(isKeyField: true);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute1);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            cAttribute att = reqCustomEntity.getKeyField();

            Assert.IsNotNull(att);
            Assert.IsTrue(reqAttribute1.iskeyfield);

        }

        /// <summary>
        ///A list of many attributes with no keyfield set should return null
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getKeyField_MultipleAttributesWithKeyFieldSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cAttribute reqAttribute1 = cNumberAttributeObject.Template(displayName: "att0", isKeyField: false);
            cAttribute reqAttribute2 = cNumberAttributeObject.Template(displayName: "att1", isKeyField: false);
            cAttribute reqAttribute3 = cNumberAttributeObject.Template(displayName: "att2", isKeyField: true);
            cAttribute reqAttribute4 = cNumberAttributeObject.Template(displayName: "att3", isKeyField: false);
            cAttribute reqAttribute5 = cNumberAttributeObject.Template(displayName: "att4", isKeyField: false);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute1);
            attributes_list.Add(1, reqAttribute2);
            attributes_list.Add(2, reqAttribute3);
            attributes_list.Add(3, reqAttribute4);
            attributes_list.Add(4, reqAttribute5);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            cAttribute att = reqCustomEntity.getKeyField();

            Assert.IsNotNull(att);
            Assert.IsTrue(att.iskeyfield);
            Assert.AreEqual("att2", att.displayname);

        }

        #endregion

        #region getAuditIdentifier

        /// <summary>
        ///A list of one attribute with no isAuditIdentity set should return null
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getAuditIdentifier_OneNonAuditIdentifierAttribute()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cAttribute reqAttribute = cNumberAttributeObject.Template(isAuditIdentity: false);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            cAttribute att = reqCustomEntity.getAuditIdentifier();

            Assert.IsNull(att);
        }

        /// <summary>
        ///A list of one attribute with no isAuditIdentity set should return null
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getAuditIdentifier_OneNonAuditIdentifierAttributeWithKeyField()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cAttribute reqAttribute = cNumberAttributeObject.Template(isAuditIdentity: false, isKeyField: true);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            cAttribute att = reqCustomEntity.getAuditIdentifier();

            Assert.IsNotNull(att);

            cAttribute keyatt = reqCustomEntity.getKeyField();
            Assert.AreEqual(keyatt.attributeid, att.attributeid);
        }

        /// <summary>
        ///A list of many attributes with no isAuditIdentity set should return null
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getAuditIdentifier_MultipleAttributesWithNoAuditIdentifierSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cAttribute reqAttribute1 = cNumberAttributeObject.Template(isAuditIdentity: false);
            cAttribute reqAttribute2 = cNumberAttributeObject.Template(isAuditIdentity: false);
            cAttribute reqAttribute3 = cNumberAttributeObject.Template(isAuditIdentity: false);
            cAttribute reqAttribute4 = cNumberAttributeObject.Template(isAuditIdentity: false);
            cAttribute reqAttribute5 = cNumberAttributeObject.Template(isAuditIdentity: false);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute1);
            attributes_list.Add(1, reqAttribute2);
            attributes_list.Add(2, reqAttribute3);
            attributes_list.Add(3, reqAttribute4);
            attributes_list.Add(4, reqAttribute5);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            cAttribute att = reqCustomEntity.getAuditIdentifier();

            Assert.IsNull(att);
        }

        /// <summary>
        ///A list of many attributes with no isAuditIdentity set should return null
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getAuditIdentifier_MultipleAttributesWithNoAuditIdentifierSetWithKeyField()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cAttribute reqAttribute1 = cNumberAttributeObject.Template(isAuditIdentity: false, isKeyField: true);
            cAttribute reqAttribute2 = cNumberAttributeObject.Template(isAuditIdentity: false);
            cAttribute reqAttribute3 = cNumberAttributeObject.Template(isAuditIdentity: false);
            cAttribute reqAttribute4 = cNumberAttributeObject.Template(isAuditIdentity: false);
            cAttribute reqAttribute5 = cNumberAttributeObject.Template(isAuditIdentity: false);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute1);
            attributes_list.Add(1, reqAttribute2);
            attributes_list.Add(2, reqAttribute3);
            attributes_list.Add(3, reqAttribute4);
            attributes_list.Add(4, reqAttribute5);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            cAttribute att = reqCustomEntity.getAuditIdentifier();

            Assert.IsNotNull(att);

            cAttribute keyatt = reqCustomEntity.getKeyField();
            Assert.AreEqual(keyatt.attributeid, att.attributeid);
        }

        /// <summary>
        ///A list of one attribute with isAuditIdentity set true should return the attribute
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getAuditIdentifier_OneAttributeWithAuditIdentifierSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cAttribute reqAttribute1 = cNumberAttributeObject.Template(isAuditIdentity: true);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute1);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            cAttribute att = reqCustomEntity.getAuditIdentifier();

            Assert.IsNotNull(att);
            Assert.IsTrue(reqAttribute1.isauditidentifer);

        }

        /// <summary>
        ///A list of many attributes with no isAuditIdentity set should return null
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getAuditIdentifier_MultipleAttributesWithAuditIdentifierSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cAttribute reqAttribute1 = cNumberAttributeObject.Template(displayName: "att0", isAuditIdentity: false);
            cAttribute reqAttribute2 = cNumberAttributeObject.Template(displayName: "att1", isAuditIdentity: false);
            cAttribute reqAttribute3 = cNumberAttributeObject.Template(displayName: "att2", isAuditIdentity: true);
            cAttribute reqAttribute4 = cNumberAttributeObject.Template(displayName: "att3", isAuditIdentity: false);
            cAttribute reqAttribute5 = cNumberAttributeObject.Template(displayName: "att4", isAuditIdentity: false);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute1);
            attributes_list.Add(1, reqAttribute2);
            attributes_list.Add(2, reqAttribute3);
            attributes_list.Add(3, reqAttribute4);
            attributes_list.Add(4, reqAttribute5);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            cAttribute att = reqCustomEntity.getAuditIdentifier();

            Assert.IsNotNull(att);
            Assert.IsTrue(att.isauditidentifer);
            Assert.AreEqual("att2", att.displayname);

        }

        #endregion

        #region Mobile Attributes

        /// <summary>
        /// Checks attributes set to show in mobile.
        /// </summary>
        [TestCategory("Spend Management")]
        [TestCategory("Greenlight")]
        [TestCategory("Custom Entities")]
        [TestMethod]
        public void CustomEntityCheckShowInMobile()
        {
            int employeeId = Moqs.CurrentUser().EmployeeID;

            cTextAttribute textAttribute = cTextAttributeObject.Template(displayInMobile: true);
            var attributesList = new SortedList<int, cAttribute> { { 0, textAttribute } };

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: employeeId, attributes: attributesList);
            cAttribute firstAttribute = reqCustomEntity.attributes[0];

            Assert.AreEqual(textAttribute.DisplayInMobile, firstAttribute.DisplayInMobile);
        }

        #endregion

        #region getUniqueAttributes

        /// <summary>
        ///A list of all the different attribute types parsed within the method with all set to unique should return an empty list.
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getUniqueAttributes_AllFalse()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            int[] lstUniques;

            cAttribute reqAttribute1 = cNumberAttributeObject.Template(isUnique: false);
            cAttribute reqAttribute2 = cNumberAttributeObject.BasicInteger(false);
            cAttribute reqAttribute3 = cTextAttributeObject.BasicSingleLine(false);
            cAttribute reqAttribute4 = cTextAttributeObject.BasicMultiLine(false);
            cAttribute reqAttribute5 = cDateTimeAttributeObject.Template(isUnique: false);
            cAttribute reqAttribute6 = cTickboxAttributeObject.Template(isUnique: false);
            cAttribute reqAttribute7 = cListAttributeObject.BasicList(isUnique: false);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute1);
            attributes_list.Add(1, reqAttribute2);
            attributes_list.Add(2, reqAttribute3);
            attributes_list.Add(3, reqAttribute4);
            attributes_list.Add(4, reqAttribute5);
            attributes_list.Add(5, reqAttribute6);
            attributes_list.Add(6, reqAttribute7);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            lstUniques = reqCustomEntity.getUniqueAttributes();

            Assert.AreEqual(0, lstUniques.Length);
        }

        /// <summary>
        ///A list of all the different attribute types parsed within the method with all set to unique should return a full list.
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getUniqueAttributes_AllTrue()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            int[] lstUniques;

            cAttribute reqAttribute1 = cNumberAttributeObject.Template(isUnique: true);
            cAttribute reqAttribute2 = cNumberAttributeObject.BasicInteger(true);
            cAttribute reqAttribute3 = cTextAttributeObject.BasicSingleLine(true);
            cAttribute reqAttribute4 = cTextAttributeObject.BasicMultiLine(true);
            cAttribute reqAttribute5 = cDateTimeAttributeObject.Template(isUnique: true);
            cAttribute reqAttribute6 = cTickboxAttributeObject.Template(isUnique: true);
            cAttribute reqAttribute7 = cListAttributeObject.BasicList(isUnique: true);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute1);
            attributes_list.Add(1, reqAttribute2);
            attributes_list.Add(2, reqAttribute3);
            attributes_list.Add(3, reqAttribute4);
            attributes_list.Add(4, reqAttribute5);
            attributes_list.Add(5, reqAttribute6);
            attributes_list.Add(6, reqAttribute7);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            lstUniques = reqCustomEntity.getUniqueAttributes();

            Assert.AreEqual(7, lstUniques.Length);
        }

        #endregion

        #region getUniqueAttributesFieldName

        /// <summary>
        ///A list of all the different attribute by fieldname for unique attributes. None should be returned when no attributes are unique
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getUniqueAttributesFieldName_NoneUnique()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string[] lstUniquesAttributes;

            cAttribute reqAttribute1 = cNumberAttributeObject.Template(isUnique: false);
            cAttribute reqAttribute2 = cNumberAttributeObject.BasicInteger(isUnique: false);
            cAttribute reqAttribute3 = cTextAttributeObject.BasicSingleLine(false);
            cAttribute reqAttribute4 = cTextAttributeObject.BasicMultiLine(false);
            cAttribute reqAttribute5 = cDateTimeAttributeObject.Template(isUnique: false);
            cAttribute reqAttribute6 = cTickboxAttributeObject.Template(isUnique: false);
            cAttribute reqAttribute7 = cListAttributeObject.BasicList(isUnique: false);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute1);
            attributes_list.Add(1, reqAttribute2);
            attributes_list.Add(2, reqAttribute3);
            attributes_list.Add(3, reqAttribute4);
            attributes_list.Add(4, reqAttribute5);
            attributes_list.Add(5, reqAttribute6);
            attributes_list.Add(6, reqAttribute7);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            lstUniquesAttributes = reqCustomEntity.getUniqueAttributesFieldNames();

            Assert.AreEqual(0, lstUniquesAttributes.Length);
        }

        /// <summary>
        ///A list of all the different attribute by fieldname for unique attributes. A full list should be returned
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_getUniqueAttributesFieldName_AllUnique()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string[] lstUniquesAttributes;

            cAttribute reqAttribute1 = cNumberAttributeObject.Template(isUnique: true);
            cAttribute reqAttribute2 = cNumberAttributeObject.BasicInteger(isUnique: true);
            cAttribute reqAttribute3 = cTextAttributeObject.BasicSingleLine(isUnique: true);
            cAttribute reqAttribute4 = cTextAttributeObject.BasicMultiLine(isUnique: true);
            cAttribute reqAttribute5 = cDateTimeAttributeObject.Template(isUnique: true);
            cAttribute reqAttribute6 = cTickboxAttributeObject.Template(isUnique: true);
            cAttribute reqAttribute7 = cListAttributeObject.BasicList(isUnique: true);

            SortedList<int, cAttribute> attributes_list = new SortedList<int, cAttribute>();
            attributes_list.Add(0, reqAttribute1);
            attributes_list.Add(1, reqAttribute2);
            attributes_list.Add(2, reqAttribute3);
            attributes_list.Add(3, reqAttribute4);
            attributes_list.Add(4, reqAttribute5);
            attributes_list.Add(5, reqAttribute6);
            attributes_list.Add(6, reqAttribute7);

            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: nEmployeeID, attributes: attributes_list);
            lstUniquesAttributes = reqCustomEntity.getUniqueAttributesFieldNames();

            Assert.AreEqual(7, lstUniquesAttributes.Length);
            Assert.AreEqual("txt0", lstUniquesAttributes[0]);
            Assert.AreEqual("txt0", lstUniquesAttributes[1]);
            Assert.AreEqual("txt0", lstUniquesAttributes[2]);
            Assert.AreEqual("txt0", lstUniquesAttributes[3]);
            Assert.AreEqual("txt0", lstUniquesAttributes[4]);
            Assert.AreEqual("cmb0", lstUniquesAttributes[5]);
            Assert.AreEqual("cmb0", lstUniquesAttributes[6]);
        }

        #endregion

        #region findOneToManyRelationship

        /// <summary>
        ///With one one-to-many relationship created, it should be found
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_findOneToManyRelationship_OneExists()
        {
            cCustomEntity entity1 = cCustomEntityObject.New();
            cCustomEntity entity2 = null;

            try
            {
                entity2 = cCustomEntityObject.New();
                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid);

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                entity2 = clsCustomEntities.getEntityById(entity2.entityid);

                // Add a new attribute
                cOneToManyRelationship expected = cOneToManyRelationshipObject.New(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, entity2));
                clsCustomEntities = new cCustomEntities(currentUser);
                entity1 = clsCustomEntities.getEntityById(entity1.entityid);
               List< cOneToManyRelationship> otm = entity1.findOneToManyRelationship(entity2.entityid);
                Assert.IsNotNull(otm);
            }
            finally
            {
                if (entity1 != null)
                {
                    cCustomEntityObject.TearDown(entity1.entityid);
                }
                if (entity2 != null)
                {
                    cCustomEntityObject.TearDown(entity2.entityid);
                }
            }
        }

        /// <summary>
        ///With zero one-to-many relationships created, none should be found
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_findOneToManyRelationship_NoneExists()
        {
            cCustomEntity entity1 = cCustomEntityObject.New();
            cCustomEntity entity2 = null;

            try
            {
                entity2 = cCustomEntityObject.New();
                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid);

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);

                entity1 = clsCustomEntities.getEntityById(entity1.entityid);
                entity2 = clsCustomEntities.getEntityById(entity2.entityid);

                List<cOneToManyRelationship> otm = entity1.findOneToManyRelationship(entity2.entityid);
                Assert.IsNull(otm);
            }
            finally
            {
                if (entity1 != null)
                {
                    cCustomEntityObject.TearDown(entity1.entityid);
                }
                if (entity2 != null)
                {
                    cCustomEntityObject.TearDown(entity2.entityid);
                }
            }
        }

        #endregion

        #region sortForms

        /// <summary>
        ///A test for sortForms using valid forms
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_sortForms_UsingValidForms()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New();            

            try
            {
                cCustomEntityForm reqFormOne = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.Template(formName: "Form C"));
                cCustomEntityForm reqFormTwo = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.Template(formName: "Form B"));
                cCustomEntityForm reqFormThree = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.Template(formName: "Form A"));

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int nEntityID = customEntities.saveEntity(reqCustomEntity);

                reqCustomEntity = customEntities.getEntityById(nEntityID);

                SortedList<string, cCustomEntityForm> lstForms = reqCustomEntity.sortForms();

                Assert.AreEqual(lstForms.Values[0].formname, "Form A");
                Assert.AreEqual(lstForms.Values[1].formname, "Form B");
                Assert.AreEqual(lstForms.Values[2].formname, "Form C");

            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for sortForms using a single form
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_sortForms_UsingASingleForm()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New();

            try
            {
                cCustomEntityForm reqFormOne = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.Template(formName: "Form C"));                                

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int nEntityID = customEntities.saveEntity(reqCustomEntity);

                reqCustomEntity = customEntities.getEntityById(nEntityID);

                SortedList<string, cCustomEntityForm> lstForms = reqCustomEntity.sortForms();

                Assert.AreEqual(lstForms.Values[0].formname, "Form C");                               
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for sortForms where no forms exist
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_sortForms_WhereNoFormExists()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New();

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int nEntityID = customEntities.saveEntity(reqCustomEntity);

                reqCustomEntity = customEntities.getEntityById(nEntityID);

                SortedList<string, cCustomEntityForm> lstForms = reqCustomEntity.sortForms();

                Assert.AreEqual(0, lstForms.Count);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        #endregion sortForms

        #region sortViews

        /// <summary>
        ///A test for sortViews using valid views
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_sortViews_UsingValidViews()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New();

            try
            {
                cCustomEntityView reqViewOne = cCustomEntityViewObject.New(reqCustomEntity.entityid, cCustomEntityViewObject.Template(viewName: "View C"));
                cCustomEntityView reqViewTwo = cCustomEntityViewObject.New(reqCustomEntity.entityid, cCustomEntityViewObject.Template(viewName: "View B"));
                cCustomEntityView reqViewThree = cCustomEntityViewObject.New(reqCustomEntity.entityid, cCustomEntityViewObject.Template(viewName: "View A"));

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int nEntityID = customEntities.saveEntity(reqCustomEntity);

                reqCustomEntity = customEntities.getEntityById(nEntityID);

                SortedList<string, cCustomEntityView> lstViews = reqCustomEntity.sortViews();

                Assert.AreEqual(lstViews.Values[0].viewname, "View A");
                Assert.AreEqual(lstViews.Values[1].viewname, "View B");
                Assert.AreEqual(lstViews.Values[2].viewname, "View C");
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for sortViews using a single view
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_sortViews_UsingASingleView()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New();

            try
            {
                cCustomEntityView reqViewOne = cCustomEntityViewObject.New(reqCustomEntity.entityid, cCustomEntityViewObject.Template(viewName: "View C"));

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int nEntityID = customEntities.saveEntity(reqCustomEntity);

                reqCustomEntity = customEntities.getEntityById(nEntityID);

                SortedList<string, cCustomEntityView> lstViews = reqCustomEntity.sortViews();

                Assert.AreEqual(lstViews.Values[0].viewname, "View C");
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for sortViews where no views exist
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_sortViews_WhereNoViewExists()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New();

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int nEntityID = customEntities.saveEntity(reqCustomEntity);

                reqCustomEntity = customEntities.getEntityById(nEntityID);

                SortedList<string, cCustomEntityView> lstViews = reqCustomEntity.sortViews();

                Assert.AreEqual(0, lstViews.Count);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        #endregion sortViews

        #region CreateFormDropDown

        /// <summary>
        ///A test for sortForms using valid forms
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_CreateFormDropDown_UsingValidForms()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New();

            try
            {
                cCustomEntityForm reqFormOne = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.Template(formName: "Form C", description: "Description C"));
                cCustomEntityForm reqFormTwo = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.Template(formName: "Form B", description: "Description B"));
                cCustomEntityForm reqFormThree = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.Template(formName: "Form A", description: "Description A"));

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int nEntityID = customEntities.saveEntity(reqCustomEntity);

                reqCustomEntity = customEntities.getEntityById(nEntityID);

                List<ListItem> lstForms = reqCustomEntity.CreateFormDropDown();

                Assert.AreEqual("Form A", lstForms[0].Text);
                Assert.AreEqual(reqFormThree.formid.ToString(), lstForms[0].Value);
                Assert.AreEqual("Form B", lstForms[1].Text);
                Assert.AreEqual(reqFormTwo.formid.ToString(), lstForms[1].Value);
                Assert.AreEqual("Form C", lstForms[2].Text);
                Assert.AreEqual(reqFormOne.formid.ToString(), lstForms[2].Value);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for sortForms where no forms exist
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntity_CreateFormDropDown_WhereNoFormsExist()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New();

            try
            {                
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int nEntityID = customEntities.saveEntity(reqCustomEntity);

                reqCustomEntity = customEntities.getEntityById(nEntityID);

                List<ListItem> lstForms = reqCustomEntity.CreateFormDropDown();

                Assert.AreEqual(0, lstForms.Count);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        #endregion CreateFormDropDown

        #region File Attachments

        /// <summary>
        /// test for a custom entity, add attachment to new green light with id = zero then update to a known id.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestCategory("Attachments"), TestMethod()]
        public void CustomEntityAddAttachmentToNewGreenlightThenUpdate()
        {
            MethodBase methodBase = new StackFrame().GetMethod();

            // Set up Objects
            var currentUser = Moqs.CurrentUser();
            cMimeTypes mimeTypes = new cMimeTypes(currentUser.AccountID, currentUser.CurrentSubAccountId);
            SortedList<int, cMimeType> cache = mimeTypes.Cache["MimeTypes" + currentUser.AccountID] as SortedList<int, cMimeType>;
            DateTime createdOn = DateTime.Now;
            DateTime? modifiedOn = null;
            int? modifiedBy = null;
            byte[] attachmentData = { 1, 2, 3, 4 };
            var attachmentid = 0;
            int? proxy = currentUser.Delegate == null ? null : (int?)currentUser.Delegate.EmployeeID;
            cAttachments attachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID, currentUser.CurrentSubAccountId, proxy);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: currentUser.EmployeeID, enableAttachments: true));
            var tableName = "custom_" + reqCustomEntity.entityid + "_attachments";
            try
            {
                cAttachment attachment = new cAttachment(0, Guid.NewGuid(), reqCustomEntity.entityid, "UT_" + methodBase.Name, methodBase.Name, @"c:\projects\filename.txt", cache.Values[0], createdOn, currentUser.EmployeeID, modifiedOn, modifiedBy, attachmentData);

                // Save attachment with id = zero
                attachmentid = attachments.saveAttachment(tableName, reqCustomEntity.getKeyField().displayname, 0, attachment, attachmentData);
                Assert.IsTrue(attachmentid > 0, "Attachment has not been saved.");

                // Update id
                attachments.UpdateNewAttachmentsWithRealId(reqCustomEntity.entityid, 999);

                // get attachment to check id
                var returnAttachment = attachments.getAttachment(tableName, attachmentid);
                Assert.IsTrue(returnAttachment.OwnerRecordID == 999, string.Format("OwnerRecord id = {0} and should be 999.", returnAttachment.attachmentID));
            }
            finally
            {
                // clear up
                attachments.deleteAttachment(tableName, attachmentid, reqCustomEntity.entityid, AttachDocumentType.None);
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        /// test for a custom entity, add attachment to new green light with id = zero then update to a known id as a different Employee ID (should not update).
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestCategory("Attachments"), TestMethod()]
        public void CustomEntityAddAttachmentToNewGreenlightThenUpdateAsADifferentEmployee()
        {
            MethodBase methodBase = new StackFrame().GetMethod();

            // Set up Objects
            var currentUser = Moqs.CurrentUser();
            cMimeTypes mimeTypes = new cMimeTypes(currentUser.AccountID, currentUser.CurrentSubAccountId);
            SortedList<int, cMimeType> cache = mimeTypes.Cache["MimeTypes" + currentUser.AccountID] as SortedList<int, cMimeType>;
            DateTime createdOn = DateTime.Now;
            DateTime? modifiedOn = null;
            int? modifiedBy = null;
            byte[] attachmentData = { 1, 2, 3, 4 };
            var attachmentid = 0;
            int? proxy = currentUser.Delegate == null ? null : (int?)currentUser.Delegate.EmployeeID;
            cAttachments attachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID, currentUser.CurrentSubAccountId, proxy);
            cAttachments badAttachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID + 1, currentUser.CurrentSubAccountId, proxy);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: currentUser.EmployeeID, enableAttachments: true));
            var tableName = "custom_" + reqCustomEntity.entityid + "_attachments";
            try
            {
                cAttachment attachment = new cAttachment(0, Guid.NewGuid(), reqCustomEntity.entityid, "UT_" + methodBase.Name, methodBase.Name, @"c:\projects\filename.txt", cache.Values[0], createdOn, currentUser.EmployeeID, modifiedOn, modifiedBy, attachmentData);

                // Save attachment with id = zero
                attachmentid = attachments.saveAttachment(tableName, reqCustomEntity.getKeyField().displayname, 0, attachment, attachmentData);
                Assert.IsTrue(attachmentid > 0, "Attachment has not been saved.");

                // Update id
                badAttachments.UpdateNewAttachmentsWithRealId(reqCustomEntity.entityid, 999);

                // get attachment to check id
                var returnAttachment = attachments.getAttachment(tableName, attachmentid);
                Assert.IsTrue(returnAttachment.OwnerRecordID == 0, string.Format("OwnerRecord id = {0} and should be 0.", returnAttachment.attachmentID));
            }
            finally
            {
                // clear up
                attachments.deleteAttachment(tableName, attachmentid, reqCustomEntity.entityid, AttachDocumentType.None);
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        /// test for a custom entity, add attachment to green light with id = 1 then attempt update to a known id.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestCategory("Attachments"), TestMethod()]
        public void CustomEntityAddAttachmentThenAttemptUpdate()
        {
            MethodBase methodBase = new StackFrame().GetMethod();

            // Set up Objects
            var currentUser = Moqs.CurrentUser();
            cMimeTypes mimeTypes = new cMimeTypes(currentUser.AccountID, currentUser.CurrentSubAccountId);
            SortedList<int, cMimeType> cache = mimeTypes.Cache["MimeTypes" + currentUser.AccountID] as SortedList<int, cMimeType>;
            DateTime createdOn = DateTime.Now;
            DateTime? modifiedOn = null;
            int? modifiedBy = null;
            byte[] attachmentData = { 1, 2, 3, 4 };
            var attachmentid = 0;
            int? proxy = currentUser.Delegate == null ? null : (int?)currentUser.Delegate.EmployeeID;
            cAttachments attachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID, currentUser.CurrentSubAccountId, proxy);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: currentUser.EmployeeID, enableAttachments: true));
            var tableName = "custom_" + reqCustomEntity.entityid + "_attachments";
            try
            {
                cAttachment attachment = new cAttachment(0, Guid.NewGuid(), reqCustomEntity.entityid, "UT_" + methodBase.Name, methodBase.Name, @"c:\projects\filename.txt", cache.Values[0], createdOn, currentUser.EmployeeID, modifiedOn, modifiedBy, attachmentData);

                // Save attachment with id = zero
                attachmentid = attachments.saveAttachment(tableName, reqCustomEntity.getKeyField().displayname, 1, attachment, attachmentData);
                Assert.IsTrue(attachmentid > 0, "Attachment has not been saved.");

                // Update id
                attachments.UpdateNewAttachmentsWithRealId(reqCustomEntity.entityid, 999);

                // get attachment to check id
                var returnAttachment = attachments.getAttachment(tableName, attachmentid);
                Assert.IsTrue(returnAttachment.OwnerRecordID == 1, string.Format("OwnerRecord id = {0} and should be 1.", returnAttachment.attachmentID));
            }
            finally
            {
                // clear up
                attachments.deleteAttachment(tableName, attachmentid, reqCustomEntity.entityid, AttachDocumentType.None);
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        /// test for a custom entity, add attachment to new green light with id = 0 then attempt update to a known id using the wrong green light id.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestCategory("Attachments"), TestMethod()]
        public void CustomEntityAddAttachmentThenAttemptUpdateWithWrongEntityId()
        {
            MethodBase methodBase = new StackFrame().GetMethod();

            // Set up Objects
            var currentUser = Moqs.CurrentUser();
            cMimeTypes mimeTypes = new cMimeTypes(currentUser.AccountID, currentUser.CurrentSubAccountId);
            SortedList<int, cMimeType> cache = mimeTypes.Cache["MimeTypes" + currentUser.AccountID] as SortedList<int, cMimeType>;
            DateTime createdOn = DateTime.Now;
            DateTime? modifiedOn = null;
            int? modifiedBy = null;
            byte[] attachmentData = { 1, 2, 3, 4 };
            var attachmentid = 0;
            int? proxy = currentUser.Delegate == null ? null : (int?)currentUser.Delegate.EmployeeID;
            cAttachments attachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID, currentUser.CurrentSubAccountId, proxy);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: currentUser.EmployeeID, enableAttachments: true));
            var tableName = "custom_" + reqCustomEntity.entityid + "_attachments";
            try
            {
                cAttachment attachment = new cAttachment(0, Guid.NewGuid(), reqCustomEntity.entityid, "UT_" + methodBase.Name, methodBase.Name, @"c:\projects\filename.txt", cache.Values[0], createdOn, currentUser.EmployeeID, modifiedOn, modifiedBy, attachmentData);

                // Save attachment with id = zero
                attachmentid = attachments.saveAttachment(tableName, reqCustomEntity.getKeyField().displayname, 0, attachment, attachmentData);
                Assert.IsTrue(attachmentid > 0, "Attachment has not been saved.");

                // Update id
                attachments.UpdateNewAttachmentsWithRealId(reqCustomEntity.entityid + 1, 999);

                // get attachment to check id
                var returnAttachment = attachments.getAttachment(tableName, attachmentid);
                Assert.IsTrue(returnAttachment.OwnerRecordID == 0, string.Format("OwnerRecord id = {0} and should be 0.", returnAttachment.attachmentID));
            }
            finally
            {
                // clear up
                attachments.deleteAttachment(tableName, attachmentid, reqCustomEntity.entityid, AttachDocumentType.None);
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        /// test for a custom entity, add attachment to new green light with id = zero then remove orphan attachment (id = zero).
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestCategory("Attachments"), TestMethod()]
        public void CustomEntityAddAttachmentToNewGreenlightThenRemoveOrphans()
        {
            MethodBase methodBase = new StackFrame().GetMethod();

            // Set up Objects
            var currentUser = Moqs.CurrentUser();
            cMimeTypes mimeTypes = new cMimeTypes(currentUser.AccountID, currentUser.CurrentSubAccountId);
            SortedList<int, cMimeType> cache = mimeTypes.Cache["MimeTypes" + currentUser.AccountID] as SortedList<int, cMimeType>;
            DateTime createdOn = DateTime.Now;
            DateTime? modifiedOn = null;
            int? modifiedBy = null;
            byte[] attachmentData = { 1, 2, 3, 4 };
            var attachmentid = 0;
            int? proxy = currentUser.Delegate == null ? null : (int?)currentUser.Delegate.EmployeeID;
            cAttachments attachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID, currentUser.CurrentSubAccountId, proxy);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: currentUser.EmployeeID, enableAttachments: true));
            var tableName = "custom_" + reqCustomEntity.entityid + "_attachments";
            try
            {
                cAttachment attachment = new cAttachment(0, Guid.NewGuid(), reqCustomEntity.entityid, "UT_" + methodBase.Name, methodBase.Name, @"c:\projects\filename.txt", cache.Values[0], createdOn, currentUser.EmployeeID, modifiedOn, modifiedBy, attachmentData);

                // Save attachment with id = zero
                attachmentid = attachments.saveAttachment(tableName, reqCustomEntity.getKeyField().displayname, 0, attachment, attachmentData);
                Assert.IsTrue(attachmentid > 0, "Attachment has not been saved.");

                // RemoveOrphans
                attachments.RemoveOrphanAttachmentsOnCustomTables(reqCustomEntity.entityid, currentUser.EmployeeID, DateTime.Now.AddHours(1));

                // get attachment to check id - should return null
                var returnAttachment = attachments.getAttachment(tableName, attachmentid);
                Assert.IsTrue(returnAttachment == null, "An attachment has been returned when it should have been deleted.");
            }
            finally
            {
                // clear up
                attachments.deleteAttachment(tableName, attachmentid, reqCustomEntity.entityid, AttachDocumentType.None);
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        /// test for a custom entity, add attachment to green light with id = 1 then delete it.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestCategory("Attachments"), TestMethod()]
        public void CustomEntityAddAttachmentThenDeleteAttachment()
        {
            MethodBase methodBase = new StackFrame().GetMethod();

            // Set up Objects
            var currentUser = Moqs.CurrentUser();
            cMimeTypes mimeTypes = new cMimeTypes(currentUser.AccountID, currentUser.CurrentSubAccountId);
            SortedList<int, cMimeType> cache = mimeTypes.Cache["MimeTypes" + currentUser.AccountID] as SortedList<int, cMimeType>;
            DateTime createdOn = DateTime.Now;
            DateTime? modifiedOn = null;
            int? modifiedBy = null;
            byte[] attachmentData = { 1, 2, 3, 4 };
            var attachmentid = 0;
            int? proxy = currentUser.Delegate == null ? null : (int?)currentUser.Delegate.EmployeeID;
            cAttachments attachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID, currentUser.CurrentSubAccountId, proxy);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: currentUser.EmployeeID, enableAttachments: true));
            var tableName = "custom_" + reqCustomEntity.entityid + "_attachments";
            try
            {
                cAttachment attachment = new cAttachment(0, Guid.NewGuid(), reqCustomEntity.entityid, "UT_" + methodBase.Name, methodBase.Name, @"c:\projects\filename.txt", cache.Values[0], createdOn, currentUser.EmployeeID, modifiedOn, modifiedBy, attachmentData);

                // Save attachment with id = zero
                attachmentid = attachments.saveAttachment(tableName, reqCustomEntity.getKeyField().displayname, 1, attachment, attachmentData);
                Assert.IsTrue(attachmentid > 0, "Attachment has not been saved.");

                // DElete attachment
                attachments.deleteAttachment(tableName, attachmentid, 1, AttachDocumentType.None);

                // get attachment to check id
                var returnAttachment = attachments.getAttachment(tableName, attachmentid);
                Assert.IsTrue(returnAttachment == null, "The attachment has not been deleted.");
            }
            finally
            {
                // clear up
                attachments.deleteAttachment(tableName, attachmentid, reqCustomEntity.entityid, AttachDocumentType.None);
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        /// test for a custom entity, add attachment to new green light with id = zero then remove orphan attachment (id = zero) as different EmployeeID (should fail to remove).
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestCategory("Attachments"), TestMethod()]
        public void CustomEntityAddAttachmentToNewGreenlightThenRemoveOrphansAsDifferentEmployeeId()
        {
            MethodBase methodBase = new StackFrame().GetMethod();

            // Set up Objects
            var currentUser = Moqs.CurrentUser();
            cMimeTypes mimeTypes = new cMimeTypes(currentUser.AccountID, currentUser.CurrentSubAccountId);
            SortedList<int, cMimeType> cache = mimeTypes.Cache["MimeTypes" + currentUser.AccountID] as SortedList<int, cMimeType>;
            DateTime createdOn = DateTime.Now;
            DateTime? modifiedOn = null;
            int? modifiedBy = null;
            byte[] attachmentData = { 1, 2, 3, 4 };
            var attachmentid = 0;
            int? proxy = currentUser.Delegate == null ? null : (int?)currentUser.Delegate.EmployeeID;
            cAttachments attachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID, currentUser.CurrentSubAccountId, proxy);
            
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: currentUser.EmployeeID, enableAttachments: true));
            var tableName = "custom_" + reqCustomEntity.entityid + "_attachments";
            try
            {
                cAttachment attachment = new cAttachment(0, Guid.NewGuid(), reqCustomEntity.entityid, "UT_" + methodBase.Name, methodBase.Name, @"c:\projects\filename.txt", cache.Values[0], createdOn, currentUser.EmployeeID, modifiedOn, modifiedBy, attachmentData);

                // Save attachment with id = zero
                attachmentid = attachments.saveAttachment(tableName, reqCustomEntity.getKeyField().displayname, 0, attachment, attachmentData);
                Assert.IsTrue(attachmentid > 0, "Attachment has not been saved.");

                // RemoveOrphans
                attachments.RemoveOrphanAttachmentsOnCustomTables(reqCustomEntity.entityid, currentUser.EmployeeID + 1, DateTime.Now.AddHours(1));

                // get attachment to check id - should return null
                var returnAttachment = attachments.getAttachment(tableName, attachmentid);
                Assert.IsTrue(returnAttachment != null, "An attachment has not been returned when it should have been.");
            }
            finally
            {
                // clear up
                attachments.deleteAttachment(tableName, attachmentid, reqCustomEntity.entityid, AttachDocumentType.None);
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        /// test for a custom entity, add attachment to new green light with id = zero then run remove orphan with time as now minus 2 hours so it is not deleted.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestCategory("Attachments"), TestMethod()]
        public void CustomEntityAddAttachmentToNewGreenlightThenRemoveOrphansFromTwoHoursAgo()
        {
            MethodBase methodBase = new StackFrame().GetMethod();

            // Set up Objects
            var currentUser = Moqs.CurrentUser();
            cMimeTypes mimeTypes = new cMimeTypes(currentUser.AccountID, currentUser.CurrentSubAccountId);
            SortedList<int, cMimeType> cache = mimeTypes.Cache["MimeTypes" + currentUser.AccountID] as SortedList<int, cMimeType>;
            DateTime createdOn = DateTime.Now;
            DateTime? modifiedOn = null;
            int? modifiedBy = null;
            byte[] attachmentData = { 1, 2, 3, 4 };
            var attachmentid = 0;
            int? proxy = currentUser.Delegate == null ? null : (int?)currentUser.Delegate.EmployeeID;
            cAttachments attachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID, currentUser.CurrentSubAccountId, proxy);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: currentUser.EmployeeID, enableAttachments: true));
            var tableName = "custom_" + reqCustomEntity.entityid + "_attachments";
            try
            {
                cAttachment attachment = new cAttachment(0, Guid.NewGuid(), reqCustomEntity.entityid, "UT_" + methodBase.Name, methodBase.Name, @"c:\projects\filename.txt", cache.Values[0], createdOn, currentUser.EmployeeID, modifiedOn, modifiedBy, attachmentData);

                // Save attachment with id = zero
                attachmentid = attachments.saveAttachment(tableName, reqCustomEntity.getKeyField().displayname, 0, attachment, attachmentData);
                Assert.IsTrue(attachmentid > 0, "Attachment has not been saved.");

                // RemoveOrphans
                attachments.RemoveOrphanAttachmentsOnCustomTables(reqCustomEntity.entityid, currentUser.EmployeeID, DateTime.Now.AddHours(-2));

                // get attachment to check id - should return null
                var returnAttachment = attachments.getAttachment(tableName, attachmentid);
                Assert.IsTrue(returnAttachment != null, "An attachment has not been returned when it should have been.");
            }
            finally
            {
                // clear up
                attachments.deleteAttachment(tableName, attachmentid, reqCustomEntity.entityid, AttachDocumentType.None);
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        /// test for a custom entity, attempt to add an attachment to a table that does not exist.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestCategory("Attachments"), TestMethod()]
        public void CustomEntityAddAttachmentToNonExistantTable()
        {
            MethodBase methodBase = new StackFrame().GetMethod();

            // Set up Objects
            var currentUser = Moqs.CurrentUser();
            cMimeTypes mimeTypes = new cMimeTypes(currentUser.AccountID, currentUser.CurrentSubAccountId);
            SortedList<int, cMimeType> cache = mimeTypes.Cache["MimeTypes" + currentUser.AccountID] as SortedList<int, cMimeType>;
            DateTime createdOn = DateTime.Now;
            DateTime? modifiedOn = null;
            int? modifiedBy = null;
            byte[] attachmentData = { 1, 2, 3, 4 };
            var attachmentid = 0;
            int? proxy = currentUser.Delegate == null ? null : (int?)currentUser.Delegate.EmployeeID;
            cAttachments attachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID, currentUser.CurrentSubAccountId, proxy);

            var tableName = "custom_nonexistant_attachments";
            cAttachment attachment = new cAttachment(0, Guid.NewGuid(), 1, "UT_" + methodBase.Name, methodBase.Name, @"c:\projects\filename.txt", cache.Values[0], createdOn, currentUser.EmployeeID, modifiedOn, modifiedBy, attachmentData);

            // Save attachment with id = zero
            attachmentid = attachments.saveAttachment(tableName, "id", 0, attachment, attachmentData);
            Assert.IsTrue(attachmentid == 0, "Attachment has been saved and it should have been rejected.");
        }

        /// <summary>
        /// test for a custom entity, attempts to clear orphan attachments from a green light that has not got attachments enabled
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestCategory("Attachments"), TestMethod()]
        public void CustomEntityClearOrphansFromGreenlightWithoutAttachments()
        {
            MethodBase methodBase = new StackFrame().GetMethod();

            // Set up Objects
            var currentUser = Moqs.CurrentUser();
            cMimeTypes mimeTypes = new cMimeTypes(currentUser.AccountID, currentUser.CurrentSubAccountId);
            SortedList<int, cMimeType> cache = mimeTypes.Cache["MimeTypes" + currentUser.AccountID] as SortedList<int, cMimeType>;
            DateTime createdOn = DateTime.Now;
            DateTime? modifiedOn = null;
            int? modifiedBy = null;
            byte[] attachmentData = { 1, 2, 3, 4 };
            var attachmentid = 0;
            int? proxy = currentUser.Delegate == null ? null : (int?)currentUser.Delegate.EmployeeID;
            cAttachments attachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID, currentUser.CurrentSubAccountId, proxy);
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: currentUser.EmployeeID, enableAttachments: false));
            var tableName = "custom_" + reqCustomEntity.entityid + "_attachments";
            try
            {
                cAttachment attachment = new cAttachment(0, Guid.NewGuid(), reqCustomEntity.entityid, "UT_" + methodBase.Name, methodBase.Name, @"c:\projects\filename.txt", cache.Values[0], createdOn, currentUser.EmployeeID, modifiedOn, modifiedBy, attachmentData);

                // Save attachment with id = zero
                attachmentid = attachments.saveAttachment(tableName, reqCustomEntity.getKeyField().displayname, 0, attachment, attachmentData);
                Assert.IsTrue(attachmentid == 0, "Attachment has been saved and should not has been.");

                // RemoveOrphans
                attachments.RemoveOrphanAttachmentsOnCustomTables(reqCustomEntity.entityid, currentUser.EmployeeID, DateTime.Now.AddHours(-1));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                // clear up
                attachments.deleteAttachment(tableName, attachmentid, reqCustomEntity.entityid, AttachDocumentType.None);
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for valid extension type is defined as a global, or non global mime type 
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestCategory("Attachments"), TestMethod()]
        public void CustomEntityCheckMimeTypeValid()
        {  
            cGlobalMimeType gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();
            cMimeType mime = cMimeTypeObject.CreateMimeType(gMime);

            try
            {
                var currentUser = Moqs.CurrentUser();
                int? proxy = currentUser.Delegate == null ? null : (int?)currentUser.Delegate.EmployeeID;
                cAttachments attachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID, currentUser.CurrentSubAccountId, proxy);
                Assert.IsNotNull(attachments);
                cMimeType mimeType = attachments.checkMimeType(gMime.FileExtension);
                Assert.IsNotNull(mimeType);             
            }
            finally
            {
                if (gMime != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(gMime.GlobalMimeID);             
                }
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }

        /// <summary>
        ///A test for a invalid extension type is not defined as a global, or non global mime type  
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestCategory("Attachments"), TestMethod()]
        public void CustomEntityCheckMimeTypeInvalid()
        {
            cGlobalMimeType gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();
            cMimeType mime = cMimeTypeObject.CreateMimeType(gMime);
            try
            {
                var currentUser = Moqs.CurrentUser();
                int? proxy = currentUser.Delegate == null ? null : (int?)currentUser.Delegate.EmployeeID;
                cAttachments attachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID, currentUser.CurrentSubAccountId, proxy);
                Assert.IsNotNull(attachments);
                cMimeType mimeType = attachments.checkMimeType(gMime.FileExtension + "invalid");
                Assert.IsNull(mimeType);
            }
            finally
            {
                if (gMime != null)
                {
                    cGlobalMimeTypeObject.DeleteGlobalMimeType(gMime.GlobalMimeID);
                }
                if (mime != null)
                {
                    cMimeTypeObject.DeleteMimeType(mime.MimeID);
                }
            }
        }

        #endregion

        #region Field Level Attachments

        /// <summary>
        ///A test for cAttachments Constructor
        ///</summary>    
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"),
         TestCategory("FieldLevelAttachments"), TestMethod()]
        public void CustomEntityAttachmentsConstructor()
        {
            var currentUser = Moqs.CurrentUser();
            MethodBase methodBase = new StackFrame().GetMethod();    
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: currentUser.EmployeeID, enableAttachments: false));
            Assert.IsNotNull(reqCustomEntity);
          
            try
            {              
                byte[] fileData = {1, 2, 3, 4};
                cAttribute att = reqCustomEntity.getAuditIdentifier();
                cAttachments attachments = new cAttachments(reqCustomEntity.entityid, att.attributeid, fileData, Guid.NewGuid().ToString(), "UT_" + methodBase.Name, "jpg");
                Assert.IsNotNull(attachments);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                // clear up
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test that file can be read and converted into a byte array
        ///</summary>    
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"),
         TestCategory("FieldLevelAttachments"), TestMethod()]
        public void CustomEntityTestGetFileData()
        {
            var currentUser = Moqs.CurrentUser();
            int? proxy = currentUser.Delegate == null ? null : (int?)currentUser.Delegate.EmployeeID;
            cAttachments attachments = new cAttachments(currentUser.AccountID, currentUser.EmployeeID,
                currentUser.CurrentSubAccountId, proxy);
            Assert.IsNotNull(attachments);
            try
            {
                byte[] buffer;
                using (Stream fs = File.OpenRead(GlobalTestVariables.ImagesPath + "/lunch receipt.jpg"))
                {
                    buffer = attachments.getFileData(fs);
                    Assert.IsTrue(buffer.Length > 0);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        ///A test to save attachment data to the db.
        ///</summary>    
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"),
         TestCategory("FieldLevelAttachments"), TestMethod()]
        public void CustomEntiySaveFieldLevelAttachment()
        {
            var currentUser = Moqs.CurrentUser();
            MethodBase methodBase = new StackFrame().GetMethod();
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: currentUser.EmployeeID, enableAttachments: false));
            Assert.IsNotNull(reqCustomEntity);
            try
            {
                byte[] fileData = { 1, 2, 3, 4 };
                cAttribute att = reqCustomEntity.getAuditIdentifier();
                cAttachments attachments = new cAttachments(reqCustomEntity.entityid, att.attributeid, fileData, Guid.NewGuid().ToString(), "UT_" + methodBase.Name, "jpg");
                Assert.IsNotNull(attachments);
                int outcome = attachments.saveAttachmentData();
                outcome = attachments.saveAttachmentData();
                Assert.IsTrue(outcome == 1, "Attachment data has been saved and it should have been rejected.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                // clear up
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }
        #endregion 
    }
}
