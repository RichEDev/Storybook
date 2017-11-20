using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System;
using SpendManagementUnitTests.Global_Objects;
using System.Text;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cColoursTest and is intended
    ///to contain all cColoursTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cColoursTest
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
        ///A test for cColours Constructor in expenses
        ///</summary>
        [TestMethod()]
        public void cColours_cColoursConstructor_expensesValidSubAccount()
        {
            int accountID = cGlobalVariables.AccountID;
            int subAccountID = cGlobalVariables.SubAccountID;
            Modules activeModule = Modules.expenses;
            cColours target = new cColours(accountID, subAccountID, activeModule);

            Assert.IsNotNull(target.defaultMenubarBGColour);
            Assert.IsNotNull(target.defaultMenubarFGColour);
            Assert.IsNotNull(target.defaultTitlebarBGColour);
            Assert.IsNotNull(target.defaultTitlebarFGColour);
            Assert.IsNotNull(target.defaultFieldBG);
            Assert.IsNotNull(target.defaultFieldFG);
            Assert.IsNotNull(target.defaultRowBGColour);
            Assert.IsNotNull(target.defaultRowFGColour);
            Assert.IsNotNull(target.defaultAltRowBGColour);
            Assert.IsNotNull(target.defaultAltRowFGColour);
            Assert.IsNotNull(target.defaultHoverColour);
            Assert.IsNotNull(target.defaultPageOptionFGColour);

            Assert.IsNotNull(target.menubarBGColour);
            Assert.IsNotNull(target.menubarFGColour);
            Assert.IsNotNull(target.titlebarBGColour);
            Assert.IsNotNull(target.titlebarFGColour);
            Assert.IsNotNull(target.fieldBG);
            Assert.IsNotNull(target.fieldFG);
            Assert.IsNotNull(target.rowColour);
            Assert.IsNotNull(target.rowFGColour);
            Assert.IsNotNull(target.altRowColour);
            Assert.IsNotNull(target.altRowFGColour);
            Assert.IsNotNull(target.hovercolour);
            Assert.IsNotNull(target.pageOptionFGColour);

            Assert.AreEqual(target.defaultMenubarBGColour, cColoursObject.GetDefaultColour(Modules.expenses, "MBBG"));
            Assert.AreEqual(target.defaultMenubarFGColour, cColoursObject.GetDefaultColour(Modules.expenses, "MBFG"));
            Assert.AreEqual(target.defaultTitlebarBGColour, cColoursObject.GetDefaultColour(Modules.expenses, "TBBG"));
            Assert.AreEqual(target.defaultTitlebarFGColour, cColoursObject.GetDefaultColour(Modules.expenses, "TBFG"));
            Assert.AreEqual(target.defaultFieldBG, cColoursObject.GetDefaultColour(Modules.expenses, "FBG"));
            Assert.AreEqual(target.defaultFieldFG, cColoursObject.GetDefaultColour(Modules.expenses, "FFG"));
            Assert.AreEqual(target.defaultRowBGColour, cColoursObject.GetDefaultColour(Modules.expenses, "R1BG"));
            Assert.AreEqual(target.defaultRowFGColour, cColoursObject.GetDefaultColour(Modules.expenses, "R1FG"));
            Assert.AreEqual(target.defaultAltRowBGColour, cColoursObject.GetDefaultColour(Modules.expenses, "R2BG"));
            Assert.AreEqual(target.defaultAltRowFGColour, cColoursObject.GetDefaultColour(Modules.expenses, "R2FG"));
            Assert.AreEqual(target.defaultHoverColour, cColoursObject.GetDefaultColour(Modules.expenses, "H"));
            Assert.AreEqual(target.defaultPageOptionFGColour, cColoursObject.GetDefaultColour(Modules.expenses, "POFG"));
        }

        /// <summary>
        ///A test for cColours Constructor in framework
        ///</summary>
        [TestMethod()]
        public void cColours_cColoursConstructor_frameworkValidSubAccount()
        {
            int accountID = cGlobalVariables.AccountID;
            int subAccountID = cGlobalVariables.SubAccountID;
            Modules activeModule = Modules.contracts;
            cColours target = new cColours(accountID, subAccountID, activeModule);

            Assert.IsNotNull(target.defaultMenubarBGColour);
            Assert.IsNotNull(target.defaultMenubarFGColour);
            Assert.IsNotNull(target.defaultTitlebarBGColour);
            Assert.IsNotNull(target.defaultTitlebarFGColour);
            Assert.IsNotNull(target.defaultFieldBG);
            Assert.IsNotNull(target.defaultFieldFG);
            Assert.IsNotNull(target.defaultRowBGColour);
            Assert.IsNotNull(target.defaultRowFGColour);
            Assert.IsNotNull(target.defaultAltRowBGColour);
            Assert.IsNotNull(target.defaultAltRowFGColour);
            Assert.IsNotNull(target.defaultHoverColour);
            Assert.IsNotNull(target.defaultPageOptionFGColour);

            Assert.IsNotNull(target.menubarBGColour);
            Assert.IsNotNull(target.menubarFGColour);
            Assert.IsNotNull(target.titlebarBGColour);
            Assert.IsNotNull(target.titlebarFGColour);
            Assert.IsNotNull(target.fieldBG);
            Assert.IsNotNull(target.fieldFG);
            Assert.IsNotNull(target.rowColour);
            Assert.IsNotNull(target.rowFGColour);
            Assert.IsNotNull(target.altRowColour);
            Assert.IsNotNull(target.altRowFGColour);
            Assert.IsNotNull(target.hovercolour);
            Assert.IsNotNull(target.pageOptionFGColour);

            Assert.AreEqual(target.defaultMenubarBGColour, cColoursObject.GetDefaultColour(Modules.contracts, "MBBG"));
            Assert.AreEqual(target.defaultMenubarFGColour, cColoursObject.GetDefaultColour(Modules.contracts, "MBFG"));
            Assert.AreEqual(target.defaultTitlebarBGColour, cColoursObject.GetDefaultColour(Modules.contracts, "TBBG"));
            Assert.AreEqual(target.defaultTitlebarFGColour, cColoursObject.GetDefaultColour(Modules.contracts, "TBFG"));
            Assert.AreEqual(target.defaultFieldBG, cColoursObject.GetDefaultColour(Modules.contracts, "FBG"));
            Assert.AreEqual(target.defaultFieldFG, cColoursObject.GetDefaultColour(Modules.contracts, "FFG"));
            Assert.AreEqual(target.defaultRowBGColour, cColoursObject.GetDefaultColour(Modules.contracts, "R1BG"));
            Assert.AreEqual(target.defaultRowFGColour, cColoursObject.GetDefaultColour(Modules.contracts, "R1FG"));
            Assert.AreEqual(target.defaultAltRowBGColour, cColoursObject.GetDefaultColour(Modules.contracts, "R2BG"));
            Assert.AreEqual(target.defaultAltRowFGColour, cColoursObject.GetDefaultColour(Modules.contracts, "R2FG"));
            Assert.AreEqual(target.defaultHoverColour, cColoursObject.GetDefaultColour(Modules.contracts, "H"));
            Assert.AreEqual(target.defaultPageOptionFGColour, cColoursObject.GetDefaultColour(Modules.contracts, "POFG"));
        }

        ///// <summary>
        /////A test for cColours Constructor in for a bad subAccount
        /////</summary>
        //[TestMethod()]
        //public void cColours_cColoursConstructor_invalidSubAccount()
        //{
        //    int accountID = cGlobalVariables.AccountID;
        //    int subAccountID = 98535;
        //    Modules activeModule = Modules.contracts;
        //    cColours target = new cColours(accountID, subAccountID, activeModule);

        //    Assert.IsNotNull(target.defaultMenubarBGColour);
        //    Assert.IsNotNull(target.defaultMenubarFGColour);
        //    Assert.IsNotNull(target.defaultTitlebarBGColour);
        //    Assert.IsNotNull(target.defaultTitlebarFGColour);
        //    Assert.IsNotNull(target.defaultFieldBG);
        //    Assert.IsNotNull(target.defaultFieldFG);
        //    Assert.IsNotNull(target.defaultRowBGColour);
        //    Assert.IsNotNull(target.defaultRowFGColour);
        //    Assert.IsNotNull(target.defaultAltRowBGColour);
        //    Assert.IsNotNull(target.defaultAltRowFGColour);
        //    Assert.IsNotNull(target.defaultHoverColour);
        //    Assert.IsNotNull(target.defaultPageOptionFGColour);

        //    Assert.IsNotNull(target.menubarBGColour);
        //    Assert.IsNotNull(target.menubarFGColour);
        //    Assert.IsNotNull(target.titlebarBGColour);
        //    Assert.IsNotNull(target.titlebarFGColour);
        //    Assert.IsNotNull(target.fieldBG);
        //    Assert.IsNotNull(target.fieldFG);
        //    Assert.IsNotNull(target.rowColour);
        //    Assert.IsNotNull(target.rowFGColour);
        //    Assert.IsNotNull(target.altRowColour);
        //    Assert.IsNotNull(target.altRowFGColour);
        //    Assert.IsNotNull(target.hovercolour);
        //    Assert.IsNotNull(target.pageOptionFGColour);
        //}

        ///// <summary>
        /////A test for DeleteColours
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //[DeploymentItem("Spend Management.dll")]
        //public void cColours_DeleteColours_()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    cColours_Accessor target = new cColours_Accessor(param0); // TODO: Initialize to an appropriate value
        //    target.DeleteColours();
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for GetColours
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //[DeploymentItem("Spend Management.dll")]
        //public void cColours_GetColours_()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    cColours_Accessor target = new cColours_Accessor(param0); // TODO: Initialize to an appropriate value
        //    target.GetColours();
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for RefreshColours
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //[DeploymentItem("Spend Management.dll")]
        //public void cColours_RefreshColours_()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    cColours_Accessor target = new cColours_Accessor(param0); // TODO: Initialize to an appropriate value
        //    target.RefreshColours();
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for RestoreDefaults
        ///</summary>
        [TestMethod()]
        public void cColours_RestoreDefaults_valid()
        {
            int accountID = cGlobalVariables.AccountID;
            int subAccountID = cGlobalVariables.SubAccountID;
            Modules activeModule = Modules.contracts;
            cColours target = new cColours(accountID, subAccountID, activeModule);

            target.RestoreDefaults();

            Assert.AreEqual(target.defaultMenubarBGColour, target.menubarBGColour);
            Assert.AreEqual(target.defaultMenubarFGColour, target.menubarFGColour);
            Assert.AreEqual(target.defaultTitlebarBGColour, target.titlebarBGColour);
            Assert.AreEqual(target.defaultTitlebarFGColour, target.titlebarFGColour);
            Assert.AreEqual(target.defaultFieldBG, target.fieldBG);
            Assert.AreEqual(target.defaultFieldFG, target.fieldFG);
            Assert.AreEqual(target.defaultRowBGColour, target.rowColour);
            Assert.AreEqual(target.defaultRowFGColour, target.rowFGColour);
            Assert.AreEqual(target.defaultAltRowBGColour, target.altRowColour);
            Assert.AreEqual(target.defaultAltRowFGColour, target.altRowFGColour);
            Assert.AreEqual(target.defaultHoverColour, target.hovercolour);
            Assert.AreEqual(target.defaultPageOptionFGColour, target.pageOptionFGColour);
        }

        ///// <summary>
        /////A test for SetDefaultColours
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //[DeploymentItem("Spend Management.dll")]
        //public void cColours_SetDefaultColours_()
        //{
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours_Accessor.SetDefaultColours(activeModule);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for UpdateColours
        ///</summary>
        [TestMethod()]
        public void cColours_UpdateColours_blanks()
        {
            int accountID = cGlobalVariables.AccountID;
            int subAccountID = cGlobalVariables.SubAccountID;
            Modules activeModule = Modules.contracts;
            cColours target = new cColours(accountID, subAccountID, activeModule);

            string menuBarBG = string.Empty;
            string menuBarFG = string.Empty;
            string titleBarBG = string.Empty;
            string titleBarFG = string.Empty;
            string fieldBG = string.Empty;
            string fieldFG = string.Empty;
            string rowBG = string.Empty;
            string rowFG = string.Empty;
            string altRowBG = string.Empty;
            string altRowFG = string.Empty;
            string hover = string.Empty;
            string pageOptionFG = string.Empty;

            bool expected = true;

            bool actual;
            actual = target.UpdateColours(menuBarBG, menuBarFG, titleBarBG, titleBarFG, fieldBG, fieldFG, rowBG, rowFG, altRowBG, altRowFG, hover, pageOptionFG);

            Assert.AreEqual(expected, actual);

            Assert.AreNotEqual(menuBarBG, target.menubarBGColour);
            Assert.AreNotEqual(menuBarFG, target.menubarFGColour);
            Assert.AreNotEqual(titleBarBG, target.titlebarBGColour);
            Assert.AreNotEqual(titleBarFG, target.titlebarFGColour);
            Assert.AreNotEqual(fieldBG, target.fieldBG);
            Assert.AreNotEqual(fieldFG, target.fieldFG);
            Assert.AreNotEqual(rowBG, target.rowColour);
            Assert.AreNotEqual(rowFG, target.rowFGColour);
            Assert.AreNotEqual(altRowBG, target.altRowColour);
            Assert.AreNotEqual(altRowFG, target.altRowFGColour);
            Assert.AreNotEqual(hover, target.hovercolour);
            Assert.AreNotEqual(pageOptionFG, target.pageOptionFGColour);
        }

        /// <summary>
        ///A test for UpdateColours
        ///</summary>
        [TestMethod()]
        public void cColours_UpdateColours_colours()
        {
            int accountID = cGlobalVariables.AccountID;
            int subAccountID = cGlobalVariables.SubAccountID;
            Modules activeModule = Modules.contracts;
            cColours target = new cColours(accountID, subAccountID, activeModule);

            string menuBarBG = "#aaaaaa";
            string menuBarFG = "#aaaaaa";
            string titleBarBG = "#aaaaaa";
            string titleBarFG = "#aaaaaa";
            string fieldBG = "#aaaaaa";
            string fieldFG = "#aaaaaa";
            string rowBG = "#aaaaaa";
            string rowFG = "#aaaaaa";
            string altRowBG = "#aaaaaa";
            string altRowFG = "#aaaaaa";
            string hover = "#aaaaaa";
            string pageOptionFG = "#aaaaaa";

            bool expected = true;

            bool actual;
            actual = target.UpdateColours(menuBarBG, menuBarFG, titleBarBG, titleBarFG, fieldBG, fieldFG, rowBG, rowFG, altRowBG, altRowFG, hover, pageOptionFG);

            Assert.AreEqual(expected, actual);

            Assert.AreEqual(menuBarBG.ToUpper(), target.menubarBGColour);
            Assert.AreEqual(menuBarFG.ToUpper(), target.menubarFGColour);
            Assert.AreEqual(titleBarBG.ToUpper(), target.titlebarBGColour);
            Assert.AreEqual(titleBarFG.ToUpper(), target.titlebarFGColour);
            Assert.AreEqual(fieldBG.ToUpper(), target.fieldBG);
            Assert.AreEqual(fieldFG.ToUpper(), target.fieldFG);
            Assert.AreEqual(rowBG.ToUpper(), target.rowColour);
            Assert.AreEqual(rowFG.ToUpper(), target.rowFGColour);
            Assert.AreEqual(altRowBG.ToUpper(), target.altRowColour);
            Assert.AreEqual(altRowFG.ToUpper(), target.altRowFGColour);
            Assert.AreEqual(hover.ToUpper(), target.hovercolour);
            Assert.AreEqual(pageOptionFG.ToUpper(), target.pageOptionFGColour);
        }

        /// <summary>
        ///A test for customiseLogonPageColours
        ///</summary>
        [TestMethod()]
        public void cColours_customiseLogonPageColours_expenses()
        {
            Modules activemodule = Modules.expenses;
            StringBuilder expected = new StringBuilder();
            expected.Append("#logonpage { background-color: #ffffff; }");
            expected.Append("#logonpage #breadcrumbbar a { color: #ffffff; }");
            expected.Append("#logonpage #breadcrumbbar { background-color: #4A65A0; }");
            expected.Append("#logonpage #pagetitlebar { background-color: #4A65A0; color: #ffffff; }");
            expected.Append("#logonpage #logonoutercontainer { color: #002B56; }");
            expected.Append("#logonpage #logoninnercontainer { border: 1px solid #013473; background-color: #ffffff; }");
            expected.Append("#logonpage #logonrightpanel { border-left: 1px dotted #013473; }");
            expected.Append("#logonpage #logonleftpanel .divider { color: #ff0000; }");
            expected.Append("#logonpage #logonleftpanel .dividerfiller { border-bottom: 1px dashed #EFEFEF; }");
            expected.Append("#logonpage #informationcontainer { background-color: #F8F8F8; border: 1px solid #CCCCCC; color: #000000; }");
            expected.Append("#logonpage #informationcontainer h4 { color: #000000; background-color: #EAEAEA; border-bottom: 1px solid #CCCCCC; }");

            string actual;
            actual = cColours.customiseLogonPageColours(activemodule);

            Assert.AreEqual(expected.ToString(), actual);
        }

        /// <summary>
        ///A test for customiseLogonPageColours
        ///</summary>
        [TestMethod()]
        public void cColours_customiseLogonPageColours_framework()
        {
            Modules activemodule = Modules.contracts;
            StringBuilder expected = new StringBuilder();
            expected.Append("#logonpage { background-color: #ffffff; }");
            expected.Append("#logonpage #breadcrumbbar a { color: #ffffff; }");
            expected.Append("#logonpage #breadcrumbbar { background-image: none; background-color: #97ce8b; }");
            expected.Append("#logonpage #pagetitlebar { background-color: #97ce8b; color: #ffffff; background-image: none; }");
            expected.Append("#logonpage #logonoutercontainer { color: #006b51; }");
            expected.Append("#logonpage #logoninnercontainer { border: 1px solid #006b51; background-color: #ffffff; }");
            expected.Append("#logonpage #logonrightpanel { border-left: 1px dotted #006b51; }");
            expected.Append("#logonpage #logonleftpanel .divider { color: #ff0000; }");
            expected.Append("#logonpage #logonleftpanel .dividerfiller { border-bottom: 1px dashed #EFEFEF; }");
            expected.Append("#logonpage #informationcontainer { background-color: #F8F8F8; border: 1px solid #CCCCCC; color: #000000; }");
            expected.Append("#logonpage #informationcontainer h4 { color: #000000; background-color: #EAEAEA; border-bottom: 1px solid #CCCCCC; }");
            expected.Append(".tooltipcontainer .tooltipcontent { background-color: #006b51; border: solid 1px #006B51; border-left: 3px solid #00241C !important; }");

            string actual;
            actual = cColours.customiseLogonPageColours(activemodule);

            Assert.AreEqual(expected.ToString(), actual);
        }

        /// <summary>
        ///A test for customiseStyles
        ///</summary>
        [TestMethod()]
        public void cColours_customiseStyles_noParameter()
        {
            string expected = cColours.customiseStyles(true);

            string actual = cColours.customiseStyles();

            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(expected, typeof(string));
            Assert.IsInstanceOfType(actual, typeof(string));
            Assert.IsTrue(expected.Length > 0);
            Assert.IsTrue(actual.Length > 0);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for customiseStyles
        ///</summary>
        [TestMethod()]
        public void cColours_customiseStyles_parameterFalse()
        {
            bool displayLeftMenuImage = false;
            string expected = "body\n{\nbackground: url(";

            string actual = cColours.customiseStyles(displayLeftMenuImage);

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Contains(expected) == false);
        }

        /// <summary>
        ///A test for customiseStyles
        ///</summary>
        [TestMethod()]
        public void cColours_customiseStyles_parameterTrue()
        {
            bool displayLeftMenuImage = true;
            string expected = "body\n{\nbackground: url(";

            string actual = cColours.customiseStyles(displayLeftMenuImage);

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Contains(expected) == true);
        }

        /// <summary>
        ///A test for customiseStyles
        ///</summary>
        [TestMethod()]
        public void cColours_customiseStyles_defaultStyles()
        {

            int accountID = cGlobalVariables.AccountID;
            int subAccountID = cGlobalVariables.SubAccountID;
            Modules activeModule = Modules.contracts;
            cColours target = new cColours(accountID, subAccountID, activeModule);
            target.RestoreDefaults();

            bool displayLeftMenuImage = false;
            string expected = "<style type=\"text/css\">\n</style>";

            string actual = cColours.customiseStyles(displayLeftMenuImage);

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Contains(expected) == true);
            Assert.AreEqual(expected, actual);
        }

        ///// <summary>
        /////A test for altRowColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_altRowColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.altRowColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for altRowFGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_altRowFGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.altRowFGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for defaultAltRowBGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_defaultAltRowBGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.defaultAltRowBGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for defaultAltRowFGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_defaultAltRowFGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.defaultAltRowFGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for defaultFieldBG
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_defaultFieldBG_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.defaultFieldBG;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for defaultFieldFG
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_defaultFieldFG_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.defaultFieldFG;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for defaultHoverColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_defaultHoverColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.defaultHoverColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for defaultMenubarBGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_defaultMenubarBGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.defaultMenubarBGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for defaultMenubarFGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_defaultMenubarFGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.defaultMenubarFGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for defaultPageOptionFGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_defaultPageOptionFGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.defaultPageOptionFGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for defaultRowBGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_defaultRowBGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.defaultRowBGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for defaultRowFGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_defaultRowFGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.defaultRowFGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for defaultTitlebarBGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_defaultTitlebarBGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.defaultTitlebarBGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for defaultTitlebarFGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_defaultTitlebarFGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.defaultTitlebarFGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for fieldBG
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_fieldBG_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.fieldBG;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for fieldFG
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_fieldFG_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.fieldFG;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for hovercolour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_hovercolour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.hovercolour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for menubarBGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_menubarBGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.menubarBGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for menubarFGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_menubarFGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.menubarFGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for pageOptionFGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_pageOptionFGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.pageOptionFGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for rowColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_rowColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.rowColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for rowFGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_rowFGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.rowFGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for titlebarBGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_titlebarBGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.titlebarBGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for titlebarFGColour
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //public void cColours_titlebarFGColour_()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    int subAccountID = 0; // TODO: Initialize to an appropriate value
        //    Modules activeModule = new Modules(); // TODO: Initialize to an appropriate value
        //    cColours target = new cColours(accountID, subAccountID, activeModule); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.titlebarFGColour;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}
    }
}
