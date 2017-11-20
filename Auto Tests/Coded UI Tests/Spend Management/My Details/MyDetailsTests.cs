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


namespace Auto_Tests.Coded_UI_Tests.Spend_Management.My_Details
{
    /// <summary>
    /// All test cases for My Details
    /// </summary>
    [CodedUITest]
    public class MyDetailsTests
    {
        public MyDetailsTests()
        {
        }


        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.MyDetailsUIMapClasses.MyDetailsUIMap cMyDetails = new UIMaps.MyDetailsUIMapClasses.MyDetailsUIMap();


        /// <summary>
        /// Associated Manual Test: 28991 - Successfully Change Password via Change My Details
        /// This does not actually change the user's password as this is covered in the Password Complexity tests
        /// </summary>
        [TestMethod]
        public void MyDetailsSuccessfullyChangePassword()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/information/mydetails.aspx");
            cMyDetails.MyDetailsChangepassword();
            cMyDetails.ValidateChangePasswordPage();
            cMyDetails.CancelChangePassword();
            cMyDetails.CancelMyDetails();
        }



        /// <summary>
        /// Associated Manual Test: #### - Successfully Change My Details
        /// </summary>
        [TestMethod]
        public void MyDetailsSuccessfullyChangeDetails()
        {
            #region Updated by James - store initial values for My Details

            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.framework));
            db.sqlexecute.Parameters.AddWithValue("@username", cGlobalVariables.AdministratorUserName(ProductType.framework));
            string strSQL = "SELECT title, firstname, surname, extension, mobileno, pagerno, telno, email, homeemail FROM employees WHERE username=@username";
            System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL);

            reader.Read();

            string sTitle = reader.GetString(0);
            string sFirstname = reader.GetString(1);
            string sSurname = reader.GetString(2);
            string sExtention = reader.GetString(3);
            string sMobileNo = reader.GetString(4);
            string sPagerNo = reader.GetString(5);
            string sTelNo = reader.GetString(6);
            string sEmail = reader.GetString(7);
            string sPersonalEmail = reader.GetString(8);

            reader.Close();
            db.sqlexecute.Parameters.Clear();

            #endregion

            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            ///Ensure the 'Employees May Edit Their Own Personal Details' is enabled
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            

            cMyDetails.SetEmployeesMayEditTheirOwnPersonalDetails();

            cMyDetails.SaveGeneralOptions();

            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/information/mydetails.aspx");
            

            ///Set initial base My Details as it is not known what the current values are.
            cMyDetails.UpdateMyDetails();
            cMyDetails.SaveMyDetails();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/information/mydetails.aspx");

          
            ///Set My Details value different from base values.
            cMyDetails.UpdateMyDetailsParams.UITitleEditText = "Mr";
            cMyDetails.UpdateMyDetailsParams.UIFirstnameEditText = "TestForname2";
            cMyDetails.UpdateMyDetailsParams.UISurnameEditText = "TestSurname2";
            cMyDetails.UpdateMyDetailsParams.UIExtensionNoEditText = "987";
            cMyDetails.UpdateMyDetailsParams.UIMobileNoEditText = "876";
            cMyDetails.UpdateMyDetailsParams.UIPagerNoEditText = "765";
            cMyDetails.UpdateMyDetailsParams.UITelNoEditText = "654";
            cMyDetails.UpdateMyDetailsParams.UIEmailAddressEditText = "james.lloyd@software-europe.co.uk";
            cMyDetails.UpdateMyDetailsParams.UIPersonalEmailAddressEditText = "james.lloyd@software-europe.co.uk";


            ///Set expected assert values
            cMyDetails.ValidateMyDetailsExpectedValues.UITitleEditValueAttribute = cMyDetails.UpdateMyDetailsParams.UITitleEditText;
            cMyDetails.ValidateMyDetailsExpectedValues.UIFirstnameEditValueAttribute = cMyDetails.UpdateMyDetailsParams.UIFirstnameEditText;
            cMyDetails.ValidateMyDetailsExpectedValues.UISurnameEditValueAttribute = cMyDetails.UpdateMyDetailsParams.UISurnameEditText;
            cMyDetails.ValidateMyDetailsExpectedValues.UIExtensionNoEditValueAttribute = cMyDetails.UpdateMyDetailsParams.UIExtensionNoEditText;
            cMyDetails.ValidateMyDetailsExpectedValues.UIMobileNoEditValueAttribute = cMyDetails.UpdateMyDetailsParams.UIMobileNoEditText;
            cMyDetails.ValidateMyDetailsExpectedValues.UIPagerNoEditValueAttribute = cMyDetails.UpdateMyDetailsParams.UIPagerNoEditText;
            cMyDetails.ValidateMyDetailsExpectedValues.UITelNoEditValueAttribute = cMyDetails.UpdateMyDetailsParams.UITelNoEditText;
            cMyDetails.ValidateMyDetailsExpectedValues.UIEmailAddressEditValueAttribute = cMyDetails.UpdateMyDetailsParams.UIEmailAddressEditText;
            cMyDetails.ValidateMyDetailsExpectedValues.UIPersonalEmailAddressEditValueAttribute = cMyDetails.UpdateMyDetailsParams.UIPersonalEmailAddressEditText;
            cMyDetails.ValidateMyDetailsExpectedValues.UIUsernameEditValueAttribute = cGlobalVariables.AdministratorUserName(ProductType.framework);

            ///Update My Details using the above values.
            cMyDetails.UpdateMyDetails();
            cMyDetails.SaveMyDetails();

            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/information/mydetails.aspx");
            

            ///retrieve employee's psositon from the database and set expected results for the assert.
            cMyDetails.ValidateMyDetailsExpectedValues.UIPositionEditValueAttribute = GetEmployeePosition();
            cMyDetails.ValidateMyDetailsExpectedValues.UIPositionEditValueAttribute = null;


            ///Assert My Details page.
            cMyDetails.ValidateMyDetails();

            #region Updated by James - set My Details back to default values for future tests

            cMyDetails.UpdateMyDetailsParams.UITitleEditText = sTitle;
            cMyDetails.UpdateMyDetailsParams.UIFirstnameEditText = sFirstname;
            cMyDetails.UpdateMyDetailsParams.UISurnameEditText = sSurname;
            cMyDetails.UpdateMyDetailsParams.UIExtensionNoEditText = sExtention;
            cMyDetails.UpdateMyDetailsParams.UIMobileNoEditText = sMobileNo;
            cMyDetails.UpdateMyDetailsParams.UIPagerNoEditText = sPagerNo;
            cMyDetails.UpdateMyDetailsParams.UITelNoEditText = sTelNo;
            cMyDetails.UpdateMyDetailsParams.UIEmailAddressEditText = sEmail;
            cMyDetails.UpdateMyDetailsParams.UIPersonalEmailAddressEditText = sPersonalEmail;

            cMyDetails.UpdateMyDetails();
            cMyDetails.SaveMyDetails();

            #endregion
        }



        /// <summary>
        /// Associated Manual Test: 28994 - Successfully Set Employee Edit Permissions for My Details Page
        /// </summary>
        [TestMethod]
        public void MyDetailsSuccessfullySetEmployeeEditPermissionsForMyDetailsPage()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            ///Ensure the 'Employees May Edit Their Own Personal Details' is enabled
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cMyDetails.SetEmployeesMayEditTheirOwnPersonalDetails();
            cMyDetails.SaveGeneralOptions();

            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/information/mydetails.aspx");

            ///Assert the enabled status of all fields in the My Details form.
            cMyDetails.ValidateMyDetailsForrmEnabledStatus();
            cMyDetails.CancelMyDetails();


            ///Set the 'Employees May Edit Their Own Personal Details' general option to disabled.
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cMyDetails.SetEmployeesMayEditTheirOwnPersonalDetailsParams.UIEmployeesmayedittheiCheckBoxChecked = false;
            cMyDetails.SetEmployeesMayEditTheirOwnPersonalDetails();
            cMyDetails.SaveGeneralOptions();

            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/information/mydetails.aspx");



            ///Set the expected enabled status for form fields.
            cMyDetails.ValidateMyDetailsForrmEnabledStatusExpectedValues.UITitleEditEnabled = false;
            cMyDetails.ValidateMyDetailsForrmEnabledStatusExpectedValues.UIFirstnameEditEnabled = false;
            cMyDetails.ValidateMyDetailsForrmEnabledStatusExpectedValues.UISurnameEditEnabled = false;
            cMyDetails.ValidateMyDetailsForrmEnabledStatusExpectedValues.UIExtensionNoEditEnabled = false;
            cMyDetails.ValidateMyDetailsForrmEnabledStatusExpectedValues.UIMobileNoEditEnabled = false;
            cMyDetails.ValidateMyDetailsForrmEnabledStatusExpectedValues.UIPagerNoEditEnabled = false;
            cMyDetails.ValidateMyDetailsForrmEnabledStatusExpectedValues.UIEmailAddressEditEnabled = false;
            cMyDetails.ValidateMyDetailsForrmEnabledStatusExpectedValues.UITelNoEditEnabled = false;
            cMyDetails.ValidateMyDetailsForrmEnabledStatusExpectedValues.UIPersonalEmailAddressEditEnabled = false;


            ///Assert the enabled status of all fields in the My Details form.
            cMyDetails.ValidateMyDetailsForrmEnabledStatus();
            cMyDetails.CancelMyDetails();
        }
        


        /// <summary>
        /// Associated Manual Test: 28993 - Successfully Cancel Changes to My Details
        /// </summary>
        [TestMethod]
        public void MyDetailsSuccessfullyCancelChangesToMyDetails()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            ///Ensure the 'Employees May Edit Their Own Personal Details' is enabled
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");


          //  cSharedMethods.RefreshBrowswerWindow();

            
            cMyDetails.SetEmployeesMayEditTheirOwnPersonalDetails();
            cMyDetails.SaveGeneralOptions();

            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/information/mydetails.aspx");
            
            
            ///Set initial base My Details as it is not known what the current values are.
            cMyDetails.UpdateMyDetails();
            cMyDetails.SaveMyDetails();
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/information/mydetails.aspx");
            

            ///Set My Details value different from base values.
            cMyDetails.UpdateMyDetailsParams.UITitleEditText = "Mr";
            cMyDetails.UpdateMyDetailsParams.UIFirstnameEditText = "TestForname2";
            cMyDetails.UpdateMyDetailsParams.UISurnameEditText = "TestSurname2";
            cMyDetails.UpdateMyDetailsParams.UIExtensionNoEditText = "987";
            cMyDetails.UpdateMyDetailsParams.UIMobileNoEditText = "876";
            cMyDetails.UpdateMyDetailsParams.UIPagerNoEditText = "765";
            cMyDetails.UpdateMyDetailsParams.UITelNoEditText = "654";
            cMyDetails.UpdateMyDetailsParams.UIEmailAddressEditText = "james.lloyd@software-europe.co.uk";
            cMyDetails.UpdateMyDetailsParams.UIPersonalEmailAddressEditText = "james.lloyd@software-europe.co.uk";

            ///retrieve employee's psositon from the database and set expected results for the assert.
            cMyDetails.ValidateMyDetailsExpectedValues.UIPositionEditValueAttribute = GetEmployeePosition();

            ///Set exepected user name.
            cMyDetails.ValidateMyDetailsExpectedValues.UIUsernameEditValueAttribute = cGlobalVariables.AdministratorUserName(ProductType.framework);

            cMyDetails.UpdateMyDetails();

            ///Cancel changes made within My Details.
            cMyDetails.CancelMyDetails();

            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/information/mydetails.aspx");
  

            ///Assert My Details page using original base values.
            cMyDetails.ValidateMyDetailsExpectedValues.UIUsernameEditValueAttribute = cGlobalVariables.AdministratorUserName(ProductType.framework);

            cMyDetails.ValidateMyDetailsExpectedValues.UIPositionEditValueAttribute = null;
            cMyDetails.ValidateMyDetails();

            cMyDetails.CancelMyDetails();

        }



        /// <summary>
        /// Associated Manual Test: 28996 - SuccessfullyVerifyFormFieldValidationForMyDetailsPage
        /// </summary>
        [TestMethod]
        public void MyDetailsSuccessfullyVerifyEmailAddressvalidation()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            ///Ensure the 'Employees May Edit Their Own Personal Details' is enabled
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/accountOptions.aspx");
            cMyDetails.SetEmployeesMayEditTheirOwnPersonalDetails();
            cMyDetails.SaveGeneralOptions();

            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/information/mydetails.aspx");

            ///Set invalid email address, missing @ symbol
            cMyDetails.SetEmailAddressinMyDetailsParams.UIEmailAddressEditText = "dylan.tillakeratnesoftware-europe.co.uk";
            cMyDetails.SetEmailAddressinMyDetails();
            cMyDetails.SaveMyDetails();

            ///Assert invalid email address error.
            cMyDetails.ValidateInvalidEmailAddressError();


            ///Set invalid email address, missing 'dot something'.
            cMyDetails.SetEmailAddressinMyDetailsParams.UIEmailAddressEditText = "dylan.tillakeratne@software-europe";
            cMyDetails.SetEmailAddressinMyDetails();
            cMyDetails.SaveMyDetails();

            ///Assert invalid email address error.
            cMyDetails.ValidateInvalidEmailAddressError();


            ///Set invalid personal email address, missing @ symbol
            cMyDetails.SetPersonalEmailAddressinMyDetailsParams.UIPersonalEmailAddressEditText = "dylan.tillakeratnesoftware-europe.co.uk";
            cMyDetails.SetPersonalEmailAddressinMyDetails();
            cMyDetails.SaveMyDetails();

            ///Assert invalid email address error.
            cMyDetails.ValidateInvalidPersonalEmailAddressError();


            ///Set invalid personal; email address, missing 'dot something'.
            cMyDetails.SetPersonalEmailAddressinMyDetailsParams.UIPersonalEmailAddressEditText = "dylan.tillakeratnesoftware-europe";
            cMyDetails.SetPersonalEmailAddressinMyDetails();
            cMyDetails.SaveMyDetails();

            ///Assert invalid personal email address error.
            cMyDetails.ValidateInvalidPersonalEmailAddressError();


            cMyDetails.CancelMyDetails();
        }



        /// <summary>
        /// Associated Manual Test: 28992 - SuccessfullyNotifyAdministratorOfIncorrectDetails
        /// </summary>
        [TestMethod]
        public void MyDetailsSuccessfullyNotifyAdministratorOfIncorrectDetails()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/information/mydetails.aspx");
            cMyDetails.NavigateMyDetailsToChangeOfDetails();

            ///verify that saving the Change Of Details notification returns the user to the my Details page.
            ///NOTE: This test method does not verify that the email has not been sent
            cMyDetails.EnterChangeOfDetailsText();
            cMyDetails.SaveChangeOfDetails();

            ///Asset that my details page is dispalyed.
            cMyDetails.ValidateMyDetailsIsCurrentPageExpectedValues.UIMyDetailsPaneInnerText = "Change My Details";
            cMyDetails.ValidateMyDetailsIsCurrentPageExpectedValues.UIMyDetailsPaneInnerText1 = "Change My Details";
            cMyDetails.ValidateMyDetailsIsCurrentPage();

            cMyDetails.CancelMyDetails();
        }



        /// <summary>
        /// Associated Manual Test: 29151 - SuccessfullyNotifyAdministratorOfIncorrectDetails
        /// </summary>
        [TestMethod]
        public void MyDetailsSuccessfullyCancelNotifyAdministratorOfIncorrectDetails()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/information/mydetails.aspx");
            cMyDetails.NavigateMyDetailsToChangeOfDetails();

            ///verify that cancelling the Change Of Details notification returns the user to the my Details page.
            ///NOTE: This test method does not verify that the email has not been sent
            cMyDetails.EnterChangeOfDetailsText();
            cMyDetails.CancelChangeOfDetails();

            ///Asset that my details page is dispalyed.
            cMyDetails.ValidateMyDetailsIsCurrentPageExpectedValues.UIMyDetailsPaneInnerText = "Change My Details";
            cMyDetails.ValidateMyDetailsIsCurrentPageExpectedValues.UIMyDetailsPaneInnerText1 = "Change My Details";
            cMyDetails.ValidateMyDetailsIsCurrentPage();

            cMyDetails.CancelMyDetails();
        }




        /// <summary>
        /// Associated Manual Test: 28989 - Successfully Verify My Details Page Layout
        /// NOTE: Not Done
        /// </summary>
        [TestMethod]
        public void MyDetailsSuccessfullyVerifyPageLayout()
        {
            // Logon

            //cMyDetails.NavigateHomeToMyDetails();


            //string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            //cMyDetails.ValidateMyDetailsPageLayoutExpectedValues.UIMrsTestForenameTestSPaneInnerText =
            //    cGlobalVariables.AdministratorUserName(ProductType.framework);

            //cMyDetails.ValidateMyDetailsPageLayout();

            //cMyDetails.CancelMyDetails();
        }



        /// <summary>
        /// Function to retreive the test users job role (position) and set the expected results for later assertion
        /// </summary>
        public string GetEmployeePosition()
        {
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.framework));
            db.sqlexecute.Parameters.AddWithValue("@username", cGlobalVariables.AdministratorUserName(ProductType.framework));
            string strSQL = "SELECT position FROM employees WHERE username=@username";
            string returnvalue = string.Empty;
            System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL);

            while (reader.Read())
            {
                if (reader.IsDBNull(0) == false)
                {
                    returnvalue = reader.GetString(0);
                }
                else
                {
                    returnvalue = "";
                }
            }

            reader.Close();
            db.sqlexecute.Parameters.Clear();

            return returnvalue;
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
