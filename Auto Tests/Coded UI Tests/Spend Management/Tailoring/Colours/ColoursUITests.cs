namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Tailoring.Colours
{
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
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using Auto_Tests.Tools;
    using Auto_Tests.UIMaps.ColoursUIMapClasses;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
    using Auto_Tests.Product_Variables.ModalMessages;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.ComponentModel;
    using Microsoft.SqlServer.Server;
    using System.Data;


    /// <summary>
    /// Summary description for ColoursUITests
    /// </summary>
    [CodedUITest]
    public class ColoursUITests
    {
        /// <summary>
        /// Colours UI map
        /// </summary>
        private ColoursUIMap _coloursMethods = new ColoursUIMap();
        /// <summary>
        /// Shared methods UI map
        /// </summary>
        private static SharedMethodsUIMap _sharedMethods = new SharedMethodsUIMap();
        /// <summary>
        /// Product that the tests will execute against
        /// </summary>
        private static ProductType _executingProduct;
        private static Colours defaultProductColours;

        public ColoursUITests()
        {
        }

        /// <summary>
        /// Sets up test suite by starting IE and logging in to the product
        /// </summary>
        /// <param name="ctx"></param>
        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            _sharedMethods = new SharedMethodsUIMap();
            _executingProduct = cGlobalVariables.GetProductFromAppConfig();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.CloseOnPlaybackCleanup = false;
            browser.Maximized = true;
            _sharedMethods.Logon(_executingProduct, LogonType.administrator);
            defaultProductColours = Colours.SetDefaultColours(_executingProduct);
        }

        /// <summary>
        /// Clean up test suite
        /// Closes browser window to deal with modal errors
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanUp()
        {
            _sharedMethods.CloseBrowserWindow();
        }

        #region 45272 - Successfully save colours, 45273 - Successfully cancel saving Colours
        /// <summary>
        /// 45272 - Successfully save colours
        /// 45273 - Successfully cancel saving colours
        /// </summary>
        [TestCategory("Colours"), TestCategory("Spend Management"), TestMethod]
        public void ColoursSuccessfullySaveColours_UITest()
        {
            //Read the original colour scheme from the database
            Colours currentProductColours = ReadDatabaseColours(_executingProduct);

            ///Navigate to colours page 
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/colours.aspx");

            #region Populate colours, Press Cancel and Verify
            //Populate controls with new colours
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarBackgroundColourEdit.Text = "000000";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarTextColourEdit.Text = "00FFFF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarBackgroundColourEdit.Text = "000099";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarTextColourEdit.Text = "FF00FF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelBackgroundColourEdit.Text = "990066";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelTextColourEdit.Text = "FFFF00";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowBackgroundColourEdit.Text = "008080";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowForegroundColourEdit.Text = "808080";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowBackgroundColourEdit.Text = "800000";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowForegroundColourEdit.Text = "FFFFFF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverBackgroundColourEdit.Text = "66FF33";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverPageOptionEdit.Text = "000000";
            _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipBackgroundColourEdit.Text = "9933CC";
            _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipTextColourEdit.Text = "FFCCFF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightLabelTextColourEdit.Text = "800080";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionTextColourEdit.Text = "FFCCFF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionBackgroundColEdit.Text = "0000CC";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionUnderlineColoEdit.Text = "CCCCFF";

            //Cancel saving colours
            _coloursMethods.PressCancel();

            //Enter the colours page and validate the colours are not saved 
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/colours.aspx");
            Assert.AreEqual(currentProductColours.MenubarBGColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarBackgroundColourEdit.Text);
            Assert.AreEqual(currentProductColours.MenubarFGColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarTextColourEdit.Text);
            Assert.AreEqual(currentProductColours.TitlebarBGColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarBackgroundColourEdit.Text);
            Assert.AreEqual(currentProductColours.TitlebarFGColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarTextColourEdit.Text);
            Assert.AreEqual(currentProductColours.FieldBG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelBackgroundColourEdit.Text);
            Assert.AreEqual(currentProductColours.FieldFG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelTextColourEdit.Text);
            Assert.AreEqual(currentProductColours.RowColourBG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowBackgroundColourEdit.Text);
            Assert.AreEqual(currentProductColours.RowColourFG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowForegroundColourEdit.Text);
            Assert.AreEqual(currentProductColours.AlternateRowColourBG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowBackgroundColourEdit.Text);
            Assert.AreEqual(currentProductColours.AlternateRowColourFG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowForegroundColourEdit.Text);
            Assert.AreEqual(currentProductColours.HoverColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverBackgroundColourEdit.Text);
            Assert.AreEqual(currentProductColours.PageOptionFGColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverPageOptionEdit.Text);
            Assert.AreEqual(currentProductColours.TooltipBGColour, _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipBackgroundColourEdit.Text);
            Assert.AreEqual(currentProductColours.TooltipTextColour, _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipTextColourEdit.Text);
            Assert.AreEqual(currentProductColours.GreenLightFieldColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightLabelTextColourEdit.Text);
            Assert.AreEqual(currentProductColours.GreenLightSectionTextColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionTextColourEdit.Text);
            Assert.AreEqual(currentProductColours.GreenLightSectionBackgroundColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionBackgroundColEdit.Text);
            Assert.AreEqual(currentProductColours.GreenLightSectionUnderlineColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionUnderlineColoEdit.Text);
            #endregion
            #region Populate colours, press Save and Verify
            //Populate controls with new colours
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarBackgroundColourEdit.Text = "000000";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarTextColourEdit.Text = "00FFFF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarBackgroundColourEdit.Text = "000099";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarTextColourEdit.Text = "FF00FF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelBackgroundColourEdit.Text = "990066";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelTextColourEdit.Text = "FFFF00";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowBackgroundColourEdit.Text = "008080";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowForegroundColourEdit.Text = "808080";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowBackgroundColourEdit.Text = "800000";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowForegroundColourEdit.Text = "FFFFFF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverBackgroundColourEdit.Text = "66FF33";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverPageOptionEdit.Text = "000000";
            _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipBackgroundColourEdit.Text = "9933CC";
            _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipTextColourEdit.Text = "FFCCFF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightLabelTextColourEdit.Text = "800080";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionTextColourEdit.Text = "FFCCFF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionBackgroundColEdit.Text = "0000CC";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionUnderlineColoEdit.Text = "CCCCFF";
            
            //Save colours
            _coloursMethods.PressSave();

            //Enter the colours page and validate the colours saved 
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/colours.aspx");

            //Verify colours
            Assert.AreEqual("000000", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarBackgroundColourEdit.Text);
            Assert.AreEqual("00FFFF", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarTextColourEdit.Text);
            Assert.AreEqual("000099", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarBackgroundColourEdit.Text);
            Assert.AreEqual("FF00FF",  _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarTextColourEdit.Text);
            Assert.AreEqual("990066", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelBackgroundColourEdit.Text);
            Assert.AreEqual("FFFF00", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelTextColourEdit.Text);
            Assert.AreEqual("008080", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowBackgroundColourEdit.Text);
            Assert.AreEqual("808080", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowForegroundColourEdit.Text);
            Assert.AreEqual("800000", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowBackgroundColourEdit.Text);
            Assert.AreEqual("FFFFFF", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowForegroundColourEdit.Text);
            Assert.AreEqual("66FF33", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverBackgroundColourEdit.Text);
            Assert.AreEqual("000000", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverPageOptionEdit.Text);
            Assert.AreEqual("9933CC", _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipBackgroundColourEdit.Text );
            Assert.AreEqual("FFCCFF", _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipTextColourEdit.Text);
            Assert.AreEqual("800080", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightLabelTextColourEdit.Text);
            Assert.AreEqual("FFCCFF",_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionTextColourEdit.Text);
            Assert.AreEqual("0000CC", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionBackgroundColEdit.Text);
            Assert.AreEqual("CCCCFF", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionUnderlineColoEdit.Text);
            #endregion
        }
        #endregion

        #region 45274 - Successfully restore default Colours, 45275 - Successfully cancel restoring default Colours
        /// <summary>
        /// 45274 - Successfully restore default Colours
        /// 45275 - Successfully cancel restoring default Colours
        /// </summary>
        [TestCategory("Colours"), TestCategory("Spend Management"), TestMethod]
        public void ColoursSuccessfullyRestoreDefaultColours_UITest()
        {
            ///Navigate to colours page 
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/colours.aspx");

            //Populate controls with new colours
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarBackgroundColourEdit.Text = "000000";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarTextColourEdit.Text = "00FFFF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarBackgroundColourEdit.Text = "000099";
             _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarTextColourEdit.Text = "FF00FF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelBackgroundColourEdit.Text = "990066";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelTextColourEdit.Text = "FFFF00";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowBackgroundColourEdit.Text = "008080";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowForegroundColourEdit.Text= "808080";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowBackgroundColourEdit.Text= "800000";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowForegroundColourEdit.Text = "FFFFFF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverBackgroundColourEdit.Text = "66FF33";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverPageOptionEdit.Text = "000000";
            _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipBackgroundColourEdit.Text = "9933CC";
            _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipTextColourEdit.Text = "FFCCFF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightLabelTextColourEdit.Text = "800080";
           _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionTextColourEdit.Text = "FFCCFF";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionBackgroundColEdit.Text = "0000CC";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionUnderlineColoEdit.Text = "CCCCFF";
           
            //Press Restore Default Colours 
            _coloursMethods.PressRestoreDefault();

            //Validate Default colours message
            _coloursMethods.ValidateRestoreDefaultsMessage();

            //Press Cancel to prevent restoring the default colours
            _coloursMethods.PressCancelToStopRestoringDefaults();

            //Validate the controls kept their original values
            Assert.AreEqual("000000", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarBackgroundColourEdit.Text);
            Assert.AreEqual("00FFFF", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarTextColourEdit.Text);
            Assert.AreEqual("000099", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarBackgroundColourEdit.Text);
            Assert.AreEqual("FF00FF",  _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarTextColourEdit.Text);
            Assert.AreEqual("990066", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelBackgroundColourEdit.Text);
            Assert.AreEqual("FFFF00", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelTextColourEdit.Text);
            Assert.AreEqual("008080", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowBackgroundColourEdit.Text);
            Assert.AreEqual("808080", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowForegroundColourEdit.Text);
            Assert.AreEqual("800000", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowBackgroundColourEdit.Text);
            Assert.AreEqual("FFFFFF", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowForegroundColourEdit.Text);
            Assert.AreEqual("66FF33", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverBackgroundColourEdit.Text);
            Assert.AreEqual("000000", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverPageOptionEdit.Text);
            Assert.AreEqual("9933CC", _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipBackgroundColourEdit.Text);
            Assert.AreEqual("FFCCFF", _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipTextColourEdit.Text);
            Assert.AreEqual("800080", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightLabelTextColourEdit.Text);
            Assert.AreEqual("FFCCFF",_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionTextColourEdit.Text);
            Assert.AreEqual("0000CC", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionBackgroundColEdit.Text);
            Assert.AreEqual("CCCCFF", _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionUnderlineColoEdit.Text);

            //Press Restore Default Colours 
            _coloursMethods.PressRestoreDefault();

            //Press OK to restore the default colours
            _coloursMethods.PressOKToRestoreDefaults();

            //Validate the controls returned to the default colours
            Assert.AreEqual(defaultProductColours.MenubarBGColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.MenubarFGColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarTextColourEdit.Text);
            Assert.AreEqual(defaultProductColours.TitlebarBGColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.TitlebarFGColour,  _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarTextColourEdit.Text);
            Assert.AreEqual(defaultProductColours.FieldBG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.FieldFG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelTextColourEdit.Text);
            Assert.AreEqual(defaultProductColours.RowColourBG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.RowColourFG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowForegroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.AlternateRowColourBG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.AlternateRowColourFG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowForegroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.HoverColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.PageOptionFGColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverPageOptionEdit.Text);
            Assert.AreEqual(defaultProductColours.TooltipBGColour, _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.TooltipTextColour, _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipTextColourEdit.Text);
            Assert.AreEqual(defaultProductColours.GreenLightFieldColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightLabelTextColourEdit.Text);
            Assert.AreEqual(defaultProductColours.GreenLightSectionTextColour,_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionTextColourEdit.Text);
            Assert.AreEqual(defaultProductColours.GreenLightSectionBackgroundColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionBackgroundColEdit.Text);
            Assert.AreEqual(defaultProductColours.GreenLightSectionUnderlineColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionUnderlineColoEdit.Text);
         
        }
        #endregion

        #region 45271 - Successfully verify colours save their default values where mandatory fields are missing
        /// <summary>
        /// 45271 - Successfully verify colours save their default values where mandatory fields are missing
        /// </summary>
        [TestCategory("Colours"), TestCategory("Spend Management"), TestMethod]
        public void ColoursSuccessfullyVerifyColoursSaveDefaultValuesWhenMandatoryFieldsAreMissing_UITest()
        {
            ///Navigate to colours page 
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/colours.aspx");

            //Populate controls with new colours
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarBackgroundColourEdit.Text = string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarTextColourEdit.Text = string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarBackgroundColourEdit.Text = string.Empty;
             _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarTextColourEdit.Text = string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelBackgroundColourEdit.Text = string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelTextColourEdit.Text = string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowBackgroundColourEdit.Text = string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowForegroundColourEdit.Text= string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowBackgroundColourEdit.Text= string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowForegroundColourEdit.Text = string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverBackgroundColourEdit.Text = string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverPageOptionEdit.Text = string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipBackgroundColourEdit.Text = string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipTextColourEdit.Text = string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightLabelTextColourEdit.Text = string.Empty;
           _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionTextColourEdit.Text = string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionBackgroundColEdit.Text = string.Empty;
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionUnderlineColoEdit.Text = string.Empty;

            //Press save
            _coloursMethods.PressSave();

            //Enter the colours page and validate the colours are not saved 
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/colours.aspx");

            //Verify colours
            Assert.AreEqual(defaultProductColours.MenubarBGColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.MenubarFGColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarTextColourEdit.Text);
            Assert.AreEqual(defaultProductColours.TitlebarBGColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.TitlebarFGColour,  _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarTextColourEdit.Text);
            Assert.AreEqual(defaultProductColours.FieldBG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.FieldFG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelTextColourEdit.Text);
            Assert.AreEqual(defaultProductColours.RowColourBG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.RowColourFG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowForegroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.AlternateRowColourBG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.AlternateRowColourFG, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowForegroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.HoverColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.PageOptionFGColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverPageOptionEdit.Text);
            Assert.AreEqual(defaultProductColours.TooltipBGColour, _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipBackgroundColourEdit.Text);
            Assert.AreEqual(defaultProductColours.TooltipTextColour, _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipTextColourEdit.Text);
            Assert.AreEqual(defaultProductColours.GreenLightFieldColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightLabelTextColourEdit.Text);
            Assert.AreEqual(defaultProductColours.GreenLightSectionTextColour,_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionTextColourEdit.Text);
            Assert.AreEqual(defaultProductColours.GreenLightSectionBackgroundColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionBackgroundColEdit.Text);
            Assert.AreEqual(defaultProductColours.GreenLightSectionUnderlineColour, _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionUnderlineColoEdit.Text);
        }
        #endregion

        #region 29943 - Unsuccessfully save Colours where Invalid data are used
        /// <summary>
        /// 29943 - Unsuccessfully save Colours where Invalid data are used
        /// </summary>
        [TestCategory("Colours"), TestCategory("Spend Management"), TestMethod]
        public void ColoursUnsuccessfullySaveColoursWhereInvalidDataAreSet_UITest()
        {
            ///Navigate to colours page 
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/colours.aspx");

            //Validate red asterisks are not displayed
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.MenuBarBackgroundColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.MenuBarTextColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.TitleBarBackgroundColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.TitleBarTextColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.FieldLabelBackgroundColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.FieldLabelTextColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.TableRowBackgroundColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.TableRowForegroundColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.TableAlternateRowBackgroundColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.TableAlternateRowForegroundColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.HoverColourBackgroundColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.HoverColourPageOptionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipBackgroundColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipTextColourAsterisk));  
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.GreenlightLabelTextColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.GreenlightSectionBackgroundColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.GreenlightSectionTextColourAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.GreenlightSectionUnderlineColourAsterisk));

            //Populate controls with new colours
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarBackgroundColourEdit.Text = "Test";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIMenuBarTextColourEdit.Text = "Test";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarBackgroundColourEdit.Text = "Test";
             _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITitleBarTextColourEdit.Text = "Test";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelBackgroundColourEdit.Text = "Test";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIFieldLabelTextColourEdit.Text = "Test";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowBackgroundColourEdit.Text = "Test";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableRowForegroundColourEdit.Text= "Test";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowBackgroundColourEdit.Text= "Test";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UITableAlternateRowForegroundColourEdit.Text = "Test";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverBackgroundColourEdit.Text = "Test";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIHoverPageOptionEdit.Text = "Test";
            _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipBackgroundColourEdit.Text = "Test";
            _coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipTextColourEdit.Text = "Test";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightLabelTextColourEdit.Text = "Test";
           _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionTextColourEdit.Text = "Test";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionBackgroundColEdit.Text = "Test";
            _coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.UIGreenlightSectionUnderlineColoEdit.Text = "Test";
            
            //Press save
            _coloursMethods.PressSave();

            //Validate invalid data modal is displayed
            _coloursMethods.ValidateInvalidColoursModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), ColoursModalMessages.InvalidColours});
            _coloursMethods.ValidateInvalidColoursModal();

            //close validation modal
            _coloursMethods.PressCloseInvalidColoursModal();

            //Validate red asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.MenuBarBackgroundColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.MenuBarTextColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.TitleBarBackgroundColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.TitleBarTextColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.FieldLabelBackgroundColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.FieldLabelTextColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.TableRowBackgroundColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.TableRowForegroundColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.TableAlternateRowBackgroundColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.TableAlternateRowForegroundColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.HoverColourBackgroundColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.HoverColourPageOptionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipBackgroundColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow1.UIColoursDocument.TooltipTextColourAsterisk));  
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.GreenlightLabelTextColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.GreenlightSectionBackgroundColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.GreenlightSectionTextColourAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_coloursMethods.UIColoursWindowsInternWindow.UIColoursDocument.GreenlightSectionUnderlineColourAsterisk));

        }
        #endregion

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

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

        /// <summary>
        /// Querys the product database and returns an object with the current colour scheme
        /// </summary>
        /// <param name="executingProduct"></param>
        /// <returns></returns>
        private Colours ReadDatabaseColours(ProductType executingProduct)
        {
            Colours colours = new Colours();
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));

            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader("SELECT stringkey, stringValue FROM accountProperties WHERE stringkey like '%colours%'"))
            {
                while (reader.Read())
                {
                    string stringKey = reader.GetString(0);
                    #region Read the colours from the database and store them in the colours object. If the database returns null stores the default colour
                    switch (stringKey)
                    {
                        case "coloursAlternateRowBackground":
                             colours.AlternateRowColourBG = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1)))? defaultProductColours.AlternateRowColourBG : reader.GetString(1);
                            break;
                        case "coloursAlternateRowForeground":
                            colours.AlternateRowColourFG = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.AlternateRowColourFG : reader.GetString(1);
                            break;
                        case "coloursFieldBackground":
                            colours.FieldBG = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.FieldBG : reader.GetString(1);
                            break;
                        case "coloursFieldForeground":
                            colours.FieldFG = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.FieldFG : reader.GetString(1);
                            break;
                        case "coloursGreenLightField":
                            colours.GreenLightFieldColour = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.GreenLightFieldColour : reader.GetString(1);
                            break;
                        case "coloursGreenLightSectionBackground":
                            colours.GreenLightSectionBackgroundColour = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.GreenLightSectionBackgroundColour : reader.GetString(1);
                            break;
                        case "coloursGreenLightSectionText":
                            colours.GreenLightSectionTextColour = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.GreenLightSectionTextColour : reader.GetString(1);
                            break;
                        case "coloursGreenLightSectionUnderline":
                            colours.GreenLightSectionUnderlineColour = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.GreenLightSectionUnderlineColour : reader.GetString(1);
                            break;
                        case "coloursHover":
                            colours.HoverColour = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.HoverColour : reader.GetString(1);
                            break;
                        case "coloursMenuBarBackground":
                            colours.MenubarBGColour = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.MenubarBGColour : reader.GetString(1);
                            break;
                        case "coloursMenuBarForeground":
                            colours.MenubarFGColour = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.MenubarFGColour : reader.GetString(1);
                            break;
                        case "coloursPageOptionForeground":
                            colours.PageOptionFGColour = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.PageOptionFGColour : reader.GetString(1);
                            break;
                        case "coloursRowBackground":
                            colours.RowColourBG = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.RowColourBG : reader.GetString(1);
                            break;
                        case "coloursRowForeground":
                            colours.RowColourFG = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.RowColourFG : reader.GetString(1);
                            break;
                        case "coloursTitleBarBackground":
                            colours.TitlebarBGColour = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.TitlebarBGColour : reader.GetString(1);
                            break;
                        case "coloursTitleBarForeground":
                            colours.TitlebarFGColour = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.TitlebarFGColour : reader.GetString(1);
                            break;
                        case "coloursTooltipBackground":
                            colours.TooltipBGColour = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.TooltipBGColour : reader.GetString(1);
                            break;
                        case "coloursTooltipText":
                            colours.TooltipTextColour = (reader.IsDBNull(1) || string.IsNullOrEmpty(reader.GetString(1))) ? defaultProductColours.TooltipTextColour : reader.GetString(1);
                            break;
                        default:
                            break;
                    }
                    #endregion
                }
                reader.Close();
            }
            return colours;
        }
    }
}
