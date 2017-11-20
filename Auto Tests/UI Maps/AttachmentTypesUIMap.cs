namespace Auto_Tests.UIMaps.AttachmentTypesUIMapClasses
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
    
    
    public partial class AttachmentTypesUIMap
    {
        private cTableDataRow row;
        private HtmlTable htmlTable;

        /// <summary>
        /// Used to click delete against a specific entry on the attachment types grid 
        ///</summary>
        public void ClickDeleteFieldLink(BrowserWindow searchSpace, string searchParameterToEdit)
        {
            htmlTable = (HtmlTable)UIAttachmentTypesWindoWindow.UIAttachmentTypesDocument.UITbl_gridAttachmentTyTable;
            htmlTable.WaitForControlReady();
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit));
            Mouse.Click(row.UIDeleteImage, new Point(3, 3));
        }
        
        /// <summary>
        /// Used to click archive against a specific entry on the attachment types grid 
        ///</summary>
        public void ClickArchiveFieldLink(BrowserWindow searchSpace, string searchParameterToEdit)
        {
            htmlTable = (HtmlTable)UIAttachmentTypesWindoWindow.UIAttachmentTypesDocument.UITbl_gridAttachmentTyTable;
            htmlTable.WaitForControlReady();
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit));
            Mouse.Click(row.UIArchiveImage, new Point(3, 3));
        }

        /// <summary>
        /// Used to validate values passed in expectedVal1 can be found in the grid
        ///</summary>
        public void ValidateAttachmentTypesTable(BrowserWindow searchSpace, string expectedVal1, bool shouldBePresent = true, bool isArchived = false)
        {
            List<String> tableHeaders = new List<string>();
            tableHeaders.Add("File extension");

            UIAttachmentTypesWindoWindow window = new UIAttachmentTypesWindoWindow();

            htmlTable = window.UIAttachmentTypesDocument.UITbl_gridAttachmentTyTable;

            Dictionary<string, int> cellLocations = GridHelpers.MapTableHeadersToLocation(htmlTable.Rows, tableHeaders);
            int rowLocation = GridHelpers.FindRowInGridForId(htmlTable, htmlTable.Rows, expectedVal1);
            
            if (!shouldBePresent)
            {
                Assert.AreEqual(-1, rowLocation);
                return;
            }

            ///Need to ensure that the rules are followed before asserting!
            UITestControlCollection cellCollection = ((HtmlRow)htmlTable.Rows[rowLocation]).Cells;
            UITestControlCollection collection = ((HtmlCell)cellCollection[1]).GetChildren();

            HtmlHyperlink hyperlink = (HtmlHyperlink)collection[0];
            UITestControlCollection imageCollection = hyperlink.GetChildren();
            HtmlImage image = (HtmlImage)imageCollection[0];

            
            if (isArchived)
            {
                Assert.AreEqual("Un-Archive", image.Title);
            }
            else
            {
                Assert.AreEqual("Archive", image.Title);
            }

            //Assert Name field
            HtmlCell cell = (HtmlCell)cellCollection[cellLocations["File extension"]];
            Assert.AreEqual(expectedVal1, cell.InnerText);
            
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
                        this.imgArchive.SearchProperties[HtmlImage.PropertyNames.Id] = null;
                        this.imgArchive.SearchProperties[HtmlImage.PropertyNames.Name] = null;
                        this.imgArchive.SearchProperties[HtmlImage.PropertyNames.Alt] = null;
                        this.imgArchive.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] = "/shared/images/icons/folder_lock.png";
                        this.imgArchive.FilterProperties[HtmlImage.PropertyNames.Class] = null;
                        this.imgArchive.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] = "title=Archive src=\"/shared/images/icons/";
                        this.imgArchive.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "1";
                        #endregion
                    }
                    return this.imgArchive;
                }
            }
        }
    }
}
