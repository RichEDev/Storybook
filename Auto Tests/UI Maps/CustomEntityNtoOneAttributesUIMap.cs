namespace Auto_Tests.UIMaps.CustomEntityNtoOneAttributesUIMapClasses
{
    using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
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
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;


    public partial class CustomEntityNtoOneAttributesUIMap
    {
        private cTableDataRow row;
        private HtmlTable htmlTable;
        private static SharedMethodsUIMap cSharedMethods = new SharedMethodsUIMap();

        public void ClickEditFieldLink(BrowserWindow searchSpace, string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument4.UITbl_gridAttributesTable;
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit));
            Mouse.Click(row.UIEditImage, new Point(3, 3));
        }

        /// <summary>
        /// Used to click the Delete icon on the attributes grid
        ///</summary>
        public void ClickDeleteFieldLink(BrowserWindow searchSpace, string searchParameterToEdit)
        {
            htmlTable = (HtmlTable)UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument4.UITbl_gridAttributesTable;
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit));
            Mouse.Click(row.UIDeleteImage, new Point(3, 3));
        }

        /// <summary>
        /// Used to validate that an attribute has been deleted from the grid
        ///</summary>
        public void ValidateAttributeDeletion(string searchParameterDeletedPrimaryKey)
        {
            htmlTable = (HtmlTable)UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument4.UITbl_gridAttributesTable;
            UITestControlCollection collection = htmlTable.Rows;

            Assert.AreEqual(-1, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterDeletedPrimaryKey));
        }

        /// <summary>
        /// Used to validate values passed in can be found in the grid
        ///</summary>
        public void ValidateAttributesGrid(BrowserWindow searchSpace, string expectedVal1, string expectedVal2, string expectedVal3, string expectedVal4, bool CanEdit = true, bool CanDelete = true)
        {
            List<String> tableHeaders = new List<string>();
            tableHeaders.Add("Display Name");
            tableHeaders.Add("Description");
            tableHeaders.Add("Type");
            tableHeaders.Add("Used for Audit");

            UICustomEntityCustomEnWindow window = new UICustomEntityCustomEnWindow();

            htmlTable = window.UICustomEntityCustomEnDocument4.UITbl_gridAttributesTable;

            Dictionary<string, int> cellLocations = GridHelpers.MapTableHeadersToLocation(htmlTable.Rows, tableHeaders);

            UITestControlCollection cellCollection = ((HtmlRow)htmlTable.Rows[GridHelpers.FindRowInGridForId(htmlTable, htmlTable.Rows, expectedVal1)]).Cells;


            ///Assert edit icon not present within the grid for basic attributes
            ///Need to ensure that the rules are followed before asserting!
            HtmlCell imageCell = (HtmlCell)cellCollection[0];
            UITestControlCollection collectin = imageCell.GetChildren();

            ///String iconImage = (String)cellCollection[0].;

            if (CanEdit)
            {
                ///Assert edit icon is present
                Assert.AreEqual(1, collectin.Count);
            }
            else
            {
                ///Assert that the edit icon is not present
                Assert.AreEqual(0, collectin.Count);
            }

            imageCell = (HtmlCell)cellCollection[1];
            if (CanDelete)
            {
                ///Assert delete icon is present
                Assert.AreEqual(1, collectin.Count);
            }
            else
            {
                ///Assert that the delete icon is not present
                Assert.AreEqual(0, collectin.Count);
            }

            ///Assert attributes field
            HtmlCell cell = (HtmlCell)cellCollection[cellLocations["Display Name"]];
            Assert.AreEqual(expectedVal1, cell.InnerText);

            cell = (HtmlCell)cellCollection[cellLocations["Description"]];
            Assert.AreEqual(expectedVal2, cell.InnerText);

            cell = (HtmlCell)cellCollection[cellLocations["Type"]];
            Assert.AreEqual(expectedVal3, cell.InnerText);

            cell = (HtmlCell)cellCollection[cellLocations["Used for Audit"]];
            object nativeelement = cell.NativeElement;
            bool ischecked = (((mshtml.HTMLTableCellClass)(nativeelement)).innerHTML).Contains("CHECKED");
            Assert.AreEqual(expectedVal4, ischecked.ToString());
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
                        this.imgEdit.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] = "alt=Edit src=\"/shared/images/icons/edit.png";
                        this.imgEdit.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "1";
                        #endregion
                    }
                    return this.imgEdit;
                }
            }
        }

        /// <summary>
        /// PressSave
        /// </summary>
        public void PressSave()
        {
            cSharedMethods.SetFocusOnControlAndPressEnter(UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UISaveButton);
        }

        /// <summary>
        /// PressCancel
        /// </summary>
        public void PressCancel()
        {
            cSharedMethods.SetFocusOnControlAndPressEnter(UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICancelButton);
        }

        /// <summary>
        /// PressSaveLookupFieldButton
        /// </summary>
        public void PressSaveLookupFieldButton()
        {
            cSharedMethods.SetFocusOnControlAndPressEnter(UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UILookupFieldToMatchSaveButton);
        }

        /// <summary>
        /// PressCancelSavingLookupFieldButton
        /// </summary>
        public void PressCancelSavingLookupFieldButton()
        {
            cSharedMethods.SetFocusOnControlAndPressEnter(UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UILookupFieldToMatchCancelButton);
        }
    }
}
