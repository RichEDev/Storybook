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


namespace Auto_Tests.Coded_UI_Tests.Framework.System_Options.Attachment_Types
{
    /// <summary>
    /// This class contains all of the tests for Attachment types within Framework
    /// </summary>
    [CodedUITest]
    public class AttachmentTypesTests
    {
        public AttachmentTypesTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.AttachmentTypesUIMapClasses.AttachmentTypesUIMap cAttachmentTypes = new UIMaps.AttachmentTypesUIMapClasses.AttachmentTypesUIMap();

        /// <summary>
        /// This test ensures that a new attachment type can be added
        /// </summary>
        [TestMethod]
        public void AttachmentTypesSuccessfullyCreateAttachmentType()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the attachment types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/AttachmentTypes.aspx");

            /// Add the attachment type
            cAttachmentTypes.AddAttachmentType();
            cAttachmentTypes.ValidateAddAttachmentType();
        }


        /// <summary>
        /// This test ensures that an attachment type can be archived
        /// </summary>
        [TestMethod]
        public void AttachmentTypesSuccessfullyArchiveAttachmentType()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the attachment types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/AttachmentTypes.aspx");

            /// Archive the attachment type
            cAttachmentTypes.ArchiveAttachmentType();
            cAttachmentTypes.ValidateAttachmentTypeIsArchived();
        }


        /// <summary>
        /// This test ensures that an attachment type can be un-archived
        /// </summary>
        [TestMethod]
        public void AttachmentTypesSuccessfullyUnArchiveAttachmentType()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the attachment types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/AttachmentTypes.aspx");

            /// Un-archive the attachment type
            cAttachmentTypes.UnArchiveAttachmentType();
            cAttachmentTypes.ValidateAttachmentTypeIsNotArchived();
        }


        /// <summary>
        /// This test ensures that an attachment type can be deleted
        /// </summary>
        [TestMethod]
        public void AttachmentTypesSuccessfullyDeleteAttachmentType()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the attachment types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/AttachmentTypes.aspx");

            /// Delete the attachment type
            cAttachmentTypes.DeleteAttachmentType();
            cAttachmentTypes.ValidateAttachmentTypeDoesNotExist();
        }


        /// <summary>
        /// This test ensures that a custom attachment type can be added
        /// </summary>
        [TestMethod]
        public void AttachmentTypesSuccessfullyCreateCustomAttachmentType()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the attachment types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/CustomAttachmentTypes.aspx");

            /// Add custom attachment type definition
            cAttachmentTypes.AddCustomAttachmentType();
            cAttachmentTypes.ValidateAddCustomAttachmentType();
        }


        /// <summary>
        /// This test ensures that a custom attachment type can be edited
        /// </summary>
        [TestMethod]
        public void AttachmentTypesSuccessfullyEditCustomAttachmentType()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the attachment types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/CustomAttachmentTypes.aspx");

            /// Edited custom attachment type definition
            cAttachmentTypes.EditCustomAttachmentType();
            cAttachmentTypes.ValidateEditCustomAttachmentType();
        }


        /// <summary>
        /// This test ensures that a custom attachment type can be deleted
        /// </summary>
        [TestMethod]
        public void AttachmentTypesSuccessfullyDeleteCustomAttachmentType()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the attachment types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/CustomAttachmentTypes.aspx");

            /// Edited custom attachment type definition
            cAttachmentTypes.DeleteCustomAttachmentType();
            cAttachmentTypes.ValidateCustomAttachmentTypeDoesNotExist();
        }


        /// <summary>
        /// This test ensures that an attachment type can be added using a custom definition
        /// </summary>
        [TestMethod]
        public void AttachmentTypesSuccessfullyCreateAttachmentTypeUsingCustomDefinition()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the attachment types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/CustomAttachmentTypes.aspx");

            /// Add custom attachment type definition
            //cAttachmentTypes.AddCustomAttachmentType();
            cAttachmentTypes.CreateCustomAttachmentTypeParams.UIFileExtensionEditText="__Custom";
            cAttachmentTypes.CreateCustomAttachmentType();
            string combo = cAttachmentTypes.CreateCustomAttachmentTypeParams.UIFileExtensionEditText +' '  +'-'+' ' + cAttachmentTypes.CreateCustomAttachmentTypeParams.UIDescriptionEditText;

            /// Add attachment type using the custom definition
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/AttachmentTypes.aspx");
            cAttachmentTypes.AddAttachmentTypeUsingCustomDefinitionParams.UIAttachmentFileTypeComboBox1SelectedItem = combo;
            cAttachmentTypes.AddAttachmentTypeUsingCustomDefinition();
            cAttachmentTypes.ValidateAddAttachmentTypeUsingCustomDefinition();

            /// Delete the attachment type
            cAttachmentTypes.DeleteAttachmentTypeUsingCustomDefinition();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/CustomAttachmentTypes.aspx");
            cAttachmentTypes.DeleteCustomAttachmentType();
        }



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
    }
}
