namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Tailoring.User_Defined_Fields
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

    using Auto_Tests.UIMaps.EmployeesNewUIMapClasses;
    using Auto_Tests.UIMaps.ESRAssignmentsUIMapClasses;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
    using Auto_Tests.UIMaps.UserDefinedFieldsNewUIMapClasses;
    using Auto_Tests.Product_Variables.PageUrlSurfices;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using Auto_Tests.Tools;
    using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Attributes;
    using Auto_Tests.Product_Variables.ModalMessages;

    /// <summary>
    /// Summary description for UserDefinedFieldsUITests
    /// </summary>
    [CodedUITest]
    public class UserDefinedFieldsUITests
    {
        /// <summary>
        /// Current Product in test run
        /// </summary>
        private static readonly ProductType ExecutingProduct = cGlobalVariables.GetProductFromAppConfig();

        /// <summary>
        /// Shared methods UI Map
        /// </summary>
        private static SharedMethodsUIMap sharedMethods = new SharedMethodsUIMap();

        /// <summary>
        /// employees methods UI Map
        /// </summary>
        private static EmployeesNewUIMap employeesMethods;

        /// <summary>
        /// assignments methods UI Map
        /// </summary>
        private static ESRAssignmentsUIMap esrAssignmentsMethods;

        /// <summary>
        /// The user defined fields methods UI Map
        /// </summary>
        private static UserDefinedFieldsNewUIMap userDefinedFieldsMethods;

        /// <summary>
        /// Cached list of userdefined fields
        /// </summary>
        private static List<UserDefinedFields> userDefinedFields;

        /// <summary>
        /// Administrator employee number
        /// </summary>
        public static int adminId;

        #region Additional class/test attributes

        // Use ClassInitialize to run code before each test has run
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.Maximized = true;
            browser.CloseOnPlaybackCleanup = false;
            sharedMethods.Logon(ExecutingProduct, LogonType.administrator);
            adminId = AutoTools.GetEmployeeIDByUsername(ExecutingProduct);
            userDefinedFields = UserDefinedFieldsRepository.PopulateUserDefinedFields();
        }

        // Use TestCleanup to run code after each test has run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            sharedMethods.CloseBrowserWindow();
        }

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            employeesMethods = new EmployeesNewUIMap();
            esrAssignmentsMethods = new ESRAssignmentsUIMap();
            userDefinedFieldsMethods = new UserDefinedFieldsNewUIMap();
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach (var userdefinedField in userDefinedFields)
            {
                UserDefinedFieldsRepository.DeleteUserDefinedField(userdefinedField, ExecutingProduct);
            }
        }
        #endregion

        /// <summary>
        /// user defined fields unsuccessfully create userdefined fields of all types with mandatory fields blank
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsUnsuccessfullyCreateUserDefinedFieldsOfAllTypesWithMandatoryFieldsBlank_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[0];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // set appliest to field
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));


            // foreach 
            for (int index = 0; index < 13; index++)
            {
                // set each feield type
                userdefinedFieldToAdd = userDefinedFields[index];
                if (userdefinedFieldToAdd._fieldType == FieldType.Number)
                {
                    userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = "Decimal";

                }
                else
                {
                    userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
                }

                // press save
                userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

                // validate mandatory field for each field type
                switch (userdefinedFieldToAdd._fieldType)
                {
                    case FieldType.Text:
                    case FieldType.Integer:
                    case FieldType.Number:
                    case FieldType.Currency:
                    case FieldType.List:
                    case FieldType.DateTime:
                    case FieldType.LargeText:
                        userDefinedFieldsMethods.ValidateModalMessageExpectedValues.ModalMessagePaneDisplayText = String.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), UserDefinedFieldsModalMessages.EmptyFieldForDisplayName });
                        userDefinedFieldsMethods.ValidateModalMessage();
                        break;
                    case FieldType.Hyperlink:
                        userDefinedFieldsMethods.ValidateModalMessageExpectedValues.ModalMessagePaneDisplayText = String.Format("Message from {0}\r\n\r\n{1}{2}{3}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), UserDefinedFieldsModalMessages.EmptyFieldForDisplayName, UserDefinedFieldsModalMessages.EmptyFieldForHyperLinkText, UserDefinedFieldsModalMessages.EmptyFieldForHyperLinkPath });
                        userDefinedFieldsMethods.ValidateModalMessage();
                        break;
                    case FieldType.TickBox:
                        userDefinedFieldsMethods.ValidateModalMessageExpectedValues.ModalMessagePaneDisplayText = String.Format("Message from {0}\r\n\r\n{1}{2}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), UserDefinedFieldsModalMessages.EmptyFieldForDisplayName, UserDefinedFieldsModalMessages.EmptyFieldForDefaultValue });
                        userDefinedFieldsMethods.ValidateModalMessage();
                        break;
                    case FieldType.Relationship:
                        userDefinedFieldsMethods.ValidateModalMessageExpectedValues.ModalMessagePaneDisplayText = String.Format("Message from {0}\r\n\r\n{1}{2}{3}{4}", new object[] { EnumHelper.GetEnumDescription(ExecutingProduct), UserDefinedFieldsModalMessages.EmptyFieldForDisplayName, UserDefinedFieldsModalMessages.EmptyFieldForRelatedTable, UserDefinedFieldsModalMessages.EmptyFieldForDisplayField, UserDefinedFieldsModalMessages.EmptyFieldForMatchField });
                        userDefinedFieldsMethods.ValidateModalMessage();
                        break;

                }

                // press close modal message
                userDefinedFieldsMethods.PressCloseOnModalMessage();
            }
        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type single line text
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCancelCreateUserDefinedField_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[0];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextMaxLengthTextBox.Text = userdefinedFieldToAdd._maxLength.ToString();
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextFormatComboBox.SelectedItem = EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format);

            // press cancel
            userDefinedFieldsMethods.PressCancelUserDefinedFieldsButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues, false);

        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type single line text
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCreateUserDefinedFieldOfTypeSingleLineText_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[0];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextMaxLengthTextBox.Text = userdefinedFieldToAdd._maxLength.ToString();
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextFormatComboBox.SelectedItem = EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format);

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);
            Assert.AreEqual(userdefinedFieldToAdd._maxLength, Int32.Parse(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextMaxLengthTextBox.Text));
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextFormatComboBox.SelectedItem);

        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type multiline text
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCreateUserDefinedFieldOfTypeMultiLineText_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[1];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextMaxLengthTextBox.Text = userdefinedFieldToAdd._maxLength.ToString();
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextFormatComboBox.SelectedItem = EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format);

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);
            Assert.AreEqual(userdefinedFieldToAdd._maxLength, Int32.Parse(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextMaxLengthTextBox.Text));
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextFormatComboBox.SelectedItem);

        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type integer
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCreateUserDefinedFieldOfTypeInteger_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[2];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);

        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type decimal
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCreateUserDefinedFieldOfTypeDecimal_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[3];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            if (EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType) == "Number")
            {
                userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = "Decimal";
            }
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DecimalPrecisionTextBox.Text = userdefinedFieldToAdd._precision.ToString();

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual("Decimal", userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);
            Assert.AreEqual(userdefinedFieldToAdd._precision.ToString(), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DecimalPrecisionTextBox.Text);
        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type currency
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCreateUserDefinedFieldOfTypeCurrency_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[4];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);

        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type YesNo
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCreateUserDefinedFieldOfTypeYesNo_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[5];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DefaultValueComboBox.SelectedItem = userdefinedFieldToAdd._defaultValue;

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            var gridValues = userdefinedFieldToAdd.UserDefinedFieldsGridValues;
            gridValues.RemoveAt(2);
            gridValues.Insert(2, "Tick Box");
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, gridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._defaultValue, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DefaultValueComboBox.SelectedItem);

        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type list
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCreateUserDefinedFieldOfTypeList_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = (UserDefinedFieldTypeList)userDefinedFields[8];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;

            foreach (var listItem in userdefinedFieldToAdd.UserDefinedFieldListItems)
            {
                Mouse.Click(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.ListItemsPane.AddListItemImage);
                userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.ListItemTextBox.Text = listItem._textItem;
                userDefinedFieldsMethods.PressSaveUserDefinedFieldListItemButton();
            }

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);
            int index = 0;
            foreach (HtmlListItem listItem in userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.ListItemsListBox.Items)
            {
                Assert.AreEqual(listItem.DisplayText, userdefinedFieldToAdd.UserDefinedFieldListItems[index++]._textItem);
            }
        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type date
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCreateUserDefinedFieldOfTypeDate_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[7];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DateFormatComboBox.SelectedItem = EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format);

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DateFormatComboBox.SelectedItem);

        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type time
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCreateUserDefinedFieldOfTypeTime_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[9];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DateFormatComboBox.SelectedItem = EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format);

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DateFormatComboBox.SelectedItem);

        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type date time
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCreateUserDefinedFieldOfTypeDateTime_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[12];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DateFormatComboBox.SelectedItem = EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format);

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DateFormatComboBox.SelectedItem);

        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type multiline large text
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCreateUserDefinedFieldOfTypeMultiLineLargeText_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[6];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.LTextMaxLengthTextBox.Text = userdefinedFieldToAdd._maxLength.ToString();
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.LTextFormatComboBox.SelectedItem = EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format);

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);
            Assert.AreEqual(userdefinedFieldToAdd._maxLength, Int32.Parse(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.LTextMaxLengthTextBox.Text));
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.LTextFormatComboBox.SelectedItem);

        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type large formatted text
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCreateUserDefinedFieldOfTypeLargeFormattedText_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[10];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.LTextMaxLengthTextBox.Text = userdefinedFieldToAdd._maxLength.ToString();
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.LTextFormatComboBox.SelectedItem = EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format);

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);
            Assert.AreEqual(userdefinedFieldToAdd._maxLength, Int32.Parse(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.LTextMaxLengthTextBox.Text));
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.LTextFormatComboBox.SelectedItem);

        }

        /// <summary>
        /// user defined fields successfully create userdefined field of type hyperlink
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyCreateUserDefinedFieldOfTypeHyperLink_UITest()
        {
            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[11];

            // press new userdefined field
            userDefinedFieldsMethods.ClickNewUserDefinedFieldLink();

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem = EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId));
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType);
            // userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.HyperlinkTextBox.Text = userdefinedFieldToAdd.HyperLinkText;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.HyperlinkPathTextBox.Text = userdefinedFieldToAdd.HyperLinkPath;
          
            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            // Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);
            Assert.AreEqual(userdefinedFieldToAdd.HyperLinkText, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.HyperlinkTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd.HyperLinkPath, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.HyperlinkPathTextBox.Text);

        }

        /// <summary>
        /// user defined fields successfully delete userdefined field
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyDeleteUserDefinedField_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            var userDefinedFieldToDelete = userDefinedFields[0];

            // navigate to userdefined fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);

            // press delete userdefined field
            esrAssignmentsMethods.ClickDeleteGridRow(this.UserDefinedFieldsGrid, userDefinedFieldToDelete.DisplayName);

            // confirm delete user defined field
            employeesMethods.PressOkOnBrowserValidation();

            // validate user defined fields grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userDefinedFieldToDelete.UserDefinedFieldsGridValues, false);
        }

        /// <summary>
        /// user defined fields successfully edit userdefined field of type text
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyEditUserDefinedFieldOfTypeText_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToEdit = userDefinedFields[0];
            var userdefinedFieldToAdd = userDefinedFields[1];

            // press edit udf
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToEdit.DisplayName);

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            Assert.IsTrue(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.Enabled == false);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            Assert.IsTrue(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.Enabled == false);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextMaxLengthTextBox.Text = userdefinedFieldToAdd._maxLength.ToString();
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextFormatComboBox.SelectedItem = EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format);

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);
            Assert.AreEqual(userdefinedFieldToAdd._maxLength, Int32.Parse(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextMaxLengthTextBox.Text));
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TextFormatComboBox.SelectedItem);

        }

        /// <summary>
        /// user defined fields successfully edit userdefined field of large type text
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyEditUserDefinedFieldOfTypeLargeText_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToEdit = userDefinedFields[6];
            var userdefinedFieldToAdd = userDefinedFields[10];

            // press edit udf
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToEdit.DisplayName);

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            Assert.IsTrue(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.Enabled == false);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            Assert.IsTrue(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.Enabled == false);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.LTextMaxLengthTextBox.Text = userdefinedFieldToAdd._maxLength.ToString();
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.LTextFormatComboBox.SelectedItem = EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format);

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);
            Assert.AreEqual(userdefinedFieldToAdd._maxLength, Int32.Parse(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.LTextMaxLengthTextBox.Text));
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)userdefinedFieldToAdd._format), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.LTextFormatComboBox.SelectedItem);

        }

        /// <summary>
        /// user defined fields successfully edit userdefined field of type date
        /// </summary>
        [TestCategory("User Defined Field Details"), TestCategory("User Defined Fields"), TestCategory("Spend Management"), TestMethod]
        public void UserDefinedFieldsSuccessfullyEditUserDefinedFieldOfTypeDate_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            // navigate to userdefine fields
            sharedMethods.NavigateToPage(ExecutingProduct, UserDefinedFieldsUrlSuffixes.UserDefinedFieldsUrl);
            //sharedMethods.UIGoogleWindowsInterneWindow.LaunchUrl(

            var userdefinedFieldToAdd = userDefinedFields[9];
            var userdefinedFieldToEdit = userDefinedFields[7];

            // press edit udf
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToEdit.DisplayName);

            // populate all fields
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text = userdefinedFieldToAdd.DisplayName;
            Assert.IsTrue(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.Enabled == false);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text = userdefinedFieldToAdd._description;
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text = userdefinedFieldToAdd._tooltip;
            Assert.IsTrue(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.Enabled == false);
            userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked = userdefinedFieldToAdd._mandatory;
            Assert.IsTrue(userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DateFormatComboBox.Enabled == false);

            // press save
            userDefinedFieldsMethods.PressSaveUserDefinedFieldButton();

            // validate udf grid
            esrAssignmentsMethods.ValidateGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.UserDefinedFieldsGridValues);

            // press edit udf
            userdefinedFieldToAdd._attributeid = esrAssignmentsMethods.ReturnIdFromGrid(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);
            esrAssignmentsMethods.ClickEditGridRow(this.UserDefinedFieldsGrid, userdefinedFieldToAdd.DisplayName);

            // verify udf details
            Assert.AreEqual(userdefinedFieldToAdd.DisplayName, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DisplayNameTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(userdefinedFieldToAdd.GetFriendlyName(userdefinedFieldToAdd.TableId)), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.AppliesToComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._description, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(userdefinedFieldToAdd._tooltip, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TooltipTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((FieldType)userdefinedFieldToAdd._fieldType), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.TypeComboBox.SelectedItem);
            Assert.AreEqual(userdefinedFieldToAdd._mandatory, userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.MandatoryCheckBox.Checked);
            
            // verify format unchanged
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)userdefinedFieldToEdit._format), userDefinedFieldsMethods.UserDefinedFieldControlsWindow.UserDefinedFieldControlsDocument.DateFormatComboBox.SelectedItem);

        }

        /// <summary>
        /// UserDefined Fields Grid
        /// </summary>
        public HtmlTable UserDefinedFieldsGrid
        {
            get
            {
                return userDefinedFieldsMethods.UserDefinedFieldLinksGridWindow.UserDefinedFieldGridDocument.UserDefinedFieldsGrid;
            }
        }

        private void ImportDataToTestingDatabase(string testName)
        {
            int userId;
            switch (testName)
            {
                case "UserDefinedFieldsSuccessfullyEditUserDefinedFieldOfTypeText_UITest":
                case "UserDefinedFieldsSuccessfullyDeleteUserDefinedField_UITest":
                    userId = UserDefinedFieldsRepository.CreateUserDefinedField(userDefinedFields[0], ExecutingProduct);
                    Assert.IsTrue(userId > 0);
                    break;
                case "UserDefinedFieldsSuccessfullyEditUserDefinedFieldOfTypeDate_UITest":
                    userId = UserDefinedFieldsRepository.CreateUserDefinedField(userDefinedFields[7], ExecutingProduct);
                    Assert.IsTrue(userId > 0);
                    break;
                case "UserDefinedFieldsSuccessfullyEditUserDefinedFieldOfTypeLargeText_UITest":
                    userId = UserDefinedFieldsRepository.CreateUserDefinedField(userDefinedFields[6], ExecutingProduct);
                    Assert.IsTrue(userId > 0);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
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
