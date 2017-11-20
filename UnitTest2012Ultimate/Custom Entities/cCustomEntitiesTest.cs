using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpendManagementLibrary.Enumerators;
using Spend_Management;
using SpendManagementLibrary;
using SortDirection = SpendManagementLibrary.SortDirection;

namespace UnitTest2012Ultimate
{
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    ///This is a test class for cCustomEntitiesTest and is intended
    ///to contain all cCustomEntities Unit Tests
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

        #region GetAttributeByFieldId

        /// <summary>
        ///A test for getAttributeByFieldId using the first valid fieldID on the entity
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getAttributeByFieldId_UsingValidFieldID()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));

            try
            {
                cAttribute expected = reqCustomEntity.attributes.Values[0];

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                cAttribute actual = customEntities.getAttributeByFieldId(expected.fieldid);

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
        ///A test for getAttributeByFieldId using an incorrect fieldID on the entity
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getAttributeByFieldId_UsingIncorrectFieldID()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));

            try
            {
                cAttribute reqAttribute = reqCustomEntity.attributes.Values[0];
                Guid incorrectFieldID = reqAttribute.fieldid;

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                customEntities.deleteEntity(reqCustomEntity.entityid, EmployeeID, 0);

                customEntities = new cCustomEntities(Moqs.CurrentUser());

                cAttribute actual = customEntities.getAttributeByFieldId(incorrectFieldID);

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

        #region GetEntityIdByAttributeId

        /// <summary>
        ///A test for getEntityIdByAttributeId using the first valid attribute ID on the entity
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getEntityIdByAttributeId_UsingFirstValidAttributeID()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity expected = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int attributeID = expected.attributes.Keys[0];

                int actualID = customEntities.getEntityIdByAttributeId(attributeID);

                Assert.AreEqual(expected.entityid, actualID);
            }
            finally
            {
                if (expected != null)
                {
                    cCustomEntityObject.TearDown(expected.entityid);
                }
            }
        }


        /// <summary>
        ///A test for getEntityIdByAttributeId using an incorrect attribute ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getEntityIdByAttributeId_UsingIncorrectAttributeID()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity expected = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int attributeID = expected.attributes.Keys[0];

                customEntities.deleteEntity(expected.entityid, EmployeeID, 0);

                customEntities = new cCustomEntities(Moqs.CurrentUser());

                int actualID = customEntities.getEntityIdByAttributeId(attributeID);

                Assert.AreEqual(0, actualID);
            }
            finally
            {
                if (expected != null)
                {
                    cCustomEntityObject.TearDown(expected.entityid);
                }
            }
        }

        #endregion

        #region GetEntityByTableId

        /// <summary>
        ///A test for getEntityByTableId using a valid ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getEntityByTableId_UsingValidID()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity expected = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                cCustomEntity actual = customEntities.getEntityByTableId(expected.table.TableID);

                cCompareAssert.AreEqual(expected, actual);
            }
            finally
            {
                if (expected != null)
                {
                    cCustomEntityObject.TearDown(expected.entityid);
                }
            }
        }

        /// <summary>
        ///A test for getEntityByTableId using an incorrect ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getEntityByTableId_UsingIncorrectID()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                customEntities.deleteEntity(reqCustomEntity.entityid, EmployeeID, 0);

                customEntities = new cCustomEntities(Moqs.CurrentUser());

                cCustomEntity actual = customEntities.getEntityByTableId(reqCustomEntity.table.TableID);

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

        #region GetEntityById

        /// <summary>
        ///A test for getEntityById
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getEntityById_UsingValidID()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity expected = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                cCustomEntity actual = customEntities.getEntityById(expected.entityid);

                cCompareAssert.AreEqual(expected, actual);
            }
            finally
            {
                if (expected != null)
                {
                    cCustomEntityObject.TearDown(expected.entityid);
                }
            }
        }

        /// <summary>
        ///A test for getEntityById where an invalid ID is used
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getEntityById_UsingIncorrectID()
        {
            int EmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: EmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                customEntities.deleteEntity(reqCustomEntity.entityid, EmployeeID, 0);

                customEntities = new cCustomEntities(Moqs.CurrentUser());

                cCustomEntity actual = customEntities.getEntityById(reqCustomEntity.entityid);

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

        #region GenerateFields
        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        [ExpectedException(typeof(NullReferenceException))]
        public void cCustomEntities_generateFields_nothingSetup()
        {
            cCustomEntities target = new cCustomEntities();
            PlaceHolder pnlSection = new PlaceHolder();
            cCustomEntityFormSection section = null;
            cCustomEntity entity = null;
            int viewid = 0;
            cCustomEntityForm form = null;
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            // should error as there isn't a section
            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_emptyEntityAndSection()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = null;
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

            // There should just be a PlaceHolder object with no content
            Assert.AreEqual(0, pnlSection.Controls.Count);
        }

        // WE NO LONGER DOTONATE LABELS
        ///// <summary>
        /////A test for generateFields
        /////</summary>
        //[TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        //public void cCustomEntities_generateFields_twoColumnOneFieldLongName()
        //{
        //    cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
        //    PlaceHolder pnlSection = new PlaceHolder();

        //    cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
        //    cCustomEntity entity = cCustomEntityObject.Template();
        //    int viewid = 0;
        //    cCustomEntityForm form = cCustomEntityFormObject.Template();
        //    int activeTabId = 0;
        //    List<string> otmTableID = null;
        //    List<string> summaryTableID = null;
        //    List<string> scriptCmds = null;

        //    cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(displayName: "This is a name that is longer than the fifty characters needed to ellide", fieldType: FieldType.Text));
        //    section.fields.Add(field);

        //    target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

        //    // div twocolumn start :: label :: span start :: input :: span end / span start :: span end / span start / tooltip / span end  / span start :: span end :: div end
        //    Assert.AreEqual(8, pnlSection.Controls.Count);

        //    Assert.AreEqual(typeof(Literal), pnlSection.Controls[0].GetType());
        //    Assert.AreEqual(typeof(Label), pnlSection.Controls[1].GetType());
        //    Assert.AreEqual(typeof(Literal), pnlSection.Controls[2].GetType());
        //    Assert.AreEqual(typeof(TextBox), pnlSection.Controls[3].GetType());
        //    Assert.AreEqual(typeof(Literal), pnlSection.Controls[4].GetType());
        //    Assert.AreEqual(typeof(Literal), pnlSection.Controls[5].GetType());
        //    Assert.AreEqual(typeof(Literal), pnlSection.Controls[6].GetType());
        //    Assert.AreEqual(typeof(Literal), pnlSection.Controls[7].GetType());

        //    Assert.AreEqual("<div class=\"twocolumn\">", ((Literal)pnlSection.Controls[0]).Text);
        //    Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[1]).AssociatedControlID);
        //    Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[2]).Text);
        //    Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[3]).ID);
        //    Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[4]).Text);
        //    Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field.attribute.tooltip + "', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[5]).Text);
        //    Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[6]).Text);
        //    Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[7]).Text);

        //    Assert.AreEqual(field.attribute.displayname.Substring(0, field.attribute.MaxLabelTextLength - 1) + "...", ((Label)pnlSection.Controls[1]).Text);
        //    Assert.AreEqual("", ((TextBox)pnlSection.Controls[3]).Text);
        //    Assert.AreEqual(500, ((TextBox)pnlSection.Controls[3]).MaxLength);
        //}

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnOneFieldShortName()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(displayName: "Short Name", fieldType: FieldType.Text));
            section.fields.Add(field);

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

            Assert.AreEqual(8, pnlSection.Controls.Count);

            Assert.AreEqual(typeof(Literal), pnlSection.Controls[0].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[1].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[2].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[3].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[4].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[5].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[6].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[7].GetType());

            Assert.AreEqual("<div class=\"twocolumn\">", ((Literal)pnlSection.Controls[0]).Text);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[1]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[2]).Text);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[3]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[4]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[5]).Text);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[6]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[7]).Text);

            Assert.AreEqual(field.attribute.displayname, ((Label)pnlSection.Controls[1]).Text);
            Assert.AreEqual("", ((TextBox)pnlSection.Controls[3]).Text);
            Assert.AreEqual(500, ((TextBox)pnlSection.Controls[3]).MaxLength);
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnOneFieldCustomLabel()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(displayName: "Short Name", fieldType: FieldType.Text), labelText: "Custom Label");
            section.fields.Add(field);

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

            Assert.AreEqual(8, pnlSection.Controls.Count);

            Assert.AreEqual(typeof(Literal), pnlSection.Controls[0].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[1].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[2].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[3].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[4].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[5].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[6].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[7].GetType());

            Assert.AreEqual("<div class=\"twocolumn\">", ((Literal)pnlSection.Controls[0]).Text);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[1]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[2]).Text);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[3]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[4]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[5]).Text);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[6]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[7]).Text);

            Assert.AreEqual("Custom Label", ((Label)pnlSection.Controls[1]).Text);
            Assert.AreEqual("", ((TextBox)pnlSection.Controls[3]).Text);
            Assert.AreEqual(500, ((TextBox)pnlSection.Controls[3]).MaxLength);
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnOneFieldNegativeMaxLength()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(displayName: "Short Name", fieldType: FieldType.Text, maxLength: -1));
            section.fields.Add(field);

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

            Assert.AreEqual(8, pnlSection.Controls.Count);

            Assert.AreEqual(typeof(Literal), pnlSection.Controls[0].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[1].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[2].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[3].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[4].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[5].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[6].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[7].GetType());

            Assert.AreEqual("<div class=\"twocolumn\">", ((Literal)pnlSection.Controls[0]).Text);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[1]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[2]).Text);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[3]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[4]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[5]).Text);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[6]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[7]).Text);

            Assert.AreEqual(field.attribute.displayname, ((Label)pnlSection.Controls[1]).Text);
            Assert.AreEqual("", ((TextBox)pnlSection.Controls[3]).Text);
            Assert.AreEqual(500, ((TextBox)pnlSection.Controls[3]).MaxLength);
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnOneFieldShortNameMandatory()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(displayName: "Short Name", fieldType: FieldType.Text, mandatory: true));
            section.fields.Add(field);

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

            Assert.AreEqual(9, pnlSection.Controls.Count);

            int i = 0;
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(RequiredFieldValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());

            i = 0;
            Assert.AreEqual("<div class=\"twocolumn\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[i++]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[i++]).Text);

            Assert.AreEqual("req" + field.attribute.attributeid.ToString(), ((RequiredFieldValidator)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);

            Assert.AreEqual(field.attribute.displayname + "*", ((Label)pnlSection.Controls[1]).Text);
            Assert.AreEqual("mandatory", ((Label)pnlSection.Controls[1]).CssClass);
            Assert.AreEqual("", ((TextBox)pnlSection.Controls[3]).Text);
            Assert.AreEqual(500, ((TextBox)pnlSection.Controls[3]).MaxLength);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((RequiredFieldValidator)pnlSection.Controls[6]).ControlToValidate);
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnOneFieldDateOnlyForCalendarIcon()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cDateTimeAttributeObject.Template(format: AttributeFormat.DateOnly));
            section.fields.Add(field);

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

            Assert.AreEqual(12, pnlSection.Controls.Count);

            int i = 0;
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Image), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());

            i = 0;
            Assert.AreEqual("<div class=\"twocolumn\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[i++]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("img" + field.attribute.attributeid.ToString(), ((Image)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("comp" + field.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("compgte" + field.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("complte" + field.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);

            Assert.AreEqual("", ((TextBox)pnlSection.Controls[3]).Text);
            Assert.AreEqual(0, ((TextBox)pnlSection.Controls[3]).MaxLength);
            Assert.AreEqual("~/shared/images/icons/cal.gif", ((Image)pnlSection.Controls[5]).ImageUrl);
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnOneFieldDateTimeForCalendarIcon()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cDateTimeAttributeObject.Template(format: AttributeFormat.DateTime));
            section.fields.Add(field);

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

            Assert.AreEqual(16, pnlSection.Controls.Count);

            int i = 0;
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Image), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(RegularExpressionValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(RequiredFieldValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(RequiredFieldValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());

            i = 0;
            Assert.AreEqual("<div class=\"twocolumn\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[i++]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("txt" + field.attribute.attributeid.ToString() + "_time", ((TextBox)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("img" + field.attribute.attributeid.ToString(), ((Image)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("comp" + field.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("compgte" + field.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("complte" + field.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("comp" + field.attribute.attributeid.ToString() + "_time", ((RegularExpressionValidator)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("reqDate" + field.attribute.attributeid.ToString(), ((RequiredFieldValidator)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("reqTime" + field.attribute.attributeid.ToString() + "_time", ((RequiredFieldValidator)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);

            Assert.AreEqual("", ((TextBox)pnlSection.Controls[3]).Text);
            Assert.AreEqual(0, ((TextBox)pnlSection.Controls[3]).MaxLength);
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnSummaryAttribute()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = new List<string>();
            List<string> scriptCmds = null;

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cSummaryAttributeObject.Template());
            section.fields.Add(field);

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

            Assert.AreEqual(3, pnlSection.Controls.Count);

            int i = 0;
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());

            i = 0;
            Assert.AreEqual("<div>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("lit" + field.attribute.attributeid.ToString(), ((Literal)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);

            Assert.AreEqual("<div id=\"summaryGrid" + field.attribute.attributeid.ToString() + "\"><img alt=\"Table loading...\" src=\"/shared/images/ajax-loader.gif\" /></div>", ((Literal)pnlSection.Controls[1]).Text);

            int nRecordid = 0; // this is set as a private global on the cCustomEntities class by generateForm before we ever get to generateFields
            Assert.AreEqual(1, summaryTableID.Count);
            Assert.AreEqual(entity.entityid.ToString() + "," + field.attribute.attributeid.ToString() + "," + viewid.ToString() + "," + activeTabId.ToString() + "," + form.formid.ToString() + "," + nRecordid.ToString() + ",'summaryGrid" + field.attribute.attributeid.ToString() + "'", summaryTableID[0]);
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnOneToManyRelationshipAttributeNoAdd()
        {
            cCustomEntity otmEntity = null;
            cCustomEntityView otmView = null;

            try
            {
                otmEntity = cCustomEntityObject.New();
                otmView = cCustomEntityViewObject.New(otmEntity.entityid, cCustomEntityViewObject.Template(allowAdd: false));

                Mock<ICurrentUser> currentUser = Moqs.CurrentUserMock();
                currentUser.Setup(x => x.CheckAccessRole(It.IsAny<AccessRoleType>(), It.IsAny<CustomEntityElementType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), AccessRequestType.Website)).Returns(false);

                cCustomEntities target = new cCustomEntities(currentUser.Object);
                PlaceHolder pnlSection = new PlaceHolder();

                cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
                cCustomEntity entity = cCustomEntityObject.Template();
                int viewid = 0;
                cCustomEntityForm form = cCustomEntityFormObject.Template();
                int activeTabId = 0;
                List<string> otmTableID = new List<string>();
                List<string> summaryTableID = null;
                List<string> scriptCmds = null;

                cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cOneToManyRelationshipObject.Template(entityID: otmEntity.entityid, viewID: otmView.viewid));
                section.fields.Add(field);

                target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

                Assert.AreEqual(3, pnlSection.Controls.Count);

                int i = 0;
                Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
                Assert.AreEqual(typeof(UpdatePanel), pnlSection.Controls[i++].GetType());
                Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());

                i = 0;
                Assert.AreEqual("<div>", ((Literal)pnlSection.Controls[i++]).Text);
                Assert.AreEqual("upnlonetomany" + field.attribute.attributeid.ToString(), ((UpdatePanel)pnlSection.Controls[i++]).ID);
                Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);

                Assert.AreEqual(1, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls.Count);
                Assert.AreEqual(typeof(Table), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].GetType());
                Assert.AreEqual(1, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls.Count);
                Assert.AreEqual(typeof(TableRow), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].GetType());
                Assert.AreEqual(1, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls.Count);
                Assert.AreEqual(typeof(TableCell), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].GetType());
                Assert.AreEqual(1, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls.Count);
                Assert.AreEqual(typeof(Literal), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls[0].GetType());

                Assert.AreEqual("lit" + field.attribute.attributeid, ((Literal)((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls[0]).ID);
                Assert.AreEqual("<div id=\"otmGrid" + field.attribute.attributeid.ToString() + "\"><img alt=\"Table loading...\" src=\"/shared/images/ajax-loader.gif\" /></div>", ((Literal)((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls[0]).Text);

                // these are set from breadcrumbs on the page (none in unit tests)
                string relentityids = string.Empty;
                string relformids = string.Empty;
                string relrecordids = string.Empty;
                string reltabids = string.Empty;
                Assert.AreEqual(1, otmTableID.Count);
                Assert.AreEqual(entity.entityid.ToString() + "," + field.attribute.attributeid.ToString() + "," + viewid.ToString() + "," + activeTabId.ToString() + "," + form.formid.ToString() + ",{0},'" + relentityids + "','" + relformids + "','" + relrecordids + "','" + reltabids + "','otmGrid" + field.attribute.attributeid.ToString() + "',''", otmTableID[0]);
            }
            finally
            {
                if (otmEntity != null)
                {
                    cCustomEntityObject.TearDown(otmEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnOneToManyRelationshipAttributeWithAddButNoAccessRole()
        {
            cCustomEntity otmEntity = cCustomEntityObject.New();

            try
            {
                cCustomEntityView otmView = cCustomEntityViewObject.New(otmEntity.entityid, cCustomEntityViewObject.Template(allowAdd: true, addForm: cCustomEntityFormObject.Template()));

                Mock<ICurrentUser> currentUser = Moqs.CurrentUserMock();
                currentUser.Setup(x => x.CheckAccessRole(It.IsAny<AccessRoleType>(), It.IsAny<CustomEntityElementType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), AccessRequestType.Website)).Returns(false);

                cCustomEntities target = new cCustomEntities(currentUser.Object);
                PlaceHolder pnlSection = new PlaceHolder();

                cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
                cCustomEntity entity = cCustomEntityObject.Template();
                int viewid = 0;
                cCustomEntityForm form = cCustomEntityFormObject.Template();
                int activeTabId = 0;
                List<string> otmTableID = new List<string>();
                List<string> summaryTableID = null;
                List<string> scriptCmds = null;

                cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cOneToManyRelationshipObject.Template(entityID: otmEntity.entityid, viewID: otmView.viewid));
                section.fields.Add(field);

                target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

                Assert.AreEqual(3, pnlSection.Controls.Count);

                int i = 0;
                Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
                Assert.AreEqual(typeof(UpdatePanel), pnlSection.Controls[i++].GetType());
                Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());

                i = 0;
                Assert.AreEqual("<div>", ((Literal)pnlSection.Controls[i++]).Text);
                Assert.AreEqual("upnlonetomany" + field.attribute.attributeid.ToString(), ((UpdatePanel)pnlSection.Controls[i++]).ID);
                Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);

                Assert.AreEqual(1, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls.Count);
                Assert.AreEqual(typeof(Table), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].GetType());
                Assert.AreEqual(1, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls.Count);
                Assert.AreEqual(typeof(TableRow), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].GetType());
                Assert.AreEqual(1, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls.Count);
                Assert.AreEqual(typeof(TableCell), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].GetType());
                Assert.AreEqual(1, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls.Count);
                Assert.AreEqual(typeof(Literal), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls[0].GetType());

                Assert.AreEqual("lit" + field.attribute.attributeid, ((Literal)((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls[0]).ID);
                Assert.AreEqual("<div id=\"otmGrid" + field.attribute.attributeid.ToString() + "\"><img alt=\"Table loading...\" src=\"/shared/images/ajax-loader.gif\" /></div>", ((Literal)((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls[0]).Text);

                // these are set from breadcrumbs on the page (none in unit tests)
                string relentityids = string.Empty;
                string relformids = string.Empty;
                string relrecordids = string.Empty;
                string reltabids = string.Empty;
                Assert.AreEqual(1, otmTableID.Count);
                Assert.AreEqual(entity.entityid.ToString() + "," + field.attribute.attributeid.ToString() + "," + viewid.ToString() + "," + activeTabId.ToString() + "," + form.formid.ToString() + ",{0},'" + relentityids + "','" + relformids + "','" + relrecordids + "','" + reltabids + "','otmGrid" + field.attribute.attributeid.ToString() + "',''", otmTableID[0]);
            }
            finally
            {
                if (otmEntity != null)
                {
                    cCustomEntityObject.TearDown(otmEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnOneToManyRelationshipAttributeWithAddAndAccessRole()
        {
            cCustomEntity otmEntity = cCustomEntityObject.New();
            cCustomEntityForm otmForm = null;
            cCustomEntityView otmView = null;
            try
            {
                otmForm = cCustomEntityFormObject.New(otmEntity.entityid, entity: cCustomEntityFormObject.Basic(otmEntity));
                otmView = cCustomEntityViewObject.New(otmEntity.entityid, cCustomEntityViewObject.Template(allowAdd: true, addForm: otmForm));

                Mock<ICurrentUser> currentUser = Moqs.CurrentUserMock();
                currentUser.Setup(x => x.CheckAccessRole(It.IsAny<AccessRoleType>(), It.IsAny<CustomEntityElementType>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), AccessRequestType.Website)).Returns(true);

                cCustomEntities target = new cCustomEntities(currentUser.Object);
                PlaceHolder pnlSection = new PlaceHolder();

                cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
                cCustomEntity entity = cCustomEntityObject.Template(enablePopupWindow: false);
                int viewid = 0;
                cCustomEntityForm form = cCustomEntityFormObject.Template();
                int activeTabId = 0;
                List<string> otmTableID = new List<string>();
                List<string> summaryTableID = null;
                List<string> scriptCmds = null;

                cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cOneToManyRelationshipObject.Template(entityID: otmEntity.entityid, viewID: otmView.viewid));
                section.fields.Add(field);

                target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

                Assert.AreEqual(3, pnlSection.Controls.Count);

                int i = 0;
                Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
                Assert.AreEqual(typeof(UpdatePanel), pnlSection.Controls[i++].GetType());
                Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());

                i = 0;
                Assert.AreEqual("<div>", ((Literal)pnlSection.Controls[i++]).Text);
                Assert.AreEqual("upnlonetomany" + field.attribute.attributeid.ToString(), ((UpdatePanel)pnlSection.Controls[i++]).ID);
                Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);

                Assert.AreEqual(1, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls.Count);
                Assert.AreEqual(typeof(Table), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].GetType());

                Assert.AreEqual(2, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls.Count);
                Assert.AreEqual(typeof(TableRow), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].GetType());
                Assert.AreEqual(typeof(TableRow), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[1].GetType());

                Assert.AreEqual(1, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls.Count);
                Assert.AreEqual(typeof(TableCell), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].GetType());
                Assert.AreEqual(1, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls.Count);
                Assert.AreEqual(typeof(LinkButton), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls[0].GetType());

                Assert.AreEqual(1, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[1].Controls.Count);
                Assert.AreEqual(typeof(TableCell), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[1].Controls[0].GetType());
                Assert.AreEqual(1, ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[1].Controls[0].Controls.Count);
                Assert.AreEqual(typeof(Literal), ((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[1].Controls[0].Controls[0].GetType());

                // these are set from breadcrumbs on the page (none in unit tests)
                string relentityids = string.Empty;
                string relformids = string.Empty;
                string relrecordids = string.Empty;
                string reltabids = string.Empty;
                string relviewid = string.Empty;

                Assert.AreEqual("lnkonetomany" + field.attribute.attributeid, ((LinkButton)((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls[0]).ID);
                Assert.AreEqual("New " + otmEntity.entityname, ((LinkButton)((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls[0]).Text);
                Assert.AreEqual(ClientIDMode.Static, ((LinkButton)((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls[0]).ClientIDMode);
                Assert.AreEqual("vgCE_" + entity.entityid.ToString(), ((LinkButton)((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls[0]).ValidationGroup);
                Assert.AreEqual(relentityids + "," + relformids + "," + relrecordids + "," + reltabids + "," + relviewid + "," + viewid + "," + otmEntity.entityid + "," + otmView.DefaultAddForm.formid + ",0", ((LinkButton)((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls[0]).CommandArgument);
                Assert.AreEqual("javascript:if (!validateform('vgCE_" + entity.entityid.ToString() + "')) { return false; }", ((LinkButton)((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[0].Controls[0].Controls[0]).OnClientClick);

                Assert.AreEqual("lit" + field.attribute.attributeid, ((Literal)((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[1].Controls[0].Controls[0]).ID);
                Assert.AreEqual("<div id=\"otmGrid" + field.attribute.attributeid.ToString() + "\"><img alt=\"Table loading...\" src=\"/shared/images/ajax-loader.gif\" /></div>", ((Literal)((UpdatePanel)pnlSection.Controls[1]).Controls[0].Controls[0].Controls[1].Controls[0].Controls[0]).Text);

                Assert.AreEqual(1, otmTableID.Count);
                Assert.AreEqual(entity.entityid.ToString() + "," + field.attribute.attributeid.ToString() + "," + viewid.ToString() + "," + activeTabId.ToString() + "," + form.formid.ToString() + ",{0},'" + relentityids + "','" + relformids + "','" + relrecordids + "','" + reltabids + "','otmGrid" + field.attribute.attributeid.ToString() + "',''", otmTableID[0]);
            }
            finally
            {
                if (otmEntity != null)
                {
                    cCustomEntityObject.TearDown(otmEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnTwoFieldsTextThenInt()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            cCustomEntityFormField field1 = cCustomEntityFormFieldObject.Template(column: (byte)0, attribute: cTextAttributeObject.Template(attributeID: 1, displayName: "Short Name 1", fieldType: FieldType.Text));
            cCustomEntityFormField field2 = cCustomEntityFormFieldObject.Template(column: (byte)1, attribute: cNumberAttributeObject.Template(attributeID: 2, displayName: "Short Name 2", fieldType: FieldType.Integer));
            section.fields.Add(field1);
            section.fields.Add(field2);

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

            Assert.AreEqual(17, pnlSection.Controls.Count);

            Assert.AreEqual(typeof(Literal), pnlSection.Controls[0].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[1].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[2].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[3].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[4].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[5].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[6].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[7].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[8].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[9].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[10].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[11].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[12].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[13].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[14].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[15].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[16].GetType());

            Assert.AreEqual("<div class=\"twocolumn\">", ((Literal)pnlSection.Controls[0]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[1]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[2]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[3]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[4]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field1.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field1.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field1.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[5]).Text);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[6]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[7]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[8]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[9]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[10]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field2.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field2.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field2.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[11]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[12]).ControlToValidate);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[13]).ControlToValidate);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[14]).ControlToValidate);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[15]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[16]).Text);
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnTwoFieldsIntThenText()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            cCustomEntityFormField field1 = cCustomEntityFormFieldObject.Template(column: (byte)1, attribute: cTextAttributeObject.Template(attributeID: 1, displayName: "Short Name 1", fieldType: FieldType.Text, toolTip: string.Empty));
            cCustomEntityFormField field2 = cCustomEntityFormFieldObject.Template(column: (byte)0, attribute: cNumberAttributeObject.Template(attributeID: 2, displayName: "Short Name 2", fieldType: FieldType.Integer));
            section.fields.Add(field1);
            section.fields.Add(field2);

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);


            Assert.AreEqual(17, pnlSection.Controls.Count);

            Assert.AreEqual(typeof(Literal), pnlSection.Controls[0].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[1].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[2].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[3].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[4].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[5].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[6].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[7].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[8].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[9].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[10].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[11].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[12].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[13].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[14].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[15].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[16].GetType());

            Assert.AreEqual("<div class=\"twocolumn\">", ((Literal)pnlSection.Controls[0]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[1]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[2]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[3]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[4]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field2.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field2.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field2.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[5]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[6]).ControlToValidate);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[7]).ControlToValidate);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[8]).ControlToValidate);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[9]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[10]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[11]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[12]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[13]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field1.attribute.attributeid.ToString() + "\">", ((Literal)pnlSection.Controls[14]).Text);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[15]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[16]).Text);
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnOneFieldOneColumnOneField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            cCustomEntityFormField field1 = cCustomEntityFormFieldObject.Template(row: (byte)0, column: (byte)0, attribute: cNumberAttributeObject.Template(attributeID: 1, displayName: "Short Name 1", fieldType: FieldType.Integer));
            cCustomEntityFormField field2 = cCustomEntityFormFieldObject.Template(row: (byte)1, column: (byte)0, attribute: cTextAttributeObject.Template(attributeID: 2, displayName: "Short Name 2", fieldType: FieldType.Text, format: AttributeFormat.MultiLine));
            section.fields.Add(field1);
            section.fields.Add(field2);

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

            Assert.AreEqual(19, pnlSection.Controls.Count);

            int i = 0;
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());

            i = 0;
            Assert.AreEqual("<div class=\"twocolumn\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[i++]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field1.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field1.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field1.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ControlToValidate);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ControlToValidate);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ControlToValidate);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("<div class=\"onecolumn\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[i++]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field2.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field2.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field2.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_twoColumnTwoFieldsOneColumnOneField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            cCustomEntityFormField field1 = cCustomEntityFormFieldObject.Template(row: (byte)0, column: (byte)0, attribute: cNumberAttributeObject.Template(attributeID: 1, displayName: "Short Name 1", fieldType: FieldType.Integer));
            cCustomEntityFormField field2 = cCustomEntityFormFieldObject.Template(row: (byte)1, column: (byte)0, attribute: cTextAttributeObject.Template(attributeID: 2, displayName: "Short Name 2", fieldType: FieldType.Text, format: AttributeFormat.FormattedText));
            cCustomEntityFormField field3 = cCustomEntityFormFieldObject.Template(row: (byte)0, column: (byte)1, attribute: cNumberAttributeObject.Template(attributeID: 3, displayName: "Short Name 3", fieldType: FieldType.Integer));
            section.fields.Add(field1);
            section.fields.Add(field2);
            section.fields.Add(field3);

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);


            Assert.AreEqual(28, pnlSection.Controls.Count);

            int i = 0;
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(CompareValidator), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());

            i = 0;
            Assert.AreEqual("<div class=\"twocolumn\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[i++]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field1.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field1.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field1.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ControlToValidate);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ControlToValidate);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ControlToValidate);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field3.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[i++]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field3.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field3.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field3.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field3.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field3.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ControlToValidate);
            Assert.AreEqual("txt" + field3.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ControlToValidate);
            Assert.AreEqual("txt" + field3.attribute.attributeid.ToString(), ((CompareValidator)pnlSection.Controls[i++]).ControlToValidate);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("<div class=\"onecolumnlarge\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[i++]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field2.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field2.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field2.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_oneColumnOneFieldOneColumnOneField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            cCustomEntityFormField field1 = cCustomEntityFormFieldObject.Template(row: (byte)0, column: (byte)0, attribute: cTextAttributeObject.Template(attributeID: 1, displayName: "Short Name 1", fieldType: FieldType.Text, format: AttributeFormat.MultiLine));
            cCustomEntityFormField field2 = cCustomEntityFormFieldObject.Template(row: (byte)1, column: (byte)0, attribute: cTextAttributeObject.Template(attributeID: 2, displayName: "Short Name 2", fieldType: FieldType.Text, format: AttributeFormat.MultiLine));
            section.fields.Add(field1);
            section.fields.Add(field2);

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

            Assert.AreEqual(16, pnlSection.Controls.Count);

            int i = 0;
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());

            i = 0;
            Assert.AreEqual("<div class=\"onecolumn\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[i++]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field1.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field1.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field1.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("<div class=\"onecolumn\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[i++]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field2.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field2.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field2.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);
        }

        /// <summary>
        ///A test for generateFields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFields_oneColumnOneFieldOneColumnOneFieldBothWithCustomLabels()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder pnlSection = new PlaceHolder();

            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template();
            cCustomEntity entity = cCustomEntityObject.Template();
            int viewid = 0;
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int activeTabId = 0;
            List<string> otmTableID = null;
            List<string> summaryTableID = null;
            List<string> scriptCmds = null;

            cCustomEntityFormField field1 = cCustomEntityFormFieldObject.Template(row: (byte)0, column: (byte)0, attribute: cTextAttributeObject.Template(attributeID: 1, displayName: "Short Name 1", fieldType: FieldType.Text, format: AttributeFormat.MultiLine), labelText: "Custom Label Number One");
            cCustomEntityFormField field2 = cCustomEntityFormFieldObject.Template(row: (byte)1, column: (byte)0, attribute: cTextAttributeObject.Template(attributeID: 2, displayName: "Short Name 2", fieldType: FieldType.Text, format: AttributeFormat.MultiLine), labelText: "Custom Label Number Two");
            section.fields.Add(field1);
            section.fields.Add(field2);

            target.generateFields(ref pnlSection, section, entity, viewid, form, activeTabId, ref otmTableID, ref summaryTableID, ref scriptCmds);

            Assert.AreEqual(16, pnlSection.Controls.Count);

            int i = 0;
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Label), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(TextBox), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());
            Assert.AreEqual(typeof(Literal), pnlSection.Controls[i++].GetType());

            i = 0;
            Assert.AreEqual("<div class=\"onecolumn\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[i++]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field1.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field1.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field1.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field1.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("<div class=\"onecolumn\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((Label)pnlSection.Controls[i++]).AssociatedControlID);
            Assert.AreEqual("<span class=\"inputs\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("txt" + field2.attribute.attributeid.ToString(), ((TextBox)pnlSection.Controls[i++]).ID);
            Assert.AreEqual("</span><span class=\"inputicon\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span><span class=\"inputtooltipfield\"><img id=\"imgToolTip" + field2.attribute.attributeid + "\" class=\"tooltipicon\" onmouseover=\"SEL.Tooltip.Show('" + field2.attribute.tooltip + "','sm', this);\" src=\"/shared/images/icons/16/plain/tooltip.png\" style=\"border-width: 0px;\" /></span><span class=\"inputvalidatorfield\" id=\"spanvalidate" + field2.attribute.attributeid + "\">", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</span>", ((Literal)pnlSection.Controls[i++]).Text);
            Assert.AreEqual("</div>", ((Literal)pnlSection.Controls[i++]).Text);

            Assert.AreEqual("Custom Label Number One", ((Label)pnlSection.Controls[1]).Text);
            Assert.AreEqual("Custom Label Number Two", ((Label)pnlSection.Controls[9]).Text);
        }


        #endregion GenerateFields

        #region Fields Generators
        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void cCustomEntities_generateFieldsInput_BlankField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            // Summary Attributes are drawn by the main generateFields, not by this method so if passed in to this method should error
            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cSummaryAttributeObject.Template());
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_CurrencyField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cNumberAttributeObject.Template(fieldType: FieldType.Currency));
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(TextBox), phDummy.Controls[0].GetType());
            Assert.AreEqual("txt" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("fillspan", ((TextBox)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(0, ((TextBox)phDummy.Controls[0]).MaxLength);
            Assert.AreEqual(TextBoxMode.SingleLine, ((TextBox)phDummy.Controls[0]).TextMode);
            Assert.AreEqual(true, ((TextBox)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_TextBoxField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(fieldType: FieldType.Text));
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(TextBox), phDummy.Controls[0].GetType());
            Assert.AreEqual("txt" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("fillspan", ((TextBox)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(500, ((TextBox)phDummy.Controls[0]).MaxLength);
            Assert.AreEqual(TextBoxMode.SingleLine, ((TextBox)phDummy.Controls[0]).TextMode);
            Assert.AreEqual(true, ((TextBox)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_TextBoxFieldMaxLength250()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(fieldType: FieldType.Text, maxLength: 250));
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(TextBox), phDummy.Controls[0].GetType());
            Assert.AreEqual("txt" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("fillspan", ((TextBox)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(250, ((TextBox)phDummy.Controls[0]).MaxLength);
            Assert.AreEqual(TextBoxMode.SingleLine, ((TextBox)phDummy.Controls[0]).TextMode);
            Assert.AreEqual(true, ((TextBox)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_TextBoxFieldMultiLine()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(fieldType: FieldType.Text, format: AttributeFormat.MultiLine));
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(TextBox), phDummy.Controls[0].GetType());
            Assert.AreEqual("txt" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("fillspan", ((TextBox)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(4000, ((TextBox)phDummy.Controls[0]).MaxLength);
            Assert.AreEqual(TextBoxMode.MultiLine, ((TextBox)phDummy.Controls[0]).TextMode);
            Assert.AreEqual(true, ((TextBox)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_TextBoxFieldReadOnly()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(fieldType: FieldType.Text), readOnly: true);
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(TextBox), phDummy.Controls[0].GetType());
            Assert.AreEqual("txt" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("fillspan", ((TextBox)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(500, ((TextBox)phDummy.Controls[0]).MaxLength);
            Assert.AreEqual(TextBoxMode.SingleLine, ((TextBox)phDummy.Controls[0]).TextMode);
            Assert.AreEqual(false, ((TextBox)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_TextBoxFieldMultiLineMaxLength4000ReadOnly()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(fieldType: FieldType.Text, maxLength: 4000, format: AttributeFormat.MultiLine), readOnly: true);
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(TextBox), phDummy.Controls[0].GetType());
            Assert.AreEqual("txt" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("fillspan", ((TextBox)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(4000, ((TextBox)phDummy.Controls[0]).MaxLength);
            Assert.AreEqual(TextBoxMode.MultiLine, ((TextBox)phDummy.Controls[0]).TextMode);
            Assert.AreEqual(false, ((TextBox)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_TextAreaFieldFormatNotSet()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(fieldType: FieldType.LargeText));
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(0, phDummy.Controls.Count);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_TextAreaField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(fieldType: FieldType.LargeText, format: AttributeFormat.MultiLine));
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(TextBox), phDummy.Controls[0].GetType());
            Assert.AreEqual("txt" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual(TextBoxMode.MultiLine, ((TextBox)phDummy.Controls[0]).TextMode);
            Assert.AreEqual(true, ((TextBox)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_TextAreaFieldReadOnly()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(fieldType: FieldType.LargeText, format: AttributeFormat.MultiLine), readOnly: true);
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(TextBox), phDummy.Controls[0].GetType());
            Assert.AreEqual("txt" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual(TextBoxMode.MultiLine, ((TextBox)phDummy.Controls[0]).TextMode);
            Assert.AreEqual(false, ((TextBox)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_RichTextAreaField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(fieldType: FieldType.LargeText, format: AttributeFormat.FormattedText));
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(4, phDummy.Controls.Count);

            Assert.AreEqual(typeof(Literal), phDummy.Controls[0].GetType());
            //Assert.AreEqual("<div><a style=\"cursor: hand;\" onclick=\"editRT('tabs" + form.formid.ToString() + "','tab" + tabID.ToString() + "','" + ((TextBox)phDummy.Controls[2]).UniqueID + "','" + ((HiddenField)phDummy.Controls[3]).UniqueID + "','" + ((Panel)phDummy.Controls[1]).UniqueID + "');\"><img src=\"/shared/images/icons/edit.png\" alt=\"Edit\" />&nbsp;&nbsp;Edit Text</a></div>", ((Literal)phDummy.Controls[0]).Text);

            Assert.AreEqual("<div style=\"vertical-align: middle;\"><a style=\"cursor: hand; vertical-align:center;\" onclick=\"EditRichTextEditor('tabs" + form.formid.ToString() + "','tab" + tabID.ToString() + "','" + ((TextBox)phDummy.Controls[2]).UniqueID + "','" + ((HiddenField)phDummy.Controls[3]).UniqueID + "','" + ((Panel)phDummy.Controls[1]).UniqueID + "','False');\"><img src=\"/shared/images/icons/edit.png\" alt=\"Edit\" style=\"vertical-align:bottom;\" />&nbsp;&nbsp;Edit Text</a></div>", ((Literal)phDummy.Controls[0]).Text);

            Assert.AreEqual(typeof(Panel), phDummy.Controls[1].GetType());
            Assert.AreEqual("rtepanel" + field.attribute.attributeid, phDummy.Controls[1].ID);
            Assert.AreEqual("rtePanel", ((Panel)phDummy.Controls[1]).CssClass);
            Assert.AreEqual(1, phDummy.Controls[1].Controls.Count);

            Assert.AreEqual(typeof(Literal), phDummy.Controls[1].Controls[0].GetType());
            Assert.AreEqual("rteliteral" + field.attribute.attributeid, phDummy.Controls[1].Controls[0].ID);

            Assert.AreEqual(typeof(TextBox), phDummy.Controls[2].GetType());
            Assert.AreEqual("txt" + field.attribute.attributeid, phDummy.Controls[2].ID);
            Assert.AreEqual(1, ((TextBox)phDummy.Controls[2]).Style.Count);
            Assert.AreEqual("none", ((TextBox)phDummy.Controls[2]).Style["display"]);
            Assert.AreEqual(true, ((TextBox)phDummy.Controls[2]).Enabled);

            Assert.AreEqual(typeof(HiddenField), phDummy.Controls[3].GetType());
            Assert.AreEqual("txthidden" + field.attribute.attributeid, phDummy.Controls[3].ID);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_RichTextAreaFieldReadOnly()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template(fieldType: FieldType.LargeText, format: AttributeFormat.FormattedText), readOnly: true);
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(3, phDummy.Controls.Count);

            Assert.AreEqual(typeof(Panel), phDummy.Controls[0].GetType());
            Assert.AreEqual("rtepanel" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("rtePanelReadOnly", ((Panel)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(1, phDummy.Controls[0].Controls.Count);

            Assert.AreEqual(typeof(Literal), phDummy.Controls[0].Controls[0].GetType());
            Assert.AreEqual("rteliteral" + field.attribute.attributeid, phDummy.Controls[0].Controls[0].ID);

            Assert.AreEqual(typeof(TextBox), phDummy.Controls[1].GetType());
            Assert.AreEqual("txt" + field.attribute.attributeid, phDummy.Controls[1].ID);
            Assert.AreEqual(1, ((TextBox)phDummy.Controls[1]).Style.Count);
            Assert.AreEqual("none", ((TextBox)phDummy.Controls[1]).Style["display"]);
            Assert.AreEqual(false, ((TextBox)phDummy.Controls[1]).Enabled);

            Assert.AreEqual(typeof(HiddenField), phDummy.Controls[2].GetType());
            Assert.AreEqual("txthidden" + field.attribute.attributeid, phDummy.Controls[2].ID);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_TickBoxField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTickboxAttributeObject.Template());
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(DropDownList), phDummy.Controls[0].GetType());
            Assert.AreEqual("cmb" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("fillspan", ((DropDownList)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(2, ((DropDownList)phDummy.Controls[0]).Items.Count);
            Assert.AreEqual("Yes", ((DropDownList)phDummy.Controls[0]).Items.FindByValue("1").Text);
            Assert.AreEqual("No", ((DropDownList)phDummy.Controls[0]).Items.FindByValue("0").Text);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Items.FindByValue("1").Selected);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Items.FindByValue("0").Selected);
            Assert.AreEqual(true, ((DropDownList)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_TickBoxFieldDefaultYes()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTickboxAttributeObject.Template(defaultValue: "Yes"));
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(DropDownList), phDummy.Controls[0].GetType());
            Assert.AreEqual("cmb" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("fillspan", ((DropDownList)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(2, ((DropDownList)phDummy.Controls[0]).Items.Count);
            Assert.AreEqual("Yes", ((DropDownList)phDummy.Controls[0]).Items.FindByValue("1").Text);
            Assert.AreEqual("No", ((DropDownList)phDummy.Controls[0]).Items.FindByValue("0").Text);
            Assert.AreEqual(true, ((DropDownList)phDummy.Controls[0]).Items.FindByValue("1").Selected);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Items.FindByValue("0").Selected);
            Assert.AreEqual(true, ((DropDownList)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_TickBoxFieldReadOnly()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTickboxAttributeObject.Template(), readOnly: true);
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(DropDownList), phDummy.Controls[0].GetType());
            Assert.AreEqual("cmb" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("fillspan", ((DropDownList)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(2, ((DropDownList)phDummy.Controls[0]).Items.Count);
            Assert.AreEqual("Yes", ((DropDownList)phDummy.Controls[0]).Items.FindByValue("1").Text);
            Assert.AreEqual("No", ((DropDownList)phDummy.Controls[0]).Items.FindByValue("0").Text);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Items.FindByValue("1").Selected);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Items.FindByValue("0").Selected);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        [ExpectedException(typeof(NullReferenceException))]
        public void cCustomEntities_generateFieldsInput_ListFieldNullListItems()
        {
            // This should error as the custom entities caching routines check for a null list and populate with an empty one
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cListAttributeObject.Template());
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_ListFieldWithNoListItems()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cListAttributeObject.Template(items: new SortedList<int, cListAttributeElement>()));
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(DropDownList), phDummy.Controls[0].GetType());
            Assert.AreEqual("cmb" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("fillspan", ((DropDownList)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(1, ((DropDownList)phDummy.Controls[0]).Items.Count);
            Assert.AreEqual("[None]", ((DropDownList)phDummy.Controls[0]).Items.FindByValue("-1").Text);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Items.FindByValue("-1").Selected);
            Assert.AreEqual(true, ((DropDownList)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_ListFieldWithNoListItemsReadOnly()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cListAttributeObject.Template(items: new SortedList<int, cListAttributeElement>()), readOnly: true);
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(DropDownList), phDummy.Controls[0].GetType());
            Assert.AreEqual("cmb" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("fillspan", ((DropDownList)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(1, ((DropDownList)phDummy.Controls[0]).Items.Count);
            Assert.AreEqual("[None]", ((DropDownList)phDummy.Controls[0]).Items.FindByValue("-1").Text);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Items.FindByValue("-1").Selected);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_ListFieldWithListItem()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cListAttributeElement item = cListAttributeElementObject.Template();
            SortedList<int, cListAttributeElement> lstItems = new SortedList<int, cListAttributeElement> { { item.elementOrder, item } };

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cListAttributeObject.Template(items: lstItems));
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(DropDownList), phDummy.Controls[0].GetType());
            Assert.AreEqual("cmb" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("fillspan", ((DropDownList)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(2, ((DropDownList)phDummy.Controls[0]).Items.Count);
            Assert.AreEqual("[None]", ((DropDownList)phDummy.Controls[0]).Items.FindByValue("-1").Text);
            Assert.AreEqual(item.elementText, ((DropDownList)phDummy.Controls[0]).Items.FindByValue(item.elementValue.ToString()).Text);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Items.FindByValue("-1").Selected);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Items.FindByValue(item.elementValue.ToString()).Selected);
            Assert.AreEqual(true, ((DropDownList)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_ListFieldWithListItemsOrdered()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cListAttributeElement item1 = cListAttributeElementObject.Template();
            cListAttributeElement item2 = cListAttributeElementObject.Template(elementValue: 20, elementText: "Boo!", sequence: 300);
            SortedList<int, cListAttributeElement> lstItems = new SortedList<int, cListAttributeElement> { { item2.elementOrder, item2 }, { item1.elementOrder, item1 } };

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cListAttributeObject.Template(items: lstItems));
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(DropDownList), phDummy.Controls[0].GetType());
            Assert.AreEqual("cmb" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("fillspan", ((DropDownList)phDummy.Controls[0]).CssClass);
            Assert.AreEqual(3, ((DropDownList)phDummy.Controls[0]).Items.Count);
            Assert.AreEqual("[None]", ((DropDownList)phDummy.Controls[0]).Items.FindByValue("-1").Text);
            Assert.AreEqual(item1.elementText, ((DropDownList)phDummy.Controls[0]).Items.FindByValue(item1.elementValue.ToString()).Text);
            Assert.AreEqual(item2.elementText, ((DropDownList)phDummy.Controls[0]).Items.FindByValue(item2.elementValue.ToString()).Text);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Items.FindByValue("-1").Selected);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Items.FindByValue(item1.elementValue.ToString()).Selected);
            Assert.AreEqual(false, ((DropDownList)phDummy.Controls[0]).Items.FindByValue(item2.elementValue.ToString()).Selected);
            Assert.AreEqual("-1", ((DropDownList)phDummy.Controls[0]).Items[0].Value);
            Assert.AreEqual("0", ((DropDownList)phDummy.Controls[0]).Items[1].Value);
            Assert.AreEqual("20", ((DropDownList)phDummy.Controls[0]).Items[2].Value);
            Assert.AreEqual(true, ((DropDownList)phDummy.Controls[0]).Enabled);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void cCustomEntities_generateFieldsInput_RunWorkflowField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cRunWorkflowAttributeObject.Template(workflow: cWorkflowObject.Template()));
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            int nRecordid = 0; // this is set as a private global on the cCustomEntities class by generateForm before we ever get to generateFieldsInput

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(Literal), phDummy.Controls[0].GetType());
            Assert.AreEqual("litRunWorkflow" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("<a href=\"javascript:runWorkflow(" + ((cRunWorkflowAttribute)field.attribute).workflow.workflowid + ", " + nRecordid + ");\">" + field.attribute.description + "</a>", ((Literal)phDummy.Controls[0]).Text);
        }

        /// <summary>
        ///A test for generateFieldsInput
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_CurrencyListField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            #region Get a default currency list
            cCurrencies clsCurrencies = new cCurrencies(Moqs.CurrentUser().AccountID, Moqs.CurrentUser().CurrentSubAccountId);
            ListItem[] lstItems = clsCurrencies.CreateDropDown().ToArray();
            string defaultItemID = "0";
            string defaultItemName = string.Empty;
            int currID = 0;
            #endregion Get a default currency list

            if (lstItems.Length == 0)
            {
                cGlobalCurrencies gCurs = new cGlobalCurrencies();
                cGlobalCurrency gCur = gCurs.getGlobalCurrencyByAlphaCode("GBP");
                try
                {
                    currID = clsCurrencies.saveCurrency(new cCurrency(Moqs.CurrentUser().AccountID, 0, gCur.globalcurrencyid, (byte)1, (byte)1, false, DateTime.UtcNow, Moqs.CurrentUser().EmployeeID, null, null));
                    clsCurrencies = new cCurrencies(Moqs.CurrentUser().AccountID, Moqs.CurrentUser().CurrentSubAccountId);
                    lstItems = clsCurrencies.CreateDropDown(currID);
                    defaultItemID = currID.ToString();
                    defaultItemName = gCur.label;

                    cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cCurrencyListAttributeObject.Template());
                    cCustomEntity entity = cCustomEntityObject.Template(defaultCurrencyID: currID);
                    cCustomEntityForm form = cCustomEntityFormObject.Template();
                    int tabID = 0;
                    string validationGroup = "valGroup";
                    target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

                    Assert.AreEqual(1, phDummy.Controls.Count);
                    Assert.AreEqual(typeof(DropDownList), phDummy.Controls[0].GetType());
                    Assert.AreEqual("ddl" + field.attribute.attributeid, phDummy.Controls[0].ID);
                    Assert.AreEqual("fillspan", ((DropDownList)phDummy.Controls[0]).CssClass);
                    Assert.AreEqual(lstItems.Length, ((DropDownList)phDummy.Controls[0]).Items.Count);
                    Assert.AreEqual(defaultItemName, ((DropDownList)phDummy.Controls[0]).Items.FindByValue(defaultItemID).Text);
                    Assert.AreEqual(true, ((DropDownList)phDummy.Controls[0]).Items.FindByValue(defaultItemID).Selected);
                }
                finally
                {
                    clsCurrencies.deleteCurrency(currID);
                }
            }
            else
            {
                defaultItemID = lstItems[0].Value;
                defaultItemName = lstItems[0].Text;
                currID = Convert.ToInt32(lstItems[0].Value);
                lstItems = clsCurrencies.CreateDropDown(currID);

                cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cCurrencyListAttributeObject.Template());
                cCustomEntity entity = cCustomEntityObject.Template(defaultCurrencyID: currID);
                cCustomEntityForm form = cCustomEntityFormObject.Template();
                int tabID = 0;
                string validationGroup = "valGroup";
                target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

                Assert.AreEqual(1, phDummy.Controls.Count);
                Assert.AreEqual(typeof(DropDownList), phDummy.Controls[0].GetType());
                Assert.AreEqual("ddl" + field.attribute.attributeid, phDummy.Controls[0].ID);
                Assert.AreEqual("fillspan", ((DropDownList)phDummy.Controls[0]).CssClass);
                Assert.AreEqual(lstItems.Length, ((DropDownList)phDummy.Controls[0]).Items.Count);
                Assert.AreEqual(defaultItemName, ((DropDownList)phDummy.Controls[0]).Items.FindByValue(defaultItemID).Text);
                Assert.AreEqual(true, ((DropDownList)phDummy.Controls[0]).Items.FindByValue(defaultItemID).Selected);
            }
        }

        /// <summary>
        /// A test for generateFieldsInput
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsInput_AdviceField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cCommentAttributeObject.Template());
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref phDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(Literal), phDummy.Controls[0].GetType());
            Assert.AreEqual("litAdvice" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual(((cCommentAttribute)field.attribute).commentText, ((Literal)phDummy.Controls[0]).Text);
        }

        /// <summary>
        /// A test to generate a Lookup Field
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_generateFieldsInput_LookupField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder placeholderDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: LookupDisplayFieldObject.Template());
            cCustomEntity entity = cCustomEntityObject.Template();
            cCustomEntityForm form = cCustomEntityFormObject.Template();
            int tabID = 0;
            string validationGroup = "valGroup";
            target.GenerateFieldsInput(ref placeholderDummy, field, validationGroup, entity, form, tabID);

            Assert.AreEqual(1, placeholderDummy.Controls.Count);
            Assert.AreEqual(typeof(Label), placeholderDummy.Controls[0].GetType());
            Assert.AreEqual("txt" + field.attribute.attributeid, placeholderDummy.Controls[0].ID);
        }

        #endregion Fields Generators

        #region Mandatory Field Generators
        /// <summary>
        ///A test for generateFieldsMandatoryValidator
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void cCustomEntities_generateFieldsMandatoryValidator_HyperlinkField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cHyperlinkAttributeObject.Template());
            string validationGroup = "valGroup";
            target.GenerateFieldsMandatoryValidator(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(RequiredFieldValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("req" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((RequiredFieldValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.MandatoryText(field.attribute.displayname), ((RequiredFieldValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((RequiredFieldValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(validationGroup, ((RequiredFieldValidator)phDummy.Controls[0]).ValidationGroup);
        }

        /// <summary>
        ///A test for generateFieldsMandatoryValidator
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsMandatoryValidator_TextField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTextAttributeObject.Template());
            string validationGroup = "valGroup";
            target.GenerateFieldsMandatoryValidator(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(RequiredFieldValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("req" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((RequiredFieldValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.MandatoryText(field.attribute.displayname), ((RequiredFieldValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((RequiredFieldValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(validationGroup, ((RequiredFieldValidator)phDummy.Controls[0]).ValidationGroup);
        }

        /// <summary>
        ///A test for generateFieldsMandatoryValidator
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsMandatoryValidator_DateTimeField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cDateTimeAttributeObject.Template());
            string validationGroup = "valGroup";
            target.GenerateFieldsMandatoryValidator(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(RequiredFieldValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("req" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((RequiredFieldValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.MandatoryText(field.attribute.displayname), ((RequiredFieldValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((RequiredFieldValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(validationGroup, ((RequiredFieldValidator)phDummy.Controls[0]).ValidationGroup);
        }

        /// <summary>
        ///A test for generateFieldsMandatoryValidator
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsMandatoryValidator_NumberField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cNumberAttributeObject.Template());
            string validationGroup = "valGroup";
            target.GenerateFieldsMandatoryValidator(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(RequiredFieldValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("req" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((RequiredFieldValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.MandatoryText(field.attribute.displayname), ((RequiredFieldValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((RequiredFieldValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(validationGroup, ((RequiredFieldValidator)phDummy.Controls[0]).ValidationGroup);
        }

        /// <summary>
        ///A test for generateFieldsMandatoryValidator
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsMandatoryValidator_ListField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cListAttributeObject.Template());
            string validationGroup = "valGroup";
            target.GenerateFieldsMandatoryValidator(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(CompareValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("req" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("cmb" + field.attribute.attributeid, ((CompareValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.MandatoryDropdown(field.attribute.displayname), ((CompareValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((CompareValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(ValidationDataType.Integer, ((CompareValidator)phDummy.Controls[0]).Type);
            Assert.AreEqual("-1", ((CompareValidator)phDummy.Controls[0]).ValueToCompare);
            Assert.AreEqual(ValidationCompareOperator.GreaterThan, ((CompareValidator)phDummy.Controls[0]).Operator);
            Assert.AreEqual(validationGroup, ((CompareValidator)phDummy.Controls[0]).ValidationGroup);
        }


        /// <summary>
        ///A test for generateFieldsMandatoryValidator
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsMandatoryValidator_TickBoxField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cTickboxAttributeObject.Template());
            string validationGroup = "valGroup";
            target.GenerateFieldsMandatoryValidator(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(CompareValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("req" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("cmb" + field.attribute.attributeid, ((CompareValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.MandatoryDropdown(field.attribute.displayname), ((CompareValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((CompareValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(ValidationDataType.Integer, ((CompareValidator)phDummy.Controls[0]).Type);
            Assert.AreEqual("-1", ((CompareValidator)phDummy.Controls[0]).ValueToCompare);
            Assert.AreEqual(ValidationCompareOperator.GreaterThan, ((CompareValidator)phDummy.Controls[0]).Operator);
            Assert.AreEqual(validationGroup, ((CompareValidator)phDummy.Controls[0]).ValidationGroup);
        }

        /// <summary>
        ///A test for generateFieldsMandatoryValidator
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void cCustomEntities_generateFieldsMandatoryValidator_RelationshipTextBoxField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cRelationshipTextBoxAttributeObject.Template());
            string validationGroup = "valGroup";
            target.GenerateFieldsMandatoryValidator(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(RequiredFieldValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("req" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((RequiredFieldValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.MandatoryText(field.attribute.displayname), ((RequiredFieldValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((RequiredFieldValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(validationGroup, ((RequiredFieldValidator)phDummy.Controls[0]).ValidationGroup);
        }

        /// <summary>
        ///A test for generateFieldsMandatoryValidator
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void cCustomEntities_generateFieldsMandatoryValidator_RunworkflowField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cRunWorkflowAttributeObject.Template());
            string validationGroup = "valGroup";
            target.GenerateFieldsMandatoryValidator(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(RequiredFieldValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("req" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((RequiredFieldValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.MandatoryText(field.attribute.displayname), ((RequiredFieldValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((RequiredFieldValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(validationGroup, ((RequiredFieldValidator)phDummy.Controls[0]).ValidationGroup);
        }

        /// <summary>
        ///A test for generateFieldsMandatoryValidator
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsMandatoryValidator_CurrencyListField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cCurrencyListAttributeObject.Template());
            string validationGroup = "valGroup";
            target.GenerateFieldsMandatoryValidator(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(RequiredFieldValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("req" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((RequiredFieldValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.MandatoryText(field.attribute.displayname), ((RequiredFieldValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((RequiredFieldValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(validationGroup, ((RequiredFieldValidator)phDummy.Controls[0]).ValidationGroup);
        }

        /// <summary>
        ///A test for generateFieldsMandatoryValidator
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsMandatoryValidator_SummaryField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cSummaryAttributeObject.Template());
            string validationGroup = "valGroup";
            target.GenerateFieldsMandatoryValidator(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(RequiredFieldValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("req" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((RequiredFieldValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.MandatoryText(field.attribute.displayname), ((RequiredFieldValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((RequiredFieldValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(validationGroup, ((RequiredFieldValidator)phDummy.Controls[0]).ValidationGroup);
        }

        /// <summary>
        ///A test for generateFieldsMandatoryValidator
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsMandatoryValidator_AdviceField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cCommentAttributeObject.Template());
            string validationGroup = "valGroup";
            target.GenerateFieldsMandatoryValidator(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(RequiredFieldValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("req" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((RequiredFieldValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.MandatoryText(field.attribute.displayname), ((RequiredFieldValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((RequiredFieldValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(validationGroup, ((RequiredFieldValidator)phDummy.Controls[0]).ValidationGroup);
        }
        #endregion Mandatory Field Generators

        #region Field Formatting Validator Generators
        /// <summary>
        ///A test for generateFieldsFormattingValidators
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void cCustomEntities_generateFieldsFormattingValidators_HyperlinkField()
        {
            cCustomEntities target = new cCustomEntities();
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cHyperlinkAttributeObject.Template());
            string validationGroup = "valGroup";
            target.GenerateFieldsFormattingValidators(ref phDummy, field, validationGroup);

            Assert.AreEqual(0, phDummy.Controls.Count);
        }

        /// <summary>
        ///A test for generateFieldsFormattingValidators
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsFormattingValidators_CurrencyField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cNumberAttributeObject.Template(fieldType: FieldType.Currency));
            string validationGroup = "valGroup";
            target.GenerateFieldsFormattingValidators(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(CompareValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("comp" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((CompareValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.FormatCurrency(field.attribute.displayname), ((CompareValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((CompareValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(ValidationDataType.Currency, ((CompareValidator)phDummy.Controls[0]).Type);
            Assert.AreEqual(ValidationCompareOperator.DataTypeCheck, ((CompareValidator)phDummy.Controls[0]).Operator);
            Assert.AreEqual(validationGroup, ((CompareValidator)phDummy.Controls[0]).ValidationGroup);
        }

        /// <summary>
        ///A test for generateFieldsFormattingValidators
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsFormattingValidators_IntegerField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cNumberAttributeObject.Template(fieldType: FieldType.Integer));
            string validationGroup = "valGroup";
            target.GenerateFieldsFormattingValidators(ref phDummy, field, validationGroup);

            Assert.AreEqual(3, phDummy.Controls.Count);
            Assert.AreEqual(typeof(CompareValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("comp" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((CompareValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.FormatInteger(field.attribute.displayname), ((CompareValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((CompareValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(ValidationDataType.Integer, ((CompareValidator)phDummy.Controls[0]).Type);
            Assert.AreEqual(ValidationCompareOperator.DataTypeCheck, ((CompareValidator)phDummy.Controls[0]).Operator);
            Assert.AreEqual(validationGroup, ((CompareValidator)phDummy.Controls[0]).ValidationGroup);

            Assert.AreEqual(typeof(CompareValidator), phDummy.Controls[1].GetType());
            Assert.AreEqual("compgte" + field.attribute.attributeid, phDummy.Controls[1].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((CompareValidator)phDummy.Controls[1]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.FormatIntegerGreaterThan(field.attribute.displayname, -2147483648), ((CompareValidator)phDummy.Controls[1]).ErrorMessage);
            Assert.AreEqual("*", ((CompareValidator)phDummy.Controls[1]).Text);
            Assert.AreEqual(ValidationDataType.Integer, ((CompareValidator)phDummy.Controls[1]).Type);
            Assert.AreEqual(ValidationCompareOperator.GreaterThanEqual, ((CompareValidator)phDummy.Controls[1]).Operator);
            Assert.AreEqual(validationGroup, ((CompareValidator)phDummy.Controls[1]).ValidationGroup);

            Assert.AreEqual(typeof(CompareValidator), phDummy.Controls[2].GetType());
            Assert.AreEqual("complte" + field.attribute.attributeid, phDummy.Controls[2].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((CompareValidator)phDummy.Controls[2]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.FormatIntegerLessThan(field.attribute.displayname, 2147483647), ((CompareValidator)phDummy.Controls[2]).ErrorMessage);
            Assert.AreEqual("*", ((CompareValidator)phDummy.Controls[2]).Text);
            Assert.AreEqual(ValidationDataType.Integer, ((CompareValidator)phDummy.Controls[2]).Type);
            Assert.AreEqual(ValidationCompareOperator.LessThanEqual, ((CompareValidator)phDummy.Controls[2]).Operator);
            Assert.AreEqual(validationGroup, ((CompareValidator)phDummy.Controls[2]).ValidationGroup);
        }

        /// <summary>
        ///A test for generateFieldsFormattingValidators
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsFormattingValidators_NumberField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cNumberAttributeObject.Template(fieldType: FieldType.Number));
            string validationGroup = "valGroup";
            target.GenerateFieldsFormattingValidators(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);
            Assert.AreEqual(typeof(CompareValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("comp" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((CompareValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.FormatNumber(field.attribute.displayname), ((CompareValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((CompareValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(ValidationDataType.Double, ((CompareValidator)phDummy.Controls[0]).Type);
            Assert.AreEqual(ValidationCompareOperator.DataTypeCheck, ((CompareValidator)phDummy.Controls[0]).Operator);
            Assert.AreEqual(validationGroup, ((CompareValidator)phDummy.Controls[0]).ValidationGroup);
        }

        /// <summary>
        ///A test for generateFieldsFormattingValidators
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsFormattingValidators_DateTimeDateOnlyField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cDateTimeAttributeObject.Template(format: AttributeFormat.DateOnly));
            string validationGroup = "valGroup";
            target.GenerateFieldsFormattingValidators(ref phDummy, field, validationGroup);

            Assert.AreEqual(3, phDummy.Controls.Count);

            Assert.AreEqual(typeof(CompareValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("comp" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((CompareValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.FormatDate(field.attribute.displayname), ((CompareValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((CompareValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(ValidationDataType.Date, ((CompareValidator)phDummy.Controls[0]).Type);
            Assert.AreEqual(ValidationCompareOperator.DataTypeCheck, ((CompareValidator)phDummy.Controls[0]).Operator);
            Assert.AreEqual(validationGroup, ((CompareValidator)phDummy.Controls[0]).ValidationGroup);
            Assert.AreEqual(ValidatorDisplay.Dynamic, ((CompareValidator)phDummy.Controls[0]).Display);

            string minDate = new DateTime(1753, 1, 1).ToShortDateString();

            Assert.AreEqual(typeof(CompareValidator), phDummy.Controls[1].GetType());
            Assert.AreEqual("compgte" + field.attribute.attributeid, phDummy.Controls[1].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((CompareValidator)phDummy.Controls[1]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.FormatDateMinimum(field.attribute.displayname, minDate), ((CompareValidator)phDummy.Controls[1]).ErrorMessage);
            Assert.AreEqual("*", ((CompareValidator)phDummy.Controls[1]).Text);
            Assert.AreEqual(ValidationDataType.Date, ((CompareValidator)phDummy.Controls[1]).Type);
            Assert.AreEqual(minDate, ((CompareValidator)phDummy.Controls[1]).ValueToCompare);
            Assert.AreEqual(ValidationCompareOperator.GreaterThanEqual, ((CompareValidator)phDummy.Controls[1]).Operator);
            Assert.AreEqual(validationGroup, ((CompareValidator)phDummy.Controls[1]).ValidationGroup);
            Assert.AreEqual(ValidatorDisplay.Dynamic, ((CompareValidator)phDummy.Controls[1]).Display);

            string maxDate = new DateTime(3000, 12, 31).ToShortDateString();

            Assert.AreEqual(typeof(CompareValidator), phDummy.Controls[2].GetType());
            Assert.AreEqual("complte" + field.attribute.attributeid, phDummy.Controls[2].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((CompareValidator)phDummy.Controls[2]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.FormatDateMaximum(field.attribute.displayname, maxDate), ((CompareValidator)phDummy.Controls[2]).ErrorMessage);
            Assert.AreEqual("*", ((CompareValidator)phDummy.Controls[2]).Text);
            Assert.AreEqual(ValidationDataType.Date, ((CompareValidator)phDummy.Controls[2]).Type);
            Assert.AreEqual(maxDate, ((CompareValidator)phDummy.Controls[2]).ValueToCompare);
            Assert.AreEqual(ValidationCompareOperator.LessThanEqual, ((CompareValidator)phDummy.Controls[2]).Operator);
            Assert.AreEqual(validationGroup, ((CompareValidator)phDummy.Controls[2]).ValidationGroup);
            Assert.AreEqual(ValidatorDisplay.Dynamic, ((CompareValidator)phDummy.Controls[2]).Display);
        }

        /// <summary>
        ///A test for generateFieldsFormattingValidators
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsFormattingValidators_DateTimeTimeOnlyField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cDateTimeAttributeObject.Template(format: AttributeFormat.TimeOnly));
            string validationGroup = "valGroup";
            target.GenerateFieldsFormattingValidators(ref phDummy, field, validationGroup);

            Assert.AreEqual(1, phDummy.Controls.Count);

            Assert.AreEqual(typeof(RegularExpressionValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("comp" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((RegularExpressionValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.FormatTime(field.attribute.displayname), ((RegularExpressionValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((RegularExpressionValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual("^(([0-9])|([0-1][0-9])|([2][0-3])):(([0-9])|([0-5][0-9]))$", ((RegularExpressionValidator)phDummy.Controls[0]).ValidationExpression);
            Assert.AreEqual(validationGroup, ((RegularExpressionValidator)phDummy.Controls[0]).ValidationGroup);
            Assert.AreEqual(ValidatorDisplay.Dynamic, ((RegularExpressionValidator)phDummy.Controls[0]).Display);
        }

        /// <summary>
        ///A test for generateFieldsFormattingValidators
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateFieldsFormattingValidators_DateTimeDateAndTimeField()
        {
            cCustomEntities target = new cCustomEntities(Moqs.CurrentUser());
            PlaceHolder phDummy = new PlaceHolder();

            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cDateTimeAttributeObject.Template(format: AttributeFormat.DateTime));
            string validationGroup = "valGroup";
            target.GenerateFieldsFormattingValidators(ref phDummy, field, validationGroup);

            Assert.AreEqual(6, phDummy.Controls.Count);

            Assert.AreEqual(typeof(CompareValidator), phDummy.Controls[0].GetType());
            Assert.AreEqual("comp" + field.attribute.attributeid, phDummy.Controls[0].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((CompareValidator)phDummy.Controls[0]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.FormatDateAndTime(field.attribute.displayname), ((CompareValidator)phDummy.Controls[0]).ErrorMessage);
            Assert.AreEqual("*", ((CompareValidator)phDummy.Controls[0]).Text);
            Assert.AreEqual(ValidationDataType.Date, ((CompareValidator)phDummy.Controls[0]).Type);
            Assert.AreEqual(ValidationCompareOperator.DataTypeCheck, ((CompareValidator)phDummy.Controls[0]).Operator);
            Assert.AreEqual(validationGroup, ((CompareValidator)phDummy.Controls[0]).ValidationGroup);
            Assert.AreEqual(ValidatorDisplay.Dynamic, ((CompareValidator)phDummy.Controls[0]).Display);

            string minDate = new DateTime(1753, 1, 1).ToShortDateString();

            Assert.AreEqual(typeof(CompareValidator), phDummy.Controls[1].GetType());
            Assert.AreEqual("compgte" + field.attribute.attributeid, phDummy.Controls[1].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((CompareValidator)phDummy.Controls[1]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.FormatDateAndTimeMinimum(field.attribute.displayname, minDate), ((CompareValidator)phDummy.Controls[1]).ErrorMessage);
            Assert.AreEqual("*", ((CompareValidator)phDummy.Controls[1]).Text);
            Assert.AreEqual(ValidationDataType.Date, ((CompareValidator)phDummy.Controls[1]).Type);
            Assert.AreEqual(ValidationCompareOperator.GreaterThanEqual, ((CompareValidator)phDummy.Controls[1]).Operator);
            Assert.AreEqual(validationGroup, ((CompareValidator)phDummy.Controls[1]).ValidationGroup);
            Assert.AreEqual(ValidatorDisplay.Dynamic, ((CompareValidator)phDummy.Controls[1]).Display);

            string maxDate = new DateTime(3000, 12, 31).ToShortDateString();

            Assert.AreEqual(typeof(CompareValidator), phDummy.Controls[2].GetType());
            Assert.AreEqual("complte" + field.attribute.attributeid, phDummy.Controls[2].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((CompareValidator)phDummy.Controls[2]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.FormatDateAndTimeMaximum(field.attribute.displayname, maxDate), ((CompareValidator)phDummy.Controls[2]).ErrorMessage);
            Assert.AreEqual("*", ((CompareValidator)phDummy.Controls[2]).Text);
            Assert.AreEqual(ValidationDataType.Date, ((CompareValidator)phDummy.Controls[2]).Type);
            Assert.AreEqual(maxDate, ((CompareValidator)phDummy.Controls[2]).ValueToCompare);
            Assert.AreEqual(ValidationCompareOperator.LessThanEqual, ((CompareValidator)phDummy.Controls[2]).Operator);
            Assert.AreEqual(validationGroup, ((CompareValidator)phDummy.Controls[2]).ValidationGroup);
            Assert.AreEqual(ValidatorDisplay.Dynamic, ((CompareValidator)phDummy.Controls[2]).Display);

            Assert.AreEqual(typeof(RegularExpressionValidator), phDummy.Controls[3].GetType());
            Assert.AreEqual("comp" + field.attribute.attributeid + "_time", phDummy.Controls[3].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid + "_time", ((RegularExpressionValidator)phDummy.Controls[3]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.FormatDateAndTime(field.attribute.displayname), ((RegularExpressionValidator)phDummy.Controls[3]).ErrorMessage);
            Assert.AreEqual("*", ((RegularExpressionValidator)phDummy.Controls[3]).Text);
            Assert.AreEqual("^(([0-9])|([0-1][0-9])|([2][0-3])):(([0-9])|([0-5][0-9]))$", ((RegularExpressionValidator)phDummy.Controls[3]).ValidationExpression);
            Assert.AreEqual(validationGroup, ((RegularExpressionValidator)phDummy.Controls[3]).ValidationGroup);
            Assert.AreEqual(ValidatorDisplay.Dynamic, ((RegularExpressionValidator)phDummy.Controls[3]).Display);

            Assert.AreEqual(typeof(RequiredFieldValidator), phDummy.Controls[4].GetType());
            Assert.AreEqual("reqDate" + field.attribute.attributeid, phDummy.Controls[4].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid, ((RequiredFieldValidator)phDummy.Controls[4]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.MandatoryDateFromDateTime(field.attribute.displayname), ((RequiredFieldValidator)phDummy.Controls[4]).ErrorMessage);
            Assert.AreEqual("*", ((RequiredFieldValidator)phDummy.Controls[4]).Text);
            Assert.AreEqual(false, ((RequiredFieldValidator)phDummy.Controls[4]).Enabled);
            Assert.AreEqual(validationGroup, ((RequiredFieldValidator)phDummy.Controls[4]).ValidationGroup);

            Assert.AreEqual(typeof(RequiredFieldValidator), phDummy.Controls[5].GetType());
            Assert.AreEqual("reqTime" + field.attribute.attributeid + "_time", phDummy.Controls[5].ID);
            Assert.AreEqual("txt" + field.attribute.attributeid + "_time", ((RequiredFieldValidator)phDummy.Controls[5]).ControlToValidate);
            Assert.AreEqual(ValidatorMessages.MandatoryTimeFromDateTime(field.attribute.displayname), ((RequiredFieldValidator)phDummy.Controls[5]).ErrorMessage);
            Assert.AreEqual("*", ((RequiredFieldValidator)phDummy.Controls[5]).Text);
            Assert.AreEqual(false, ((RequiredFieldValidator)phDummy.Controls[5]).Enabled);
            Assert.AreEqual(validationGroup, ((RequiredFieldValidator)phDummy.Controls[5]).ValidationGroup);
        }
        #endregion Field Formatting Validator Generators

        #region SaveEntity

        /// <summary>
        ///A test for saveEntity, where the description field has been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveEntity_WithDescriptionSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            int nCustomEntityID = 0;
            string sEntityName = "saveEntity Unit Test Custom Entity";
            string sPluralName = "saveEntity Unit Test Custom Entities";
            DateTime dtCreatedOn = DateTime.Now;

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                cCustomEntity expected = cCustomEntityObject.Template(createdBy: nEmployeeID, entityName: sEntityName, pluralName: sPluralName, createdOn: dtCreatedOn);

                nCustomEntityID = customEntities.saveEntity(expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Ensure the custom entity has been saved and an ID has been set
                Assert.AreNotEqual(0, nCustomEntityID);

                // Get the newly created custom entity
                cCustomEntity actual = customEntities.getEntityById(nCustomEntityID);

                // Set the entity ID of the expected custom entity
                expected = cCustomEntityObject.Template(entityID: nCustomEntityID, createdBy: nEmployeeID, entityName: sEntityName, pluralName: sPluralName, createdOn: dtCreatedOn);

                // Do not compare the values of CreatedOn, as they may be different, the number of attributes will be different
                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: new List<string> { "attributes", "createdon" });

                // the auto-created attributes should be present
                Assert.AreEqual(5, actual.attributes.Count);
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "ID" && x.GetType() == typeof(cNumberAttribute)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Created By" && x.GetType() == typeof(cManyToOneRelationship)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Modified By" && x.GetType() == typeof(cManyToOneRelationship)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Created On" && x.GetType() == typeof(cDateTimeAttribute)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Modified On" && x.GetType() == typeof(cDateTimeAttribute)));

                // Ensure the newly created entity has the correct base attributes
                Assert.AreEqual(5, actual.attributes.Count);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
            }
            finally
            {
                if (nCustomEntityID != 0)
                {
                    cCustomEntityObject.TearDown(nCustomEntityID);
                }
            }
        }

        /// <summary>
        ///A test for saveEntity, where the description field has been left blank
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveEntity_WithNoDescriptionSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            int nCustomEntityID = 0;
            string sEntityName = "saveEntity Unit Test Custom Entity";
            string sPluralName = "saveEntity Unit Test Custom Entities";
            string sDescription = "";
            DateTime dtCreatedOn = DateTime.Now;

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                cCustomEntity expected = cCustomEntityObject.Template(createdBy: nEmployeeID, entityName: sEntityName, pluralName: sPluralName, createdOn: dtCreatedOn, description: sDescription);

                nCustomEntityID = customEntities.saveEntity(expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Ensure the custom entity has been saved and an ID has been set
                Assert.AreNotEqual(0, nCustomEntityID);

                // Get the newly created custom entity
                cCustomEntity actual = customEntities.getEntityById(nCustomEntityID);

                // Set the entity ID of the expected custom entity
                expected = cCustomEntityObject.Template(entityID: nCustomEntityID, createdBy: nEmployeeID, entityName: sEntityName, pluralName: sPluralName, createdOn: dtCreatedOn, description: sDescription);

                // Do not compare the values of CreatedOn, as they may be different, the number of attributes will be different
                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: new List<string> { "attributes", "createdon" });

                // the auto-created attributes should be present
                Assert.AreEqual(5, actual.attributes.Count);
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "ID" && x.GetType() == typeof(cNumberAttribute)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Created By" && x.GetType() == typeof(cManyToOneRelationship)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Modified By" && x.GetType() == typeof(cManyToOneRelationship)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Created On" && x.GetType() == typeof(cDateTimeAttribute)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Modified On" && x.GetType() == typeof(cDateTimeAttribute)));

                // Ensure the newly created entity has the correct base attributes
                Assert.AreEqual(5, actual.attributes.Count);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
            }
            finally
            {
                if (nCustomEntityID != 0)
                {
                    cCustomEntityObject.TearDown(nCustomEntityID);
                }
            }
        }

        /// <summary>
        ///A test for saveEntity, where the default currency ID has been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveEntity_WithDefaultCurrencySet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            int nCustomEntityID = 0;
            string sEntityName = "saveEntity Unit Test Custom Entity";
            string sPluralName = "saveEntity Unit Test Custom Entities";
            cCustomEntity expected = null;

            try
            {
                cCurrencies reqCurrencies = new cCurrencies(Moqs.CurrentUser().AccountID, Moqs.CurrentUser().CurrentSubAccountId);
                cCurrency reqCurrency = (cCurrency)reqCurrencies.currencyList.GetByIndex(0);

                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.Now;

                expected = cCustomEntityObject.Template(createdBy: nEmployeeID, entityName: sEntityName, pluralName: sPluralName, createdOn: dtCreatedOn, defaultCurrencyID: reqCurrency.currencyid);

                nCustomEntityID = customEntities.saveEntity(expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Ensure the custom entity has been saved and an ID has been set
                Assert.AreNotEqual(0, nCustomEntityID);

                // Get the newly created custom entity
                cCustomEntity actual = customEntities.getEntityById(nCustomEntityID);

                // Set the entity ID of the expected custom entity
                expected = cCustomEntityObject.Template(entityID: nCustomEntityID, createdBy: nEmployeeID, entityName: sEntityName, pluralName: sPluralName, createdOn: dtCreatedOn, defaultCurrencyID: reqCurrency.currencyid);

                // Do not compare the values of CreatedOn, as they may be different, the number of attributes will be different
                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: new List<string> { "attributes", "createdon" });

                // the auto-created attributes should be present
                Assert.AreEqual(5, actual.attributes.Count);
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "ID" && x.GetType() == typeof(cNumberAttribute)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Created By" && x.GetType() == typeof(cManyToOneRelationship)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Modified By" && x.GetType() == typeof(cManyToOneRelationship)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Created On" && x.GetType() == typeof(cDateTimeAttribute)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Modified On" && x.GetType() == typeof(cDateTimeAttribute)));

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
            }
            finally
            {
                if (nCustomEntityID != 0)
                {
                    cCustomEntityObject.TearDown(nCustomEntityID);
                }
            }
        }

        /// <summary>
        ///A test for saveEntity, where modified by has been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveEntity_WithModifiedByHasBeenSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            int nCustomEntityID = 0;
            string sEntityName = "saveEntity Unit Test Custom Entity";
            string sPluralName = "saveEntity Unit Test Custom Entities";

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.Now;

                cCustomEntity expected = cCustomEntityObject.Template(createdBy: nEmployeeID, entityName: sEntityName, pluralName: sPluralName, createdOn: dtCreatedOn, modifiedBy: nEmployeeID);

                nCustomEntityID = customEntities.saveEntity(expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Ensure the custom entity has been saved and an ID has been set
                Assert.AreNotEqual(0, nCustomEntityID);

                // Get the newly created custom entity
                cCustomEntity actual = customEntities.getEntityById(nCustomEntityID);

                // Set the entity ID of the expected custom entity
                expected = cCustomEntityObject.Template(entityID: nCustomEntityID, createdBy: nEmployeeID, entityName: sEntityName, pluralName: sPluralName, createdOn: dtCreatedOn, modifiedBy: nEmployeeID);

                // Do not compare the values of CreatedOn, as they may be different, the number of attributes will be different
                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: new List<string> { "attributes", "createdon" });

                // the auto-created attributes should be present
                Assert.AreEqual(5, actual.attributes.Count);
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "ID" && x.GetType() == typeof(cNumberAttribute)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Created By" && x.GetType() == typeof(cManyToOneRelationship)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Modified By" && x.GetType() == typeof(cManyToOneRelationship)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Created On" && x.GetType() == typeof(cDateTimeAttribute)));
                Assert.IsTrue(actual.attributes.Values.Any(x => x.displayname == "Modified On" && x.GetType() == typeof(cDateTimeAttribute)));

                // Ensure the newly created entity has the correct base attributes
                Assert.AreEqual(5, actual.attributes.Count);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
            }
            finally
            {
                if (nCustomEntityID != 0)
                {
                    cCustomEntityObject.TearDown(nCustomEntityID);
                }
            }
        }

        #endregion

        #region SaveAttribute

        /// <summary>
        ///A test for saveAttribute, where description is not set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_WhereDescriptionIsNotSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveAttribute_WhereDescriptionIsNotSet unit test attribute";
            string sDescritpion = "";
            string sTooltip = "cCustomEntities_saveAttribute_WhereDescriptionIsNotSet unit test tooltip";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities reqCustomEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cNumberAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, description: sDescritpion, allowDelete: true, allowEdit: true);

                int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cNumberAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, fieldID: gFieldID, description: sDescritpion, allowDelete: true, allowEdit: true);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveAttribute, where tooltip is not set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_WhereTooltipIsNotSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveAttribute_WhereTooltipIsNotSet unit test attribute";
            string sTooltip = "";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities reqCustomEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cNumberAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, allowDelete: true, allowEdit: true);

                int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cNumberAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, allowDelete: true, allowEdit: true, fieldID: gFieldID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveAttribute, where modifiedby is not set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_WhereModifiedByIsNotSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveAttribute_WhereModifiedByIsNotSet unit test attribute";
            string sTooltip = "cCustomEntities_saveAttribute_WhereModifiedByIsNotSet unit test tooltip";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities reqCustomEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cNumberAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, modifiedBy: nEmployeeID, allowDelete: true, allowEdit: true);

                int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cNumberAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, fieldID: gFieldID, modifiedBy: nEmployeeID, allowDelete: true, allowEdit: true);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveAttribute, where format is set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_WhereFormatIsSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveAttribute_WhereFormatIsSet unit test attribute";
            string sTooltip = "cCustomEntities_saveAttribute_WhereFormatIsSet unit test tooltip";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities reqCustomEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cTextAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, modifiedBy: nEmployeeID, format: AttributeFormat.MultiLine, allowDelete: true, allowEdit: true);

                int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cTextAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, fieldID: gFieldID, modifiedBy: nEmployeeID, format: AttributeFormat.MultiLine, allowDelete: true, allowEdit: true);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveAttribute, using field type list
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_UsingFieldTypeList()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveAttribute_UsingFieldTypeList unit test attribute";
            string sTooltip = "cCustomEntities_saveAttribute_UsingFieldTypeList unit test tooltip";
            string sElementText = "Unit Test List Item";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities reqCustomEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                SortedList<int, cListAttributeElement> lstElements = new SortedList<int, cListAttributeElement>();

                cListAttributeElement reqListItemElement = cListAttributeElementObject.Template(elementText: sElementText);
                lstElements.Add(0, reqListItemElement);

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cListAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, modifiedBy: nEmployeeID, items: lstElements, fieldType: FieldType.List, allowDelete: true, allowEdit: true);

                int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cListAttributeObject.Template(attributeID: nAttributeID, fieldID: gFieldID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, modifiedBy: nEmployeeID, items: lstElements, fieldType: FieldType.List, allowDelete: true, allowEdit: true);

                // Do not compare the values of CreatedOn, as they may be different
                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: new List<string> { "elementValue", "createdon" });

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        /// A test for saveAttribute, using a List attribute that already exists. This will cover additional blocks of code
        /// in the private method SaveListItems
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_UsingExistingListAttribute()
        {
            int employeeId = Moqs.CurrentUser().EmployeeID;
            const string DisplayName = "cCustomEntities_saveAttribute_UsingExistingListAttribute unit test attribute";
            const string Tooltip = "cCustomEntities_saveAttribute_UsingExistingListAttribute unit test tooltip";
            const string ElementText = "Unit Test List Item";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: employeeId));

            try
            {
                cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                #region Create List Attribute with valid list item

                SortedList<int, cListAttributeElement> lstElements = new SortedList<int, cListAttributeElement>();

                cListAttributeElement reqListItemElement = cListAttributeElementObject.Template(elementText: ElementText);
                lstElements.Add(0, reqListItemElement);

                DateTime createdOn = DateTime.UtcNow;

                cListAttribute reqListAttribute = cListAttributeObject.Template(displayName: DisplayName, toolTip: Tooltip, createdOn: createdOn, modifiedBy: employeeId, items: lstElements, fieldType: FieldType.List, allowDelete: true, allowEdit: true);

                int attributeId = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, reqListAttribute);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                reqListAttribute = (cListAttribute)reqCustomEntity.getAttributeById(attributeId);

                #endregion

                #region Create additional list item and save it to the existing list attribute

                cListAttributeElement reqSecondListItemElement = cListAttributeElementObject.Template(elementValue: 1, elementText: ElementText + " 2", sequence: 993);

                reqListAttribute.items.Add(1, reqSecondListItemElement);

                attributeId = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, reqListAttribute);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                #endregion

                cAttribute actual = reqCustomEntity.getAttributeById(attributeId);

                // Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid fieldId = actual.fieldid;

                reqListAttribute = cListAttributeObject.Template(attributeID: attributeId, fieldID: fieldId, displayName: DisplayName, toolTip: Tooltip, createdOn: createdOn, modifiedBy: employeeId, items: lstElements, fieldType: FieldType.List, allowDelete: true, allowEdit: true);

                // Do not compare the values of CreatedOn, as they may be different and items as the saved item elementValues may be different
                cCompareAssert.AreEqual(reqListAttribute, actual, lstOmittedProperties: new List<string> { "elementValue", "createdon" });

                // Do not compare the values of item's elementValues as the saved item elementValues may be different and is expected
                //cCompareAssert.AreEqual(reqListAttribute.items, ((cListAttribute)actual).items, lstOmittedProperties: new List<string> { "elementValue" });

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= reqListAttribute.createdon.AddTicks(-reqListAttribute.createdon.Ticks));
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
        ///A test for saveAttribute, using a List attribute that already exists. This will cover additional blocks of code
        ///in the private method SaveListItems
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_UsingExistingListAttributeWhereItemsAreRemoved()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveAttribute_UsingExistingListAttributeWhereItemsAreRemoved unit test attribute";
            string sTooltip = "cCustomEntities_saveAttribute_UsingExistingListAttributeWhereItemsAreRemoved unit test tooltip";
            string sElementText = "Unit Test List Item";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID, description: "saveAttribute_UsingExistingListAttributeWhereItemsAreRemoved unit test"));

            try
            {
                cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                #region Create List Attribute with two valid list items

                SortedList<int, cListAttributeElement> lstElements = new SortedList<int, cListAttributeElement>();

                cListAttributeElement reqListItemElement = cListAttributeElementObject.Template(elementText: sElementText);
                cListAttributeElement reqSecondListItemElement = cListAttributeElementObject.Template(elementText: sElementText + " 2", sequence: 1);
                lstElements.Add(0, reqListItemElement);
                lstElements.Add(1, reqSecondListItemElement);

                DateTime dtCreatedOn = DateTime.UtcNow;

                cListAttribute reqListAttribute = cListAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, modifiedBy: nEmployeeID, items: lstElements, fieldType: FieldType.List, allowDelete: true, allowEdit: true);

                int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, reqListAttribute);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                Assert.AreEqual(2, (reqListAttribute.items.Count));

                #endregion

                #region Remove a list item and re-save the attribute

                // Update the ID of the list item within lstElements to the correct value
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                reqListAttribute = (cListAttribute)reqCustomEntity.getAttributeById(nAttributeID);
                lstElements[1] = reqListAttribute.items[1];

                lstElements.Remove(0);

                // Create a new version of reqListAttribute, without the first list item
                reqListAttribute = cListAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, modifiedBy: nEmployeeID, items: lstElements, fieldType: FieldType.List, allowDelete: true, allowEdit: true);

                reqCustomEntities.saveAttribute(reqCustomEntity.entityid, reqListAttribute);

                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                #endregion

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                reqListAttribute = cListAttributeObject.Template(attributeID: nAttributeID, fieldID: gFieldID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, modifiedBy: nEmployeeID, items: lstElements, fieldType: FieldType.List, allowDelete: true, allowEdit: true);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(reqListAttribute, actual, lstOmittedProperties: lstOmittedProps);
                Assert.AreEqual(1, ((cListAttribute)actual).items.Count);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= reqListAttribute.createdon.AddTicks(-reqListAttribute.createdon.Ticks));
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
        ///A test for saveAttribute, using a List attribute that already exists as a delegate. This will cover additional blocks of code
        ///in the private method SaveListItems
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_UsingExistingListAttributeWhereItemsAreRemovedAsDelegate()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveAttribute_UsingExistingListAttributeWhereItemsAreRemovedAsDelegate unit test attribute";
            string sTooltip = "cCustomEntities_saveAttribute_UsingExistingListAttributeWhereItemsAreRemovedAsDelegate unit test tooltip";
            string sElementText = "Unit Test List Item";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUserDelegateMock().Object);

                #region Create List Attribute with two valid list items

                SortedList<int, cListAttributeElement> lstElements = new SortedList<int, cListAttributeElement>();

                cListAttributeElement reqListItemElement = cListAttributeElementObject.Template(elementText: sElementText);
                cListAttributeElement reqSecondListItemElement = cListAttributeElementObject.Template(elementText: sElementText + " 2", sequence: 1);
                lstElements.Add(0, reqListItemElement);
                lstElements.Add(1, reqSecondListItemElement);

                DateTime dtCreatedOn = DateTime.UtcNow;

                cListAttribute reqListAttribute = cListAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, modifiedBy: nEmployeeID, items: lstElements, fieldType: FieldType.List, allowDelete: true, allowEdit: true);

                int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, reqListAttribute);


                #endregion

                #region Remove a list item and re-save the attribute


                // Update the ID of the list item within lstElements to the correct value
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);
                reqListAttribute = (cListAttribute)reqCustomEntity.getAttributeById(nAttributeID);
                lstElements[1] = reqListAttribute.items[1];

                lstElements.Remove(0);

                // Create a new version of reqListAttribute, without the first list item
                reqListAttribute = cListAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, modifiedBy: nEmployeeID, items: lstElements, fieldType: FieldType.List, allowDelete: true, allowEdit: true);

                reqCustomEntities.saveAttribute(reqCustomEntity.entityid, reqListAttribute);

                reqCustomEntities = new cCustomEntities(Moqs.CurrentUserDelegateMock().Object);

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                #endregion

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                reqListAttribute = cListAttributeObject.Template(attributeID: nAttributeID, fieldID: gFieldID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, modifiedBy: nEmployeeID, items: lstElements, fieldType: FieldType.List, allowDelete: true, allowEdit: true);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(reqListAttribute, actual, lstOmittedProperties: lstOmittedProps);
                Assert.AreEqual(1, ((cListAttribute)actual).items.Count);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= reqListAttribute.createdon.AddTicks(-reqListAttribute.createdon.Ticks));
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
        ///A test for saveAttribute, using cTextAttribute
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_UsingTextAttribute()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveAttribute_UsingTextAttribute unit test attribute";
            string sTooltip = "cCustomEntities_saveAttribute_UsingTextAttribute unit test tooltip";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities reqCustomEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cTextAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, maxLength: 69, allowDelete: true, allowEdit: true, displayInMobile: true);

                int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cTextAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, fieldID: gFieldID, maxLength: 69, allowDelete: true, allowEdit: true, displayInMobile: true);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveAttribute, using cTickboxAttribute
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_UsingTickboxAttribute()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveAttribute_UsingTickboxAttribute unit test attribute";
            string sTooltip = "cCustomEntities_saveAttribute_UsingTickboxAttribute unit test tooltip";
            string sDefaultValue = "Unit test default value";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities reqCustomEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cTickboxAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, defaultValue: sDefaultValue, allowDelete: true, allowEdit: true);

                int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cTickboxAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, defaultValue: sDefaultValue, allowDelete: true, allowEdit: true, fieldID: gFieldID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveAttribute, using cDateTimeAttribute
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_UsingDateTimeAttribute()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveAttribute_UsingDateTimeAttribute unit test attribute";
            string sTooltip = "cCustomEntities_saveAttribute_UsingDateTimeAttribute unit test tooltip";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities reqCustomEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cDateTimeAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, allowDelete: true, allowEdit: true);

                int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cDateTimeAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, allowDelete: true, allowEdit: true, fieldID: gFieldID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveAttribute, using cNumberAttribute
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_UsingNumberAttribute()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveAttribute_UsingNumberAttribute unit test attribute";
            string sTooltip = "cCustomEntities_saveAttribute_UsingNumberAttribute unit test tooltip";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities reqCustomEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cNumberAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, allowDelete: true, allowEdit: true);

                int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cNumberAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, allowDelete: true, allowEdit: true, fieldID: gFieldID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveAttribute, using cRunWorkflowAttribute !!! NOT CURRENTLY WORKING - CANNOT CREATE RUNWORKFLOW ATTRIBUTE
        ///</summary>
        //[TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        //public void cCustomEntities_saveAttribute_UsingRunWorkflowAttribute()
        //{
        //    int nEmployeeID = Moqs.CurrentUser().EmployeeID;
        //    string sDisplayName = "cCustomEntities_saveAttribute_UsingRunWorkflowAttribute unit test attribute";
        //    string sAttributeName = "cCustomEntities_saveAttribute_UsingRunWorkflowAttribute unit test name";
        //    string sTooltip = "cCustomEntities_saveAttribute_UsingRunWorkflowAttribute unit test tooltip";

        //    cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

        //    cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

        //    try
        //    {
        //        //cTables tables = new cTables(Moqs.CurrentUser().AccountID);
        //        cTable reqBaseTable = new cTable();

        //        //cWorkflow reqWorkFlow = cWorkflowObject.New(cWorkflowObject.Template(createdBy: nEmployeeID, baseTable: reqBaseTable));

        //        cWorkflow reqWorkFlow = cWorkflowObject.Template(createdBy: nEmployeeID, baseTable: reqBaseTable);

        //        DateTime dtCreatedOn = DateTime.UtcNow;

        //        cAttribute expected = cRunWorkflowAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, attributeName: sAttributeName, workflow: reqWorkFlow);

        //        int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, expected);

        //        reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

        //        cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

        //        //Store the Guid of the newly created attribute
        //        Assert.IsNotNull(actual.fieldid);
        //        Guid gFieldID = actual.fieldid;

        //        expected = cRunWorkflowAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, attributeName: sAttributeName, workflow: reqWorkFlow, fieldID: gFieldID);

        //        // Do not compare the values of CreatedOn, as they may be different
        //        List<string> lstOmittedProps = new List<string>();
        //        lstOmittedProps.Add("createdon");

        //        cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

        //        // Ensure the value of CreatedOn is greater than the one set at the start of the test
        //        Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
        //    }
        //    finally
        //    {
        //        if (reqCustomEntity != null)
        //        {
        //            reqCustomEntities.deleteEntity(reqCustomEntity.entityid, nEmployeeID, 0);
        //        }
        //    }

        //}

        /// <summary>
        ///A test for saveAttribute, using cAdviceAttribute
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_UsingCommentAttribute()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveAttribute_UsingCommentAttribute unit test attribute";
            string sTooltip = "cCustomEntities_saveAttribute_UsingCommentAttribute unit test tooltip";
            string sAdviceText = "cCustomEntities_saveAttribute_UsingCommentAttribute unit test advice text";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities reqCustomEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cCommentAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, adviceText: sAdviceText, allowDelete: true, allowEdit: true);

                int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cCommentAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, adviceText: sAdviceText, allowDelete: true, allowEdit: true, fieldID: gFieldID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveAttribute, using cAttachmentAttribute
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveAttribute_UsingAttachmentAttribute()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveAttribute_UsingAttachmentAttribute unit test attribute";
            string sTooltip = "cCustomEntities_saveAttribute_UsingAttachmentAttribute unit test tooltip";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities reqCustomEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cAttachmentAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, allowDelete: true, allowEdit: true);

                int nAttributeID = reqCustomEntities.saveAttribute(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cAttachmentAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, allowDelete: true, allowEdit: true, fieldID: gFieldID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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

        #region deleteEntity
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteEntity_EntityIDDoesNotExist()
        {
            cCustomEntities clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
            int successVal = clsCustomEntities.deleteEntity(-999, -999, -999);

            Assert.AreEqual(-2, successVal); // -2 returned if entity doesn't exist
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteEntity_ValidEntity()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = null;

            try
            {
                entity1 = cCustomEntityObject.New();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                int successVal = clsCustomEntities.deleteEntity(entity1.entityid, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                Assert.AreEqual(0, successVal);
            }
            finally
            {
                if (entity1 != null)
                {
                    cCustomEntityObject.TearDown(entity1.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteEntity_EntityReferencedByOneToMany()
        {
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;

            try
            {
                entity1 = cCustomEntityObject.New();
                entity2 = cCustomEntityObject.New();
                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid);

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity3 = clsCustomEntities.getEntityById(entity2.entityid);

                cOneToManyRelationship attribute = cOneToManyRelationshipObject.New(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, entity3));

                clsCustomEntities = new cCustomEntities(currentUser);
                entity3 = clsCustomEntities.getEntityById(entity3.entityid);

                int successVal = clsCustomEntities.deleteEntity(entity3.entityid, currentUser.EmployeeID, 0);

                Assert.AreEqual(-1, successVal);
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

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteEntity_EntityReferencedByManyToOne()
        {
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;

            try
            {
                entity1 = cCustomEntityObject.New();
                entity2 = cCustomEntityObject.New();

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity3 = clsCustomEntities.getEntityById(entity2.entityid);
                cTextAttribute txtAtt = cTextAttributeObject.New(entity3.entityid, cTextAttributeObject.Template());
                cManyToOneRelationship attribute = cManyToOneRelationshipObject.New(entity1.entityid, cManyToOneRelationshipObject.Template(relatedTable: entity3.table, autoCompleteMatchFieldIDs: new List<Guid>() { txtAtt.fieldid }, autocompleteDisplayFieldID: txtAtt.fieldid));

                clsCustomEntities = new cCustomEntities(currentUser);

                int successVal = clsCustomEntities.deleteEntity(entity3.entityid, currentUser.EmployeeID, 0);

                Assert.AreEqual(-4, successVal);
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
        #endregion deleteEntity

        #region deleteAttribute
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void cCustomEntities_deleteAttribute_InstantiatedWithoutCurrentUser()
        {
            cCustomEntities clsCustomEntities = new cCustomEntities();
            var result = clsCustomEntities.deleteAttribute(0, Moqs.CurrentUser().EmployeeID, 0);
            Assert.IsTrue(result == -1);
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_InvalidAttributeID()
        {
            cCustomEntities clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
            var result = clsCustomEntities.deleteAttribute(-999, Moqs.CurrentUser().EmployeeID, 0);
            Assert.IsTrue(result == -1, "deleteAttribute should return -1");
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_DeletePrimaryKeyAttributeShouldFail()
        {
            cCustomEntity entity = null;

            try
            {
                entity = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity.attributes.Count);

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);

                cNumberAttribute attribute = (cNumberAttribute)(from x in entity.attributes.Values
                                                                where x.iskeyfield == true && x.GetType() == typeof(cNumberAttribute)
                                                                select x).FirstOrDefault();
                Assert.IsNotNull(attribute);

                // Remove the default primary key attribute
                int retVal = clsCustomEntities.deleteAttribute(attribute.attributeid, currentUser.EmployeeID, 0);

                Assert.AreEqual(-3, retVal, "The -3 return code should have been set when trying to delete the primary key");

                // Ensure the id attribute is still there
                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity2 = clsCustomEntities.getEntityById(entity.entityid);
                attribute = (cNumberAttribute)(from x in entity2.attributes.Values
                                               where x.iskeyfield == true && x.GetType() == typeof(cNumberAttribute)
                                               select x).FirstOrDefault();
                Assert.IsNotNull(attribute);
            }
            finally
            {
                if (entity != null)
                {
                    cCustomEntityObject.TearDown(entity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void cCustomEntities_deleteAttribute_HyperlinkAttribute()
        {
            cCustomEntity entity = null;

            try
            {
                entity = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity.attributes.Count);

                // Add a new attribute
                cHyperlinkAttribute expected = cHyperlinkAttributeObject.New(entity.entityid, cHyperlinkAttributeObject.Template());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity2 = clsCustomEntities.getEntityById(entity.entityid);
                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);

                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity.entityid);
                cHyperlinkAttribute actual = (cHyperlinkAttribute)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
            }
            finally
            {
                if (entity != null)
                {
                    cCustomEntityObject.TearDown(entity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_TextAttribute()
        {
            cCustomEntity entity = null;

            try
            {
                entity = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity.attributes.Count);

                // Add a new attribute
                cTextAttribute expected = cTextAttributeObject.New(entity.entityid, cTextAttributeObject.BasicSingleLine());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity2 = clsCustomEntities.getEntityById(entity.entityid);
                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);
                clsCustomEntities = new cCustomEntities(currentUser);

                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity.entityid);
                cTextAttribute actual = (cTextAttribute)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
            }
            finally
            {
                if (entity != null)
                {
                    cCustomEntityObject.TearDown(entity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_DateTimeAttribute()
        {
            cCustomEntity entity = null;

            try
            {
                entity = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity.attributes.Count);

                // Add a new attribute
                cDateTimeAttribute expected = cDateTimeAttributeObject.New(entity.entityid, cDateTimeAttributeObject.BasicDateOnly());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity2 = clsCustomEntities.getEntityById(entity.entityid);
                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);
                clsCustomEntities = new cCustomEntities(currentUser);
                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity.entityid);
                cDateTimeAttribute actual = (cDateTimeAttribute)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
            }
            finally
            {
                if (entity != null)
                {
                    cCustomEntityObject.TearDown(entity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_NumberAttribute()
        {
            cCustomEntity entity = null;

            try
            {
                entity = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity.attributes.Count);

                // Add a new attribute
                cNumberAttribute expected = cNumberAttributeObject.New(entity.entityid, cNumberAttributeObject.BasicInteger());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity2 = clsCustomEntities.getEntityById(entity.entityid);
                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);
                clsCustomEntities = new cCustomEntities(currentUser);

                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity.entityid);
                cNumberAttribute actual = (cNumberAttribute)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
            }
            finally
            {
                if (entity != null)
                {
                    cCustomEntityObject.TearDown(entity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_ListAttribute()
        {
            cCustomEntity entity = null;

            try
            {
                entity = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity.attributes.Count);

                // Add a new attribute
                cListAttribute expected = cListAttributeObject.New(entity.entityid, cListAttributeObject.BasicList());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity2 = clsCustomEntities.getEntityById(entity.entityid);
                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);
                clsCustomEntities = new cCustomEntities(currentUser);
                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity.entityid);
                cListAttribute actual = (cListAttribute)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
            }
            finally
            {
                if (entity != null)
                {
                    cCustomEntityObject.TearDown(entity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_TickBoxAttribute()
        {
            cCustomEntity entity = null;

            try
            {
                entity = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity.attributes.Count);

                // Add a new attribute
                cTickboxAttribute expected = cTickboxAttributeObject.New(entity.entityid, cTickboxAttributeObject.Template());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity2 = clsCustomEntities.getEntityById(entity.entityid);

                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);
                clsCustomEntities = new cCustomEntities(currentUser);

                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity.entityid);
                cTickboxAttribute actual = (cTickboxAttribute)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
            }
            finally
            {
                if (entity != null)
                {
                    cCustomEntityObject.TearDown(entity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void cCustomEntities_deleteAttribute_RelationshipTextBoxAttribute()
        {
            cCustomEntity entity = null;

            try
            {
                entity = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity.attributes.Count);

                // Add a new attribute
                cRelationshipTextBoxAttribute expected = cRelationshipTextBoxAttributeObject.New(entity.entityid, cRelationshipTextBoxAttributeObject.Template());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity2 = clsCustomEntities.getEntityById(entity.entityid);
                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);

                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity.entityid);
                cRelationshipTextBoxAttribute actual = (cRelationshipTextBoxAttribute)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
            }
            finally
            {
                if (entity != null)
                {
                    cCustomEntityObject.TearDown(entity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_AttachmentAttribute()
        {
            cCustomEntity entity = null;

            try
            {
                entity = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity.attributes.Count);

                // Add a new attribute
                cAttachmentAttribute expected = cAttachmentAttributeObject.New(entity.entityid, cAttachmentAttributeObject.Template());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity2 = clsCustomEntities.getEntityById(entity.entityid);

                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);
                clsCustomEntities = new cCustomEntities(currentUser);

                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity.entityid);
                cAttachmentAttribute actual = (cAttachmentAttribute)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
            }
            finally
            {
                if (entity != null)
                {
                    cCustomEntityObject.TearDown(entity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_ManyToOneAttribute()
        {
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;

            try
            {
                entity1 = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity1.attributes.Count);

                // Add a new attribute
                cManyToOneRelationship expected = cManyToOneRelationshipObject.New(entity1.entityid, cManyToOneRelationshipObject.BasicMTO());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);
                clsCustomEntities = new cCustomEntities(currentUser);

                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                cManyToOneRelationship actual = (cManyToOneRelationship)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
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

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_ManyToOneAttributeUsedInJoinViaOnViewFields()
        {
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;

            try
            {
                #region setup
                entity1 = cCustomEntityObject.New(cCustomEntityObject.Template(entityName: "MtO in JoinVia Field Test GreenLight"));

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity1.attributes.Count);

                // Add a new attribute
                cManyToOneRelationship expected = cManyToOneRelationshipObject.New(entity1.entityid, cManyToOneRelationshipObject.BasicMTO());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count, "Initial GreenLight attribute count with MtO added");
                #endregion setup

                #region create fields
                cFields clsFields = new cFields(currentUser.AccountID);
                // get a field to pick from the employees table linked to by the many to one we will delete
                cField userName = clsFields.GetFieldByID(new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795"));
                cField firstName = clsFields.GetFieldByID(new Guid("6614ACAD-0A43-4E30-90EC-84DE0792B1D6"));
                JoinVia joinVia = new JoinVia(0, "MtO Att Test", Guid.NewGuid(), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(expected.fieldid, JoinViaPart.IDType.Field) } });

                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid, cCustomEntityViewObject.Template(allowAdd: true, addForm: form, fields: new SortedList<byte, cCustomEntityViewField> { { 0, new cCustomEntityViewField(userName, joinVia) }, { 1, new cCustomEntityViewField(firstName, joinVia) } }));

                Assert.IsTrue(view.viewid > 0);
                clsCustomEntities = new cCustomEntities(currentUser);
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                cCustomEntityView view2 = entity2.getViewById(view.viewid);

                // ensure the fields are in the view picked via the manytoone
                Assert.AreEqual(userName.FieldID, view2.fields.Where(x => x.Value.Field.FieldID == userName.FieldID).Select(x => x.Value.Field.FieldID).FirstOrDefault());
                Assert.AreEqual(firstName.FieldID, view2.fields.Where(x => x.Value.Field.FieldID == firstName.FieldID).Select(x => x.Value.Field.FieldID).FirstOrDefault());
                // ensure the joinvia has an id
                Assert.IsTrue(view2.fields.Where(x => x.Value.Field.FieldID == userName.FieldID).Select(x => x.Value.JoinVia.JoinViaID).FirstOrDefault() > 0);
                #endregion create fields/filters

                // Remove the added attribute
                int attDeleteReturnCode = clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);
                Assert.AreEqual(0, attDeleteReturnCode);

                #region check the removed fields/joinvia
                clsCustomEntities = new cCustomEntities(currentUser);
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                view2 = entity2.getViewById(view.viewid);
                // ensure the fields picked via the deleted many to one is gone
                Assert.AreEqual(0, view2.fields.Count);

                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                cManyToOneRelationship actual = (cManyToOneRelationship)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
                #endregion check the removed fields/joinvia
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

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_ManyToOneAttributeUsedInJoinViaOnViewFilters()
        {
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;

            try
            {
                #region setup
                entity1 = cCustomEntityObject.New(cCustomEntityObject.Template(entityName: "MtO in JoinVia Filter Test GreenLight"));

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity1.attributes.Count);

                // Add a new attribute
                cManyToOneRelationship expected = cManyToOneRelationshipObject.New(entity1.entityid, cManyToOneRelationshipObject.BasicMTO());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count, "Initial GreenLight attribute count with MtO added");
                #endregion setup

                #region create fields/filters
                cFields clsFields = new cFields(currentUser.AccountID);
                // get a field to pick from the employees table linked to by the many to one we will delete
                cField userName = clsFields.GetFieldByID(new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795"));
                cField firstName = clsFields.GetFieldByID(new Guid("6614ACAD-0A43-4E30-90EC-84DE0792B1D6"));
                JoinVia joinVia = new JoinVia(0, "MtO Att Test", Guid.NewGuid(), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(expected.fieldid, JoinViaPart.IDType.Field) } });

                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid, cCustomEntityViewObject.Template(allowAdd: true, addForm: form, fields: new SortedList<byte, cCustomEntityViewField> { { 0, new cCustomEntityViewField(userName, joinVia) }, { 1, new cCustomEntityViewField(firstName, joinVia) } }, filters: new SortedList<byte, FieldFilter> { { 0, new FieldFilter(userName, ConditionType.Equals, "ut mto", "", 0, joinVia) } }));

                Assert.IsTrue(view.viewid > 0);
                clsCustomEntities = new cCustomEntities(currentUser);
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                cCustomEntityView view2 = entity2.getViewById(view.viewid);

                // ensure the fields are in the view picked via the manytoone
                Assert.AreEqual(userName.FieldID, view2.fields.Where(x => x.Value.Field.FieldID == userName.FieldID).Select(x => x.Value.Field.FieldID).FirstOrDefault(), "Initial field setup not present");
                Assert.AreEqual(firstName.FieldID, view2.fields.Where(x => x.Value.Field.FieldID == firstName.FieldID).Select(x => x.Value.Field.FieldID).FirstOrDefault(), "Initial field setup not present");
                // ensure the filter is in the view picked via the manytoone
                Assert.AreEqual(userName.FieldID, view2.filters.Where(x => x.Value.Field.FieldID == userName.FieldID).Select(x => x.Value.Field.FieldID).FirstOrDefault(), "Initial filter setup not present");
                // ensure the joinvia has an id
                Assert.IsTrue(view2.fields.Where(x => x.Value.Field.FieldID == userName.FieldID).Select(x => x.Value.JoinVia.JoinViaID).FirstOrDefault() > 0, "The initial joinVia should have a saved id greater than 0");
                #endregion create fields/filters

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);

                #region check the removed fields/filters/joinvia
                clsCustomEntities = new cCustomEntities(currentUser);
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                view2 = entity2.getViewById(view.viewid);

                // ensure the fields picked via the deleted many to one are gone
                Assert.AreEqual(0, view2.fields.Count, "Deleted fields are still present");
                // ensure the filter picked via the deleted many to one is gone
                Assert.AreEqual(0, view2.filters.Count, "Deleted filters are still present");

                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                cManyToOneRelationship actual = (cManyToOneRelationship)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count, "The ending attribute count should be the default 5"); // The entity should now have 5 attributes
                Assert.IsNull(actual, "The MtO should have been deleted"); // the attribute should now be gone
                #endregion check the removed fields/filters/joinvia
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

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_ManyToOneAttributeUsedInJoinViaOnViewSortedColumn()
        {
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;

            try
            {
                #region setup
                entity1 = cCustomEntityObject.New(cCustomEntityObject.Template(entityName: "MtO in JoinVia and View with SortColumn Test GreenLight"));

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity1.attributes.Count);

                // Add a new attribute
                cManyToOneRelationship expected = cManyToOneRelationshipObject.New(entity1.entityid, cManyToOneRelationshipObject.BasicMTO());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count, "Initial GreenLight attribute count with MtO added");
                #endregion setup

                #region create fields/filters/sort
                cFields clsFields = new cFields(currentUser.AccountID);
                // get a field to pick from the employees table linked to by the many to one we will delete
                cField userName = clsFields.GetFieldByID(new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795"));
                cField firstName = clsFields.GetFieldByID(new Guid("6614ACAD-0A43-4E30-90EC-84DE0792B1D6"));
                JoinVia joinVia = new JoinVia(0, "MtO Att Test", Guid.NewGuid(), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(expected.fieldid, JoinViaPart.IDType.Field) } });

                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid, cCustomEntityViewObject.Template(allowAdd: true, addForm: form, fields: new SortedList<byte, cCustomEntityViewField> { { 0, new cCustomEntityViewField(userName, joinVia) }, { 1, new cCustomEntityViewField(firstName, joinVia) } }, sortColumn: new GreenLightSortColumn(userName.FieldID, SpendManagementLibrary.SortDirection.Ascending, joinVia)));

                Assert.IsTrue(view.viewid > 0);
                clsCustomEntities = new cCustomEntities(currentUser);
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                cCustomEntityView view2 = entity2.getViewById(view.viewid);

                // ensure the fields are in the view picked via the manytoone
                Assert.AreEqual(userName.FieldID, view2.fields.Where(x => x.Value.Field.FieldID == userName.FieldID).Select(x => x.Value.Field.FieldID).FirstOrDefault());
                Assert.AreEqual(firstName.FieldID, view2.fields.Where(x => x.Value.Field.FieldID == firstName.FieldID).Select(x => x.Value.Field.FieldID).FirstOrDefault());
                // ensure the joinvia has an id
                Assert.IsTrue(view2.fields.Where(x => x.Value.Field.FieldID == userName.FieldID).Select(x => x.Value.JoinVia.JoinViaID).FirstOrDefault() > 0);

                // ensure the sort column has been set
                Assert.AreEqual(userName.FieldID, view2.SortColumn.FieldID);
                Assert.AreEqual(SpendManagementLibrary.SortDirection.Ascending, view2.SortColumn.SortDirection);
                Assert.IsNotNull(view2.SortColumn.JoinVia);
                Assert.IsTrue(view2.SortColumn.JoinVia.JoinViaID > 0);
                int joinViaID = view2.SortColumn.JoinVia.JoinViaID;
                #endregion create fields/filters/sort

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);

                #region check the removed fields/filters/joinvia/sort
                clsCustomEntities = new cCustomEntities(currentUser);
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                view2 = entity2.getViewById(view.viewid);
                // ensure the fields picked via the deleted many to one is gone
                Assert.AreEqual(0, view2.fields.Count);

                // ensure the sortcolumn
                Assert.AreEqual(Guid.Empty, view2.SortColumn.FieldID);
                Assert.IsNull(view2.SortColumn.JoinVia);

                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                cManyToOneRelationship actual = (cManyToOneRelationship)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone

                #endregion check the removed fields/filters/joinvia/sort
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
        /// Test that deleting a many to one also deletes an associated Lookup Field
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_ManyToOneAttributeWithLookupField()
        {
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;
            LookupDisplayField lookupDisplay = null;

            try
            {
                entity1 = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity1.attributes.Count);

                // Add a new attribute
                cManyToOneRelationship expected = cManyToOneRelationshipObject.New(entity1.entityid, cManyToOneRelationshipObject.BasicMTO());

                // Add a Lookup Field
                expected.TriggerLookupFields.Add(LookupDisplayFieldObject.New(entity1.entityid, LookupDisplayFieldObject.Template(expected.attributeid, fieldid: expected.fieldid)));

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);

                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);
                clsCustomEntities = new cCustomEntities(currentUser);

                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity1.entityid);
                cManyToOneRelationship actual = (cManyToOneRelationship)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
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

                if (lookupDisplay != null)
                {
                    cCustomEntityObject.TearDown(lookupDisplay.attributeid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_OneToManyAttribute()
        {
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;

            try
            {
                entity1 = cCustomEntityObject.New();
                entity2 = cCustomEntityObject.New();
                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid);

                // An entities should have 5 default attributes when first created
                Assert.AreEqual(5, entity1.attributes.Count);
                Assert.AreEqual(5, entity2.attributes.Count);

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                entity2 = clsCustomEntities.getEntityById(entity2.entityid);

                // Add a new attribute
                cOneToManyRelationship expected = cOneToManyRelationshipObject.New(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, entity2));

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity3 = clsCustomEntities.getEntityById(entity1.entityid);
                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity3.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);
                clsCustomEntities = new cCustomEntities(currentUser);

                // Refresh the entity and get the attribute that should no longer be there
                entity3 = clsCustomEntities.getEntityById(entity1.entityid);
                cOneToManyRelationship actual = (cOneToManyRelationship)entity3.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity3.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
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

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void cCustomEntities_deleteAttribute_RunWorkflowAttribute()
        {
            // this will error currently as runworkflow does not get used and will rely upon workflows test objects etc
            // needs to be revisted during workflow development
            cCustomEntity entity = null;

            try
            {
                entity = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity.attributes.Count);

                // Add a new attribute
                cRunWorkflowAttribute expected = cRunWorkflowAttributeObject.New(entity.entityid, cRunWorkflowAttributeObject.BasicRunWorkflow());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity2 = clsCustomEntities.getEntityById(entity.entityid);
                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);

                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity.entityid);
                cRunWorkflowAttribute actual = (cRunWorkflowAttribute)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
            }
            finally
            {
                if (entity != null)
                {
                    cCustomEntityObject.TearDown(entity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_CurrencyListAttribute()
        {
            cCustomEntity entity = null;

            try
            {
                entity = cCustomEntityObject.New(entity: cCustomEntityObject.BasicCurrencyType());

                // A financial entity should have 6 default attributes when first created
                Assert.AreEqual(6, entity.attributes.Count);

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);

                cCurrencyListAttribute attribute = (cCurrencyListAttribute)(from x in entity.attributes.Values
                                                                            where x.attributename == "GreenLightCurrency" && x.GetType() == typeof(cCurrencyListAttribute)
                                                                            select x).FirstOrDefault();
                Assert.IsNotNull(attribute);

                // Remove the default primary key attribute
                clsCustomEntities.deleteAttribute(attribute.attributeid, currentUser.EmployeeID, 0);

                // Ensure the id attribute is still there
                clsCustomEntities = new cCustomEntities(currentUser);
                attribute = (cCurrencyListAttribute)(from x in entity.attributes.Values
                                                     where x.attributename == "GreenLightCurrency" && x.GetType() == typeof(cCurrencyListAttribute)
                                                     select x).FirstOrDefault();
                Assert.IsNotNull(attribute);
            }
            finally
            {
                if (entity != null)
                {
                    cCustomEntityObject.TearDown(entity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_SummaryAttribute()
        {
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;

            try
            {
                entity1 = cCustomEntityObject.New();
                entity2 = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity1.attributes.Count);

                // Add a new attribute
                cSummaryAttribute expected = cSummaryAttributeObject.New(entity1.entityid, cSummaryAttributeObject.Template(summaryElements: new Dictionary<int, cSummaryAttributeElement>(), summaryColumns: new Dictionary<int, cSummaryAttributeColumn>(), sourceEntityID: entity2.entityid));

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity3 = clsCustomEntities.getEntityById(entity1.entityid);
                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity3.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);
                clsCustomEntities = new cCustomEntities(currentUser);

                // Refresh the entity and get the attribute that should no longer be there
                entity3 = clsCustomEntities.getEntityById(entity1.entityid);
                cSummaryAttribute actual = (cSummaryAttribute)entity3.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity3.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
            }
            finally
            {
                if (entity2 != null)
                {
                    cCustomEntityObject.TearDown(entity2.entityid);
                }
                if (entity1 != null)
                {
                    cCustomEntityObject.TearDown(entity1.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_deleteAttribute_AdviceAttribute()
        {
            cCustomEntity entity = null;

            try
            {
                entity = cCustomEntityObject.New();

                // An entity should have 5 default attributes when first created
                Assert.AreEqual(5, entity.attributes.Count);

                // Add a new attribute
                cCommentAttribute expected = cCommentAttributeObject.New(entity.entityid, cCommentAttributeObject.Template());

                ICurrentUser currentUser = Moqs.CurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity2 = clsCustomEntities.getEntityById(entity.entityid);

                // The entity should now have 6 attributes
                Assert.AreEqual(6, entity2.attributes.Count);

                // Remove the added attribute
                clsCustomEntities.deleteAttribute(expected.attributeid, currentUser.EmployeeID, 0);
                clsCustomEntities = new cCustomEntities(currentUser);

                // Refresh the entity and get the attribute that should no longer be there
                entity2 = clsCustomEntities.getEntityById(entity.entityid);
                cCommentAttribute actual = (cCommentAttribute)entity2.getAttributeById(expected.attributeid);

                Assert.AreEqual(5, entity2.attributes.Count); // The entity should now have 5 attributes
                Assert.IsNull(actual); // the attribute should now be gone
            }
            finally
            {
                if (entity != null)
                {
                    cCustomEntityObject.TearDown(entity.entityid);
                }
            }
        }
        #endregion deleteAttribute

        #region SaveForm

        #region Basic CustomEntityForm saving

        /// <summary>
        /// A test for saveForm, using form with no tabs, sections or fields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_UsingBasicForm()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sFormName = "cCustomEntities_saveForm_UsingValidForm form name";
            string sFormDescription = "cCustomEntities_saveForm_UsingValidForm form description";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities customEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a template form, using the custom entity ID from above
                cCustomEntityForm expected = cCustomEntityFormObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription);

                int nFormID = customEntities.saveForm(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                expected = cCustomEntityFormObject.Template(formID: nFormID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityForm actual = reqCustomEntity.getFormById(nFormID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        /// A test to ensure that saveForm works when creating a copy, using a form with no tabs, sections or fields
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_CreateCopyUsingBasicForm()
        {
            int employeeID = Moqs.CurrentUser().EmployeeID;
            const string FormName = "cCustomEntities_saveForm_CreateCopyUsingBasicForm form name";
            const string FormDescription = "cCustomEntities_saveForm_CreateCopyUsingBasicForm form description";
            cCustomEntity reqCustomEntity = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: employeeID));
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create the initial Form we wish to make a copy of
                cCustomEntityForm initialForm = cCustomEntityFormObject.Template(createdBy: employeeID, entityID: reqCustomEntity.entityid, formName: FormName, description: FormDescription);

                int initialFormID = customEntities.saveForm(reqCustomEntity.entityid, initialForm);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                Assert.IsTrue(initialFormID > 0);

                initialForm = cCustomEntityFormObject.Template(initialFormID, createdBy: employeeID, entityID: reqCustomEntity.entityid, formName: FormName, description: FormDescription);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                // Create a copy of the initial Form, using a new form name
                int copiedFormID = customEntities.saveForm(reqCustomEntity.entityid, initialForm, "Unit Test Form Copy");
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                Assert.IsTrue(copiedFormID > 0 && copiedFormID != initialFormID);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityForm copiedForm = reqCustomEntity.getFormById(copiedFormID);

                Assert.AreEqual("Unit Test Form Copy", copiedForm.formname);

                cCompareAssert.AreEqual(initialForm, copiedForm, new List<string> { "formid", "formname", "createdon" });

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(copiedForm.createdon.AddTicks(-copiedForm.createdon.Ticks) >= initialForm.createdon.AddTicks(-initialForm.createdon.Ticks));
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
        ///A test for saveForm where no form description has been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_WithoutDescription()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sFormName = "cCustomEntities_saveForm_WithoutDescription form name";
            string sFormDescription = "";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities customEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a template form, using the custom entity ID from above
                cCustomEntityForm expected = cCustomEntityFormObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription);

                int nFormID = customEntities.saveForm(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                expected = cCustomEntityFormObject.Template(formID: nFormID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityForm actual = reqCustomEntity.getFormById(nFormID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveForm where no save button text has been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_WithoutSaveButtonText()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sFormName = "cCustomEntities_saveForm_WithoutSaveButtonText form name";
            string sFormDescription = "cCustomEntities_saveForm_WithoutSaveButtonText form description";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities customEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a template form, using the custom entity ID from above
                cCustomEntityForm expected = cCustomEntityFormObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription, saveText: "");

                int nFormID = customEntities.saveForm(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                expected = cCustomEntityFormObject.Template(formID: nFormID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription, saveText: "");

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityForm actual = reqCustomEntity.getFormById(nFormID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveForm where no save and new button text has been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_WithoutSaveAndNewButtonText()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sFormName = "cCustomEntities_saveForm_WithoutSaveAndNewButtonText form name";
            string sFormDescription = "cCustomEntities_saveForm_WithoutSaveAndNewButtonText form description";
            cCustomEntity reqCustomEntity = null;
            cCustomEntities customEntities = null;

            try
            {
                reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a template form, using the custom entity ID from above
                cCustomEntityForm expected = cCustomEntityFormObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription, saveAndDuplicateText: "");

                int nFormID = customEntities.saveForm(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                expected = cCustomEntityFormObject.Template(formID: nFormID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription, saveAndDuplicateText: "");

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityForm actual = reqCustomEntity.getFormById(nFormID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveForm where no save and stay button text has been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_WithoutSaveAndStayButtonText()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sFormName = "cCustomEntities_saveForm_WithoutSaveAndStayButtonText form name";
            string sFormDescription = "cCustomEntities_saveForm_WithoutSaveAndStayButtonText form description";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a template form, using the custom entity ID from above
                cCustomEntityForm expected = cCustomEntityFormObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription, saveAndStayText: "");

                int nFormID = customEntities.saveForm(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                expected = cCustomEntityFormObject.Template(formID: nFormID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription, saveAndStayText: "");

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityForm actual = reqCustomEntity.getFormById(nFormID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveForm where no cancel button text has been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_WithoutCancelButtonText()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sFormName = "cCustomEntities_saveForm_WithoutCancelButtonText form name";
            string sFormDescription = "cCustomEntities_saveForm_WithoutCancelButtonText form description";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a template form, using the custom entity ID from above
                cCustomEntityForm expected = cCustomEntityFormObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription, cancelText: "");

                int nFormID = customEntities.saveForm(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                expected = cCustomEntityFormObject.Template(formID: nFormID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription, cancelText: "");

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityForm actual = reqCustomEntity.getFormById(nFormID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveForm where modified by has been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_WithModifiedBySet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sFormName = "cCustomEntities_saveForm_WithModifiedBySet form name";
            string sFormDescription = "cCustomEntities_saveForm_WithModifiedBySet form description";
            DateTime dtModifiedOn = DateTime.UtcNow;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a template form, using the custom entity ID from above
                cCustomEntityForm expected = cCustomEntityFormObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription, modifiedBy: nEmployeeID, modifiedOn: dtModifiedOn);

                int nFormID = customEntities.saveForm(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                expected = cCustomEntityFormObject.Template(formID: nFormID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription, modifiedBy: nEmployeeID, modifiedOn: dtModifiedOn);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityForm actual = reqCustomEntity.getFormById(nFormID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveForm using a delegate
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_UsingDelegate()
        {
            Mock<ICurrentUser> currentUser = Moqs.CurrentUserDelegateMock();

            string sFormName = "cCustomEntities_saveForm_UsingDelegate form name";
            string sFormDescription = "cCustomEntities_saveForm_UsingDelegate form description";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: GlobalTestVariables.DelegateId));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(currentUser.Object);

                // Create a template form, using the custom entity ID from above
                cCustomEntityForm expected = cCustomEntityFormObject.Template(createdBy: GlobalTestVariables.DelegateId, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription);

                int nFormID = customEntities.saveForm(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                expected = cCustomEntityFormObject.Template(formID: nFormID, createdBy: GlobalTestVariables.DelegateId, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityForm actual = reqCustomEntity.getFormById(nFormID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string> { "createdon" };

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveForm where the formName and formID already exist in the database
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_WhereFormNameAndFormIDAlreadyExist()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sFormName = "cCustomEntities_saveForm_WhereFormNameAndFormIDAlreadyExist form name";
            string sFormDescription = "cCustomEntities_saveForm_WhereFormNameAndFormIDAlreadyExist form description";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a new form in the database to ensure that the desired name and ID have already been used
                cCustomEntityForm reqEntityForm = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription));

                // Create a template form, using the custom entity ID from above
                cCustomEntityForm reqSecondEntiyForm = cCustomEntityFormObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, formName: sFormName, description: sFormDescription);

                int nFormID = customEntities.saveForm(reqCustomEntity.entityid, reqSecondEntiyForm);

                Assert.AreEqual(-1, nFormID);
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

        #region Form tab, form section and form field saving

        /// <summary>
        ///  A test for saveForm where form tabs exist on the form
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_WithFormTabs()
        {
            int employeeId = Moqs.CurrentUser().EmployeeID;
            const string FormName = "cCustomEntities_saveForm_WithFormTabs form name";
            const string FormDescription = "cCustomEntities_saveForm_WithFormTabs form description";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: employeeId));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                SortedList<int, cCustomEntityFormTab> tabList = new SortedList<int, cCustomEntityFormTab>();
                cCustomEntityFormTab reqTab = cCustomEntityFormTabObject.Template();
                tabList.Add(0, reqTab);

                // Create a template form, using the custom entity ID from above
                cCustomEntityForm expected = cCustomEntityFormObject.Template(createdBy: employeeId, entityID: reqCustomEntity.entityid, formName: FormName, description: FormDescription, tabs: tabList);

                int formId = customEntities.saveForm(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                #region recreate the saved versions of expected

                SortedList<int, cCustomEntityFormTab> savedTabs = new SortedList<int, cCustomEntityFormTab>
                    {
                        {
                            expected.tabs[0].tabid,
                            cCustomEntityFormTabObject.Template(
                                expected.tabs[0].tabid, formId, expected.tabs[0].headercaption, 0)
                            }
                    };

                expected = cCustomEntityFormObject.Template(
                    formID: formId,
                    createdBy: employeeId,
                    entityID: reqCustomEntity.entityid,
                    formName: expected.formname,
                    description: expected.description,
                    tabs: savedTabs);

                #endregion recreate the saved versions of expected

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityForm actual = reqCustomEntity.getFormById(formId);

                // Do not compare the values of CreatedOn and tabid as they may be different
                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: new List<string> { "createdon" });

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///  A test for saveForm where form tabs and sections exist on the form
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_WithFormTabsAndSections()
        {
            int employeeId = Moqs.CurrentUser().EmployeeID;
            const string FormName = "cCustomEntities_saveForm_WithFormTabsAndSections form name";
            const string FormDescription = "cCustomEntities_saveForm_WithFormTabsAndSections form description";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: employeeId));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                #region Create required form tabs and sections

                SortedList<int, cCustomEntityFormTab> tabList = new SortedList<int, cCustomEntityFormTab>();
                cCustomEntityFormTab reqTab = cCustomEntityFormTabObject.Template();
                tabList.Add(0, reqTab);

                SortedList<int, cCustomEntityFormSection> sectionList = new SortedList<int, cCustomEntityFormSection>();
                cCustomEntityFormSection reqSection = cCustomEntityFormSectionObject.Template(tab: reqTab);
                sectionList.Add(0, reqSection);

                #endregion

                // Create a template form, using the custom entity ID from above
                cCustomEntityForm expected = cCustomEntityFormObject.Template(createdBy: employeeId, entityID: reqCustomEntity.entityid, formName: FormName, description: FormDescription, tabs: tabList, sections: sectionList);

                int formId = customEntities.saveForm(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                #region recreate the saved versions of expected

                SortedList<int, cCustomEntityFormTab> savedTabs = new SortedList<int, cCustomEntityFormTab>
                    {
                        {
                            expected.tabs[0].tabid,
                            cCustomEntityFormTabObject.Template(
                                expected.tabs[0].tabid, formId, expected.tabs[0].headercaption, 0)
                            }
                    };

                SortedList<int, cCustomEntityFormSection> savedSections = new SortedList<int, cCustomEntityFormSection>
                    {
                        {
                            expected.sections[0].sectionid,
                            cCustomEntityFormSectionObject.Template(
                                expected.sections[0].sectionid,
                                formId,
                                expected.sections[0].headercaption,
                                0,
                                savedTabs[expected.tabs[0].tabid])
                            }
                    };

                savedTabs.Values[0].sections.Add(savedSections.Values[0]);

                expected = cCustomEntityFormObject.Template(
                    formID: formId,
                    createdBy: employeeId,
                    entityID: reqCustomEntity.entityid,
                    formName: expected.formname,
                    description: expected.description,
                    tabs: savedTabs,
                    sections: savedSections);

                #endregion recreate the saved versions of expected

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityForm actual = reqCustomEntity.getFormById(formId);

                // Do not compare the values of CreatedOn, as they may be different
                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: new List<string> { "createdon" });

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        /// A test for saveForm where form tabs, sections and fields exist on the form
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_WithFormTabsAndSectionsAndFields()
        {
            int employeeId = Moqs.CurrentUser().EmployeeID;
            const string FormName = "cCustomEntities_saveForm_WithFormTabsAndSectionsAndFields form name";
            const string FormDescription = "cCustomEntities_saveForm_WithFormTabsAndSectionsAndFields form description";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: employeeId));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                #region Create required form tab, section, field and form attribute

                SortedList<int, cCustomEntityFormTab> tabList = new SortedList<int, cCustomEntityFormTab>();
                cCustomEntityFormTab reqTab = cCustomEntityFormTabObject.Template();
                tabList.Add(0, reqTab);

                SortedList<int, cCustomEntityFormSection> sectionList = new SortedList<int, cCustomEntityFormSection>();
                cCustomEntityFormSection reqSection = cCustomEntityFormSectionObject.Template(tab: reqTab);
                sectionList.Add(0, reqSection);

                cAttribute reqNumberAttribute = cNumberAttributeObject.New(reqCustomEntity.entityid, cNumberAttributeObject.Template(createdBy: employeeId));

                SortedList<int, cCustomEntityFormField> fieldList = new SortedList<int, cCustomEntityFormField>();
                cCustomEntityFormField reqField = cCustomEntityFormFieldObject.Template(section: reqSection, attribute: reqNumberAttribute);
                fieldList.Add(0, reqField);

                #endregion

                // Create a template form, using the custom entity ID from above
                cCustomEntityForm expected = cCustomEntityFormObject.Template(createdBy: employeeId, entityID: reqCustomEntity.entityid, formName: FormName, description: FormDescription, tabs: tabList, sections: sectionList, fields: fieldList);

                int formId = customEntities.saveForm(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                #region recreate the saved versions of expected

                SortedList<int, cCustomEntityFormTab> savedTabs = new SortedList<int, cCustomEntityFormTab>
                    {
                        {
                            expected.tabs[0].tabid,
                            cCustomEntityFormTabObject.Template(
                                expected.tabs[0].tabid, formId, expected.tabs[0].headercaption, 0)
                            }
                    };

                SortedList<int, cCustomEntityFormSection> savedSections = new SortedList<int, cCustomEntityFormSection>
                    {
                        {
                            expected.sections[0].sectionid,
                            cCustomEntityFormSectionObject.Template(
                                expected.sections[0].sectionid,
                                formId,
                                expected.sections[0].headercaption,
                                0,
                                savedTabs[expected.tabs[0].tabid])
                            }
                    };

                SortedList<int, cCustomEntityFormField> savedFormFields = new SortedList<int, cCustomEntityFormField>
                    {
                        {
                            expected.fields[0].attribute.attributeid,
                            cCustomEntityFormFieldObject.Template(
                                formId,
                                expected.fields[0].attribute,
                                expected.fields[0].isReadOnly,
                                savedSections[expected.sections[0].sectionid],
                                expected.fields[0].column,
                                expected.fields[0].row,
                                expected.fields[0].labelText)
                            }
                    };

                savedSections.Values[0].fields.Add(savedFormFields.Values[0]);
                savedTabs.Values[0].sections.Add(savedSections.Values[0]);

                expected = cCustomEntityFormObject.Template(
                    formID: formId,
                    createdBy: employeeId,
                    entityID: reqCustomEntity.entityid,
                    formName: expected.formname,
                    description: expected.description,
                    tabs: savedTabs,
                    sections: savedSections,
                    fields: savedFormFields);

                #endregion recreate the saved versions of expected

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityForm actual = reqCustomEntity.getFormById(formId);

                // Do not compare the values of CreatedOn, as they may be different
                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: new List<string> { "createdon" });

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        /// A test to ensure that saveForm works when creating a copy, using a form with tabs, sections and fields
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveForm_CreateCopyUsingFormWithTabsAndSectionsAndFields()
        {
            var employeeId = Moqs.CurrentUser().EmployeeID;
            const string FormName = "cCustomEntities_saveForm_CreateCopyUsingFormWithTabsAndSectionsAndFields form name";
            const string FormDescription = "cCustomEntities_saveForm_CreateCopyUsingFormWithTabsAndSectionsAndFields form description";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: employeeId));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                #region Create required form tab, section, field and form attribute

                SortedList<int, cCustomEntityFormTab> tabList = new SortedList<int, cCustomEntityFormTab>();
                cCustomEntityFormTab reqTab = cCustomEntityFormTabObject.Template();
                tabList.Add(0, reqTab);

                SortedList<int, cCustomEntityFormSection> sectionList = new SortedList<int, cCustomEntityFormSection>();
                cCustomEntityFormSection reqSection = cCustomEntityFormSectionObject.Template(tab: reqTab);
                sectionList.Add(0, reqSection);

                cAttribute reqNumberAttribute = cNumberAttributeObject.New(reqCustomEntity.entityid, cNumberAttributeObject.Template(createdBy: employeeId));

                SortedList<int, cCustomEntityFormField> fieldList = new SortedList<int, cCustomEntityFormField>();
                cCustomEntityFormField reqField = cCustomEntityFormFieldObject.Template(section: reqSection, attribute: reqNumberAttribute);
                fieldList.Add(0, reqField);

                #endregion

                // Create a template form, using the custom entity ID from above
                cCustomEntityForm expected = cCustomEntityFormObject.Template(createdBy: employeeId, entityID: reqCustomEntity.entityid, formName: FormName, description: FormDescription, tabs: tabList, sections: sectionList, fields: fieldList);

                var formId = customEntities.saveForm(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a copy of the Form
                var copiedFormId = customEntities.saveForm(reqCustomEntity.entityid, expected, "Unit Test Form Copy");
                customEntities = new cCustomEntities(Moqs.CurrentUser());

                Assert.IsTrue(copiedFormId > 0 && copiedFormId != formId);

                #region recreate the saved versions of expected

                SortedList<int, cCustomEntityFormTab> savedTabs = new SortedList<int, cCustomEntityFormTab>
                    {
                        {
                            expected.tabs[0].tabid,
                            cCustomEntityFormTabObject.Template(
                                expected.tabs[0].tabid, copiedFormId, expected.tabs[0].headercaption, 0)
                            }
                    };

                SortedList<int, cCustomEntityFormSection> savedSections = new SortedList<int, cCustomEntityFormSection>
                    {
                        {
                            expected.sections[0].sectionid,
                            cCustomEntityFormSectionObject.Template(
                                expected.sections[0].sectionid,
                                copiedFormId,
                                expected.sections[0].headercaption,
                                0,
                                savedTabs[expected.tabs[0].tabid])
                            }
                    };

                SortedList<int, cCustomEntityFormField> savedFormFields = new SortedList<int, cCustomEntityFormField>
                    {
                        {
                            expected.fields[0].attribute.attributeid,
                            cCustomEntityFormFieldObject.Template(
                                copiedFormId,
                                expected.fields[0].attribute,
                                expected.fields[0].isReadOnly,
                                savedSections[expected.sections[0].sectionid],
                                expected.fields[0].column,
                                expected.fields[0].row,
                                expected.fields[0].labelText)
                            }
                    };

                savedSections.Values[0].fields.Add(savedFormFields.Values[0]);
                savedTabs.Values[0].sections.Add(savedSections.Values[0]);

                expected = cCustomEntityFormObject.Template(
                    formID: copiedFormId,
                    createdBy: employeeId,
                    entityID: reqCustomEntity.entityid,
                    formName: "Unit Test Form Copy",
                    description: expected.description,
                    tabs: savedTabs,
                    sections: savedSections,
                    fields: savedFormFields);

                #endregion recreate the saved versions of expected

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityForm copiedForm = reqCustomEntity.getFormById(copiedFormId);

                // Do not compare the values of CreatedOn, as they may be different
                cCompareAssert.AreEqual(expected, copiedForm, lstOmittedProperties: new List<string> { "createdon" });

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(copiedForm.createdon.AddTicks(-copiedForm.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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

        #endregion SaveForm

        #region SaveRelationship

        /// <summary>
        ///A test for saveRelationship, using cOneToManyRelationshipAttribute
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveRelationship_UsingOneToManyRelationshipAttribute()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveRelationship_UsingOneToManyRelationshipAttribute unit test attribute";
            string sTooltip = "cCustomEntities_saveRelationship_UsingOneToManyRelationshipAttribute unit test tooltip";
            DateTime dtCreatedOn = DateTime.UtcNow;

            cCustomEntity reqFirstCustomEntity = null;
            cCustomEntity reqSecondCustomEntity = null;

            try
            {
                reqFirstCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));
                reqSecondCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

                #region Setup required views and forms

                cCustomEntityView reqView = cCustomEntityViewObject.New(reqFirstCustomEntity.entityid, cCustomEntityViewObject.Template(entityID: reqFirstCustomEntity.entityid));

                cCustomEntityForm reqForm = cCustomEntityFormObject.New(reqFirstCustomEntity.entityid, cCustomEntityFormObject.Template(entityID: reqFirstCustomEntity.entityid));

                #endregion

                cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                cOneToManyRelationship expected = cOneToManyRelationshipObject.Template(entityID: reqFirstCustomEntity.entityid, parentEntityID: reqSecondCustomEntity.entityid, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, relatedTable: reqFirstCustomEntity.table, viewID: reqView.viewid);

                int nAttributeID = reqCustomEntities.saveRelationship(reqSecondCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                reqSecondCustomEntity = reqCustomEntities.getEntityById(reqSecondCustomEntity.entityid);

             List<cOneToManyRelationship> actual = reqSecondCustomEntity.findOneToManyRelationship(reqFirstCustomEntity.entityid);

                //Store the Guid of the newly created attribute
                foreach (var item in actual)
                {
                    Assert.IsNotNull(item.fieldid);
                    Guid gFieldID = item.fieldid;

                    expected = cOneToManyRelationshipObject.Template(entityID: reqFirstCustomEntity.entityid, parentEntityID: reqSecondCustomEntity.entityid, attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, relatedTable: reqFirstCustomEntity.table, fieldID: gFieldID, viewID: reqView.viewid);

                    // Do not compare the values of CreatedOn, as they may be different
                    List<string> lstOmittedProps = new List<string>();
                    lstOmittedProps.Add("createdon");
                    lstOmittedProps.Add("accountid");

                    cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                    // Ensure the value of CreatedOn is greater than the one set at the start of the test
                    Assert.IsTrue(item.createdon.AddTicks(-item.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
                }
            }
            finally
            {
                if (reqSecondCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqSecondCustomEntity.entityid);
                }
                if (reqFirstCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqFirstCustomEntity.entityid);
                }
            }

        }

        /// <summary>
        ///A test for saveRelationship, using cManyToOneRelationshipAttribute
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveRelationship_UsingManyToOneRelationshipAttribute()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            cTables tables = new cTables(GlobalTestVariables.AccountId);
            cTable reqTable = tables.GetTableByName("Employees");
            string sDisplayName = "cCustomEntities_saveRelationship_UsingManyToOneRelationshipAttribute unit test attribute";
            string sTooltip = "cCustomEntities_saveRelationship_UsingManyToOneRelationshipAttribute unit test tooltip";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cManyToOneRelationshipObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, relatedTable: reqTable, aliasTable: reqTable);

                int nAttributeID = reqCustomEntities.saveRelationship(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cManyToOneRelationshipObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, fieldID: gFieldID, relatedTable: reqTable, aliasTable: reqTable);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");
                lstOmittedProps.Add("accountid");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveRelationship, using cManyToOneRelationshipAttribute which does not have an alias table set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveRelationship_UsingManyToOneRelationshipAttributeWhereAliasTableNotSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            cTables tables = new cTables(GlobalTestVariables.AccountId);
            cTable reqTable = tables.GetTableByName("Employees");
            string sDisplayName = "cCustomEntities_saveRelationship_UsingManyToOneRelationshipAttribute unit test attribute";
            string sTooltip = "cCustomEntities_saveRelationship_UsingManyToOneRelationshipAttribute unit test tooltip";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cManyToOneRelationshipObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, relatedTable: reqTable, aliasTable: null);

                int nAttributeID = reqCustomEntities.saveRelationship(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cManyToOneRelationshipObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, fieldID: gFieldID, relatedTable: reqTable, aliasTable: null);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");
                lstOmittedProps.Add("accountid");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveRelationship, where the attribute description has not been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveRelationship_WhereDescriptionIsNotSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            cTables tables = new cTables(GlobalTestVariables.AccountId);
            cTable reqTable = tables.GetTableByName("Employees");
            string sDisplayName = "cCustomEntities_saveRelationship_WhereDescriptionIsNotSet unit test";
            string sTooltip = "cCustomEntities_saveRelationship_WhereDescriptionIsNotSet unit test tooltip";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cManyToOneRelationshipObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, relatedTable: reqTable, aliasTable: reqTable, description: "");

                int nAttributeID = reqCustomEntities.saveRelationship(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cManyToOneRelationshipObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, fieldID: gFieldID, relatedTable: reqTable, aliasTable: reqTable, description: "");

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");
                lstOmittedProps.Add("accountid");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveRelationship, where the attribute tooltip has not been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveRelationship_WhereTooltipIsNotSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            cTables tables = new cTables(GlobalTestVariables.AccountId);
            cTable reqTable = tables.GetTableByName("Employees");
            string sDisplayName = "cCustomEntities_saveRelationship_WhereTooltipIsNotSet unit test attribute";
            string sTooltip = "";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cManyToOneRelationshipObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, relatedTable: reqTable, aliasTable: reqTable);

                int nAttributeID = reqCustomEntities.saveRelationship(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cManyToOneRelationshipObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, fieldID: gFieldID, relatedTable: reqTable, aliasTable: reqTable);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");
                lstOmittedProps.Add("accountid");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveRelationship, where the modified by property has been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveRelationship_WhereModifiedByIsSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            cTables tables = new cTables(GlobalTestVariables.AccountId);
            cTable reqTable = tables.GetTableByName("Employees");
            string sDisplayName = "cCustomEntities_saveRelationship_WhereModifiedByIsSet unit test attribute";
            string sTooltip = "cCustomEntities_saveRelationship_WhereModifiedByIsSet unit test tooltip";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cManyToOneRelationshipObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, relatedTable: reqTable, aliasTable: reqTable, modifiedBy: nEmployeeID, modifiedOn: dtCreatedOn);

                int nAttributeID = reqCustomEntities.saveRelationship(reqCustomEntity.entityid, expected);
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cManyToOneRelationshipObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, fieldID: gFieldID, relatedTable: reqTable, aliasTable: reqTable, modifiedBy: nEmployeeID, modifiedOn: dtCreatedOn);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");
                lstOmittedProps.Add("accountid");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveRelationship, using a non-relationship attribute
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        [ExpectedException(typeof(NullReferenceException))]
        public void cCustomEntities_saveRelationship_UsingNonRelationshipAttribute()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sDisplayName = "cCustomEntities_saveRelationship_UsingNonRelationshipAttribute unit test attribute";
            string sTooltip = "cCustomEntities_saveRelationship_UsingNonRelationshipAttribute unit test tooltip";

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                DateTime dtCreatedOn = DateTime.UtcNow;

                cAttribute expected = cNumberAttributeObject.Template(displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn);

                int nAttributeID = reqCustomEntities.saveRelationship(reqCustomEntity.entityid, expected);

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                cAttribute actual = reqCustomEntity.getAttributeById(nAttributeID);

                //Store the Guid of the newly created attribute
                Assert.IsNotNull(actual.fieldid);
                Guid gFieldID = actual.fieldid;

                expected = cNumberAttributeObject.Template(attributeID: nAttributeID, displayName: sDisplayName, toolTip: sTooltip, createdOn: dtCreatedOn, fieldID: gFieldID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");
                lstOmittedProps.Add("accountid");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }

        }

        #endregion SaveRelationship

        #region SaveView

        /// <summary>
        ///A test for saveView, using a basic view
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveView_UsingBasicView()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sViewName = "cCustomEntities_saveView_UsingBasicView view name";
            string sViewDescription = "cCustomEntities_saveView_UsingBasicView view description";
            GreenLightSortColumn oSortColumn = new GreenLightSortColumn(new Guid("12345678-1234-1234-1234-123456789123"), SortDirection.Ascending, null);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a template view, using the custom entity ID from above
                cCustomEntityView expected = cCustomEntityViewObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn);

                int nViewID = customEntities.saveView(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());
                expected = cCustomEntityViewObject.Template(viewID: nViewID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityView actual = reqCustomEntity.getViewById(nViewID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");
                lstOmittedProps.Add("accountid");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveView where the menuID has been specified
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveView_WithMenuIDSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sViewName = "cCustomEntities_saveView_WithMenuIDSet view name";
            string sViewDescription = "cCustomEntities_saveView_WithMenuIDSet view description";
            GreenLightSortColumn oSortColumn = new GreenLightSortColumn(new Guid("12345678-1234-1234-1234-123456789123"), SortDirection.Ascending, null);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a template view, using the custom entity ID from above
                cCustomEntityView expected = cCustomEntityViewObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn, menuID: 1);

                int nViewID = customEntities.saveView(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());
                expected = cCustomEntityViewObject.Template(viewID: nViewID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn, menuID: 1);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityView actual = reqCustomEntity.getViewById(nViewID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");
                lstOmittedProps.Add("accountid");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveView without description
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveView_WithoutDescription()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sViewName = "cCustomEntities_saveView_WithoutDescription view name";
            string sViewDescription = "";
            GreenLightSortColumn oSortColumn = new GreenLightSortColumn(new Guid("12345678-1234-1234-1234-123456789123"), SortDirection.Ascending, null);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a template view, using the custom entity ID from above
                cCustomEntityView expected = cCustomEntityViewObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn);

                int nViewID = customEntities.saveView(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());
                expected = cCustomEntityViewObject.Template(viewID: nViewID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityView actual = reqCustomEntity.getViewById(nViewID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveView where modified by is set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveView_WhereModifiedByIsSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sViewName = "cCustomEntities_saveView_WithoutDescription view name";
            string sViewDescription = "cCustomEntities_saveView_WithoutDescription view description";
            GreenLightSortColumn oSortColumn = new GreenLightSortColumn(new Guid("12345678-1234-1234-1234-123456789123"), SortDirection.Ascending, null);
            DateTime dtModifiedOn = DateTime.UtcNow;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a template view, using the custom entity ID from above
                cCustomEntityView expected = cCustomEntityViewObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn, modifiedBy: nEmployeeID, modifiedOn: dtModifiedOn);

                int nViewID = customEntities.saveView(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());
                expected = cCustomEntityViewObject.Template(viewID: nViewID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn, modifiedBy: nEmployeeID, modifiedOn: dtModifiedOn);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityView actual = reqCustomEntity.getViewById(nViewID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveView where allowAdd has been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveView_WhereAllowAddIsSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sViewName = "cCustomEntities_saveView_WhereAllowAddIsSet view name";
            string sViewDescription = "cCustomEntities_saveView_WhereAllowAddIsSet view description";
            GreenLightSortColumn oSortColumn = new GreenLightSortColumn(new Guid("12345678-1234-1234-1234-123456789123"), SortDirection.Ascending, null);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                cCustomEntityForm reqForm = cCustomEntityFormObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid);

                // Create a template view, using the custom entity ID from above
                cCustomEntityView expected = cCustomEntityViewObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn, allowAdd: true, addForm: reqForm);

                int nViewID = customEntities.saveView(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());
                expected = cCustomEntityViewObject.Template(viewID: nViewID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn, allowAdd: true, addForm: reqForm);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityView actual = reqCustomEntity.getViewById(nViewID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveView where allowEdit has been set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveView_WhereAllowEditIsSet()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sViewName = "cCustomEntities_saveView_WhereAllowEditIsSet view name";
            string sViewDescription = "cCustomEntities_saveView_WhereAllowEditIsSet view description";
            GreenLightSortColumn oSortColumn = new GreenLightSortColumn(new Guid("12345678-1234-1234-1234-123456789123"), SortDirection.Ascending, null);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                cCustomEntityForm reqForm = cCustomEntityFormObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid);

                // Create a template view, using the custom entity ID from above
                cCustomEntityView expected = cCustomEntityViewObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn, allowEdit: true, editForm: reqForm);

                int nViewID = customEntities.saveView(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());
                expected = cCustomEntityViewObject.Template(viewID: nViewID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn, allowEdit: true, editForm: reqForm);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityView actual = reqCustomEntity.getViewById(nViewID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveView where the view name and view ID already exist in the database
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveView_WhereViewNameAndViewIDAlreadyExist()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sViewName = "cCustomEntities_saveView_WhereViewNameAndViewIDAlreadyExist view name";
            string sViewDescription = "cCustomEntities_saveView_WhereViewNameAndViewIDAlreadyExist view description";
            GreenLightSortColumn oSortColumn = new GreenLightSortColumn(new Guid("12345678-1234-1234-1234-123456789123"), SortDirection.Ascending, null);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                // Create a new view
                cCustomEntityView reqFirstView = cCustomEntityViewObject.New(reqCustomEntity.entityid, cCustomEntityViewObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn));

                // Create a template view, using the custom entity ID from above
                cCustomEntityView reqSecondView = cCustomEntityViewObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn);

                int nViewID = customEntities.saveView(reqCustomEntity.entityid, reqSecondView);

                Assert.AreEqual(-1, nViewID);
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
        ///A test for saveView where the view fields exist
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveView_WhereViewFieldsExist()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;
            string sViewName = "cCustomEntities_saveView_WhereViewFieldsExist view name";
            string sViewDescription = "cCustomEntities_saveView_WhereViewFieldsExist view description";
            GreenLightSortColumn oSortColumn = new GreenLightSortColumn(new Guid("12345678-1234-1234-1234-123456789123"), SortDirection.Ascending, null);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                cFields fields = new cFields(Moqs.CurrentUser().AccountID);
                cField reqField = fields.GetFieldByID(reqCustomEntity.attributes.Values[0].fieldid);

                SortedList<byte, cCustomEntityViewField> lstFields = new SortedList<byte, cCustomEntityViewField>();
                lstFields.Add(0, new cCustomEntityViewField(reqField));

                // Create a template view, using the custom entity ID from above
                cCustomEntityView expected = cCustomEntityViewObject.Template(createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn, fields: lstFields);

                int nViewID = customEntities.saveView(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());
                expected = cCustomEntityViewObject.Template(viewID: nViewID, createdBy: nEmployeeID, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn, fields: lstFields);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityView actual = reqCustomEntity.getViewById(nViewID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
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
        ///A test for saveView where the view fields exist using a delegate
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_saveView_WhereViewFieldsExistUsingDelegate()
        {
            Mock<ICurrentUser> currentUser = Moqs.CurrentUserDelegateMock();
            string sViewName = "cCustomEntities_saveView_WhereViewFieldsExistUsingDelegate view name";
            string sViewDescription = "cCustomEntities_saveView_WhereViewFieldsExistUsingDelegate view description";
            GreenLightSortColumn oSortColumn = new GreenLightSortColumn(new Guid("12345678-1234-1234-1234-123456789123"), SortDirection.Ascending, null);

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: GlobalTestVariables.DelegateId));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(currentUser.Object);

                cFields fields = new cFields(Moqs.CurrentUser().AccountID);
                cField reqField = fields.GetFieldByID(reqCustomEntity.attributes.Values[0].fieldid);

                SortedList<byte, cCustomEntityViewField> lstFields = new SortedList<byte, cCustomEntityViewField>();
                lstFields.Add(0, new cCustomEntityViewField(reqField));

                // Create a template view, using the custom entity ID from above
                cCustomEntityView expected = cCustomEntityViewObject.Template(createdBy: GlobalTestVariables.DelegateId, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn, fields: lstFields);

                int nViewID = customEntities.saveView(reqCustomEntity.entityid, expected);
                customEntities = new cCustomEntities(Moqs.CurrentUser());
                expected = cCustomEntityViewObject.Template(viewID: nViewID, createdBy: GlobalTestVariables.DelegateId, entityID: reqCustomEntity.entityid, viewName: sViewName, description: sViewDescription, sortColumn: oSortColumn, fields: lstFields);

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                cCustomEntityView actual = reqCustomEntity.getViewById(nViewID);

                // Do not compare the values of CreatedOn, as they may be different
                List<string> lstOmittedProps = new List<string>();
                lstOmittedProps.Add("createdon");

                cCompareAssert.AreEqual(expected, actual, lstOmittedProperties: lstOmittedProps);

                // Ensure the value of CreatedOn is greater than the one set at the start of the test
                Assert.IsTrue(actual.createdon.AddTicks(-actual.createdon.Ticks) >= expected.createdon.AddTicks(-expected.createdon.Ticks));
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        #endregion SaveView

        #region DeleteForm

        /// <summary>
        ///A test for deleteForm, using a valid formID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_deleteForm_UsingValidFormID()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                cCustomEntityForm reqForm = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.Template(entityID: reqCustomEntity.entityid));

                int retcode = customEntities.deleteForm(reqForm.formid, nEmployeeID, 0);

                Assert.AreEqual(null, reqCustomEntity.getFormById(reqForm.formid));
                Assert.AreEqual(0, retcode);
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
        ///A test for deleteForm, using an incorrect formID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_deleteForm_UsingIncorrectFormID()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int retcode = customEntities.deleteForm(-69, nEmployeeID, 0);

                Assert.AreEqual(null, reqCustomEntity.getFormById(-69));
                Assert.AreEqual(0, retcode);
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        #endregion DeleteForm

        #region DeleteView

        /// <summary>
        ///A test for deleteView, using a valid viewID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_deleteView_UsingValidViewID()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                cCustomEntityView reqView = cCustomEntityViewObject.New(reqCustomEntity.entityid, cCustomEntityViewObject.Template(entityID: reqCustomEntity.entityid));

                int retcode = customEntities.deleteView(reqView.viewid);

                Assert.AreEqual(null, reqCustomEntity.getViewById(reqView.viewid));
                Assert.AreEqual(0, retcode);
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
        ///A test for deleteView, using an incorrect viewID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_deleteView_UsingIncorrectViewID()
        {
            int nEmployeeID = Moqs.CurrentUser().EmployeeID;

            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: nEmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                int retcode = customEntities.deleteView(-69);

                Assert.AreEqual(null, reqCustomEntity.getViewById(-69));
                Assert.AreEqual(0, retcode);
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

        #region saveEntitySystemView
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void cCustomEntities_saveEntitySystemView_ViewentityNull()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;

            try
            {
                entity1 = cCustomEntityObject.New();
                entity2 = cCustomEntityObject.New();
                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid);

                Assert.AreEqual(5, entity1.attributes.Count);
                Assert.AreEqual(5, entity2.attributes.Count);

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity3 = clsCustomEntities.getEntityById(entity1.entityid);
                cCustomEntity entity4 = clsCustomEntities.getEntityById(entity2.entityid);

                cOneToManyRelationship attribute = cOneToManyRelationshipObject.New(entity3.entityid, cOneToManyRelationshipObject.BasicOTM(entity3.entityid, entity4));

                int returnVal = clsCustomEntities.saveEntitySystemView(null, entity3.entityid, attribute.attributeid, entity4);
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

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_saveEntitySystemView_NoDescriptionModifiedByNull()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;
            cCustomEntity viewentity = null;

            try
            {
                entity1 = cCustomEntityObject.New();
                entity2 = cCustomEntityObject.New();
                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid);

                Assert.AreEqual(5, entity1.attributes.Count);
                Assert.AreEqual(5, entity2.attributes.Count);

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity3 = clsCustomEntities.getEntityById(entity1.entityid);
                cCustomEntity entity4 = clsCustomEntities.getEntityById(entity2.entityid);

                cOneToManyRelationship attribute = cOneToManyRelationshipObject.New(entity3.entityid, cOneToManyRelationshipObject.BasicOTM(entity3.entityid, entity4));

                viewentity = new cCustomEntity(0, entity3.entityname + "_" + entity4.entityname, entity3.pluralname + "_" + entity4.pluralname, "", DateTime.UtcNow, currentUser.EmployeeID, null, null, new SortedList<int, cAttribute>(), new SortedList<int, cCustomEntityForm>(), new SortedList<int, cCustomEntityView>(), null, null, entity3.EnableAttachments, entity3.AudienceView, entity3.AllowMergeConfigAccess, true, entity4.entityid, entity3.entityid, entity3.EnableCurrencies, entity3.DefaultCurrencyID, entity3.EnablePopupWindow, entity3.DefaultPopupView, entity3.FormSelectionAttributeId, entity3.OwnerId, entity3.SupportContactId, entity3.SupportQuestion, entity3.EnableLocking, entity3.BuiltIn);

                int returnVal = clsCustomEntities.saveEntitySystemView(viewentity, entity3.entityid, attribute.attributeid, entity4);

                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity5 = clsCustomEntities.getSystemViewEntity(entity4.entityid, entity3.entityid);

                Assert.IsNotNull(entity5);
                Assert.AreEqual(false, entity5.modifiedby.HasValue);
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

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_saveEntitySystemView_DescriptionModifiedByNull()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;
            cCustomEntity viewentity = null;

            try
            {
                entity1 = cCustomEntityObject.New();
                entity2 = cCustomEntityObject.New();
                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid);

                Assert.AreEqual(5, entity1.attributes.Count);
                Assert.AreEqual(5, entity2.attributes.Count);

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity3 = clsCustomEntities.getEntityById(entity1.entityid);
                cCustomEntity entity4 = clsCustomEntities.getEntityById(entity2.entityid);

                cOneToManyRelationship attribute = cOneToManyRelationshipObject.New(entity3.entityid, cOneToManyRelationshipObject.BasicOTM(entity3.entityid, entity4));

                viewentity = new cCustomEntity(0, entity3.entityname + "_" + entity4.entityname, entity3.pluralname + "_" + entity4.pluralname, "System View description", DateTime.UtcNow, currentUser.EmployeeID, null, null, new SortedList<int, cAttribute>(), new SortedList<int, cCustomEntityForm>(), new SortedList<int, cCustomEntityView>(), null, null, entity3.EnableAttachments, entity3.AudienceView, entity3.AllowMergeConfigAccess, true, entity4.entityid, entity3.entityid, entity3.EnableCurrencies, entity3.DefaultCurrencyID, entity3.EnablePopupWindow, entity3.DefaultPopupView, entity3.FormSelectionAttributeId, entity3.OwnerId, entity3.SupportContactId, entity3.SupportQuestion, entity3.EnableLocking, entity3.BuiltIn);

                int returnVal = clsCustomEntities.saveEntitySystemView(viewentity, entity3.entityid, attribute.attributeid, entity4);

                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity5 = clsCustomEntities.getSystemViewEntity(entity4.entityid, entity3.entityid);

                Assert.IsNotNull(entity5);
                Assert.AreEqual("System View description", entity5.description);
                Assert.AreEqual(false, entity5.modifiedby.HasValue);
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

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_saveEntitySystemView_DescriptionModifiedByNotNull()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;
            cCustomEntity viewentity = null;

            try
            {
                entity1 = cCustomEntityObject.New();
                entity2 = cCustomEntityObject.New();
                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid);

                Assert.AreEqual(5, entity1.attributes.Count);
                Assert.AreEqual(5, entity2.attributes.Count);

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity3 = clsCustomEntities.getEntityById(entity1.entityid);
                cCustomEntity entity4 = clsCustomEntities.getEntityById(entity2.entityid);

                cOneToManyRelationship attribute = cOneToManyRelationshipObject.New(entity3.entityid, cOneToManyRelationshipObject.BasicOTM(entity3.entityid, entity4));

                viewentity = new cCustomEntity(0, entity3.entityname + "_" + entity4.entityname, entity3.pluralname + "_" + entity4.pluralname, "System View description", DateTime.UtcNow, currentUser.EmployeeID, DateTime.UtcNow, currentUser.EmployeeID, new SortedList<int, cAttribute>(), new SortedList<int, cCustomEntityForm>(), new SortedList<int, cCustomEntityView>(), null, null, entity3.EnableAttachments, entity3.AudienceView, entity3.AllowMergeConfigAccess, true, entity4.entityid, entity3.entityid, entity3.EnableCurrencies, entity3.DefaultCurrencyID, entity3.EnablePopupWindow, entity3.DefaultPopupView, entity3.FormSelectionAttributeId, entity3.OwnerId, entity3.SupportContactId, entity3.SupportQuestion, entity3.EnableLocking, entity3.BuiltIn);

                int returnVal = clsCustomEntities.saveEntitySystemView(viewentity, entity3.entityid, attribute.attributeid, entity4);

                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity5 = clsCustomEntities.getSystemViewEntity(entity4.entityid, entity3.entityid);

                Assert.IsNotNull(entity5);
                Assert.AreEqual("System View description", entity5.description);
                // saveCustomEntitySystemView stored procedure can't set modifiedBy (an entity shouldn't be able to edit a system view, so that doesn't matter)
                Assert.AreEqual(false, entity5.modifiedby.HasValue);
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

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_saveEntitySystemView_EditSystemView()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = null;
            cCustomEntity entity2 = null;
            cCustomEntity viewentity = null;

            try
            {
                entity1 = cCustomEntityObject.New();
                entity2 = cCustomEntityObject.New();
                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid);

                Assert.AreEqual(5, entity1.attributes.Count);
                Assert.AreEqual(5, entity2.attributes.Count);

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity3 = clsCustomEntities.getEntityById(entity1.entityid);
                cCustomEntity entity4 = clsCustomEntities.getEntityById(entity2.entityid);

                cOneToManyRelationship attribute = cOneToManyRelationshipObject.New(entity3.entityid, cOneToManyRelationshipObject.BasicOTM(entity3.entityid, entity4));

                viewentity = new cCustomEntity(0, entity3.entityname + "_" + entity4.entityname, entity3.pluralname + "_" + entity4.pluralname, "System View description", DateTime.UtcNow, currentUser.EmployeeID, DateTime.UtcNow, currentUser.EmployeeID, new SortedList<int, cAttribute>(), new SortedList<int, cCustomEntityForm>(), new SortedList<int, cCustomEntityView>(), null, null, entity3.EnableAttachments, entity3.AudienceView, entity3.AllowMergeConfigAccess, true, entity4.entityid, entity3.entityid, entity3.EnableCurrencies, entity3.DefaultCurrencyID, entity3.EnablePopupWindow, entity3.DefaultPopupView, entity3.FormSelectionAttributeId, entity3.OwnerId, entity3.SupportContactId, entity3.SupportQuestion, entity3.EnableLocking, entity3.BuiltIn);

                int returnVal = clsCustomEntities.saveEntitySystemView(viewentity, entity3.entityid, attribute.attributeid, entity4);

                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity entity5 = clsCustomEntities.getSystemViewEntity(entity4.entityid, entity3.entityid);

                Assert.IsNotNull(entity5);
                Assert.AreEqual("System View description", entity5.description);
                // saveCustomEntitySystemView stored procedure can't set modifiedBy (an entity shouldn't be able to edit a system view, so that doesn't matter)
                Assert.AreEqual(false, entity5.modifiedby.HasValue);

                returnVal = clsCustomEntities.saveEntitySystemView(entity5, entity3.entityid, attribute.attributeid, entity4);

                Assert.AreEqual(0, returnVal);
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
        #endregion saveEntitySystemView

        #region saveSummaryAttribute
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void cCustomEntities_saveSummaryAttribute_NullElementsAndColumns()
        {
            cCustomEntity entity1 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test"));

            try
            {
                cCustomEntities clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                int returnVal = clsCustomEntities.saveSummaryAttribute(entity1.entityid, cSummaryAttributeObject.Template());
            }
            finally
            {
                if (entity1 != null)
                {
                    cCustomEntityObject.TearDown(entity1.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_saveSummaryAttribute_EmptyElementsAndColumns()
        {
            cCustomEntity entity1 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test"));

            try
            {
                cCustomEntities clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                int returnVal = clsCustomEntities.saveSummaryAttribute(entity1.entityid, cSummaryAttributeObject.Template(summaryColumns: new Dictionary<int, cSummaryAttributeColumn>(), summaryElements: new Dictionary<int, cSummaryAttributeElement>()));

                // Shouldn't error but shouldn't save
                Assert.AreEqual(0, returnVal);
            }
            finally
            {
                if (entity1 != null)
                {
                    cCustomEntityObject.TearDown(entity1.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_saveSummaryAttribute_BasicSummary()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test parent"));
            cCustomEntity entity2 = null;
            cCustomEntity entity3 = null;
            int returnVal = -999;

            try
            {
                entity2 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test child"));
                entity3 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test shared"));

                Assert.IsNotNull(entity1, "entity1 should not be null");
                Assert.IsNotNull(entity2, "entity2 should not be null");
                Assert.IsNotNull(entity3, "entity3 should not be null");
                Assert.IsTrue(entity1.entityid > 0, "entity1.entityid should be greater than 0");
                Assert.IsTrue(entity2.entityid > 0, "entity2.entityid should be greater than 0");
                Assert.IsTrue(entity3.entityid > 0, "entity3.entityid should be greater than 0");

                cCustomEntityForm form2 = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view2 = cCustomEntityViewObject.New(entity2.entityid);
                cCustomEntityForm form3 = cCustomEntityFormObject.New(entity3.entityid, cCustomEntityFormObject.Basic(entity3));
                cCustomEntityView view3 = cCustomEntityViewObject.New(entity3.entityid);

                Assert.IsNotNull(form2, "form2 should not be null");
                Assert.IsNotNull(view2, "view2 should not be null");
                Assert.IsNotNull(form3, "form3 should not be null");
                Assert.IsNotNull(view3, "view3 should not be null");
                Assert.IsTrue(form2.formid > 0, "form2.formid should be greater than 0");
                Assert.IsTrue(view2.viewid > 0, "view2.viewid should be greater than 0");
                Assert.IsTrue(form3.formid > 0, "form3.formid should be greater than 0");
                Assert.IsTrue(view3.viewid > 0, "view3.viewid should be greater than 0");

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity parent = null;
                cCustomEntity child = clsCustomEntities.getEntityById(entity2.entityid);
                cCustomEntity related = clsCustomEntities.getEntityById(entity3.entityid);

                Assert.IsNotNull(child, "child should not be null");
                Assert.IsNotNull(related, "related should not be null");

                cOneToManyRelationship attribute1to2 = cOneToManyRelationshipObject.New(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, child));
                cOneToManyRelationship attribute1to3 = cOneToManyRelationshipObject.New(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, related));
                cOneToManyRelationship attribute2to3 = cOneToManyRelationshipObject.New(child.entityid, cOneToManyRelationshipObject.BasicOTM(child.entityid, related));

                Assert.IsNotNull(attribute1to2, "attribute1to2 should not be null");
                Assert.IsNotNull(attribute1to3, "attribute1to3 should not be null");
                Assert.IsNotNull(attribute2to3, "attribute2to3 should not be null");
                Assert.IsTrue(attribute1to2.attributeid > 0, "attribute1to2.attributeid should be greater than 0");
                Assert.IsTrue(attribute1to3.attributeid > 0, "attribute1to3.attributeid should be greater than 0");
                Assert.IsTrue(attribute2to3.attributeid > 0, "attribute2to3.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(entity1.entityid);
                child = clsCustomEntities.getEntityById(child.entityid);
                related = clsCustomEntities.getEntityById(related.entityid);

                int firstAttributeID = (from x in related.attributes select x.Value.attributeid).FirstOrDefault();

                cSummaryAttributeColumn column = cSummaryAttributeColumnObject.Template(columnAttributeID: firstAttributeID);
                cSummaryAttributeElement element1 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute1to3.attributeid, order: 0);
                cSummaryAttributeElement element2 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute2to3.attributeid, order: 1);

                returnVal = clsCustomEntities.saveSummaryAttribute(entity1.entityid, cSummaryAttributeObject.Template(summaryColumns: new Dictionary<int, cSummaryAttributeColumn> { { firstAttributeID, column } }, summaryElements: new Dictionary<int, cSummaryAttributeElement> { { 0, element1 }, { 1, element2 } }, sourceEntityID: related.entityid));

                // Should be the new attribute id
                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(parent.entityid);
                cSummaryAttribute summaryAttribute = (cSummaryAttribute)parent.getAttributeById(returnVal);

                Assert.IsNotNull(summaryAttribute, "summaryAttribute should not be null");
                Assert.AreEqual(1, summaryAttribute.SummaryColumns.Count);
                Assert.AreEqual(2, summaryAttribute.SummaryElements.Count);
            }
            finally
            {
                if (returnVal > 0)
                {
                    cSummaryAttributeObject.TearDown(returnVal);
                }
                if (entity1 != null)
                {
                    cCustomEntityObject.TearDown(entity1.entityid);
                }
                if (entity2 != null)
                {
                    cCustomEntityObject.TearDown(entity2.entityid);
                }
                if (entity3 != null)
                {
                    cCustomEntityObject.TearDown(entity3.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_saveSummaryAttribute_BasicSummaryAsDelegate()
        {
            ICurrentUser currentUser = Moqs.CurrentUserDelegateMock().Object;
            cCustomEntity entity1 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test parent"));
            cCustomEntity entity2 = null;
            cCustomEntity entity3 = null;
            int returnVal = -999;

            try
            {
                entity2 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test child"));
                entity3 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test shared"));

                Assert.IsNotNull(entity1, "entity1 should not be null");
                Assert.IsNotNull(entity2, "entity2 should not be null");
                Assert.IsNotNull(entity3, "entity3 should not be null");
                Assert.IsTrue(entity1.entityid > 0, "entity1.entityid should be greater than 0");
                Assert.IsTrue(entity2.entityid > 0, "entity2.entityid should be greater than 0");
                Assert.IsTrue(entity3.entityid > 0, "entity3.entityid should be greater than 0");

                cCustomEntityForm form2 = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view2 = cCustomEntityViewObject.New(entity2.entityid);
                cCustomEntityForm form3 = cCustomEntityFormObject.New(entity3.entityid, cCustomEntityFormObject.Basic(entity3));
                cCustomEntityView view3 = cCustomEntityViewObject.New(entity3.entityid);

                Assert.IsNotNull(form2, "form2 should not be null");
                Assert.IsNotNull(view2, "view2 should not be null");
                Assert.IsNotNull(form3, "form3 should not be null");
                Assert.IsNotNull(view3, "view3 should not be null");
                Assert.IsTrue(form2.formid > 0, "form2.formid should be greater than 0");
                Assert.IsTrue(view2.viewid > 0, "view2.viewid should be greater than 0");
                Assert.IsTrue(form3.formid > 0, "form3.formid should be greater than 0");
                Assert.IsTrue(view3.viewid > 0, "view3.viewid should be greater than 0");

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity parent = null;
                cCustomEntity child = clsCustomEntities.getEntityById(entity2.entityid);
                cCustomEntity related = clsCustomEntities.getEntityById(entity3.entityid);

                Assert.IsNotNull(child, "child should not be null");
                Assert.IsNotNull(related, "related should not be null");

                cOneToManyRelationship attribute1to2 = cOneToManyRelationshipObject.New(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, child));
                cOneToManyRelationship attribute1to3 = cOneToManyRelationshipObject.New(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, related));
                cOneToManyRelationship attribute2to3 = cOneToManyRelationshipObject.New(child.entityid, cOneToManyRelationshipObject.BasicOTM(child.entityid, related));

                Assert.IsNotNull(attribute1to2, "attribute1to2 should not be null");
                Assert.IsNotNull(attribute1to3, "attribute1to3 should not be null");
                Assert.IsNotNull(attribute2to3, "attribute2to3 should not be null");
                Assert.IsTrue(attribute1to2.attributeid > 0, "attribute1to2.attributeid should be greater than 0");
                Assert.IsTrue(attribute1to3.attributeid > 0, "attribute1to3.attributeid should be greater than 0");
                Assert.IsTrue(attribute2to3.attributeid > 0, "attribute2to3.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(entity1.entityid);
                child = clsCustomEntities.getEntityById(child.entityid);
                related = clsCustomEntities.getEntityById(related.entityid);

                int firstAttributeID = (from x in related.attributes select x.Value.attributeid).FirstOrDefault();

                cSummaryAttributeColumn column = cSummaryAttributeColumnObject.Template(columnAttributeID: firstAttributeID);
                cSummaryAttributeElement element1 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute1to3.attributeid, order: 0);
                cSummaryAttributeElement element2 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute2to3.attributeid, order: 1);

                returnVal = clsCustomEntities.saveSummaryAttribute(entity1.entityid, cSummaryAttributeObject.Template(summaryColumns: new Dictionary<int, cSummaryAttributeColumn> { { firstAttributeID, column } }, summaryElements: new Dictionary<int, cSummaryAttributeElement> { { 0, element1 }, { 1, element2 } }, sourceEntityID: related.entityid));

                // Should be the new attribute id
                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(parent.entityid);
                cSummaryAttribute summaryAttribute = (cSummaryAttribute)parent.getAttributeById(returnVal);

                Assert.IsNotNull(summaryAttribute, "summaryAttribute should not be null");
                Assert.AreEqual(1, summaryAttribute.SummaryColumns.Count);
                Assert.AreEqual(2, summaryAttribute.SummaryElements.Count);
            }
            finally
            {
                if (returnVal > 0)
                {
                    cSummaryAttributeObject.TearDown(returnVal);
                }
                if (entity1 != null)
                {
                    cCustomEntityObject.TearDown(entity1.entityid);
                }
                if (entity2 != null)
                {
                    cCustomEntityObject.TearDown(entity2.entityid);
                }
                if (entity3 != null)
                {
                    cCustomEntityObject.TearDown(entity3.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_saveSummaryAttribute_EditBasicSummaryRemoveElement()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test parent"));
            cCustomEntity entity2 = null;
            cCustomEntity entity3 = null;
            int returnVal = -999;

            try
            {
                entity2 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test child"));
                entity3 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test shared"));

                Assert.IsNotNull(entity1, "entity1 should not be null");
                Assert.IsNotNull(entity2, "entity2 should not be null");
                Assert.IsNotNull(entity3, "entity3 should not be null");
                Assert.IsTrue(entity1.entityid > 0, "entity1.entityid should be greater than 0");
                Assert.IsTrue(entity2.entityid > 0, "entity2.entityid should be greater than 0");
                Assert.IsTrue(entity3.entityid > 0, "entity3.entityid should be greater than 0");

                cCustomEntityForm form2 = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view2 = cCustomEntityViewObject.New(entity2.entityid);
                cCustomEntityForm form3 = cCustomEntityFormObject.New(entity3.entityid, cCustomEntityFormObject.Basic(entity3));
                cCustomEntityView view3 = cCustomEntityViewObject.New(entity3.entityid);

                Assert.IsNotNull(form2, "form2 should not be null");
                Assert.IsNotNull(view2, "view2 should not be null");
                Assert.IsNotNull(form3, "form3 should not be null");
                Assert.IsNotNull(view3, "view3 should not be null");
                Assert.IsTrue(form2.formid > 0, "form2.formid should be greater than 0");
                Assert.IsTrue(view2.viewid > 0, "view2.viewid should be greater than 0");
                Assert.IsTrue(form3.formid > 0, "form3.formid should be greater than 0");
                Assert.IsTrue(view3.viewid > 0, "view3.viewid should be greater than 0");

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity parent = null;
                cCustomEntity child = clsCustomEntities.getEntityById(entity2.entityid);
                cCustomEntity related = clsCustomEntities.getEntityById(entity3.entityid);

                Assert.IsNotNull(child, "child should not be null");
                Assert.IsNotNull(related, "related should not be null");

                cOneToManyRelationship attribute1to2 = cOneToManyRelationshipObject.New(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, child));
                cOneToManyRelationship attribute1to3 = cOneToManyRelationshipObject.New(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, related));
                cOneToManyRelationship attribute2to3 = cOneToManyRelationshipObject.New(child.entityid, cOneToManyRelationshipObject.BasicOTM(child.entityid, related));

                Assert.IsNotNull(attribute1to2, "attribute1to2 should not be null");
                Assert.IsNotNull(attribute1to3, "attribute1to3 should not be null");
                Assert.IsNotNull(attribute2to3, "attribute2to3 should not be null");
                Assert.IsTrue(attribute1to2.attributeid > 0, "attribute1to2.attributeid should be greater than 0");
                Assert.IsTrue(attribute1to3.attributeid > 0, "attribute1to3.attributeid should be greater than 0");
                Assert.IsTrue(attribute2to3.attributeid > 0, "attribute2to3.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(entity1.entityid);
                child = clsCustomEntities.getEntityById(child.entityid);
                related = clsCustomEntities.getEntityById(related.entityid);

                int firstAttributeID = (from x in related.attributes select x.Value.attributeid).FirstOrDefault();

                cSummaryAttributeColumn column = cSummaryAttributeColumnObject.Template(columnAttributeID: firstAttributeID);
                cSummaryAttributeElement element1 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute1to3.attributeid, order: 0);
                cSummaryAttributeElement element2 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute2to3.attributeid, order: 1);

                returnVal = clsCustomEntities.saveSummaryAttribute(entity1.entityid, cSummaryAttributeObject.Template(summaryColumns: new Dictionary<int, cSummaryAttributeColumn> { { firstAttributeID, column } }, summaryElements: new Dictionary<int, cSummaryAttributeElement> { { 0, element1 }, { 1, element2 } }, sourceEntityID: related.entityid));

                // Should be the new attribute id
                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(parent.entityid);
                cSummaryAttribute summaryAttribute = (cSummaryAttribute)parent.getAttributeById(returnVal);

                Assert.IsNotNull(summaryAttribute, "summaryAttribute should not be null");
                Assert.AreEqual(1, summaryAttribute.SummaryColumns.Count);
                Assert.AreEqual(2, summaryAttribute.SummaryElements.Count);

                // Remove a summary element
                summaryAttribute.SummaryElements.Remove(summaryAttribute.SummaryElements.Last().Key);
                // save it again
                int newReturnVal = clsCustomEntities.saveSummaryAttribute(parent.entityid, summaryAttribute);

                // return ids should be the same
                Assert.IsTrue(newReturnVal > 0);
                Assert.AreEqual(returnVal, newReturnVal);

                // refresh the entity and attribute
                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(parent.entityid);
                summaryAttribute = (cSummaryAttribute)parent.getAttributeById(newReturnVal);

                Assert.IsNotNull(summaryAttribute, "summaryAttribute should not be null");
                Assert.AreEqual(1, summaryAttribute.SummaryColumns.Count);
                Assert.AreEqual(1, summaryAttribute.SummaryElements.Count); // now one element
                Assert.AreEqual(attribute1to3.attributeid, summaryAttribute.SummaryElements.First().Value.OTM_AttributeId); // check its the right one
            }
            finally
            {
                if (returnVal > 0)
                {
                    cSummaryAttributeObject.TearDown(returnVal);
                }
                if (entity1 != null)
                {
                    cCustomEntityObject.TearDown(entity1.entityid);
                }
                if (entity2 != null)
                {
                    cCustomEntityObject.TearDown(entity2.entityid);
                }
                if (entity3 != null)
                {
                    cCustomEntityObject.TearDown(entity3.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_saveSummaryAttribute_EditBasicSummaryRemoveColumn()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test parent"));
            cCustomEntity entity2 = null;
            cCustomEntity entity3 = null;
            int returnVal = -999;

            try
            {
                entity2 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test child"));
                entity3 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "SaveSummaryAttribute test shared"));

                Assert.IsNotNull(entity1, "entity1 should not be null");
                Assert.IsNotNull(entity2, "entity2 should not be null");
                Assert.IsNotNull(entity3, "entity3 should not be null");
                Assert.IsTrue(entity1.entityid > 0, "entity1.entityid should be greater than 0");
                Assert.IsTrue(entity2.entityid > 0, "entity2.entityid should be greater than 0");
                Assert.IsTrue(entity3.entityid > 0, "entity3.entityid should be greater than 0");

                cCustomEntityForm form2 = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view2 = cCustomEntityViewObject.New(entity2.entityid);
                cCustomEntityForm form3 = cCustomEntityFormObject.New(entity3.entityid, cCustomEntityFormObject.Basic(entity3));
                cCustomEntityView view3 = cCustomEntityViewObject.New(entity3.entityid);

                Assert.IsNotNull(form2, "form2 should not be null");
                Assert.IsNotNull(view2, "view2 should not be null");
                Assert.IsNotNull(form3, "form3 should not be null");
                Assert.IsNotNull(view3, "view3 should not be null");
                Assert.IsTrue(form2.formid > 0, "form2.formid should be greater than 0");
                Assert.IsTrue(view2.viewid > 0, "view2.viewid should be greater than 0");
                Assert.IsTrue(form3.formid > 0, "form3.formid should be greater than 0");
                Assert.IsTrue(view3.viewid > 0, "view3.viewid should be greater than 0");

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity parent = null;
                cCustomEntity child = clsCustomEntities.getEntityById(entity2.entityid);
                cCustomEntity related = clsCustomEntities.getEntityById(entity3.entityid);

                Assert.IsNotNull(child, "child should not be null");
                Assert.IsNotNull(related, "related should not be null");

                cOneToManyRelationship attribute1to2 = cOneToManyRelationshipObject.New(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, child));
                cOneToManyRelationship attribute1to3 = cOneToManyRelationshipObject.New(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, related));
                cOneToManyRelationship attribute2to3 = cOneToManyRelationshipObject.New(child.entityid, cOneToManyRelationshipObject.BasicOTM(child.entityid, related));

                Assert.IsNotNull(attribute1to2, "attribute1to2 should not be null");
                Assert.IsNotNull(attribute1to3, "attribute1to3 should not be null");
                Assert.IsNotNull(attribute2to3, "attribute2to3 should not be null");
                Assert.IsTrue(attribute1to2.attributeid > 0, "attribute1to2.attributeid should be greater than 0");
                Assert.IsTrue(attribute1to3.attributeid > 0, "attribute1to3.attributeid should be greater than 0");
                Assert.IsTrue(attribute2to3.attributeid > 0, "attribute2to3.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(entity1.entityid);
                child = clsCustomEntities.getEntityById(child.entityid);
                related = clsCustomEntities.getEntityById(related.entityid);

                string dt = DateTime.UtcNow.Ticks.ToString();

                int firstAttributeID = (from x in related.attributes select x.Value.attributeid).FirstOrDefault();
                int secondAttributeID = (from x in related.attributes
                                         where x.Value.attributeid != firstAttributeID
                                         select x.Value.attributeid).FirstOrDefault();

                cSummaryAttributeColumn column1 = cSummaryAttributeColumnObject.Template(columnAttributeID: firstAttributeID);
                cSummaryAttributeColumn column2 = cSummaryAttributeColumnObject.Template(columnAttributeID: secondAttributeID, alternateHeader: "Alt column 2 header " + dt, order: 1);
                cSummaryAttributeElement element1 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute1to3.attributeid, order: 0);
                cSummaryAttributeElement element2 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute2to3.attributeid, order: 1);

                returnVal = clsCustomEntities.saveSummaryAttribute(entity1.entityid, cSummaryAttributeObject.Template(summaryColumns: new Dictionary<int, cSummaryAttributeColumn> { { column1.ColumnAttributeID, column1 }, { column2.ColumnAttributeID, column2 } }, summaryElements: new Dictionary<int, cSummaryAttributeElement> { { 0, element1 }, { 1, element2 } }, sourceEntityID: related.entityid));

                // Should be the new attribute id
                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(parent.entityid);
                cSummaryAttribute summaryAttribute = (cSummaryAttribute)parent.getAttributeById(returnVal);

                Assert.IsNotNull(summaryAttribute, "summaryAttribute should not be null");
                Assert.AreEqual(2, summaryAttribute.SummaryColumns.Count);
                Assert.AreEqual(2, summaryAttribute.SummaryElements.Count);

                // Remove a summary column
                summaryAttribute.SummaryColumns.Remove(summaryAttribute.SummaryColumns.Last().Key);
                // save it again
                int newReturnVal = clsCustomEntities.saveSummaryAttribute(parent.entityid, summaryAttribute);

                // return ids should be the same
                Assert.IsTrue(newReturnVal > 0);
                Assert.AreEqual(returnVal, newReturnVal);

                // refresh the entity and attribute
                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(parent.entityid);
                summaryAttribute = (cSummaryAttribute)parent.getAttributeById(newReturnVal);

                Assert.IsNotNull(summaryAttribute, "summaryAttribute should not be null");
                Assert.AreEqual(1, summaryAttribute.SummaryColumns.Count); // now one column
                Assert.AreEqual(2, summaryAttribute.SummaryElements.Count);
            }
            finally
            {
                if (returnVal > 0)
                {
                    cSummaryAttributeObject.TearDown(returnVal);
                }
                if (entity1 != null)
                {
                    cCustomEntityObject.TearDown(entity1.entityid);
                }
                if (entity2 != null)
                {
                    cCustomEntityObject.TearDown(entity2.entityid);
                }
                if (entity3 != null)
                {
                    cCustomEntityObject.TearDown(entity3.entityid);
                }
            }
        }
        #endregion saveSummaryAttribute

        #region DeleteCustomEntityRecord
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void cCustomEntities_DeleteCustomEntityRecord_Nulls()
        {
            cCustomEntities clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
            int success = clsCustomEntities.DeleteCustomEntityRecord(null, 0, 0);
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_DeleteCustomEntityRecord_NegativeRecordID()
        {
            cCustomEntity entity = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test"));

            try
            {
                cCustomEntities clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                int success = clsCustomEntities.DeleteCustomEntityRecord(entity, -999, 0);

                // deleting a record that doesn't exist, doesn't cause an error, so 0 for success
                Assert.AreEqual(0, success);
            }
            finally
            {
                if (entity != null) cCustomEntityObject.TearDown(entity.entityid);
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_DeleteCustomEntityRecord_ZeroRecordID()
        {
            cCustomEntity entity = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test"));

            try
            {
                cCustomEntities clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                int success = clsCustomEntities.DeleteCustomEntityRecord(entity, 0, 0);

                Assert.AreEqual(0, success);
            }
            finally
            {
                if (entity != null) cCustomEntityObject.TearDown(entity.entityid);
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_DeleteCustomEntityRecord_ErrorDuringPrivateDelete()
        {
            cCustomEntity entity = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test"));

            try
            {
                Mock<ICurrentUser> currentUser = Moqs.CurrentUserDelegateMock();
                // this should cause the private method's delete try to fail and fall into the catch
                currentUser.Setup(x => x.Delegate).Returns<Employee>(null);

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser.Object);

                int success = clsCustomEntities.DeleteCustomEntityRecord(entity, -999, 0);

                // returns -99 on failure
                Assert.AreEqual(-99, success);
            }
            finally
            {
                if (entity != null) cCustomEntityObject.TearDown(entity.entityid);
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_DeleteCustomEntityRecord_BasicEntityRecord()
        {
            cCustomEntity entity = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test"));

            try
            {
                cCustomEntityForm form = cCustomEntityFormObject.New(entity.entityid, cCustomEntityFormObject.Basic(entity));

                Dictionary<int, object> recordValues;
                int recordID = CustomEntityRecord.New(entity, out recordValues);

                Assert.AreEqual(4, recordValues.Count);

                cCustomEntities clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                SortedList<int, object> values = clsCustomEntities.getEntityRecord(entity, recordID, form);

                Assert.AreEqual(1, values.Count); // check we get a value back via normal get

                int success = clsCustomEntities.DeleteCustomEntityRecord(entity, recordID, 0);

                // when sucessfully deleted, passes back 0 on success
                Assert.AreEqual(0, success);

                clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                values = clsCustomEntities.getEntityRecord(entity, recordID, form);

                Assert.AreEqual(0, values.Count);
            }
            finally
            {
                if (entity != null) cCustomEntityObject.TearDown(entity.entityid);
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_DeleteCustomEntityRecord_BasicEntityRecordAsDelegate()
        {
            ICurrentUser currentUser = Moqs.CurrentUserDelegateMock().Object;
            cCustomEntity entity = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test"));

            try
            {
                cCustomEntityForm form = cCustomEntityFormObject.New(entity.entityid, cCustomEntityFormObject.Basic(entity));

                Dictionary<int, object> recordValues;
                int recordID = CustomEntityRecord.New(entity, out recordValues);

                Assert.AreEqual(4, recordValues.Count);

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                SortedList<int, object> values = clsCustomEntities.getEntityRecord(entity, recordID, form);

                Assert.AreEqual(1, values.Count); // check we get a value back via normal get

                int success = clsCustomEntities.DeleteCustomEntityRecord(entity, recordID, 0);

                // when sucessfully deleted, passes back 0 on success
                Assert.AreEqual(0, success);

                clsCustomEntities = new cCustomEntities(currentUser);
                values = clsCustomEntities.getEntityRecord(entity, recordID, form);

                Assert.AreEqual(0, values.Count);
            }
            finally
            {
                if (entity != null) cCustomEntityObject.TearDown(entity.entityid);
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_DeleteCustomEntityRecord_OTMEntityRecord()
        {
            cCustomEntity entity1 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test"));
            cCustomEntity entity2 = null;
            cCustomEntity entity3 = null;
            cCustomEntity entity4 = null;

            try
            {
                entity2 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test"));

                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid);
                cCustomEntities clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                entity3 = clsCustomEntities.getEntityById(entity1.entityid);
                entity4 = clsCustomEntities.getEntityById(entity2.entityid);

                cOneToManyRelationship attribute = cOneToManyRelationshipObject.New(entity3.entityid, cOneToManyRelationshipObject.BasicOTM(entity3.entityid, entity4));

                clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                entity3 = clsCustomEntities.getEntityById(entity3.entityid);
                entity4 = clsCustomEntities.getEntityById(entity4.entityid);

                Dictionary<int, object> recordValues;
                int recordID1 = CustomEntityRecord.New(entity3, out recordValues);

                Assert.AreEqual(4, recordValues.Count);

                int recordID2 = CustomEntityRecord.New(entity4, out recordValues, attribute.attributeid, recordID1);

                Assert.AreEqual(5, recordValues.Count); // entity 4 should have the extra value for the one to many that is on entity3

                clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                SortedList<int, object> values = clsCustomEntities.getEntityRecord(entity4, recordID2, form);

                Assert.AreEqual(1, values.Count); // check we get a value back via normal get for the 1 field on the form

                int success = clsCustomEntities.DeleteCustomEntityRecord(entity4, recordID2, 0);

                // when sucessfully deleted, passes back 0
                Assert.AreEqual(0, success);

                clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                values = clsCustomEntities.getEntityRecord(entity4, recordID2, form);

                Assert.AreEqual(0, values.Count);
            }
            finally
            {
                if (entity1 != null) cCustomEntityObject.TearDown(entity1.entityid);
                if (entity2 != null) cCustomEntityObject.TearDown(entity2.entityid);
                if (entity3 != null) cCustomEntityObject.TearDown(entity3.entityid);
                if (entity4 != null) cCustomEntityObject.TearDown(entity4.entityid);
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_DeleteCustomEntityRecord_OTMEntityRecordParentRecord()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test parent"));
            cCustomEntity entity2 = null;
            cCustomEntity entity3 = null;
            int returnVal = -999;

            try
            {
                entity2 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test child"));
                entity3 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test shared"));

                Assert.IsNotNull(entity1, "entity1 should not be null");
                Assert.IsNotNull(entity2, "entity2 should not be null");
                Assert.IsNotNull(entity3, "entity3 should not be null");
                Assert.IsTrue(entity1.entityid > 0, "entity1.entityid should be greater than 0");
                Assert.IsTrue(entity2.entityid > 0, "entity2.entityid should be greater than 0");
                Assert.IsTrue(entity3.entityid > 0, "entity3.entityid should be greater than 0");

                cCustomEntityForm form2 = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view2 = cCustomEntityViewObject.New(entity2.entityid);
                cCustomEntityForm form3 = cCustomEntityFormObject.New(entity3.entityid, cCustomEntityFormObject.Basic(entity3));
                cCustomEntityView view3 = cCustomEntityViewObject.New(entity3.entityid);

                Assert.IsNotNull(form2, "form2 should not be null");
                Assert.IsNotNull(view2, "view2 should not be null");
                Assert.IsNotNull(form3, "form3 should not be null");
                Assert.IsNotNull(view3, "view3 should not be null");
                Assert.IsTrue(form2.formid > 0, "form2.formid should be greater than 0");
                Assert.IsTrue(view2.viewid > 0, "view2.viewid should be greater than 0");
                Assert.IsTrue(form3.formid > 0, "form3.formid should be greater than 0");
                Assert.IsTrue(view3.viewid > 0, "view3.viewid should be greater than 0");

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity parent = null;
                cCustomEntity child = clsCustomEntities.getEntityById(entity2.entityid);
                cCustomEntity related = clsCustomEntities.getEntityById(entity3.entityid);

                Assert.IsNotNull(child, "child should not be null");
                Assert.IsNotNull(related, "related should not be null");

                cOneToManyRelationship attribute1to2 = cOneToManyRelationshipObject.NewWithDerived(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, child));
                cOneToManyRelationship attribute1to3 = cOneToManyRelationshipObject.NewWithDerived(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, related));
                cOneToManyRelationship attribute2to3 = cOneToManyRelationshipObject.NewWithDerived(child.entityid, cOneToManyRelationshipObject.BasicOTM(child.entityid, related));

                Assert.IsNotNull(attribute1to2, "attribute1to2 should not be null");
                Assert.IsNotNull(attribute1to3, "attribute1to3 should not be null");
                Assert.IsNotNull(attribute2to3, "attribute2to3 should not be null");
                Assert.IsTrue(attribute1to2.attributeid > 0, "attribute1to2.attributeid should be greater than 0");
                Assert.IsTrue(attribute1to3.attributeid > 0, "attribute1to3.attributeid should be greater than 0");
                Assert.IsTrue(attribute2to3.attributeid > 0, "attribute2to3.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(entity1.entityid);
                child = clsCustomEntities.getEntityById(child.entityid);
                related = clsCustomEntities.getEntityById(related.entityid);

                int firstAttributeID = (from x in related.attributes select x.Value.attributeid).FirstOrDefault();

                cSummaryAttributeColumn column = cSummaryAttributeColumnObject.Template(columnAttributeID: firstAttributeID);
                cSummaryAttributeElement element1 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute1to3.attributeid, order: 0);
                cSummaryAttributeElement element2 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute2to3.attributeid, order: 1);

                returnVal = clsCustomEntities.saveSummaryAttribute(entity1.entityid, cSummaryAttributeObject.Template(summaryColumns: new Dictionary<int, cSummaryAttributeColumn> { { firstAttributeID, column } }, summaryElements: new Dictionary<int, cSummaryAttributeElement> { { 0, element1 }, { 1, element2 } }, sourceEntityID: related.entityid));

                // Should be the new attribute id
                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(parent.entityid);
                cSummaryAttribute summaryAttribute = (cSummaryAttribute)parent.getAttributeById(returnVal);

                Assert.IsNotNull(summaryAttribute, "summaryAttribute should not be null");
                Assert.AreEqual(1, summaryAttribute.SummaryColumns.Count);
                Assert.AreEqual(2, summaryAttribute.SummaryElements.Count);

                cCustomEntity systemViewEntity = clsCustomEntities.getSystemViewEntity(related.entityid, parent.entityid);

                Dictionary<int, object> recordValues;
                int recordID1 = CustomEntityRecord.New(parent, out recordValues);

                Assert.IsTrue(recordID1 > 0);
                Assert.AreEqual(4, recordValues.Count);

                int recordID2 = CustomEntityRecord.New(related, out recordValues, attribute1to3.attributeid, recordID1);

                Assert.IsTrue(recordID2 > 0);
                Assert.AreEqual(5, recordValues.Count);

                int success = clsCustomEntities.DeleteCustomEntityRecord(parent, recordID1, 0);

                // when sucessfully deleted, passes back 0
                Assert.AreEqual(0, success);
            }
            finally
            {
                if (returnVal > 0) cSummaryAttributeObject.TearDown(returnVal);
                if (entity1 != null) cCustomEntityObject.TearDown(entity1.entityid);
                if (entity2 != null) cCustomEntityObject.TearDown(entity2.entityid);
                if (entity3 != null) cCustomEntityObject.TearDown(entity3.entityid);
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_DeleteCustomEntityRecord_OTMEntityRecordParentRecordsParentRecord()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test parent"));
            cCustomEntity entity2 = null;
            cCustomEntity entity3 = null;

            try
            {
                entity2 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test child"));
                entity3 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test shared"));

                Assert.IsNotNull(entity1, "entity1 should not be null");
                Assert.IsNotNull(entity2, "entity2 should not be null");
                Assert.IsNotNull(entity3, "entity3 should not be null");
                Assert.IsTrue(entity1.entityid > 0, "entity1.entityid should be greater than 0");
                Assert.IsTrue(entity2.entityid > 0, "entity2.entityid should be greater than 0");
                Assert.IsTrue(entity3.entityid > 0, "entity3.entityid should be greater than 0");

                cCustomEntityForm form2 = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view2 = cCustomEntityViewObject.New(entity2.entityid, cCustomEntityViewObject.Template(editForm: form2, allowEdit: true));
                cCustomEntityForm form3 = cCustomEntityFormObject.New(entity3.entityid, cCustomEntityFormObject.Basic(entity3));
                cCustomEntityView view3 = cCustomEntityViewObject.New(entity3.entityid, cCustomEntityViewObject.Template(editForm: form3, allowEdit: true));

                Assert.IsNotNull(form2, "form2 should not be null");
                Assert.IsNotNull(view2, "view2 should not be null");
                Assert.IsNotNull(form3, "form3 should not be null");
                Assert.IsNotNull(view3, "view3 should not be null");
                Assert.IsTrue(form2.formid > 0, "form2.formid should be greater than 0");
                Assert.IsTrue(view2.viewid > 0, "view2.viewid should be greater than 0");
                Assert.IsTrue(form3.formid > 0, "form3.formid should be greater than 0");
                Assert.IsTrue(view3.viewid > 0, "view3.viewid should be greater than 0");

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity parent = null;
                cCustomEntity child = clsCustomEntities.getEntityById(entity2.entityid);
                cCustomEntity related = clsCustomEntities.getEntityById(entity3.entityid);

                Assert.IsNotNull(child, "child should not be null");
                Assert.IsNotNull(related, "related should not be null");

                cOneToManyRelationship attribute1to2 = cOneToManyRelationshipObject.New(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, child));
                cOneToManyRelationship attribute1to3 = cOneToManyRelationshipObject.New(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, related));
                cOneToManyRelationship attribute2to3 = cOneToManyRelationshipObject.New(child.entityid, cOneToManyRelationshipObject.BasicOTM(child.entityid, related));

                Assert.IsNotNull(attribute1to2, "attribute1to2 should not be null");
                Assert.IsNotNull(attribute1to3, "attribute1to3 should not be null");
                Assert.IsNotNull(attribute2to3, "attribute2to3 should not be null");
                Assert.IsTrue(attribute1to2.attributeid > 0, "attribute1to2.attributeid should be greater than 0");
                Assert.IsTrue(attribute1to3.attributeid > 0, "attribute1to3.attributeid should be greater than 0");
                Assert.IsTrue(attribute2to3.attributeid > 0, "attribute2to3.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(entity1.entityid);
                child = clsCustomEntities.getEntityById(child.entityid);
                related = clsCustomEntities.getEntityById(related.entityid);

                // construct records for parent, parent to child and child to related
                // to trigger a recursive deletecustomentityrecord call
                Dictionary<int, object> recordValues;
                int recordID1 = CustomEntityRecord.New(parent, out recordValues);

                Assert.IsTrue(recordID1 > 0);
                Assert.AreEqual(4, recordValues.Count);

                int recordID2 = CustomEntityRecord.New(child, out recordValues, attribute1to2.attributeid, recordID1);

                Assert.IsTrue(recordID2 > 0);
                Assert.AreEqual(5, recordValues.Count);

                int recordID3 = CustomEntityRecord.New(related, out recordValues, attribute1to3.attributeid, recordID2);

                Assert.IsTrue(recordID3 > 0);
                Assert.AreEqual(5, recordValues.Count);

                int recordID4 = CustomEntityRecord.New(related, out recordValues, attribute2to3.attributeid, recordID3);

                Assert.IsTrue(recordID4 > 0);
                Assert.AreEqual(5, recordValues.Count);

                int success = clsCustomEntities.DeleteCustomEntityRecord(parent, recordID1, 0);

                // when sucessfully deleted, passes back 0
                Assert.AreEqual(0, success);
            }
            finally
            {
                if (entity1 != null) cCustomEntityObject.TearDown(entity1.entityid);
                if (entity2 != null) cCustomEntityObject.TearDown(entity2.entityid);
                if (entity3 != null) cCustomEntityObject.TearDown(entity3.entityid);
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_DeleteCustomEntityRecord_OTMEntityRecordParentRecordWithAudienceAllowed()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test parent", audienceViewType: AudienceViewType.AllowAllIfNoneExist));
            cCustomEntity entity2 = null;
            cCustomEntity entity3 = null;
            cAudience audience1 = null;

            int returnVal = -999;

            try
            {
                entity2 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test child"));
                entity3 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test shared"));

                Assert.IsNotNull(entity1, "entity1 should not be null");
                Assert.IsNotNull(entity2, "entity2 should not be null");
                Assert.IsNotNull(entity3, "entity3 should not be null");
                Assert.IsTrue(entity1.entityid > 0, "entity1.entityid should be greater than 0");
                Assert.IsTrue(entity2.entityid > 0, "entity2.entityid should be greater than 0");
                Assert.IsTrue(entity3.entityid > 0, "entity3.entityid should be greater than 0");

                cCustomEntityForm form2 = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view2 = cCustomEntityViewObject.New(entity2.entityid);
                cCustomEntityForm form3 = cCustomEntityFormObject.New(entity3.entityid, cCustomEntityFormObject.Basic(entity3));
                cCustomEntityView view3 = cCustomEntityViewObject.New(entity3.entityid);

                Assert.IsNotNull(form2, "form2 should not be null");
                Assert.IsNotNull(view2, "view2 should not be null");
                Assert.IsNotNull(form3, "form3 should not be null");
                Assert.IsNotNull(view3, "view3 should not be null");
                Assert.IsTrue(form2.formid > 0, "form2.formid should be greater than 0");
                Assert.IsTrue(view2.viewid > 0, "view2.viewid should be greater than 0");
                Assert.IsTrue(form3.formid > 0, "form3.formid should be greater than 0");
                Assert.IsTrue(view3.viewid > 0, "view3.viewid should be greater than 0");

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity parent = null;
                cCustomEntity child = clsCustomEntities.getEntityById(entity2.entityid);
                cCustomEntity related = clsCustomEntities.getEntityById(entity3.entityid);

                Assert.IsNotNull(child, "child should not be null");
                Assert.IsNotNull(related, "related should not be null");

                cOneToManyRelationship attribute1to2 = cOneToManyRelationshipObject.NewWithDerived(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, child));
                cOneToManyRelationship attribute1to3 = cOneToManyRelationshipObject.NewWithDerived(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, related));
                cOneToManyRelationship attribute2to3 = cOneToManyRelationshipObject.NewWithDerived(child.entityid, cOneToManyRelationshipObject.BasicOTM(child.entityid, related));

                Assert.IsNotNull(attribute1to2, "attribute1to2 should not be null");
                Assert.IsNotNull(attribute1to3, "attribute1to3 should not be null");
                Assert.IsNotNull(attribute2to3, "attribute2to3 should not be null");
                Assert.IsTrue(attribute1to2.attributeid > 0, "attribute1to2.attributeid should be greater than 0");
                Assert.IsTrue(attribute1to3.attributeid > 0, "attribute1to3.attributeid should be greater than 0");
                Assert.IsTrue(attribute2to3.attributeid > 0, "attribute2to3.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(entity1.entityid);
                child = clsCustomEntities.getEntityById(child.entityid);
                related = clsCustomEntities.getEntityById(related.entityid);

                int firstAttributeID = (from x in related.attributes select x.Value.attributeid).FirstOrDefault();

                cSummaryAttributeColumn column = cSummaryAttributeColumnObject.Template(columnAttributeID: firstAttributeID);
                cSummaryAttributeElement element1 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute1to3.attributeid, order: 0);
                cSummaryAttributeElement element2 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute2to3.attributeid, order: 1);

                returnVal = clsCustomEntities.saveSummaryAttribute(entity1.entityid, cSummaryAttributeObject.Template(summaryColumns: new Dictionary<int, cSummaryAttributeColumn> { { firstAttributeID, column } }, summaryElements: new Dictionary<int, cSummaryAttributeElement> { { 0, element1 }, { 1, element2 } }, sourceEntityID: related.entityid));

                // Should be the new attribute id
                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(parent.entityid);
                cSummaryAttribute summaryAttribute = (cSummaryAttribute)parent.getAttributeById(returnVal);

                Assert.IsNotNull(summaryAttribute, "summaryAttribute should not be null");
                Assert.AreEqual(1, summaryAttribute.SummaryColumns.Count);
                Assert.AreEqual(2, summaryAttribute.SummaryElements.Count);

                cCustomEntity systemViewEntity = clsCustomEntities.getSystemViewEntity(related.entityid, parent.entityid);

                Dictionary<int, object> recordValues;
                int recordID1 = CustomEntityRecord.New(parent, out recordValues);

                Assert.IsTrue(recordID1 > 0);
                Assert.AreEqual(4, recordValues.Count);

                int recordID2 = CustomEntityRecord.New(related, out recordValues, attribute1to3.attributeid, recordID1);

                Assert.IsTrue(recordID2 > 0);
                Assert.AreEqual(5, recordValues.Count);

                audience1 = cAudienceObject.New();
                cAudienceRecordStatusObject.New(recordID1, parent.AudienceTable.TableID, new List<int> { audience1.audienceID });

                int success = clsCustomEntities.DeleteCustomEntityRecord(parent, recordID1, 0);

                // when sucessfully deleted, passes back 0
                Assert.AreEqual(0, success);
            }
            finally
            {
                if (audience1 != null) cAudienceObject.TearDown(audience1.audienceID);
                if (returnVal > 0) cSummaryAttributeObject.TearDown(returnVal);
                if (entity1 != null) cCustomEntityObject.TearDown(entity1.entityid);
                if (entity2 != null) cCustomEntityObject.TearDown(entity2.entityid);
                if (entity3 != null) cCustomEntityObject.TearDown(entity3.entityid);
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_DeleteCustomEntityRecord_OTMEntityRecordParentRecordWithAudienceDenied()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test parent", audienceViewType: AudienceViewType.AllowAllIfNoneExist));
            cCustomEntity entity2 = null;
            cCustomEntity entity3 = null;
            cAudience audience1 = null;
            cAudience audience2 = null;
            int returnVal = -999;

            try
            {
                entity2 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test child"));
                entity3 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test shared"));

                Assert.IsNotNull(entity1, "entity1 should not be null");
                Assert.IsNotNull(entity2, "entity2 should not be null");
                Assert.IsNotNull(entity3, "entity3 should not be null");
                Assert.IsTrue(entity1.entityid > 0, "entity1.entityid should be greater than 0");
                Assert.IsTrue(entity2.entityid > 0, "entity2.entityid should be greater than 0");
                Assert.IsTrue(entity3.entityid > 0, "entity3.entityid should be greater than 0");

                cCustomEntityForm form2 = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view2 = cCustomEntityViewObject.New(entity2.entityid);
                cCustomEntityForm form3 = cCustomEntityFormObject.New(entity3.entityid, cCustomEntityFormObject.Basic(entity3));
                cCustomEntityView view3 = cCustomEntityViewObject.New(entity3.entityid);

                Assert.IsNotNull(form2, "form2 should not be null");
                Assert.IsNotNull(view2, "view2 should not be null");
                Assert.IsNotNull(form3, "form3 should not be null");
                Assert.IsNotNull(view3, "view3 should not be null");
                Assert.IsTrue(form2.formid > 0, "form2.formid should be greater than 0");
                Assert.IsTrue(view2.viewid > 0, "view2.viewid should be greater than 0");
                Assert.IsTrue(form3.formid > 0, "form3.formid should be greater than 0");
                Assert.IsTrue(view3.viewid > 0, "view3.viewid should be greater than 0");

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity parent = null;
                cCustomEntity child = clsCustomEntities.getEntityById(entity2.entityid);
                cCustomEntity related = clsCustomEntities.getEntityById(entity3.entityid);

                Assert.IsNotNull(child, "child should not be null");
                Assert.IsNotNull(related, "related should not be null");

                cOneToManyRelationship attribute1to2 = cOneToManyRelationshipObject.NewWithDerived(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, child));
                cOneToManyRelationship attribute1to3 = cOneToManyRelationshipObject.NewWithDerived(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, related));
                cOneToManyRelationship attribute2to3 = cOneToManyRelationshipObject.NewWithDerived(child.entityid, cOneToManyRelationshipObject.BasicOTM(child.entityid, related));

                Assert.IsNotNull(attribute1to2, "attribute1to2 should not be null");
                Assert.IsNotNull(attribute1to3, "attribute1to3 should not be null");
                Assert.IsNotNull(attribute2to3, "attribute2to3 should not be null");
                Assert.IsTrue(attribute1to2.attributeid > 0, "attribute1to2.attributeid should be greater than 0");
                Assert.IsTrue(attribute1to3.attributeid > 0, "attribute1to3.attributeid should be greater than 0");
                Assert.IsTrue(attribute2to3.attributeid > 0, "attribute2to3.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(entity1.entityid);
                child = clsCustomEntities.getEntityById(child.entityid);
                related = clsCustomEntities.getEntityById(related.entityid);

                int firstAttributeID = (from x in related.attributes select x.Value.attributeid).FirstOrDefault();

                cSummaryAttributeColumn column = cSummaryAttributeColumnObject.Template(columnAttributeID: firstAttributeID);
                cSummaryAttributeElement element1 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute1to3.attributeid, order: 0);
                cSummaryAttributeElement element2 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute2to3.attributeid, order: 1);

                returnVal = clsCustomEntities.saveSummaryAttribute(entity1.entityid, cSummaryAttributeObject.Template(summaryColumns: new Dictionary<int, cSummaryAttributeColumn> { { firstAttributeID, column } }, summaryElements: new Dictionary<int, cSummaryAttributeElement> { { 0, element1 }, { 1, element2 } }, sourceEntityID: related.entityid));

                // Should be the new attribute id
                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(parent.entityid);
                cSummaryAttribute summaryAttribute = (cSummaryAttribute)parent.getAttributeById(returnVal);

                Assert.IsNotNull(summaryAttribute, "summaryAttribute should not be null");
                Assert.AreEqual(1, summaryAttribute.SummaryColumns.Count);
                Assert.AreEqual(2, summaryAttribute.SummaryElements.Count);

                cCustomEntity systemViewEntity = clsCustomEntities.getSystemViewEntity(related.entityid, parent.entityid);

                Dictionary<int, object> recordValues;
                int recordID1 = CustomEntityRecord.New(parent, out recordValues);

                Assert.IsTrue(recordID1 > 0);
                Assert.AreEqual(4, recordValues.Count);

                int recordID2 = CustomEntityRecord.New(related, out recordValues, attribute1to3.attributeid, recordID1);

                Assert.IsTrue(recordID2 > 0);
                Assert.AreEqual(5, recordValues.Count);

                audience1 = cAudienceObject.New(employeeIDList: new List<int> { GlobalTestVariables.AlternativeEmployeeId });
                cAudienceRecordStatusObject.New(recordID1, parent.AudienceTable.TableID, new List<int> { audience1.audienceID });

                audience2 = cAudienceObject.New();
                cAudienceRecordStatusObject.New(recordID1, parent.AudienceTable.TableID, new List<int> { audience2.audienceID }, canDelete: false);

                int success = clsCustomEntities.DeleteCustomEntityRecord(parent, recordID1, 0);

                // when deleted fails due to audience check, passes back -1
                Assert.AreEqual(-1, success);

                // update audience to be allowed to delete
                cAudienceRecordStatusObject.New(recordID1, parent.AudienceTable.TableID, new List<int> { audience2.audienceID });

                success = clsCustomEntities.DeleteCustomEntityRecord(parent, recordID1, 0);

                // when sucessfully deleted, passes back 0
                Assert.AreEqual(0, success);
            }
            finally
            {
                if (audience2 != null) cAudienceObject.TearDown(audience2.audienceID);
                if (audience1 != null) cAudienceObject.TearDown(audience1.audienceID);
                if (returnVal > 0) cSummaryAttributeObject.TearDown(returnVal);
                if (entity1 != null) cCustomEntityObject.TearDown(entity1.entityid);
                if (entity2 != null) cCustomEntityObject.TearDown(entity2.entityid);
                if (entity3 != null) cCustomEntityObject.TearDown(entity3.entityid);
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_DeleteCustomEntityRecord_OTMEntityRecordParentRecordWithNoAudience()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test parent", audienceViewType: AudienceViewType.AllowAllIfNoneExist));
            cCustomEntity entity2 = null;
            cCustomEntity entity3 = null;
            cAudience audience1 = null;
            int returnVal = -999;

            try
            {
                entity2 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test child"));
                entity3 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test shared"));

                Assert.IsNotNull(entity1, "entity1 should not be null");
                Assert.IsNotNull(entity2, "entity2 should not be null");
                Assert.IsNotNull(entity3, "entity3 should not be null");
                Assert.IsTrue(entity1.entityid > 0, "entity1.entityid should be greater than 0");
                Assert.IsTrue(entity2.entityid > 0, "entity2.entityid should be greater than 0");
                Assert.IsTrue(entity3.entityid > 0, "entity3.entityid should be greater than 0");

                cCustomEntityForm form2 = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view2 = cCustomEntityViewObject.New(entity2.entityid);
                cCustomEntityForm form3 = cCustomEntityFormObject.New(entity3.entityid, cCustomEntityFormObject.Basic(entity3));
                cCustomEntityView view3 = cCustomEntityViewObject.New(entity3.entityid);

                Assert.IsNotNull(form2, "form2 should not be null");
                Assert.IsNotNull(view2, "view2 should not be null");
                Assert.IsNotNull(form3, "form3 should not be null");
                Assert.IsNotNull(view3, "view3 should not be null");
                Assert.IsTrue(form2.formid > 0, "form2.formid should be greater than 0");
                Assert.IsTrue(view2.viewid > 0, "view2.viewid should be greater than 0");
                Assert.IsTrue(form3.formid > 0, "form3.formid should be greater than 0");
                Assert.IsTrue(view3.viewid > 0, "view3.viewid should be greater than 0");

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity parent = null;
                cCustomEntity child = clsCustomEntities.getEntityById(entity2.entityid);
                cCustomEntity related = clsCustomEntities.getEntityById(entity3.entityid);

                Assert.IsNotNull(child, "child should not be null");
                Assert.IsNotNull(related, "related should not be null");

                cOneToManyRelationship attribute1to2 = cOneToManyRelationshipObject.NewWithDerived(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, child));
                cOneToManyRelationship attribute1to3 = cOneToManyRelationshipObject.NewWithDerived(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, related));
                cOneToManyRelationship attribute2to3 = cOneToManyRelationshipObject.NewWithDerived(child.entityid, cOneToManyRelationshipObject.BasicOTM(child.entityid, related));

                Assert.IsNotNull(attribute1to2, "attribute1to2 should not be null");
                Assert.IsNotNull(attribute1to3, "attribute1to3 should not be null");
                Assert.IsNotNull(attribute2to3, "attribute2to3 should not be null");
                Assert.IsTrue(attribute1to2.attributeid > 0, "attribute1to2.attributeid should be greater than 0");
                Assert.IsTrue(attribute1to3.attributeid > 0, "attribute1to3.attributeid should be greater than 0");
                Assert.IsTrue(attribute2to3.attributeid > 0, "attribute2to3.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(entity1.entityid);
                child = clsCustomEntities.getEntityById(child.entityid);
                related = clsCustomEntities.getEntityById(related.entityid);

                int firstAttributeID = (from x in related.attributes select x.Value.attributeid).FirstOrDefault();

                cSummaryAttributeColumn column = cSummaryAttributeColumnObject.Template(columnAttributeID: firstAttributeID);
                cSummaryAttributeElement element1 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute1to3.attributeid, order: 0);
                cSummaryAttributeElement element2 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute2to3.attributeid, order: 1);

                returnVal = clsCustomEntities.saveSummaryAttribute(entity1.entityid, cSummaryAttributeObject.Template(summaryColumns: new Dictionary<int, cSummaryAttributeColumn> { { firstAttributeID, column } }, summaryElements: new Dictionary<int, cSummaryAttributeElement> { { 0, element1 }, { 1, element2 } }, sourceEntityID: related.entityid));

                // Should be the new attribute id
                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(parent.entityid);
                cSummaryAttribute summaryAttribute = (cSummaryAttribute)parent.getAttributeById(returnVal);

                Assert.IsNotNull(summaryAttribute, "summaryAttribute should not be null");
                Assert.AreEqual(1, summaryAttribute.SummaryColumns.Count);
                Assert.AreEqual(2, summaryAttribute.SummaryElements.Count);

                cCustomEntity systemViewEntity = clsCustomEntities.getSystemViewEntity(related.entityid, parent.entityid);

                Dictionary<int, object> recordValues;
                int recordID1 = CustomEntityRecord.New(parent, out recordValues);

                Assert.IsTrue(recordID1 > 0);
                Assert.AreEqual(4, recordValues.Count);

                int recordID2 = CustomEntityRecord.New(related, out recordValues, attribute1to3.attributeid, recordID1);

                Assert.IsTrue(recordID2 > 0);
                Assert.AreEqual(5, recordValues.Count);

                audience1 = cAudienceObject.New(employeeIDList: new List<int> { GlobalTestVariables.AlternativeEmployeeId });
                cAudienceRecordStatusObject.New(recordID1, parent.AudienceTable.TableID, new List<int> { audience1.audienceID });

                int success = clsCustomEntities.DeleteCustomEntityRecord(parent, recordID1, 0);

                // when deleted fails due to audience check, passes back -1
                Assert.AreEqual(-1, success);
            }
            finally
            {
                if (audience1 != null) cAudienceObject.TearDown(audience1.audienceID);
                if (returnVal > 0) cSummaryAttributeObject.TearDown(returnVal);
                if (entity1 != null) cCustomEntityObject.TearDown(entity1.entityid);
                if (entity2 != null) cCustomEntityObject.TearDown(entity2.entityid);
                if (entity3 != null) cCustomEntityObject.TearDown(entity3.entityid);
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_DeleteCustomEntityRecord_SystemViewEntityRecord()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test parent"));
            cCustomEntity entity2 = null;
            cCustomEntity entity3 = null;
            int returnVal = -999;

            try
            {
                entity2 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test child"));
                entity3 = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "DeleteCustomEntityRecord test shared"));

                Assert.IsNotNull(entity1, "entity1 should not be null");
                Assert.IsNotNull(entity2, "entity2 should not be null");
                Assert.IsNotNull(entity3, "entity3 should not be null");
                Assert.IsTrue(entity1.entityid > 0, "entity1.entityid should be greater than 0");
                Assert.IsTrue(entity2.entityid > 0, "entity2.entityid should be greater than 0");
                Assert.IsTrue(entity3.entityid > 0, "entity3.entityid should be greater than 0");

                cCustomEntityForm form2 = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view2 = cCustomEntityViewObject.New(entity2.entityid);
                cCustomEntityForm form3 = cCustomEntityFormObject.New(entity3.entityid, cCustomEntityFormObject.Basic(entity3));
                cCustomEntityView view3 = cCustomEntityViewObject.New(entity3.entityid);

                Assert.IsNotNull(form2, "form2 should not be null");
                Assert.IsNotNull(view2, "view2 should not be null");
                Assert.IsNotNull(form3, "form3 should not be null");
                Assert.IsNotNull(view3, "view3 should not be null");
                Assert.IsTrue(form2.formid > 0, "form2.formid should be greater than 0");
                Assert.IsTrue(view2.viewid > 0, "view2.viewid should be greater than 0");
                Assert.IsTrue(form3.formid > 0, "form3.formid should be greater than 0");
                Assert.IsTrue(view3.viewid > 0, "view3.viewid should be greater than 0");

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity parent = null;
                cCustomEntity child = clsCustomEntities.getEntityById(entity2.entityid);
                cCustomEntity related = clsCustomEntities.getEntityById(entity3.entityid);

                Assert.IsNotNull(child, "child should not be null");
                Assert.IsNotNull(related, "related should not be null");

                cOneToManyRelationship attribute1to2 = cOneToManyRelationshipObject.NewWithDerived(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, child));
                cOneToManyRelationship attribute1to3 = cOneToManyRelationshipObject.NewWithDerived(entity1.entityid, cOneToManyRelationshipObject.BasicOTM(entity1.entityid, related));
                cOneToManyRelationship attribute2to3 = cOneToManyRelationshipObject.NewWithDerived(child.entityid, cOneToManyRelationshipObject.BasicOTM(child.entityid, related));

                Assert.IsNotNull(attribute1to2, "attribute1to2 should not be null");
                Assert.IsNotNull(attribute1to3, "attribute1to3 should not be null");
                Assert.IsNotNull(attribute2to3, "attribute2to3 should not be null");
                Assert.IsTrue(attribute1to2.attributeid > 0, "attribute1to2.attributeid should be greater than 0");
                Assert.IsTrue(attribute1to3.attributeid > 0, "attribute1to3.attributeid should be greater than 0");
                Assert.IsTrue(attribute2to3.attributeid > 0, "attribute2to3.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(entity1.entityid);
                child = clsCustomEntities.getEntityById(child.entityid);
                related = clsCustomEntities.getEntityById(related.entityid);

                int firstAttributeID = (from x in related.attributes select x.Value.attributeid).FirstOrDefault();

                cSummaryAttributeColumn column = cSummaryAttributeColumnObject.Template(columnAttributeID: firstAttributeID);
                cSummaryAttributeElement element1 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute1to3.attributeid, order: 0);
                cSummaryAttributeElement element2 = cSummaryAttributeElementObject.Template(summaryAttributeID: 0, attributeID: 0, otmAttributeID: attribute2to3.attributeid, order: 1);

                returnVal = clsCustomEntities.saveSummaryAttribute(entity1.entityid, cSummaryAttributeObject.Template(summaryColumns: new Dictionary<int, cSummaryAttributeColumn> { { firstAttributeID, column } }, summaryElements: new Dictionary<int, cSummaryAttributeElement> { { 0, element1 }, { 1, element2 } }, sourceEntityID: related.entityid));

                // Should be the new attribute id
                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(currentUser);
                parent = clsCustomEntities.getEntityById(parent.entityid);
                cSummaryAttribute summaryAttribute = (cSummaryAttribute)parent.getAttributeById(returnVal);

                Assert.IsNotNull(summaryAttribute, "summaryAttribute should not be null");
                Assert.AreEqual(1, summaryAttribute.SummaryColumns.Count);
                Assert.AreEqual(2, summaryAttribute.SummaryElements.Count);

                // we'll need the system view entity itself to delete from
                // but the actual delete happens on the related entity's record
                // so we need a parent record after that and then a related entity record with that parent
                cCustomEntity systemViewEntity = clsCustomEntities.getSystemViewEntity(related.entityid, parent.entityid);

                Dictionary<int, object> recordValues;
                int recordID1 = CustomEntityRecord.New(parent, out recordValues);

                Assert.IsTrue(recordID1 > 0);
                Assert.AreEqual(4, recordValues.Count);

                int recordID2 = CustomEntityRecord.New(related, out recordValues, attribute1to3.attributeid, recordID1);

                Assert.IsTrue(recordID2 > 0);
                Assert.AreEqual(5, recordValues.Count);

                // to test, we call the delete of the related entity record id but through the system view entity
                // as if we were deleting it via a summary table link
                int success = clsCustomEntities.DeleteCustomEntityRecord(systemViewEntity, recordID2, systemViewEntity.Views.FirstOrDefault().Value.viewid);

                // when sucessfully deleted, passes back 0
                Assert.AreEqual(0, success);
            }
            finally
            {
                if (returnVal > 0) cSummaryAttributeObject.TearDown(returnVal);
                if (entity1 != null) cCustomEntityObject.TearDown(entity1.entityid);
                if (entity2 != null) cCustomEntityObject.TearDown(entity2.entityid);
                if (entity3 != null) cCustomEntityObject.TearDown(entity3.entityid);
            }
        }
        #endregion DeleteCustomEntityRecord

        #region getRelationshipAttributes
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void cCustomEntities_getRelationshipAttributes_NonExistentEntityID()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            try
            {
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                Dictionary<int, cOneToManyRelationship> otmRelationships = null;
                clsCustomEntities.getRelationshipAttributes(-1, -1, ref otmRelationships);
            }
            finally
            {

            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_getRelationshipAttributes_NullotmRelationshipsParam()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entityA = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getRelationshipAttributes test parent"));
            cCustomEntity entityB = null;
            cCustomEntity entityC = null;
            int entityID_A, entityID_B, entityID_C;

            try
            {
                entityB = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getRelationshipAttributes test child of entityA"));
                entityC = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getRelationshipAttributes test child of entityA and entityB"));

                Assert.IsNotNull(entityA, "entityA should not be null");
                Assert.IsNotNull(entityB, "entityB should not be null");
                Assert.IsNotNull(entityC, "entityC should not be null");

                Assert.IsTrue(entityA.entityid > 0, "entityA.entityid should be greater than 0");
                Assert.IsTrue(entityB.entityid > 0, "entityB.entityid should be greater than 0");
                Assert.IsTrue(entityC.entityid > 0, "entityC.entityid should be greater than 0");

                cCustomEntityForm formB = cCustomEntityFormObject.New(entityB.entityid, cCustomEntityFormObject.Basic(entityB));
                cCustomEntityView viewB = cCustomEntityViewObject.New(entityB.entityid);
                cCustomEntityForm formC = cCustomEntityFormObject.New(entityC.entityid, cCustomEntityFormObject.Basic(entityC));
                cCustomEntityView viewC = cCustomEntityViewObject.New(entityC.entityid);

                Assert.IsNotNull(formB, "formB should not be null");
                Assert.IsNotNull(viewB, "viewB should not be null");
                Assert.IsNotNull(formC, "formC should not be null");
                Assert.IsNotNull(viewC, "viewC should not be null");

                Assert.IsTrue(formB.formid > 0, "formB.formid should be greater than 0");
                Assert.IsTrue(viewB.viewid > 0, "viewB.viewid should be greater than 0");
                Assert.IsTrue(formC.formid > 0, "formC.formid should be greater than 0");
                Assert.IsTrue(viewC.viewid > 0, "viewC.viewid should be greater than 0");

                entityID_A = entityA.entityid;
                entityID_B = entityB.entityid;
                entityID_C = entityC.entityid;

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                entityA = clsCustomEntities.getEntityById(entityID_A);
                entityB = clsCustomEntities.getEntityById(entityID_B);
                entityC = clsCustomEntities.getEntityById(entityID_C);

                cOneToManyRelationship attributeAtoB = cOneToManyRelationshipObject.New(entityA.entityid, cOneToManyRelationshipObject.BasicOTM(entityB.entityid, entityB));
                cOneToManyRelationship attributeAtoC = cOneToManyRelationshipObject.New(entityA.entityid, cOneToManyRelationshipObject.BasicOTM(entityC.entityid, entityC));
                cOneToManyRelationship attributeBtoC = cOneToManyRelationshipObject.New(entityB.entityid, cOneToManyRelationshipObject.BasicOTM(entityC.entityid, entityC));

                Assert.IsNotNull(attributeAtoB, "attributeAtoB should not be null");
                Assert.IsNotNull(attributeAtoC, "attributeAtoC should not be null");
                Assert.IsNotNull(attributeBtoC, "attributeBtoC should not be null");

                Assert.IsTrue(attributeAtoB.attributeid > 0, "attributeAtoB.attributeid should be greater than 0");
                Assert.IsTrue(attributeAtoC.attributeid > 0, "attributeAtoC.attributeid should be greater than 0");
                Assert.IsTrue(attributeBtoC.attributeid > 0, "attributeBtoC.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                Dictionary<int, cOneToManyRelationship> otmRelationships = null;
                clsCustomEntities.getRelationshipAttributes(entityA.entityid, entityC.entityid, ref otmRelationships);

                Assert.AreEqual(2, otmRelationships.Count);
            }
            finally
            {
                if (entityA != null)
                {
                    cCustomEntityObject.TearDown(entityA.entityid);
                }

                if (entityB != null)
                {
                    cCustomEntityObject.TearDown(entityB.entityid);
                }

                if (entityC != null)
                {
                    cCustomEntityObject.TearDown(entityC.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_getRelationshipAttributes_MatchesFound()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entityA = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getRelationshipAttributes test parent"));
            cCustomEntity entityB = null;
            cCustomEntity entityC = null;
            int entityID_A, entityID_B, entityID_C;

            try
            {
                entityB = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getRelationshipAttributes test child of entityA"));
                entityC = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getRelationshipAttributes test child of entityA and entityB"));

                Assert.IsNotNull(entityA, "entityA should not be null");
                Assert.IsNotNull(entityB, "entityB should not be null");
                Assert.IsNotNull(entityC, "entityC should not be null");

                Assert.IsTrue(entityA.entityid > 0, "entityA.entityid should be greater than 0");
                Assert.IsTrue(entityB.entityid > 0, "entityB.entityid should be greater than 0");
                Assert.IsTrue(entityC.entityid > 0, "entityC.entityid should be greater than 0");

                cCustomEntityForm formB = cCustomEntityFormObject.New(entityB.entityid, cCustomEntityFormObject.Basic(entityB));
                cCustomEntityView viewB = cCustomEntityViewObject.New(entityB.entityid);
                cCustomEntityForm formC = cCustomEntityFormObject.New(entityC.entityid, cCustomEntityFormObject.Basic(entityC));
                cCustomEntityView viewC = cCustomEntityViewObject.New(entityC.entityid);

                Assert.IsNotNull(formB, "formB should not be null");
                Assert.IsNotNull(viewB, "viewB should not be null");
                Assert.IsNotNull(formC, "formC should not be null");
                Assert.IsNotNull(viewC, "viewC should not be null");

                Assert.IsTrue(formB.formid > 0, "formB.formid should be greater than 0");
                Assert.IsTrue(viewB.viewid > 0, "viewB.viewid should be greater than 0");
                Assert.IsTrue(formC.formid > 0, "formC.formid should be greater than 0");
                Assert.IsTrue(viewC.viewid > 0, "viewC.viewid should be greater than 0");

                entityID_A = entityA.entityid;
                entityID_B = entityB.entityid;
                entityID_C = entityC.entityid;

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                entityA = clsCustomEntities.getEntityById(entityID_A);
                entityB = clsCustomEntities.getEntityById(entityID_B);
                entityC = clsCustomEntities.getEntityById(entityID_C);

                cOneToManyRelationship attributeAtoB = cOneToManyRelationshipObject.New(entityA.entityid, cOneToManyRelationshipObject.BasicOTM(entityB.entityid, entityB));
                cOneToManyRelationship attributeAtoC = cOneToManyRelationshipObject.New(entityA.entityid, cOneToManyRelationshipObject.BasicOTM(entityC.entityid, entityC));
                cOneToManyRelationship attributeBtoC = cOneToManyRelationshipObject.New(entityB.entityid, cOneToManyRelationshipObject.BasicOTM(entityC.entityid, entityC));

                Assert.IsNotNull(attributeAtoB, "attributeAtoB should not be null");
                Assert.IsNotNull(attributeAtoC, "attributeAtoC should not be null");
                Assert.IsNotNull(attributeBtoC, "attributeBtoC should not be null");

                Assert.IsTrue(attributeAtoB.attributeid > 0, "attributeAtoB.attributeid should be greater than 0");
                Assert.IsTrue(attributeAtoC.attributeid > 0, "attributeAtoC.attributeid should be greater than 0");
                Assert.IsTrue(attributeBtoC.attributeid > 0, "attributeBtoC.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);

                Dictionary<int, cOneToManyRelationship> otmRelationships = new Dictionary<int, cOneToManyRelationship>();
                clsCustomEntities.getRelationshipAttributes(entityA.entityid, entityC.entityid, ref otmRelationships);

                Assert.IsTrue(otmRelationships.Count == 2);
            }
            finally
            {
                if (entityA != null)
                {
                    cCustomEntityObject.TearDown(entityA.entityid);
                }

                if (entityB != null)
                {
                    cCustomEntityObject.TearDown(entityB.entityid);
                }

                if (entityC != null)
                {
                    cCustomEntityObject.TearDown(entityC.entityid);
                }
            }
        }
        #endregion

        #region generateForm

        /// <summary>
        ///A test for generateForm using a cNumber form attribute
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateForm_WithNumberFormAttribute()
        {
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID));

            try
            {
                cCustomEntityView reqView = cCustomEntityViewObject.New(reqCustomEntity.entityid, cCustomEntityViewObject.Template(entityID: reqCustomEntity.entityid));

                cCustomEntityForm reqForm = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.Basic(reqCustomEntity));

                cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                sEntityBreadCrumb breadcrumb = new sEntityBreadCrumb(reqCustomEntity.entityid, reqForm.formid, 0, reqForm.tabs.Values[0].tabid, reqView.viewid);

                List<sEntityBreadCrumb> lstBreadCrumb = new List<sEntityBreadCrumb>();
                lstBreadCrumb.Add(breadcrumb);

                Dictionary<int, List<string>> otmTableID;
                Dictionary<int, List<string>> summaryTableID;
                List<string> scriptCmds = new List<string>();

                otmTableID = new Dictionary<int, List<string>>();
                summaryTableID = new Dictionary<int, List<string>>();

                Panel pnlSection = reqCustomEntities.generateForm(reqCustomEntity, reqForm.formid, reqView.viewid, 0, reqForm.tabs.Values[0].tabid, lstBreadCrumb, ref otmTableID, ref summaryTableID, ref scriptCmds);

                Assert.AreEqual(typeof(AjaxControlToolkit.TabContainer), pnlSection.Controls[0].GetType());
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
        ///A test for generateForm using a cOneToManyRelationship form attribute
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateForm_WithOneToManyFormAttribute()
        {
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID));

            cCustomEntity reqSecondCustomEntity = null;

            try
            {
                reqSecondCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID));

                #region Setup required views and forms for the related custom entity

                cCustomEntityView reqRelatedView = cCustomEntityViewObject.New(reqSecondCustomEntity.entityid, cCustomEntityViewObject.Template(entityID: reqSecondCustomEntity.entityid));

                cCustomEntityForm reqRelatedForm = cCustomEntityFormObject.New(reqSecondCustomEntity.entityid, cCustomEntityFormObject.Template(entityID: reqSecondCustomEntity.entityid));

                #endregion

                #region New balls please
                cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);
                reqSecondCustomEntity = reqCustomEntities.getEntityById(reqSecondCustomEntity.entityid);
                #endregion

                cCustomEntityView reqView = cCustomEntityViewObject.New(reqCustomEntity.entityid, cCustomEntityViewObject.Template(entityID: reqCustomEntity.entityid));
                cCustomEntityForm reqForm = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.BasicWithOneToManyFormAttribute(reqCustomEntity, reqSecondCustomEntity));

                #region New balls please
                reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);
                reqSecondCustomEntity = reqCustomEntities.getEntityById(reqSecondCustomEntity.entityid);
                #endregion

                sEntityBreadCrumb breadcrumb = new sEntityBreadCrumb(reqCustomEntity.entityid, reqForm.formid, 0, reqForm.tabs.Values[0].tabid, reqView.viewid);

                List<sEntityBreadCrumb> lstBreadCrumb = new List<sEntityBreadCrumb>();
                lstBreadCrumb.Add(breadcrumb);

                Dictionary<int, List<string>> otmTableID;
                Dictionary<int, List<string>> summaryTableID;
                List<string> scriptCmds = new List<string>();

                otmTableID = new Dictionary<int, List<string>>();
                summaryTableID = new Dictionary<int, List<string>>();

                Panel pnlSection = reqCustomEntities.generateForm(reqCustomEntity, reqForm.formid, reqView.viewid, 0, reqForm.tabs.Values[0].tabid, lstBreadCrumb, ref otmTableID, ref summaryTableID, ref scriptCmds);

                Assert.AreEqual(typeof(AjaxControlToolkit.TabContainer), pnlSection.Controls[0].GetType());

                Assert.AreEqual("tab" + reqForm.tabs.Values[0].tabid.ToString(), pnlSection.Controls[0].Controls[0].ID.ToString());
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
                if (reqSecondCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqSecondCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for generateForm using multiple tabs
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_generateForm_WithMultipleTabs()
        {
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID));

            try
            {
                cCustomEntityView reqView = cCustomEntityViewObject.New(reqCustomEntity.entityid, cCustomEntityViewObject.Template(entityID: reqCustomEntity.entityid));

                cCustomEntityForm reqForm = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.Basic(reqCustomEntity));

                cCustomEntityFormTab reqSecondTab = cCustomEntityFormTabObject.Template(headerCaption: "This is the second tab", formID: reqForm.formid, order: 1);

                reqForm.tabs.Add(reqSecondTab.order, reqSecondTab);

                cCustomEntities reqCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                reqCustomEntities.saveForm(reqCustomEntity.entityid, reqForm);

                reqCustomEntity = reqCustomEntities.getEntityById(reqCustomEntity.entityid);

                sEntityBreadCrumb breadcrumb = new sEntityBreadCrumb(reqCustomEntity.entityid, reqForm.formid, 0, reqForm.tabs.Values[0].tabid, reqView.viewid);

                List<sEntityBreadCrumb> lstBreadCrumb = new List<sEntityBreadCrumb>();
                lstBreadCrumb.Add(breadcrumb);

                Dictionary<int, List<string>> otmTableID;
                Dictionary<int, List<string>> summaryTableID;
                List<string> scriptCmds = new List<string>();

                otmTableID = new Dictionary<int, List<string>>();
                summaryTableID = new Dictionary<int, List<string>>();

                Panel pnlSection = reqCustomEntities.generateForm(reqCustomEntity, reqForm.formid, reqView.viewid, 0, reqForm.tabs.Values[1].tabid, lstBreadCrumb, ref otmTableID, ref summaryTableID, ref scriptCmds);

                Assert.AreEqual(typeof(AjaxControlToolkit.TabContainer), pnlSection.Controls[0].GetType());

                Assert.AreEqual("tab" + reqForm.tabs.Values[1].tabid.ToString(), pnlSection.Controls[0].Controls[0].ID.ToString());
            }
            finally
            {
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        #endregion generateForm

        #region getSystemViewEntity

        /// <summary>
        ///A test for getSystemViewEntity where valid derived and parent ID's have been used
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getSystemViewEntity_UsingValidEntityIDs()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = cCustomEntityObject.New();
            cCustomEntity entity2 = null;
            cCustomEntity viewentity = null;

            try
            {
                entity2 = cCustomEntityObject.New();

                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid);

                Assert.AreEqual(5, entity1.attributes.Count);
                Assert.AreEqual(5, entity2.attributes.Count);

                cCustomEntities clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                cCustomEntity entity3 = clsCustomEntities.getEntityById(entity1.entityid);
                cCustomEntity entity4 = clsCustomEntities.getEntityById(entity2.entityid);

                cOneToManyRelationship attribute = cOneToManyRelationshipObject.New(entity3.entityid, cOneToManyRelationshipObject.BasicOTM(entity3.entityid, entity4));

                viewentity = new cCustomEntity(0, entity3.entityname + "_" + entity4.entityname, entity3.pluralname + "_" + entity4.pluralname, "System View description", DateTime.UtcNow, currentUser.EmployeeID, DateTime.UtcNow, currentUser.EmployeeID, new SortedList<int, cAttribute>(), new SortedList<int, cCustomEntityForm>(), new SortedList<int, cCustomEntityView>(), null, null, entity3.EnableAttachments, entity3.AudienceView, entity3.AllowMergeConfigAccess, true, entity4.entityid, entity3.entityid, entity3.EnableCurrencies, entity3.DefaultCurrencyID, entity3.EnablePopupWindow, entity3.DefaultPopupView, entity3.FormSelectionAttributeId, entity3.OwnerId, entity3.SupportContactId, entity3.SupportQuestion, entity3.EnableLocking, entity3.BuiltIn);

                int returnVal = clsCustomEntities.saveEntitySystemView(viewentity, entity3.entityid, attribute.attributeid, entity4);

                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                cCustomEntity entity5 = clsCustomEntities.getSystemViewEntity(entity4.entityid, entity3.entityid);

                Assert.IsNotNull(entity5);
                Assert.AreEqual("System View description", entity5.description);
                Assert.AreEqual(false, entity5.modifiedby.HasValue);
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
        ///A test for getSystemViewEntity where a valid derived entity ID and an invalid parent entity ID have been used
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getSystemViewEntity_UsingIncorrectParentEntityID()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = cCustomEntityObject.New();
            cCustomEntity entity2 = null;
            cCustomEntity viewentity = null;

            try
            {
                entity2 = cCustomEntityObject.New();

                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid);

                Assert.AreEqual(5, entity1.attributes.Count);
                Assert.AreEqual(5, entity2.attributes.Count);

                cCustomEntities clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                cCustomEntity entity3 = clsCustomEntities.getEntityById(entity1.entityid);
                cCustomEntity entity4 = clsCustomEntities.getEntityById(entity2.entityid);

                cOneToManyRelationship attribute = cOneToManyRelationshipObject.New(entity3.entityid, cOneToManyRelationshipObject.BasicOTM(entity3.entityid, entity4));

                viewentity = new cCustomEntity(0, entity3.entityname + "_" + entity4.entityname, entity3.pluralname + "_" + entity4.pluralname, "System View description", DateTime.UtcNow, currentUser.EmployeeID, DateTime.UtcNow, currentUser.EmployeeID, new SortedList<int, cAttribute>(), new SortedList<int, cCustomEntityForm>(), new SortedList<int, cCustomEntityView>(), null, null, entity3.EnableAttachments, entity3.AudienceView, entity3.AllowMergeConfigAccess, true, entity4.entityid, entity3.entityid, entity3.EnableCurrencies, entity3.DefaultCurrencyID, entity3.EnablePopupWindow, entity3.DefaultPopupView, entity3.FormSelectionAttributeId, entity3.OwnerId, entity3.SupportContactId, entity3.SupportQuestion, entity3.EnableLocking, entity3.BuiltIn);

                int returnVal = clsCustomEntities.saveEntitySystemView(viewentity, entity3.entityid, attribute.attributeid, entity4);

                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                cCustomEntity entity5 = clsCustomEntities.getSystemViewEntity(entity4.entityid, -69);

                Assert.IsNull(entity5);
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
        ///A test for getSystemViewEntity where an invalid derived entity ID and a valid parent entity ID have been used
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getSystemViewEntity_UsingIncorrectDerivedEntityID()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = cCustomEntityObject.New();
            cCustomEntity entity2 = null;
            cCustomEntity viewentity = null;

            try
            {
                entity2 = cCustomEntityObject.New();

                cCustomEntityForm form = cCustomEntityFormObject.New(entity2.entityid, cCustomEntityFormObject.Basic(entity2));
                cCustomEntityView view = cCustomEntityViewObject.New(entity2.entityid);

                Assert.AreEqual(5, entity1.attributes.Count);
                Assert.AreEqual(5, entity2.attributes.Count);

                cCustomEntities clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                cCustomEntity entity3 = clsCustomEntities.getEntityById(entity1.entityid);
                cCustomEntity entity4 = clsCustomEntities.getEntityById(entity2.entityid);

                cOneToManyRelationship attribute = cOneToManyRelationshipObject.New(entity3.entityid, cOneToManyRelationshipObject.BasicOTM(entity3.entityid, entity4));

                viewentity = new cCustomEntity(0, entity3.entityname + "_" + entity4.entityname, entity3.pluralname + "_" + entity4.pluralname, "System View description", DateTime.UtcNow, currentUser.EmployeeID, DateTime.UtcNow, currentUser.EmployeeID, new SortedList<int, cAttribute>(), new SortedList<int, cCustomEntityForm>(), new SortedList<int, cCustomEntityView>(), null, null, entity3.EnableAttachments, entity3.AudienceView, entity3.AllowMergeConfigAccess, true, entity4.entityid, entity3.entityid, entity3.EnableCurrencies, entity3.DefaultCurrencyID, entity3.EnablePopupWindow, entity3.DefaultPopupView, entity3.FormSelectionAttributeId, entity3.OwnerId, entity3.SupportContactId, entity3.SupportQuestion, entity3.EnableLocking, entity3.BuiltIn);

                int returnVal = clsCustomEntities.saveEntitySystemView(viewentity, entity3.entityid, attribute.attributeid, entity4);

                Assert.IsTrue(returnVal > 0);

                clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());
                cCustomEntity entity5 = clsCustomEntities.getSystemViewEntity(-69, entity3.entityid);

                Assert.IsNull(entity5);
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

        #endregion getSystemViewEntity

        #region getParentBreadcrumb
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getParentBreadcrumb_WithInvalidEntityID()
        {
            ICurrentUser currentuser = Moqs.CurrentUser();
            cCustomEntity entityA = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getParentBreadcrumb test entity A"));
            cCustomEntity entityB = null;
            cCustomEntity entityC = null;
            int entityID_A, entityID_B, entityID_C;

            try
            {
                entityB = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getParentBreadcrumb test child of entityA"));
                entityC = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getParentBreadcrumb test child of entityA and entityB"));

                entityID_A = entityA.entityid;
                entityID_B = entityB.entityid;
                entityID_C = entityC.entityid;

                Assert.IsNotNull(entityA, "entityA should not be null");
                Assert.IsNotNull(entityB, "entityB should not be null");
                Assert.IsNotNull(entityC, "entityC should not be null");

                Assert.IsTrue(entityA.entityid > 0, "entityA.entityid should be greater than 0");
                Assert.IsTrue(entityB.entityid > 0, "entityB.entityid should be greater than 0");
                Assert.IsTrue(entityC.entityid > 0, "entityC.entityid should be greater than 0");

                cCustomEntityForm formA = cCustomEntityFormObject.New(entityA.entityid, cCustomEntityFormObject.Basic(entityA));
                cCustomEntityView viewA = cCustomEntityViewObject.New(entityA.entityid);
                cCustomEntityForm formB = cCustomEntityFormObject.New(entityB.entityid, cCustomEntityFormObject.Basic(entityB));
                cCustomEntityView viewB = cCustomEntityViewObject.New(entityB.entityid);
                cCustomEntityForm formC = cCustomEntityFormObject.New(entityC.entityid, cCustomEntityFormObject.Basic(entityC));
                cCustomEntityView viewC = cCustomEntityViewObject.New(entityC.entityid);

                Assert.IsNotNull(formA, "formA should not be null");
                Assert.IsNotNull(viewA, "viewA should not be null");
                Assert.IsNotNull(formB, "formB should not be null");
                Assert.IsNotNull(viewB, "viewB should not be null");
                Assert.IsNotNull(formC, "formC should not be null");
                Assert.IsNotNull(viewC, "viewC should not be null");

                Assert.IsTrue(formA.formid > 0, "formA.formid should be greater than 0");
                Assert.IsTrue(viewA.viewid > 0, "viewA.viewid should be greater than 0");
                Assert.IsTrue(formB.formid > 0, "formB.formid should be greater than 0");
                Assert.IsTrue(viewB.viewid > 0, "viewB.viewid should be greater than 0");
                Assert.IsTrue(formC.formid > 0, "formC.formid should be greater than 0");
                Assert.IsTrue(viewC.viewid > 0, "viewC.viewid should be greater than 0");

                cCustomEntities clsCustomEntities = new cCustomEntities(currentuser);
                entityA = clsCustomEntities.getEntityById(entityID_A);
                entityB = clsCustomEntities.getEntityById(entityID_B);
                entityC = clsCustomEntities.getEntityById(entityID_C);

                // Create Tabs for the forms and sections
                cCustomEntityFormTab formAtab = formA.tabs.Values[0];
                cCustomEntityFormTab formBtab = formB.tabs.Values[0];
                cCustomEntityFormTab formCtab = formC.tabs.Values[0];

                // add a second tab onto formC for the A to C OTM attribute
                cCustomEntityFormTab formAtab2 = cCustomEntityFormTabObject.Template(formID: formA.formid, headerCaption: "formAtab2 tab", order: (byte)1);
                formA.tabs.Add(formAtab2.tabid, formAtab2);

                cCustomEntityFormSection formAsection2 = cCustomEntityFormSectionObject.Template(formID: formA.formid, headerCaption: "formAtab2 section 2", tab: formAtab2, order: (byte)1);
                formA.sections.Add(formAsection2.sectionid, formAsection2);
                clsCustomEntities.saveForm(entityA.entityid, formA);

                clsCustomEntities = new cCustomEntities(currentuser);
                entityA = clsCustomEntities.getEntityById(entityID_A);
                entityB = clsCustomEntities.getEntityById(entityID_B);
                entityC = clsCustomEntities.getEntityById(entityID_C);

                formA = entityA.Forms.Values[0];
                formB = entityB.Forms.Values[0];
                formC = entityC.Forms.Values[0];

                // Create Tabs for the forms and sections
                formAtab = formA.tabs.Values[1]; // These formA tabs are opposite way around because of the save routine!
                formBtab = formB.tabs.Values[0];
                formCtab = formC.tabs.Values[0];
                formAtab2 = formA.tabs.Values[0];

                Assert.IsNotNull(formAtab, "formAtab should not be null");
                Assert.IsNotNull(formBtab, "formBtab should not be null");
                Assert.IsNotNull(formCtab, "formCtab should not be null");
                Assert.IsNotNull(formAtab2, "formCtab2 should not be null");

                Assert.IsTrue(formAtab.tabid > 0, "formAtab.tabid should be greater than 0");
                Assert.IsTrue(formBtab.tabid > 0, "formBtab.tabid should be greater than 0");
                Assert.IsTrue(formCtab.tabid > 0, "formCtab.tabid should be greater than 0");
                Assert.IsTrue(formAtab2.tabid > 0, "formAtab2.tabid should be greater than 0");

                cCustomEntityFormSection formAsection = formAtab.sections[0];
                formAsection2 = formAtab2.sections[0];
                cCustomEntityFormSection formBsection = formBtab.sections[0];
                cCustomEntityFormSection formCsection = formCtab.sections[0];

                Assert.IsNotNull(formAsection, "formAsection should not be null");
                Assert.IsNotNull(formAsection2, "formAsection2 should not be null");
                Assert.IsNotNull(formBsection, "formBsection should not be null");
                Assert.IsNotNull(formCsection, "formCsection should not be null");

                Assert.IsTrue(formAsection.sectionid > 0, "formAsection.sectionid should be greater than 0");
                Assert.IsTrue(formAsection2.sectionid > 0, "formAsection2.sectionid should be greater than 0");
                Assert.IsTrue(formBsection.sectionid > 0, "formBsection.sectionid should be greater than 0");
                Assert.IsTrue(formCsection.sectionid > 0, "formCsection.sectionid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentuser);
                entityA = clsCustomEntities.getEntityById(entityID_A);
                entityB = clsCustomEntities.getEntityById(entityID_B);
                entityC = clsCustomEntities.getEntityById(entityID_C);

                cOneToManyRelationship attributeAtoB = cOneToManyRelationshipObject.New(entityA.entityid, cOneToManyRelationshipObject.BasicOTM(entityB.entityid, entityB));
                cOneToManyRelationship attributeAtoC = cOneToManyRelationshipObject.New(entityA.entityid, cOneToManyRelationshipObject.BasicOTM(entityC.entityid, entityC));
                cOneToManyRelationship attributeBtoC = cOneToManyRelationshipObject.New(entityB.entityid, cOneToManyRelationshipObject.BasicOTM(entityC.entityid, entityC));

                Assert.IsNotNull(attributeAtoB, "attributeAtoB should not be null");
                Assert.IsNotNull(attributeAtoC, "attributeAtoC should not be null");
                Assert.IsNotNull(attributeBtoC, "attributeBtoC should not be null");

                Assert.IsTrue(attributeAtoB.attributeid > 0, "attributeAtoB.attributeid should be greater than 0");
                Assert.IsTrue(attributeAtoC.attributeid > 0, "attributeAtoC.attributeid should be greater than 0");
                Assert.IsTrue(attributeBtoC.attributeid > 0, "attributeBtoC.attributeid should be greater than 0");

                // Add OTM attributes to the forms
                cCustomEntityFormField formA_OTMField1 = cCustomEntityFormFieldObject.Template(formID: formA.formid, attribute: attributeAtoB, section: formAsection);
                cCustomEntityFormField formA_OTMField2 = cCustomEntityFormFieldObject.Template(formID: formA.formid, attribute: attributeAtoC, section: formAsection2);
                cCustomEntityFormField formB_OTMField = cCustomEntityFormFieldObject.Template(formID: formB.formid, attribute: attributeBtoC, section: formBsection);
                cCustomEntityFormField formC_TxtField = cCustomEntityFormFieldObject.Template(formID: formC.formid, attribute: cTextAttributeObject.BasicSingleLine(), section: formCsection);

                Assert.IsNotNull(formA_OTMField1, "formA_OTMField1 should not be null");
                Assert.IsNotNull(formA_OTMField2, "formA_OTMField2 should not be null");
                Assert.IsNotNull(formB_OTMField, "formB_OTMField should not be null");
                Assert.IsNotNull(formC_TxtField, "formC_TxtField should not be null");

                formA.fields.Add(formA_OTMField1.attribute.attributeid, formA_OTMField1);
                formA.fields.Add(formA_OTMField2.attribute.attributeid, formA_OTMField2);
                formB.fields.Add(formB_OTMField.attribute.attributeid, formB_OTMField);
                formC.fields.Add(formC_TxtField.attribute.attributeid, formC_TxtField);

                clsCustomEntities.saveForm(entityA.entityid, formA);
                clsCustomEntities.saveForm(entityB.entityid, formB);
                clsCustomEntities.saveForm(entityC.entityid, formC);

                Dictionary<int, object> entityA_rec = new Dictionary<int, object>();
                Dictionary<int, object> entityBA_rec = new Dictionary<int, object>();
                Dictionary<int, object> entityCA_rec = new Dictionary<int, object>();
                Dictionary<int, object> entityCB_rec = new Dictionary<int, object>();

                int entityA_recID, entityB_recID, entityCA_recID, entityCB_recID;

                // create rec in entity A
                entityA_recID = CustomEntityRecord.New(entityA, out entityA_rec);
                Assert.IsTrue(entityA_recID > 0, "entityA record ID should be greater than 0");

                // create child rec in entityB with OTM from entity A
                entityB_recID = CustomEntityRecord.New(entityB, out entityBA_rec, attributeAtoB.attributeid, entityA_recID);
                Assert.IsTrue(entityB_recID > 0, "entityB record ID should be greater than 0");

                // create child rec in entityC with OTM from entity A
                entityCA_recID = CustomEntityRecord.New(entityC, out entityCA_rec, attributeAtoC.attributeid, entityA_recID);
                Assert.IsTrue(entityCA_recID > 0, "entityCA record ID should be greater than 0");

                // create child rec in entityC with OTM from entity B
                entityCB_recID = CustomEntityRecord.New(entityC, out entityCB_rec, attributeBtoC.attributeid, entityB_recID);
                Assert.IsTrue(entityCB_recID > 0, "entityCB record ID should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentuser);
                sEntityBreadCrumb topCrumb = new sEntityBreadCrumb(entityA.entityid, formA.formid, entityA_recID, formAtab.tabid, 0);

                sEntityBreadCrumb parentBreadcrumbAC = clsCustomEntities.getParentBreadcrumb(-1, topCrumb, entityCA_recID);
                Assert.IsNotNull(parentBreadcrumbAC);
                Assert.AreEqual(0, parentBreadcrumbAC.EntityID);
                Assert.AreEqual(0, parentBreadcrumbAC.FormID);
                Assert.AreEqual(0, parentBreadcrumbAC.RecordID);
                Assert.AreEqual(0, parentBreadcrumbAC.TabID);
            }
            finally
            {
                cCustomEntityObject.TearDown(entityA.entityid);

                if (entityB != null)
                    cCustomEntityObject.TearDown(entityB.entityid);

                if (entityC != null)
                    cCustomEntityObject.TearDown(entityC.entityid);
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getParentBreadcrumb_Valid()
        {
            ICurrentUser currentuser = Moqs.CurrentUser();
            cCustomEntity entityA = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getParentBreadcrumb test entity A"));
            cCustomEntity entityB = null;
            cCustomEntity entityC = null;
            int entityID_A, entityID_B, entityID_C;

            try
            {
                entityB = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getParentBreadcrumb test child of entityA"));
                entityC = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getParentBreadcrumb test child of entityA and entityB"));

                entityID_A = entityA.entityid;
                entityID_B = entityB.entityid;
                entityID_C = entityC.entityid;

                Assert.IsNotNull(entityA, "entityA should not be null");
                Assert.IsNotNull(entityB, "entityB should not be null");
                Assert.IsNotNull(entityC, "entityC should not be null");

                Assert.IsTrue(entityA.entityid > 0, "entityA.entityid should be greater than 0");
                Assert.IsTrue(entityB.entityid > 0, "entityB.entityid should be greater than 0");
                Assert.IsTrue(entityC.entityid > 0, "entityC.entityid should be greater than 0");

                cCustomEntityForm formA = cCustomEntityFormObject.New(entityA.entityid, cCustomEntityFormObject.Basic(entityA));
                cCustomEntityView viewA = cCustomEntityViewObject.New(entityA.entityid);
                cCustomEntityForm formB = cCustomEntityFormObject.New(entityB.entityid, cCustomEntityFormObject.Basic(entityB));
                cCustomEntityView viewB = cCustomEntityViewObject.New(entityB.entityid);
                cCustomEntityForm formC = cCustomEntityFormObject.New(entityC.entityid, cCustomEntityFormObject.Basic(entityC));
                cCustomEntityView viewC = cCustomEntityViewObject.New(entityC.entityid);

                Assert.IsNotNull(formA, "formA should not be null");
                Assert.IsNotNull(viewA, "viewA should not be null");
                Assert.IsNotNull(formB, "formB should not be null");
                Assert.IsNotNull(viewB, "viewB should not be null");
                Assert.IsNotNull(formC, "formC should not be null");
                Assert.IsNotNull(viewC, "viewC should not be null");

                Assert.IsTrue(formA.formid > 0, "formA.formid should be greater than 0");
                Assert.IsTrue(viewA.viewid > 0, "viewA.viewid should be greater than 0");
                Assert.IsTrue(formB.formid > 0, "formB.formid should be greater than 0");
                Assert.IsTrue(viewB.viewid > 0, "viewB.viewid should be greater than 0");
                Assert.IsTrue(formC.formid > 0, "formC.formid should be greater than 0");
                Assert.IsTrue(viewC.viewid > 0, "viewC.viewid should be greater than 0");

                cCustomEntities clsCustomEntities = new cCustomEntities(currentuser);
                entityA = clsCustomEntities.getEntityById(entityID_A);
                entityB = clsCustomEntities.getEntityById(entityID_B);
                entityC = clsCustomEntities.getEntityById(entityID_C);

                // Create Tabs for the forms and sections
                cCustomEntityFormTab formAtab = formA.tabs.Values[0];
                cCustomEntityFormTab formBtab = formB.tabs.Values[0];
                cCustomEntityFormTab formCtab = formC.tabs.Values[0];

                // add a second tab onto formC for the A to C OTM attribute
                cCustomEntityFormTab formAtab2 = cCustomEntityFormTabObject.Template(formID: formA.formid, headerCaption: "formAtab2 tab", order: (byte)1);
                formA.tabs.Add(formAtab2.tabid, formAtab2);

                cCustomEntityFormSection formAsection2 = cCustomEntityFormSectionObject.Template(formID: formA.formid, headerCaption: "formAtab2 section 2", tab: formAtab2, order: (byte)1);
                formA.sections.Add(formAsection2.sectionid, formAsection2);
                clsCustomEntities.saveForm(entityA.entityid, formA);

                clsCustomEntities = new cCustomEntities(currentuser);
                entityA = clsCustomEntities.getEntityById(entityID_A);
                entityB = clsCustomEntities.getEntityById(entityID_B);
                entityC = clsCustomEntities.getEntityById(entityID_C);

                formA = entityA.Forms.Values[0];
                formB = entityB.Forms.Values[0];
                formC = entityC.Forms.Values[0];

                // Create Tabs for the forms and sections
                formAtab = formA.tabs.Values[1]; // These formA tabs are opposite way around because of the save routine!
                formBtab = formB.tabs.Values[0];
                formCtab = formC.tabs.Values[0];
                formAtab2 = formA.tabs.Values[0];

                Assert.IsNotNull(formAtab, "formAtab should not be null");
                Assert.IsNotNull(formBtab, "formBtab should not be null");
                Assert.IsNotNull(formCtab, "formCtab should not be null");
                Assert.IsNotNull(formAtab2, "formCtab2 should not be null");

                Assert.IsTrue(formAtab.tabid > 0, "formAtab.tabid should be greater than 0");
                Assert.IsTrue(formBtab.tabid > 0, "formBtab.tabid should be greater than 0");
                Assert.IsTrue(formCtab.tabid > 0, "formCtab.tabid should be greater than 0");
                Assert.IsTrue(formAtab2.tabid > 0, "formAtab2.tabid should be greater than 0");

                cCustomEntityFormSection formAsection = formAtab.sections[0];
                formAsection2 = formAtab2.sections[0];
                cCustomEntityFormSection formBsection = formBtab.sections[0];
                cCustomEntityFormSection formCsection = formCtab.sections[0];

                Assert.IsNotNull(formAsection, "formAsection should not be null");
                Assert.IsNotNull(formAsection2, "formAsection2 should not be null");
                Assert.IsNotNull(formBsection, "formBsection should not be null");
                Assert.IsNotNull(formCsection, "formCsection should not be null");

                Assert.IsTrue(formAsection.sectionid > 0, "formAsection.sectionid should be greater than 0");
                Assert.IsTrue(formAsection2.sectionid > 0, "formAsection2.sectionid should be greater than 0");
                Assert.IsTrue(formBsection.sectionid > 0, "formBsection.sectionid should be greater than 0");
                Assert.IsTrue(formCsection.sectionid > 0, "formCsection.sectionid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentuser);
                entityA = clsCustomEntities.getEntityById(entityID_A);
                entityB = clsCustomEntities.getEntityById(entityID_B);
                entityC = clsCustomEntities.getEntityById(entityID_C);

                cOneToManyRelationship attributeAtoB = cOneToManyRelationshipObject.New(entityA.entityid, cOneToManyRelationshipObject.BasicOTM(entityB.entityid, entityB));
                cOneToManyRelationship attributeAtoC = cOneToManyRelationshipObject.New(entityA.entityid, cOneToManyRelationshipObject.BasicOTM(entityC.entityid, entityC));
                cOneToManyRelationship attributeBtoC = cOneToManyRelationshipObject.New(entityB.entityid, cOneToManyRelationshipObject.BasicOTM(entityC.entityid, entityC));

                Assert.IsNotNull(attributeAtoB, "attributeAtoB should not be null");
                Assert.IsNotNull(attributeAtoC, "attributeAtoC should not be null");
                Assert.IsNotNull(attributeBtoC, "attributeBtoC should not be null");

                Assert.IsTrue(attributeAtoB.attributeid > 0, "attributeAtoB.attributeid should be greater than 0");
                Assert.IsTrue(attributeAtoC.attributeid > 0, "attributeAtoC.attributeid should be greater than 0");
                Assert.IsTrue(attributeBtoC.attributeid > 0, "attributeBtoC.attributeid should be greater than 0");

                // Add OTM attributes to the forms
                cCustomEntityFormField formA_OTMField1 = cCustomEntityFormFieldObject.Template(formID: formA.formid, attribute: attributeAtoB, section: formAsection);
                cCustomEntityFormField formA_OTMField2 = cCustomEntityFormFieldObject.Template(formID: formA.formid, attribute: attributeAtoC, section: formAsection2);
                cCustomEntityFormField formB_OTMField = cCustomEntityFormFieldObject.Template(formID: formB.formid, attribute: attributeBtoC, section: formBsection);
                cCustomEntityFormField formC_TxtField = cCustomEntityFormFieldObject.Template(formID: formC.formid, attribute: cTextAttributeObject.BasicSingleLine(), section: formCsection);

                Assert.IsNotNull(formA_OTMField1, "formA_OTMField1 should not be null");
                Assert.IsNotNull(formA_OTMField2, "formA_OTMField2 should not be null");
                Assert.IsNotNull(formB_OTMField, "formB_OTMField should not be null");
                Assert.IsNotNull(formC_TxtField, "formC_TxtField should not be null");

                formA.fields.Add(formA_OTMField1.attribute.attributeid, formA_OTMField1);
                formA.fields.Add(formA_OTMField2.attribute.attributeid, formA_OTMField2);
                formB.fields.Add(formB_OTMField.attribute.attributeid, formB_OTMField);
                formC.fields.Add(formC_TxtField.attribute.attributeid, formC_TxtField);

                clsCustomEntities.saveForm(entityA.entityid, formA);
                clsCustomEntities.saveForm(entityB.entityid, formB);
                clsCustomEntities.saveForm(entityC.entityid, formC);

                Dictionary<int, object> entityA_rec = new Dictionary<int, object>();
                Dictionary<int, object> entityBA_rec = new Dictionary<int, object>();
                Dictionary<int, object> entityCA_rec = new Dictionary<int, object>();
                Dictionary<int, object> entityCB_rec = new Dictionary<int, object>();

                int entityA_recID, entityB_recID, entityCA_recID, entityCB_recID;

                // create rec in entity A
                entityA_recID = CustomEntityRecord.New(entityA, out entityA_rec);
                Assert.IsTrue(entityA_recID > 0, "entityA record ID should be greater than 0");

                // create child rec in entityB with OTM from entity A
                entityB_recID = CustomEntityRecord.New(entityB, out entityBA_rec, attributeAtoB.attributeid, entityA_recID);
                Assert.IsTrue(entityB_recID > 0, "entityB record ID should be greater than 0");

                // create child rec in entityC with OTM from entity A
                entityCA_recID = CustomEntityRecord.New(entityC, out entityCA_rec, attributeAtoC.attributeid, entityA_recID);
                Assert.IsTrue(entityCA_recID > 0, "entityCA record ID should be greater than 0");

                // create child rec in entityC with OTM from entity B
                entityCB_recID = CustomEntityRecord.New(entityC, out entityCB_rec, attributeBtoC.attributeid, entityB_recID);
                Assert.IsTrue(entityCB_recID > 0, "entityCB record ID should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentuser);
                sEntityBreadCrumb topCrumb = new sEntityBreadCrumb(entityA.entityid, formA.formid, entityA_recID, formAtab.tabid, 0);

                sEntityBreadCrumb parentBreadcrumbAC = clsCustomEntities.getParentBreadcrumb(entityC.entityid, topCrumb, entityCA_recID);
                Assert.IsNotNull(parentBreadcrumbAC);
                Assert.AreEqual(parentBreadcrumbAC.EntityID, entityA.entityid);

                sEntityBreadCrumb parentBreadcrumbABC = clsCustomEntities.getParentBreadcrumb(entityC.entityid, topCrumb, entityCB_recID);
                Assert.IsNotNull(parentBreadcrumbABC);
                Assert.AreEqual(parentBreadcrumbABC.EntityID, entityB.entityid);

                sEntityBreadCrumb parentBreadcrumbBA = clsCustomEntities.getParentBreadcrumb(entityB.entityid, topCrumb, entityB_recID);
                Assert.IsNotNull(parentBreadcrumbBA);
                Assert.AreEqual(parentBreadcrumbBA.EntityID, entityA.entityid);
            }
            finally
            {
                cCustomEntityObject.TearDown(entityA.entityid);

                if (entityB != null)
                    cCustomEntityObject.TearDown(entityB.entityid);

                if (entityC != null)
                    cCustomEntityObject.TearDown(entityC.entityid);
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        [ExpectedException(typeof(NullReferenceException))]
        public void cCustomEntities_getParentBreadcrumb_WithEmptyTopCrumb()
        {
            ICurrentUser currentuser = Moqs.CurrentUser();
            cCustomEntity entityA = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getParentBreadcrumb test entity A"));
            cCustomEntity entityB = null;

            try
            {
                sEntityBreadCrumb topCrumb = new sEntityBreadCrumb();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentuser);
                sEntityBreadCrumb parentBreadcrumbBA = clsCustomEntities.getParentBreadcrumb(entityB.entityid, topCrumb, 0);
            }
            finally
            {
                if (entityA != null)
                    cCustomEntityObject.TearDown(entityA.entityid);
            }

        }
        #endregion

        #region CacheList

        /// <summary>
        ///A test for CacheList. 
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod]
        public void cCustomEntities_CacheList_CountAndItemsTest()
        {
            cCustomEntity reqCustomEntity1 = null;
            cCustomEntity reqCustomEntity2 = null;
            cCustomEntity reqCustomEntity3 = null;

            try
            {
                reqCustomEntity1 = cCustomEntityObject.New(cCustomEntityObject.Template(entityName: "unit test ultra unique name 1", pluralName: "unit test plural 1", description: "unit test desc 1", createdBy: Moqs.CurrentUser().EmployeeID));
                reqCustomEntity2 = cCustomEntityObject.New(cCustomEntityObject.Template(entityName: "unit test ultra unique name 2", pluralName: "unit test plural 2", description: "unit test desc 2", createdBy: Moqs.CurrentUser().EmployeeID));
                reqCustomEntity3 = cCustomEntityObject.New(cCustomEntityObject.Template(entityName: "unit test ultra unique name 3", pluralName: "unit test plural 3", description: "unit test desc 3", createdBy: Moqs.CurrentUser().EmployeeID));

                cCustomEntities_Accessor target = new cCustomEntities_Accessor();

                target.nAccountid = Moqs.CurrentUser().AccountID;
                target.oCurrentUser = Moqs.CurrentUser();
                //target.expdata = new DatabaseConnection(cAccounts.getConnectionString(Moqs.CurrentUser().AccountID));
                target.tables = new cTables(Moqs.CurrentUser().AccountID);
                target.fields = new cFields(Moqs.CurrentUser().AccountID);
                SortedList<int, cCustomEntity> actual;
                actual = target.CacheList();

                Assert.IsTrue(actual.Count >= 3);
                Assert.IsNotNull(actual.Values.Where(x => (x.entityid == reqCustomEntity1.entityid && x.entityname == reqCustomEntity1.entityname)));
                Assert.IsNotNull(actual.Values.Where(x => (x.entityid == reqCustomEntity2.entityid && x.entityname == reqCustomEntity2.entityname)));
                Assert.IsNotNull(actual.Values.Where(x => (x.entityid == reqCustomEntity3.entityid && x.entityname == reqCustomEntity3.entityname)));
            }
            catch (Exception e)
            {
                Assert.Fail("Setup of db for custom entities failed. " + e.Message);
            }
            finally
            {
                if (reqCustomEntity1 != null) { cCustomEntityObject.TearDown(reqCustomEntity1.entityid); }
                if (reqCustomEntity2 != null) { cCustomEntityObject.TearDown(reqCustomEntity2.entityid); }
                if (reqCustomEntity3 != null) { cCustomEntityObject.TearDown(reqCustomEntity3.entityid); }
            }
        }

        #endregion CacheList

        #region getEntityRecord

        /// <summary>
        ///A test for getEntityRecord using a valid entity record
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getEntityRecord_UsingAValidEntityRecord()
        {
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                #region Create a new Form with a tab, section and form field

                cNumberAttribute attribute = cNumberAttributeObject.New(reqCustomEntity.entityid);

                cCustomEntityFormTab tab = cCustomEntityFormTabObject.Template();
                cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template(tab: tab);
                cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: attribute, section: section);

                SortedList<int, cCustomEntityFormTab> lstTabs = new SortedList<int, cCustomEntityFormTab>() { { tab.order, tab } };
                SortedList<int, cCustomEntityFormSection> lstSections = new SortedList<int, cCustomEntityFormSection>() { { section.sectionid, section } };
                SortedList<int, cCustomEntityFormField> lstFields = new SortedList<int, cCustomEntityFormField>() { { field.attribute.attributeid, field } };

                cCustomEntityForm reqForm = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.Template(tabs: lstTabs, sections: lstSections, fields: lstFields));

                customEntities = new cCustomEntities(Moqs.CurrentUser());

                customEntities.saveForm(reqCustomEntity.entityid, reqForm);

                #endregion

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                Dictionary<int, object> entityRecords = new Dictionary<int, object>();

                //Create a new record for reqCustomEntity, using the attribute created above
                int nRecordID = CustomEntityRecord.New(reqCustomEntity, out entityRecords, parameterList: new SortedList<int, object> { { attribute.attributeid, 69 } });

                customEntities = new cCustomEntities(Moqs.CurrentUser());

                SortedList<int, object> lstReturnedVals = customEntities.getEntityRecord(reqCustomEntity, nRecordID, reqCustomEntity.Forms.Values[0]);

                Assert.AreEqual(typeof(System.Decimal), lstReturnedVals.Values[0].GetType());
                Assert.AreEqual((System.Decimal)69, lstReturnedVals[attribute.attributeid]);
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
        ///A test for getEntityRecord using a valid entity record, which contains multiple form attributes
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getEntityRecord_UsingAValidEntityRecordWithMultipleAttributes()
        {
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                #region Create a new Form with a tab, section and mutliple form fields

                cNumberAttribute numberAttribute = cNumberAttributeObject.New(reqCustomEntity.entityid);
                cDateTimeAttribute dateTimeAttribute = cDateTimeAttributeObject.New(reqCustomEntity.entityid, cDateTimeAttributeObject.BasicDateOnly());
                cTickboxAttribute tickBoxAttribute = cTickboxAttributeObject.New(reqCustomEntity.entityid, cTickboxAttributeObject.Template(defaultValue: "Unit test default value", allowDelete: true, allowEdit: true));

                cCustomEntityFormTab tab = cCustomEntityFormTabObject.Template();
                cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template(tab: tab);
                cCustomEntityFormField numberField = cCustomEntityFormFieldObject.Template(attribute: numberAttribute, section: section);
                cCustomEntityFormField dateTimeField = cCustomEntityFormFieldObject.Template(attribute: dateTimeAttribute, section: section);
                cCustomEntityFormField tickBoxField = cCustomEntityFormFieldObject.Template(attribute: tickBoxAttribute, section: section);

                SortedList<int, cCustomEntityFormTab> lstTabs = new SortedList<int, cCustomEntityFormTab>() { { tab.order, tab } };
                SortedList<int, cCustomEntityFormSection> lstSections = new SortedList<int, cCustomEntityFormSection>() { { section.sectionid, section } };
                SortedList<int, cCustomEntityFormField> lstFields = new SortedList<int, cCustomEntityFormField>();
                lstFields.Add(numberField.attribute.attributeid, numberField);
                lstFields.Add(dateTimeField.attribute.attributeid, dateTimeField);
                lstFields.Add(tickBoxField.attribute.attributeid, tickBoxField);

                cCustomEntityForm reqForm = cCustomEntityFormObject.New(reqCustomEntity.entityid, cCustomEntityFormObject.Template(tabs: lstTabs, sections: lstSections, fields: lstFields));

                customEntities = new cCustomEntities(Moqs.CurrentUser());

                customEntities.saveForm(reqCustomEntity.entityid, reqForm);

                #endregion Create a new Form with a tab, section and mutliple form fields

                reqCustomEntity = customEntities.getEntityById(reqCustomEntity.entityid);

                Dictionary<int, object> entityRecords = new Dictionary<int, object>();

                DateTime dtNow = DateTime.UtcNow;

                SortedList<int, object> parameterList = new SortedList<int, object>();
                parameterList.Add(numberAttribute.attributeid, 69);
                parameterList.Add(dateTimeAttribute.attributeid, dtNow);
                parameterList.Add(tickBoxAttribute.attributeid, true);

                //Create a new record for reqCustomEntity, using the attribute created above
                int nRecordID = CustomEntityRecord.New(reqCustomEntity, out entityRecords, parameterList: parameterList);

                customEntities = new cCustomEntities(Moqs.CurrentUser());

                SortedList<int, object> lstReturnedVals = customEntities.getEntityRecord(reqCustomEntity, nRecordID, reqCustomEntity.Forms.Values[0]);

                // Assert the value for the number attribute has been returned
                Assert.AreEqual(typeof(System.Decimal), lstReturnedVals[numberAttribute.attributeid].GetType());
                Assert.AreEqual((System.Decimal)69, lstReturnedVals[numberAttribute.attributeid]);

                // Assert the value for the datetime attribute has been returned
                Assert.AreEqual(typeof(System.DateTime), lstReturnedVals[dateTimeAttribute.attributeid].GetType());
                Assert.AreEqual(dtNow.ToString(), lstReturnedVals[dateTimeAttribute.attributeid].ToString());

                // Assert the value for the tickbox attribute has been returned
                Assert.AreEqual(typeof(System.Boolean), lstReturnedVals[tickBoxAttribute.attributeid].GetType());
                Assert.AreEqual(true, lstReturnedVals[tickBoxAttribute.attributeid]);

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
        ///A test for getEntityRecord using a derived custom entity record
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_getEntityRecord_UsingSystemViewEntity()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity startEntity = null;
            cCustomEntity relatedEntity = null;
            int startEntityID, relatedEntityID;

            try
            {
                // create a start entity
                startEntity = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getEntityRecord Test Start Entity"));

                relatedEntity = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "getEntityRecord Test Related Entity"));

                startEntityID = startEntity.entityid;
                relatedEntityID = relatedEntity.entityid;

                cCustomEntityForm formA = cCustomEntityFormObject.New(startEntity.entityid, cCustomEntityFormObject.Basic(startEntity));
                cCustomEntityView viewA = cCustomEntityViewObject.New(startEntity.entityid);
                cCustomEntityForm formB = cCustomEntityFormObject.New(relatedEntity.entityid, cCustomEntityFormObject.Basic(relatedEntity));
                cCustomEntityView viewB = cCustomEntityViewObject.New(relatedEntity.entityid);

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                startEntity = clsCustomEntities.getEntityById(startEntityID);
                relatedEntity = clsCustomEntities.getEntityById(relatedEntityID);

                cOneToManyRelationship attributeOTM = cOneToManyRelationshipObject.NewWithDerived(startEntityID, cOneToManyRelationshipObject.BasicOTM(relatedEntityID, relatedEntity));

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity systemViewEntity = clsCustomEntities.getSystemViewEntity(relatedEntityID, startEntityID);

                Dictionary<int, object> entityRecords = new Dictionary<int, object>();

                //Create a new record for reqCustomEntity, using the attribute created above
                int nFirstRecordID = CustomEntityRecord.New(startEntity, out entityRecords);

                int nRecordID = CustomEntityRecord.New(relatedEntity, out entityRecords, parentOTMAttributeID: attributeOTM.attributeid, parentEntityRecordID: nFirstRecordID);

                clsCustomEntities = new cCustomEntities(Moqs.CurrentUser());

                SortedList<int, object> lstReturnedVals = clsCustomEntities.getEntityRecord(systemViewEntity, nRecordID, relatedEntity.Forms.Values[0]);

                Assert.AreEqual(1, lstReturnedVals.Count);
                Assert.AreEqual(typeof(System.Int32), lstReturnedVals.Values[0].GetType());
                Assert.AreEqual(nFirstRecordID, lstReturnedVals.Values[0]);
                Assert.AreEqual("ID", relatedEntity.getAttributeById(lstReturnedVals.Keys[0]).displayname);
            }
            finally
            {
                if (startEntity != null)
                {
                    cCustomEntityObject.TearDown(startEntity.entityid);
                }
                if (relatedEntity != null)
                {
                    cCustomEntityObject.TearDown(relatedEntity.entityid);
                }
            }
        }

        #endregion getEntityRecord

        #region createParentEntityReportConfig
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_createParentEntityReportConfig_NullStartEntity()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            try
            {
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                List<cCustomEntity> retConfig = clsCustomEntities.createParentEntityReportConfig(null, null, 1, null);

                Assert.IsNotNull(retConfig);
                Assert.AreEqual(retConfig.Count, 0);

            }
            finally
            {

            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_createParentEntityReportConfig_NullCurrentEntity()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity startEntity = null;

            try
            {
                // create a start entity
                startEntity = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "createParentEntityReportConfig Test Entity"));

                Assert.IsNotNull(startEntity, "startEntity should not be null");
                Assert.IsTrue(startEntity.entityid > 0, "startEntity.entityid should be greater than 0");

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                List<cCustomEntity> retConfig = clsCustomEntities.createParentEntityReportConfig(startEntity, null, 1, null);

                Assert.IsNotNull(retConfig);
                Assert.AreEqual(1, retConfig.Count);

            }
            finally
            {
                if (startEntity != null)
                {
                    cCustomEntityObject.TearDown(startEntity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_createParentEntityReportConfig_WithValidOTMRelationship()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity startEntity = null;
            cCustomEntity relatedEntity = null;
            int startEntityID, relatedEntityID;

            try
            {
                // create a start entity
                startEntity = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "createParentEntityReportConfig Test Start Entity"));
                Assert.IsNotNull(startEntity, "startEntity should not be null");
                Assert.IsTrue(startEntity.entityid > 0, "startEntity.entityid should be greater than 0");

                relatedEntity = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "createParentEntityReportConfig Test Related Entity"));
                Assert.IsNotNull(relatedEntity, "relatedEntity should not be null");
                Assert.IsTrue(relatedEntity.entityid > 0, "relatedEntity.entityid should be greater than 0");

                startEntityID = startEntity.entityid;
                relatedEntityID = relatedEntity.entityid;

                cCustomEntityForm formA = cCustomEntityFormObject.New(startEntity.entityid, cCustomEntityFormObject.Basic(startEntity));
                cCustomEntityView viewA = cCustomEntityViewObject.New(startEntity.entityid);
                cCustomEntityForm formB = cCustomEntityFormObject.New(relatedEntity.entityid, cCustomEntityFormObject.Basic(relatedEntity));
                cCustomEntityView viewB = cCustomEntityViewObject.New(relatedEntity.entityid);

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                startEntity = clsCustomEntities.getEntityById(startEntityID);
                relatedEntity = clsCustomEntities.getEntityById(relatedEntityID);

                cOneToManyRelationship attributeOTM = cOneToManyRelationshipObject.New(startEntityID, cOneToManyRelationshipObject.BasicOTM(relatedEntityID, relatedEntity));
                Assert.IsNotNull(attributeOTM, "attributeOTM should not be null");
                Assert.IsTrue(attributeOTM.attributeid > 0, "attributeOTM.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                List<cCustomEntity> retConfig = clsCustomEntities.createParentEntityReportConfig(startEntity, relatedEntity, 1, null);

                Assert.IsNotNull(retConfig);
                Assert.AreEqual(2, retConfig.Count);

            }
            finally
            {
                if (startEntity != null)
                {
                    cCustomEntityObject.TearDown(startEntity.entityid);
                }

                if (relatedEntity != null)
                {
                    cCustomEntityObject.TearDown(relatedEntity.entityid);
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_createParentEntityReportConfig_WithValidOTMSystemView()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity startEntity = null;
            cCustomEntity relatedEntity = null;
            int startEntityID, relatedEntityID;

            try
            {
                // create a start entity
                startEntity = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "createParentEntityReportConfig Test Start Entity"));
                Assert.IsNotNull(startEntity, "startEntity should not be null");
                Assert.IsTrue(startEntity.entityid > 0, "startEntity.entityid should be greater than 0");

                relatedEntity = cCustomEntityObject.New(entity: cCustomEntityObject.Template(description: "createParentEntityReportConfig Test Related Entity"));
                Assert.IsNotNull(relatedEntity, "relatedEntity should not be null");
                Assert.IsTrue(relatedEntity.entityid > 0, "relatedEntity.entityid should be greater than 0");

                startEntityID = startEntity.entityid;
                relatedEntityID = relatedEntity.entityid;

                cCustomEntityForm formA = cCustomEntityFormObject.New(startEntity.entityid, cCustomEntityFormObject.Basic(startEntity));
                cCustomEntityView viewA = cCustomEntityViewObject.New(startEntity.entityid);
                cCustomEntityForm formB = cCustomEntityFormObject.New(relatedEntity.entityid, cCustomEntityFormObject.Basic(relatedEntity));
                cCustomEntityView viewB = cCustomEntityViewObject.New(relatedEntity.entityid);

                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                startEntity = clsCustomEntities.getEntityById(startEntityID);
                relatedEntity = clsCustomEntities.getEntityById(relatedEntityID);

                cOneToManyRelationship attributeOTM = cOneToManyRelationshipObject.NewWithDerived(startEntityID, cOneToManyRelationshipObject.BasicOTM(relatedEntityID, relatedEntity));
                Assert.IsNotNull(attributeOTM, "attributeOTM should not be null");
                Assert.IsTrue(attributeOTM.attributeid > 0, "attributeOTM.attributeid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity systemViewEntity = clsCustomEntities.getSystemViewEntity(relatedEntityID, startEntityID);
                Assert.IsNotNull(systemViewEntity, "systemViewEntity should not be null");
                Assert.IsTrue(systemViewEntity.entityid > 0, "systemViewEntity.entityid should be greater than 0");

                clsCustomEntities = new cCustomEntities(currentUser);
                List<cCustomEntity> retConfig = clsCustomEntities.createParentEntityReportConfig(startEntity, systemViewEntity, 1, null);

                Assert.IsNotNull(retConfig);
                Assert.AreEqual(3, retConfig.Count);

            }
            finally
            {
                if (startEntity != null)
                {
                    cCustomEntityObject.TearDown(startEntity.entityid);
                }

                if (relatedEntity != null)
                {
                    cCustomEntityObject.TearDown(relatedEntity.entityid);
                }
            }
        }
        #endregion

        #region GetAudienceRecords

        /// <summary>
        ///A test for GetAudienceRecords using an entity with no audience set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetAudienceRecords_UsingEntityWithNoAudience()
        {
            cCustomEntity reqCustomEntity = cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID);

            cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

            SerializableDictionary<string, object> results = customEntities.GetAudienceRecords(reqCustomEntity.entityid, Moqs.CurrentUser().EmployeeID);

            Assert.IsNull(results);

        }

        /// <summary>
        ///A test for GetAudienceRecords using an entity with no audience records, where an audience record ID is set
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetAudienceRecords_UsingEntityWithNoAudienceAndAudienceRecordIDSet()
        {
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID, audienceViewType: AudienceViewType.AllowAllIfNoneExist));

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                cNumberAttribute attribute = (cNumberAttribute)reqCustomEntity.attributes.Values[0];

                SerializableDictionary<string, object> results = customEntities.GetAudienceRecords(reqCustomEntity.entityid, Moqs.CurrentUser().EmployeeID, 1);

                Assert.AreEqual(0, results.Count);
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
        ///A test for GetAudienceRecords using an invalid custom entity
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetAudienceRecords_UsingInvalidEntity()
        {
            cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

            SerializableDictionary<string, object> results = customEntities.GetAudienceRecords(-69, Moqs.CurrentUser().EmployeeID);

            Assert.IsNull(results);
        }

        /// <summary>
        ///A test for GetAudienceRecords using a custom entity with an audience record that is not in cache
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetAudienceRecords_UsingEntityWithAudienceNotInCache()
        {
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID, audienceViewType: AudienceViewType.AllowAllIfNoneExist));

            cAudience reqAudience = null;

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                Dictionary<int, object> entityRecords = new Dictionary<int, object>();

                //Create a new record for reqCustomEntity, using the attribute created above
                int nRecordID = CustomEntityRecord.New(reqCustomEntity, out entityRecords);

                List<int> lstEmployeeIDs = new List<int> { Moqs.CurrentUser().EmployeeID };

                reqAudience = cAudienceObject.New(lstEmployeeIDs);

                List<int> lstAudienceIDs = new List<int> { reqAudience.audienceID };

                cAudienceRecordStatus expected = cAudienceRecordStatusObject.New(nRecordID, reqCustomEntity.AudienceTable.TableID, lstAudienceIDs, true, true, true);

                SerializableDictionary<string, object> results = customEntities.GetAudienceRecords(reqCustomEntity.entityid, Moqs.CurrentUser().EmployeeID);

                cCompareAssert.AreEqual(expected, (cAudienceRecordStatus)results[nRecordID.ToString()], new List<string> { "Status" });
            }
            finally
            {
                if (reqAudience != null)
                {
                    cAudienceObject.TearDown(reqAudience.audienceID);
                }
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for GetAudienceRecords using a custom entity with an audience record which is not in cache
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetAudienceRecords_UsingEntityWithAudienceNotInCacheAndRecordIDSpecified()
        {
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID, audienceViewType: AudienceViewType.AllowAllIfNoneExist));

            cAudience reqAudience = null;

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                Dictionary<int, object> entityRecords = new Dictionary<int, object>();

                //Create a new record for reqCustomEntity, using the attribute created above
                int nRecordID = CustomEntityRecord.New(reqCustomEntity, out entityRecords);

                List<int> lstEmployeeIDs = new List<int> { Moqs.CurrentUser().EmployeeID };

                reqAudience = cAudienceObject.New(lstEmployeeIDs);

                List<int> lstAudienceIDs = new List<int> { reqAudience.audienceID };

                cAudienceRecordStatus expected = cAudienceRecordStatusObject.New(nRecordID, reqCustomEntity.AudienceTable.TableID, lstAudienceIDs, true, true, true);

                SerializableDictionary<string, object> results = customEntities.GetAudienceRecords(reqCustomEntity.entityid, Moqs.CurrentUser().EmployeeID, nRecordID);

                cCompareAssert.AreEqual(expected, (cAudienceRecordStatus)results[nRecordID.ToString()], new List<string> { "Status" });
            }
            finally
            {
                if (reqAudience != null)
                {
                    cAudienceObject.TearDown(reqAudience.audienceID);
                }
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for GetAudienceRecords using a custom entity with an audience record which IS in cache
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetAudienceRecords_UsingEntityWithAudienceInCache()
        {
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID, audienceViewType: AudienceViewType.AllowAllIfNoneExist));

            cAudience reqAudience = null;

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                Dictionary<int, object> entityRecords = new Dictionary<int, object>();

                //Create a new record for reqCustomEntity, using the attribute created above
                int nRecordID = CustomEntityRecord.New(reqCustomEntity, out entityRecords);

                customEntities = new cCustomEntities(Moqs.CurrentUser());

                List<int> lstEmployeeIDs = new List<int> { Moqs.CurrentUser().EmployeeID };

                reqAudience = cAudienceObject.New(lstEmployeeIDs);

                List<int> lstAudienceIDs = new List<int> { reqAudience.audienceID };

                cAudienceRecordStatus expected = cAudienceRecordStatusObject.New(nRecordID, reqCustomEntity.AudienceTable.TableID, lstAudienceIDs, true, true, true);

                // Call GetAudienceRecords to insert the audience records into cache
                SerializableDictionary<string, object> results = customEntities.GetAudienceRecords(reqCustomEntity.entityid, Moqs.CurrentUser().EmployeeID);

                cCompareAssert.AreEqual(expected, (cAudienceRecordStatus)results[nRecordID.ToString()], new List<string> { "Status" });
                results = null;

                // Call GetAudienceRecords again and ensure the correct results are returned from cache
                results = customEntities.GetAudienceRecords(reqCustomEntity.entityid, Moqs.CurrentUser().EmployeeID);

                cCompareAssert.AreEqual(expected, (cAudienceRecordStatus)results[nRecordID.ToString()], new List<string> { "Status" });
            }
            finally
            {
                if (reqAudience != null)
                {
                    cAudienceObject.TearDown(reqAudience.audienceID);
                }
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for GetAudienceRecords using a custom entity with an audience record in cache, specifying an audience record ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetAudienceRecords_UsingEntityWithAudienceInCacheAndRecordIDSpecified()
        {
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID, audienceViewType: AudienceViewType.AllowAllIfNoneExist));

            cAudience reqAudience = null;

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                Dictionary<int, object> entityRecords = new Dictionary<int, object>();

                //Create a new record for reqCustomEntity, using the attribute created above
                int nFirstRecordID = CustomEntityRecord.New(reqCustomEntity, out entityRecords);
                int nSecondRecordID = CustomEntityRecord.New(reqCustomEntity, out entityRecords);

                List<int> lstEmployeeIDs = new List<int> { Moqs.CurrentUser().EmployeeID };

                reqAudience = cAudienceObject.New(lstEmployeeIDs);

                List<int> lstAudienceIDs = new List<int> { reqAudience.audienceID };

                cAudienceRecordStatus firstAudienceRecord = cAudienceRecordStatusObject.New(nFirstRecordID, reqCustomEntity.AudienceTable.TableID, lstAudienceIDs, true, true, true);
                cAudienceRecordStatus secondAudienceRecord = cAudienceRecordStatusObject.New(nSecondRecordID, reqCustomEntity.AudienceTable.TableID, lstAudienceIDs, true, true, true);

                // Call GetAudienceRecords to insert the audience records into cache
                SerializableDictionary<string, object> results = customEntities.GetAudienceRecords(reqCustomEntity.entityid, Moqs.CurrentUser().EmployeeID);

                Assert.AreEqual(2, results.Count);
                cCompareAssert.AreEqual(firstAudienceRecord, (cAudienceRecordStatus)results[nFirstRecordID.ToString()], new List<string> { "Status" });
                cCompareAssert.AreEqual(secondAudienceRecord, (cAudienceRecordStatus)results[nSecondRecordID.ToString()], new List<string> { "Status" });

                results = null;

                // Call GetAudienceRecrods again to ensure the specified record ID is returned from cache
                results = customEntities.GetAudienceRecords(reqCustomEntity.entityid, Moqs.CurrentUser().EmployeeID, nFirstRecordID);

                Assert.AreEqual(1, results.Count);
                cCompareAssert.AreEqual(firstAudienceRecord, (cAudienceRecordStatus)results[nFirstRecordID.ToString()], new List<string> { "Status" });
            }
            finally
            {
                if (reqAudience != null)
                {
                    cAudienceObject.TearDown(reqAudience.audienceID);
                }
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for GetAudienceRecords using a custom entity with two audiences, both not in cache
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetAudienceRecords_UsingEntityWithTwoAudiencesNotInCache()
        {
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID, audienceViewType: AudienceViewType.AllowAllIfNoneExist));

            cAudience reqAudience = null;

            cAudience reqSecondAudience = null;

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                Dictionary<int, object> entityRecords = new Dictionary<int, object>();

                //Create new records for reqCustomEntity, using the attribute created above
                int nFirstRecordID = CustomEntityRecord.New(reqCustomEntity, out entityRecords);
                int nSecondRecordID = CustomEntityRecord.New(reqCustomEntity, out entityRecords);

                List<int> lstEmployeeIDs = new List<int> { Moqs.CurrentUser().EmployeeID };

                reqAudience = cAudienceObject.New(lstEmployeeIDs);
                reqSecondAudience = cAudienceObject.New(lstEmployeeIDs);

                List<int> lstAudienceIDs = new List<int> { reqAudience.audienceID, reqSecondAudience.audienceID };

                cAudienceRecordStatus firstAudienceRecordStatus = cAudienceRecordStatusObject.New(nFirstRecordID, reqCustomEntity.AudienceTable.TableID, lstAudienceIDs, true, true, true);
                cAudienceRecordStatus secondAudienceRecordStatus = cAudienceRecordStatusObject.New(nSecondRecordID, reqCustomEntity.AudienceTable.TableID, lstAudienceIDs, true, true, true);

                SerializableDictionary<string, object> results = customEntities.GetAudienceRecords(reqCustomEntity.entityid, Moqs.CurrentUser().EmployeeID);

                cCompareAssert.AreEqual(firstAudienceRecordStatus, (cAudienceRecordStatus)results[nFirstRecordID.ToString()], new List<string> { "Status", "AudienceID", "RecordID" });
                cCompareAssert.AreEqual(secondAudienceRecordStatus, (cAudienceRecordStatus)results[nSecondRecordID.ToString()], new List<string> { "Status", "AudienceID", "RecordID" });

                cAudienceRecordStatus firstActual = (cAudienceRecordStatus)results[nFirstRecordID.ToString()];
                cAudienceRecordStatus secondActual = (cAudienceRecordStatus)results[nSecondRecordID.ToString()];

                // The 'Status' of the audience records are set during GetAudienceRecords. Ensure they are correct
                Assert.AreEqual(2, firstActual.Status);
                Assert.AreEqual(2, secondActual.Status);

                // The 'AudienceID' of each audience record will be the valid last audience used
                Assert.AreEqual(reqAudience.audienceID, firstActual.AudienceID);
                Assert.AreEqual(reqAudience.audienceID, secondActual.AudienceID);
            }
            finally
            {
                if (reqAudience != null)
                {
                    cAudienceObject.TearDown(reqAudience.audienceID);
                }
                if (reqSecondAudience != null)
                {
                    cAudienceObject.TearDown(reqSecondAudience.audienceID);
                }
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        /// <summary>
        ///A test for GetAudienceRecords using a custom entity that is in cache and an invalid audience record ID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetAudienceRecords_UsingEntityWithAudiencesInCacheAndRecordIDNotValid()
        {
            cCustomEntity reqCustomEntity = cCustomEntityObject.New(cCustomEntityObject.Template(createdBy: Moqs.CurrentUser().EmployeeID, audienceViewType: AudienceViewType.AllowAllIfNoneExist));

            cAudience reqAudience = null;

            cAudience reqSecondAudience = null;

            try
            {
                cCustomEntities customEntities = new cCustomEntities(Moqs.CurrentUser());

                Dictionary<int, object> entityRecords = new Dictionary<int, object>();

                //Create new records for reqCustomEntity, using the attribute created above
                int nFirstRecordID = CustomEntityRecord.New(reqCustomEntity, out entityRecords);
                int nSecondRecordID = CustomEntityRecord.New(reqCustomEntity, out entityRecords);

                List<int> lstEmployeeIDs = new List<int> { Moqs.CurrentUser().EmployeeID };

                reqAudience = cAudienceObject.New(lstEmployeeIDs);
                reqSecondAudience = cAudienceObject.New(lstEmployeeIDs);

                List<int> lstAudienceIDs = new List<int> { reqAudience.audienceID, reqSecondAudience.audienceID };

                cAudienceRecordStatus firstAudienceRecordStatus = cAudienceRecordStatusObject.New(nFirstRecordID, reqCustomEntity.AudienceTable.TableID, lstAudienceIDs, true, true, true);
                cAudienceRecordStatus secondAudienceRecordStatus = cAudienceRecordStatusObject.New(nSecondRecordID, reqCustomEntity.AudienceTable.TableID, lstAudienceIDs, true, true, true);

                SerializableDictionary<string, object> results = customEntities.GetAudienceRecords(reqCustomEntity.entityid, Moqs.CurrentUser().EmployeeID);

                Assert.AreEqual(2, results.Count);

                results = customEntities.GetAudienceRecords(reqCustomEntity.entityid, Moqs.CurrentUser().EmployeeID, -69);

                cCompareAssert.AreEqual(firstAudienceRecordStatus, (cAudienceRecordStatus)results[nFirstRecordID.ToString()], new List<string> { "Status", "AudienceID", "RecordID" });
                cCompareAssert.AreEqual(secondAudienceRecordStatus, (cAudienceRecordStatus)results[nSecondRecordID.ToString()], new List<string> { "Status", "AudienceID", "RecordID" });

                cAudienceRecordStatus firstActual = (cAudienceRecordStatus)results[nFirstRecordID.ToString()];
                cAudienceRecordStatus secondActual = (cAudienceRecordStatus)results[nSecondRecordID.ToString()];

                // The 'Status' of the audience records are set during GetAudienceRecords. Ensure they are correct
                Assert.AreEqual(2, firstActual.Status);
                Assert.AreEqual(2, secondActual.Status);

                // The 'AudienceID' of each audience record will be the valid last audience used
                Assert.AreEqual(reqAudience.audienceID, firstActual.AudienceID);
                Assert.AreEqual(reqAudience.audienceID, secondActual.AudienceID);
            }
            finally
            {
                if (reqAudience != null)
                {
                    cAudienceObject.TearDown(reqAudience.audienceID);
                }
                if (reqSecondAudience != null)
                {
                    cAudienceObject.TearDown(reqSecondAudience.audienceID);
                }
                if (reqCustomEntity != null)
                {
                    cCustomEntityObject.TearDown(reqCustomEntity.entityid);
                }
            }
        }

        #endregion

        #region GetViewIconsByName

        /// <summary>
        /// A test for GetViewIconsByName where a file name is not used
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetViewIconsByName_UsingNoFileName()
        {
            var actual = cCustomEntities.GetViewIconsByName(string.Empty, 0, GlobalTestVariables.StaticLibraryFolderLocation, GlobalTestVariables.StaticLibraryPath);

            Assert.IsTrue(actual.FurtherResults);
            Assert.AreEqual(0, actual.ResultStartNumber);
            Assert.AreEqual(30, actual.ResultEndNumber);
            Assert.AreEqual(30, actual.MenuIcons.Count);
            Assert.IsTrue(actual.MenuIcons[0].IconName.EndsWith(".png"));
            Assert.IsTrue(actual.MenuIcons[0].IconUrl.EndsWith(".png"));
        }

        /// <summary>
        /// A test for GetViewIconsByName where an invalid file name is used
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetViewIconsByName_UsingInvalidFileName()
        {
            var actual = cCustomEntities.GetViewIconsByName("<>", 0, GlobalTestVariables.StaticLibraryFolderLocation, GlobalTestVariables.StaticLibraryPath);

            Assert.IsFalse(actual.FurtherResults);
            Assert.AreEqual(0, actual.ResultStartNumber);
            Assert.AreEqual(0, actual.ResultEndNumber);
            Assert.AreEqual(0, actual.MenuIcons.Count);
        }

        /// <summary>
        /// A test for GetViewIconsByName where a valid file name is used
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetViewIconsByName_UsingValidFileName()
        {
            var actual = cCustomEntities.GetViewIconsByName("window_dialog.png", 0, GlobalTestVariables.StaticLibraryFolderLocation, GlobalTestVariables.StaticLibraryPath);

            Assert.IsFalse(actual.FurtherResults);
            Assert.AreEqual(0, actual.ResultStartNumber);
            Assert.AreEqual(1, actual.ResultEndNumber);
            Assert.AreEqual(1, actual.MenuIcons.Count);
            Assert.AreEqual("window_dialog.png", actual.MenuIcons[0].IconName);
            Assert.IsTrue(actual.MenuIcons[0].IconUrl.EndsWith("window_dialog.png"));
        }

        /// <summary>
        /// A test for GetViewIconsByName where a valid file name is used and no results exist
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetViewIconsByName_UsingValidFileNameWhereNoResultsExist()
        {
            var actual = cCustomEntities.GetViewIconsByName("thisIconDoesNotExistAndNeverWill", 0, GlobalTestVariables.StaticLibraryFolderLocation, GlobalTestVariables.StaticLibraryPath);

            Assert.IsFalse(actual.FurtherResults);
            Assert.AreEqual(0, actual.ResultStartNumber);
            Assert.AreEqual(0, actual.ResultEndNumber);
            Assert.AreEqual(0, actual.MenuIcons.Count);
        }

        /// <summary>
        /// A test for GetViewIconsByName where the start number is greater than zero
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetViewIconsByName_WhereStartNumberIsGreaterThanZero()
        {
            var actual = cCustomEntities.GetViewIconsByName(string.Empty, 30, GlobalTestVariables.StaticLibraryFolderLocation, GlobalTestVariables.StaticLibraryPath);

            Assert.IsTrue(actual.FurtherResults);
            Assert.AreEqual(30, actual.ResultStartNumber);
            Assert.AreEqual(60, actual.ResultEndNumber);
            Assert.AreEqual(30, actual.MenuIcons.Count);
            Assert.IsTrue(actual.MenuIcons[0].IconName.EndsWith(".png"));
            Assert.IsTrue(actual.MenuIcons[0].IconUrl.EndsWith(".png"));

            actual = cCustomEntities.GetViewIconsByName(string.Empty, 60, GlobalTestVariables.StaticLibraryFolderLocation, GlobalTestVariables.StaticLibraryPath);

            Assert.IsTrue(actual.FurtherResults);
            Assert.AreEqual(60, actual.ResultStartNumber);
            Assert.AreEqual(90, actual.ResultEndNumber);
            Assert.AreEqual(30, actual.MenuIcons.Count);
            Assert.IsTrue(actual.MenuIcons[0].IconName.EndsWith(".png"));
            Assert.IsTrue(actual.MenuIcons[0].IconUrl.EndsWith(".png"));
        }

        /// <summary>
        /// A test for GetViewIconsByName where the start number is less than zero
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetViewIconsByName_WhereStartNumberIsLessThanZero()
        {
            var actual = cCustomEntities.GetViewIconsByName(string.Empty, -6, GlobalTestVariables.StaticLibraryFolderLocation, GlobalTestVariables.StaticLibraryPath);

            Assert.IsFalse(actual.FurtherResults);
            Assert.AreEqual(0, actual.ResultStartNumber);
            Assert.AreEqual(0, actual.ResultEndNumber);
            Assert.AreEqual(0, actual.MenuIcons.Count);
        }

        /// <summary>
        /// A test for GetViewIconsByName where the static library location is not valid
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_GetViewIconsByName_UsingInvalidStaticLibraryLocation()
        {
            var actual = cCustomEntities.GetViewIconsByName(string.Empty, 0, "InvalidPath", GlobalTestVariables.StaticLibraryPath);

            Assert.IsFalse(actual.FurtherResults);
            Assert.AreEqual(0, actual.ResultStartNumber);
            Assert.AreEqual(0, actual.ResultEndNumber);
            Assert.AreEqual(0, actual.MenuIcons.Count);
        }

        #endregion GetViewIconsByName


        /// <summary>
        //A test to get a list of views and ensure that the selected value is the default view for the entity.
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_createViewDropDown_Valid()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity = cCustomEntityObject.New(cCustomEntityObject.Template(description: "createViewDropDown test entity"));

            try
            {
                Assert.IsNotNull(entity, "entityA should not be null");
                Assert.IsTrue(entity.entityid > 0, "entityA.entityid should be greater than 0");

                //create view for the entity
                cCustomEntityView view = cCustomEntityViewObject.New(entity.entityid);

                //assign view to entity
                entity = cCustomEntityObject.Template(entityID: entity.entityid, defaultPopupView: view.viewid);
                cCustomEntities clsentities = new cCustomEntities();
                ListItem[] popupViews = clsentities.createViewDropDown(entity.entityid, view.viewid,
                                                                            currentUser.AccountID);
                int ddlSelectedValue = Convert.ToInt32(popupViews[0].Value);

                Assert.AreEqual(ddlSelectedValue, view.viewid);
                Assert.IsTrue(popupViews[0].Selected);
            }
            finally
            {
                if (entity != null)
                {
                    cCustomEntityObject.TearDown(entity.entityid);
                }
            }
        }


        /// <summary>
        //A test to check if a view is associated with a popup view.
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Custom Entities"), TestMethod()]
        public void cCustomEntities_checkViewDoesNotBelongToPopupView_Valid()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntity entity1 = cCustomEntityObject.New(cCustomEntityObject.Template(description: "checkViewDoesNotBelongToPopupView test entity1"));
            cCustomEntity entity2 = null;
            try
            {
                Assert.IsNotNull(entity1, "entity1 should not be null");
                Assert.IsTrue(entity1.entityid > 0, "entity1.entityid should be greater than 0");

                //create view for the entity
                cCustomEntityView view = cCustomEntityViewObject.New(entity1.entityid);

                //assign defaultpopup viewid to new entity
                entity2 = cCustomEntityObject.New(cCustomEntityObject.Template(description: "checkViewDoesNotBelongToPopupView test entity2", defaultPopupView: view.viewid, enablePopupWindow: true));

                Assert.IsNotNull(entity2, "entity2 should not be null");
                Assert.IsTrue(entity2.entityid > 0, "entity2.entityid should be greater than 0");

                int viewid = Convert.ToInt32(view.viewid);
                cCustomEntities clsentities = new cCustomEntities();

                int outcome = clsentities.checkViewDoesNotBelongToPopupView(viewid, currentUser.AccountID);

                //check should return a count of 1 as it belongs to an entity as a default popup view                              
                Assert.AreEqual(outcome, 1);
            }
            finally
            {
                if (entity1 != null) cCustomEntityObject.TearDown(entity1.entityid);
                if (entity2 != null) cCustomEntityObject.TearDown(entity2.entityid);
            }
        }
    
    }
}
