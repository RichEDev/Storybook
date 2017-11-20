namespace Auto_Tests.UIMaps.CustomEntityFormsClientUIMapClasses
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
    using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
    using System.Linq;
    using System.Text;
    using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Forms_Client;


    public class CustomEntityFormsClientControls
    {
        internal HtmlEdit standardSingleLineTxt { get; set; }
        internal cHtmlTextAreaWrapper multilineTxt { get; set; }
        internal HtmlEdit numberTxt { get; set; }
        internal HtmlEdit decimalTxt { get; set; }
        internal HtmlEdit currencyTxt { get; set; }
        internal HtmlEdit dateTimeDateTxt { get; set; }
        internal HtmlEdit dateTimeTimeTxt { get; set; }
        internal HtmlEdit dateTxt { get; set; }
        internal HtmlEdit timeTxt { get; set; }
        internal cHtmlTextAreaWrapper multilineLargeTxt { get; set; }
        internal HtmlComboBox yesNoDefaultCmb { get; set; }
        internal HtmlComboBox yesCmb { get; set; }
        internal HtmlComboBox noCmb { get; set; }
        internal HtmlEdit wideSingleLineTxt { get; set; }
        internal HtmlComboBox standardListCmb { get; set; }
        internal HtmlComboBox wideListCmb { get; set; }

        /// <summary>
        /// Cached list of Greenlights
        /// </summary>
        internal static List<CustomEntity> _customEntities;
        /// <summary>
        /// Shared methods UI Map
        /// </summary>
        private static SharedMethodsUIMap _sharedMethods = new SharedMethodsUIMap();
        protected CustomEntityFormsClientUIMap _cCustomEntityFormsClientMethods;
        protected ControlLocator<HtmlControl> _ControlLocator { get; private set; }

        public CustomEntityFormsClientControls(CustomEntityFormsClientUIMap cCustomEntityFormsClientMethods)
        {
            _ControlLocator = new ControlLocator<HtmlControl>();
            _cCustomEntityFormsClientMethods = cCustomEntityFormsClientMethods;
            FindControls();
        }

        /// <summary>
        /// Locates controls that are declared within this class
        /// </summary>
        public void FindControls()
        {
            _customEntities = CustomEntityFormsClientUITest._customEntities;
            standardSingleLineTxt = (HtmlEdit)_ControlLocator.findControl(getFieldDisplayId(_customEntities[0].attribute[0]), new HtmlEdit(_cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            multilineTxt = (cHtmlTextAreaWrapper)_ControlLocator.findControl(getFieldDisplayId(_customEntities[0].attribute[1]), new cHtmlTextAreaWrapper(_sharedMethods.ExtractHtmlMarkUpFromPage(), getFieldDisplayId(_customEntities[0].attribute[1]), _cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            numberTxt = (HtmlEdit)_ControlLocator.findControl(getFieldDisplayId(_customEntities[0].attribute[2]), new HtmlEdit(_cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            decimalTxt = (HtmlEdit)_ControlLocator.findControl(getFieldDisplayId(_customEntities[0].attribute[3]), new HtmlEdit(_cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            currencyTxt = (HtmlEdit)_ControlLocator.findControl(getFieldDisplayId(_customEntities[0].attribute[4]), new HtmlEdit(_cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            string datetimeControlID = getFieldDisplayId(_customEntities[0].attribute[5]);
            dateTimeDateTxt = (HtmlEdit)_ControlLocator.findControl(datetimeControlID, new HtmlEdit(_cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            dateTimeTimeTxt = (HtmlEdit)_ControlLocator.findControl(datetimeControlID + "_time", new HtmlEdit(_cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            dateTxt = (HtmlEdit)_ControlLocator.findControl(getFieldDisplayId(_customEntities[0].attribute[6]), new HtmlEdit(_cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            timeTxt = (HtmlEdit)_ControlLocator.findControl(getFieldDisplayId(_customEntities[0].attribute[7]), new HtmlEdit(_cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            multilineLargeTxt = (cHtmlTextAreaWrapper)_ControlLocator.findControl(getFieldDisplayId(_customEntities[0].attribute[8]), new cHtmlTextAreaWrapper(_sharedMethods.ExtractHtmlMarkUpFromPage(), getFieldDisplayId(_customEntities[0].attribute[8]), _cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            yesCmb = (HtmlComboBox)_ControlLocator.findControl(getFieldDisplayId(_customEntities[0].attribute[12]), new HtmlComboBox(_cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            noCmb = (HtmlComboBox)_ControlLocator.findControl(getFieldDisplayId(_customEntities[0].attribute[13]), new HtmlComboBox(_cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            wideSingleLineTxt = (HtmlEdit)_ControlLocator.findControl(getFieldDisplayId(_customEntities[0].attribute[14]), new HtmlEdit(_cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            standardListCmb = (HtmlComboBox)_ControlLocator.findControl(getFieldDisplayId(_customEntities[0].attribute[15]), new HtmlComboBox(_cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
            wideListCmb = (HtmlComboBox)_ControlLocator.findControl(getFieldDisplayId(_customEntities[0].attribute[16]), new HtmlComboBox(_cCustomEntityFormsClientMethods.UINewCustomEntity1WindWindow.UINewCustomEntity1Document.UICtl00_contentmain_taPane));
        }

        internal static string getFieldDisplayId(CustomEntitiesUtilities.CustomEntityAttribute attribute)
        {
            CustomEntitiesUtilities.CustomEntityFormField formField = new CustomEntitiesUtilities.CustomEntityFormField();
            for (int sectionindexer = 0; sectionindexer < _customEntities[0].form[0].tabs[0].sections.Count; sectionindexer++)
            {
                formField = (from field in _customEntities[0].form[0].tabs[0].sections[sectionindexer].fields
                             where field.attribute._attributeid == attribute._attributeid
                             select field).FirstOrDefault();
                if (formField != null) { break; }
            }

            StringBuilder displayId = new StringBuilder("ctl00_contentmain_");
            displayId.Append(string.Format("tabs{0}_tab{1}_", new object[] { formField._formid, _customEntities[0].form[0].tabs[0]._tabid }));
            if (attribute._fieldType != FieldType.TickBox && attribute._fieldType != FieldType.List)
            {
                displayId.Append(string.Format("txt{0}", new object[] { formField.attribute._attributeid }));
            }
            else
            {
                displayId.Append(string.Format("cmb{0}", new object[] { formField.attribute._attributeid }));
            }
            return displayId.ToString();
        }
    }


    public partial class CustomEntityFormsClientUIMap
    {
        protected ControlLocator<HtmlControl> _controlLocator { get; private set; }
        private cTableDataRow row;
        private HtmlTable htmlTable;
        /// <summary>
        /// Cached list of Greenlights
        /// </summary>
        internal static List<CustomEntity> _customEntities;
        public void ClickEditFieldLink(string searchParameterToEdit)
        {
            _customEntities = CustomEntityFormsClientUITest._customEntities;
            int keyField = _customEntities[0].attribute[0]._attributeid - 5;
            string gridId = "tbl_grid" + _customEntities[0].entityId + _customEntities[0].view[0]._viewid + keyField;
            _controlLocator = new ControlLocator<HtmlControl>();
            htmlTable = (HtmlTable)_controlLocator.findControl(gridId, new HtmlTable(UINewCustomEntity1WindWindow.UINewCustomEntity1Document));
            htmlTable.WaitForControlReady();
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit));
            Mouse.Click(row.UIEditImage, new Point(3, 3));
        }

        /// <summary>
        /// Used to click delete against a specific entry on the forms grid 
        ///</summary>
        public void ClickDeleteFieldLink(string searchParameterToEdit)
        {
            _customEntities = CustomEntityFormsClientUITest._customEntities;
            int keyField = _customEntities[0].attribute[0]._attributeid - 5;
            string gridId = "UITbl_grid" + _customEntities[0].entityId + _customEntities[0].view[0]._viewid + keyField;
            htmlTable = (HtmlTable)_controlLocator.findControl(gridId, new HtmlTable(UINewCustomEntity1WindWindow.UINewCustomEntity1Document));
            htmlTable.WaitForControlReady();
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit));
            Mouse.Click(row.UIDeleteImage, new Point(3, 3));
        }

        /// <summary>
        /// Validates view table
        /// </summary>
        /// <param name="expectedVal1"></param>
        /// <param name="expectedVal2"></param>
        /// <param name="shouldBePresent"></param>
        public void ValidateViewTable(string expectedVal1, string expectedVal2, bool shouldBePresent = true)
        {
            List<String> tableHeaders = new List<string>();
            foreach (var viewField in _customEntities[0].view[0].fields)
            {
                if (viewField._viewFieldId != 0)
                {
                    var attribute = (from att in _customEntities[0].attribute
                                     where att.FieldId == viewField._fieldid
                                       select att).FirstOrDefault();
                    tableHeaders.Add(attribute.DisplayName);
                }
            }

            int keyField = _customEntities[0].attribute[0]._attributeid - 5;
            string gridId = "UITbl_grid" + _customEntities[0].entityId + _customEntities[0].view[0]._viewid + keyField;
            htmlTable = (HtmlTable)_controlLocator.findControl(gridId, new HtmlTable(UINewCustomEntity1WindWindow.UINewCustomEntity1Document));


            Dictionary<string, int> cellLocations = GridHelpers.MapTableHeadersToLocation(htmlTable.Rows, tableHeaders);
            int rowLocation = GridHelpers.FindRowInGridForId(htmlTable, htmlTable.Rows, expectedVal1);
            if (!shouldBePresent)
            {
                Assert.AreEqual(-1, rowLocation);
            }
            else
            {
                UITestControlCollection cellCollection = ((HtmlRow)htmlTable.Rows[rowLocation]).Cells;

                HtmlCell cell;
                foreach (var viewField in _customEntities[0].view[0].fields)
                {
                    if (viewField._viewFieldId != 0)
                    {
                        var attribute = (from att in _customEntities[0].attribute
                                         where att.FieldId == viewField._fieldid
                                         select att).FirstOrDefault();
                        cell = (HtmlCell)cellCollection[cellLocations[attribute.DisplayName]];
                        Assert.AreEqual(attribute.attributeData.clientData1, cell.InnerText);
                    }
                }
            }
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
                        this.imgDelete.FilterProperties[HtmlImage.PropertyNames.Src] = "http://chronos4.sel-expenses.com/shared/images/icons/delete2.png";
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
                        this.imgEdit.FilterProperties[HtmlImage.PropertyNames.Src] = "http://chronos4.sel-expenses.com/shared/images/icons/edit.png";
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
