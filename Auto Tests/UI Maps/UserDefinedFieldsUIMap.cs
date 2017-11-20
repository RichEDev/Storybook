namespace Auto_Tests.UIMaps.UserDefinedFieldsUIMapClasses
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
    using System.Configuration;
    using SpendManagementLibrary;
    using System.Data.SqlClient;
    using System.Reflection;
    using Auto_Tests.Coded_UI_Tests.Spend_Management.System_Options.IP_Address_Filtering;
    using Auto_Tests.Tools;


    public partial class UserDefinedFieldsUIMap
    {
        private cUI_DataRow row;
        private HtmlTable table;

        public void ClickDeleteUdfField(BrowserWindow searchSpace, string searchParameterToRemove) 
        {
            table = (HtmlTable)UIUserdefinedFieldsWinWindow1.UIUserdefinedFieldsDocument.UITbl_gridFieldsTable;
            UITestControlCollection collection = table.Rows;

            row = new cUI_DataRow(searchSpace, GridHelpers.FindRowInGridForId(table,collection,searchParameterToRemove ));
            Mouse.Click(row.UIDeleteImage, new Point(5, 5));

            searchSpace.PerformDialogAction(BrowserDialogAction.Ok);  
        }

        public void ClickEditFieldLink(BrowserWindow  searchSpace, string searchParameterToEdit)
        {
            table = (HtmlTable)UIUserdefinedFieldsWinWindow1.UIUserdefinedFieldsDocument.UITbl_gridFieldsTable;
            UITestControlCollection collection = table.Rows;

            row = new cUI_DataRow(searchSpace, GridHelpers.FindRowInGridForId(table, collection, searchParameterToEdit));
            Mouse.Click(row.UIEditImage, new Point(0, 0));     
        }
    
        public void ValidateAddedUDF(BrowserWindow searchSpace, string searchParameterToValidate)
        {
            table = (HtmlTable)UIUserdefinedFieldsWinWindow1.UIUserdefinedFieldsDocument.UITbl_gridFieldsTable;
            UITestControlCollection collection = table.Rows;

            UITestControlCollection cellCollection = ((HtmlRow)table.Rows[GridHelpers.FindRowInGridForId(table, collection, searchParameterToValidate)]).Cells;
            HtmlCell cell = (HtmlCell)cellCollection[2];
            Assert.AreEqual(searchParameterToValidate, cell.InnerText);
        }

        public void ClickTableHeader(HtmlHyperlink sortableTableHeader)
        {
            Mouse.Click(sortableTableHeader, new Point(2, 2));
        }

        public void DoesTableContainCorrectElements(SortByColumn sortby, EnumHelper.TableSortOrder sortingOrder)
        {
            List<UserDefinedFieldsDTO> expectedData = getUserDefinedFiledsfromDatabase(sortby, sortingOrder);
            List<UserDefinedFieldsDTO> actualData = new List<UserDefinedFieldsDTO>();

            HtmlTable table = (HtmlTable)UIUserdefinedFieldsWinWindow1.UIUserdefinedFieldsDocument.UITbl_gridFieldsTable;
            UITestControlCollection collection = table.Rows;

            //Remove table header from list.
            collection.RemoveAt(0);
            //Ensure correct cound otherwise not point carrying on
            Assert.AreEqual(table.RowCount - 1, expectedData.Count);
            
            foreach (HtmlRow row in collection)
            {
                UITestControlCollection rowCollection = row.Cells;
                String displayName = ((HtmlCell)rowCollection[2]).InnerText != null ? ((HtmlCell)rowCollection[2]).InnerText : "";
                String description = ((HtmlCell)rowCollection[3]).InnerText != null ? ((HtmlCell)rowCollection[3]).InnerText : "";
                String fieldType = ((HtmlCell)rowCollection[4]).InnerText != null ? ((HtmlCell)rowCollection[4]).InnerText : "";

                object x = ((HtmlCell)rowCollection[5]).NativeElement;
                bool ischecked = (((mshtml.HTMLTableCellClass)(x)).innerHTML).Contains("CHECKED");
                String appliesTo = ((HtmlCell)rowCollection[6]).InnerText;
          
                String mandatory = ((HtmlCell)rowCollection[5]).InnerText;
                actualData.Add(new UserDefinedFieldsDTO(0, displayName, description, 0, ischecked, appliesTo));
            }

            IEnumerator<UserDefinedFieldsDTO> e1 = expectedData.GetEnumerator();
            IEnumerator<UserDefinedFieldsDTO> e2 = actualData.GetEnumerator();
            
            while (e1.MoveNext() && e2.MoveNext()) 
            { 
                    Assert.AreEqual(e1.Current, e2.Current); 
            } 
        }

        internal List<UserDefinedFieldsDTO> getUserDefinedFiledsfromDatabase(SortByColumn sortby, Auto_Tests.Tools.EnumHelper.TableSortOrder sortingOrder)
        {
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.expenses));
            return new UserDefinedFieldsDAO(db).GetAll(sortby, sortingOrder);
        }

       
        private class cUI_DataRow : HtmlRow
        {
            private HtmlImage mUIDeleteImage;
            private HtmlImage mUIEditImage;

            public cUI_DataRow(UITestControl searchLimitContainer, int rowId) : base(searchLimitContainer) 
            {
                this.SearchProperties[HtmlRow.PropertyNames.Id] = "tbl_gridFields";
                this.SearchProperties[HtmlRow.PropertyNames.Name] = null;
                this.FilterProperties[HtmlRow.PropertyNames.ControlDefinition] = "id=tbl_gridFields";
                this.FilterProperties[HtmlRow.PropertyNames.RowIndex] = Convert.ToString(rowId);
                this.FilterProperties[HtmlRow.PropertyNames.Class] = null;
                this.FilterProperties[HtmlRow.PropertyNames.TagInstance] = "2";
                this.WindowTitles.Add("User Defined Fields");
            }

            public HtmlImage UIDeleteImage
            {
                get
                {
                    if ((this.mUIDeleteImage == null))
                    {
                        this.mUIDeleteImage = new HtmlImage(this);
                        #region Search Criteria
                        this.mUIDeleteImage.SearchProperties[HtmlImage.PropertyNames.Id] = null;
                        this.mUIDeleteImage.SearchProperties[HtmlImage.PropertyNames.Name] = null;
                        this.mUIDeleteImage.SearchProperties[HtmlImage.PropertyNames.Alt] = "Delete";
                        this.mUIDeleteImage.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] = "/shared/images/icons/delete2.png";
                        this.mUIDeleteImage.FilterProperties[HtmlImage.PropertyNames.Src] = "http://fwtest.sel-expenses.com/shared/images/icons/delete2.png";
                        this.mUIDeleteImage.FilterProperties[HtmlImage.PropertyNames.Class] = null;
                        this.mUIDeleteImage.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] = "alt=Delete src=\"/shared/images/icons/del";
                        this.mUIDeleteImage.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "2";
                        this.mUIDeleteImage.WindowTitles.Add("User Defined Fields");
                        #endregion
                    }
                    return this.mUIDeleteImage;
                }
            }

            public HtmlImage UIEditImage
            {
                get
                {
                    if ((this.mUIEditImage == null))
                    {
                        this.mUIEditImage = new HtmlImage(this);
                        #region Search Criteria
                        this.mUIEditImage.SearchProperties[HtmlImage.PropertyNames.Id] = null;
                        this.mUIEditImage.SearchProperties[HtmlImage.PropertyNames.Name] = null;
                        this.mUIEditImage.SearchProperties[HtmlImage.PropertyNames.Alt] = "Edit";
                        this.mUIEditImage.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] = "/shared/images/icons/edit.png";
                        this.mUIEditImage.FilterProperties[HtmlImage.PropertyNames.Src] = "http://fwtest.sel-expenses.com/shared/images/icons/edit.png";
                        this.mUIEditImage.FilterProperties[HtmlImage.PropertyNames.Class] = null;
                        this.mUIEditImage.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] = "alt=Edit src=\"/shared/images/icons/edit.";
                        this.mUIEditImage.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "1";
                        this.mUIEditImage.WindowTitles.Add("User Defined Fields");
                        #endregion
                    }
                    return this.mUIEditImage;
                }
            }

        }

        
    }
}
