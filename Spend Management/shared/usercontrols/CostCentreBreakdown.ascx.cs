namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using AjaxControlToolkit;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementHelpers;
    using SpendManagementLibrary;

    using Syncfusion.Web.UI.HTML;

    /// <summary>
    ///     User control class for Cost Centre Breakdown panel
    /// </summary>
    public partial class CostCentreBreakdown : UserControl
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

        #region Fields

        /// <summary>
        ///     Are cost centres used - boolean passed through to javascript
        /// </summary>
        public bool costCentresEnabled;

        /// <summary>
        ///     Department select list id
        /// </summary>
        public string ddlDepartmentsId;

        /// <summary>
        ///     Cost Code select list id
        /// </summary>
        public string ddlCostCodesId;

        /// <summary>
        ///     Project Code select list id
        /// </summary>
        public string ddlProjectCodesId;

        /// <summary>
        ///     If cost centres are use in use, use departments?
        /// </summary>
        public bool departmentCodeEnabled;

        /// <summary>
        ///     If cost centres are use in use, use cost codes?
        /// </summary>
        public bool costCodeEnabled;

        /// <summary>
        ///     If cost centres are use in use, use project codes?
        /// </summary>
        public bool projectCodeEnabled;

        public HtmlBlockElement departmentsGrid = new HtmlBlockElement {ID = "TempDGrid"};
        public HtmlBlockElement costCodesGrid = new HtmlBlockElement {ID = "TempCCGrid"};
        public HtmlBlockElement projectCodesGrid = new HtmlBlockElement {ID = "TempPCGrid"};

        public ModalPopupExtender departmentsModal = new ModalPopupExtender {ID = "TempDModal"};
        public ModalPopupExtender costCodesModal = new ModalPopupExtender {ID = "TempCCModal"};
        public ModalPopupExtender projectCodesModal = new ModalPopupExtender {ID = "TempPCModal"};

        public Panel departmentsPanel = new Panel {ID = "TempDPanel"};
        public Panel costCodesPanel = new Panel {ID = "TempCCPanel"};
        public Panel projectCodesPanel = new Panel {ID = "TempPCPanel"};

        public bool useDepartmentDescription;
        public bool useCostCodeDescription;
        public bool useProjectCodeDescription;

        private Dictionary<string, string> departmentEntries = new Dictionary<string, string>();
        private Dictionary<string, string> costCodeEntries = new Dictionary<string, string>();
        private Dictionary<string, string> projectCodeEntries = new Dictionary<string, string>();

        private Dictionary<int, List<List<int>>> lstItems;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the for aeEmployee, allows [None] values
        /// </summary>
        public bool EmptyValuesEnabled { get; set; }

        /// <summary>
        /// Gets or sets the show form buttons or not
        /// </summary>
        public bool HideButtons { get; set; }

        /// <summary>
        /// Gets or sets the modal extender id that is used to pop up the display if the displayType is Modal
        /// </summary>
        public string ModalExtenderId { get; set; }

        /// <summary>
        /// Gets or sets whether or not the cost centre panel is read only
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the display type for the panel, inline or popup/modal
        /// </summary>
        public UserControlType? UserControlDisplayType { get; set; } = UserControlType.None;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds a cost centre breakdown to the page for an inline item row
        /// </summary>
        /// <param name="dep">Department Id</param>
        /// <param name="cc">Cost Code Id</param>
        /// <param name="pc">Project Code Id</param>
        /// <param name="percent">int between 1 and 100</param>
        public void AddCostCentreBreakdownRow(int? dep, int? cc, int? pc, int percent)
        {
            this.AddCostCentreBreakdownRow(1, dep, cc, pc, percent);
        }

        /// <summary>
        ///     Adds a cost centre breakdown to the page for an item row
        /// </summary>
        /// <param name="itemId">Must be a unique int (greater than 0) for the page/item</param>
        /// <param name="dep">Department Id</param>
        /// <param name="cc">Cost Code Id</param>
        /// <param name="pc">Project Code Id</param>
        /// <param name="percent">int between 1 and 100</param>
        public void AddCostCentreBreakdownRow(int itemId, int? dep, int? cc, int? pc, int percent)
        {
            if (this.lstItems == null)
            {
                this.lstItems = new Dictionary<int, List<List<int>>>();
            }

            if (itemId > 0 && this.lstItems.ContainsKey(itemId))
            {
                // build the Cost Centres mini-array with values or 0 if null
                List<int> lstCostCentres = new List<int>
                {
                    (dep.HasValue) ? dep.Value : 0,
                    (cc.HasValue) ? cc.Value : 0,
                    (pc.HasValue) ? pc.Value : 0,
                    (percent > 0 && percent <= 100) ? percent : 1
                };

                this.lstItems[itemId].Add(lstCostCentres);
            }
            else if (itemId > 0)
            {
                // build the Cost Centres mini-array with values or 0 if null
                List<int> lstCostCentres = new List<int>
                {
                    (dep.HasValue) ? dep.Value : 0,
                    (cc.HasValue) ? cc.Value : 0,
                    (pc.HasValue) ? pc.Value : 0,
                    (percent > 0 && percent <= 100) ? percent : 1
                };

                List<List<int>> lstCostCentreRow = new List<List<int>> {lstCostCentres};

                // add it to the public array under the item's id
                this.lstItems.Add(itemId, lstCostCentreRow);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cMisc clsMisc = new cMisc(currentUser.AccountID);

            var generalOptions = this.GeneralOptionsFactory[currentUser.CurrentSubAccountId].WithCodeAllocation();

            if (this.UserControlDisplayType == UserControlType.None)
            {
                throw new MissingFieldException("UserControlDisplayType is required to be set.");
            }

            if (this.IsPostBack)
            {
                return;
            }

            #region populate vars

            if (this.lstItems == null)
            {
                this.lstItems = new Dictionary<int, List<List<int>>>();
            }

            this.ddlDepartmentsId = "";
            this.ddlCostCodesId = "";
            this.ddlProjectCodesId = "";
            this.costCentresEnabled = false;
            this.departmentCodeEnabled = false;
            this.costCodeEnabled = false;
            this.projectCodeEnabled = false;
            this.useDepartmentDescription = generalOptions.CodeAllocation.UseDepartmentDescription;
            this.useCostCodeDescription = generalOptions.CodeAllocation.UseCostCodeDescription;
            this.useProjectCodeDescription = generalOptions.CodeAllocation.UseProjectCodeDesc;

            #endregion

            if ((generalOptions.CodeAllocation.DepartmentsOn && generalOptions.CodeAllocation.UseDepartmentCodes) ||
                (generalOptions.CodeAllocation.CostCodesOn && generalOptions.CodeAllocation.UseCostCodes) ||
                (generalOptions.CodeAllocation.ProjectCodesOn && generalOptions.CodeAllocation.UseProjectCodes))
            {
                #region create table and determine used codes and label names

                this.costCentresEnabled = true;

                #region table header

                StringBuilder javaScript = new StringBuilder();
                TableHeaderCell thDelete = new TableHeaderCell();
                Image deleteImg = new Image
                {
                    ImageUrl = "/shared/images/icons/delete2.png",
                    AlternateText = "Delete Row"
                };
                thDelete.Controls.Add(deleteImg);

                if (this.ReadOnly)
                {
                    deleteImg.Style.Add(HtmlTextWriterStyle.Display, "none");
                }

                this.trCostCentreBreakdown.Controls.Add(thDelete);

                cDepCostItem[] lstCostCodes = currentUser.Employee.GetCostBreakdown().ToArray();

                if (generalOptions.CodeAllocation.DepartmentsOn && generalOptions.CodeAllocation.UseDepartmentCodes)
                {
                    this.departmentCodeEnabled = true;

                    this.departmentEntries = lstCostCodes
                        .Where(x => x.departmentid > 0)
                        .DistinctBy(x => x.departmentid)
                        .ToDictionary(x => x.departmentid.ToString(CultureInfo.InvariantCulture), y => "");

                    foreach (List<List<int>> arr in this.lstItems.Values)
                    {
                        foreach (List<int> ar in arr)
                        {
                            if (ar[0] > 0 &&
                                !this.departmentEntries.ContainsKey(ar[0].ToString(CultureInfo.InvariantCulture)))
                            {
                                this.departmentEntries.Add(ar[0].ToString(CultureInfo.InvariantCulture), "");
                            }
                        }
                    }

                    javaScript.Append(this.PopulateControls(ref currentUser,
                        ref this.costCentreDepartmentsHolder,
                        ref this.ddlDepartmentsId,
                        ref this.departmentsGrid,
                        ref this.departmentsModal,
                        ref this.departmentsPanel,
                        ref this.departmentEntries,
                        clsMisc.GetGeneralFieldByCode("department").description, // heading
                        (this.useDepartmentDescription
                            ? "BC95890F-47D4-4FEC-A6AF-BBEEC4470497"
                            : "87D021DA-EAB8-40F7-8C70-CF5ADCE9486C"),
                        "A0F31CB0-16BB-4ACE-AAEA-69A7189D9599", // tableId - departments
                        (this.useDepartmentDescription
                            ? "990FD383-14F8-4F50-A2E2-13A9D1F847B7"
                            : "9617A83E-6621-4B73-B787-193110511C17"), // displayfieldid - description or department
                        new List<string>
                        {
                            "9617A83E-6621-4B73-B787-193110511C17",
                            "990FD383-14F8-4F50-A2E2-13A9D1F847B7"
                        }, // searchfieldids - department, description
                        new SortedList<int, FieldFilter>
                        {
                            {
                                0,
                                new FieldFilter(
                                    new cFields(currentUser.AccountID).GetFieldByID(
                                        new Guid("03BB1843-A231-4BE7-B564-1B813D6A5988")), ConditionType.Equals, "0",
                                    null, 0, null)
                            }
                        }));
                }

                if (generalOptions.CodeAllocation.CostCodesOn)
                {
                    this.costCodeEnabled = true;

                    this.costCodeEntries = lstCostCodes
                        .Where(x => x.costcodeid > 0)
                        .DistinctBy(x => x.costcodeid)
                        .ToDictionary(x => x.costcodeid.ToString(CultureInfo.InvariantCulture), y => "");

                    foreach (List<List<int>> arr in this.lstItems.Values)
                    {
                        foreach (List<int> ar in arr)
                        {
                            if (ar[1] > 0 &&
                                !this.costCodeEntries.ContainsKey(ar[1].ToString(CultureInfo.InvariantCulture)))
                            {
                                this.costCodeEntries.Add(ar[1].ToString(CultureInfo.InvariantCulture), "");
                            }
                        }
                    }

                    javaScript.Append(this.PopulateControls(ref currentUser,
                        ref this.costCentreCostCodesHolder,
                        ref this.ddlCostCodesId,
                        ref this.costCodesGrid,
                        ref this.costCodesModal,
                        ref this.costCodesPanel,
                        ref this.costCodeEntries,
                        clsMisc.GetGeneralFieldByCode("costcode").description, // heading
                        (this.useCostCodeDescription
                            ? "D3F54727-45FB-4D82-B400-C37BFD8E1E73"
                            : "E1F85384-D8E7-4BDC-BA27-339B59BEDB85"),
                        "02009E21-AA1D-4E0D-908A-4E9D73DDFBDF", // tableId - costcodes
                        (this.useCostCodeDescription
                            ? "AF80D035-6093-4721-8AFC-061424D2AB72"
                            : "359DFAC9-74E6-4BE5-949F-3FB224B1CBFC"), // displayfieldid - description or costcode
                        new List<string>
                        {
                            "359DFAC9-74E6-4BE5-949F-3FB224B1CBFC",
                            "AF80D035-6093-4721-8AFC-061424D2AB72"
                        }, // searchfieldids - costcode, description
                        new SortedList<int, FieldFilter>
                        {
                            {
                                0,
                                new FieldFilter(
                                    new cFields(currentUser.AccountID).GetFieldByID(
                                        new Guid("8178629C-5908-4458-89F6-D7EE7438314D")), ConditionType.Equals, "0",
                                    null, 0, null)
                            }
                        }));
                }

                if (generalOptions.CodeAllocation.ProjectCodesOn)
                {
                    this.projectCodeEnabled = true;


                    this.projectCodeEntries = lstCostCodes
                        .Where(x => x.projectcodeid > 0)
                        .DistinctBy(x => x.projectcodeid)
                        .ToDictionary(x => x.projectcodeid.ToString(CultureInfo.InvariantCulture), y => "");

                    foreach (List<List<int>> arr in this.lstItems.Values)
                    {
                        foreach (List<int> ar in arr)
                        {
                            if (ar[2] > 0 &&
                                !this.projectCodeEntries.ContainsKey(ar[2].ToString(CultureInfo.InvariantCulture)))
                            {
                                this.projectCodeEntries.Add(ar[2].ToString(CultureInfo.InvariantCulture), "");
                            }
                        }
                    }

                    javaScript.Append(this.PopulateControls(ref currentUser,
                        ref this.costCentreProjectCodesHolder,
                        ref this.ddlProjectCodesId,
                        ref this.projectCodesGrid,
                        ref this.projectCodesModal,
                        ref this.projectCodesPanel,
                        ref this.projectCodeEntries,
                        clsMisc.GetGeneralFieldByCode("projectcode").description, // heading
                        (this.useProjectCodeDescription
                            ? "F4FE7871-8043-4020-9150-B21BDE238F94"
                            : "C944F362-E745-4A31-B626-FB360DE9B908"),
                        "E1EF483C-7870-42CE-BE54-ECC5C1D5FB34", // tableId - project_codes
                        (this.useProjectCodeDescription
                            ? "0AD6004F-7DFD-4655-95FE-5C86FF5E4BE4"
                            : "6D06B15E-A157-4F56-9FF2-E488D7647219"), // displayfieldid - description or projectcode
                        new List<string>
                        {
                            "6D06B15E-A157-4F56-9FF2-E488D7647219",
                            "0AD6004F-7DFD-4655-95FE-5C86FF5E4BE4"
                        }, // searchfieldids - projectcode, description
                        new SortedList<int, FieldFilter>
                        {
                            {
                                0,
                                new FieldFilter(
                                    new cFields(currentUser.AccountID).GetFieldByID(
                                        new Guid("7B406750-ADBD-461F-9D36-97DBDBD8F451")), ConditionType.Equals, "0",
                                    null, 0, null)
                            }
                        }));
                }

                this.trCostCentreBreakdown.Cells.Add(new TableHeaderCell {Text = "&#037;"});

                #endregion table header

                #region table rows

                // done by js ccbAddCostCentreBreakdownRow(dep,cc,pc,perc)

                #endregion table rows

                #endregion create table and determine used codes and label names

                #region buttons

                if (this.HideButtons == false)
                {
                    if (this.ReadOnly == false)
                    {
                        Image btnSave = new Image
                        {
                            ImageUrl = "/shared/images/buttons/btn_save.png",
                            AlternateText = "Save",
                            ID = "btnSave"
                        };
                        btnSave.Attributes.Add("onclick", "ccbSave();");

                        Image btnCancel = new Image
                        {
                            ImageUrl = "/shared/images/buttons/cancel_up.gif",
                            AlternateText = "Cancel",
                            ID = "btnCancel"
                        };
                        btnCancel.Attributes.Add("onclick", "ccbClose();");

                        Literal litSpacer = new Literal {Text = "&nbsp;"};

                        this.pnlButtons.Controls.Add(btnSave);
                        this.pnlButtons.Controls.Add(litSpacer);
                        this.pnlButtons.Controls.Add(btnCancel);
                    }
                    else
                    {
                        Image btnClose = new Image
                        {
                            ImageUrl = "/shared/images/buttons/btn_close.png",
                            AlternateText = "Close"
                        };
                        btnClose.Attributes.Add("onclick", "ccbClose();");

                        this.pnlButtons.Controls.Add(btnClose);
                    }
                }
                else
                {
                    this.pnlButtons.Visible = false;
                }

                #endregion buttons

                #region get users default breakdown

                StringBuilder codesJs = new StringBuilder();
                codesJs.Append("var ccbAutoCompleteEntries = { departments: {}, costcodes: {}, projectcodes: {} };\n");

                foreach (KeyValuePair<string, string> kvp in this.departmentEntries.Where(x =>
                    !string.IsNullOrWhiteSpace(x.Value)))
                {
                    codesJs.Append(string.Format("ccbAutoCompleteEntries.departments[\"{0}\"] = \"{1}\";\n", kvp.Key,
                        kvp.Value));
                }

                foreach (KeyValuePair<string, string> kvp in this.costCodeEntries.Where(x =>
                    !string.IsNullOrWhiteSpace(x.Value)))
                {
                    codesJs.Append(string.Format("ccbAutoCompleteEntries.costcodes[\"{0}\"] = \"{1}\";\n", kvp.Key,
                        kvp.Value));
                }

                foreach (KeyValuePair<string, string> kvp in this.projectCodeEntries.Where(x =>
                    !string.IsNullOrWhiteSpace(x.Value)))
                {
                    codesJs.Append(string.Format("ccbAutoCompleteEntries.projectcodes[\"{0}\"] = \"{1}\";\n", kvp.Key,
                        kvp.Value));
                }

                codesJs.Append("var ccbEmployeeDefaults = [];\n");

                for (int i = 0; i < lstCostCodes.Length; i++)
                {
                    cDepCostItem tempCostItem = lstCostCodes[i];
                    // new array item of array(dep,cc,pc,perc)
                    codesJs.Append(string.Format("ccbEmployeeDefaults[{0}] = [{1},{2},{3},{4}];\n", i,
                        tempCostItem.departmentid, tempCostItem.costcodeid, tempCostItem.projectcodeid,
                        tempCostItem.percentused));
                }

                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "EmployeeDefaults", codesJs.ToString(),
                    true);

                #endregion get users default breakdown

                #region transfer current item array to js

                StringBuilder arrayJs = new StringBuilder();
                arrayJs.Append("var ccbItemArray = [];\n");

                foreach (KeyValuePair<int, List<List<int>>> kvp in this.lstItems)
                {
                    // new array item of array(dep,cc,pc,perc)
                    arrayJs.Append("ccbItemArray[" + kvp.Key + "] = [];\n");
                    for (int i = 0; i < kvp.Value.Count; i++)
                    {
                        arrayJs.Append(string.Format("ccbItemArray[{0}][{1}] = [{2},{3},{4},{5}];\n", kvp.Key, i,
                            kvp.Value[i][0], kvp.Value[i][1], kvp.Value[i][2], kvp.Value[i][3]));
                    }
                }

                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ItemArray", arrayJs.ToString(), true);

                #endregion transfer current item array to js

                #region run the inline javascript populator

                if (this.UserControlDisplayType == UserControlType.Inline)
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "PopulateInlineCostCentres",
                        "ccbShowCostCentreBreakdown(1);", true);
                }

                #endregion run the inline javascript populator
            }
            else
            {
                #region transfer empty item array to js

                StringBuilder arrayJs = new StringBuilder();
                arrayJs.Append("var ccbItemArray = [];\n");
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ItemArray", arrayJs.ToString(), true);

                #endregion transfer empty item array to js

                this.pnlCostCentreBreakdown.Style.Add(HtmlTextWriterStyle.Display, "none");
            }

            this.costCentreDepartmentsHolder.DataBind();
            this.costCentreCostCodesHolder.DataBind();
            this.costCentreProjectCodesHolder.DataBind();
        }

        /// <summary>
        ///     Set up the controls used for a cost centre column
        /// </summary>
        /// <param name="selectContainer">The panel reference that the select control will be added to</param>
        /// <param name="controlId">The id to use on the select control</param>
        /// <param name="headerText">The label for this column header</param>
        /// <param name="selectOptions">The list of listitems to populate the select list with</param>
        /// <returns>JavaScript for autocomplete controls and search grid</returns>
        private StringBuilder PopulateControls(ref CurrentUser currentUser, ref Panel controlHolder,
            ref string controlClientId, ref HtmlBlockElement grid, ref ModalPopupExtender modal, ref Panel panel,
            ref Dictionary<string, string> entries, string headerText, string gridType, string tableId,
            string displayFieldId, List<string> searchFieldIds, SortedList<int, FieldFilter> filters = null)
        {
            if (!controlHolder.ID.EndsWith("Holder"))
            {
                throw new ArgumentException(
                    "The AutoComplete Combo container must have an ID that ends with \"Holder\".", "controlHolder");
            }

            string controlId = controlHolder.ID.Substring(0, controlHolder.ID.Length - 6);

            TableHeaderCell tableHeader = new TableHeaderCell();
            this.trCostCentreBreakdown.Cells.Add(tableHeader);
            tableHeader.Text = headerText;

            StringBuilder javaScript = new StringBuilder();
            controlHolder.CssClass = "autocompletecombo-container";
            controlHolder.Style.Add(HtmlTextWriterStyle.WhiteSpace, "no-wrap");
            controlHolder.Attributes.Add("data-type", gridType);
            DropDownList select = new DropDownList {ID = controlId + "Select", CssClass = "autocompletecombo-select"};
            select.Attributes.Add("onchange", "SEL.AutoCompleteCombo.SelectChange(this);");
            TextBox autoComplete = new TextBox {ID = controlId, CssClass = "autocompletecombo-text"};
            TextBox autoCompleteId = new TextBox {ID = controlId + "_ID", CssClass = "autocompletecombo-id"};
            autoCompleteId.Style.Add(HtmlTextWriterStyle.Display, "none");
            Image searchIcon = new Image
            {
                ID = controlId + "SearchIcon",
                ImageUrl = GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/find.png",
                CssClass = "btn autocompletecombo-icon"
            };
            searchIcon.Attributes.Add("onclick", "SEL.AutoCompleteCombo.Search(this);");

            Dictionary<string, JSFieldFilter> filterDictionary = filters == null
                ? null
                : filters.ToDictionary(x => x.Key.ToString(CultureInfo.InvariantCulture),
                    x => new JSFieldFilter
                    {
                        ConditionType = x.Value.Conditiontype,
                        FieldID = x.Value.Field.FieldID,
                        Order = x.Value.Order,
                        ValueOne = x.Value.ValueOne.ToString(CultureInfo.InvariantCulture)
                    });

            List<sAutoCompleteResult> selectOptions = AutoComplete.GetAutoCompleteMatches(currentUser, 0, tableId,
                displayFieldId, string.Join(",", searchFieldIds), "", true, filterDictionary);

            if (selectOptions.Count <= 25)
            {
                if (this.EmptyValuesEnabled)
                {
                    ListItem none = new ListItem("[None]", "0");
                    select.Items.Add(none);
                }

                select.Items.AddRange(selectOptions.Select(x => new ListItem(x.label, x.value)).ToArray());
                autoComplete.Style.Add(HtmlTextWriterStyle.Display, "none");
                searchIcon.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
            else
            {
                select.Style.Add(HtmlTextWriterStyle.Display, "none");
                List<string> ints = entries.Keys.ToList();
                Dictionary<string, string> so = selectOptions.Where(x => ints.Contains(x.value))
                    .ToDictionary(x => x.value, y => y.label);

                foreach (KeyValuePair<string, string> kvp in so)
                {
                    entries[kvp.Key] = kvp.Value;
                }

                autoComplete.Attributes.Add("data-jsbind",
                    AutoComplete.createAutoCompleteBindString("##CONTROLID##", 15, new Guid(tableId),
                        new Guid(displayFieldId), searchFieldIds.Select(x => new Guid(x)).ToList(),
                        fieldFilters: filters));
            }

            controlHolder.Controls.Add(select);
            controlHolder.Controls.Add(autoComplete);
            controlHolder.Controls.Add(autoCompleteId);
            controlHolder.Controls.Add(searchIcon);

            if (this.ReadOnly)
            {
                select.Enabled = false;
                autoComplete.Enabled = false;
                searchIcon.Style.Add(HtmlTextWriterStyle.Display, "none");
            }

            // new modal, panel, link, grid

            LinkButton dummySearchLinkButton = new LinkButton {ID = controlId + "SearchLink"};
            dummySearchLinkButton.Style.Add(HtmlTextWriterStyle.Display, "none");
            CSSButton searchCancel =
                new CSSButton {ID = controlId + "SearchCancel", Text = "cancel", CausesValidation = false};
            HtmlBlockElement searchButtons = new HtmlBlockElement {ClassName = "formpanelbuttons"};
            searchButtons.Controls.Add(searchCancel);
            panel = new Panel
            {
                ID = controlId + "SearchPanel",
                CssClass = "modalpanel formpanel autocompletecombo-search"
            };
            panel.Style.Add(HtmlTextWriterStyle.Display, "none");
            panel.Controls.Add(new HtmlBlockElement {ClassName = "sectiontitle", InnerText = headerText + " Search"});
            grid = new HtmlBlockElement {ID = controlId + "SearchGrid", ClassName = "autocompletecombo-grid"};
            panel.Controls.Add(grid);
            panel.Controls.Add(searchButtons);
            modal = new ModalPopupExtender
            {
                ID = controlId + "SearchModal",
                TargetControlID = dummySearchLinkButton.ID,
                PopupControlID = panel.ID,
                BackgroundCssClass = "modalBackground",
                CancelControlID = searchCancel.ID
            };

            this.comboModals.Controls.Add(panel);
            this.comboModals.Controls.Add(modal);
            this.comboModals.Controls.Add(dummySearchLinkButton);

            controlClientId = controlHolder.ClientID;

            return javaScript;
        }

        #endregion
    }

    /// <summary>
    ///     Set what type of context this user control is being used in
    /// </summary>
    public enum UserControlType
    {
        /// <summary>
        ///     Default "null" value
        /// </summary>
        None,

        /// <summary>
        ///     Inline version - visable in the onload page flow
        /// </summary>
        Inline,

        /// <summary>
        ///     Used in a modal popup window
        /// </summary>
        Modal
    }

    /// <summary>
    ///     Object to hold cost centre breakdown data in a more useable form
    /// </summary>
    [Serializable]
    public class cCcbItemArray
    {
        #region Fields

        private readonly cCcbItem tempItem;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Empty serialisation constructor
        /// </summary>
        public cCcbItemArray()
        {
        }

        /// <summary>
        ///     Creates an object from the javascript
        /// </summary>
        /// <param name="ccbItemArrayData"></param>
        public cCcbItemArray(IList<object> ccbItemArrayData)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            cDepartments clsDepartments = new cDepartments(currentUser.AccountID);
            cCostcodes clsCostCodes = new cCostcodes(currentUser.AccountID);
            cProjectCodes clsProjectCodes = new cProjectCodes(currentUser.AccountID);

            for (int i = 0; i < (ccbItemArrayData).Count(); i++)
            {
                int itemID = i;
                object[] ccbRows = (object[]) ccbItemArrayData[i];

                if (ccbRows == null)
                {
                    continue;
                }

                for (int j = 0; j < ccbRows.Count(); j++)
                {
                    cDepartment clsDepartment;
                    if (((object[]) ccbRows[j])[0] != null && Convert.ToString(((object[]) ccbRows[j])[0]) != "" &&
                        Convert.ToInt32(((object[]) ccbRows[j])[0]) != 0)
                    {
                        int departmentID = Convert.ToInt32(((object[]) ccbRows[j])[0]);
                        clsDepartment = clsDepartments.GetDepartmentById(departmentID);
                        clsDepartment.UserdefinedFields = null;
                    }
                    else
                    {
                        clsDepartment = null;
                    }

                    cCostCode clsCostCode;
                    if (((object[]) ccbRows[j])[1] != null && Convert.ToString(((object[]) ccbRows[j])[1]) != "" &&
                        Convert.ToInt32(((object[]) ccbRows[j])[1]) != 0)
                    {
                        int costCodeID = Convert.ToInt32(((object[]) ccbRows[j])[1]);
                        clsCostCode = clsCostCodes.GetCostcodeById(costCodeID);
                        clsCostCode.UserdefinedFields = null;
                    }

                    else
                    {
                        clsCostCode = null;
                    }

                    cProjectCode clsProjectCode;
                    if (((object[]) ccbRows[j])[2] != null && Convert.ToString(((object[]) ccbRows[j])[2]) != "" &&
                        Convert.ToInt32(((object[]) ccbRows[j])[2]) != 0)
                    {
                        int projectCodeID = Convert.ToInt32(((object[]) ccbRows[j])[2]);
                        clsProjectCode = clsProjectCodes.getProjectCodeById(projectCodeID);
                    }
                    else
                    {
                        clsProjectCode = null;
                    }

                    int nPercentSplit;
                    if (((object[]) ccbRows[j])[3] != null && Convert.ToString(((object[]) ccbRows[j])[3]) != "" &&
                        Convert.ToInt32(((object[]) ccbRows[j])[3]) != 0)
                    {
                        nPercentSplit = Convert.ToInt32(((object[]) ccbRows[j])[3]);
                    }
                    else
                    {
                        nPercentSplit = 100;
                    }

                    this.tempItem = new cCcbItem(itemID, clsDepartment, clsCostCode, clsProjectCode, nPercentSplit);
                    this.itemArray.Add(this.tempItem);
                }
            }
        }

        #endregion

        #region Public Properties

        public List<cCcbItem> itemArray
        {
            get;
            //set { tempItemArray = value; }
        } = new List<cCcbItem>();

        #endregion
    }

    [Serializable]
    public class cCcbItem
    {
        #region Fields

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Create javascriptable object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dep"></param>
        /// <param name="cc"></param>
        /// <param name="pc"></param>
        /// <param name="perc"></param>
        public cCcbItem(int id, cDepartment dep, cCostCode cc, cProjectCode pc, int perc)
        {
            this.relatedItemID = id;
            this.departmentID = dep;
            this.costCodeID = cc;
            this.projectCodeID = pc;
            this.percentageSplit = perc;
        }

        /// <summary>
        ///     Empty serialisation constructor
        /// </summary>
        public cCcbItem()
        {
        }

        #endregion

        #region Public Properties

        public cCostCode costCodeID
        {
            get;
            //set { clsCC = value; }
        }

        public cDepartment departmentID
        {
            get;
            //set { clsDep = value; }
        }

        public int percentageSplit
        {
            get;
            //set { nPerc = value; }
        }

        public cProjectCode projectCodeID
        {
            get;
            //set { clsPC = value; }
        }

        public int relatedItemID
        {
            get;
            //set { nItemId = value; }
        }

        #endregion
    }
}
