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
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using System.Threading; 


namespace Auto_Tests.Coded_UI_Tests.Expenses.Tailoring.General_Options
{
    /// <summary>
    /// This class contains all of the Expenses specific tests for Password Complexity
    /// </summary>
    [CodedUITest]
    public class GeneralOptionsTests
    {
        public GeneralOptionsTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.GeneralOptionsUIMapClasses.GeneralOptionsUIMap cGeneralOptions = new UIMaps.GeneralOptionsUIMapClasses.GeneralOptionsUIMap();


        /// <summary>
        /// Validate all options can be changed within Expenses general options - General Details
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateGeneralDetailsWithInExpensesGeneralOptions()
        {

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXGeneralDetailsApproveOwnClaims);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXGeneralDetailsCashCreditCardItems);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXGeneralDetailsClaimantsRequiredOdoReadings);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXGeneralDetailsClaimsPartSubmitted);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXGeneralDetailsDisplayBankDetails);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXGeneralDetailsEditOwnDetails);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXGeneralDetailsEmailHotelReviews);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXGeneralDetailsEmployeeDirectory);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXGeneralDetailsEnterPreApprovedClaims);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXGeneralDetailsHotelReviews);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXGeneralDetailsApproveOwnClaim);

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Make sure all checkboxes are un-checked
            cGeneralOptions.EXGeneralDetailsUnTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Validate each checkbox can be ticked seperately
            validateCheckBox(liCheckboes, true, ProductType.expenses, GeneralOptionsPage.none, GeneralOptionsTab.none);

            /// Make sure all checkbox are ticked
            cGeneralOptions.EXGeneralDetailsTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

            /// Validate each checkbox can be un-ticked seperately
            validateCheckBox(liCheckboes, false, ProductType.expenses, GeneralOptionsPage.none, GeneralOptionsTab.none);

            /// Validate each drop down list can be changed
            cGeneralOptions.EXGeneralDetailsUpdateDropDownListsFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ExGeneralDetailsValidateDropDownListsFirstValue();

            cGeneralOptions.EXGeneralDetailsUpdateDropDownListsSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ExGeneralDetailsValidateDropDownListsSecondValue();

            /// Validate radio buttons can be updated
            cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXGeneralDetailsClaimantsRequiredOdoReadings.Checked = false;
            cGeneralOptions.EXGeneralDetailsUpdateRadioButtonsToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ExGeneralDetailsValidateRadioButtonsFirstValues();

            cGeneralOptions.EXGeneralDetailsUpdateRadioButtonsToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ExGeneralDetailsValidateRadioButtonsSecondValues();

            /// Validate odometer reading date
            cGeneralOptions.EXGeneralDetailsSetOdometerReadingDate();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ExGeneralDetailsValidateOdometerReadingsDate();
        }


        /// <summary>
        /// Validate all options can be changed within Expenses general options - Self Registration.
        /// The 'Allow Self Registration' checkbox has to be tested separately as it dictates 
        /// whether all of the other checkboxes are enabled
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateSelfRegWithInExpensesGeneralOptions()
        {

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegAdditionalUDFs);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegAdvancesSignoffGroup);
            // liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegAllowSelfReg);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegBankDetails);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegCarDetails);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegDefaultDepartment);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegEmployeeContactDetails);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegEmployeeInformation);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegHomeAddressDetails);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegItemRole);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegRole);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegSignoffGroup);

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsSelfRegTab();
            
            /// Make sure all checkboxes are un-checked
            if (cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegAllowSelfReg.Checked)
            {
                cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegAllowSelfReg.Checked = false;
                cGeneralOptions.PressDisableSelfRegMessageOK();
            }            
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsSelfRegTab();

            /// Validate the 'Allow Self Registration' checkbox is un-checked and that it can be selected
            Assert.AreEqual(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegAllowSelfReg.Checked, false);
            cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegAllowSelfReg.Checked = true;
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsSelfRegTab();
            Assert.AreEqual(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXSelfRegAllowSelfReg.Checked, true);
            
            /// Validate each checkbox can be ticked seperately
            validateCheckBox(liCheckboes, true, ProductType.expenses, GeneralOptionsPage.none, GeneralOptionsTab.selfReg);

            /// Make sure all checkboxes are ticked
            cGeneralOptions.EXSelfRegTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsSelfRegTab();

            /// Validate each checkbox can be un-ticked seperately
            validateCheckBox(liCheckboes, false, ProductType.expenses, GeneralOptionsPage.none, GeneralOptionsTab.selfReg);

            /// Validate each drop down list can be changed
            cGeneralOptions.EXSelfRegUpdateAllDropDownsToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsSelfRegTab();
            cGeneralOptions.ExSelfRegValidateDropDownListFirstValue();

            cGeneralOptions.EXSelfRegUpdateAllDropDownsToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsSelfRegTab();
            cGeneralOptions.ExSelfRegValidateDropDownListSecondValue();
        }


        /// <summary>
        /// Validate all options can be changed within Expenses general options - Delegates
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateDelegatesWithInExpensesGeneralOptions()
        {

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDelUpdatedApproveAdvanceRequests);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDelUpdatedAuditLog);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDelUpdatedCheckandpayexpenses);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDelUpdatedExportData);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDelUpdatedImportCorpStatement);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDelUpdatedModifyCatsSystemOpts);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDelUpdatedModifyEmployees);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDelUpdatedQuickEditDesign);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDelUpdatedSubmitClaims);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDelUpdatedViewAdminReports);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDelUpdatedViewClaimantReports);

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsDelegatesTab();

            /// Make sure all checkboxes are un-checked
            cGeneralOptions.EXDelegatesUnTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsDelegatesTab();

            /// Validate each checkbox can be ticked seperately
            validateCheckBox(liCheckboes, true, ProductType.expenses, GeneralOptionsPage.none, GeneralOptionsTab.delegates);

            /// Make sure all checkboxes are ticked
            cGeneralOptions.EXDelegatesTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsDelegatesTab();

            /// Validate each checkbox can be un-ticked seperately
            validateCheckBox(liCheckboes, false, ProductType.expenses, GeneralOptionsPage.none, GeneralOptionsTab.delegates);        
        }


        /// <summary>
        /// Validate all options can be changed within Expenses general options - Declaration
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateDeclarationWithInExpensesGeneralOptions()
        {

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDeclarationElectronicDecl);

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsDeclarationTab();

            /// Make sure all checkboxes are un-checked
            cGeneralOptions.EXDeclarationUnTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsDeclarationTab();

            /// Validate each checkbox can be ticked seperately
            validateCheckBox(liCheckboes, true, ProductType.expenses, GeneralOptionsPage.none, GeneralOptionsTab.declaration);

            /// Make sure all checkboxes are ticked
            cGeneralOptions.EXDeclarationTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsDeclarationTab();

            /// Validate each checkbox can be un-ticked seperately
            validateCheckBox(liCheckboes, false, ProductType.expenses, GeneralOptionsPage.none, GeneralOptionsTab.declaration);

            /// Validate each text box can be updated
            cGeneralOptions.EXDeclarationUpdateAllTextBoxesToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsDeclarationTab();
            cGeneralOptions.ExDeclarationValidateTextBoxesFirstValue();

            cGeneralOptions.EXDeclarationUpdateAllTextBoxesToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsDeclarationTab();
            cGeneralOptions.ExDeclarationValidateTextBoxesSecondValue();
        }


        /// <summary>
        /// Validate all options can be changed within Expenses general options - Code Allocation
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateCodeAllocationWithInExpensesGeneralOptions()
        {

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXCodeAllocationClaimantsCostCodeBreakdown);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXCodeAllocationClaimantsshouldbeshoCheckBox);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXCodeAllocationClaimantsshowndeppobreakdown);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXCodeAllocationClaimantsshownprojectcode);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXCodeAllocationCostCodesInGenDetails);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXCodeAllocationDepartmentsshowningendetails);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXCodeAllocationDepoDesc);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXCodeAllocationItemsAssignedCostCodes);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXCodeAllocationItemsshouldbeassigneCheckBox);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXCodeAllocationProjectcodes);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXCodeAllocationProjectCodesDesc);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXCodeAllocationProjectcodesingendetails);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXCodeAllocationUsedefaultallocationCheckBox);            

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesCodeAllocationTab();

            /// Make sure all checkboxes are un-checked
            cGeneralOptions.EXCodeAllocationUnTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesCodeAllocationTab();

            /// Validate each checkbox can be ticked seperately
            validateCheckBox(liCheckboes, true, ProductType.expenses, GeneralOptionsPage.newExpenses, GeneralOptionsTab.codeAllocation);

            /// Make sure all checkboxes are ticked
            cGeneralOptions.EXCodeAllocationTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesCodeAllocationTab();

            /// Validate each checkbox can be un-ticked seperately
            validateCheckBox(liCheckboes, false, ProductType.expenses, GeneralOptionsPage.newExpenses, GeneralOptionsTab.codeAllocation);        
        }


        /// <summary>
        /// Validate all options can be changed within Expenses general options - Duty of Care
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateDutyofCareWithInExpensesGeneralOptions()
        {

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDutyofCareDrivingLicenceExpiryDate);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDutyofCareInsuranceExpiryDate);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDutyofCareMOTExpiry);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXDutyofCareTaxExpiryDate);

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesDutyofCareTab();

            /// Make sure all checkboxes are un-checked
            cGeneralOptions.EXDutyofCareUnTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesDutyofCareTab();

            /// Validate each checkbox can be ticked seperately
            validateCheckBox(liCheckboes, true, ProductType.expenses, GeneralOptionsPage.newExpenses, GeneralOptionsTab.dutyOfCare);

            /// Make sure all checkboxes are ticked
            cGeneralOptions.EXDutyofCareTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesDutyofCareTab();

            /// Validate each checkbox can be un-ticked seperately
            validateCheckBox(liCheckboes, false, ProductType.expenses, GeneralOptionsPage.newExpenses, GeneralOptionsTab.dutyOfCare);        
        }


        /// <summary>
        /// Validate all options can be changed within Expenses general options - Mileage and Cars
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateMileageandCarsWithInExpensesGeneralOptions()
        {

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXMileageandCarsaddnewaddressascompany);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXMileageandCarsallowemployeestoaddnewcars);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXMileageandCarsAllowMultipleDestinations);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXMileageandCarsClaimantsCanAddAddress);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXMileageandCarsenableautolog);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXMileageandCarspostcodeanywhere);

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesMileageandCarsTab();

            /// Make sure all checkboxes are un-checked
            cGeneralOptions.EXMileageandCarsUnTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesMileageandCarsTab();

            /// Validate each checkbox can be ticked seperately
            validateCheckBox(liCheckboes, true, ProductType.expenses, GeneralOptionsPage.newExpenses, GeneralOptionsTab.mileageAndCars);

            /// Make sure all checkboxes are ticked
            cGeneralOptions.EXMileageandCarsTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesMileageandCarsTab();

            /// Validate each checkbox can be un-ticked seperately
            validateCheckBox(liCheckboes, false, ProductType.expenses, GeneralOptionsPage.newExpenses, GeneralOptionsTab.mileageAndCars);  
      
            /// Validate Shortest / Quickest
            cGeneralOptions.EXMileageandCarsUpdateCalculationToQuickest();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesMileageandCarsTab();
            cGeneralOptions.ExMilageandCarsValidateCalculationQuickest();

            cGeneralOptions.EXMileageandCarsUpdateCalculationToShortest();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesMileageandCarsTab();
            cGeneralOptions.ExMilageandCarsValidateCalculationShortest();
        }


        /// <summary>
        /// Validate all options can be changed within Expenses general options - Other Preferences
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateOtherPreferencesWithInExpensesGeneralOptions()
        {

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXOtherPreferencescannotoverrideexchangerates);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXOtherPreferencespostcodesmandatorywhenadding);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXOtherPreferencesreceiptscanbeuploaded);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXOtherPreferencesselecthomeaddress);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXOtherPreferencessingleclaimattime);

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesOtherPreferencesTab();

            /// Make sure all checkboxes are un-checked
            cGeneralOptions.EXOtherPreferencesUnTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesOtherPreferencesTab();

            /// Validate each checkbox can be ticked seperately
            validateCheckBox(liCheckboes, true, ProductType.expenses, GeneralOptionsPage.newExpenses, GeneralOptionsTab.otherPreferences);

            /// Make sure all checkboxes are ticked
            cGeneralOptions.EXOtherPreferencesTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNewExpensesPage();
            cGeneralOptions.ClickNewExpensesOtherPreferencesTab();

            /// Validate each checkbox can be un-ticked seperately
            validateCheckBox(liCheckboes, false, ProductType.expenses, GeneralOptionsPage.newExpenses, GeneralOptionsTab.otherPreferences);        
        }


        /// <summary>
        /// Validate all options can be changed within Expenses general options - NHS Options
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateNHSOptionsWithInExpensesGeneralOptions()
        {

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXNHSOptionsESRnumbermandatory);

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNHSOptionsPage();         

            /// Make sure all checkboxes are un-checked
            cGeneralOptions.EXNHSOptionsUnTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNHSOptionsPage();

            /// Validate each checkbox can be ticked seperately
            validateCheckBox(liCheckboes, true, ProductType.expenses, GeneralOptionsPage.nhsOptions, GeneralOptionsTab.none);

            /// Make sure all checkboxes are ticked
            cGeneralOptions.EXNHSOptionsTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNHSOptionsPage();

            /// Validate each checkbox can be un-ticked seperately
            validateCheckBox(liCheckboes, false, ProductType.expenses, GeneralOptionsPage.nhsOptions, GeneralOptionsTab.none);
        
            /// Set ESR activation to none
            cGeneralOptions.EXNHSOptionsUpdateDropDownsToNone();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNHSOptionsPage();
            cGeneralOptions.EXNHSOptionsValidateDropDownSetToNone();

            /// Set ESR activation to automatic
            cGeneralOptions.EXNHSOptionsUpdateDropDownsToAutomatic();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNHSOptionsPage();
            cGeneralOptions.EXNHSOptionsValidateDropDownSetToAutomatic();

            /// Validate ESR Grace period
            cGeneralOptions.EXNHSOptionsUpdateGracePeriodFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNHSOptionsPage();
            cGeneralOptions.EXNHSOptionsValidateGracePeriodFirstValue();

            cGeneralOptions.EXNHSOptionsUpdateGracePeriodSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickNHSOptionsPage();
            cGeneralOptions.EXNHSOptionsValidateGracePeriodSecondValue();
        }


        /// <summary>
        /// Validate all options can be changed within Expenses general options - Email Server
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateEmailServersWithInExpensesGeneralOptions()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickEmailServerPage();

            /// Validate text boxes can be updated
            cGeneralOptions.EXEmailServerUpdateAllTextboxesToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickEmailServerPage();
            cGeneralOptions.ExpensesEmailServerValidateTextBoxesContainFirstValue();

            cGeneralOptions.EXEmailServerUpdateAllTextboxesToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickEmailServerPage();
            cGeneralOptions.ExpensesEmailServerValidateTextBoxesContainSecondValue();

            /// Validate radio buttons can be changed
            cGeneralOptions.EXEmailServerUpdateRadioButtonsToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickEmailServerPage();
            cGeneralOptions.ExpensesEmailServerValidateRadioButtonsContainFirstValue();

            cGeneralOptions.EXEmailServerUpdateRadioButtonsToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickEmailServerPage();
            cGeneralOptions.ExpensesEmailServerValidateRadioButtonsContainSecondValue();
        }


        /// <summary>
        /// Validate all options can be changed within Expenses general options - Main Administrator
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateMainAdministratorWithInExpensesGeneralOptions()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickMainAdministratorPage();

            /// Validate text boxes can be updated
            cGeneralOptions.FWMainAdministratorChangeAllDropDownsToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickMainAdministratorPage();
            cGeneralOptions.FrameworkMainAdministratorValidateDropDownListOnFirstValue();

            //cGeneralOptions.FWMainAdministratorChangeAllDropDownsToSecondValueParams.UIMainAdministratorComboBox1SelectedItem = "[None]";
            cGeneralOptions.FWMainAdministratorChangeAllDropDownsToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickMainAdministratorPage();
            cGeneralOptions.FrameworkMainAdministratorValidateDropDownListOnSecondValue();
        }


        /// <summary>
        /// Validate all options can be changed within Expenses general options - Regional Settings
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateRegionalSettingsWithInExpensesGeneralOptions()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickRegionalSettingsPage();

            /// Validate text boxes can be updated
            cGeneralOptions.FWRegionalSettingsChangeAllDropDownsToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickRegionalSettingsPage();
            cGeneralOptions.FrameworkRegionalSettingsValidateDropDownListOnFirstValue();

            cGeneralOptions.FWRegionalSettingsChangeAllDropDownsToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickRegionalSettingsPage();
            cGeneralOptions.FrameworkRegionalSettingsValidateDropDownListOnSecondValue();
        }

      
        /// <summary>
        /// Validate all options can be changed within Expenses general options - Field Settings
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateFieldSettingsWithInExpensesGeneralOptions()
        {
            #region Declare modals

            List<FieldSettingsModal> liModals = new List<FieldSettingsModal>();
            liModals.Add(FieldSettingsModal.company);
            liModals.Add(FieldSettingsModal.costcode);
            liModals.Add(FieldSettingsModal.country);
            liModals.Add(FieldSettingsModal.currency);
            liModals.Add(FieldSettingsModal.department);
            liModals.Add(FieldSettingsModal.from);
            liModals.Add(FieldSettingsModal.otherdetails);
            liModals.Add(FieldSettingsModal.projectcode);
            liModals.Add(FieldSettingsModal.reason);
            liModals.Add(FieldSettingsModal.to);

            #endregion

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXFieldSettingsDisplayForCash);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXFieldSettingsDisplayOnCreditCardItems);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXFieldSettingsDisplayOnIndividualItem);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXFieldSettingsDisplayOnPurchaseCardItems);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXFieldSettingsMandatoryOnCashItems);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXFieldSettingsMandatoryOnCreditCardItems);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.EXFieldSettingsMandatoryOnPurchaseCardItems);

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            foreach (FieldSettingsModal modal in liModals)
            {
                /// Go to General Options
                cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

                /// Press the desired modal and clear all the tick boxes
                selectCorrectGeneralOptionsPage(GeneralOptionsPage.newExpenses, GeneralOptionsTab.none, modal);
                cGeneralOptions.EXFieldSettingsUpdateTxtBoxToFirstValue();
                //Thread.Sleep(1000);
                cGeneralOptions.ClickOffDisplayText();
                cGeneralOptions.EXFieldSettingsUnTickAll();
                cGeneralOptions.PressFieldSettingsModalSaveButton();
                cGeneralOptions.PressGeneralOptionsSaveButton();

                /// Go to General Options
                cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

                /// Press the desired modal and select each checkbox
                selectCorrectGeneralOptionsPage(GeneralOptionsPage.newExpenses, GeneralOptionsTab.none, modal);
                cGeneralOptions.ExFieldSettingsValidateTxtBoxFirstValue();
                validateCheckBox(liCheckboes, true, ProductType.expenses, GeneralOptionsPage.newExpenses, GeneralOptionsTab.none, modal);
                cGeneralOptions.PressFieldSettingsModalSaveButton();
                cGeneralOptions.PressGeneralOptionsSaveButton();

                /// Go to General Options
                cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

                /// Press the desired modal and select all the tick boxes
                selectCorrectGeneralOptionsPage(GeneralOptionsPage.newExpenses, GeneralOptionsTab.none, modal);
                cGeneralOptions.EXFieldSettingsUpdateTxtBoxToSecondValue();
                cGeneralOptions.EXFieldSettingsTickAll();
                cGeneralOptions.PressFieldSettingsModalSaveButton();
                cGeneralOptions.PressGeneralOptionsSaveButton();

                /// Go to General Options
                cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx");

                /// Press the desired modal and select each checkbox
                selectCorrectGeneralOptionsPage(GeneralOptionsPage.newExpenses, GeneralOptionsTab.none, modal);
                cGeneralOptions.ExFieldSettingsValidateTxtBoxSecondValue();
                validateCheckBox(liCheckboes, false, ProductType.expenses, GeneralOptionsPage.newExpenses, GeneralOptionsTab.none, modal);
                cGeneralOptions.PressFieldSettingsModalSaveButton();
                cGeneralOptions.PressGeneralOptionsSaveButton();
            }
        }


        /// <summary>
        /// Use this method to validate each checkbox in the list of checkboxes can be ticked/unticked
        /// </summary>
        /// <param name="liCheckBoxesToValidate"></param>
        /// <param name="bSelect"></param>
        public void validateCheckBox(List<HtmlCheckBox> liCheckBoxesToValidate, bool bSelect, ProductType product, GeneralOptionsPage page, GeneralOptionsTab tab, FieldSettingsModal modal = FieldSettingsModal.none)
        {
            foreach(HtmlCheckBox currentCheckBox in liCheckBoxesToValidate)
            {
                /// Perform the select/unselect on the checkbox and validate
                currentCheckBox.Checked = bSelect;
                if (modal != FieldSettingsModal.none) { cGeneralOptions.PressFieldSettingsModalSaveButton(); }
                cGeneralOptions.PressGeneralOptionsSaveButton();
                cSharedMethods.NavigateToPage(product, "/shared/admin/accountOptions.aspx");
                selectCorrectGeneralOptionsPage(page, tab, modal);
                Assert.AreEqual(currentCheckBox.Checked, bSelect);
                
                /// Ensure no other checkboxes have been affected                         
                foreach (HtmlCheckBox otherCheckBox in liCheckBoxesToValidate)
                {
                    if (otherCheckBox != currentCheckBox)
                    {
                        Assert.AreEqual(otherCheckBox.Checked, !bSelect);
                    }
                }

                /// Reset the checbox
                currentCheckBox.Checked = !bSelect;
                if (modal != FieldSettingsModal.none) { cGeneralOptions.PressFieldSettingsModalSaveButton(); }
                cGeneralOptions.PressGeneralOptionsSaveButton();
                cSharedMethods.NavigateToPage(product, "/shared/admin/accountOptions.aspx");
                selectCorrectGeneralOptionsPage(page, tab, modal);
                Assert.AreEqual(currentCheckBox.Checked, !bSelect);
            }
        }

        /// <summary>
        /// Select the correct page within General Options
        /// </summary>
        /// <param name="page"></param>
        /// <param name="tab"></param>
        public void selectCorrectGeneralOptionsPage(GeneralOptionsPage page, GeneralOptionsTab tab, FieldSettingsModal modal)
        {
            #region Select the correct page
            switch (page)
            {
                case GeneralOptionsPage.emailServer:
                    cGeneralOptions.ClickEmailServerPage();
                    break;
                case GeneralOptionsPage.generalOptions:
                    cGeneralOptions.ClickGeneralOptionsPage();
                    break;
                case GeneralOptionsPage.mainAdministrator:
                    cGeneralOptions.ClickMainAdministratorPage();
                    break;
                case GeneralOptionsPage.passwordSettings:
                    cGeneralOptions.ClickPasswordSettingsPage();
                    break;
                case GeneralOptionsPage.regionalSettings:
                    cGeneralOptions.ClickRegionalSettingsPage();
                    break;
                case GeneralOptionsPage.newExpenses:
                    cGeneralOptions.ClickNewExpensesPage();
                    break;
                case GeneralOptionsPage.nhsOptions:
                    cGeneralOptions.ClickNHSOptionsPage();
                    break;
                default:
                    break;
            }
            #endregion


            #region Select the correct tab
            switch (tab)
            {
                case GeneralOptionsTab.contracts:
                    cGeneralOptions.ClickGeneralOptionsContractsTab();
                    break;
                case GeneralOptionsTab.generalDetails:
                    cGeneralOptions.ClickGeneralOptionsGeneralDetailsTab();
                    break;
                case GeneralOptionsTab.invoices:
                    cGeneralOptions.ClickGeneralOptionsInvoicesTab();
                    break;
                case GeneralOptionsTab.suppliers:
                    cGeneralOptions.ClickGeneralOptionsSuppliersTab();
                    break;
                case GeneralOptionsTab.codeAllocation:
                    cGeneralOptions.ClickNewExpensesCodeAllocationTab();
                    break;
                case GeneralOptionsTab.declaration:
                    cGeneralOptions.ClickGeneralOptionsDeclarationTab();
                    break;
                case GeneralOptionsTab.delegates:
                    cGeneralOptions.ClickGeneralOptionsDelegatesTab();
                    break;
                case GeneralOptionsTab.dutyOfCare:
                    cGeneralOptions.ClickNewExpensesDutyofCareTab();
                    break;
                case GeneralOptionsTab.mileageAndCars:
                    cGeneralOptions.ClickNewExpensesMileageandCarsTab();
                    break;
                case GeneralOptionsTab.otherPreferences:
                    cGeneralOptions.ClickNewExpensesOtherPreferencesTab();
                    break;
                case GeneralOptionsTab.selfReg:
                    cGeneralOptions.ClickGeneralOptionsSelfRegTab();
                    break;
                default:
                    break;
            }
            #endregion


            #region Select the correct modal
            switch (modal)
            {
                case FieldSettingsModal.company:
                    //cGeneralOptions.ClickCompanyModal();
                    cGeneralOptions.ClickEditCompany();
                    break;
                case FieldSettingsModal.costcode:
                    //cGeneralOptions.ClickCostcodeModal();
                    cGeneralOptions.ClickEditCostCodeModal();
                    break;
                case FieldSettingsModal.country:
                    cGeneralOptions.ClickCountryModl();
                    break;
                case FieldSettingsModal.currency:
                    cGeneralOptions.ClickCurrencyModal();
                    break;
                case FieldSettingsModal.department:
                    cGeneralOptions.ClickDepartmentModal();
                    break;
                case FieldSettingsModal.from:
                    cGeneralOptions.ClickFromModal();
                    break;
                case FieldSettingsModal.otherdetails:
                    cGeneralOptions.ClickOtherDetailsModal();
                    break;
                case FieldSettingsModal.projectcode:
                    cGeneralOptions.ClickProjectCodeModal();
                    break;
                case FieldSettingsModal.reason:
                    cGeneralOptions.ClickReasonModal();
                    break;
                case FieldSettingsModal.to:
                    cGeneralOptions.ClickToModal();
                    break;
                default:
                    break;
            }
            #endregion
        }


        /// <summary>
        /// This method stores the original values from the account properties table. This will be run before each test within this class.
        /// </summary>
        [TestInitialize()]
        public void SaveOriginalAccountProperties()
        {
            cDatabaseConnection dbProduct = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            cDatabaseConnection dbDataSource = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            #region Save General Options

            System.Data.SqlClient.SqlDataReader reader = dbProduct.GetReader("SELECT subAccountID, stringKey, stringValue, isGlobal FROM accountProperties");
            dbDataSource.ExecuteSQL("DELETE FROM AutoAccountProperties");

            int iSubAccountID = new int();
            string sStringKey = string.Empty;
            string sStringValue = string.Empty;
            string iIsGlobal = string.Empty;

            while (reader.Read())
            {
                iSubAccountID = reader.GetInt32(0);
                sStringKey = reader.GetValue(1).ToString();
                sStringValue = reader.GetValue(2).ToString();
                iIsGlobal = reader.GetValue(3).ToString();
                dbDataSource.sqlexecute.Parameters.AddWithValue("@stringValue", sStringValue);
                dbDataSource.ExecuteSQL("INSERT INTO AutoAccountProperties (subAccountID, stringKey, stringValue, isGlobal) VALUES (" + iSubAccountID + ", '" + sStringKey + "',  @stringValue, '" + iIsGlobal + "')");
                dbDataSource.sqlexecute.Parameters.Clear();
            }

            reader.Close();

            #endregion


            #region Save Add Screen

            reader = dbProduct.GetReader("SELECT fieldid, display, mandatory, code, description, individual, displaycc, mandatorycc, displaypc, mandatorypc FROM addscreen");
            dbDataSource.ExecuteSQL("DELETE FROM AutoAddScreen");

            Guid gfieldID = new Guid();
            bool bDisplay = new bool();
            bool bMandatory = new bool();
            string sCode = string.Empty;
            string sDescription = string.Empty;
            bool bIndividual = new bool();
            bool bDisplayCC = new bool();
            bool bMandatoryCC = new bool();
            bool bDisplayPC = new bool();
            bool bMandatoryPC = new bool();

            while (reader.Read())
            {
                gfieldID = reader.GetGuid(0);
                bDisplay = reader.GetBoolean(1);
                bMandatory = reader.GetBoolean(2);
                sCode = reader.GetString(3);
                sDescription = reader.GetString(4);
                bIndividual = reader.GetBoolean(5);
                bDisplayCC = reader.GetBoolean(6);
                bMandatoryCC = reader.GetBoolean(7);
                bDisplayPC = reader.GetBoolean(8);
                bMandatoryPC = reader.GetBoolean(9);

                dbDataSource.ExecuteSQL("INSERT INTO AutoAddScreen (fieldid, display, mandatory, code, description, individual, displaycc, mandatorycc, displaypc, mandatorypc) " +
                    "VALUES ('" + gfieldID + "', '" + bDisplay + "', '" + bMandatory + "', '" + sCode + "', '" + sDescription + "', '" + bIndividual + "', '" + bDisplayCC + "', '" +
                    bMandatoryCC + "', '" + bDisplayCC + "', '" + bMandatoryPC + "')");

            }

            reader.Close();

            #endregion

        }


        /// <summary>
        /// This method resets the original values to the account properties table. This will be run after each test within this class.
        /// </summary>
        [TestCleanup()]
        public void ResetOriginalAccountProperties()
        {
            cDatabaseConnection dbProduct = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["expensesDatabase"].ToString());
            cDatabaseConnection dbDataSource = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            #region Reset General Options

            System.Data.SqlClient.SqlDataReader reader = dbDataSource.GetReader("SELECT subAccountID, stringKey, stringValue, isGlobal FROM AutoAccountProperties");
            dbProduct.ExecuteSQL("DELETE FROM accountProperties");

            int iSubAccountID = new int();
            string sStringKey = string.Empty;
            string sStringValue = string.Empty;
            string iIsGlobal = string.Empty;

            while (reader.Read())
            {
                iSubAccountID = reader.GetInt32(0);
                sStringKey = reader.GetValue(1).ToString();
                sStringValue = reader.GetValue(2).ToString();
                iIsGlobal = reader.GetValue(3).ToString();
                dbProduct.sqlexecute.Parameters.AddWithValue("@stringValue", sStringValue);
                dbProduct.ExecuteSQL("INSERT INTO accountProperties (subAccountID, stringKey, stringValue, isGlobal) VALUES (" + iSubAccountID + ", '" + sStringKey + "',  @stringValue, '" + iIsGlobal + "')");
                dbProduct.sqlexecute.Parameters.Clear();
            }
            reader.Close();

            #endregion


            #region Reset Add Screen

            reader = dbDataSource.GetReader("SELECT fieldid, display, mandatory, code, description, individual, displaycc, mandatorycc, displaypc, mandatorypc FROM AutoAddScreen");
            dbProduct.ExecuteSQL("DELETE FROM addscreen");

            Guid gfieldID = new Guid();
            bool bDisplay = new bool();
            bool bMandatory = new bool();
            string sCode = string.Empty;
            string sDescription = string.Empty;
            bool bIndividual = new bool();
            bool bDisplayCC = new bool();
            bool bMandatoryCC = new bool();
            bool bDisplayPC = new bool();
            bool bMandatoryPC = new bool();

            while (reader.Read())
            {
                gfieldID = reader.GetGuid(0);
                bDisplay = reader.GetBoolean(1);
                bMandatory = reader.GetBoolean(2);
                sCode = reader.GetString(3);
                sDescription = reader.GetString(4);
                bIndividual = reader.GetBoolean(5);
                bDisplayCC = reader.GetBoolean(6);
                bMandatoryCC = reader.GetBoolean(7);
                bDisplayPC = reader.GetBoolean(8);
                bMandatoryPC = reader.GetBoolean(9);

                dbProduct.ExecuteSQL("INSERT INTO addscreen (fieldid, display, mandatory, code, description, individual, displaycc, mandatorycc, displaypc, mandatorypc) " +
                    "VALUES ('" + gfieldID + "', '" + bDisplay + "', '" + bMandatory + "', '" + sCode + "', '" + sDescription + "', '" + bIndividual + "', '" + bDisplayCC + "', '" +
                    bMandatoryCC + "', '" + bDisplayCC + "', '" + bMandatoryPC + "')");
            }

            reader.Close();

            #endregion

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





    /// <summary>
    /// Use this to pass a specific General Options Page
    /// </summary>
    public enum GeneralOptionsPage
    {
        none,
        generalOptions,
        newExpenses,
        emailServer,
        mainAdministrator,
        regionalSettings,
        passwordSettings,
        nhsOptions
    }


    /// <summary>
    /// Use this to pass a specific General Options Tab
    /// </summary>
    public enum GeneralOptionsTab
    {
        none,
        generalDetails,
        contracts,
        invoices,
        suppliers,
        selfReg,
        delegates,
        declaration,
        codeAllocation,
        dutyOfCare,
        mileageAndCars,
        otherPreferences
    }


    /// <summary>
    /// Use this to pass a specific General Options Tab
    /// </summary>
    public enum FieldSettingsModal
    {
        none,
        company,
        costcode,
        country,
        currency,
        department,
        from,
        otherdetails,
        projectcode,
        reason,
        to
    }









}
