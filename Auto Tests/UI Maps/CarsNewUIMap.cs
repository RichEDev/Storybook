namespace Auto_Tests.UIMaps.CarsNewUIMapClasses
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
    
    
    public partial class CarsNewUIMap
    {
        private cTableDataRow row;
        private HtmlTable htmlTable;

        #region employee grid
        public void ClickEditFieldLink(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)CarGridWindow.CarGridDocument.CarGrid;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 3));
            Mouse.Click(row.UIEditImage, new Point(3, 3));
        }

        public void ClickDeleteFieldLink(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)CarGridWindow.CarGridDocument.CarGrid;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 3));
            Mouse.Click(row.UIDeleteImage, new Point(3, 3));
        }

        public void ClickArchiveFieldLink(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)CarGridWindow.CarGridDocument.CarGrid;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 3));
            Mouse.Click(row.UIArchiveImage, new Point(3, 3));
        }

        public int ReturnCarIDFromGrid(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)CarGridWindow.CarGridDocument.CarGrid;
            UITestControlCollection collection = htmlTable.Rows;
            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit));
            int carID = 0;
            int.TryParse(row.UIArchiveImage.Container.FriendlyName.Replace("tbl_gridCars_", ""), out carID);
            return carID;
        }


        public void ValidateCarsGrid(string expectedVal1, string expectedVal2, string expectedVal3, string expectedVal4, string expectedVal5,  string expectedVal6,  string expectedVal7, bool shouldBePresent = true)
        {
            List<String> tableHeaders = new List<string>();
            tableHeaders.Add("Vehicle Make");
            tableHeaders.Add("Vehicle Model");
            tableHeaders.Add("Registration Number");
            tableHeaders.Add("Vehicle Start Date");
            tableHeaders.Add("Vehicle End Date");
            tableHeaders.Add("Vehicle Engine Type");
            tableHeaders.Add("Vehicle Status");

            htmlTable = (HtmlTable)CarGridWindow.CarGridDocument.CarGrid;

            Dictionary<string, int> cellLocations = GridHelpers.MapTableHeadersToLocation(htmlTable.Rows, tableHeaders);
            int rowLocation = GridHelpers.FindRowInGridForId(htmlTable, htmlTable.Rows, expectedVal1,3);
            if (!shouldBePresent)
            {
                Assert.AreEqual(-1, rowLocation);
            }
            else
            {
                UITestControlCollection cellCollection = ((HtmlRow)htmlTable.Rows[rowLocation]).Cells;

                HtmlCell cell = (HtmlCell)cellCollection[cellLocations["Vehicle Make"]];
                Assert.AreEqual(expectedVal1, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["Vehicle Model"]];
                Assert.AreEqual(expectedVal2, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["Registration Number"]];
                Assert.AreEqual(expectedVal3, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["Vehicle Start Date"]];
                Assert.AreEqual(expectedVal4, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["Vehicle End Date"]];
                Assert.AreEqual(expectedVal5, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["Vehicle Engine Type"]];
                Assert.AreEqual(expectedVal6, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["Vehicle Status"]];
                object nativeelement = cell.NativeElement;
                bool ischecked = (((mshtml.HTMLTableCellClass)(nativeelement)).innerHTML).Contains("CHECKED");
                Assert.AreEqual(expectedVal7, ischecked.ToString());
            }
        }

        private class cTableDataRow : HtmlRow
        {
            private HtmlImage imgDelete;
            private HtmlImage imgEdit;
            private HtmlImage imgArchive;

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
                        //this.imgDelete.FilterProperties[HtmlImage.PropertyNames.Src] = "http://fwtest.sel-expenses.com/shared/images/icons/delete2.png";
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
                        //this.imgEdit.FilterProperties[HtmlImage.PropertyNames.Src] = "http://fwtest.sel-expenses.com/shared/images/icons/edit.png";
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
        }
        #endregion

    }
}
