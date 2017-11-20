namespace Auto_Tests.UIMaps.CustomEntityAttributesUIMapClasses
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
    using Auto_Tests.Tools;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
    
    public partial class CustomEntityAttributesUIMap
    {
        private cTableDataRow row;
        private HtmlTable htmlTable;
        private static SharedMethodsUIMap cSharedMethods = new SharedMethodsUIMap();

        public void ClickEditFieldLink(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument2.UITbl_gridAttributesTable;
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit));
            Mouse.Click(row.UIEditImage, new Point(3, 3));
        }

        /// <summary>
        /// Used to click the Delete icon on the grid
        ///</summary>
        public void ClickDeleteFieldLink(string searchParameterToEdit)
        {
            htmlTable = (HtmlTable)UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument2.UITbl_gridAttributesTable;
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit));
            Mouse.Click(row.UIDeleteImage, new Point(3, 3));
        }

        /// <summary>
        /// Used to validate that an attribute has been deleted from the grid
        ///</summary>
        public void ValidateAttributeDeletion(string searchParameterDeletedPrimaryKey)
        {
            htmlTable = (HtmlTable)UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument2.UITbl_gridAttributesTable;
            UITestControlCollection collection = htmlTable.Rows;

            Assert.AreEqual(-1, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterDeletedPrimaryKey));
        }

        /// <summary>
        /// Used to validate values passed in can be found in the grid
        ///</summary>
        public void ValidateAttributesGrid(string expectedVal1, string expectedVal2, string expectedVal3, string expectedVal4, bool CanEdit = true, bool CanDelete = true, bool shouldBePresent = true)
        {
            List<String> tableHeaders = new List<string>();
            tableHeaders.Add("Display Name");
            tableHeaders.Add("Description");
            tableHeaders.Add("Type");
            tableHeaders.Add("Used for Audit");

            UICustomEntityCustomEnWindow window = new UICustomEntityCustomEnWindow();

            htmlTable = window.UICustomEntityCustomEnDocument2.UITbl_gridAttributesTable;

            Dictionary<string, int> cellLocations = GridHelpers.MapTableHeadersToLocation(htmlTable.Rows, tableHeaders);
            int rowLocation = GridHelpers.FindRowInGridForId(htmlTable, htmlTable.Rows, expectedVal1);
           

            if (!shouldBePresent)
            {
                Assert.AreEqual(-1, rowLocation);
            }
            else
            {
                UITestControlCollection cellCollection = ((HtmlRow)htmlTable.Rows[rowLocation]).Cells;
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
        public void VerifyCorrectSortingOrderForTable(SortAttributesByColumn sortby, EnumHelper.TableSortOrder sortingOrder, int entityid, ProductType executingProduct)
        {
            List<CustomEntitiesUtilities.CustomEntityAttribute> expectedData = getAttributesOrderFromDatabase(sortby, sortingOrder, entityid, executingProduct);
            List<CustomEntitiesUtilities.CustomEntityAttribute> actualData = new List<CustomEntitiesUtilities.CustomEntityAttribute>();

            HtmlTable table = (HtmlTable)UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.UITbl_gridAttributesTable;
            UITestControlCollection collection = table.Rows;

            //Remove table header from list
            collection.RemoveAt(0);
            //Ensure count of data in table is the same with count of data in database
            Assert.AreEqual(table.RowCount - 1, expectedData.Count);

            foreach (HtmlRow row in collection)
            {
                UITestControlCollection rowCollection = row.Cells;
                string displayName = ((HtmlCell)rowCollection[2]).InnerText != null ? ((HtmlCell)rowCollection[2]).InnerText : "";
                string description = ((HtmlCell)rowCollection[3]).InnerText != null ? ((HtmlCell)rowCollection[3]).InnerText : "";
                string fieldType = ((HtmlCell)rowCollection[4]).InnerText != null ? ((HtmlCell)rowCollection[4]).InnerText : "";

                object x = ((HtmlCell)rowCollection[5]).NativeElement;
                bool ischecked = (((mshtml.HTMLTableCellClass)(x)).innerHTML).Contains("CHECKED");

                if (fieldType == "Date/Time")
                {
                    fieldType = "DateTime";
                }

                actualData.Add(new CustomEntitiesUtilities.CustomEntityAttribute(displayName, description, (FieldType)Enum.Parse(typeof(FieldType), fieldType, true), ischecked));
            }

            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.AreEqual(actualData[i].DisplayName, expectedData[i].DisplayName);
                Assert.AreEqual(actualData[i]._description, expectedData[i]._description);
                Assert.AreEqual(actualData[i]._fieldType, expectedData[i]._fieldType);
                Assert.AreEqual(actualData[i]._isAuditIdenity, expectedData[i]._isAuditIdenity);
            }
        }

        /// <summary>
        /// Used to get the order of the data from the database
        ///</summary>
        internal List<CustomEntitiesUtilities.CustomEntityAttribute> getAttributesOrderFromDatabase(SortAttributesByColumn sortby, EnumHelper.TableSortOrder sortingOrder, int entityid, ProductType executingProduct)
        {
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));
            return new AttributesDAO(db).GetCorrectSortingOrderFromDB(sortby, sortingOrder, entityid);
        }

        /// <summary>
        ///  Validate extra fields follow standards when Type Text is selected
        /// </summary>
        public void ValidateFieldsWhenTypeTextIsSelected()
        {
            SelectTextInTypeField();
            ValidateFormatFieldInTypeText();
            ValidateFormatFieldDropdownList();
            ValidateMaximumLengthFieldInTypeText();
            SelectSingleLineInFormatField();
            ValidateDisplayWidthFieldInTypeText();
            ValidateDisplayWidthDropdownListInText();
        }

        /// <summary>
        ///  Validate extra fields follow standards when Type Decimal is selected
        /// </summary>
        public void ValidateFieldsWhenTypeDecimalIsSelected()
        {
            SelectTextInTypeFieldParams.UITypeComboBoxSelectedItem = "Decimal";
            SelectTextInTypeField();
            ValidatePrecisionField();
        }

        /// <summary>
        ///  Validate extra fields follow standards when Type Yes/No is selected
        /// </summary>
        public void ValidateFieldsWhenTypeYesNoIsSelected()
        {
            SelectTextInTypeFieldParams.UITypeComboBoxSelectedItem = "Yes/No";
            SelectTextInTypeField();
            ValidateDefaultValueField();
            ValidateDefaultValueDropdownList();
        }

        /// <summary>
        /// Validate extra fields follow standards when Type List is selected
        /// </summary>
        public void ValidateFieldsWhenTypeListIsSelected()
        {
            SelectTextInTypeFieldParams.UITypeComboBoxSelectedItem = "List";
            SelectTextInTypeField();
            ValidateListItemsField();
            ValidateDisplayWidthFieldInTypeList();

            HtmlImage plusIcon = UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.UIListitemsPane.UINewListItemImage;
            string plusIconExpectedText = "New List Item";
            Assert.AreEqual(plusIconExpectedText, plusIcon.HelpText);

            HtmlImage editIcon = UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.UIListitemsPane.UIEditListItemImage;
            string editIconExpectedText = "Edit List Item";
            Assert.AreEqual(editIconExpectedText, editIcon.HelpText);

            HtmlImage deleteIcon = UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.UIListitemsPane.UIDeleteListItemImage;
            string deleteIconExpectedText = "Delete List Item";
            Assert.AreEqual(deleteIconExpectedText, deleteIcon.HelpText);
        }

        /// <summary>
        /// Validate extra fields follow standards when Type Date is selected
        /// </summary>
        public void ValidateFieldsWhenTypeDateIsSelected()
        {
            SelectTextInTypeFieldParams.UITypeComboBoxSelectedItem = "Date";
            SelectTextInTypeField();
            ValidateFormatFieldInTypeDate();
            ValidateFormatFieldDropdownListInDate();
        }

        /// <summary>
        /// Validate extra fields follow standards when Type Large Text is selected
        /// </summary>
        public void ValidateFieldsWhenTypeLargeTextIsSelected()
        {
            SelectTextInTypeFieldParams.UITypeComboBoxSelectedItem = "Large Text";
            SelectTextInTypeField();
            ValidateFormatFieldInTypeLargeText();
            ValidateFormatFieldDropdownListInLargeText();
            ValidateMaximumLengthFieldInTypeLargeText();
        }

        /// <summary>
        /// Validate extra fields follow standards when Type Comment is selected
        /// </summary>
        public void ValidateFieldsWhenTypeCommentIsSelected()
        {
            SelectTextInTypeFieldParams.UITypeComboBoxSelectedItem = "Comment";
            SelectTextInTypeField();
            ValidateCommentAdviceTextField();
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
        /// ClickSaveBtnForListItem
        /// </summary>
    //public void SaveListItem()
    //{
    //        #region Variable Declarations
    //        HtmlSpan uISavePane = this.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument7.UISavePane;
    //        #endregion

    //        // Click 'save' pane
    //        Mouse.Click(uISavePane, new Point(28, 12));
    //        // Keyboard.SendKeys("{ENTER}");
    //}

        /// <summary>
        /// PressCancel
        /// </summary>
        public void PressCancel()
        {
            cSharedMethods.SetFocusOnControlAndPressEnter(UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICancelButton);
        }

        /// <summary>
        /// PressSaveAttribute
        /// </summary>
        public void PressSaveAttribute()
        {
            cSharedMethods.SetFocusOnControlAndPressEnter(UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UISaveButton);
        }

        /// <summary>
        /// ClickSaveBtnForEditingListItem
        /// </summary>
        public void ClickSaveBtnForEditingListItem()
        {
            cSharedMethods.SetFocusOnControlAndPressEnter(UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIListItemCancelButton);
        }
    }

    /// <summary>
    /// Abstract base for shared controls on the attributes page
    /// </summary>
    internal abstract class CustomEntityAttributesBase
    {
        internal HtmlEdit DisplayNameTxt { get; private set; }
        internal cHtmlTextAreaWrapper DescriptionTxt { get; private set; }
        internal HtmlCheckBox IsAudit { get; private set; }
        internal HtmlComboBox TypeComboBx { get; private set; }
        internal HtmlCheckBox IsUnique { get; private set; }

        protected CustomEntityAttributesUIMap _cCustomEntitiesAttributesMethods;
        protected SharedMethodsUIMap _cSharedMethods;
        protected ControlLocator<HtmlControl> _ControlLocator { get; private set; }
        protected FieldType _fieldType;

        internal CustomEntityAttributesBase(CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods, SharedMethodsUIMap cSharedMethods, FieldType type)
        {
            _cCustomEntitiesAttributesMethods = cCustomEntitiesAttributesMethods;
            _cSharedMethods = cSharedMethods;
            _fieldType = type;
            _ControlLocator = new ControlLocator<HtmlControl>();
            FindControls();
        }

        /// <summary>
        /// Locates controls that are declared within this class
        /// </summary>
        internal virtual void FindControls()
        {
            DisplayNameTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_txtattributename", new HtmlEdit(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            DescriptionTxt = (cHtmlTextAreaWrapper)_ControlLocator.findControl("ctl00_contentmain_txtattributedescription", new cHtmlTextAreaWrapper(_cSharedMethods.ExtractHtmlMarkUpFromPage(), "ctl00_contentmain_txtattributedescription", _cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            IsAudit = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_chkAuditIdentifier", new HtmlCheckBox(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            IsUnique = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_chkUnique", new HtmlCheckBox(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            TypeComboBx = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_cmbattributetype", new HtmlComboBox(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
        }
    }

    /// <summary>
    /// Builds upon the attributes base class with more shared html objects
    /// </summary>
    internal class CustomEntityBaseType : CustomEntityAttributesBase
    {
        internal HtmlCheckBox IsMandatory { get; private set; }
        internal cHtmlTextAreaWrapper TooltipTxt { get; private set; }

        internal CustomEntityBaseType(CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods, FieldType type) : base(cCustomEntitiesAttributesMethods, new SharedMethodsUIMap(), type) { }

        internal override void FindControls()
        {
            base.FindControls();
            IsMandatory = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_chkattributemandatory", new HtmlCheckBox(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            TooltipTxt = (cHtmlTextAreaWrapper)_ControlLocator.findControl("ctl00_contentmain_txtattributetooltip", new cHtmlTextAreaWrapper(_cSharedMethods.ExtractHtmlMarkUpFromPage(), "ctl00_contentmain_txtattributetooltip", _cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
        }
    }

    /// <summary>
    ///  Summary Attributes
    /// </summary>
    internal class CustomEntityAttributeSummary : CustomEntityAttributesBase
    {
        internal cHtmlTextAreaWrapper CommentTxt { get; private set; }

        internal CustomEntityAttributeSummary(CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods) : base(cCustomEntitiesAttributesMethods, new SharedMethodsUIMap(), FieldType.OTMSummary) { }

        internal override void FindControls()
        {
            base.FindControls();
            CommentTxt = (cHtmlTextAreaWrapper)_ControlLocator.findControl("ctl00_contentmain_txtAdviceText", new cHtmlTextAreaWrapper(_cSharedMethods.ExtractHtmlMarkUpFromPage(), "ctl00_contentmain_txtAdviceText", _cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
        }
    }

    /// <summary>
    /// Text Attributes
    /// </summary>
    internal class CustomEntityAttributeText : CustomEntityBaseType
    {
        internal HtmlComboBox FormatComboBx { get; private set; }
        internal HtmlEdit MaxLengthTxt { get; private set; }
        internal HtmlEdit LargeMaxLengthTxt { get; private set; }
        internal HtmlComboBox DisplayWidthComboBx { get; private set; }
        internal HtmlComboBox LargeFormatComboBx { get; private set; }

        internal CustomEntityAttributeText(CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods) : base(cCustomEntitiesAttributesMethods, FieldType.Text) { }


        internal override void FindControls()
        {
            base.FindControls();
            MaxLengthTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_txtmaxlength", new HtmlEdit(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            LargeMaxLengthTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_txtmaxlengthlarge", new HtmlEdit(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            FormatComboBx = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_cmbtextformat", new HtmlComboBox(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            LargeFormatComboBx = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_cmbtextformatlarge", new HtmlComboBox(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            DisplayWidthComboBx = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_cmbDisplayWidth", new HtmlComboBox(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
        }
    }

    /// <summary>
    /// Decimal attributes
    /// </summary>
    internal class CustomEntityAttributeDecimal : CustomEntityBaseType
    {
        internal HtmlEdit PrecisionTxt { get; private set; }

        internal CustomEntityAttributeDecimal(CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods) : base(cCustomEntitiesAttributesMethods, FieldType.Number) { }

        internal override void FindControls()
        {
            base.FindControls();
            PrecisionTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_txtprecision", new HtmlEdit(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
        }
    }

    //YES No check box attributes
    internal class CustomEntityAttributeYesNo : CustomEntityBaseType
    {
        internal HtmlComboBox DefaultValue { get; private set; }

        internal CustomEntityAttributeYesNo(CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods) : base(cCustomEntitiesAttributesMethods, FieldType.TickBox) { }

        internal override void FindControls()
        {
            base.FindControls();
            DefaultValue = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_cmbdefaultvalue", new HtmlComboBox(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
        }
    }

    /// <summary>
    /// Date attributes
    /// </summary>
    internal class CustomEntityAttributeDate : CustomEntityBaseType
    {
        internal HtmlComboBox FormatCbx { get; private set; }

        internal CustomEntityAttributeDate(CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods) : base(cCustomEntitiesAttributesMethods, FieldType.DateTime) { }

        internal override void FindControls()
        {
            base.FindControls();
            FormatCbx = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_cmbdateformat", new HtmlComboBox(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
        }
    }
    #region class try
    internal class Row
    {

        internal string DisplayName { get; private set; }
        internal string Description { get; private set; }
        internal string Type { get; private set; }
        internal bool UsedForAudit { get; private set; }
        internal bool CanEdit { get; private set; }
        internal bool CanDelete { get; set; }

        public Row(string DisplayName, string Description, string Type, bool UsedForAudit, bool CanEdit = true, bool CanDelete = true)
        {
            this.DisplayName = DisplayName;
            this.Description = Description;
            this.Type = Type;
            this.UsedForAudit = UsedForAudit;
            this.CanEdit = CanEdit;
            this.CanDelete = CanDelete;
        }
    }
#endregion
}
