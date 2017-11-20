using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
using Auto_Tests.UIMaps.CustomEntityFormsUIMapClasses;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Auto_Tests.Tools;
using Auto_Tests.UIMaps.CustomEntitiesUIMapClasses;


namespace Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Forms
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class CustomEntityFormsAdministrationTests
    {
        private static SharedMethodsUIMap cSharedMethods = new SharedMethodsUIMap();
        private static CustomEntityFormsUIMap cCustomEntityFormMethods = new CustomEntityFormsUIMap();
        private static CustomEntitiesUIMap cCustomEntityMethods = new CustomEntitiesUIMap();
        private static ProductType ExecutingProduct = ProductType.expenses;
        private static List<CustomEntitiesUtilities.CustomEntity> customEntities;

        public CustomEntityFormsAdministrationTests()
        {

        }

        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            cSharedMethods.StartIE(ExecutingProduct);
            cSharedMethods.Logon(ExecutingProduct, LogonType.administrator);
            CachePopulatorForForms CustomEntityDataFromLithium = new CachePopulatorForForms();
            customEntities = CustomEntityDataFromLithium.PopulateCache();
            Assert.IsNotNull(customEntities);
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            //cSharedMethods.CloseBrowserWindow();
        }


        [TestMethod]
        public void CustomEntityFormsSuccessfullyCancelEditingTab()
        {

            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(ExecutingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0]._entityId);

            //click new form
           


        }

        //[TestMethod]
        //public void DragAndDropAttributeOnformTest()
        //{
        //    SharedMethodsUIMap cSharedMethods = new SharedMethodsUIMap();
        //    CustomEntityFormsUIMap map = new CustomEntityFormsUIMap();
        //    Dictionary<string, HtmlDiv> AvailablefieldstoManipulate = new Dictionary<string, HtmlDiv>();
        //    /*cSharedMethods.StartIE(ProductType.expenses);
        //    cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);//4029
        //    cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/aecustomentity.aspx?entityid=3395");
           
        //    map.ClickForms();
        //    map.ClickNewForm();
        //    map.ClickFormDesign();
        //    map.ClickAddForm();
        //    HtmlEdit uITabnameEdit = map.UICustomEntity12WindowWindow.UICustomEntity12Document.UITabnameEdit;
        //    uITabnameEdit.Text = "Section1";
        //    map.ClickSaveTab();
        //    map.ClickExpandAvailableFields();*/
        //    //map.cust;
        //    HtmlDiv pane = map.UICustomEntity13WindowWindow.UICustomEntity13Document.AvailableFields;
        //    UITestControlCollection availableFieldsCollection = pane.GetChildren();
        //    foreach (HtmlDiv field in availableFieldsCollection)
        //    {
        //        UITestControlCollection availableFieldsCollection2 = field.GetChildren();
        //        if (availableFieldsCollection2.Count == 0)
        //        {
        //            MessageBox.Show(field.DisplayText);
        //        }
        //        else
        //        {
        //            foreach (HtmlDiv childField in availableFieldsCollection2)
        //            {
        //                MessageBox.Show(childField.DisplayText);
        //                AvailablefieldstoManipulate.Add(childField.DisplayText, childField);
        //            }
        //        }
        //    }

        //    HtmlDiv formDesignWindow = map.UICustomEntity13WindowWindow.UICustomEntity13Document.UIFormDesignSection1Pane.UISection1Pane;
        //    Mouse.Click(AvailablefieldstoManipulate["name"]);
        //    //Mouse.Click(new Point(AvailablefieldstoManipulate["name"].BoundingRectangle.X, AvailablefieldstoManipulate["name"].BoundingRectangle.Y), 1000);
        //    Mouse.StartDragging(AvailablefieldstoManipulate["name"]);
        //    Mouse.StopDragging(formDesignWindow, new Point(60,100));
        //    Mouse.Click(formDesignWindow);
        //}


        #region Additional test attributes

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        //}

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        //}

        #endregion

        /// <summary>
        /// Imports the form data into the codedui database
        ///</summary>
        private void ImportFormDataToEx_CodedUIDatabase(string test)
        {
            int result;
            switch (test)
            {
                case "CustomEntityFormsSuccessfullySortFormsGrid_UITest":
                    foreach (CustomEntitiesUtilities.CustomEntityForm form in customEntities[0].form)
                    {
                        result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], form);
                        Assert.IsTrue(result > 0);
                    }
                    break;
                case "CustomEntityFormsUnsuccessfullyEditDuplicateForm_UITest":
                    for (int i = 0; i < 2; i++)
                    {
                        result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], customEntities[0].form[i]);
                        Assert.IsTrue(result > 0);
                    }
                    break;
                default:
                    result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], customEntities[0].form[0]);
                    Assert.IsTrue(result > 0);
                    break;
            }
        }

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
        private TestContext testContextInstance;
    }
}
