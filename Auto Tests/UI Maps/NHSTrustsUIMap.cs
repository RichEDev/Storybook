namespace Auto_Tests.UIMaps.NHSTrustsUIMapClasses
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
    
    
    public partial class NHSTrustsUIMap
    {
        private HtmlTable htmlTable;
        private cTableDataRow row;

        public void ValidateNhsTrustArchivedState(string searchParameterToEdit, bool isArchived = false)
        {
            htmlTable = (HtmlTable)NHSTrustsControlsModalsLinksWindow.NHSTrustsGridDocument.NHSTrustsGrid;

            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 4));
            if (isArchived)
            {
                Assert.IsTrue(row.UIArchiveImage.AbsolutePath.Contains("folder_into.gif"), "The NHSTrust isnt archived");

            }
            else
            {
                Assert.IsTrue(row.UIArchiveImage.AbsolutePath.Contains("folder_lock.gif"), "The NHSTrust is archived");
            }
        }


        public int ReturnNhsTrustIdFromGrid(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)NHSTrustsControlsModalsLinksWindow.NHSTrustsGridDocument.NHSTrustsGrid;
            UITestControlCollection collection = htmlTable.Rows;
            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 4));
            int nhsTrustId = 0;
            int.TryParse(row.UIEditImage.Container.FriendlyName.Replace("tbl_trusts_", ""), out nhsTrustId);
            return nhsTrustId;
        }

        public void ClickEditFieldLink(string searchParameterToEdit)
        {
            htmlTable = (HtmlTable)NHSTrustsControlsModalsLinksWindow.NHSTrustsGridDocument.NHSTrustsGrid;

            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 4));
            Mouse.Click(row.UIEditImage, new Point(3, 3));
        }

        public void ClickDeleteFieldLink(string searchParameterToEdit)
        {
            htmlTable = (HtmlTable)NHSTrustsControlsModalsLinksWindow.NHSTrustsGridDocument.NHSTrustsGrid;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 4));
            Mouse.Click(row.UIDeleteImage, new Point(3, 3));
        }

        public void ClickArchiveFieldLink(string searchParameterToArchive)
        {
            htmlTable = (HtmlTable)NHSTrustsControlsModalsLinksWindow.NHSTrustsGridDocument.NHSTrustsGrid;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToArchive, 4));
            Mouse.Click(row.UIArchiveImage, new Point(3, 3));
        }

        public void ClickUnArchiveFieldLink(string searchParameterToUnArchive)
        {
            htmlTable = (HtmlTable)NHSTrustsControlsModalsLinksWindow.NHSTrustsGridDocument.NHSTrustsGrid;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToUnArchive, 4));
            Mouse.Click(row.UIUnarchiveImage, new Point(3, 3));
        }

        public void ValidateNHSTrustGrid(string expectedVal1, string expectedVal2, string expectedVal3, string expectedVal4, bool shouldBePresent = true)
        {
            List<String> tableHeaders = new List<string>();
            tableHeaders.Add("Trust Name");
            tableHeaders.Add("Trust VPD");
            tableHeaders.Add("Run Sequence");
            tableHeaders.Add("ESR Interface Version");

            NHSTrustsControlsModalsLinksWindow window = new NHSTrustsControlsModalsLinksWindow();
            htmlTable = window.NHSTrustsGridDocument.NHSTrustsGrid;

            Dictionary<string, int> cellLocations = GridHelpers.MapTableHeadersToLocation(htmlTable.Rows, tableHeaders);
            int rowLocation = GridHelpers.FindRowInGridForId(htmlTable, htmlTable.Rows, expectedVal1, 6);
            if (!shouldBePresent)
            {
                Assert.AreEqual(-1, rowLocation);
            }
            else
            {
                UITestControlCollection cellCollection = ((HtmlRow)htmlTable.Rows[GridHelpers.FindRowInGridForId(htmlTable, htmlTable.Rows, expectedVal1, 4)]).Cells;

                //Assert Trust Name field
                HtmlCell cell = (HtmlCell)cellCollection[cellLocations["Trust Name"]];
                Assert.AreEqual(expectedVal1, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["Trust VPD"]];
                Assert.AreEqual(expectedVal2, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["Run Sequence"]];
                Assert.AreEqual(expectedVal3, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["ESR Interface Version"]];
                Assert.AreEqual(expectedVal4, cell.InnerText);
            }
        }

        private class cTableDataRow : HtmlRow
        {
            private HtmlImage imgDelete;
            private HtmlImage imgEdit;
            private HtmlImage imgArchive;
            private HtmlImage imgUnarchive;

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
            public HtmlImage UIUnarchiveImage
            {
                get
                {
                    if ((this.imgUnarchive == null))
                    {
                        this.imgUnarchive = new HtmlImage(this);
                        #region Search Criteria
                        this.imgUnarchive.SearchProperties[HtmlImage.PropertyNames.Alt] = null;
                        this.imgUnarchive.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] = "/shared/images/icons/folder_into.png";
                        this.imgUnarchive.FilterProperties.Add(new PropertyExpression(HtmlImage.PropertyNames.Src, "sel-expenses.com/shared/images/icons/folder_into.png", PropertyExpressionOperator.Contains));
                        this.imgUnarchive.FilterProperties[HtmlImage.PropertyNames.Class] = "";
                        this.imgUnarchive.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] = "title=Archive src=\"/shared/images/icons/folder_into.png";
                        this.imgUnarchive.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "1";
                        #endregion
                    }
                    return this.imgUnarchive;
                }
            }
        }
    }
}
