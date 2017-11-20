namespace Auto_Tests.UIMaps.IPAddressFilteringUIMapClasses
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
    using System.Threading;
    using Auto_Tests.UIMaps.IPAddressFilteringUIMapClasses;
    using System.Text;
    using System.Linq;
    using Auto_Tests.Coded_UI_Tests.Spend_Management.System_Options.IP_Address_Filtering;
    using System.Configuration;
    using SpendManagementLibrary;
    using System.Data.SqlClient;
    using System.Reflection;
    using Auto_Tests.Tools;
    using System.Collections;

    public partial class IPAddressFilteringUIMap
    {
        private TableDataRow _row;
        private HtmlTable _htmlTable;


        public void ClickDeleteFieldLink(string searchParameterToRemove)
        {
            _htmlTable = (HtmlTable)UIIPAddressFilteringWiWindow.UIIPAddressFilteringDocument.UITbl_gridIPFiltersTable; 
            UITestControlCollection collection = _htmlTable.Rows;

            _row = new TableDataRow(_htmlTable, GridHelpers.FindRowInGridForId(_htmlTable, collection, searchParameterToRemove));

            Mouse.Click(_row.UIDeleteImage, new Point(0, 0));
        }

        public void ClickEditFieldLink(BrowserWindow searchSpace, string searchParameterToEdit)
        {
            _htmlTable = (HtmlTable)UIIPAddressFilteringWiWindow.UIIPAddressFilteringDocument.UITbl_gridIPFiltersTable;
            UITestControlCollection collection = _htmlTable.Rows;

            _row = new TableDataRow(_htmlTable, GridHelpers.FindRowInGridForId(_htmlTable, collection, searchParameterToEdit));
            Mouse.Click(_row.UIEditImage, new Point(0, 0));
        }

        public void ValidateIPFiltersDeletion(string searchParameterDeletedPrimaryKey)
        {
            _htmlTable = (HtmlTable)UIIPAddressFilteringWiWindow.UIIPAddressFilteringDocument.UITbl_gridIPFiltersTable;
            UITestControlCollection collection = _htmlTable.Rows;

            Assert.AreEqual(-1, GridHelpers.FindRowInGridForId(_htmlTable, collection, searchParameterDeletedPrimaryKey));
        }

        public void ValidateIPFilter(BrowserWindow searchSpace)
        {
            List<String> tableHeaders = new List<string>();
            tableHeaders.Add("IP Address");
            tableHeaders.Add("Description");
            tableHeaders.Add("Active");

            UIIPAddressFilteringWiWindow window = new UIIPAddressFilteringWiWindow();
            _htmlTable = window.UIIPAddressFilteringDocument.UITbl_gridIPFiltersTable;

            Dictionary<string, int> cellLocations = GridHelpers.MapTableHeadersToLocation(_htmlTable.Rows, tableHeaders);
            UITestControlCollection cellCollection = ((HtmlRow)_htmlTable.Rows[GridHelpers.FindRowInGridForId(_htmlTable, _htmlTable.Rows, PopulateIPFilterDetailsParams.UIIPAddressEditText)]).Cells;

            //Assert IP Address field
            HtmlCell cell = (HtmlCell)cellCollection[cellLocations["IP Address"]];
            Assert.AreEqual(PopulateIPFilterDetailsParams.UIIPAddressEditText, cell.InnerText);

            cell = (HtmlCell)cellCollection[cellLocations["Description"]];
            Assert.AreEqual(PopulateIPFilterDetailsParams.UIDescriptionEditText, cell.InnerText);
            
            cell = (HtmlCell)cellCollection[cellLocations["Active"]];
            object nativeelement = cell.NativeElement;
            bool ischecked = (((mshtml.HTMLTableCellClass)(nativeelement)).innerHTML).Contains("CHECKED");
            Assert.AreEqual(PopulateIPFilterDetailsParams.UIActiveCheckBoxChecked, ischecked);
        }

        public void ClickTableHeader(HtmlHyperlink sortableTableHeader)
        {
            Mouse.Click(sortableTableHeader, new Point(2, 2));
        }

        public void DoesTableContainCorrectElements(SortIPFiltersByColumn sortby, EnumHelper.TableSortOrder sortingOrder)
        {
            List<IPFiltersDTO> expectedData = GetIPFiltersfromDatabase(sortby, sortingOrder);
            List<IPFiltersDTO> actualData = new List<IPFiltersDTO>();

            HtmlTable table = (HtmlTable)UIIPAddressFilteringWiWindow.UIIPAddressFilteringDocument.UITbl_gridIPFiltersTable;
            UITestControlCollection collection = table.Rows;

            //Remove table header from list.
            collection.RemoveAt(0);
            //Ensure correct cound otherwise not point carrying on
            Assert.AreEqual(table.RowCount - 1, expectedData.Count);

            foreach (HtmlRow row in collection)
            {
                UITestControlCollection rowCollection = row.Cells;
                string ipAddress = ((HtmlCell)rowCollection[2]).InnerText != null ? ((HtmlCell)rowCollection[2]).InnerText : "";
                string description = ((HtmlCell)rowCollection[3]).InnerText != null ? ((HtmlCell)rowCollection[3]).InnerText : "";
                
                object x = ((HtmlCell)rowCollection[4]).NativeElement;
                bool ischecked = (((mshtml.HTMLTableCellClass)(x)).innerHTML).Contains("CHECKED");
               
                actualData.Add(new IPFiltersDTO(ipAddress, description, ischecked));
            }

            Assert.IsTrue(expectedData.SequenceEqual(actualData));
        }

      
        internal List<IPFiltersDTO> GetIPFiltersfromDatabase(SortIPFiltersByColumn sortby, Auto_Tests.Tools.EnumHelper.TableSortOrder sortingOrder)
        {
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.expenses));
            return new IPFiltersDAO(db).GetAll(sortby, sortingOrder);
        }

        private class TableDataRow : HtmlRow
        {
            private HtmlImage imgDelete;
            private HtmlImage imgEdit;

            public TableDataRow(UITestControl searchLimitContainer, int rowId) : base(searchLimitContainer) 
            {
                //this.SearchProperties[HtmlRow.PropertyNames.Id] = "tbl_gridIPFilters";
                this.SearchProperties[HtmlRow.PropertyNames.Name] = null;
                //this.FilterProperties[HtmlRow.PropertyNames.ControlDefinition] = "id=tbl_gridIPFilters";
                this.FilterProperties[HtmlRow.PropertyNames.RowIndex] = Convert.ToString(rowId);
                ////this.FilterProperties[HtmlRow.PropertyNames.Class] = null;
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
                        //this.imgDelete.SearchProperties[HtmlImage.PropertyNames.Id] = null;
                        //this.imgDelete.SearchProperties[HtmlImage.PropertyNames.Name] = null;
                        this.imgDelete.SearchProperties[HtmlImage.PropertyNames.Alt] = "Delete";
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] = "/shared/images/icons/delete2.png";
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.Src] = "http://fwtest.sel-expenses.com/shared/images/icons/delete2.png";
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.Class] = null;
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] = "alt=Delete src=\"/shared/images/icons/del";
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "2";
                  //      this.imgDelete.WindowTitles.Add("IP Address Filtering");
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
                    //    this.imgEdit.WindowTitles.Add("IP Address Filtering");
                        #endregion
                    }
                    return this.imgEdit;
                }
            }

        }
    }
    

}
