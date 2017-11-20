namespace Auto_Tests.UIMaps.ESRAssignmentsUIMapClasses
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
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

    /// <summary>
    /// The assignments ui map.
    /// </summary>
    public partial class ESRAssignmentsUIMap
    {
        /// <summary>
        /// The row.
        /// </summary>
        private cTableDataRow row;

        /// <summary>
        /// The header.
        /// </summary>
        /// <param name="htmlTable">
        /// The html table.
        /// </param>
        /// <returns>
        /// The <see cref="SortedList"/>.
        /// </returns>
        public SortedList<int, string> GetTableHeaders(ref HtmlTable htmlTable)
        {
            var tableHeaderIndex = new SortedList<int, string>();

            UITestControlCollection rowCells = ((HtmlRow)htmlTable.Rows[0]).Cells;
            foreach (HtmlHeaderCell cell in rowCells)
            {
                if (cell.InnerText != null)
                {
                    tableHeaderIndex.Add(rowCells.IndexOf(cell), cell.InnerText);
                }
            }

            return tableHeaderIndex;
        }

        internal void ValidateArchiveGridRowStatus(HtmlTable htmlTable, string searchParameterToArchive, bool archive = true)
        {
            UITestControlCollection collection = htmlTable.Rows;
            var tableHeaderIndex = this.GetTableHeaders(ref htmlTable);
            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToArchive, tableHeaderIndex.First().Key));
            if (archive)
            {
                Assert.IsTrue(row.UIUnarchiveImage.HelpText.Equals("Un-Archive"));
            }
            else
            {
                Assert.IsTrue(row.UIUnarchiveImage.HelpText.Equals("Archive"));
            }
        }
        
        /// <summary>
        /// The return id from grid.
        /// </summary>
        /// <param name="htmlTable">
        /// The html table.
        /// </param>
        /// <param name="KeyField">
        /// The key field.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ReturnIdFromGrid(HtmlTable htmlTable, string KeyField)
        {

            UITestControlCollection collection = htmlTable.Rows;
            var tableHeaderIndex = this.GetTableHeaders(ref htmlTable);
            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, KeyField, tableHeaderIndex.First().Key));
            int keyFieldId = 0;
            int.TryParse(row.UIArchiveImage.Container.FriendlyName.Replace(htmlTable.Id + "_", string.Empty), out keyFieldId);
            return keyFieldId;
        }

        /// <summary>
        /// The click archive grid row.
        /// </summary>
        /// <param name="htmlTable">
        /// The html table.
        /// </param>
        /// <param name="searchParameterToArchive">
        /// The search parameter to archive.
        /// </param>
        internal void ClickArchiveGridRow(HtmlTable htmlTable, string searchParameterToArchive)
        {
            UITestControlCollection collection = htmlTable.Rows;
            var tableHeaderIndex = this.GetTableHeaders(ref htmlTable);
            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToArchive, tableHeaderIndex.First().Key));
            Mouse.Click(row.UIArchiveImage, new Point(3, 3));
        }

        /// <summary>
        /// The click edit grid row.
        /// </summary>
        /// <param name="htmlTable">
        /// The html table.
        /// </param>
        /// <param name="searchParameterToEdit">
        /// The search parameter to edit.
        /// </param>
        public void ClickEditGridRow(HtmlTable htmlTable, string searchParameterToEdit)
        {
            UITestControlCollection collection = htmlTable.Rows;
            var tableHeaderIndex = this.GetTableHeaders(ref htmlTable);
            row = new cTableDataRow( htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, tableHeaderIndex.First().Key));
            Mouse.Click(row.UIEditImage, new Point(3, 3));
        }

        /// <summary>
        /// The click delete grid row.
        /// </summary>
        /// <param name="htmlTable">
        /// The html table.
        /// </param>
        /// <param name="searchParameterToEdit">
        /// The search parameter to edit.
        /// </param>
        public void ClickDeleteGridRow(HtmlTable htmlTable, string searchParameterToEdit)
        {
            UITestControlCollection collection = htmlTable.Rows;
            var tableHeaderIndex = this.GetTableHeaders(ref htmlTable);
            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, tableHeaderIndex.First().Key));
            Mouse.Click(row.UIDeleteImage, new Point(3, 3));
        }

        internal void ClickCheckGridRow(HtmlTable htmlTable, string searchParameterToSelect, int selectionControlId)
        {
            UITestControlCollection collection = htmlTable.Rows;
            var tableHeaderIndex = this.GetTableHeaders(ref htmlTable);
            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToSelect, tableHeaderIndex.First().Key));
            row.UICheckBox.SearchProperties[HtmlCheckBox.PropertyNames.Value] = selectionControlId.ToString();
            Mouse.Click(row.UICheckBox, new Point(3, 3));
        }

        public void ClickMembersGridRow(HtmlTable htmlTable, string searchParameterToClick)
        {
            UITestControlCollection collection = htmlTable.Rows;
            var tableHeaderIndex = this.GetTableHeaders(ref htmlTable);
            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToClick, tableHeaderIndex.First().Key));
            Mouse.Click(row.UITeamImage, new Point(3, 3));
        }

        /// <summary>
        /// The validate grid.
        /// </summary>
        /// <param name="htmlTable">
        /// The html table.
        /// </param>
        /// <param name="expectedValues">
        /// The expected values.
        /// </param>
        /// <param name="shouldBePresent">
        /// The should be present.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// </exception>
        public void ValidateGrid(HtmlTable htmlTable, List<string> expectedValues, bool shouldBePresent = true)
        {
            UITestControlCollection collection = htmlTable.Rows;
            var tableHeaderIndex = this.GetTableHeaders(ref htmlTable);
            int rowLocation = GridHelpers.FindRowInGridForId(htmlTable, htmlTable.Rows, expectedValues[0], tableHeaderIndex.First().Key);
            if (!shouldBePresent)
            {
                Assert.AreEqual(-1, rowLocation);
            }
            else
            {
                UITestControlCollection cellCollection =((HtmlRow)htmlTable.Rows[rowLocation]).Cells;

                if (tableHeaderIndex.Count == expectedValues.Count)
                {
                    foreach (KeyValuePair<int, string> tableHeader in tableHeaderIndex)
                    {
                        HtmlCell cell = (HtmlCell)cellCollection[tableHeader.Key];

                        if (expectedValues[tableHeaderIndex.IndexOfKey(tableHeader.Key)].ToLower() != "true" && expectedValues[tableHeaderIndex.IndexOfKey(tableHeader.Key)].ToLower() != "false" && expectedValues[tableHeaderIndex.IndexOfKey(tableHeader.Key)].ToLower() != "yes" && expectedValues[tableHeaderIndex.IndexOfKey(tableHeader.Key)].ToLower() != "no")
                        {
                            Assert.AreEqual(expectedValues[tableHeaderIndex.IndexOfKey(tableHeader.Key)].Trim(), cell.InnerText.Trim());
                        }
                        else
                        {
                            object nativeelement = cell.NativeElement;
                            if (((mshtml.HTMLTableCellClass)nativeelement).innerHTML.ToLower().Contains("checked") || ((mshtml.HTMLTableCellClass)nativeelement).innerHTML.ToLower().Contains("yes"))
                            {
                                Assert.AreEqual(expectedValues[tableHeaderIndex.IndexOfKey(tableHeader.Key)].ToLower(), "true");
                            }
                            else
                            {
                                Assert.AreEqual(expectedValues[tableHeaderIndex.IndexOfKey(tableHeader.Key)].ToLower(), "false");
                            }
                        }
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException("Grid Header count isnt equal to the expected row count");
                }
            }
        }

        private class cTableDataRow : HtmlRow
        {
            private HtmlImage imgDelete;

            private HtmlImage imgEdit;

            private HtmlImage imgArchive;

            private HtmlImage teamImage;

            private HtmlImage imgUnarchive;

            private HtmlCheckBox CheckBox;

            public cTableDataRow(UITestControl searchLimitContainer, int rowId)
                : base(searchLimitContainer)
            {
                this.SearchProperties[HtmlRow.PropertyNames.Name] = null;
                this.FilterProperties[HtmlRow.PropertyNames.RowIndex] = Convert.ToString(rowId);
                this.FilterProperties[HtmlRow.PropertyNames.TagInstance] = "2";
            }

            public HtmlImage UITeamImage
            {
                get
                {
                    if ((this.teamImage == null))
                    {
                        this.teamImage = new HtmlImage(this);

                        #region Search Criteria

                        this.teamImage.SearchProperties[HtmlImage.PropertyNames.Alt] = "Members";
                        this.teamImage.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] = "/shared/images/icons/users3.png";
                        this.teamImage.FilterProperties.Add(new PropertyExpression(HtmlImage.PropertyNames.Src, "sel-expenses.com/shared/images/icons/users3.png", PropertyExpressionOperator.Contains));
                        this.teamImage.FilterProperties[HtmlImage.PropertyNames.LinkAbsolutePath] = "ShowTeamMembers(172);";
                        this.teamImage.FilterProperties[HtmlImage.PropertyNames.Href] = "javascript:ShowTeamMembers(172);";
                        this.teamImage.FilterProperties[HtmlImage.PropertyNames.Class] = "";
                        //this.teamImage.FilterProperties.Add(new PropertyExpression(HtmlImage.PropertyNames.ControlDefinition, "alt=Members", PropertyExpressionOperator.Contains));
                        //this.imgDelete.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] = "alt=Members src=\"/shared/images/icons/us";
                        this.teamImage.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "3";

                        #endregion
                    }
                    return this.teamImage;
                }
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
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] =
                            "/shared/images/icons/delete2.png";
                        // this.imgDelete.FilterProperties[HtmlImage.PropertyNames.Src] = "http://fwtest.sel-expenses.com/shared/images/icons/delete2.png";
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.Class] = null;
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] =
                            "alt=Delete src=\"/shared/images/icons/del";
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
                        this.imgEdit.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] =
                            "/shared/images/icons/edit.png";
                        //this.imgEdit.FilterProperties[HtmlImage.PropertyNames.Src] = "http://fwtest.sel-expenses.com/shared/images/icons/edit.png";
                        this.imgEdit.FilterProperties[HtmlImage.PropertyNames.Class] = null;
                        this.imgEdit.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] =
                            "alt=Edit src=\"/shared/images/icons/edit.";
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
                        this.imgArchive.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] =
                            "/shared/images/icons/folder_lock.png";
                        this.imgArchive.FilterProperties.Add(
                            new PropertyExpression(
                                HtmlImage.PropertyNames.Src,
                                "sel-expenses.com/shared/images/icons/folder_lock.png",
                                PropertyExpressionOperator.Contains));
                        this.imgArchive.FilterProperties[HtmlImage.PropertyNames.Class] = string.Empty;
                        this.imgArchive.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] =
                            "title=Archive src=\"/shared/images/icons/folder_lock.png";
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
                        this.imgUnarchive.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] =
                            "/shared/images/icons/folder_into.png";
                        this.imgUnarchive.FilterProperties.Add(
                            new PropertyExpression(
                                HtmlImage.PropertyNames.Src,
                                "sel-expenses.com/shared/images/icons/folder_into.png",
                                PropertyExpressionOperator.Contains));
                        this.imgUnarchive.FilterProperties[HtmlImage.PropertyNames.Class] = string.Empty;
                        this.imgUnarchive.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] =
                            "title=Archive src=\"/shared/images/icons/folder_into.png";
                        this.imgUnarchive.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "1";

                        #endregion
                    }
                    return this.imgUnarchive;
                }
            }

            public HtmlCheckBox UICheckBox
            {
                get
                {
                    if ((this.CheckBox == null))
                    {
                        this.CheckBox = new HtmlCheckBox(this);
                        #region Search Criteria
                        this.CheckBox.SearchProperties[HtmlCheckBox.PropertyNames.Id] = null;
                        this.CheckBox.SearchProperties[HtmlCheckBox.PropertyNames.Name] = "selectgridTeamEmployees";
                        this.CheckBox.SearchProperties[HtmlCheckBox.PropertyNames.Value] = "21403";
                        this.CheckBox.SearchProperties[HtmlCheckBox.PropertyNames.LabeledBy] = null;
                        this.CheckBox.FilterProperties[HtmlCheckBox.PropertyNames.Title] = null;
                        this.CheckBox.FilterProperties[HtmlCheckBox.PropertyNames.Class] = null;
                        // this.CheckBox.FilterProperties[HtmlCheckBox.PropertyNames.ControlDefinition] = "type=checkbox value=21403 name=selectgri";
                        this.CheckBox.FilterProperties[HtmlCheckBox.PropertyNames.ControlDefinition] = "type=checkbox value=" + this.CheckBox.SearchProperties[HtmlCheckBox.PropertyNames.Value] + "name=selectgri";
                        // this.CheckBox.FilterProperties[HtmlCheckBox.PropertyNames.TagInstance] = "14";
                        #endregion
                    }
                    return this.CheckBox;
                }
            }
        }
    }
}
