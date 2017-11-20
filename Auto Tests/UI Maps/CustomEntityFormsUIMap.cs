namespace Auto_Tests.UIMaps.CustomEntityFormsUIMapClasses
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
    using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
    using System.Windows.Forms;
    using System.Linq;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
    using System.Text;
    using System.Threading;

    public partial class CustomEntityFormsUIMap
    {
        private cTableDataRow row;
        private HtmlTable htmlTable;
        private Dictionary<string, HtmlSpan> availableFields;
        private static SharedMethodsUIMap cSharedMethods = new SharedMethodsUIMap();

        /// <summary>
        /// Used to click edit against a specific entry on the forms grid
        /// </summary>
        /// <param name="searchParameterToEdit"></param>
        public void ClickEditFieldLink(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UITbl_gridFormsTable;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 3));
            Mouse.Click(row.UIEditImage, new Point(3, 3));
        }

        /// <summary>
        /// Used to click delete against a specific entry on the forms grid 
        ///</summary>
        public void ClickDeleteFieldLink(BrowserWindow searchSpace, string searchParameterToEdit)
        {
            htmlTable = (HtmlTable)UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UITbl_gridFormsTable;
            htmlTable.WaitForControlReady();
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 3));
            Mouse.Click(row.UIDeleteImage, new Point(3, 3));
        }

        /// <summary>
        /// Used to click copy against a specific entry on the forms grid
        /// </summary>
        /// <param name="searchParameterToEdit"></param>
        public void ClickCopyFormLink(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UITbl_gridFormsTable;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit, 3));
            Mouse.Click(row.UICreateacopyofthisForImage, new Point(3, 3));
        }

        /// <summary>
        /// Used to validate values passed in expectedVal1 and expectedVal2 can be found in the grid
        ///</summary>
        public void ValidateFormTable(BrowserWindow searchSpace, string expectedVal1, string expectedVal2, bool shouldBePresent = true)
        {
            List<String> tableHeaders = new List<string>();
            tableHeaders.Add("Form Name");
            tableHeaders.Add("Description");

            UICustomEntitymyCEWindWindow window = new UICustomEntitymyCEWindWindow();

            htmlTable = window.UICustomEntitymyCEDocument.UITbl_gridFormsTable;

            Dictionary<string, int> cellLocations = GridHelpers.MapTableHeadersToLocation(htmlTable.Rows, tableHeaders);
            int rowLocation = GridHelpers.FindRowInGridForId(htmlTable, htmlTable.Rows, expectedVal1, 3);
            if (!shouldBePresent)
            {
                Assert.AreEqual(-1, rowLocation);
            }
            else
            {
                UITestControlCollection cellCollection = ((HtmlRow)htmlTable.Rows[rowLocation]).Cells;

                //Assert C.E Name field
                HtmlCell cell = (HtmlCell)cellCollection[cellLocations["Form Name"]];
                Assert.AreEqual(expectedVal1, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["Description"]];
                Assert.AreEqual(expectedVal2, cell.InnerText);
            }
        }

        /// <summary>
        /// Used to validate that the selected form has been deleted from the grid
        ///</summary>
        public void ValidateFormDeletion(string searchParameterDeletedPrimaryKey)
        {
            htmlTable = (HtmlTable)UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UITbl_gridFormsTable;
            UITestControlCollection collection = htmlTable.Rows;

            Assert.AreEqual(-1, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterDeletedPrimaryKey, 3));
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
        public void VerifyCorrectSortingOrderForTable(SortFormsByColumn sortby, EnumHelper.TableSortOrder sortingOrder, int entityid, ProductType executingProduct)
        {
            List<CustomEntitiesUtilities.CustomEntityForm> expectedData = getFormsOrderFromDatabase(sortby, sortingOrder, entityid, executingProduct);
            List<CustomEntitiesUtilities.CustomEntityForm> actualData = new List<CustomEntitiesUtilities.CustomEntityForm>();

            HtmlTable table = (HtmlTable)UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UITbl_gridFormsTable;
            UITestControlCollection collection = table.Rows;

            //Remove table header from list
            collection.RemoveAt(0);
            //Ensure count of data in table is the same with count of data in database
            Assert.AreEqual(table.RowCount - 1, expectedData.Count);

            foreach (HtmlRow row in collection)
            {
                UITestControlCollection rowCollection = row.Cells;
                //string formName = ((HtmlCell)rowCollection[2]).InnerText != null ? ((HtmlCell)rowCollection[2]).InnerText : "";
                //string description = ((HtmlCell)rowCollection[3]).InnerText != null ? ((HtmlCell)rowCollection[3]).InnerText : "";

                string formName = ((HtmlCell)rowCollection[3]).InnerText != null ? ((HtmlCell)rowCollection[3]).InnerText : "";
                string description = ((HtmlCell)rowCollection[4]).InnerText != null ? ((HtmlCell)rowCollection[4]).InnerText : "";

                actualData.Add(new CustomEntitiesUtilities.CustomEntityForm(formName, description));
            }

            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.AreEqual(actualData[i]._formName, expectedData[i]._formName);
                Assert.AreEqual(actualData[i]._description, expectedData[i]._description);
            }
        }

        /// <summary>
        /// Used to get the order of the data from the database
        ///</summary>
        internal List<CustomEntitiesUtilities.CustomEntityForm> getFormsOrderFromDatabase(SortFormsByColumn sortby, Auto_Tests.Tools.EnumHelper.TableSortOrder sortingOrder, int entityid, ProductType executingProduct)
        {
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));
            return new FormsDAO(db).GetCorrectSortingOrderFromDB(sortby, sortingOrder, entityid);
        }

        private class cTableDataRow : HtmlRow
        {
            private HtmlImage imgDelete;
            private HtmlImage imgEdit;
            private HtmlImage imgCopy;

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

            public HtmlImage UICreateacopyofthisForImage
            {
                get
                {
                    if ((this.imgCopy == null))
                    {
                        this.imgCopy = new HtmlImage(this);
                        #region Search Criteria
                        this.imgCopy.SearchProperties[HtmlImage.PropertyNames.Id] = null;
                        this.imgCopy.SearchProperties[HtmlImage.PropertyNames.Name] = null;
                        this.imgCopy.SearchProperties[HtmlImage.PropertyNames.Alt] = "Create a copy of this Form";
                        this.imgCopy.FilterProperties[HtmlImage.PropertyNames.AbsolutePath] = "/static/icons/16/new-icons/copy.png";
                        this.imgCopy.FilterProperties[HtmlImage.PropertyNames.Class] = null;
                        this.imgCopy.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] = "alt=\"Create a copy of this Form\" src=\"/s";
                        this.imgCopy.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "3";
                        #endregion
                    }
                    return this.imgCopy;
                }
            }
        }

        /// <summary>
        /// RightClickAndPaste
        /// </summary>
        public void RightClickAndPaste(HtmlControl control)
        {
            if (control.ControlType == ControlType.Edit)
            {
                #region Variable Declarations
                //HtmlEdit uITextforsaveandstaybuEdit = this.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UITextforsaveandstaybuEdit;
                WinMenuItem uIPasteMenuItem = this.UIItemWindow.UIContextMenu.UIPasteMenuItem;
                #endregion

                // Click 'Text for 'save and stay' button' text box
                Mouse.Click(control, new Point(145, 13));

                // Right-Click 'Text for 'save and stay' button' text box
                Mouse.Click(control, MouseButtons.Right, ModifierKeys.None, new Point(145, 13));

                // Click 'Paste' menu item
                Mouse.Click(uIPasteMenuItem, new Point(47, 15));
            }
            else
            {
                throw new ArgumentException("Invalid HtmlControl! I only support edit boxes!!");
            }
        }

        /// <summary>
        /// ClickCogOnTab
        /// </summary>
        public void ClickCogOnTab(string tabName)
        {
            HtmlImage uIOptionsImage = null;
            foreach (UITestControl control in UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument15.UITabBarPane.GetParent().GetChildren())
            {
                //Get child controls of tab span - doesn't get NEW TAB since its a seperate span div
                UITestControlCollection childControls = control.GetChildren();
                foreach (UITestControl tabControlIterator in childControls)
                {
                    HtmlSpan tabControl = tabControlIterator as HtmlSpan;
                    if (tabControl.FriendlyName == tabName)
                    {
                        uIOptionsImage = tabControl.GetChildren()[1].GetChildren()[0] as HtmlImage;
                    }
                }
            }

            // Click 'Options' image
            Mouse.Click(uIOptionsImage, new Point(12, 5));
        }

        /// <summary>
        /// Verify the forms context menu against the expected menu items
        /// </summary>
        /// <param name="menuItems"></param>
        public void VerifyFormsContextMenuItems(List<String> menuItems)
        {
            //UITestControlCollection CogMenuChildren = this.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument8.UICogMenu.GetChildren();
            //UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument13.UISectionContextPanel.GetChildren()
            UITestControlCollection CogMenuChildren = this.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument13.UISectionContextPanel.GetChildren();
            List<String> foundMenuItems = new List<string>();
            foreach (UITestControl menuItem in CogMenuChildren)
            {
                foundMenuItems.Add(menuItem.FriendlyName);
            }
            Assert.IsTrue(Enumerable.SequenceEqual(menuItems, foundMenuItems));
        }

        /// <summary>
        /// Verify the correct tabs for the form against the expected tabs
        /// </summary>
        /// <param name="expectedTabsName"></param>
        /// <returns></returns>
        public bool VerifyCorrectTabsForForm(List<string> expectedTabsName)
        {
            List<string> actualTabNames = new List<string>();
            foreach (UITestControl control in UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument15.UITabBarPane.GetParent().GetChildren())
            {
                HtmlSpan myControl = control as HtmlSpan;
                actualTabNames.Add(myControl.FriendlyName);
            }

            try
            {
                Assert.IsTrue(Enumerable.SequenceEqual(expectedTabsName, actualTabNames));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// ClickSaveForm
        /// </summary>
        public void ClickSaveForm()
        {
            cSharedMethods.SetFocusOnControlAndPressEnter(UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UISaveButton);
        }

        /// <summary>
        /// ClickCancelOnFormModal
        /// </summary>
        public void ClickCancelOnFormModal()
        {
            cSharedMethods.SetFocusOnControlAndPressEnter(UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICancelButton);
        }

        /// <summary>
        /// ClickSaveTab
        /// </summary>
        public void ClickSaveTab()
        {
            cSharedMethods.SetFocusOnControlAndPressEnter(UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument17.UITabSaveButton);
        }

        /// <summary>
        /// ClickCancelOnTabModal
        /// </summary>
        public void ClickCancelOnTabModal()
        {
            cSharedMethods.SetFocusOnControlAndPressEnter(UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument17.UITabCancelButton);
        }

        /// <summary>
        /// ClickCancelTab
        /// </summary>
        public void ClickCancelTab()
        {
            cSharedMethods.SetFocusOnControlAndPressEnter(UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument17.UITabCancelButton);
        }

        /// <summary>
        /// ClickNewSection
        /// </summary>
        public void ClickNewSection()
        {
            #region Variable Declarations
            HtmlImage uINewsectionImage = this.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument9.UICtl00_contentmain_poPane.UINewsectionImage;
            #endregion

            // Click 'New section' image
            Mouse.Click(uINewsectionImage, new Point(18, 10));
        }

        /// <summary>
        /// Click the cog on a section
        /// </summary>
        /// <param name="sectionName"></param>
        public void ClickCogOnSection(HtmlImage sectionName)
        {
            if (sectionName == null) throw new ArgumentNullException("Section name cannot be null!!");
            Mouse.Click(sectionName);
        }

        /* Could be done better with bit shifting an enum value!! */
        internal void AssertContextMenu(FieldType fieldType, int attributeId)
        {
            ControlLocator<HtmlSpan> contextLocator = new ControlLocator<HtmlSpan>();
            string id = string.Format("form_attribute_{0}_optionsmenu", attributeId);
            HtmlSpan contextOptionsMenu = contextLocator.findControl(id, this.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UIFormDesignPane);

            StringBuilder sBuilder = new StringBuilder();
            foreach (HtmlImage img in contextOptionsMenu.GetChildren())
            {
                sBuilder.Append(img.Alt);
                sBuilder.Append(" ");
            }

            switch (fieldType)
            {
                case FieldType.Number:
                case FieldType.Integer:
                case FieldType.Currency:
                case FieldType.DateTime:
                case FieldType.List:
                case FieldType.TickBox:
                    Assert.AreEqual("Move left Move right Edit label text Read only Delete ", sBuilder.ToString());
                    break;
                case FieldType.Text:
                case FieldType.LargeText:
                    Assert.AreEqual("Move left Move right Edit label text Edit default field value Read only Delete ", sBuilder.ToString());
                    break;
                case FieldType.Comment:
                    Assert.AreEqual("Move left Move right Delete ", sBuilder.ToString());
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }

        /// <summary>
        /// Click the passed in icon on the form field found
        /// by the attribute id
        /// </summary>
        /// <param name="attributeId"></param>
        /// <param name="altTextForImageToClick"></param>
        public void ClickIconOnFormField(int attributeId, string altTextForImageToClick)
        {
            ControlLocator<HtmlSpan> contextLocator = new ControlLocator<HtmlSpan>();
            string id = string.Format("form_attribute_{0}_optionsmenu", attributeId);
            HtmlSpan contextOptionsMenu = contextLocator.findControl(id, this.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UIFormDesignPane);
   
            foreach (HtmlImage img in contextOptionsMenu.GetChildren())
            {
                if (img.Alt == altTextForImageToClick)
                {
                    if (altTextForImageToClick == "Delete")
                    {
                        Thread.Sleep(2000);
                        RefreshAvailableFieldsCache();
                    }
                    Mouse.Click(img, new Point(4, 4));
                }
            }

        }

        /// <summary>
        /// Moves mouse to the passed in controlid
        /// </summary>
        /// <param name="controlId">Id of the control to move to</param>
        public void MoveMouseToControl(string controlId)
        {
            ControlLocator<HtmlControl> labelLocator = new ControlLocator<HtmlControl>();
            HtmlControl label1 = labelLocator.findControl(controlId, UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UIFormDesignPane);
            label1.EnsureClickable(new Point(157, 14));
            Mouse.Move(label1, new Point(157, 14));
        }

        /// <summary>
        /// Gets contents of the available fields span
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public HtmlSpan GetAvailableField(string fieldName)
        {
            return AvailableFields[fieldName];
        }

        /// <summary>
        /// Attemps to find the passed in attribute in the availble fields
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool DoesAttributeExistInAvaialbleFields(string fieldName)
        {
            return AvailableFields.ContainsKey(fieldName);
        }

        /// <summary>
        /// Update the cache for available fields
        /// </summary>
        public void RefreshAvailableFieldsCache()
        {
            PopulateFields();
        }


        private Dictionary<string, HtmlSpan> AvailableFields
        {
            get
            {
                if (availableFields == null)
                {
                    availableFields = new Dictionary<string, HtmlSpan>();
                    PopulateFields();
                }
                return availableFields;
            }
        }

        /// <summary>
        /// Populate the available fields cache
        /// </summary>
        private void PopulateFields()
        {
            AvailableFields.Clear();
            UIGreenLightCustomEntiWindow window = new UIGreenLightCustomEntiWindow();
            window.SearchConfigurations.Add(SearchConfiguration.AlwaysSearch);
            HtmlDiv availablefields = window.UIGreenLightCustomEntiDocument9.UIAvailableFields;

            UITestControlCollection availableFieldsCollection = availablefields.GetChildren()[0].GetChildren();
            foreach (HtmlSpan field in availableFieldsCollection)
            {
                if (!AvailableFields.ContainsKey(field.DisplayText))
                {
                    AvailableFields.Add(field.DisplayText, field);
                }
            }
            //Add all controls
            availableFieldsCollection = availablefields.GetChildren()[1].GetChildren();

            foreach (HtmlSpan field in availableFieldsCollection)
            {
                if (!AvailableFields.ContainsKey(field.DisplayText))
                {
                    AvailableFields.Add(field.DisplayText, field);
                }
            }
        }

        /// <summary>
        /// Click Tab which corrosponds to the tab name
        /// </summary>
        /// <param name="tabName"></param>
        public void ClickTab(string tabName)
        {
            foreach (UITestControl control in UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument15.UITabBarPane.GetParent().GetChildren()[0].GetChildren())
            {
                HtmlSpan tab = control as HtmlSpan;
                if (tab.InnerText == tabName)
                {
                    Mouse.Click(tab);
                }
            }
        }

        /// <summary>
        /// Method verifys the correct menu items appear for the section when 
        /// menu shown. This method expects the Inner text from the section to be passed
        /// in the correct order!
        /// </summary>
        /// <param name="menuItemNames"></param>
        public void VerifySectionContextMenu(List<string> menuItemNames)
        {
            List<string> actualItems = new List<string>();

            foreach (HtmlImage contextMenuImages in UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument13.UISectionContextPanel.GetChildren())
            {
                actualItems.Add(contextMenuImages.Alt);
            }
            CollectionAssert.AreEqual(menuItemNames, actualItems);
        }

        public void ClickIconOnTabContextMenu(string menuItemFriendlyName)
        {
            foreach (HtmlImage contextMenuImages in this.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument.UIPopupFormOptionsPane.GetChildren())
            {
                if (menuItemFriendlyName == contextMenuImages.Alt)
                {
                    Mouse.Click(contextMenuImages, new Point(3, 3));
                }
            }
        }
        /// <summary>
        /// Ensure the correct sections appear on the passed in tab name
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="sections"></param>
        public void VerifyCorrectSectionsAppearOnTab(string tabName, List<string> sections)
        {
            if (string.IsNullOrEmpty(tabName))
            {
                throw new ArgumentNullException("Tab name cannot be null or empty!!");
            }
            if (sections == null)
            {
                throw new ArgumentNullException("Expected sections cannot be null!!");
            }
            HtmlSpan tabControl = (HtmlSpan)GetTabControlByTabName(tabName);
            string tabId = tabControl.Id;

            HtmlDiv Sections = UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument5.UIFormTabsPane;
            List<string> actualValues = new List<string>();
            foreach (HtmlDiv i in Sections.GetChildren())
            {
                if (i.GetChildren().Count > 0)
                {
                    if (tabId.Contains(i.Id) && !string.IsNullOrEmpty(((HtmlDiv)((HtmlDiv)i.GetChildren()[0]).GetChildren()[0]).InnerText))
                    {
                        foreach (HtmlDiv htmlDiv in i.GetChildren())
                        {
                            actualValues.Add(((HtmlDiv)htmlDiv.GetChildren()[0]).InnerText);
                        }
                    }
                }
            }
            CollectionAssert.AreEqual(sections, actualValues);
        }

        /// <summary>
        /// Gets the UITestControl for the tab via the provided
        /// tab name
        /// </summary>
        /// <param name="tabName"></param>
        /// <returns></returns>
        public UITestControl GetTabControlByTabName(string tabName)
        {
            foreach (UITestControl control in UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument15.UITabBarPane.GetParent().GetChildren())
            {
                //Get child controls of tab span - doesn't get NEW TAB since its a seperate span div
                UITestControlCollection childControls = control.GetChildren();
                foreach (UITestControl tabControlIterator in childControls)
                {
                    HtmlSpan tabControl = tabControlIterator as HtmlSpan;
                    if (tabControl.FriendlyName == tabName)
                    {
                        return tabControl;
                    }
                }
            }
            return null;
        }

        public void AssertCorrectAttributesAppearForSection(string tabId, string sectionName, List<string> expectedAttributesUnderSection)
        {
            UITestControl sectionDesigner = UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument5.UIFormTabsPane;
            List<string> actualItemsInSections = new List<string>();
            foreach (HtmlDiv FormTabs in sectionDesigner.GetChildren())
            {
                if(GetTabControlByTabName(tabId).FriendlyName == tabId) 
                {
                    foreach (HtmlDiv Sections in FormTabs.GetChildren())
                    {
                        if (((HtmlDiv)Sections.GetChildren()[0]).FriendlyName == sectionName)
                        {
                            foreach (HtmlSpan Attribute in Sections.GetChildren()[1].GetChildren())
                            {
                                actualItemsInSections.Add(((HtmlSpan)Attribute).DisplayText);
                            }
                            break;
                        }
                    }
                }
            }
            CollectionAssert.AreEqual(expectedAttributesUnderSection,actualItemsInSections);
        }

        /// <summary>
        /// Populates the section name control text box.
        /// </summary>
        /// <param name="text">Text to put into the control text</param>
        internal void PopulateSectionNameTextBox(string text)
        {
                HtmlSectionControlEditText.Text = text;
        }

        /// <summary>
        /// Populates the form name control text box.
        /// </summary>
        /// <param name="text">Text to put into the control text</param>
        internal void PopulateTabNameTextBox(string text)
        {
            HtmlTabControlEditText.Text = text;
        }

        #region form properties
        private HtmlEdit _htmlTabControlEditText;
        internal HtmlEdit HtmlTabControlEditText 
        { 
            get 
            {
                if (_htmlTabControlEditText == null)
                {
                    _htmlTabControlEditText = new ControlLocator<HtmlEdit>().findControl("ctl00_contentmain_txttabheader", new HtmlEdit(UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));
                }
                return _htmlTabControlEditText;
            }
        }

        private HtmlEdit _htmlSectionControlEditText;
        internal HtmlEdit HtmlSectionControlEditText
        {
            get 
            {
                if (_htmlSectionControlEditText == null)
                {
                    _htmlSectionControlEditText = new ControlLocator<HtmlEdit>().findControl("ctl00_contentmain_txtsectionheader", new HtmlEdit(UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));
                }
                return _htmlSectionControlEditText;
            }
        }
        
        #endregion
    }
}
