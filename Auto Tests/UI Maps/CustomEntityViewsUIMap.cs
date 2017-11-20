namespace Auto_Tests.UIMaps.CustomEntityViewsUIMapClasses
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
    using Auto_Tests.Coded_UI_Tests.GreenLight.CustomVEntityViews.ViewsDatabaseAdaptor;
    using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Views;
    using System.Linq;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
      
    
    public partial class CustomEntityViewsUIMap
    {
        private cTableDataRow row;
        private HtmlTable htmlTable;

        public void ClickEditFieldLink(string searchParameterToEdit)
        {

            htmlTable = (HtmlTable)UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument2.UITbl_gridViewsTable;
            // htmlTable.WaitForControlReady();    
            UITestControlCollection collection = htmlTable.Rows;

            row = new cTableDataRow(htmlTable, GridHelpers.FindRowInGridForId(htmlTable, collection, searchParameterToEdit));
            Mouse.Click(row.UIEditImage, new Point(3, 3));
        }

        /// <summary>
        /// Used to click delete against a specific entry on the forms grid 
        ///</summary>
        public void ClickDeleteFieldLink(string searchParameterToEdit)
        {
            htmlTable = (HtmlTable)UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument2.UITbl_gridViewsTable;
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
            tableHeaders.Add("View Name");
            tableHeaders.Add("Description");

            UIGreenLightCustomEntiWindow window = new UIGreenLightCustomEntiWindow();

            htmlTable = window.UIGreenLightCustomEntiDocument2.UITbl_gridViewsTable;

            Dictionary<string, int> cellLocations = GridHelpers.MapTableHeadersToLocation(htmlTable.Rows, tableHeaders);
            int rowLocation = GridHelpers.FindRowInGridForId(htmlTable, htmlTable.Rows, expectedVal1);
            if (!shouldBePresent)
            {
                Assert.AreEqual(-1, rowLocation);
            }
            else
            {
                UITestControlCollection cellCollection = ((HtmlRow)htmlTable.Rows[rowLocation]).Cells;

                //Assert C.E Name field
                HtmlCell cell = (HtmlCell)cellCollection[cellLocations["View Name"]];
                Assert.AreEqual(expectedVal1, cell.InnerText);

                cell = (HtmlCell)cellCollection[cellLocations["Description"]];
                Assert.AreEqual(expectedVal2, cell.InnerText);
            }
        }

        /// <summary>
        /// Used to compare the order of the data in the grid and the database match
        ///</summary>
        public void VerifyCorrectSortingOrderForTable(SortViewsByColumn sortby, EnumHelper.TableSortOrder sortingOrder, int entityid, ProductType executingProduct)
        {
            List<CustomEntitiesUtilities.CustomEntityView> expectedData = GetViewsOrderFromDatabase(sortby, sortingOrder, entityid, executingProduct);
            List<CustomEntitiesUtilities.CustomEntityView> actualData = new List<CustomEntitiesUtilities.CustomEntityView>();

            HtmlTable table = (HtmlTable)UIGreenLightAdministrationViewsWindow.UIGreenLightAdministrationViewsDocument.UITbl_gridViewsTable;
            UITestControlCollection collection = table.Rows;

            //Remove table header from list
            collection.RemoveAt(0);
            //Ensure count of data in table is the same with count of data in database
            Assert.AreEqual(table.RowCount - 1, expectedData.Count);

            foreach (HtmlRow row in collection)
            {
                UITestControlCollection rowCollection = row.Cells;
                string formName = ((HtmlCell)rowCollection[2]).InnerText != null ? ((HtmlCell)rowCollection[2]).InnerText : "";
                string description = ((HtmlCell)rowCollection[3]).InnerText != null ? ((HtmlCell)rowCollection[3]).InnerText : "";

                actualData.Add(new CustomEntitiesUtilities.CustomEntityView()
                {
                    _viewName = formName,
                    _description = description
                });
            }

            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.AreEqual(expectedData[i]._viewName, actualData[i]._viewName);
                Assert.AreEqual(expectedData[i]._description, actualData[i]._description);
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
        /// Used to get the order of the data from the database
        ///</summary>
        internal List<CustomEntitiesUtilities.CustomEntityView> GetViewsOrderFromDatabase(SortViewsByColumn sortby, Auto_Tests.Tools.EnumHelper.TableSortOrder sortingOrder, int entityid, ProductType executingProduct)
        {
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));
            return ViewsDatabaseAdaptor.GetCorrectSortingOrderFromDB(db,sortby, sortingOrder, entityid);
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
                        this.imgEdit.FilterProperties[HtmlImage.PropertyNames.ControlDefinition] = "alt=Edit src=\"/shared/images/icons/edit.";
                        this.imgEdit.FilterProperties[HtmlImage.PropertyNames.TagInstance] = "1";
                        #endregion
                    }
                    return this.imgEdit;
                }
            }
        }

        /// <summary>
        /// Creates a dictionary that stores the tree as it appears on the UI
        /// </summary>
        internal Dictionary<string, LeafInfo> CreateAvailableFieldsFromUI(UITestControl currentControl, Dictionary<string, LeafInfo> availableFieldsContentsInUI, int leafLevel)
        {
            UITestControlCollection liFields = currentControl.GetChildren();

            foreach (HtmlCustom testControl in liFields)
            {
                if (testControl.Class == "jstree-open")
                {
                    int deeperLeaf = leafLevel + 1;
                    availableFieldsContentsInUI = CreateAvailableFieldsFromUI(testControl.GetChildren()[2], availableFieldsContentsInUI, deeperLeaf);
                }
                else
                {
                    LeafInfo leaf = new LeafInfo(leafLevel, testControl.Class);
                    availableFieldsContentsInUI.Add(testControl.Id, leaf);
                }
            }

            return availableFieldsContentsInUI;
        }

        /// <summary>
        /// Iterates through the dictionary and asserts if the pair is hidden or not 
        /// </summary>
        internal void ValidateAvailableFields(List<CustomEntitiesUtilities.CustomEntityAttribute> expectedAvailableFields)
        {
            Dictionary<string, LeafInfo> availableFieldsContentsInUI = new Dictionary<string, LeafInfo>();

            int leafLevel = 0;

            availableFieldsContentsInUI = CreateAvailableFieldsFromUI(UIGreenLightMyCEWindowWindow.UIGreenLightMyCEDocument.UICtl00_contentmain_taPane.GetChildren()[0], availableFieldsContentsInUI, leafLevel);

            foreach (KeyValuePair<string, LeafInfo> pair in availableFieldsContentsInUI)
            {
                CustomEntitiesUtilities.CustomEntityAttribute attribute;
                KeyValuePair<string, LeafInfo> lastPair = GetLastLeafForLevel(pair.Value.Level, availableFieldsContentsInUI);
                string nodeClass = pair.Value.NodeClass;
                //Checks to see if the pair is found in the expectedAvailableFields list if it is in the first level of the tree or if it is found in the base table list if it is in the second level of the tree
                if (pair.Value.Level == 0)
                {
                    attribute = GetAttributeFromAvailableFields(pair, expectedAvailableFields);
                }
                else
                {
                    CustomEntitiesUtilities.Field field = new CustomEntitiesUtilities.Field();
                    field = GetFieldFromBaseFieldsOrUDFs(pair, expectedAvailableFields);
                    attribute = field as CustomEntitiesUtilities.CustomEntityAttribute;
                    if (field != null)
                    {
                        if (field._isForeignKey == true)
                        {
                            attribute._fieldType = FieldType.Relationship;
                        }
                    }
                }

                //If it is found, checks to see if the attribute is the last node on the tree and if it is a relationship and asserts the class
                if (attribute != null)
                {
                    if (!pair.Equals(lastPair))
                    {
                        if (attribute._fieldType == FieldType.Relationship)
                        {
                            CustomEntitiesUtilities.CustomEntityNtoOneAttribute nto1Relationship = attribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                            if (!nto1Relationship._isExpanded)
                            {
                                Assert.AreEqual(nodeClass, "jstree-closed");
                            }
                            else
                            {
                                Assert.AreEqual(nodeClass, "jstree-open");
                            }
                        }
                        else
                        {
                            Assert.AreEqual(nodeClass, "jstree-leaf");
                        }
                    }
                    else if (attribute._fieldType == FieldType.Relationship)
                    {
                        Assert.AreEqual(nodeClass, "jstree-closed jstree-last");
                    }
                    else
                    {
                        Assert.AreEqual(nodeClass, "jstree-last jstree-leaf");
                    }
                }
                else
                {
                    //If it's not found checks to see if the last node on the tree
                    if (!pair.Equals(lastPair))
                    {
                        Assert.AreEqual(nodeClass, "jstree-leaf tree-node-disabled");
                    }
                    else
                    {
                        Assert.IsTrue(nodeClass.Contains("jstree-leaf"), "Not leaf on the tree");
                        Assert.IsTrue(nodeClass.Contains("jstree-last"), "Not last leaf on the tree");
                        Assert.IsTrue(nodeClass.Contains("tree-node-disabled"), "Leaf is enabled");
                    }
                }
            }
        }

        public class AvailbleFieldsColumnsNode : HtmlDocument
        {
            private UITestControl searchSpace;
            public AvailbleFieldsColumnsNode(UITestControl searchLimit) : base(searchLimit) { searchSpace = searchLimit; }

            public UITestControl findTreeNode(string nameOfNodeToFind)
            {
                UITestControl testControl = new UITestControl(searchSpace);
                testControl.SearchProperties[UITestControl.PropertyNames.Name] = null;
                testControl.SearchProperties["TagName"] = "LI";
                //testControl.FilterProperties["Class"] = "jstree-closed";
                testControl.FilterProperties["InnerText"] = nameOfNodeToFind;
                testControl.Find();
                return testControl;
            }
        }

        /// <summary>
        /// Checks if a pair from the dictionary is the last node of the tree for the specific level
        /// </summary>
        internal KeyValuePair<string, LeafInfo> GetLastLeafForLevel(int level, Dictionary<string, LeafInfo> leaves)
        {
            KeyValuePair<string, LeafInfo> lastLeaf = new KeyValuePair<string,LeafInfo>();
            foreach (KeyValuePair<string, LeafInfo> leaf in leaves)
            {
                if (leaf.Value.Level == level)
                {
                    lastLeaf = leaf;
                }
            }
            return lastLeaf;
        } 
        
        /// <summary>
        /// Checks if a pair from the dictionary is present in the list of base tables or the list of UDFs or if it the User defined fields folder.
        /// If it's found it returns either the base table field or the UDF or the User Defined Folder otherwise it returns null.
        /// </summary>
        internal static CustomEntitiesUtilities.Field GetFieldFromBaseFieldsOrUDFs(KeyValuePair<string, LeafInfo> pair, List<CustomEntitiesUtilities.CustomEntityAttribute> expectedAvailableFields)
        {
            foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in expectedAvailableFields)
            {
                if (attribute._fieldType == FieldType.Relationship)
                {
                    CustomEntitiesUtilities.CustomEntityNtoOneAttribute nto1Relationship = attribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                    if (nto1Relationship._isExpanded)
                    {
                        foreach (CustomEntitiesUtilities.Field field in nto1Relationship._baseTableFields)
                        {
                            if (pair.Key.EndsWith(field.FieldId.ToString()))
                            {
                                return field;
                            }
                        }
                        if (nto1Relationship._udfFolder != null)
                        {
                            if (nto1Relationship._udfFolder._isExpanded)
                            {
                                foreach (CustomEntitiesUtilities.Field field in nto1Relationship._UDFFields)
                                {
                                    if (pair.Key.EndsWith(field.FieldId.ToString()))
                                    {
                                        return field;
                                    }
                                }
                            }
                            else if(pair.Key.EndsWith(nto1Relationship._udfFolder.FieldId.ToString()))
                            {
                                return nto1Relationship._udfFolder;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if a pair from the dictionary is present in the list of attributes.
        /// If it's found it returns the attribute otherwise it returns null.
        /// </summary>
        internal static CustomEntitiesUtilities.CustomEntityAttribute GetAttributeFromAvailableFields(KeyValuePair<string, LeafInfo> pair, List<CustomEntitiesUtilities.CustomEntityAttribute> expectedAvailableFields)
        {
            foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in expectedAvailableFields)
            {
                if (pair.Key.EndsWith(attribute.FieldId.ToString()))
                {
                    return attribute;
                }
            }
            return null;
        }

        /// <summary>
        /// Class used to construct the leafs of the tree as it appears on the UI
        /// Level holds how deep we are in the tree
        /// NodeClass holds the css class of the leaf
        /// </summary>
        internal class LeafInfo
        {
            public int Level { get; set; }
            public string NodeClass { get; set; }

            public LeafInfo(int level, string nodeClass) 
            {
                Level = level;
                NodeClass = nodeClass;
            }

            public LeafInfo() { }
        }


        private UITestControl _treeNode;
        private string _controlInnerText = string.Empty;
        internal UITestControl TreeNode
        {
            get 
            {
                if (_treeNode == null)
                {
                    _treeNode = new UITestControl(UIGreenLightCustomEntiWindow16.UIGreenLightCustomEntiDocument2);

                    _treeNode.SearchProperties[UITestControl.PropertyNames.Name] = "";
                    _treeNode.SearchProperties["TagName"] = "LI";
                    _treeNode.FilterProperties["Class"] = "jstree-leaf";
                    _treeNode.FilterProperties["InnerText"] = _controlInnerText;
                    //treeNode.FilterProperties["InnerText"] = "  Account Holder Name";
                    _treeNode.WindowTitles.Add("GreenLight: Custom Entity 1");
                }
                return _treeNode;
            }
            set { _treeNode = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClickBaseTableField(string baseTableFieldName)
        {
            _controlInnerText = baseTableFieldName;
            // Click 'Account Holder Name' link
            if (TreeNode.GetChildren().Count >= 2)
            {
                Mouse.Click(TreeNode.GetChildren()[1], new Point(47, 8));
            }
            else
            {
                Mouse.Click(TreeNode, new Point(47, 8));
            }
        }

        /// <summary>
        /// Expand N TO one Cars node
        /// </summary>
        public void ExpandNToOneToCarsNode()
        {
            #region Variable Declarations
            HtmlHyperlink uIN1tocarsHyperlink = this.UIGreenLightCustomEntiWindow16.UIGreenLightCustomEntiDocument4.UIK22b15c9cc50a4cd68b5Custom.UIN1tocarsHyperlink;
            #endregion

            // Click 'n:1 to cars' link
            Mouse.Click(uIN1tocarsHyperlink, new Point(70, 14));
}

        /// <summary>
        /// MoveSelectedListItem
        /// 
        /// Clicks the plus associated with the passed in listItemName 
        /// </summary>
        public void MoveSelectedListItem(string listItemName)
        {
            HtmlCustom listItemPane = new HtmlCustom(this.UIGreenLightCustomEntiWindow16.UIGreenLightCustomEntiDocument8.UIPetrolDieselLPG0SelePane);

            listItemPane.SearchProperties["Id"] = null;
            listItemPane.SearchProperties[UITestControl.PropertyNames.Name] = null;
            listItemPane.SearchProperties["TagName"] = "LI";
            listItemPane.FilterProperties["InnerText"] = listItemName;
            listItemPane.WindowTitles.Add("GreenLight: Custom Entity 1");

            //Click the '+' to add it to the selected pane
            listItemPane.EnsureClickable(new Point(24, 11));
            Mouse.Click(listItemPane.GetChildren()[1]);
        }

        /// <summary>
        /// MoveSelectedListItem
        /// 
        /// Clicks the plus associated with the passed in listItemName 
        /// </summary>
        //public void MoveSelectedListItemWithMouse(string listItemName)
        //{
        //    HtmlCustom uIStandardlistitem2Custom = this.UIGreenLightCustomEntiWindow16.UIGreenLightCustomEntiDocument17.UIDivFilterListPane.UIStandardlistitem2Custom;
        //    UITestControl uIStandardlistitem2Custom1 = this.UIGreenLightCustomEntiWindow16.UIGreenLightCustomEntiDocument17.UIDivFilterListPane;
        //    uIStandardlistitem2Custom.SearchProperties["Id"] = null;
        //    uIStandardlistitem2Custom.SearchProperties[UITestControl.PropertyNames.Name] = null;
        //    uIStandardlistitem2Custom.SearchProperties["TagName"] = "LI";
        //    uIStandardlistitem2Custom.FilterProperties["InnerText"] = listItemName;
        //    uIStandardlistitem2Custom.WindowTitles.Add("GreenLight: Custom Entity 1");

        //    //Click the '+' to add it to the selected pane
        //    Mouse.Click(uIStandardlistitem2Custom);
        //    Mouse.StartDragging(uIStandardlistitem2Custom, new Point(72, 9));
        //    try
        //    {
        //        Mouse.StopDragging(uIStandardlistitem2Custom1, new Point(150, 20));
        //        Mouse.Click();
        //    }
        //    catch (PlaybackFailureException) { Mouse.Click(); }
        //}


        /// <summary>
        /// 
        /// Removes the selected item from the list filter modal.
        /// 
        /// <param name="listItemDisplayText">display text of list item to remove</param>
        /// </summary>
        public void RemoveSelectedListItem(string listItemText)
        {
           UITestControl selectedListFilters =  UIGreenLightCustomEntiWindow16.UIGreenLightCustomEntiDocument13.UIDivFilterListPane.UIISelectedListItemFilterPane.GetChildren()[1];
           UITestControlCollection listItems = selectedListFilters.GetChildren();

            foreach(UITestControl lItem in listItems) 
            {
                HtmlCustom selectedListItem = lItem as HtmlCustom;
                if (selectedListItem != null)
                {
                    if (selectedListItem.InnerText == listItemText)
                    {
                        //Get hyperlink for delete and click
                        Mouse.Click(selectedListItem.GetChildren()[1], new Point(10, 12));
                        break;
                    }
                }
            }
          
        }

        /// <summary>
        /// Verifies modal message when setting filter condition and leaving mandatory fields blank. Closes
        /// modal message once complete
        /// </summary>
        /// <param name="viewcontrols"></param>
        /// <param name="conditionType">Filter Condition to set </param>
        /// <param name="ExpectedModalMessage">Expected modal message</param>
        internal void VerifyFilterValidationMessageOnSave(CustomEntityViewsControls viewcontrols, ConditionType conditionType, string ExpectedModalMessage)
        {
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(conditionType);
            PressSaveOnViewFilterModal();
            ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = ExpectedModalMessage;
            ValidateMessageModal();
            PressCloseOnMessageModal();
        }
    }

    internal class CustomEntityViewsControls
    {
        internal HtmlEdit ViewNameTxt { get; set; }
        internal cHtmlTextAreaWrapper DescriptionTxt { get; set; }
        internal HtmlComboBox menulist { get; set; }
        internal cHtmlTextAreaWrapper menuDescriptionTxt { get; set; }
        internal HtmlComboBox addformlistlist { get; set; }
        internal HtmlComboBox editformlist { get; set; }
        internal HtmlCheckBox AllowDeleteOption { get; set; }
        internal HtmlCheckBox AllowApprovalOption { get; set; }
        internal HtmlComboBox SortColumnlist { get; set; }
        internal HtmlComboBox SortDirectionlist { get; set; }
        internal HtmlDiv columnsDropPane { get; set; }
        internal HtmlDiv columnsDragPane { get; set; }
        internal HtmlComboBox filterCriteriaOption { get; set; }
        internal HtmlEdit textFilterValue1Txt { get; set; }
        internal HtmlEdit textFilterValue2Txt { get; set; }
        internal HtmlEdit timeFilterValue1Txt { get; set; }
        internal HtmlEdit timeFilterValue2Txt { get; set; }
        internal HtmlComboBox cmbFilterOption { get; set; }
        internal HtmlDiv filtersDropPane { get; set; }
        internal HtmlDiv filtersDragPane { get; set; }
        internal HtmlEdit searchTxt { get; set; }

        protected CustomEntityViewsUIMap _cCustomEntityViewsMethods;
        protected ControlLocator<HtmlControl> _ControlLocator { get; private set; }

        internal CustomEntityViewsControls(CustomEntityViewsUIMap cCustomEntityViewsMethods, string tabName)
        {
            _cCustomEntityViewsMethods = cCustomEntityViewsMethods;
            _ControlLocator = new ControlLocator<HtmlControl>();
            FindControls(tabName);
        }

        /// <summary>
        /// Locates controls that are declared within this class
        /// </summary>
        internal void FindControls(string tabName)
        {
            if (tabName == EnumHelper.GetEnumDescription(TabName.GeneralDetails))
            {
                SharedMethodsUIMap _sharedMethods = new SharedMethodsUIMap();
                ViewNameTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewGeneralDetails_txtviewname", new HtmlEdit(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument1.UICtl00_contentmain_taPane));
                DescriptionTxt = (cHtmlTextAreaWrapper)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewGeneralDetails_txtviewdescription", new cHtmlTextAreaWrapper(_sharedMethods.ExtractHtmlMarkUpFromPage(), "ctl00_contentmain_tabConViews_tabViewGeneralDetails_txtviewdescription", _cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument1.UICtl00_contentmain_taPane));
                menulist = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewGeneralDetails_cmbmenu", new HtmlComboBox(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument1.UICtl00_contentmain_taPane));
                menuDescriptionTxt = (cHtmlTextAreaWrapper)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewGeneralDetails_txtViewMenuDescription", new cHtmlTextAreaWrapper(_sharedMethods.ExtractHtmlMarkUpFromPage(), "ctl00_contentmain_tabConViews_tabViewGeneralDetails_txtViewMenuDescription", _cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument1.UICtl00_contentmain_taPane));
                addformlistlist = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewGeneralDetails_cmbviewaddform", new HtmlComboBox(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument1.UICtl00_contentmain_taPane));
                editformlist = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewGeneralDetails_cmbvieweditform", new HtmlComboBox(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument1.UICtl00_contentmain_taPane));
                AllowDeleteOption = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewGeneralDetails_chkviewallowdelete", new HtmlCheckBox(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument1.UICtl00_contentmain_taPane));
                AllowApprovalOption = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewGeneralDetails_chkviewallowapproval", new HtmlCheckBox(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument1.UICtl00_contentmain_taPane));
            }
            else if (tabName == EnumHelper.GetEnumDescription(TabName.Sorting))
            {
                SortColumnlist = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewSort_ddlSortColumn", new HtmlComboBox(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument3.UICtl00_contentmain_taPane));
                SortDirectionlist = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewSort_ddlSortOrder", new HtmlComboBox(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument3.UICtl00_contentmain_taPane));
            }
            else if (tabName == EnumHelper.GetEnumDescription(TabName.Columns))
            {
                columnsDragPane = (HtmlDiv)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewFields_tcFields_tcTree", new HtmlDiv(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument));
                columnsDropPane = (HtmlDiv)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewFields_tcFields_tcDrop", new HtmlDiv(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument));
            }
            else if (tabName == EnumHelper.GetEnumDescription(TabName.Filters))
            {
                filtersDragPane = (HtmlDiv)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewFilters_tcFilters_tcTree", new HtmlDiv(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument));
                filtersDropPane = (HtmlDiv)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewFilters_tcFilters_tcDrop", new HtmlDiv(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument));
                filterCriteriaOption = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewFilters_tcFilters_ddlFilter", new HtmlComboBox(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow12.UIGreenLightCustomEntiDocument));
                textFilterValue1Txt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewFilters_tcFilters_txtFilterCriteria1", new HtmlEdit(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow12.UIGreenLightCustomEntiDocument));
                textFilterValue2Txt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewFilters_tcFilters_txtFilterCriteria2", new HtmlEdit(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow12.UIGreenLightCustomEntiDocument));
                timeFilterValue1Txt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewFilters_tcFilters_txtTimeCriteria1", new HtmlEdit(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow12.UIGreenLightCustomEntiDocument));
                timeFilterValue2Txt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewFilters_tcFilters_txtTimeCriteria2", new HtmlEdit(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow12.UIGreenLightCustomEntiDocument));
                cmbFilterOption = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewFilters_tcFilters_cmbFilterCriteria1", new HtmlComboBox(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow12.UIGreenLightCustomEntiDocument));
            }
            else if (tabName == EnumHelper.GetEnumDescription(TabName.Icon))
            {
                searchTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabConViews_tabViewIcon_txtViewCustomIconSearch", new HtmlEdit(_cCustomEntityViewsMethods.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument));
            }
        }
    }
}
