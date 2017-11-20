namespace Auto_Tests.UIMaps.EmployeesNewUIMapClasses
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Input;
    using System.CodeDom.Compiler;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using MouseButtons = System.Windows.Forms.MouseButtons;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using Auto_Tests.Tools;
    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees;
    using System.Text;
    
    
    public partial class EmployeesNewUIMap
    {
        private cTableDataRow row;
        private HtmlTable htmlTable;

        public string ModalMessage
        {
            get
            {
                string modalMessage = this.EmployeeValidationMessageWindow.EmployeeValidationMessageDocument.MasterPopupPane.InnerText;
                var sb = new StringBuilder(modalMessage.Length);

                foreach (char i in modalMessage)
                    if (i != '\n' && i != '\r' && i != '\t')
                    {
                        sb.Append(i);
                    }
                return sb.ToString();
            }
        }

        #region employee grid
        public void ClickEditFieldLink(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)EmployeeGridWindow.EmployeeGridDocument.EmployeeGrid;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 7));
            Mouse.Click(row.UIEditImage, new Point(3, 3));
        }

        public void ClickDeleteFieldLink(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)EmployeeGridWindow.EmployeeGridDocument.EmployeeGrid;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 7));
            Mouse.Click(row.UIDeleteImage, new Point(3, 3));
        }

        public void ClickArchiveFieldLink(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)EmployeeGridWindow.EmployeeGridDocument.EmployeeGrid;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 7));
            Mouse.Click(row.UIArchiveImage, new Point(3,3));
        }

        public void ClickInactiveFieldLink(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)EmployeeGridWindow.EmployeeGridDocument.EmployeeGrid;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 7));
            Mouse.Click(row.UIInactiveImage, new Point(3, 3));
        }

        public void CheckInactiveFieldExists(string searchParameterToEdit, Boolean exists)
        {

            htmlTable = (HtmlTable)EmployeeGridWindow.EmployeeGridDocument.EmployeeGrid;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 7));
            if (exists)
            {
                Assert.IsTrue(row.UIInactiveImage.Exists);
            }
            else
            {
                Assert.IsFalse(row.UIInactiveImage.Exists);
            }
        }

        public int ReturnEmployeeIDFromGrid(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)EmployeeGridWindow.EmployeeGridDocument.EmployeeGrid;   
            UITestControlCollection collection = htmlTable.Rows;
            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 7));
            int employeeID = 0;
            int.TryParse(row.UIArchiveImage.Container.FriendlyName.Replace("tbl_gridEmployees_", ""), out employeeID);
            return employeeID;
        }


        public void ValidateEmployeesGrid(string expectedVal1, string expectedVal2, string expectedVal3, string expectedVal4, string expectedVal5 = "--", bool shouldBePresent = true)
        {
            List<String> tableHeaders = new List<string>();
            tableHeaders.Add("Username");
            tableHeaders.Add("Title (Mr/Mrs/Dr)");
            tableHeaders.Add("First Name");
            tableHeaders.Add("Surname");
            if (expectedVal5 != "--")
            {
                tableHeaders.Add("Group Name");
            }

            htmlTable = (HtmlTable)EmployeeGridWindow.EmployeeGridDocument.EmployeeGrid;

            Dictionary<string, int> cellLocations = GridHelpers.MapTableHeadersToLocation(htmlTable.Rows, tableHeaders);
            int rowLocation = GridHelpers.FindRowInGridForId(htmlTable, htmlTable.Rows, expectedVal1, 7);
            if (!shouldBePresent)
            {
                Assert.AreEqual(-1, rowLocation);
            }
            else
            {
                UITestControlCollection cellCollection = ((HtmlRow)htmlTable.Rows[rowLocation]).Cells;

                HtmlCell cell = (HtmlCell)cellCollection[cellLocations["Username"]];
                Assert.AreEqual(expectedVal1, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["Title (Mr/Mrs/Dr)"]];
                Assert.AreEqual(expectedVal2, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["First Name"]];
                Assert.AreEqual(expectedVal3, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["Surname"]];
                Assert.AreEqual(expectedVal4, cell.InnerText);
                if (expectedVal5 != "--")
                {
                    cell = (HtmlCell)cellCollection[cellLocations["Group Name"]];
                    Assert.AreEqual(expectedVal5, cell.InnerText);
                }
            }
        } 

        private class cTableDataRow : HtmlRow
        {
            private HtmlImage imgDelete;
            private HtmlImage imgEdit;
            private HtmlImage imgArchive;
            private HtmlImage imgInactive;

            public cTableDataRow(UITestControl searchLimitContainer, int rowId)
                : base(searchLimitContainer)
            {
                this.SearchProperties[HtmlRow.PropertyNames.Name] = null;
                this.FilterProperties[HtmlRow.PropertyNames.RowIndex] = Convert.ToString(rowId);
                this.FilterProperties[HtmlRow.PropertyNames.TagInstance] = "2";
            }

            public HtmlImage UIDeleteImage
            {
                get
                {
                    if ((this.imgDelete == null))
                    {
                        this.imgDelete = new HtmlImage(this);
                        #region Search Criteria
                        this.imgDelete.SearchProperties[HtmlImage.PropertyNames.Alt] = "Delete";
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] = "/shared/images/icons/delete2.png";
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.Src] = "http://fwtest.sel-expenses.com/shared/images/icons/delete2.png";
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.Class] = null;
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] = "alt=Delete src=\"/shared/images/icons/del";
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "2";
                        #endregion
                    }
                    return this.imgDelete;
                }
            }

            public HtmlImage UIEditImage
            {
                get
                {
                    if ((this.imgEdit == null))
                    {
                        this.imgEdit = new HtmlImage(this);
                        #region Search Criteria
                        this.imgEdit.SearchProperties[HtmlImage.PropertyNames.Id] = null;
                        this.imgEdit.SearchProperties[HtmlImage.PropertyNames.Name] = null;
                        this.imgEdit.SearchProperties[HtmlImage.PropertyNames.Alt] = "Edit";
                        this.imgEdit.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] = "/shared/images/icons/edit.png";
                        this.imgEdit.FilterProperties[HtmlImage.PropertyNames.Src] = "http://fwtest.sel-expenses.com/shared/images/icons/edit.png";
                        this.imgEdit.FilterProperties[HtmlImage.PropertyNames.Class] = null;
                        this.imgEdit.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] = "alt=Edit src=\"/shared/images/icons/edit.";
                        this.imgEdit.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "1";
                        #endregion
                    }
                    return this.imgEdit;
                }
            }

            public HtmlImage UIArchiveImage
            {
                get
                {
                    if ((this.imgArchive == null))
                    {
                        this.imgArchive = new HtmlImage(this);
                        #region Search Criteria
                        this.imgArchive.SearchProperties[HtmlImage.PropertyNames.Alt] = null;
                        this.imgArchive.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] = "/shared/images/icons/folder_lock.png";
                        this.imgArchive.FilterProperties.Add(new PropertyExpression(HtmlImage.PropertyNames.Src, "sel-expenses.com/shared/images/icons/folder_lock.png", PropertyExpressionOperator.Contains));
                        this.imgArchive.FilterProperties[HtmlImage.PropertyNames.Class] = "";
                        this.imgArchive.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] = "title=Archive src=\"/shared/images/icons/folder_lock.png";
                        this.imgArchive.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "1";
                        #endregion
                    }
                    return this.imgArchive;
                }
            }

            public HtmlImage UIInactiveImage
            {
                get
                {
                    if ((this.imgInactive == null))
                    {
                        this.imgInactive = new HtmlImage(this);
                        #region Search Criteria
                        this.imgInactive.SearchProperties[HtmlImage.PropertyNames.Alt] = "Inactive Account";
                        this.imgInactive.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] = "/static/icons/16/new-icons/stopwatch_stop.png";
                        this.imgInactive.FilterProperties.Add(new PropertyExpression(HtmlImage.PropertyNames.Src, "sel-expenses.com/static/icons/16/new-icons/stopwatch_stop.png", PropertyExpressionOperator.Contains));
                        this.imgInactive.FilterProperties[HtmlImage.PropertyNames.Class] = "";
                        this.imgInactive.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] = "alt=Inactive Account src=\"/static/icons/16/new-icons/stopwatch_stop.png";
                        this.imgInactive.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "23";
                        #endregion
                    }
                    return this.imgInactive;
                }
            }
        }
        #endregion

        internal class EmployeeControls
        {
            internal HtmlEdit usernameTxt { get; set; }
            internal HtmlEdit title { get; set; }
            internal HtmlEdit firstName { get; set; }
            internal HtmlEdit middleNames { get; set; }
            internal HtmlEdit surname { get; set; }
            internal HtmlEdit maidenName { get; set; }
            internal HtmlEdit emailAddress { get; set; }
            internal HtmlEdit extensionNumber { get; set; }
            internal HtmlEdit mobileNumber { get; set; }
            internal HtmlEdit pagerNumber { get; set; }
            internal HtmlComboBox locale { get; set; }
            internal HtmlCheckBox sendPasswordEmail { get; set; }
            internal HtmlCheckBox sendWelcomeEmail { get; set; }

            internal HtmlEdit creditAccount { get; set; }
            internal HtmlEdit payrollNumber { get; set; }
            internal HtmlEdit position { get; set; }
            internal HtmlEdit niNumber { get; set; }
            internal HtmlEdit hireDate { get; set; }
            internal HtmlEdit termDate { get; set; }
            internal HtmlComboBox primaryCountry { get; set; }
            internal HtmlComboBox primaryCurrency { get; set; }
            internal HtmlComboBox lineManager { get; set; }
            internal HtmlEdit startingMileage { get; set; }
            internal HtmlEdit currentMileage { get; set; }
            internal HtmlEdit startMileageDate { get; set; }
            internal HtmlComboBox trust { get; set; }

            internal HtmlEdit homeEmail { get; set; }
            internal HtmlEdit telNumvber { get; set; }
            internal HtmlEdit faxNumber { get; set; }
            internal HtmlEdit licenceNumber { get; set; }
            internal HtmlEdit licenceExpiryDate { get; set; }
            internal HtmlEdit lastChecked { get; set; }
            internal HtmlEdit checkedBy { get; set; }
            internal HtmlEdit accName { get; set; }
            internal HtmlEdit accNumber { get; set; }
            internal HtmlEdit accType { get; set; }
            internal HtmlEdit accSortCode { get; set; }
            internal HtmlEdit accRef { get; set; }
            internal HtmlComboBox gender { get; set; }
            internal HtmlEdit dob { get; set; }

            internal HtmlComboBox signoffGroup { get; set; }
            internal HtmlComboBox ccSignoffGroup { get; set; }
            internal HtmlComboBox pcSignoffGroup { get; set; }
            internal HtmlComboBox aSignoffGroup { get; set; }

            internal HtmlCheckBox standard { get; set; }
            internal HtmlCheckBox esrOutboundImportSummary { get; set; }            
            internal HtmlCheckBox esrOutboundImportStarted { get; set; }
            internal HtmlCheckBox esrOutboundManagerAccessUpdate { get; set; }
            internal HtmlCheckBox esrOutboundInvalidPostcodes { get; set; }
            internal HtmlDiv employeesDiv { get; set; }

            protected EmployeesNewUIMap _cEmployeesMethods;
            protected ControlLocator<HtmlControl> _ControlLocator { get; private set; }

            internal EmployeeControls(EmployeesNewUIMap cEmployeesMethods, string tabName)
            {
                _cEmployeesMethods = cEmployeesMethods;
                _ControlLocator = new ControlLocator<HtmlControl>();
                FindControls(tabName);
            }

            /// <summary>
            /// Locates controls that are declared within this class
            /// </summary>
            internal void FindControls(string tabName)
            {
                if (tabName == EnumHelper.GetEnumDescription(EmployeeTabName.GeneralDetails))
                {
                    //employeesDiv = (HtmlDiv)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_body", new HtmlDiv(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //employeesDiv = (HtmlDiv)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral", new HtmlDiv(_cEmployeesMethods.UINewEmployeeWindowsInWindow.UINewEmployeeDocument));
                    usernameTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral_txtusername", new HtmlEdit(_cEmployeesMethods.EmployeeGeneralTabWindow.EmployeeGeneralTabDocument.EmployeeGeneralTabPane));
                    title = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral_txttitle", new HtmlEdit(_cEmployeesMethods.EmployeeGeneralTabWindow.EmployeeGeneralTabDocument.EmployeeGeneralTabPane));
                    firstName = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral_txtfirstname", new HtmlEdit(_cEmployeesMethods.EmployeeGeneralTabWindow.EmployeeGeneralTabDocument.EmployeeGeneralTabPane));
                    middleNames = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral_txtmiddlenames", new HtmlEdit(_cEmployeesMethods.EmployeeGeneralTabWindow.EmployeeGeneralTabDocument.EmployeeGeneralTabPane));
                    surname = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral_txtsurname", new HtmlEdit(_cEmployeesMethods.EmployeeGeneralTabWindow.EmployeeGeneralTabDocument.EmployeeGeneralTabPane));
                    maidenName = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral_txtmaidenname", new HtmlEdit(_cEmployeesMethods.EmployeeGeneralTabWindow.EmployeeGeneralTabDocument.EmployeeGeneralTabPane));
                    emailAddress = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral_txtemail", new HtmlEdit(_cEmployeesMethods.EmployeeGeneralTabWindow.EmployeeGeneralTabDocument.EmployeeGeneralTabPane));
                    extensionNumber = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral_txtextension", new HtmlEdit(_cEmployeesMethods.EmployeeGeneralTabWindow.EmployeeGeneralTabDocument.EmployeeGeneralTabPane));
                    mobileNumber = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral_txtmobileno", new HtmlEdit(_cEmployeesMethods.EmployeeGeneralTabWindow.EmployeeGeneralTabDocument.EmployeeGeneralTabPane));
                    pagerNumber = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral_txtpagerno", new HtmlEdit(_cEmployeesMethods.EmployeeGeneralTabWindow.EmployeeGeneralTabDocument.EmployeeGeneralTabPane));
                    locale = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral_ddlstLocale", new HtmlComboBox(_cEmployeesMethods.EmployeeGeneralTabWindow.EmployeeGeneralTabDocument.EmployeeGeneralTabPane));
                    sendPasswordEmail = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral_chkSendPasswordEmail", new HtmlCheckBox(_cEmployeesMethods.EmployeeGeneralTabWindow.EmployeeGeneralTabDocument.EmployeeGeneralTabPane));
                    sendWelcomeEmail = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabGeneral_chkWelcomeEmail", new HtmlCheckBox(_cEmployeesMethods.EmployeeGeneralTabWindow.EmployeeGeneralTabDocument.EmployeeGeneralTabPane));
                }
                else if (tabName == EnumHelper.GetEnumDescription(EmployeeTabName.Work))
                {
                    creditAccount = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabWork_txtcreditaccount", new HtmlEdit(_cEmployeesMethods.EmployeeWorkTabWindow.EmployeeWorkTabDocument.EmployeeWorkTabPane));
                    payrollNumber = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabWork_txtpayroll", new HtmlEdit(_cEmployeesMethods.EmployeeWorkTabWindow.EmployeeWorkTabDocument.EmployeeWorkTabPane));
                    position = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabWork_txtposition", new HtmlEdit(_cEmployeesMethods.EmployeeWorkTabWindow.EmployeeWorkTabDocument.EmployeeWorkTabPane));
                    niNumber = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabWork_txtninumber", new HtmlEdit(_cEmployeesMethods.EmployeeWorkTabWindow.EmployeeWorkTabDocument.EmployeeWorkTabPane));
                    hireDate = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabWork_txthiredate", new HtmlEdit(_cEmployeesMethods.EmployeeWorkTabWindow.EmployeeWorkTabDocument.EmployeeWorkTabPane));
                    termDate = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabWork_txtleavedate", new HtmlEdit(_cEmployeesMethods.EmployeeWorkTabWindow.EmployeeWorkTabDocument.EmployeeWorkTabPane));
                    primaryCountry = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabWork_cmbcountry", new HtmlComboBox(_cEmployeesMethods.EmployeeWorkTabWindow.EmployeeWorkTabDocument.EmployeeWorkTabPane));
                    primaryCurrency = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabWork_cmbcurrency", new HtmlComboBox(_cEmployeesMethods.EmployeeWorkTabWindow.EmployeeWorkTabDocument.EmployeeWorkTabPane));
                    lineManager = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabWork_cmblinemanager", new HtmlComboBox(_cEmployeesMethods.EmployeeWorkTabWindow.EmployeeWorkTabDocument.EmployeeWorkTabPane));
                    startingMileage = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabWork_txtstartmiles", new HtmlEdit(_cEmployeesMethods.EmployeeWorkTabWindow.EmployeeWorkTabDocument.EmployeeWorkTabPane));
                    startMileageDate = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabWork_txtstartmileagedate", new HtmlEdit(_cEmployeesMethods.EmployeeWorkTabWindow.EmployeeWorkTabDocument.EmployeeWorkTabPane));
                    currentMileage = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabWork_txtCurrentMileage", new HtmlEdit(_cEmployeesMethods.EmployeeWorkTabWindow.EmployeeWorkTabDocument.EmployeeWorkTabPane));
                    trust = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabWork_ddlNHSTrust", new HtmlComboBox(_cEmployeesMethods.EmployeeWorkTabWindow.EmployeeWorkTabDocument.EmployeeWorkTabPane));
                }
                else if (tabName == EnumHelper.GetEnumDescription(EmployeeTabName.Personal) || tabName == EnumHelper.GetEnumDescription(EmployeeTabName.AllTabs))
                {
                    //homeEmail = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_txtemailhome", new HtmlEdit(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //telNumvber = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_txttelno", new HtmlEdit(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //faxNumber = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_txtfaxno", new HtmlEdit(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //licenceNumber = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_txtlicencenumber", new HtmlEdit(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //licenceExpiryDate = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_txtlicenceexpiry", new HtmlEdit(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //lastChecked = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_txtlastchecked", new HtmlEdit(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //checkedBy = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_txtlicencecheckedby", new HtmlEdit(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //accName = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_txtaccountholder", new HtmlEdit(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //accNumber = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_txtaccountnumber", new HtmlEdit(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //accType = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_txtaccounttype", new HtmlEdit(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //accSortCode = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_txtsortcode", new HtmlEdit(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //accRef = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_txtreference", new HtmlEdit(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //gender = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_cmbgender", new HtmlComboBox(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //dob = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabPersonal_txtdateofbirth", new HtmlEdit(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));

                }
                else if (tabName == EnumHelper.GetEnumDescription(EmployeeTabName.Claims) || tabName == EnumHelper.GetEnumDescription(EmployeeTabName.AllTabs))
                {
                    //signoffGroup = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabClaims_cmbsignoffs", new HtmlComboBox(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //ccSignoffGroup = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabClaims_cmbsignoffcc", new HtmlComboBox(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //pcSignoffGroup = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabClaims_cmbsignoffpc", new HtmlComboBox(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //aSignoffGroup = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabClaims_cmbadvancesgroup", new HtmlComboBox(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                }
                else if(tabName == EnumHelper.GetEnumDescription(EmployeeTabName.Notification) || tabName == EnumHelper.GetEnumDescription(EmployeeTabName.AllTabs))
                {
                    //standard = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabEmailNotifications_emailNotification_4", new HtmlCheckBox(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //esrOutboundImportSummary = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabEmailNotifications_emailNotification_3", new HtmlCheckBox(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //esrOutboundImportStarted = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabEmailNotifications_emailNotification_6", new HtmlCheckBox(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //esrOutboundManagerAccessUpdate = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabEmailNotifications_emailNotification_11", new HtmlCheckBox(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                    //esrOutboundInvalidPostcodes = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_tabsGeneral_tabEmailNotifications_emailNotification_12", new HtmlCheckBox(_cEmployeesMethods.UINewEmployeeWindowsInWindow2.UINewEmployeeDocument));
                }
            }
        }
    }
}
