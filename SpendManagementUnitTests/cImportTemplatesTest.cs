using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using System.Xml.Linq;
using SpendManagementUnitTests.Global_Objects;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cImportTemplatesTest and is intended
    ///to contain all cImportTemplatesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cImportTemplatesTest
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
        [TestCleanup()]
        public void MyTestCleanup()
        {
            cESRTrustObject.DeleteTrust();
            cImportTemplateObject.DeleteImportTemplate();

            System.Web.HttpContext.Current.Session["myid"] = null;
            cEmployeeObject.DeleteDelegateUTEmployee();
        }
        
        #endregion


        /// <summary>
        ///A test for saveImportTemplateMappings
        ///</summary>
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Projects\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void saveImportTemplateMappingsTest()
        {
            

            
            //target.saveImportTemplateMappings(TemplateID, lstMappings);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test to add an import template and associated mappings to the database
        ///</summary>
        [TestMethod()]
        public void AddImportTemplateTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            cESRTrustObject.CreateESRTrustGlobalVariable();
            List<cImportTemplateMapping> lstMappings = new List<cImportTemplateMapping>();
            cImportTemplates target = new cImportTemplates(AccountID);

            #region Create Import template Mappings
            cFields clsfields = new cFields(cGlobalVariables.AccountID);
            //Employee Info
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("6a76898b-4052-416c-b870-61479ca15ac1"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employee Number", 2, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("28471060-247d-461c-abf6-234bcb4698aa"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Title", 3, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("9d70d151-5905-4a67-944f-1ad6d22cd931"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Last Name", 4, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("6614acad-0a43-4e30-90ec-84de0792b1d6"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "First Name", 5, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("b3caf703-e72b-4eb8-9d5c-b389e16c8c43"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Middle Names", 6, ImportElementType.Employee, false, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("76473c0a-df08-40f9-8de0-632d0111a912"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Hire Date", 11, ImportElementType.Employee, true, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("b7cbf994-4a23-4405-93df-d66d4c3ed2a3"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Termination Date", 12, ImportElementType.Employee, false, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("a0891ce2-d0c2-4b5b-9a78-7aaa3aaa87c1"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employee's Address 1st line", 14, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("0330c639-1524-402b-b7bc-04d26bfc05a1"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employee's Address Town", 16, ImportElementType.Employee, false, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("9c9f07dd-a9d0-4ccf-9231-dd3c10d491b8"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employees Address Postcode", 18, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("8816caec-b520-4223-b738-47d2f22f3e1a"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employees Address Country", 19, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("0f951c3e-29d1-49f0-ac13-4cfcabf21fda"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Office e-Mail", 24, ImportElementType.Employee, false, DataType.stringVal));

            //ESR Assignment Info
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("7c0d8eab-d9af-415f-9bb7-d1be01f69e2f"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment ID", 1, ImportElementType.Assignment, true, DataType.intVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("c23858b8-7730-440e-b481-c43fe8a1dbef"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Number", 2, ImportElementType.Assignment, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("c53828af-99ff-463f-93f9-2721df44e5f2"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Earliest Assignment Start Date", 3, ImportElementType.Assignment, false, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("36eb4bb6-f4d5-414c-9106-ee62db01d902"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Final Assignment End Date", 4, ImportElementType.Assignment, false, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("9721ec22-404b-468b-83a4-d17d7559d3ef"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Status", 5, ImportElementType.Assignment, false, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("3e5750f0-1061-46c4-ad94-089d40a62dec"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Address 1st Line", 9, ImportElementType.Assignment, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("737a99ad-b0c5-4325-a565-c4d3fba536dd"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Address Postcode", 13, ImportElementType.Assignment, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("96f11c6d-7615-4abd-94ec-0e4d34e187a0"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Supervisor Employee Number", 17, ImportElementType.Assignment, false, DataType.intVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("fec46ed7-57f9-4c51-9916-ec92834371c3"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Primary Assignment", 22, ImportElementType.Assignment, true, DataType.booleanVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("c50dae62-8dae-4289-a0ce-584c3129159e"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Location", 37, ImportElementType.Assignment, true, DataType.stringVal));

            //Linked to the employees table
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("5f4a4551-1c05-4c85-b6d9-06d036bc327e"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Job Name", 39, ImportElementType.Assignment, false, DataType.stringVal));

            #endregion

            cImportTemplate template = new cImportTemplate(0, "Unit Test Template", ApplicationType.ESROutboundImport, true, cGlobalVariables.NHSTrustID, lstMappings, DateTime.Now, cGlobalVariables.EmployeeID, null, null);

            int ID = target.saveImportTemplate(template);

            Assert.IsTrue(ID > 0);

            cGlobalVariables.TemplateID = ID;

            //target = new cImportTemplates(cGlobalVariables.AccountID);
            //System.Threading.Thread.Sleep(1000);

            cImportTemplate actual = target.getImportTemplateByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(template.TemplateName, actual.TemplateName);
            Assert.AreEqual(template.appType, actual.appType);
            Assert.AreEqual(template.IsAutomated, actual.IsAutomated);
            Assert.AreEqual(template.NHSTrustID, actual.NHSTrustID);

            #region Template mappings

            int index = 0;

            foreach(cImportTemplateMapping impTemp in template.Mappings)
            {
                Assert.AreEqual(ID, actual.Mappings[index].TemplateID);
                Assert.AreEqual(impTemp.FieldID, actual.Mappings[index].FieldID);
                Assert.AreEqual(impTemp.DestinationField, actual.Mappings[index].DestinationField);
                Assert.AreEqual(impTemp.ColRef, actual.Mappings[index].ColRef);
                Assert.AreEqual(impTemp.ElementType, actual.Mappings[index].ElementType);
                Assert.AreEqual(impTemp.Mandatory, actual.Mappings[index].Mandatory);
                Assert.AreEqual(impTemp.dataType, actual.Mappings[index].dataType);

                index++;
            }

            #endregion
        }

        /// <summary>
        /// Edit an import template and its associated mappings 
        /// </summary>
        [TestMethod()]
        public void EditImportTemplateTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            List<cImportTemplateMapping> lstMappings = new List<cImportTemplateMapping>();

            cImportTemplate template = cImportTemplateObject.CreateImportTemplate();
            cImportTemplates target = new cImportTemplates(AccountID);

             

            int ID = target.saveImportTemplate(new cImportTemplate(template.TemplateID, "Unit Test Template Updated", template.appType, template.IsAutomated, template.NHSTrustID, lstMappings, template.createdOn, template.createdBy, template.modifiedOn, template.modifiedBy));

            Assert.IsTrue(ID > 0);

            //System.Threading.Thread.Sleep(1000);
            //target = new cImportTemplates(cGlobalVariables.AccountID);
            
            cImportTemplate actual = target.getImportTemplateByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual("Unit Test Template Updated", actual.TemplateName);
            Assert.AreEqual(template.appType, actual.appType);
            Assert.AreEqual(template.IsAutomated, actual.IsAutomated);
            Assert.AreEqual(template.NHSTrustID, actual.NHSTrustID);

            #region Template mappings

            int index = 0;

            if (actual.Mappings.Count > 0)
            {
                foreach (cImportTemplateMapping impTemp in template.Mappings)
                {
                    Assert.AreEqual(ID, actual.Mappings[index].TemplateID);
                    Assert.AreEqual(impTemp.FieldID, actual.Mappings[index].FieldID);
                    Assert.AreEqual(impTemp.DestinationField, actual.Mappings[index].DestinationField);
                    Assert.AreEqual(impTemp.ColRef, actual.Mappings[index].ColRef);
                    Assert.AreEqual(impTemp.ElementType, actual.Mappings[index].ElementType);
                    Assert.AreEqual(impTemp.Mandatory, actual.Mappings[index].Mandatory);
                    Assert.AreEqual(impTemp.dataType, actual.Mappings[index].dataType);

                    index++;
                }
            }

            #endregion
        }

        /// <summary>
        ///A test to add an import template and associated mappings to the database as a delegate
        ///</summary>
        [TestMethod()]
        public void AddImportTemplateAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int AccountID = cGlobalVariables.AccountID;
            cESRTrustObject.CreateESRTrustGlobalVariable();
            List<cImportTemplateMapping> lstMappings = new List<cImportTemplateMapping>();
            cImportTemplates target = new cImportTemplates(AccountID);

            #region Create Import template Mappings
            cFields clsfields = new cFields(cGlobalVariables.AccountID);
            //Employee Info
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("6a76898b-4052-416c-b870-61479ca15ac1"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employee Number", 2, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("28471060-247d-461c-abf6-234bcb4698aa"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Title", 3, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("9d70d151-5905-4a67-944f-1ad6d22cd931"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Last Name", 4, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("6614acad-0a43-4e30-90ec-84de0792b1d6"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "First Name", 5, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("b3caf703-e72b-4eb8-9d5c-b389e16c8c43"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Middle Names", 6, ImportElementType.Employee, false, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("76473c0a-df08-40f9-8de0-632d0111a912"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Hire Date", 11, ImportElementType.Employee, true, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("b7cbf994-4a23-4405-93df-d66d4c3ed2a3"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Termination Date", 12, ImportElementType.Employee, false, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("a0891ce2-d0c2-4b5b-9a78-7aaa3aaa87c1"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employee's Address 1st line", 14, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("0330c639-1524-402b-b7bc-04d26bfc05a1"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employee's Address Town", 16, ImportElementType.Employee, false, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("9c9f07dd-a9d0-4ccf-9231-dd3c10d491b8"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employees Address Postcode", 18, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("8816caec-b520-4223-b738-47d2f22f3e1a"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employees Address Country", 19, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("0f951c3e-29d1-49f0-ac13-4cfcabf21fda"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Office e-Mail", 24, ImportElementType.Employee, false, DataType.stringVal));

            //ESR Assignment Info
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("7c0d8eab-d9af-415f-9bb7-d1be01f69e2f"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment ID", 1, ImportElementType.Assignment, true, DataType.intVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("c23858b8-7730-440e-b481-c43fe8a1dbef"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Number", 2, ImportElementType.Assignment, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("c53828af-99ff-463f-93f9-2721df44e5f2"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Earliest Assignment Start Date", 3, ImportElementType.Assignment, false, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("36eb4bb6-f4d5-414c-9106-ee62db01d902"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Final Assignment End Date", 4, ImportElementType.Assignment, false, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("9721ec22-404b-468b-83a4-d17d7559d3ef"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Status", 5, ImportElementType.Assignment, false, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("3e5750f0-1061-46c4-ad94-089d40a62dec"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Address 1st Line", 9, ImportElementType.Assignment, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("737a99ad-b0c5-4325-a565-c4d3fba536dd"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Address Postcode", 13, ImportElementType.Assignment, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("96f11c6d-7615-4abd-94ec-0e4d34e187a0"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Supervisor Employee Number", 17, ImportElementType.Assignment, false, DataType.intVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("fec46ed7-57f9-4c51-9916-ec92834371c3"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Primary Assignment", 22, ImportElementType.Assignment, true, DataType.booleanVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("c50dae62-8dae-4289-a0ce-584c3129159e"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Assignment Location", 37, ImportElementType.Assignment, true, DataType.stringVal));

            //Linked to the employees table
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("5f4a4551-1c05-4c85-b6d9-06d036bc327e"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Job Name", 39, ImportElementType.Assignment, false, DataType.stringVal));

            #endregion

            cImportTemplate template = new cImportTemplate(0, "Unit Test Template", ApplicationType.ESROutboundImport, true, cGlobalVariables.NHSTrustID, lstMappings, DateTime.Now, cGlobalVariables.EmployeeID, null, null);

            int ID = target.saveImportTemplate(template);

            Assert.IsTrue(ID > 0);

            cGlobalVariables.TemplateID = ID;

            //target = new cImportTemplates(cGlobalVariables.AccountID);
            //System.Threading.Thread.Sleep(1000);

            cImportTemplate actual = target.getImportTemplateByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(template.TemplateName, actual.TemplateName);
            Assert.AreEqual(template.appType, actual.appType);
            Assert.AreEqual(template.IsAutomated, actual.IsAutomated);
            Assert.AreEqual(template.NHSTrustID, actual.NHSTrustID);

            #region Template mappings

            int index = 0;

            foreach (cImportTemplateMapping impTemp in template.Mappings)
            {
                Assert.AreEqual(ID, actual.Mappings[index].TemplateID);
                Assert.AreEqual(impTemp.FieldID, actual.Mappings[index].FieldID);
                Assert.AreEqual(impTemp.DestinationField, actual.Mappings[index].DestinationField);
                Assert.AreEqual(impTemp.ColRef, actual.Mappings[index].ColRef);
                Assert.AreEqual(impTemp.ElementType, actual.Mappings[index].ElementType);
                Assert.AreEqual(impTemp.Mandatory, actual.Mappings[index].Mandatory);
                Assert.AreEqual(impTemp.dataType, actual.Mappings[index].dataType);

                index++;
            }

            #endregion
        }

        /// <summary>
        /// Edit an import template and its associated mappings as a delegate
        /// </summary>
        [TestMethod()]
        public void EditImportTemplateAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int AccountID = cGlobalVariables.AccountID;
            List<cImportTemplateMapping> lstMappings = new List<cImportTemplateMapping>();

            cImportTemplate template = cImportTemplateObject.CreateImportTemplate();
            cImportTemplates target = new cImportTemplates(AccountID);



            int ID = target.saveImportTemplate(new cImportTemplate(template.TemplateID, "Unit Test Template Updated", template.appType, template.IsAutomated, template.NHSTrustID, lstMappings, template.createdOn, template.createdBy, template.modifiedOn, template.modifiedBy));

            Assert.IsTrue(ID > 0);

            //System.Threading.Thread.Sleep(1000);
            //target = new cImportTemplates(cGlobalVariables.AccountID);

            cImportTemplate actual = target.getImportTemplateByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual("Unit Test Template Updated", actual.TemplateName);
            Assert.AreEqual(template.appType, actual.appType);
            Assert.AreEqual(template.IsAutomated, actual.IsAutomated);
            Assert.AreEqual(template.NHSTrustID, actual.NHSTrustID);

            #region Template mappings

            int index = 0;

            if (actual.Mappings.Count > 0)
            {
                foreach (cImportTemplateMapping impTemp in template.Mappings)
                {
                    Assert.AreEqual(ID, actual.Mappings[index].TemplateID);
                    Assert.AreEqual(impTemp.FieldID, actual.Mappings[index].FieldID);
                    Assert.AreEqual(impTemp.DestinationField, actual.Mappings[index].DestinationField);
                    Assert.AreEqual(impTemp.ColRef, actual.Mappings[index].ColRef);
                    Assert.AreEqual(impTemp.ElementType, actual.Mappings[index].ElementType);
                    Assert.AreEqual(impTemp.Mandatory, actual.Mappings[index].Mandatory);
                    Assert.AreEqual(impTemp.dataType, actual.Mappings[index].dataType);

                    index++;
                }
            }

            #endregion
        }

        /// <summary>
        ///A test for getImportTemplateMappings
        ///</summary>
        [TestMethod()]
        public void GetImportTemplateMappingsTest()
        {
            cImportTemplate template = cImportTemplateObject.CreateImportTemplate();
            cImportTemplates target = new cImportTemplates(cGlobalVariables.AccountID);

            List<cImportTemplateMapping> actual = target.getImportTemplateMappings(cGlobalVariables.TemplateID);

            int index = 0;

            foreach (cImportTemplateMapping impTemp in template.Mappings)
            {
                Assert.AreEqual(cGlobalVariables.TemplateID, actual[index].TemplateID);
                Assert.AreEqual(impTemp.FieldID, actual[index].FieldID);
                Assert.AreEqual(impTemp.DestinationField, actual[index].DestinationField);
                Assert.AreEqual(impTemp.ColRef, actual[index].ColRef);
                Assert.AreEqual(impTemp.ElementType, actual[index].ElementType);
                Assert.AreEqual(impTemp.Mandatory, actual[index].Mandatory);
                Assert.AreEqual(impTemp.dataType, actual[index].dataType);

                index++;
            }
        }

        /// <summary>
        ///A test to get an import template by a valid ID
        ///</summary>
        [TestMethod()]
        public void GetImportTemplateByAValidIDTest()
        {
            cImportTemplate expected = cImportTemplateObject.CreateImportTemplate();
            cImportTemplates target = new cImportTemplates(cGlobalVariables.AccountID);

            cImportTemplate actual = target.getImportTemplateByID(cGlobalVariables.TemplateID);

            Assert.IsNotNull(actual);
            cCompareAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test to get an import template by an invalid ID
        ///</summary>
        [TestMethod()]
        public void GetImportTemplateByAnInvalidIDTest()
        {
            cImportTemplate expected = cImportTemplateObject.CreateImportTemplate();
            cImportTemplates target = new cImportTemplates(cGlobalVariables.AccountID);

            cImportTemplate actual = target.getImportTemplateByID(0);

            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test for GetApplicationXMLMappings
        ///</summary>
        [TestMethod()]
        public void GetApplicationXMLMappingsTest()
        {
            cImportTemplates target = new cImportTemplates(cGlobalVariables.AccountID); // TODO: Initialize to an appropriate value

            Dictionary<string, List<XMLMapFields>> actual = target.GetApplicationXMLMappings(ApplicationType.ESROutboundImport);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        ///A test to delete an import template with a valid ID
        ///</summary>
        [TestMethod()]
        public void DeleteImportTemplateWithAValidIDTest()
        {
            cImportTemplateObject.CreateImportTemplate();
            cImportTemplates target = new cImportTemplates(cGlobalVariables.AccountID);
            
            target.deleteImportTemplate(cGlobalVariables.TemplateID);
            //System.Threading.Thread.Sleep(1000);
            //target = new cImportTemplates(cGlobalVariables.AccountID);

            Assert.IsNull(target.getImportTemplateByID(cGlobalVariables.TemplateID));
        }

        /// <summary>
        ///A test to delete an import template with an invalid ID
        ///</summary>
        [TestMethod()]
        public void DeleteImportTemplateWithAnInvalidIDTest()
        {
            cImportTemplateObject.CreateImportTemplate();
            cImportTemplates target = new cImportTemplates(cGlobalVariables.AccountID);

            target.deleteImportTemplate(0);
            //System.Threading.Thread.Sleep(1000);
            //target = new cImportTemplates(cGlobalVariables.AccountID);

            Assert.IsNotNull(target.getImportTemplateByID(cGlobalVariables.TemplateID));
        }


        /// <summary>
        ///A test to delete an import template with a valid ID as a delegate
        ///</summary>
        [TestMethod()]
        public void DeleteImportTemplateWithAValidIDAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            cImportTemplateObject.CreateImportTemplate();
            cImportTemplates target = new cImportTemplates(cGlobalVariables.AccountID);

            target.deleteImportTemplate(cGlobalVariables.TemplateID);
            //System.Threading.Thread.Sleep(1000);
            //target = new cImportTemplates(cGlobalVariables.AccountID);

            Assert.IsNull(target.getImportTemplateByID(cGlobalVariables.TemplateID));
        }

        
        /// <summary>
        ///A test for checkExistenceOfESRAutomatedTemplate
        ///</summary>
        [TestMethod()]
        public void CheckExistenceOfESRAutomatedTemplateTest()
        {
            cImportTemplateObject.CreateImportTemplate();
            cImportTemplates target = new cImportTemplates(cGlobalVariables.AccountID);
            
            int actual = target.checkExistenceOfESRAutomatedTemplate(cGlobalVariables.NHSTrustID);
            Assert.IsTrue(actual > 0);
        }

        /// <summary>
        ///A test for checkExistenceOfESRAutomatedTemplate where an import template does not exist
        ///</summary>
        [TestMethod()]
        public void CheckExistenceOfESRAutomatedTemplateWhereTemplateDoesNotExistTest()
        {
            cESRTrustObject.CreateESRTrustGlobalVariable();
            cImportTemplates target = new cImportTemplates(cGlobalVariables.AccountID);

            int actual = target.checkExistenceOfESRAutomatedTemplate(cGlobalVariables.NHSTrustID);
            Assert.IsTrue(actual == 0);
        }

        /// <summary>
        ///A test for CacheList
        ///</summary>
        [TestMethod()]
        public void CacheListTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            //Make sure there is nothing in the cache before running this test
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
            Cache.Remove("ImportTemplates" + AccountID);

            cImportTemplateObject.CreateImportTemplate();

            SortedList<int, cImportTemplate> expected = (SortedList<int, cImportTemplate>)Cache["ImportTemplates" + AccountID];

            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);
            Cache.Remove("ImportTemplates" + AccountID);
        }

        /// <summary>
        ///A test for cImportTemplates Constructor
        ///</summary>
        [TestMethod()]
        public void cImportTemplatesConstructorTest()
        {
            cImportTemplates target = new cImportTemplates(cGlobalVariables.AccountID);
            Assert.IsNotNull(target);
        }
    }
}
