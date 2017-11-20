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
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using System.Web.UI.HtmlControls;
using Auto_Tests.UIMaps.IPAddressFilteringUIMapClasses;
using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;



namespace Auto_Tests.Coded_UI_Tests.Spend_Management.System_Options.IP_Address_Filtering
{
    /// <summary>
    /// Summary description for IPAddressFiltering
    /// </summary>
    [CodedUITest]
    public class IPAddressFilteringTests
    {
        private static SharedMethodsUIMap _sharedMethods = new SharedMethodsUIMap();
        private IPAddressFilteringUIMap _IPAddressFiltering = new IPAddressFilteringUIMap();
        private IPFiltersDAO _IPFilterData;
        private bool _runTestCleanup;
        private IPFiltersDTO _localIPAddressFilter = CreateLocalIPAddressFilter();
        private List<IPFiltersDTO> _databaseIPFilter;
        private static ProductType _executingProduct = cGlobalVariables.GetProductFromAppConfig();

        public IPAddressFilteringTests()
        {
        }

        /// <summary>
        /// Runs before all tests and sets up the testing environment
        /// Start IE, Logs in the product and Reads the necessary data from lithium
        /// </summary>
        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.CloseOnPlaybackCleanup = false;
            browser.Maximized = true;
            _sharedMethods.Logon(_executingProduct, LogonType.administrator);
        }

        /// <summary>
        /// Runs when all tests are over and closes IE
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanUp()
        {
            _sharedMethods.CloseBrowserWindow();
        }

        #region 35610 - Successfully add IP Address Filter
        /// <summary>
        /// 35610 - Successfully add IP Address Filter
        /// </summary>
        [TestCategory("IPFilters"), TestCategory("Spend Management"), TestMethod]
        public void IPFiltersSuccessfullyAddNewIPFilter_UITest()
        {
            _runTestCleanup = true;

            ///Navigate to IP Address Filtering page

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/adminIPfilters.aspx");

            ///Click New IP Address Filter

            _IPAddressFiltering.ClickNewIPFilter();

            ///Populate fields

            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIIPAddressEditText = _localIPAddressFilter.ipAddress;
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIDescriptionEditText = _localIPAddressFilter.description;
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIActiveCheckBoxChecked = _localIPAddressFilter.active;
            _IPAddressFiltering.PopulateIPFilterDetails();

            ///Press Save

            _IPAddressFiltering.PressSave();

            ///Validate IP Address is added in the grid  

            _IPAddressFiltering.ValidateIPFilter(_sharedMethods.browserWindow);
        }
        #endregion

        #region 35614 - Successfully edit IP Address Filter
        /// <summary>
        /// 35614 - Successfully edit IP Address Filter
        /// </summary>
        [TestCategory("IPFilters"), TestCategory("Spend Management"), TestMethod]
        public void IPFiltersSuccessfullyEditIPFilter_UITest()
        {
            _runTestCleanup = true;
            IPFiltersDTO localIPAddressFilter = CreateLocalIPAddressFilter();
            
            ImportDataToEx_CodeduiDB(localIPAddressFilter);

            ///Navigate to IP Address Filtering page

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/adminIPfilters.aspx");

            ///Click Edit IP Address Filter

            _IPAddressFiltering.ClickEditFieldLink(_sharedMethods.browserWindow, localIPAddressFilter.ipAddress);

            ///Populate fields with new information

            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIIPAddressEditText = "1.1.1.1";
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIDescriptionEditText = "EDITED Description";
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIActiveCheckBoxChecked = !localIPAddressFilter.active;
            _IPAddressFiltering.PopulateIPFilterDetails();

            ///Press Save

            _IPAddressFiltering.PressSave();

            ///Validate IP address is edited

            _IPAddressFiltering.ValidateIPFilter(_sharedMethods.browserWindow);

            ///Validate IP Filter Details modal contains correct details after the save

            _IPAddressFiltering.ClickEditFieldLink(_sharedMethods.browserWindow, "1.1.1.1");
            _IPAddressFiltering.ValidateIPAddressField();
            _IPAddressFiltering.ValidateIPFilterDescriptionField();
            _IPAddressFiltering.ValidateIPFilterActiveFieldExpectedValues.UIActiveCheckBox1Checked = !localIPAddressFilter.active;
            _IPAddressFiltering.ValidateIPFilterActiveField();

            ///Bring values to their initial state

            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIIPAddressEditText = localIPAddressFilter.ipAddress;
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIDescriptionEditText = localIPAddressFilter.description;
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIActiveCheckBoxChecked = false;
            _IPAddressFiltering.PopulateIPFilterDetails();
            _IPAddressFiltering.PressSave();
        }
        #endregion

        #region 35615 - Successfully delete IP Address Filter
        /// <summary>
        /// 35615 - Successfully delete IP Address Filter
        /// </summary>
        [TestCategory("IPFilters"), TestCategory("Spend Management"), TestMethod]
        public void IPFiltersSuccessfullyDeleteIPFilter_UITest()
        {
            _runTestCleanup = false;
            IPFiltersDTO localIPAddressFilter = CreateLocalIPAddressFilter();

            ImportDataToEx_CodeduiDB(localIPAddressFilter);

            ///Navigate to IP Address Filtering page

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/adminIPfilters.aspx");

            ///Click Delete IP Address Filter

            _IPAddressFiltering.ClickDeleteFieldLink(localIPAddressFilter.ipAddress);

            Keyboard.SendKeys("{Enter}");

            ///Validate IP Address is deleted from the grid

            _IPAddressFiltering.ValidateIPFiltersDeletion(localIPAddressFilter.ipAddress);
        }
        #endregion

        #region 35612 - Unsuccessfully add IP Address filter where mandatory fields are missing
        /// <summary>
        /// 35612 - Unsuccessfully add IP Address filter where mandatory fields are missing
        /// </summary>
        [TestCategory("IPFilters"), TestCategory("Spend Management"), TestMethod]
        public void IPFiltersUnsuccessfullyAddIPFilterMissingMandatoryFields_UITest()
        {
            _runTestCleanup = false;

            ///Navigate to IP Address Filtering page

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/adminIPfilters.aspx");

            ///Click New IP Address Filter

            _IPAddressFiltering.ClickNewIPFilter();

            ///Populate fields

            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIActiveCheckBoxChecked = false;
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIDescriptionEditText = "";
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIIPAddressEditText = "";
            _IPAddressFiltering.PopulateIPFilterDetails();

            ///Press Save

            _IPAddressFiltering.PressSave();

            ///Validate IP Address mandatory fields missing

            _IPAddressFiltering.ValidateMandatoryFieldsMissing();
            _IPAddressFiltering.PressClose();
            _IPAddressFiltering.PressCancel();
        }
        #endregion

        #region 35611 - Unsuccessfully add IP Addres Filter where invalid data are used
        /// <summary>
        /// 35611 - Unsuccessfully add IP Addres Filter where invalid data are used
        /// </summary>
        [TestCategory("IPFilters"), TestCategory("Spend Management"), TestMethod]
        public void IPFiltersUnsuccessfullyAddIPFilterUsingInvalidData_UITest()
        {
            _runTestCleanup = false;

            ///Navigate to IP Address Filtering page

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/adminIPfilters.aspx");

            ///Click New IP Address Filter

            _IPAddressFiltering.ClickNewIPFilter();

            ///Populate fields

            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIActiveCheckBoxChecked = false;
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIDescriptionEditText = "Testing Invalid Data";
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIIPAddressEditText = "abcdefg";
            _IPAddressFiltering.PopulateIPFilterDetails();

            ///Press Save

            _IPAddressFiltering.PressSave();

            ///Validate IP Address invalid data have been entered

            _IPAddressFiltering.ValidateAddIPAddressFilterWithInvalidData();

            _IPAddressFiltering.PressClose();
            _IPAddressFiltering.PressCancel();
        }
        #endregion

        #region 35617 - Unsuccessfully add duplicate IP Address Filter
        /// <summary>
        /// 35617 - Unsuccessfully add duplicate IP Address Filter
        /// </summary>
        [TestCategory("IPFilters"), TestCategory("Spend Management"), TestMethod]
        public void IPFiltersUnsuccessfullyAddDuplicateIPFilter_UITest()
        {
            _runTestCleanup = true;
            IPFiltersDTO localIPAddressFilter = CreateLocalIPAddressFilter();

            ImportDataToEx_CodeduiDB(localIPAddressFilter);

            ///Navigate to IP Address Filtering page

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/adminIPfilters.aspx");

            ///Click New IP Address Filter

            _IPAddressFiltering.ClickNewIPFilter();

            ///Populate fields

            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIIPAddressEditText = localIPAddressFilter.ipAddress;
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIDescriptionEditText = localIPAddressFilter.description;
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIActiveCheckBoxChecked = localIPAddressFilter.active;
            _IPAddressFiltering.PopulateIPFilterDetails();

            ///Press Save

            _IPAddressFiltering.PressSave();

            ///Validate IP Address entered is duplicate

            _IPAddressFiltering.ValidateDuplicateIPAddressFilter();

            _IPAddressFiltering.PressClose();
            _IPAddressFiltering.PressCancel();
        }
        #endregion

        #region 35613 - Successfully cancel adding IP Address Filter
        /// <summary> 
        /// 35613 - Successfully cancel adding IP Address Filter
        /// </summary>
        [TestCategory("IPFilters"), TestCategory("Spend Management"), TestMethod]
        public void IPFiltersSuccessfullyCancelAddingIPFilter_UITest()
        {
            _runTestCleanup = false;

            ///Navigate to IP Address Filtering page

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/adminIPfilters.aspx");

            ///Click New IP Address Filter

            _IPAddressFiltering.ClickNewIPFilter();

            ///Populate fields

            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIActiveCheckBoxChecked = false;
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIDescriptionEditText = "__Testing Cancel";
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIIPAddressEditText = "2.2.2.2";
            _IPAddressFiltering.PopulateIPFilterDetails();

            ///Press Cancel

            _IPAddressFiltering.PressCancel();

            ///Validate IP Address is not added in the grid

            _IPAddressFiltering.ValidateCancelAddingIPAddressFilter();
        }
        #endregion

        #region 43218 - Successfully cancel deleting IP Address Filter
        /// <summary> 
        /// 43218 - Successfully cancel deleting IP Address Filter
        /// </summary>
        [TestCategory("IPFilters"), TestCategory("Spend Management"), TestMethod]
        public void IPFiltersSuccessfullyCancelDeletingIPFilter_UITest()
        {
            _runTestCleanup = true;
            IPFiltersDTO localIPAddressFilter = CreateLocalIPAddressFilter();

            ImportDataToEx_CodeduiDB(localIPAddressFilter);

            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIIPAddressEditText = localIPAddressFilter.ipAddress;
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIDescriptionEditText = localIPAddressFilter.description;
            _IPAddressFiltering.PopulateIPFilterDetailsParams.UIActiveCheckBoxChecked = localIPAddressFilter.active;

            ///Navigate to IP Address Filtering page

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/adminIPfilters.aspx");

             ///Click Delete IP Address Filter and Cancel the deletion

            _IPAddressFiltering.ClickDeleteFieldLink(localIPAddressFilter.ipAddress);

            Keyboard.SendKeys("{Tab}");
            Keyboard.SendKeys("{Enter}");

            ///Validate IP Address is not deleted from the grid

            _IPAddressFiltering.ValidateIPFilter(_sharedMethods.browserWindow);
        }
        #endregion

        #region 43220 - Successfully verify IP Filtering page grid
        /// <summary> 
        /// 43220 - Successfully verify IP Filtering page grid
        /// </summary>
        [TestCategory("IPFilters"), TestCategory("Spend Management"), TestMethod]
        public void IPFiltersSuccessfullySortIPFilterGrid_UITest()
        {
            _runTestCleanup = true;
            _databaseIPFilter = CombineDataToImport();
            
            ImportDataToEx_CodeduiDB(_databaseIPFilter);

            RestoreDefaultSortingOrder();

            ///Navigate to IP Address Filtering page

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/adminIPfilters.aspx");

            //Ensure Table is sorted correctly

            //Sorts IP Address
            HtmlHyperlink displayNameLink = _IPAddressFiltering.UIIPAddressFilteringWiWindow.UIIPAddressFilteringDocument.UITbl_gridIPFiltersTable1.UIIPAddressHyperlink;
            _IPAddressFiltering.ClickTableHeader(displayNameLink);
            _IPAddressFiltering.DoesTableContainCorrectElements(SortIPFiltersByColumn.IPAddress, Auto_Tests.Tools.EnumHelper.TableSortOrder.DESC);  
            _IPAddressFiltering.ClickTableHeader(displayNameLink);
            _IPAddressFiltering.DoesTableContainCorrectElements(SortIPFiltersByColumn.IPAddress, Auto_Tests.Tools.EnumHelper.TableSortOrder.ASC);
                  

            //Sorts Description
            displayNameLink = _IPAddressFiltering.UIIPAddressFilteringWiWindow.UIIPAddressFilteringDocument.UITbl_gridIPFiltersTable1.UIDescriptionHyperlink;
            _IPAddressFiltering.ClickTableHeader(displayNameLink);
            _IPAddressFiltering.DoesTableContainCorrectElements(SortIPFiltersByColumn.Description, Auto_Tests.Tools.EnumHelper.TableSortOrder.ASC);
            _IPAddressFiltering.ClickTableHeader(displayNameLink);
            _IPAddressFiltering.DoesTableContainCorrectElements(SortIPFiltersByColumn.Description, Auto_Tests.Tools.EnumHelper.TableSortOrder.DESC);
                     
            //Sorts Active
            displayNameLink = _IPAddressFiltering.UIIPAddressFilteringWiWindow.UIIPAddressFilteringDocument.UITbl_gridIPFiltersTable2.UIActiveHyperlink;
            _IPAddressFiltering.ClickTableHeader(displayNameLink);
            _IPAddressFiltering.DoesTableContainCorrectElements(SortIPFiltersByColumn.Active, Auto_Tests.Tools.EnumHelper.TableSortOrder.ASC);
            _IPAddressFiltering.ClickTableHeader(displayNameLink);
            _IPAddressFiltering.DoesTableContainCorrectElements(SortIPFiltersByColumn.Active, Auto_Tests.Tools.EnumHelper.TableSortOrder.DESC);

            RestoreDefaultSortingOrder();
        }
        #endregion

        #region 35608 - Successfully verify IP Filtering Page
        /// <summary> 
        /// 35608 - Successfully verify IP Filtering Page
        /// </summary>
        [TestCategory("IPFilters"), TestCategory("Spend Management"), TestMethod]
        public void IPFiltersSuccessfullyVerifyIPFiltersPage_UITest()
        {
            _runTestCleanup = false;

            ///Navigate to IP Address Filtering page
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/adminIPfilters.aspx");

            DateTime dt = DateTime.Now;
            string day = dt.ToString("dd");
            string monthName = dt.ToString("MMMM");
            string year = dt.ToString("yyyy");

            string currentTimeStr = day + " " + monthName + " " + year;
            _sharedMethods.VerifyPageLayout("IP Address Filtering", "Home : Administrative Settings : System Options : IP Address Filtering", "Company PolicyHelp & SupportExit", "Mr James Lloyd | Developer | " + currentTimeStr, "Page Options New IP Filter");
        }
        #endregion

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        // [TestInitialize()]
        //   public void MyTestInitialize()
        //   {       
        // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463

        //  }

        /// <summary> 
        /// Runs after every test to remove ipFilters of the database
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            ///Set Build Server's IP Address to inactive to ensure rest of tests will be able to logon
            cDatabaseConnection dbEx = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));
            string strSQL = "UPDATE ipfilters SET Active = 'false' Where Active = 'true'";
            dbEx.ExecuteSQL(strSQL);
            dbEx.sqlexecute.Parameters.Clear();

            if (_runTestCleanup)
            {
                if (TestContext.TestName.Equals("IPFiltersSuccessfullySortIPFilterGrid"))
                {
                    cDatabaseConnection db1 = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));
                    _IPFilterData = new IPFiltersDAO(db1);
                    int result = _IPFilterData.DeleteIPFiltersFromDB(_databaseIPFilter);
                    //Verify that data have been deleted from the ex_codedui database
                    Assert.IsTrue(result > -1);

                }
                else
                {
                    //IPFiltersNewIPFilter
                    cDatabaseConnection db1 = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));
                    _IPFilterData = new IPFiltersDAO(db1);
                    int result = _IPFilterData.DeleteIPFiltersFromDB(_localIPAddressFilter);
                    //Verify that data have been deleted from the ex_codedui database
                    Assert.IsTrue(result > -1);
                }
            }

        }

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
        /// Reads data from Lithium and returns a list with ip filters
        /// </summary>
        private List<IPFiltersDTO> ReadDataFromLithium(int numberOfData = -1)
        {
            List<IPFiltersDTO> databaseIPFilter = new List<IPFiltersDTO>();

            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());
            
            string SQLString = "SELECT description, ipaddress, active FROM IPFilters";

            if (numberOfData != -1) 
            {
                SQLString = "SELECT TOP " + numberOfData + " description, ipaddress, active FROM IPFilters";
            }


            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(SQLString))
            {
                while (reader.Read())
                {
                    #region Set variables

                    IPFiltersDTO filter = new IPFiltersDTO();

                    if (!reader.IsDBNull(0))
                    {
                        filter.description = reader.GetString(0);
                    }
                    filter.ipAddress = reader.GetString(1);
                    filter.active = reader.GetBoolean(2);
                    databaseIPFilter.Add(filter);

                    #endregion
                }
                reader.Close();
              }
            return databaseIPFilter;  
        }

        /// <summary> 
        /// Using the local IP Address creates an IP filter
        /// </summary>
        private static IPFiltersDTO CreateLocalIPAddressFilter() 
        {
            IPFiltersDTO localIP = new IPFiltersDTO();

            localIP.description = "Local Host's IP Address";
            localIP.ipAddress = _sharedMethods.GetIPAddressOfLocalMachine();
            localIP.active = true;

            return localIP ;
        }

        /// <summary> 
        /// Returns a list with the ip filters from lithium and the local ip address filter
        /// </summary>
        private List<IPFiltersDTO> CombineDataToImport()
        {
            List<IPFiltersDTO> cDatabaseIPFilter = ReadDataFromLithium(4);

            IPFiltersDTO localIPAddress = CreateLocalIPAddressFilter();

            if (cDatabaseIPFilter.Contains(localIPAddress) == false)
            {
                cDatabaseIPFilter.Add(localIPAddress);
            }

            return cDatabaseIPFilter;        
        }

        /// <summary> 
        /// Imports a single ip filter to the codedui database
        /// </summary>
        private void ImportDataToEx_CodeduiDB(IPFiltersDTO IPFilterData)
        {
            List<IPFiltersDTO> IPFilters = new List<IPFiltersDTO>();
            IPFilters.Add(IPFilterData);
            ImportDataToEx_CodeduiDB(IPFilters);
        }

        /// <summary> 
        /// Imports a list of IP filters to the codedui database
        /// </summary>
        private void ImportDataToEx_CodeduiDB(List<IPFiltersDTO> databaseIPFilter) 
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));
            string strSQL0 = "DELETE FROM ipfilters";
            dbex_CodedUI.ExecuteSQL(strSQL0);
            dbex_CodedUI.sqlexecute.Parameters.Clear();
            foreach (IPFiltersDTO IPFilterData in databaseIPFilter)
            {
                string strSQL1 = "INSERT INTO ipfilters VALUES (@IPAddress, @Description, @Active, NULL, NULL, NULL, NULL)";
                dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@IPAddress", IPFilterData.ipAddress);
                dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@Description", IPFilterData.description);
                dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@Active", (IPFilterData.active ? 1 : 0));
                dbex_CodedUI.ExecuteSQL(strSQL1);
                dbex_CodedUI.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary> 
        /// Restores the default sorting order of the IP filters grid
        /// </summary>
        private void RestoreDefaultSortingOrder()
        {
            int employeeid = AutoTools.GetEmployeeIDByUsername(_executingProduct);
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));
            //Ensure employee is recaching
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@employeeID", employeeid);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@currentDate", DateTime.Now);
            dbex_CodedUI.ExecuteSQL("UPDATE employees SET CacheExpiry = @currentDate WHERE employeeID = @employeeID");
            dbex_CodedUI.sqlexecute.Parameters.Clear();

            //Ensure grid always uses default sorting order
            string strSQL2 = "DELETE FROM employeeGridSortOrders WHERE employeeID = @employeeID AND gridID = 'gridIPFilters'";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@employeeID", employeeid);
            dbex_CodedUI.ExecuteSQL(strSQL2);
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }
    }
}
