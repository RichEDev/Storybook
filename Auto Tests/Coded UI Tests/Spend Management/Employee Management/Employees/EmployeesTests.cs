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


namespace Auto_Tests.Coded_UI_Tests.Spend_Management.User_Management.Employees
{
    /// <summary>
    /// Summary description for adding, editting and deleting employees 
    /// </summary>
    [CodedUITest]
    public class EmployeesTests
    {
        
        public EmployeesTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.EmployeesUIMapClasses.EmployeesUIMap cEmployeesMethods = new UIMaps.EmployeesUIMapClasses.EmployeesUIMap();
        
        ///<summary></summary>
        ///test to ensure mandatory fields are validated when adding a new employee
        ///</summary>       
        [TestMethod]
        public void EmployeesUnsuccessfullyAddEmployeeWithoutMandatoryFields()
        {
            /// Logon to Expenses
            
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to Employees Page

            cSharedMethods.NavigateToPage(ProductType.expenses,"/shared/admin/selectemployee.aspx");

            ///Validate correct page

            cEmployeesMethods.ValidateSelectEmployee();

            /// Add employee without populating mandatory fields

            cEmployeesMethods.AddEmployeeNoMandatoryData();

            ///Validate warning messages

            cEmployeesMethods.ValidateMandatoryTitle();
            cEmployeesMethods.ValidateMandatoryUsername();
            cEmployeesMethods.ValidateEmployeeFirstName();
            cEmployeesMethods.ValidateEmployeeSurname();
          
           
        }


        [TestMethod]
        public void EmployeesUnsuccessfullyAddEmployeeWithInvalidFields()
        {
            /// Logon to Expenses

            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to Employees Page

            cSharedMethods.NavigateToPage(ProductType.expenses,"/shared/admin/selectemployee.aspx");

            ///Validate correct page

            cEmployeesMethods.ValidateSelectEmployee();

            /// Add employee 

            cEmployeesMethods.AddEmployeeInvlaidFieldsParams.UITerminationDateEdit1Text="32132010";
            cEmployeesMethods.AddEmployeeInvlaidFieldsParams.UIHireDateEditText = "32132010";
            cEmployeesMethods.AddEmployeeInvlaidFields();

            ///Validate warning messages

            cEmployeesMethods.ValidateHireDateFormat();
            cEmployeesMethods.ValidateLeaveDateValidFormat();
            cEmployeesMethods.ValidateEmailAddressFormat();
        }


        [TestMethod]
        public void EmployeesUnsuccessfullyAddEmployeeWithCancel()
        {
            /// Logon to Expenses

            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to Employees Page

           cSharedMethods.NavigateToPage(ProductType.expenses,"/shared/admin/selectemployee.aspx");

            ///Validate correct page

            cEmployeesMethods.ValidateSelectEmployee();

            /// Add employee 

            cEmployeesMethods.AddEmployeeCancel();

            /// Search for employee

            cEmployeesMethods.SearchforUser();
            
            ///Validate warning messages

            cEmployeesMethods.ValidateEmployeeDoesNotExist();

        }


        [TestMethod]
        public void EmployeesUnsuccessfullyAddEmployeeWithoutMandatoryFieldsFramework()
        {
            /// Logon to Framework

            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Employees Page

            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/selectemployee.aspx");
            
            ///Validate correct page

            cEmployeesMethods.ValidateSelectEmployee();
         
            /// Add employee without populating mandatory fields

            cEmployeesMethods.AddEmployeeNoMandatoryData();
            
            ///Validate warning messages

            cEmployeesMethods.ValidateMandatoryTitle();
            cEmployeesMethods.ValidateMandatoryUsername();
            cEmployeesMethods.ValidateEmployeeFirstName();
            cEmployeesMethods.ValidateEmployeeSurname();
          
           
        }


        [TestMethod]
        public void EmployeesUnsuccessfullyAddEmployeeWithInvalidFieldsFramework()
        {
            /// Logon to Framework

            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Employees Page

            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/selectemployee.aspx");

            ///Validate correct page

            cEmployeesMethods.ValidateSelectEmployee();

            /// Add employee 

            cEmployeesMethods.AddEmployeeInvlaidFieldsParams.UITerminationDateEdit1Text = "32132010";
            cEmployeesMethods.AddEmployeeInvlaidFieldsParams.UIHireDateEditText = "32132010";
            cEmployeesMethods.AddEmployeeInvlaidFields();
    
            ///Validate warning messages

            cEmployeesMethods.ValidateHireDateFormat();
            cEmployeesMethods.ValidateLeaveDateValidFormat();
            cEmployeesMethods.ValidateEmailAddressFormat();
        }


        [TestMethod]
        public void EmployeesUnsuccessfullyAddEmployeeWithCancelFramework()
        {
            /// Logon to Framework

            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Employees Page

            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/selectemployee.aspx");

            ///Validate correct page

            cEmployeesMethods.ValidateSelectEmployee();

            /// Add employee 

            cEmployeesMethods.AddEmployeeFramweorkCancel();

            /// Search for employee

            cEmployeesMethods.SearchforUser();

            ///Validate warning messages

            cEmployeesMethods.ValidateEmployeeDoesNotExist();
        }


        [TestMethod]
        public void EmployeesSuccessfullyAddEmployee()
        {
            #region Logon and navigate to new employee page

            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            /// Press the New Employee link
            cEmployeesMethods.ClickNewEmployeeLink();

            #endregion


            #region Populate General Details tab

            /// Populate Logon Details section
            cEmployeesMethods.PopulateLogonDetailsSection();

            /// Populate Employee Name section
            cEmployeesMethods.PopulateEmployeeNameSection();

            /// Populate Employee Contact Details section
            cEmployeesMethods.PopulateEmployeeContactDetailsSection();

            /// Populate Regional Settings section
            cEmployeesMethods.PopulateRegionalSettingsSection();

            /// Set Email Employee options
            cEmployeesMethods.UnSelectSendPasswordEmailOption();

            #endregion


            #region Populate Permissions tab

            /// Click Permissions tab
            cEmployeesMethods.ClickPermissionsTab();

            /// Click Add Access Role link twice to bring up the modal
            cEmployeesMethods.ClickAddAccessRoleLink();
            cEmployeesMethods.ClickAddAccessRoleLink();

            /// Click the Manager access role
            cEmployeesMethods.SelectManagerAccessRole();

            /// Click the save button
            cEmployeesMethods.PressAccessRoleModalSaveButton();

            #endregion


            #region Populate Work tab

            /// Click Work tab
            cEmployeesMethods.ClickWorkTab();

            /// Populate Employment Information section
            cEmployeesMethods.PopulateEmployeeInformationFirstSection();

            cEmployeesMethods.ClickHireDateParams.UIHireDateEdit1Text = "__/__/____";
            cEmployeesMethods.ClickHireDate();
            cSharedMethods.TypeInDate("01/01/2010");

            cEmployeesMethods.ClickTerminationDateParams.UITerminationDateEdit2Text = "__/__/____";
            cEmployeesMethods.ClickTerminationDate();
            cSharedMethods.TypeInDate("01/01/2015");

            cEmployeesMethods.PopulateEmployeeInformationThirdSection();

            cEmployeesMethods.ClickStartingMileageDateAgainParams.UIStartingMileageDateEdit1Text = "__/__/____";
            cEmployeesMethods.ClickStartingMileageDateAgain();
            cSharedMethods.TypeInDate("01/01/2010");

            /// Populate NHS Details section
            cEmployeesMethods.PopulateNHSDetails();

            /// Populate ESR Assignment section
            cEmployeesMethods.ClickAddESRAssignmentLink();

            cEmployeesMethods.PopulateESRAssignmentDetails();

            cEmployeesMethods.ClickESRAssignmentStartDate();

            cSharedMethods.TypeInDate("03/03/2009");

            cEmployeesMethods.ClickESRAssignmentEndDate();

            cSharedMethods.TypeInDate("04/04/2015");

            cEmployeesMethods.PressESRAssignmentModalSave();

            /// Select Cost Centre Breakdown
            cEmployeesMethods.PopulateCostCentreBreakdown();

            #endregion


            #region Populate Personal tab

            /// Click Personal tab
            cEmployeesMethods.ClickPersonalTab();

            /// Populate Home Contact Details
            cEmployeesMethods.PopulateHomeContactDetails();

            /// Populate Driving Licence Details
            cEmployeesMethods.PopulateDrivingLicenceDetails();

            cEmployeesMethods.ClickLicenceExpiryDate();

            cSharedMethods.TypeInDate("06/06/2016");

            cEmployeesMethods.ClickLastChecked();

            cSharedMethods.TypeInDate("01/01/2011");
            #region Upload driving licence 
            /// Upload test driving licence document
            //cEmployeesMethods.ClickDrivingLicenceGreenCross();

            //cEmployeesMethods.ClickDrivingLicenceModalBrowse();

            //cEmployeesMethods.SelectMyDocumentsTestFile();

            //cEmployeesMethods.PressDrivingLicenceModalUpload();

            //cEmployeesMethods.PressDrivingLicenceModalCancel();
            #endregion
            /// Populate Bank Details
            cEmployeesMethods.PopulateBankDetails();

            /// Populate Personal Information
            cEmployeesMethods.PopulatePersonalInformation();

            cEmployeesMethods.ClickDateOfBirth();

            cSharedMethods.TypeInDate("06/11/1986");

            #endregion


            #region Populate Claims tab

            cEmployeesMethods.ClickClaimsTab();

            cEmployeesMethods.PopulateClaimSignoffParams.UISignoffGroupAdvancesComboBox1SelectedItem = "Line Manager";
            cEmployeesMethods.PopulateClaimSignoffParams.UISignoffGroupComboBox1SelectedItem = "Line Manager";
            cEmployeesMethods.PopulateClaimSignoffParams.UISignoffGroupCreditCaComboBoxSelectedItem = "Line Manager";
            cEmployeesMethods.PopulateClaimSignoffParams.UISignoffGroupPurchaseComboBoxSelectedItem = "Line Manager";
            cEmployeesMethods.PopulateClaimSignoff();

            cEmployeesMethods.ClickAddItemRoleLink();

            cEmployeesMethods.SelectManagerItemRole();

            cEmployeesMethods.PressItemRoleSave();

            #endregion


            #region Notifications tab

            cEmployeesMethods.ClickNotificationsTab();

            cEmployeesMethods.SelectAllNotificationBoxes();

            #endregion


            #region Press save and view employee record

            cEmployeesMethods.PressEmployeeRecordSave();

            cEmployeesMethods.SearchForCodedUIAdminParams.UIUsernameEditText = "__AutomatedEmployee";
            cEmployeesMethods.SearchForCodedUIAdmin();

            cEmployeesMethods.ValidateCodedUIAdminEmployeeExists();
          
            cEmployeesMethods.ClickEditCodedUIAdminEmployee();

            #endregion


            #region Validate employee record

            /// Validate General Details tab
            cEmployeesMethods.ValidateLogonDetails();
            cEmployeesMethods.ValidateEmployeeNameSection();
            cEmployeesMethods.ValidateEmploymentContactDetails();
            cEmployeesMethods.ValidateRegionalSettings();
            
            /// Validate Permissions tab
            cEmployeesMethods.ClickPermissionsTab();
            cEmployeesMethods.ValidateAccessRole();
            
            /// Validate Work tab
            cEmployeesMethods.ClickWorkTab();
            cEmployeesMethods.ValidateEmployeeInformation();
            cEmployeesMethods.ValidateNHSDetails();
            cEmployeesMethods.ValidateESRAssignmentNumbers();
            cEmployeesMethods.ValidateCostCentreBreakdown();
            
            /// Validate Personal tab
            cEmployeesMethods.ClickPersonalTab();
            cEmployeesMethods.ValidateHomeContractDetails();
            cEmployeesMethods.ValidateDrivingLicenceDetails();
            cEmployeesMethods.ValidateBankDetails();
            cEmployeesMethods.ValidatePersonalInformation();
            
            /// Validate Claims tab
            cEmployeesMethods.ClickClaimsTab();
            cEmployeesMethods.ValidateClaimSignoffExpectedValues.UISignoffGroupAdvancesComboBoxSelectedItem = "Line Manager";
            cEmployeesMethods.ValidateClaimSignoffExpectedValues.UISignoffGroupComboBoxSelectedItem = "Line Manager";
            cEmployeesMethods.ValidateClaimSignoffExpectedValues.UISignoffGroupCreditCaComboBoxSelectedItem = "Line Manager";
            cEmployeesMethods.ValidateClaimSignoffExpectedValues.UISignoffGroupPurchaseComboBoxSelectedItem = "Line Manager";
            cEmployeesMethods.ValidateClaimSignoff();
            cEmployeesMethods.ValidateItemRoles();
            
            /// Validate Notifications tab
            cEmployeesMethods.ClickNotificationsTab();
            cEmployeesMethods.ValidateNotifications();

            #endregion
        }


        [TestMethod]
        public void EmployeesUnsuccessfullyAddDuplicateEmployee()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            /// Press the New Employee link
            cEmployeesMethods.ClickNewEmployeeLink();
           
            /// Attempt to add employee with a duplicate username
            
            #region Populate General Details tab

            /// Populate Logon Details section
            cEmployeesMethods.PopulateLogonDetailsSection();

            /// Populate Employee Name section
            cEmployeesMethods.PopulateEmployeeNameSection();

            /// Populate Employee Contact Details section
            cEmployeesMethods.PopulateEmployeeContactDetailsSection();

            /// Populate Regional Settings section
            cEmployeesMethods.PopulateRegionalSettingsSection();

            /// Set Email Employee options
            cEmployeesMethods.UnSelectSendPasswordEmailOption();

            #endregion

            /// Validate error message

            cEmployeesMethods.PressEmployeeRecordSave();

            cEmployeesMethods.ValidateDuplicateEmployeeMessage();

            cEmployeesMethods.PressDuplicateEmployeeMessageOK();
        }


        [TestMethod]
        public void EmployeesSuccessfullyAddFrameworkEmployee()
        {

            #region Logon and navigate to new employee page

            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the select employee page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/selectemployee.aspx");

            /// Press the New Employee link
            cEmployeesMethods.ClickNewEmployeeLink();

            #endregion


            #region Populate General Details tab

            /// Populate Logon Details section
            cEmployeesMethods.PopulateLogonDetailsSection();

            /// Populate Employee Name section
            cEmployeesMethods.PopulateEmployeeNameSection();

            /// Populate Employee Contact Details section
            cEmployeesMethods.PopulateEmployeeContactDetailsSection();

            /// Populate Regional Settings section
            cEmployeesMethods.PopulateRegionalSettingsSection();

            /// Set Email Employee options
            cEmployeesMethods.UnSelectSendPasswordEmailOption();

            #endregion


            #region Populate Permissions tab

            /// Click Permissions tab
            cEmployeesMethods.ClickPermissionsTab();

            /// Click Add Access Role link twice to bring up the modal
            cEmployeesMethods.ClickAddAccessRoleLink();
            cEmployeesMethods.ClickAddAccessRoleLink();

            /// Click the Manager access role
            //cEmployeesMethods.FrameworkSetAccessRoleParams.UISelectgridNewAccessRCheckBoxChecked;
            cEmployeesMethods.FrameworkSetAccessRole();


            #endregion


            #region Populate Work tab

            /// Click Work tab
            cEmployeesMethods.ClickWorkTab();

            /// Populate Employment Information section
            cEmployeesMethods.PopulateEmployeeInformationFirstSection();

            cEmployeesMethods.ClickHireDateParams.UIHireDateEdit1Text = "__/__/____";
            cEmployeesMethods.ClickHireDate();
            cSharedMethods.TypeInDate("01/01/2010");

            cEmployeesMethods.ClickTerminationDateParams.UITerminationDateEdit2Text = "__/__/____";
            cEmployeesMethods.ClickTerminationDate();
            cSharedMethods.TypeInDate("01/01/2015");

            cEmployeesMethods.PopulatePrimaryCountryPrimaryCurrency();

            cEmployeesMethods.PressEmployeeRecordSave();

            #endregion           


            #region Validate the add

            cEmployeesMethods.SearchForCodedUIAdminParams.UIUsernameEditText = "__AutomatedEmployee";
            cEmployeesMethods.SearchForCodedUIAdmin();

            cEmployeesMethods.ValidateFrameworkEmployeeExists();

            cEmployeesMethods.ClickEditCodedUIAdminEmployee();

            cEmployeesMethods.ValidateLogonDetails();
            cEmployeesMethods.ValidateEmployeeNameSection();
            cEmployeesMethods.ValidateEmploymentContactDetails();
            cEmployeesMethods.ValidateRegionalSettings();
            cEmployeesMethods.ValidateAccessRoleFramework();
            cEmployeesMethods.ValidateFrameworkEmployeeInformation();

            #endregion
        }


        [TestMethod]
        public void EmployeesUnsuccessfullyAddDuplicateFrameworkEmployee()
        {
            #region Logon and navigate to new employee page

            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the select employee page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/selectemployee.aspx");

            /// Press the New Employee link
            cEmployeesMethods.ClickNewEmployeeLink();

            #endregion


            #region Populate General Details tab

            /// Populate Logon Details section
            cEmployeesMethods.PopulateLogonDetailsSection();

            /// Populate Employee Name section
            cEmployeesMethods.PopulateEmployeeNameSection();

            /// Populate Employee Contact Details section
            cEmployeesMethods.PopulateEmployeeContactDetailsSection();

            /// Populate Regional Settings section
            cEmployeesMethods.PopulateRegionalSettingsSection();

            /// Set Email Employee options
            cEmployeesMethods.UnSelectSendPasswordEmailOption();

            #endregion


            #region Validate employee cannot be added

            cEmployeesMethods.PressEmployeeRecordSave();

            cEmployeesMethods.ValidateDuplicateEmployeeMessage();

            cEmployeesMethods.PressDuplicateEmployeeMessageOK();

            #endregion
        }


        [TestMethod]
        public void EmployeesSuccessfullyArchiveFrameworkEmployee()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to employee page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/selectemployee.aspx");

            /// Search for user and press the archive button
            cEmployeesMethods.SearchForCodedUIAdminParams.UIUsernameEditText = "__AutomatedEmployee";
            cEmployeesMethods.SearchForCodedUIAdmin();
            cEmployeesMethods.ClickEmployeeArchiveIcon();

            /// Validate employee is archived
            cEmployeesMethods.ValidateArchiveEmployee();
        }


        [TestMethod]
        public void EmployeesSuccessfullyDeleteFrameworkEmployee()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to employee page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/selectemployee.aspx");

            /// Search for user and press the delete button
            cEmployeesMethods.SearchForCodedUIAdminParams.UIUsernameEditText = "__AutomatedEmployee";
            cEmployeesMethods.SearchForCodedUIAdmin();
            cEmployeesMethods.ClickEmployeeDeleteIcon();
            cEmployeesMethods.ClickEmployeeDeleteConfirmationOK();

            /// Validate the employee is deleted
            cEmployeesMethods.ValidateEmployeeDoesNotExist();
        }


        [TestMethod]
        public void EmployeesUnsuccessfullyEditEmployeeWithCancel()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the select employee page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            /// Edit the employee
            cEmployeesMethods.SearchForCodedUIAdminParams.UIUsernameEditText = "__AutomatedEmployee";
            cEmployeesMethods.SearchForCodedUIAdmin();
            cEmployeesMethods.ClickEditEmployeeIcon();


            /// Update the employee's name information and press the cancel button
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UIFirstNameEditText = "Automated EDITED";
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UIMiddleNamesEditText = "Middlename EDITED";
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UISurnameEditText = "Employee EDITED";
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UITitleEditText = "Mrs";
            cEmployeesMethods.PopulateEmployeeNameSection();
            cEmployeesMethods.PressEmployeeRecordCancel();

            /// Validate the employee's name has not been changed
            cEmployeesMethods.SearchForCodedUIAdminParams.UIUsernameEditText = "__AutomatedEmployee";
            cEmployeesMethods.SearchForCodedUIAdmin();
            cEmployeesMethods.ValidateCodedUIAdminEmployeeExists();
        }


        [TestMethod]
        public void EmployeesSuccessfullyEditEmployee()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to employee page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            /// Search for user and press the edit button
            cEmployeesMethods.SearchForCodedUIAdminParams.UIUsernameEditText = "__AutomatedEmployee";
            cEmployeesMethods.SearchForCodedUIAdmin();
            cEmployeesMethods.ClickEditEmployeeIcon();

            /// Edit the users details and press save

            #region General Details Tab

            cEmployeesMethods.PopulateLogonDetailsSectionParams.UIUsernameEditText = "__AutomatedEmployeeEDITED";
            cEmployeesMethods.PopulateLogonDetailsSection();
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UIFirstNameEditText = "Automated EDITED";
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UIMiddleNamesEditText = "Middlename EDITED";
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UIMaidenNameEditText = "Maiden EDITED";
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UISurnameEditText = "Employee EDITED";
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UITitleEditText = "Mrs";
            cEmployeesMethods.PopulateEmployeeNameSection();
            cEmployeesMethods.PopulateEmployeeContactDetailsSectionParams.UIEmailAddressEditText = "edited@edited.com";
            cEmployeesMethods.PopulateEmployeeContactDetailsSectionParams.UIExtensionNumberEditText = "321";
            cEmployeesMethods.PopulateEmployeeContactDetailsSectionParams.UIMobileNumberEditText = "654";
            cEmployeesMethods.PopulateEmployeeContactDetailsSectionParams.UIPagerNumberEditText = "987";
            cEmployeesMethods.PopulateEmployeeContactDetailsSection();
            cEmployeesMethods.PopulateRegionalSettingsSectionParams.UILocaleComboBoxSelectedItem = "[None]";
            cEmployeesMethods.PopulateRegionalSettingsSection();

            #endregion


            #region Permissions Tab

            cEmployeesMethods.ClickPermissionsTab();
            cEmployeesMethods.DeleteManagerAccessRole();

            #endregion


            #region Work Tab

            cEmployeesMethods.ClickWorkTab();
            cEmployeesMethods.PopulateEmployeeInformationFirstSectionParams.UICreditAccountEdit1Text = "7531";
            cEmployeesMethods.PopulateEmployeeInformationFirstSectionParams.UINationalInsuranceNumEdit1Text = "8888";
            cEmployeesMethods.PopulateEmployeeInformationFirstSectionParams.UIPayrollNumberEdit1Text = "1357";
            cEmployeesMethods.PopulateEmployeeInformationFirstSectionParams.UIPositionEdit1Text = "Claimant";
            cEmployeesMethods.PopulateEmployeeInformationFirstSection();
            cEmployeesMethods.ClickHireDateParams.UIHireDateEdit1Text = "__/__/____";
            cEmployeesMethods.ClickHireDate();
            cSharedMethods.TypeInDate("09/09/2008");

            cEmployeesMethods.ClickTerminationDateParams.UITerminationDateEdit2Text = "__/__/____";
            cEmployeesMethods.ClickTerminationDate();
            cSharedMethods.TypeInDate("12/12/2009");

            cEmployeesMethods.PopulateEmployeeInformationSecondSectionParams.UIPrimaryCountryComboBoxSelectedItem = "United States";
            cEmployeesMethods.PopulateEmployeeInformationSecondSectionParams.UIPrimaryCurrencyComboBoxSelectedItem = "Euro";
            cEmployeesMethods.PopulateEmployeeInformationSecondSectionParams.UILineManagerComboBoxSelectedItem = "Newton, Mr Darren (darren)";
            cEmployeesMethods.PopulateEmployeeInformationSecondSectionParams.UIStartingMileageEditText = "999";
            cEmployeesMethods.PopulateEmployeeInformationSecondSection();

            cEmployeesMethods.ClickStartingMileageDateParams.UIStartingMileageDateEditText = "__/__/____";
            cEmployeesMethods.ClickStartingMileageDate();
            cSharedMethods.TypeInDate("07/07/2007");

            cEmployeesMethods.PopulateNHSDetailsParams.UITrustComboBox1SelectedItem = "[None]";
            cEmployeesMethods.PopulateNHSDetails();

            #endregion


            #region Personal Tab

            cEmployeesMethods.ClickPersonalTab();
            cEmployeesMethods.PopulateHomeContactDetailsParams.UIEmailAddressEdit2Text = "editedhome@editedhome.com";
            cEmployeesMethods.PopulateHomeContactDetailsParams.UIFaxNumberEdit1Text = "887766";
            cEmployeesMethods.PopulateHomeContactDetailsParams.UITelephoneNumberEdit1Text = "665544";
            cEmployeesMethods.PopulateHomeContactDetails();

            #endregion


            #region Claims Tab

            cEmployeesMethods.ClickClaimsTab();

            cEmployeesMethods.PopulateClaimSignoffParams.UISignoffGroupAdvancesComboBox1SelectedItem = "[None]";
            cEmployeesMethods.PopulateClaimSignoffParams.UISignoffGroupComboBox1SelectedItem = "[None]";
            cEmployeesMethods.PopulateClaimSignoffParams.UISignoffGroupCreditCaComboBoxSelectedItem = "[None]";
            cEmployeesMethods.PopulateClaimSignoffParams.UISignoffGroupPurchaseComboBoxSelectedItem = "[None]";
            cEmployeesMethods.PopulateClaimSignoff();

            #endregion


            #region Notifications Tab

            cEmployeesMethods.ClickNotificationsTab();

            cEmployeesMethods.UnselectAllNotificationBoxes();

            cEmployeesMethods.PressEmployeeRecordSave();

            #endregion

            /// Search for the user and validate the change
            cEmployeesMethods.SearchForCodedUIAdminParams.UIUsernameEditText = "__AutomatedEmployee";
            cEmployeesMethods.SearchForCodedUIAdmin();
            cEmployeesMethods.ValidateEmployeeEdited();
            
            /// Edit the user and validate the change
            cEmployeesMethods.ClickEditedEmployeeEditIcon();

            cEmployeesMethods.ValidateEditedGeneralDetails();
            cEmployeesMethods.ValidateNoAccessRoleSelected();
            cEmployeesMethods.ValidateEditedWorkTab();
            cEmployeesMethods.ValidatePersonalTab();

            cEmployeesMethods.ValidateClaimsTabExpectedValues.UISignoffGroupComboBoxSelectedItem         = "[None]";
            cEmployeesMethods.ValidateClaimsTabExpectedValues.UISignoffGroupAdvancesComboBoxSelectedItem = "[None]";
            cEmployeesMethods.ValidateClaimsTabExpectedValues.UISignoffGroupCreditCaComboBoxSelectedItem = "[None]";
            cEmployeesMethods.ValidateClaimsTabExpectedValues.UISignoffGroupPurchaseComboBoxSelectedItem = "[None]";


            cEmployeesMethods.ValidateClaimsTab();
            cEmployeesMethods.ValidateEditedNotificationsTab();
            
            /// Amend the basic details back to default for future tests
            cEmployeesMethods.PopulateLogonDetailsSectionParams.UIUsernameEditText = "__AutomatedEmployee";
            cEmployeesMethods.PopulateLogonDetailsSection();
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UIFirstNameEditText = "Automated";
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UIMiddleNamesEditText = "Middlename";
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UIMaidenNameEditText = "Maiden";
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UISurnameEditText = "Employee";
            cEmployeesMethods.PopulateEmployeeNameSectionParams.UITitleEditText = "Mrs";
            cEmployeesMethods.PopulateEmployeeNameSection();
            cEmployeesMethods.PressEmployeeRecordSave();
        }


        [TestMethod]
        public void EmployeesSuccessfullyArchiveEmployee()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to employee page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            /// Search for user and press the archive button
            cEmployeesMethods.SearchForCodedUIAdminParams.UIUsernameEditText = "__AutomatedEmployee";
            cEmployeesMethods.SearchForCodedUIAdmin();
            cEmployeesMethods.ClickEmployeeArchiveIcon();

            /// Validate employee is archived
            cEmployeesMethods.ValidateArchiveEmployee();
        }


        [TestMethod]
        public void EmployeesSuccessfullyUnarchiveEmployee()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to employee page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            /// Search for user and press the un-archive button
            cEmployeesMethods.SearchForCodedUIAdminParams.UIUsernameEditText = "__AutomatedEmployee";
            cEmployeesMethods.SearchForCodedUIAdmin();
            cEmployeesMethods.ClickEmployeeUnArchiveIcon();

            /// Validate employee is archived
            cEmployeesMethods.ValidateUnarchiveEmployee();
        }


        [TestMethod]
        public void EmployeesUnsuccessfullyDeleteEmployeeWhereNotArchived()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to employee page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            /// Search for user and press the delete button
            cEmployeesMethods.SearchForCodedUIAdminParams.UIUsernameEditText = "__AutomatedEmployee";
            cEmployeesMethods.SearchForCodedUIAdmin();
            cEmployeesMethods.ClickEmployeeDeleteIcon();
            cEmployeesMethods.ClickEmployeeDeleteConfirmationOK();

            /// Validate the employee is not deleted
            cEmployeesMethods.ValidateUnarchiveEmployeeDeleteMessage();
            cEmployeesMethods.ClickUnarchivedEmployeeMessageOK();
        }


        [TestMethod]
        public void EmployeesSuccessfullyDeleteEmployee()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to employee page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            /// Search for user and press the delete button
            cEmployeesMethods.SearchForCodedUIAdminParams.UIUsernameEditText = "__AutomatedEmployee";
            cEmployeesMethods.SearchForCodedUIAdmin();
            cEmployeesMethods.ClickEmployeeDeleteIcon();
            cEmployeesMethods.ClickEmployeeDeleteConfirmationOK();

            /// Validate the employee is deleted
            cEmployeesMethods.ValidateEmployeeDoesNotExist();
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
