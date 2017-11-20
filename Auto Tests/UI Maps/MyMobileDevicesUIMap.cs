namespace Auto_Tests.UIMaps.MyMobileDevicesUIMapClasses
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
    
    
    public partial class MyMobileDevicesUIMap
    {
        private cTableDataRow row;
        private HtmlTable htmlTable;

        /// <summary>
        /// Used to Click Edit against a Mobile Device
        ///</summary>
        public void ClickEditFieldLink(BrowserWindow searchSpace, string searchParameterToEdit)
        {
            UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITbl_myMobileDevicesTable.WaitForControlReady();
            htmlTable = (HtmlTable)UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITbl_myMobileDevicesTable;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit));
            Mouse.Click(row.UIEditImage, new Point(3, 3));
        }

        /// <summary>
        /// Used to click delete against a specific entry on the mobile devices grid 
        ///</summary>
        public void ClickDeleteFieldLink(BrowserWindow searchSpace, string searchParameterToEdit)
        {
            htmlTable = (HtmlTable)UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITbl_myMobileDevicesTable;
            htmlTable.WaitForControlReady();
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit));
            Mouse.Click(row.UIDeleteImage, new Point(3, 3));
        }

        /// <summary>
        /// Used to validate values passed in expectedVal1 and expectedVal2 can be found in the grid
        ///</summary>
        public void ValidateMyMobileDevicesTable(BrowserWindow searchSpace, string expectedVal1, string expectedVal2, string expectedVal3, bool shouldBePresent = true)
        {
            List<String> tableHeaders = new List<string>();
            tableHeaders.Add("Name");
            tableHeaders.Add("Type");
            tableHeaders.Add("Key Assigned");
            
            UIMyMobileDevicesWindoWindow window = new UIMyMobileDevicesWindoWindow();

            htmlTable = window.UIMyMobileDevicesDocument.UITbl_myMobileDevicesTable;

            Dictionary<string, int> cellLocations = GridHelpers.MapTableHeadersToLocation(htmlTable.Rows, tableHeaders);
            int rowLocation = GridHelpers.FindRowInGridForId(htmlTable, htmlTable.Rows, expectedVal1);
            if (!shouldBePresent)
            {
                Assert.AreEqual(-1, rowLocation);
            }
            else
            {
                UITestControlCollection cellCollection = ((HtmlRow)htmlTable.Rows[rowLocation]).Cells;

                //Assert Name field
                HtmlCell cell = (HtmlCell)cellCollection[cellLocations["Name"]];
                Assert.AreEqual(expectedVal1, cell.InnerText);
                
                //Assert Type field
                cell = (HtmlCell)cellCollection[cellLocations["Type"]];
                Assert.AreEqual(expectedVal2, cell.InnerText);
          
                cell = (HtmlCell)cellCollection[cellLocations["Key Assigned"]];
                object nativeelement = cell.NativeElement;
                bool ischecked = (((mshtml.HTMLTableCellClass)(nativeelement)).innerHTML).Contains("CHECKED");
                Assert.AreEqual(expectedVal3, ischecked.ToString());
            }
        }

        /// <summary>
        /// Used to Click the Header of the Table in order to sort the grid by column
        ///</summary>
        public void ClickTableHeader(HtmlHyperlink sortableTableHeader)
        {
            // sortableTableHeader.WaitForControlReady();
            Mouse.Click(sortableTableHeader, new Point(32, 7));
        }

        /// <summary>
        /// Used to compare the order of the data in the grid and the database match
        ///</summary>
        public void VerifyCorrectSortingOrderForTable(SortMobileDevicesByColumn sortby, EnumHelper.TableSortOrder sortingOrder, int employeeID, ProductType executingProduct)
        {
            List<MobileDevicesMethods.MobileDevice> expectedData = getMobileDevicesOrderFromDatabase(sortby, sortingOrder, employeeID, executingProduct);
            List<MobileDevicesMethods.MobileDevice> actualData = new List<MobileDevicesMethods.MobileDevice>();

            HtmlTable table = (HtmlTable)UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITbl_myMobileDevicesTable1;
            UITestControlCollection collection = table.Rows;

            //Remove table header from list
            collection.RemoveAt(0);
            //Ensure count of data in table is the same with count of data in database
            Assert.AreEqual(table.RowCount - 1, expectedData.Count);

            foreach (HtmlRow row in collection)
            {
                UITestControlCollection rowCollection = row.Cells;
                string Name = ((HtmlCell)rowCollection[2]).InnerText != null ? ((HtmlCell)rowCollection[2]).InnerText : "";
                string Type = ((HtmlCell)rowCollection[3]).InnerText != null ? ((HtmlCell)rowCollection[3]).InnerText : "";
                string ActivationKey = ((HtmlCell)rowCollection[4]).InnerText != null ? ((HtmlCell)rowCollection[4]).InnerText : "";

                actualData.Add(new MobileDevicesMethods.MobileDevice()  {DeviceName = Name, DeviceType = Type, PairingKey = ActivationKey});
            }

            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.AreEqual(actualData[i].DeviceName, expectedData[i].DeviceName);
                Assert.AreEqual(actualData[i].DeviceType, expectedData[i].DeviceType);
                Assert.AreEqual(actualData[i].PairingKey, expectedData[i].PairingKey);
            }
        }

        /// <summary>
        /// Validate label of the modal
        ///</summary>
        public void ValidateLabel(HtmlLabel label, string innerText)
        {
            Assert.AreEqual(label.InnerText, innerText);
        }

        /// <summary>
        /// Used to get the order of the data from the database
        ///</summary>
        internal List<MobileDevicesMethods.MobileDevice> getMobileDevicesOrderFromDatabase(SortMobileDevicesByColumn sortby, Auto_Tests.Tools.EnumHelper.TableSortOrder sortingOrder, int employeeID, ProductType executingProduct)
        {
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));
            return new MobileDevicesDAO(db).GetCorrectSortingOrderFromDB(sortby, sortingOrder, employeeID, executingProduct);
        }

        private class cTableDataRow : HtmlRow
        {
            private HtmlImage imgDelete;
            private HtmlImage imgEdit;

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
        }
    }
}
