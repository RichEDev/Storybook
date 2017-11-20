namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Logon
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Windows.Input;

    using Auto_Tests.UIMaps.EmployeesNewUIMapClasses;
    using Auto_Tests.UIMaps.ESRAssignmentsUIMapClasses;
    //using Auto_Tests.UIMaps.LogonNewUIMapClasses;
    using Auto_Tests.UIMaps.LogonUIMapClasses;
    using Auto_Tests.UIMaps.PasswordComplexityUIMapClasses;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;

    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;

    /// <summary>
    /// The logon tests.
    /// </summary>
    [CodedUITest]
    public class LogonTests
    {
        /// <summary>
        /// Current Product in test run
        /// </summary>
        private static readonly ProductType ExecutingProduct = cGlobalVariables.GetProductFromAppConfig();

        /// <summary>
        /// Shared methods UI Map
        /// </summary>
        private static readonly PasswordComplexityUIMap PasswordComplexity = new PasswordComplexityUIMap();

        /// <summary>
        /// Shared methods UI Map
        /// </summary>
        private static LogonUIMap logonMethods = new LogonUIMap();

        /// <summary>
        /// Shared methods UI Map
        /// </summary>
        //private static LogonNewUIMap logonMethodsNew = new LogonNewUIMap();

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
        private static ESRAssignmentsUIMap assignmentsMethods;

        /// <summary>
        /// Cached list of Employees
        /// </summary>
        // private static List<Employees> employees;

        /// <summary>
        /// Administrator employee number
        /// </summary>
        private static int adminId;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Gets or sets the int sql.
        /// </summary>
        public string IntSql { get; set; }

        #region Additional test attributes

        /// <summary>
        /// The class init.
        /// </summary>
        /// <param name="ctx">
        /// The ctx.
        /// </param>
        [ClassInitialize]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.Maximized = true;
            browser.CloseOnPlaybackCleanup = false;
            sharedMethods.NavigateToPage(ProductType.framework, "");
            adminId = AutoTools.GetEmployeeIDByUsername(ProductType.framework);
            // employees = EmployeesRepository.PopulateEmployee();
        }

        /// <summary>
        /// The class clean up.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanUp()
        {
            sharedMethods.CloseBrowserWindow();
        }

        /// <summary>
        /// The my test initialize.
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            employeesMethods = new EmployeesNewUIMap();
            sharedMethods = new SharedMethodsUIMap();
            //logonMethodsNew = new LogonNewUIMap();
            logonMethods = new LogonUIMap();
        }

        /// <summary>
        /// The my test clean up.
        /// </summary>
        [TestCleanup]
        public void MyTestCleanup()
        {
        }
        #endregion

        /// <summary>
        /// 28165 - Successfully logon using the shared logon screen
        /// </summary>
        [TestMethod]
        public void LogonSuccessfullyLogon()
        {
            // Open web browser and navigate to framwork logon page
            // sharedMethods.StartIE(ProductType.framework);

            // Logon successfully as claimant user.
            LogonSuccessfully(LogonType.administrator);

            // Check logon has been sucessful.
            logonMethods.ValidateSuccessfulLogon();

            logonMethods.SuccessfullyExit();
        }

        /// <summary>
        /// 28159 -  Successfully Logon Using Remember Me Functionality
        /// </summary>
        [TestMethod]
        public void LogonSuccessfullyLogonUsingRememberMeFunctionality()
        {
            // Open web browser and navigate to framwork logon page.
            // sharedMethods.StartIE(ProductType.framework);

            // Enable the 'Remember Me' option.
            logonMethods.SetRemeberMe();

            // Logon successfully as claimant user.
            LogonSuccessfully(LogonType.administrator);

            // Exist the application.
            logonMethods.SuccessfullyExit();

            // check that Company ID and Username are populated. Check that username is empty and remeber me is checked.
            logonMethods.ValidateAutoPopulatedLogonDetailsExpectedValues.UICompanyIDEditText =
                cGlobalVariables.CompanyID(ProductType.framework);
            logonMethods.ValidateAutoPopulatedLogonDetailsExpectedValues.UIUsernameEditText =
                cGlobalVariables.AdministratorUserName(ProductType.framework);
            logonMethods.ValidateAutoPopulatedLogonDetails();

            // Enter Password and login
            logonMethods.EnterPasswordParams.UIPasswordEditPassword =
                Playback.EncryptText(cGlobalVariables.AdministratorPassword(ProductType.framework));
            logonMethods.EnterPassword();
            logonMethods.SelectLogon();

            logonMethods.SuccessfullyExit();

            // Uncheck the remember me option.
            logonMethods.SetRemeberMeParams.UIRememberDetailsCheckBoxChecked = false;
            logonMethods.SetRemeberMe();

            // Enter Password and logon
            logonMethods.EnterPassword();
            logonMethods.SelectLogon();

            logonMethods.SuccessfullyExit();

            // check that logon details are not remembered after unckecking the 'Remember Me' optin.
            logonMethods.ValidateAutoPopulatedLogonDetailsExpectedValues.UICompanyIDEditText = string.Empty;
            logonMethods.ValidateAutoPopulatedLogonDetailsExpectedValues.UIUsernameEditText = string.Empty;
            logonMethods.ValidateAutoPopulatedLogonDetailsExpectedValues.UIPasswordEditInnerText = null;
            logonMethods.ValidateAutoPopulatedLogonDetailsExpectedValues.UIRememberDetailsCheckBox1Checked = false;
            logonMethods.ValidateAutoPopulatedLogonDetails();
        }

        /// <summary>
        /// 28168 -  Unsuccessfully Logon Where Logon Details are Incorrect.
        /// Sub test - Logon unsuccessful where company id is invalid.
        /// </summary>
        [TestMethod]
        public void LogonUnsuccessfullyLogonWhereCompanyIDisIncorrect()
        {
            // Open web browser and navigate to framwork logon page.
            // sharedMethods.StartIE(ProductType.framework);

            // Enter invalid Comapny ID.
            logonMethods.EnterCompanyIDParams.UICompanyIDEditText = "NotACompany";
            logonMethods.EnterCompanyID();

            // Enter valid username.
            logonMethods.EnterUsernameParams.UIUsernameEditText =
                cGlobalVariables.AdministratorUserName(ProductType.framework);
            logonMethods.EnterUsername();

            // Enter valid password.
            logonMethods.EnterPasswordParams.UIPasswordEditPassword =
                Playback.EncryptText(cGlobalVariables.AdministratorPassword(ProductType.framework));
            logonMethods.EnterPassword();

            // Attempt logon & check that logon has been unsuccessful: Incorrect details.
            logonMethods.SelectLogon();
            logonMethods.ValidateUnsuccessfulLogon();
        }

        /// <summary>
        /// 28168 -  Unsuccessfully Logon Where Logon Details are Incorrect.
        /// Sub test - Logon unsuccessful where username is invalid.
        /// </summary>
        [TestMethod]
        public void LogonUnsuccessfullyLogonWhereUsernameIsIncorrect()
        {
            // Open web browser and navigate to framwork logon page.
            // sharedMethods.StartIE(ProductType.framework);

            // Enter valid Comapny ID.
            logonMethods.EnterCompanyIDParams.UICompanyIDEditText = cGlobalVariables.CompanyID(ProductType.framework);
            logonMethods.EnterCompanyID();


            // Enter invalid username.
            logonMethods.EnterUsernameParams.UIUsernameEditText = "NotARealUsername";
            logonMethods.EnterUsername();

            // Enter valid password.
            logonMethods.EnterPasswordParams.UIPasswordEditPassword =
                Playback.EncryptText(cGlobalVariables.AdministratorPassword(ProductType.framework));
            logonMethods.EnterPassword();


            // Attempt logon & check that logon has been unsuccessful: Incorrect details.
            logonMethods.SelectLogon();

            // Retrieve the current number of maximum retries settings from the database.
            int iMaxRetries = DbRetrieveMaxRetries();

            // Construct string to asseert.
            string sInvalidLogonMsg = "The details you have entered are incorrect. " + iMaxRetries + " attempts left.";

            // Set expected values and check that logon has been unsuccessful.
            logonMethods.ValidateInvalidDetailsErrorXattemptsLeftExpectedValues.UIDividerPaneInnerText =
                sInvalidLogonMsg;
            logonMethods.ValidateInvalidDetailsErrorXattemptsLeft();
        }

        /// <summary>
        /// 28168 -  Unsuccessfully Logon Where Logon Details are Incorrect.
        /// Sub test - Logon unsuccessful where password is invalid.
        /// </summary>
        [TestMethod]
        public void LogonUnsuccessfullyLogonWherePasswordIsIncorrect()
        {
            // Logon and exit claimant user to reset Retry Count.
            sharedMethods.Logon(ProductType.framework, LogonType.claimant);
            logonMethods.SuccessfullyExit();

            // open web browser and navigate to framwork logon page.
            sharedMethods.StartIE(ProductType.framework);

            // fail to logon successfully due to incorrect password.
            FailedLogonInvalidPassword(LogonType.claimant);

            // Retrieve the current number of maximum retries settings from the database.
            int iMaxRetries = DbRetrieveMaxRetries();

            // Retrive the number of logon attempts by the user.
            int iRetryCount = DbRetrieveRetryCount(cGlobalVariables.ClaimantUserName(ProductType.framework));

            // Calculate number of remaining logon attempts.
            int iAttemptsLeft = iMaxRetries - iRetryCount;

            // Construct string to asseert.
            string sInvalidLogonMsg2 = "The details you have entered are incorrect. " + iAttemptsLeft
                                       + " attempts left.";

            // Set expected values and check that logon has been unsuccessful.
            logonMethods.ValidateInvalidDetailsErrorXattemptsLeftExpectedValues.UIDividerPaneInnerText =
                sInvalidLogonMsg2;
            logonMethods.ValidateInvalidDetailsErrorXattemptsLeft();
        }

        /// <summary>
        /// 29029 -  Unsuccessfully Logon Where User is Archived.
        /// </summary>
        [TestMethod]
        public void LogonUnsuccessfullyLogonWhereUserIsArchived()
        {
            // Determine current archive status of the claimant user.
            bool archived = DbIsUserArchived(cGlobalVariables.AdministratorUserName(ProductType.framework));

            if (archived == false)
            {
                // Logon as administator to archive claimant user
                sharedMethods.Logon(ProductType.framework, LogonType.administrator);
                sharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/selectemployee.aspx");
                logonMethods.SearchForEmployeeParams.UIUsernameEditText =
                    cGlobalVariables.ClaimantUserName(ProductType.framework);
                logonMethods.SearchForEmployee();

                logonMethods.ArchiveEmployee();

                logonMethods.SuccessfullyExitAfterArchiving();
            }

            // Attempt to logon as claimant where user is archived.
            LogonSuccessfully(LogonType.claimant);

            // Check that the logon was unsuccessful.
            logonMethods.ValidateUnsuccessfulLogonByArchivedUser();

            // Return claimant user back to its original status,
            sharedMethods.Logon(ProductType.framework, LogonType.administrator);
            sharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/selectemployee.aspx");
            logonMethods.SearchForEmployeeParams.UIUsernameEditText = cGlobalVariables.ClaimantUserName(
                ProductType.framework);
            logonMethods.SearchForEmployee();

            logonMethods.UnarchiveEmployee();
            logonMethods.SuccessfullyExitAfterArchiving();
        }

        /// <summary>
        /// 29030 -  Successfully Cancel Request Forgotten Details
        /// </summary>
        [TestMethod]
        public void LogonSuccessfullyCancelForgottenDetailsRequest()
        {
            // open web browser and navigate to framwork logon page.
            // sharedMethods.StartIE(ProductType.framework);
    
            // Go to forgotten Details page, enter email address but the cancel.
            //logonMethods.NavigateLogonToForgottenDetails();
            //logonMethods.ForgottenDetailsEnterEmailAddress();
            //logonMethods.CancelForgottenDetails();

            //// Check that user is returned to the logon page.
            //logonMethods.ValidateLogonPageIsCurrentPage();

            //logonMethodsNew.PressForgottenDetailsButton();
            //logonMethodsNew.ForgottenDetailsControlsWindow.ForgottenDetailsControlsDocument.EmailAddressTextBox.Text = "@";
            //logonMethodsNew.PressCancelForgottenDetailsButton();
            logonMethods.ValidateLogonPageIsCurrentPage();
        }

        /// <summary>
        /// 29030 -  Successfully Request Forgotten Details
        /// </summary>
        [TestMethod]
        public void LogonSuccessfullyRequestForgottenDetails()
        {
            // open web browser and navigate to framwork logon page.
            // sharedMethods.StartIE(ProductType.framework);

            // Retrieve current email address and store.
            string sOriginalEmailAddress = DbGetEmailAddress(cGlobalVariables.AdministratorUserName(ProductType.framework));

            // unique email address accross all databases.
            string sUniqueEmailAddress = "Auto3.unique@software-europe.co.uk";

            // Set the email address of the claimant user to be a unique emai address.
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.framework));
            db.sqlexecute.Parameters.AddWithValue("@email", sUniqueEmailAddress);
            db.sqlexecute.Parameters.AddWithValue("@Username", cGlobalVariables.AdministratorUserName(ProductType.framework));
            string strSQL = "UPDATE employees SET email = @email WHERE username=@Username";

            db.ExecuteSQL(strSQL);
            db.sqlexecute.Parameters.Clear();


            //// Go to forgotten Details page, enter email address and select send.
            //logonMethods.NavigateLogonToForgottenDetails();

            //logonMethods.ForgottenDetailsEnterEmailAddressParams.UIEmailAddressEditText = sUniqueEmailAddress;
            //logonMethods.ForgottenDetailsEnterEmailAddress();
            //logonMethods.SendFogottenDetailsRequest();

            //// Check that user is returned to the logon page.
            //logonMethods.ValidateSuccessfulForgottenDetailsRequest();
            //logonMethods.CancelForgottenDetails();

            //logonMethodsNew.PressForgottenDetailsButton();
            //logonMethodsNew.ForgottenDetailsControlsWindow.ForgottenDetailsControlsDocument.EmailAddressTextBox.Text = sUniqueEmailAddress;
            //logonMethodsNew.PressSubmitForgottenDetailsButton();
            logonMethods.ValidateSuccessfulForgottenDetailsRequest();
            //logonMethodsNew.PressCancelForgottenDetailsButton();

            // Set the email address of the claimant user back to its original values.
            db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.framework));
            db.sqlexecute.Parameters.AddWithValue("@email", sOriginalEmailAddress);
            db.sqlexecute.Parameters.AddWithValue("@Username", cGlobalVariables.AdministratorUserName(ProductType.framework));
            strSQL = "UPDATE employees SET email = @email WHERE username=@Username";

            db.ExecuteSQL(strSQL);
            db.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// 29182 -  Unsuccessfully Request Forgotten Details Where Email Address Does Not Exist
        /// </summary>
        [TestMethod]
        public void LogonUnsuccessfullyRequestForgottenDetailsWhereEmailAddressDoesNotExist()
        {
            // open web browser and navigate to framwork logon page.
            // sharedMethods.StartIE(ProductType.framework);

            // Go to forgotten Details page, enter email address but the cancel.
            logonMethods.NavigateLogonToForgottenDetails();
            logonMethods.ForgottenDetailsEnterEmailAddressParams.UIEmailAddressEditText =
                "NotAReal.Email@software-europe.co.uk";
            logonMethods.ForgottenDetailsEnterEmailAddress();
            logonMethods.SendFogottenDetailsRequest();

            // Check that user is shown the unable to find email address message.
            logonMethods.ValidateForgottenDetailsInvalidEmailError();

            logonMethods.CancelForgottenDetails();
        }

        /// <summary>
        /// 29184 -  Unsuccessfully Request Forgotten Details Where Email Address Is Not Unique
        /// </summary>
        [TestMethod]
        public void LogonUnsuccessfullyRequestForgottenDetailsWhereEmailAddressIsNotUnique()
        {
            // Get the administrators email address
            string sAdminEmailAddress = DbGetEmailAddress(cGlobalVariables.AdministratorUserName(ProductType.framework));

            // Get the claimants email address
            string sClaimantEmailAddress = DbGetEmailAddress(cGlobalVariables.ClaimantUserName(ProductType.framework));


            // Set the email address of the claimant user same as the administrator user.
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.framework));
            db.sqlexecute.Parameters.AddWithValue("@email", sAdminEmailAddress);
            db.sqlexecute.Parameters.AddWithValue("@Username", cGlobalVariables.ClaimantUserName(ProductType.framework));
            string strSQL = "UPDATE employees SET email = @email WHERE username=@Username";

            db.ExecuteSQL(strSQL);
            db.sqlexecute.Parameters.Clear();

            // open web browser and navigate to framwork logon page.
            // sharedMethods.StartIE(ProductType.framework);

            // Go to forgotten Details page, enter email address but the cancel.
            logonMethods.NavigateLogonToForgottenDetails();
            logonMethods.ForgottenDetailsEnterEmailAddressParams.UIEmailAddressEditText = sAdminEmailAddress;
            logonMethods.ForgottenDetailsEnterEmailAddress();
            logonMethods.SendFogottenDetailsRequest();

            // Check that user is shown the unable to find email address message.
            logonMethods.ValidateNonUniqueEmailMessage();

            logonMethods.CancelForgottenDetails();

            // Set the email address of the claimant user same as the administrator user.
            db.sqlexecute.Parameters.AddWithValue("@email", sClaimantEmailAddress);
            db.sqlexecute.Parameters.AddWithValue("@Username", cGlobalVariables.ClaimantUserName(ProductType.framework));
            strSQL = "UPDATE employees SET email = @email WHERE username=@Username";

            db.ExecuteSQL(strSQL);
            db.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// 29189 -  Unsuccessfully Logon Where User Exceeds Maximum Retries.
        /// </summary>
        [TestMethod]
        public void LogonUnsuccessfullyLogonWhereUserExceedsMaximumRetries()
        {
            // Retrieve the current number of maximum retries settings from the database.
            int iMaxRetries = DbRetrieveMaxRetries();

            // Logon and exit claimant user to reset Retry Count.
            sharedMethods.Logon(ProductType.framework, LogonType.claimant);
            logonMethods.SuccessfullyExit();

            // Repeatedly fail logon attempts until users account is locked
            int RetryCount = 0;
            while (RetryCount <= iMaxRetries)
            {
                // fail to logon successfully due to incorrect password.
                FailedLogonInvalidPassword(LogonType.claimant);

                // Increment retry count
                RetryCount++;
            }

            //// Check that logon has been unsucessful.
            logonMethods.ValidateLockedAccountMessage();

            // unarchive locked user account to reset data for other tests
            sharedMethods.Logon(ProductType.framework, LogonType.administrator);
            sharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/selectemployee.aspx");
            logonMethods.SearchForEmployeeParams.UIUsernameEditText = cGlobalVariables.ClaimantUserName(
                ProductType.framework);
            logonMethods.SearchForEmployee();

            logonMethods.UnarchiveEmployee();
            logonMethods.SuccessfullyExit();

            // Logon failed as claimant user.
            FailedLogonInvalidPassword(LogonType.claimant);

            // Construct string to asseert.
            RetryCount = iMaxRetries--;
            string sInvalidLogonMsg = "The details you have entered are incorrect. " + iMaxRetries + " attempts left.";

            // Set expected values and check that logon has been unsuccessful.
            logonMethods.ValidateInvalidDetailsErrorXattemptsLeftExpectedValues.UIDividerPaneInnerText =
                sInvalidLogonMsg;
            logonMethods.ValidateInvalidDetailsErrorXattemptsLeft();
        }

        /// <summary>
        /// 29190 -  Successfully Verify Logon Retry Count Is Reset Where Logon is Successful.
        /// </summary>
        [TestMethod]
        public void LogonSuccessfullyVerifyLogonRetryCountIsResetWhereLogonIsSuccessful()
        {
            #region Ensure password complexity options do not make test fail

            sharedMethods.Logon(ProductType.framework, LogonType.administrator);

            // Navigate to the general options page
            sharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");

            // Select the Password settings page
            PasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();

            // Set Password Settings to 'any' length
            PasswordComplexity.PasswordSettingsSetExpiryPeriodParams.UIPasswordexpiresafterEditText = "0";
            PasswordComplexity.PasswordSettingsSetExpiryPeriod();
            PasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            #endregion

            // Retrieve the current number of maximum retries settings from the database.
            int iMaxRetries = DbRetrieveMaxRetries();

            // Logon and exit claimant user to reset Retry Count.
            sharedMethods.Logon(ProductType.framework, LogonType.claimant);
            logonMethods.SuccessfullyExit();

            // Repeatedly logon until users account is nearly (but not) locked
            int iRetryCount = 1;
            string username = string.Empty;
            while (iRetryCount < iMaxRetries)
            {
                // fail to logon successfully due to incorrect password.
                FailedLogonInvalidPassword(LogonType.administrator);

                // Increment retry count
                iRetryCount++;
            }

            // Logon unsuccessfully as claimant user.
            LogonSuccessfully(LogonType.claimant);

            // Log out and fail to logon successfully.
            logonMethods.SuccessfullyExit();

            // fail to logon successfully due to incorrect password.
            FailedLogonInvalidPassword(LogonType.claimant);

            // Check that logon has been unsuccesful and the remaining logon attempts
            iMaxRetries--;
            string sInvalidLogonMsg = "The details you have entered are incorrect. " + iMaxRetries + " attempts left.";
            logonMethods.ValidateInvalidDetailsErrorXattemptsLeftExpectedValues.UIDividerPaneInnerText =
                sInvalidLogonMsg;
            logonMethods.ValidateInvalidDetailsErrorXattemptsLeft();
        }

        /// <summary>
        /// Successfully logon where inactive IP Addresses are set
        /// </summary>
        [TestMethod]
        public void LogonSuccessfullyLogonWithInactiveIPAddressFilter()
        {
            #region

            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.framework));
            string strSQL0 = "DELETE FROM ipfilters";
            db.ExecuteSQL(strSQL0);
            db.sqlexecute.Parameters.Clear();
            string strSQL1 =
                "INSERT INTO ipfilters VALUES ( '192.168.111.163','Ritsa''s IP address',0, NULL, NULL, NULL, NULL)";
            db.ExecuteSQL(strSQL1);
            db.sqlexecute.Parameters.Clear();
            string strSQL2 =
                "INSERT INTO ipfilters VALUES ( '192.168.111.168','Build Server''s IP Address',0, NULL, NULL, NULL, NULL)";
            db.ExecuteSQL(strSQL2);
            db.sqlexecute.Parameters.Clear();
            string strSQL3 =
                "INSERT INTO ipfilters VALUES ( '192.168.111.120','James''s IP address',0, NULL, NULL, NULL, NULL)";
            db.ExecuteSQL(strSQL3);
            db.sqlexecute.Parameters.Clear();

            #endregion

            // Logon successfully as claimant user.
            sharedMethods.Logon(ProductType.framework, LogonType.administrator);

            // Check logon has been sucessful.
            logonMethods.ValidateSuccessfulLogon();

            logonMethods.SuccessfullyExit();

            db.ExecuteSQL(strSQL0);
            db.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Successfully logon where active IP Address Filtering is set
        /// </summary>
        [TestMethod]
        public void LogonSuccessfullyLogonWithActiveIPAddressFilter()
        {
            #region

            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.framework));
            string strSQL0 = "DELETE FROM ipfilters";
            db.ExecuteSQL(strSQL0);
            db.sqlexecute.Parameters.Clear();
            string strSQL1 = "INSERT INTO ipfilters VALUES ( '" + sharedMethods.GetIPAddressOfLocalMachine()
                             + "'  ,'Local host''s IP address',1, NULL, NULL, NULL, NULL)";
            db.ExecuteSQL(strSQL1);
            db.sqlexecute.Parameters.Clear();
            string strSQL2 =
                "INSERT INTO ipfilters VALUES ( '192.168.111.168','Build Server''s IP Address',1, NULL, NULL, NULL, NULL)";
            db.ExecuteSQL(strSQL2);
            db.sqlexecute.Parameters.Clear();
            string strSQL3 =
                "INSERT INTO ipfilters VALUES ( '192.168.111.120','James''s IP address',0, NULL, NULL, NULL, NULL)";
            db.ExecuteSQL(strSQL3);
            db.sqlexecute.Parameters.Clear();

            #endregion

            // Logon successfully as claimant user.
            sharedMethods.Logon(ProductType.framework, LogonType.administrator);

            // Check logon has been sucessful.
            logonMethods.ValidateSuccessfulLogon();

            logonMethods.SuccessfullyExit();

            db.ExecuteSQL(strSQL0);
            db.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Unsuccessfully logon where IP Addresses are set
        /// </summary>
        [TestMethod]
        public void LogonUnsuccessfullyLogonWithIPAddressFilterSet()
        {
            #region

            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.framework));
            string strSQL0 = "DELETE FROM ipfilters";
            db.ExecuteSQL(strSQL0);
            db.sqlexecute.Parameters.Clear();
            string strSQL1 =
                "INSERT INTO ipfilters VALUES ( '192.168.111.163','Ritsa''s IP address',0, NULL, NULL, NULL, NULL)";
            db.ExecuteSQL(strSQL1);
            db.sqlexecute.Parameters.Clear();
            string strSQL3 =
                "INSERT INTO ipfilters VALUES ( '192.168.111.120','James''s IP address',1, NULL, NULL, NULL, NULL)";
            db.ExecuteSQL(strSQL3);
            db.sqlexecute.Parameters.Clear();

            #endregion

            // Logon successfully as claimant user.
            sharedMethods.Logon(ProductType.framework, LogonType.administrator);

            // Check logon has been sucessful.
            logonMethods.ValidateIPAddressFilterIsApplied();

            db.ExecuteSQL(strSQL0);
            db.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Unsuccessfully logon where inactive IP Address is set
        /// </summary>
        [TestMethod]
        public void LogonUnsuccessfullyLogonWithInactiveIPAddressesSet()
        {
            #region

            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.framework));
            string strSQL0 = "DELETE FROM ipfilters";
            db.ExecuteSQL(strSQL0);
            db.sqlexecute.Parameters.Clear();
            string strSQL1 =
                "INSERT INTO ipfilters VALUES ( '192.168.111.163','Ritsa''s IP address',0, NULL, NULL, NULL, NULL)";
            db.ExecuteSQL(strSQL1);
            db.sqlexecute.Parameters.Clear();
            string strSQL2 =
                "INSERT INTO ipfilters VALUES ( '192.168.111.168','Build Server''s IP Address',0, NULL, NULL, NULL, NULL)";
            db.ExecuteSQL(strSQL2);
            db.sqlexecute.Parameters.Clear();
            string strSQL3 =
                "INSERT INTO ipfilters VALUES ( '192.168.111.120','James''s IP address',1, NULL, NULL, NULL, NULL)";
            db.ExecuteSQL(strSQL3);
            db.sqlexecute.Parameters.Clear();

            #endregion

            // Logon successfully as claimant user.
            sharedMethods.Logon(ProductType.framework, LogonType.administrator);

            // Check logon has been sucessful.
            logonMethods.ValidateIPAddressFilterIsApplied();

            db.ExecuteSQL(strSQL0);
            db.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Shared Logon Functions - Retrieve the current number of maximum retries settings from the database.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DbRetrieveMaxRetries()
        {
            int iMaxRetries = 0;

            /// Retrive the number of maximum retries as set in General Options.
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.framework));
            db.sqlexecute.Parameters.AddWithValue("@MaxRetries", "pwdMaxRetries");
            string strSQL = "SELECT stringValue FROM accountProperties WHERE stringKey=@MaxRetries";

            System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL);

            while (reader.Read())
            {
                iMaxRetries = System.Convert.ToInt32(reader.GetString(0));
            }

            reader.Close();
            db.sqlexecute.Parameters.Clear();

            return iMaxRetries;
        }

        /// <summary>
        /// Shared Logon Functions - Retrieve the current Retry Count for the user from the database.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DbRetrieveRetryCount(string username)
        {
            // Retrieve the number of faield attempted logon made by the user.
            int iRetryCount = 0;

            cDatabaseConnection db2 = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.framework));
            db2.sqlexecute.Parameters.AddWithValue("@Username", username);
            string strSQL2 = "SELECT retryCount FROM employees WHERE username=@Username";


            System.Data.SqlClient.SqlDataReader reader2 = db2.GetReader(strSQL2);

            while (reader2.Read())
            {
                iRetryCount = reader2.GetInt32(0);
            }

            reader2.Close();
            db2.sqlexecute.Parameters.Clear();

            return iRetryCount;
        }

        /// <summary>
        /// Shared Logon Functions - Retrieve the email address for the user from the database.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DbGetEmailAddress(string username)
        {
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.framework));
            db.sqlexecute.Parameters.AddWithValue("@Username", username);
            string strSQL = "SELECT email FROM employees WHERE username=@Username";

            System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL);

            string sEmailAddress = string.Empty;

            while (reader.Read())
            {
                if (reader.IsDBNull(0) == false)
                {
                    sEmailAddress = reader.GetString(0);
                }
                else
                {
                    sEmailAddress = string.Empty;
                }
            }

            reader.Close();
            db.sqlexecute.Parameters.Clear();

            return sEmailAddress;
        }

        /// <summary>
        /// Shared Logon Functions - Retrieve the current archived status of the user.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool DbIsUserArchived(string username)
        {
            bool archived = false;

            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.framework));
            db.sqlexecute.Parameters.AddWithValue("@Username", username);
            string strSQL = "SELECT archived FROM employees WHERE username=@Username";


            System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL);

            while (reader.Read())
            {
                archived = reader.GetBoolean(0);
            }

            reader.Close();
            db.sqlexecute.Parameters.Clear();

            return archived;
        }

        /// <summary>
        /// Shared Logon Function - Logon successfully.
        /// Not used shared logon methods due to this (Logon page) being the area to test.
        /// </summary>
        /// <param name="userType">
        /// The user Type.
        /// </param>
        public void LogonSuccessfully(LogonType userType)
        {
            // Logon successfully
            string username = string.Empty;
            string password = string.Empty;

            // Set username & password depending on the logon type.
            if (userType == LogonType.administrator)
            {
                username = cGlobalVariables.AdministratorUserName(ProductType.framework);
                password = cGlobalVariables.AdministratorPassword(ProductType.framework);
            }
            else
            {
                username = cGlobalVariables.ClaimantUserName(ProductType.framework);
                password = cGlobalVariables.ClaimantPassword(ProductType.framework);
            }


            logonMethods.EnterCompanyIDParams.UICompanyIDEditText = cGlobalVariables.CompanyID(ProductType.framework);
            logonMethods.EnterCompanyID();

            logonMethods.EnterUsernameParams.UIUsernameEditText = username;
            logonMethods.EnterUsername();

            logonMethods.EnterPasswordParams.UIPasswordEditPassword = Playback.EncryptText(password);
            logonMethods.EnterPassword();

            logonMethods.SelectLogon();
        }

        /// <summary>
        /// Shared Logon Function - Fail to logon due to incorrectly entered password.
        /// </summary>
        /// <param name="userType">
        /// The user Type.
        /// </param>
        public void FailedLogonInvalidPassword(LogonType userType)
        {
            // Set username depending on the logon type.
            string username;

            if (userType == LogonType.administrator)
            {
                username = cGlobalVariables.AdministratorUserName(ProductType.framework);
            }
            else
            {
                username = cGlobalVariables.ClaimantUserName(ProductType.framework);
            }

            // Enter correct company ID.
            logonMethods.EnterCompanyIDParams.UICompanyIDEditText = cGlobalVariables.CompanyID(ProductType.framework);
            logonMethods.EnterCompanyID();

            // Enter correct Username.
            logonMethods.EnterUsernameParams.UIUsernameEditText = username;
            logonMethods.EnterUsername();

            // Enter incorrect Password.
            logonMethods.EnterPasswordParams.UIPasswordEditPassword = Playback.EncryptText("WrongPassword");
            logonMethods.EnterPassword();

            // Select the logon button
            logonMethods.SelectLogon();
        }
    }
}
