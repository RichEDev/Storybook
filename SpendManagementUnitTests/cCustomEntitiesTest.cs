using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using System;
using Moq;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cCustomEntitiesTest and is intended
    ///to contain all cCustomEntitiesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cCustomEntitiesTest
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


        #region older tests
        ///// <summary>
        /////A test for saveEntity
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //public void saveEntityTest()
        //{
        //    Moqs Moqs = new Moqs();

        //    int accountid = cGlobalVariables.AccountID;
        //    cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
        //    cCustomEntity entity = new cCustomEntity(0, "New Entity", "New Entities", "A description", DateTime.Now, 517, null, null, new SortedList<int, cAttribute>(), new SortedList<int, cCustomEntityForm>(), new SortedList<int, cCustomEntityView>(), null, null, false, false, true, false, null, null, false, null);
           

        //    int actual;

        //    //add a new entity
            
        //    actual = target.saveEntity(entity);
        //    System.Diagnostics.Debug.WriteLine("id=" + actual);
        //    Assert.IsTrue(actual > 0, "New record not added successfully");

        //    System.Threading.Thread.Sleep(4000);
        //    target = new cCustomEntities(Moqs.CurrentUser());
        //    cCustomEntity newrecord = target.getEntityById(actual);
            
        //    Assert.AreEqual(entity.entityname, newrecord.entityname);
        //    Assert.IsNotNull(newrecord.createdon);
        //    Assert.AreEqual(entity.createdby, newrecord.createdby);
        //    Assert.AreEqual(entity.description, newrecord.description);
        //    Assert.AreEqual(entity.pluralname, newrecord.pluralname);
            

        //    //add the same entity again, should return -1
        //    actual = target.saveEntity(entity);
        //    Assert.AreEqual(-1, actual, "Error, duplicate record allowed");
            
        //    //update an existing entity
        //    cCustomEntity existingentity = target.getEntityById(13);
        //    entity = new cCustomEntity(existingentity.entityid, "Changed Name", "Changed Names", "Changed Description", existingentity.createdon, existingentity.createdby, DateTime.Now, 517, new SortedList<int, cAttribute>(), new SortedList<int, cCustomEntityForm>(), new SortedList<int, cCustomEntityView>(), existingentity.table, existingentity.AudienceTable, true, true, true, false, null, null, false, null);
        //    actual = target.saveEntity(entity);
        //    Assert.IsTrue(actual > 0, "Record not updated successfully");
        //}

        ///// <summary>
        /////A test for deleteEntity
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //public void deleteEntityTest()
        //{
        //    Moqs Moqs = new Moqs();

        //    int accountid = cGlobalVariables.AccountID;
        //    cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
        //    int entityid = 10;
        //    target.deleteEntity(entityid, cGlobalVariables.EmployeeID, 0);
        //}

        ///// <summary>
        /////A test for getEntityById
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //public void getEntityByIdTest()
        //{
        //    Moqs Moqs = new Moqs();
            
        //    int accountid = cGlobalVariables.AccountID;
        //    cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
        //    int id = 13;
        //    cCustomEntity expected = new cCustomEntity(13, "Patent", "Patents", "", new DateTime(2008, 11, 16, 14, 48, 14), 517, null, null, null, null, null, null, null, false, false, true, false, null, null, false, null);
        //    cCustomEntity actual;
        //    actual = target.getEntityById(id);
        //    Assert.AreEqual(expected.description, actual.description);
        //    Assert.AreEqual(expected.entityid, actual.entityid);
        //    Assert.AreEqual(expected.entityname, actual.entityname);
        //    Assert.AreEqual(expected.createdby, actual.createdby);
        //    Assert.AreEqual(expected.createdon.ToLongDateString(), actual.createdon.ToLongDateString());
            
        //}

        ///// <summary>
        /////A test for saveAttribute
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //public void saveAttributeTest()
        //{
        //    Moqs Moqs = new Moqs();

        //    int accountid = cGlobalVariables.AccountID;
        //    cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
        //    cCustomEntity entity;
        //    int entityid = 13;
           
        //    //add a text attribute
        //    cAttribute attribute = new cTextAttribute(0, "Single Line Text", "Single Line Text", "Single Line Text Description", "Single Line Text Tooltip", false, FieldType.Text, DateTime.Now, 517, null, null, 50, AttributeFormat.SingleLine, Guid.Empty, false, false, false, false);
            
        //    int actual = target.saveAttribute(entityid, attribute);
        //    //give cache time to catch up
        //    System.Threading.Thread.Sleep(4000);
        //    target = new cCustomEntities(Moqs.CurrentUser());
        //    entity = target.getEntityById(entityid);

        //    cAttribute newattribute = entity.getAttributeById(actual);
        //    Assert.AreEqual(attribute.attributename, newattribute.attributename);
        //    Assert.AreEqual(attribute.createdby, newattribute.createdby);
        //    Assert.AreEqual(attribute.createdon.ToLongDateString(), newattribute.createdon.ToLongDateString());
        //    Assert.AreEqual(attribute.description, newattribute.description);
        //    Assert.AreEqual(attribute.displayname, newattribute.displayname);
        //    Assert.AreEqual(attribute.fieldtype, newattribute.fieldtype);
        //    Assert.AreEqual(attribute.mandatory, newattribute.mandatory);
        //    Assert.AreEqual(attribute.tooltip, newattribute.tooltip);

        //    //add an integer attribute
        //    attribute = new cNumberAttribute(0, "Integer", "Integer", "Integer Description", "Integer Tooltip", false, FieldType.Integer, DateTime.Now, 517, null, null, 0, Guid.Empty, false, false, false, false, false);

        //    actual = target.saveAttribute(entityid, attribute);
        //    //give cache time to catch up
        //    System.Threading.Thread.Sleep(4000);
        //    target = new cCustomEntities(Moqs.CurrentUser());
        //    entity = target.getEntityById(entityid);

        //    newattribute = entity.getAttributeById(actual);
        //    Assert.AreEqual(attribute.attributename, newattribute.attributename);
        //    Assert.AreEqual(attribute.createdby, newattribute.createdby);
        //    Assert.AreEqual(attribute.createdon.ToLongDateString(), newattribute.createdon.ToLongDateString());
        //    Assert.AreEqual(attribute.description, newattribute.description);
        //    Assert.AreEqual(attribute.displayname, newattribute.displayname);
        //    Assert.AreEqual(attribute.fieldtype, newattribute.fieldtype);
        //    Assert.AreEqual(attribute.mandatory, newattribute.mandatory);
        //    Assert.AreEqual(attribute.tooltip, newattribute.tooltip);

        //    //add a decimal attribute
        //    attribute = new cNumberAttribute(0, "Decimal", "Decimal", "Decimal Description", "Decimal Tooltip", false, FieldType.Number, DateTime.Now, 517, null, null, 2, Guid.Empty, false, false, false, false, false);

        //    actual = target.saveAttribute(entityid, attribute);
        //    //give cache time to catch up
        //    System.Threading.Thread.Sleep(4000);
        //    target = new cCustomEntities(Moqs.CurrentUser());
        //    entity = target.getEntityById(entityid);

        //    newattribute = entity.getAttributeById(actual);
        //    Assert.AreEqual(attribute.attributename, newattribute.attributename);
        //    Assert.AreEqual(attribute.createdby, newattribute.createdby);
        //    Assert.AreEqual(attribute.createdon.ToLongDateString(), newattribute.createdon.ToLongDateString());
        //    Assert.AreEqual(attribute.description, newattribute.description);
        //    Assert.AreEqual(attribute.displayname, newattribute.displayname);
        //    Assert.AreEqual(attribute.fieldtype, newattribute.fieldtype);
        //    Assert.AreEqual(attribute.mandatory, newattribute.mandatory);
        //    Assert.AreEqual(attribute.tooltip, newattribute.tooltip);
        //    Assert.AreEqual(((cNumberAttribute)attribute).precision, ((cNumberAttribute)newattribute).precision);

        //    //add a currency attribute
        //    attribute = new cNumberAttribute(0, "Currency", "Currency", "Currency Description", "Currency Tooltip", false, FieldType.Currency, DateTime.Now, 517, null, null, 2, Guid.Empty, false, false, false, false, false);

        //    actual = target.saveAttribute(entityid, attribute);
        //    //give cache time to catch up
        //    System.Threading.Thread.Sleep(4000);
        //    target = new cCustomEntities(Moqs.CurrentUser());
        //    entity = target.getEntityById(entityid);

        //    newattribute = entity.getAttributeById(actual);
        //    Assert.AreEqual(attribute.attributename, newattribute.attributename);
        //    Assert.AreEqual(attribute.createdby, newattribute.createdby);
        //    Assert.AreEqual(attribute.createdon.ToLongDateString(), newattribute.createdon.ToLongDateString());
        //    Assert.AreEqual(attribute.description, newattribute.description);
        //    Assert.AreEqual(attribute.displayname, newattribute.displayname);
        //    Assert.AreEqual(attribute.fieldtype, newattribute.fieldtype);
        //    Assert.AreEqual(attribute.mandatory, newattribute.mandatory);
        //    Assert.AreEqual(attribute.tooltip, newattribute.tooltip);
        //    Assert.AreEqual(2, ((cNumberAttribute)newattribute).precision);

        //    //add a yes/np attribute
        //    attribute = new cTickboxAttribute(0, "Yes/No Name", "Yes/No Display", "Yes/No Description", "Yes/No Tooltip", false, FieldType.TickBox, DateTime.Now, 517, null, null, "Yes", Guid.Empty, false, false, false, false);

        //    actual = target.saveAttribute(entityid, attribute);
        //    //give cache time to catch up
        //    System.Threading.Thread.Sleep(4000);
        //    target = new cCustomEntities(Moqs.CurrentUser());
        //    entity = target.getEntityById(entityid);

        //    newattribute = entity.getAttributeById(actual);
        //    Assert.AreEqual(attribute.attributename, newattribute.attributename);
        //    Assert.AreEqual(attribute.createdby, newattribute.createdby);
        //    Assert.AreEqual(attribute.createdon.ToLongDateString(), newattribute.createdon.ToLongDateString());
        //    Assert.AreEqual(attribute.description, newattribute.description);
        //    Assert.AreEqual(attribute.displayname, newattribute.displayname);
        //    Assert.AreEqual(attribute.fieldtype, newattribute.fieldtype);
        //    Assert.AreEqual(attribute.mandatory, newattribute.mandatory);
        //    Assert.AreEqual(attribute.tooltip, newattribute.tooltip);
        //    Assert.AreEqual(((cTickboxAttribute)attribute).defaultvalue, ((cTickboxAttribute)newattribute).defaultvalue);

        //    //add a list attribute
        //    SortedList<int, cListAttributeElement> listitems = new SortedList<int, cListAttributeElement>();
        //    listitems.Add(1, new cListAttributeElement( 1, "List Item 1",1));
        //    listitems.Add(2, new cListAttributeElement( 2, "List Item 2",2));
        //    attribute = new cListAttribute(0, "List Name", "List Display", "List Description", "List Tooltip", false, FieldType.List, DateTime.Now, 517, null, null, listitems, new Guid(), false,false, false, false);
            
        //    actual = target.saveAttribute(entityid, attribute);
        //    //give cache time to catch up
        //    System.Threading.Thread.Sleep(4000);
        //    target = new cCustomEntities(Moqs.CurrentUser());
        //    entity = target.getEntityById(entityid);

        //    newattribute = entity.getAttributeById(actual);
        //    Assert.AreEqual(attribute.attributename, newattribute.attributename);
        //    Assert.AreEqual(attribute.createdby, newattribute.createdby);
        //    Assert.AreEqual(attribute.createdon.ToLongDateString(), newattribute.createdon.ToLongDateString());
        //    Assert.AreEqual(attribute.description, newattribute.description);
        //    Assert.AreEqual(attribute.displayname, newattribute.displayname);
        //    Assert.AreEqual(attribute.fieldtype, newattribute.fieldtype);
        //    Assert.AreEqual(attribute.mandatory, newattribute.mandatory);
        //    Assert.AreEqual(attribute.tooltip, newattribute.tooltip);
        //    CollectionAssert.AreEquivalent(((cListAttribute)attribute).items, ((cListAttribute)newattribute).items);
            
        //    //add a Short Date attribute

        //    attribute = new cDateTimeAttribute(0, "Short Date Name", "Short Date Display", "Short Date Description", "Short Date Tooltip", false, FieldType.DateTime, DateTime.Now, 517, null, null, AttributeFormat.DateOnly, Guid.Empty, false, false, false, false);

        //    actual = target.saveAttribute(entityid, attribute);
        //    //give cache time to catch up
        //    System.Threading.Thread.Sleep(4000);
        //    target = new cCustomEntities(Moqs.CurrentUser());
        //    entity = target.getEntityById(entityid);

        //    newattribute = entity.getAttributeById(actual);
        //    Assert.AreEqual(attribute.attributename, newattribute.attributename);
        //    Assert.AreEqual(attribute.createdby, newattribute.createdby);
        //    Assert.AreEqual(attribute.createdon.ToLongDateString(), newattribute.createdon.ToLongDateString());
        //    Assert.AreEqual(attribute.description, newattribute.description);
        //    Assert.AreEqual(attribute.displayname, newattribute.displayname);
        //    Assert.AreEqual(attribute.fieldtype, newattribute.fieldtype);
        //    Assert.AreEqual(attribute.mandatory, newattribute.mandatory);
        //    Assert.AreEqual(attribute.tooltip, newattribute.tooltip);
        //    Assert.AreEqual(((cDateTimeAttribute)attribute).format, ((cDateTimeAttribute)newattribute).format);

        //    //add a Time attribute

        //    attribute = new cDateTimeAttribute(0, "Time Name", "Time Display", "Time Description", "Time Tooltip", false, FieldType.DateTime, DateTime.Now, 517, null, null, AttributeFormat.TimeOnly, Guid.Empty, false, false, false, false);

        //    actual = target.saveAttribute(entityid, attribute);
        //    //give cache time to catch up
        //    System.Threading.Thread.Sleep(4000);
        //    target = new cCustomEntities(Moqs.CurrentUser());
        //    entity = target.getEntityById(entityid);

        //    newattribute = entity.getAttributeById(actual);
        //    Assert.AreEqual(attribute.attributename, newattribute.attributename);
        //    Assert.AreEqual(attribute.createdby, newattribute.createdby);
        //    Assert.AreEqual(attribute.createdon.ToLongDateString(), newattribute.createdon.ToLongDateString());
        //    Assert.AreEqual(attribute.description, newattribute.description);
        //    Assert.AreEqual(attribute.displayname, newattribute.displayname);
        //    Assert.AreEqual(attribute.fieldtype, newattribute.fieldtype);
        //    Assert.AreEqual(attribute.mandatory, newattribute.mandatory);
        //    Assert.AreEqual(attribute.tooltip, newattribute.tooltip);
        //    Assert.AreEqual(((cDateTimeAttribute)attribute).format, ((cDateTimeAttribute)newattribute).format);
            
        //    //add a DateTime attribute

        //    attribute = new cDateTimeAttribute(0, "Date/Time Name", "Date/Time Display", "Date/Time Description", "Date/Time Tooltip", false, FieldType.DateTime, DateTime.Now, 517, null, null, AttributeFormat.DateTime, Guid.Empty, false, false, false, false);

        //    actual = target.saveAttribute(entityid, attribute);
        //    //give cache time to catch up
        //    System.Threading.Thread.Sleep(4000);
        //    target = new cCustomEntities(Moqs.CurrentUser());
        //    entity = target.getEntityById(entityid);

        //    newattribute = entity.getAttributeById(actual);
        //    Assert.AreEqual(attribute.attributename, newattribute.attributename);
        //    Assert.AreEqual(attribute.createdby, newattribute.createdby);
        //    Assert.AreEqual(attribute.createdon.ToLongDateString(), newattribute.createdon.ToLongDateString());
        //    Assert.AreEqual(attribute.description, newattribute.description);
        //    Assert.AreEqual(attribute.displayname, newattribute.displayname);
        //    Assert.AreEqual(attribute.fieldtype, newattribute.fieldtype);
        //    Assert.AreEqual(attribute.mandatory, newattribute.mandatory);
        //    Assert.AreEqual(attribute.tooltip, newattribute.tooltip);
        //    Assert.AreEqual(((cDateTimeAttribute)attribute).format, ((cDateTimeAttribute)newattribute).format);

        //    //add a Large attribute

        //    attribute = new cTextAttribute(0, "Large Text Name", "Large Text Display", "Large Text Description", "Large Text Tooltip", false, FieldType.LargeText, DateTime.Now, 517, null, null, null, AttributeFormat.MultiLine, Guid.Empty, false, false, false, false);

        //    actual = target.saveAttribute(entityid, attribute);
        //    //give cache time to catch up
        //    System.Threading.Thread.Sleep(4000);
        //    target = new cCustomEntities(Moqs.CurrentUser());
        //    entity = target.getEntityById(entityid);

        //    newattribute = entity.getAttributeById(actual);
        //    Assert.AreEqual(attribute.attributename, newattribute.attributename);
        //    Assert.AreEqual(attribute.createdby, newattribute.createdby);
        //    Assert.AreEqual(attribute.createdon.ToLongDateString(), newattribute.createdon.ToLongDateString());
        //    Assert.AreEqual(attribute.description, newattribute.description);
        //    Assert.AreEqual(attribute.displayname, newattribute.displayname);
        //    Assert.AreEqual(attribute.fieldtype, newattribute.fieldtype);
        //    Assert.AreEqual(attribute.mandatory, newattribute.mandatory);
        //    Assert.AreEqual(attribute.tooltip, newattribute.tooltip);
        //    Assert.AreEqual(((cTextAttribute)attribute).format, ((cTextAttribute)newattribute).format);
        //    Assert.AreEqual(((cTextAttribute)attribute).maxlength, ((cTextAttribute)newattribute).maxlength);
            
        //}
        #endregion older tests

        /// <summary>
        ///A test for cCustomEntities Constructor
        ///</summary>
        [TestMethod()]
        public void cCustomEntities_cCustomEntitiesConstructor_WithoutParameters()
        {
            cCustomEntities target = new cCustomEntities();
            Assert.IsNull(target.CustomEntities);
        }

        /// <summary>
        ///A test for cCustomEntities Constructor
        ///</summary>
        [TestMethod()]
        public void cCustomEntities_cCustomEntitiesConstructor_WithParameters()
        {
            ICurrentUser currentUser = new Moqs().CurrentUser();
            cCustomEntities target = new cCustomEntities(currentUser);
            Assert.IsNotNull(target.CustomEntities);
        }

        /// <summary>
        ///A test for GetAudienceRecords
        ///</summary>
        [TestMethod()]
        public void cCustomEntities_GetAudienceRecords_NoValues()
        {
            cCustomEntities target = new cCustomEntities(new Moqs().CurrentUser());
            int entityID = 0;
            int employeeID = 0;
            int recordID = 0;
            SerializableDictionary<string, object> expected = null;
            SerializableDictionary<string, object> actual;
            actual = target.GetAudienceRecords(entityID, employeeID, recordID);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SortFormFieldOrder
        ///</summary>
        [TestMethod()]
        public void cCustomEntities_SortFormFieldOrder_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            List<sCEFieldDetails> lstFields = null; // TODO: Initialize to an appropriate value
            List<sCEFieldDetails> lstFieldsExpected = null; // TODO: Initialize to an appropriate value
            target.SortFormFieldOrder(ref lstFields);
            Assert.AreEqual(lstFieldsExpected, lstFields);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SortList
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void cCustomEntities_SortList_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            SortedList<string, cCustomEntity> expected = null; // TODO: Initialize to an appropriate value
            SortedList<string, cCustomEntity> actual;
            actual = target.SortList();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for clsOTMgrid_InitialiseRow
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void cCustomEntities_clsOTMgrid_InitialiseRow_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            cNewGridRow row = null; // TODO: Initialize to an appropriate value
            SerializableDictionary<string, object> gridInfo = null; // TODO: Initialize to an appropriate value
            target.clsOTMgrid_InitialiseRow(row, gridInfo);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for createAttributeGrid
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_createAttributeGrid_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            cCustomEntity entity = null; // TODO: Initialize to an appropriate value
            string[] expected = null; // TODO: Initialize to an appropriate value
            string[] actual;
            actual = target.createAttributeGrid(entity);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for createDerivedTable
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_createDerivedTable_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int presaveAttributeId = 0; // TODO: Initialize to an appropriate value
            int relationshipAttributeId = 0; // TODO: Initialize to an appropriate value
            cCustomEntity entity = null; // TODO: Initialize to an appropriate value
            cTable relatedtable = null; // TODO: Initialize to an appropriate value
            cCustomEntity relatedentity = null; // TODO: Initialize to an appropriate value
            target.createDerivedTable(employeeid, presaveAttributeId, relationshipAttributeId, entity, relatedtable, relatedentity);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for createFormGrid
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_createFormGrid_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            cCustomEntity entity = null; // TODO: Initialize to an appropriate value
            string[] expected = null; // TODO: Initialize to an appropriate value
            string[] actual;
            actual = target.createFormGrid(entity);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for createParentEntityReportConfig
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_createParentEntityReportConfig_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            cCustomEntity startEntity = null; // TODO: Initialize to an appropriate value
            cCustomEntity curEntity = null; // TODO: Initialize to an appropriate value
            int level = 0; // TODO: Initialize to an appropriate value
            Dictionary<int, cCustomEntity> recHistory = null; // TODO: Initialize to an appropriate value
            Dictionary<int, cCustomEntity> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<int, cCustomEntity> actual;
            actual = target.createParentEntityReportConfig(startEntity, curEntity, level, recHistory);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for createViewGrid
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_createViewGrid_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            cCustomEntity entity = null; // TODO: Initialize to an appropriate value
            string[] expected = null; // TODO: Initialize to an appropriate value
            string[] actual;
            actual = target.createViewGrid(entity);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for deleteAttribute
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_deleteAttribute_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int attributeid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int delegateid = 0; // TODO: Initialize to an appropriate value
            target.deleteAttribute(attributeid, employeeid, delegateid);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for deleteEntity
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_deleteEntity_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int entityid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int delegateid = 0; // TODO: Initialize to an appropriate value
            target.deleteEntity(entityid, employeeid, delegateid);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for deleteForm
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_deleteForm_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int formid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int delegateid = 0; // TODO: Initialize to an appropriate value
            target.deleteForm(formid, employeeid, delegateid);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for deleteView
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_deleteView_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int viewid = 0; // TODO: Initialize to an appropriate value
            int employeeid = 0; // TODO: Initialize to an appropriate value
            int delegateid = 0; // TODO: Initialize to an appropriate value
            target.deleteView(viewid, employeeid, delegateid);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_generateFields_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            PlaceHolder pnlSection = null; // TODO: Initialize to an appropriate value
            PlaceHolder pnlSectionExpected = null; // TODO: Initialize to an appropriate value
            cCustomEntityFormSection section = null; // TODO: Initialize to an appropriate value
            cCustomEntity entity = null; // TODO: Initialize to an appropriate value
            int viewid = 0; // TODO: Initialize to an appropriate value
            cCustomEntityForm form = null; // TODO: Initialize to an appropriate value
            int activeTabId = 0; // TODO: Initialize to an appropriate value
            List<string> otmTableID = null; // TODO: Initialize to an appropriate value
            List<string> otmTableIDExpected = null; // TODO: Initialize to an appropriate value
            List<string> summaryTableID = null; // TODO: Initialize to an appropriate value
            List<string> summaryTableIDExpected = null; // TODO: Initialize to an appropriate value
            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID);
            Assert.AreEqual(pnlSectionExpected, pnlSection);
            Assert.AreEqual(otmTableIDExpected, otmTableID);
            Assert.AreEqual(summaryTableIDExpected, summaryTableID);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for generateForm
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_generateForm_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            cCustomEntity entity = null; // TODO: Initialize to an appropriate value
            int formid = 0; // TODO: Initialize to an appropriate value
            int viewid = 0; // TODO: Initialize to an appropriate value
            Nullable<int> recordid = new Nullable<int>(); // TODO: Initialize to an appropriate value
            int tabid = 0; // TODO: Initialize to an appropriate value
            List<sEntityBreadCrumb> crumbs = null; // TODO: Initialize to an appropriate value
            Dictionary<int, List<string>> otmTableID = null; // TODO: Initialize to an appropriate value
            Dictionary<int, List<string>> otmTableIDExpected = null; // TODO: Initialize to an appropriate value
            Dictionary<int, List<string>> summaryTableID = null; // TODO: Initialize to an appropriate value
            Dictionary<int, List<string>> summaryTableIDExpected = null; // TODO: Initialize to an appropriate value
            Panel expected = null; // TODO: Initialize to an appropriate value
            Panel actual;
            actual = target.generateForm(entity, formid, viewid, recordid, tabid, crumbs, ref otmTableID, ref summaryTableID);
            Assert.AreEqual(otmTableIDExpected, otmTableID);
            Assert.AreEqual(summaryTableIDExpected, summaryTableID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for generateOneToManyGrid
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_generateOneToManyGrid_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            cOneToManyRelationship onetomany = null; // TODO: Initialize to an appropriate value
            int id = 0; // TODO: Initialize to an appropriate value
            int viewid = 0; // TODO: Initialize to an appropriate value
            int tabid = 0; // TODO: Initialize to an appropriate value
            int formid = 0; // TODO: Initialize to an appropriate value
            int recid = 0; // TODO: Initialize to an appropriate value
            string relentityids = string.Empty; // TODO: Initialize to an appropriate value
            string relformids = string.Empty; // TODO: Initialize to an appropriate value
            string relrecordids = string.Empty; // TODO: Initialize to an appropriate value
            string reltabids = string.Empty; // TODO: Initialize to an appropriate value
            string[] expected = null; // TODO: Initialize to an appropriate value
            string[] actual;
            actual = target.generateOneToManyGrid(onetomany, id, viewid, tabid, formid, recid, relentityids, relformids, relrecordids, reltabids);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for generateSections
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_generateSections_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            TabPanel tabpnl = null; // TODO: Initialize to an appropriate value
            TabPanel tabpnlExpected = null; // TODO: Initialize to an appropriate value
            cCustomEntityFormTab tab = null; // TODO: Initialize to an appropriate value
            cCustomEntity entity = null; // TODO: Initialize to an appropriate value
            int viewid = 0; // TODO: Initialize to an appropriate value
            cCustomEntityForm form = null; // TODO: Initialize to an appropriate value
            int activeTabId = 0; // TODO: Initialize to an appropriate value
            List<string> otmTableID = null; // TODO: Initialize to an appropriate value
            List<string> otmTableIDExpected = null; // TODO: Initialize to an appropriate value
            List<string> summaryTableID = null; // TODO: Initialize to an appropriate value
            List<string> summaryTableIDExpected = null; // TODO: Initialize to an appropriate value
            target.generateSections(ref tabpnl, tab, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID);
            Assert.AreEqual(tabpnlExpected, tabpnl);
            Assert.AreEqual(otmTableIDExpected, otmTableID);
            Assert.AreEqual(summaryTableIDExpected, summaryTableID);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for generateSummaryGrid
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_generateSummaryGrid_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int entityId = 0; // TODO: Initialize to an appropriate value
            int attributeId = 0; // TODO: Initialize to an appropriate value
            int viewId = 0; // TODO: Initialize to an appropriate value
            int activeTab = 0; // TODO: Initialize to an appropriate value
            int formId = 0; // TODO: Initialize to an appropriate value
            int recordId = 0; // TODO: Initialize to an appropriate value
            string[] expected = null; // TODO: Initialize to an appropriate value
            string[] actual;
            actual = target.generateSummaryGrid(entityId, attributeId, viewId, activeTab, formId, recordId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for generateTabs
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_generateTabs_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            cCustomEntityForm form = null; // TODO: Initialize to an appropriate value
            cCustomEntity entity = null; // TODO: Initialize to an appropriate value
            int viewid = 0; // TODO: Initialize to an appropriate value
            int activeTabId = 0; // TODO: Initialize to an appropriate value
            Dictionary<int, List<string>> otmTableID = null; // TODO: Initialize to an appropriate value
            Dictionary<int, List<string>> otmTableIDExpected = null; // TODO: Initialize to an appropriate value
            Dictionary<int, List<string>> summaryTableID = null; // TODO: Initialize to an appropriate value
            Dictionary<int, List<string>> summaryTableIDExpected = null; // TODO: Initialize to an appropriate value
            TabContainer expected = null; // TODO: Initialize to an appropriate value
            TabContainer actual;
            actual = target.generateTabs(form, entity, viewid, activeTabId, ref otmTableID, ref summaryTableID);
            Assert.AreEqual(otmTableIDExpected, otmTableID);
            Assert.AreEqual(summaryTableIDExpected, summaryTableID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getAttributeByFieldId
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_getAttributeByFieldId_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            Guid fieldID = new Guid(); // TODO: Initialize to an appropriate value
            cAttribute expected = null; // TODO: Initialize to an appropriate value
            cAttribute actual;
            actual = target.getAttributeByFieldId(fieldID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getAttributeListItems
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_getAttributeListItems_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cListAttributeElement>> expected = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cListAttributeElement>> actual;
            actual = target.getAttributeListItems();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getAttributes
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_getAttributes_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cAttribute>> expected = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cAttribute>> actual;
            actual = target.getAttributes();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getEntityById
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_getEntityById_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int id = 0; // TODO: Initialize to an appropriate value
            cCustomEntity expected = null; // TODO: Initialize to an appropriate value
            cCustomEntity actual;
            actual = target.getEntityById(id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getEntityByTableId
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_getEntityByTableId_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            Guid id = new Guid(); // TODO: Initialize to an appropriate value
            cCustomEntity expected = null; // TODO: Initialize to an appropriate value
            cCustomEntity actual;
            actual = target.getEntityByTableId(id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getEntityIdByAttributeId
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_getEntityIdByAttributeId_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int attributeid = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.getEntityIdByAttributeId(attributeid);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getEntityRecord
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_getEntityRecord_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            cCustomEntity entity = null; // TODO: Initialize to an appropriate value
            int id = 0; // TODO: Initialize to an appropriate value
            cCustomEntityForm form = null; // TODO: Initialize to an appropriate value
            SortedList<int, object> expected = null; // TODO: Initialize to an appropriate value
            SortedList<int, object> actual;
            actual = target.getEntityRecord(entity, id, form);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getFormFields
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_getFormFields_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cCustomEntityFormSection>> lstsections = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cAttribute>> lstattributes = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cCustomEntityFormField>> expected = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cCustomEntityFormField>> actual;
            actual = target.getFormFields(lstsections, lstattributes);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getFormSections
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_getFormSections_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cCustomEntityFormTab>> lsttabs = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cCustomEntityFormSection>> expected = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cCustomEntityFormSection>> actual;
            actual = target.getFormSections(lsttabs);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getFormTabs
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_getFormTabs_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cCustomEntityFormTab>> expected = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cCustomEntityFormTab>> actual;
            actual = target.getFormTabs();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getForms
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_getForms_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cAttribute>> lstattributes = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cCustomEntityForm>> expected = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cCustomEntityForm>> actual;
            actual = target.getForms(lstattributes);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }


        /// <summary>
        ///A test for getParentBreadcrumb
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_getParentBreadcrumb_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int entityid = 0; // TODO: Initialize to an appropriate value
            sEntityBreadCrumb topCrumb = new sEntityBreadCrumb(); // TODO: Initialize to an appropriate value
            int id = 0; // TODO: Initialize to an appropriate value
            sEntityBreadCrumb expected = new sEntityBreadCrumb(); // TODO: Initialize to an appropriate value
            sEntityBreadCrumb actual;
            actual = target.getParentBreadcrumb(entityid, topCrumb, id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getRelationshipAttributes
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_getRelationshipAttributes_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int entityid = 0; // TODO: Initialize to an appropriate value
            int relationship_entityid = 0; // TODO: Initialize to an appropriate value
            Dictionary<int, cOneToManyRelationship> otmRelationships = null; // TODO: Initialize to an appropriate value
            Dictionary<int, cOneToManyRelationship> otmRelationshipsExpected = null; // TODO: Initialize to an appropriate value
            target.getRelationshipAttributes(entityid, relationship_entityid, ref otmRelationships);
            Assert.AreEqual(otmRelationshipsExpected, otmRelationships);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for getSummaryAttributeColumns
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_getSummaryAttributeColumns_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            int attributeid = 0; // TODO: Initialize to an appropriate value
            Dictionary<string, cSummaryAttributeColumn> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<string, cSummaryAttributeColumn> actual;
            actual = target.getSummaryAttributeColumns(attributeid);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getSummaryAttributeElements
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_getSummaryAttributeElements_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            int attributeid = 0; // TODO: Initialize to an appropriate value
            Dictionary<int, cSummaryAttributeElement> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<int, cSummaryAttributeElement> actual;
            actual = target.getSummaryAttributeElements(attributeid);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getSystemViewEntity
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_getSystemViewEntity_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int derived_entityid = 0; // TODO: Initialize to an appropriate value
            int parent_entityid = 0; // TODO: Initialize to an appropriate value
            cCustomEntity expected = null; // TODO: Initialize to an appropriate value
            cCustomEntity actual;
            actual = target.getSystemViewEntity(derived_entityid, parent_entityid);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getViewFields
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_getViewFields_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<byte, cField>> expected = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<byte, cField>> actual;
            actual = target.getViewFields();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getViewFilters
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_getViewFilters_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<byte, cCustomEntityViewFilter>> expected = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<byte, cCustomEntityViewFilter>> actual;
            actual = target.getViewFilters();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getViews
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_getViews_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cCustomEntityForm>> lstforms = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cCustomEntityView>> expected = null; // TODO: Initialize to an appropriate value
            SortedList<int, SortedList<int, cCustomEntityView>> actual;
            actual = target.getViews(lstforms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getViewsByMenuId
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_getViewsByMenuId_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int id = 0; // TODO: Initialize to an appropriate value
            List<cCustomEntityView> expected = null; // TODO: Initialize to an appropriate value
            List<cCustomEntityView> actual;
            actual = target.getViewsByMenuId(id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for lnkonetomany_Click
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_lnkonetomany_Click_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            EventArgs e = null; // TODO: Initialize to an appropriate value
            target.lnkonetomany_Click(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for refreshCache
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_refreshCache_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            target.refreshCache();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for saveAttribute
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_saveAttribute_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int entityid = 0; // TODO: Initialize to an appropriate value
            cAttribute attribute = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.saveAttribute(entityid, attribute);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for saveEntity
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_saveEntity_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            cCustomEntity entity = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.saveEntity(entity);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for saveEntitySystemView
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_saveEntitySystemView_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            cCustomEntity viewentity = null; // TODO: Initialize to an appropriate value
            int parententityid = 0; // TODO: Initialize to an appropriate value
            int relativeAttributeId = 0; // TODO: Initialize to an appropriate value
            cCustomEntity donorentity = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.saveEntitySystemView(viewentity, parententityid, relativeAttributeId, donorentity);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for saveForm
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_saveForm_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int entityid = 0; // TODO: Initialize to an appropriate value
            cCustomEntityForm form = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.saveForm(entityid, form);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for saveFormFields
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_saveFormFields_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            int formid = 0; // TODO: Initialize to an appropriate value
            cCustomEntityForm form = null; // TODO: Initialize to an appropriate value
            target.saveFormFields(formid, form);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for saveFormSections
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_saveFormSections_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            int formid = 0; // TODO: Initialize to an appropriate value
            cCustomEntityForm form = null; // TODO: Initialize to an appropriate value
            target.saveFormSections(formid, form);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for saveFormTabs
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_saveFormTabs_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            int formid = 0; // TODO: Initialize to an appropriate value
            cCustomEntityForm form = null; // TODO: Initialize to an appropriate value
            target.saveFormTabs(formid, form);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for saveListItems
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_saveListItems_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            int entityid = 0; // TODO: Initialize to an appropriate value
            int attributeID = 0; // TODO: Initialize to an appropriate value
            SortedList<int, cListAttributeElement> lstItems = null; // TODO: Initialize to an appropriate value
            bool isNew = false; // TODO: Initialize to an appropriate value
            target.saveListItems(entityid, attributeID, lstItems, isNew);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for saveRelationship
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_saveRelationship_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int entityid = 0; // TODO: Initialize to an appropriate value
            cAttribute attribute = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.saveRelationship(entityid, attribute);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for saveSummaryAttribute
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_saveSummaryAttribute_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int entityid = 0; // TODO: Initialize to an appropriate value
            cSummaryAttribute attribute = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.saveSummaryAttribute(entityid, attribute);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for saveView
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_saveView_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int entityid = 0; // TODO: Initialize to an appropriate value
            cCustomEntityView view = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.saveView(entityid, view);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for saveViewFields
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        [DeploymentItem("Spend Management.dll")]
        public void cCustomEntities_saveViewFields_()
        {
            cCustomEntities_Accessor target = new cCustomEntities_Accessor(); // TODO: Initialize to an appropriate value
            int viewid = 0; // TODO: Initialize to an appropriate value
            SortedList<byte, cField> fields = null; // TODO: Initialize to an appropriate value
            target.saveViewFields(viewid, fields);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CustomEntities
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_CustomEntities_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            SortedList<int, cCustomEntity> actual;
            actual = target.CustomEntities;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for _accountid
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities__accountid_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target._accountid;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for activeDisplayTab
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        
        
        public void cCustomEntities_activeDisplayTab_()
        {
            cCustomEntities target = new cCustomEntities(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.activeDisplayTab = expected;
            actual = target.activeDisplayTab;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
