namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Imports_Exports.NHSTrusts
{
    using System;
    using System.Collections.Generic;

    using Auto_Tests.Product_Variables.ModalMessages;
    using Auto_Tests.Product_Variables.PageUrlSurfices;
    using Auto_Tests.Tools;
    using Auto_Tests.UIMaps.NHSTrustsUIMapClasses;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;

    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    /// <summary>
    /// Summary description for NHSTrustDetails
    /// </summary>
    [CodedUITest]
    public class NHSTrustsUITests
    {
        /// <summary>
        /// Current Product in test run
        /// </summary>
        private static readonly  ProductType _executingProduct = cGlobalVariables.GetProductFromAppConfig();

        /// <summary>
        /// Shared methods UI Map
        /// </summary>
        private static SharedMethodsUIMap _sharedMethods = new SharedMethodsUIMap();

        /// <summary>
        /// employees methods UI Map
        /// </summary>
        private static NHSTrustsUIMap nhsTrustsMethods;

        /// <summary>
        /// Cached list of Employees
        /// </summary>
        private static List<NHSTrusts> nhsTrusts;

        /// <summary>
        /// Administrator employee number
        /// </summary>
        private static int adminId;

        #region Additional class/test attributes
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.Maximized = true;
            browser.CloseOnPlaybackCleanup = false;
            _sharedMethods.Logon(_executingProduct, LogonType.administrator);
            adminId = AutoTools.GetEmployeeIDByUsername(_executingProduct);
            nhsTrusts = NHSTrustsRepository.PopulateNhsTrust();
        }

        // Use TestCleanup to run code after each test has run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            _sharedMethods.CloseBrowserWindow();
        }
        
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Guid genericName;
            nhsTrustsMethods = new NHSTrustsUIMap();
            foreach (var nhsTrust in nhsTrusts)
            {
                genericName = Guid.NewGuid();
                nhsTrust.TrustName = string.Format(
                    "{0}{1}", new object[] { "Custom Trust ", genericName });
            }
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach (NHSTrusts NHSTrust in nhsTrusts)
            {
                if (NHSTrust.TrustID > 0)
                {
                    NHSTrustsRepository.DeleteNhsTrusts(NHSTrust.TrustID, _executingProduct, adminId);
                }
            }
        }
        #endregion

        #region successfully add new NHSTrust
        /// <summary>
        /// This test ensures a NHS Trust can be added within Expenses
        /// </summary>
        [TestCategory("NHSTrust Details"), TestCategory("NHSTrusts"), TestCategory("Expenses"), TestMethod]
        public void NHSTrustsSuccessfullyAddNewNHSTrust_UITest() 
        {
            // Navigate to NHS Trust Details page
            _sharedMethods.NavigateToPage(_executingProduct, NHSTustUrlSuffixes.NHSTustUrl);

            #region add nhs trusts and valiate grid
            foreach (NHSTrusts trustToAdd in nhsTrusts)
            {
                // Click New NHS Trust
                nhsTrustsMethods.ClickNewTrustLink();

                var periodType = FriendlyName.GetPeriodTypeFriendlyName(trustToAdd.PeriodType);
                var periodRun = FriendlyName.GetPeriodRunFriendlyName(trustToAdd.PeriodRun);
                var interfaceVersion = EnumHelper.GetEnumDescription((ESRInterfaceVersion)trustToAdd.ESRInterfaceVersion);

               // Populate fields           
                nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustNameTextBox.Text = trustToAdd.TrustName;
                nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustVPDTextBox.Text = trustToAdd.TrustVPD;
                nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.PeriodTypeComboBox.SelectedItem = periodType;
                nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.PeriodRunComboBox.SelectedItem = periodRun;
                nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.FTPAddressTextBox.Text = trustToAdd.FtpAddress;
                nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.FTPUsernameTextBox.Text = trustToAdd.FtpUsername;
                nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.FTPPasswordTextBox.Text = Playback.EncryptText(trustToAdd.FtpPassword);
                nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.RunSequenceTextBox.Text = trustToAdd.RunSequenceNumber.ToString();
                nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.DelimiterCharacterTextBox.Text = trustToAdd.DelimiterCharacter;
                nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.ESRInterfaceVersionComboBox.SelectedItem = interfaceVersion;
        
                // Press Save
                nhsTrustsMethods.PressSaveNHSTrust();

                // Validate NHS Trust is added in the grid  
                nhsTrustsMethods.ValidateNHSTrustGrid(trustToAdd.TrustName, trustToAdd.TrustVPD, trustToAdd.RunSequenceNumber.ToString(), interfaceVersion);
            }
            #endregion

            #region edit nhs trusts and validate fields
            foreach (NHSTrusts trustToEdit in nhsTrusts)
            {
                var periodType = FriendlyName.GetPeriodTypeFriendlyName(trustToEdit.PeriodType);
                var periodRun = FriendlyName.GetPeriodRunFriendlyName(trustToEdit.PeriodRun);
                var interfaceVersion = EnumHelper.GetEnumDescription((ESRInterfaceVersion)trustToEdit.ESRInterfaceVersion);

                // Click edit nhs trust
                trustToEdit.TrustID = nhsTrustsMethods.ReturnNhsTrustIdFromGrid(trustToEdit.TrustName);
                nhsTrustsMethods.ClickEditFieldLink(trustToEdit.TrustName);

                // Populate fields           
                Assert.AreEqual(trustToEdit.TrustName, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustNameTextBox.Text);
                Assert.AreEqual(trustToEdit.TrustVPD, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustVPDTextBox.Text);
                Assert.AreEqual(periodType, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.PeriodTypeComboBox.SelectedItem);
                Assert.AreEqual(periodRun, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.PeriodRunComboBox.SelectedItem);
                Assert.AreEqual(trustToEdit.FtpAddress, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.FTPAddressTextBox.Text);
                Assert.AreEqual(trustToEdit.FtpUsername, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.FTPUsernameTextBox.Text);
                Assert.AreEqual(trustToEdit.RunSequenceNumber.ToString(), nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.RunSequenceTextBox.Text);
                Assert.AreEqual(trustToEdit.DelimiterCharacter, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.DelimiterCharacterTextBox.Text);
                Assert.AreEqual(interfaceVersion, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.ESRInterfaceVersionComboBox.SelectedItem);

                // Press cancel
                nhsTrustsMethods.PressCancelNHSTrust();
            }
            #endregion
        }
        #endregion

        #region successfully cancel adding NHSTrust
        /// <summary>
        /// This test ensures a NHS Trust in not added within Expenses when you press cancel. 
        /// </summary>
        [TestCategory("NHSTrust Details"), TestCategory("NHSTrusts"), TestCategory("Expenses"), TestMethod]
        public void NHSTrustSuccessfullyCancelAddingNHSTrust_UITest()
        {
            _sharedMethods.NavigateToPage(_executingProduct, NHSTustUrlSuffixes.NHSTustUrl);
            NHSTrusts trustToAdd = nhsTrusts[0];

            var periodType = FriendlyName.GetPeriodTypeFriendlyName(trustToAdd.PeriodType);
            var periodRun = FriendlyName.GetPeriodRunFriendlyName(trustToAdd.PeriodRun);
            var interfaceVersion = EnumHelper.GetEnumDescription((ESRInterfaceVersion)trustToAdd.ESRInterfaceVersion);

            // Click New NHS Trust
            nhsTrustsMethods.ClickNewTrustLink();

            // Populate fields           
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustNameTextBox.Text = trustToAdd.TrustName;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustVPDTextBox.Text = trustToAdd.TrustVPD;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.PeriodTypeComboBox.SelectedItem = periodType;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.PeriodRunComboBox.SelectedItem = periodRun;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.FTPUsernameTextBox.Text = trustToAdd.FtpUsername;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.FTPPasswordTextBox.Text = Playback.EncryptText(trustToAdd.FtpPassword);
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.RunSequenceTextBox.Text = trustToAdd.RunSequenceNumber.ToString();
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.DelimiterCharacterTextBox.Text = trustToAdd.DelimiterCharacter;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.ESRInterfaceVersionComboBox.SelectedItem = interfaceVersion;

            // Press Cancel
            nhsTrustsMethods.PressCancelNHSTrust();

            // Validate nhs trusts grid
            nhsTrustsMethods.ValidateNHSTrustGrid(trustToAdd.TrustName, trustToAdd.TrustVPD, trustToAdd.RunSequenceNumber.ToString(), trustToAdd.ESRInterfaceVersion.ToString(), false);

        }
        #endregion

        #region unsuccessfully add NHSTrust where mandatory fields blank
        /// <summary>
        /// This test ensures a NHS Trust cannot be added within Expenses when mandatory fields are blank
        /// </summary>
        [TestCategory("NHSTrust Details"), TestCategory("NHSTrusts"), TestCategory("Expenses"), TestMethod]
        public void NHSTrustUnsuccessfullyAddNHSTrustWhereMandatoryFieldsBlank_UITest()
        {
            // Navigate to NHS Trust Details page
            _sharedMethods.NavigateToPage(_executingProduct, NHSTustUrlSuffixes.NHSTustUrl);

            // Click Edit Trust link
            nhsTrustsMethods.ClickNewTrustLink();

            // Click Save
            nhsTrustsMethods.PressSaveNHSTrust();

            // Validate New NHS Trust with mandatory fields missing cannot be added
            nhsTrustsMethods.ValidateNHSTrustModalMesageExpectedValues.NHSTrustsModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), NHSTrustModalMessages.EmptyFieldForTrustName });
            nhsTrustsMethods.ValidateNHSTrustModalMesage();
        }
        #endregion

        #region unsuccessfully add duplicate new NHSTrust
        /// <summary>
        /// This test ensures a NHS Trust cannot be duplicated within Expenses
        /// </summary>
        [TestCategory("NHSTrust Details"), TestCategory("NHSTrusts"), TestCategory("Expenses"), TestMethod]
        public void NHSTrustsUnsuccessfullyAddNHSTrustWithDuplicateNameAndVPD_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            NHSTrusts duplicateTrustToAdd = nhsTrusts[0];
            NHSTrusts uniqueTrustToAdd = nhsTrusts[1];

            // Navigate to NHS Trust Details page
            _sharedMethods.NavigateToPage(_executingProduct, NHSTustUrlSuffixes.NHSTustUrl);

            // Click New Trust link
            nhsTrustsMethods.ClickNewTrustLink();

            // Populate fields
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustNameTextBox.Text = duplicateTrustToAdd.TrustName;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustVPDTextBox.Text = duplicateTrustToAdd.TrustVPD;

            // Click Save
           nhsTrustsMethods.PressSaveNHSTrust();

            // Validate Duplicate cannot be added
           nhsTrustsMethods.ValidateNHSTrustModalMesageExpectedValues.NHSTrustsModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), NHSTrustModalMessages.DuplicateFieldForTrustName});
           nhsTrustsMethods.ValidateNHSTrustModalMesage();

            // press close
            nhsTrustsMethods.PressCloseOnModalMessage();

            // Populate fields
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustNameTextBox.Text = uniqueTrustToAdd.TrustName;

            // Click Save
           nhsTrustsMethods.PressSaveNHSTrust();

            // Validate Duplicate cannot be added
           nhsTrustsMethods.ValidateNHSTrustModalMesageExpectedValues.NHSTrustsModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), NHSTrustModalMessages.DuplicateFieldForTrustVpd });
           nhsTrustsMethods.ValidateNHSTrustModalMesage();
        }
        #endregion

        #region successfully edit new NHSTrust
        /// <summary>
        /// This test ensures a NHS Trust can be edited within Expenses
        /// </summary>
        [TestCategory("NHSTrust Details"), TestCategory("NHSTrusts"), TestCategory("Expenses"), TestMethod]
        public void NHSTrustsSuccessfullyEditNHSTrust_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            NHSTrusts trustToEdit = nhsTrusts[0];
            NHSTrusts trustToAdd = nhsTrusts[1];

            var periodType = FriendlyName.GetPeriodTypeFriendlyName(trustToAdd.PeriodType);
            var periodRun = FriendlyName.GetPeriodRunFriendlyName(trustToAdd.PeriodRun);
            var interfaceVersion = EnumHelper.GetEnumDescription((ESRInterfaceVersion)trustToAdd.ESRInterfaceVersion);

            // Navigate to NHS Trust Details page
            _sharedMethods.NavigateToPage(_executingProduct, NHSTustUrlSuffixes.NHSTustUrl);
            
            // Click Edit Trust link
            nhsTrustsMethods.ClickEditFieldLink(trustToEdit.TrustName);

            // Populate fields
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustNameTextBox.Text = trustToAdd.TrustName;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustVPDTextBox.Text = trustToAdd.TrustVPD;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.PeriodTypeComboBox.SelectedItem = periodType;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.PeriodRunComboBox.SelectedItem = periodRun;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.FTPAddressTextBox.Text = trustToAdd.FtpAddress;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.FTPUsernameTextBox.Text = trustToAdd.FtpUsername;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.FTPPasswordTextBox.Text = Playback.EncryptText(trustToAdd.FtpPassword);
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.RunSequenceTextBox.Text = trustToAdd.RunSequenceNumber.ToString();
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.DelimiterCharacterTextBox.Text = trustToAdd.DelimiterCharacter;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.ESRInterfaceVersionComboBox.SelectedItem = interfaceVersion;
            
            // Click Save
            nhsTrustsMethods.PressSaveNHSTrust();

            // Validate NHS Trust is added in the grid  
            nhsTrustsMethods.ValidateNHSTrustGrid(trustToAdd.TrustName, trustToAdd.TrustVPD, trustToAdd.RunSequenceNumber.ToString(), interfaceVersion);

            // Click Edit Trust link
            nhsTrustsMethods.ClickEditFieldLink(trustToAdd.TrustName);

            // Validate New NHS Trust
            Assert.AreEqual(trustToAdd.TrustName, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustNameTextBox.Text);
            Assert.AreEqual(trustToAdd.TrustVPD, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustVPDTextBox.Text);
            Assert.AreEqual(periodType, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.PeriodTypeComboBox.SelectedItem);
            Assert.AreEqual(periodRun, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.PeriodRunComboBox.SelectedItem);
            Assert.AreEqual(trustToAdd.FtpAddress, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.FTPAddressTextBox.Text);
            Assert.AreEqual(trustToAdd.FtpUsername, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.FTPUsernameTextBox.Text);
            Assert.AreEqual(trustToAdd.RunSequenceNumber.ToString(), nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.RunSequenceTextBox.Text);
            Assert.AreEqual(trustToAdd.DelimiterCharacter, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.DelimiterCharacterTextBox.Text);
            Assert.AreEqual(interfaceVersion, nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.ESRInterfaceVersionComboBox.SelectedItem);
        }
        #endregion

        #region unsuccessfully edit duplicate new NHSTrust
        /// <summary>
        /// This test ensures a NHS Trust cannot be duplicated within Expenses
        /// </summary>
        [TestCategory("NHSTrust Details"), TestCategory("NHSTrusts"), TestCategory("Expenses"), TestMethod]
        public void NHSTrustsUnsuccessfullyEditNHSTrustWithDuplicateNameAndVPD_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            NHSTrusts trustToEdit = nhsTrusts[0];
            NHSTrusts duplicateTrustToAdd = nhsTrusts[1];
            NHSTrusts uniqueTrustToAdd = nhsTrusts[2];

            // Navigate to NHS Trust Details page
            _sharedMethods.NavigateToPage(_executingProduct, NHSTustUrlSuffixes.NHSTustUrl);

            // Click New Trust link
            nhsTrustsMethods.ClickEditFieldLink(trustToEdit.TrustName);

            // Populate fields
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustNameTextBox.Text = duplicateTrustToAdd.TrustName;
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustVPDTextBox.Text = duplicateTrustToAdd.TrustVPD;

            // Click Save
            nhsTrustsMethods.PressSaveNHSTrust();

            // Validate Duplicate cannot be added
            nhsTrustsMethods.ValidateNHSTrustModalMesageExpectedValues.NHSTrustsModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), NHSTrustModalMessages.DuplicateFieldForTrustName });
            nhsTrustsMethods.ValidateNHSTrustModalMesage();

            // press close
            nhsTrustsMethods.PressCloseOnModalMessage();

            // Populate fields
            nhsTrustsMethods.NHSTrustsControlsModalsLinksWindow.NHSTrustsControlsDocument.TrustNameTextBox.Text = uniqueTrustToAdd.TrustName;

            // Click Save
            nhsTrustsMethods.PressSaveNHSTrust();

            // Validate Duplicate cannot be added
            nhsTrustsMethods.ValidateNHSTrustModalMesageExpectedValues.NHSTrustsModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), NHSTrustModalMessages.DuplicateFieldForTrustVpd });
            nhsTrustsMethods.ValidateNHSTrustModalMesage();
        }
        #endregion

        #region successfully archive NHSTrust
        /// <summary>
        /// This test ensures a NHS Trust can be archived within Expenses
        /// </summary>
        [TestCategory("NHSTrust Details"), TestCategory("NHSTrusts"), TestCategory("Expenses"), TestMethod]
        public void NHSTrustSuccessfullyArchiveNHSTrust_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            NHSTrusts trustToArchive = nhsTrusts[0];

            // Navigate to NHS Trust Details page
            _sharedMethods.NavigateToPage(_executingProduct, NHSTustUrlSuffixes.NHSTustUrl);

            // Click Archive Trust link
            nhsTrustsMethods.ClickArchiveFieldLink(trustToArchive.TrustName);
            trustToArchive.Archived = true;
           
            // Validate Archive NHS Trust
            nhsTrustsMethods.ValidateNhsTrustArchivedState(trustToArchive.TrustName, trustToArchive.Archived);
        }
        #endregion

        #region successfully unarchive NHSTrust
        /// <summary>
        /// This test ensures a NHS Trust can be Un-archived within Expenses
        /// </summary>
        [TestCategory("NHSTrust Details"), TestCategory("NHSTrusts"), TestCategory("Expenses"), TestMethod]
        public void NHSTrustSuccessfullyUnarchiveNHSTrust_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            NHSTrusts trustToUnarchive = nhsTrusts[0];

            // Navigate to NHS Trust Details page
            _sharedMethods.NavigateToPage(_executingProduct, NHSTustUrlSuffixes.NHSTustUrl);

            // Click Archive Trust link
            nhsTrustsMethods.ClickUnArchiveFieldLink(trustToUnarchive.TrustName);

            // Validate Archive NHS Trust
            nhsTrustsMethods.ValidateNhsTrustArchivedState(trustToUnarchive.TrustName);
        }
        #endregion

        #region successfully delete NHSTrust
        /// <summary>
        /// This test ensures a NHS Trust can be deleted within Expenses. 
        /// </summary>
        [TestCategory("NHSTrust Details"), TestCategory("NHSTrusts"), TestCategory("Expenses"), TestMethod]
        public void NHSTrustSuccessfullyDeleteNHSTrust_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);
            NHSTrusts trustToDelete = nhsTrusts[0];

            // Navigate to NHS Trust Details page
            _sharedMethods.NavigateToPage(_executingProduct, NHSTustUrlSuffixes.NHSTustUrl);

            // Click Delete Trust link
            nhsTrustsMethods.ClickDeleteFieldLink(trustToDelete.TrustName);

            // Click OK to confirm deletion
            nhsTrustsMethods.ClickOKToConfirmDeletion();

            // Validate Delete NHS Trust
            nhsTrustsMethods.ValidateNHSTrustGrid(trustToDelete.TrustName, trustToDelete.TrustVPD, trustToDelete.RunSequenceNumber.ToString(), trustToDelete.ESRInterfaceVersion.ToString(), false);
        }
        #endregion

        #region unsuccessfully delete NHSTrust associated with financial export
        /// <summary> INCOMPLETE
        /// This test ensures a NHS Trust cannot be deleted if it is associated with a Financial Export
        /// </summary>
        [/*TestCategory("NHSTrust Details"), TestCategory("NHSTrusts"), TestCategory("Expenses"), */TestMethod]
        public void NHSTrustUnsuccessfullyDeleteNHSTrustAssociatedWithFinancialExport_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            NHSTrusts trusttoAdd = nhsTrusts[0];

            // Navigate to Financial Export page
            _sharedMethods.NavigateToPage(_executingProduct, FinancialExportsUrlSuffixes.FinancialExportUrl);

            // Click New Financial Export
            nhsTrustsMethods.ClickNewFinancialExport();

            // Populate Fields
            nhsTrustsMethods.PopulateESRFinancialExportDetailsParams.UINHSTrustComboBox1SelectedItem = trusttoAdd.TrustName;
            nhsTrustsMethods.PopulateESRFinancialExportDetailsParams.UIReportComboBoxSelectedItem = "Employees";
            nhsTrustsMethods.PopulateESRFinancialExportDetails();

            // Press Save
            nhsTrustsMethods.PressESRFinancialExportSave();

            // Validate ESR Financial Export Created
            //_NHSTrustsMethods.ValidateESRFinancialExport();

            // Navigate to NHS Trust Details page
            _sharedMethods.NavigateToPage(_executingProduct, NHSTustUrlSuffixes.NHSTustUrl);

            // Delete NHS Trust
            nhsTrustsMethods.ClickDeleteFieldLink(trusttoAdd.TrustName);

            // Click OK to confirm deletion
            nhsTrustsMethods.ClickOKToConfirmDeletion();

            // Validate NHS Trust cannot be deleted
            nhsTrustsMethods.ValidateNHSTrustModalMesageExpectedValues.NHSTrustsModalMessageInnerText = string.Format("Message from {0}\r\n\r\n{1}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), NHSTrustModalMessages.AssociatedFinanicialExport });
            nhsTrustsMethods.ValidateNHSTrustModalMesage();

            NHSTrustsRepository.DeleteFinancialExport(trusttoAdd.TrustID, _executingProduct, true);
        }
        #endregion

        /// <summary>
        /// The import data to testing database.
        /// </summary>
        /// <param name="testName">
        /// The test name.
        /// </param>
        internal void ImportDataToTestingDatabase(string testName)
        {
            int trustId;
            switch (testName)
            {
                case "NHSTrustUnsuccessfullyDeleteNHSTrustAssociatedWithFinancialExport_UITest":
                case "NHSTrustsUnsuccessfullyAddDuplicateNHSTrust_UITest":
                case "NHSTrustsSuccessfullyEditNHSTrust_UITests":
                case "NHSTrustSuccessfullyArchiveNHSTrust_UITest":
                case "NHSTrustSuccessfullyDeleteNHSTrust_UITest":
                case "NHSTrustsUnsuccessfullyAddNHSTrustWithDuplicateNameAndVPD_UITest":
                    trustId = NHSTrustsRepository.CreateNhsTrusts(nhsTrusts[0], _executingProduct, adminId);
                    Assert.IsTrue(trustId > 0);
                    break;
                case "NHSTrustSuccessfullyUnarchiveNHSTrust_UITest":
                    var trusttoAdd = nhsTrusts[0];
                    trusttoAdd.Archived = true;
                    trustId = NHSTrustsRepository.CreateNhsTrusts(trusttoAdd, _executingProduct, adminId);
                    Assert.IsTrue(trustId > 0);
                    break;
                case "NHSTrustsUnsuccessfullyEditNHSTrustWithDuplicateNameAndVPD_UITest":
                    trustId = NHSTrustsRepository.CreateNhsTrusts(nhsTrusts[0], _executingProduct, adminId);
                    Assert.IsTrue(trustId > 0);
                    trustId = NHSTrustsRepository.CreateNhsTrusts(nhsTrusts[1], _executingProduct, adminId);
                    Assert.IsTrue(trustId > 0);
                    break;
                default:
                    trustId = NHSTrustsRepository.CreateNhsTrusts(nhsTrusts[0], _executingProduct, adminId);
                    Assert.IsTrue(trustId > 0);
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
