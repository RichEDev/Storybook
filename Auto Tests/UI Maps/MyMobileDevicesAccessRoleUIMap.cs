using Auto_Tests.UIMaps;
namespace Auto_Tests.UIMaps.MyMobileDevicesAccessRoleUIMapClasses
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
    
    
    public partial class MyMobileDevicesAccessRoleUIMap
    {
        private cTableDataRow row;
        private HtmlTable htmlTable;
        public void ClickEditFieldLink(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)UIYoudonothavepermissiWindow.UIAccessRolesDocument.UITbl_accessRolesGridTable;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit));
            Mouse.Click(row.UIEditImage, new Point(3, 3));
        }

        public void ClickAddViewAccessRole(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)UIYoudonothavepermissiWindow.UIAccessRoleMyMobileDeDocument.UICtl00_contentmain_tcTable;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit,0));
            Mouse.Click(row.UIViewTick, new Point(3, 3));
        }

        private class cTableDataRow : HtmlRow
        {
            private HtmlImage imgDelete;
            private HtmlImage imgEdit;
            private HtmlCheckBox tickView;

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


            public HtmlCheckBox UIViewTick
            {
                get
                {
                    if ((this.tickView == null))
                    {
                        this.tickView = new HtmlCheckBox(this);
                        #region Search Criteria
                        ///this.tickView.SearchProperties[HtmlImage.PropertyNames.Id] = "ctl00_contentmain_tcElementAccess_tb2_chk_element_view_165";
                        this.tickView.SearchProperties[HtmlImage.PropertyNames.Type] = "checkbox";
                        this.tickView.FilterProperties[HtmlImage.PropertyNames.ClassName] = "HtmlCheckBox";
                        #endregion
                    }
                    return this.tickView;
                }
            }
        }
    }
}
