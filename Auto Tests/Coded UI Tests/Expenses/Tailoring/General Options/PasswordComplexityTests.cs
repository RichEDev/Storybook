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
using System.Configuration;

namespace Auto_Tests.Coded_UI_Tests.Expenses.Tailoring.General_Options
{
    /// <summary>
    /// This class contains all of the Expenses specific tests for Password Complexity
    /// </summary>
    [CodedUITest]
    public class PasswordComplexityTests
    {
        public PasswordComplexityTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.PasswordComplexityUIMapClasses.PasswordComplexityUIMap cPasswordComplexity = new UIMaps.PasswordComplexityUIMapClasses.PasswordComplexityUIMap();


        #region Test the password complexity and password length options are saved correctly

        #region Test password complexity saving

        /// <summary>
        /// This test ensures that Password settings within General Options can be exclusively set to include a symbol
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeSymbol()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the general options page            
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Select the Password settings page
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();

            /// Set Password Settings exclusively to 'must contain symbol' and press save
            cPasswordComplexity.PasswordSettingsSetComplexityToOnlyMustContainSymbol();
            cPasswordComplexity.PressGeneralOptionsSaveButton();

            /// Navigate back to general options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();

            /// Validate the changes have been saved
            cPasswordComplexity.ValidatePasswordSettingsComplexityContainsSymbolOnly();
        }


        /// <summary>
        /// This test ensures that Password settings within General Options can be exclusively set to include numbers
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeNumbers()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the general options page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Select the Password settings page
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();

            /// Set Password Settings exclusively to 'must contain numbers' and press save
            cPasswordComplexity.PasswordSettingsSetComplexityToOnlyMustContainNumbers();
            cPasswordComplexity.PressGeneralOptionsSaveButton();

            /// Navigate back to general options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Validate the changes have been saved
            cPasswordComplexity.ValidatePasswordSettingsComplexityContainsNumbersOnly();
        }


        /// <summary>
        /// This test ensures that Password settings within General Options can be exclusively set to include upper case characters
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeUpperCase()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the general options page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Select the Password settings page
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();

            /// Set Password Settings exclusively to 'must contain upper case characters' and press save
            cPasswordComplexity.PasswordSettingsSetComplexityToOnlyMustContainUpperCase();
            cPasswordComplexity.PressGeneralOptionsSaveButton();

            /// Navigate back to general options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Validate the changes have been saved
            cPasswordComplexity.ValidatePasswordSettingsComplexityContainsUpperOnly();
        }


        /// <summary>
        /// This test ensures that Password settings within General Options can be set to include upper case characters, numbers and symbols
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeAll()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the general options page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Select the Password settings page
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();

            /// Set Password Settings to include numbers, symbols and upper case letters
            cPasswordComplexity.PasswordSettingsSetComplexityToMustContainAll();
            cPasswordComplexity.PressGeneralOptionsSaveButton();

            /// Navigate back to general options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Validate the changes have been saved
            cPasswordComplexity.ValidatePasswordSettingsComplexityContainsAll();
        }

        #endregion


        #region Test password length saving

        /// <summary>
        /// This test ensures that Password settings within General Options can be set to 'Any' length
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullySetPasswordLengthToAny()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the general options page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Select the Password settings page
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();

            /// Set Password Settings to 'any' length
            cPasswordComplexity.PasswordSettingsSetLengthToAny();
            cPasswordComplexity.PressGeneralOptionsSaveButton();

            /// Navigate back to general options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Validate the changes have been saved
            cPasswordComplexity.ValidatePasswordSettingsLengthToAny();
        }


        /// <summary>
        /// This test ensures that Password settings within General Options can be set to an 'Equal To' length
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullySetPasswordLengthToEqualTo()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the general options page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Select the Password settings page
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();

            /// Set Password Settings to include 'equal to' length
            cPasswordComplexity.PasswordSettingsSetLengthToEqualTo();
            cPasswordComplexity.PressGeneralOptionsSaveButton();

            /// Navigate back to general options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Validate the changes have been saved
            cPasswordComplexity.ValidatePasswordSettingsLengthToEqualTo();
        }


        /// <summary>
        /// This test ensures that Password settings within General Options can be set to a 'Greater Than' length
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullySetPasswordLengthToGreaterThan()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the general options page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Select the Password settings page
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();

            /// Set Password Settings to include 'greater than' length
            cPasswordComplexity.PasswordSettingsSetLengthToGreaterThan();
            cPasswordComplexity.PressGeneralOptionsSaveButton();

            /// Navigate back to general options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Validate the changes have been saved
            cPasswordComplexity.ValidatePasswordSettingsLengthToGreaterThan();
        }


        /// <summary>
        /// This test ensures that Password settings within General Options can be set to a 'Less Than' length
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullySetPasswordLengthToLessThan()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the general options page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Select the Password settings page
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();

            /// Set Password Settings to include 'less than' length
            cPasswordComplexity.PasswordSettingsSetLengthToLessThan();
            cPasswordComplexity.PressGeneralOptionsSaveButton();

            /// Navigate back to general options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Validate the changes have been saved
            cPasswordComplexity.ValidatePasswordSettingsLengthToLessThan();
        }


        /// <summary>
        /// This test ensures that Password settings within General Options can be set to a 'Between' length
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullySetPasswordLengthToBetween()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the general options page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Select the Password settings page
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();

            /// Set Password Settings to include 'between' length
            cPasswordComplexity.PasswordSettingsSetLengthToBetween();
            cPasswordComplexity.PressGeneralOptionsSaveButton();

            /// Navigate back to general options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Validate the changes have been saved
            cPasswordComplexity.ValidatePasswordSettingsLengthToBetween();
        }

        #endregion

        #endregion


        #region Test the password complexity options on their own


        #region Test for 'symbol'

        /// <summary>
        /// This test ensures that a users password can be changed from My Details, where password complexity is set 
        /// to 'must include symbol'. NOTE: This is testing 'must include a symbol' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromMyDetailsWherePasswordComplexityIncludesSymbol()
        {
            /// Ensure password complexity is set to only have include symbol
            ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeSymbol();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Change my Password page from My Details
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ValidateSymbolPolicyInMyDetails();

            /// Try and update the password to one which does not conform to policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password86");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password86");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Update the password successfully
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password!");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from the admin employees page, where password complexity is set 
        /// to 'must include symbol'. NOTE: This is testing 'must include a symbol' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromAdminEmployeesWherePasswordComplexityIncludesSymbol()
        {
            /// Ensure password complexity is set to only have include symbol
            ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeSymbol();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page and search for the current user
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployeeParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployee();
            cPasswordComplexity.ValidateSymbolPolicyInAdminEmployee();

            /// Try and set the new password to one which does not conform
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password86");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password86");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Enter the new password
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password!");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password!");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password!");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed when logging on, where password complexity is set 
        /// to 'must include symbol'. NOTE: This is testing 'must include a symbol' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromLogonWherePasswordComplexityIncludesSymbol()
        {
            /// Ensure password complexity is set to only have include symbol
            ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeSymbol();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriodParams.UIPasswordexpiresafterEditText = "1";
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Set the expiry date for the users password to 2010-01-01     
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            db.ExecuteSQL("UPDATE employees SET lastchange = '2010-01-01' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Enter an incorrect password
            cPasswordComplexity.ValidateSymbolPolicyInLogonScreen();
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password86");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password86");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a valid password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password!");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password!");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password!");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }

        #endregion


        #region Test for 'numbers'

        /// <summary>
        /// This test ensures that a users password can be changed from My Details, where password complexity is set 
        /// to 'must include numbers'. NOTE: This is testing 'must include a numbers' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromMyDetailsWherePasswordComplexityIncludesNumbers()
        {
            /// Ensure password complexity is set to only have include numbers
            ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeNumbers();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Change my Password page from My Details
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();

            /// Try and update the password to one which does not conform to policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Update the password successfully
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password6");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password6");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password6");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from the admin employees page, where password complexity is set 
        /// to 'must include numbers'. NOTE: This is testing 'must include numbers' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromAdminEmployeesWherePasswordComplexityIncludesNumbers()
        {
            /// Ensure password complexity is set to only have include symbol
            ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeNumbers();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page and search for the current user
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployeeParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployee();
            cPasswordComplexity.ValidateNumbersPolicyInAdminEmployee();

            /// Try and set the new password to one which does not conform
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Enter the new password
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password7");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password7");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password7");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed when logging on, where password complexity is set 
        /// to 'must include numbers'. NOTE: This is testing 'must include numbers' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromLogonWherePasswordComplexityIncludesNumbers()
        {
            /// Ensure password complexity is set to only have include numbers
            ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeNumbers();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriodParams.UIPasswordexpiresafterEditText = "1";
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Set the expiry date for the users password to 2010-01-01     
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            db.ExecuteSQL("UPDATE employees SET lastchange = '2010-01-01' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Enter an incorrect password
            cPasswordComplexity.ValidateNumbersPolicyInLogonPage();
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a valid password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password8");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password8");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password8");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }

        #endregion


        #region Test for 'upper case'

        /// <summary>
        /// This test ensures that a users password can be changed from My Details, where password complexity is set 
        /// to 'must include upper case letters'. NOTE: This is testing 'must include upper case letters' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromMyDetailsWherePasswordComplexityIncludesUpperCase()
        {
            /// Ensure password complexity is set to only have include upper case
            ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeUpperCase();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Change my Password page from My Details
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ValidateUpperCasePolicyInMyDetails();

            /// Try and update the password to one which does not conform to policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Update the password successfully
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password6");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password6");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password6");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from the admin employees page, where password complexity is set 
        /// to 'must include upper case letters'. NOTE: This is testing 'must include upper case letters' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromAdminEmployeesWherePasswordComplexityIncludesUpperCase()
        {
            /// Ensure password complexity is set to only have include upper case
            ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeUpperCase();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page and search for the current user
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployeeParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployee();
            cPasswordComplexity.ValidateUpperCasePolicyInAdminEmployee();

            /// Try and set the new password to one which does not conform
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("password1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("password1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Enter the new password
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password7");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password7");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password7");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed when logging on, where password complexity is set 
        /// to 'must include upper case letters'. NOTE: This is testing 'must include upper case letters' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromLogonWherePasswordComplexityIncludesUpperCase()
        {
            /// Ensure password complexity is set to only have include upper case
            ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeUpperCase();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriodParams.UIPasswordexpiresafterEditText = "1";
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Set the expiry date for the users password to 2010-01-01     
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            db.ExecuteSQL("UPDATE employees SET lastchange = '2010-01-01' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Enter an incorrect password
            cPasswordComplexity.ValidateUpperCasePolicyInLogonPage();
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("password1");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("password1");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a valid password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password8");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password8");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password8");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }

        #endregion


        #region Test for all complexity options at the same time

        /// <summary>
        /// This test ensures that a users password can be changed from My Details, where password complexity is set 
        /// to must include upper case letters, numbers and symbols 
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromMyDetailsWherePasswordComplexityIncludesAll()
        {
            /// Ensure password complexity is set to only have include upper case, numbers and symbols
            ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeAll();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Change my Password page from My Details
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ValidateUpperCasePolicyInMyDetails();
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();
            cPasswordComplexity.ValidateSymbolPolicyInMyDetails();

            /// Try and update the password to one which does not conform to policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain numbers
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain upper case letters
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("password!1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("password!1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain symbols
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not any
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Update the password successfully
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password!6");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password!6");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password!6");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from Admin employee, where password complexity is set 
        /// to must include upper case letters, numbers and symbols 
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromAdminEmployeeWherePasswordComplexityIncludesAll()
        {
            /// Ensure password complexity is set to only have include upper case, numbers and symbols
            ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeAll();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page and search for the current user
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployeeParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployee();
            cPasswordComplexity.ValidateNumbersPolicyInAdminEmployee();

            /// Try and set the new password to one which does contain numbers
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain symbols
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain upper case
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ssword1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ssword1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain any
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Enter the new password
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword7");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword7");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ssword7");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from the logon screen, where password complexity is set 
        /// to must include upper case letters, numbers and symbols 
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromLogonWherePasswordComplexityIncludesAll()
        {
            /// Ensure password complexity is set to include upper case, numbers and symbols
            ExPasswordComplexitySuccessfullySetPasswordComplexityToIncludeAll();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            //// Set the expiry date for the users password to 2010-01-01     
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            db.ExecuteSQL("UPDATE employees SET lastchange = '2010-01-01' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Enter a password that does not contain a number
            cPasswordComplexity.ValidateNumbersPolicyInLogonPage();
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not contain a symbol
            cPasswordComplexity.ValidateNumbersPolicyInLogonPage();
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not contain an upper case letter
            cPasswordComplexity.ValidateNumbersPolicyInLogonPage();
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ssword1");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ssword1");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not contain any
            cPasswordComplexity.ValidateNumbersPolicyInLogonPage();
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a valid password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword8");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword8");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ssword8");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }

        #endregion

        #endregion


        #region Test the Password length options on their own


        #region Tests for 'length is any'

        /// <summary>
        /// This test ensures that a users password can be changed from My Details, where password length is set 
        /// to 'any'. NOTE: This is testing 'any' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromMyDetailsWherePasswordLengthIsAny()
        {
            /// Ensure password length is set to 'any'
            ExPasswordComplexitySuccessfullySetPasswordLengthToAny();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Change my Password page from My Details
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();

            /// Update the password successfully
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            Playback.Wait(2000);

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from the admin employees page, where password length is set 
        /// to 'any'. NOTE: This is testing 'any' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromAdminEmployeesWherePasswordLengthIsAny()
        {
            /// Ensure password length is set to 'any'
            ExPasswordComplexitySuccessfullySetPasswordLengthToAny();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page and search for the current user
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployeeParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployee();

            /// Enter the new password
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            Playback.Wait(2000);

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed when logging on, where password length is set 
        /// to 'any'. NOTE: This is testing 'any' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromLogonWherePasswordLengthIsAny()
        {
            /// Ensure password length is set to 'any'
            ExPasswordComplexitySuccessfullySetPasswordLengthToAny();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Set the expiry date for the users password to 2010-01-01     
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            db.ExecuteSQL("UPDATE employees SET lastchange = '2010-01-01' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Enter a valid password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            Playback.Wait(2000);

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }

        #endregion


        #region Tests for 'length is equal to'

        /// <summary>
        /// This test ensures that a users password can be changed from My Details, where password length is set 
        /// to 'equal to'. NOTE: This is testing 'equal to' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromMyDetailsWherePasswordLengthIsEqualTo()
        {
            /// Ensure password length is set to 'equal to'
            ExPasswordComplexitySuccessfullySetPasswordLengthToEqualTo();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Change my Password page from My Details
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ValidateNumbersPolicyInMyDetailsExpectedValues.UIYournewpasswordmustcCustom1InnerText =
                "Your new password must be 6 characters in length.";
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();

            /// Try and update the password to one which does not conform to policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Passw");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Passw");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Update the password successfully
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Passwo");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Passwo");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Passwo");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from the admin employees page, where password length is set 
        /// to 'equal to'. NOTE: This is testing 'equal to' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromAdminEmployeesWherePasswordLengthIsEqualTo()
        {
            /// Ensure password length is set to 'equal to'
            ExPasswordComplexitySuccessfullySetPasswordLengthToEqualTo();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page and search for the current user
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployeeParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployee();
            cPasswordComplexity.ValidateSymbolPolicyInAdminEmployeeExpectedValues.UIYournewpasswordmustcCustomInnerText =
                "Your new password must be 6 characters in length.";
            cPasswordComplexity.ValidateSymbolPolicyInAdminEmployee();

            /// Try and set the new password to one which does not conform
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Passw");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Passw");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Enter the new password
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Passwo");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Passwo");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Passwo");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed when logging on, where password length is set 
        /// to 'equal to'. NOTE: This is testing 'equal to' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromLogonWherePasswordLengthIsEqualTo()
        {
            /// Ensure password length is set to 'equal to'
            ExPasswordComplexitySuccessfullySetPasswordLengthToEqualTo();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriodParams.UIPasswordexpiresafterEditText = "1";
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Set the expiry date for the users password to 2010-01-01     
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            db.ExecuteSQL("UPDATE employees SET lastchange = '2010-01-01' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Enter an incorrect password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Passw");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Passw");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a valid password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Passwo");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Passwo");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Passwo");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }

        #endregion


        #region Tests for 'length is less than'


        /// <summary>
        /// This test ensures that a users password can be changed from My Details, where password length is set 
        /// to 'less than'. NOTE: This is testing 'less than' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromMyDetailsWherePasswordLengthIsLessThan()
        {
            /// Ensure password length is set to 'less than'
            ExPasswordComplexitySuccessfullySetPasswordLengthToLessThan();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Change my Password page from My Details
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ValidateNumbersPolicyInMyDetailsExpectedValues.UIYournewpasswordmustcCustom1InnerText =
                "Your new password must be less than 6 characters in length.";
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();

            /// Try and update the password to one which does not conform to policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Update the password successfully
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Pass");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from the admin employees page, where password length is set 
        /// to 'less than'. NOTE: This is testing 'less than' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromAdminEmployeesWherePasswordLengthIsLessThan()
        {
            /// Ensure password length is set to 'less than'
            ExPasswordComplexitySuccessfullySetPasswordLengthToLessThan();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page and search for the current user
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployeeParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployee();
            cPasswordComplexity.ValidateSymbolPolicyInAdminEmployeeExpectedValues.UIYournewpasswordmustcCustomInnerText =
                "Your new password must be less than 6 characters in length.";
            cPasswordComplexity.ValidateSymbolPolicyInAdminEmployee();

            /// Try and set the new password to one which does not conform
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Enter the new password
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Pass");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed when logging on, where password length is set 
        /// to 'less than'. NOTE: This is testing 'less than' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromLogonWherePasswordLengthIsLessThan()
        {
            /// Ensure password length is set to 'less than'
            ExPasswordComplexitySuccessfullySetPasswordLengthToLessThan();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Set the expiry date for the users password to 2010-01-01     
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            db.ExecuteSQL("UPDATE employees SET lastchange = '2010-01-01' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Enter an incorrect password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a valid password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Pass");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        #endregion


        #region Tests for 'length is greater than'

        /// <summary>
        /// This test ensures that a users password can be changed from My Details, where password length is set 
        /// to 'greater than'. NOTE: This is testing 'greater than' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromMyDetailsWherePasswordLengthIsGreaterThan()
        {
            /// Ensure password length is set to 'greater than'
            ExPasswordComplexitySuccessfullySetPasswordLengthToGreaterThan();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Change my Password page from My Details
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ValidateNumbersPolicyInMyDetailsExpectedValues.UIYournewpasswordmustcCustom1InnerText =
                "Your new password must be greater than 6 characters in length.";
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();

            /// Try and update the password to one which does not conform to policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Update the password successfully
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from the admin employees page, where password length is set 
        /// to 'greater than'. NOTE: This is testing 'greater than' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromAdminEmployeesWherePasswordLengthIsGreaterThan()
        {
            /// Ensure password length is set to 'greater than'            
            ExPasswordComplexitySuccessfullySetPasswordLengthToGreaterThan();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page and search for the current user
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployeeParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployee();
            cPasswordComplexity.ValidateSymbolPolicyInAdminEmployeeExpectedValues.UIYournewpasswordmustcCustomInnerText =
                "Your new password must be greater than 6 characters in length.";
            cPasswordComplexity.ValidateSymbolPolicyInAdminEmployee();

            /// Try and set the new password to one which does not conform
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Enter the new password
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            Playback.Wait(2000);

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed when logging on, where password length is set 
        /// to 'greater than'. NOTE: This is testing 'greater than' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromLogonWherePasswordLengthIsGreaterThan()
        {
            /// Ensure password length is set to 'greater than'
            ExPasswordComplexitySuccessfullySetPasswordLengthToGreaterThan();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Set the expiry date for the users password to 2010-01-01     
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            db.ExecuteSQL("UPDATE employees SET lastchange = '2010-01-01' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Enter an incorrect password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a valid password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        #endregion


        #region Tests for 'length is between'

        /// <summary>
        /// This test ensures that a users password can be changed from My Details, where password length is set 
        /// to 'between'. NOTE: This is testing 'between' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromMyDetailsWherePasswordLengthIsBetween()
        {
            /// Ensure password length is set to 'between'
            ExPasswordComplexitySuccessfullySetPasswordLengthToBetween();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            ///// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Change my Password page from My Details
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ValidateNumbersPolicyInMyDetailsExpectedValues.UIYournewpasswordmustcCustom1InnerText =
                "Your new password must be at least 6 and no greater than 10 characters in length.";
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();

            /// Try and update the password to one which does not conform to policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not conform to policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password123");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password123");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Update the password successfully
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password2");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password2");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password2");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from the admin employees page, where password length is set 
        /// to 'between'. NOTE: This is testing 'between' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromAdminEmployeesWherePasswordLengthIsBetween()
        {
            /// Ensure password length is set to 'between'
            ExPasswordComplexitySuccessfullySetPasswordLengthToBetween();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page and search for the current user
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployeeParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployee();
            cPasswordComplexity.ValidateSymbolPolicyInAdminEmployeeExpectedValues.UIYournewpasswordmustcCustomInnerText =
                "Your new password must be at least 6 and no greater than 10 characters in length.";
            cPasswordComplexity.ValidateSymbolPolicyInAdminEmployee();

            /// Try and set the new password to one which does not conform
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does not conform
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password123");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password123");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Enter the new password
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password2");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed when logging on, where password length is set 
        /// to 'between'. NOTE: This is testing 'betwee' exclusively
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromLogonWherePasswordLengthIsBetween()
        {
            /// Ensure password length is set to 'between'
            ExPasswordComplexitySuccessfullySetPasswordLengthToBetween();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Set the expiry date for the users password to 2010-01-01     
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            db.ExecuteSQL("UPDATE employees SET lastchange = '2010-01-01' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Enter an incorrect password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Pass");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter an incorrect password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password123");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password123");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a valid password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password2");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password2");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "Password2");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        #endregion


        #endregion


        #region Test the Password length options with all of the complexity options switch on


        #region Tests for 'length is equal to' with complexity on


        /// <summary>
        /// This test ensures that a users password can be changed from My Details, where password length is set 
        /// to 'equal to' and maximum password complexity is on
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromMyDetailsWherePasswordLengthIsEqualToAndAllComplexityOn()
        {
            /// Ensure password length is set to 'equal to' and complexity is on
            ExPasswordComplexitySuccessfullySetPasswordLengthToEqualTo();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSelectAllComplexity();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Change my Password page from My Details
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ValidateUpperCasePolicyInMyDetails();
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();
            cPasswordComplexity.ValidateSymbolPolicyInMyDetails();

            /// Try and update the password to one which does not conform to length policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssw1!!!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssw1!!!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain numbers
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssw!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssw!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain upper case letters
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ssw1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ssw1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain symbols
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Passw1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Passw1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Update the password successfully
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssw1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssw1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ssw1");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from the admin employees page, where password length is set 
        /// to 'equal to' and maximum password complexity is on
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromAdminEmployeesWherePasswordLengthIsEqualToAndAllComplexityOn()
        {
            /// Ensure password length is set to 'equal to' and complexity is on
            ExPasswordComplexitySuccessfullySetPasswordLengthToEqualTo();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSelectAllComplexity();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page and search for the current user
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployeeParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployee();
            cPasswordComplexity.ValidateNumbersPolicyInAdminEmployee();
            cPasswordComplexity.ValidateSymbolPolicyInAdminEmployee();
            cPasswordComplexity.ValidateUpperCasePolicyInAdminEmployee();

            /// Try and set the new password to one which does meet any criteria
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain numbers
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@sswo");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@sswo");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain symbols
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Passwo");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Passwo");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain upper case
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ssw0");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ssw0");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which contains all complexity but does not meet length
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssw01");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssw01");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Enter the new password
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssw1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssw1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            Playback.Wait(2000);

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ssw1");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed when logging on, where password length is set 
        /// to 'equal to' and maximum password complexity is on
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromLogonWherePasswordLengthIsEqualToAndAllComplexityOn()
        {
            /// Ensure password length is set to 'equal to' and complexity is on
            ExPasswordComplexitySuccessfullySetPasswordLengthToEqualTo();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSelectAllComplexity();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Set the expiry date for the users password to 2010-01-01     
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            db.ExecuteSQL("UPDATE employees SET lastchange = '2010-01-01' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Validate the policies - my details has been used as the DOM object is the same
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();
            cPasswordComplexity.ValidateSymbolPolicyInMyDetails();
            cPasswordComplexity.ValidateUpperCasePolicyInMyDetails();

            /// Enter a password that does not meet any criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet numbers criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@sswo");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@sswo");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet sybmols criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Passw0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Passw0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet upper case criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ssw0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ssw0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet length criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssw01");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssw01");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a valid password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssw0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssw0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ssw0");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        #endregion


        #region Tests for 'length is less than' with complexity on



        /// <summary>
        /// This test ensures that a users password can be changed from My Details, where password length is set 
        /// to 'less than' and maximum password complexity is on
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromMyDetailsWherePasswordLengthIsLessThanAndAllComplexityOn()
        {
            /// Ensure password length is set to 'less than' and complexity is on
            ExPasswordComplexitySuccessfullySetPasswordLengthToLessThan();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSelectAllComplexity();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Change my Password page from My Details
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ValidateUpperCasePolicyInMyDetails();
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();
            cPasswordComplexity.ValidateSymbolPolicyInMyDetails();

            /// Try and update the password to one which does not conform to length policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssw1!!!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssw1!!!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does conform to length policy but not complexity
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("pass");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("pass");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain numbers
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ss!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ss!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain upper case letters
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ss1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ss1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain symbols
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Pass1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Pass1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Update the password successfully
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ss1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ss1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ss1");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from the admin employees page, where password length is set 
        /// to 'less than' and maximum password complexity is on
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromAdminEmployeesWherePasswordLengthIsLessThanAndAllComplexityOn()
        {
            /// Ensure password length is set to 'less than' and complexity is on
            ExPasswordComplexitySuccessfullySetPasswordLengthToLessThan();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSelectAllComplexity();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page and search for the current user
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployeeParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployee();
            cPasswordComplexity.ValidateNumbersPolicyInAdminEmployee();
            cPasswordComplexity.ValidateSymbolPolicyInAdminEmployee();
            cPasswordComplexity.ValidateUpperCasePolicyInAdminEmployee();

            /// Try and set the new password to one which does meet any criteria
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain numbers
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssw");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssw");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain symbols
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Pass1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Pass1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain upper case
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ss1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ss1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which contains all complexity but does not meet length
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssw01");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssw01");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Enter the new password
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ss1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ss1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            Playback.Wait(2000);

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ss1");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed when logging on, where password length is set 
        /// to 'less than' and maximum password complexity is on
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromLogonWherePasswordLengthIsLessThanAndAllComplexityOn()
        {
            /// Ensure password length is set to 'less than' and complexity is on
            ExPasswordComplexitySuccessfullySetPasswordLengthToLessThan();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSelectAllComplexity();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Set the expiry date for the users password to 2010-01-01     
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            db.ExecuteSQL("UPDATE employees SET lastchange = '2010-01-01' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Validate the policies - my details has been used as the DOM object is the same
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();
            cPasswordComplexity.ValidateSymbolPolicyInMyDetails();
            cPasswordComplexity.ValidateUpperCasePolicyInMyDetails();

            /// Enter a password that does not meet any criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet numbers criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssw");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssw");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet sybmols criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Pass0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Pass0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet upper case criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ss0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ss0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet length criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssw01");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssw01");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a valid password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ss1");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ss1");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            Playback.Wait(3000);

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ss1");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        #endregion


        #region Tests for 'length is greater than' with complexity on


        /// <summary>
        /// This test ensures that a users password can be changed from My Details, where password length is set 
        /// to 'greater than' and maximum password complexity is on
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromMyDetailsWherePasswordLengthIsGreaterThanAndAllComplexityOn()
        {
            /// Ensure password length is set to 'greater than' and complexity is on
            ExPasswordComplexitySuccessfullySetPasswordLengthToGreaterThan();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSelectAllComplexity();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Change my Password page from My Details
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ValidateUpperCasePolicyInMyDetails();
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();
            cPasswordComplexity.ValidateSymbolPolicyInMyDetails();

            /// Try and update the password to one which does not conform to length policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@s1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@s1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does conform to length policy but not complexity
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("passwordreallylong");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("passwordreallylong");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain numbers
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain upper case letters
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ssword1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ssword1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain symbols
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password9");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password9");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Update the password successfully
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword2");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword2");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ssword2");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from the admin employees page, where password length is set 
        /// to 'greater than' and maximum password complexity is on
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromAdminEmployeesWherePasswordLengthIsGreaterThanAndAllComplexityOn()
        {
            /// Ensure password length is set to 'greater than' and complexity is on
            ExPasswordComplexitySuccessfullySetPasswordLengthToGreaterThan();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSelectAllComplexity();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page and search for the current user
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployeeParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployee();
            cPasswordComplexity.ValidateNumbersPolicyInAdminEmployee();
            cPasswordComplexity.ValidateSymbolPolicyInAdminEmployee();
            cPasswordComplexity.ValidateUpperCasePolicyInAdminEmployee();

            /// Try and set the new password to one which does meet any criteria
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("pass");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("pass");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain numbers
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain symbols
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain upper case
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ssword2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ssword2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which contains all complexity but does not meet length
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@s1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@s1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Enter the new password
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            Playback.Wait(2000);

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ssword2");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed when logging on, where password length is set 
        /// to 'greater than' and maximum password complexity is on
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromLogonWherePasswordLengthIsGreaterThanAndAllComplexityOn()
        {
            /// Ensure password length is set to 'greater than' and complexity is on
            ExPasswordComplexitySuccessfullySetPasswordLengthToGreaterThan();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSelectAllComplexity();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Set the expiry date for the users password to 2010-01-01     
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            db.ExecuteSQL("UPDATE employees SET lastchange = '2010-01-01' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Validate the policies - my details has been used as the DOM object is the same
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();
            cPasswordComplexity.ValidateSymbolPolicyInMyDetails();
            cPasswordComplexity.ValidateUpperCasePolicyInMyDetails();

            /// Enter a password that does not meet any criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("pass");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("pass");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet numbers criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet sybmols criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password3");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password3");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet upper case criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ssword0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ssword0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet length criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@s1");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@s1");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a valid password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword4");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword4");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ssword4");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        #endregion


        #region Tests for 'length is between' with complexity on


        /// <summary>
        /// This test ensures that a users password can be changed from My Details, where password length is set 
        /// to 'between' and maximum password complexity is on
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromMyDetailsWherePasswordLengthIsBetweenAndAllComplexityOn()
        {
            /// Ensure password length is set to 'between' and complexity is on
            ExPasswordComplexitySuccessfullySetPasswordLengthToBetween();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSelectAllComplexity();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Change my Password page from My Details
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ValidateUpperCasePolicyInMyDetails();
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();
            cPasswordComplexity.ValidateSymbolPolicyInMyDetails();

            /// Try and update the password to one which does not conform to minimum length policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@s1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@s1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not conform to maximum length policy
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword1!!!!!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword1!!!!!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does conform to length policy but not complexity
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("password");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain numbers
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword!");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain upper case letters
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ssword1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ssword1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Try and update the password to one which does not contain symbols
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password9");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password9");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Update the password successfully
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText("Password1");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword2");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword2");
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ssword2");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed from the admin employees page, where password length is set 
        /// to 'between' and maximum password complexity is on
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromAdminEmployeesWherePasswordLengthIsBetweenAndAllComplexityOn()
        {
            /// Ensure password length is set to 'between' and complexity is on
            ExPasswordComplexitySuccessfullySetPasswordLengthToBetween();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSelectAllComplexity();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page and search for the current user
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployeeParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cPasswordComplexity.ClickOnChangePasswordFromAdminEmployee();
            cPasswordComplexity.ValidateNumbersPolicyInAdminEmployee();
            cPasswordComplexity.ValidateSymbolPolicyInAdminEmployee();
            cPasswordComplexity.ValidateUpperCasePolicyInAdminEmployee();

            /// Try and set the new password to one which does meet any criteria
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("pass");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("pass");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain numbers
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain symbols
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which does contain upper case
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ssword2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ssword2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which contains all complexity but does not meet minimum length
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@s1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@s1");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Try and set the new password to one which contains all complexity but does not meet maximum length
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword1!!!!!");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword1!!!!!");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInAdminEmployee();

            /// Enter the new password
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword2");
            cPasswordComplexity.ChangePasswordFromAdminEmployeeWithSymbol();
            cPasswordComplexity.PressAdminEmployeeChangePasswordSaveButton();
            Playback.Wait(2000);

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ssword2");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }


        /// <summary>
        /// This test ensures that a users password can be changed when logging on, where password length is set 
        /// to 'between' and maximum password complexity is on
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordFromLogonWherePasswordLengthIsBetweenAndAllComplexityOn()
        {
            /// Ensure password length is set to 'between' and complexity is on
            ExPasswordComplexitySuccessfullySetPasswordLengthToBetween();
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSelectAllComplexity();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "0";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            Playback.Wait(2000);

            /// Set the expiry date for the users password to 2010-01-01     
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            db.ExecuteSQL("UPDATE employees SET lastchange = '2010-01-01' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Validate the policies - my details has been used as the DOM object is the same
            cPasswordComplexity.ValidateNumbersPolicyInMyDetails();
            cPasswordComplexity.ValidateSymbolPolicyInMyDetails();
            cPasswordComplexity.ValidateUpperCasePolicyInMyDetails();

            /// Enter a password that does not meet any criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("pass");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("pass");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet numbers criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet sybmols criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("Password3");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("Password3");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet upper case criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("p@ssword0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("p@ssword0");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet minimum length criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@s1");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@s1");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a password that does not meet maximum length criteria
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword1!!!!!");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword1!!!!!");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInLogonPage();

            /// Enter a valid password
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText("P@ssword4");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText("P@ssword4");
            cPasswordComplexity.ChangePasswordFromLogonPageWithSymbol();
            cPasswordComplexity.PressLogonPageChangePasswordSaveButton();

            /// Logon using the new password to make sure it works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, "P@ssword4");
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }

        #endregion

        #endregion


        #region Test the password expiry


        /// <summary>
        /// This test ensures that the password expiry functionality works correctly
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordWherePasswordExpirySet()
        {
            /// Create cDatabaseConnection
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            DateTime dtDate = new DateTime();
            string sDateToUse = string.Empty;

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Set password expiry to 5
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetLengthToAny();
            cPasswordComplexity.PasswordSettingsSetExpiryPeriodParams.UIPasswordexpiresafterEditText = "5";
            cPasswordComplexity.PasswordSettingsSetExpiryPeriod();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            /// Navigate back to general options to ensure the save method is given enough time
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Successfully logon (therefore testing lastchange date when it is today)
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();

            /// Update the employees lastchange date to five days ago
            dtDate = DateTime.Now.AddDays(-5);
            sDateToUse = dtDate.ToString("yyyy-MM-dd");
            db.ExecuteSQL("UPDATE employees SET lastchange = '" + sDateToUse + "' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Successfully logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();

            /// Update the employees lastchange date to six days ago
            dtDate = DateTime.Now.AddDays(-6);
            sDateToUse = dtDate.ToString("yyyy-MM-dd");
            db.ExecuteSQL("UPDATE employees SET lastchange = '" + sDateToUse + "' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");

            /// Logon and get the password screen
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);
            Assert.AreNotEqual(cPasswordComplexity.ValidateExpensesLogonAfterPasswordChangeExpectedValues.UIHomePane1InnerText, "Home Page");

            /// Reset lastchange date to today 
            dtDate = DateTime.Now;
            sDateToUse = dtDate.ToString("yyyy-MM-dd");
            db.ExecuteSQL("UPDATE employees SET lastchange = '" + sDateToUse + "' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'");
        }


        #endregion


        #region Test previous passwords not permitted


        /// <summary>
        /// This test ensures that a users password can be changed, where previous passwords not permitted is set to two
        /// </summary>
        [TestMethod]
        public void ExPasswordComplexitySuccessfullyChangePasswordWherePreviousNotPermittedIsSet()
        {
            /// Change the number of previous passwords not permitted to 2
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cPasswordComplexity.SelectPasswordSettingsWithinGeneralOptions();
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswordsParams.UIPreviouspasswordsnotEditText = "2";
            cPasswordComplexity.PasswordSettingsSetPreviousNotPermittedPasswords();
            cPasswordComplexity.PasswordSettingsSetComplexityToNone();
            cPasswordComplexity.PasswordSettingsSetLengthToAny();
            cPasswordComplexity.PressGeneralOptionsSaveButton();
            
            string sRandomPassword1 = "Password" + DateTime.Now.ToString("yyMMddHHmmss") + DateTime.Now.Millisecond.ToString();
            Playback.Wait(3);
            string sRandomPassword2 = "Password" + DateTime.Now.ToString("yyMMddHHmmss") + DateTime.Now.Millisecond.ToString();
            Playback.Wait(3);
            string sRandomPassword3 = "Password" + DateTime.Now.ToString("yyMMddHHmmss") + DateTime.Now.Millisecond.ToString();
            Playback.Wait(3);
            string sRandomPassword4 = "Password" + DateTime.Now.ToString("yyMMddHHmmss") + DateTime.Now.Millisecond.ToString();

            /// Change the password in My Details From Password1 to three new random passwords              
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText(cGlobalVariables.AdministratorPassword(ProductType.expenses));
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText(sRandomPassword1);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText(sRandomPassword1);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText(sRandomPassword1);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText(sRandomPassword2);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText(sRandomPassword2);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText(sRandomPassword2);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText(sRandomPassword3);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText(sRandomPassword3);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Try and change to Password to one that is not permitted
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/information/mydetails.aspx");
            cPasswordComplexity.ClickOnChangePasswordFromMyDetails();
            cPasswordComplexity.ValidatePreviousPasswordPolicy();
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText(sRandomPassword3);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText(sRandomPassword2);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText(sRandomPassword2);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();
            cPasswordComplexity.ValidatePasswordDoesNotConformInMyDetails();

            /// Change the password to the latest permitted password
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIOldPasswordEditPassword = Playback.EncryptText(sRandomPassword3);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UINewPasswordEditPassword = Playback.EncryptText(sRandomPassword4);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbolParams.UIConfirmNewPasswordEditPassword = Playback.EncryptText(sRandomPassword4);
            cPasswordComplexity.ChangePasswordFromMyDetailsWithSymbol();
            cPasswordComplexity.PressMyDetailsChangePasswordSaveButton();

            /// Logon to make sure the password works
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator, sRandomPassword4);
            cPasswordComplexity.ValidateExpensesLogonAfterPasswordChange();
        }

        #endregion



        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void ResetUserPassword()
        {
            cPasswords password = new cPasswords();
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());

            string sAdminPasswordReset = "UPDATE employees SET password = '" + password.Encrypt(cGlobalVariables.AdministratorPassword(ProductType.expenses)) + "' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'";
            string sAdminArchiveReset = "UPDATE employees SET archived = 0 WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'";
            string sClaimantPasswordReset = "UPDATE employees SET password = '" + password.Encrypt(cGlobalVariables.ClaimantPassword(ProductType.expenses)) + "' WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'";
            string sClaimantArchiveReset = "UPDATE employees SET archived = 0 WHERE username = '" + cGlobalVariables.AdministratorUserName(ProductType.expenses) + "'";

            db.ExecuteSQL(sAdminPasswordReset);
            db.ExecuteSQL(sAdminArchiveReset);
            db.ExecuteSQL(sClaimantPasswordReset);
            db.ExecuteSQL(sClaimantArchiveReset);
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
