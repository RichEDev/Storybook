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


namespace Auto_Tests.Coded_UI_Tests.Framework.Tailoring.General_Options
{
    /// <summary>
    /// This class contains all of the Framework specific tests for Password Complexity
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
        /// Summary for this test
        /// </summary>
        [TestMethod]
        public void JamesCopyTest()
        {
            cDatabaseConnection dbProduct = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["frameworkDatabase"].ToString());
            cDatabaseConnection dbDataSource = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

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
        }


        /// <summary>
        /// Validate all options can be changed within Framework general options - General Details
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateGeneralDetailsWithInFrameworkGeneralOptions()
        {

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.AutoUpdateProductLicenceTotalsCheckBox);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.EmployeesMayEditOwnDetailsCheckBox);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.FlashingNotesIconCheckBox);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.HyperlinkAttachmentsEnabledCheckBox);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.ShowProductInHomePageSearchCheckBox);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.TaskDueDateMandatoryCheckBox);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.TaskEndDateMandatoryCheckBox);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.TaskStartDateMandatoryCheckBox);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.UploadAttachmentsEnabledCheckBox);

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");

            /// Make sure all checkboxes are un-checked
            cGeneralOptions.FWGeneralDetailsUnTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");

            /// Validate each checkbox can be ticked seperately
            validateCheckBox(liCheckboes, true, GeneralOptionsPage.none, GeneralOptionsTab.generalDetails);

            /// Make sure all checkbox are ticked
            cGeneralOptions.FWGeneralDetailsTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");

            /// Validate each checkbox can be un-ticked seperately
            validateCheckBox(liCheckboes, false, GeneralOptionsPage.none, GeneralOptionsTab.generalDetails);

            /// Validate each drop down list can be changed
            cGeneralOptions.FWGeneralDetailsChangeAllDropDownsToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.FrameworkGeneralDetailsValidateDropDownListOnFirstValue();

            cGeneralOptions.FWGeneralDetailsChangeAllDropDownsToLastValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.FrameworkGeneralDetailsValidateDropDownListOnLastValue();

            /// Validate text boxes can be updated
            cGeneralOptions.FWGeneralDetailsUpdateAllTextboxesToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.FrameworkGeneralDetailsValidateTextBoxesContainFirstValue();

            cGeneralOptions.FWGeneralDetailsUpdateAllTextboxesToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.FrameworkGeneralDetailsValidateTextBoxesContainSecondValue();
        }


        /// <summary>
        /// Validate all options can be changed within Framework general options - Contracts
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateContractsWithInFrameworkGeneralOptions()
        {

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.FWContractsContractAllowNotesInArchivedCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.FWContractsContractAutoGenConNumCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.FWContractsContractAutoGenUpdatableCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.FWContractsContractAutoSequenceVaritionsCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.FWContractsContractAutoUpdateAnnualConValCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.FWContractsContractContractCatMandatoryCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.FWContractsContractContractDatesCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.FWContractsContractInflatorActiveCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.FWContractsContractInvoiceFrequencyActiveCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.FWContractsContractTermTypeFieldCB);

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsContractsTab();

            /// Make sure all checkboxes are un-checked
            cGeneralOptions.FWContractsUnTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsContractsTab();

            /// Validate each checkbox can be ticked seperately
            validateCheckBox(liCheckboes, true, GeneralOptionsPage.none, GeneralOptionsTab.contracts);
           
            /// Make sure all checkboxes are ticked
            cGeneralOptions.FWContractsTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsContractsTab();

            /// Validate each checkbox can be un-ticked seperately
            validateCheckBox(liCheckboes, false, GeneralOptionsPage.none, GeneralOptionsTab.contracts);

            /// Validate text boxes can be updated
            cGeneralOptions.FWContractsUpdateAllTextboxesToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsContractsTab();
            cGeneralOptions.FrameworkContractsValidateTextBoxesContainFirstValue();

            cGeneralOptions.FWContractsUpdateAllTextboxesToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsContractsTab();
            cGeneralOptions.FrameworkContractsValidateTextBoxesContainSecondValue();
        }


        /// <summary>
        /// Validate all options can be changed within Framework general options - Invoices
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateInvoicesWithInFrameworkGeneralOptions()
        {

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.FWInvoicesAutoPONumbersCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument.FWInvoicesKeepInvoiceForecastCB);

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsInvoicesTab();

            /// Make sure all checkboxes are un-checked
            cGeneralOptions.FWInvoicesUnTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsInvoicesTab();

            /// Validate each checkbox can be ticked seperately
            validateCheckBox(liCheckboes, true, GeneralOptionsPage.none, GeneralOptionsTab.invoices);

            /// Make sure all checkboxs are ticked
            cGeneralOptions.FWInvoicesTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsInvoicesTab();

            /// Validate each checkbox can be un-ticked seperately
            validateCheckBox(liCheckboes, false, GeneralOptionsPage.none, GeneralOptionsTab.invoices);

            /// Validate text boxes can be updated
            cGeneralOptions.FWInvoicesUpdateAllTextboxesToFirstValueParams.UICurrentPOSequenceNumEditText = "6";
            cGeneralOptions.FWInvoicesUpdateAllTextboxesToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsInvoicesTab();
            cGeneralOptions.FrameworkInvoicesValidateTextBoxesContainFirstValueExpectedValues.UICurrentPOSequenceNumEditText = "6";
            cGeneralOptions.FrameworkInvoicesValidateTextBoxesContainFirstValue();

            cGeneralOptions.FWInvoicesUpdateAllTextboxesToSecondValueParams.UICurrentPOSequenceNumEditText = "3";
            cGeneralOptions.FWInvoicesUpdateAllTextboxesToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsInvoicesTab();
            cGeneralOptions.FrameworkInvoicesValidateTextBoxesContainSecondValueExpectedValues.UICurrentPOSequenceNumEditText = "3";
            cGeneralOptions.FrameworkInvoicesValidateTextBoxesContainSecondValue();
        }


        /// <summary>
        /// Validate all options can be changed within Framework general options - Suppliers
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateSuppliersWithInFrameworkGeneralOptions()
        {

            #region Declare checkboxes

            List<HtmlCheckBox> liCheckboes = new List<HtmlCheckBox>();
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument1.FWSuppliersCategoryMandatoryCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument1.FWSuppliersContractFieldCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument1.FWSuppliersFinancialCheckCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument1.FWSuppliersFinancialStatusCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument1.FWSuppliersFinancialYearEndCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument1.FWSuppliersNumberEmployeesFieldCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument1.FWSuppliersStatusMandatoryCB);
            liCheckboes.Add(cGeneralOptions.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument1.FWSuppliersTurnoverCB);

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsSuppliersTab();

            /// Make sure all checkboxes are un-checked
            cGeneralOptions.FWSuppliersUnTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsSuppliersTab();

            /// Validate each checkbox can be ticked seperately
            validateCheckBox(liCheckboes, true, GeneralOptionsPage.none, GeneralOptionsTab.suppliers);

            /// Make sure all checkboxs are ticked
            cGeneralOptions.FWSuppliersTickAll();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsSuppliersTab();

            /// Validate each checkbox can be un-ticked seperately
            validateCheckBox(liCheckboes, false, GeneralOptionsPage.none, GeneralOptionsTab.suppliers);

            /// Validate text boxes can be updated
            cGeneralOptions.FWSuppliersUpdateAllTextboxesToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsSuppliersTab();
            cGeneralOptions.FrameworkSuppliersValidateTextBoxesContainFirstValue();

            cGeneralOptions.FWSuppliersUpdateAllTextboxesToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickGeneralOptionsSuppliersTab();
            cGeneralOptions.FrameworkSuppliersValidateTextBoxesContainSecondValue();
        }


        /// <summary>
        /// Validate all options can be changed within Framework general options - Email Server
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateEmailServersWithInFrameworkGeneralOptions()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickEmailServerPage();

            /// Validate text boxes can be updated
            cGeneralOptions.FWEmailServerUpdateAllTextboxesToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickEmailServerPage();
            cGeneralOptions.FrameworkEmailServerValidateTextBoxesContainFirstValue();

            cGeneralOptions.FWEmailServerUpdateAllTextboxesToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickEmailServerPage();
            cGeneralOptions.FrameworkEmailServerValidateTextBoxesContainSecondValue();
        }


        /// <summary>
        /// Validate all options can be changed within Framework general options - Main Administrator
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateMainAdministratorWithInFrameworkGeneralOptions()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickMainAdministratorPage();

            /// Validate text boxes can be updated
            cGeneralOptions.FWMainAdministratorChangeAllDropDownsToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickMainAdministratorPage();
            cGeneralOptions.FrameworkMainAdministratorValidateDropDownListOnFirstValue();

            cGeneralOptions.FWMainAdministratorChangeAllDropDownsToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickMainAdministratorPage();
            cGeneralOptions.FrameworkMainAdministratorValidateDropDownListOnSecondValue();
        }


        /// <summary>
        /// Validate all options can be changed within Framework general options - Regional Settings
        /// </summary>
        [TestMethod]
        public void GeneralOptionsSuccessfullyValidateRegionalSettingsWithInFrameworkGeneralOptions()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to General Options
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickRegionalSettingsPage();

            /// Validate text boxes can be updated
            cGeneralOptions.FWRegionalSettingsChangeAllDropDownsToFirstValueParams.UIDefaultCountryComboBoxSelectedItem = "Chile";
            cGeneralOptions.FWRegionalSettingsChangeAllDropDownsToFirstValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickRegionalSettingsPage();
            cGeneralOptions.FrameworkRegionalSettingsValidateDropDownListOnFirstValueExpectedValues.UIDefaultCountryComboBoxSelectedItem = "Chile";
            cGeneralOptions.FrameworkRegionalSettingsValidateDropDownListOnFirstValue();

            cGeneralOptions.FWRegionalSettingsChangeAllDropDownsToSecondValue();
            cGeneralOptions.PressGeneralOptionsSaveButton();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cGeneralOptions.ClickRegionalSettingsPage();
            cGeneralOptions.FrameworkRegionalSettingsValidateDropDownListOnSecondValue();
        }



        /// <summary>
        /// Use this method to validate each checkbox in the list of checkboxes can be ticked/unticked
        /// </summary>
        /// <param name="liCheckBoxesToValidate"></param>
        /// <param name="bSelect"></param>
        public void validateCheckBox(List<HtmlCheckBox> liCheckBoxesToValidate, bool bSelect, GeneralOptionsPage page, GeneralOptionsTab tab)
        {
            foreach(HtmlCheckBox currentCheckBox in liCheckBoxesToValidate)
            {
                /// Perform the select/unselect on the checkbox and validate
                currentCheckBox.Checked = bSelect;
                cGeneralOptions.PressGeneralOptionsSaveButton();
                cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
                selectCorrectGeneralOptionsPage(page, tab);
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
                cGeneralOptions.PressGeneralOptionsSaveButton();
                cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
                selectCorrectGeneralOptionsPage(page, tab);
                Assert.AreEqual(currentCheckBox.Checked, !bSelect);
            }
        }


        /// <summary>
        /// summary
        /// </summary>
        /// <param name="page"></param>
        /// <param name="tab"></param>
        public void selectCorrectGeneralOptionsPage(GeneralOptionsPage page, GeneralOptionsTab tab)
        {
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
            }


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
                default:
                    break;
            }
        }




        /// <summary>
        /// This method stores the original values from the account properties table. This will be run before each test within this class.
        /// </summary>
        [TestInitialize()]
        public void SaveOriginalAccountProperties()
        {
            cDatabaseConnection dbProduct = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["frameworkDatabase"].ToString());
            cDatabaseConnection dbDataSource = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

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
        }


        /// <summary>
        /// This method resets the original values to the account properties table. This will be run after each test within this class.
        /// </summary>
        [TestCleanup()]
        public void ResetOriginalAccountProperties()
        {
            cDatabaseConnection dbProduct = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["frameworkDatabase"].ToString());
            cDatabaseConnection dbDataSource = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

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
        emailServer,
        mainAdministrator,
        regionalSettings,
        passwordSettings
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
    }


}
